/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if GWS || Window
    using System;
    using System.Runtime.CompilerServices;

    public static partial class Colors
    {
        #region CONST & VARIABLES
        public static int RShift = 0;
        public static int GShift = 8;
        public static int BShift = 16;
        public const int AShift = 24;

        public const uint AMASK = 0xFF000000;
        public const uint RBMASK = 0x00FF00FF;
        public const uint GMASK = 0x0000FF00;
        public const uint AGMASK = AMASK | GMASK;
        public const uint ONEALPHA = 0x01000000;

        public const int Transparent = 16777215;
        public const uint White = 4294967295;
        public const int Gray = -8355712;

        public readonly static float[] Alphas = new float[]
        {
            0f ,
            0.003921569f ,
            0.007843138f ,
            0.011764706f ,
            0.015686275f ,
            0.019607844f ,
            0.023529412f ,
            0.02745098f ,
            0.03137255f ,
            0.03529412f ,
            0.039215688f ,
            0.043137256f ,
            0.047058824f ,
            0.050980393f ,
            0.05490196f ,
            0.05882353f ,
            0.0627451f ,
            0.06666667f ,
            0.07058824f ,
            0.07450981f ,
            0.078431375f ,
            0.08235294f ,
            0.08627451f ,
            0.09019608f ,
            0.09411765f ,
            0.09803922f ,
            0.101960786f ,
            0.105882354f ,
            0.10980392f ,
            0.11372549f ,
            0.11764706f ,
            0.12156863f ,
            0.1254902f ,
            0.12941177f ,
            0.13333334f ,
            0.13725491f ,
            0.14117648f ,
            0.14509805f ,
            0.14901961f ,
            0.15294118f ,
            0.15686275f ,
            0.16078432f ,
            0.16470589f ,
            0.16862746f ,
            0.17254902f ,
            0.1764706f ,
            0.18039216f ,
            0.18431373f ,
            0.1882353f ,
            0.19215687f ,
            0.19607843f ,
            0.2f ,
            0.20392157f ,
            0.20784314f ,
            0.21176471f ,
            0.21568628f ,
            0.21960784f ,
            0.22352941f ,
            0.22745098f ,
            0.23137255f ,
            0.23529412f ,
            0.23921569f ,
            0.24313726f ,
            0.24705882f ,
            0.2509804f ,
            0.25490198f ,
            0.25882354f ,
            0.2627451f ,
            0.26666668f ,
            0.27058825f ,
            0.27450982f ,
            0.2784314f ,
            0.28235295f ,
            0.28627452f ,
            0.2901961f ,
            0.29411766f ,
            0.29803923f ,
            0.3019608f ,
            0.30588236f ,
            0.30980393f ,
            0.3137255f ,
            0.31764707f ,
            0.32156864f ,
            0.3254902f ,
            0.32941177f ,
            0.33333334f ,
            0.3372549f ,
            0.34117648f ,
            0.34509805f ,
            0.34901962f ,
            0.3529412f ,
            0.35686275f ,
            0.36078432f ,
            0.3647059f ,
            0.36862746f ,
            0.37254903f ,
            0.3764706f ,
            0.38039216f ,
            0.38431373f ,
            0.3882353f ,
            0.39215687f ,
            0.39607844f ,
            0.4f ,
            0.40392157f ,
            0.40784314f ,
            0.4117647f ,
            0.41568628f ,
            0.41960785f ,
            0.42352942f ,
            0.42745098f ,
            0.43137255f ,
            0.43529412f ,
            0.4392157f ,
            0.44313726f ,
            0.44705883f ,
            0.4509804f ,
            0.45490196f ,
            0.45882353f ,
            0.4627451f ,
            0.46666667f ,
            0.47058824f ,
            0.4745098f ,
            0.47843137f ,
            0.48235294f ,
            0.4862745f ,
            0.49019608f ,
            0.49411765f ,
            0.49803922f ,
            0.5019608f ,
            0.5058824f ,
            0.50980395f ,
            0.5137255f ,
            0.5176471f ,
            0.52156866f ,
            0.5254902f ,
            0.5294118f ,
            0.53333336f ,
            0.5372549f ,
            0.5411765f ,
            0.54509807f ,
            0.54901963f ,
            0.5529412f ,
            0.5568628f ,
            0.56078434f ,
            0.5647059f ,
            0.5686275f ,
            0.57254905f ,
            0.5764706f ,
            0.5803922f ,
            0.58431375f ,
            0.5882353f ,
            0.5921569f ,
            0.59607846f ,
            0.6f ,
            0.6039216f ,
            0.60784316f ,
            0.6117647f ,
            0.6156863f ,
            0.61960787f ,
            0.62352943f ,
            0.627451f ,
            0.6313726f ,
            0.63529414f ,
            0.6392157f ,
            0.6431373f ,
            0.64705884f ,
            0.6509804f ,
            0.654902f ,
            0.65882355f ,
            0.6627451f ,
            0.6666667f ,
            0.67058825f ,
            0.6745098f ,
            0.6784314f ,
            0.68235296f ,
            0.6862745f ,
            0.6901961f ,
            0.69411767f ,
            0.69803923f ,
            0.7019608f ,
            0.7058824f ,
            0.70980394f ,
            0.7137255f ,
            0.7176471f ,
            0.72156864f ,
            0.7254902f ,
            0.7294118f ,
            0.73333335f ,
            0.7372549f ,
            0.7411765f ,
            0.74509805f ,
            0.7490196f ,
            0.7529412f ,
            0.75686276f ,
            0.7607843f ,
            0.7647059f ,
            0.76862746f ,
            0.77254903f ,
            0.7764706f ,
            0.78039217f ,
            0.78431374f ,
            0.7882353f ,
            0.7921569f ,
            0.79607844f ,
            0.8f ,
            0.8039216f ,
            0.80784315f ,
            0.8117647f ,
            0.8156863f ,
            0.81960785f ,
            0.8235294f ,
            0.827451f ,
            0.83137256f ,
            0.8352941f ,
            0.8392157f ,
            0.84313726f ,
            0.84705883f ,
            0.8509804f ,
            0.85490197f ,
            0.85882354f ,
            0.8627451f ,
            0.8666667f ,
            0.87058824f ,
            0.8745098f ,
            0.8784314f ,
            0.88235295f ,
            0.8862745f ,
            0.8901961f ,
            0.89411765f ,
            0.8980392f ,
            0.9019608f ,
            0.90588236f ,
            0.9098039f ,
            0.9137255f ,
            0.91764706f ,
            0.92156863f ,
            0.9254902f ,
            0.92941177f ,
            0.93333334f ,
            0.9372549f ,
            0.9411765f ,
            0.94509804f ,
            0.9490196f ,
            0.9529412f ,
            0.95686275f ,
            0.9607843f ,
            0.9647059f ,
            0.96862745f ,
            0.972549f ,
            0.9764706f ,
            0.98039216f ,
            0.9843137f ,
            0.9882353f ,
            0.99215686f ,
            0.99607843f ,
            1f
        };
        #endregion

        #region CONSTRUCTORS
        static Colors()
        {
            ChangeScheme(null, null, null);
        }
        internal static void Initialize() { }
        #endregion

        #region RGBA COLOR SCHEME
        /// <summary>
        /// Changes Color Scheme i.e values of R, G, B, A will be intreprated.
        /// Defaults: RShift = 0, GShift = 8, BShift = 16, AShift = 24;
        /// For SDL you may want to switch RShift with BShift.
        /// </summary>
        /// <param name="rShift"></param>
        /// <param name="gShift"></param>
        /// <param name="bShift"></param>
        public static void ChangeScheme(int? rShift, int? gShift, int? bShift)
        {
            RShift = rShift ?? RShift;
            GShift = gShift ?? GShift;
            BShift = bShift ?? BShift;
            Pens.Reset();
        }
        #endregion

        #region INT TO RGBA
        public static int Change(this int c, byte? alpha)
        {
            if (alpha == null || alpha == 255)
                return c;

            if (alpha == c.Alpha())
                return c;
            ToRGB(c, out int r, out int g, out int b);
            return ToColor(r, g, b, alpha.Value);
        }

        public static int Change(this int c, float? delta)
        {
            if (delta == null || delta == 1)
                return c;
            var alpha = (byte)(delta.Value * 255);
            if (alpha == c.Alpha())
                return c;
            ToRGB(c, out int r, out int g, out int b);
            return ToColor(r, g, b, alpha);
        }
        public static int ToColor(int r, int g, int b, int a)
        {
            return ((byte)a << AShift)
                 | ((byte)((r) & 0xFF) << RShift)
                 | ((byte)((g) & 0xFF) << GShift)
                 | ((byte)((b) & 0xFF) << BShift);
        }
        public static int ToColor(int r, int g, int b)
        {
            return ((byte)255 << AShift)
                 | ((byte)((r) & 0xFF) << RShift)
                 | ((byte)((g) & 0xFF) << GShift)
                 | ((byte)((b) & 0xFF) << BShift);
        }
        public static int ToColor(float r, float g, float b, float a)
        {
            return ((byte)a.Round() << AShift)
                 | ((byte)(r.Round() & 0xFF) << RShift)
                 | ((byte)(g.Round() & 0xFF) << GShift)
                 | ((byte)(b.Round() & 0xFF) << BShift);
        }
        public static int ToColor(float r, float g, float b)
        {
            return ((byte)255 << AShift)
                 | ((byte)(r.Round() & 0xFF) << RShift)
                 | ((byte)(g.Round() & 0xFF) << GShift)
                 | ((byte)(b.Round() & 0xFF) << BShift);
        }

        public static void ToRGB(this int value, out int r, out int g, out int b)
        {
            r = (byte)((value >> RShift) & 0xFF);
            g = (byte)((value >> GShift) & 0xFF);
            b = (byte)((value >> BShift) & 0xFF);
        }
        public static void ToRGB(this int value, out byte r, out byte g, out byte b)
        {
            r = (byte)((value >> RShift) & 0xFF);
            g = (byte)((value >> GShift) & 0xFF);
            b = (byte)((value >> BShift) & 0xFF);
        }

        public static void ToRGBA(this int color, out int r, out int g, out int b, out int a, byte? externalAlpha = null)
        {
            ToRGB(color, out r, out g, out b);

            if (externalAlpha != null)
                a = externalAlpha.Value;
            else
                a = (byte)((color >> AShift) & 0xFF);
        }
        public static byte Alpha(this int value)
        {
            return (byte)((value >> AShift) & 0xFF);
        }
        public static void ToBytes(this int color, out byte r, out byte g, out byte b, out byte a)
        {
            r = (byte)((color >> RShift) & 0xFF);
            g = (byte)((color >> GShift) & 0xFF);
            b = (byte)((color >> BShift) & 0xFF);
            a = (byte)((color >> AShift) & 0xFF);
        }
        #endregion

        #region REPEAT
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int[] Repeat(this int c, int repeat = 0, bool invert = false)
        {
            if (repeat <= 0)
            {
                if (invert)
                    return new int[] { c ^ 0xffffff };
                return new int[] { c };
            }

            var data = new int[repeat];
            if (invert) c ^= 0xffffff;

            fixed (int* d = data)
            {
                for (int i = 0; i < repeat; i++)
                    d[i] = c;
            }
            return data;
        }
        #endregion

        #region COLOR LIST
        public static int[] GetKnownColors() =>
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

        #region BLEND
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Blend(int color1, int color2)
        {
            float delta = Alphas[(byte)((color2 >> AShift) & 0xFF)];

            uint alpha = (uint)(delta * 255);
            if (alpha == 0)
                return color1;

            if (alpha == 255)
                return color2;

            uint c1 = (uint)color1;
            uint c2 = (uint)color2;
            uint invAlpha = 255 - alpha;
            uint rb = ((invAlpha * (c1 & RBMASK)) + (alpha * (c2 & RBMASK))) >> 8;
            uint ag = (invAlpha * ((c1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((c2 & GMASK) >> 8)));

            return (int)((rb & RBMASK) | (ag & AGMASK));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Blend(int color1, int color2, float delta)
        {
            uint alpha = (uint)(delta * 255);
            if (alpha == 0)
                return color1;

            if (alpha == 255)
                return color2;

            uint c1 = (uint)color1;
            uint c2 = (uint)color2;
            uint invAlpha = 255 - alpha;
            uint rb = ((invAlpha * (c1 & RBMASK)) + (alpha * (c2 & RBMASK))) >> 8;
            uint ag = (invAlpha * ((c1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((c2 & GMASK) >> 8)));

            return (int)((rb & RBMASK) | (ag & AGMASK));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Blend(int color1, int color2, int color3, int color4, float Dx, float Dy)
        {
            uint c1 = (uint)color1;
            uint c2 = (uint)color2;

            uint c3 = (uint)color3;
            uint c4 = (uint)color4;
            uint rb, ag;

            uint alpha = (uint)(Dx * 255);
            uint invAlpha = 255 - alpha;

            if (alpha == 255)
                c1 = c2;

            else if (alpha != 0)
            {
                rb = ((invAlpha * (c1 & RBMASK)) + (alpha * (c2 & RBMASK))) >> 8;
                ag = (invAlpha * ((c1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((c2 & GMASK) >> 8)));
                c1 = ((rb & RBMASK) | (ag & AGMASK));
            }

            if (alpha == 255)
                c3 = c4;
            else if (alpha != 0)
            {
                rb = ((invAlpha * (c3 & RBMASK)) + (alpha * (c4 & RBMASK))) >> 8;
                ag = (invAlpha * ((c3 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((c4 & GMASK) >> 8)));
                c3 = ((rb & RBMASK) | (ag & AGMASK));
            }

            alpha = (uint)(Dy * 255);
            invAlpha = 255 - alpha;

            if (alpha == 255)
                return (int)c3;
            else if (alpha != 0)
            {
                rb = ((invAlpha * (c1 & RBMASK)) + (alpha * (c3 & RBMASK))) >> 8;
                ag = (invAlpha * ((c1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((c3 & GMASK) >> 8)));
                return (int)((rb & RBMASK) | (ag & AGMASK));
            }
            else
                return (int)c1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Blend(int color1, int color2, float position, float size)
        {
            uint alpha = (uint)((position / size) * 255).Round();
            if (alpha == 0)
                return color1;

            if (alpha == 255)
                return color2;

            uint c1 = (uint)color1;
            uint c2 = (uint)color2;
            uint invAlpha = 255 - alpha;
            uint rb = ((invAlpha * (c1 & RBMASK)) + (alpha * (c2 & RBMASK))) >> 8;
            uint ag = (invAlpha * ((c1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((c2 & GMASK) >> 8)));

            return (int)((rb & RBMASK) | (ag & AGMASK));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Blend(uint c1, uint c2)
        {
            float delta = Alphas[(byte)((c2 >> AShift) & 0xFF)];

            uint alpha = (uint)(delta * 255);
            if (alpha == 0)
                return (int)c1;

            if (alpha == 255)
                return (int)c2;

            uint invAlpha = 255 - alpha;
            uint rb = ((invAlpha * (c1 & RBMASK)) + (alpha * (c2 & RBMASK))) >> 8;
            uint ag = (invAlpha * ((c1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((c2 & GMASK) >> 8)));

            return (int)((rb & RBMASK) | (ag & AGMASK));
        }

        #endregion

        #region MIX
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Mix(int* dest, int destIndex, int destLen, int color, byte? externalAlpha = null)
        {
            if (destIndex >= destLen)
                return false;
            dest[destIndex] = Mix(dest[destIndex], color);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Mix(int color1, int color2, byte? alpha = null)
        {
            color2 = color2.Change(alpha);
            var a2 = color2.Alpha();
            var color = Blend(color1, color2);
            return color.Change((byte)(a2 + ((a2 * (255 - a2)) >> 8)));
        }
        #endregion

        #region GET DIAGONAL COLOR
        public static int GetDiagonalColor(int c1, int c2, float wh, float ij)
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

            return ToColor(iRed1, iGreen1, iBlue1, iAlpha1);
        }
        #endregion

        #region RGB EQUAL
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RGBEqual(this int first, int other)
        {
            if (other.Alpha() == 0 && first.Alpha() == 0)
                return true;

            ToRGB(first, out int r1, out int g1, out int b1);
            ToRGB(other, out int r2, out int g2, out int b2);

            return r1 == r2 && g1 == g2 && b1 == b2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RGBEqual(this int first, int? other)
        {
            if (other == null)
                return false;
            return RGBEqual(first, other.Value);
        }
        #endregion

        #region SCALE DATA
        public static unsafe IntPtr Scale(this IntPtr Data, int dataLength, int currentScale, int newScale, out int length)
        {
            if (currentScale == newScale)
            {
                length = dataLength;
                return Data;
            }
            int* p = (int*)Data;
            if (currentScale > newScale)
            {
                var unit = ((float)currentScale / newScale).Round();
                Collection<int> Array = new Collection<int>(newScale, true);

                for (int i = 0; i < dataLength; i += unit)
                    Array.Add(p[i]);

                length = Array.Count;
                fixed (int* q = Array.Data)
                    return new IntPtr(q);
            }
            else
            {
                var scale = (float)currentScale / newScale;
                var unit = newScale / currentScale;
                Collection<int> Array = new Collection<int>(newScale, true);
                for (int i = 0; i < dataLength - 1; i++)
                {
                    var c1 = p[i];
                    var c2 = p[i + 1];
                    float pos = scale;

                    Array.Add(c1);
                    for (int j = 0; j <= unit; j++)
                    {
                        Blend(c1, c2, pos, newScale);
                        pos += scale;
                    }
                    Array.Add(c2);
                }
                length = Array.Count;
                fixed (int* q = Array.Data)
                    return new IntPtr(q);
            }
        }
        #endregion

        #region HUE, BRIGHTNESS, SATURATION
        public static float Hue(this Rgba rgba)
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
        public static float Brightness(this Rgba rgba)
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
        public static float Saturation(this Rgba rgba)
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

            // if max == min, then there is no color and
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
        public static bool RGBEqual(this Rgba rgba, Rgba other)
        {
            if (other.A == 0 && rgba.A == 0)
                return true;
            return rgba.R == other.R && rgba.G == other.G && rgba.B == other.B;
        }
        public static bool RGBAEqual(this Rgba rgba, Rgba other)
        {
            if (other.A == 0 && rgba.A == 0)
                return true;
            return rgba.R == other.R && rgba.G == other.G && rgba.B == other.B && rgba.A == other.A;
        }
        #endregion

        #region LERP
        public static int Lerp(this int rgba, int to, float amount)
        {
            ToRGB(rgba, out int r1, out int g1, out int b1);
            ToRGB(to, out int r2, out int g2, out int b2);

            // lerp the colours to get the difference
            byte r = (byte)LerpF(r1, r2, amount),
                 g = (byte)LerpF(g1, b2, amount),
                 b = (byte)LerpF(b1, g2, amount);

            // return the new colour
            return ToColor(r, g, b);
        }
        public static float LerpF(float start, float end, float amount)
        {
            float difference = end - start;
            float adjusted = difference * amount;
            return start + adjusted;
        }
        #endregion

        #region LIGHT/DARK/INVERT
        public static int Invert(this int rgba)
        {
            byte r, g, b, a;
            ToRGBA(rgba, out int R, out int G, out int B, out int A);

            if (255 - R < byte.MinValue)
                r = byte.MinValue;
            else
                r = (byte)(255 - R);
            if (255 - G < byte.MinValue)
                g = byte.MinValue;
            else
                g = (byte)(255 - G);
            if (255 - B < byte.MinValue)
                b = byte.MinValue;
            else
                b = (byte)(255 - B);
            if (255 - A < byte.MinValue)
                a = byte.MinValue;
            else
                a = (byte)(255 - A);

            return ToColor(r, g, b, a);
        }

        public static int Lighten(this int rgba, float percent) =>
            Lerp(rgba, -1, percent);
        public static int Darken(this int rgba, float percent) =>
            Lerp(rgba, 0, percent);
        #endregion

        #region REPEAT
        public static int[] Repeat(Rgba rgba, int repeat = 0)
        {
            if (repeat <= 0)
                return new int[] { rgba.Color };

            var data = new int[repeat];
            for (int i = 0; i < repeat; i++)
                data[i] = rgba.Color;
            return data;
        }
        #endregion

        #region CHANGE
        public static Rgba Change(this Rgba rgba, byte? alpha)
        {
            if (alpha == null || alpha == 255)
                return new Rgba(rgba);

            if (alpha == 0 || alpha == rgba.A)
                return new Rgba(rgba);
            return new Rgba(rgba.R, rgba.G, rgba.B, alpha.Value);
        }
        #endregion

        #region LERP
        public static Rgba Lerp(this Rgba rgba, Rgba to, float amount)
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
            return new Rgba(r, g, b, (byte)255);
        }
        #endregion

        #region LIGHT/ DARK / INVERT
        public static Rgba Lighten(this Rgba rgba, float percent) =>
            new Rgba(Lighten(rgba.Color, percent));
        public static Rgba Darken(this Rgba rgba, float percent) =>
            new Rgba(Darken(rgba.Color, percent));
        public static Rgba Invert(this Rgba rgba)
        {
            byte r, g, b, a;
            Rgba w = Rgba.White;

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
        public static Rgba GetGradient(this Rgba rgba, Rgba second, float k)
        {
            float r, g, b, a;
            r = (float)(rgba.R + (second.R - rgba.R) * k);
            g = (float)(rgba.G + (second.G - rgba.G) * k);
            b = (float)(rgba.B + (second.B - rgba.B) * k);
            a = (float)(rgba.A + (second.A - rgba.A) * k);
            return new Rgba(r, g, b, a);
        }
        public static Rgba GetTweenColor(this Rgba rgba, Rgba second, float RatioOf2)
        {
            if (RatioOf2 <= 0)
                return new Rgba(rgba);

            if (RatioOf2 >= 1f)
                return new Rgba(second.R, second.G, second.B, second.A);

            // figure out how much of each color we should be.
            float RatioOf1 = 1f - RatioOf2;
            return new Rgba(
                rgba.R * RatioOf1 + second.R * RatioOf2,
                rgba.G * RatioOf1 + second.G * RatioOf2,
                rgba.B * RatioOf1 + second.B * RatioOf2);
        }
        public static float SumOfDistances(Rgba rgba, Rgba second)
        {
            float dist = Math.Abs(rgba.R - second.R) +
                Math.Abs(rgba.G - second.G) + Math.Abs(rgba.B - second.B);
            return dist;
        }
        #endregion

        #region BLEND - MIX
        public static Rgba Blend(this Rgba rgba, Rgba c2, float e0 = 0)
        {
            return Blend(rgba.Color, c2.Color, e0);
        }
        public static Rgba Mix(this Rgba rgba, Rgba c2, byte? alpha)
        {
            Rgba color2 = c2;
            c2.Change(alpha);
            var color = Blend(rgba, color2);
            color.Change((byte)(color2.A + ((color2.A * (255 - color2.A)) >> 8)));
            return color;
        }
        #endregion

        #region ROTATE 
        static int Clamp(double v)
        {
            return System.Convert.ToInt32(Math.Max(0F, Math.Min(v + 0.5, 255.0F)));
        }
        public static Rgba Rotate(this Rgba rgba, Rotation angle)
        {
            angle.SinCos(out float sin, out float cos);

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
            return new Rgba(rx, gx, bx);
        }
        #endregion
    }
#endif
}
