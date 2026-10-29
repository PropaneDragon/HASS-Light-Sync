using LightSync.API.Captures;

namespace LightSync.API.Plugins
{
    public abstract class CapturePlugin : Plugin, ICapturePlugin
    {
        public abstract ICaptureInfoSource CaptureInfoSource { get; }

        public override async Task<bool> InitialiseAsync(CancellationToken cancellationToken = default)
        {
            await CaptureInfoSource.InitialiseAsync(cancellationToken);
            return await InitialiseInternalAsync(cancellationToken);
        }

        public override async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);

            await CaptureInfoSource.DisposeAsync();
            await DisposeInternalAsync();
        }

        protected abstract Task<bool> InitialiseInternalAsync(CancellationToken cancellationToken);
        protected abstract Task DisposeInternalAsync();
    }
}
