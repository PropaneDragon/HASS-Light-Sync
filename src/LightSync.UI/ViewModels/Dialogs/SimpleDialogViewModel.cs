using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.ViewModels.Dialogs
{
    internal enum DialogResult
    {
        Accept,
        Reject,
        Cancel,
        Close
    }

    internal enum AcceptButtonType
    {
        Accent,
        Danger
    }

    internal interface ISimpleDialogViewModel : IDialogViewModelBase<DialogResult> { }

    internal partial class SimpleDialogViewModel : DialogViewModelBase<DialogResult>, ISimpleDialogViewModel
    {
        [ObservableProperty]
        public partial string Message { get; private set; } = string.Empty;

        [ObservableProperty]
        public partial bool IsDanger { get; private set; }

        [ObservableProperty]
        public partial string? AcceptText { get; private set; } = string.Empty;
        [ObservableProperty]
        public partial string? RejectText { get; private set; } = string.Empty;
        [ObservableProperty]
        public partial string? CancelText { get; private set; } = string.Empty;
        [ObservableProperty]
        public partial string? CloseText { get; private set; } = string.Empty;

        public SimpleDialogViewModel()
        {
        }

        public override async Task OnLoadedAsync(object? context = null, CancellationToken cancellationToken = default)
        {
            if (context is not SimpleDialogData dialogData)
                return;

            Title = dialogData.Title;
            Message = dialogData.Message;

            AcceptText = dialogData.AcceptText;
            RejectText = dialogData.RejectText;
            CancelText = dialogData.CancelText;
            CloseText = dialogData.CloseText;

            IsDanger = dialogData.AcceptButtonType == AcceptButtonType.Danger;

            if (AcceptText == null && RejectText == null && CancelText == null && CloseText == null)
                CloseText = "Close";

            await base.OnLoadedAsync(context, cancellationToken);
        }

        [RelayCommand]
        private void AcceptButton() => Close(DialogResult.Accept);
        [RelayCommand]
        private void RejectButton() => Close(DialogResult.Reject);
        [RelayCommand]
        private void CancelButton() => Close(DialogResult.Cancel);
        [RelayCommand]
        private void CloseButtonButton() => Close(DialogResult.Close);
    }

    internal record SimpleDialogData(string Title, string Message, string? AcceptText = null, string? RejectText = null, string? CancelText = null, string? CloseText = null, AcceptButtonType AcceptButtonType = AcceptButtonType.Accent) { }
}
