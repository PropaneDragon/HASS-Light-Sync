using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightSync.UI.Navigation;
using LightSync.UI.Plugins;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.ViewModels.Pages
{
    internal interface IHomePageViewModel : IViewModelBase { }

    internal partial class HomePageViewModel : ViewModelBase, IHomePageViewModel
    {
        private readonly INavigator _navigator;
        private readonly IPluginManager _pluginManager;

        [ObservableProperty]
        public partial bool IsLoadingPlugins { get; private set; } = true;

        public HomePageViewModel(INavigator navigator, IPluginManager pluginManager)
        {
            _navigator = navigator;
            _pluginManager = pluginManager;
        }

        public override async Task OnLoadedAsync(CancellationToken cancellationToken = default)
        {
            await LoadPlugins(cancellationToken);
            await base.OnLoadedAsync(cancellationToken);
        }

        private async Task LoadPlugins(CancellationToken cancellationToken = default)
        {
            IsLoadingPlugins = true;

            await _pluginManager.LoadPluginsAsync(cancellationToken);

            var plugins = _pluginManager.Plugins;
            foreach (var plugin in plugins)
            {
                await Task.Run(async () => await plugin.InitialiseAsync(cancellationToken));
                await Task.Run(async () => await plugin.LightInfoSource.InitialiseAsync(cancellationToken), cancellationToken);
            }

            IsLoadingPlugins = false;
        }

        [RelayCommand]
        private async Task NavigateToConfigureArea(CancellationToken cancellationToken = default)
        {
            await _navigator.NavigateToAsync<IAreaConfigurationViewModel>(cancellationToken);
        }
    }
}
