using CommunityToolkit.Mvvm.Input;
using LightSync.Core.Areas;
using LightSync.UI.Collections;
using LightSync.UI.Navigation;
using LightSync.UI.ViewModels.Areas;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.ViewModels.Pages
{
    internal interface IAreasPageViewModel : IPageViewModelBase { }

    internal partial class AreasPageViewModel : PageViewModelBase, IAreasPageViewModel
    {
        private readonly INavigator _navigator;
        private readonly IAreaDefinitionFactory _areaDefinitionFactory;
        private readonly IAreaManager _areaManager;

        public ObservableCollection<AreaDefinitionViewModel> Areas { get; }
        public TrailingObservableCollection<AreaDefinitionViewModel> AreasWithPlaceholder { get; }

        public AreasPageViewModel(INavigator navigator, IAreaDefinitionFactory areaDefinitionFactory, IAreaManager areaManager)
        {
            _navigator = navigator;
            _areaDefinitionFactory = areaDefinitionFactory;
            _areaManager = areaManager;

            Areas = [.. areaManager.Areas.Select(area => new AreaDefinitionViewModel(area))];
            AreasWithPlaceholder = new(Areas, new CreateAreaPlaceholder(AddNewArea));
        }

        public override async Task OnLoadedAsync(object? context = null, CancellationToken cancellationToken = default)
        {
            await base.OnLoadedAsync(context, cancellationToken);
        }

        [RelayCommand]
        private async Task AddNewArea()
        {
            var area = _areaDefinitionFactory.CreateEmptyArea();
            await _navigator.NavigateToAsync<IEditAreaPageViewModel>(area);
        }

        [RelayCommand]
        private async Task EditArea(AreaDefinitionViewModel? areaDefinition)
        {
            if (areaDefinition == null)
                return;

            await _navigator.NavigateToAsync<IEditAreaPageViewModel>(areaDefinition.AssociatedAreaDefinition);
        }
    }

    internal partial class CreateAreaPlaceholder(Func<Task> addFunc)
    {
        private readonly Func<Task> _addFunc = addFunc;

        [RelayCommand]
        private async Task AddNewArea()
        {
            await _addFunc.Invoke();
        }
    }
}
