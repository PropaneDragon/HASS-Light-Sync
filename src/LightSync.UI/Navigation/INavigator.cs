using LightSync.UI.ViewModels;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.Navigation
{
    public interface INavigator
    {
        IViewModelBase? CurrentViewModel { get; }

        Task NavigateToAsync<TViewModel>(CancellationToken cancellationToken = default) where TViewModel : IViewModelBase;
    }
}
