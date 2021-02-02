/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

namespace MnM.GWS
{
    public struct ShapeArea: IRectangle, IShapeID
    {
        public readonly int X, Y, Width, Height;
        public readonly uint ShapeID;

        public ShapeArea(int x, int y, int w, int h, uint shapeID = 0)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            ShapeID = shapeID;
        }
        public bool Valid => Width != 0 && Height != 0;
        int ISize.Width => Width;
        int ISize.Height => Height;
        int IPoint.X => X;
        int IPoint.Y => Y;
        uint IShapeID.ShapeID => ShapeID;
    }
}
