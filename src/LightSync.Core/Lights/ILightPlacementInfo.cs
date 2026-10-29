using LightSync.API.Lights;
using System.Numerics;

namespace LightSync.Core.Lights
{
    public interface ILightPlacementInfo
    {
        double RelativeCaptureRadius { get; set; }
        double RelativePositionX { get; set; }
        double RelativePositionY { get; set; }

        ILightInfo Light { get; }

    }
}
