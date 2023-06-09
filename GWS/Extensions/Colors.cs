/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if(GWS || Window)
namespace MnM.GWS
{
    using System;
    using System.Runtime.CompilerServices;

    partial class Colours
    {
        #region RGBA TO RGB555
        public static ushort ToRGB555(this int colour)
        {
            int r = ((colour >> RShift) & 0xFF);
            int g = ((colour >> GShift) & 0xFF);
            int b = ((colour >> BShift) & 0xFF);
            int a = ((colour >> AShift) & 0xFF);
            return (ushort)((a >= 128 ? 0x8000 : 0x0000) |
                ((r & 0xF8) << 7) | ((g & 0xF8) << 2) | (b >> 3));
        }
        #endregion

        #region COLOR LIST
        public static int[] GetKnownColours() =>
            new[]
            {
             16777215,
             16777216,
             16842752,
             16908288,
             16973824,
             17039360,
             17104896,
             17170432,
             17235968,
             17301504,
             17367040,
             17432576,
             17498112,
             17563648,
             17629184,
             17694720,
             17760256,
             17825792,
             17891328,
             17956864,
             18022400,
             18087936,
             18153472,
             18219008,
             18284544,
             18350080,
             18415616,
             18481152,
             18546688,
             18612224,
             18677760,
             18743296,
             18808832,
             18874368,
             -16777216,
             -8388608,
             -7667712,
             -3342336,
             -65536,
             -16751616,
             -16744448,
             -8355840,
             -7632128,
             -16640,
             -3027456,
             -6620672,
             -16711936,
             -8388864,
             -256,
             -256,
             -9430759,
             -28642,
             -5590496,
             -14513374,
             -11039954,
             -11579601,
             -13447886,
             -9325764,
             -3088320,
             -2004671,
             -4947386,
             -7652024,
             -3354296,
             -8257461,
             -13669547,
             -6250913,
             -1206940,
             -5583514,
             -9868951,
             -3319190,
             -14446997,
             -7307152,
             -6715273,
             -1152901,
             -16712580,
             -16711809,
             -2818177,
             -16777088,
             -8388480,
             -16744320,
             -8355712,
             -1323385,
             -340345,
             -1954934,
             -16777077,
             -7667573,
             -15514229,
             -7619441,
             -7278960,
             -2396013,
             -2948972,
             -6751336,
             -3394919,
             -13447782,
             -13806944,
             -14013787,
             -5658199,
             -1648467,
             -13631571,
             -1118545,
             -2177872,
             -1646416,
             -14540110,
             -16021832,
             -2927174,
             -7368772,
             -9717827,
             -4144960,
             -8055353,
             -10724147,
             -12614195,
             -14784046,
             -7555886,
             -2894893,
             -2572328,
             -2723622,
             -14637606,
             -7114533,
             -12839716,
             -2302756,
             -2252579,
             -7882530,
             -32,
             -334106,
             -8743191,
             -1146130,
             -5576466,
             -8355600,
             -7543056,
             -1808,
             -983056,
             -16,
             -10443532,
             -4989195,
             -2296331,
             -657931,
             -327691,
             -1800,
             -9273094,
             -2626566,
             -1642246,
             -2950406,
             -1640963,
             -16776961,
             -65281,
             -65281,
             -7138049,
             -16759297,
             -12098561,
             -4953601,
             -11501569,
             -16741121,
             -8740609,
             -16734721,
             -4081921,
             -3424001,
             -16721921,
             -4596993,
             -5382401,
             -4856577,
             -3873537,
             -1972993,
             -3281921,
             -2756609,
             -659201,
             -1116673,
             -2295553,
             -3278081,
             -984321,
             -328961,
             -16711681,
             -2031617,
             -983041,
             -1,
            };
        #endregion

