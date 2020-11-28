/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if (GWS || Window)
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    public static class Curves
    {
        #region VARIABLES
        static float[] Lookup;
        static float dfConicWidth;
        static float dfConicHeight;
        const float RootOf2 = 1.41421356237f;
        const float Half = .5f;        
        const float HalfMultRootOf2 = 0.707106769f; // Half * RootOf2
        const float HalfMinusHalfMultRootOf2 = -0.207106769f;//Half- Half * RootOf2
        #endregion

        #region CONSTRUCTOR
        static Curves()
        {
            Lookup = new float[]
{
            1.0f,
            1.0f,
            2.0f,
            6.0f,
            24.0f,
            120.0f,
            720.0f,
            5040.0f,
            40320.0f,
            362880.0f,
            3628800.0f,
            39916800.0f,
            479001600.0f,
            6227020800.0f,
            87178291200.0f,
            1307674368000.0f,
            20922789888000.0f,
            355687428096000.0f,
            6402373705728000.0f,
            121645100408832000.0f,
            2432902008176640000.0f,
            51090942171709440000.0f,
            1124000727777607680000.0f,
            25852016738884976640000.0f,
            620448401733239439360000.0f,
            15511210043330985984000000.0f,
            403291461126605635584000000.0f,
            10888869450418352160768000000.0f,
            304888344611713860501504000000.0f,
            8841761993739701954543616000000.0f,
            265252859812191058636308480000000.0f,
            8222838654177922817725562880000000.0f,
            263130836933693530167218012160000000.0f
};

        }
        internal static void Initialize() { }
        #endregion

        #region PROPERTIES
        public static float DefaultConicWidth
        {
            get
            {
                if (dfConicWidth <= 0)
                    return 300f;
                return dfConicWidth;
            }
            set => dfConicWidth = value;
        }
        public static float DefaultConicHeight
        {
            get
            {
                if (dfConicHeight <= 0)
                    return 300f;
                return dfConicHeight;
            }
            set => dfConicHeight = value;
        }
        #endregion

        #region CIRCLE FROM A POINT ON IT AND THE CENTER
        public static void GetCircleData(VectorF pointOnCircle, VectorF centerOfCircle, out float x, out float y, out float width)
        {
            //Equation of circle : (x-h)^2 + (y-k)^2 = r^2;
            var xMinusHSqr = Numbers.Sqr(pointOnCircle.X - centerOfCircle.X);
            var yMinusKSqr = Numbers.Sqr(pointOnCircle.Y - centerOfCircle.Y);

            var radius = (float)Math.Sqrt(xMinusHSqr + yMinusKSqr);
            x = centerOfCircle.X - radius;
            y = centerOfCircle.Y - radius;
            width = radius * 2f;
        }
        #endregion

        #region GET ELLIPSE POINT/S
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetEllipsePoint(float angle, float CX, float CY, float Rx, float Ry, 
            out float x, out float y, Rotation rotation, bool pieAngle)
        {
            Angles.SinCos(angle, out float sin, out float cos);
            float cosrx, sinry, cs, RxRy = Rx * Ry;

            if (Rx == Ry || !pieAngle)
            {
                cosrx = Rx * cos;
                sinry = Ry * sin;
            }
            else
            {
                cs = (float)Math.Sqrt(Numbers.Sqr(Ry * cos) + Numbers.Sqr(Rx * sin));
                cosrx = (RxRy * cos) / cs;
                sinry = (RxRy * sin) / cs;
            }
            x = CX + cosrx;
            y = CY + sinry;

            if (rotation)
                rotation.Rotate(x, y, out x, out y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="Center"></param>
        /// <returns></returns>
        public static VectorF GetEllipsePoint(float angle, VectorF p1, VectorF p2, VectorF Center)
        {
            Angles.SinCos(angle, out float sin, out float cos);
            float x, y;
            p1.Assign(p1.X - Center.X, p1.Y - Center.Y);
            p2.Assign(p2.X - Center.X, p2.Y - Center.Y);
            x = p1.X * sin + p2.X * cos;
            y = p1.Y * sin + p2.Y * cos;
            x += Center.X;
            y += Center.Y;
            return new VectorF(x, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static VectorF GetEllipsePoint(float angle, float CX, float CY, float RadiusX, float RadiusY, Rotation rotation, bool pieAngle)
        {
            GetEllipsePoint(angle, CX, CY, RadiusX, RadiusY, out float x, out float y, rotation, pieAngle);
            return new VectorF(x, y);
        }

        public static VectorF[] GetEllipsePoints( float Cx, float Cy, float Rx, float Ry, bool pieAngle = false, Rotation angle = default(Rotation))
        {
            bool WMajor = Rx > Ry;
            var points = new VectorF[361];
            float x, y;

            GetEllipsePoint(0, Cx, Cy, Rx, Ry, out x, out y, angle, pieAngle);
            points[0] = new VectorF(x, y);

            GetEllipsePoint(90, Cx, Cy, Rx, Ry, out x, out y, angle, pieAngle);
            points[90] = new VectorF(x, y);

            GetEllipsePoint(180, Cx, Cy, Rx, Ry, out x, out y, angle, pieAngle);
            points[180] = new VectorF(x, y);

            int step;
            for (int i = 1; i < 360; i += step)
            {
                step = GetCurveStep(WMajor, i, Rx, Ry);
                GetEllipsePoint(i, Cx, Cy, Rx, Ry, out x, out y, angle, pieAngle);
                points[i] = new VectorF(x, y);
            }
            return points;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetCirclePoint(float angle, float CX, float RadiusX, out float x, out float y)
        {
            Angles.SinCos(angle, out float sin, out float cos);
            float cosrx, sinry;

            cosrx = RadiusX * cos;
            sinry = RadiusX * sin;
            x = CX + cosrx;
            y = CX + sinry;
        }
        public static void GetCircleArc(float x, float y, float w, VectorAction<float> action, int startAngle, int endAngle, int step = 1)
        {
            var radius = w / 2f;
            var cx = x + radius;
            float px, py;
            if (step < 1)
                step = 1;
            Numbers.Order(ref startAngle, ref endAngle);
            for (int i = startAngle; i <= endAngle; i += step)
            {
                GetCirclePoint(i, cx, radius, out px, out py);
                action(px, py);
            }
        }

        public static VectorF GetEllipsePoint(this IConic e, float angle, bool pieAngle) =>
            GetEllipsePoint(angle, e.Cx, e.Cy, e.Rx, e.Ry, e.Rotation, pieAngle);

        public static VectorF GetEllipsePoint(this IConic e, float angle, bool pieAngle, Rotation userAngle) =>
            GetEllipsePoint(angle, e.Cx, e.Cy, e.Rx, e.Ry, userAngle, pieAngle);
        #endregion

        #region GET CUBIC BEZIER 4 POINTS
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<VectorF> GetBezier4Points(float x, float y, float rx, float ry, float startAngle, float endAngle)
        {
            /* center */
            var cx = x + rx;
            var cy = y + ry;

            startAngle *= Angles.Radian;
            endAngle *= Angles.Radian;

            var ssin = (float)Math.Sin(startAngle);
            var scos = (float)Math.Cos(startAngle);
            var esin = (float)Math.Sin(endAngle);
            var ecos = (float)Math.Cos(endAngle);

            /* adjust angles for ellipses */
            var alpha = (float)Math.Atan2(rx * ssin, ry * scos);
            var beta = (float)Math.Atan2(rx * esin, ry * ecos);


            if (Math.Abs(beta - alpha) > Angles.PI)
            {
                if (beta > alpha)
                    beta -= 2 * Angles.PI;
                else
                    alpha -= 2 * Angles.PI;
            }

            var delta = (beta - alpha) / 2f;

            var bcp = (float)(4f / 3 * (1 - Math.Cos(delta)) / Math.Sin(delta));


            /* starting point */
            float x1 = cx + rx * scos;
            float y1 = cy + ry * ssin;

            var x2 = cx + rx * (scos - bcp * ssin);
            var y2 = cy + ry * (ssin + bcp * scos);

            var x3 = cx + rx * (ecos + bcp * esin);
            var y3 = cy + ry * (esin - bcp * ecos);

            var x4 = cx + rx * ecos;
            var y4 = cy + ry * esin;

            return Enumerables.ToIEnumerable(x1, y1, x2, y2, x3, y3, x4, y4).ToPoints();
        }
        #endregion

        #region INTERCEPTION
        public static bool Interception(VectorF p1, VectorF p2, float x, float y, float width, float height,
            out VectorF iPt1, out VectorF iPt2, Rotation angle = default(Rotation), bool ignoreAngle = false)
        {
            var Rx = width / 2f;
            var Ry = height / 2f;
            return Interception(Rx, Ry, x + Rx, y + Rx, p1, p2, out iPt1, out iPt2, angle, ignoreAngle);
        }

        /// <summary>
        /// Source Credit:http://csharphelper.com/blog/2017/08/calculate-where-a-line-segment-and-an-ellipse-intersect-in-c/
        /// </summary>
        /// <param name="Rx"></param>
        /// <param name="Ry"></param>
        /// <param name="Cx"></param>
        /// <param name="Cy"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="iPt1"></param>
        /// <param name="iPt2"></param>
        /// <param name="angle"></param>
        /// <param name="ignoreAngle"></param>
        /// <returns></returns>
        public static bool Interception(float Rx, float Ry, float Cx, float Cy, VectorF p1, VectorF p2, out VectorF iPt1, out VectorF iPt2,
            Rotation angle = default(Rotation), bool ignoreAngle = false)
        {
            iPt1 = iPt2 = VectorF.Empty;
            // If the ellipse or line segment are empty, return no intersections.
            if ((Rx == 0) || (Ry == 0) ||
                ((p1.X == p2.X) && (p1.Y == p2.Y)))
                return false;

            var x1 = p1.X;
            var x2 = p2.X;
            var y1 = p1.Y;
            var y2 = p2.Y;

            if (!ignoreAngle && angle)
            {
                //var type = angle.AntiClock ? 
                //    RotateType.Clockwise : RotateType.AntiClockwise;

                angle.Rotate(x1, y1, out x1, out y1, antiClock: true);
                angle.Rotate(x2, y2, out x2, out y2, antiClock: true);
            }

            // Translate so the ellipse is centered at the origin.
            x1 -= Cx;
            y1 -= Cy;
            x2 -= Cx;
            y2 -= Cy;

            var dx = x2 - x1;
            var dy = y2 - y1;
            var dxSqr = Numbers.Sqr(dx);
            var dySqr = Numbers.Sqr(dy);
            var rxSqr = Numbers.Sqr(Rx);
            var rySqr = Numbers.Sqr(Ry);

            // Calculate the quadratic parameters.
            float A = dxSqr / rxSqr +
                      dySqr / rySqr;
            float B = 2 * x1 * dx / rxSqr + 2 * y1 * dy / rySqr;
            float C = x1 * x1 / rxSqr + y1 * y1 / rySqr - 1f;

            // Calculate the discriminant.
            float discriminant = B * B - 4 * A * C;
            float t1 = -1, t2 = -1;

            bool intercepts = false;
            if (discriminant == 0)
            {
                // One real solution.
                t1 = (-B / 2 / A);
            }
            else if (discriminant > 0)
            {
                // Two real solutions.
                t1 = ((float)((-B + Math.Sqrt(discriminant)) / 2 / A));
                t2 = ((float)((-B - Math.Sqrt(discriminant)) / 2 / A));
            }
            if (t1 >= 0)
            {
                float ix1 = p1.X + (x2 - x1) * t1 + Cx;
                float iy1 = p1.Y + (y2 - y1) * t1 + Cy;
                iPt1 = new VectorF(ix1, iy1);

                intercepts = true;
            }
            if (t2 >= 0f)
            {
                float ix1 = x1 + (x2 - x1) * t2 + Cx;
                float iy1 = y1 + (y2 - y1) * t2 + Cy;
                iPt2 = new VectorF(ix1, iy1);

                intercepts = true;
            }
            if (!ignoreAngle && intercepts && angle != null && angle)
            {
                iPt1 = angle.Rotate(iPt1);
                iPt2 = angle.Rotate(iPt2);
            }
            return intercepts;
        }

        public static bool Interception(this IConic conic, VectorF p1, VectorF p2, out VectorF iPt1, out VectorF iPt2, bool ignoreAngle = false) =>
            Interception(conic.Rx, conic.Ry, conic.Cx, conic.Cy, p1, p2, out iPt1, out iPt2, conic.Rotation, ignoreAngle);
        #endregion

        #region CHECK CURVE STEP
        static int GetCurveStep(bool WMajor, float angle, float Rx, float Ry)
        {
            if (angle > 360)
                angle %= 360;

            if (WMajor)
            {
                if (
                    angle.IsWithIn(0, 23) ||
                    angle.IsWithIn(359, 359 - 23) ||
                    angle.IsWithIn(180 - 23, 180 + 23))
                {
                    if (Rx < 20)
                        return 1;
                    else if (Rx > 200)
                        return 3;
                    return 4;
                }
            }
            else
            {
                if (
                    angle.IsWithIn(90 - 23, 90 + 23) ||
                    angle.IsWithIn(270 - 23, 270 + 23))
                {
                    if (Ry < 20)
                        return 1;
                    else if (Ry > 200)
                        return 3;
                    return 4;
                }
            }
            return Rx > 200 || Ry > 200 ? 3 : 6;
        }
        #endregion

        #region GET BEZIER POINTS
        /// <summary>
        /// Source : https://en.wikipedia.org/wiki/Bernstein_polynomial
        /// </summary>
        /// <param name="n"></param>
        /// <param name="i"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        static float Bernstein(int n, int i, double t)
        {
            float basis;
            float ti; /* t^i */
            float tni; /* (1 - t)^i */

            if (t == 0.0 && i == 0)
                ti = 1.0f;
            else
                ti = (float)Math.Pow(t, i);

            if (n == i && t == 1.0f)
                tni = 1.0f;
            else
                tni = (float)Math.Pow((1 - t), (n - i));

            //Bernstein basis
            n = Math.Min(Math.Max(n, 0), 32);
            i = Math.Min(Math.Max(i, 0), 32);
            basis = (Lookup[n] / (Lookup[i] * Lookup[n - i])) * ti * tni;
            return basis;
        }

        /// <summary>
        /// Source: https://en.wikipedia.org/wiki/Bernstein_polynomial
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="multiplier"></param>
        /// <param name="dataPoints"></param>
        /// <param name="result"></param>
        /// <param name="removeLast"></param>
        static unsafe void GetBezierPoints<T>(int multiplier, IList<float> dataPoints, ref T result) where T : ICollection<VectorF>
        {
            multiplier = Math.Min(multiplier, 8);

            var cpts = dataPoints.Count * multiplier;
            int npts = (dataPoints.Count) / 2;
            int icount, jcount;
            double step, t;
            var _pts = new float[cpts * 2];

            icount = 0;
            t = 0;
            step = 1.0 / (cpts - 1);
            fixed (float* p = _pts)
            {
                for (var i = 0; i != cpts; i++)
                {
                    if ((1.0 - t) < 5e-6)
                        t = 1.0;

                    jcount = 0;
                    p[icount] = 0.0f;
                    p[icount + 1] = 0.0f;
                    for (int j = 0; j != npts; j++)
                    {
                        var basis = Bernstein(npts - 1, j, t);
                        var x = basis * dataPoints[jcount];
                        var y = basis * dataPoints[jcount + 1];

                        p[icount] += x;
                        p[icount + 1] += y;

                        jcount = jcount + 2;
                    }

                    icount += 2;
                    t += step;
                }
            }
            var pts = _pts.ToPoints();
            pts.RemoveAt(pts.Count - 1);
            if (result is Collection<VectorF>)
                (result as Collection<VectorF>).AddRange(pts);
            else
            {
                foreach (var item in pts)
                    result.Add(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void GetBezierPoints<T>(int multiplier, ref T points, ICollection<VectorF> source)
            where T : ICollection<VectorF>
        {
            float[] dataPoints = new float[source.Count * 2];
            int g = 0;

            fixed(float* data = dataPoints)
            {
                foreach (var item in source)
                {
                    data[g++] = item.X;
                    data[g++] = item.Y;
                }
            }            
            GetBezierPoints(multiplier, dataPoints, ref points);
        }
       
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBezierPoints<T>(int multiplier, ref T points, params VectorF[] source) 
            where T : ICollection<VectorF>
        {
            GetBezierPoints(multiplier, ref points, source as ICollection<VectorF>);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBezierPoints<T>(int multiplier, BezierType type, ref T points, IList<VectorF> Source, bool pickFromLastPoint = true) 
            where T : IList<VectorF> 
        {
            multiplier = Math.Min(multiplier, 8);

            if (pickFromLastPoint)
            {
                VectorF p;
                switch (type)
                {
                    case BezierType.Quadratric:
                        if (Source.Count < 3)
                            goto default;
                        GetBezierPoints(multiplier, ref points, Source[0], Source[1], Source[2]);
                        p = points[points.Count - 1];
                        for (int k = 3; k < Source.Count; k++)
                        {
                            if (k % 2 == 0)
                            {
                                GetBezierPoints(multiplier, ref points, p, Source[k - 1], Source[k]);
                                p = points[points.Count - 1];
                            }
                        }
                        break;
                    case BezierType.Multiple:
                        if (Source.Count < 4)
                            goto default;
                        GetBezierPoints(multiplier, ref points, Source[0], Source[1], Source[2], Source[3]);
                        p = points[points.Count - 1];
                        for (int k = 4; k < Source.Count; k++)
                        {
                            if (k % 3 == 0)
                            {
                                GetBezierPoints(multiplier, ref points, p, Source[k - 2], Source[k - 1], Source[k]);
                                p = points[points.Count - 1];
                            }
                        }
                        break;
                    default:
                        GetBezierPoints(multiplier, ref points, Source);
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case BezierType.Quadratric:
                        if (Source.Count < 3)
                            goto default;

                        GetBezierPoints(multiplier, ref points, Source[0], Source[1], Source[2]);
                        for (int k = 3; k < Source.Count; k++)
                        {
                            if (k % 3 == 2)
                            {
                                GetBezierPoints(multiplier, ref points, Source[k - 2], Source[k - 1], Source[k]);
                            }
                        }
                        break;
                    case BezierType.Multiple:
                        if (Source.Count < 4)
                            goto default;
                        GetBezierPoints(multiplier, ref points, Source[0], Source[1], Source[2], Source[3]);
                        for (int k = 4; k < Source.Count; k++)
                        {
                            if (k % 4 == 3)
                            {
                                GetBezierPoints(multiplier, ref points, Source[k - 3], Source[k - 2], Source[k - 1], Source[k]);
                            }
                        }
                        break;
                    default:
                        GetBezierPoints(multiplier, ref points, Source);
                        break;
                }

            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Collection<VectorF> GetBezierPoints(int multiplier, BezierType type, IList<VectorF> Source, bool pickFromLastPoint = true)
        {
            multiplier = Math.Min(multiplier, 8);
            var points = new Collection<VectorF>(Source.Count * multiplier);
            GetBezierPoints(multiplier, type, ref points, Source, pickFromLastPoint);
            points.RemoveLast();
            return points;
        }
        #endregion

        #region ORDER 3 POINTS
        /// <summary>
        /// Orders 3 points in a way so that first 2 points i.e p1 and p2 becomes the points with longest or shortest  distance(depending on 'p1p2AsMajor' parameter) among all 3.
        /// </summary>
        /// <param name="p1">1st point.</param>
        /// <param name="p2">2nd point.</param>
        /// <param name="p3">3rd point.</param>
        /// <param name="p1p2AsMajor">If true p1 and p2 becomes points which has the longest distance among all 3 other wise shortest.</param>
        public static void Order3Points(ref VectorF p1, ref VectorF p2, ref VectorF p3, bool p1p2AsMajor = false)
        {
            var p1p2 = p1.DistanceSquared(p2);
            var p1p3 = p1.DistanceSquared(p3);
            var p2p3 = p2.DistanceSquared(p3);

            if (p1p2AsMajor)
            {
                if (p1p2 < p1p3 || p1p2 < p2p3)
                {
                    if (p1p3 > p2p3)
                        Numbers.Swap(ref p2, ref p3);
                    else
                        Numbers.Swap(ref p1, ref p3);
                }
            }
            else
            {
                if (p1p2 > p1p3 || p1p2 > p2p3)
                {
                    if (p1p3 < p2p3)
                        Numbers.Swap(ref p2, ref p3);
                    else
                        Numbers.Swap(ref p1, ref p3);
                }
            }
        }

        /// <summary>
        /// <summary>
        /// Orders 3 points in a way so that first 2 points i.e p1 and p2 becomes the points with longest or shortest  distance(depending on 'p1p2AsMajor' parameter) among all 3.
        /// </summary>
        /// <param name="p1">1st point.</param>
        /// <param name="p2">2nd point.</param>
        /// <param name="p3">3rd point.</param>
        /// <param name="p1p2AsMajor">If true p1 and p2 becomes points which has the longest distance among all 3 other wise shortest.</param>
        /// <returns>Array of points</returns>
        public static VectorF[] Order3Points(VectorF p1, VectorF p2, VectorF p3, bool p1p2AsMajor = false)
        {
            Order3Points(ref p1, ref p2, ref p3, p1p2AsMajor);
            return new VectorF[] { p1, p2, p3 };
        }
        #endregion

        #region MISSING 2 POINTS OF ELLIPSE
        public static void Missing2PointsOfEllipse(ref VectorF p1, ref VectorF p2, ref VectorF p3, out VectorF p4, out VectorF p5, bool p1p2AsMajor = false)
        {
            Order3Points(ref p1, ref p2, ref p3, p1p2AsMajor);

            p4 = new VectorF(p1.X * Half + p2.X * HalfMultRootOf2 + p3.X * HalfMinusHalfMultRootOf2,
                p1.Y * Half + p2.Y * HalfMultRootOf2 + p3.Y * HalfMinusHalfMultRootOf2);

            p5 = new VectorF(p3.X * Half + p2.X * HalfMultRootOf2 + p1.X * HalfMinusHalfMultRootOf2,
                p3.Y * Half + p2.Y * HalfMultRootOf2 + p1.Y * HalfMinusHalfMultRootOf2);

            //p4 = (p1 * Half) + (p2 * 0.5f * RootOf2) + (p3 * (0.5f - (0.5f * RootOf2)));
            //p5 = (p3 * 0.5f) + (p2 * 0.5f * RootOf2) + (p1 * (0.5f - (0.5f * RootOf2)));
        }
        public static void Missing2PointsOfEllipse(VectorF p1, VectorF p2, VectorF p3, out VectorF p4, out VectorF p5)
        {
            p4 = new VectorF(p1.X * Half + p2.X * HalfMultRootOf2 + p3.X * HalfMinusHalfMultRootOf2,
                p1.Y * Half + p2.Y * HalfMultRootOf2 + p3.Y * HalfMinusHalfMultRootOf2);

            p5 = new VectorF(p3.X * Half + p2.X * HalfMultRootOf2 + p1.X * HalfMinusHalfMultRootOf2,
                p3.Y * Half + p2.Y * HalfMultRootOf2 + p1.Y * HalfMinusHalfMultRootOf2);

            //p4 = (p1 * 0.5f) + (p2 * 0.5f * RootOf2) + (p3 * (0.5f - (0.5f * RootOf2)));
            //p5 = (p3 * 0.5f) + (p2 * 0.5f * RootOf2) + (p1 * (0.5f - (0.5f * RootOf2)));
        }
        #endregion

        #region GET 5 ELLIPSE CREATION POINTS
        /// <summary>
        /// Creates a conic section which results in an ellipse from the given points with angle of rotation if supplied.
        /// Providing three points will always result in this conic being ellipse.
        /// Because other two points will be calculated in a way that it will result in a valid ellipse rather than parabola or hyperbola.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        public static void GetEllipseMakingPoints(ref VectorF p1, ref VectorF p2, ref VectorF p3, ref VectorF p4, CurveType type, out VectorF p5)
        {
            if (type.HasFlag(CurveType.Fitting))
            {
                var a = (p1 + p2) / 2;
                var b = (p2 + p3) / 2;
                var c = (p3 + p4) / 2;
                var d = (p1 + p4) / 2;
                p1 = a;
                p2 = b;
                p3 = c;
                p4 = d;
                Missing2PointsOfEllipse(p1, p3, p2, out _, out p5);

            }
            else if (type.HasFlag(CurveType.FourthPointIsCenter))
            {
                p5 = GetEllipsePoint(45, p1, p2, p4);
                p3 = GetEllipsePoint(135, p1, p2, p4);
                p4 = GetEllipsePoint(-90, p1, p2, p4);
            }
            else
            {
                Missing2PointsOfEllipse(p1, p2, p3, out _, out p5);
            }
        }

        /// <summary>
        /// Creates a conic section which results in an ellipse from the given points with angle of rotation if supplied.
        /// Providing three points will always result in this conic being ellipse.
        /// Because other two points will be calculated in a way that it will result in a valid ellipse rather than parabola or hyperbola.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        public static void GetEllipseMakingPoints(ref VectorF p1, ref VectorF p2, ref VectorF p3, CurveType type, out VectorF p4, out VectorF p5)
        {
            bool fitting = type.HasFlag(CurveType.Fitting);

            if (fitting)
            {
                p4 = Vectors.FourthPointOfRhombus(p1, p2, p3);
                var a = (p1 + p2) / 2;
                var b = (p2 + p3) / 2;
                var c = (p3 + p4) / 2;
                var d = (p1 + p4) / 2;
                p1 = a;
                p2 = b;
                p3 = c;
                p4 = d;
                Missing2PointsOfEllipse(p1, p3, p2, out _, out p5);
            }
            else if (type.HasFlag(CurveType.ThirdPointOnEllipse))
            {
                Order3Points(ref p1, ref p2, ref p3);
                Missing2PointsOfEllipse(p1, p2, p3, out p4, out p5);
            }
            else
            {
                p4 = GetEllipsePoint(-90, p1, p2, p3);
                p5 = GetEllipsePoint(45, p1, p2, p3);
                p3 = GetEllipsePoint(135, p1, p2, p3);
            }
        }
        #endregion

        #region GET 4 ANGLES
        public static float[] GetAngles(params float[] rotations)
        {
            var angles = new float[4];

            if (rotations.Length == 0)
                return angles;

            Array.Copy(rotations, 0, angles, 0, Math.Min(rotations.Length, angles.Length));

            if (angles[0] != 0 || angles[1] != 0)
            {
                if (angles[2] == 0)
                    angles[2] = Numbers.Avg(angles[0], angles[2]);
                if (angles[3] == 0)
                    angles[3] = angles[2] +  Math.Abs(angles[1] - angles[2]);
            }
            return angles;
        }
        #endregion

        #region TRANSLATE CONIC TO ORIGIN
        //https://math.stackexchange.com/a/3586659/654108
        public static void TranslateConicToOrigin(float A, float B, float C, ref float D, ref float E, ref float F)
        {
            float numerator, denominator;

            numerator = (-A * E * E) + (B * D * E) - (C * D * D);
            denominator = (4 * A * C) - (B * B);

            float addValue = numerator / denominator;
            F = F + addValue;
            D = 0;
            E = 0;
        }
        #endregion

        #region BRESENHAM ELLIPSE
        public static void BresenhamEllipse(int x, int y, int w, int h, VectorAction<int> action, Position portion = Position.All)
        {
            if (portion == 0)
                portion = Position.All;

            int a = w / 2;
            int b = h / 2;
            int a2 = a * a;
            int b2 = b * b;
            int fa2 = 4 * a2;
            int fb2 = 4 * b2;
            int x0, y0, sigma;
            int xc = x + a;
            int yc = y + b;

            bool left = portion.HasFlag(Position.Left);
            bool top = portion.HasFlag(Position.Top);
            bool right = portion.HasFlag(Position.Right);
            bool bottom = portion.HasFlag(Position.Bottom);


            for (x0 = 0, y0 = b, sigma = 2 * b2 + a2 * (1 - 2 * b); b2 * x0 <= a2 * y0; x0++)
            {
                if (bottom || right)
                    action(xc + x0, yc + y0);

                if (bottom || left)
                    action(xc - x0, yc + y0);

                if (right || top)
                    action(xc + x0, yc - y0);

                if (left || top)
                    action(xc - x0, yc - y0);

                if (sigma >= 0)
                {
                    sigma += fa2 * (1 - y0);
                    y0--;
                }
                sigma += b2 * (4 * x0 + 6);
            }

            for (x0 = a, y0 = 0, sigma = 2 * a2 + b2 * (1 - 2 * a); a2 * y0 <= b2 * x0; y0++)
            {
                if (bottom || right)
                    action(xc + x0, yc + y0);

                if (bottom || left)
                    action(xc - x0, yc + y0);

                if (right || top)
                    action(xc + x0, yc - y0);

                if (left || top)
                    action(xc - x0, yc - y0);

                if (sigma >= 0)
                {
                    sigma += fb2 * (1 - x0);
                    x0--;
                }
                sigma += a2 * (4 * y0 + 6);
            }
        }
        public static void BresenhamEllipse(int x, int y, int w, int h, FillAction<int> action)
        {
            int a = w / 2;
            int b = h / 2;
            int a2 = a * a;
            int b2 = b * b;
            int fa2 = 4 * a2;
            int fb2 = 4 * b2;
            int x0, y0, sigma;
            int xc = x + a;
            int yc = y + b;

            for (x0 = 0, y0 = b, sigma = 2 * b2 + a2 * (1 - 2 * b); b2 * x0 <= a2 * y0; x0++)
            {
                action(xc + x0, yc + y0, true, xc - x0, null, 0);
                action(xc + x0, yc - y0, true, xc - x0, null, 0);

                if (sigma >= 0)
                {
                    sigma += fa2 * (1 - y0);
                    y0--;
                }
                sigma += b2 * (4 * x0 + 6);
            }

            for (x0 = a, y0 = 0, sigma = 2 * a2 + b2 * (1 - 2 * a); a2 * y0 <= b2 * x0; y0++)
            {
                action(xc + x0, yc + y0, true, xc - x0, null, 0);
                action(xc + x0, yc - y0, true, xc - x0, null, 0);

                if (sigma >= 0)
                {
                    sigma += fb2 * (1 - x0);
                    x0--;
                }
                sigma += a2 * (4 * y0 + 6);
            }
        }
        #endregion

        #region ROUNDED RECTANGLE POINTS
        public static IList<VectorF> RoundedBoxPoints(float x, float y, float width, float height, float cornerRadius)
        {
            cornerRadius = Math.Min(cornerRadius, Math.Min(width / 2, height / 2) - 1);

            VectorF p1 = new VectorF(x, y + cornerRadius);
            VectorF p2 = new VectorF(x, y);
            VectorF p3 = new VectorF(x + cornerRadius, y);

            VectorF p4 = new VectorF(x + width - cornerRadius, y);
            VectorF p5 = new VectorF(x + width, y);
            VectorF p6 = new VectorF(x + width, y + cornerRadius);

            VectorF p7 = new VectorF(x + width, y + height - cornerRadius);
            VectorF p8 = new VectorF(x + width, y + height);
            VectorF p9 = new VectorF(x + width - cornerRadius, y + height);

            VectorF p10 = new VectorF(x + cornerRadius, y + height);
            VectorF p11 = new VectorF(x, y + height);
            VectorF p12 = new VectorF(x, y + height - cornerRadius);

            var source = new VectorF[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12 };

            return GetBezierPoints(4, BezierType.Quadratric, source, false);
        }
        #endregion

        #region CORRECT ARC POINTS
        public static bool CorrectArcPoints(this IConic conic, ref VectorF startPoint, ref VectorF endPoint, VectorF? center = null)
        {
            return CorrectArcPoints(conic.Cx, conic.Cy, conic.Rx, conic.Ry, conic.Rotation, ref startPoint, ref endPoint, center);
        }
        public static bool CorrectArcPoints(float Cx, float Cy, float Rx, float Ry, Rotation Angle, 
            ref VectorF startPoint, ref VectorF endPoint, VectorF? center = null)
        {
            var c = center ?? new VectorF(Cx, Cy);
            VectorF m, p;
            var move = -Math.Max(Rx, Ry);
            p = Vectors.Move(startPoint, c, move);

            bool ok1 = Interception(Rx, Ry, Cx, Cy, p, c, out _, out m, Angle, false);
            if (ok1)
                startPoint = m ? m : startPoint;

            p = Vectors.Move(endPoint, c, move);
            bool ok2 = Interception(Rx, Ry, Cx, Cy, p, c, out _, out m, Angle, false);
            if (ok2)
                endPoint = m ? m : endPoint;

            return (ok1 || ok2);
        }
        #endregion

        #region EFFECTIVE ROTATION
        public static Rotation EffectiveRotation(this IConic conic, out float width, out float height)
        {
            var rotation = conic.Rotation is Rotation? 
                (Rotation) conic.Rotation: new Rotation(conic.Rotation);
            var degree = rotation.Degree;
            width = conic.Bounds.Width;
            height = conic.Bounds.Height;
            bool a45to90 = degree >= 45 && degree < 90;
            bool a90to135 = degree >= 90 && degree < 135;
            bool a225to270 = degree >= 225 && degree < 270;
            bool a270to315 = degree >= 270 && degree < 315;

            if (a45to90 || a90to135 || a225to270 || a270to315)
                Numbers.Swap(ref width, ref height);

            if (a45to90 || a225to270 || a90to135)
                rotation = -rotation;
            else if (a270to315)
                rotation += rotation;

            if (a90to135 || a270to315)
                rotation += conic.TiltAngle;

            return rotation;
        }
        #endregion

        #region GET PIE TRIANGLE
        /// <summary>
        /// Returns array of points which is of length 3.
        /// First point represents center of the arc and second and third points represents start and end angle respectively.
        /// </summary>
        /// <param name="conic">conic object to get a pie cut from.</param>
        /// <param name="type">Type of curve i.e. arc or pie whichever is needed.</param>
        /// <param name="StartAngle">Starting angle of cut.</param>
        /// <param name="EndAngle">Ending angle of cut.</param>
        /// <param name="center">If not null, pie cut will be calculated using this center otherwise center of conic will be used.</param>
        /// <returns>Array of points</returns>
        public static VectorF[] GetPieTriangle(this IConic conic, CurveType type, float StartAngle, float EndAngle, VectorF? center = null)
        {
            if (!type.HasFlag(CurveType.Arc) && !type.HasFlag(CurveType.Pie))
                return null;

            if (StartAngle == 0 && EndAngle == 0)
                return null;

            var c = GetCalculatedCenter(conic.Rotation, conic.Cx, conic.Cy, center, out Rotation eAngle);
            var startPoint = GetEllipsePoint(StartAngle, c.X, c.Y, conic.Rx, conic.Ry, eAngle, true);
            var endPoint = GetEllipsePoint(EndAngle, c.X, c.Y, conic.Rx, conic.Ry, eAngle, true);
            CorrectArcPoints(conic.Cx, conic.Cy, conic.Rx, conic.Ry, conic.Rotation, ref startPoint, ref endPoint, c);
            return new VectorF[] { c, startPoint, endPoint };
        }

        /// <summary>
        /// Returns array of points which is of length 3.
        /// First point represents center of the arc and second and third points represents start and end angle respectively.
        /// </summary>
        /// <param name="conic">conic object to get a pie cut from.</param>
        /// <param name="type">Type of curve i.e. arc or pie whichever is needed.</param>
        /// <param name="center">If not null, pie cut will be calculated using this center otherwise center of conic will be used.</param>
        /// <returns>Array of points</returns>
        public static VectorF[] GetPieTriangle(this IConic conic, CurveType type, VectorF? center = null)
        {
            return conic.GetPieTriangle(type, conic.StartAngle, conic.EndAngle);
        }

        static VectorF GetCalculatedCenter(Rotation angle, float Cx, float Cy, VectorF? intendedCenter, out Rotation eAngle)
        {
            var autoCenter = intendedCenter == null || !intendedCenter.Value;
            eAngle = autoCenter ? angle : Rotation.Empty;
            VectorF center = intendedCenter ?? VectorF.Empty;

            if (autoCenter)
                center = new VectorF(Cx, Cy);

            if (!autoCenter && angle)
                center = angle.Rotate(center, true);

            return center;
        }
        #endregion

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
        public static void NissenEllipse(int x, int y, int width, int height, Rotation angle, PixelAction<float> action, DrawCommand lineCommand = 0)
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
        public static void GwsEllipse(int x, int y, int width, int height, Rotation angle, PixelAction<float> action, DrawCommand lineCommand = 0)
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
#endif
}
