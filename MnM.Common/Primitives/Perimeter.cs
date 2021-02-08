/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    public struct Perimeter: IPerimeter
    {
        #region VARIABLES
        public readonly int X;
        public readonly int Y;
        public readonly int Width;
        public readonly int Height;
        public readonly int ProcessID;
        public readonly uint ShapeID;
        public readonly static Perimeter Empty = new Perimeter();
        #endregion

        #region CONSTRUCTORS
        public Perimeter(int x, int y, int w, int h, int processID = 0, uint shapeID = 0)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            ProcessID = processID;
            ShapeID = shapeID;
        }
        public Perimeter(IPerimeter perimeter) : this()
        {
            perimeter.GetBounds(out X, out Y, out Width, out Height);
            ProcessID = perimeter.ProcessID;
            ShapeID = perimeter.ShapeID;
        }
        public Perimeter(IPerimeter perimeter, int processID) : this()
        {
            perimeter.GetBounds(out X, out Y, out Width, out Height);
            ProcessID = processID;
        }
        public Perimeter(uint shapeID, IPerimeter perimeter) : this()
        {
            perimeter.GetBounds(out X, out Y, out Width, out Height);
            ProcessID = perimeter.ProcessID;
            ShapeID = shapeID;
        }
        public Perimeter(IRectangle r, int processID = 0, uint shapeID = 0)
        {
            X = r.X;
            Y = r.Y;
            Width = r.Width;
            Height = r.Height;
            ProcessID = processID;
            ShapeID = shapeID;
        }
        #endregion

        #region PROPERTIES
        int IProcessID.ProcessID => ProcessID; 
        uint IShapeID.ShapeID => ShapeID;
        #endregion

        #region GET BOUNDS
        public void GetBounds(out int x, out int y, out int w, out int h, int xExpand = 0, int yExpand = 0)
        {
            x = X;
            y = Y;
            w = Width;
            h = Height;
            if (xExpand == 0 && yExpand == 0)
                return;
            int x2 = x + w;
            int y2 = y + h;
            if (xExpand != 0)
            {
                x -= xExpand;
                if (x < 0)
                    x = 0;
                x2 += xExpand;
            }
            if (yExpand != 0)
            {
                y -= yExpand;
                if (y < 0)
                    y = 0;
                y2 += yExpand;
            }
            w = x2 - x;
            h = y2 - y;
        }
        #endregion
    }
}
