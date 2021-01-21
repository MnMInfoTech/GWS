/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if GWS || Window
using System.Collections.Generic;

namespace MnM.GWS
{
    public abstract class _Glyph : IGlyph
    {
        public _Glyph()
        {
            ID = "Glyph".NewID();
        }

        #region PROPERTIES
        public char Character { get; protected set; }
        public virtual bool IsOutLine { get; set; }
        public abstract int X { get; set; }
        public abstract int Y { get; set; }
        public abstract float Width { get; }
        public abstract float Height { get; }
        public string ID { get; private set; }
        public Rectangle RecentlyDrawn { get; set; }
        #endregion

        #region DRAW
        public abstract bool Draw(IWritable buffer, ISettings Settings);
        #endregion

        #region TO SHAPE
        public virtual IEnumerable<VectorF> Figure() => null;
        #endregion

        public abstract object Clone();
    }
}
#endif
