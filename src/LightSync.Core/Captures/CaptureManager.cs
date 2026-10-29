using LightSync.API.Captures;
using System.Collections.Concurrent;

namespace LightSync.Core.Captures
{
    public class CaptureManager : ICaptureManager
    {
        private readonly ConcurrentDictionary<ICaptureLease, ICaptureInfo> _leaseInfoCache = [];
        private readonly ConcurrentDictionary<ICaptureInfo, CaptureProcessor> _leases = [];

        public ICaptureLease AcquireLease(ICaptureInfo captureInfo, int framerate = 15)
        {
            if (!_leases.ContainsKey(captureInfo))
                _leases[captureInfo] = new CaptureProcessor(captureInfo, this, 900, 900);

            var processor = _leases[captureInfo];
            var lease = processor.CreateLease(framerate);

            _leaseInfoCache[lease] = captureInfo;

            return lease;
        }

        public bool ReleaseLease(ICaptureLease lease)
        {
            if (!_leaseInfoCache.ContainsKey(lease))
                return false;

            var captureInfo = _leaseInfoCache[lease];
            if (!_leases.ContainsKey(captureInfo))
                return false;

            var processor = _leases[captureInfo];
            processor.Remove(lease);

            if (processor.IsEmpty)
            {
                processor.Dispose();
                _leases.TryRemove(captureInfo, out _);
            }

            return true;
        }
    }
}
