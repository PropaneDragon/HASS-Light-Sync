using CommunityToolkit.Mvvm.ComponentModel;
using LightSync.Core.Lights.Actions;
using LightSync.UI.Plugins;
using LightSync.UI.ViewModels.Lights;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.ViewModels.Pages
{
    internal interface IAreaConfigurationViewModel : IPageViewModelBase { }

    internal partial class AreaConfigurationViewModel : PageViewModelBase, IAreaConfigurationViewModel, IDisposable
    {
        private IPluginManager _pluginManager;

        [ObservableProperty]
        public partial bool IsLoadingLights { get; private set; } = true;

        [ObservableProperty]
        public partial LightInfoViewModel? SelectedLight { get; set; }

        public ObservableCollection<LightInfoViewModel> AvailableLights { get; init; } = [];
        
        public AreaConfigurationViewModel(IPluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        public override async Task OnLoadedAsync(CancellationToken cancellationToken = default)
        {
            await UpdateLightsAsync(cancellationToken);
            await base.OnLoadedAsync(cancellationToken);
        }

        public void Dispose()
        {

        }

        partial void OnSelectedLightChanged(LightInfoViewModel? value)
        {
            if (value == null)
                return;

            FlashLightAction flashLightAction = new();
            _ = flashLightAction.PerformActionAsync(default, value.AssociatedLightInfo);
        }

        private async Task UpdateLightsAsync(CancellationToken cancellationToken = default)
        {
            IsLoadingLights = true;

            ClearAvailableLights();

            var plugins = _pluginManager.Plugins;

            foreach (var plugin in plugins)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var availableLights = plugin.LightInfoSource.Lights;

                foreach (var light in availableLights)
                {
                    var lightVM = new LightInfoViewModel(light);
                    AvailableLights.Add(lightVM);
                }
            }

            IsLoadingLights = false;
        }

        private void ClearAvailableLights()
        {
            foreach (var lightVM in AvailableLights)
            {
                lightVM.Dispose();
            }

            AvailableLights.Clear();
        }
    }
}
