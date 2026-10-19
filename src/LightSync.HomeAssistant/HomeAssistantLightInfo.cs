using LightSync.API.Lights;
using LightSync.HomeAssistant.Commands;
using System.Drawing;

namespace LightSync.HomeAssistant
{
    internal struct HomeAssistantLightInfo : ILightInfo
    {
        private readonly ConnectionManager _connectionManager;

        public bool IsOn { get; }

        public string Name { get; }

        public string Identifier { get; }

        public Color Colour { get; private set; }

        public event EventHandler<OnChangedEventArgs>? OnChanged;
        public event EventHandler<NameChangedEventArgs>? NameChanged;
        public event EventHandler<ColourChangedEventArgs>? ColourChanged;

        public HomeAssistantLightInfo(ConnectionManager connectionManager, string name, string identifier, bool isOn = false, Color color = default)
        {
            _connectionManager = connectionManager;

            Name = name;
            Identifier = identifier;
            IsOn = isOn;
            Colour = color;
        }

        public async readonly Task SetOnAsync(bool on, CancellationToken cancellationToken = default)
        {
            await _connectionManager.AddCommandAsync(new SetLightStateCommand(on, Identifier));
        }

        public async readonly Task SetColourAsync(Color colour, CancellationToken cancellationToken = default)
        {
            await _connectionManager.AddCommandAsync(new SetLightStateCommand(colour, Identifier));
        }
    }
}
