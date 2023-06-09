/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MnM.GWS
{
    #region IGLYPHSLOT
    /// <summary>
    /// Represents information vital to draw a character for a given font.
    /// the information is directly fetched from the font object.
    /// </summary>
    internal interface IGlyphSlot : IRectangleF
    {
        #region PROPERTIES
        /// <summary>
        /// List of Points that forms an outline of the character.
        /// </summary>
        VectorF[] Points { get; }

        /// <summary>
        /// List of curve contours
        /// </summary>
        IList<int> Contours { get; }

        /// <summary>
        /// The character this slot represents for drawing on screen.
        /// </summary>
        char Character { get; }

        /// <summary>
        /// XHeight of the slot.
        /// </summary>
        int XHeight { get; }

        /// <summary>
        /// Minimum of points which forms perimiter of the slot.
        /// </summary>
        VectorF Min { get; }

        /// <summary>
        /// Maximum of points which forms the perimeter of the slot.
        /// </summary>
        VectorF Max { get; }

        /// <summary>
        /// Indicates if the slot is initialzed and ready for the process or not.
        /// </summary>
        bool Initialized { get; }
        #endregion
    }
    #endregion

    #region GLYPH SLOT
    /// <summary>
    /// Represents information vital to draw a character for a given font.
    /// the information is directly fetched from the font object.
    /// </summary>
    class GlyphSlot : IGlyphSlot
    {
        #region VARIABLES
        const string tostr = "Char: {0}, Area: {1}, {2}, {3}, {4}";
        VectorF min, max;
        float X, Y;
        float W, H;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new glyph slot with the given parameters
        /// </summary>
        /// <param name="c">A charact the slot is to represent</param>
        /// <param name="data">A list points which forms a information to create lines and quadratic beziers using the glyph renderer.</param>
        /// <param name="contours">Int array determines how many contours and what is the lenght of each one which defines a group of points to send for bezier processing</param>
        /// <param name="xHeight">Height of the slot</param>
        /// <returns>IGlyphSlot</returns>
        public GlyphSlot(char c, IList<VectorF> data, int[] contours, float xHeight)
        {
            if (data == null)
                Points = new VectorF[4];
            else
                Points = data.ToArray();

            XHeight = xHeight.Ceiling();
            Character = c;
            Contours = contours;
            Initialize();
        }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// List of Points that forms an outline of the character.
        /// </summary>
        public VectorF[] Points { get; private set; }

        /// <summary>
        /// List of curve contours
        /// </summary>
        public IList<int> Contours { get; private set; }

        /// <summary>
        /// The character this slot represents for drawing on screen.
        /// </summary>
        public char Character { get; private set; }

        /// <summary>
        /// XHeight of the slot.
        /// </summary>
        public int XHeight { get; private set; }

        /// <summary>
        /// Minimum of points which forms perimiter of the slot.
        /// </summary>
        public VectorF Min => min;

        /// <summary>
        /// Maximum of points which forms the perimeter of the slot.
        /// </summary>
        public VectorF Max => max;

        /// <summary>
        /// Indicates if the slot is initialzed and ready for the process or not.
        /// </summary>
        public bool Initialized { get; private set; }

        public bool Valid => W >= 0 && H >= 0;
        float IPointF.X => X;
        float IPointF.Y => Y;
        float ISizeF.Width => W;
        float ISizeF.Height => H;
        #endregion

        #region INITIALIZE
        unsafe void Initialize()
        {
            if (Initialized)
                return;

            min = max = VectorF.Empty;

            if (Initialized)
                return;

            float h;
            int len = Points.Length;

            fixed (VectorF* pts = Points)
            {
                if (char.IsWhiteSpace(Character))
                {
                    if (Character == ' ')
                    {
                        W = pts[1].X - pts[0].X;
                        return;
                    }
                    return;
                }
                if (len < 4)
                    return;

                var num = len - 4;

                min = new VectorF(float.MaxValue, float.MaxValue);
                max = new VectorF(float.MinValue, float.MinValue);

                for (int i = 0; i < num; i++)
                {
                    min = min.Min(pts[i]);
                    max = max.Max(pts[i]);
                }

                for (int i = 0; i < num; i++)
                    pts[i] -= min;

                W = Math.Max(pts[num + 1].X - pts[num].X - min.X, max.X - min.X);
                h = max.Y - min.Y;
                H = (int)h;
                if (h - H != 0)
                    ++H;
                X = min.X;
                Y = XHeight - H - min.Y;
            }

            Initialized = true;
        }

        #endregion

        #region GET BOUNDS
        public void GetBounds(out float x, out float y, out float w, out float h)
        {
            if (W <= 0 || H <= 0)
            {
                x = y = w = h = 0;
                return;
            }
            x = X;
            y = Y;
            w = W;
            h = H;
        }
        #endregion

        public override string ToString()
        {
            return string.Format(tostr, Character, X, Y, W, H);
        }
    }
    #endregion
}
#endif

