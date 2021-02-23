/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public sealed class Boundary: IBoundary
    {
        #region VARIABLES
        /// X co-ordinate of recently drawn area of this object.
        /// </summary>
        public int X1 = int.MaxValue;

        /// <summary>
        /// Y co-ordinate of recently drawn area of this object.
        /// </summary>
        public int Y1 = int.MaxValue;

        /// <summary>
        /// Far right X co-ordinate of recently drawn area of this object.
        /// </summary>
        public int X2 = 0;

        /// <summary>
        /// Far bottom Y co-ordinate of recently drawn area of this object.
        /// </summary>
        public int Y2 = 0;

        /// <summary>
        /// ID of shape being rendered.
        /// </summary>
        public uint ShapeID;

        /// <summary>
        /// ID of process being used to render the shape.
        /// </summary>
        public int ProcessID;

        /// <summary>
        /// Type this object represents for the purpose of rendering operation.
        /// </summary>
        public byte Type;

        static string description = "X: {0}, Y: {1}, R: {2}, B: {3}";
        #endregion

        #region CONSTRUCTORS
        public Boundary() { }
        public Boundary(byte type)
        {
            Type = type;
        }
        public Boundary(int x, int y, int w, int h, byte type = 0)
        {
            X1 = x;
            Y1 = y;
            X2 = X1 + w;
            Y2 = Y1 + h;
            Type = type;
        }
        public Boundary(IBoundable perimeter)
        {
            perimeter.GetBounds(out int x, out int y, out int w, out int h);
            X1 = x;
            Y1 = y;
            X2 = X1 + w;
            Y2 = Y1 + h;
            if (perimeter is IType)
                Type = ((IType)perimeter).Type;
        }
        #endregion

        #region PROPERTIES
        public bool Valid => X2 > 0 && Y2 > 0;
        int IProcessID.ProcessID => ProcessID;
        uint IShapeID.ShapeID => ShapeID;
        byte IBoundary.Type { get => Type; set => Type = value; }
        byte IType.Type => Type;
        #endregion

        #region NOTIFY
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Notify(int x1, int y1, int x2, int y2)
        {
            if (x2 == 0 && y2 == 0)
            {
                X1 = Y1 = int.MaxValue;
                X2 = Y2 = 0;
                return;
            }
            if (x1 < X1)
                X1 = x1;
            if (y1 < Y1)
                Y1 = y1;
            if (x2 > X2)
                X2 = x2;
            if (y2 > Y2)
                Y2 = y2;
        }
        #endregion

        #region GET BOUNDS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetBounds(out int x, out int y, out int w, out int h)
        {
            if (X2 == 0 || Y2 == 0 || X2 - X1 <= 0 || Y2 - Y1 <= 0)
            {
                x = y = w = h = 0;
                return;
            }
            x = X1;
            y = Y1;
            var x2 = X2;
            var y2 = Y2;
            w = x2 - x;
            h = y2 - y;
        }
        #endregion

        public static implicit operator bool (Boundary boundary) =>
            boundary.X2 != 0 && boundary.Y2 != 0 && 
            boundary.X2 - boundary.X1 > 0 && boundary.Y2 - boundary.Y1 > 0;

        public override string ToString()
        {
            return string.Format(description, X1, Y1, X2, Y2);
        }
    }
}
