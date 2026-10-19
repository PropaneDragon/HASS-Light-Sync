using LightSync.HomeAssistant.Commands;
using System.Collections.Concurrent;

namespace LightSync.HomeAssistant
{
    internal class ConnectionManager : IConnectionManager, IAsyncDisposable
    {
        private readonly ConcurrentDictionary<uint, IHomeAssistantCommand> _commandQueue = [];

        private ConnectionClient? _connectionClient = null;
        private CancellationTokenSource? _loopCancellationToken;

        public bool IsConnected => _connectionClient?.State == ConnectionClient.ConnectionState.Connected;

        public ConnectionManager()
        {
        }

        public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var address = new Uri("");
            var accessToken = "";

            if (_loopCancellationToken != null)
                await _loopCancellationToken.CancelAsync();

            if (_connectionClient != null)
                await _connectionClient.DisposeAsync();

            try
            {
                _connectionClient = new ConnectionClient(address, accessToken);
                await _connectionClient.TryConnectAsync(cancellationToken);
            }
            catch
            {
                return false;
            }

            _loopCancellationToken = new();

            _ = RunCommandLoop(_loopCancellationToken.Token);

            return true;
        }

        public async Task AddCommandAsync(IHomeAssistantCommand command, CancellationToken cancellationToken = default)
        {
            if (_connectionClient != null)
            {
                var id = _connectionClient.GetNextCommandId();
                var request = await Task.Run(() => command.GetMessage(id), cancellationToken);

                if (command.CommandType != HomeAssistantCommandType.NoResposne)
                _commandQueue[id] = command;

                await _connectionClient.SendMessageAsync(request, cancellationToken);
            }
        }

        public bool RemoveCommand(IHomeAssistantCommand command)
        {
            var foundPair = _commandQueue.FirstOrDefault(kv => kv.Value == command);
            return _commandQueue.TryRemove(foundPair);
        }

        public bool RemoveCommand(uint commandId)
        {
            return _commandQueue.TryRemove(commandId, out _);
        }

        public async ValueTask DisposeAsync()
        {
            if (_loopCancellationToken != null)
            {
                await _loopCancellationToken.CancelAsync();
                _loopCancellationToken.Dispose();
            }

            if (_connectionClient != null)
            {
                await _connectionClient.DisposeAsync();
            }
        }

        private async Task RunCommandLoop(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested && _connectionClient != null && _connectionClient.State == ConnectionClient.ConnectionState.Connected)
            {
                var messageElement = await _connectionClient.WaitForMessageAsync(cancellationToken);
                if (!messageElement.TryGetProperty("id", out var messageIdProperty))
                    continue;

                var id = messageIdProperty.GetUInt32();
                if (!_commandQueue.ContainsKey(id))
                    continue;

                var command = _commandQueue[id];
                _ = Task.Run(() => command.MessageReceived(messageElement), cancellationToken);

                if (command.CommandType == HomeAssistantCommandType.OneShot)
                    _commandQueue.TryRemove(id, out _);
            }
        }
    }
}
