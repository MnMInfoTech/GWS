/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    /// <summary>
    /// Defines conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0 where A, B, C, D, E and F are constants. 
    /// </summary>
    public struct Conic : IConic
    {
        #region VARIABLES
        /// <summary>
        /// Indicates type of this conic i.e. Ellipse or Hyperbola or Parabola.
        /// </summary>
        readonly ConicType Type;

        /// <summary>
        /// X co-ordinate of center of this conic.
        /// </summary>
        public readonly int Cx;

        /// <summary>
        /// Y co-ordinate of center of this conic.
        /// </summary>
        public readonly int Cy;

        /// <summary>
        /// Gets original tilt angle of this object. 
        /// Only exists when this object is created throught points and not bounds.
        /// </summary>
        public readonly float TiltAngle;

        /// <summary>
        /// Far left cordinate where this conic starts from.
        /// </summary>
        readonly float X;

        /// <summary>
        /// Far top cordinate where this conic starts from.
        /// </summary>
        readonly float Y;

        /// <summary>
        /// A in conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0.
        /// </summary>
        readonly float A;

        /// <summary>
        /// B in conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0.
        /// </summary>
        readonly float B;

        /// <summary>
        /// C in conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0.
        /// </summary>
        readonly float C;

        /// <summary>
        /// D in conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0.
        /// </summary>
        readonly float D;

        /// <summary>
        /// E in conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0.
        /// </summary>
        readonly float E;

        /// <summary>
        /// F in conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0.
        /// </summary>
        readonly float F;

        readonly int XEnd, YEnd, YMax, XMax, XStart, YStart;
        readonly bool RightYGreater, TopXGreater;
        readonly static Collection<float> List1 = new Collection<float>();
        readonly static Collection<float> List2 = new Collection<float>();
        readonly bool valid;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new conic for circle or ellipse or pie or an arc specified by the bounding area, cut angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="x">X cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="width">Width of a bounding area where the circle/ellipse is to be drawn -> circle/ellipse's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the circle is to be drawn ->circle/ellipse's minor Y axis = Height/2</param>
        /// <param name="rotation">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">Start angle from where a curve start</param>
        internal Conic(Rotation rotation, float x, float y, float width, float height, float startAngle = 0, float endAngle = 0, float tiltAngle = 0) : this()
        {
            valid = (startAngle == 0 && endAngle == 0) || (startAngle != endAngle);

            if (!valid)
                return;
            ID = "Conic".NewID();

            StartAngle = startAngle;
            EndAngle = endAngle;
            Rx = width / 2;
            Ry = height / 2;
            X = x;
            Y = y;
            Cx = (x + Rx).Round();
            Cy = (y + Ry).Round();

            TiltAngle = tiltAngle;

            if (rotation && rotation.HasCenter)
                rotation.RotateCenter(ref Cx, ref Cy, Rx, Ry, out X, out Y);

            Rotation = new Rotation(rotation.Degree + tiltAngle, Cx, Cy, rotation.HasCenter);

            DefineBoundary(Rotation, out A, out B, out C, out D, out E, out F,
                out YMax, out XMax, out YStart, out XStart, out YEnd, out XEnd, out TopXGreater, out RightYGreater);
        }

        /// <summary>
        /// Creates a new conic for circle or ellipse or pie or an arc specified by the bounding area, cut angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="x">X cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="width">Width of a bounding area where the circle/ellipse is to be drawn -> circle/ellipse's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the circle is to be drawn ->circle/ellipse's minor Y axis = Height/2</param>
        /// <param name="rotation">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">Start angle from where a curve start</param>
        public Conic(float x, float y, float width, float height, float startAngle = 0, float endAngle = 0,
            Rotation rotation = default(Rotation), VectorF Scale = default(VectorF)) : this()
        {
            var bounds = new RectangleF(x, y, width, height);

            if ((!rotation || rotation.Skew == 0) && !Scale.HasScale)
                goto mks;
            bounds = bounds.Scale(rotation, Scale, out bool flat);
            if (flat)
                rotation = Rotation.Empty;
            mks:
            this = new Conic(rotation, bounds.X, bounds.Y, bounds.Width, bounds.Height, startAngle, endAngle);
        }

        /// <summary>
        /// Creates a conic section from the given points with angle of rotation if supplied. Every conic will result in being either ellipse or arc or pie.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="p5">Fifth point</param>
        /// <param name="rotation">Angle to apply rotation while rendering the arc/pie</param>
        public Conic(VectorF p1, VectorF p2, VectorF p3, VectorF p4, VectorF p5,
            Rotation rotation = default(Rotation), VectorF Scale = default(VectorF))
        {
            IList<VectorF> Original = new VectorF[] { p1, p2, p3, p4, p5 };

            if ((!rotation || rotation.Skew == 0) && !Scale.HasScale)
                goto mks;

            Original = Original.RotateAndScale(rotation, Scale, out _, out bool flat);
            if (flat)
                rotation = Rotation.Empty;
            mks:
            float Cx, Cy, W, H;
            var angle = LinePair.ConicAngle(Original[0], Original[1], Original[2], Original[3], Original[4], out Cx, out Cy, out W, out H);

            var startAngle = Original[0].GetAngle(angle, Cx, Cy);
            var endAngle = Original[1].GetAngle(angle, Cx, Cy);
            this = new Conic(rotation, Cx - W / 2, Cy - H / 2, W, H, startAngle, endAngle, angle);
        }

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
        /// <param name="rotation">Angle to apply rotation while rendering the conic</param>
        public Conic(VectorF p1, VectorF p2, VectorF p3, VectorF p4, CurveType type,
            Rotation rotation = default(Rotation), VectorF Scale = default(VectorF))
        {
            Curves.GetEllipseMakingPoints(ref p1, ref p2, ref p3, ref p4, type, out VectorF p5);
            this = new Conic(p1, p2, p3, p4, p5, rotation, Scale);
        }

        /// <summary>
        /// Creates a conic section which results in an ellipse from the given points with angle of rotation if supplied.
        /// Providing three points will always result in this conic being ellipse.
        /// Because other two points will be calculated in a way that it will result in a valid ellipse rather than parabola or hyperbola.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="rotation">Angle to apply rotation while rendering the conic</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        public Conic(VectorF p1, VectorF p2, VectorF p3, CurveType type = CurveType.Full,
            Rotation rotation = default(Rotation), VectorF Scale = default(VectorF))
        {
            Curves.GetEllipseMakingPoints(ref p1, ref p2, ref p3, type, out VectorF p4, out VectorF p5);
            this = new Conic(p1, p2, p3, p4, p5, rotation, Scale);
        }

        /// <summary>
        /// Creates a new conic for circle or ellipse or pie or an arc specified by the bounding area, cut angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="angle"></param>
        public Conic(RectangleF bounds, float startAngle = 0, float endAngle = 0,
            Rotation rotation = default(Rotation), VectorF Scale = default(VectorF)) :
            this(bounds.X, bounds.Y, bounds.Width, bounds.Height, startAngle, endAngle, rotation, Scale)
        { }
        #endregion

        #region PROPERTIES
        public bool Valid => valid;
        /// <summary>
        /// X coo-rdinate of center of this conic.
        /// </summary>
        int ICurvature.Cx => Cx;

        /// <summary>
        /// Y co-ordinate of center of this conic.
        /// </summary>
        int ICurvature.Cy => Cy;

        /// <summary>
        /// Radius of this conic on X axis.
        /// </summary>
        public float Rx { get; private set; }

        /// <summary>
        /// Radius of this conic on Y axis.
        /// </summary>
        public float Ry { get; private set; }

        /// <summary>
        /// Start angle from where a curve start.
        /// </summary>
        public float StartAngle { get; private set; }

        /// <summary>
        /// End Angle where a curve stops.
        /// </summary>
        public float EndAngle { get; private set; }

        /// <summary>
        /// Rotation angle of this conic.
        /// </summary>
        public Rotation Rotation { get; private set; }

        /// <summary>
        /// Area of this conic.
        /// </summary>
        public RectangleF Bounds => new RectangleF(X, Y, Rx * 2, Ry * 2);

        /// <summary>
        /// Indicates if radius on X axis is greater than Radius on Y axis.
        /// </summary>
        public bool WMajor => Rx > Ry;
        public string ID { get; private set; }
        float IConic.TiltAngle => TiltAngle;
        #endregion

        #region DRAW TO
        public bool Draw(IBuffer buffer, IReadContext readContext, out IPen Pen)
        {
            Pen = null;
            if (buffer.Settings.Stroke != 0 && buffer.Settings.FillMode != FillMode.Original)
                return false;

            buffer.Settings.FillCommand &= ~FillCommand.Outlininig;
            bool drawEndsOnly = buffer.Settings.FillCommand.HasFlag(FillCommand.DrawEndsOnly);
            Pen = buffer.Settings.GetPen(this, readContext);

            buffer.CreateAction(Pen, out FillAction<float> action);
            Renderer.Process(this, action, buffer.Settings, drawEndsOnly);
            return true;
        }
        #endregion

        #region TO SHAPE
        public IEnumerable<VectorF> ToShape()
        {
            var pts = Curves.GetEllipsePoints(Cx, Cy, Rx, Ry, false, Rotation);
            return (pts);
        }
        #endregion

        #region DEFINE ELLIPSE BOUNDARY
        void DefineBoundary(Rotation rotation, out float A, out float B, out float C, out float D, out float E, out float F,
            out int YMax, out int XMax, out int YStart, out int XStart, out int YEnd, out int XEnd, out bool TopXGreater, out bool RightYGreater)
        {
            var rxSqr = Numbers.Sqr(Rx);
            var rySqr = Numbers.Sqr(Ry);

            float degree = rotation.Degree;

            float cos = 1, sin = 0;

            if (rotation)
            {
                Angles.SinCos(-degree, out sin, out cos);
            }

            float b0 = ((1 / rxSqr) - (1 / rySqr));
            A = ((cos * cos) / rxSqr) + ((sin * sin) / rySqr);
            B = 2 * cos * sin * b0;
            C = ((cos * cos) / rySqr) + ((sin * sin) / rxSqr);
            D = E = 0;
            F = -1;

            var HMajor = Rx <= Ry;

            if (HMajor)
            {
                Angles.SinCos(-degree - 90, out sin, out cos);
            }

            var a = Rx > Ry ? Rx : Ry;
            var b = Rx > Ry ? Ry : Rx;
            var MajorSqr = a * a;

            var Distance = (float)Math.Sqrt(a * a - b * b);
            var xc = Distance * cos;
            var yc = Distance * sin;
            float A2 = (MajorSqr - xc * xc);
            float B2 = (MajorSqr - yc * yc);
            float C2 = (-xc * yc);

            var minus2C = Math.Sqrt(A2 + B2 - (2 * C2));
            var plus2C = Math.Sqrt(A2 + B2 + (2 * C2));

            var XTopMidPositive = (float)((B2 - C2) / minus2C);
            var XMidBotPositive = (float)((B2 + C2) / plus2C);

            var YTopMidPositive = (float)((A2 - C2) / minus2C);
            var YMidBotPositive = (float)(-(A2 + C2) / plus2C);

            YMax = (int)Math.Sqrt(A2);
            XMax = (int)Math.Sqrt(B2);

            TopXGreater = XTopMidPositive > XMidBotPositive;
            var yPos1 = TopXGreater ? YTopMidPositive : YMidBotPositive;
            var yPos2 = TopXGreater ? YMidBotPositive : YTopMidPositive;
            RightYGreater = YTopMidPositive > -YMidBotPositive;
            YStart = (int)(RightYGreater ? YTopMidPositive : -YMidBotPositive);
            YEnd = (int)(RightYGreater ? -YMidBotPositive : YTopMidPositive);

            float val1, val2, val3, val4;
            SolveEquation((int)yPos1, true, out val1, out val2);
            SolveEquation((int)yPos2, true, out val3, out val4);
            Numbers.Order(ref val1, ref val2);
            Numbers.Order(ref val3, ref val4);

            XStart = (int)val2;
            XEnd = (int)val4;
        }
        #endregion

        #region GET BOUNDARY
        /// <summary>
        /// Gets the maximum position from where scan lines must be requested to fill or draw the curve. 
        /// Value returned depends on whether operation is for drawing or filling. Loop is Boundary ----> 0.
        /// </summary>
        /// <param name="horizontal">Direction of scanning if true then vertically otherwise horizontally.</param>
        /// <param name="forDrawingOnly">If true, value returned is appropriate for drawing end pixels otherwise it is appropriated for filling operation.</param>
        /// <returns>Maximum position in curve.</returns>
        public int GetBoundary(bool horizontal, bool forDrawingOnly = false)
        {
            int boundary;
            if (forDrawingOnly)
                boundary = (horizontal ? YStart : XStart);
            else
                boundary = (horizontal ? YMax : XMax);

            return boundary;
        }
        #endregion

        #region GET DATA AT
        public IList<float>[] GetDataAt(int position, bool horizontal, bool forDrawingOnly, out int axis1, out int axis2)
        {
            float v1, v2, v3, v4;

            List1.Clear();
            List2.Clear();

            bool only2 = SolveEquation(position, horizontal, forDrawingOnly, out v1, out v2, out v3, out v4, out axis1, out axis2);
            List1.Add(v1);
            List2.Add(v3);

            if (!only2)
            {
                List1.Add(v2);
                List2.Add(v4);
            }
            var l1 = List1.ToArray();
            var l2 = List2.ToArray();
            return new IList<float>[] { l1, l2 };
        }
        #endregion

        #region SOLVE EQUATION
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool SolveEquation(float position, bool horizontal, out float v1, out float v2)
        {
            bool validLine = true;
            var evSqr = Numbers.Sqr(position);
            var mult = horizontal ? C : A;
            var oMult = horizontal ? A : C;
            float a1 = (mult * evSqr) + F;
            float b1 = B * position;

            double disc = (b1 * b1) - (4 * oMult * a1);
            if (disc < 0 && (Type == ConicType.Ellipse))
            {
                disc = 0;
                validLine = false;
            }

            double b2 = -b1 / (2 * oMult);
            double d2 = Math.Sqrt(disc) / (2 * oMult);
            v1 = ((float)(b2 + d2)).RoundF(4);
            v2 = ((float)(b2 - d2)).RoundF(4);

            Numbers.Order(ref v1, ref v2);
            return validLine;
        }

        /// <summary>
        /// Solves standard equation for this conic and returns four quardrants and two axis values.
        /// i.e. 2 axis lines either horizontal or vertical.
        /// </summary>
        /// <param name="position">Poistion to query data for. Use GetBoundary function to get max position and iterate from that in descending order by 1 step at a time.
        /// For example, var max = GetBoundary(true, false); for i=max; i>=0; i-=1 { Solvequation (i.....) } </param>
        /// <param name="horizontal">If true, a pair of two horizontal axis lines is returned otherwise vertical.</param>
        /// <param name="forDrawingOnly">If true, only one point from each line is returned.
        /// While drawing curve like ellipse, top - bottom and left - right halves are processed and so it is necessary to avoid overlapped pixels.</param>
        /// <param name="v1">Value of first quardrant.</param>
        /// <param name="v2">Value of second quardrant.
        /// Will be Nan if fordrawingOnly is true.</param>
        /// <param name="v3">Value of third quardrant.</param>
        /// <param name="v4">Value of fourth quardrant.
        /// Will be Nan if fordrawingOnly is true.</param>
        /// <param name="a1">Value of first axis</param>
        /// <param name="a2">Value of second axis</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SolveEquation(int position, bool horizontal, bool forDrawingOnly, out float v1, out float v2, out float v3, out float v4, out int a1, out int a2)
        {
            SolveEquation(position, horizontal, out float val1, out float val2);
            GetAxis(position, horizontal, out a1, out a2);

            if (Type == ConicType.Ellipse)
            {
                v1 = horizontal ? Cx + val1 : Cy - val1;
                v2 = horizontal ? Cx + val2 : Cy - val2;
                v3 = horizontal ? Cx - val1 : Cy + val1;
                v4 = horizontal ? Cx - val2 : Cy + val2;
            }
            else 
            {
                v1 = horizontal ? Cx + val1 : Cy + val1;
                v2 = horizontal ? Cx + val2 : Cy + val2;
                v3 = horizontal ? Cx - val1 : Cy - val1;
                v4 = horizontal ? Cx - val2 : Cy - val2;
            }

            Numbers.Order(ref v1, ref v2);
            Numbers.Order(ref v3, ref v4);

            bool only2 = position > (horizontal ? YEnd : XEnd);
            only2 = only2 && forDrawingOnly;

            if (only2)
            {
                if (horizontal)
                {
                    v1 = RightYGreater ? v2 : v1;
                    v3 = RightYGreater ? v3 : v4;
                }
                else
                {
                    v1 = TopXGreater ? v1 : v2;
                    v3 = TopXGreater ? v4 : v3;

                }
                v2 = v4 = float.NaN;
            }
            return only2;
        }
        #endregion

        #region GET AXIS
        public void GetAxis(int position, bool horizontal, out int axis1, out int axis2)
        {
            var a1 = horizontal ? Cy : Cx;
            var a2 = a1;

            float p1 = position;
            float p2 = -position;
            if (Type == ConicType.Ellipse && horizontal)
                Numbers.Swap(ref p1, ref p2);
            axis1 = (a1 + p1).Round();
            axis2 = (a2 + p2).Round();
        }
        #endregion
    }
}
