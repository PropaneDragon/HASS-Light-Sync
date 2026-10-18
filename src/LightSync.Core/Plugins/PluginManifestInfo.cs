namespace LightSync.Core.Plugins
{
    public readonly struct PluginManifestInfo
    {
        public bool IsValid => Manifest != null;

        public string ManifestDirectory { get; init; }
        public string? ErrorMessage { get; init; }
        public IPluginManifest? Manifest { get; init; }

        private PluginManifestInfo(string manifestDirectory)
        {
            ManifestDirectory = manifestDirectory;
        }

        internal PluginManifestInfo(string manifestDirectory, IPluginManifest pluginManifest) : this(manifestDirectory)
        {
            Manifest = pluginManifest;
        }

        internal PluginManifestInfo(string manifestDirectory, string errorMessage) : this(manifestDirectory)
        {
            ErrorMessage = errorMessage;
        }
    }
}
