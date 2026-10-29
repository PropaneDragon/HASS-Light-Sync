using LightSync.API.Captures;

namespace LightSync.API.Plugins
{
    public interface ICapturePlugin : IPlugin
    {
        ICaptureInfoSource CaptureInfoSource { get; }
    }
}
