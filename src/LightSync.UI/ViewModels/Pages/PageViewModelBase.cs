using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.ViewModels.Pages
{
    internal interface IPageViewModelBase : IViewLocatableViewModel
    {
        public Task OnLoadedAsync(object? context = null, CancellationToken cancellationToken = default);
        public Task OnUnloadedAsync(CancellationToken cancellationToken = default);
    }

    internal abstract class PageViewModelBase : ObservableObject, IPageViewModelBase
    {
        public virtual async Task OnLoadedAsync(object? context = null, CancellationToken cancellationToken = default) { }
        public virtual async Task OnUnloadedAsync(CancellationToken cancellationToken = default) { }
    }
}