        #region GET DIAGONAL COLOR
        public static int GetDiagonalColour(int c1, int c2, float wh, float ij)
        {
            c1.ToRGBA(out int r1, out int g1, out int b1, out int a1);
            c2.ToRGBA(out int r2, out int g2, out int b2, out int a2);

            var whij = wh - ij;

            var iRed1 = (r1 * whij).Round();
            var iGreen1 = (g1 * whij).Round();
            var iBlue1 = (b1 * whij).Round();
            var iAlpha1 = (a1 * whij).Round();

            var iRed2 = (r2 * ij).Round();
            var iGreen2 = (g2 * ij).Round();
            var iBlue2 = (b2 * ij).Round();
            var iAlpha2 = (a2 * ij).Round();

            iRed1 += iRed2;
            iGreen1 += iGreen2;
            iBlue1 += iBlue2;
            iAlpha1 += iAlpha2;

            iRed1 = (iRed1 / wh).Round();
            iGreen1 = (iGreen1 / whij).Round();
            iBlue1 = (iBlue1 / whij).Round();
            iAlpha1 = (iAlpha1 / whij).Round();

            return ToColour(iRed1, iGreen1, iBlue1, iAlpha1);
        }
        #endregion

        #region RGB EQUAL
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RGBEqual(this int first, int other)
        {
            if (other.Alpha() == 0 && first.Alpha() == 0)
                return true;

            ToRGB(first, out int r1, out int g1, out int b1);
            ToRGB(other, out int r2, out int g2, out int b2);

            return r1 == r2 && g1 == g2 && b1 == b2;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RGBEqual(this int first, int? other)
        {
            if (other == null)
                return false;
            return RGBEqual(first, other.Value);
        }
        #endregion

        #region HUE, BRIGHTNESS, SATURATION
        public static float Hue(this IRgba rgba)
        {
            if (rgba.R == rgba.G && rgba.G == rgba.B)
                return 0; // 0 makes as good an UNDEFINED value as any

            float r = Alphas[rgba.R];
            float g = Alphas[rgba.G];
            float b = Alphas[rgba.B];

            float max, min;
            float delta;
            float hue = 0.0f;

            max = r; min = r;

            if (g > max) max = g;
            if (b > max) max = b;

            if (g < min) min = g;
            if (b < min) min = b;

            delta = max - min;

            if (r == max)
            {
                hue = (g - b) / delta;
            }
            else if (g == max)
            {
                hue = 2 + (b - r) / delta;
            }
            else if (b == max)
            {
                hue = 4 + (r - g) / delta;
            }
            hue *= 60;

            if (hue < 0.0f)
            {
                hue += 360.0f;
            }
            return hue;
        }
        public static float Brightness(this IRgba rgba)
        {
            float r = Alphas[rgba.R];
            float g = Alphas[rgba.G];
            float b = Alphas[rgba.B];

            float max, min;

            max = r; min = r;

            if (g > max) max = g;
            if (b > max) max = b;

            if (g < min) min = g;
            if (b < min) min = b;

            return (max + min) / 2;
        }
        public static float Saturation(this IRgba rgba)
        {
            float r = Alphas[rgba.R];
            float g = Alphas[rgba.G];
            float b = Alphas[rgba.B];

            float max, min;
            float l, s = 0;

            max = r; min = r;

            if (g > max) max = g;
            if (b > max) max = b;

            if (g < min) min = g;
            if (b < min) min = b;

            // if max == min, then there is no colour and
            // the saturation is zero.
            //
            if (max != min)
            {
                l = (max + min) / 2;

                if (l <= .5)
                {
                    s = (max - min) / (max + min);
                }
                else
                {
                    s = (max - min) / (2 - max - min);
                }
            }
            return s;
        }
        #endregion

        #region RGB EQUAL
        public static bool RGBEqual(this IRgba rgba, IRgba other)
        {
            if (other.A == 0 && rgba.A == 0)
                return true;
            return rgba.R == other.R && rgba.G == other.G && rgba.B == other.B;
        }
        public static bool RGBAEqual(this IRgba rgba, IRgba other)
        {
            if (other.A == 0 && rgba.A == 0)
                return true;
            return rgba.R == other.R && rgba.G == other.G && rgba.B == other.B && rgba.A == other.A;
        }
        #endregion

        #region LIGHT/DARK/INVERT
        public static int Invert(this int rgba)
        {
            byte r, g, b, a;
            ToRGBA(rgba, out int R, out int G, out int B, out int A);

            if (MAX - R < byte.MinValue)
                r = byte.MinValue;
            else
                r = (byte)(MAX - R);
            if (MAX - G < byte.MinValue)
                g = byte.MinValue;
            else
                g = (byte)(MAX - G);
            if (MAX - B < byte.MinValue)
                b = byte.MinValue;
            else
                b = (byte)(MAX - B);
            if (MAX - A < byte.MinValue)
                a = byte.MinValue;
            else
                a = (byte)(MAX - A);

            return ToColour(r, g, b, a);
        }

        public static int Lighten(this int rgba, float percent) =>
            Lerp(rgba, -1, percent);
        public static int Darken(this int rgba, float percent) =>
            Lerp(rgba, 0, percent);
        #endregion

        #region CHANGE
        public static Rgba Change(this IRgba rgba, byte? alpha)
        {
            if (alpha == null || alpha == MAX)
                return new Rgba(rgba);

            if (alpha == 0 || alpha == rgba.A)
                return new Rgba(rgba);
            return new Rgba(rgba.R, rgba.G, rgba.B, alpha.Value);
        }
        #endregion

        #region LERP
        public static Rgba Lerp(this IRgba rgba, IRgba to, float amount)
        {
            // start colours as lerp-able floats
            float sr = rgba.R, sg = rgba.G, sb = rgba.B;

            // end colours as lerp-able floats
            float er = to.R, eg = to.G, eb = to.B;

            // lerp the colours to get the difference
            byte r = (byte)LerpF(sr, er, amount),
                 g = (byte)LerpF(sg, eg, amount),
                 b = (byte)LerpF(sb, eb, amount);

            // return the new colour
            return new Rgba(r, g, b, (byte)MAX);
        }
        #endregion

        #region LIGHT/ DARK / INVERT
        public static Rgba Lighten(this IRgba rgba, float percent) =>
            new Rgba(Lighten(rgba.Colour, percent));
        public static Rgba Darken(this IRgba rgba, float percent) =>
            new Rgba(Darken(rgba.Colour, percent));
        public static Rgba Invert(this IRgba rgba)
        {
            byte r, g, b, a;
            IRgba w = Rgba.White;

            if (w.R - rgba.R < byte.MinValue)
                r = byte.MinValue;
            else
                r = (byte)(w.R - rgba.R);
            if (w.G - rgba.G < byte.MinValue)
                g = byte.MinValue;
            else
                g = (byte)(w.G - rgba.G);
            if (w.B - rgba.B < byte.MinValue)
                b = byte.MinValue;
            else
                b = (byte)(w.B - rgba.B);
            if (w.A - rgba.A < byte.MinValue)
                a = byte.MinValue;
            else
                a = (byte)(w.A - rgba.A);
            return new Rgba(r, g, b, a);
        }
        #endregion

        #region COLOR DISTANCE
        public static Rgba GetGradient(this IRgba rgba, IRgba second, float k)
        {
            float r, g, b, a;
            r = (int)(rgba.R + (second.R - rgba.R) * k);
            g = (int)(rgba.G + (second.G - rgba.G) * k);
            b = (int)(rgba.B + (second.B - rgba.B) * k);
            a = (int)(rgba.A + (second.A - rgba.A) * k);
            return new Rgba(r.Round(), g.Round(), b.Round(), a.Round());
        }
        public static Rgba GetTweenColour(this IRgba rgba, IRgba second, float RatioOf2)
        {
            if (RatioOf2 <= 0)
                return new Rgba(rgba);

            if (RatioOf2 >= 1f)
                return new Rgba(second.R, second.G, second.B, second.A);

            // figure out how much of each colour we should be.
            float RatioOf1 = 1f - RatioOf2;
            return new Rgba(
                (rgba.R * RatioOf1 + second.R * RatioOf2).Round(),
                (rgba.G * RatioOf1 + second.G * RatioOf2).Round(),
                (rgba.B * RatioOf1 + second.B * RatioOf2).Round());
        }
        public static float SumOfDistances(IRgba rgba, IRgba second)
        {
            float dist = Math.Abs(rgba.R - second.R) +
                Math.Abs(rgba.G - second.G) + Math.Abs(rgba.B - second.B);
            return dist;
        }

        public static float ColourDistance(this IRgba e1, IRgba e2)
        {
            var rmean = (e1.R + e2.R) / 2;
            var r = e1.R - e2.R;
            var g = e1.G - e2.G;
            var b = e1.B - e2.B;
            return (float) Math.Sqrt((((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8));
        }
        public static int ColourDistanceSquared(this IRgba e1, IRgba e2)
        {
            var rmean = (e1.R + e2.R) / 2;
            var r = e1.R - e2.R;
            var g = e1.G - e2.G;
            var b = e1.B - e2.B;
            return ((((512 + rmean) * r * r) >> 8) + 4 * g * g + (((767 - rmean) * b * b) >> 8));
        }
        #endregion

        #region BLEND - MIX
        public static Rgba Blend(this IRgba rgba, IRgba c2, byte alpha)
        {
            return Blend(rgba.Colour, c2.Colour, alpha);
        }
        #endregion

        #region ROTATE 
        static int Clamp(double v)
        {
            return System.Convert.ToInt32(Math.Max(0F, Math.Min(v + 0.5, 255f)));
        }
        public static Rgba Rotate(this IRgba rgba, IRotation rotation)
        {
            float sin = 0;
            float cos = 1;
            rotation?.SinCos(out sin, out cos);

            float[,] selfMatrix = new float[,] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
            float sqrtOneThirdTimesSin = (float)Math.Sqrt(1f / 3f) * sin;
            float oneThirdTimesOneSubCos = 1f / 3f * (1f - cos);
            selfMatrix[0, 0] = cos + (1f - cos) / 3f;
            selfMatrix[0, 1] = oneThirdTimesOneSubCos - sqrtOneThirdTimesSin;
            selfMatrix[0, 2] = oneThirdTimesOneSubCos + sqrtOneThirdTimesSin;
            selfMatrix[1, 0] = selfMatrix[0, 2];
            selfMatrix[1, 1] = cos + oneThirdTimesOneSubCos;
            selfMatrix[1, 2] = selfMatrix[0, 1];
            selfMatrix[2, 0] = selfMatrix[0, 1];
            selfMatrix[2, 1] = selfMatrix[0, 2];
            selfMatrix[2, 2] = cos + oneThirdTimesOneSubCos;
            float rx = rgba.R * selfMatrix[0, 0] + rgba.G * selfMatrix[0, 1] + rgba.B * selfMatrix[0, 2];
            float gx = rgba.R * selfMatrix[1, 0] + rgba.G * selfMatrix[1, 1] + rgba.B * selfMatrix[1, 2];
            float bx = rgba.R * selfMatrix[2, 0] + rgba.G * selfMatrix[2, 1] + rgba.B * selfMatrix[2, 2];
            return new Rgba(rx.Round(), gx.Round(), bx.Round());
        }
        #endregion

        #region CHANGE BRIGHTNESS
        /// <summary>
        /// Pavel Vladov's answer at: https://stackoverflow.com/questions/801406/c-create-a-lighter-darker-colour-based-on-a-system-colour.
        /// </summary>
        /// <param name="colour">Colour to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
        /// Negative values produce darker colours.</param>
        /// <returns>
        /// Corrected <see cref="Colour"/> structure.
        /// </returns>
        public static int ChangeBrightness(int colour, float correctionFactor)
        {
            ToRGBA(colour, out int r, out int g, out int b, out int a);

            float red = r;
            float green = g;
            float blue = b;
            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (MAX - red) * correctionFactor + red;
                green = (MAX - green) * correctionFactor + green;
                blue = (MAX - blue) * correctionFactor + blue;
            }
            return ToColour((int)red, (int)green, (int)blue, a);
        }
        #endregion

        #region CHANGE SCHEME
        static partial void ChangeScheme2()
        {
            BrushStyle.Reset();
        }
        #endregion
    }
}
#endif