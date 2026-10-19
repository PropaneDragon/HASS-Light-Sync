using CommunityToolkit.Mvvm.ComponentModel;
using LightSync.UI.Plugins;
using LightSync.UI.ViewModels.Lights;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.ViewModels.Pages
{
    internal interface IAreaConfigurationViewModel : IViewModelBase { }

    internal partial class AreaConfigurationViewModel : ViewModelBase, IAreaConfigurationViewModel, IDisposable
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
            _ = value?.AssociatedLightInfo.SetColourAsync(System.Drawing.Color.Aqua);
        }

        private async Task UpdateLightsAsync(CancellationToken cancellationToken = default)
        {
            IsLoadingLights = true;

            ClearAvailableLights();

            var plugins = _pluginManager.Plugins;

            foreach (var plugin in plugins)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var availableLights = await Task.Run(async () => await plugin.LightInfoSource.GetAvailableLightsAsync(cancellationToken), cancellationToken);

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
