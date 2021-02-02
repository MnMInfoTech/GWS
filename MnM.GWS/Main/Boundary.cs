/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public partial class Boundary : IBoundary
    {
        #region VARIABLES
        /// <summary>
        /// X co-ordinate of recently drawn area of this object.
        /// </summary>
        volatile int X1 = int.MaxValue;

        /// <summary>
        /// Y co-ordinate of recently drawn area of this object.
        /// </summary>
        volatile int Y1 = int.MaxValue;

        /// <summary>
        /// Far right X co-ordinate of recently drawn area of this object.
        /// </summary>
        volatile int X2 = 0;

        /// <summary>
        /// Far bottom Y co-ordinate of recently drawn area of this object.
        /// </summary>
        volatile int Y2 = 0;

        /// <summary>
        /// 
        /// </summary>
        volatile int dstX;

        /// <summary>
        /// 
        /// </summary>
        volatile int dstY;

        public static Boundary Empty = new Boundary();

        static string description = "X: {0}, Y: {1}, R: {2}, B: {3}";
        #endregion

        #region CONSTRUCTORS
        public Boundary() { }
        #endregion

        #region PROPERTIES
        public bool Valid => X2 != 0 && Y2 != 0;
        public int Width => X2 - X1;
        public int Height => Y2 - Y1;
        public int X => X1;
        public int Y => Y1;
        public uint ShapeID { get; set; }
        public int DstX 
        {
            get
            {
                int i = dstX;
                GetX(ref i);
                return i;
            }
            set => dstX = value;
        }
        public int DstY 
        { 
            get
            {
                int i = dstY;
                GetY(ref i);
                return i;
            }
            set => dstY = value; 
        }
        #endregion

        #region GET DESTINATION X, Y
        partial void GetX(ref int dstX);
        partial void GetY(ref int dstY);
        #endregion

        #region CONTAINS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(int x, int y)
        {
            if (x < X1 || y < Y1 || x > X2 || y > Y2)
                return false;
            return true;
        }
        #endregion

        #region INTERSECTS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects<T>(T other) where T : IPoint, ISize
        {
            if (other.Width == 0 || other.Height == 0)
                return false;
            bool xOverlap = X >= other.X && X <= (other.X + other.Width) ||
                 other.X >= X && X <= (X + Width);

            bool yOverlap = Y >= other.Y && Y <= (other.Y + other.Height) ||
                 other.Y >= Y && Y <= (Y + Height);

            if (!xOverlap || !yOverlap)
                return false;

            return true;
        }
        #endregion

        #region INTERSECT
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IRectangle Intersect<T>(T b) where T : IPoint, ISize
        {
            int x1 = Math.Max(X, b.X);
            int y1 = Math.Max(Y, b.Y);
            int x2 = Math.Min(X + Width, b.X + b.Width);
            int y2 = Math.Min(Y + Height, b.Y + b.Height);

            if (x2 >= x1
                && y2 >= y1)
            {
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);
            }
            return Rectangle.Empty;
        }
        #endregion

        #region NOTIFY
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Notify(int x1, int y1, int x2, int y2) =>
            Notify2(x1, y1, x2, y2);
        partial void Notify2(int x1, int y1, int x2, int y2);
        #endregion

        #region MERGE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Merge(IRectangle boundary) =>
            Merge2(boundary);
        partial void Merge2(IRectangle boundary);
        #endregion

        #region COPY
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Copy(IRectangle boundary) =>
            Copy2(boundary);
        partial void Copy2(IRectangle boundary);
        #endregion

        #region CLEAR
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearBounds()
        {
            X1 = Y1 = int.MaxValue;
            X2 = Y2 = 0;
        }
        #endregion

        #region DRAW
        public void Draw(IWritable buffer, Command command = 0)
        {
            if (X2 == 0 || Y2 == 0)
                return;

            PixelAction action;
            IReadable pen;

            if (buffer is IReadable)
                pen = (IReadable)buffer;
            else
                pen = Pens.Black;

            command |= Command.Screen| Command.Dot;
            var boundary = Factory.newBoundary();

            int x1 = X1 + 3;
            int y1 = Y1 + 3;
            int x2 = X2 - 3;
            int y2 = Y2 - 3;

            buffer.CreatePixelAction(pen, out action, boundary);
            Renderer.ProcessLine(x1, y1, x1, y2, action, command);
            Renderer.ProcessLine(x1, y2, x2, y2, action, command);
            Renderer.ProcessLine(x2, y2, x2, y1, action, command);
            Renderer.ProcessLine(x2, y1, x1, y1, action, command);

            (buffer as IUpdatable)?.Update(command, boundary);
        }
        #endregion

        #region GET BOUNDS
        public IRectangle GetBounds(int xExpand = 1, int yExpand = 1)
        {
            if (X2 == 0 || Y2 == 0)
                return Rectangle.Empty;
            var x = X1 - xExpand;
            var y = Y1 - yExpand;
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            return Rectangle.FromLTRB(x, y, X2 + xExpand * 2, Y2 + yExpand * 2);
        }
        #endregion

        #region CLONE
        public IBoundary Clone()
        {
            var bdr = new Boundary();
            bdr.Copy(this);
            return bdr;
        }
        object ICloneable.Clone() =>
            Clone();
        #endregion

        #region OPERATORS
        public static implicit operator bool(Boundary boundary) =>
            boundary.Valid;
        #endregion

        #region GET ENUMERATOR
        public IEnumerator<Vector> GetEnumerator()
        {
            yield return new Vector(X1, Y1);
            yield return new Vector(X1, Y2);
            yield return new Vector(X2, Y2);
            yield return new Vector(X2, Y1);
        }
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion

        public override string ToString()
        {
            return string.Format(description, X1, Y1, X2, Y2);
        }
    }
}
#endif
