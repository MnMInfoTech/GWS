/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    #region IBOXF
    /// <summary>
    /// Represents a trapezium(as defined in the British English) which has parallel sides equal in length.
    /// Also Oppsites sides have an agle of 90 degree between them.
    /// Sides are represented in points consist of float X & Y values.
    /// </summary>
    public interface IBoxF : IRectangleF, IShape, IFigure, IEquatable<IRectangleF>, ICount
    { }
    #endregion

    #region BOXF
    /// <summary>
    /// Represents a trapezium(as defined in the British English) which has parallel sides equal in length.
    /// Also Oppsites sides have an agle of 90 degree between them.
    /// Sides are represented in points consist of float X & Y values.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BoxF : IBoxF, IExDraw, IExResizable
    {
        #region VARIABLES
        /// <summary>
        /// X co-ordinate of location of this object.
        /// </summary>
        public float X;

        /// <summary>
        /// Y co-ordinate of location of this object.
        /// </summary>
        public float Y;

        /// <summary>
        /// Width of this object.
        /// </summary>
        public float Width;

        /// <summary>
        /// Height of this object.
        /// </summary>
        public float Height;

        const string toStr = "{0}, x: {1}, y: {2}, w: {3}, h: {4}";
        public static readonly BoxF Empty = new BoxF();
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new rect with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        public BoxF(float x, float y, float w, float h) : this()
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="r">Area to copy bounds from.</param>
        public BoxF(Rectangle r) :
            this(r.X, r.Y, r.Width, r.Height)
        { }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="area">Area to copy bounds from.</param>
        public BoxF(RectangleF r) :
            this(r.X, r.Y, r.Width, r.Height)
        { }
        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public BoxF(VectorF xy, SizeF wh) :
            this(xy.X, xy.Y, wh.Width, wh.Height)
        { }

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public BoxF(Vector xy, Size wh) :
            this(xy.X, xy.Y, wh.Width, wh.Height)
        { }

        /// <summary>
        /// Creates a new square with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the box.</param>
        /// <param name="y">Far top horizontal co-rodinate of the box.</param>
        /// <param name="w">Width of the box.</param>
        public BoxF(float x, float y, float w) :
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
        public static BoxF FromLTRB(float x, float y, float right, float bottom, bool correct = true)
        {
            if (!correct)
                return new BoxF(x, y, right - x, bottom - y);
            Numbers.Order(ref x, ref right);
            Numbers.Order(ref y, ref bottom);
            var w = right - x;
            if (w == 0)
                w = 1;
            var h = bottom - y;
            if (h == 0)
                h = 1;
            return new BoxF(x, y, w, h);
        }

        BoxF(BoxF box, int w, int h)
        {
            X = box.X;
            Y = box.Y;
            Width = w;
            Height = h;
        }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// X co-ordinate of center of this object.
        /// </summary>
        public float Cx => X + Width / 2;

        /// <summary>
        /// Y co-ordinate of center of this object.
        /// </summary>
        public float Cy => Y + Height / 2;

        public bool Valid => Width > 0 && Height > 0;

        float IPointF.X => X;
        float IPointF.Y => Y;
        float ISizeF.Width => Width;
        float ISizeF.Height => Height;
        int ICount.Count => 4;
        bool IOriginCompatible.IsOriginBased => X == 0 && Y == 0;
        int IPoint.X => (int)X;
        int IPoint.Y => (int)Y;
        int ISize.Width
        {
            get
            {
                var fw = Width;
                var w = (int)fw;
                if (fw - w != 0)
                    ++w;
                return w;
            }
        }
        int ISize.Height
        {
            get
            {
                var fh = Height;
                var h = (int)fh;
                if (fh - h != 0)
                    ++h;
                return h;
            }
        }
        #endregion

        #region GET BOUNDS
        public void GetBounds(out float x, out float y, out float w, out float h)
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

        #region DRAW
        bool IExDraw.Draw(IEnumerable<IParameter> parameters, IExRenderer renderer)
        {
            parameters.Extract(out IExSession info);

            bool OriginalFill = info.Command == 0 || (info.Command & Command.OriginalFill) == Command.OriginalFill;
            bool DrawOutLine = (info.Command & Command.DrawOutLines) == Command.DrawOutLines;

            if (
                (info.Rotation == null || !info.Rotation.Valid)
                &&(info.Scale == null || !info.Scale.HasScale)
                &&(info.Stroke == 0 || OriginalFill))
            {
                info.SetPen(this);
                var B = Y + Height;
                var R = X + Width;
                var action = renderer.CreateRenderAction(info);
                var draw = LineFill.Horizontal;

                var drawLines = new ILine[]
                {
                    new Line(X, Y, R, Y),
                    new Line(R, Y, R, B),
                    new Line(R, B, X, B),
                    new Line(X, B, X, Y)
                };
                PrimitiveList<IAxisLine> fillLines = null;
                if (!DrawOutLine)
                {
                    int y = (int)Y;
                    int x = (int)X;
                    int b = B.Round();
                    var r = R.Round();

                    fillLines = new PrimitiveList<IAxisLine>(b - y);

                    while (y <= b)
                    {
                        fillLines.Add(new AxisLine(x, r, y, draw));
                        ++y;
                    }
                }
                return action(fillLines, drawLines, null);
            }
            renderer.RenderPolygon(this, info);
            return true;
        }
        #endregion

        #region EQUALITY
        public bool Equals(IRectangleF other)
        {
            if (other == null)
                return false;
            if (!Valid && other.Valid || Valid && !other.Valid)
                return false;
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }
        public override bool Equals(object obj)
        {
            if (obj is IRectangleF)
                return Equals((BoxF)obj);
            else if (obj is Rectangle)
                return Equals((BoxF)obj);
            return false;
        }
        public override int GetHashCode()
        {
            return new { X, Y, Width, Height }.GetHashCode();
        }
        #endregion

        #region OPERATORS
        public static implicit operator bool(BoxF r) =>
            r.Valid ;
        public static bool operator ==(BoxF a, BoxF b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(BoxF a, BoxF b)
        {
            return !a.Equals(b);
        }
        #endregion

        #region PERIMETER
        VectorF[] IPolygonal<VectorF>.GetPoints()
        {
           return new VectorF[]
           {
                new VectorF(X, Y),
                new VectorF(X + Width, Y),
                new VectorF(X + Width, Y + Height),
                new VectorF(X, Y + Height),
           };
        }
        #endregion

        #region RESIZE
        object IExResizable.Resize(int w, int h, out bool success, ResizeCommand resizeCommand)
        {
            success = false;
            var iw = ((ISize)this).Width;
            var ih = ((ISize)this).Height;

            if
            (
               (w == iw && h == ih) ||
               (w == 0 && h == 0)
            )
            {
                return this;
            }

            bool SizeOnlyToFit = (resizeCommand & ResizeCommand.SizeOnlyToFit) == ResizeCommand.SizeOnlyToFit;

            if (SizeOnlyToFit && iw > w && ih > h)
                return this;

            if (SizeOnlyToFit)
            {
                if (w < iw)
                    w = iw;
                if (h < ih)
                    h = ih;
            }
            success = true;
            return new BoxF(this, w, h);
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            return new Box(0, 0, Width, Height);
        }
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, "BoxF", X, Y, Width, Height);
        }
    }
    #endregion
}
#endif
