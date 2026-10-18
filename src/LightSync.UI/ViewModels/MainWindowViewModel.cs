using LightSync.UI.Navigation;
using LightSync.UI.ViewModels.Pages;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.ViewModels
{
    internal interface IMainWindowViewModel : IViewModelBase { }

    internal partial class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        public INavigator Navigator { get; }

        public MainWindowViewModel(INavigator navigator)
        {
            Navigator = navigator;
        }

        public override async Task OnLoadedAsync(CancellationToken cancellationToken = default)
        {
            await Navigator.NavigateToAsync<IHomePageViewModel>(cancellationToken);
            await base.OnLoadedAsync(cancellationToken);
        }
    }
}
