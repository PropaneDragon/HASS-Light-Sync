namespace LightSync.Core.Areas
{
    public interface IAreaManager
    {
        IReadOnlyList<IAreaDefinition> Areas { get; }

        event EventHandler<AreaEventArgs>? AreaAdded;
        event EventHandler<AreaEventArgs>? AreaRemoved;

        bool AddArea(IAreaDefinition areaDefinition);
        bool RemoveArea(IAreaDefinition areaDefinition);
    }

    public sealed class AreaEventArgs(IAreaDefinition areaDefinition) : EventArgs
    {
        public IAreaDefinition AreaDefinition { get; } = areaDefinition;
    }
}
