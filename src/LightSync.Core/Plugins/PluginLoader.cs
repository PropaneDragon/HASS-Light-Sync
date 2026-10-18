using LightSync.API.Plugins;
using System.Reflection;

namespace LightSync.Core.Plugins
{
    public class PluginLoader : IPluginLoader
    {
        public PluginInfo GetPluginInfo(PluginManifestInfo manifestInfo)
        {
            if (manifestInfo.Manifest == null)
                return new(manifestInfo, "The manifest file is invalid.");

            var entryName = Path.GetFileName(manifestInfo.Manifest.EntryAssemblyName);
            var entryAssemblyPath = Path.Combine(manifestInfo.ManifestDirectory, entryName);

            if (!File.Exists(entryAssemblyPath))
                return new(manifestInfo, "The entry assembly could not be located.");

            try
            {
                var assembly = new PluginLoadContext(entryAssemblyPath).LoadFromAssemblyPath(entryAssemblyPath);
                var pluginTypes = GetPluginTypes(assembly);

                if (pluginTypes.Count > 1)
                    return new(manifestInfo, "Multiple plugin types were found under the assembly. Only one plugin type is allowed.");

                if (pluginTypes.Count < 1)
                    return new(manifestInfo, "No valid plugin type was found under the assembly.");

                var plugin = GetPlugin(pluginTypes[0]);
                return new(manifestInfo, plugin);
            }
            catch (Exception ex)
            {
                return new(manifestInfo, $"The plugin failed to load. {ex.Message}");
            }
        }

        private static IReadOnlyList<Type> GetPluginTypes(Assembly assembly)
        {
            return [.. assembly.ExportedTypes.Where(type => typeof(IPlugin).IsAssignableFrom(type))];
        }

        private static IPlugin GetPlugin(Type pluginType)
        {
            if (!typeof(IPlugin).IsAssignableFrom(pluginType))
                throw new InvalidOperationException($"Provided type {pluginType} does not inherit {typeof(IPlugin)} and cannot be instantiated.");

            return Activator.CreateInstance(pluginType) is IPlugin plugin ? plugin : throw new NullReferenceException($"Plugin {pluginType} could not be instantiated.");
        }
    }
}
