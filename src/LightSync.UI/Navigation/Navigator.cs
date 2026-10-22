using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using LightSync.UI.ViewModels.Pages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.Navigation
{
    internal partial class Navigator : ObservableObject, INavigator, IDisposable
    {
        private readonly ILifetimeScope _rootScope;

        private ILifetimeScope? _pageScope;

        [ObservableProperty]
        public partial IPageViewModelBase? CurrentViewModel { get; private set; }

        public Navigator(ILifetimeScope root)
        {
            _rootScope = root;
        }

        public async Task NavigateToAsync(Type viewModelType, CancellationToken cancellationToken = default) => await NavigateToAsync(scope => (IPageViewModelBase)scope.Resolve(viewModelType), cancellationToken);
        public async Task NavigateToAsync<TViewModel>(CancellationToken cancellationToken = default) where TViewModel : IPageViewModelBase => await NavigateToAsync(scope => scope.Resolve<TViewModel>(), cancellationToken);

        public void Dispose()
        {
            _pageScope?.Dispose();
        }

        private async Task NavigateToAsync(Func<ILifetimeScope, IPageViewModelBase> viewModelFunc, CancellationToken cancellationToken = default)
        {
            await (CurrentViewModel?.OnUnloadedAsync(cancellationToken) ?? Task.CompletedTask);

            var pageScope = _rootScope.BeginLifetimeScope();
            var viewModel = viewModelFunc(pageScope);

            CurrentViewModel = viewModel;

            _pageScope?.Dispose();
            _pageScope = pageScope;

            await CurrentViewModel.OnLoadedAsync(cancellationToken);
        }
    }
}
