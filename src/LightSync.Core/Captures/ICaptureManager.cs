using LightSync.API.Captures;

namespace LightSync.Core.Captures
{
    public interface ICaptureManager
    {
        ICaptureLease AcquireLease(ICaptureInfo captureInfo, int framerate = 15);
        bool ReleaseLease(ICaptureLease lease);
    }
}
