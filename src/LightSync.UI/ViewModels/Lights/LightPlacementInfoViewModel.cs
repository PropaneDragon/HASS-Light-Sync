using CommunityToolkit.Mvvm.ComponentModel;
using LightSync.Core.Lights;
using System;

namespace LightSync.UI.ViewModels.Lights
{
    internal partial class LightPlacementInfoViewModel : ObservableObject, IDisposable
    {
        [ObservableProperty]
        public partial double RelativeCaptureRadius { get; set; }

        [ObservableProperty]
        public partial double RelativePositionX { get; set; }

        [ObservableProperty]
        public partial double RelativePositionY { get; set; }

        public ILightPlacementInfo AssociatedLightPlacementInfo { get; init; }

        public LightInfoViewModel LightInfo { get; init; }

        public LightPlacementInfoViewModel(ILightPlacementInfo lightPlacementInfo)
        {
            AssociatedLightPlacementInfo = lightPlacementInfo;
            LightInfo = new(lightPlacementInfo.Light);

            RelativeCaptureRadius = lightPlacementInfo.RelativeCaptureRadius;
            RelativePositionX = lightPlacementInfo.RelativePositionX;
            RelativePositionY = lightPlacementInfo.RelativePositionY;
        }

        public void Dispose()
        {
            LightInfo.Dispose();
        }

        partial void OnRelativeCaptureRadiusChanged(double value)
        {
            AssociatedLightPlacementInfo.RelativeCaptureRadius = value;
        }

        partial void OnRelativePositionXChanged(double value)
        {
            AssociatedLightPlacementInfo.RelativePositionX = value;
        }

        partial void OnRelativePositionYChanged(double value)
        {
            AssociatedLightPlacementInfo.RelativePositionY = value;
        }
    }
}
