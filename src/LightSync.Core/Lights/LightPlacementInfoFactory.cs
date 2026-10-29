using LightSync.API.Lights;
using System.Drawing;
using System.Numerics;

namespace LightSync.Core.Lights
{
    public class LightPlacementInfoFactory : ILightPlacementInfoFactory
    {
        public ILightPlacementInfo CreatePlacementInfo(ILightInfo light, double relativePositionX, double relativePositionY, double relativeCaptureRadius = 0.1) => new LightPlacementInfo(light, relativePositionX, relativePositionY, relativeCaptureRadius);
    }
}
