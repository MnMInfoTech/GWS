/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect : IBoundable
    {
        #region VARIABLES
        public int X;
        public int Y;
        public int W;
        public int H;
        public static readonly Rect Empty = new Rect();
        #endregion

        #region CONSTRUCTOR
        public Rect(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
        public Rect(IBoundable rc, int expand = 0)
        {
            rc.GetBounds(out X, out Y, out W, out H);
            if (expand != 0)
            {
                X -= expand;
                if (X < 0)
                    X = 0;
                Y -= expand;
                if (Y < 0)
                    Y = 0;
                W += expand;
                H += expand;
            }
        }
        #endregion

        #region PROPERTIES
        public bool Valid => W > 0 && H > 0;
        #endregion

        #region GET BOUNDS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        #region CONTAINS
        public bool ContainsX(int x) =>
            x >= X && x <= X + W;
        public bool ContainsY(int y) =>
            y >= Y && y <= Y + H;
        #endregion

        #region OPERATORS
        public static implicit operator Rectangle(Rect r) =>
            new Rectangle(r.X, r.Y, r.W, r.H);

        public static implicit operator Rect(Rectangle r)=>
            new Rect(r.X, r.Y, r.Width, r.Height);

        public static implicit operator Perimeter(Rect r) =>
            new Perimeter(r.X, r.Y, r.W, r.H);

        public static implicit operator Boundary(Rect r) =>
            new Boundary(r.X, r.Y, r.W, r.H);

        public static implicit operator bool(Rect r) =>
            r.W > 0 && r.H > 0;
        #endregion
    }
}
