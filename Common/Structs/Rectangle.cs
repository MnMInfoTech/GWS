/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    /// <summary>
    /// Represents a trapezium(as defined in the British English) which has parallel sides equal in length.
    /// Sides are represented in points consist of integer X & Y values.
    /// Also Opposite sides have an angle of 90 degree between them.
    /// </summary>
    public interface IRectangle : IBounds, IPoint, ISize, IObject, IParameter
    { 
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rectangle : IRectangle
#if GWS || Window
        , IDraw
#endif
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

        public static readonly Rectangle Empty = new Rectangle();
        public static readonly Rectangle EmptyBoundary = new Rectangle(int.MaxValue, int.MaxValue, 0, 0);
        static string description = "X: {0}, Y: {1}, W: {2}, H: {3}";
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new rectangle with specified parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-ordinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-ordinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        public Rectangle(int x, int y, int w, int h, bool positiveLocation = false)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            if (positiveLocation)
            {
                if (X < 0) X = 0;
                if (Y < 0) Y = 0;
            }
        }

        /// <summary>
        /// Creates a new rectangle identical to the area of specified rectangle.
        /// </summary>
        /// <param name="rc">Area to copy bounds from.</param>
        public Rectangle(int x, int y, int w, int h, ISize clip, int expansion) : this()
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
            X -= expansion;
            Y -= expansion;
            Width += expansion;
            Height += expansion;
            if (X < 0) X = 0;
            if (Y < 0) Y = 0;
            if (Height == clip.Height)
                --Height;
            if (Width == clip.Width)
                --Width;

            if (X + Width >= clip.Width)
                Width = clip.Width - X;
            if (Y + Height >= clip.Height)
                Height = clip.Height - Y;
        }

        /// <summary>
        /// Creates a new rectangle identical to the area of specified rectangle.
        /// </summary>
        /// <param name="rc">Area to copy bounds from.</param>
        public Rectangle(IBounds rc, ISize clip, int expansion) : this()
        {
            if (rc == null)
                return;
            rc.GetBounds(out X, out Y, out Width, out Height);

            this = new Rectangle(X, Y, Width, Height, clip, expansion);
        }

        /// <summary>
        /// Creates a new rectangle identical to the area of specified rectangle.
        /// </summary>
        /// <param name="rc">Area to copy bounds from.</param>
        public Rectangle(IBounds rc, bool positiveLocation = false) : this()
        {
            if (rc == null)
                return;
            rc.GetBounds(out X, out Y, out Width, out Height);
            if (positiveLocation)
            {
                if (X < 0) X = 0;
                if (Y < 0) Y = 0;
            }
        }

        /// <summary>
        /// Creates a new rectangle identical to the area of specified rectangle.
        /// </summary>
        /// <param name="rc">Area to copy bounds from.</param>
        public Rectangle(IBoundsF rc, bool positiveLocation = false) : this()
        {
            if (rc == null)
                return;
            rc.GetBounds(out float x, out float y, out float w, out float h);
            this = new Rectangle(x, y, w, h, positiveLocation);
        }

        /// <summary>
        /// Creates a new rectangle with specified parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-ordinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-ordinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        public Rectangle(float x, float y, float w, float h, bool positiveLocation = false)
        {
            X = (int)x;
            Y = (int)y;
            if (positiveLocation)
            {
                if (X < 0) X = 0;
                if (Y < 0) Y = 0;
            }
            Width = (int)w;
            Height = (int)h;
            if (w - Width != 0)
                ++Width;
            if (h - Height != 0)
                ++Height;
        }

        /// <summary>
        /// Creates a new rectangle with specified left, top, right and bottom parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-ordinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-ordinate of the rectangle.</param>
        /// <param name="right">Far right horizontal co-ordinate of the rectangle.</param>
        /// <param name="bottom">Far bottom horizontal co-ordinate of the rectangle.</param>
        /// <returns>Rectangle</returns>
        public static Rectangle FromLTRB(int x, int y, int right, int bottom, bool positiveLocation = false)
        {
            return new Rectangle(x, y, right - x, bottom - y, positiveLocation);
        }

        /// <summary>
        /// Creates a new rectangle with specified left, top, right and bottom parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-ordinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-ordinate of the rectangle.</param>
        /// <param name="right">Far right horizontal co-ordinate of the rectangle.</param>
        /// <param name="bottom">Far bottom horizontal co-ordinate of the rectangle.</param>
        /// <returns>Rectangle</returns>
        public static Rectangle FromLTRB(float x, float y, float right, float bottom, bool positiveLocation = false)
        {
            return new Rectangle(x, y, right - x, bottom - y, positiveLocation);
        }

        /// <summary>
        /// Creates a box matching the specified location and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public Rectangle(IPointF xy, ISizeF wh, bool positiveLocation = false) :
            this(xy.X, xy.Y, wh.Width, wh.Height, positiveLocation)
        { }

        /// <summary>
        /// Creates a box matching the specified location and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public Rectangle(IPoint xy, ISize wh, bool positiveLocation = false) :
            this(xy?.X ?? 0, xy?.Y ?? 0, wh.Width, wh.Height, positiveLocation)
        { }

        /// <summary>
        /// Creates a box matching the specified location and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public Rectangle(IPoint xy, int w, int h, bool positiveLocation = false) :
            this(xy?.X ?? 0, xy?.Y ?? 0, w, h, positiveLocation)
        { }

        /// <summary>
        /// Creates a new square with specified parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-ordinate of the box.</param>
        /// <param name="y">Far top horizontal co-ordinate of the box.</param>
        /// <param name="w">Width of the box.</param>
        public Rectangle(float x, float y, float w, bool positiveLocation = false) :
            this(x, y, w, w, positiveLocation)
        { }

        /// <summary>
        /// Creates rectangle from the location and size of given object.
        /// </summary>
        /// <param name="object">Any object which implements IObject interface.</param>
        public Rectangle(IObject @object):
            this()
        {
            if (@object == null)
                return;
            X = @object.X;
            Y = @object.Y;
            Width = @object.Width;
            Height = @object.Height;
        }
        #endregion

        #region PROPERTIES
        public bool Valid => Width != 0 && Height != 0;

        /// <summary>
        /// X co-ordinate of center of this object.
        /// </summary>
        public int Cx => X + Width / 2;

        /// <summary>
        /// Y co-ordinate of center of this object.
        /// </summary>
        public int Cy => Y + Height / 2;

        public int Right => X + Width;
        public int Bottom => Y + Height;

        int IPoint.X => X;
        int IPoint.Y => Y;
        int ISize.Width => Width;
        int ISize.Height => Height;
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

        #region OPERATORS
        public static implicit operator bool(Rectangle r) =>
            r.Width != 0 && r.Height != 0;
        public static bool operator ==(Rectangle a, Rectangle b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Rectangle a, Rectangle b)
        {
            return !a.Equals(b);
        }
        public static explicit operator Point (Rectangle r)=>
            new Point(r.X, r.Y);
        public static explicit operator Size(Rectangle r) =>
            new Size(r.Width, r.Height);
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

#if GWS || Window
        #region DRAW
        public bool Draw(IEnumerable<IParameter> parameters, IRenderer renderer)
        {
            var action = renderer.CreateRenderAction(parameters);
            action(null, new ILine[]
            {
                new Line(X, Y, X, Y + Height),
                new Line(X, Y + Height, X + Width, Y + Height),
                new Line(X + Width, Y + Height, X + Width, Y),
                new Line(X + Width, Y, X, Y)
            }, null);
            return true;
        }
        #endregion
#endif

        public override string ToString()
        {
            return string.Format(description, X, Y, Width, Height);
        }
    }
}
