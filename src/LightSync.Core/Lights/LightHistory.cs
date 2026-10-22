using LightSync.API.Lights;
using System.Drawing;

namespace LightSync.Core.Lights
{
    public class LightHistory : IDisposable
    {
        private readonly TimeSpan _restoreTransitionTime;
        private readonly List<LightState> _states = [];

        public enum UpdateRule
        {
            UpdateExisting,
            IgnoreExisting
        }

        public LightHistory(params ILightInfo[] lightInfo)
        {
            _states = [.. lightInfo.Select(lightInfo => new LightState(lightInfo))];
        }

        public LightHistory(TimeSpan restoreTransitionTime, params ILightInfo[] lightInfo) : this(lightInfo)
        {
            _restoreTransitionTime = restoreTransitionTime;
        }

        public static LightHistory CreateStored(params ILightInfo[] lightInfo) => new(lightInfo);
        public static LightHistory CreateStored(TimeSpan restoreTransitionTime, params ILightInfo[] lightInfo) => new(restoreTransitionTime, lightInfo);

        public void Store(UpdateRule updateRule = UpdateRule.UpdateExisting, params ILightInfo[] lights)
        {
            foreach (var light in lights)
            {
                var existingState = _states.FirstOrDefault(state => state.LightInfo == light);
                if (existingState != null && updateRule == UpdateRule.IgnoreExisting)
                    continue;

                if (existingState != null)
                    _states.Remove(existingState);

                _states.Add(new(light));
            }
        }

        public void Restore() => Restore(_restoreTransitionTime);
        public void Restore(TimeSpan transitionTime = default)
        {
            foreach (var state in _states)
            {
                var lightInfo = state.LightInfo;

                if (state.On)
                {
                    _ = lightInfo.SetColourAsync(state.Colour, transitionTime);
                    continue;
                }
                else
                {
                    _ = lightInfo.SetOnAsync(false, transitionTime);
                    continue;
                }
            }
        }

        public void Dispose()
        {
            Restore();
        }

        private record LightState
        {
            public bool On { get; }
            public Color Colour { get; }

            public ILightInfo LightInfo { get; }

            public LightState(ILightInfo lightInfo)
            {
                LightInfo = lightInfo;
                On = lightInfo.IsOn;
                Colour = lightInfo.Colour;
            }
        }
    }
}
