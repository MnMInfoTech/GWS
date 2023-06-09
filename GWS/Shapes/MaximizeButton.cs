/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if (GWS || Window)

using System.Collections.Generic;
using System.Threading.Tasks;

namespace MnM.GWS
{
    #region MAXIMIZE-BUTTON
    public struct MaximizeButton : IShape, IExDraw
    {
        #region VARIABLES
        readonly IBox Box;
        #endregion

        #region CONSTRUCTORS
        public MaximizeButton(float x, float y, float w, float h) : this()
        {
            Box = new Box(x, y, w, h);
        }
        public MaximizeButton(IBounds r)
        {
            int x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            this = new MaximizeButton(x, y, w, h);
        }
        public MaximizeButton(IBoundsF r)
        {
            float x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            this = new MaximizeButton(x, y, w, h);
        }
        public MaximizeButton(IBounds r, IScale scale)
        {
            int x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            Rectangles.Scale(ref x, ref y, ref w, ref h, scale);
            this = new MaximizeButton(x, y, w, h);
        }
        public MaximizeButton(IBoundsF r, IScale scale)
        {
            float x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            Rectangles.Scale(ref x, ref y, ref w, ref h, scale);
            this = new MaximizeButton(x, y, w, h);
        }
        public MaximizeButton(float x, float y, float w, float h, IScale scale) : this()
        {
            Rectangles.Scale(ref x, ref y, ref w, ref h, scale);
            Box = new Box(x,y,w,h);
        }
        public MaximizeButton(MaximizeButton m)
        {
            Box =(IBox) m.Box.GetOriginBasedVersion();
        }
        #endregion

        #region PROPERTIES
        public bool Valid => Box.Valid;
        public int Width => Box.Width;
        public int Height => Box.Height;
        public int X => Box.X;
        public int Y => Box.Y;
        bool IOriginCompatible.IsOriginBased => Box.IsOriginBased;
        #endregion

        #region GET BOUNDS
        public void GetBounds(out int x, out int y, out int w, out int h)
        {
            Box.GetBounds(out x, out y, out w, out h);
        }
        #endregion

        #region DRAW
        bool IExDraw.Draw(IEnumerable<IParameter> parameters, IExRenderer renderer)
        {
            ((IExDraw)Box).Draw(parameters.AppendItem(Command.DrawOutLines.Add()), renderer);
            return true;
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            return new MaximizeButton(this);
        }
        #endregion
    }
    #endregion
}
#endif