using LightSync.API.Lights;
using LightSync.API.Plugins;

namespace LightSync.HomeAssistant
{
    public class HomeAssistantPlugin : LightPlugin
    {
        private readonly ConnectionManager _connectionManager = new();
        private readonly HomeAssistantLightInfoSource _lightInfoSource;

        public HomeAssistantPlugin()
        {
            _lightInfoSource = new(_connectionManager);
        }

        public override string Name => "Home Assistant Light Sources";

        public override ILightInfoSource LightInfoSource => _lightInfoSource;

        protected override async Task<bool> InitialiseInternalAsync(CancellationToken cancellationToken = default)
        {
            if (!await _connectionManager.ConnectAsync(cancellationToken))
                return false;

            return true;
        }

        protected override async Task DisposeInternalAsync()
        {
            await _connectionManager.DisposeAsync();
        }
    }
}
