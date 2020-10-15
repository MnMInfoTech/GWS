/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
    public abstract class _Font : IFont
    {
        #region Variables
        protected string id;
        #endregion

        #region PROPERTIES
        public string ID => id;
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

        #region ASSIGN ID
        public void AssignIDIfNone()
        {
            if (id == null)
                id = "Font".NewID();
        }
        #endregion
    }
}
