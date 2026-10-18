using System.Text.Json.Serialization;

namespace LightSync.Core.Plugins
{
    internal record PluginManifest : IPluginManifest
    {
        [JsonPropertyName("name"), JsonRequired]
        public required string Name { get; init; }

        [JsonPropertyName("entryAssemblyName"), JsonRequired]
        public required string EntryAssemblyName { get; init; }

        [JsonPropertyName("apiVersion")]
        public string? ApiVersion { get; init; }
    }
}
