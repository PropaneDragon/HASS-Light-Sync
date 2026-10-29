using LightSync.API.Plugins;
using LightSync.Core.Paths;
using LightSync.Core.Plugins;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.Plugins
{
    internal interface IPluginManager
    {
        IReadOnlyList<IPlugin> Plugins { get; }
        IReadOnlyList<ILightPlugin> LightPluugins { get; }
        IReadOnlyList<ICapturePlugin> CapturePlugins { get; }

        Task LoadPluginsAsync(CancellationToken cancellationToken = default);
    }

    internal class PluginManager : IPluginManager
    {
        private readonly Lock _pluginsLock = new();
        private readonly List<IPlugin> _plugins = [];

        private readonly IPluginLoader _pluginLoader;
        private readonly IPluginManifestLoader _pluginManifestLoader;
        private readonly ICommonPaths _commonPaths;

        public IReadOnlyList<IPlugin> Plugins { get { lock (_pluginsLock) return [.. _plugins]; } }
        public IReadOnlyList<ILightPlugin> LightPluugins => [.. Plugins.OfType<ILightPlugin>()];
        public IReadOnlyList<ICapturePlugin> CapturePlugins => [.. Plugins.OfType<ICapturePlugin>()];

        public PluginManager(IPluginLoader pluginLoader, IPluginManifestLoader pluginManifestLoader, ICommonPaths commonPaths)
        {
            _pluginLoader = pluginLoader;
            _pluginManifestLoader = pluginManifestLoader;
            _commonPaths = commonPaths;
        }

        public async Task LoadPluginsAsync(CancellationToken cancellationToken = default)
        {
            var existingPlugins = Plugins;

            lock (_pluginsLock)
            {
                _plugins.Clear();
            }

            foreach (var plugin in existingPlugins)
            {
                await plugin.DisposeAsync();
            }

            cancellationToken.ThrowIfCancellationRequested();

            IReadOnlyList<PluginManifestInfo> applicationPluginManifests = [];
            IReadOnlyList<PluginManifestInfo> userPluginManifests = [];

            try
            {
                applicationPluginManifests = await _pluginManifestLoader.GetAllPluginManifestsAsync(_commonPaths.ApplicationPluginsPath, cancellationToken);
            }
            catch
            {
                //Todo
            }

            try
            {
                userPluginManifests = await _pluginManifestLoader.GetAllPluginManifestsAsync(_commonPaths.UserPluginsPath, cancellationToken);
            }
            catch
            {
                //Todo
            }

            List<PluginManifestInfo> allPluginManifests = [.. applicationPluginManifests, .. userPluginManifests];

            foreach (var manifestInfo in allPluginManifests)
            {
                if (!manifestInfo.IsValid)
                    continue; //Todo: Mark as invalid and give reason!

                var pluginInfo = _pluginLoader.GetPluginInfo(manifestInfo);
                if (!pluginInfo.IsValid)
                    continue; //Todo: Mark as invalid and give reason!

                if (pluginInfo.Plugin == null)
                    continue; //Shouldn't get here (IsValid should cover this.

                lock (_pluginsLock)
                {
                    _plugins.Add(pluginInfo.Plugin);
                }
            }
        }
    }
}
