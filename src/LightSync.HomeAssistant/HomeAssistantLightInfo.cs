using LightSync.API.Lights;
using System.Drawing;

namespace LightSync.HomeAssistant
{
    internal struct HomeAssistantLightInfo : ILightInfo
    {
        public string Name { get; }

        public string Identifier { get; }

        public Color Colour { get; private set; }

        public ConnectionManager ConnectionManager { get; }

        public HomeAssistantLightInfo(ConnectionManager connectionManager, string name, string identifier, Color color = default)
        {
            ConnectionManager = connectionManager;
            Name = name;
            Identifier = identifier;
            Colour = color;
        }

        public Task SetColourAsync(Color colour)
        {
            throw new NotImplementedException();
        }
    }
}
