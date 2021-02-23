/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    public struct Bounds: IBoundable, IType
    {
        #region VARIABLES
        public readonly int X;
        public readonly int Y;
        public readonly int W;
        public readonly int H;
        public readonly byte Type;

        public readonly static Bounds Empty = new Bounds();
        static string description = "X: {0}, Y: {1}, R: {2}, B: {3}";
        #endregion

        #region CONSTRUCTORS
        public Bounds(int x, int y, int w, int h, byte type = 0) 
        {
            X = x;
            Y = y;
            W = w;
            H = h;
            Type = type;
        }
        public Bounds(IBoundable boundable) : this()
        {
            if (boundable == null || !boundable.Valid)
                return;
            boundable.GetBounds(out X, out Y, out W, out H);
            if (boundable is IType)
                Type = ((IType)boundable).Type;
        }
        public Bounds(IBoundable boundable, byte type) : this(boundable)
        {
            Type = type;
        }
        #endregion

        #region PROPERTIES
        public bool Valid => W > 0 && H > 0;
        byte IType.Type => Type;
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

        #region OPERATORS
        public static implicit operator bool(Bounds r) =>
            r.W > 0 && r.H > 0;
        #endregion

        #region TO STRING
        public override string ToString()
        {
            return string.Format(description, X, Y, X + W, Y + H);
        }
        #endregion
    }
}
