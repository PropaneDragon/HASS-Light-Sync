using LightSync.Abstractions.Lights;

namespace LightSync.Abstractions.Plugins
{
    public interface IPlugin : IAsyncDisposable
    {
        string Name { get; }

        ILightInfoSource LightInfoSource { get; }

        Task<bool> InitialiseAsync(CancellationToken cancellationToken = default);
    }
}
