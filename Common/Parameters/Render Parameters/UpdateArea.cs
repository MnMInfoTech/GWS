/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System.Runtime.CompilerServices;
using System;

namespace MnM.GWS
{
    #region IUPDATE AREA
    /// <summary>
    /// Represents an object which reveals an information absolute other object's type and perimeter.
    /// </summary>
    public interface IUpdateArea : IRectangle, ITypedBounds, IHitTestable, IParameter
    { }
    #endregion

    public struct UpdateArea : IUpdateArea, IEquatable<IBounds>
    {
        #region VARIABLES
        /// <summary>
        /// X co-ordinate of location of this object.
        /// </summary>
        public int X;

        /// <summary>
        /// Y co-ordinate of location of this object.
        /// </summary>
        public int Y;

        /// <summary>
        /// Width of this object.
        /// </summary>
        public int Width;

        /// <summary>
        /// Height of this object.
        /// </summary>
        public int Height;

        /// <summary>
        /// GWS defined type of screen object.
        /// </summary>
        public readonly ObjType Type;

        public static readonly UpdateArea Empty = new UpdateArea();
        static string description = "X: {0}, Y: {1}, W: {2}, H: {3}, Type: {4}";
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Creates a new rect with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        public UpdateArea(int x, int y, int w, int h, ObjType? type = null) : this()
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            Type = type ?? 0;
        }

        /// <summary>
        /// Creates a new rect with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        public UpdateArea(float x, float y, float w, float h, ObjType? type = null)
        {
            X = (int)x;
            Y = (int)y;
            Width = (int)w;
            Height = (int)h;
            if (w - Width != 0)
                ++Width;
            if (h - Height != 0)
                ++Height;
            Type = type ?? 0;
        }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="rc">Area to copy bounds from.</param>
        public UpdateArea(IBounds rc, ObjType? type = null) : this()
        {
            rc?.GetBounds(out X, out Y, out Width, out Height);
            if (type != null)
                Type = type.Value;
            else if (rc is IType)
                Type = ((IType)rc).Type;

        }
        public UpdateArea(IBounds rc1, IBounds rc2) :
            this()
        {
            if (rc1 == null && rc2 == null)
                return;
            ObjType? type = null;
            if (rc1 is IType)
                type = ((IType)rc1).Type;
            else if (rc2 is IType)
                type = ((IType)rc2).Type;

            if (rc1 == null || !rc1.Valid)
            {
                this = new UpdateArea(rc2, type);
                return;
            }
            if (rc2 == null || !rc2.Valid)
            {
                this = new UpdateArea(rc1, type);
                return;
            }

            int Right, Bottom;
            rc1.GetBounds(out X, out Y, out Width, out Height);
            Right = X + Width;
            Bottom = Y + Height;

            int x1, y1, w1, h1, r1, b1;
            rc2.GetBounds(out x1, out y1, out w1, out h1);
            r1 = x1 + w1;
            b1 = y1 + h1;
            if (x1 < X)
                X = x1;
            if (y1 < Y)
                Y = y1;
            if (r1 > Right)
                Right = r1;
            if (b1 > Bottom)
                Bottom = b1;

            Width = Right - X;
            Height = Bottom - Y;

            if (type != null)
                Type = type.Value;
        }
        /// <summary>
        /// Creates a new rect with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        public UpdateArea(IParameter parameter, int x, int y, int w, int h, ObjType? type = null) : this()
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            if (type != null)
                Type = type.Value;
            else if (parameter is IType)
                Type = ((IType)parameter).Type;
        }

        /// <summary>
        /// Creates a new rect with specifed left, top, right and bottom parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="right">Far right horizontal co-rodinate of the rectangle.</param>
        /// <param name="bottom">Far bottom horizontal co-rodinate of the rectangle.</param>
        /// <returns>RectF</returns>
        public static UpdateArea FromLTRB(int x, int y, int right, int bottom, ObjType? type = null)
        {
            return new UpdateArea(x, y, right - x, bottom - y, type);

        }

        /// <summary>
        /// Creates a new rect with specifed left, top, right and bottom parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="right">Far right horizontal co-rodinate of the rectangle.</param>
        /// <param name="bottom">Far bottom horizontal co-rodinate of the rectangle.</param>
        /// <returns>RectF</returns>
        public static UpdateArea FromLTRB(float x, float y, float right, float bottom, ObjType? type = null)
        {
            return new UpdateArea(x, y, right - x, bottom - y, type);
        }

        public static UpdateArea FromLTRB(IBounds parameter, int x, int y, int r, int b,
         ObjType? type = null)
        {
            type = type ?? (parameter as IType)?.Type;
            return new UpdateArea(x, y, r - x, b - y, type);
        }
        #endregion

        #region PROPERTIES
        public bool Valid => Width > 0 && Height > 0;
        public int Right => X + Width;
        public int Bottom => Y + Height;
        int IPoint.X => X;
        int IPoint.Y => Y;
        int ISize.Width => Width;
        int ISize.Height => Height;
        ObjType IType.Type => Type;
        #endregion

        #region GET BOUNDS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetBounds(out int x, out int y, out int w, out int h)
        {
            if (Width == 0 || Height == 0)
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

        #region CONTAINS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(float x, float y)
        {
            if (Width == 0 || Height == 0 || x < X || y < Y || x > X + Width || y > Y + Height)
                return false;
            return true;
        }
        #endregion

        #region EQUALITY
        public bool Equals(IBounds rect)
        {
            return Rectangles.Equals(rect, X, Y, Width, Height);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is IBounds)
                return Rectangles.Equals((IBounds)obj, X, Y, Width, Height);
            return false;
        }

        public override int GetHashCode()
        {
            return new { X, Y, Width, Height }.GetHashCode();
        }
        #endregion

        public override string ToString()
        {
            return string.Format(description, X, Y, Width, Height, Type);
        }
    }
}
