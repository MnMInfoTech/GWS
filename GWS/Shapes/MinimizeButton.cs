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
    #region MINIMIZE BUTTON
    public struct MinimizeButton: IShape, IExDraw
    {
        #region VARIABLES
        readonly ILine Line;
        readonly IBox Box;
        #endregion

        #region CONSTRUCTORS
        public MinimizeButton(float x, float y, float w, float h) : this()
        {
            Line = new Line(x, y + h / 2f, x + w, y + h / 2f);
            Box = new Box(x, y, w, h);
        }
        public MinimizeButton(IBounds r)
        {
            int x, y,  w, h;
            r.GetBounds(out x, out y, out w, out h);
            this = new MinimizeButton(x, y, w, h);
        }
        public MinimizeButton(IBoundsF r)
        {
            float x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            this = new MinimizeButton(x, y, w, h);
        }
        public MinimizeButton(MinimizeButton m)
        {
            Box = (IBox)m.Box.GetOriginBasedVersion();
            Line = new Line(Box.X, Box.Y + Box.Height / 2f, Box.X + Box.Width, Box.Y + Box.Height / 2f);
        }
        #endregion

        #region PROPERTIES
        public bool Valid => Line.Valid;
        public int Width => Box.Width;
        public int Height => Box.Height;
        public int X => Box.X;
        public int Y => Box.Y;
        bool IOriginCompatible.IsOriginBased => Box.IsOriginBased;
        #endregion

        #region GET BOUNDS
        public void GetBounds(out int x, out int y, out int w, out int h)
        {
            x = Box.X;
            y = Box.Y;
            w = Box.Width;
            h = Box.Height;
        }
        #endregion

        #region DRAW
        bool IExDraw.Draw(IEnumerable<IParameter> parameters, IExRenderer renderer)
        {
            ((IExDraw)Line).Draw(parameters.AppendItems(1.ToThickness()), renderer);
            ((IExDraw)Box).Draw(parameters.AppendItem((Command.DrawOutLines | Command.Transparent).Add()), renderer);
            return (true);
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            return new MinimizeButton(this);
        }
        #endregion
    }
    #endregion
}
#endif