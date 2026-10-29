namespace LightSync.Core.Captures
{
    public interface ICaptureFeed
    {
        bool HasData { get; }

        int Framerate { get; }
        int Width { get; }
        int Height { get; }
        int DataLength { get; }

        bool CopyLatestData(byte[] destination);
        bool TryCopyLatestData(byte[] destination, DateTimeOffset lastCopy);

        void UpdateData(byte[] data, int width, int height);
    }
}
