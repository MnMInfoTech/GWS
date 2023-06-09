/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)
using System;

namespace MnM.GWS
{
    public struct Hsv
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
        public readonly float Value;

        public Hsv(float hue, float saturation, float value)
        {
            Hue = hue;
            Saturation = saturation;
            Value = value;
        }

                /// <summary>
        /// https://stackoverflow.com/questions/359612/how-to-change-rgb-colour-to-hsv
        /// </summary>
        /// <param name="rgba"></param>
        public static implicit operator Hsv(Rgba rgba)
        {
            int max = Math.Max(rgba.R, Math.Max(rgba.G, rgba.B));
            int min = Math.Min(rgba.R, Math.Min(rgba.G, rgba.B));

            var hue = rgba.Hue();
            var saturation = (max == 0) ? 0 : 1f - (1f * min / max);
            var value = max / 255f;
            return new Hsv(hue, saturation, value);
        }

        /// Source: http://en.wikipedia.org/wiki/HSL_and_HSV
        public static implicit operator Rgba(Hsv hsv)
        {
            float tint = hsv.Value * hsv.Saturation;
            float h1 = hsv.Hue / 60;
            float x = tint * (1 - Math.Abs(h1 % 2 - 1));
            float m = hsv.Value - tint;
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
            return new Rgba((r1 + m).Round(), (g1 + m).Round(), (b1 + m).Round());
        }
    }
}
#endif 
