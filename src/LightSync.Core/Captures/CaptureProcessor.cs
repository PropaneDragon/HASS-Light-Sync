using LightSync.API.Captures;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LightSync.Core.Captures
{
    internal class CaptureProcessor : IDisposable
    {
        private readonly Lock _leaseLock = new();
        private readonly Lock _framerateLock = new();

        private readonly int _width;
        private readonly int _height;
        private readonly List<ICaptureLease> _leases = [];
        private readonly ICaptureInfo _captureInfo;
        private readonly ICaptureManager _captureManager;

        private int _framerate;
        private CancellationTokenSource? _processCancellationTokenSource;
        private Task? _processTask;

        public bool IsEmpty { get { lock (_leaseLock) return _leases.Count == 0; } }

        public int Framerate
        {
            get { lock(_framerateLock) { return _framerate; } }
            set { lock (_framerateLock) { _framerate = value; } }
        }

        public CaptureProcessor(ICaptureInfo captureInfo, ICaptureManager captureManager, int width, int height)
        {
            _captureInfo = captureInfo;
            _captureManager = captureManager;

            _width = width;
            _height = height;
        }

        public ICaptureLease CreateLease(int framerate)
        {
            var lease = new CaptureLease(_captureManager, framerate);
            lock (_leaseLock)
                _leases.Add(lease);

            RecalculateFramerate();
            UpdateProcessInfo();

            return lease;
        }

        public void Remove(ICaptureLease lease)
        {
            lock (_leaseLock)
                _leases.Remove(lease);

            RecalculateFramerate();
        }

        public void Dispose()
        {
            _processCancellationTokenSource?.Cancel();
            _processCancellationTokenSource?.Dispose();
        }

        private void RecalculateFramerate()
        {
            lock(_leaseLock)
                Framerate = _leases.Count != 0 ? _leases.Max(lease => lease.CaptureFeed.Framerate) : 0;
        }

        private void UpdateProcessInfo()
        {
            if (_processTask != null)
                return;

            _processCancellationTokenSource?.Cancel();
            _processCancellationTokenSource?.Dispose();

            _processCancellationTokenSource = new();
            _processTask = Task.Run(() => ProcessAsync(_processCancellationTokenSource.Token), _processCancellationTokenSource.Token);
        }

        private async Task ProcessAsync(CancellationToken cancellationToken)
        {
            await using var session = await _captureInfo.OpenAsync(_width, _height, cancellationToken);
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1) / Framerate);

            byte[] buffer = [];

            while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
            {
                if (session.Width > _width || session.Height > _height)
                    break;

                var bufferSize = session.Width * session.Height * 4;

                if (buffer.Length != bufferSize)
                    buffer = new byte[bufferSize];

                var status = await session.TryGetLatestFrameAsync(buffer, cancellationToken);
                if (status != CaptureStatus.Ok)
                    continue;

                lock (_leaseLock)
                {
                    foreach (var lease in _leases)
                    {
                        lease.CaptureFeed.UpdateData(buffer, session.Width, session.Height);
                    }
                }

                timer.Period = TimeSpan.FromSeconds(1) / Framerate;
            }
        }
    }
}
