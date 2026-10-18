using System.Reflection;
using System.Runtime.Loader;

namespace LightSync.Core.Plugins
{
    internal class PluginLoadContext : AssemblyLoadContext
    {
        private readonly string _pluginDllPath;
        private readonly AssemblyDependencyResolver _dependencyResolver;

        public PluginLoadContext(string pluginDllPath) : base(Path.GetFileNameWithoutExtension(pluginDllPath))
        {
            _pluginDllPath = pluginDllPath;
            _dependencyResolver = new AssemblyDependencyResolver(pluginDllPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (Default.Assemblies.Any(assembly => assembly.GetName().Name == assemblyName.Name))
                return null;

            var path = _dependencyResolver.ResolveAssemblyToPath(assemblyName);

            return string.IsNullOrWhiteSpace(path) ? null : LoadFromAssemblyPath(path);
        }

        protected override nint LoadUnmanagedDll(string unmanagedDllName)
        {
            var path = _dependencyResolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            return path is null ? nint.Zero : LoadUnmanagedDllFromPath(path);
        }
    }
}
