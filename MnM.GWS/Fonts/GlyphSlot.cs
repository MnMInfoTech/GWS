/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
#if AllHidden
    partial class _Factory
    {
#else
    public
#endif
        /// <summary>
        /// Represents information vital to draw a character for a given font.
        /// the information is directly fetched from the font object.
        /// </summary>
        struct GlyphSlot 
        {
            #region VARIABLES
            const string tostr = "Char: {0}, Area: {1}";
            VectorF min, max;
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
            public GlyphSlot( char c,  IList<VectorF> data, int[] contours,  float xHeight) : this()
            {
                Bounds = RectangleF.Empty;

                if (data == null)
                    Points = new VectorF[4];
                else
                    Points = data.ToArray();

                XHeight = xHeight.Ceiling();
                Character = c;
                Contours = contours;
                initialize();
            }
            void initialize()
            {
                if (Initialized)
                    return;

                Bounds = InitializeGlyphSlot(this, out min, out max);

                Initialized = true;

                if (Character == ' ')
                    return;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static RectangleF InitializeGlyphSlot(GlyphSlot slot, out VectorF Min, out VectorF Max)
            {
                Min = Max = VectorF.Empty;

                float x = slot.Bounds.X;
                float y = slot.Bounds.Y;
                float w = slot.Bounds.Width;
                float h = slot.Bounds.Height;

                if (slot.Initialized)
                    return new RectangleF(x, y, w, h);

                if (char.IsWhiteSpace(slot.Character))
                {
                    if (slot.Character == ' ')
                        w = (slot.Points[1].X - slot.Points[0].X).Ceiling();

                    return new RectangleF(x, y, w, h);
                }
                if (slot.Points.Count < 4)
                    return new RectangleF(x, y, w, h);

                var num = slot.Points.Count - 4;

                Min = new VectorF(float.MaxValue, float.MaxValue);
                Max = new VectorF(float.MinValue, float.MinValue);

                var points = slot.Points;

                for (int i = 0; i < num; i++)
                {
                    Min = Min.Min(points[i]);
                    Max = Max.Max(points[i]);
                }

                for (int i = 0; i < num; i++)
                    slot.Points[i] -= Min;

                w = Math.Max(slot.Points[num + 1].X - slot.Points[num].X - Min.X, Max.X - Min.X).Round();
                h = (Max.Y - Min.Y).Ceiling();
                x = Min.X.Ceiling();
                y = (slot.XHeight - h) - Min.Y.Ceiling();
                return new RectangleF(x, y, w, h);
            }
            #endregion

            #region PROPERTIES
            /// <summary>
            /// List of Points that forms an outline of the character.
            /// </summary>
            public IList<VectorF> Points { get; private set; }
           
            /// <summary>
            /// List of curve contours
            /// </summary>
            public IList<int> Contours { get; private set; }

            /// <summary>
            /// Are of the slot - determines where character is to be drawn.
            /// </summary>
            public RectangleF Bounds { get; private set; }

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
            #endregion
         
            public override string ToString()
            {
                return string.Format(tostr, Character, Bounds.ToString());
            }
        }
#if AllHidden
    }
#endif
}
