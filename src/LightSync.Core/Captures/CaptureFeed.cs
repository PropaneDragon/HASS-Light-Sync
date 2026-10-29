namespace LightSync.Core.Captures
{
    internal class CaptureFeed : ICaptureFeed
    {
        private readonly Lock _updateLock = new();
        private readonly Lock _bufferLock = new();
        private readonly Lock _currentLock = new();

        private int _width;
        private int _height;
        private byte[]? _buffer;
        private byte[]? _current;
        private DateTimeOffset _lastUpdate = DateTimeOffset.MinValue;

        public bool HasData { get; private set; } = false;

        public int Framerate { get; init; }
        public int DataLength => Width * Height * 4;

        public int Width
        {
            get { lock(_updateLock) return _width; }
            private set { lock(_updateLock) _width = value; }
        }

        public int Height
        {
            get { lock(_updateLock) return _height; }
            private set { lock(_updateLock) _height = value; }
        }

        public CaptureFeed(int framerate)
        {
            Framerate = framerate;
        }

        public bool CopyLatestData(byte[] destination)
        {
            lock (_currentLock)
            {
                if (_current == null)
                    return false;

                if (destination.Length < _current.Length)
                    return false; //Maybe throw?

                Array.Copy(_current, destination, _current.Length);
                return true;
            }
        }

        public bool TryCopyLatestData(byte[] destination, DateTimeOffset lastCopy)
        {
            bool hasNewData;
            lock (_updateLock)
                hasNewData = _lastUpdate > lastCopy;

            if (!hasNewData)
                return false;

            return CopyLatestData(destination);
        }

        public void UpdateData(byte[] data, int width, int height)
        {
            Width = width;
            Height = height;

            lock (_bufferLock)
            {
                if (_buffer == null || _buffer.Length != data.Length)
                    _buffer = new byte[data.Length];

                Array.Copy(data, _buffer, data.Length);

                lock (_currentLock)
                {
                    (_buffer, _current) = (_current, _buffer);
                }
            }

            lock (_updateLock)
                _lastUpdate = DateTimeOffset.UtcNow;

            HasData = true;
        }
    }
}
