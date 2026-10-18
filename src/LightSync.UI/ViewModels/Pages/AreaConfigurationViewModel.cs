using CommunityToolkit.Mvvm.ComponentModel;
using LightSync.UI.Plugins;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.ViewModels.Pages
{
    internal interface IAreaConfigurationViewModel : IViewModelBase { }

    internal partial class AreaConfigurationViewModel : ViewModelBase, IAreaConfigurationViewModel
    {
        private IPluginManager _pluginManager;

        [ObservableProperty]
        public partial bool IsLoadingLights { get; private set; } = true;

        public AreaConfigurationViewModel(IPluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        public override async Task OnLoadedAsync(CancellationToken cancellationToken = default)
        {
            IsLoadingLights = true;

            var plugins = _pluginManager.Plugins;

            foreach (var plugin in plugins)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var availableLights = await Task.Run(async () => await plugin.LightInfoSource.GetAvailableLightsAsync(cancellationToken), cancellationToken);
            }            

            IsLoadingLights = false;

            await base.OnLoadedAsync(cancellationToken);
        }
    }
}
