using LightSync.API.Captures;
using SeeShark;
using SeeShark.Device;

namespace LightSync.VideoCapture
{
    internal class VideoCaptureSession : ICaptureSession
    {
        private readonly Lock _frameLock = new();
        private readonly Lock _sizeLock = new();

        private readonly int _maxWidth, _maxHeight;
        private readonly Display _display;

        private int _width, _height;
        private Frame? _lastFrame;
        private FrameConverter? _frameConverter;

        public int Width
        {
            get { lock (_sizeLock) return _width; }
            private set { lock (_sizeLock) _width = value; }
        }

        public int Height
        {
            get { lock (_sizeLock) return _height; }
            private set { lock (_sizeLock) _height = value; }
        }

        public CaptureFormat Format => CaptureFormat.Rgba32;

        public VideoCaptureSession(Display display, int maxWidth, int maxHeight)
        {
            _display = display;
            _maxWidth = maxWidth;
            _maxHeight = maxHeight;

            _width = maxWidth;
            _height = maxHeight;

            display.OnFrame += Display_OnFrame;
        }

        public async Task<CaptureStatus> TryGetLatestFrameAsync(byte[] pixels, CancellationToken cancellationToken = default)
        {
            if (!_display.IsPlaying || _display.IsDisposed)
                return CaptureStatus.Ended;

            if (_lastFrame == null)
                return CaptureStatus.NoNewFrame;

            lock (_frameLock)
            {
                try
                {
                    if (_lastFrame.Width <= 0 || _lastFrame.Height <= 0)
                        return CaptureStatus.NoNewFrame;

                    (Width, Height) = FitSize(_lastFrame.Width, _lastFrame.Height);

                    _frameConverter ??= new FrameConverter(_lastFrame.Width, _lastFrame.Height, _lastFrame.PixelFormat, Width, Height, PixelFormat.Rgba);

                    var convertedFrame = _frameConverter.Convert(_lastFrame);
                    if (!convertedFrame.RawData.TryCopyTo(pixels))
                        return CaptureStatus.Ended;

                    return CaptureStatus.Ok;
                }
                catch
                {

                }

                return CaptureStatus.NoNewFrame;
            }
        }

        private (int width, int height) FitSize(int width, int height)
        {
            var scaledWidth = (double)_maxWidth / width;
            var scaledHeight = (double)_maxHeight / height;
            var scale = Math.Min(scaledWidth, scaledHeight);

            if (scale > 1d)
                return (width, height);

            return ((int)Math.Round(width * scale), (int)Math.Round(height * scale));
        }

        public async ValueTask DisposeAsync()
        {
            _display.OnFrame -= Display_OnFrame;
            _display.StopCapture();
            _display.Dispose();
        }

        private void Display_OnFrame(object? sender, FrameEventArgs e)
        {
            lock (_frameLock)
            {
                _lastFrame = e.Frame;
            }
        }
    }
}
