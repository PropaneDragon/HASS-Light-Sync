using LightSync.API.Lights;

namespace LightSync.API.Plugins
{
    public interface IPlugin : IAsyncDisposable
    {
        string Name { get; }

        ILightInfoSource LightInfoSource { get; }

        Task<bool> InitialiseAsync(CancellationToken cancellationToken = default);
    }
}
