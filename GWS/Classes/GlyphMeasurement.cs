/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    #region IMEASURED - GLYPHS
    /// <summary>
    /// Represents an object representing measurement of glyphs and glyph-lines.
    /// </summary>
    public interface IGlyphMeasurement : IEnumerable<IGlyph>, IPoint, ISize, IHBY
    {
        /// <summary>
        /// Gets a total number of glyphs measured in this measurement.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets a collection of information to construct glyph lines.
        /// </summary>
        IReadOnlyList<IGlyphLineInfo> Lines { get; }
    }
    #endregion

    #region MEASURED - GLYPHS
    internal sealed class GlyphMeasurement : IGlyphMeasurement
    {
        #region VARIABLES
        readonly int X, Y, Width, Height;
        readonly IEnumerable<IGlyph> Data;
        readonly IReadOnlyList<IGlyphLineInfo> Lines;
        public static readonly GlyphMeasurement Empty = new GlyphMeasurement();
        static string description = @"Count: {0},
                    X: {1}, Y:{2}, W:{3}, H:{4}";
        #endregion

        #region CONSTRUCTORS
        GlyphMeasurement() { }
        /// <summary>
        /// Returns an instance of IGlyphs.
        /// </summary>
        /// <param name="area">Area of the glyphs.</param>
        /// <param name="text">Text of the glyphs.</param>
        /// <returns></returns>
        public GlyphMeasurement(IEnumerable<IGlyph> glyphs, int count,
           IRectangle area, IReadOnlyList<IGlyphLineInfo> lines, float minHBY)
        {
            Data = glyphs;
            Count = count;
            Lines = lines;
            area.GetBounds(out X, out Y, out Width, out Height);
            MinHBY = minHBY;
        }
        #endregion

        #region PROPERTIES
        public int Count { get; private set; }
        public float MinHBY { get; private set; }
        public bool Valid => Width != 0 && Height != 0;

        int IPoint.X => X;
        int IPoint.Y => Y;
        int ISize.Width => Width;
        int ISize.Height => Height;
        IReadOnlyList<IGlyphLineInfo> IGlyphMeasurement.Lines => Lines;
        #endregion

        #region IENUMERABLE<IGlyph>
        public IEnumerator<IGlyph> GetEnumerator()
        {
            return Data.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion

        public override string ToString()
        {
            return string.Format(description, Count, X, Y, Width, Height);
        }
    }
    #endregion
}
#endif
