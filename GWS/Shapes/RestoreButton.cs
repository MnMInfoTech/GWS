/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if (GWS || Window) 

using System.Collections.Generic;

namespace MnM.GWS
{
    #region RESTORE-BUTTON
    public struct RestoreButton: IShape, IExDraw
    {
        #region VARIABLES
        readonly IBox Box;
        readonly IPolygon Polygon;
        #endregion

        #region CONSTRUCTOR
        public RestoreButton(float x, float y, float w, float h) : this()
        {
            Polygon = new Polygon(
                new VectorF[]
                {
                    new VectorF(x, y + 2),
                    new VectorF(x + w - 2, y + 2),
                    new VectorF(x + w - 2, y + h),
                    new VectorF(x, y + h),
                    VectorF.Break,
                    new VectorF(x + 2, y + 2),
                    new VectorF(x + 2, y),
                    new VectorF(x + w, y),
                    new VectorF(x + w, y + h - 2),
                    new VectorF(x + w - 2, y + h - 2),
                    VectorF.Segment,
                }) ;
            Box = new Box(x, y, w, h);
        }
        public RestoreButton(float x, float y, float w, float h, IScale scale) : this()
        {
            if (scale != null && scale.HasScale)
            {
                Rectangles.Scale(ref x, ref y, ref w, ref h, scale);
            }
            Box = new Box(x, y, w, h);
        }
        public RestoreButton(IBounds r)
        {
            int x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            this = new RestoreButton(x, y, w, h);
        }
        public RestoreButton(IBoundsF r)
        {
            float x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            this = new RestoreButton(x, y, w, h);
        }
        public RestoreButton(IBounds r, IScale scale)
        {
            int x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            this = new RestoreButton(x, y, w, h,scale);
        }
        public RestoreButton(IBoundsF r, IScale scale)
        {
            float x, y, w, h;
            r.GetBounds(out x, out y, out w, out h);
            this = new RestoreButton(x, y, w, h,scale);
        }

        RestoreButton(RestoreButton m)
        {
            Box = (IBox)m.Box.GetOriginBasedVersion();
            Polygon = (IPolygon)m.Polygon.GetOriginBasedVersion();
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

        #region DRAW
        bool IExDraw.Draw(IEnumerable<IParameter> parameters, IExRenderer graphics)
        {
            //var action = graphics.CreateRenderAction(parameters);
            graphics.RenderPolygon(Polygon, parameters.AppendItems(Command.DrawOutLines.Add()));
            //var lines = Box.GetPoints().ToLines(PointJoin.CircularJoin);
            //action(null, lines, null, new Offset(0, 2));
            //action(null, new ILine[] { lines[0], lines[1] },  null, new Offset(2, 0));
            return true;
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            return new RestoreButton(this);
        }
        #endregion
    }
    #endregion
}
#endif