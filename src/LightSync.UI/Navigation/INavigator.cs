using LightSync.UI.ViewModels.Pages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.Navigation
{
    internal interface INavigator
    {
        IPageViewModelBase? CurrentViewModel { get; }

        Task NavigateToAsync(Type viewModelType, CancellationToken cancellationToken = default);
        Task NavigateToAsync<TViewModel>(CancellationToken cancellationToken = default) where TViewModel : IPageViewModelBase;
    }
}
