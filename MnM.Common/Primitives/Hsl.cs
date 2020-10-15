/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
#if (GWS || Window)
    public struct Hsl
    {
        /// <summary>
        /// Range: 0 - 360
        /// </summary>
        public readonly float Hue;
        /// <summary>
        /// Range 0 -1
        /// </summary>
        public readonly float Saturation;
        /// <summary>
        /// Range 0 - 1
        /// </summary>
        public readonly float Lightness;

        public Hsl(float hue, float saturation, float lightness)
        {
            Hue = hue;
            Saturation = saturation;
            Lightness = lightness;
        }

        /// Source: http://en.wikipedia.org/wiki/HSL_and_HSV
        public static implicit operator Rgba(Hsl hsl)
        {
            float tint = (1 - Math.Abs(2 * hsl.Lightness - 1)) * hsl.Saturation;
            float h1 = hsl.Hue / 60;
            float x = tint * (1 - Math.Abs(h1 % 2 - 1));
            float m = hsl.Lightness - 0.5f * tint;
            float r1, g1, b1;

            if (h1 < 1)
            {
                r1 = tint;
                g1 = x;
                b1 = 0;
            }
            else if (h1 < 2)
            {
                r1 = x;
                g1 = tint;
                b1 = 0;
            }
            else if (h1 < 3)
            {
                r1 = 0;
                g1 = tint;
                b1 = x;
            }
            else if (h1 < 4)
            {
                r1 = 0;
                g1 = x;
                b1 = tint;
            }
            else if (h1 < 5)
            {
                r1 = x;
                g1 = 0;
                b1 = tint;
            }
            else //if (h1 < 6)
            {
                r1 = tint;
                g1 = 0;
                b1 = x;
            }

            return new Rgba(r1 + m, g1 + m, b1 + m);
        }
    }
#endif
}
