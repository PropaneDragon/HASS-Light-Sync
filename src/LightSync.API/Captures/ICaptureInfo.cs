namespace LightSync.API.Captures
{

    public interface ICaptureInfo
    {
        string Name { get; }
        string Identifier { get; }

        Task<ICaptureSession> OpenAsync(int maxWidth, int maxHeight, CancellationToken cancellationToken = default);
    }
}
