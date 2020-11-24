/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
#if (GWS || Window)
    public struct Rgba : IPenContext, IColor
    {
        #region VARAIBLES
        /// <summary>
        /// Red component of this color.
        /// </summary>
        public readonly byte R;

        /// <summary>
        /// Green component of this color.
        /// </summary>
        public readonly byte G;

        /// <summary>
        /// Blue component of this color.
        /// </summary>
        public readonly byte B;

        /// <summary>
        /// Alpha component of this color.
        /// </summary>
        public readonly byte A;

        /// <summary>
        /// Interger  4 channels color value.
        /// </summary>
        public readonly int Color;
        #endregion

        #region CONSTRUCTORS
        public Rgba(byte r, byte g, byte b, byte a) : this()
        {
            R = r;
            G = g;
            B = b;
            A = a;
            Color = (a << Colors.AShift)
                 | ((byte)((r) & 0xFF) << Colors.RShift)
                 | ((byte)((g) & 0xFF) << Colors.GShift)
                 | ((byte)((b) & 0xFF) << Colors.BShift);
        }
        public Rgba(int value) : this()
        {
            Color = value;
            R = (byte)((value >> Colors.RShift) & 0xFF);
            G = (byte)((value >> Colors.GShift) & 0xFF);
            B = (byte)((value >> Colors.BShift) & 0xFF);
            A = (byte)((value >> Colors.AShift) & 0xFF);
        }
        public Rgba(byte r, byte g, byte b) :
            this(r, g, b, (byte)255)
        { }
        public Rgba(int r, int g, int b, int a = 255) :
            this((byte)r, (byte)g, (byte)b, (byte)a)
        { }

        public Rgba(int value, byte externalAlpha)
        {
            R = (byte)((value >> Colors.RShift) & 0xFF);
            G = (byte)((value >> Colors.GShift) & 0xFF);
            B = (byte)((value >> Colors.BShift) & 0xFF);
            A = (byte)((value >> Colors.AShift) & 0xFF);

            Color = ((byte)externalAlpha << Colors.AShift)
                 | ((byte)((R) & 0xFF) << Colors.RShift)
                 | ((byte)((G) & 0xFF) << Colors.GShift)
                 | ((byte)((B) & 0xFF) << Colors.BShift);
        }
        public Rgba(float r, float g, float b, float a = 255) :
            this(r.Round(), g.Round(), b.Round(), a.Round())
        { }
        public Rgba(double r, double g, double b, double a = 255) :
            this(r.Round(), g.Round(), b.Round(), a.Round())
        { }
        public Rgba(Rgba c, byte newAlpha) :
            this(c.R, c.G, c.B, newAlpha)
        { }
        public Rgba(IColor c) :
            this(c.Color)
        { }
        public Rgba(Rgba c, float newAlpha) :
            this(c.R, c.G, c.B, (byte)(newAlpha * 255))
        { }
        #endregion

        #region IRGBA
        int IColor.Color => Color;
        #endregion

        #region CHANGE
        public void Change(byte? alpha)
        {
            if (alpha == null || alpha == 255)
                return;

            if (alpha == 0 || alpha == A)
                return;
            this = new Rgba(R, G, B, alpha.Value);
        }
        #endregion

        #region EQUALITY
        public override bool Equals(object other)
        {
            if (!(other is Rgba))
                return false;

            var c = (Rgba)other;
            return c.Color == Color;
        }
        public bool Equals(Rgba c)
        {
            return c.Color == Color;
        }
        public override int GetHashCode()
        {
            return Color;
        }
        #endregion

        #region PREDEFINED PENS
        public static Rgba Empty { get; private set; }
        public static Rgba ActiveBorder { get; private set; }
        public static Rgba ActiveCaption { get; private set; }
        public static Rgba ActiveCaptionText { get; private set; }
        public static Rgba AppWorkspace { get; private set; }
        public static Rgba Control { get; private set; }
        public static Rgba ControlDark { get; private set; }
        public static Rgba ControlDarkDark { get; private set; }
        public static Rgba ControlLight { get; private set; }
        public static Rgba ControlLightLight { get; private set; }
        public static Rgba ControlText { get; private set; }
        public static Rgba Desktop { get; private set; }
        public static Rgba GrayText { get; private set; }
        public static Rgba Highlight { get; private set; }
        public static Rgba HighlightText { get; private set; }
        public static Rgba HotTrack { get; private set; }
        public static Rgba InactiveBorder { get; private set; }
        public static Rgba InactiveCaption { get; private set; }
        public static Rgba InactiveCaptionText { get; private set; }
        public static Rgba Info { get; private set; }
        public static Rgba InfoText { get; private set; }
        public static Rgba Menu { get; private set; }
        public static Rgba MenuText { get; private set; }
        public static Rgba ScrollBar { get; private set; }
        public static Rgba Window { get; private set; }
        public static Rgba WindowFrame { get; private set; }
        public static Rgba WindowText { get; private set; }
        public static Rgba Transparent { get; private set; }
        public static Rgba AliceBlue { get; private set; }
        public static Rgba AntiqueWhite { get; private set; }
        public static Rgba Aqua { get; private set; }
        public static Rgba Aquamarine { get; private set; }
        public static Rgba Azure { get; private set; }
        public static Rgba Beige { get; private set; }
        public static Rgba Bisque { get; private set; }
        public static Rgba Black { get; private set; }
        public static Rgba BlanchedAlmond { get; private set; }
        public static Rgba Blue { get; private set; }
        public static Rgba BlueViolet { get; private set; }
        public static Rgba Brown { get; private set; }
        public static Rgba BurlyWood { get; private set; }
        public static Rgba CadetBlue { get; private set; }
        public static Rgba Chartreuse { get; private set; }
        public static Rgba Chocolate { get; private set; }
        public static Rgba Coral { get; private set; }
        public static Rgba CornflowerBlue { get; private set; }
        public static Rgba Cornsilk { get; private set; }
        public static Rgba Crimson { get; private set; }
        public static Rgba Cyan { get; private set; }
        public static Rgba DarkBlue { get; private set; }
        public static Rgba DarkCyan { get; private set; }
        public static Rgba DarkGoldenrod { get; private set; }
        public static Rgba DarkGray { get; private set; }
        public static Rgba DarkGreen { get; private set; }
        public static Rgba DarkKhaki { get; private set; }
        public static Rgba DarkMagenta { get; private set; }
        public static Rgba DarkOliveGreen { get; private set; }
        public static Rgba DarkOrange { get; private set; }
        public static Rgba DarkOrchid { get; private set; }
        public static Rgba DarkRed { get; private set; }
        public static Rgba DarkSalmon { get; private set; }
        public static Rgba DarkSeaGreen { get; private set; }
        public static Rgba DarkSlateBlue { get; private set; }
        public static Rgba DarkSlateGray { get; private set; }
        public static Rgba DarkTurquoise { get; private set; }
        public static Rgba DarkViolet { get; private set; }
        public static Rgba DeepPink { get; private set; }
        public static Rgba DeepSkyBlue { get; private set; }
        public static Rgba DimGray { get; private set; }
        public static Rgba DodgerBlue { get; private set; }
        public static Rgba Firebrick { get; private set; }
        public static Rgba FloralWhite { get; private set; }
        public static Rgba ForestGreen { get; private set; }
        public static Rgba Fuchsia { get; private set; }
        public static Rgba Gainsboro { get; private set; }
        public static Rgba GhostWhite { get; private set; }
        public static Rgba Gold { get; private set; }
        public static Rgba Goldenrod { get; private set; }
        public static Rgba Gray { get; private set; }
        public static Rgba Green { get; private set; }
        public static Rgba GreenYellow { get; private set; }
        public static Rgba Honeydew { get; private set; }
        public static Rgba HotPink { get; private set; }
        public static Rgba IndianRed { get; private set; }
        public static Rgba Indigo { get; private set; }
        public static Rgba Ivory { get; private set; }
        public static Rgba Khaki { get; private set; }
        public static Rgba Lavender { get; private set; }
        public static Rgba LavenderBlush { get; private set; }
        public static Rgba LawnGreen { get; private set; }
        public static Rgba LemonChiffon { get; private set; }
        public static Rgba LightBlue { get; private set; }
        public static Rgba LightCoral { get; private set; }
        public static Rgba LightCyan { get; private set; }
        public static Rgba LightGoldenrodYellow { get; private set; }
        public static Rgba LightGray { get; private set; }
        public static Rgba LightGreen { get; private set; }
        public static Rgba LightPink { get; private set; }
        public static Rgba LightSalmon { get; private set; }
        public static Rgba LightSeaGreen { get; private set; }
        public static Rgba LightSkyBlue { get; private set; }
        public static Rgba LightSlateGray { get; private set; }
        public static Rgba LightSteelBlue { get; private set; }
        public static Rgba LightYellow { get; private set; }
        public static Rgba Lime { get; private set; }
        public static Rgba LimeGreen { get; private set; }
        public static Rgba Linen { get; private set; }
        public static Rgba Magenta { get; private set; }
        public static Rgba Maroon { get; private set; }
        public static Rgba MediumAquamarine { get; private set; }
        public static Rgba MediumBlue { get; private set; }
        public static Rgba MediumOrchid { get; private set; }
        public static Rgba MediumPurple { get; private set; }
        public static Rgba MediumSeaGreen { get; private set; }
        public static Rgba MediumSlateBlue { get; private set; }
        public static Rgba MediumSpringGreen { get; private set; }
        public static Rgba MediumTurquoise { get; private set; }
        public static Rgba MediumVioletRed { get; private set; }
        public static Rgba MidnightBlue { get; private set; }
        public static Rgba MintCream { get; private set; }
        public static Rgba MistyRose { get; private set; }
        public static Rgba Moccasin { get; private set; }
        public static Rgba NavajoWhite { get; private set; }
        public static Rgba Navy { get; private set; }
        public static Rgba OldLace { get; private set; }
        public static Rgba Olive { get; private set; }
        public static Rgba OliveDrab { get; private set; }
        public static Rgba Orange { get; private set; }
        public static Rgba OrangeRed { get; private set; }
        public static Rgba Orchid { get; private set; }
        public static Rgba PaleGoldenrod { get; private set; }
        public static Rgba PaleGreen { get; private set; }
        public static Rgba PaleTurquoise { get; private set; }
        public static Rgba PaleVioletRed { get; private set; }
        public static Rgba PapayaWhip { get; private set; }
        public static Rgba PeachPuff { get; private set; }
        public static Rgba Peru { get; private set; }
        public static Rgba Pink { get; private set; }
        public static Rgba Plum { get; private set; }
        public static Rgba PowderBlue { get; private set; }
        public static Rgba Purple { get; private set; }
        public static Rgba Red { get; private set; }
        public static Rgba RosyBrown { get; private set; }
        public static Rgba RoyalBlue { get; private set; }
        public static Rgba SaddleBrown { get; private set; }
        public static Rgba Salmon { get; private set; }
        public static Rgba SandyBrown { get; private set; }
        public static Rgba SeaGreen { get; private set; }
        public static Rgba SeaShell { get; private set; }
        public static Rgba Sienna { get; private set; }
        public static Rgba Silver { get; private set; }
        public static Rgba SkyBlue { get; private set; }
        public static Rgba SlateBlue { get; private set; }
        public static Rgba SlateGray { get; private set; }
        public static Rgba Snow { get; private set; }
        public static Rgba SpringGreen { get; private set; }
        public static Rgba SteelBlue { get; private set; }
        public static Rgba Tan { get; private set; }
        public static Rgba Teal { get; private set; }
        public static Rgba Thistle { get; private set; }
        public static Rgba Tomato { get; private set; }
        public static Rgba Turquoise { get; private set; }
        public static Rgba Violet { get; private set; }
        public static Rgba Wheat { get; private set; }
        public static Rgba White { get; private set; }
        public static Rgba WhiteSmoke { get; private set; }
        public static Rgba Yellow { get; private set; }
        public static Rgba YellowGreen { get; private set; }
        public static Rgba ButtonFace { get; private set; }
        public static Rgba ButtonHighlight { get; private set; }
        public static Rgba ButtonShadow { get; private set; }
        public static Rgba GradientActiveCaption { get; private set; }
        public static Rgba GradientInactiveCaption { get; private set; }
        public static Rgba MenuBar { get; private set; }
        public static Rgba MenuHighlight { get; private set; }
        #endregion

        #region MATH OPERATIONS
        public static Rgba operator +(Rgba A, Rgba B)
        {
            var r = A.R + B.R;
            var g = A.G + B.G;
            var b = A.B + B.B;
            var a = A.A + B.A;
            return new Rgba(r, g, b, a);
        }
        public static Rgba operator +(Rgba A, float B)
        {
            var r = A.R + B;
            var g = A.G + B;
            var b = A.B + B;
            var a = A.A + B;
            return new Rgba(r, g, b, a);
        }
        public static Rgba operator +(float A, Rgba B)
        {
            return B + A;
        }

        public static Rgba operator -(Rgba A, Rgba B)
        {
            var r = A.R - B.R;
            var g = A.G - B.G;
            var b = A.B - B.B;
            var a = A.A - B.A;
            return new Rgba(r, g, b, a);
        }
        public static Rgba operator -(Rgba A, float B)
        {
            return A + (-B);
        }
        public static Rgba operator -(float A, Rgba B)
        {
            return B + (-A);
        }

        public static Rgba operator *(Rgba A, Rgba B)
        {
            var r = A.R * B.R;
            var g = A.G * B.G;
            var b = A.B * B.B;
            var a = A.A * B.A;
            return new Rgba(r, g, b, a);
        }
        public static Rgba operator *(Rgba A, float B)
        {
            var r = A.R * B;
            var g = A.G * B;
            var b = A.B * B;
            var a = A.A * B;
            return new Rgba(r, g, b, a);
        }
        public static Rgba operator *(float A, Rgba B)
        {
            return B * (A);
        }

        public static Rgba operator /(Rgba A, Rgba B)
        {
            var r = A.R / B.R;
            var g = A.G / B.G;
            var b = A.B / B.B;
            var a = A.A / B.A;
            return new Rgba(r, g, b, a);
        }
        public static Rgba operator /(Rgba A, float B)
        {
            var r = A.R / B;
            var g = A.G / B;
            var b = A.B / B;
            var a = A.A / B;
            return new Rgba(r, g, b, a);
        }
        public static Rgba Divide(float A, Rgba B)
        {
            return B / (A);
        }
        #endregion

        #region CONVERSION OPERATORS
        public static explicit operator int(Rgba color) =>
            color.Color;

        public static implicit operator Rgba(int value) =>
            new Rgba(value);

        /// <summary>
        /// http://csharphelper.com/blog/2016/08/convert-between-rgb-and-hls-color-models-in-c/
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

        /// <summary>
        /// https://stackoverflow.com/questions/359612/how-to-change-rgb-color-to-hsv
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

        /// <summary>
        /// https://www.programmingalgorithms.com/algorithm/rgb-to-yuv/
        /// </summary>
        /// <param name="rgba"></param>
        public static implicit operator Yuv(Rgba rgba)
        {
            float y = rgba.R * .299000f + rgba.G * .587000f + rgba.B * .114000f;
            float u = rgba.R * -.168736f + rgba.G * -.331264f + rgba.B * .500000f + 128;
            float v = rgba.R * .500000f + rgba.G * -.418688f + rgba.B * -.081312f + 128;
            return new Yuv(y, u, v);
        }

        /// <summary>
        /// https://www.cyotek.com/blog/converting-colours-between-rgb-and-cmyk-in-csharp
        /// </summary>
        /// <param name="rgba"></param>
        public static implicit operator Cmyk(Rgba rgba)
        {
            float c;
            float m;
            float y;
            float k;
            float rf;
            float gf;
            float bf;

            rf = rgba.R / 255F;
            gf = rgba.G / 255F;
            bf = rgba.B / 255F;

            k = (1 - Math.Max(Math.Max(rf, gf), bf));
            c = ((1 - rf - k) / (1 - k));
            m = ((1 - gf - k) / (1 - k));
            y = ((1 - bf - k) / (1 - k));

            return new Cmyk(c, m, y, k);
        }
        #endregion

        #region RESET
        internal static void Reset()
        {
            Empty = new Rgba(0, 0, 0, 0);
            ActiveBorder = new Rgba(180, 180, 180, 255);
            ActiveCaption = new Rgba(209, 180, 153, 255);
            ActiveCaptionText = new Rgba(0, 0, 0, 255);
            AppWorkspace = new Rgba(171, 171, 171, 255);
            Control = new Rgba(240, 240, 240, 255);
            ControlDark = new Rgba(160, 160, 160, 255);
            ControlDarkDark = new Rgba(105, 105, 105, 255);
            ControlLight = new Rgba(227, 227, 227, 255);
            ControlLightLight = new Rgba(255, 255, 255, 255);
            ControlText = new Rgba(0, 0, 0, 255);
            Desktop = new Rgba(0, 0, 0, 255);
            GrayText = new Rgba(109, 109, 109, 255);
            Highlight = new Rgba(255, 153, 51, 255);
            HighlightText = new Rgba(255, 255, 255, 255);
            HotTrack = new Rgba(204, 102, 0, 255);
            InactiveBorder = new Rgba(252, 247, 244, 255);
            InactiveCaption = new Rgba(219, 205, 191, 255);
            InactiveCaptionText = new Rgba(0, 0, 0, 255);
            Info = new Rgba(225, 255, 255, 255);
            InfoText = new Rgba(0, 0, 0, 255);
            Menu = new Rgba(240, 240, 240, 255);
            MenuText = new Rgba(0, 0, 0, 255);
            ScrollBar = new Rgba(200, 200, 200, 255);
            Window = new Rgba(255, 255, 255, 255);
            WindowFrame = new Rgba(100, 100, 100, 255);
            WindowText = new Rgba(0, 0, 0, 255);
            Transparent = new Rgba(16777215);
            AliceBlue = new Rgba(255, 248, 240, 255);
            AntiqueWhite = new Rgba(215, 235, 250, 255);
            Aqua = new Rgba(255, 255, 0, 255);
            Aquamarine = new Rgba(212, 255, 127, 255);
            Azure = new Rgba(255, 255, 240, 255);
            Beige = new Rgba(220, 245, 245, 255);
            Bisque = new Rgba(196, 228, 255, 255);
            Black = new Rgba(0, 0, 0, 255);
            BlanchedAlmond = new Rgba(205, 235, 255, 255);
            Blue = new Rgba(255, 0, 0, 255);
            BlueViolet = new Rgba(226, 43, 138, 255);
            Brown = new Rgba(42, 42, 165, 255);
            BurlyWood = new Rgba(135, 184, 222, 255);
            CadetBlue = new Rgba(160, 158, 95, 255);
            Chartreuse = new Rgba(0, 255, 127, 255);
            Chocolate = new Rgba(30, 105, 210, 255);
            Coral = new Rgba(80, 127, 255, 255);
            CornflowerBlue = new Rgba(237, 149, 100, 255);
            Cornsilk = new Rgba(220, 248, 255, 255);
            Crimson = new Rgba(60, 20, 220, 255);
            Cyan = new Rgba(255, 255, 0, 255);
            DarkBlue = new Rgba(139, 0, 0, 255);
            DarkCyan = new Rgba(139, 139, 0, 255);
            DarkGoldenrod = new Rgba(11, 134, 184, 255);
            DarkGray = new Rgba(169, 169, 169, 255);
            DarkGreen = new Rgba(0, 100, 0, 255);
            DarkKhaki = new Rgba(107, 183, 189, 255);
            DarkMagenta = new Rgba(139, 0, 139, 255);
            DarkOliveGreen = new Rgba(47, 107, 85, 255);
            DarkOrange = new Rgba(0, 140, 255, 255);
            DarkOrchid = new Rgba(204, 50, 153, 255);
            DarkRed = new Rgba(0, 0, 139, 255);
            DarkSalmon = new Rgba(122, 150, 233, 255);
            DarkSeaGreen = new Rgba(139, 188, 143, 255);
            DarkSlateBlue = new Rgba(139, 61, 72, 255);
            DarkSlateGray = new Rgba(79, 79, 47, 255);
            DarkTurquoise = new Rgba(209, 206, 0, 255);
            DarkViolet = new Rgba(211, 0, 148, 255);
            DeepPink = new Rgba(147, 20, 255, 255);
            DeepSkyBlue = new Rgba(255, 191, 0, 255);
            DimGray = new Rgba(105, 105, 105, 255);
            DodgerBlue = new Rgba(255, 144, 30, 255);
            Firebrick = new Rgba(34, 34, 178, 255);
            FloralWhite = new Rgba(240, 250, 255, 255);
            ForestGreen = new Rgba(34, 139, 34, 255);
            Fuchsia = new Rgba(255, 0, 255, 255);
            Gainsboro = new Rgba(220, 220, 220, 255);
            GhostWhite = new Rgba(255, 248, 248, 255);
            Gold = new Rgba(0, 215, 255, 255);
            Goldenrod = new Rgba(32, 165, 218, 255);
            Gray = new Rgba(128, 128, 128, 255);
            Green = new Rgba(0, 128, 0, 255);
            GreenYellow = new Rgba(47, 255, 173, 255);
            Honeydew = new Rgba(240, 255, 240, 255);
            HotPink = new Rgba(180, 105, 255, 255);
            IndianRed = new Rgba(92, 92, 205, 255);
            Indigo = new Rgba(130, 0, 75, 255);
            Ivory = new Rgba(240, 255, 255, 255);
            Khaki = new Rgba(140, 230, 240, 255);
            Lavender = new Rgba(250, 230, 230, 255);
            LavenderBlush = new Rgba(245, 240, 255, 255);
            LawnGreen = new Rgba(0, 252, 124, 255);
            LemonChiffon = new Rgba(205, 250, 255, 255);
            LightBlue = new Rgba(230, 216, 173, 255);
            LightCoral = new Rgba(128, 128, 240, 255);
            LightCyan = new Rgba(255, 255, 224, 255);
            LightGoldenrodYellow = new Rgba(210, 250, 250, 255);
            LightGray = new Rgba(211, 211, 211, 255);
            LightGreen = new Rgba(144, 238, 144, 255);
            LightPink = new Rgba(193, 182, 255, 255);
            LightSalmon = new Rgba(122, 160, 255, 255);
            LightSeaGreen = new Rgba(170, 178, 32, 255);
            LightSkyBlue = new Rgba(250, 206, 135, 255);
            LightSlateGray = new Rgba(153, 136, 119, 255);
            LightSteelBlue = new Rgba(222, 196, 176, 255);
            LightYellow = new Rgba(224, 255, 255, 255);
            Lime = new Rgba(0, 255, 0, 255);
            LimeGreen = new Rgba(50, 205, 50, 255);
            Linen = new Rgba(230, 240, 250, 255);
            Magenta = new Rgba(255, 0, 255, 255);
            Maroon = new Rgba(0, 0, 128, 255);
            MediumAquamarine = new Rgba(170, 205, 102, 255);
            MediumBlue = new Rgba(205, 0, 0, 255);
            MediumOrchid = new Rgba(211, 85, 186, 255);
            MediumPurple = new Rgba(219, 112, 147, 255);
            MediumSeaGreen = new Rgba(113, 179, 60, 255);
            MediumSlateBlue = new Rgba(238, 104, 123, 255);
            MediumSpringGreen = new Rgba(154, 250, 0, 255);
            MediumTurquoise = new Rgba(204, 209, 72, 255);
            MediumVioletRed = new Rgba(133, 21, 199, 255);
            MidnightBlue = new Rgba(112, 25, 25, 255);
            MintCream = new Rgba(250, 255, 245, 255);
            MistyRose = new Rgba(225, 228, 255, 255);
            Moccasin = new Rgba(181, 228, 255, 255);
            NavajoWhite = new Rgba(173, 222, 255, 255);
            Navy = new Rgba(128, 0, 0, 255);
            OldLace = new Rgba(230, 245, 253, 255);
            Olive = new Rgba(0, 128, 128, 255);
            OliveDrab = new Rgba(35, 142, 107, 255);
            Orange = new Rgba(0, 165, 255, 255);
            OrangeRed = new Rgba(0, 69, 255, 255);
            Orchid = new Rgba(214, 112, 218, 255);
            PaleGoldenrod = new Rgba(170, 232, 238, 255);
            PaleGreen = new Rgba(152, 251, 152, 255);
            PaleTurquoise = new Rgba(238, 238, 175, 255);
            PaleVioletRed = new Rgba(147, 112, 219, 255);
            PapayaWhip = new Rgba(213, 239, 255, 255);
            PeachPuff = new Rgba(185, 218, 255, 255);
            Peru = new Rgba(63, 133, 205, 255);
            Pink = new Rgba(203, 192, 255, 255);
            Plum = new Rgba(221, 160, 221, 255);
            PowderBlue = new Rgba(230, 224, 176, 255);
            Purple = new Rgba(128, 0, 128, 255);
            Red = new Rgba(0, 0, 255, 255);
            RosyBrown = new Rgba(143, 143, 188, 255);
            RoyalBlue = new Rgba(225, 105, 65, 255);
            SaddleBrown = new Rgba(19, 69, 139, 255);
            Salmon = new Rgba(114, 128, 250, 255);
            SandyBrown = new Rgba(96, 164, 244, 255);
            SeaGreen = new Rgba(87, 139, 46, 255);
            SeaShell = new Rgba(238, 245, 255, 255);
            Sienna = new Rgba(45, 82, 160, 255);
            Silver = new Rgba(192, 192, 192, 255);
            SkyBlue = new Rgba(235, 206, 135, 255);
            SlateBlue = new Rgba(205, 90, 106, 255);
            SlateGray = new Rgba(144, 128, 112, 255);
            Snow = new Rgba(250, 250, 255, 255);
            SpringGreen = new Rgba(127, 255, 0, 255);
            SteelBlue = new Rgba(180, 130, 70, 255);
            Tan = new Rgba(140, 180, 210, 255);
            Teal = new Rgba(128, 128, 0, 255);
            Thistle = new Rgba(216, 191, 216, 255);
            Tomato = new Rgba(71, 99, 255, 255);
            Turquoise = new Rgba(208, 224, 64, 255);
            Violet = new Rgba(238, 130, 238, 255);
            Wheat = new Rgba(179, 222, 245, 255);
            White = new Rgba(255, 255, 255, 255);
            WhiteSmoke = new Rgba(245, 245, 245, 255);
            Yellow = new Rgba(0, 255, 255, 255);
            YellowGreen = new Rgba(50, 205, 154, 255);
            ButtonFace = new Rgba(240, 240, 240, 255);
            ButtonHighlight = new Rgba(255, 255, 255, 255);
            ButtonShadow = new Rgba(160, 160, 160, 255);
            GradientActiveCaption = new Rgba(234, 209, 185, 255);
            GradientInactiveCaption = new Rgba(242, 228, 215, 255);
            MenuBar = new Rgba(240, 240, 240, 255);
            MenuHighlight = new Rgba(255, 153, 51, 255);
        }
        #endregion

        #region FROM NAME
        public static Rgba FromName(string name)
        {
            switch (name)
            {
                case "ActiveBorder":
                    return Rgba.ActiveBorder;
                case "ActiveCaption":
                    return Rgba.ActiveCaption;
                case "ActiveCaptionText":
                    return Rgba.ActiveCaptionText;
                case "AppWorkspace":
                    return Rgba.AppWorkspace;
                case "Control":
                    return Rgba.Control;
                case "ControlDark":
                    return Rgba.ControlDark;
                case "ControlDarkDark":
                    return Rgba.ControlDarkDark;
                case "ControlLight":
                    return Rgba.ControlLight;
                case "ControlLightLight":
                    return Rgba.ControlLightLight;
                case "ControlText":
                    return Rgba.ControlText;
                case "Desktop":
                    return Rgba.Desktop;
                case "GrayText":
                    return Rgba.GrayText;
                case "Highlight":
                    return Rgba.Highlight;
                case "HighlightText":
                    return Rgba.HighlightText;
                case "HotTrack":
                    return Rgba.HotTrack;
                case "InactiveBorder":
                    return Rgba.InactiveBorder;
                case "InactiveCaption":
                    return Rgba.InactiveCaption;
                case "InactiveCaptionText":
                    return Rgba.InactiveCaptionText;

                case "Info":
                    return Rgba.Info;
                case "InfoText":
                    return Rgba.InfoText;
                case "Menu":
                    return Rgba.Menu;
                case "MenuText":
                    return Rgba.MenuText;
                case "ScrollBar":
                    return Rgba.ScrollBar;
                case "Window":
                    return Rgba.Window;
                case "WindowFrame":
                    return Rgba.WindowFrame;
                case "WindowText":
                    return Rgba.WindowText;
                case "Transparent":
                    return Rgba.Transparent;
                case "AliceBlue":
                    return Rgba.AliceBlue;
                case "AntiqueWhite":
                    return Rgba.AntiqueWhite;
                case "Aqua":
                    return Rgba.Aqua;
                case "Aquamarine":
                    return Rgba.Aquamarine;
                case "Azure":
                    return Rgba.Azure;
                case "Beige":
                    return Rgba.Beige;
                case "Bisque":
                    return Rgba.Bisque;
                case "Black":
                    return Rgba.Black;
                case "BlanchedAlmond":
                    return Rgba.BlanchedAlmond;
                case "Blue":
                    return Rgba.Blue;
                case "BlueViolet":
                    return Rgba.BlueViolet;
                case "Brown":
                    return Rgba.Brown;
                case "BurlyWood":
                    return Rgba.BurlyWood;
                case "CadetBlue":
                    return Rgba.CadetBlue;
                case "Chartreuse":
                    return Rgba.Chartreuse;
                case "Chocolate":
                    return Rgba.Chocolate;
                case "Coral":
                    return Rgba.Coral;
                case "CornflowerBlue":
                    return Rgba.CornflowerBlue;
                case "Cornsilk":
                    return Rgba.Cornsilk;
                case "Crimson":
                    return Rgba.Crimson;
                case "Cyan":
                    return Rgba.Cyan;
                case "DarkBlue":
                    return Rgba.DarkBlue;
                case "DarkCyan":
                    return Rgba.DarkCyan;
                case "DarkGoldenrod":
                    return Rgba.DarkGoldenrod;
                case "DarkGray":
                    return Rgba.DarkGray;
                case "DarkGreen":
                    return Rgba.DarkGreen;
                case "DarkKhaki":
                    return Rgba.DarkKhaki;
                case "DarkMagenta":
                    return Rgba.DarkMagenta;
                case "DarkOliveGreen":
                    return Rgba.DarkOliveGreen;
                case "DarkOrange":
                    return Rgba.DarkOrange;
                case "DarkOrchid":
                    return Rgba.DarkOrchid;
                case "DarkRed":
                    return Rgba.DarkRed;
                case "DarkSalmon":
                    return Rgba.DarkSalmon;
                case "DarkSeaGreen":
                    return Rgba.DarkSeaGreen;
                case "DarkSlateBlue":
                    return Rgba.DarkSlateBlue;
                case "DarkSlateGray":
                    return Rgba.DarkSlateGray;
                case "DarkTurquoise":
                    return Rgba.DarkTurquoise;
                case "DarkViolet":
                    return Rgba.DarkViolet;
                case "DeepPink":
                    return Rgba.DeepPink;
                case "DeepSkyBlue":
                    return Rgba.DeepSkyBlue;
                case "DimGray":
                    return Rgba.DimGray;
                case "DodgerBlue":
                    return Rgba.DodgerBlue;
                case "Firebrick":
                    return Rgba.Firebrick;
                case "FloralWhite":
                    return Rgba.FloralWhite;
                case "ForestGreen":
                    return Rgba.ForestGreen;
                case "Fuchsia":
                    return Rgba.Fuchsia;
                case "Gainsboro":
                    return Rgba.Gainsboro;
                case "GhostWhite":
                    return Rgba.GhostWhite;
                case "Gold":
                    return Rgba.Gold;
                case "Goldenrod":
                    return Rgba.Goldenrod;
                case "Gray":
                    return Rgba.Gray;
                case "Green":
                    return Rgba.Green;
                case "GreenYellow":
                    return Rgba.GreenYellow;
                case "Honeydew":
                    return Rgba.Honeydew;
                case "HotPink":
                    return Rgba.HotPink;
                case "IndianRed":
                    return Rgba.IndianRed;
                case "Indigo":
                    return Rgba.Indigo;
                case "Ivory":
                    return Rgba.Ivory;
                case "Khaki":
                    return Rgba.Khaki;
                case "Lavender":
                    return Rgba.Lavender;
                case "LavenderBlush":
                    return Rgba.LavenderBlush;
                case "LawnGreen":
                    return Rgba.LawnGreen;
                case "LemonChiffon":
                    return Rgba.LemonChiffon;
                case "LightBlue":
                    return Rgba.LightBlue;
                case "LightCoral":
                    return Rgba.LightCoral;
                case "LightCyan":
                    return Rgba.LightCyan;
                case "LightGoldenrodYellow":
                    return Rgba.LightGoldenrodYellow;
                case "LightGray":
                    return Rgba.LightGray;
                case "LightGreen":
                    return Rgba.LightGreen;
                case "LightPink":
                    return Rgba.LightPink;
                case "LightSalmon":
                    return Rgba.LightSalmon;
                case "LightSeaGreen":
                    return Rgba.LightSeaGreen;
                case "LightSkyBlue":
                    return Rgba.LightSkyBlue;
                case "LightSlateGray":
                    return Rgba.LightSlateGray;
                case "LightSteelBlue":
                    return Rgba.LightSteelBlue;
                case "LightYellow":
                    return Rgba.LightYellow;
                case "Lime":
                    return Rgba.Lime;
                case "LimeGreen":
                    return Rgba.LimeGreen;
                case "Linen":
                    return Rgba.Linen;
                case "Magenta":
                    return Rgba.Magenta;
                case "Maroon":
                    return Rgba.Maroon;
                case "MediumAquamarine":
                    return Rgba.MediumAquamarine;
                case "MediumBlue":
                    return Rgba.MediumBlue;
                case "MediumOrchid":
                    return Rgba.MediumOrchid;
                case "MediumPurple":
                    return Rgba.MediumPurple;
                case "MediumSeaGreen":
                    return Rgba.MediumSeaGreen;
                case "MediumSlateBlue":
                    return Rgba.MediumSlateBlue;
                case "MediumSpringGreen":
                    return Rgba.MediumSpringGreen;
                case "MediumTurquoise":
                    return Rgba.MediumTurquoise;
                case "MediumVioletRed":
                    return Rgba.MediumVioletRed;
                case "MidnightBlue":
                    return Rgba.MidnightBlue;
                case "MintCream":
                    return Rgba.MintCream;
                case "MistyRose":
                    return Rgba.MistyRose;
                case "Moccasin":
                    return Rgba.Moccasin;
                case "NavajoWhite":
                    return Rgba.NavajoWhite;
                case "Navy":
                    return Rgba.Navy;
                case "OldLace":
                    return Rgba.OldLace;
                case "Olive":
                    return Rgba.Olive;
                case "OliveDrab":
                    return Rgba.OliveDrab;
                case "Orange":
                    return Rgba.Orange;
                case "OrangeRed":
                    return Rgba.OrangeRed;
                case "Orchid":
                    return Rgba.Orchid;
                case "PaleGoldenrod":
                    return Rgba.PaleGoldenrod;
                case "PaleGreen":
                    return Rgba.PaleGreen;
                case "PaleTurquoise":
                    return Rgba.PaleTurquoise;
                case "PaleVioletRed":
                    return Rgba.PaleVioletRed;
                case "PapayaWhip":
                    return Rgba.PapayaWhip;
                case "PeachPuff":
                    return Rgba.PeachPuff;
                case "Peru":
                    return Rgba.Peru;
                case "Pink":
                    return Rgba.Pink;
                case "Plum":
                    return Rgba.Plum;
                case "PowderBlue":
                    return Rgba.PowderBlue;
                case "Purple":
                    return Rgba.Purple;
                case "Red":
                    return Rgba.Red;
                case "RosyBrown":
                    return Rgba.RosyBrown;
                case "RoyalBlue":
                    return Rgba.RoyalBlue;
                case "SaddleBrown":
                    return Rgba.SaddleBrown;
                case "Salmon":
                    return Rgba.Salmon;
                case "SandyBrown":
                    return Rgba.SandyBrown;
                case "SeaGreen":
                    return Rgba.SeaGreen;
                case "SeaShell":
                    return Rgba.SeaShell;
                case "Sienna":
                    return Rgba.Sienna;
                case "Silver":
                    return Rgba.Silver;
                case "SkyBlue":
                    return Rgba.SkyBlue;
                case "SlateBlue":
                    return Rgba.SlateBlue;
                case "SlateGray":
                    return Rgba.SlateGray;
                case "Snow":
                    return Rgba.Snow;
                case "SpringGreen":
                    return Rgba.SpringGreen;
                case "SteelBlue":
                    return Rgba.SteelBlue;
                case "Tan":
                    return Rgba.Tan;
                case "Teal":
                    return Rgba.Teal;
                case "Thistle":
                    return Rgba.Thistle;
                case "Tomato":
                    return Rgba.Tomato;
                case "Turquoise":
                    return Rgba.Turquoise;
                case "Violet":
                    return Rgba.Violet;
                case "Wheat":
                    return Rgba.Wheat;
                case "White":
                    return Rgba.White;
                case "WhiteSmoke":
                    return Rgba.WhiteSmoke;
                case "Yellow":
                    return Rgba.Yellow;
                case "YellowGreen":
                    return Rgba.YellowGreen;
                case "ButtonFace":
                    return Rgba.ButtonFace;
                case "ButtonHighlight":
                    return Rgba.ButtonHighlight;
                case "ButtonShadow":
                    return Rgba.ButtonShadow;
                case "GradientActiveCaption":
                    return Rgba.GradientActiveCaption;
                case "GradientInactiveCaption":
                    return Rgba.GradientInactiveCaption;
                case "MenuBar":
                    return Rgba.MenuBar;
                case "MenuHighlight":
                    return Rgba.MenuHighlight;
                default:
                    return Rgba.Empty;
            }
        }
        #endregion
    }
#endif
}
