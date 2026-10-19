namespace LightSync.API.Lights
{
    public interface ILightInfoSource : IAsyncDisposable
    {
        Task InitialiseAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ILightInfo>> GetAvailableLightsAsync(CancellationToken cancellationToken = default);
    }
}
