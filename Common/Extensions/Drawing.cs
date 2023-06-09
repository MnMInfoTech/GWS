using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#if NoObjectLimit
using gint = System.Int32;
#else
using gint = System.UInt16;
#endif
#if NoFrameLimit
using fint = System.Int32;
#else
using fint = System.UInt16;
#endif

namespace MnM.GWS
{
    public static partial class Drawing
    {
        #region CONSTS
        internal const bool Initialize = true;
        internal const float START_EPSILON = Lines.START_EPSILON;
        internal const float END_EPSILON = Lines.END_EPSILON;

        internal const int Big = Vectors.Big;
        internal const int BigExp = Vectors.BigExp;

        internal const float HALF = 0.5F;
        internal const byte ZERO = 0;
        internal const byte ONE = 1;
        internal const byte TWO = 2;
        internal const byte MAX = 255;

        internal const int NOCOLOR = 0;

        internal const uint AMASK = Colours.AMASK;
        internal const uint RBMASK = Colours.RBMASK;
        internal const uint GMASK = Colours.GMASK;
        internal const uint AGMASK = AMASK | GMASK;
        internal const uint ONEALPHA = Colours.ONEALPHA;
        internal const int Inversion = Colours.Inversion;
        internal const uint AlphaRemoval = Colours.AlphaRemoval;

        internal const gint OBJEMPTY = default(gint);
        internal const fint CHLDEMPTY = default(fint);
        #endregion

