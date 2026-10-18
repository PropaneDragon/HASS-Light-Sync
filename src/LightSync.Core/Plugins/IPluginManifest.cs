namespace LightSync.Core.Plugins
{
    public interface IPluginManifest
    {
        string Name { get; }
        string EntryAssemblyName { get; }
        string? ApiVersion { get; }
    }
}
