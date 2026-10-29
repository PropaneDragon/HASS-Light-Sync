using LightSync.API.Lights;

namespace LightSync.API.Plugins
{
    public abstract class LightPlugin : Plugin, ILightPlugin
    {
        public abstract ILightInfoSource LightInfoSource { get; }

        public override async Task<bool> InitialiseAsync(CancellationToken cancellationToken = default)
        {
            await LightInfoSource.InitialiseAsync(cancellationToken);
            return await InitialiseInternalAsync(cancellationToken);
        }

        public override async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);

            await LightInfoSource.DisposeAsync();
            await DisposeInternalAsync();
        }

        protected abstract Task<bool> InitialiseInternalAsync(CancellationToken cancellationToken);
        protected abstract Task DisposeInternalAsync();
    }
}
