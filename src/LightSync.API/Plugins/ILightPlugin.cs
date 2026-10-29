using LightSync.API.Lights;

namespace LightSync.API.Plugins
{
    public interface ILightPlugin : IPlugin
    {
        ILightInfoSource LightInfoSource { get; }
    }
}
