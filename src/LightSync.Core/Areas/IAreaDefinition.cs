using LightSync.Core.Lights;

namespace LightSync.Core.Areas
{
    public interface IAreaDefinition
    {
        public string Name { get; set; }
        IReadOnlyList<ILightPlacementInfo> LightPlacementInfo { get; }

        bool AddPlacementInfo(ILightPlacementInfo lightPlacementInfo);
    }
}
