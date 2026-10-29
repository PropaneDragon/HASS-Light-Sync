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
        public partial string Title { get; private set; } = string.Empty;

        [ObservableProperty]
        public partial IDialogViewModelBase? CurrentViewModel { get; private set; }

        public DialogPresenter(ILifetimeScope rootScope)
        {
            _rootScope = rootScope;
        }

        public async Task<DialogResult> ShowSimpleAsync(string title, string message, string? acceptText = null, string? rejectText = null, string? cancelText = null, string? closeText = null, AcceptButtonType acceptButtonType = AcceptButtonType.Accent) =>
            await ShowAsync<ISimpleDialogViewModel, DialogResult>(new SimpleDialogData(title, message, acceptText, rejectText, cancelText, closeText, acceptButtonType));

        public async Task<TResult?> ShowAsync<TViewModel, TResult>(object? context = null, CancellationToken cancellationToken = default) where TViewModel : IDialogViewModelBase<TResult>
        {
            if (IsOpen)
                CurrentViewModel?.Cancel();

            await using var scope = _rootScope.BeginLifetimeScope();
            var viewModel = scope.Resolve<TViewModel>();

            CurrentViewModel = viewModel;
            IsOpen = true;
            Title = CurrentViewModel.Title;

            await (CurrentViewModel.OnLoadedAsync(context, cancellationToken) ?? Task.CompletedTask);

            Title = CurrentViewModel.Title;

            try
            {
                return await viewModel.CompletedTask.Task;
            }
            finally
            {
                IsOpen = false;

                await (CurrentViewModel?.OnUnloadedAsync(cancellationToken) ?? Task.CompletedTask);
                await Task.Delay(TimeSpan.FromMilliseconds(200), cancellationToken); //Allow for any animations

                CurrentViewModel = null;
            }
        }
    }
}
