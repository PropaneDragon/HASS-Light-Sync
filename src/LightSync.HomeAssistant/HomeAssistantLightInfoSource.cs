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
            if (!_connectionManager.IsConnected)
                await _connectionManager.ConnectAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ILightInfo>> GetAvailableLightsAsync(CancellationToken cancellationToken = default)
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

        public async ValueTask DisposeAsync()
        {
            await _connectionManager.DisposeAsync();
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
            if (!element.TryGetProperty("entity_id", out var entityIdElement) 
                || !element.TryGetProperty("state", out var stateElement)
                || !element.TryGetProperty("attributes", out var attributesElement))
                return null;

            var entityId = entityIdElement.GetString();
            var state = stateElement.GetString();

            if (string.IsNullOrWhiteSpace(entityId))
                return null;

            if (!attributesElement.TryGetProperty("friendly_name", out var friendlyNameElement) || !attributesElement.TryGetProperty("rgb_color", out var rgbColourElement))
                return null;

            var friendlyName = friendlyNameElement.GetString();

            if (string.IsNullOrWhiteSpace(friendlyName))
                return null;


            if (!attributesElement.TryGetProperty("supported_color_modes", out var supportedColourModes))
                return null;

            if (supportedColourModes.ValueKind != JsonValueKind.Array)
                return null;

            if (!supportedColourModes.EnumerateArray().ToImmutableArray().Any(colourModeElement => colourModeElement.GetString() == "xy" || colourModeElement.GetString() == "hs"))
                return null;

            int brightness = attributesElement.TryGetProperty("brightness", out var brightnessElement) && brightnessElement.ValueKind == JsonValueKind.Number ? brightnessElement.GetInt32() : 0;
            var colour = Color.FromArgb(brightness, Color.Black);

            if (rgbColourElement.ValueKind == JsonValueKind.Array)
            {
                var rgbArray = rgbColourElement.EnumerateArray().ToImmutableArray();

                var r = rgbArray[0].GetInt32();
                var g = rgbArray[1].GetInt32();
                var b = rgbArray[2].GetInt32();

                colour = Color.FromArgb(brightness, r, g, b);
            }

            return new HomeAssistantLightInfo(_connectionManager, friendlyName, entityId, state == "on", colour);
        }
    }
}