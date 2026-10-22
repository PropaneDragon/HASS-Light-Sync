using Autofac;
using CommunityToolkit.Mvvm.ComponentModel;
using LightSync.UI.ViewModels.Dialogs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.Dialogs
{
    internal partial class DialogPresenter : ObservableObject, IDialogService
    {
        private readonly ILifetimeScope _rootScope;

        [ObservableProperty]
        public partial bool IsOpen { get; private set; }

        [ObservableProperty]
        public partial IDialogViewModelBase? CurrentViewModel { get; private set; }

        public DialogPresenter(ILifetimeScope rootScope)
        {
            _rootScope = rootScope;
        }

        public async Task<TResult?> ShowAsync<TViewModel, TResult>(CancellationToken cancellationToken = default) where TViewModel : IDialogViewModelBase<TResult>
        {
            if (IsOpen)
                CurrentViewModel?.Cancel();

            await using var scope = _rootScope.BeginLifetimeScope();
            var viewModel = scope.Resolve<TViewModel>();

            CurrentViewModel = viewModel;
            IsOpen = true;

            await (CurrentViewModel?.OnLoadedAsync(cancellationToken) ?? Task.CompletedTask);

            try
            {
                return await viewModel.CompletedTask.Task;
            }
            finally
            {
                IsOpen = false;

                await (CurrentViewModel?.OnUnloadedAsync(cancellationToken) ?? Task.CompletedTask);
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken); //Allow for any animations

                CurrentViewModel = null;
            }
        }
    }
}
