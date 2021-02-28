/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    public struct Perimeter : IPerimeter
    {
        #region VARIABLES
        public readonly int X;
        public readonly int Y;
        public readonly int W;
        public readonly int H;
        public readonly int ProcessID;
        public readonly uint ShapeID;
        public readonly byte Type;

        public readonly static Perimeter Empty = new Perimeter();
        static string description = "X: {0}, Y: {1}, R: {2}, B: {3}";
        #endregion

        #region CONSTRUCTORS
        public Perimeter(int x, int y, int w, int h) : this()
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        public Perimeter(int x, int y, int w, int h, int processID, uint shapeID = 0, byte lifePriority = 0) :
            this(x, y, w, h)
        {
            ProcessID = processID;
            ShapeID = shapeID;
            Type = lifePriority;
        }
        public Perimeter(IBoundable perimeter, int x, int y, int w, int h) :
            this(perimeter)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
        public Perimeter(IBoundable perimeter, int x, int y, int w, int h, uint shapeID) :
           this(perimeter)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
            ShapeID = shapeID;
        }
        public Perimeter(IBoundable perimeter) : this()
        {
            perimeter.GetBounds(out X, out Y, out W, out H);

            if (perimeter is IProcessID)
                ProcessID = ((IProcessID)perimeter).ProcessID;
            if (perimeter is IShapeID)
                ShapeID = ((IShapeID)perimeter).ShapeID;
            if (perimeter is IType)
                Type = ((IType)perimeter).Type;
        }
        public Perimeter(IBoundable perimeter, int processID) :
            this(perimeter)
        {
            perimeter.GetBounds(out X, out Y, out W, out H);
            ProcessID = processID;
        }
        public Perimeter(uint shapeID, IBoundable perimeter) :
            this(perimeter)
        {
            ShapeID = shapeID;
        }
        public Perimeter(IBoundable r, int processID, byte lifePriority, uint shapeID)
        {
            r.GetBounds(out X, out Y, out W, out H);
            ProcessID = processID;
            ShapeID = shapeID;
            Type = lifePriority;
        }
        #endregion

        #region PROPERTIES
        int IProcessID.ProcessID => ProcessID;
        uint IShapeID.ShapeID => ShapeID;
        byte IType.Type => Type;
        public bool Valid => W > 0 && H > 0;
        #endregion

        #region GET BOUNDS
        public void GetBounds(out int x, out int y, out int w, out int h)
        {
            if (W <= 0 || H <= 0)
            {
                x = y = w = h = 0;
                return;
            }
            x = X;
            y = Y;
            w = W;
            h = H;
        }
        #endregion

        #region TO STRING
        public override string ToString()
        {
            return string.Format(description, X, Y, X + W, Y + H);
        }
        #endregion

        #region OPERATORS
        public static implicit operator bool(Perimeter p) =>
            p.W > 0 && p.H > 0;
        #endregion
    }
}
