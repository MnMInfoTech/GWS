/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MnM.GWS
{
    #region IBOX
    /// <summary>
    /// Represents a trapezium(as defined in the British English) which has parallel sides equal in length.
    /// Also Oppsites sides have an agle of 90 degree between them.
    /// Sides are represented in points consist of float X & Y values.
    /// </summary>
    public interface IBox : IShape, IFigure, IRectangle, ICount
    { }
    #endregion

    #region BOX
    /// <summary>
    /// Represents a trapezium(as defined in the British English) which has parallel sides equal in length.
    /// Sides are represented in points consist of integer X & Y values.
    /// Also Oppsite sides have an agle of 90 degree between them.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Box : IBox, IExDraw, IExResizable
    {
        #region VARIABLES
        /// <summary>
        /// Far left horizontal corodinate of this object.
        /// </summary>
        public readonly int X;

        /// <summary>
        /// Far top vertical corodinate of this object.
        /// </summary>
        public readonly int Y;

        /// <summary>
        /// far right horizontal corodinate (X + Width) of this object.
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// Deviation from the far top vertical corodinate (Y) of this object.
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// Empty instance of this object.
        /// </summary>
        public static readonly Box Empty = new Box();

        const string toStr = "{0}, x: {1}, y: {2}, w: {3}, h: {4}";
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new rect with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        public Box(int x, int y, int w, int h) : this()
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
        public Box(float x, float y, float w, float h) :
                this(x.Round(), y.Round(), w.Round(), h.Round())
        { }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="area">Area to copy bounds from.</param>
        public Box(IBounds area) : this()
        {
            if (area == null)
                return;
            area.GetBounds(out X, out Y, out Width, out Height);
        }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="area">Area to copy bounds from.</param>
        public Box(IBoundsF area) : this()
        {
            if (area == null)
                return;
            float x, y, w, h;
            area.GetBounds(out x, out y, out w, out h);
            this=new Box(x, y, w, h);
        }

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public Box(IPointF xy, ISizeF wh) :
            this(xy.X, xy.Y, wh.Width, wh.Height)
        { }

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public Box(IPoint xy, ISize wh) :
            this(xy.X, xy.Y, wh.Width, wh.Height)
        { }

        /// <summary>
        /// Creates a new square with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the box.</param>
        /// <param name="y">Far top horizontal co-rodinate of the box.</param>
        /// <param name="w">Width of the box.</param>
        public Box(float x, float y, float w) :
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
        public static Box FromLTRB(float x, float y, float right, float bottom, bool correct = true)
        {
            if (!correct)
                return new Box(x, y, right - x, bottom - y);
            Numbers.Order(ref x, ref right);
            Numbers.Order(ref y, ref bottom);
            var w = right - x;
            if (w == 0)
                w = 1;
            var h = bottom - y;
            if (h == 0)
                h = 1;
            return new Box(x, y, w, h);
        }
        Box(IBox box, int w, int h)
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
        public int Cx => X + Width / 2;

        /// <summary>
        /// Y co-ordinate of center of this object.
        /// </summary>
        public int Cy => Y + Height / 2;
        public bool Valid => Width >= 0 && Height >= 0;

        int IPoint.X => X;
        int IPoint.Y => Y;
        int ISize.Width => Width;
        int ISize.Height => Height;
        int ICount.Count => 4;
        bool IOriginCompatible.IsOriginBased => X == 0 && Y == 0;
        #endregion

        #region EQUALITY
        public bool Equals(IBounds other)
        {
            if (!Valid && other == null)
                return true;
            if (other == null)
                return false;
            if (!Valid && !other.Valid)
                return true;
            other.GetBounds(out int x, out int y, out int w, out int h);

            return X == x && Y == y && Width == w && Height == h;
        }
        public override bool Equals(object obj)
        {
            if (obj is IBounds)
                return Equals((IBounds)obj);
            return false;
        }
        public override int GetHashCode()
        {
            return new { X, Y, Width, Height }.GetHashCode();
        }
        #endregion

        #region GET BOUNDS
        void IBounds.GetBounds(out int x, out int y, out int w, out int h)
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
            if (Width <= 0 || Height <= 0)
                return true;

            parameters.Extract(out IExSession session);

            //fillmode, rotation, scale, bounds, command, linecommand
            bool OriginalFill = session.Command == 0 || (session.Command & Command.OriginalFill) == Command.OriginalFill;
            bool DrawOutLine = (session.Command & Command.DrawOutLines) == Command.DrawOutLines;

            if ((session.Rotation == null || !session.Rotation.Valid)
                && (session.Scale == null || !session.Scale.HasScale) 
                &&  (session.Stroke == 0 ||  OriginalFill))
            {
                session.SetPen(this);
                var B = Y + Height;
                var R = X + Width;

                var action = renderer.CreateRenderAction(session);
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
                    fillLines = new PrimitiveList<IAxisLine>(Height);
                    int y = Y;
                    int x = X;
                    while (y <= B)
                    {
                        fillLines.Add(new AxisLine(x, R, y, draw));
                        ++y;
                    }
                }
                return action(fillLines, drawLines, null);//, Bresenham.Yes);                
            }            
            renderer.RenderPolygon(this, session);
            return true;
        }
        #endregion

        #region OPERATORS
        public static implicit operator bool(Box r) =>
            r.Valid;
        public static bool operator ==(Box a, Rectangle b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Box a, Rectangle b)
        {
            return !a.Equals(b);
        }
        public static explicit operator RectangleF(Box r) =>
            new RectangleF(r.X, r.Y, r.Width, r.Height);

        public static explicit operator BoxF(Box r) =>
            new BoxF(r.X, r.Y, r.Width, r.Height);
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
            return new Box(this, w, h);
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

        public override string ToString() =>
            string.Format(toStr, "Box", X, Y, Width, Height);
    }   
    #endregion
}
#endif
