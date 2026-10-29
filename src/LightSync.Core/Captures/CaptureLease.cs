using LightSync.API.Captures;

namespace LightSync.Core.Captures
{
    internal class CaptureLease : ICaptureLease
    {
        private readonly ICaptureManager _captureManager;

        public ICaptureFeed CaptureFeed { get; init; }

        public CaptureLease(ICaptureManager captureManager, int framerate)
        {
            _captureManager = captureManager;

            CaptureFeed = new CaptureFeed(framerate);
        }

        public void Dispose()
        {
            _captureManager.ReleaseLease(this);
        }
    }
}
