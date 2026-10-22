using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.ViewModels.Dialogs
{
    internal interface IDialogViewModelBase
    {
        public Task OnLoadedAsync(CancellationToken cancellationToken = default);
        public Task OnUnloadedAsync(CancellationToken cancellationToken = default);

        void Cancel();
    }

    internal interface IDialogViewModelBase<TResult> : IDialogViewModelBase
    {
        TaskCompletionSource<TResult?> CompletedTask { get; }
    }

    internal abstract class DialogViewModelBase<TResult> : ObservableObject, IDialogViewModelBase<TResult>
    {
        public TaskCompletionSource<TResult?> CompletedTask { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public virtual void Cancel() => Close();
        public virtual async Task OnLoadedAsync(CancellationToken cancellationToken = default) { }
        public virtual async Task OnUnloadedAsync(CancellationToken cancellationToken = default) { }

        protected void Close(TResult? result = default) => CompletedTask.TrySetResult(result);
    }
}
