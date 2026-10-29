namespace LightSync.Core.Areas
{
    public class AreaManager : IAreaManager
    {
        private readonly Lock _areaLock = new();
        private readonly List<IAreaDefinition> _areas = [];

        public IReadOnlyList<IAreaDefinition> Areas
        {
            get { lock(_areaLock) { return [.. _areas]; } }
        }

        public event EventHandler<AreaEventArgs>? AreaAdded;
        public event EventHandler<AreaEventArgs>? AreaRemoved;

        public AreaManager()
        {

        }

        public bool AddArea(IAreaDefinition areaDefinition)
        {
            lock (_areaLock)
            {
                if (_areas.Contains(areaDefinition))
                    return false;

                _areas.Add(areaDefinition);
            }

            AreaAdded?.Invoke(this, new(areaDefinition));

            return true;
        }

        public bool RemoveArea(IAreaDefinition areaDefinition)
        {
            lock (_areaLock)
            {
                if (!_areas.Contains(areaDefinition))
                    return false;

                _areas.Remove(areaDefinition);
            }

            AreaRemoved?.Invoke(this, new(areaDefinition));

            return true;
        }
    }
}
