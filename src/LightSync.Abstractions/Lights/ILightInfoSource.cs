namespace LightSync.Abstractions.Lights
{
    public interface ILightInfoSource : IAsyncDisposable
    {
        Task InitialiseAsync();
        Task<List<ILightInfo>> GetAvailableLightsAsync();
    }
}
