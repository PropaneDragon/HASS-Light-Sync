using LightSync.API.Lights;

namespace LightSync.Core.Lights.Actions
{
    public interface ITemporaryLightAction
    {
        public Task PerformActionAsync(CancellationToken cancellationToken = default, params ILightInfo[] lights);
    }
}
