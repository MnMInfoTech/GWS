/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public static partial  class Vectors
    {
        #region ANGLE BETWEEN 2 Points
        public static Rotation AngleFromVerticalCounterPart(this VectorF p, VectorF q)
        {
            var DY = p.Y - q.Y;
            var DX = p.X - q.X;
            float theta1 = (float)Math.Atan2(DY, DX);
            float theta2 = (float)Math.Atan2(DY, 0);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            var Bounds = new RectangleF(p.X, p.Y, q.X, q.Y);
            return new Rotation(angle, Bounds);
        }
        public static Rotation AngleFromHorizontalCounterPart(this VectorF p, VectorF q)
        {
            var DY = p.Y - q.Y;
            var DX = p.X - q.X;
            float theta1 = (float)Math.Atan2(DY, DX);
            float theta2 = (float)Math.Atan2(0, DX);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            var Bounds = new RectangleF(p.X, p.Y, q.X, q.Y);
            return new Rotation(angle, Bounds);
        }

        public static Rotation AngleFromVerticalCounterPart(this Vector p, Vector q)
        {
            var DY = p.Y - q.Y;
            var DX = p.X - q.X;
            float theta1 = (float)Math.Atan2(DY, DX);
            float theta2 = (float)Math.Atan2(DY, 0);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            var Bounds = new RectangleF(p.X, p.Y, q.X, q.Y);
            return new Rotation(angle, Bounds);
        }
        public static Rotation AngleFromHorizontalCounterPart(this Vector p, Vector q)
        {
            var DY = p.Y - q.Y;
            var DX = p.X - q.X;
            float theta1 = (float)Math.Atan2(DY, DX);
            float theta2 = (float)Math.Atan2(0, DX);
            var angle = Math.Abs(theta1 - theta2) * Angles.Radinv;
            var Bounds = new RectangleF(p.X, p.Y, q.X, q.Y);
            return new Rotation(angle, Bounds);
        }
        #endregion

        #region FIND PERPENDICULAR
        public static VectorF FindPerpendicularTo(this VectorF p, VectorF lp1, VectorF lp2)
        {
            return Lines.Perpendicular(lp1.X, lp1.Y, lp2.X, lp2.Y, p.X, p.Y);
        }
        public static ILine FindPerpendicularLine(this VectorF p, VectorF lp1, VectorF lp2)
        {
            var p1 = Lines.Perpendicular(lp1.X, lp1.Y, lp2.X, lp2.Y, p.X, p.Y);
            return Factory.newLine(p, p1);
        }
        #endregion

        #region SOLVE CONIC FROM 5 POINTS 
        /// <summary>
        /// Gets conic constants from given five points. This is an efficient alternative to Crammer's equatin solving algorithm.      
        /// Source: https://phys.libretexts.org/Bookshelves/Astronomy_and_Cosmology_TextMaps/Map%3A_Celestial_Mechanics_(Tatum)/2%3A_Conic_Sections/2.8%3A_Fitting_a_Conic_Section_Through_Five_Points.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="p5"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        /// <param name="E"></param>
        /// <param name="F"></param>
        /// <param name="Cx"></param>
        /// <param name="Cy"></param>
        /// <returns></returns>
        public static ConicType SolveConicEquation(VectorF p1, VectorF p2, VectorF p3, VectorF p4, VectorF p5,
            out float A, out float B, out float C, out float D, out float E, out float F, out float Cx, out float Cy, out float Width,
            out float Height, out float angle)
        {
            LinePair.ConicEquation(p1, p2, p3, p4, p5, out A, out B, out C, out D, out E, out F);

            float bsq4ac = (B * B) - (4 * A * C);
            Cx = (((2 * C * D) - (B * E)) / bsq4ac);
            Cy = (-(((B * D) - (2 * A * E)) / bsq4ac));

            angle = (float)(Math.Atan(B / (A - C)) / 2) * Angles.Radinv;

            Curves.TranslateConicToOrigin(A, B, C, ref D, ref E, ref F);

            ConicType type;

            if (bsq4ac < 0)
                type = ConicType.Ellipse;

            else if (bsq4ac == 0)
                type = ConicType.Parabola;
            else
                type = ConicType.Hyperbola;


            if (type == ConicType.Ellipse)
            {
                float sin, cos, sin2, cos2, sinSq, cosSq;
                Angles.SinCos(angle, out sin, out cos);

                sin2 = (float)Math.Sin(2 * angle * Angles.Radian);
                cos2 = (float)Math.Cos(2 * angle * Angles.Radian);
                sinSq = sin * sin;
                cosSq = cos * cos;

                float standard_A, standard_B, standard_C;
                standard_B = B * cos2 + (C - A) * sin2;  //should be equal to 0 or near it

                if (standard_B < 0 || standard_B > 0)
                {
                    int pp = 0;
                    //rotation issue
                }
                standard_A = A * cosSq + B * sin * cos + C * sinSq;
                standard_C = A * sinSq - B * sin * cos + C * cosSq;
                float aSqr = -F / standard_A;
                float bSqr = -F / standard_C;

                //multiply by two to get the non-semi axes
                Width = (float)Math.Sqrt(Math.Abs(aSqr)) * 2;
                Height = (float)Math.Sqrt(Math.Abs(bSqr)) * 2;
            }
            else
            {
                Width = Curves.DefaultConicWidth;
                Height = Curves.DefaultConicHeight;
            }
            return type;
        }
        #endregion

        #region TO LINES
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<ILine> ToLines(this IEnumerable<VectorF> data, PointJoin join, float stroke = 0)
        {
            int count = data.Count();

            var lines = new Collection<ILine>(count / 2 + 1);
            bool connectEach = join.HasFlag(PointJoin.ConnectEach);
            bool unique = join.HasFlag(PointJoin.NoRepeat);
            bool joinEnds = join.HasFlag(PointJoin.ConnectEnds);
            bool noTooClose = join.HasFlag(PointJoin.AvoidTooClose);

            VectorF p0, p1, first;
            p0 = p1 = first = VectorF.Empty;

            foreach (var item in data)
            {
                if (!item)
                    continue;

                p1 = item;

                if (!first)
                    first = p1;

                if (!p0)
                {
                    p0 = p1;
                    continue;
                }
                if (unique && p1.Equals(p0))
                    continue;

                if (noTooClose)
                {
                    if (p0.VeryCloseTo(p1))
                        continue;
                }
                lines.Add(Factory.newLine(p0, p1, stroke));

                if (connectEach)
                    p0 = p1;
                else
                    p0 = VectorF.Empty;
            }

            if (connectEach && joinEnds && !Equals(p1, first))
            {
                var line = Factory.newLine(p1, first, stroke);
                lines.Add(line);
            }

            if (join.HasFlag(PointJoin.RemoveLast))
                lines.RemoveAt(lines.Count - 1);

            return lines;
        }
        #endregion
    }
}
