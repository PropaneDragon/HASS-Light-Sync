using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using LightSync.UI.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.Navigation
{
    internal partial class Navigator : ObservableObject, INavigator, IDisposable
    {
        private ILifetimeScope _rootScope;
        private ILifetimeScope? _pageScope;

        [ObservableProperty]
        public partial IViewModelBase? CurrentViewModel { get; private set; }

        public Navigator(ILifetimeScope root)
        {
            _rootScope = root;
        }

        public async Task NavigateToAsync<TViewModel>(CancellationToken cancellationToken = default) where TViewModel : IViewModelBase
        {
            await (CurrentViewModel?.OnUnloadedAsync(cancellationToken) ?? Task.CompletedTask);

            var pageScope = _rootScope.BeginLifetimeScope();
            var viewModel = pageScope.Resolve<TViewModel>();

            CurrentViewModel = viewModel;

            _pageScope?.Dispose();
            _pageScope = pageScope;

            await CurrentViewModel.OnLoadedAsync(cancellationToken);
        }

        public void Dispose()
        {
            _pageScope?.Dispose();
        }
    }
}
