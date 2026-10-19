namespace LightSync.UI.Extensions
{
    internal static class ColourExtensions
    {
        public static Avalonia.Media.Color ToAvalonia(this System.Drawing.Color colour) => new(colour.A, colour.R, colour.G, colour.B);
        public static System.Drawing.Color ToSystem(this Avalonia.Media.Color colour) => System.Drawing.Color.FromArgb(colour.A, colour.R, colour.G, colour.B);
    }
}
