using LightSync.Core.Lights;

namespace LightSync.Core.Areas
{
    internal class AreaDefinition : IAreaDefinition
    {
        private readonly Lock _nameLock = new();
        private readonly Lock _lightPlacementInfoLock = new();
        private readonly List<ILightPlacementInfo> _lightPlacementInfos = [];

        private string _name;

        public string Name
        {
            get { lock(_nameLock) { return _name; } }
            set { lock(_nameLock) { _name = value; } }
        }

        public IReadOnlyList<ILightPlacementInfo> LightPlacementInfo
        {
            get { lock(_lightPlacementInfoLock) { return [.. _lightPlacementInfos]; } }
        }

        public AreaDefinition(string name)
        {
            _name = name;
        }

        public bool AddPlacementInfo(ILightPlacementInfo lightPlacementInfo)
        {
            lock (_lightPlacementInfoLock)
            {
                if (_lightPlacementInfos.Contains(lightPlacementInfo))
                    return false;

                _lightPlacementInfos.Add(lightPlacementInfo);
            }

            return true;
        }
    }
}
