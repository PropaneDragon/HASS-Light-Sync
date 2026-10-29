namespace LightSync.API.Captures
{
    public enum CaptureFormat { Bgra32, Rgba32 };
    public enum CaptureStatus { Ok, NoNewFrame, Ended }

    public interface ICaptureSession : IAsyncDisposable
    {
        int Width { get; }
        int Height { get; }

        CaptureFormat Format { get; }

        Task<CaptureStatus> TryGetLatestFrameAsync(byte[] pixels, CancellationToken cancellationToken = default);
    }
}
