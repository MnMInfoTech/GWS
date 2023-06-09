/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

namespace MnM.GWS
{
    using System;
    using System.Runtime.CompilerServices;

    public static partial class Colours
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

        public const uint RMask = 0xff000000 >> 8;
        public const uint GMask = 0x00ff0000 >> 8;
        public const uint BMask = 0x0000ff00 >> 8;
        public const uint AMask = 0x000000ff >> 8;

        public const int Transparent = 16777215;
        public const int White = -1;
        public const int Gray = -8355712;
        public const int Black = -16777216;
        public const uint UWhite = unchecked((uint)White);
        public const int Inversion = 0xFFFFFF;
        public const uint AlphaRemoval = 0xFF000000;
        public const uint UInversion = 0xFFFFFF;

        public const byte MAX = 255;
        public const byte TWO = 2;

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
        static Colours()
        {
            ChangeScheme(null, null, null);
        }
        internal static void Initialize() { }
        #endregion

        #region RGBA COLOR SCHEME
        /// <summary>
        /// Changes Colour Scheme i.e values of R, G, B, A will be intreprated.
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
            Rgba.Reset();
            ChangeScheme2();
        }
        static partial void ChangeScheme2();
        #endregion

        #region REPEAT
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int[] Repeat(this int c, int repeat = 0, bool invert = false)
        {
            if (repeat <= 0)
            {
                if (invert)
                    return new int[] { c ^ 0xffffff };
                return new int[] { c };
            }

            var data = new int[repeat + 1];
            if (invert) c ^= 0xffffff;

            fixed (int* d = data)
            {
                for (int i = 0; i < repeat; i++)
                    d[i] = c;
            }
            return data;
        }

        public static unsafe int[] Repeat(this IColour colour, int repeat = 0, bool invert = false)
        {
            int c = colour.Colour;

            if (repeat <= 0)
            {
                if (invert)
                    return new int[] { c ^ 0xffffff };
                return new int[] { c };
            }

            var data = new int[repeat + 1];
            if (invert) c ^= 0xffffff;

            fixed (int* d = data)
            {
                for (int i = 0; i < repeat; i++)
                    d[i] = c;
            }
            return data;
        }
        #endregion

        #region BLEND
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Blend(this int colour1, int colour2)
        {
            uint alpha = (uint)((colour2 >> AShift) & 0xFF);
            if (alpha == 0)
                return colour1;

            if (alpha == MAX)
                return colour2;

            uint c1 = (uint)colour1;
            uint c2 = (uint)colour2;
            uint invAlpha = MAX - alpha;
            uint rb = ((invAlpha * (c1 & RBMASK)) + (alpha * (c2 & RBMASK))) >> 8;
            uint ag = (invAlpha * ((c1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((c2 & GMASK) >> 8)));

            return (int)((rb & RBMASK) | (ag & AGMASK));
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Blend(this int dstColour, int srcColour, byte alpha)
        {
            uint C1 = (uint)dstColour;
            uint C2 = (uint)srcColour;
            uint invAlpha = (byte)(MAX - alpha);
            uint RB = ((invAlpha * (C1 & RBMASK)) + (alpha * (C2 & RBMASK))) >> 8;
            uint AG = (invAlpha * ((C1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((C2 & GMASK) >> 8)));

            srcColour = (int)((RB & RBMASK) | (AG & AGMASK));
            return srcColour;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Blend(this int dstColour, int srcColour, byte alpha, out byte iDelta)
        {
            uint C1 = (uint)dstColour;
            uint C2 = (uint)srcColour;
            uint invAlpha = (uint)(MAX - alpha);
            uint RB = ((invAlpha * (C1 & RBMASK)) + (alpha * (C2 & RBMASK))) >> 8;
            uint AG = (invAlpha * ((C1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((C2 & GMASK) >> 8)));

            srcColour = (int)((RB & RBMASK) | (AG & AGMASK));
            iDelta = (byte)((srcColour >> AShift) & 0xFF);
            return srcColour;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Blend(this int colour1, int colour2, float position, float size)
        {
            var POS = (position / size) * MAX;
            var pos = POS.Round();

            uint alpha = (uint)pos;
            uint c1 = (uint)colour1;
            uint c2 = (uint)colour2;
            uint invAlpha = MAX - alpha;
            uint rb = ((invAlpha * (c1 & RBMASK)) + (alpha * (c2 & RBMASK))) >> 8;
            uint ag = (invAlpha * ((c1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((c2 & GMASK) >> 8)));

            return (int)((rb & RBMASK) | (ag & AGMASK));
        }
        #endregion

        #region BLEND BI-LINEAR
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Blend(this int colour1, int colour2, int colour3, int colour4,
            float Dx, float Dy, int backgroundColour = 0)
        {
            return (int)Blend
            (
                (uint)colour1,
                (uint)colour2,
                (uint)colour3,
                (uint)colour4,
                (byte)(Dx * MAX),
                (byte)(Dy * MAX),
                (uint)backgroundColour
            );
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Blend(this uint colour1, uint colour2, uint colour3, uint colour4,
            float Dx, float Dy, uint backgroundColour = 0)
        {
            return Blend
            (
                colour1,
                colour2,
                colour3,
                colour4,
                (byte)(Dx * MAX),
                (byte)(Dy * MAX),
                backgroundColour
            );
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Blend(this uint colour1, uint colour2, uint colour3, uint colour4,
            byte Dx, byte Dy, uint backgroundColour = 0)
        {
            #region BI-LINEAR INTERPOLATION SKIPPING CONDITION
            if (
                (Dx == 0 && Dy == 0)
                || (colour2 == colour1 && colour3 == colour1 && colour3 == colour4)
                || (Dx == 0 && colour3 == 0 && colour4 == 0)
                || (Dy == 0 && colour1 == 0 && colour2 == 0)
               )
            {
                return colour1;
            }
            #endregion

            #region ASSIGN DEFAULT COLOR TO EMPTY NEIGHBOUR PIXELS
            if (colour1 == 0)
                colour1 = backgroundColour;
            if (colour2 == 0)
                colour2 = backgroundColour;
            if (colour3 == 0)
                colour3 = backgroundColour;
            if (colour4 == 0)
                colour4 = backgroundColour;
            #endregion

            #region VARIABLE INITIALIZATION
            uint RB, AG, alpha, invAlpha;
            uint srcColour, dstColour;
            bool goback = true, goback2 = true;
            #endregion

            srcColour = colour1;
            alpha = Dx;

            if (alpha == MAX || alpha < TWO)
                goto HORIZONTAL_BLEND;

            dstColour = colour2;
            goto BLEND;

            HORIZONTAL_BLEND:
            colour1 = srcColour;
            srcColour = colour3;
            if (alpha == MAX || alpha < TWO)
                goto VERTICAL_BLEND;

            dstColour = colour4;
            goto BLEND;

            VERTICAL_BLEND:
            dstColour = srcColour;
            srcColour = colour1;
            alpha = Dy;

            goto BLEND;

            BLEND:
            invAlpha = (MAX - alpha);
            RB = ((invAlpha * (srcColour & RBMASK)) + (alpha * (dstColour & RBMASK))) >> 8;
            AG = (invAlpha * ((srcColour & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((dstColour & GMASK) >> 8)));
            srcColour = ((RB & RBMASK) | (AG & AGMASK));

            if (goback)
            {
                goback = false;
                goto HORIZONTAL_BLEND;
            }
            if (goback2)
            {
                goback2 = false;
                goto VERTICAL_BLEND;
            }
            return srcColour;
        }
        #endregion

        #region MIX
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Mix(int* dest, int destIndex, int destLen, int colour, byte? externalAlpha = null)
        {
            if (destIndex >= destLen)
                return false;
            dest[destIndex] = Mix(dest[destIndex], colour, externalAlpha);
            return true;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Mix(int colour1, int colour2, byte? alpha = null)
        {
            if (alpha != null)
                colour2 = colour2.Change(alpha.Value);
            var a2 = colour2.Alpha();
            var colour = Blend(colour1, colour2);
            return colour.Change((byte)(a2 + ((a2 * (MAX - a2)) >> 8)));
        }
        #endregion

        #region INT TO RGBA
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Change(this int srcColour, byte iBlend)
        {
            return (iBlend << AShift) | (srcColour & Inversion);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ReplaceAlpha(this int srcColour, int another)
        {
            var iBlend = (byte)((another >> AShift) & 0xFF);
            var r = (byte)((srcColour >> RShift) & 0xFF);
            var g = (byte)((srcColour >> GShift) & 0xFF);
            var b = (byte)((srcColour >> BShift) & 0xFF);

            return (iBlend << AShift)
             | ((r & 0xFF) << RShift)
             | ((g & 0xFF) << GShift)
             | ((b & 0xFF) << BShift);
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToColour(int r, int g, int b, int a)
        {
            return ((byte)a << AShift)
                 | ((byte)((r) & 0xFF) << RShift)
                 | ((byte)((g) & 0xFF) << GShift)
                 | ((byte)((b) & 0xFF) << BShift);
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToColour(int r, int g, int b)
        {
            return ((byte)MAX << AShift)
                 | ((byte)((r) & 0xFF) << RShift)
                 | ((byte)((g) & 0xFF) << GShift)
                 | ((byte)((b) & 0xFF) << BShift);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToColour(float r, float g, float b, float a)
        {
            return ((byte)a.Round() << AShift)
                 | ((byte)(r.Round() & 0xFF) << RShift)
                 | ((byte)(g.Round() & 0xFF) << GShift)
                 | ((byte)(b.Round() & 0xFF) << BShift);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToColour(float r, float g, float b)
        {
            return ((byte)MAX << AShift)
                 | ((byte)(r.Round() & 0xFF) << RShift)
                 | ((byte)(g.Round() & 0xFF) << GShift)
                 | ((byte)(b.Round() & 0xFF) << BShift);
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToRGB(this int colour, out int r, out int g, out int b)
        {
            r = (byte)((colour >> RShift) & 0xFF);
            g = (byte)((colour >> GShift) & 0xFF);
            b = (byte)((colour >> BShift) & 0xFF);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToRGB(this int value, out byte r, out byte g, out byte b)
        {
            r = (byte)((value >> RShift) & 0xFF);
            g = (byte)((value >> GShift) & 0xFF);
            b = (byte)((value >> BShift) & 0xFF);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToRGBA(this int colour, out int r, out int g, out int b, out int a,
            byte? externalAlpha = null)
        {
            ToRGB(colour, out r, out g, out b);

            if (externalAlpha != null)
                a = externalAlpha.Value;
            else
                a = (byte)((colour >> AShift) & 0xFF);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToRGBA(this int colour, out int r, out int g, out int b, out int a)
        {
            ToRGB(colour, out r, out g, out b);
            a = (byte)((colour >> AShift) & 0xFF);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Alpha(this int value)
        {
            return (byte)((value >> AShift) & 0xFF);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToBytes(this int colour, out byte r, out byte g, out byte b, out byte a)
        {
            r = (byte)((colour >> RShift) & 0xFF);
            g = (byte)((colour >> GShift) & 0xFF);
            b = (byte)((colour >> BShift) & 0xFF);
            a = (byte)((colour >> AShift) & 0xFF);
        }
        #endregion

        #region LERP
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Lerp(this int rgba, int to, float amount)
        {
            ToRGB(rgba, out int r1, out int g1, out int b1);
            ToRGB(to, out int r2, out int g2, out int b2);

            // lerp the colours to get the difference
            byte r = (byte)LerpF(r1, r2, amount),
                 g = (byte)LerpF(g1, b2, amount),
                 b = (byte)LerpF(b1, g2, amount);

            // return the new colour
            return ToColour(r, g, b);
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LerpF(float start, float end, float amount)
        {
            float difference = end - start;
            float adjusted = difference * amount;
            return start + adjusted;
        }
        #endregion

        #region COLOR DISTANCE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ColourDistance(this int colour1, int colour2)
        {
            var f = ColourDistanceSquared(colour1, colour2);
            if (f == 0)
                return 0;
            if (f < 0)
                f = -f;
            return (float)Math.Sqrt(f);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ColourDistanceSquared(this int colour1, int colour2)
        {
            if (colour1 == 0 && colour2 == 0 || colour1 == colour2)
                return 0;
            var r1 = (colour1 >> RShift) & 0xFF;
            var g1 = (colour1 >> GShift) & 0xFF;
            var b1 = (colour1 >> BShift) & 0xFF;

            var r2 = (colour2 >> RShift) & 0xFF;
            var g2 = (colour2 >> GShift) & 0xFF;
            var b2 = (colour2 >> BShift) & 0xFF;

            var rmean = (r1 + r2) / 2;
            var distance = (((512 + rmean) * ((r1 - r2) ^ 2)) >> 8) +
                4 * ((g1 - g2) ^ 2) +
                (((767 - rmean) * ((b1 - b2) ^ 2)) >> 8);
            return distance;
        }
        #endregion

        #region CLOSENESS
        public static bool ColoursAreClose(this int colour1, int colour2, int threshold = 50)
        {
            var r1 = (colour1 >> RShift) & 0xFF;
            var g1 = (colour1 >> GShift) & 0xFF;
            var b1 = (colour1 >> BShift) & 0xFF;

            var r2 = (colour2 >> RShift) & 0xFF;
            var g2 = (colour2 >> GShift) & 0xFF;
            var b2 = (colour2 >> BShift) & 0xFF;


            int r = r1 - r2;
            int g = g1 - g2;
            int b = b1 - b2;
            return (r * r + g * g + b * b) <= threshold * threshold;
        }
        #endregion
    }
}
