using LightSync.API.Lights;
using LightSync.HomeAssistant.Commands;
using System.Collections.Immutable;
using System.Drawing;
using System.Text.Json;

namespace LightSync.HomeAssistant
{
    internal class HomeAssistantLightInfoSource : ILightInfoSource
    {
        private readonly ConnectionManager _connectionManager;

        public HomeAssistantLightInfoSource(ConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task InitialiseAsync(CancellationToken cancellationToken = default)
        {
        }

        public async Task<List<ILightInfo>> GetAvailableLightsAsync(CancellationToken cancellationToken = default)
        {
            var statesCommand = new GetStatesHomeAssistantCommand();
            var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            await _connectionManager.AddCommandAsync(statesCommand, cancellationToken);

            while (!timeout.IsCancellationRequested && !statesCommand.StatesReceived)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500), cancellationToken);
            }

            if (timeout.IsCancellationRequested)
                return [];

            return [.. statesCommand.StateElements
                .Where(IsValidLightElement)
                .Select(ConvertToLightInfo)
                .Where(lightInfo => lightInfo != null)
                .Select(lightInfo => lightInfo!)];
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }

        private bool IsValidLightElement(JsonElement element)
        {
            if (!element.TryGetProperty("entity_id", out var entityIdElement))
                return false;

            var entityId = entityIdElement.GetString();

            if (string.IsNullOrWhiteSpace(entityId))
                return false;

            return entityId.StartsWith("light.");
        }

        private ILightInfo? ConvertToLightInfo(JsonElement element)
        {
            if (!element.TryGetProperty("entity_id", out var entityIdElement) || !element.TryGetProperty("attributes", out var attributesElement))
                return null;

            var entityId = entityIdElement.GetString();

            if (string.IsNullOrWhiteSpace(entityId))
                return null;

            if (!attributesElement.TryGetProperty("friendly_name", out var friendlyNameElement) || !attributesElement.TryGetProperty("rgb_color", out var rgbColourElement))
                return null;

            var friendlyName = friendlyNameElement.GetString();

            if (string.IsNullOrWhiteSpace(friendlyName))
                return null;

            if (rgbColourElement.ValueKind != JsonValueKind.Array)
                return null;

            var rgbArray = rgbColourElement.EnumerateArray().ToImmutableArray();

            var r = rgbArray[0].GetInt32();
            var g = rgbArray[1].GetInt32();
            var b = rgbArray[2].GetInt32();
            var colour = Color.FromArgb(r, g, b);

            return new HomeAssistantLightInfo(_connectionManager, friendlyName, entityId, colour);
        }
    }
}
