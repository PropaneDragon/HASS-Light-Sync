using CommunityToolkit.Mvvm.ComponentModel;
using LightSync.Core.Areas;

namespace LightSync.UI.ViewModels.Areas
{
    internal partial class AreaDefinitionViewModel : ObservableObject
    {
        public IAreaDefinition AssociatedAreaDefinition { get; }

        public AreaDefinitionViewModel(IAreaDefinition areaDefinition)
        {
            AssociatedAreaDefinition = areaDefinition;
        }
    }
}
