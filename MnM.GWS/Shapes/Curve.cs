/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public struct Curve : ICurve
    {
        #region VARIABLES
        ICurve attachedCurve;
        readonly IConic Conic;
        readonly Line Line1;
        readonly Line Line2;
        IList<ILine> Extra;
        readonly bool NegativeMotion;

        readonly static Collection<float> List1 = new Collection<float>(6);
        readonly static Collection<float> List2 = new Collection<float>(6);
        static bool singleDraw = false;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Creates a new circle or ellipse or pie or an arc specified by the bounding area, start and end angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="x">X cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="width">Width of a bounding area where the circle/ellipse is to be drawn -> circle/ellipse's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the circle is to be drawn ->circle/ellipse's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. If type includes NoSweepAngle option otherwise effective end angle is start angle + end angle</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        /// <param name="assignID">If true assign an unique id to the object</param>
        public Curve(float x, float y, float width, float height, float startAngle = 0, float endAngle = 0,
             CurveType type = CurveType.Pie, Rotation rotation = default(Rotation), VectorF Scale = default(VectorF)) : this()
        {
            if (!type.HasFlag(CurveType.NoSweepAngle))
                endAngle += startAngle;

            Conic = new Conic(x, y, width, height, startAngle, endAngle, rotation, Scale);
            if (startAngle != 0 || endAngle != 0)
            {
                this = new Curve(Conic, Conic.GetPieTriangle(type, startAngle, endAngle), type);
            }
            else
            {
                Type = CurveType.Full;
                ID = Name.NewID();
            }
        }

        /// <summary>
        /// Creates a ellipse or pie or arc using the specified 3 points and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        /// <param name="assignID">If true assign an unique id to the object</param>
        /// <returns>ICurve</returns>
        public Curve(VectorF p1, VectorF p2, VectorF p3, CurveType type = CurveType.Pie, Rotation rotation = default(Rotation), VectorF Scale = default(VectorF)) : this()
        {
            Conic = new Conic(p1, p2, p3, type, rotation, Scale);
            this = new Curve(Conic, Conic.GetPieTriangle(type), type);
        }

        /// <summary>
        /// Creates a ellipse or pie or arc using the specified 4 points and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Third point</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        /// <param name="assignID">If true assign an unique id to the object</param>
        public Curve(VectorF p1, VectorF p2, VectorF p3, VectorF p4, CurveType type = CurveType.Full, Rotation rotation = default(Rotation), VectorF Scale = default(VectorF)) : this()
        {
            Conic = new Conic(p1, p2, p3, p4, type, rotation, Scale);
            this = new Curve(Conic, Conic.GetPieTriangle(type), type);
        }

        /// <summary> 
        /// Creates a ellipse or pie or arc using the specified five points conic section and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="p5">Fifth point</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        /// <param name="assignID">If true assign an unique id to the object</param>
        public Curve(VectorF p1, VectorF p2, VectorF p3, VectorF p4, VectorF p5, CurveType type = CurveType.Pie,
            Rotation rotation = default(Rotation), VectorF Scale = default(VectorF)) : this()
        {
            Conic = new Conic(p1, p2, p3, p4, p5, rotation, Scale);
            this = new Curve(Conic, Conic.GetPieTriangle(type), type);
        }

        /// <summary>
        /// Creates a curve replicationg data provided by circle parameter.
        /// </summary>
        /// <param name="conic">A circle whiose identical copy is to be created</param>
        /// <param name="assignID">If true assign an unique id to the object</param>
        /// <returns>ICurve</returns>
        public Curve(IConic conic, Rotation rotation = default(Rotation), VectorF Scale = default(VectorF)) : this()
        {
            CurveType type = CurveType.Full;
            float startAngle = 0, endAngle = 0;

            if (conic is ICurve)
            {
                startAngle = (conic as ICurve).StartAngle;
                endAngle = (conic as ICurve).EndAngle;
                type = (conic as ICurve).Type;
            }
            Conic = new Conic(conic.Bounds, 0, 0, rotation, Scale);
            this = new Curve(Conic, Conic.GetPieTriangle(type, startAngle, endAngle), type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conic">A conic whose perimeter will be used.</param>
        /// <param name="pie"></param>
        /// <param name="type"></param>
        /// <param name="assignID"></param>
        public Curve(IConic conic, VectorF[] pie, CurveType type) : this()
        {
            Conic = conic;
            ID = "Conic".NewID();

            if (pie == null || pie.Length < 3 || (!type.HasFlag(CurveType.Pie) && !type.HasFlag(CurveType.Arc)) || pie[1].Equals(pie[2]))
            {
                Type = CurveType.Full;
                ArcLine = Line1 = Line2 = Line.Empty;
                StartAngle = EndAngle = 0;
                NegativeMotion = false;
                Extra = null;
                return;
            }
            var pieP1 = pie[0];
            var pieP2 = pie[1];
            var pieP3 = pie[2];

            Type = type;

            if (Type.HasFlag(CurveType.Pie))
                Type = Type.Exclude(CurveType.Arc);
            else
                Type = Type.Exclude(CurveType.Pie);

            NegativeMotion = Type.HasFlag(CurveType.AntiClock);
            ArcLine = new Line(pieP2, pieP3);
            Line1 = new Line(pieP2, pieP1);
            Line2 = new Line(pieP3, pieP1);
            StartAngle = pieP2.GetAngle(pieP1.X, pieP1.Y);
            EndAngle = pieP3.GetAngle(pieP1.X, pieP1.Y);
            Extra = GetClosingLines();
        }
        #endregion

        #region PROPERTIES
        public bool Valid => Conic.Valid;
        public int Cx => Conic.Cx;
        public int Cy => Conic.Cy;
        public float Rx => Conic.Rx;
        public float Ry => Conic.Ry;
        public Rotation Rotation => Conic.Rotation;
        public RectangleF Bounds => Conic.Bounds;
        public string ID { get; private set; }
        public ICurve AttachedCurve
        {
            get => attachedCurve;
            set
            {
                attachedCurve = value;
                Extra = GetClosingLines();
            }
        }
        public string Name
        {
            get
            {
                if (Full)
                {
                    if (Conic.Rx == Conic.Ry)
                        return "Circle";
                    return "Elipse";
                }
                if (Type.HasFlag(CurveType.Pie))
                    return "Pie";
                if (Type.HasFlag(CurveType.ClosedArc))
                    return "ClosedArc";
                if (Type.HasFlag(CurveType.Arc))
                    return "Arc";
                return "Ellipse";
            }
        }
        public bool Full => Type == CurveType.Full;
        bool ICut.IsEmpty => Full;
        public VectorF[] PieTriangle
        {
            get
            {
                if (Full)
                    return null;

                return new VectorF[]
                {
                    new VectorF(Conic.Cx, Conic.Cy),
                    Conic.GetEllipsePoint(StartAngle, true),
                    Conic.GetEllipsePoint(EndAngle, true)
                };
            }
        }
        public  float TiltAngle => Conic.TiltAngle;
        #endregion

        #region DRAW TO
        public bool Draw(IWritable buffer, IReadContext readContext, out IPen Pen)
        {
            Pen = null;
            if (!Valid)
                return true;

            if (singleDraw || buffer.Settings.Stroke == 0 || buffer.Settings.FillMode == FillMode.Original)
            {
                if ((Type.HasFlag(CurveType.Arc) && buffer.Settings.FillMode != FillMode.FillOutLine) 
                    || buffer.Settings.FillMode == FillMode.DrawOutLine)
                    buffer.Settings.FillCommand |= FillCommand.DrawEndsOnly;

                buffer.Settings.FillCommand &= ~FillCommand.Outlininig;
                bool drawEndsOnly = buffer.Settings.FillCommand.HasFlag(FillCommand.DrawEndsOnly);
                Pen = buffer.GetPen(this, readContext);

                buffer.CreateAction(Pen, out FillAction<float> action);
                Renderer.Process(this, action, buffer.Settings, drawEndsOnly);
                return true;
            }

            var outerCurve = (this).StrokedCurve(buffer.Settings.Stroke, buffer.Settings.FillMode, buffer.Settings.StrokeMode);
            Pen = buffer.GetPen(outerCurve, readContext);

            switch (buffer.Settings.FillMode)
            {
                case FillMode.Outer:
                case FillMode.Inner:
                case FillMode.FillOutLine:
                default:
                    singleDraw = true;
                    outerCurve.Draw(buffer, Pen, out _);
                    singleDraw = false;
                    break;
                case FillMode.ExceptOutLine:
                    singleDraw = true;
                    buffer.Settings.FillMode = FillMode.DrawOutLine;
                    outerCurve.Draw(buffer, Pen, out _);
                    buffer.Settings.FillMode = FillMode.Original;
                    buffer.Settings.FillCommand &= ~FillCommand.DrawEndsOnly;
                    outerCurve.AttachedCurve.Draw(buffer, Pen, out _);
                    singleDraw = false;
                    break;
            }
            return true;
        }
        #endregion

        #region TO SHAPE
        public IEnumerable<VectorF> ToShape()
        {
            VectorF[] pts;
            if (Full)
                pts = Curves.GetEllipsePoints(Cx, Cy, Rx, Ry, false, Rotation);
            else
                pts = Curves.GetArcPoints(StartAngle, EndAngle,
                    Type.HasFlag(CurveType.Pie), Cx, Cy, Rx, Ry, Rotation, Type.HasFlag(CurveType.AntiClock));
            return (pts);
        }
        #endregion

        #region GET BOUNDARIES
        public int GetBoundary(bool horizontal, bool forDrawingOnly = false) =>
            Conic.GetBoundary(horizontal, forDrawingOnly);
        #endregion

        #region GET AXIS
        public void GetAxis(int position, bool horizontal, out int axis1, out int axis2) =>
            Conic.GetAxis(position, horizontal, out axis1, out axis2);
        #endregion

        #region GET DATA
        public IList<float>[] GetDataAt(int position, bool horizontal, bool forOutLinesOnly, out int axis1, out int axis2)
        {
            IList<float>[] list, attachList = null;
            bool commonPosition = position <= AttachedCurve?.GetBoundary(horizontal, false);

            if (commonPosition)
                attachList = AttachedCurve.GetDataAt(position, horizontal, forOutLinesOnly, out _, out _);

            list = SolveEquation(position, horizontal, forOutLinesOnly, out axis1, out axis2);

            if (attachList != null)
            {
                list[0] = list[0].Join( attachList[0]).ToArray();
                list[1] = list[1].Join( attachList[1]).ToArray();
            }
            return list;
        }
        IList<float>[] SolveEquation(int position, bool horizontal, bool forDrawingOnly, out int axis1, out int axis2)
        {
            float v1, v2, v3, v4;

            List1.Clear();
            List2.Clear();

            bool only2 = Conic.SolveEquation(position, horizontal, forDrawingOnly, out v1, out v2, out v3, out v4, out axis1, out axis2);

            if (CheckPixel(v1, axis1, horizontal))
                List1.Add(v1);

            if (CheckPixel(v3, axis2, horizontal))
                List2.Add(v3);

            if (!only2)
            {
                if (CheckPixel(v2, axis1, horizontal))
                    List1.Add(v2);

                if (CheckPixel(v4, axis2, horizontal))
                    List2.Add(v4);
            }

            if (Full || forDrawingOnly)
                goto mks;

            if (!Full)
            {
                AddValsSafe(axis1, horizontal, List1);
                AddValsSafe(axis2, horizontal, List2);
            }

        mks:
            var l1 = List1.ToArray();
            var l2 = List2.ToArray();
            return new IList<float>[] { l1, l2 };
        }
        public bool SolveEquation(int position, bool horizontal, bool forDrawingOnly, out float v1, out float v2, out float v3, out float v4, out int a1, out int a2) =>
            Conic.SolveEquation(position, horizontal, forDrawingOnly, out v1, out v2, out v3, out v4, out a1, out a2);
        #endregion

        #region CLONE - DISPOSE
        public object Clone()
        {
            var curve = new Curve(this);
            return curve;
        }
        public void Dispose()
        {
            AttachedCurve = null;
        }
        #endregion

        #region PROPERTIES
        public CurveType Type { get; private set; }
        public ILine ArcLine { get; private set; }
        public float StartAngle { get; private set; }
        public float EndAngle { get; private set; }
        #endregion

        #region CHECK PIXEL
        public bool CheckPixel(float val, int axis, bool horizontal)
        {
            if (Full)
                return true;
            if (horizontal)
            {
                if (NegativeMotion)
                    return !ArcLine.IsGreaterThan(val, axis);
                return !ArcLine.IsLessThan(val, axis);
            }
            else
            {
                if (NegativeMotion)
                    return !ArcLine.IsGreaterThan(axis, val);
                return !ArcLine.IsLessThan(axis, val);
            }
        }
        #endregion

        #region ADD VAL SAFE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddValsSafe(int axis, bool horizontal, ICollection<float> list)
        {
            float value;

            if (Type.HasFlag(CurveType.ClosedArc))
            {
                if (ArcLine.Scan(axis, horizontal, out value))
                    list.Add(value);
                return;
            }

            if (Type.HasFlag(CurveType.Arc))
            {
                if (Extra != null)
                {
                    foreach (var l in Extra)
                        if (l.Scan(axis, horizontal, out value))
                            list.Add(value);
                }
                return;
            }

            if (Type.HasFlag(CurveType.Pie))
            {
                if (Line1.Scan(axis, horizontal, out value))
                    list.Add(value);
                if (Line2.Scan(axis, horizontal, out value))
                    list.Add(value);

                HandleAxis(Line1, axis, horizontal, list);
                HandleAxis(Line2, axis, horizontal, list);
                return;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void HandleAxis(ILine l, float axis, bool horizontal, ICollection<float> list)
        {
            if (list.Count % 2 == 0)
                return;

            if (horizontal)
            {
                //if (l.Type != LineType.Horizontal)
                //    return;
                if (l.Y1.Round() == axis)
                    list.Add(l.X1);
            }
            else
            {
                //if (l.Type != LineType.Vertical)
                //    return;
                if (l.X1.Round() == axis)
                    list.Add(l.Y1);
            }
        }
        #endregion

        #region GET CLOSING LINES
        public IList<ILine> GetClosingLines()
        {
            if (Full)
                return null;

            if (attachedCurve == null)
            {
                if (Type.HasFlag(CurveType.Pie))
                    Extra = new ILine[] { Line1, Line2 };

                else if (Type.HasFlag(CurveType.ClosedArc))
                    Extra = new ILine[] { ArcLine }; 

                return Extra;
            }

            IList<ILine> lines = Extra;
            var closingLines = attachedCurve.GetClosingLines();
            var arcLine = attachedCurve.ArcLine;


            if (Type.HasFlag(CurveType.Pie))
                lines = lines.AppendItems(closingLines).ToArray();
            else if (Type.HasFlag(CurveType.ClosedArc))
                lines = new ILine[] { ArcLine, arcLine };
            else if (Type.HasFlag(CurveType.Arc))
                lines = ArcLine.JoinEnds(arcLine);
            return lines;
        }
        #endregion
    }
}
