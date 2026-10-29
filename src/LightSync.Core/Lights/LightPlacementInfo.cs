using LightSync.API.Lights;
using System.Drawing;
using System.Numerics;

namespace LightSync.Core.Lights
{
    internal class LightPlacementInfo : ILightPlacementInfo
    {
        public double RelativeCaptureRadius { get; set; }
        public double RelativePositionX { get; set; }
        public double RelativePositionY { get; set; }

        public ILightInfo Light { get; }

        public LightPlacementInfo(ILightInfo lightInfo, double relativePositionX, double relativePositionY, double relativeCaptureRadius = 0.1)
        {
            Light = lightInfo;
            RelativePositionX = relativePositionX;
            RelativePositionY = relativePositionY;
            RelativeCaptureRadius = relativeCaptureRadius;
        }
    }
}
