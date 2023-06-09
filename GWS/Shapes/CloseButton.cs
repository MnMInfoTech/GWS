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
    #region CLOSE BUTTON
    /// <summary>
    /// 
    /// </summary>
    public struct CloseButton: IShape, IExDraw, IExResizable
    {
        #region VARIABLES
        readonly ILine L1, L2;
        #endregion

        #region CONSTRUCTORS
        public CloseButton(float x, float y, float w, float h) : this()
        {
            L1 = new Line(x, y, x + w, y + h);
            L2 = new Line(x + w, y, x, y+ h);
        }
        public CloseButton(IBounds r)
        {
            int x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            this = new CloseButton(x, y, w, h);
        }
        public CloseButton(IBoundsF r)
        {
            float x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            this = new CloseButton(x, y, w, h);
        }
        public CloseButton(float x, float y, float w, float h, IScale scale) : this()
        {
            Rectangles.Scale(ref x, ref y, ref w, ref h, scale);
            this = new CloseButton(x, y, w, h);
        }
        public CloseButton(IBounds r, IScale scale)
        {
            int x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            Rectangles.Scale(ref x, ref y, ref w, ref h, scale);
            this = new CloseButton(x, y, w, h);
        }
        public CloseButton(IBoundsF r, IScale scale)
        {
            float x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            Rectangles.Scale(ref x, ref y, ref w, ref h, scale);
            this = new CloseButton(x, y, w, h);
        }
        CloseButton(CloseButton cross, int w, int h)
        {
            L1= (ILine)((IExResizable)cross.L1).Resize(w,h, out _);
            L2 = (ILine)((IExResizable)cross.L2).Resize(w, h, out _);
        }
        CloseButton(CloseButton cross)
        {
            var x = cross.X;
            var y = cross.Y;
            L1 = new Line(cross.L1.X1 - x, cross.L1.Y1 - y, cross.L1.X2 - x, cross.L1.Y2 - y);
            L2 = new Line(cross.L2.X1 - x, cross.L2.Y1 - y, cross.L2.X2 - x, cross.L2.Y2 - y);
        }
        #endregion

        #region PROPERTIES
        public bool Valid => L1.Valid;
        bool IOriginCompatible.IsOriginBased => L1.IsOriginBased;
        public int X => L1.X;
        public int Y => L1.Y;
        public int Width => L1.Width;
        public int Height => L1.Height;
        #endregion

        #region DRAW
        bool IExDraw.Draw(IEnumerable<IParameter> parameters, IExRenderer renderer)
        {
            var command = (Command.DrawEndPixel).Add();
            var Parameters = parameters.AppendItem(command);
            ((IExDraw)L1).Draw(Parameters, renderer);
            ((IExDraw)L2).Draw(Parameters, renderer);
            return true;
        }
        #endregion

        #region RESIZE
        object IExResizable.Resize(int w, int h, out bool success, ResizeCommand resizeCommand)
        {
            success = false;
            var iw = ((ISize)this).Width;
            var ih = ((ISize)this).Height;

            if
            (
               (w == iw && h == ih) ||
               (w == 0 && h == 0)
            )
            {
                return this;
            }

            bool SizeOnlyToFit = (resizeCommand & ResizeCommand.SizeOnlyToFit) == ResizeCommand.SizeOnlyToFit;

            if (SizeOnlyToFit && iw > w && ih > h)
                return this;

            if (SizeOnlyToFit)
            {
                if (w < iw)
                    w = iw;
                if (h < ih)
                    h = ih;
            }
            success = true;
            return new CloseButton(this, w, h);
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            return new CloseButton(this);
        }
        #endregion
    }
    #endregion
}
#endif