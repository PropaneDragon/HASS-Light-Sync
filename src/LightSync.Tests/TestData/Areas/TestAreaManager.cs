using LightSync.Core.Areas;

namespace LightSync.Tests.TestData.Areas
{
    public class TestAreaManager : IAreaManager
    {
        public IReadOnlyList<IAreaDefinition> Areas { get; } = [
            new TestAreaDefinition()
        ];

        public event EventHandler<AreaEventArgs>? AreaAdded;
        public event EventHandler<AreaEventArgs>? AreaRemoved;

        public bool AddArea(IAreaDefinition areaDefinition)
        {
            throw new NotImplementedException();
        }

        public bool RemoveArea(IAreaDefinition areaDefinition)
        {
            throw new NotImplementedException();
        }
    }
}
