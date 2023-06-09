/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)

namespace MnM.GWS
{
    #region IGLYPH-LINEINFO
    public interface IGlyphLineInfo : IObject, ISpanHolder
    {
        float LineY { get; }
    }
    #endregion

    #region GLYPH- LINEINFO
    internal struct GlyphLineInfo : IGlyphLineInfo
    {
        readonly int X, Y, Width, Height;

        #region CONSTRUCTOR
        public GlyphLineInfo(ref int start, int cursor, float x, float y, ref float w, ref float h,
            ref float newX, ref float newY, ref int rnCount, float lh)
        {
            int i = rnCount == 0 ? 1 : 0;
            int count = cursor - start + i;
            Span = new Span(start, count);
            LineY = newY - y;
            X = (int)x;
            Y = (int)y;
            Width = w.Round();
            Height = h.Round();
            w = h = 0;
            start = cursor;
            newX = x;
            newY += lh;
            rnCount = 0;
        }
        #endregion

        #region PROPERTIES
        public Span Span { get; private set; }
        public float LineY { get; private set; }
        public bool Valid => Width != 0 && Height != 0;

        int IPoint.X => X;
        int IPoint.Y => Y;
        int ISize.Width => Width;
        int ISize.Height => Height;
        #endregion
    }
    #endregion
}
#endif
