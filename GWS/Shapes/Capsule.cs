/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    #region ICAPSULE
    public interface ICapsule : IRoundBox
    { }
    #endregion

    #region CAPSULE
    /// <summary>
    /// Represents an object which has four sides just as Box but has all corners rounded to a certain radius.
    /// </summary>
    public struct Capsule : ICapsule, IExResizable
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

        public readonly VectorF[] Points;

        /// <summary>
        /// Radius of a circle at all four corner.
        /// </summary>
        public readonly float CornerRadius;

        const string toStr = "{0}, x: {1}, y: {2}, width: {3}, height: {4}, cornerRadius: {5}";
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new rouded box with specifed parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="w">Width of the rectangle.</param>
        /// <param name="h">Height of the rectangle.</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        public Capsule(float x, float y, float w, float h, float cornerRadius) : this()
        {
            X = (int)x;
            Y = (int)y;
            Width = (int)w;
            Height = (int)h;
            if (w - Width != 0)
                ++Width;
            if (h - Height != 0)
                ++Height;
            CornerRadius = cornerRadius;
            Points = Curves.CapsulePoints(x, y, w, h, CornerRadius);
        }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="area">Area to match bounds from.</param>
        public Capsule(IRectangle area, float cornerRadius) :
                this(area.X, area.Y, area.Width, area.Height, cornerRadius)
        { }

        /// <summary>
        /// Creates a new rect identical to the area of specifed rectangle.
        /// </summary>
        /// <param name="area">Area to copy bounds from.</param>
        public Capsule(IRectangleF area, float cornerRadius) :
            this(area.X, area.Y, area.Width, area.Height, cornerRadius)
        { }

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public Capsule(IPointF xy, ISizeF wh, float cornerRadius) :
            this(xy.X, xy.Y, wh.Width, wh.Height, cornerRadius)
        { }

        /// <summary>
        /// Creates a box matching the specifiedlocation and size.
        /// </summary>
        /// <param name="xy">Location of the box.</param>
        /// <param name="wh">Size of the box.</param>
        public Capsule(IPoint xy, ISize wh, float cornerRadius) :
            this(xy.X, xy.Y, wh.Width, wh.Height, cornerRadius)
        { }

        /// <summary>
        /// Creates a new rect with specifed left, top, right and bottom parameters.
        /// </summary>
        /// <param name="x">Far left horizontal co-rodinate of the rectangle.</param>
        /// <param name="y">Far top horizontal co-rodinate of the rectangle.</param>
        /// <param name="right">Far right horizontal co-rodinate of the rectangle.</param>
        /// <param name="bottom">Far bottom horizontal co-rodinate of the rectangle.</param>
        /// <returns>RectF</returns>
        public static Capsule FromLTRB(float x, float y, float right, float bottom, float cornerRadius)
        {
            return new Capsule(x, y, right - x, bottom - y, cornerRadius);
        }

        Capsule(ICapsule capsule, int w, int h)
        {
            X = capsule.X;
            Y = capsule.Y;
            Width = w;
            Height = h;
            CornerRadius = capsule.CornerRadius;
            Points = capsule.Points.Scale(new Scale(capsule, w, h)).ToArray();
        }
        Capsule(ICapsule capsule)
        {
            X = 0;
            Y = 0;
            Width = capsule.Width;
            Height = capsule.Height;
            CornerRadius = capsule.CornerRadius;
            var subtract = new Point(capsule.X, capsule.Y);
            Points = capsule.GetPoints().Select(pt => pt - subtract).ToArray();
        }
        #endregion

        #region PROPERTIES
        public bool IsEmpty =>
            Width == 0 && Height == 0;
        public bool Valid => Width >= 0 && Height >= 0;
        VectorF[] IRoundBox.Points => Points;
        bool IOriginCompatible.IsOriginBased => X == 0 && Y == 0;
        int IPoint.X => X;
        int IPoint.Y => Y;
        int ISize.Width => Width;
        int ISize.Height => Height;
        float IRoundBox.CornerRadius => CornerRadius;
        #endregion

        #region PERIMATER
        VectorF[] IPolygonal<VectorF>.GetPoints() =>
            Points;
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
            return new Capsule(this, w, h);
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            return new Capsule(this);
        }
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, "Capsule", X, Y, Width, Height, CornerRadius);
        }
    }  
    #endregion
}
#endif