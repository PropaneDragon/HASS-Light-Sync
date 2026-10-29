using LightSync.UI.ViewModels.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.Dialogs
{
    internal interface IDialogService
    {
        public bool IsOpen { get; }
        public string Title { get; }

        public IDialogViewModelBase? CurrentViewModel { get; }

        Task<TResult?> ShowAsync<TViewModel, TResult>(object? context = null, CancellationToken cancellationToken = default) where TViewModel : IDialogViewModelBase<TResult>;
        Task<DialogResult> ShowSimpleAsync(string title, string message, string? acceptText = null, string? rejectText = null, string? cancelText = null, string? closeText = null, AcceptButtonType acceptButtonType = AcceptButtonType.Accent);
    }
}
