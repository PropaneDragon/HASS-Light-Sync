using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using LightSync.Core.Captures;
using LightSync.UI.Plugins;
using LightSync.UI.ViewModels.Captures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.ViewModels.Pages
{
    internal interface ISourcesPageViewModel : IPageViewModelBase { }

    internal partial class SourcesPageViewModel : PageViewModelBase, ISourcesPageViewModel
    {
        private readonly IPluginManager _pluginManager;
        private readonly ICaptureManager _captureManager;
        private readonly DispatcherTimer _bitmapUpdateTimer;

        private byte[] _rawData = [];
        private ICaptureLease? _captureLease;
        private WriteableBitmap? _back;
        private WriteableBitmap? _front;
        private DateTimeOffset _lastCapture = DateTimeOffset.MinValue;

        [ObservableProperty]
        public partial IReadOnlyList<CaptureInfoViewModel> Sources { get; private set; } = [];

        [ObservableProperty]
        public partial WriteableBitmap? PreviewBitmap { get; private set; }

        public SourcesPageViewModel(IPluginManager pluginManager, ICaptureManager captureManager)
        {
            _pluginManager = pluginManager;
            _captureManager = captureManager;

            _bitmapUpdateTimer = new(TimeSpan.FromMilliseconds(20), DispatcherPriority.Render, OnUpdateTimerTick);

            SynchroniseSourcesFromPlugins();
        }

        public override async Task OnUnloadedAsync(CancellationToken cancellationToken = default)
        {
            _bitmapUpdateTimer.Stop();

            PreviewBitmap?.Dispose();
            _back?.Dispose();
            _front?.Dispose();
            _captureLease?.Dispose();
        }

        private void SynchroniseSourcesFromPlugins()
        {
            Sources = [.. _pluginManager.CapturePlugins.SelectMany(plugin => plugin.CaptureInfoSource.Captures).Select(capture => new CaptureInfoViewModel(capture))];

            var testSource = Sources.LastOrDefault();
            if (testSource != null)
            {
                _captureLease = _captureManager.AcquireLease(testSource.AssociatedCaptureInfo);
            }
            //UpdateFilter();
        }

        private void OnUpdateTimerTick(object? sender, EventArgs e)
        {
            if (_captureLease == null)
                return;

            if (!_captureLease.CaptureFeed.HasData)
                return;

            var width = _captureLease.CaptureFeed.Width;
            var height = _captureLease.CaptureFeed.Height;
            if (width <= 0 || height <= 0)
                return;

            var dataLength = _captureLease.CaptureFeed.DataLength;
            if (_rawData.Length != dataLength)
                _rawData = new byte[dataLength];

            if (!_captureLease.CaptureFeed.TryCopyLatestData(_rawData, _lastCapture))
                return;

            _lastCapture = DateTimeOffset.Now;

            var size = new PixelSize(width, height);
            if (_back == null || _back.PixelSize != size)
            {
                _back?.Dispose();
                _back = new(size, new(96, 96), PixelFormat.Rgba8888, AlphaFormat.Opaque);
            }

            var rowBytes = width * 4;
            using (var framebuffer = _back.Lock())
            {
                if (framebuffer.RowBytes == rowBytes)
                    Marshal.Copy(_rawData, 0, framebuffer.Address, dataLength);
                else
                    for (var y = 0; y < height; ++y)
                        Marshal.Copy(_rawData, y * rowBytes, framebuffer.Address + (y * framebuffer.RowBytes), rowBytes);
            }

            (_front, _back) = (_back,  _front);

            PreviewBitmap = _front;
        }
    }
}
