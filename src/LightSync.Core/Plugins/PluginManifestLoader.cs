using System.Text.Json;

namespace LightSync.Core.Plugins
{
    public class PluginManifestLoader : IPluginManifestLoader
    {
        private static readonly string _manifestFileName = "plugin.json";
        private static readonly JsonSerializerOptions _exampleSerialiserOptions = new(JsonSerializerDefaults.General)
        {
            WriteIndented = true
        };

        public async Task<IReadOnlyList<PluginManifestInfo>> GetAllPluginManifestsAsync(string pluginsDirectory, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(pluginsDirectory))
                throw new IOException("Plugin directory doesn't exist.");

            var directories = await Task.Run(() => Directory.EnumerateDirectories(pluginsDirectory), cancellationToken);
            var manifestInfos = new List<PluginManifestInfo>();

            foreach (var directory in directories)
            {
                var manifestPath = Path.Combine(directory, _manifestFileName);

                try
                {
                    var manifest = await TryLoadPluginManifestAsync(manifestPath, cancellationToken);
                    manifestInfos.Add(new(directory, manifest));
                }
                catch (Exception ex)
                {
                    try { await TryWriteExampleJsonFileAsync(directory, cancellationToken); } catch { }

                    manifestInfos.Add(new(directory, $"The manifest could not be loaded. {ex.Message}"));
                    continue;
                }
            }

            return manifestInfos;
        }

        private static async Task<IPluginManifest> TryLoadPluginManifestAsync(string manifestPath, CancellationToken cancellationToken = default)
        {
            if (!File.Exists(manifestPath))
                throw new IOException("Manifest file does not exist.");

            try
            {
                using var readStream = File.OpenRead(manifestPath);
                var manifest = await JsonSerializer.DeserializeAsync<PluginManifest>(readStream, cancellationToken: cancellationToken);

                return manifest ?? throw new NullReferenceException("Manifest file could not be deserialised.");
            }
            catch (Exception ex)
            {
                throw new IOException("Failed to read manifest file.", ex);
            }
        }

        private static async Task TryWriteExampleJsonFileAsync(string pluginDirectory, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(pluginDirectory))
                return;

            var exampleFilePath = Path.Combine(pluginDirectory, $"{_manifestFileName}.example");

            try
            {
                using var writeStream = File.OpenWrite(exampleFilePath);
                var exampleManifest = new PluginManifest()
                {
                    Name = "Plugin name",
                    ApiVersion = "1.0",
                    EntryAssemblyName = "Entry plugin dll assembly"
                };

                await JsonSerializer.SerializeAsync(writeStream, exampleManifest, _exampleSerialiserOptions, cancellationToken);
            }
            catch
            {
                //Write to log.
            }
        }
    }
}
