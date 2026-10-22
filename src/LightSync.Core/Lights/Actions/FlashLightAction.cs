using LightSync.API.Lights;
using System.Drawing;

namespace LightSync.Core.Lights.Actions
{
    public class FlashLightAction : TemporaryLightAction
    {
        private readonly int _flashes;

        public FlashLightAction(int flashes = 3)
        {
            _flashes = flashes;
        }

        protected override async Task ExecuteActionAsync(CancellationToken cancellationToken = default, params ILightInfo[] lights)
        {
            var defaultLightState = LightHistory.CreateStored(lights);

            for (int flash = 0; flash < _flashes; ++flash)
            {
                DimLights(lights);
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                defaultLightState.Restore(TimeSpan.FromMilliseconds(200));
                await Task.Delay(TimeSpan.FromMilliseconds(500));
            }
        }

        private static void DimLights(ILightInfo[] lights)
        {
            foreach (var light in lights)
            {
                var dimmedBrightness = light.Colour.A < 128 ? light.Colour.A + 40 : light.Colour.A - 40;

                _ = light.SetColourAsync(Color.FromArgb(dimmedBrightness, light.Colour), TimeSpan.FromMilliseconds(200));
            }
        }
    }
}
