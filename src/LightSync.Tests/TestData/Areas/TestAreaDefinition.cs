using LightSync.Core.Areas;
using LightSync.Core.Lights;

namespace LightSync.Tests.TestData.Areas
{
    internal class TestAreaDefinition : IAreaDefinition
    {
        public string Name { get; set; } = "Test area";

        public IReadOnlyList<ILightPlacementInfo> LightPlacementInfo => [];

        public bool AddPlacementInfo(ILightPlacementInfo lightPlacementInfo)
        {
            throw new NotImplementedException();
        }
    }
}
