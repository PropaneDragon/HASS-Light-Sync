using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using LightSync.API.Lights;
using LightSync.Core.Lights;
using System;

namespace LightSync.UI.ViewModels.Lights
{
    internal partial class LightEditorLightInfoViewModel : ObservableObject, IDisposable
    {
        public bool HasLightPlacement => LightPlacementIndex >= 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasLightPlacement))]
        public partial int LightPlacementIndex { get; set; } = -1;

        public ILightInfo AssociatedLightInfo => LightInfo.AssociatedLightInfo;
        public ILightPlacementInfo? AssociatedLightPlacementInfo => LightPlacementInfo?.AssociatedLightPlacementInfo;

        public LightInfoViewModel LightInfo { get; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AssociatedLightPlacementInfo))]
        public partial LightPlacementInfoViewModel? LightPlacementInfo { get; private set; }

        public LightEditorLightInfoViewModel(ILightInfo lightInfo)
        {
            LightInfo = new(lightInfo);
        }

        public void SetLightPlacement(ILightPlacementInfo? lightPlacementInfo)
        {
            LightPlacementInfo?.Dispose();
            LightPlacementInfo = null;

            if (lightPlacementInfo != null)
                LightPlacementInfo = new(lightPlacementInfo);
        }

        public void Dispose()
        {
            LightInfo.Dispose();
            LightPlacementInfo?.Dispose();
        }
    }
}
