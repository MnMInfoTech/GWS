/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)
using System;

namespace MnM.GWS
{
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

        /// <summary>
        /// http://csharphelper.com/blog/2016/08/convert-between-rgb-and-hls-colour-models-in-c/
        /// </summary>
        /// <param name="rgba"></param>
        public static implicit operator Hsl(Rgba rgba)
        {
            float double_r = rgba.R;
            float double_g = rgba.G;
            float double_b = rgba.B;

            // Get the maximum and minimum RGB components.
            float max = double_r;
            if (max < double_g) max = double_g;
            if (max < double_b) max = double_b;

            float min = double_r;
            if (min > double_g) min = double_g;
            if (min > double_b) min = double_b;

            float diff = max - min;
            var l = (max + min) / 2;
            float h, s;


            if (Math.Abs(diff) < 0.00001)
            {
                s = 0;
                h = 0;
            }
            else
            {
                if (l <= 0.5) s = diff / (max + min);
                else s = diff / (2 - max - min);

                float r_dist = (max - double_r) / diff;
                float g_dist = (max - double_g) / diff;
                float b_dist = (max - double_b) / diff;

                if (double_r == max) h = b_dist - g_dist;
                else if (double_g == max) h = 2 + r_dist - b_dist;
                else h = 4 + g_dist - r_dist;

                h = h * 60;
                if (h < 0) h += 360;
            }
            return new Hsl(h, s, l);
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

            return new Rgba((r1 + m).Round(), (g1 + m).Round(), (b1 + m).Round());
        }
    }
}
#endif
