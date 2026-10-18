using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LightSync.HomeAssistant
{
    public class ConnectionClient : IAsyncDisposable
    {
        private readonly string _accessToken;
        private readonly Uri _socketUri;
        private readonly SemaphoreSlim _writeSemaphore = new(1, 1);

        private uint _commandId = 0;
        private ClientWebSocket? _webSocket;

        public ConnectionState State { get; private set; } = ConnectionState.Disconnected;

        public enum ConnectionState
        {
            Disconnected,
            Authenticating,
            Connected
        }

        public ConnectionClient(Uri homeAssistantAddress, string accessToken)
        {
            var authority = homeAssistantAddress.Authority;
            var scheme = GetWebSocketScheme(homeAssistantAddress);

            _socketUri = new($"{scheme}://{authority}/api/websocket");
            _accessToken = accessToken;
        }

        public async Task TryConnectAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (_webSocket?.State == WebSocketState.Open)
                throw new InvalidOperationException("Attempted to connect to a socket which is already open!");

            if (_webSocket != null)
            {
                if (_webSocket.State == WebSocketState.Open)
                {
                    var closeToken = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Re-opening socket", CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, closeToken.Token).Token).ConfigureAwait(false);
                }

                _webSocket.Dispose();
            }

            State = ConnectionState.Disconnected;

            _webSocket = new();

            await _webSocket.ConnectAsync(_socketUri, cancellationToken).ConfigureAwait(false);
            await PerformHandshakeAsync(cancellationToken);
        }

        public async Task SendMessageAsync(JsonNode node, CancellationToken cancellationToken = default)
        {
            if (_webSocket == null)
                throw new InvalidOperationException("The web socket cannot be written to as it is null.");

            await _writeSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                await using var stream = WebSocketStream.CreateWritableMessageStream(_webSocket, WebSocketMessageType.Text);
                await using var writer = new Utf8JsonWriter(stream);

                node.WriteTo(writer);
                await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _writeSemaphore.Release();
            }
        }

        public uint GetNextCommandId()
        {
            return Interlocked.Increment(ref _commandId);
        }

        public async Task<JsonElement> WaitForMessageAsync(CancellationToken cancellationToken = default)
        {
            if (_webSocket == null)
                throw new InvalidOperationException("The web socket cannot be read from as it is null.");

            using var stream = WebSocketStream.CreateReadableMessageStream(_webSocket);
            using var jsonDocument = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

            return jsonDocument.RootElement.Clone();
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);

            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            if (_webSocket != null)
            {
                if (_webSocket.State == WebSocketState.Open)
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection ending", cancellationToken.Token).ConfigureAwait(false);
                _webSocket.Dispose();
            }
        }

        private async Task PerformHandshakeAsync(CancellationToken cancellationToken = default)
        {
            State = ConnectionState.Authenticating;

            var handshakeCancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var combinedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, handshakeCancellationToken.Token);

            var authMessage = await WaitForMessageAsync(combinedCancellationToken.Token).ConfigureAwait(false);

            if (!authMessage.TryGetProperty("type", out var authTypeElement) || authTypeElement.GetString() != "auth_required")
                throw new InvalidDataException("Handshake could not be performed as the auth message was malformed!");

            var authObject = new JsonObject()
            {
                ["type"] = "auth",
                ["access_token"] = _accessToken
            };

            await SendMessageAsync(authObject, combinedCancellationToken.Token).ConfigureAwait(false);

            var confirmationMessage = await WaitForMessageAsync(combinedCancellationToken.Token).ConfigureAwait(false);
            if (!confirmationMessage.TryGetProperty("type", out var confirmationTypeElement))
                throw new InvalidDataException("Response from handshake was malformed!");

            if (confirmationTypeElement.GetString() == "auth_invalid")
                throw new Exception("Access token invalid."); //Todo - Make a proper exception.

            if (confirmationTypeElement.GetString() != "auth_ok")
                throw new InvalidDataException("Confirmation message from Home Assistant was malformed!");

            State = ConnectionState.Connected;
        }

        private static string GetWebSocketScheme(Uri homeAssistantAddress) => homeAssistantAddress.Scheme switch
        {
            "https" => "wss",
            _ => "ws"
        };
    }
}
