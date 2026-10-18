namespace LightSync.Core.Plugins
{
    public interface IPluginLoader
    {
        PluginInfo GetPluginInfo(PluginManifestInfo manifestInfo);
    }
}
