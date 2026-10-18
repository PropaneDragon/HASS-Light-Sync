using System.Drawing;

namespace LightSync.Abstractions.Lights
{
    public interface ILightInfo
    {
        string Name { get; }
        string Identifier { get; }

        Color Colour { get; }

        Task SetColourAsync(Color colour);
    }
}
