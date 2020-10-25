/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    public static partial class Curves
    {
        #region GET ARC POINTS
        public static VectorF[] GetArcPoints(float startAngle, float endAngle, bool addCenter, float Cx, float Cy, float Rx, float Ry,
            Rotation angle = default(Rotation), bool negativeMotion = false)
        {
            bool WMajor = Rx > Ry;
            if (startAngle < 0)
                startAngle += 360;
            if (endAngle < 0)
                endAngle += 360;

            negativeMotion = negativeMotion || startAngle > endAngle;
            Numbers.Order(ref startAngle, ref endAngle);

            var first = GetEllipsePoint(startAngle, Cx, Cy, Rx, Ry, angle, true);
            var last = GetEllipsePoint(endAngle, Cx, Cy, Rx, Ry, angle, true);
            return GetArcPoints(first, last, addCenter, WMajor, Cx, Cy, Rx, Ry, angle, negativeMotion);
        }

        public static VectorF[] GetArcPoints(VectorF first, VectorF last, bool addCenter, bool WMajor, float Cx, float Cy, float Rx, float Ry,
            Rotation angle = default(Rotation), bool negativeMotion = false)
        {
            VectorF center = (angle) ?
                angle.Rotate(Cx, Cy) : new VectorF(Cx, Cy);

            var points = new VectorF[362];
            if (addCenter)
                points[0] = center;

            var line = Factory.newLine(first, last);
            Func<float, float, bool> condition;

            if (negativeMotion)
                condition = (m, n) => line.IsLessThan(m, n);
            else
                condition = (m, n) => line.IsGreaterThan(m, n);

            float x, y;
            int step;

            GetEllipsePoint(0, Cx, Cy, Rx, Ry, out x, out y, angle, false);
            if (condition(x, y))
                points[1] = (new VectorF(x, y));

            GetEllipsePoint(90, Cx, Cy, Rx, Ry, out x, out y, angle, false);
            if (condition(x, y))
                points[90] = (new VectorF(x, y));

            GetEllipsePoint(180, Cx, Cy, Rx, Ry, out x, out y, angle, false);
            if (condition(x, y))
                points[180] = (new VectorF(x, y));

            GetEllipsePoint(270, Cx, Cy, Rx, Ry, out x, out y, angle, false);
            if (condition(x, y))
                points[270] = (new VectorF(x, y));

            for (int i = 1; i <= 360; i += step)
            {
                step = GetCurveStep(WMajor, i, Rx, Ry);
                GetEllipsePoint(i, Cx, Cy, Rx, Ry, out x, out y, angle, false);
                if (condition(x, y))
                    points[i] = (new VectorF(x, y));
            }

            return points;
        }
        #endregion

        #region ROTATED ELLIPSE
        /// <summary>
        /// Very fast ellipse drawing function by Michael "h4tt3n" Nissen version 4.0 March 2010
        /// </summary>
        public static void NissenEllipse(int x, int y, int width, int height, Rotation angle, PixelAction<float> action, LineCommand lineCommand = 0)
        {
            //These constants decide the graphic quality of the ellipse
            const int face_length = 6;  //approx.face length in pixels
            const int max_faces = 512;  //maximum number of faces in ellipse
            const int min_faces = 32;   //minimum number of faces in ellipse
            float a = width / 2f;
            float b = height / 2f;
            float h = ((a - b) * (a - b)) / ((a + b) * (a + b));
            float circumference = 0.25f * Angles.PI * (a + b) * (3 * (1 + h * 0.25f) + 1 / (1 - h * 0.25f));
            float num_faces = circumference / face_length;

            if (num_faces > max_faces) num_faces = max_faces;
            if (num_faces < min_faces) num_faces = min_faces;
            num_faces -= num_faces % 4;

            angle.SinCos(out float sin, out float cos);

            float c = (float)Math.Cos(2 * Angles.PI / num_faces);
            float s = (float)Math.Sin(2 * Angles.PI / num_faces);
            float x1 = 1f;
            float y1 = 0f;
            float x2, y2;
            float lx1, lx2, ly1, ly2;

            for (int i = 1; i < num_faces - 1; i++)
            {
                x2 = x1;
                y2 = y1;
                x1 = c * x2 - s * y2;
                y1 = s * x2 + c * y2;

                lx1 = x + a * x2 * cos - b * y2 * sin;
                ly1 = y + a * x2 * sin + b * y2 * cos;
                lx2 = x + a * x1 * cos - b * y1 * sin;
                ly2 = y + a * x1 * sin + b * y1 * cos;

                Renderer.ProcessLine(lx1, ly1, lx2, ly2, action, lineCommand);
            }
            lx1 = x + a * x1 * cos - b * y1 * sin;
            ly1 = y + a * x1 * sin + b * y1 * cos;
            lx2 = x + a * cos;
            ly2 = y + a * sin;
            Renderer.ProcessLine(lx1, ly1, lx2, ly2, action, lineCommand);
        }
        #endregion

        #region GWS FAST ELLIPSE
        public static void GwsEllipse(int x, int y, int width, int height, Rotation angle, PixelAction<float> action, LineCommand lineCommand = 0)
        {
            float x1 = 1f, y1 = 0f, x2 = 0, y2 = 0, lbx = 0, lby = 0, rbx = 0,
                rby = 0, ltx = 0, lty = 0, rtx = 0, rty = 0, lbx1 = 0, lby1 = 0,
                rbx1 = 0, rby1 = 0, ltx1 = 0, lty1 = 0, rtx1 = 0, rty1 = 0;

            var total = 45;
            angle.SinCos(out float sin, out float cos);
            var a = width / 2f;
            var b = height / 2f;

            var c = (float)Math.Cos(Angles.PI / (total * 2f));
            var s = (float)Math.Sin(Angles.PI / (total * 2f));
            var rxcos = a * cos;
            var rxsin = a * sin;
            var rycos = b * cos;
            var rysin = b * sin;
            var cx = x + a;
            var cy = y + b;

            for (float i = 0; i < 45; i++)
            {
                x2 = x1;
                y2 = y1;
                x1 = c * x2 - s * y2;
                y1 = s * x2 + c * y2;

                GetRotatedQuadrants(x1, y1, cx, cy, rxcos, rxsin, rycos, rysin, out ltx1, out lty1, out rtx1, out rty1, out lbx1, out lby1, out rbx1, out rby1);
                GetRotatedQuadrants(x2, y2, cx, cy, rxcos, rxsin, rycos, rysin, out ltx, out lty, out rtx, out rty, out lbx, out lby, out rbx, out rby);
                Renderer.ProcessLine(lbx1, lby1, lbx, lby, action, lineCommand);
                Renderer.ProcessLine(rtx1, rty1, rtx, rty, action, lineCommand);
                Renderer.ProcessLine(rbx1, rby1, rbx, rby, action, lineCommand);
                Renderer.ProcessLine(ltx1, lty1, ltx, lty, action, lineCommand);

            }
        }
        static void GetRotatedQuadrants(float x, float y, float CX, float CY, float rxcos, float rxsin, float rycos, float rysin, out float ltx, out float lty,
            out float rtx, out float rty, out float lbx, out float lby, out float rbx, out float rby)
        {
            var xrysin = x * rysin;
            var xrycos = x * rycos;
            var xrxsin = x * rxsin;
            var xrxcos = x * rxcos;

            var yrycos = y * rycos;
            var yrysin = y * rysin;
            var yrxcos = y * rxcos;
            var yrxsin = y * rxsin;

            lbx = CX + (xrxcos - yrysin);
            rtx = CX - (xrxcos - yrysin);
            rbx = CX + (yrxcos + xrysin);
            ltx = CX - (xrysin + yrxcos);

            lby = CY + (xrxsin + yrycos);
            rty = CY - (xrxsin + yrycos);
            rby = CY + (yrxsin - xrycos);
            lty = CY + (xrycos - yrxsin);
        }
        #endregion

        #region GET STROKED CURVE
        /// <summary>
        /// Creates a stroked curve by reading data from reference curve and applying specified stroke and fill mode.
        /// </summary>
        /// <param name="curve">Reference curve to use for creating stroke curve.</param>
        /// <param name="stroke">Stroke value to be used to obtain stroke curve.</param>
        /// <param name="fill">Fill mode to be used to obtain stroked curve.</param>
        /// <param name="mode">Stroke mode to be used to obtain stroked. curve</param>
        /// <param name="assignID">If true assign an unique id to the object</param>
        /// <returns>ICurve</returns>
        public static ICurve StrokedCurve(this ICurve curve, float stroke, FillMode fill, StrokeMode mode)
        {

            if ((fill == FillMode.Outer && mode == StrokeMode.StrokeInner) ||
                (fill == FillMode.Inner && mode == StrokeMode.StrokeOuter))
            {
                // requested stroked curve is the reference curve itself.
                return curve;
            }

            ICurve outerCurve, innerCurve = null;

            float start = curve.Full ? 0 : curve.StartAngle;
            float end = curve.Full ? 0 : curve.EndAngle;

            //get stroked areas.
            curve.Bounds.GetStrokeAreas(out RectangleF outer, out RectangleF inner, stroke, mode);
            if (stroke < 0)
                Numbers.Swap(ref outer, ref inner);

            //need get get stroked centers if curve is not full.
            curve.GetStrokedCenters(stroke, mode, out VectorF? mainCenter, out VectorF? childCenter);

            if (fill != FillMode.Outer)
            {
                //if inner bounds is not equal to curve bounds get inner curve.
                if (inner != curve.Bounds)
                {
                    IConic innerConic = Factory.newConic(curve.Rotation, inner.X, inner.Y, inner.Width, inner.Height);
                    VectorF[] pieTriangle = curve.Full ? null : innerConic.GetPieTriangle(curve.Type, start, end, childCenter);
                    innerCurve = Factory.newCurve(innerConic, pieTriangle, curve.Type);

                }
                else
                    innerCurve = curve;
                if (fill == FillMode.Inner)
                    return innerCurve;
            }
            //if outer bounds is not equal to curve bounds get outer curve.
            if (outer != curve.Bounds)
            {
                var outerConic = Factory.newConic(curve.Rotation, outer.X, outer.Y, outer.Width, outer.Height);
                VectorF[] pieTriangle = curve.Full ? null : outerConic.GetPieTriangle(curve.Type, start, end, mainCenter);
                outerCurve = Factory.newCurve(outerConic, pieTriangle, curve.Type);
            }
            else
                outerCurve = curve;

            if (fill == FillMode.Outer)
                return outerCurve;

            //if you have reached here means we have 2 curves and we need to attach child curve to main one.
            outerCurve.AttachedCurve = innerCurve;
            return outerCurve;
        }
        #endregion

        #region GET STROKED CENTERS
        public static void GetStrokedCenters(this ICurve curve, float stroke, StrokeMode mode, out VectorF? mainCenter, out VectorF? childCenter)
        {
            mainCenter = childCenter = null;

            if (curve.Full)
                return;

            if (!curve.Full)
            {
                var triangle = curve.PieTriangle;
                if (curve.Type.HasFlag(CurveType.Arc) && !curve.Type.HasFlag(CurveType.ClosedArc))
                {
                    childCenter = mainCenter = triangle[0];
                    return;
                }

                Renderer.StrokePoints(triangle, "Triangle", stroke, mode, out IList<VectorF> main, out IList<VectorF> child);
                var clockWise = main.ClockWise();

                if (clockWise != curve.Type.HasFlag(CurveType.AntiClock) ||
                    curve.EndAngle.Round() == 180 ||
                    curve.StartAngle.Round() == 180)
                    Numbers.Swap(ref main, ref child);

                if (curve.Type.HasFlag(CurveType.CrossStroke))
                    Numbers.Swap(ref main, ref child);
                childCenter = child[0];
                mainCenter = main[0];
            }
        }
        #endregion
    }
}
