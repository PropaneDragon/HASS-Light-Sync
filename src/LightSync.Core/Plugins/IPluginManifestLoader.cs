namespace LightSync.Core.Plugins
{
    public interface IPluginManifestLoader
    {
        Task<IReadOnlyList<PluginManifestInfo>> GetAllPluginManifestsAsync(string pluginsDirectory, CancellationToken cancellationToken = default);
    }
}
