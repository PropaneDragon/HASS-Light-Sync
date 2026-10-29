using CommunityToolkit.Mvvm.ComponentModel;
using LightSync.API.Plugins;
using LightSync.Core.Progress;
using LightSync.UI.Dialogs;
using LightSync.UI.Navigation;
using LightSync.UI.Plugins;
using LightSync.UI.ViewModels.Pages;
using LightSync.UI.ViewModels.Progress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.ViewModels
{
    internal interface IMainWindowViewModel : IPageViewModelBase { }

    internal partial class MainWindowViewModel : PageViewModelBase, IMainWindowViewModel
    {
        private readonly IPluginManager _pluginManager;
        private readonly IProgressInfoFactory _progressInfoFactory;

        private readonly List<MainWindowPage> _pages =
        [
            new("Areas", "fa7-bolt", typeof(IAreasPageViewModel)),
            new("Sources", "fa7-eye", typeof(ISourcesPageViewModel)),
            new("Lights", "fa7-lightbulb", typeof(ILightsPageViewModel)),
            new("Settings", "fa7-gears", typeof(ISettingsPageViewModel))
        ];

        public INavigator Navigator { get; }
        public IDialogService DialogService { get; }
        public ProgressServiceViewModel ProgressService { get; }

        public IReadOnlyList<MainWindowPage> Pages => _pages;

        [ObservableProperty]
        public partial MainWindowPage CurrentPage { get; set; }

        public MainWindowViewModel(
            INavigator navigator,
            IDialogService dialogService,
            IProgressService progressService,
            IPluginManager pluginManager,
            IProgressInfoFactory progressInfoFactory)
        {
            _pluginManager = pluginManager;
            _progressInfoFactory = progressInfoFactory;

            Navigator = navigator;
            DialogService = dialogService;
            ProgressService = new(progressService);
            CurrentPage = _pages.First();
        }

        public override async Task OnLoadedAsync(object? context = null, CancellationToken cancellationToken = default)
        {
            using var progressInfo = _progressInfoFactory.CreateAndShowProgressInfo("Finding plugins");

            await _pluginManager.LoadPluginsAsync(cancellationToken);

            progressInfo.Message = "Loading plugins";
            progressInfo.IsIndeterminate = false;

            var plugins = _pluginManager.Plugins;
            if (plugins.Count > 0)
            {
                var pluginProgressStep = 100d / plugins.Count;

                foreach (var plugin in _pluginManager.Plugins)
                {
                    _ = await plugin.InitialiseAsync(cancellationToken);

                    progressInfo.ProgressPercentage += pluginProgressStep;
                }
            }

            await base.OnLoadedAsync(context, cancellationToken);
        }

        partial void OnCurrentPageChanged(MainWindowPage oldValue, MainWindowPage newValue)
        {
            if (oldValue == newValue)
                return;

            _ = Navigator.NavigateToAsync(newValue.ViewModelType);
        }
    }

    internal record MainWindowPage(string Title, string Icon, Type ViewModelType)
    {
    }
}
