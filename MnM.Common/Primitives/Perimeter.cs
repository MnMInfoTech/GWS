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
        public readonly byte LifePriority;

        public readonly static Perimeter Empty = new Perimeter();
        static string description = "X: {0}, Y: {1}, R: {2}, B: {3}";
        #endregion

        #region CONSTRUCTORS
        public Perimeter(int x, int y, int w, int h): this()
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        public Perimeter(int x, int y, int w, int h, int processID, uint shapeID = 0, byte lifePriority = 0):
            this(x, y, w, h)
        {
            ProcessID = processID;
            ShapeID = shapeID;
            LifePriority = lifePriority;
        }
        public Perimeter(IBoundable perimeter, int x, int y, int w, int h):
            this(perimeter)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
        public Perimeter(IBoundable perimeter, int x, int y, int w, int h, uint shapeID) :
           this(perimeter)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            ShapeID = shapeID;
        }
        public Perimeter(IBoundable perimeter) : this()
        {
            perimeter.GetBounds(out X, out Y, out Width, out Height);

            if (perimeter is IProcessID)
                ProcessID = ((IProcessID)perimeter).ProcessID;
            if (perimeter is IShapeID)
                ShapeID = ((IShapeID)perimeter).ShapeID;
            if (perimeter is ILifePriority)
                LifePriority = ((ILifePriority)perimeter).LifePriority;
        }
        public Perimeter(IBoundable perimeter, int processID) :
            this(perimeter)
        {
            perimeter.GetBounds(out X, out Y, out Width, out Height);
            ProcessID = processID;
        }
        public Perimeter(uint shapeID, IBoundable perimeter) : 
            this(perimeter)
        {
            ShapeID = shapeID;
        }
        public Perimeter(IBoundable r, int processID, byte lifePriority, uint shapeID)
        {
            r.GetBounds(out X, out Y, out Width, out Height);
            ProcessID = processID;
            ShapeID = shapeID;
            LifePriority = lifePriority;
        }
        #endregion

        #region PROPERTIES
        int IProcessID.ProcessID => ProcessID; 
        uint IShapeID.ShapeID => ShapeID;
        byte ILifePriority.LifePriority => LifePriority;
        public bool Valid => Width > 0 && Height > 0;
        #endregion

        #region GET BOUNDS
        public void GetBounds(out int x, out int y, out int w, out int h)
        {
            if (Width <= 0 || Height <= 0)
            {
                x = y = w = h = 0;
                return;
            }
            x = X;
            y = Y;
            w = Width;
            h = Height;
        }
        #endregion

        #region TO STRING
        public override string ToString()
        {
            return string.Format(description, X, Y, X + Width, Y + Height);
        }
        #endregion
    }
}
