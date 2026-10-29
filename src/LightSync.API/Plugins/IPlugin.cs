namespace LightSync.API.Plugins
{
    public interface IPlugin : IAsyncDisposable
    {
        string Name { get; }

        Task<bool> InitialiseAsync(CancellationToken cancellationToken = default);
    }
}
