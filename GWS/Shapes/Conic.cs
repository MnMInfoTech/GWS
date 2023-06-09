/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if GWS || Window
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    #region ICONIC
    /// <summary>
    /// Defines conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0 where A, B, C, D, E and F are constants. 
    /// </summary>
    public interface IConic : IShape, IDegreeHolder, IRectangleF
    {
        /// <summary>
        /// Start angle from where a curve start.
        /// </summary>
        float StartAngle { get; }

        /// <summary>
        /// End Angle where a curve stops.
        /// </summary>
        float EndAngle { get; }

        /// <summary>
        /// X co-ordinate of center of this conic.
        /// </summary>
        int Cx { get; }

        /// <summary>
        /// Y co-ordinate of center of this conic.
        /// </summary>
        int Cy { get; }

        /// <summary>
        /// X axis radius of this conic.
        /// </summary>
        float Rx { get; }

        /// <summary>
        /// Y axis radius of this conic.
        /// </summary>
        float Ry { get; }

        /// <summary>
        /// Gets X co-ordinate of the location this object.
        /// </summary>
        new float X { get; }

        /// <summary>
        /// Gets Y co-ordinate of the location of this object.
        /// </summary>
        new float Y { get; }

        /// <summary>
        /// Gets width of this object.
        /// </summary>
        new float Width { get; }

        /// <summary>
        /// Gets height of this object.
        /// </summary>
        new float Height { get; }

        /// <summary>
        /// Gets original tilt angle of this object. 
        /// Only exists when this object is created through points and not bounds.
        /// </summary>
        float TiltAngle { get; }

        /// <summary>
        /// Get a collections of length 2 which is of scan line fragments for a given position (in relation to YMax if horizontal otherwise XMax).
        /// </summary>
        /// <param name="position">Position (in relation to YMax if horizontal otherwise XMax).</param>
        /// <param name="horizontal">If true horizontal scan of curve is done otherwise vertical scan is performed.</param>
        /// <param name="forDrawingOnly">If true only end points are obtained.</param>
        /// <param name="axis1">First value for Y if horizontal otherwise X.</param>
        /// <param name="axis2">Second value for Y if horizontal otherwise X.</param>
        /// <returns></returns>
        IReadOnlyList<float>[] GetDataAt(int position, bool horizontal, bool forDrawingOnly, out int axis1, out int axis2);

        /// <summary>
        /// Gets the maximum position from where scan lines must be requested to fill or draw the curve. 
        /// Value returned depends on whether operation is for drawing or filling. Loop is Boundary ----> 0.
        /// </summary>
        /// <param name="horizontal">Direction of scanning if true then vertically otherwise horizontally.</param>
        /// <param name="forDrawingOnly">If true, value returned is appropriate for drawing end pixels otherwise it is appropriated for filling operation.</param>
        /// <returns>Maximum position in curve.</returns>
        int GetBoundary(bool horizontal, bool forDrawingOnly = false);
    }
    #endregion

    #region CONIC
    public struct Conic : IConic, IFigure, IExDraw, IExResizable
    {
        #region VARIABLES
        /// <summary>
        /// Indicates type of this conic i.e. Ellipse or Hyperbola or Parabola.
        /// </summary>
        ConicType Type;

        /// <summary>
        /// Far left cordinate where this conic starts from.
        /// </summary>
        public readonly float X;

        /// <summary>
        /// Far top cordinate where this conic starts from.
        /// </summary>
        public readonly float Y;

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
        /// A in conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0.
        /// </summary>
        float A;

        /// <summary>
        /// B in conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0.
        /// </summary>
        float B;

        /// <summary>
        /// C in conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0.
        /// </summary>
        float C;

        /// <summary>
        /// D in conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0.
        /// </summary>
        float D;

        /// <summary>
        /// E in conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0.
        /// </summary>
        float E;

        /// <summary>
        /// F in conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0.
        /// </summary>
        float F;

        int XEnd, YEnd, YMax, XMax, XStart, YStart;
        bool RightYGreater, TopXGreater;

        const string toStr = "{0}, x: {1}, y: {2}, w: {3}, h: {4}";
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
        internal Conic(IDegree degree, float x, float y, float width, float height, 
            float startAngle = 0, float endAngle = 0, float tiltAngle = 0) : this()
        {
            if(startAngle == endAngle && startAngle != 0)
                return;

            StartAngle = startAngle;
            EndAngle = endAngle;
            Rx = width / 2;
            Ry = height / 2;
            X = x;
            Y = y;
            Cx = (x + Rx).Round();
            Cy = (y + Ry).Round();

            TiltAngle = tiltAngle;
            if (degree is ICentreHolder && ((ICentreHolder)degree).Centre != null)
                degree.RotateCenter(ref Cx, ref Cy, Rx, Ry, out X, out Y);            
            float angle = (degree?.Angle ?? 0) + tiltAngle;
            if(degree is ISkewHolder)
            {
                var skew = ((ISkewHolder)degree).Skew;
                if(skew!=null && skew.HasScale && skew.Type == SkewType.Diagonal)
                    angle += skew.Degree;
            }
            Degree = new Rotation(angle, Cx, Cy);
            DefineBoundary(Degree);
        }

        /// <summary>
        /// Creates a new conic for circle or ellipse or pie or an arc specified by the bounding area, cut angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="x">X cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="width">Width of a bounding area where the circle/ellipse is to be drawn -> circle/ellipse's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the circle is to be drawn ->circle/ellipse's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">Start angle from where a curve start</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Conic(float x, float y, float width, float height, float startAngle = 0, float endAngle = 0,
            params IParameter[] parameters) : this()
        {
            this = new Conic(parameters, x, y, width, height, startAngle, endAngle);
        }

        /// <summary>
        /// Creates a conic section from the given points with angle of rotation if supplied. Every conic will result in being either ellipse or arc or pie.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="p5">Fifth point</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Conic(VectorF p1, VectorF p2, VectorF p3, VectorF p4, VectorF p5,
            params IParameter[] parameters)
        {
            this = new Conic(parameters, p1, p2, p3, p4, p5);
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
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Conic(VectorF p1, VectorF p2, VectorF p3, VectorF p4, CurveType type,
            params IParameter[] parameters)
        {
            this = new Conic(parameters, p1, p2, p3, p4, type);
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
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Conic(VectorF p1, VectorF p2, VectorF p3, CurveType type = CurveType.Full,
           params IParameter[] parameters)
        {

            this = new Conic(parameters, p1, p2, p3, type);
        }

        /// <summary>
        /// Creates a new conic for circle or ellipse or pie or an arc specified by the bounding area, cut angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Conic(IBoundsF bounds, float startAngle = 0, float endAngle = 0,
            params IParameter[] parameters)  
        {
            bounds.GetBounds(out float x, out float y, out float w, out float h);
            this = new Conic(parameters, x, y, w, h, startAngle, endAngle);
        }

        /// <summary>
        /// Creates a new conic for circle or ellipse or pie or an arc specified by the bounding area, cut angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Conic(IBounds bounds, float startAngle = 0, float endAngle = 0,
            params IParameter[] parameters)
        {
            bounds.GetBounds(out int x, out int y, out int w, out int h);
            this = new Conic(parameters, x, y, w, h, startAngle, endAngle);
        }

        /// <summary>
        /// Creates a new conic for circle or ellipse or pie or an arc specified by the bounding area, cut angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="x">X cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="width">Width of a bounding area where the circle/ellipse is to be drawn -> circle/ellipse's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the circle is to be drawn ->circle/ellipse's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">Start angle from where a curve start</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Conic(IEnumerable<IParameter> parameters, 
            float x, float y, float width, float height, float startAngle = 0, float endAngle = 0) : this()
        {
            parameters.ExtractRotationScaleParameters(out IRotation rotation, out IScale scale);
            var bounds = new RectangleF(x, y, width, height);
            bounds = bounds.Scale(rotation, scale);
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
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Conic(IEnumerable<IParameter> parameters, 
            VectorF p1, VectorF p2, VectorF p3, VectorF p4, VectorF p5)
        {
            IList<VectorF> Original = new VectorF[] { p1, p2, p3, p4, p5 };
            parameters.ExtractRotationScaleParameters(out IRotation rotation, out IScale scale);

            if ((rotation == null || !rotation.Valid) &&
                (scale == null || !scale.HasScale))
                goto mks;

            Original = Original.Rotate(rotation, scale).ToArray();

            mks:
            float Cx, Cy, W, H;

            var tilt = LinePair.ConicAngle(Original[0], Original[1], Original[2], Original[3],
                Original[4], out Cx, out Cy, out W, out H);
            var startAngle = Original[0].GetAngle(tilt, Cx, Cy);
            var endAngle = Original[1].GetAngle(tilt, Cx, Cy);
            this = new Conic(rotation, Cx - W / 2, Cy - H / 2, W, H, startAngle, endAngle, tilt);
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
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Conic(IEnumerable<IParameter> parameters, VectorF p1, VectorF p2, VectorF p3, VectorF p4, CurveType type)
        {
            Curves.GetEllipseMakingPoints(ref p1, ref p2, ref p3, ref p4, type, out VectorF p5);
            this = new Conic(parameters, p1, p2, p3, p4, p5);
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
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Conic(IEnumerable<IParameter> parameters, VectorF p1, VectorF p2, VectorF p3, CurveType type = CurveType.Full)
        {

            Curves.GetEllipseMakingPoints(ref p1, ref p2, ref p3, type, out VectorF p4, out VectorF p5);
            this = new Conic(parameters ,p1, p2, p3, p4, p5);
        }

        /// <summary>
        /// Creates a new conic for circle or ellipse or pie or an arc specified by the bounding area, cut angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Conic(IEnumerable<IParameter> parameters, IBoundsF bounds, float startAngle = 0, float endAngle = 0)
        {
            bounds.GetBounds(out float x, out float y, out float w, out float h);
            this = new Conic(parameters, x, y, w, h, startAngle, endAngle);
        }

        /// <summary>
        /// Creates a new conic for circle or ellipse or pie or an arc specified by the bounding area, cut angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Conic(IEnumerable<IParameter> parameters, IBounds bounds, float startAngle = 0, float endAngle = 0)
        {
            bounds.GetBounds(out int x, out int y, out int w, out int h);
            this = new Conic(parameters, x, y, w, h, startAngle, endAngle);
        }
        public Conic(IConic conic): this()
        {
            if (conic.StartAngle == conic.EndAngle && conic.StartAngle != 0)
                return;

            X = conic.X;
            Y = conic.Y;
            Rx = conic.Rx;
            Ry = conic.Ry;
            Cx = conic.Cx;
            Cy = conic.Cy;
            StartAngle = conic.StartAngle;
            EndAngle = conic.EndAngle;
            TiltAngle = conic.TiltAngle;
            if (conic.Degree != null && conic.Degree.Valid)
                Degree = (IRotation)conic.Degree.Clone();
            DefineBoundary(Degree);
        }

        Conic(IConic conic, bool ok) : this()
        {
            if (conic.StartAngle == conic.EndAngle && conic.StartAngle != 0)
                return;

            X = 0;
            Y = 0;
            Rx = conic.Rx;
            Ry = conic.Ry;
            Cx = (int)Rx;
            Cy = (int)Ry;
            StartAngle = conic.StartAngle;
            EndAngle = conic.EndAngle;
            TiltAngle = conic.TiltAngle;
            if (conic.Degree != null && conic.Degree.Valid)
                Degree = (IRotation)conic.Degree.Clone();
            DefineBoundary(Degree);
        }
        #endregion

        #region PROPERTIES
        public bool Valid => (StartAngle == 0 && EndAngle == 0) || (StartAngle != EndAngle);
        public float Rx { get; private set; }
        public float Ry { get; private set; }
        public float Width => Rx * 2;
        public float Height => Ry * 2;
        public float StartAngle { get; private set; }
        public float EndAngle { get; private set; }
        public IDegree Degree { get; private set; }
        int IConic.Cx => Cx;
        int IConic.Cy => Cy;
        float IPointF.X => X;
        float IPointF.Y => Y;
        float IConic.TiltAngle => TiltAngle;
        bool IOriginCompatible.IsOriginBased => (int)(Cx - Rx) == 0 && (int)(Cy - Ry) == 0;
        int IPoint.X => (int)(Cx - Rx);
        int IPoint.Y => (int)(Cy - Ry);
        float IConic.X => (Cx - Rx);
        float IConic.Y => (Cy - Ry);
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

        #region GET BOUNDS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        bool IExDraw.Draw(IEnumerable<IParameter> parameters, IExRenderer graphics)
        {
            RectangleF outer, inner;
            IConic myConic = this;

            parameters.Extract(out IExSession session);

            bool OriginalFill = session.Command == 0 || (session.Command & Command.OriginalFill) == Command.OriginalFill;

            if ((session.Stroke == 0) ||  OriginalFill)
            {
                outer = new RectangleF(X, Y, Rx * 2, Ry * 2);
            }
            else
            {
                this.GetStrokeAreas(out outer, out inner, session.Stroke, session.Command.ToEnum<FillCommand>());
                if (session.Stroke < 0)
                    Numbers.Swap(ref outer, ref inner);
            }
            bool drawEndsOnly = (session.Command & Command.DrawOutLines) == Command.DrawOutLines;

            if (session.Scale != null && session.Scale.HasScale || session.Rotation != null && session.Rotation.Valid)
            {
                myConic = new Conic(X, Y, Width, Height, 0, 0, session.Rotation, session.Scale);
            }

            session.SetPen(myConic, outer.Expand());

            using (var polyFill = graphics.newPolyFill(session, session.Command))
            {
               polyFill.ProcessConic(myConic, session.Stroke, drawEndsOnly);
            }
            return true;
        }
        #endregion

        #region DEFINE ELLIPSE BOUNDARY
        void DefineBoundary(IDegree rotation)
        {
            var rxSqr = Numbers.Sqr(Rx);
            var rySqr = Numbers.Sqr(Ry);

            float degree = 0;
            float cos = 1, sin = 0;

            if (rotation != null && rotation.Valid)
            {
                degree = rotation.Angle;
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
                Angles.SinCos(-degree - 90, out sin, out cos);

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

            float val1 = 0, val2 = 0, val3 = 0, val4 = 0;
            SolveEquation((int)yPos1, true, ref val1, ref val2);
            SolveEquation((int)yPos2, true, ref val3, ref val4);
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
        /// <summary>
        /// Get a collections of length 2 which is of scan line fragments for a given position (in relation to YMax if horizontal otherwise XMax).
        /// </summary>
        /// <param name="position">Position (in relation to YMax if horizontal otherwise XMax).</param>
        /// <param name="horizontal">If true horizontal scan of curve is done oterwise vertical scan is performed.</param>
        /// <param name="forDrawingOnly">If true only end points are obtained.</param>
        /// <param name="axis1">First value for Y if horizontal otherwise X.</param>
        /// <param name="axis2">Second value for Y if horizontal otherwise X.</param>
        /// <returns></returns>
        public IReadOnlyList<float>[] GetDataAt(int position, bool horizontal, bool forDrawingOnly, out int axis1, out int axis2)
        {
            bool only2 = SolveEquation(position, horizontal, forDrawingOnly,
                out float v1, out float v2, out float v3, out float v4, out axis1, out axis2);

            int i = !only2? 2: 1;
            int j = (axis1 != axis2 || v1 != v3)? 1: 0;

            if (!only2 && (axis1 != axis2 || v2 != v4))
                ++j;

            var l1 = (i == 2) ? new float[] { v1, v2 } : new float[] { v1 };
            float[] l2;
            l2 = j == 0 ? new float[0] : 
                ((j == 2) ? new float[] { v3, v4 } : new float[] { v3 });
            return new IReadOnlyList<float>[] { l1, l2 };
        }
        #endregion

        #region SOLVE EQUATION PRIVATE
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SolveEquation(float position, bool horizontal, ref float v1, ref float v2)
        {
            var evSqr = Numbers.Sqr(position);
            var mult = horizontal ? C : A;
            var oMult = horizontal ? A : C;
            float a1 = (mult * evSqr) + F;
            float b1 = B * position;

            double disc = (b1 * b1) - (4 * oMult * a1);
            if (disc < 0 && (Type == ConicType.Ellipse))
            {
                disc = 0;
            }
            double b2 = -b1 / (2 * oMult);
            double d2 = Math.Sqrt(disc) / (2 * oMult);
            v1 = ((float)(b2 + d2)).RoundF(4);
            v2 = ((float)(b2 - d2)).RoundF(4);
            Numbers.Order(ref v1, ref v2);
        }
        #endregion

        #region SOLVE EQUATION
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool SolveEquation(int position, bool horizontal, bool forDrawingOnly, 
            out float v1, out float v2, out float v3, out float v4, out int a1, out int a2)
        {
            float val1 = 0, val2 = 0;
            SolveEquation(position, horizontal, ref val1, ref val2);
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
        /// <summary>
        /// Gets two axis line positions for given position.
        /// If queried for boundary position, it gives minimum and maximum value of bounds.
        /// </summary>
        /// <param name="position">Position (in relation to YMax if horizontal otherwise XMax).</param>
        /// <param name="horizontal">If true horizontal scan of curve is done oterwise vertical scan is performed.</param>
        /// <param name="axis1">First axis line.</param>
        /// <param name="axis2">Second axis line.</param>
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

        #region GET POINTS
        VectorF[] IPolygonal<VectorF>.GetPoints()
        {
           return Curves.GetEllipsePoints(Cx, Cy, Rx, Ry, false, Degree);
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
            return new Conic(Degree, X, Y, w, h, StartAngle, EndAngle, TiltAngle);
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            return new Conic(this, true);
        }
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, "Conic", X, Y, Rx * 2, Ry * 2);
        }
    }
    #endregion
}
#endif
