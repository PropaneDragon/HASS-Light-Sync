using LightSync.UI.ViewModels.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.Dialogs
{
    internal interface IDialogService
    {
        public bool IsOpen { get; }

        public IDialogViewModelBase? CurrentViewModel { get; }

        Task<TResult?> ShowAsync<TViewModel, TResult>(CancellationToken cancellationToken = default) where TViewModel : IDialogViewModelBase<TResult>;
    }
}
