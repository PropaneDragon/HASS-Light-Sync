namespace LightSync.Core.Areas
{
    public class AreaDefinitionFactory : IAreaDefinitionFactory
    {
        public IAreaDefinition CreateEmptyArea(string? name = null) => new AreaDefinition(name ?? "New area");
    }
}
