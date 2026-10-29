using LightSync.API.Captures;
using LightSync.API.Plugins;
using SeeShark.FFmpeg;

namespace LightSync.VideoCapture
{
    public class VideoCapturePlugin : CapturePlugin
    {
        public override ICaptureInfoSource CaptureInfoSource { get; } = new VideoCaptureInfoSource();

        public override string Name => "Video capture";

        public VideoCapturePlugin()
        {
            var path = Path.GetDirectoryName(typeof(VideoCapturePlugin).Assembly.Location);
            if (path == null)
                return;

            FFmpegManager.SetupFFmpeg(path);
        }

        protected override async Task<bool> InitialiseInternalAsync(CancellationToken cancellationToken)
        {
            return true;
        }

        protected override async Task DisposeInternalAsync()
        {
        }
    }
}
