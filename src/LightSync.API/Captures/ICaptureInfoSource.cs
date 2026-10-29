namespace LightSync.API.Captures
{
    public interface ICaptureInfoSource : IAsyncDisposable
    {
        IReadOnlyList<ICaptureInfo> Captures { get; }

        Task InitialiseAsync(CancellationToken cancellationToken = default);
        Task RefreshAsync(CancellationToken cancellationToken = default);
    }
}
