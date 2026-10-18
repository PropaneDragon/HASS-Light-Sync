namespace LightSync.Core.Paths
{
    public class CommonPaths : ICommonPaths
    {
        private static readonly string _applicationDirectoryName = "LightSync";
        private static readonly string _applicationPluginDirectoryName = "Plugins";

        public string ApplicationPluginsPath => Path.Combine(AppContext.BaseDirectory, _applicationPluginDirectoryName);

        public string UserPluginsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _applicationDirectoryName, _applicationPluginDirectoryName);
    }
}
