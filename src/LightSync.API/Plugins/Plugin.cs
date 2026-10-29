namespace LightSync.API.Plugins
{
    public abstract class Plugin : IPlugin
    {
        public abstract string Name { get; }

        public abstract Task<bool> InitialiseAsync(CancellationToken cancellationToken = default);
        public abstract ValueTask DisposeAsync();
    }
}
