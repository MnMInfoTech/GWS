/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.Text;

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
        public abstract RectangleF Bounds { get; }
        public abstract int X { get; set; }
        public abstract int Y { get; set; }
        public string ID { get; private set; }
        RectangleF IBoundsF.Bounds => Bounds;
        #endregion

        #region DRAW
        public abstract bool Draw(IBuffer buffer, IReadContext readContext, out IPen Pen);
        #endregion

        #region TO SHAPE
        public virtual IEnumerable<VectorF> Figure() => null;
        #endregion

        public virtual bool Contains(int x, int y)
        {
            if (x < X)
                return false;
            if (y < Y)
                return false;
            if (x > X + Bounds.Width)
                return false;
            if (y > Y + Bounds.Height)
                return false;
            return true;
        }
        public abstract object Clone();
    }
}
