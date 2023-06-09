/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice must not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if GWS || Window

using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    #region IELLIPSE
    public interface IEllipse : IShape, IFigure
    {
        /// <summary>
        /// X axis radius of this conic.
        /// </summary>
        float Rx { get; }

        /// <summary>
        /// Y axis radius of this conic.
        /// </summary>
        float Ry { get; }
    }
    #endregion

    #region ELLIPSE
    public struct Ellipse: IEllipse, IExResizable
    {
        #region VARIABLES
        /// <summary>
        /// Far left cordinate where this conic starts from.
        /// </summary>
        public readonly float Cx;

        /// <summary>
        /// Far top cordinate where this conic starts from.
        /// </summary>
        public readonly float Cy;

        /// <summary>
        /// Radius of this conic on X axis.
        /// </summary>
        public readonly float Rx;

        /// <summary>
        /// Radius of this conic on Y axis.
        /// </summary>
        public readonly float Ry;

        readonly VectorF[] Points;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new ellipse specified by the bounding area.
        /// </summary>
        /// <param name="x">X cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="w">Width of a bounding area where the circle/ellipse is to be drawn -> 
        /// ellipse's minor X axis = Width/2</param>
        public Ellipse(float x, float y, float w)
            : this(x, y, w, w) { }

        /// <summary>
        /// Creates a new ellipse specified by the bounding area.
        /// </summary>
        /// <param name="x">X cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="w">Width of a bounding area where the circle/ellipse is to be drawn ->
        /// ellipse's minor X axis = Width/2</param>
        /// <param name="h">Height of a bounding area where the circle is to be drawn ->
        /// ellipse's minor Y axis = Height/2</param>
        public Ellipse(float x, float y, float w, float h)
        {
            Rx = w / 2f;
            Ry = h / 2f;
            Cx = x + Rx;
            Cy = y + Ry;
            Points = Curves.GetEllipsePoints(Cx, Cy, Rx, Ry);
            Points[0] = VectorF.Empty;
        }

        /// <summary>
        /// Creates a new ellipse specified by the bounding area.
        /// </summary>
        /// <param name="x">X cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="w">Width of a bounding area where the circle/ellipse is to be drawn -> circle/ellipse's minor X axis = Width/2</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Ellipse(float x, float y, float w, params IParameter[] parameters)
            : this(x, y, w, w, parameters)
        { }

        /// <summary>
        /// Creates a new ellipse specified by the bounding area.
        /// </summary>
        /// <param name="x">X cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="w">Width of a bounding area where the circle/ellipse is to be drawn -> circle/ellipse's minor X axis = Width/2</param>
        /// <param name="h">Height of a bounding area where the circle is to be drawn ->circle/ellipse's minor Y axis = Height/2</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Ellipse(float x, float y, float w, float h, params IParameter[] parameters)
            : this(parameters, x, y, w, h) 
        { }

        /// <summary>
        /// Creates a new ellipse specified by the bounding area.
        /// </summary>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        /// <param name="x">X cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="w">Width of a bounding area where the circle/ellipse is to be drawn -> circle/ellipse's minor X axis = Width/2</param>
        /// <param name="h">Height of a bounding area where the circle is to be drawn ->circle/ellipse's minor Y axis = Height/2</param>
        public Ellipse(IEnumerable<IParameter> parameters, float x, float y, float w, float h)
        {
            var bounds = new RectangleF(x, y, w, h);

            parameters.ExtractRotationScaleParameters(out IRotation rotation, out IScale scale);
            bounds = bounds.Scale(rotation, scale);
            bounds.GetBounds(out x, out y, out w, out h);

            Rx = w / 2f;
            Ry = h / 2f;
            Cx = x + Rx;
            Cy = y + Ry;

            if (rotation != null && rotation.Valid && rotation.Centre == null)
            {
                var oc = rotation.Centre;
                rotation.SetCentre(Cx, Cy);
                Points = Curves.GetEllipsePoints(Cx, Cy, Rx, Ry, rotation: rotation);
                rotation.SetCentre( oc?.Cx, oc?.Cy);
            }
            else
                Points = Curves.GetEllipsePoints(Cx, Cy, Rx, Ry, rotation: rotation);
            Points[0] = VectorF.Empty;
        }

        /// <summary>
        /// Creates a new ellipse specified by the bounding area.
        /// </summary>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        /// <param name="x">X cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="w">Width of a bounding area where the circle/ellipse is to be drawn -> circle/ellipse's minor X axis = Width/2</param>
        /// <param name="h">Height of a bounding area where the circle is to be drawn ->circle/ellipse's minor Y axis = Height/2</param>
        public Ellipse(IRotation rotation, float x, float y, float w, float h)
        {
            Rx = w / 2f;
            Ry = h / 2f;
            Cx = x + Rx;
            Cy = y + Ry;

            if (rotation != null && rotation.Valid && rotation.Centre == null)
            {
                var oc = rotation.Centre;
                rotation.SetCentre(Cx, Cy);
                Points = Curves.GetEllipsePoints(Cx, Cy, Rx, Ry, rotation: rotation);
                rotation.SetCentre(oc?.Cx, oc?.Cy);
            }
            else
                Points = Curves.GetEllipsePoints(Cx, Cy, Rx, Ry, rotation: rotation);
            Points[0] = VectorF.Empty;
        }
        public Ellipse(IBounds bounds)
        {
            bounds.GetBounds(out int x, out int y, out int w, out int h);
            this = new Ellipse(x, y, w, h);
        }
        public Ellipse(IBoundsF bounds)
        {
            bounds.GetBounds(out float x, out float y, out float w, out float h);
            this = new Ellipse(x, y, w, h);
        }
        Ellipse(IEllipse ellipse, int w, int h)
        {
            Rx = w / 2f;
            Ry = h / 2f;
            Cx = ellipse.X + Rx;
            Cy = ellipse.Y + Ry;

            Points = ellipse.GetPoints().Scale(new Scale(ellipse, w, h)).ToArray();
        }

        Ellipse(IEllipse ellipse)
        {
            Rx = ellipse.Rx;
            Ry = ellipse.Ry;
            Cx = Rx;
            Cy = Ry;
            var subtract = new Point(ellipse.X, ellipse.Y);
            Points = ellipse.GetPoints().Select(pt => pt - subtract).ToArray();
        }
        #endregion

        #region PROPERTIES
        public bool Valid => Rx > 0 && Ry > 0;
        public string TypeName => "Ellipse";
        float IEllipse.Rx => Rx;
        float IEllipse.Ry => Ry;
        bool IOriginCompatible.IsOriginBased => (int)(Cx - Rx) == 0 && (int)(Cy - Ry) == 0;
        int IPoint.X => (int)(Cx - Rx);
        int IPoint.Y => (int)(Cy - Ry);
        int ISize.Width
        {
            get
            {
                var fw = Rx * 2;
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
                var fh = Ry * 2;
                var h = (int)fh;
                if (fh - h != 0)
                    ++h;
                return h;
            }
        }
        #endregion

        #region PERIMETER
        VectorF[] IPolygonal<VectorF>.GetPoints() =>
            Points;
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            return new Ellipse(this);
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
            return new Ellipse(this, w, h);
        }
        #endregion
    }
    #endregion
}
#endif
