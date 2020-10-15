/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
#if (GWS || Window)
    public struct Cmyk
    {
        public readonly float C;
        public readonly float M;
        public readonly float Y;
        public readonly float K;

        public Cmyk(float c, float m, float y, float k)
        {
            C = Math.Max(c, 0);
            M = Math.Max(m, 0);
            Y = Math.Max(y, 0);
            K = Math.Max(k, 0);
        }

        /// <summary>
        /// https://www.cyotek.com/blog/converting-colours-between-rgb-and-cmyk-in-csharp
        /// </summary>
        /// <param name="cmyk"></param>
        public static implicit operator Rgba(Cmyk cmyk)
        {
            int r;
            int g;
            int b;
            var kFactor = 1 - cmyk.K;

            r = (int)(255 * (1 - cmyk.C) * kFactor);
            g = (int)(255 * (1 - cmyk.M) * kFactor);
            b = (int)(255 * (1 - cmyk.Y) * kFactor);
            return new Rgba(r, g, b);
        }
    }
#endif
}