        #region PROCESS AXIS POINTS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="points">Collection of scan points to process.
        /// <param name="processor"></param>
        /// <param name="dstW">Width of the operation area.</param>
        /// <param name="dstH">Height of the operation area.</param>
        /// <param name="AbsBoundary">Absolute boundary co-ordinates to be represent affected area.</param>
        /// <param name="Session"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Process(this IEnumerable<IScanPoint> points, Process<IAxisPointProcessArgs, RenderProcessResult> processor,
            int dstW, int dstH, ref int[] AbsBoundary, ISession Session, int universalOffsetX = 0, int universalOffsetY = 0)
        {
            var session = (IExSession)Session;
            IOffset Offset = null;
            IPoint UserPoint = null;
            LineCommand lineType = 0;
            sbyte Thickness = 0;
            IPointF center = null;
            int originX = 0, originY = 0;

            if (session != null)
            {
                Offset = session.Offset;
                UserPoint = session.UserPoint;
                lineType = session.Command.ToEnum<LineCommand>();
                Thickness = session.Thickness;
                center = session.RenderBounds?.Center();
                if ((session.Command & Command.OriginBased) == Command.OriginBased && session.RenderBounds != null)
                {
                    originX = session.RenderBounds.X;
                    originY = session.RenderBounds.Y;
                }
            }

            #region MAKE SURE BOUNDARIES ARE VALID
            if (AbsBoundary == null)
            {
                AbsBoundary = new int[4];
                AbsBoundary[0] = AbsBoundary[1] = int.MaxValue;
            }
            #endregion

            var arg = new AxisPointProcessArgs();

            #region LOCATION VARAIBLES
            var offX = Offset?.X ?? 0;
            var offY = Offset?.Y ?? 0;
            var userX = UserPoint?.X ?? 0;
            var userY = UserPoint?.Y ?? 0;
            #endregion

            #region LINE VARIABLES
            var LineCap = (lineType & LineCommand.LineCap) == LineCommand.LineCap;
            var Dot = (lineType & LineCommand.DottedLine) == LineCommand.DottedLine;
            var Dash = (lineType & LineCommand.DashedLine) == LineCommand.DashedLine;
            var DashDotDot = (lineType & LineCommand.DashDotDashLine) == LineCommand.DashDotDashLine;
            var Abnormal = Dot || Dash || DashDotDot;
            var EndOfLoop = (lineType & LineCommand.DrawEndPixel) == LineCommand.DrawEndPixel;
            var Bresenham = (lineType & LineCommand.Bresenham) == LineCommand.Bresenham;

            ILineSegment line = null;
            IAxisPoint[] pts = null;
            IAxisPoint pt = null;
            IScanPoint previous = null;
            IScanPoint first = null;
            bool IsLine = false;
            int ival = 0, nextPos = 0;
            float alfa = 0, alf = 0, delta;
            float initialValue = 0, m = 0, c = 0;
            int im = 0;
            int Start = 0, End = 0, step = 1;
            bool NextDue = false;

            float x1 = 0, y1 = 0, x2 = float.MaxValue, y2 = float.MaxValue;
            float px1 = 0, py1 =0, px2 = 0, py2 = 0, qx2 = 0, qy2 = 0;
            int thickStep = 1;
            var thickness = Thickness;
            if (thickness < 0)
            {
                thickness = (sbyte)(-thickness);
            }
            sbyte thick = thickness;
            bool HasThickness = thick != 0;
            bool horizontal = false;
            int cursor = 0;
            int Count;
            bool bresenham = false;
            bool CarryForward = false;
            bool DrawingThickLine = false;
            PointKind kind = 0;
            int oVal = 0, iival = 0;
            float cx = center?.X ?? 0, cy = center?.Y ?? 0; 
            int Exp = 0;

            bool lineCap, dot, dash, dashDotDot, abnormal;
            int endOfLoop;
            SlopeType oldSteep = 0, steep = 0;
            #endregion

            #region PREPARE COLLECTION TO LOOP
            var data = points;
            first = data.FirstOrDefault();
            if (first is IPointType)
            {
                var k = ((IPointType)first).Kind;
                if ((k & PointKind.Break) != PointKind.Break)
                    data = points.AppendItem(first);
            }
            #endregion

            fixed (int* absBoundary = AbsBoundary)
            {
                #region LINES LOOP
                foreach (var item in data)
                {
                    if (item == null)
                        continue;

                    #region RESET VARS
                    cursor = 0;
                    pts = null;
                    pt = null;
                    kind = 0;
                    bresenham = IsLine = false;
                    Exp = Vectors.BigExp;
                    lineCap = LineCap;
                    dot = Dot;
                    dash = Dash;
                    dashDotDot = DashDotDot;
                    abnormal = Abnormal;
                    bresenham = Bresenham;
                    endOfLoop = 1;
                    #endregion

                    #region ITEM IS ILINE
                    if (item is ILineSegment)
                    {
                        line = (ILineSegment)item;
                        var lt = line is ILineCommand ? ((ILineCommand)line).LineCommand : 0;
                        x1 = px1 = line.X1;
                        y1 = py1 = line.Y1;
                        qx2 = line.X2;
                        qy2 = line.Y2;
                        IsLine = true;
                        lineCap = (lt & LineCommand.LineCap) == LineCommand.LineCap;
                        dot = (lt & LineCommand.DottedLine) == LineCommand.DottedLine;
                        dash = (lt & LineCommand.DashedLine) == LineCommand.DashedLine;
                        dashDotDot = (lt & LineCommand.DashDotDashLine) == LineCommand.DashDotDashLine;
                        abnormal = Dot || Dash || DashDotDot;
                        bresenham = (lt & LineCommand.Bresenham) == LineCommand.Bresenham || Bresenham;
                        endOfLoop = steep != oldSteep || (lt & LineCommand.DrawEndPixel) == LineCommand.DrawEndPixel || EndOfLoop ? 0 : 1;
                        goto GET_LINE_COORDINATES;
                    }
                    #endregion

                    #region ITEM IS POINT
                    if (item is IPointF)
                    {
                        var v = (IPointF)item;
                        kind = item is IPointType ? ((IPointType)item).Kind : 0;
                        if ((kind & PointKind.Break) != PointKind.Break)
                        {
                            if (previous == null)
                            {
                                previous = item;
                                continue;
                            }
                            var pf = new VectorF(item);
                            x1 = px1 = pf.X;
                            y1 = py1 = pf.Y;
                            qx2 = v.X;
                            qy2 = v.Y;
                            previous = item;
                            if (first == null)
                                first = item;
                            IsLine = true;
                            if ((kind & PointKind.Bresenham) == PointKind.Bresenham)
                                bresenham = true;
                            goto GET_LINE_COORDINATES;
                        }

                        previous = null;
                        first = null;
                        pts = v.ToAxisPoints(bresenham);
                        Count = pts.Length;
                        NextDue = true;
                        goto DRAW_PIXEL;
                    }
                    #endregion

                    #region ITEM IS AXIS POINT
                    if (item is IAxisPoint)
                    {
                        previous = null;
                        first = null;
                        Count = 1;
                        pt = (IAxisPoint)item;
                        NextDue = true;
                        goto DRAW_PIXEL;
                    }
                    #endregion

                    #region ITEM IS IENUMERABLE<IAXIS POINT>
                    if (item is IEnumerable<IAxisPoint>)
                    {
                        previous = null;
                        first = null;
                        pts = ((IEnumerable<IAxisPoint>)item).ToArray();
                        Count = pts.Length;
                        NextDue = true;
                        goto DRAW_PIXEL;
                    }
                    #endregion

                    #region ITEM IS END POINTS
                    if (item is IEndPoints)
                    {
                        previous = null;
                        first = null;
                        pts = ((IEndPoints)item).Points.ToArray();
                        Count = pts.Length;
                        NextDue = true;
                        goto DRAW_PIXEL;
                    }
                    #endregion

                    GET_LINE_COORDINATES:
                    #region INTIALIZE X1, Y1, X2, Y2 WITH LINE CONTINUITY CORRECTION
                    if (CarryForward || x2 == x1 && y2 == y1)
                    {
                        x1 = CarryForward ? px2 : horizontal ? ival + alfa : Start;
                        y1 = CarryForward ? py2 : horizontal ? Start : ival + alfa;
                        CarryForward = false;
                    }

                    x2 = qx2;
                    y2 = qy2;
                    #endregion

                    #region CALCULATE LINE VITALS I.E M, C ETC. 
                    step = ONE;
                    var dx = x2 - x1;
                    var dy = y2 - y1;

                    if ((dx * dx + dy * dy) < 2f)
                    {
                        px2 = x1;
                        py2 = y1;
                        CarryForward = true;
                        continue;
                    }
                    if (bresenham)
                    {
                        im = Vectors.Slope(out horizontal, x1, y1, x2, y2);
                        if (im == 0)
                            Exp = 0;
                        goto CHECK_THICKNESS;
                    }
                    var pdy = dy;
                    var pdx = dx;
                    if (pdy < 0)
                        pdy = -pdy;
                    if (pdx < 0)
                        pdx = -pdx;
                    steep = (pdy > pdx) ? SlopeType.Steep : SlopeType.NonSteep;
                    horizontal = steep == SlopeType.Steep;

                    m = dy;
                    if (dx != 0)
                        m /= dx;
                    c = y1 - m * x1;

                    bresenham = bresenham || pdx == 0 || pdy == 0;
                    if (bresenham)
                    {
                        im = Vectors.Slope(out horizontal, x1, y1, x2, y2);
                        if (im == 0)
                            Exp = 0;
                    }
                    #region DETERMINE THICKNESS DIRECTION
                    CHECK_THICKNESS:
                    if (HasThickness)
                    {
                        float middle;
                        if (horizontal)
                        {
                            if (x2 > x1)
                                middle = x1 + (x2 - x1) / 2;
                            else
                                middle = x2 + (x1 - x2) / 2;
                            if (middle > cx)
                                thickStep = -1;
                        }
                        else
                        {
                            if (y2 > y1)
                                middle = y1 + (y2 - y1) / 2;
                            else
                                middle = y2 + (y1 - y2) / 2;
                            if (middle > cy)
                                thickStep = -1;
                        }
                    }
                    #endregion

                    #region INTEGER LINE VITALS
                    if (bresenham)
                    {
                        if (horizontal)
                        {
                            Start = (int)y1;
                            End = (int)y2;
                            Count = End - Start;

                            if (y1 > y2)
                            {
                                step = -1;
                                Count = -Count;
                            }
                            ival = x1.Round();
                        }
                        else
                        {
                            Start = (int)x1;
                            End = (int)x2;
                            Count = End - Start;
                            if (x1 > x2)
                            {
                                step = -1;
                                Count = -Count;
                            }
                            ival = y1.Round();
                        }
                        oVal = ival;
                        iival = ival << Exp;
                        NextDue = false;
                        cursor = 0;
                        goto DRAW_PIXEL;
                    }
                    #endregion

                    #region FLOAT LINE VITALS
                    float start = horizontal ? y1 : x1;
                    float end = horizontal ? y2 : x2;

                    if (horizontal)
                        m = ONE / m;
                    var tc = horizontal ? -c * m : c;
                    bool negative = start > end;

                    Start = start.Round();
                    End = end.Round();

                    initialValue = Start * m + tc;
                    Count = (End - Start);

                    if (negative)
                    {
                        m = -m;
                        step = -step;
                        Count = -Count;
                    }
                    #endregion
                    #endregion

                    #region PREPARE FOR LOOPING
                    NextDue = true;
                    ival = (int)initialValue;
                    #endregion

                    DRAW_PIXEL:
                    #region DRAW PIXEL
                    if (Count < endOfLoop)
                    {
                        oldSteep = steep;
                        continue;
                    }
                    thick = thickness;
                    var iDelta = MAX;

                    if (IsLine)
                        goto DETERMINE_ALPHA;

                    if (pts != null)
                        pt = pts[Count - 1];
                    #endregion

                    #region HANDLE STAND ALONE POINT
                    initialValue = pt.Val;
                    ival = (int)initialValue;
                    Start = pt.Axis;
                    horizontal = pt.IsHorizontal;
                    #endregion

                    DETERMINE_ALPHA:
                    #region DETERMINE ALPHA
                    if (bresenham)
                        goto GET_X_Y;

                    alfa = initialValue - ival;
                    #endregion

                    CALCULATE_ALPHA:
                    #region CALCULATE ALPHA 
                    alf = alfa;
                    if (alf == ZERO || bresenham)
                    {
                        NextDue = false;
                        alf = 1F;
                        goto GET_X_Y;
                    }
                    nextPos = NextDue ? ZERO : ONE;
                    if (alf < ZERO)
                    {
                        nextPos = -nextPos;
                        alf = -alf;
                    }
                    if (NextDue)
                        alf = ONE - alf;

                    ival += nextPos;

                    delta = (alf) * MAX;
                    iDelta = (byte)delta.Round();
                    #endregion

                    GET_X_Y:
                    #region GET X, Y
                    var dstX = horizontal ? ival : Start;
                    var dstY = horizontal ? Start : ival;
                    var x = dstX;
                    var y = dstY;

                    dstX += (offX + userX - originX + universalOffsetX);
                    dstY += (offY + userY - originY + universalOffsetY);

                    if (dstX < absBoundary[0])
                        absBoundary[0] = dstX;
                    if (dstY < absBoundary[1])
                        absBoundary[1] = dstY;

                    if (dstX > absBoundary[2])
                        absBoundary[2] = dstX;
                    if (dstY > absBoundary[3])
                        absBoundary[3] = dstY;
                    #endregion

                    #region CLIPPING
                    if (dstX < 0 || dstY < 0 || dstX >= dstW || dstY >= dstH || iDelta < TWO)
                    {
                        goto DRAW_NEXT_PIXEL;
                    }
                    #endregion

                    #region LOOP
                    arg.DstX = dstX;
                    arg.DstY = dstY;
                    arg.Alpha = iDelta;
                    arg.Horizontal = horizontal;
                    arg.PenX = x;
                    arg.PenY = y;

                    switch (processor(arg))
                    {
                        case RenderProcessResult.Sucess:
                        default:
                            break;
                        case RenderProcessResult.MoveNextPixel:
                           goto DRAW_NEXT_PIXEL;
                        case RenderProcessResult.MoveNextLine:
                            continue;
                        case RenderProcessResult.ExitLoop:
                            goto EXIT_LOOP;
                    }
                    #endregion

                    #region HANDLE LINE THICKNESS
                    if (HasThickness)
                    {
                        if (thick-- > 0)
                        {
                            DrawingThickLine = true;
                            if (IsLine || horizontal)
                                ival += thickStep;
                            else
                                Start += thickStep;
                            goto GET_X_Y;
                        }
                        if (DrawingThickLine)
                        {
                            DrawingThickLine = false;
                            if (NextDue)
                                NextDue = false;
                            goto CALCULATE_ALPHA;
                        }
                        if (bresenham)
                            ival = oVal;
                    }
                    #endregion

                    DRAW_NEXT_PIXEL:
                    #region NEXT PIXEL
                    if (NextDue)
                    {
                        NextDue = false;
                        goto DRAW_PIXEL;
                    }
                    #endregion

                    #region NEXT IN LINE
                    --Count;
                    if (!IsLine)
                        goto DRAW_PIXEL;

                    Start += step;
                    if (!bresenham)
                    {
                        initialValue += m;

                        if (abnormal)
                        {
                            if (dot)
                            {
                                if (cursor == 0)
                                {
                                    cursor = 1;
                                    initialValue += m;
                                    Start += step;
                                    --Count;
                                }
                                else
                                    cursor = 0;
                            }
                            else if (dash)
                            {
                                if (cursor > 1)
                                {
                                    cursor = -1;
                                    initialValue += m;
                                    Start += step;
                                    --Count;
                                }
                                ++cursor;
                            }
                            else if (dashDotDot)
                            {
                                if (cursor == 2 || cursor > 3)
                                {
                                    initialValue += m;
                                    Start += step;
                                    --Count;
                                }
                                if (cursor > 3)
                                    cursor = -1;
                                ++cursor;
                            }
                        }

                        NextDue = true;
                        ival = (int)initialValue;
                        alfa = initialValue - ival;
                    }
                    else
                    {
                        if (abnormal)
                        {
                            if (dot)
                            {
                                if (cursor == 0)
                                {
                                    cursor = 1;
                                    Start += step;
                                    --Count;
                                }
                                else
                                    cursor = 0;
                            }
                            else if (dash)
                            {
                                if (cursor > 1)
                                {
                                    cursor = -1;
                                    Start += step;
                                    --Count;
                                }
                                ++cursor;
                            }
                            else if (dashDotDot)
                            {
                                if (cursor == 2 || cursor > 3)
                                {
                                    Start += step;
                                    --Count;
                                }
                                if (cursor > 3)
                                    cursor = -1;
                                ++cursor;
                            }
                        }
                        iival += im;
                        oVal = iival >> Exp;
                        ival = oVal;
                    }
                    goto DRAW_PIXEL;
                    #endregion
                }
                #endregion

                EXIT_LOOP:
                goto EXIT;
            }
            EXIT:
            return;
        }
        #endregion

