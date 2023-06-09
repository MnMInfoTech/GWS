/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MnM.GWS
{
    public interface IRgb : IColour
    {
        byte R { get; }
        byte G { get; }
        byte B { get; }
    }
    public interface IRgba : IRgb, IPen, ICloneable, IEquatable<IRgba>, IValid
    { 
        byte A { get; }
    }

    public struct Rgba : IRgba, IProperty
    {
        #region VARAIBLES
        /// <summary>
        /// Interger  4 channels colour value.
        /// </summary>
        public readonly int Colour;

        const string toStr = "C: {0}, R: {1}, G: {2}, B: {3}, A: {4}";
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new Colour structure with Red, Green, Blue and Alpha components.
        /// </summary>
        /// <param name="r">Red component</param>
        /// <param name="g">Green component</param>
        /// <param name="b">Blue component</param>
        /// <param name="a">Alpha component</param>
        public Rgba(byte r, byte g, byte b, byte a) : this()
        {
            Colour = (a << Colours.AShift)
                 | ((byte)((r) & 0xFF) << Colours.RShift)
                 | ((byte)((g) & 0xFF) << Colours.GShift)
                 | ((byte)((b) & 0xFF) << Colours.BShift);
        }
        public Rgba(int value) : this()
        {
            Colour = value;
        }
        public Rgba(byte r, byte g, byte b) :
            this(r, g, b, (byte)255)
        { }
        public Rgba(int r, int g, int b, int a = 255) :
            this((byte)r, (byte)g, (byte)b, (byte)a)
        { }

        public Rgba(int value, byte externalAlpha)
        {
            Colour = (externalAlpha << Colours.AShift) | (value & Colours.Inversion);
        }
        public Rgba(IRgba c, byte newAlpha) :
            this(c.R, c.G, c.B, newAlpha)
        { }
        public Rgba(IRgba c, float newAlpha) :
            this(c.R, c.G, c.B, (byte)(newAlpha * 255))
        { }
        public Rgba(IColour colour) : 
            this(colour.Colour)
        {
        }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Red component of this colour.
        /// </summary>
        public byte R => (byte)((Colour >> Colours.RShift) & 0xFF);

        /// <summary>
        /// Green component of this colour.
        /// </summary>
        public byte G => (byte)((Colour >> Colours.GShift) & 0xFF);

        /// <summary>
        /// Blue component of this colour.
        /// </summary>
        public byte B => (byte)((Colour >> Colours.BShift) & 0xFF);

        /// <summary>
        /// Alpha component of this colour.
        /// </summary>
        public byte A => (byte)((Colour >> Colours.AShift) & 0xFF);
        int IColour.Colour => Colour;
        byte IRgb.R => R;
        byte IRgb.G => G;
        byte IRgb.B => B;
        byte IRgba.A => A;
        object IValue.Value => this;
        bool IValid.Valid => Colour != 0;
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

        #region READ PIXEL
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadPixel(int x, int y) => Colour;
        #endregion

        #region READLINE/S
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void ReadLine(int start, int end, int axis,
            bool horizontal, out int[] pixels, out int srcIndex, out int copyLength)
        {
            pixels = new int[0];
            srcIndex = 0;
            if (end < start)
            {
                int temp = start;
                start = end;
                end = temp;
            }
            copyLength = end - start;
            if (copyLength < 0)
                goto mks;
            if (copyLength == 0)
                copyLength = 1;

            pixels = new int[copyLength + 1];
            fixed (int* d = pixels)
            {
                for (int i = 0; i < copyLength; i++)
                    d[i] = Colour;
            }
            return;
        mks:
            copyLength = 0;
        }
        #endregion

        #region COPY TO
        public unsafe Task<ISize> CopyTo(IntPtr dest, int dstLen, int dstW,
            IEnumerable<IParameter> parameters = null)
        {
            int* dst = (int*)dest;
            int dstH = dstLen / dstW;

            #region EXTRACT PARAMETRS
            parameters.Extract(out IExSession info);
            var dstX = info.UserPoint?.X ?? 0;
            var dstY = info.UserPoint?.Y ?? 0;
            #endregion

            if (info.CopyArea == null)
                info.CopyArea = new Rectangle(dstX, dstY, dstW, dstH);

            info.CopyArea.GetBounds(out int x, out int y, out int w, out int h);

#if Advance
            if (info.Clip != null && info.Clip.Valid)
            {
                info.Clip.GetBounds(out int clipX, out int clipY, out int clipW, out int clipH);
                int clipR = clipX + clipW;
                int clipB = clipY + clipH;

                if (dstX > clipR && dstY > clipB)
                    return Task.FromResult((ISize)Size.Empty);

                if (dstY < clipY || dstY > clipB || dstX > clipR)
                    return Task.FromResult((ISize)Size.Empty);

                var dstEX = dstX + w;
                if (dstX < clipX)
                    dstX = clipX;
                if (dstEX > clipR)
                    dstEX = clipR;
                w = dstEX - dstX;
            }
#endif
            if (w < 0)
                return Task.FromResult((ISize)Size.Empty);

            var right = x + w;
            var bottom = y + h;

            if (y < 0)
            {
                bottom += y;
                y = 0;
            }

            int destIndex = dstX + dstY * dstW;
            int i = 0;
            int[] source = this.Repeat(w, (info.Command & Command.InvertColour) == Command.InvertColour);
            int srcIndex = 0;
            var cmd = info.Command.ToEnum<CopyCommand>();
            fixed (int* src = source)
            {
                while (y < bottom)
                {
                    if (destIndex + w - 1 >= dstLen)
                        break;
                    Blocks.Copy(src, srcIndex, dst, destIndex, w, cmd);
                    destIndex += dstW;
                    ++i;
                    ++y;
                }
            }
            return Task.FromResult((ISize)new Size(right - x, dstY - y));
        }
        #endregion

        #region CLONE
        object ICloneable.Clone()
        {
            return new Rgba(Colour);
        }
        #endregion

        #region EQUALITY
        public override bool Equals(object other)
        {
            if (!(other is IRgba))
                return false;

            var c = (IRgba)other;
            return c.Colour == Colour;
        }
        public bool Equals(IRgba c)
        {
            return c.Colour == Colour;
        }
        public override int GetHashCode()
        {
            return Colour;
        }
        #endregion

        #region PREDEFINED COLORS
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
        public static Rgba Grey { get; private set; }
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

        /// <summary>
        /// Retrieves a default disabled pen available in GWS.
        /// </summary>
        public static Rgba DisabledBackground { get; private set; }

        /// <summary>
        /// Retrieves a default disabled pen available in GWS.
        /// </summary>
        public static Rgba DisabledForeground { get; private set; }

        /// <summary>
        /// Retrieves a default background pen available in GWS.
        /// </summary>
        public static Rgba Background { get; private set; }

        /// <summary>
        /// Retrieves a default foreground pen available in GWS.
        /// </summary>
        public static Rgba Foreground { get; private set; }

        /// <summary>
        /// Retrieves a default hover background pen available in GWS.
        /// </summary>
        public static Rgba HoverBackground { get; private set; }

        /// <summary>
        /// Retrieves a default hover foreground pen available in GWS.
        /// </summary>
        public static Rgba HoverForeground { get; private set; }
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
        public static Rgba operator +(Rgba A, int B)
        {
            var r = A.R + B;
            var g = A.G + B;
            var b = A.B + B;
            var a = A.A + B;
            return new Rgba(r, g, b, a);
        }
        public static Rgba operator +(int A, Rgba B)
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
        public static Rgba operator -(Rgba A, int B)
        {
            return A + (-B);
        }
        public static Rgba operator -(int A, Rgba B)
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
        public static Rgba operator *(Rgba A, int B)
        {
            var r = A.R * B;
            var g = A.G * B;
            var b = A.B * B;
            var a = A.A * B;
            return new Rgba(r, g, b, a);
        }
        public static Rgba operator *(int A, Rgba B)
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
        public static Rgba operator /(Rgba A, int B)
        {
            var r = A.R / B;
            var g = A.G / B;
            var b = A.B / B;
            var a = A.A / B;
            return new Rgba(r, g, b, a);
        }
        public static Rgba Divide(int A, Rgba B)
        {
            return B / (A);
        }
        #endregion

        #region CONVERSION OPERATORS
        public static implicit operator int(Rgba colour) =>
            colour.Colour;

        public static implicit operator Rgba(int value) =>
            new Rgba(value);

        #endregion

        #region RESET
        internal static void Reset()
        {
            Empty = new Rgba(0, 0, 0, 0);
            ActiveBorder = new Rgba(180, 180, 180, 255);
            ActiveCaption = new Rgba(209, 180, 153, 255);
            ActiveCaptionText = new Rgba(0, 0, 0, 255);
            AliceBlue = new Rgba(255, 248, 240, 255);
            AntiqueWhite = new Rgba(215, 235, 250, 255);
            AppWorkspace = new Rgba(171, 171, 171, 255);
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
            ButtonFace = new Rgba(240, 240, 240, 255);
            ButtonHighlight = new Rgba(255, 255, 255, 255);
            ButtonShadow = new Rgba(160, 160, 160, 255);
            CadetBlue = new Rgba(160, 158, 95, 255);
            Chartreuse = new Rgba(0, 255, 127, 255);
            Chocolate = new Rgba(30, 105, 210, 255);
            Control = new Rgba(240, 240, 240, 255);
            ControlDark = new Rgba(160, 160, 160, 255);
            ControlDarkDark = new Rgba(105, 105, 105, 255);
            ControlLight = new Rgba(227, 227, 227, 255);
            ControlLightLight = new Rgba(255, 255, 255, 255);
            ControlText = new Rgba(0, 0, 0, 255);
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
            Desktop = new Rgba(0, 0, 0, 255);
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
            GradientActiveCaption = new Rgba(234, 209, 185, 255);
            GradientInactiveCaption = new Rgba(242, 228, 215, 255);
            Gray = new Rgba(128, 128, 128, 255);
            Grey = new Rgba(128, 128, 128, 255);
            GrayText = new Rgba(109, 109, 109, 255);
            Green = new Rgba(0, 128, 0, 255);
            GreenYellow = new Rgba(47, 255, 173, 255);
            Highlight = new Rgba(255, 153, 51, 255);
            HighlightText = new Rgba(255, 255, 255, 255);
            Honeydew = new Rgba(240, 255, 240, 255);
            HotPink = new Rgba(180, 105, 255, 255);
            HotTrack = new Rgba(204, 102, 0, 255);
            InactiveBorder = new Rgba(252, 247, 244, 255);
            InactiveCaption = new Rgba(219, 205, 191, 255);
            InactiveCaptionText = new Rgba(0, 0, 0, 255);
            IndianRed = new Rgba(92, 92, 205, 255);
            Indigo = new Rgba(130, 0, 75, 255);
            Info = new Rgba(225, 255, 255, 255);
            InfoText = new Rgba(0, 0, 0, 255);
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
            Menu = new Rgba(240, 240, 240, 255);
            MenuBar = new Rgba(240, 240, 240, 255);
            MenuHighlight = new Rgba(255, 153, 51, 255);
            MenuText = new Rgba(0, 0, 0, 255);
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
            ScrollBar = new Rgba(200, 200, 200, 255);
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
            Transparent = new Rgba(16777215);
            Turquoise = new Rgba(208, 224, 64, 255);
            Violet = new Rgba(238, 130, 238, 255);
            Wheat = new Rgba(179, 222, 245, 255);
            White = new Rgba(255, 255, 255, 255);
            WhiteSmoke = new Rgba(245, 245, 245, 255);
            Window = new Rgba(255, 255, 255, 255);
            WindowFrame = new Rgba(100, 100, 100, 255);
            WindowText = new Rgba(0, 0, 0, 255);
            Yellow = new Rgba(0, 255, 255, 255);
            YellowGreen = new Rgba(50, 205, 154, 255);

            DisabledBackground = Silver;
            DisabledForeground = DarkGray;
            Background = Transparent;// new Rgba(-4144960);
            Foreground = Black;
            HoverBackground =  Navy;
            HoverForeground = LightSkyBlue;
        }
        #endregion

        #region FROM NAME
        public static Rgba FromName(string name)
        {
            switch (name)
            {
                case "ActiveBorder":
                    return ActiveBorder;
                case "ActiveCaption":
                    return ActiveCaption;
                case "ActiveCaptionText":
                    return ActiveCaptionText;
                case "AppWorkspace":
                    return AppWorkspace;
                case "Control":
                    return Control;
                case "ControlDark":
                    return ControlDark;
                case "ControlDarkDark":
                    return ControlDarkDark;
                case "ControlLight":
                    return ControlLight;
                case "ControlLightLight":
                    return ControlLightLight;
                case "ControlText":
                    return ControlText;
                case "Desktop":
                    return Desktop;
                case "GrayText":
                    return GrayText;
                case "Highlight":
                    return Highlight;
                case "HighlightText":
                    return HighlightText;
                case "HotTrack":
                    return HotTrack;
                case "InactiveBorder":
                    return InactiveBorder;
                case "InactiveCaption":
                    return InactiveCaption;
                case "InactiveCaptionText":
                    return InactiveCaptionText;

                case "Info":
                    return Info;
                case "InfoText":
                    return InfoText;
                case "Menu":
                    return Menu;
                case "MenuText":
                    return MenuText;
                case "ScrollBar":
                    return ScrollBar;
                case "Window":
                    return Window;
                case "WindowFrame":
                    return WindowFrame;
                case "WindowText":
                    return WindowText;
                case "Transparent":
                    return Transparent;
                case "AliceBlue":
                    return AliceBlue;
                case "AntiqueWhite":
                    return AntiqueWhite;
                case "Aqua":
                    return Aqua;
                case "Aquamarine":
                    return Aquamarine;
                case "Azure":
                    return Azure;
                case "Beige":
                    return Beige;
                case "Bisque":
                    return Bisque;
                case "Black":
                    return Black;
                case "BlanchedAlmond":
                    return BlanchedAlmond;
                case "Blue":
                    return Blue;
                case "BlueViolet":
                    return BlueViolet;
                case "Brown":
                    return Brown;
                case "BurlyWood":
                    return BurlyWood;
                case "CadetBlue":
                    return CadetBlue;
                case "Chartreuse":
                    return Chartreuse;
                case "Chocolate":
                    return Chocolate;
                case "Coral":
                    return Coral;
                case "CornflowerBlue":
                    return CornflowerBlue;
                case "Cornsilk":
                    return Cornsilk;
                case "Crimson":
                    return Crimson;
                case "Cyan":
                    return Cyan;
                case "DarkBlue":
                    return DarkBlue;
                case "DarkCyan":
                    return DarkCyan;
                case "DarkGoldenrod":
                    return DarkGoldenrod;
                case "DarkGray":
                    return DarkGray;
                case "DarkGreen":
                    return DarkGreen;
                case "DarkKhaki":
                    return DarkKhaki;
                case "DarkMagenta":
                    return DarkMagenta;
                case "DarkOliveGreen":
                    return DarkOliveGreen;
                case "DarkOrange":
                    return DarkOrange;
                case "DarkOrchid":
                    return DarkOrchid;
                case "DarkRed":
                    return DarkRed;
                case "DarkSalmon":
                    return DarkSalmon;
                case "DarkSeaGreen":
                    return DarkSeaGreen;
                case "DarkSlateBlue":
                    return DarkSlateBlue;
                case "DarkSlateGray":
                    return DarkSlateGray;
                case "DarkTurquoise":
                    return DarkTurquoise;
                case "DarkViolet":
                    return DarkViolet;
                case "DeepPink":
                    return DeepPink;
                case "DeepSkyBlue":
                    return DeepSkyBlue;
                case "DimGray":
                    return DimGray;
                case "DodgerBlue":
                    return DodgerBlue;
                case "Firebrick":
                    return Firebrick;
                case "FloralWhite":
                    return FloralWhite;
                case "ForestGreen":
                    return ForestGreen;
                case "Fuchsia":
                    return Fuchsia;
                case "Gainsboro":
                    return Gainsboro;
                case "GhostWhite":
                    return GhostWhite;
                case "Gold":
                    return Gold;
                case "Goldenrod":
                    return Goldenrod;
                case "Gray":
                    return Gray;
                case "Green":
                    return Green;
                case "GreenYellow":
                    return GreenYellow;
                case "Honeydew":
                    return Honeydew;
                case "HotPink":
                    return HotPink;
                case "IndianRed":
                    return IndianRed;
                case "Indigo":
                    return Indigo;
                case "Ivory":
                    return Ivory;
                case "Khaki":
                    return Khaki;
                case "Lavender":
                    return Lavender;
                case "LavenderBlush":
                    return LavenderBlush;
                case "LawnGreen":
                    return LawnGreen;
                case "LemonChiffon":
                    return LemonChiffon;
                case "LightBlue":
                    return LightBlue;
                case "LightCoral":
                    return LightCoral;
                case "LightCyan":
                    return LightCyan;
                case "LightGoldenrodYellow":
                    return LightGoldenrodYellow;
                case "LightGray":
                    return LightGray;
                case "LightGreen":
                    return LightGreen;
                case "LightPink":
                    return LightPink;
                case "LightSalmon":
                    return LightSalmon;
                case "LightSeaGreen":
                    return LightSeaGreen;
                case "LightSkyBlue":
                    return LightSkyBlue;
                case "LightSlateGray":
                    return LightSlateGray;
                case "LightSteelBlue":
                    return LightSteelBlue;
                case "LightYellow":
                    return LightYellow;
                case "Lime":
                    return Lime;
                case "LimeGreen":
                    return LimeGreen;
                case "Linen":
                    return Linen;
                case "Magenta":
                    return Magenta;
                case "Maroon":
                    return Maroon;
                case "MediumAquamarine":
                    return MediumAquamarine;
                case "MediumBlue":
                    return MediumBlue;
                case "MediumOrchid":
                    return MediumOrchid;
                case "MediumPurple":
                    return MediumPurple;
                case "MediumSeaGreen":
                    return MediumSeaGreen;
                case "MediumSlateBlue":
                    return MediumSlateBlue;
                case "MediumSpringGreen":
                    return MediumSpringGreen;
                case "MediumTurquoise":
                    return MediumTurquoise;
                case "MediumVioletRed":
                    return MediumVioletRed;
                case "MidnightBlue":
                    return MidnightBlue;
                case "MintCream":
                    return MintCream;
                case "MistyRose":
                    return MistyRose;
                case "Moccasin":
                    return Moccasin;
                case "NavajoWhite":
                    return NavajoWhite;
                case "Navy":
                    return Navy;
                case "OldLace":
                    return OldLace;
                case "Olive":
                    return Olive;
                case "OliveDrab":
                    return OliveDrab;
                case "Orange":
                    return Orange;
                case "OrangeRed":
                    return OrangeRed;
                case "Orchid":
                    return Orchid;
                case "PaleGoldenrod":
                    return PaleGoldenrod;
                case "PaleGreen":
                    return PaleGreen;
                case "PaleTurquoise":
                    return PaleTurquoise;
                case "PaleVioletRed":
                    return PaleVioletRed;
                case "PapayaWhip":
                    return PapayaWhip;
                case "PeachPuff":
                    return PeachPuff;
                case "Peru":
                    return Peru;
                case "Pink":
                    return Pink;
                case "Plum":
                    return Plum;
                case "PowderBlue":
                    return PowderBlue;
                case "Purple":
                    return Purple;
                case "Red":
                    return Red;
                case "RosyBrown":
                    return RosyBrown;
                case "RoyalBlue":
                    return RoyalBlue;
                case "SaddleBrown":
                    return SaddleBrown;
                case "Salmon":
                    return Salmon;
                case "SandyBrown":
                    return SandyBrown;
                case "SeaGreen":
                    return SeaGreen;
                case "SeaShell":
                    return SeaShell;
                case "Sienna":
                    return Sienna;
                case "Silver":
                    return Silver;
                case "SkyBlue":
                    return SkyBlue;
                case "SlateBlue":
                    return SlateBlue;
                case "SlateGray":
                    return SlateGray;
                case "Snow":
                    return Snow;
                case "SpringGreen":
                    return SpringGreen;
                case "SteelBlue":
                    return SteelBlue;
                case "Tan":
                    return Tan;
                case "Teal":
                    return Teal;
                case "Thistle":
                    return Thistle;
                case "Tomato":
                    return Tomato;
                case "Turquoise":
                    return Turquoise;
                case "Violet":
                    return Violet;
                case "Wheat":
                    return Wheat;
                case "White":
                    return White;
                case "WhiteSmoke":
                    return WhiteSmoke;
                case "Yellow":
                    return Yellow;
                case "YellowGreen":
                    return YellowGreen;
                case "ButtonFace":
                    return ButtonFace;
                case "ButtonHighlight":
                    return ButtonHighlight;
                case "ButtonShadow":
                    return ButtonShadow;
                case "GradientActiveCaption":
                    return GradientActiveCaption;
                case "GradientInactiveCaption":
                    return GradientInactiveCaption;
                case "MenuBar":
                    return MenuBar;
                case "MenuHighlight":
                    return MenuHighlight;
                default:
                    return Empty;
            }
        }
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, Colour, R, G, B, A);
        }
    }
}
