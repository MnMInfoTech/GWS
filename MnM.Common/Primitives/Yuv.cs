/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
#if (GWS || Window)
    public struct Yuv
    {
        public readonly float Y;
        public readonly float U;
        public readonly float V;

        public Yuv(float y, float u, float v)
        {
            Y = y;
            U = u;
            V = v;
        }

        public bool Equals(Yuv yuv)
        {
            return (this.Y == yuv.Y) && (this.U == yuv.U) && (this.V == yuv.V);
        }

        //Source: https://github.com/DavidSM64/N64-YUV2RGB-Library/blob/master/C%23/N64YUV2RGB.cs
        public static implicit operator Rgba(Yuv yuv)
        {
            var r = ((int)Math.Round(1.164f * (yuv.Y - 16) + 1.596f * (yuv.V - 128)));
            var g = ((int)Math.Round(1.164f * (yuv.Y - 16) - 0.813f * (yuv.V - 128) - 0.391f * (yuv.U - 128)));
            var b = ((int)Math.Round(1.164f * (yuv.Y - 16) + 2.018f * (yuv.U - 128)));
            return new Rgba(r, g, b);
        }
    }
#endif
}
