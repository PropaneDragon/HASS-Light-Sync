using LightSync.API.Captures;
using SeeShark.Device;

namespace LightSync.VideoCapture
{
    internal class VideoCaptureInfoSource : ICaptureInfoSource
    {
        private readonly Lock _capturesLock = new();
        private readonly List<ICaptureInfo> _captures = [];

        private DisplayManager? _displayManager;

        public IReadOnlyList<ICaptureInfo> Captures
        {
            get { lock (_capturesLock) { return _captures; } }
            private set
            {
                lock (_capturesLock)
                {
                    _captures.Clear();
                    _captures.AddRange(value);
                }
            }
        }

        public VideoCaptureInfoSource()
        {

        }

        public async Task InitialiseAsync(CancellationToken cancellationToken = default)
        {
            _displayManager = new DisplayManager();

            await RefreshAsync(cancellationToken);
        }

        public async Task RefreshAsync(CancellationToken cancellationToken = default)
        {
            if (_displayManager == null)
                return;

            Captures = [.. _displayManager.Devices.Select(device => new VideoCaptureInfo(_displayManager, device))];
        }

        public async ValueTask DisposeAsync()
        {
            _displayManager?.Dispose();
        }
    }
}
