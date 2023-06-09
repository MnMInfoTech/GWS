/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    /// <summary>
    /// Represents an object which provides a certain list of colours to form a spectrum using a specified stop positions.
    /// </summary>
    public interface IBrushStyle: IReadOnlyList<int>, IPenContext, ICloneable
    {
        /// <summary>
        /// Kind of gradient that colours should generate.
        /// Usually one may pick on value from Gradient enum for native GWS gradients or define their own and handle it in spectrum and brush class on their own.
        /// </summary>
        sbyte Gradient { get; }

        /// <summary>
        /// Gets the first colour in this style.
        /// </summary>
        int StartColour { get; }

        /// <summary>
        /// Gets the last colour in this style.
        /// </summary>
        int EndColour { get; }

        /// <summary>
        /// Get a colour on a given position assuming the size of spectrum specified by the size parameter.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="Span">Size to be used to calculate colour</param>
        /// <param name="invert">specifies if calculation should be done in reverse order.</param>
        /// <returns>Colour</returns>
        int GetColour(float position, float Span, bool invert = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w">Width of the spectrum to be generated.</param>
        /// <param name="h">Height of the spectrum to be generated.</param>
        /// <param name="Data">Colours providing sample of spectrum.</param>
        /// <param name="length">Length of the spectrum.</param>
        /// <param name="radial">Indicates if spectrum is radial and as such requires additional calculation of circular distances.</param>
        /// <returns>True if this style has gradient among the predefined gradients otherwise false.
        /// In case of user supplied gradient where a false result is returned, 
        /// task of getting data and length must be handled by the user.</returns>
        bool GetColours(int w, int h, out int[] Data, out int length, out bool radial);
    }

    /// <summary>
    /// Represents an object which provides a certain list of colours to form a spectrum using a specified stop positions.
    /// </summary>
    public partial struct BrushStyle : IBrushStyle, IEquatable<BrushStyle>, IProperty
    {
        #region VARIABLES   
        /// <summary>
        /// Number of positions.
        /// </summary>
        public readonly int PositionCount;

        /// <summary>
        /// Kind of gradient that colours should generate.
        /// Usually one may pick on value from Gradient enum for native GWS gradients or define their own and handle it in spectrum and brush class on their own.
        /// </summary>
        public readonly sbyte Gradient;

        readonly int[] SpectrumColours;
        readonly int[] Positions;

        byte valid;

        const string toString = "{0}.{1}";
        public readonly static BrushStyle Empty =
            new BrushStyle(BrushType.Horizontal, Rgba.Black, Rgba.DimGray);

        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new fill style with specified gradient and values of colours to use.
        /// </summary>
        /// <param name="colours">Colours represented by integer values</param>
        /// <param name="gradient">This determines which gradient style to use</param>
        /// <param name="stops">Colour stops to use while navigating a colour spectrum offered by the style</param>
        public BrushStyle(int[] colours, sbyte gradient, IList<int> stops, bool matchSize = true)
        {
            Positions = null;
            SpectrumColours = null;
            valid = 1;
            switch (colours.Length)
            {
                case 0:
                    SpectrumColours = new int[] { (int)Rgba.Black, (int)Rgba.Silver, (int)Rgba.Gray };
                    break;
                case 1:
                    SpectrumColours = new int[] { colours[0], colours[0].Darken(0.25f), colours[0].Darken(0.5f) };
                    break;
                case 2:
                    SpectrumColours = new int[] { colours[0], Colours.Blend(colours[0], colours[1], 127), colours[1] };
                    break;
                default:
                    SpectrumColours = colours;
                    break;
            }

            Gradient = gradient;
            if (stops?.Count > 0)
                Positions = stops.ToArray();
            PositionCount = Positions?.Length ?? 0;
        }

        /// <summary>
        /// Creates a new fill style with specified gradient and values of colours to use.
        /// </summary>
        /// <param name="colours">Colours represented by Rgba struct values</param>
        /// <param name="gradient">This determines which gradient style to use</param>
        /// <param name="stops">Colour stops to use while navigating a colour spectrum offered by the style</param>
        public BrushStyle(IColour[] colours, sbyte gradient, IList<int> stops, bool matchSize = true) :
            this(colours.Select(c => c.Colour).ToArray(), gradient, stops, matchSize)
        { }

        /// <summary>
        /// Creates a new fill style with specified gradient and values of colours to use.
        /// </summary>
        /// <param name="gradient">This determines which gradient style to use</param>
        /// <param name="colours">Colours represented by integer values</param>
        public BrushStyle(sbyte gradient, params int[] colours) :
            this(colours, gradient, null)
        { }

        /// <summary>
        /// Creates a new fill style with specified values of colours to use.
        /// </summary>
        /// <param name="colours">Colours represented by integer values</param>
        public BrushStyle(params int[] values) :
            this(values, BrushType.Horizontal, null)
        { }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Calculated colour at a given index.
        /// </summary>
        /// <param name="index">Index to get a calculated colour</param>
        /// <returns></returns>
        public int this[int index] =>
            SpectrumColours[index];

        /// <summary>
        /// numenr of colours in this style.
        /// </summary>
        public int Count => SpectrumColours.Length;

        /// <summary>
        /// Gets the first colour in list.
        /// </summary>
        public int StartColour =>
            SpectrumColours[0];

        /// <summary>
        /// Gets the last colour in list.
        /// </summary>
        public int EndColour =>
            SpectrumColours[SpectrumColours.Length - 1];

        public bool Valid => valid != 0;

        sbyte IBrushStyle.Gradient => Gradient;
        object IValue.Value => this;
        #endregion

        #region EQUALITY
        public static bool operator ==(BrushStyle fs1, BrushStyle fs2) =>
            fs1.Equals(fs2);
        public static bool operator !=(BrushStyle fs1, BrushStyle fs2) =>
            !fs1.Equals(fs2);

        public override int GetHashCode() =>
            new { Gradient, SpectrumColours }.GetHashCode();
        public override bool Equals(object obj)
        {
            if (!(obj is BrushStyle))
                return false;

            return Equals((BrushStyle)obj);
        }
        public bool Equals(BrushStyle other) =>
            other.GetHashCode() == GetHashCode();
        #endregion

        #region GET POSITION
        /// <summary>
        /// Gets a position at a given index. It can then be used to get a calculated colour at a given position.
        /// </summary>
        /// <param name="index">Index in a position array</param>
        /// <returns></returns>
        public int Position(int index) =>
            Positions[index];
        #endregion

        #region CHANGE - INVERT
        /// <summary>
        /// Gives a new style with changed gradient.
        /// </summary>
        /// <param name="gradient">A new gradient for which the stle is required for.</param>
        /// <returns></returns>
        public BrushStyle Change(sbyte gradient)
        {
            return new BrushStyle(SpectrumColours, gradient, Positions);
        }
        /// <summary>
        /// gives a new copy of this style but with inverted order of colours.
        /// </summary>
        /// <returns></returns>
        public BrushStyle Invert()
        {
            var colours = new int[SpectrumColours.Length];
            SpectrumColours.CopyTo(colours, 0);
            Array.Reverse(colours);
            if (Positions != null)
                return new BrushStyle(colours, Gradient, Positions);
            else
                return new BrushStyle(colours, Gradient, null);
        }
        #endregion

        #region GET COLOR
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetColour(float position, float Span, bool invert = false)
        {
            int colour;
            int colour1, colour2;
            uint alpha, invAlpha, c1, c2, rb, ag;
            int PosCount = PositionCount;

            if (Span == 0 || position <= 0)
            {
                colour = SpectrumColours[0];
                goto assignColour;
            }
            if (position == -1 || Span == -1)
            {
                colour = EndColour;
                goto assignColour;
            }

            #region WORK OUT 2 COLORS

            if (Count < 3 || Span == 0)
            {
                colour1 = SpectrumColours[0];
                colour2 = EndColour;
                goto Blend;
            }
            float pos = Span / (Count - 1);
            if (pos == 0)
                pos = 1;
            int f = 0;
            if (PosCount > 0)
            {
                for (int g = 0; g < PosCount; g++)
                {
                    if (Positions[g] > pos)
                        break;
                    f = g;
                }
            }
            else
            {
                f = (int)(position / pos);
            }

            position %= pos;
            if (f >= Count)
                f = 0;
            if (f < 0)
                f = 0;

            colour1 = SpectrumColours[f];
            ++f;
            if (f > Count - 1)
                colour1 = SpectrumColours[--f];

            colour2 = SpectrumColours[f];
            Span = (float)Math.Ceiling(pos);
        #endregion

        Blend:
            #region BLEND
            if (invert)
            {
                var temp = colour1;
                colour1 = colour2;
                colour2 = temp;
            }

            float delta = position / Span;
            alpha = (uint)(delta * 255);
            if (alpha == 0)
            {
                colour = colour1;
                goto assignColour;
            }

            if (alpha == 255)
            {
                colour = colour2;
                goto assignColour;
            }
            c1 = (uint)colour1;
            c2 = (uint)colour2;
            invAlpha = 255 - alpha;
            rb = ((invAlpha * (c1 & Colours.RBMASK)) + (alpha * (c2 & Colours.RBMASK))) >> 8;
            ag = (invAlpha * ((c1 & Colours.AGMASK) >> 8)) + (alpha * (Colours.ONEALPHA | ((c2 & Colours.GMASK) >> 8)));

            colour = (int)((rb & Colours.RBMASK) | (ag & Colours.AGMASK));
        #endregion

        assignColour:
            return colour;
        }
        #endregion

        #region GET COLORS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetColours(int w, int h, out int[] Data, out int length, out bool radial)
        {
            Data = new int[0];
            radial = Gradient == BrushType.Elliptical || Gradient == BrushType.MiddleCircular; 
            var width = w;
            var height = h;
            VectorF center = new VectorF(width / 2f, height / 2f);

            switch (Gradient)
            {
                case BrushType.Solid:
                case BrushType.Horizontal:
                    length = width + 1;
                    break;
                case BrushType.HorizontalCentral:
                    length = width + 1;
                    break;
                case BrushType.Vertical:
                    length = height + 1;
                    break;

                case BrushType.VerticalCentral:
                    length = height + 1;
                    break;

                case BrushType.ForwardDiagonal:
                case BrushType.BackwardDiagonal:
                case BrushType.DiagonalCentral:
                    length = (width + height) + 1;
                    break;

                case BrushType.HorizontalSwitch:
                    length = (width) * 2 + 1;
                    break;

                case BrushType.Circular:
                    length = Math.Max(width, height) / 2 + 1;
                    break;

                case BrushType.Rectangular:
                    length = Math.Min(width, height) + 1;
                    break;

                case BrushType.Elliptical:
                case BrushType.MiddleCircular:
                    length = (int)Math.Ceiling(center.Length());
                    break;
                case BrushType.Conical:
                case BrushType.Conical2:
                    length = 361;
                    break;
                default:
                    length = w * h;
                    return false;
            }

            if (Gradient == BrushType.Solid)
            {
                Data = GetColour(0, 1, false).Repeat(length);
                return true;
            }

            bool centerToLeftRight = (Gradient > BrushType.BackwardDiagonal && Gradient < BrushType.Conical2) ||
                Gradient == BrushType.Rectangular;
            bool linear = Gradient > BrushType.Solid && Gradient < 11 ||
                Gradient == BrushType.Rectangular || Gradient == BrushType.Circular;

            if (linear || radial)
            {
                Data = new int[length];
                if (Data.Length == 0)
                    return true;

                int start = 0;
                int end = length, max = length;

                if (centerToLeftRight)
                {
                    end = (int)(end / 2f) + 1;
                    max = (int)(max / 2f) + 1;
                }
                int len = Data.Length;

                for (int i = start; i < end; i++)
                {
                    int idx = i - start;
                    Data[idx] = GetColour(i, max);

                    if (centerToLeftRight)
                        Data[len - 1 - idx] = Data[idx];
                }
            }
            return true;
        }
        #endregion

        #region OPERATORS
        public static implicit operator bool(BrushStyle b) =>
            b.valid != 0;
        #endregion

        #region CLONE
        public BrushStyle Clone()
        {
            if (SpectrumColours == null)
                return BrushStyle.Empty;
            BrushStyle f = new BrushStyle(SpectrumColours.ToArray(), Gradient, Positions);
            return f;
        }
        object ICloneable.Clone() =>
            Clone();
        #endregion

        public override string ToString()
        {
            return string.Format(toString, Gradient, SpectrumColours.GetHashCode());
        }

        #region IENUMERABLE
        public IEnumerator<int> GetEnumerator()
        {
            foreach (var item in SpectrumColours)
            {
                yield return item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion
    }

    partial struct BrushStyle
    {
        #region PREDEFINED FILL STYLES
        public static BrushStyle Default { get; private set; }
        public static BrushStyle Blue { get; private set; }
        public static BrushStyle Selection { get; private set; }
        public static BrushStyle LightSelection { get; private set; }
        public static BrushStyle OrangeSelection { get; private set; }
        public static BrushStyle Black { get; private set; }
        public static BrushStyle WindowFrame { get; private set; }
        public static BrushStyle Silver { get; private set; }
        public static BrushStyle DarkSilver { get; private set; }
        public static BrushStyle ListDarkAlternateStyle { get; private set; }
        public static BrushStyle ListAlternateStyle { get; private set; }
        public static BrushStyle ListDarkDefaultStyle { get; private set; }
        public static BrushStyle DropDownButton { get; private set; }
        public static BrushStyle CalendarHighlight { get; private set; }
        public static BrushStyle WhiteWash { get; private set; }
        public static BrushStyle GridAlternateDefaultCell { get; private set; }
        public static BrushStyle GridBackGround { get; private set; }
        public static BrushStyle GridSelection { get; private set; }
        public static BrushStyle GridColumnHeader { get; private set; }
        public static BrushStyle GridLastRowHeader { get; private set; }
        public static BrushStyle Canvas { get; private set; }
        public static BrushStyle Hover { get; private set; }
        #endregion

        internal static void Reset()
        {
            Default = new BrushStyle(BrushType.Horizontal, new Rgba(255, 255, 255), new Rgba(224, 224, 224));
            Blue = new BrushStyle(BrushType.BackwardDiagonal, Rgba.DodgerBlue, Rgba.Black);
            Selection = new BrushStyle(BrushType.ForwardDiagonal, Rgba.MidnightBlue, Rgba.Blue);
            LightSelection = new BrushStyle(BrushType.Horizontal, Rgba.Khaki, Rgba.Orange);
            OrangeSelection = new BrushStyle(BrushType.Horizontal, Rgba.Gold, Rgba.Orange);
            Black = new BrushStyle(BrushType.Horizontal, Rgba.Black, Rgba.DimGray);
            WindowFrame = new BrushStyle(BrushType.Horizontal, Rgba.WindowFrame, Rgba.Silver);
            Silver = new BrushStyle(BrushType.Vertical, Rgba.Silver, Rgba.White);
            DarkSilver = new BrushStyle(BrushType.Vertical, Rgba.Silver, new Rgba(105, 105, 105));
            ListDarkAlternateStyle = new BrushStyle(BrushType.BackwardDiagonal, Rgba.SeaGreen, Rgba.Aqua);

            ListAlternateStyle = new BrushStyle(BrushType.Horizontal, Rgba.Azure, Rgba.Silver);
            ListDarkAlternateStyle = new BrushStyle(BrushType.Horizontal, Rgba.Black, Rgba.DimGray);
            DropDownButton = new BrushStyle(BrushType.Vertical, Rgba.Silver, Rgba.White);
            CalendarHighlight = new BrushStyle(BrushType.Horizontal, Rgba.Navy, Rgba.Blue);
            WhiteWash = new BrushStyle(BrushType.Solid, Rgba.White);
            GridAlternateDefaultCell = new BrushStyle(BrushType.Horizontal, Rgba.Black, Rgba.Gray);
            GridBackGround = new BrushStyle(BrushType.ForwardDiagonal, Rgba.Brown, Rgba.Cyan);
            GridSelection = new BrushStyle(BrushType.Vertical, Rgba.CornflowerBlue, Rgba.Aqua);
            GridColumnHeader = new BrushStyle(BrushType.Vertical, Rgba.Black, Rgba.Gray);
            GridLastRowHeader = new BrushStyle(BrushType.Vertical, Rgba.CornflowerBlue, Rgba.SkyBlue);
            Canvas = new BrushStyle(BrushType.Vertical, Rgba.CornflowerBlue, Rgba.Aqua);
            Hover = new BrushStyle(BrushType.Horizontal, Rgba.GradientActiveCaption);
        }
    }
}
#endif

