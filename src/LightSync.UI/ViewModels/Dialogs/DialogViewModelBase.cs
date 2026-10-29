using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.ViewModels.Dialogs
{
    internal interface IDialogViewModelBase : IViewLocatableViewModel
    {
        public string Title { get; }

        public Task OnLoadedAsync(object? context = null, CancellationToken cancellationToken = default);
        public Task OnUnloadedAsync(CancellationToken cancellationToken = default);

        void Cancel();
    }

    internal interface IDialogViewModelBase<TResult> : IDialogViewModelBase
    {
        TaskCompletionSource<TResult?> CompletedTask { get; }
    }

    internal abstract partial class DialogViewModelBase<TResult> : ObservableObject, IDialogViewModelBase<TResult>
    {
        [ObservableProperty]
        public partial string Title { get; protected set; } = string.Empty;

        public TaskCompletionSource<TResult?> CompletedTask { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public virtual void Cancel() => Close();
        public virtual async Task OnLoadedAsync(object? context = null, CancellationToken cancellationToken = default) { }
        public virtual async Task OnUnloadedAsync(CancellationToken cancellationToken = default) { }

        protected void Close(TResult? result = default) => CompletedTask.TrySetResult(result);
    }
}
