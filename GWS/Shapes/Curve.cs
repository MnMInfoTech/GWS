/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System.Collections.Generic;

namespace MnM.GWS
{
    #region ICURVE
    /// <summary>
    /// Represents an object which has a  one continious segment of curved line which does not form a closed loop, 
    /// where the sum of the distances from two points (foci) to every point on the line is constant.
    /// </summary>
    public interface ICurve : IConic
    {
        /// <summary>
        /// Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it.
        /// </summary>
        CurveType Type { get; }

        /// <summary>
        /// Indicates if any child curve is attached to this object or not.
        /// GWS uses this property to apply strokes to the original curve.
        /// </summary>
        ICurve AttachedCurve { get; }

        /// <summary>
        /// Gets a collection of three points in the following order:
        /// a start point of curve,
        /// center point of the curve,
        /// an end point of the curve.
        /// </summary>
        VectorF[] PieTriangle { get; }

        /// <summary>
        /// Indicates a straight line between start and end points of this curve.
        /// </summary>
        ILine ArcLine { get; }

        /// <summary>
        /// Gets a collection of lines necessary to draw closing cut for this curve.
        /// </summary>
        /// <returns>
        /// If this curve is an arc and is is not closed then ..
        /// If it has no stroke then no lines returned.
        /// If it has stroke than two lines each obtained from joining correspoinding start and end points of inner and outer curve will be returned.
        /// If this curve is a pie then...
        /// If it has no stroke then two pie lines i.e one from start point to the center and another from end point to the center will be returned.
        /// If it has stroke than four pie lines from each curves  consists of inner and outer curve will be returned.
        /// </returns>
        IReadOnlyList<ILine> GetClosingLines();
    }
    #endregion

    #region CURVE
    public struct Curve : ICurve, ICut, IExDraw, IExResizable
    {
        #region VARIABLES
        ICurve attachedCurve;
        readonly Conic Conic;
        readonly ILine Line1;
        readonly ILine Line2;
        IReadOnlyList<ILine> Extra;
        readonly bool NegativeMotion;
        readonly static PrimitiveList<float> List1 = new PrimitiveList<float>(6);
        readonly static PrimitiveList<float> List2 = new PrimitiveList<float>(6);

        public static readonly Curve Empty = new Curve();
        const string toStr = "{0}, x: {1}, y: {2}, w: {3}, h: {4}";
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
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Curve(float x, float y, float width, float height, float startAngle = 0, float endAngle = 0,
             CurveType type = CurveType.Pie, params IParameter[] parameters) : this ()
        {
            this = new Curve(parameters, x, y, width, height, startAngle, endAngle, type);
        }

        /// <summary>
        /// Creates a ellipse or pie or arc using the specified 3 points and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        /// <returns>ICurve</returns>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Curve(VectorF p1, VectorF p2, VectorF p3, CurveType type = CurveType.Pie,
            params IParameter[] parameters) : this()
        {
            this = new Curve(parameters, p1, p2, p3, type);
        }

