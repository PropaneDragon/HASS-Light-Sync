using LightSync.API.Lights;
using System.Numerics;

namespace LightSync.Core.Lights
{
    public interface ILightPlacementInfoFactory
    {
        ILightPlacementInfo CreatePlacementInfo(ILightInfo light, double relativePositionX, double relativePositionY, double relativeCaptureRadius = 0.1);
    }
}
