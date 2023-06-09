/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details.
* Author: Mukesh Adhvaryu
*/
#if (GWS || Window)
using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    #region END-BUTTON
    public struct EndButton: IShape, IExDraw, IExResizable
    {
        #region VARIABLES
        IBox End;
        #endregion

        #region CONSTRUCTORS
        public EndButton(float x, float y, float width, float height, Position position, float thickness = 2)
        {
            float r, b;

            switch (position)
            {
                case Position.Left:
                default:
                    r = x + thickness;
                    b = y + height - 2;
                    y += 2;
                    break;
                case Position.Top:
                    r = x + width - 2;
                    b = y + thickness;
                    x += 2;
                    break;
                case Position.Right:
                    r = x + width;
                    b = y + height - 2;
                    x = r - thickness;
                    y += 2;
                    break;
                case Position.Bottom:
                    r = x + width - 2;
                    b = y + height;
                    y = b - thickness;
                    x += 2;
                    break;
            }
            End = Box.FromLTRB(x, y, r, b);
        }
        EndButton(IBox capsule)
        {
            End = capsule;
        }
        #endregion

        #region PROPERTIES
        bool IOriginCompatible.IsOriginBased => End.X == 0 && End.Y == 0;
        public int X => End.X;
        public int Y => End.Y;
        public bool Valid => End.Valid;
        public int Width => End.Width;
        public int Height => End.Height;
        #endregion

        #region DRAW
        bool IExDraw.Draw(IEnumerable<IParameter> parameters, IExRenderer renderer)
        {
            renderer.Render(End, parameters);
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
            this = new EndButton((IBox)((IExResizable)End).Resize(w, h, out _, resizeCommand));
            success = true;
            return this;
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            return new EndButton((IBox)End.GetOriginBasedVersion());
        }
        #endregion
    }
    #endregion
}
#endif
