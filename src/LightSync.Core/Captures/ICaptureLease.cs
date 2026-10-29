namespace LightSync.Core.Captures
{
    public interface ICaptureLease : IDisposable
    {
        ICaptureFeed CaptureFeed { get; }
    }
}
