using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HarfBuzzSharp;
using LightSync.Core.Areas;
using LightSync.Core.Lights;
using LightSync.Core.Progress;
using LightSync.UI.Behaviours;
using LightSync.UI.Dialogs;
using LightSync.UI.Navigation;
using LightSync.UI.Plugins;
using LightSync.UI.ViewModels.Dialogs;
using LightSync.UI.ViewModels.Lights;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LightSync.UI.ViewModels.Pages
{
    internal interface IEditAreaPageViewModel : IPageViewModelBase { }

    internal partial class EditAreaPageViewModel : PageViewModelBase, IEditAreaPageViewModel
    {
        private readonly IPluginManager _pluginManager;
        private readonly IProgressInfoFactory _progressInfoFactory;
        private readonly ILightPlacementInfoFactory _lightPlacementInfoFactory;
        private readonly IDialogService _dialogService;
        private readonly INavigator _navigator;
        private readonly IAreaManager _areaManager;

        private IAreaDefinition? _areaDefinition;

        [ObservableProperty]
        public partial string? NameFilter { get; set; }

        [ObservableProperty]
        public partial IReadOnlyList<LightEditorLightInfoViewModel> Lights { get; private set; } = [];

        [ObservableProperty]
        public partial IReadOnlyList<LightEditorLightInfoViewModel> FilteredLights { get; private set; } = [];

        public ObservableCollection<LightEditorLightInfoViewModel> PlacedLights { get; } = [];

        public EditAreaPageViewModel(
            IPluginManager pluginManager,
            IProgressInfoFactory progressInfoFactory,
            ILightPlacementInfoFactory lightPlacementInfoFactory,
            IDialogService dialogService,
            INavigator navigator,
            IAreaManager areaManager)
        {
            _pluginManager = pluginManager;
            _progressInfoFactory = progressInfoFactory;
            _lightPlacementInfoFactory = lightPlacementInfoFactory;
            _dialogService = dialogService;
            _navigator = navigator;
            _areaManager = areaManager;

            SynchroniseLightsFromPlugins();
        }

        public override async Task OnLoadedAsync(object? context = null, CancellationToken cancellationToken = default)
        {
            if (context is not IAreaDefinition areaDefinition)
            {
                await _navigator.NavigateToAsync<IAreasPageViewModel>(null, cancellationToken);
                return;
            }

            _areaDefinition = areaDefinition;

            UpdateFromAreaDefinition(areaDefinition);

            await base.OnLoadedAsync(context, cancellationToken);
        }

        private void UpdateFromAreaDefinition(IAreaDefinition areaDefinition)
        {
            foreach (var lightPlacement in areaDefinition.LightPlacementInfo)
            {
                var foundLight = Lights.FirstOrDefault(light => light.AssociatedLightInfo == lightPlacement.Light);
                if (foundLight == null)
                    continue;

                if (PlacedLights.Contains(foundLight))
                    continue;

                foundLight.SetLightPlacement(lightPlacement);

                PlacedLights.Add(foundLight);
            }

            RecountLights();
            UpdateFilter();
        }

        private void UpdateDefinition()
        {
            if (_areaDefinition == null)
                return;

            foreach (var placedLight in PlacedLights)
            {
                if (placedLight.AssociatedLightPlacementInfo == null)
                    continue;

                _areaDefinition.AddPlacementInfo(placedLight.AssociatedLightPlacementInfo);
            }
        }

        private void SynchroniseLightsFromPlugins()
        {
            Lights = [.. _pluginManager.LightPluugins.SelectMany(plugin => plugin.LightInfoSource.Lights).Select(light => new LightEditorLightInfoViewModel(light))];

            UpdateFilter();
        }

        private void UpdateFilter()
        {
            FilteredLights = [.. Lights.Where(LightIsIncludedInFilter).OrderBy(light => light.HasLightPlacement ? light.LightPlacementIndex : int.MaxValue).ThenBy(light => light.AssociatedLightInfo.Name)];
        }

        private void RecountLights()
        {
            foreach (var light in Lights)
            {
                var lightIndex = PlacedLights.IndexOf(light);
                light.LightPlacementIndex = lightIndex >= 0 ? lightIndex + 1 : lightIndex;
            }
        }

        private bool LightIsIncludedInFilter(LightEditorLightInfoViewModel light)
        {
            return
                !string.IsNullOrWhiteSpace(light.AssociatedLightInfo.Name) &&
                (string.IsNullOrWhiteSpace(NameFilter) || light.AssociatedLightInfo.Name.Contains(NameFilter, StringComparison.CurrentCultureIgnoreCase));
        }

        private async Task<bool> ConfirmSaveAsync()
        {
            var result = await _dialogService.ShowSimpleAsync("Save changes", "This will discard any recent changes you have made. Are you sure you want to continue?", "Save", "Don't save", "Cancel");
            if (result == DialogResult.Cancel)
                return false;

            if (result == DialogResult.Reject)
                return true;

            return true;
        }

        partial void OnNameFilterChanged(string? value) => UpdateFilter();

        [RelayCommand]
        private async Task ReloadLights(CancellationToken cancellationToken)
        {
            using var progressInfo = _progressInfoFactory.CreateAndShowProgressInfo("Refreshing lights");

            var plugins = _pluginManager.Plugins;
            if (plugins.Count <= 0)
                return;

            SynchroniseLightsFromPlugins();
        }

        [RelayCommand]
        private void LightDropped(object data)
        {
            if (data is not DropInfo dropInfo)
                return;

            if (dropInfo.Data is not LightEditorLightInfoViewModel lightInfo)
                return;

            if (dropInfo.Target == null)
                return;

            var bounds = dropInfo.Target.Bounds;
            var relativePosition = new Point(dropInfo.Position.X / bounds.Width, dropInfo.Position.Y / bounds.Height);

            if (lightInfo.LightPlacementInfo != null)
            {
                lightInfo.LightPlacementInfo.RelativePositionX = relativePosition.X;
                lightInfo.LightPlacementInfo.RelativePositionY = relativePosition.Y;
            }
            else
            {
                var lightPlacementInfo = _lightPlacementInfoFactory.CreatePlacementInfo(lightInfo.AssociatedLightInfo, relativePosition.X, relativePosition.Y);
                lightInfo.SetLightPlacement(lightPlacementInfo);
                PlacedLights.Add(lightInfo);
                RecountLights();
                UpdateFilter();
            }
        }

        [RelayCommand]
        private async Task SaveAreaDefinition()
        {
            if (_areaDefinition == null)
                return;

            UpdateDefinition();

            _areaManager.AddArea(_areaDefinition);

            await _navigator.NavigateToAsync<IAreasPageViewModel>();
        }

        [RelayCommand]
        private async Task DeleteAreaDefinition()
        {
            var result = await _dialogService.ShowSimpleAsync("Delete area", "Are you sure you want to delete the area?", "Delete", "Don't delete", "Cancel", acceptButtonType: AcceptButtonType.Danger);
            if (result != DialogResult.Accept)
                return;

            if (_areaDefinition == null)
                return;

            _areaManager.RemoveArea(_areaDefinition);

            await _navigator.NavigateToAsync<IAreasPageViewModel>();
        }

        [RelayCommand]
        private async Task GoBack()
        {
            if (!await ConfirmSaveAsync())
                return;

            await _navigator.NavigateToAsync<IAreasPageViewModel>();
        }
    }
}
