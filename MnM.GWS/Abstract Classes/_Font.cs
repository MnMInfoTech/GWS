/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if GWS || Window
using System.Collections.Generic;

namespace MnM.GWS
{
    public abstract class _Font : IFont
    {
        #region VARIABLES
        protected readonly int id;
        static int uniqueid;
        #endregion

        public _Font()
        {
            id = (++uniqueid);
        }

        #region PROPERTIES
        public virtual bool EnableKerning { get; set; }
        public virtual bool Hinting { get; set; }
        public abstract IFontInfo Info { get; }
        public abstract int Size { get; set; }
        public abstract bool Kerning { get; }
        #endregion

        #region GET KERNING
        public abstract int GetKerning(char previous, char now);
        #endregion

        #region GET GLYPH
        public abstract IGlyph GetGlyph(char character);
        #endregion

        #region MEASURE GLYPHS
        public abstract void MeasureGlyphs(IList<IGlyph> Glyphs, float destX, float destY,
            out RectangleF Area, out IList<IGlyph> ResultGlyphs, out float minHBY, ITextStyle drawStyle = null);
        #endregion
    }
}
#endif
