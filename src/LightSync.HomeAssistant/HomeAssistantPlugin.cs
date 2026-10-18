using LightSync.API.Lights;
using LightSync.API.Plugins;

namespace LightSync.HomeAssistant
{
    public class HomeAssistantPlugin : IPlugin
    {
        private readonly ConnectionManager _connectionManager = new();
        private readonly HomeAssistantLightInfoSource _lightInfoSource;

        public HomeAssistantPlugin()
        {
            _lightInfoSource = new(_connectionManager);
        }

        public string Name => "Home Assistant Plugin";

        public ILightInfoSource LightInfoSource => _lightInfoSource;

        public async Task<bool> InitialiseAsync(CancellationToken cancellationToken = default)
        {
            if (!await _connectionManager.ConnectAsync(cancellationToken))
                return false;

            return true;
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);

            await _lightInfoSource.DisposeAsync();
            await _connectionManager.DisposeAsync();
        }
    }
}
