using LightSync.API.Plugins;

namespace LightSync.Core.Plugins
{
    public readonly struct PluginInfo
    {
        public readonly bool IsValid => Plugin != null;

        public string? ErrorMessage { get; init; }
        public PluginManifestInfo ManifestInfo { get; init; }
        public IPlugin? Plugin { get; init; }

        private PluginInfo(PluginManifestInfo manifestInfo)
        {
            ManifestInfo = manifestInfo;
        }

        internal PluginInfo(PluginManifestInfo manifestInfo, string errorMessage) : this(manifestInfo)
        {
            ErrorMessage = errorMessage;
        }

        internal PluginInfo(PluginManifestInfo manifestInfo, IPlugin plugin) : this(manifestInfo)
        {
            Plugin = plugin;
        }
    }
}
