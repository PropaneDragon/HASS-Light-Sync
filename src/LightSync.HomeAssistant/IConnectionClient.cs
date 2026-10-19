using System.Text.Json;
using System.Text.Json.Nodes;

namespace LightSync.HomeAssistant
{
    public interface IConnectionClient
    {
        ConnectionClient.ConnectionState State { get; }

        uint GetNextCommandId();
        Task SendMessageAsync(JsonNode node, CancellationToken cancellationToken = default);
        Task TryConnectAsync(CancellationToken cancellationToken = default);
        Task<JsonElement> WaitForMessageAsync(CancellationToken cancellationToken = default);
    }
}