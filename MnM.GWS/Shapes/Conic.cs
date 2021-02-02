/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    /// <summary>
    /// Defines conic section Ax2 + Bxy + Cy2 +  Dx + Ey + F = 0 where A, B, C, D, E and F are constants. 
    /// </summary>

#if HideGWSObjects
    partial class NativeFactory
    {
#else
    public
#endif
    partial struct Conic : IConic
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
        uint id;
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
            
            DefineBoundary(Rotation, ref A, ref B, ref C, ref D, ref E, ref F,
                ref YMax, ref XMax, ref YStart, ref XStart, ref YEnd, ref XEnd, ref TopXGreater, ref RightYGreater);
            Name = TypeName.NewName();
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
        /// Indicates if radius on X axis is greater than Radius on Y axis.
        /// </summary>
        public bool WMajor => Rx > Ry;
        public uint ID
        {
            get
            {
                if (id == 0)
                    id = this.NewID();
                return id;
            }
        }
            public string TypeName => "Conic";
        public string Name { get; private set; }
        float IPointF.X => X;
        float IPointF.Y => Y;
        float ISizeF.Width => Rx * 2;
        float ISizeF.Height => Ry * 2;
        float IConic.TiltAngle => TiltAngle;
        #endregion

        #region DRAW TO
        public bool Draw(IWritable buffer, ISettings Settings)
        {
            bool success = false;
            Draw2(buffer, Settings, ref success);
            return success;
        }
        partial void Draw2(IWritable buffer, ISettings Settings, ref bool success);
        #endregion

        #region CONTAINS
        public bool Contains(float x, float y)
        {
            if (x < X || y < Y || x > X + Rx * 2 || y > Y + Ry * 2)
                return false;
            return true;
        }
        #endregion

        #region TO SHAPE
        public IEnumerable<VectorF> Figure()
        {
            var pts = Curves.GetEllipsePoints(Cx, Cy, Rx, Ry, false, Rotation);
            return (pts);
        }
        #endregion

        #region DEFINE ELLIPSE BOUNDARY
        partial void DefineBoundary(Rotation rotation, ref float A, ref float B, ref float C, ref float D, ref float E, ref float F,
            ref int YMax, ref int XMax, ref int YStart, ref int XStart, ref int YEnd, ref int XEnd, ref bool TopXGreater, ref bool RightYGreater);
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
        partial void SolveEquation(float position, bool horizontal, ref float v1, ref float v2);

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
#if HideGWSObjects
    }
#endif
}
#endif
