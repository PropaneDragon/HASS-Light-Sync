namespace LightSync.Core.Areas
{
    public interface IAreaDefinitionFactory
    {
        IAreaDefinition CreateEmptyArea(string? name = null);
    }
}
