using LightSync.API.Lights;
using LightSync.HomeAssistant.Commands;
using System.Drawing;

namespace LightSync.HomeAssistant
{
    internal class HomeAssistantLightInfo : LightInfo
    {
        private readonly ConnectionManager _connectionManager;

        public HomeAssistantLightInfo(ConnectionManager connectionManager, string name, string identifier, bool isOn = false, Color color = default) : base(name, identifier)
        {
            _connectionManager = connectionManager;

            IsOn = isOn;
            Colour = color;
        }

        public override async Task SetOnAsync(bool on, TimeSpan transitionTime = default, CancellationToken cancellationToken = default)
        {
            await _connectionManager.AddCommandAsync(new SetLightStateCommand(on, Identifier, transitionTime));
        }

        public override async Task SetColourAsync(Color colour, TimeSpan transitionTime = default, CancellationToken cancellationToken = default)
        {
            await _connectionManager.AddCommandAsync(new SetLightStateCommand(colour, Identifier, transitionTime));
        }
    }
}
