/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

#if GWS || Window

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    #region IPIE
    public interface IPie : IArc
    { }
    #endregion

    #region PIE
    public struct Pie : IPie, IExResizable
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
        /// X axis radius of this conic.
        /// </summary>
        public readonly float Rx;

        /// <summary>
        /// Y axis radius of this conic.
        /// </summary>
        public readonly float Ry;

        /// <summary>
        /// Start angle from where a curve start.
        /// </summary>
        public readonly float StartAngle;

        /// <summary>
        /// End Angle where a curve stops.
        /// </summary>
        public readonly float EndAngle;

        readonly VectorF[] Points;
        const string toStr = "{0}, x: {1}, y: {2}, w: {3}, h: {4}, start: {5}, end: {6}";
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new pie specified by the bounding area, start and end angles.
        /// </summary>
        /// <param name="x">X cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="w">Width of a bounding area where the pie is to be drawn -> 
        /// circle/ellipse's minor X axis = Width/2</param>
        /// <param name="h">Height of a bounding area where the pie is to be drawn ->
        /// circle/ellipse's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. sweepAngle option is true
        /// effective end angle is start angle + end angle</param>
        /// <param name="negativeMotion">Points of this pie are processed in anti clock wise motion.
        /// i.e. if start angle = 0 and end angle = 90, this option will draw an pie or pie from 90 to 360.</param>
        /// <param name="sweepAngle">If true effective end angle is start angle + end angle</param>
        /// <param name="pieAngle">If true, start and end angles are adjusted to pie compatible angles.</param>
        public Pie(float x, float y, float w, float h, float startAngle, float endAngle,
            bool negativeMotion = false, bool sweepAngle = true, bool pieAngle = true)
        {
            Rx = w / 2f;
            Ry = h / 2f;
            Cx = x + Rx;
            Cy = y + Ry;
            StartAngle = startAngle;
            EndAngle = endAngle;

            Points = Curves.GetArcPoints(StartAngle, EndAngle, true,
                    Cx, Cy, Rx, Ry, null, negativeMotion, sweepAngle, pieAngle);

            Points = Points.Where(pt => pt).ToArray();
        }

        /// <summary>
        /// Creates a new pie specified by the bounding area, start and end angles 
        /// and parameters which may include rotation and/or scale..
        /// </summary>
        /// <param name="x">X cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="w">Width of a bounding area where the pie is to be drawn -> 
        /// circle/ellipse's minor X axis = Width/2</param>
        /// <param name="h">Height of a bounding area where the pie is to be drawn ->
        /// circle/ellipse's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. sweepAngle option is true
        /// effective end angle is start angle + end angle</param>
        /// <param name="negativeMotion">Points of this pie are processed in anti clock wise motion.
        /// i.e. if start angle = 0 and end angle = 90, this option will draw an pie or pie from 90 to 360.</param>
        /// <param name="sweepAngle">If true effective end angle is start angle + end angle</param>
        /// <param name="pieAngle">If true, start and end angles are adjusted to pie compatible angles.</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Pie(float x, float y, float w, float h, float startAngle, float endAngle,
            bool negativeMotion = false, bool sweepAngle = true, bool pieAngle = true,
            params IParameter[] parameters)
        {
            this = new Pie(parameters, x, y, w, h, startAngle, endAngle,
                negativeMotion, sweepAngle, pieAngle);
        }

        /// <summary>
        /// Creates a new ellipse specified by the bounding area.
        /// </summary>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        /// <param name="x">X cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="w">Width of a bounding area where the pie is to be drawn -> 
        /// circle/ellipse's minor X axis = Width/2</param>
        /// <param name="h">Height of a bounding area where the pie is to be drawn ->
        /// circle/ellipse's minor Y axis = Height/2</param>
        public Pie(IEnumerable<IParameter> parameters, float x, float y, float w, float h,
            float startAngle, float endAngle, bool negativeMotion = false, bool sweepAngle = true, bool pieAngle = true)
        {
            var bounds = new RectangleF(x, y, w, h);

            parameters.ExtractRotationScaleParameters(out IRotation rotation, out IScale scale);
            bounds = bounds.Scale(rotation, scale);
            bounds.GetBounds(out x, out y, out w, out h);

            Rx = w / 2f;
            Ry = h / 2f;
            Cx = x + Rx;
            Cy = y + Ry;
            StartAngle = startAngle;
            EndAngle = endAngle;

            if (rotation != null && rotation.Valid && rotation.Centre == null)
            {
                var oc = rotation.Centre;
                rotation.SetCentre(Cx, Cy);
                Points = Curves.GetArcPoints(StartAngle, EndAngle, true,
                    Cx, Cy, Rx, Ry, rotation, negativeMotion, sweepAngle, pieAngle);
                rotation.SetCentre( oc?.Cx, oc?.Cy);
            }
            else
                Points = Curves.GetArcPoints(StartAngle, EndAngle, true,
                    Cx, Cy, Rx, Ry, rotation, negativeMotion, sweepAngle, pieAngle);

            Points = Points.Where(pt => pt).ToArray();
        }

        public Pie(IBounds bounds, float startAngle, float endAngle,
            bool negativeMotion = false, bool sweepAngle = true, bool pieAngle = true)
        {
            bounds.GetBounds(out int x, out int y, out int w, out int h);
            this = new Pie(x, y, w, h, startAngle, endAngle, negativeMotion, sweepAngle, pieAngle);
        }
        public Pie(IBoundsF bounds, float startAngle, float endAngle,
            bool negativeMotion = false, bool sweepAngle = true, bool pieAngle = true)
        {
            bounds.GetBounds(out float x, out float y, out float w, out float h);
            this = new Pie(x, y, w, h, startAngle, endAngle, negativeMotion, sweepAngle, pieAngle);
        }

        Pie(IPie pie, int w, int h)
        {
            Rx = w / 2f;
            Ry = h / 2f;
            Cx = pie.X + Rx;
            Cy = pie.Y + Ry;
            StartAngle = pie.StartAngle;
            EndAngle = pie.EndAngle;
            Points = pie.GetPoints().Scale(new Scale(pie, w, h)).ToArray();
        }

        Pie(IPie pie)
        {
            Rx = pie.Rx;
            Ry = pie.Ry;
            Cx = Rx;
            Cy = Ry;
            StartAngle = pie.StartAngle;
            EndAngle = pie.EndAngle;
            var subtract = new Point(pie.X, pie.Y);
            Points = pie.GetPoints().Select(pt => pt - subtract).ToArray();
        }
        #endregion

        #region PROPERTIES
        float IArc.Rx => Rx;
        float IArc.Ry => Ry;
        float IArc.StartAngle => StartAngle;
        float IArc.EndAngle => EndAngle;
        public bool Valid => Rx > 0 && Ry > 0 && (StartAngle != 0 || EndAngle != 0);
        public string TypeName => "Pie";
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
            return new Pie(this, w, h);
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            return new Pie(this);
        }
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, "Pie", Cx - Rx, Cy - Ry, Rx * 2, Ry * 2, StartAngle, EndAngle);
        }
    }
    #endregion
}
#endif
