using LightSync.API.Lights;

namespace LightSync.Core.Lights.Actions
{
    public abstract class TemporaryLightAction : ITemporaryLightAction
    {
        protected TimeSpan RestoreTransitionTime { get; set; } = TimeSpan.FromSeconds(1);

        public async Task PerformActionAsync(CancellationToken cancellatonToken = default, params ILightInfo[] lights)
        {
            using var history = LightHistory.CreateStored(RestoreTransitionTime, lights);
            await ExecuteActionAsync(cancellatonToken, lights);
        }

        protected abstract Task ExecuteActionAsync(CancellationToken cancellationToken = default, params ILightInfo[] lights);
    }
}