        /// <summary>
        /// Creates a ellipse or pie or arc using the specified 4 points and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Third point</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Curve(VectorF p1, VectorF p2, VectorF p3, VectorF p4, CurveType type = CurveType.Full,
            params IParameter[] parameters) : this()
        {
            this = new Curve(parameters, p1, p2, p3, p4, type);
        }

        /// <summary> 
        /// Creates a ellipse or pie or arc using the specified five points conic section and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="p5">Fifth point</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Curve(VectorF p1, VectorF p2, VectorF p3, VectorF p4, VectorF p5, CurveType type = CurveType.Pie,
             params IParameter[] parameters) : this()
        {
            this = new Curve(parameters, p1, p2, p3, p4, p5, type);
        }

        /// <summary>
        /// Creates a new conic for circle or ellipse or pie or an arc specified by the bounding area, cut angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Curve(IBoundsF bounds, float startAngle = 0, float endAngle = 0, CurveType type = CurveType.Full,
            params IParameter[] parameters)
        {
            bounds.GetBounds(out float x, out float y, out float w, out float h);
            this = new Curve(parameters, x, y, w, h, startAngle, endAngle, type);
        }

        /// <summary>
        /// Creates a new conic for circle or ellipse or pie or an arc specified by the bounding area, cut angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Curve(IBounds bounds, float startAngle = 0, float endAngle = 0, CurveType type = CurveType.Full,
            params IParameter[] parameters)
        {
            bounds.GetBounds(out int x, out int y, out int w, out int h);
            this = new Curve(parameters, x, y, w, h, startAngle, endAngle, type);
        }

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
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Curve(IEnumerable<IParameter> parameters, float x, float y, float width, float height,
            float startAngle = 0, float endAngle = 0, CurveType type = CurveType.Pie) : this()
        {
            if ((type & CurveType.NoSweepAngle) != CurveType.NoSweepAngle)
                endAngle += startAngle;

            Conic = new Conic(parameters, x, y, width, height, startAngle, endAngle);
            if (startAngle != 0 || endAngle != 0)
            {
                this = new Curve(Conic, Conic.GetPieTriangle(type, startAngle, endAngle), type);
            }
            else
            {
                Type = CurveType.Full;
            }
        }

        /// <summary>
        /// Creates a ellipse or pie or arc using the specified 3 points and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        /// <returns>ICurve</returns>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Curve(IEnumerable<IParameter> parameters, VectorF p1, VectorF p2, VectorF p3, 
            CurveType type = CurveType.Pie) : this()
        {
            var Conic = new Conic(parameters, p1, p2, p3, type);
            this = new Curve(Conic, Conic.GetPieTriangle(type, Conic.StartAngle, Conic.EndAngle), type);
        }

        /// <summary>
        /// Creates a ellipse or pie or arc using the specified 4 points and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Third point</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Curve(IEnumerable<IParameter> parameters, VectorF p1, VectorF p2, VectorF p3, VectorF p4, CurveType type = CurveType.Full) : this()
        {
            var Conic = new Conic(parameters, p1, p2, p3, p4, type);
            this = new Curve(Conic, Conic.GetPieTriangle(type, Conic.StartAngle, Conic.EndAngle), type);
        }

        /// <summary> 
        /// Creates a ellipse or pie or arc using the specified five points conic section and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="p5">Fifth point</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Curve(IEnumerable<IParameter> parameters, VectorF p1, VectorF p2, VectorF p3, VectorF p4, VectorF p5, CurveType type = CurveType.Pie) : this()
        {
            var Conic = new Conic(parameters, p1, p2, p3, p4, p5);
            this = new Curve(Conic, Conic.GetPieTriangle(type, Conic.StartAngle, Conic.EndAngle), type);
        }

        /// <summary>
        /// Creates a curve replicationg data provided by circle parameter.
        /// </summary>
        /// <param name="conic">A circle whiose identical copy is to be created</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public Curve(IConic conic, params IParameter[] parameters) : this()
        {
            CurveType type = CurveType.Full;
            float startAngle = 0, endAngle = 0;

            if (conic is ICurve)
            {
                var curve = ((ICurve)conic);
                startAngle = curve.StartAngle;
                endAngle = ((Curve)conic).EndAngle;
                type = curve.Type;
            }
            Conic = new Conic(conic.X, conic.Y, conic.Width, conic.Height, 0, 0, parameters);
            this = new Curve(Conic, Conic.GetPieTriangle(type, startAngle, endAngle), type);
        }

        /// <summary>
        /// Creates a curve replicationg data provided by conic parameter and specified array of 3 elements representing pie trianlge.
        /// </summary>
        /// <param name="conic">A conic whose perimeter will be used.</param>
        /// <param name="pie"></param>
        /// <param name="type"></param>
        public Curve(IConic conic, VectorF[] pie, CurveType type) : this()
        {
            if (conic is Conic)
                Conic = (Conic)conic;
            else
                Conic = new Conic(conic);

            if (pie == null || pie.Length < 3 || pie[1].Equals(pie[2]) ||
                ((type & CurveType.Pie) != CurveType.Pie && (type & CurveType.Arc) != CurveType.Arc))
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

            if ((Type & CurveType.Pie) == CurveType.Pie)
                Type &= ~CurveType.Arc;
            else
                Type &= ~CurveType.Pie;

            NegativeMotion = (Type & CurveType.AntiClock) == CurveType.AntiClock;
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
        public float X => Conic.X;
        public float Y => Conic.Y;
        public int Cx => Conic.Cx;
        public int Cy => Conic.Cy;
        public float Rx => Conic.Rx;
        public float Ry => Conic.Ry;
        public float Width => Conic.Width;
        public float Height => Conic.Height;
        public IDegree Degree => Conic.Degree;
        public ICurve AttachedCurve
        {
            get => attachedCurve;
            set
            {
                attachedCurve = value;
                Extra = GetClosingLines();
            }
        }
        bool ICut.IsEmpty => Type == CurveType.Full;
        public VectorF[] PieTriangle
        {
            get
            {
                if (Type == CurveType.Full)
                    return null;

                return new VectorF[]
                {
                    new VectorF(Conic.Cx, Conic.Cy),
                    Conic.GetEllipsePoint(StartAngle, true),
                    Conic.GetEllipsePoint(EndAngle, true)
                };
            }
        }
        public float TiltAngle => Conic.TiltAngle;
        public CurveType Type { get; private set; }
        public ILine ArcLine { get; private set; }
        public float StartAngle { get; private set; }
        public float EndAngle { get; private set; }
        bool IOriginCompatible.IsOriginBased => ((IConic)Conic).IsOriginBased;
        int IPoint.X => (int)Conic.X;
        int IPoint.Y => (int)Conic.Y;
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
        public void GetBounds(out float x, out float y, out float w, out float h) =>
            Conic.GetBounds(out x, out y, out w, out h);
        #endregion

        #region DRAW
        bool IExDraw.Draw(IEnumerable<IParameter> parameters, IExRenderer graphics)
        {
            if (!Valid)
                return true;

            parameters.Extract(out IExSession session);

            ICurve myCurve;
            ICurve outerCurve = null;

            bool OriginalFill = session.Command == 0 || (session.Command & Command.OriginalFill) == Command.OriginalFill;
            bool DrawOutLine = (session.Command & Command.DrawOutLines) == Command.DrawOutLines;
            bool ExceptOutLine = !OriginalFill && (session.Command & Command.FillOddLines) == Command.FillOddLines;
            bool FillOutLine = !OriginalFill && !ExceptOutLine && !DrawOutLine;

            bool ComeBack = false;

            myCurve = this;
            if (session.Scale != null && session.Scale.HasScale || session.Rotation != null && session.Rotation.Valid)
                myCurve = new Curve(this, session.Scale, session.Rotation);
            
            if (session.Stroke == 0 || OriginalFill)
                goto DRAW_CURVE;

            outerCurve = myCurve.StrokedCurve(session.Stroke, session.Command.ToEnum<FillCommand>());
            
            if (FillOutLine || DrawOutLine)
            {
                myCurve = outerCurve;
                goto DRAW_CURVE;
            }
            if (!ExceptOutLine)
                goto DRAW_CURVE;

            ComeBack = true;
            DrawOutLine = true;
            myCurve = outerCurve;
            goto DRAW_CURVE;

            DRAW_ATTACH_CURVE:
            myCurve = outerCurve.AttachedCurve;
            DrawOutLine = false;

            DRAW_CURVE:
            bool drawEndsOnly = DrawOutLine;
            session.SetPen(myCurve);

            using (var polyFill = graphics.newPolyFill(session, session.Command))
            {
                polyFill.ProcessConic(myCurve, session.Stroke, drawEndsOnly);
            }
            if (ComeBack)
            {
                ComeBack = false;
                goto DRAW_ATTACH_CURVE;
            }
            return true;
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
        public IReadOnlyList<float>[] GetDataAt(int position, bool horizontal, bool forOutLinesOnly, out int axis1, out int axis2)
        {
            IReadOnlyList<float>[] list, attachList = null;
            bool commonPosition = position <= AttachedCurve?.GetBoundary(horizontal, false);

            if (commonPosition)
                attachList = AttachedCurve.GetDataAt(position, horizontal, forOutLinesOnly, out _, out _);

            list = SolveEquation(position, horizontal, forOutLinesOnly, out axis1, out axis2);

            if (attachList != null)
            {
                list[0] = list[0].AppendItems(attachList[0]).ToArray();
                list[1] = list[1].AppendItems(attachList[1]).ToArray();
            }
            return list;
        }
        IReadOnlyList<float>[] SolveEquation(int position, bool horizontal, bool forDrawingOnly, out int axis1, out int axis2)
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

            bool Full = Type == CurveType.Full;
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
            return new IReadOnlyList<float>[] { l1, l2 };
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

        #region CHECK PIXEL
        public bool CheckPixel(float val, int axis, bool horizontal)
        {
            if (Type == CurveType.Full)
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddValsSafe(int axis, bool horizontal, ICollection<float> list)
        {
            float value;

            if ((Type & CurveType.ClosedArc) == CurveType.ClosedArc)
            {
                if (ArcLine.Scan(axis, horizontal, out value))
                    list.Add(value);
                return;
            }

            if ((Type & CurveType.Arc) == CurveType.Arc)
            {
                if (Extra != null)
                {
                    foreach (var l in Extra)
                        if (l.Scan(axis, horizontal, out value))
                            list.Add(value);
                }
                return;
            }

            if ((Type & CurveType.Pie) == CurveType.Pie)
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

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// <summary>
        /// Gets a collection of lines necessary to draw closing cut for this curve.
        /// </summary>
        /// <returns>
        /// If this curve is an arc and is is not closed then ..
        /// If it has no stroke then no lines returned.
        /// If it has stroke than two lines each obtained from joining correspoinding start and end points of inner and outer curve will be returned.
        /// If this curve is a pie then...
        /// If it has no stroke then two pie lines i.e one from start point to the center and another from end point to the center will be returned.
        /// If it has stroke than four pie lines from each curves  consists of inner and outer curve will be returned.
        /// </returns>
        public IReadOnlyList<ILine> GetClosingLines()
        {
            if (Type == CurveType.Full)
                return null;

            if (attachedCurve == null)
            {
                if ((Type & CurveType.Pie) == CurveType.Pie)
                    Extra = new ILine[] { Line1, Line2 };

                else if ((Type & CurveType.ClosedArc) == CurveType.ClosedArc)
                    Extra = new ILine[] { ArcLine };

                return Extra;
            }

            IReadOnlyList<ILine> lines = Extra;
            var closingLines = attachedCurve.GetClosingLines();
            var arcLine = attachedCurve.ArcLine;


            if ((Type & CurveType.Pie) == CurveType.Pie)
                lines = lines.AppendItems(closingLines).ToArray();
            else if ((Type & CurveType.ClosedArc) == CurveType.ClosedArc)
                lines = new ILine[] { ArcLine, arcLine };
            else if ((Type & CurveType.Arc) == CurveType.Arc)
                lines = ArcLine.JoinEnds(arcLine);
            return lines;
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
            var conic = (IConic) ((IExResizable)Conic).Resize(w, h, out _);
            success = true;
            return new Curve(conic, conic.GetPieTriangle(Type, StartAngle, EndAngle), Type);
        }
        #endregion

        #region GET ORIGIN BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion()
        {
            if (((IOriginCompatible)this).IsOriginBased)
                return this;
            var newConic = (IConic) ((IOriginCompatible)Conic).GetOriginBasedVersion();
            var curve = new Curve(newConic, newConic.GetPieTriangle(Type, StartAngle, EndAngle), Type);
            if(curve.attachedCurve != null)
            {
                attachedCurve = (ICurve)curve.attachedCurve.GetOriginBasedVersion();
                Extra = GetClosingLines();
            }
            return curve;
        }
        #endregion

        public override string ToString()
        {
            return string.Format(toStr, "Curve", Conic.Cx - Conic.Rx, Conic.Cy - Conic.Ry, Rx * 2, Ry * 2);
        }
    }
    #endregion
}
#endif
