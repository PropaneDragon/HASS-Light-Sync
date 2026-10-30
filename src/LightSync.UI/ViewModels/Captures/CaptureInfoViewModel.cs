using CommunityToolkit.Mvvm.ComponentModel;
using LightSync.API.Captures;
using System;

namespace LightSync.UI.ViewModels.Captures
{
    internal partial class CaptureInfoViewModel : ObservableObject, IDisposable
    {
        public ICaptureInfo AssociatedCaptureInfo { get; init; }

        [ObservableProperty]
        public partial bool IsSelected { get; set; }

        [ObservableProperty]
        public partial string? Name { get; private set; }

        [ObservableProperty]
        public partial string? Identifier { get; private set; }

        public CaptureInfoViewModel(ICaptureInfo captureInfo)
        {
            AssociatedCaptureInfo = captureInfo;

            Name = captureInfo.Name;
            Identifier = captureInfo.Identifier;
        }

        public void Dispose()
        {
        }
    }
}
