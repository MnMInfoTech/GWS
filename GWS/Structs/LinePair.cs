/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)
using System;

namespace MnM.GWS
{
    /// <summary>
    /// Source: https://phys.libretexts.org/Bookshelves/Astronomy_and_Cosmology_TextMaps/Map%3A_Celestial_Mechanics_(Tatum)/2%3A_Conic_Sections/2.8%3A_Fitting_a_Conic_Section_Through_Five_Points
    /// </summary>
    public struct LinePair
    {
        public readonly float X2Coefficient, XYCoefficient, Y2Coefficient, XCoefficient, YCoefficient;//line pair equation coefficients
        public readonly float C;

       public  LinePair(ILine line1, ILine line2): this()
        {
            //get standard-form coefficients from slop-intercept form
            var m1 = line1.Slope();
            var m2 = line2.Slope();

            var a1 = -m1;
            var b1 = 1.0f;
            var c1 = -(line1.Y1 - m1 * line1.X1);

            var a2 = -line2.Slope();
            var b2 = 1.0f;
            var c2 = -(line2.Y1 - m2 * line2.X1);

            //eqn of pair of 2 lines
            //a1 a2 x ^ 2 + a1 b2 x y + a1 c2 x +a2 b1 x y + a2 c1 x +b1 b2 y^ 2 + b1 c2 y +b2 c1 y +c1 c2 = 0

            C = c1 * c2;
            X2Coefficient = (a1 * a2) / C;
            XYCoefficient = ((a1 * b2) + (a2 * b1)) / C;
            Y2Coefficient = (b1 * b2) / C;
            XCoefficient = ((a1 * c2) + (a2 * c1)) / C;
            YCoefficient = ((b1 * c2) + (b2 * c1)) / C;

            C = C / C; //1.0
        }
       public  LinePair(IPointF p1, IPointF p2, IPointF p3, IPointF p4) 
        {
            var m1 = Vectors.Slope(p1, p2);
            var m2 = Vectors.Slope(p3, p4);

            var a1 = -m1;
            var b1 = 1.0f;
            var c1 = -(p1.Y - m1 * p1.X);

            var a2 = -m2;
            var b2 = 1.0f;
            var c2 = -(p3.Y - m2 * p3.X);

            //eqn of pair of 2 lines
            //a1 a2 x ^ 2 + a1 b2 x y + a1 c2 x +a2 b1 x y + a2 c1 x +b1 b2 y^ 2 + b1 c2 y +b2 c1 y +c1 c2 = 0

            C = c1 * c2;
            X2Coefficient = (a1 * a2) / C;
            XYCoefficient = ((a1 * b2) + (a2 * b1)) / C;
            Y2Coefficient = (b1 * b2) / C;
            XCoefficient = ((a1 * c2) + (a2 * c1)) / C;
            YCoefficient = ((b1 * c2) + (b2 * c1)) / C;

            C = C / C; //1.0
        }

        public float SolveEquation(float x, float y)
        {
            float value = X2Coefficient * x * x + XYCoefficient * x * y + Y2Coefficient * y * y + XCoefficient * x + YCoefficient * y + C;
            return value;
        }

        public static void ConicEquation(IPointF p1, IPointF p2, IPointF p3, IPointF p4, IPointF p5, 
            out float A, out float B, out float C, out float D, out float E,out float F)
        {
            var firstPair = new LinePair(p1, p2, p3, p4);
            var secondPair = new LinePair(p1, p3, p2, p4);

            float firstSolution, secondSolution, lambda;

            //solve the equation using the last point to obtain lambda
            firstSolution = firstPair.SolveEquation(p5.X, p5.Y);
            secondSolution = secondPair.SolveEquation(p5.X, p5.Y);
            lambda = -firstSolution / secondSolution;

            //multiply the coefficients of the second pair of lines with lambda and sum with the first pair's coefficients to obtain the Conic's coefficients
            A = (firstPair.X2Coefficient + secondPair.X2Coefficient * lambda);
            B = (firstPair.XYCoefficient + secondPair.XYCoefficient * lambda);
            C = (firstPair.Y2Coefficient + secondPair.Y2Coefficient * lambda);
            D = (firstPair.XCoefficient + secondPair.XCoefficient * lambda);
            E = (firstPair.YCoefficient + secondPair.YCoefficient * lambda);
            F = (firstPair.C + secondPair.C * lambda);
        }

        public static float ConicAngle(VectorF p1, VectorF p2, VectorF p3, VectorF p4, VectorF p5, out float Cx, out float Cy, out float Width, out float Height)
        {
            ConicEquation(p1, p2, p3, p4, p5, out float A, out float B, out float C, out float D, out float E, out float F);

            float bsq4ac = (B * B) - (4 * A * C);
            Cx = (((2 * C * D) - (B * E)) / bsq4ac);
            Cy = (-(((B * D) - (2 * A * E)) / bsq4ac));
            float angle = (float)(Math.Atan(B / (A - C)) / 2) * Angles.Radinv;

            Curves.TranslateConicToOrigin(A, B, C, ref D, ref E, ref F);

            if (bsq4ac < 0)
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
            return angle;
        }
    }
}
#endif
