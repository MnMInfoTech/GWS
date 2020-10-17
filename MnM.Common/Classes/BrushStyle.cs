/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
#if (GWS || Window)
    /// <summary>
    /// Represents an object which provides a certain list of colors to form a spectrum using a specified stop positions.
    /// </summary>
    public partial class BrushStyle : IEquatable<BrushStyle>, ICloneable, IReadContext, IReadOnlyList<int>
    {
        #region VARIABLES   
        /// <summary>
        /// Number of positions.
        /// </summary>
        public readonly int PositionCount;

        /// <summary>
        /// Kind of gradient that colors should generate.
        /// Usually one may pick on value from Gradient enum for native GWS gradients or define their own and handle it in spectrum and brush class on their own.
        /// </summary>
        public readonly int Gradient;

        /// <summary>
        /// Key which represents the ID of this style.
        /// </summary>
        public readonly string ID;

        readonly int[] Colors;
        readonly int[] Positions;

        byte valid;
        bool match;

        const string toString = "{0}.{1}";
        const string BrushKey = "{0}.{1}.{2}";
        public readonly static BrushStyle Empty =
            new BrushStyle(BrushType.Horizontal, Rgba.Black, Rgba.DimGray);

        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new fill style with specified gradient and values of colors to use.
        /// </summary>
        /// <param name="colors">Colours represented by integer values</param>
        /// <param name="gradient">This determines which gradient style to use</param>
        /// <param name="stops">Colour stops to use while navigating a color spectrum offered by the style</param>
        public BrushStyle(int[] colors, int gradient, IList<int> stops, bool matchSize = true)
        {
            match = matchSize;
            Positions = null;
            Colors = null;
            valid = 1;
            switch (colors.Length)
            {
                case 0:
                    Colors = new int[] { (int)Rgba.Black, (int)Rgba.Silver, (int)Rgba.Gray };
                    break;
                case 1:
                    Colors = new int[] { colors[0], colors[0].Darken(0.25f), colors[0].Darken(0.5f) };
                    break;
                case 2:
                    Colors = new int[] { colors[0], GWS.Colors.Blend(colors[0], colors[1], .5f), colors[1] };
                    break;
                default:
                    Colors = colors;
                    break;
            }

            Gradient = gradient;
            var value = Colors[0].GetHashCode();

            for (int i = 1; i < Colors.Length; i++)
                value = Numbers.Combine(value.GetHashCode(), Colors[i].GetHashCode());

            ID = string.Format(toString, Gradient, value);
            if (stops?.Count > 0)
                Positions = stops.ToArray();
            PositionCount = Positions?.Length ?? 0;
        }

        /// <summary>
        /// Creates a new fill style with specified gradient and values of colors to use.
        /// </summary>
        /// <param name="colors">Colours represented by integer values</param>
        /// <param name="gradient">Gradient enum determines which gradient style to use</param>
        /// <param name="stops">Colour stops to use while navigating a color spectrum offered by the style</param>
        public BrushStyle(int[] colors, BrushType gradient, IList<int> stops, bool matchSize = true) :
            this(colors, (int)gradient, stops, matchSize)
        { }

        /// <summary>
        /// Creates a new fill style with specified gradient and values of colors to use.
        /// </summary>
        /// <param name="colors">Colors represented by Rgba struct values</param>
        /// <param name="gradient">This determines which gradient style to use</param>
        /// <param name="stops">Colour stops to use while navigating a color spectrum offered by the style</param>
        public BrushStyle(IColor[] colors, BrushType gradient, IList<int> stops, bool matchSize = true) :
            this(colors.Select(c => c.Color).ToArray(), (int)gradient, stops, matchSize)
        { }


        /// <summary>
        /// Creates a new fill style with specified gradient and values of colors to use.
        /// </summary>
        /// <param name="colors">Colors represented by Rgba struct values</param>
        /// <param name="gradient">This determines which gradient style to use</param>
        /// <param name="stops">Colour stops to use while navigating a color spectrum offered by the style</param>
        public BrushStyle(Rgba[] colors, int gradient, IList<int> stops, bool matchSize = true) :
            this(colors.Select(c => c.Color).ToArray(), gradient, stops, matchSize)
        { }

        /// <summary>
        /// Creates a new fill style with specified gradient and values of colors to use.
        /// </summary>
        /// <param name="gradient">This determines which gradient style to use</param>
        /// <param name="colors">Colors represented by integer values</param>
        public BrushStyle(BrushType gradient, params int[] colors) :
            this(colors, (int)gradient, null)
        { }

        /// <summary>
        /// Creates a new fill style with specified gradient and values of colors to use.
        /// </summary>
        /// <param name="gradient">This determines which gradient style to use</param>
        /// <param name="colors">Colors represented by Rgba struct values</param>
        public BrushStyle(int gradient, params IColor[] colors) :
            this(colors.Select(c => c.Color).ToArray(), gradient, null)
        { }


        /// <summary>
        /// Creates a new fill style with specified gradient and values of colors to use.
        /// </summary>
        /// <param name="gradient">This determines which gradient style to use</param>
        /// <param name="colors">Colors represented by Rgba struct values</param>
        public BrushStyle(BrushType gradient, params IColor[] colors) :
            this(colors.Select(c => c.Color).ToArray(), (int)gradient, null)
        { }

        /// <summary>
        /// Creates a new fill style with specified values of colors to use.
        /// </summary>
        /// <param name="colors">Colors represented by integer values</param>
        public BrushStyle(params int[] values) :
            this(values, BrushType.Horizontal, null)
        { }

        /// <summary>
        /// Creates a new fill style with specified Rgba values of colors to use.
        /// </summary>
        /// <param name="gradient">This determines which gradient style to use</param>
        /// <param name="colors">Colors represented by integer values</param>
        public BrushStyle(params IColor[] values) :
            this(values, BrushType.Horizontal, null)
        { }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Calculated color at a given index.
        /// </summary>
        /// <param name="index">Index to get a calculated color</param>
        /// <returns></returns>
        public int this[int index] =>
            Colors[index];

        /// <summary>
        /// numenr of colors in this style.
        /// </summary>
        public int Count => Colors.Length;

        /// <summary>
        /// Gets the last color in list.
        /// </summary>
        public int EndColor =>
            Colors[Colors.Length - 1];

        public bool Valid => valid != 0;

        public bool Match
        {
            get => match;
            set => match = value;
        }
        #endregion

        #region EQUALITY
        public static bool operator ==(BrushStyle fs1, BrushStyle fs2) =>
            fs1.Equals(fs2);
        public static bool operator !=(BrushStyle fs1, BrushStyle fs2) =>
            !fs1.Equals(fs2);

        public override int GetHashCode() =>
            ID.GetHashCode();
        public override bool Equals(object obj)
        {
            if (!(obj is BrushStyle))
                return false;

            return Equals((BrushStyle)obj);
        }
        public bool Equals(BrushStyle other) =>
            other.ID == ID;
        #endregion

        #region GET BRUSH - KEY
        /// <summary>
        /// Gets the key calculated using the gradient value in this style for a particular width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns> string value of key</returns>
        public string GetBrushKey(int width, int height) =>
            string.Format(BrushKey, ID, width, height);
        #endregion

        #region GET POSITION
        /// <summary>
        /// Gets a position at a given index. It can then be used to get a calculated color at a given position.
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
        /// <param name="g">A new gradient for which the stle is required for.</param>
        /// <returns></returns>
        public BrushStyle Change(int g)
        {
            return new BrushStyle(Colors, g, Positions);
        }

        /// <summary>
        /// Gives a new style with changed gradient.
        /// </summary>
        /// <param name="g">A new gradient for which the stle is required for.</param>
        /// <returns></returns>
        public BrushStyle Change(BrushType g)
        {
            return new BrushStyle(Colors, g, Positions);
        }

        /// <summary>
        /// gives a new copy of this style but with inverted order of colors.
        /// </summary>
        /// <returns></returns>
        public BrushStyle Invert()
        {
            var colors = new int[Colors.Length];
            Colors.CopyTo(colors, 0);
            Array.Reverse(colors);
            if (Positions != null)
                return new BrushStyle(colors, Gradient, Positions);
            else
                return new BrushStyle(colors, Gradient, null);
        }
        #endregion

        #region GET COLORS
        /// <summary>
        /// Get a color on a given position assuming the size of spectrum specified by the size parameter.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size">Size to be used to calculate color</param>
        /// <param name="invert">specifies if calculation should be done in reverse order.</param>
        /// <returns>Color</returns>
        public int GetColor(float position, float Span, bool invert = false)
        {
            int color;
            int color1, color2;
            uint alpha, invAlpha, c1, c2, rb, ag;
            int PosCount = PositionCount;

            if (Span == 0 || position <= 0)
            {
                color = Colors[0];
                goto assignColor;
            }
            if (position == -1 || Span == -1)
            {
                color = EndColor;
                goto assignColor;
            }

            #region WORK OUT 2 COLORS

            if (Count < 3 || Span == 0)
            {
                color1 = Colors[0];
                color2 = EndColor;
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

            color1 = Colors[f];
            ++f;
            if (f > Count - 1)
                color1 = Colors[--f];

            color2 = Colors[f];
            Span = (float)Math.Ceiling(pos);
        #endregion

        Blend:
            #region BLEND
            if (invert)
            {
                var temp = color1;
                color1 = color2;
                color2 = temp;
            }

            float delta = position / Span;
            alpha = (uint)(delta * 255);
            if (alpha == 0)
            {
                color = color1;
                goto assignColor;
            }

            if (alpha == 255)
            {
                color = color2;
                goto assignColor;
            }
            c1 = (uint)color1;
            c2 = (uint)color2;
            invAlpha = 255 - alpha;
            rb = ((invAlpha * (c1 & GWS.Colors.RBMASK)) + (alpha * (c2 & GWS.Colors.RBMASK))) >> 8;
            ag = (invAlpha * ((c1 & GWS.Colors.AGMASK) >> 8)) + (alpha * (GWS.Colors.ONEALPHA | ((c2 & GWS.Colors.GMASK) >> 8)));

            color = (int)((rb & GWS.Colors.RBMASK) | (ag & GWS.Colors.AGMASK));
        #endregion

        assignColor:
            return color;
        }
        #endregion

        #region OPERATORS
        public static implicit operator bool(BrushStyle b) =>
            b.valid != 0;
        #endregion

        #region CLONE
        public BrushStyle Clone()
        {
            if (Colors == null)
                return BrushStyle.Empty;
            BrushStyle f = new BrushStyle(Colors.ToArray(), Gradient, Positions);
            return f;
        }
        object ICloneable.Clone() =>
            Clone();
        #endregion

        public override string ToString() => ID;

        #region IENUMERABLE
        public IEnumerator<int> GetEnumerator()
        {
            foreach (var item in Colors)
            {
                yield return item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion
    }

    partial class BrushStyle
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
#endif
}

