namespace LightSync.API.Lights
{
    public interface ILightInfoSource : IAsyncDisposable
    {
        Task InitialiseAsync(CancellationToken cancellationToken = default);
        Task<List<ILightInfo>> GetAvailableLightsAsync(CancellationToken cancellationToken = default);
    }
}