        #region PROCESS AXIS LINES
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines">Collection of scan lines to process.
        /// <param name="processor"></param>
        /// <param name="dstW">Width of the operation area.</param>
        /// <param name="dstH">Height of the operation area.</param>
        /// <param name="AbsBoundary">Absolute boundary co-ordinates to be represent affected area.</param>
        /// <param name="Session"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Process(this IEnumerable<IScanLine> lines, Process<IAxisLineProcessArgs, RenderProcessResult> processor,
            int dstW, int dstH, ref int[] AbsBoundary, ISession Session, int universalOffsetX = 0, int universalOffsetY = 0)
        {
            var session = (IExSession)Session;
            IOffset Offset = null;
            IPoint UserPoint = null;
            bool XOR = false;
            int originX = 0, originY = 0;
            bool Register = false;

            if (session != null)
            {
                Offset = session.Offset;
                UserPoint = session.UserPoint;
                XOR = (session.Command & Command.XORFill) == Command.XORFill;
                Register = (session.Command & Command.RegisterContainer) == Command.RegisterContainer;
                if ((session.Command & Command.OriginBased) == Command.OriginBased && session.RenderBounds != null)
                {
                    originX = session.RenderBounds.X;
                    originY = session.RenderBounds.Y;
                }
            }
            #region MAKE SURE BOUNDARIES ARE VALID
            if (AbsBoundary == null)
            {
                AbsBoundary = new int[4];
                AbsBoundary[0] = AbsBoundary[1] = int.MaxValue;
            }
            #endregion

            var arg = new AxisLineProcessArgs();

            #region LOCATION VARAIBLES
            int copyLength = 0, dstX = 0, dstY = 0, dstEX = 0, dstEY = 0, x =0, y=0;
            float start, end;
            int Start, End;
            int Axis;
            bool horizontal;
            #endregion

            #region SCAN LINE HANDLING VARAIBLES
            float* data = null;
            int* iData = null;
            bool IsLineCollection = false;
            IAxisLine Line = null;
            int LineCount = 0;
            IAxisLine[] Lines = null;
            int LineCursor = 0;
            int scanOffX = 0, scanOffY = 0;
            var offX = Offset?.X ?? 0;
            var offY = Offset?.Y ?? 0;
            var userX = UserPoint?.X ?? 0;
            var userY = UserPoint?.Y ?? 0;
            #endregion

            fixed (int* absBoundary = AbsBoundary)
            {
                bool IntLine = false;
                bool FloatLine = false;

                #region LOOP
                foreach (var l in lines)
                {
                    if (l == null)
                        continue;

                    #region REST VARS
                    LineCursor = LineCount = 0;
                    Lines = null;
                    IsLineCollection = false;
                    Line = null;
                    scanOffX = scanOffY = 0;
                    #endregion

                    #region LINE IS AXISLINE
                    if (l is IAxisLine)
                    {
                        Line = (IAxisLine)l;
                        LineCount = 1;
                        LineCursor = 0;
                        Lines = null;
                        IsLineCollection = false;
                        goto PROCESS_AXIS_LINE;
                    }
                    #endregion

                    #region LINE IS IENUMERABLE<IAXISLINE>
                    if (l is IEnumerable<IAxisLine>)
                    {
                        Lines = ((IEnumerable<IAxisLine>)l).ToArray();
                        LineCount = Lines.Length;
                        if (LineCount == 0)
                            continue;
                        LineCursor = 0;
                        IsLineCollection = true;
                        Line = Lines[LineCursor++];
                        if (l is IOffsetHolder)
                        {
                            scanOffX = ((IOffsetHolder)l).Offset?.X ?? 0;
                            scanOffY = ((IOffsetHolder)l).Offset?.Y ?? 0;
                        }
                        goto PROCESS_AXIS_LINE;
                    }
                    #endregion

                    PROCESS_AXIS_LINE:
                    #region PROCESS AXIS LINE
                    IntLine = Line is IIndexer<int>;
                    arg.Alpha = (Line is IAlpha) ? ((IAlpha)Line).Alpha : MAX;
                    FloatLine = !IntLine && Line is IIndexer<float>;

                    if
                    (
                        Line.Count == 0 || !(IntLine || FloatLine) || arg.Alpha < TWO
                    )
                    {
                        goto NEXT_AXIS_LINE;
                    }
                    horizontal = Line.IsHorizontal;
                    bool EndsOnly = (Line.Draw & LineFill.EndsOnly) == LineFill.EndsOnly;
                    bool LinesOnly = (Line.Draw & LineFill.WithEnds) != LineFill.WithEnds;
                    int axisOffset = horizontal ? offY + scanOffY : offX + scanOffX;
                    int valOffset = horizontal ? offX + scanOffX : offY + scanOffY;
                    Axis = Line.Axis + axisOffset;
                    int li = XOR ? 2 : 1;
                    int increment = 1;
                    int count = Line.Count;
                    data = null;
                    iData = null;
                    copyLength = 0;
                    #endregion

                    #region HANDLE SINGLE LINE / POINT 
                    if (count > 2)
                        goto HANDLE_POLYLINE;

                    if (IntLine)
                        goto GET_INTLINE_START;

                    start = ((IIndexer<float>)Line)[0];
                    end = start;
                    Start = End = (int)start;
                    if (start - Start >= START_EPSILON)
                        ++Start;

                    goto GET_LINE_END;

                    GET_INTLINE_START:
                    Start = End = ((IIndexer<int>)Line)[0];

                    GET_LINE_END:
                    if (count == 1)
                        goto GET_DSTX_DSTY;

                    if (IntLine)
                        goto GET_INTLINE_END;

                    end = ((IIndexer<float>)Line)[1];
                    End = (int)end;
                    if (end - End >= END_EPSILON)
                        ++End;
                    goto GET_DSTX_DSTY;

                    GET_INTLINE_END:
                    End = ((IIndexer<int>)Line)[1];

                    GET_DSTX_DSTY:
                    #region GET DSTX DSTY              
                    Start += valOffset;
                    End += valOffset;
                    copyLength = End - Start;

                    dstX = horizontal ? Start : Axis;
                    dstY = horizontal ? Axis : Start;
                    x = dstX;
                    y = dstY;
                    dstX += (userX - originX + universalOffsetX);
                    dstY += (userY - originY + universalOffsetY);

                    dstEX = dstX;
                    dstEY = dstY;

                    dstEX += (horizontal ? copyLength : 0);
                    dstEY += (!horizontal ? copyLength : 0);
                    #endregion

                    #region UPDATE BOUNDARY
                    if (dstX < absBoundary[0])
                        absBoundary[0] = dstX;
                    if (dstY < absBoundary[1])
                        absBoundary[1] = dstY;

                    if (dstEX > absBoundary[2])
                        absBoundary[2] = dstEX;

                    if (dstEY > absBoundary[3])
                        absBoundary[3] = dstEY;

                    if (XOR)
                        continue;
                    #endregion

                    goto HANDLE_CLIPPING;
                    #endregion

                    HANDLE_POLYLINE:
                    #region HANDLE POLY LINE
                    if (!Register && count % 2 == 0)
                        increment = 2;

                    if (IntLine)
                        goto GET_INT_POLYLINE;

                    fixed (float* p = ((IIndexer<float>)Line).GetData())
                        data = p;

                    start = data[li - 1];
                    end = data[li];
                    goto LINE_LOOP;

                    GET_INT_POLYLINE:
                    fixed (int* p = ((IIndexer<int>)Line).GetData())
                        iData = p;

                    Start = iData[li - 1];
                    End = iData[li];

                    goto GET_POLY_DSTX_DSTY;
                    #endregion

                    LINE_LOOP:
                    #region GET INT START - END
                    Start = (int)start;
                    End = (int)end;

                    if (start - Start >= START_EPSILON)
                        ++Start;
                    if (end - End >= END_EPSILON)
                        ++End;
                    #endregion

                    GET_POLY_DSTX_DSTY:
                    #region GET DSTX DSTY              
                    Start += valOffset;
                    End += valOffset;
                    copyLength = End - Start;

                    dstX = horizontal ? Start : Axis;
                    dstY = horizontal ? Axis : Start;
                    x = dstX;
                    y = dstY;
                    dstX += (userX - originX + universalOffsetX);
                    dstY += (userY - originY + universalOffsetY);

                    dstEX = dstX;
                    dstEY = dstY;
                    dstEX += (horizontal ? copyLength : 0);
                    dstEY += (!horizontal ? copyLength : 0);
                    #endregion

                    #region UPDATE ABS BAUNDARY
                    if (dstX < absBoundary[0])
                        absBoundary[0] = dstX;
                    if (dstY < absBoundary[1])
                        absBoundary[1] = dstY;

                    dstEX = dstX + (horizontal ? copyLength : 0);
                    dstEY = dstY + (horizontal ? 0 : copyLength);

                    if (dstEX > absBoundary[2])
                        absBoundary[2] = dstEX;
                    if (dstEY > absBoundary[3])
                        absBoundary[3] = dstEY;
                    #endregion

                    HANDLE_CLIPPING:
                    #region HANDLE SIZE BASED CLIPPING
                    if (horizontal)
                    {
                        if (dstY < 0 || dstY >= dstH || dstX >= dstW || dstX + copyLength < 0)
                            goto NEXT_LINE;

                        if (dstX + copyLength > dstW)
                            copyLength = dstW - dstX - 1;

                        if (dstX < 0)
                        {
                            copyLength += dstX;
                            dstX = 0;
                        }
                    }
                    else
                    {
                        if (dstX < 0 || dstX >= dstW || dstY >= dstH || dstY + copyLength < 0)
                            goto NEXT_LINE;

                        if (dstY + copyLength > dstH)
                            copyLength = dstH - dstY - 1;
                        if (dstY < 0)
                        {
                            copyLength += dstY;
                            dstY = 0;
                        }
                    }
                    #endregion

                    if (copyLength < 0)
                        goto NEXT_LINE;

                    #region LOOP
                    arg.DstX = dstX;
                    arg.DstY = dstY;
                    arg.copyLength = copyLength;
                    arg.Horizontal = horizontal;
                    arg.PenStart = horizontal ? x : y;
                    arg.PenAxis = horizontal ? y : x;

                    switch (processor(arg))
                    {
                        case RenderProcessResult.Sucess:
                        case RenderProcessResult.MoveNextPixel:
                        default:
                            break;
                        case RenderProcessResult.MoveNextFragment:
                            goto NEXT_LINE;
                        case RenderProcessResult.MoveNextLine:
                            goto NEXT_AXIS_LINE;
                        case RenderProcessResult.ExitLoop:
                            goto EXIT_LOOP;
                    }
                    #endregion

                    NEXT_LINE:
                    #region NEXT LINE
                    li += increment;
                    if (li < count)
                    {
                        if (IntLine)
                        {
                            Start = iData[li - 1];
                            End = iData[li];
                            goto GET_POLY_DSTX_DSTY;
                        }
                        start = data[li - 1];
                        end = data[li];
                        goto LINE_LOOP;
                    }
                    #endregion

                    NEXT_AXIS_LINE:
                    #region NEXT AXIS LINE
                    if (!IsLineCollection || LineCursor >= LineCount)
                        continue;
                    Line = Lines[LineCursor++];
                    goto PROCESS_AXIS_LINE;
                    #endregion
                }
                #endregion
                EXIT_LOOP:
                goto EXIT;
            }
            EXIT:
            return;
        }

        public static unsafe void Process(this IEnumerable<IScanLine> lines, Process<IAxisLineProcessArgs, RenderProcessResult> processor,
            out int[] AbsBoundary, int dstW, int dstH, ISession Session, int universalOffsetX = 0, int universalOffsetY = 0)
        {
            #region MAKE SURE BOUNDARIES ARE VALID
            AbsBoundary = new int[4];
            AbsBoundary[0] = AbsBoundary[1] = int.MaxValue;
            #endregion

            Process(lines, processor, dstW, dstH, ref AbsBoundary, Session, universalOffsetX, universalOffsetY);
        }
        #endregion

        #region PROCESS IMAGE DATA
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Process(this Process<IImageProcessArgs, IBounds> imageProcessor, IntPtr source, int srcW, int srcH, 
            int dstW, int dstH, ref int[] AbsBoundary, ISession Session, int universalOffsetX = 0, int universalOffsetY = 0)
        {

            if (source == IntPtr.Zero || imageProcessor == null)
                return;
            var session = (IExSession)Session;

            IOffset Offset = null;
            IPoint UserPoint = null;
            bool XOR = false;
            IBounds copyArea = null;
                        int originX = 0, originY = 0;

            if (session != null)
            {
                Offset = session.Offset;
                UserPoint = session.UserPoint;
                XOR = (session.Command & Command.XORFill) == Command.XORFill;
                copyArea = session.CopyArea;
                if ((session.Command & Command.OriginBased) == Command.OriginBased && session.RenderBounds != null)
                {
                    originX = session.RenderBounds.X;
                    originY = session.RenderBounds.Y;
                }
            }

            #region MAKE SURE BOUNDARIES ARE VALID
            if (AbsBoundary == null)
            {
                AbsBoundary = new int[4];
                AbsBoundary[0] = AbsBoundary[1] = int.MaxValue;
            }
            #endregion

            var arg = new ImageProcessArgs();

            #region LOCATION VARAIBLES
            var offX = Offset?.X ?? 0;
            var offY = Offset?.Y ?? 0;
            var userX = UserPoint?.X ?? 0;
            var userY = UserPoint?.Y ?? 0;
            #endregion

            arg.ImageSource = source;
            arg.ImageWidth = srcW;
            arg.ImageHeight = srcH;
            arg.DstX = userX + offX - originX + universalOffsetX;
            arg.DstY = userY + offY- originY + universalOffsetY;
            arg.ImageCopyArea = copyArea;
            var bounds = imageProcessor(arg);

            if (bounds != null)
            {
                bounds.GetBounds(out int x, out int y, out int w, out int h);
                fixed (int* absBoundary = AbsBoundary)
                {
                    if (x < absBoundary[0])
                        absBoundary[0] = x;
                    if (y < absBoundary[1])
                        absBoundary[1] = y;
                    if (x + w > absBoundary[2])
                        absBoundary[2] = x + w;
                    if (y + h > absBoundary[3])
                        absBoundary[3] = y + h;
                }
            }
        }
        #endregion
    }
}
