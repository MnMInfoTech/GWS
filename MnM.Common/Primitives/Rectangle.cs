/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    /// <summary>
    /// Represents a trapezium(as defined in the British English) which has parallel sides equal in length.
    /// Sides are represented in points consist of integer X & Y values.
    /// Also Oppsite sides have an agle of 90 degree between them.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle : IRectangle, IEquatable<Rectangle>
    {
        #region VARIABLES
        public int X, Y, Width, Height;
        public static Rectangle Empty = new Rectangle();
        static string description = "X: {0}, Y: {1}, W: {2}, H: {3}";
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new rect with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        public unsafe Rectangle(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        /// <summary>
        /// Creates a new rect with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        public Rectangle(float x, float y, float w, float h) :
            this(x.Round(), y.Round(), w.Round(), h.Round())
        { }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="area">Area to copy bounds from.</param>
        public Rectangle(IRectangle area) :
            this(area.X, area.Y, area.Width, area.Height)
        { }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="area">Area to copy bounds from.</param>
        public Rectangle(IRectangleF area) :
                this(area.X, area.Y, area.Width, area.Height)
        { }

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public Rectangle(IPointF xy, ISizeF wh) :
            this(xy.X, xy.Y, wh.Width, wh.Height)
        { }

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public Rectangle(IPoint xy, ISize wh) :
            this(xy.X, xy.Y, wh.Width, wh.Height)
        { }

        /// <summary>
        /// Creates a new square with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the box.</param>
        /// <param name="y">Far top horizontal co-rodinate of the box.</param>
        /// <param name="w">Width of the box.</param>
        public Rectangle(float x, float y, float w) :
            this(x, y, w, w)
        { }


        /// <summary>
        /// Creates a new rect with specifed left, top, right and bottom parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="right">Far right horizontal co-rodinate of the rectangle.</param>
        /// <param name="bottom">Far bottom horizontal co-rodinate of the rectangle.</param>
        /// <returns>RectF</returns>
        public static Rectangle FromLTRB(int x, int y, int right, int bottom, bool correct = true)
        {
            if (!correct)
                return new Rectangle(x, y, right - x, bottom - y);
            Numbers.Order(ref x, ref right);
            Numbers.Order(ref y, ref bottom);
            var w = right - x;
            if (w == 0)
                w = 1;
            var h = bottom - y;
            if (h == 0)
                h = 1;
            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// Creates a new rect with specifed left, top, right and bottom parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="right">Far right horizontal co-rodinate of the rectangle.</param>
        /// <param name="bottom">Far bottom horizontal co-rodinate of the rectangle.</param>
        /// <returns>RectF</returns>
        public static Rectangle FromLTRB(float x, float y, float right, float bottom, bool correct = true)
        {
            if (!correct)
                return new Rectangle(x, y, right - x, bottom - y);
            Numbers.Order(ref x, ref right);
            Numbers.Order(ref y, ref bottom);
            var w = right - x;
            if (w == 0)
                w = 1;
            var h = bottom - y;
            if (h == 0)
                h = 1;
            return new Rectangle(x, y, w, h);
        }
        #endregion

        #region PROPERTIES
        public int Right => X + Width;
        public int Bottom => Y + Height;
        /// <summary>
        /// X co-ordinate of center of this object.
        /// </summary>
        public int Cx => X + Width / 2;

        /// <summary>
        /// Y co-ordinate of center of this object.
        /// </summary>
        public int Cy => Y + Height / 2;
        int IPoint.X => X;
        int IPoint.Y => Y;
        int ISize.Width => Width;
        int ISize.Height => Height;
        public bool Valid => Width != 0 && Height != 0;
        #endregion

        #region EQUALITY
        public bool Equals(Rectangle other)
        {
            return Rects.Equals(other, X, Y, Width, Height);
        }
        public override bool Equals(object obj)
        {
            if (obj is IBoundable)
                return Rects.Equals((IBoundable)obj, X, Y, Width, Height);
            return false;
        }
        public override int GetHashCode()
        {
            return new { X, Y, Width, Height }.GetHashCode();
        }
        #endregion

        #region GET BOUNDS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

#if GWS || Window
        #region DRAW
        public void Draw(IWritable buffer, ulong command = 0)
        {
            PixelAction action;
            IReadable pen = null;
            if (buffer is IReadable)
            {
                pen = buffer as IReadable;
            }
            if (pen == null)
                pen = Pens.Black;

            command |= Command.WriteToScreen;
            var boundary = new Session();
            boundary.Choice = ReadChoice.InvertColor;

            buffer.CreatePixelAction(pen, out action, boundary);
            Renderer.ProcessLine(X, Y, X, Y + Height, action, command);
            Renderer.ProcessLine(X, Y + Height, X + Width, Y + Height, action, command);
            Renderer.ProcessLine(X + Width, Y + Height, X + Width, Y, action, command);
            Renderer.ProcessLine(X + Width, Y, X, Y, action, command);
        }
        #endregion
#endif

        #region OPERATORS
        public static implicit operator bool(Rectangle r) =>
            r.Width> 0 && r.Height > 0;
        public static bool operator ==(Rectangle a, Rectangle b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Rectangle a, Rectangle b)
        {
            return !a.Equals(b);
        }

#if GWS || Window
        public static explicit operator RectangleF(Rectangle r) =>
            new RectangleF(r.X, r.Y, r.Width, r.Height);
#endif
        #endregion

        public override string ToString()
        {
            return string.Format(description, X, Y, Width, Height);
        }
    }
}
