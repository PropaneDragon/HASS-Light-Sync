using LightSync.API.Captures;
using SeeShark.Device;

namespace LightSync.VideoCapture
{
    internal class VideoCaptureInfo : ICaptureInfo
    {
        private readonly DisplayManager _displayManager;
        private readonly DisplayInfo _displayInfo;

        public string Name { get; }

        public string Identifier { get; }

        public VideoCaptureInfo(DisplayManager displayManager, DisplayInfo displayInfo)
        {
            _displayManager = displayManager;
            _displayInfo = displayInfo;

            Name = displayInfo.Name ?? "Unknown";
            Identifier = displayInfo.Path;
        }

        public async Task<ICaptureSession> OpenAsync(int maxWidth, int maxHeight, CancellationToken cancellationToken = default)
        {
            var display = _displayManager.GetDevice(_displayInfo, new() { DrawMouse = false });
            display.StartCapture();

            return new VideoCaptureSession(display, maxWidth, maxHeight);
        }
    }
}
