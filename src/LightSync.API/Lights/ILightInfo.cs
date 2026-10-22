using System.Drawing;

namespace LightSync.API.Lights
{
    public interface ILightInfo
    {
        bool IsOn { get; }

        string Name { get; }
        string Identifier { get; }

        Color Colour { get; }

        Task SetOnAsync(bool on, CancellationToken cancellationToken = default);
        Task SetOnAsync(bool on, TimeSpan transitionTime, CancellationToken cancellationToken = default);
        Task SetColourAsync(Color colour, CancellationToken cancellationToken = default);
        Task SetColourAsync(Color colour, TimeSpan transitionTime, CancellationToken cancellationToken = default);

        event EventHandler<OnChangedEventArgs> OnChanged;
        event EventHandler<NameChangedEventArgs> NameChanged;
        event EventHandler<ColourChangedEventArgs> ColourChanged;
    }

    public sealed class OnChangedEventArgs(bool on) : EventArgs
    {
        public bool IsOn { get; } = on;
    }

    public sealed class NameChangedEventArgs(string name) : EventArgs
    {
        public string Name { get; } = name;
    }

    public sealed class ColourChangedEventArgs(Color colour) : EventArgs
    {
        public Color Colour { get; } = colour;
    }
}
