using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.ViewModels
{
    public interface IViewModelBase
    {
        public Task OnLoadedAsync(CancellationToken cancellationToken = default);
        public Task OnUnloadedAsync(CancellationToken cancellationToken = default);
    }

    public abstract class ViewModelBase : ObservableObject, IViewModelBase
    {
        public virtual async Task OnLoadedAsync(CancellationToken cancellationToken = default) { }
        public virtual async Task OnUnloadedAsync(CancellationToken cancellationToken = default) { }
    }
}
