/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
#if (GWS || Window)
    public static partial class Renderer
    {
        #region INSTANCE VARIABLE
        internal const bool Initialize = true;
        #endregion

        #region GET PEN
        /// <summary>
        /// Gets an existing pen or creates one matching the size of the shape which is to be rendered.
        /// </summary>
        /// <param name="shape">An element for which buffer pen is required</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <returns>Buffer pen</returns>
        public static IPen GetPen(this IWritable buffer, IRenderable shape, IReadContext context, RectangleF? suppliedBounds = null)
        {
            bool enabled = true;
            IPen Pen = null;
            var Settings = buffer.Settings;

            if (shape is IVisible2)
                enabled = (shape as IVisible2).Enabled;

            if (!enabled)
            {
                Pen = Pens.DisabledPen;
                return Pen;
            }
            var rc = suppliedBounds ?? shape.Bounds;
            Settings.Bounds = rc.Expand();

            if (Settings.Clip)
                Settings.Bounds = Settings.Bounds.Clamp(Settings.Clip);
            else
                Settings.Bounds = Settings.Bounds.Clamp(Vectors.UHD8kWidth, Vectors.UHD8kHeight);

            var w = Settings.Bounds.Width + 1;
            var h = Settings.Bounds.Height + 1;

            IReadContext penContext = context ??
                (buffer as IForeground)?.Foreground ?? BrushStyle.Black;

            Pen = penContext.ToPen(w, h);

            if (shape is IRotatable)
                Settings.Rotation = (shape as IRotatable).Rotation;
            Settings.PenID = Pen.ID;
            (Pen as ISettings)?.CopySettings(Settings);
            (buffer as ISettings)?.CopySettings(Settings);
            return Pen;
        }
        #endregion

        #region FILL
        /// <summary>
        /// Fills the area covered by specified lines.
        /// </summary>
        /// <param name="lines">Lines which forms the perimeter of a shape</param>
        /// <param name="action">A FillAction delegate which has routine to do something with the information emerges by using breshenham line algorithm</param>
        /// <param name="y">Far top position to consider for filling</param>
        /// <param name="bottom">Far bottom position to consider for filling</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fill(this IEnumerable<ILine> lines, FillAction<float> action, int y, int bottom, FillCommand command, LineCommand lineCommand)
        {
            using (var polyFill = Factory.newPolyFill())
            {
                polyFill.Begin(y, bottom, command | FillCommand.OddEvenPolyFill);
                polyFill.Scan(lines);
                polyFill.Fill(action, lineCommand);
            }
        }
        #endregion

        #region PROCESS LINE
        /// <summary>
        /// Processes a line using standard line algorithm between two points specified by x1, y1 and x2, y2 of a line segment using specified action.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="action">A FillAction delegate which has routine to do something with the information emerges by using standard line algorithm</param>
        /// <param name="skip">LineSkip option used to filter the lines so that shallow and steep gradients can be processed seperately.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessLine(int x1, int y1, int x2, int y2, PixelAction<int> action, LineCommand command, SlopeType skip = SlopeType.None)
        {
            if (skip == SlopeType.Both)
                return;
            var type = Lines.Type(x1, y1, x2, y2, out SlopeType nskip);
            if (type == LineType.Point || nskip == skip)
                return;
            if (!Lines.DrawParams(x1, y1, x2, y2, out bool horizontalScan, out int m,
                out int step, out int min, out int max, out int value))
                return;
            var Draw = command;
            Draw &= ~LineCommand.Breshenham;
            Draw &= ~LineCommand.Distinct;
            int i = 0;

            while (min != max)
            {
                var val = value >> Vectors.BigExp;
                switch (Draw)
                {
                    case LineCommand.Dot:
                        if (i == 0)
                        {
                            action(val, min, horizontalScan);
                            i = 1;
                        }
                        else
                            i = 0;
                        break;
                    case LineCommand.Dash:
                        if (i < 2)
                        {
                            action(val, min, horizontalScan);
                            ++i;
                        }
                        else if (i == 3)
                            i = 0;
                        else
                            ++i;
                        break;
                    case LineCommand.DashDotDash:
                        if (i < 2)
                        {
                            action(val, min, horizontalScan);
                            ++i;
                        }
                        else if (i == 3)
                        {
                            action(val, min, horizontalScan);
                            i = 0;
                        }
                        else
                            ++i;
                        break;
                    default:
                        action(val, min, horizontalScan);
                        break;
                }
                value += m;
                min += step;
            }
            action(value >> Vectors.BigExp, min, horizontalScan);
        }

        /// <summary>
        /// Processes a line using speedier GWS line algorithm between two points specified by x1, y1 and x2, y2 using specified action.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="action">A FillAction delegate which has routine to do something with the information emerges by using GWS line algorithm</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessLine(float x1, float y1, float x2, float y2, PixelAction<float> action, LineCommand command, SlopeType skip = SlopeType.None)
        {
            if (skip == SlopeType.Both)
                return;

            var type = Lines.Type(x1, y1, x2, y2, out SlopeType nskip);
            if (type == LineType.Point || nskip == skip)
                return;
            var horizontalScan = nskip == SlopeType.Steep ? true : false;

            if (type == LineType.Horizontal || type == LineType.Vertical)
            {
                ProcessLine(x1.Ceiling(), y1.Ceiling(), x2.Ceiling(), y2.Ceiling(), action.ToIntPixelAction(), command);
                return;
            }

            if (!Lines.DrawParams(x1, y1, x2, y2, horizontalScan, false, out float m,
                out int step, out int min, out int max, out float val, out _))
                return;
            var Draw = command;
            Draw &= ~LineCommand.Breshenham;
            Draw &= ~LineCommand.Distinct;

            int i = 0;
            while (min != max)
            {
                switch (Draw)
                {
                    case LineCommand.Dot:
                        if (i == 0)
                        {
                            action(val, min, horizontalScan);
                            i = 1;
                        }
                        else
                            i = 0;
                        break;
                    case LineCommand.Dash:
                        if (i < 2)
                        {
                            action(val, min, horizontalScan);
                            ++i;
                        }
                        else if (i == 3)
                            i = 0;
                        else
                            ++i;
                        break;
                    case LineCommand.DashDotDash:
                        if (i < 2)
                        {
                            action(val, min, horizontalScan);
                            ++i;
                        }
                        else if (i == 3)
                        {
                            action(val, min, horizontalScan);
                            i = 0;
                        }
                        else
                            ++i;
                        break;
                    default:
                        action(val, min, horizontalScan);
                        break;
                }
                val += m;
                min += step;
            }
            action(val, min, horizontalScan);
        }
        #endregion

        #region SCAN LINE
        /// <summary>
        /// Scans a line using standard line algorithm between two points of a line segment using specified action.
        /// Line can be scanned horizontal y or vertically. 
        /// If scanned horizontally, then scanning is done from y1 to y2 taking rounded values of both,
        /// else scanning is done from x1 to x2 taking rounded values of both.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="horizontalScan">If null, steepness of line would be scan mode, else if true, line should be scanned from top to bottom otherwise left to right</param>
        /// <param name="action">A FillAction delegate which has routine to do something with the information emerges by using standard line algorithm</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ScanLine(float x1, float y1, float x2, float y2, bool? steep, PixelAction<float> action)
        {
            var type = Lines.Type(x1, y1, x2, y2, out SlopeType slopType);
            var originalSteep = slopType == SlopeType.Steep ? true : false;
            var horizontalScan = steep ?? originalSteep;

            if (type == LineType.Point || type == LineType.Horizontal && horizontalScan || type == LineType.Vertical && !horizontalScan)
                return;

            if (!Lines.DrawParams(x1, y1, x2, y2, horizontalScan, true, out float m,
                out int step, out int min, out int max, out float val, out bool negative))
                return;

            while (min != max)
            {
                if (negative)
                {
                    val += m;
                    min += step;
                }
                action(val, min, horizontalScan);

                if (!negative)
                {
                    val += m;
                    min += step;
                }
            }
        }
        #endregion

        #region STROKING
        /// <summary>
        /// Returns inner and outer perimeter resulted due to stroking of specified points.
        /// </summary>
        /// <param name="Points">Points to stroke.</param>
        /// <param name="shapeType">Type of shape the points belong to.</param>
        /// <param name="stroke">Amount of stroke.</param>
        /// <param name="mode">Stroke mode to apply.</param>
        /// <param name="outerP">Outer perimeter returned.</param>
        /// <param name="innerP">Inner perimeter returned.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StrokePoints(IEnumerable<VectorF> Points, string shapeType, float stroke, StrokeMode mode,
            out IList<VectorF> outerP, out IList<VectorF> innerP)
        {
            var afterStroke = Factory.ShapeParser.GetAfterStroke(shapeType);
            var join = Factory.ShapeParser.GetStrokeJoin(shapeType);
            var points = Points.Clean(join);

            bool reset1st = afterStroke.HasFlag(AfterStroke.Reset1st);

            if (mode == StrokeMode.StrokeMiddle)
            {
                var half = (stroke / 2f);
                outerP = points.StrokePoints2(half, join, reset1st);
                innerP = points.StrokePoints2(-half, join, reset1st);
                var i = innerP.ToArea();
                var o = outerP.ToArea();
                if (!o.Contains(i))
                    Numbers.Swap(ref outerP, ref innerP);
            }
            else if (mode == StrokeMode.StrokeInner)
            {
                innerP = points.StrokePoints2(-stroke, join, reset1st);
                outerP = points;
                var i = innerP.ToArea();
                var o = outerP.ToArea();
                if (!o.Contains(i))
                    innerP = points.StrokePoints2(stroke, join, reset1st);
            }
            else
            {
                outerP = points.StrokePoints2(stroke, join, reset1st);
                innerP = points;
                var i = innerP.ToArea();
                var o = outerP.ToArea();
                if (!o.Contains(i))
                    outerP = points.StrokePoints2(-stroke, join, reset1st);
            }
        }
        #endregion

        #region GET LINES
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetLines(string ShapeName, IList<VectorF> Outer, IList<VectorF> Inner, float stroke, out IList<ILine> outerLines, out IList<ILine> innerLines,
            bool avoidTooClose = false, bool dontCloseEnds = false)
        {
            var join = PointJoin.ConnectEach | Factory.ShapeParser.GetStrokeJoin(ShapeName);
            if (avoidTooClose)
                join |= PointJoin.AvoidTooClose;

            if (stroke == 0)
            {
                outerLines = Outer.ToLines(join);
                innerLines = null;
                return;
            }
            bool close = Factory.ShapeParser.GetAfterStroke(ShapeName).HasFlag(AfterStroke.JoinEnds);

            outerLines = Outer.ToLines(join);
            innerLines = Inner.ToLines(join);

            if (close && !dontCloseEnds)
            {
                var l1 = outerLines[0];
                var l2 = innerLines[0];

                var close1 = new Line(l1.X1, l1.Y1, l2.X1, l2.Y1);
                l1 = outerLines.Last();
                l2 = innerLines.Last();
                var close2 = new Line(l1.X2, l1.Y2, l2.X2, l2.Y2);
                innerLines.Add(close1);
                outerLines.Add(close2);
            }
            if (Factory.ShapeParser.NoeedToSwapPerimeters(ShapeName))
                Numbers.Swap(ref outerLines, ref innerLines);
        }
        #endregion

        #region GET DRAW PARAMS
        /// <summary>
        /// Gets array of collection of lines accroding to crrent stroke settings for the given original points.
        /// </summary>
        /// <param name="fillMode"></param>
        /// <param name="fillCommand"></param>
        /// <param name="Stroke"></param>
        /// <param name="ShapeName"></param>
        /// <param name="Original"></param>
        /// <param name="Outer"></param>
        /// <param name="Inner"></param>
        /// <returns></returns>
        public static IList<ILine>[] GetDrawParams(FillMode fillMode, FillCommand fillCommand, float Stroke, string ShapeName,
            IEnumerable<VectorF> Original, IList<VectorF> Outer, IList<VectorF> Inner)
        {
            var mode = fillMode;
            IList<ILine>[] Data = new IList<ILine>[4];
            float stroke = mode == FillMode.Original ? 0 : Stroke;

            IList<ILine> outer, inner, outer1, inner1;
            outer1 = inner1 = null;

            var avoidClosingEnds = (mode == FillMode.Outer || mode == FillMode.Inner);
            avoidClosingEnds = avoidClosingEnds && (ShapeName == "Bezier" || ShapeName == "Arc");
            if (stroke == 0)
            {
                var join = PointJoin.ConnectEach | Factory.ShapeParser.GetStrokeJoin(ShapeName);

                outer = Original.ToLines(join);
                inner = null;

                if (Factory.ShapeParser.DontJoinPointsIfTooClose(ShapeName))
                {
                    join |= PointJoin.AvoidTooClose;
                    outer1 = Original.ToLines(join);
                    inner1 = null;
                }
            }
            else
            {
                GetLines(ShapeName, Outer, Inner, stroke, out outer, out inner, false, avoidClosingEnds);
                if (Factory.ShapeParser.DontJoinPointsIfTooClose(ShapeName))
                    GetLines(ShapeName, Outer, Inner, stroke, out outer1, out inner1, true, avoidClosingEnds);
            }

            if (Stroke == 0 && mode == FillMode.Inner)
                mode = FillMode.Original;

            switch (mode)
            {
                case FillMode.Original:
                default:
                    Data[0] = (outer1 ?? outer);
                    switch (ShapeName)
                    {
                        case "Bezier":
                        case "Arc":
                        case "BezierArc":
                        case "ClosedArc":
                            return Data;
                        default:
                            break;
                    }
                    Data[1] = outer;
                    break;

                case FillMode.FillOutLine:
                    Data[0] = (outer1 ?? outer);
                    Data[2] = (inner1 ?? inner);
                    if (ShapeName == "Bezier" && stroke == 0)
                        return Data;
                    Data[1] = outer;
                    Data[3] = inner;

                    if (fillCommand.HasFlag(FillCommand.Outlininig))
                    {
                        switch (ShapeName)
                        {
                            case "Bezier":
                            case "BezierArc":
                                Data[1].RemoveLast();
                                Data[3].RemoveLast();
                                break;
                            default:
                                break;
                        }
                    }
                    break;

                case FillMode.DrawOutLine:
                    Data[0] = (outer1 ?? outer);
                    Data[2] = (inner1 ?? inner);
                    break;

                case FillMode.ExceptOutLine:
                    Data[0] = (outer1 ?? outer);
                    Data[2] = (inner1 ?? inner);
                    switch (ShapeName)
                    {
                        case "Bezier":
                        case "Arc":
                        case "BezierArc":
                        case "ClosedArc":
                            return Data;
                        default:
                            break;
                    }
                    Data[1] = inner;
                    break;

                case FillMode.Inner:
                    Data[0] = (inner1 ?? inner);
                    switch (ShapeName)
                    {
                        case "Bezier":
                        case "Arc":
                        case "BezierArc":
                        case "ClosedArc":
                            return Data;
                        default:
                            break;
                    }
                    Data[1] = inner;
                    break;

                case FillMode.Outer:
                    Data[0] = (outer1 ?? outer);
                    switch (ShapeName)
                    {
                        case "Bezier":
                        case "Arc":
                        case "BezierArc":
                        case "ClosedArc":
                            return Data;
                        default:
                            break;
                    }
                    Data[1] = outer;
                    break;

            }
            return Data;
        }
        #endregion
    }

    partial class Renderer
    {
        #region READ PIXEL
        /// <summary>
        /// Reads a pixel after applying applying offset and rotation transformation (if exists) to get the correct co-ordinate.
        /// </summary>
        /// <param name="x">X co-ordinate of the location to read pixel from.</param>
        /// <param name="y">Y co-ordinate of the location to read pixel from.</param>
        /// <returns>Pixel value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int ReadPixel(this IReadable pen, float x, float y)
        {
            int h = pen.Height;
            int x0 = (int)x;
            int y0 = (int)y;

            float Dx = x - x0;
            float Dy = y - y0;

            if (Dx == 0 && Dy == 0)
                return pen.ReadPixel(x0, y0);

            #region BI-LINEAR INTERPOLATION
            int color;
            uint rb, ag;
            bool only2 = (y0 >= h || y0 + 1 >= h);

            uint c1 = (uint)pen.ReadPixel(x0 + 1, y0);
            uint c2 = (uint)pen.ReadPixel(x0, y0);

            uint alpha = (uint)(Dx * 255);
            uint invAlpha = 255 - alpha;

            if (alpha == 255)
                c1 = c2;

            else if (alpha != 0)
            {
                rb = ((invAlpha * (c1 & Colors.RBMASK)) + (alpha * (c2 & Colors.RBMASK))) >> 8;
                ag = (invAlpha * ((c1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((c2 & Colors.GMASK) >> 8)));
                c1 = ((rb & Colors.RBMASK) | (ag & Colors.AGMASK));
            }
            if (Dy == 0 || only2)
                return (int)c1;

            uint c3 = (uint)pen.ReadPixel(x0, y0 + 1);
            uint c4 = (uint)pen.ReadPixel(x0, y0 + 2);

            if (alpha == 255)
                c3 = c4;
            else if (alpha != 0)
            {
                rb = ((invAlpha * (c3 & Colors.RBMASK)) + (alpha * (c4 & Colors.RBMASK))) >> 8;
                ag = (invAlpha * ((c3 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((c4 & Colors.GMASK) >> 8)));
                c3 = ((rb & Colors.RBMASK) | (ag & Colors.AGMASK));
            }

            alpha = (uint)(Dy * 255);
            invAlpha = 255 - alpha;

            if (alpha == 255)
                color = (int)c3;
            else if (alpha != 0)
            {
                rb = ((invAlpha * (c1 & Colors.RBMASK)) + (alpha * (c3 & Colors.RBMASK))) >> 8;
                ag = (invAlpha * ((c1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((c3 & Colors.GMASK) >> 8)));
                color = (int)((rb & Colors.RBMASK) | (ag & Colors.AGMASK));
            }
            else
                color = (int)c1;
            #endregion

            return color;
        }
        #endregion

        #region WRITE PIXEL
        /// <summary>
        /// Writes pixel to the this block at given co-ordinates of location using specified color.
        /// </summary>
        /// <param name="buffer">Memory block to write pixel to.</param>
        /// <param name="x">X cordinate on 2d buffer memory block</param>
        /// <param name="y">Y cordinate on 2d buffer memory block</param>
        /// <param name="color">colour of pixel.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WritePixel(this IWritable buffer, float x, float y, int color)
        {
            int x0 = (int)x;
            int y0 = (int)y;

            float alpha1 = x - x0;
            float alpha2 = y - y0;
            bool aa = !buffer.Settings.LineCommand.HasFlag(LineCommand.Breshenham);

            if (alpha1 == 0 && alpha2 == 0 || !aa)
            {
                buffer.WritePixel(x0, y0, true, color);
                return;
            }

            if (alpha1 == 0 || alpha2 == 0)
            {
                bool horizontal = alpha1 != 0 ? true : false;
                if (horizontal)
                    buffer.WritePixel(x, y0, true, color);
                else
                    buffer.WritePixel(y, x0, false, color);
                return;
            }
            else
            {
                var invAlpha1 = 1 - alpha1;
                var invAlpha2 = 1 - alpha2;

                buffer.WritePixel(x0, y0, true, color, invAlpha1 * invAlpha2);
                buffer.WritePixel(x0 + 1, y0, true, color, alpha1 * invAlpha2);
                buffer.WritePixel(y0 + 1, x0, false, color, invAlpha1 * alpha2);
                buffer.WritePixel(y0 + 1, x0 + 1, false, color, alpha1 * alpha2);
            }
        }
        #endregion

        #region WRITE PIXELS
        /// <summary>
        /// Writes pixels on memory block with a given color on positions specified by a collection of points.
        /// </summary>
        /// <param name="buffer">Memory block to write pixel to.</param>
        /// <param name="points">Collection of points to offer positions to write pixels.</param>
        /// <param name="color">Color by which pixels should be written.</param>
        public static void WritePixels(this IWritable buffer, IEnumerable<VectorF> points, int color)
        {
            if (points == null)
                return;
            foreach (var p in points)
                buffer.WritePixel(p.X, p.Y, color);
        }

        /// <summary>
        /// Writes pixels on memory block with a given color on positions specified by a collection of points.
        /// </summary>
        /// <param name="buffer">Memory block to write pixel to.</param>
        /// <param name="points">Collection of points to offer positions to write pixels.</param>
        /// <param name="color">Color by which pixels should be written.</param>
        public static void WritePixels(this IWritable buffer, int color, IEnumerable<Vector> points)
        {
            if (points == null)
                return;
            foreach (var p in points)
                buffer.WritePixel(p.X, p.Y, true, color);
        }

        /// <summary>
        /// Writes pixels on memory block with a given color on positions specified by a collection of points.
        /// </summary>
        /// <param name="buffer">Memory block to write pixel to.</param>
        /// <param name="points">Collection of points to offer positions to write pixels.</param>
        /// <param name="pen">Pen to read corresponding pixels from in oroder to copy them.</param>
        public static void WritePixels(this IWritable buffer, IEnumerable<VectorF> points, IReadable pen)
        {
            if (points == null)
                return;
            foreach (var p in points)
                buffer.WritePixel(p.X, p.Y, pen.ReadPixel((int)p.X, (int)p.Y));
        }

        /// <summary>
        /// Writes pixels on memory block with a given color on positions specified by a collection of points.
        /// </summary>
        /// <param name="buffer">Memory block to write pixel to.</param>
        /// <param name="points">Collection of points to offer positions to write pixels.</param>
        /// <param name="pen">Pen to read corresponding pixels from in oroder to copy them.</param>
        public static void WritePixels(this IWritable buffer, IReadable pen, IEnumerable<Vector> points)
        {
            if (points == null)
                return;
            foreach (var p in points)
                buffer.WritePixel(p.X, p.Y, true, pen.ReadPixel(p.X, p.Y));
        }
        #endregion

        #region CREATE LINE ACTION
        /// <summary>
        /// Retuns an action delegate for rendering a glyph on specified buffer target using specified buffer pen.
        /// </summary>
        /// <param name="buffer">Buffer which to render a memory block on</param>
        /// <param name="pen">Buffer pen which to read pixeld from</param>
        /// <returns>An instance of FillAction delegate</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateAction(this IWritable buffer, IReadable pen, out FillAction<float> action)
        {
            action = (val1, axis, horizontal, val2, alpha) =>
            {
                buffer.WriteLine(val1, val2, axis, horizontal, pen, alpha);
            };
        }

        /// <summary>
        /// Retuns an action delegate for rendering an axial line or pixel on specified buffer target using specified buffer pen.
        /// </summary>
        /// <param name="buffer">Buffer which to render a memory block on</param>
        /// <param name="pen">Buffer pen which to read pixeld from</param>
        /// <returns>An instance of FillAction delegate</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateAction(this IWritable buffer, IReadable pen, out PixelAction<float> action)
        {
            action = (val, axis, horizontal) =>
            {
                var v = (int)val;
                int x = horizontal ? v : axis;
                int y = horizontal ? axis : v;
                if (horizontal)
                {
                    buffer.WritePixel(val, axis, horizontal, pen.ReadPixel(x, y));
                }
                else
                {
                    buffer.WritePixel(val, axis, horizontal, pen.ReadPixel(x, y));
                }
            };
        }

        /// <summary>
        /// Retuns an action delegate for rendering an axial line or pixel on specified buffer target using specified buffer pen.
        /// </summary>
        /// <param name="buffer">Buffer which to render a memory block on</param>
        /// <param name="pen">Buffer pen which to read pixeld from</param>
        /// <returns>An instance of FillAction delegate</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateAction(this IWritable buffer, IReadable pen, out PixelAction<int> action)
        {
            action = (val, axis, horizontal) =>
            {
                int x = horizontal ? val : axis;
                int y = horizontal ? axis : val;

                buffer.WritePixel(x, y, pen.ReadPixel(x, y));
            };
        }

        /// <summary>
        /// Retuns an action delegate for storing an axial line intersection information.
        /// </summary>
        /// <typeparam name="T">Represents any object which implements ICollection of objects of type U and has public parameterless constructor.</typeparam>
        /// <typeparam name="U">Desired type to represent information derived from action.</typeparam>
        /// <param name="Results">A collection of object of Type U to store information derived from action delegate.</param>
        /// <param name="Min"></param>
        /// <param name="action">An action delegate created for storing an axial line intersection information.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateAction<T>(this T[] Results, int Min, out PixelAction<float> action) where T : ICollection<float>, new()
        {
            action = (val, axis, horizontal) =>
            {
                int i;

                i = axis - Min;
                if (i >= Results.Length)
                    return;
                if (Results[i] == null)
                    Results[i] = new T();

                Results[i].Add(val);
            };
        }
        #endregion

        #region TO PIXEL ACTION
        public static PixelAction<T> ToPixelAction<T>(this FillAction<T> action)
        {
            return (v, axis, h) => action(v, axis, h, v);
        }
        public static PixelAction<T> ToPixelAction<T, U>(this FillAction<T> action)
        {
            return (v, axis, h) => action(v, axis, h, v, null);
        }
        public static PixelAction<int> ToIntPixelAction(this PixelAction<float> action)
        {
            return (v, axis, h) => action(v, axis, h);
        }

        public static PixelAction<int> ToPixelAction(this VectorAction<int> action)
        {
            return (v, axis, h) => action(h ? v : axis, h ? axis : v);
        }
        public static PixelAction<float> ToPixelAction(this VectorAction<float> action)
        {
            return (v, axis, h) => action(h ? v : axis, h ? axis : v);
        }
        #endregion
    }
    partial class Renderer
    {
        #region DRAW - ADD SHAPE AT LOCATION
        /// <summary>
        /// Draws any element on the given path. This renderer has a built-in support for the following kind of elements:
        /// 1. IShape
        /// 2. IDrawable
        /// 3. ICurve
        /// 4. IText
        /// Please note that in case your element does not implement any of the above, you must provide your own rendering routine
        /// by overriding RenderCustom method. Once you have handled it return true otherwise an exception wiil be raised.
        /// </summary>
        /// <param name="buffer">buffer target which to render a shape on</param>
        /// <param name="shape">Element which is to be rendered</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void Draw(this IWritable buffer, IRenderable shape, IReadContext context) =>
            buffer.Render(shape, context, null, null);

        /// <summary>
        /// Draws any element on the given path. This renderer has a built-in support for the following kind of elements:
        /// 1. IShape
        /// 2. IDrawable
        /// 3. ICurve
        /// 4. IText
        /// Please note that in case your element does not implement any of the above, you must provide your own rendering routine
        /// by overriding RenderCustom method. Once you have handled it return true otherwise an exception wiil be raised.
        /// </summary>
        /// <param name="buffer">buffer target which to render a shape on</param>
        /// <param name="shape">Element which is to be rendered</param>
        public static void Draw(this IWritable buffer, IRenderable shape) =>
            buffer.Render(shape, null, null, null);

        /// <summary>
        /// Draws any element on the given path. This renderer has a built-in support for the following kind of elements:
        /// 1. IShape
        /// 2. IDrawable
        /// 3. ICurve
        /// 4. IText
        /// Please note that in case your element does not implement any of the above, you must provide your own rendering routine
        /// by overriding RenderCustom method. Once you have handled it return true otherwise an exception wiil be raised.
        /// </summary>
        /// <param name="buffer">buffer target which to render a shape on</param>
        /// <param name="shape">Element which is to be rendered</param>
        /// <param name="drawX">X coordinate of a location where draw should take place</param>
        /// <param name="drawY">Y coordinate of a location where draw should take place</param>
        public static void Draw(this IWritable buffer, IRenderable shape, int? drawX, int? drawY) =>
            buffer.Render(shape, null, drawX, drawY);

        /// <summary>
        /// Draws any element on the given path. This renderer has a built-in support for the following kind of elements:
        /// 1. IShape
        /// 2. IDrawable
        /// 3. ICurve
        /// 4. IText
        /// Please note that in case your element does not implement any of the above, you must provide your own rendering routine
        /// by overriding RenderCustom method. Once you have handled it return true otherwise an exception wiil be raised.
        /// </summary>
        /// <param name="buffer">buffer target which to render a shape on</param>
        /// <param name="shape">Element which is to be rendered</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="drawX">X coordinate of a location where draw should take place</param>
        /// <param name="drawY">Y coordinate of a location where draw should take place</param>
        public static void Draw(this IWritable buffer, IRenderable shape, IReadContext context, int? drawX, int? drawY) =>
            buffer.Render(shape, context, drawX, drawY);

        /// <summary>
        /// Adds a shape object to this collection.
        /// </summary>
        /// <typeparam name="T">Any object which implements IElement</typeparam>
        /// <param name="shape">A shape object to be added of type specifie by TShape</param>
        /// <param name="context">The drawing context for the shape i.e a pen or color or a brush or even an another graphics or buffer object from which a data can be read.</param>
        /// <returns>the same Shape object which is added to collection. 
        /// this lets user to pass something like new shape(....) and then used it further more.
        /// for example: var ellipse = Add(Factory.newEllipse(10,10,100,200), Colour.Red, null, null);
        /// </returns>
        public static T Add<T>(this IObjCollection collection, T shape, IReadContext context) where T : IRenderable =>
            collection.Add(shape, context, null, null);

        /// <summary>
        /// Adds a shape object to this collection.
        /// </summary>
        /// <typeparam name="T">Any object which implements IElement</typeparam>
        /// <param name="shape">A shape object to be added of type specifie by TShape</param>
        /// <returns>the same Shape object which is added to collection. 
        /// this lets user to pass something like new shape(....) and then used it further more.
        /// for example: var ellipse = Add(Factory.newEllipse(10,10,100,200), Colour.Red, null, null);
        /// </returns>
        public static T Add<T>(this IObjCollection collection, T shape) where T : IRenderable =>
            collection.Add(shape, null, null, null);

        /// <summary>
        /// Adds a shape object to this collection.
        /// </summary>
        /// <typeparam name="T">Any object which implements IElement</typeparam>
        /// <param name="shape">A shape object to be added of type specifie by TShape</param>
        /// <param name="drawX">X coordinate of a location where draw should take place</param>
        /// <param name="drawY">Y coordinate of a location where draw should take place</param>
        /// <returns>the same Shape object which is added to collection. 
        /// this lets user to pass something like new shape(....) and then used it further more.
        /// for example: var ellipse = Add(Factory.newEllipse(10,10,100,200), Colour.Red, null, null);
        /// </returns>
        public static T Add<T>(this IObjCollection collection, T shape, int? drawX, int? drawY) where T : IRenderable =>
            collection.Add(shape, null, drawX, drawY);
        #endregion

        #region DRAW IMAGE
        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangele to the given destination.
        /// </summary>
        /// <param name="block">buffer which to render a memory block on</param>
        /// <param name="source">1D array interpreted as a 2D array of Pixels with specified srcW width</param>
        /// <param name="srcW">Width of the entire source</param>
        /// <param name="destX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="destY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyX">Top left x co-ordinate of area in source to cop.</param>
        /// <param name="copyY">Top left y co-ordinate of area in source to copy</param>
        /// <param name="copyW">Width of area in the source to copy.</param>
        /// <param name="copyH">Height of area in the source to copy</param>
        public static unsafe void DrawImage(this IWritable block, IntPtr source, int srcW, int srcH, int destX, int destY,
            int copyX, int copyY, int copyW, int copyH)
        {
            var src = Factory.newSurface(source, srcW, srcH);
            src.CopyTo(block, destX, destY, copyX, copyY, copyW, copyH);
        }

        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangele to the given destination.
        /// </summary>
        /// <param name="block">buffer which to render a memory block on</param>
        /// <param name="source">1D array interpreted as a 2D array of Pixels with specified srcW width</param>
        /// <param name="srcLen">Length of an entire source</param>
        /// <param name="srcW">Width of the entire source</param>
        /// <param name="destX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="destY">Top left y co-ordinate of destination on buffer</param>
        public static unsafe void DrawImage(this IWritable block, IntPtr source, int srcW, int srcH, int destX, int destY) =>
            DrawImage(block, source, srcW, srcH, destX, destY, 0, 0, srcW, srcH);

        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangele to the given destination.
        /// </summary>
        /// <param name="block">buffer which to render a memory block on</param>
        /// <param name="source">1D array interpreted as a 2D array of Pixels with specified srcW width</param>
        /// <param name="srcW">Width of the entire source</param>
        /// <param name="destX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="destY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyX">Top left x co-ordinate of area in source to cop.</param>
        /// <param name="copyY">Top left y co-ordinate of area in source to copy</param>
        /// <param name="copyW">Width of area in the source to copy.</param>
        /// <param name="copyH">Height of area in the source to copy</param>
        public unsafe static void DrawImage(this IWritable block, byte[] source, int srcW, int srcH, int destX, int destY,
            int? copyX = null, int? copyY = null, int? copyW = null, int? copyH = null)
        {
            var srcLen = source.Length / 4;
            var rc = Rects.CompitibleRc(srcW, srcLen / srcW, copyX, copyY, copyW, copyH);
            fixed (byte* b = source)
            {
                block.DrawImage((IntPtr)b, srcW, srcH, destX, destY, rc.X, rc.Y, rc.Width, rc.Height);
            }
        }

        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangele to the given destination.
        /// </summary>
        /// <param name="block">buffer which to render a memory block on</param>
        /// <param name="source">1D array interpreted as a 2D array of Pixels with specified srcW width</param>
        /// <param name="srcW">Width of the entire source</param>
        /// <param name="destX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="destY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyX">Top left x co-ordinate of area in source to cop.</param>
        /// <param name="copyY">Top left y co-ordinate of area in source to copy</param>
        /// <param name="copyW">Width of area in the source to copy.</param>
        /// <param name="copyH">Height of area in the source to copy</param>
        public unsafe static void DrawImage(this IWritable block, int[] source, int srcW, int srcH, int destX, int destY,
            int? copyX = null, int? copyY = null, int? copyW = null, int? copyH = null)
        {
            var rc = Rects.CompitibleRc(srcW, source.Length / srcW, copyX, copyY, copyW, copyH);
            var srcLen = source.Length;
            fixed (int* src = source)
            {
                block.DrawImage((IntPtr)src, srcW, srcH, destX, destY, rc.X, rc.Y, rc.Width, rc.Height);
            }
        }

        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangele to the given destination.
        /// </summary>
        /// <param name="block">buffer which to render a memory block on</param>
        /// <param name="source">1D array interpreted as a 2D array of Pixels with specified srcW width</param>
        /// <param name="destX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="destY">Top left y co-ordinate of destination on buffer</param>
        public static unsafe void DrawImage(this IWritable block, ICopyable source, int destX, int destY) =>
            source.CopyTo(block, destX, destY, 0, 0, source.Width, source.Height);

        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangele to the given destination.
        /// </summary>
        /// <param name="block">buffer which to render a memory block on</param>
        /// <param name="source">1D array interpreted as a 2D array of Pixels with specified srcW width</param>
        /// <param name="destX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="destY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyX">Top left x co-ordinate of area in source to cop.</param>
        /// <param name="copyY">Top left y co-ordinate of area in source to copy</param>
        /// <param name="copyW">Width of area in the source to copy.</param>
        /// <param name="copyH">Height of area in the source to copy</param>
        public static unsafe void DrawImage(this IWritable block, ICopyable source, int destX, int destY, int copyX, int copyY, int copyW, int copyH) =>
            source.CopyTo(block, destX, destY, copyX, copyY, copyW, copyH);

        /// <summary>
        /// Draws an image by taking an area from a source - capable of being copied to the given destination buffer.
        /// </summary>
        /// <param name="block">buffer which to render a memory block on</param>
        /// <param name="source">Source - capable of being copied to any buffer</param>
        /// <param name="destX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="destY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyRc">An Area from source to be copied</param>
        public static void DrawImage(this IWritable block, ICopyable source, int destX, int destY, Rectangle copyRc)
        {
            source.CopyTo(block, destX, destY, copyRc.X, copyRc.Y, copyRc.Width, copyRc.Height);
        }
        #endregion

        #region DRAW LINE
        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine(this IWritable buffer, float x1, float y1, float x2, float y2, IReadContext context)
        {
            if (buffer == null)
                return;
            var line = new Line(x1, y1, x2, y2);
            buffer.Render(line, context);
        }

        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine(this IWritable buffer, float x1, float y1, float x2, float y2)
        {
            if (buffer == null)
                return;
            var line = new Line(x1, y1, x2, y2);
            buffer.Render(line);
        }
        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by points p1 & p2.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="line">Line to draw</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine(this IWritable buffer, ILine line, IReadContext context)
        {
            if (buffer == null)
                return;
            buffer.Render(line, context);
        }


        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by points p1 & p2.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="l">Line to draw</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 before rendering the line segment</param>
        public static void DrawLine(this IWritable buffer, ILine l) =>
            buffer.DrawLine(l, null);

        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by points p1 & p2.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="p1">Start point of line segment</param>
        /// <param name="p2">end point of line segment</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 before rendering the line segment</param>
        public static void DrawLine(this IWritable buffer, VectorF p1, VectorF p2, IReadContext context = null) =>
            buffer.DrawLine(p1.X, p1.Y, p2.X, p2.Y, context);

        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by points p1 & p2.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="p1">Start point of line segment</param>
        /// <param name="p2">end point of line segment</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 before rendering the line segment</param>
        public static void DrawLine(this IWritable buffer, Vector p1, Vector p2, IReadContext context = null) =>
            buffer.DrawLine(p1.X, p1.Y, p2.X, p2.Y, context);

        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by points p1 & x2, y2.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="p1">Start point of line segment</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 before rendering the line segment</param>
        public static void DrawLine(this IWritable buffer, VectorF p1, float x2, float y2, IReadContext context = null) =>
            buffer.DrawLine(p1.X, p1.Y, x2, y2, context);

        /// <summary>
        /// Renders a collection of line segments using standard line algorithm.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="lines">A collection of lines</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawLines(this IWritable buffer, IEnumerable<ILine> lines, IReadContext context = null)
        {
            var draw = buffer.Settings.LineCommand;
            foreach (var l in lines)
            {
                buffer.Settings.LineCommand = draw;
                DrawLine(buffer, l, context);
            }
        }

        /// <summary>
        /// Renders a collection of line segments using standard line algorithm.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="lines">A collection of lines</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawLines(this IWritable buffer, IEnumerable<VectorF> points, IReadContext context = null, bool connectEach = true)
        {
            var draw = buffer.Settings.LineCommand;
            VectorF previous = VectorF.Empty;
            VectorF first = VectorF.Empty;

            foreach (var p in points)
            {
                buffer.Settings.LineCommand = draw;
                if (!first)
                    first = p;
                if (!previous)
                {
                    previous = p;
                    continue;
                }
                DrawLine(buffer, previous, p, context);
                if (!connectEach)
                    previous = Vector.Empty;
            }
            if (connectEach && first && previous)
            {
                DrawLine(buffer, previous, first, context);
            }
        }

        /// <summary>
        /// Renders a collection of line segments using standard line algorithm.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="lines">A collection of lines</param>
        public static void DrawLines(this IWritable buffer, IReadContext context, params ILine[] lines) =>
            buffer.DrawLines(lines as IEnumerable<ILine>, context);

        /// <summary>
        /// Renders a collection of line segments using standard line algorithm.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="lines">A collection of lines</param>
        public static void DrawLines(this IWritable buffer, params ILine[] lines) =>
            buffer.DrawLines(lines as IEnumerable<ILine>, null);

        /// <summary>
        /// Renders line segments by first creating them from an array of integer values specified using standard line algorithm.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="connectEach">If true then each line segment will be connected to the previous and next one</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 of each line segment before rendering the line segment</param>
        /// <param name="values">An interger array of values.Each subsequent four elements get converted to a line segment
        /// For example if values are int[]{23, 56, 98, 205} creates Line(X1 = 23, Y1 = 56, X2 = 98,Y2 = 205) </param>
        public static void DrawLines(this IWritable buffer, IReadContext context, bool connectEach, params int[] values)
        {
            var points = (values).ToPointsF();
            if (connectEach && points.Count > 2)
                points.Add(points[0]);

            if (connectEach)
            {
                for (int i = 1; i < points.Count; i++)
                    buffer.DrawLine(points[i - 1], points[i], context);
            }
            else
            {
                for (int i = 1; i < points.Count; i += 2)
                    buffer.DrawLine(points[i - 1], points[i], context);
            }
        }

        /// <summary>
        /// Renders line segments by first creating them from an array of float values specified using standard line algorithm.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="connectEach">If true then each line segment will be connected to the previous and next one</param>
        /// <param name="angle"></param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 of each line segment before rendering the line segment</param>
        /// <param name="values">A float array of values.Each subsequent four elements get converted to a line segment
        /// For example if values are int[]{23.33f, 56.67f, 98.45f, 205.21f} creates Line(X1 = 23.33f, Y1 = 56.67f, X2 = 98.45f,Y2 = 205.21f) </param>
        public static void DrawLines(this IWritable buffer, IReadContext context, bool connectEach, params float[] values)
        {
            var points = values.ToPoints();
            if (connectEach && points.Count > 2)
                points.Add(points[0]);

            if (connectEach)
            {
                for (int i = 1; i < points.Count; i++)
                    buffer.DrawLine(points[i - 1], points[i], context);
            }
            for (int i = 1; i < points.Count; i += 2)
            {
                buffer.DrawLine(points[i - 1], points[i], context);
            }
        }

        /// <summary>
        /// Renders line segments by first creating them from an array of integer values specified using standard line algorithm.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="connectEach">If true then each line segment will be connected to the previous and next one</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 of each line segment before rendering the line segment</param>
        /// <param name="values">An interger array of values.Each subsequent four elements get converted to a line segment
        /// For example if values are int[]{23, 56, 98, 205} creates Line(X1 = 23, Y1 = 56, X2 = 98,Y2 = 205) </param>
        public static void DrawLines(this IWritable buffer, bool connectEach, Rotation angle, params int[] values) =>
            buffer.DrawLines(null, connectEach, values);

        /// <summary>
        /// Renders line segments by first creating them from an array of float values specified using standard line algorithm.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="connectEach">If true then each line segment will be connected to the previous and next one</param>
        /// <param name="angle"></param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 of each line segment before rendering the line segment</param>
        /// <param name="values">A float array of values.Each subsequent four elements get converted to a line segment
        /// For example if values are int[]{23.33f, 56.67f, 98.45f, 205.21f} creates Line(X1 = 23.33f, Y1 = 56.67f, X2 = 98.45f,Y2 = 205.21f) </param>
        public static void DrawLines(this IWritable buffer, bool connectEach, Rotation angle, params float[] values) =>
            buffer.DrawLines(null, connectEach, values);

        /// <summary>
        /// Renders line segments by first creating them from an array of integer values specified using standard line algorithm.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="connectEach">If true then each line segment will be connected to the previous and next one</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 of each line segment before rendering the line segment</param>
        /// <param name="values">An interger array of values.Each subsequent four elements get converted to a line segment
        /// For example if values are int[]{23, 56, 98, 205} creates Line(X1 = 23, Y1 = 56, X2 = 98,Y2 = 205) </param>
        public static void DrawLines(this IWritable buffer, bool connectEach, params int[] values) =>
            buffer.DrawLines(null, connectEach, values);

        /// <summary>
        /// Renders line segments by first creating them from an array of float values specified using standard line algorithm.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="connectEach">If true then each line segment will be connected to the previous and next one</param>
        /// <param name="values">A float array of values.Each subsequent four elements get converted to a line segment
        /// For example if values are int[]{23.33f, 56.67f, 98.45f, 205.21f} creates Line(X1 = 23.33f, Y1 = 56.67f, X2 = 98.45f,Y2 = 205.21f) </param>
        public static void DrawLines(this IWritable buffer, bool connectEach, params float[] values) =>
            buffer.DrawLines(null, connectEach, values);
        #endregion

        #region DRAW CIRCLE
        /// <summary>
        /// Draws a circle specified by the bounding area and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a circle/ellipse on</param>
        /// <param name="x">X cordinate of a bounding area where the circle is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle is to be drawn</param>
        /// <param name="width">Width of a bounding area where the circle is to be drawn -> circle's minor X axis = Width/2</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the circle</param>
        public static void DrawCircle(this IWritable buffer, float x, float y, float width, IReadContext context) =>
            RenderCircleOrEllipse(buffer, x, y, width, width, context);

        /// <summary>
        /// Draws a circle specified by the bounding area and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a circle/ellipse on</param>
        /// <param name="x">X cordinate of a bounding area where the circle is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle is to be drawn</param>
        /// <param name="width">Width of a bounding area where the circle is to be drawn -> circle's minor X axis = Width/2</param>
        /// <param name="angle">Angle to apply rotation while rendering the circle</param>
        public static void DrawCircle(this IWritable buffer, float x, float y, float width) =>
            RenderCircleOrEllipse(buffer, x, y, width, width, null);

        /// <summary>
        /// Draws a circle specified by the center point and another point which provides a start location and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a circle/ellipse on</param>
        /// <param name="pointOnCircle">A point on a circle which you want</param>
        /// <param name="centerOfCircle">Center of a circle</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the circle</param>
        public static void DrawCircle(this IWritable buffer, VectorF pointOnCircle, VectorF centerOfCircle, IReadContext context)
        {
            Curves.GetCircleData(pointOnCircle, centerOfCircle, out float x, out float y, out float w);
            RenderCircleOrEllipse(buffer, x, y, w, w, context);
        }

        /// <summary>
        /// Draws a circle specified by the center point and another point which provides a start location and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a circle/ellipse on</param>
        /// <param name="pointOnCircle">A point on a circle which you want</param>
        /// <param name="centerOfCircle">Center of a circle</param>
        /// <param name="angle">Angle to apply rotation while rendering the circle</param>
        public static void DrawCircle(this IWritable buffer, VectorF pointOnCircle, VectorF centerOfCircle)
        {
            Curves.GetCircleData(pointOnCircle, centerOfCircle, out float x, out float y, out float w);
            RenderCircleOrEllipse(buffer, x, y, w, w, null);
        }
        #endregion

        #region DRAW ELLIPSE
        /// <summary>
        /// Draws an ellipse specified by the bounding area and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a ellipse on</param>
        /// <param name="x">X cordinate of a bounding area where the ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the ellipse is to be drawn</param>
        /// <param name="width">Width of a bounding area where the ellipse is to be drawn -> ellipse's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the ellipse is to be drawn -> ellipse's minor Y axis = Height/2</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawEllipse(this IWritable buffer, float x, float y, float width, float height, IReadContext context) =>
            RenderCircleOrEllipse(buffer, x, y, width, height, context);

        /// <summary>
        /// Draws an ellipse specified by the bounding area and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a ellipse on</param>
        /// <param name="x">X cordinate of a bounding area where the ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the ellipse is to be drawn</param>
        /// <param name="width">Width of a bounding area where the ellipse is to be drawn -> ellipse's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the ellipse is to be drawn -> ellipse's minor Y axis = Height/2</param>
        public static void DrawEllipse(this IWritable buffer, float x, float y, float width, float height) =>
            RenderCircleOrEllipse(buffer, x, y, width, height, null);

        /// <summary>
        /// Draws an ellipse passing through three points and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a circle/ellipse on</param>
        /// <param name="p1">First point on an ellipse</param>
        /// <param name="p2">Second point  on the ellipse</param>
        /// <param name="p3">third point on the ellipse</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered.
        /// Such as fitting, third point on ellipse or on center etc.</param>
        public static void DrawEllipse(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, IReadContext context, CurveType type = CurveType.Full) =>
            RenderCircleOrEllipse(buffer, p1, p2, p3, context, type);

        /// <summary>
        /// Draws an ellipse passing through three points and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a circle/ellipse on</param>
        /// <param name="p1">First point on an ellipse</param>
        /// <param name="p2">Second point  on the ellipse</param>
        /// <param name="p3">third point on the ellipse</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered.
        /// Such as fitting, third point on ellipse or on center etc.</param>
        public static void DrawEllipse(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, CurveType type = CurveType.Full) =>
            RenderCircleOrEllipse(buffer, p1, p2, p3, null, type);

        /// <summary>
        /// Draws an ellipse passing through four points and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a circle/ellipse on</param>
        /// <param name="p1">First point on an ellipse</param>
        /// <param name="p2">Second point  on the ellipse</param>
        /// <param name="p3">third point on the ellipse</param>
        /// <param name="p4">third point on the ellipse</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered.
        /// Such as fitting, third point on ellipse or on center etc.</param>
        public static void DrawEllipse(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, VectorF p4, IReadContext context, CurveType type = CurveType.Full) =>
            RenderCircleOrEllipse(buffer, p1, p2, p3, p4, context, type);

        /// <summary>
        /// Draws an ellipse passing through four points and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a circle/ellipse on</param>
        /// <param name="p1">First point on an ellipse</param>
        /// <param name="p2">Second point  on the ellipse</param>
        /// <param name="p3">third point on the ellipse</param>
        /// <param name="p4">third point on the ellipse</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered.
        /// Such as fitting, third point on ellipse or on center etc.</param>
        public static void DrawEllipse(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, VectorF p4, CurveType type = CurveType.Full) =>
            RenderCircleOrEllipse(buffer, p1, p2, p3, p4, null, type);

        /// <summary>
        /// Draws an ellipse passing through five points conic section and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a circle/ellipse on</param>
        /// <param name="p1">First point on an ellipse</param>
        /// <param name="p2">Second point  on the ellipse</param>
        /// <param name="p3">third point on the ellipse</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="p5">Fifth point</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawEllipse(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, VectorF p4, VectorF p5, IReadContext context) =>  
            RenderCircleOrEllipse(buffer, p1, p2, p3, p4, p5, context);

        /// <summary>
        /// Draws an ellipse passing through five points conic section and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a circle/ellipse on</param>
        /// <param name="first">First point on an ellipse</param>
        /// <param name="second">Second point  on the ellipse</param>
        /// <param name="third">third point on the ellipse</param>
        /// <param name="fourth">Fourth point</param>
        /// <param name="fifth">Fifth point</param>
        public static void DrawEllipse(this IWritable buffer, VectorF first, VectorF second, VectorF third, VectorF fourth, VectorF fifth) =>
            RenderCircleOrEllipse(buffer, first, second, third, fourth, fifth, null);
        #endregion

        #region DRAW ARC
        /// <summary>
        /// Draws an arc specified by the bounding area and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="buffer">buffer which to render an arc on</param>
        /// <param name="x">X cordinate of a bounding area where the arc is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the arc is to be drawn</param>
        /// <param name="width">Width of a bounding area where the arc is to be drawn -> arc's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the arc is to be drawn ->arc's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. If type includes NoSweepAngle option otherwise effective end angle is start angle + end angle</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="type"> Defines the type of an arc along with other supplimentary options on how to draw it</param>
        public static void DrawArc(this IWritable buffer, float x, float y, float width, float height, float startAngle, float endAngle,
            IReadContext context, CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, x, y, width, height, startAngle, endAngle, context, type);

        /// <summary>
        /// Draws an arc specified by the bounding area and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="buffer">buffer which to render an arc on</param>
        /// <param name="x">X cordinate of a bounding area where the arc is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the arc is to be drawn</param>
        /// <param name="width">Width of a bounding area where the arc is to be drawn -> arc's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the arc is to be drawn ->arc's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. If type includes NoSweepAngle option otherwise effective end angle is start angle + end angle</param>
        /// <param name="type"> Defines the type of an arc along with other supplimentary options on how to draw it</param>
        public static void DrawArc(this IWritable buffer, float x, float y, float width, float height, float startAngle, float endAngle,
            CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, x, y, width, height, startAngle, endAngle, null, type.Exclude(CurveType.Pie).Include(CurveType.Arc));

        /// <summary>
        /// Draws an arc passing through three points and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render an arc on</param>
        /// <param name="p1">First point on an arc</param>
        /// <param name="p2">Second point  on the arc</param>
        /// <param name="p3">Third point on the arc</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawArc(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, IReadContext context, CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, p1, p2, p3, context, type.Exclude(CurveType.Pie).Include(CurveType.Arc));

        /// <summary>
        /// Draws an arc passing through four points and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render an arc on</param>
        /// <param name="p1">First point on an arc</param>
        /// <param name="p2">Second point  on the arc</param>
        /// <param name="p3">Third point on the arc</param>
        /// <param name="p4">Fourth point on the arc</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawArc(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, VectorF p4, IReadContext context,
            CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, p1, p2, p3, p4, context, type.Exclude(CurveType.Pie).Include(CurveType.Arc));

        /// <summary>
        /// Draws an arc passing through four points and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render an arc on</param>
        /// <param name="p1">First point on an arc</param>
        /// <param name="p2">Second point  on the arc</param>
        /// <param name="p3">Third point on the arc</param>
        /// <param name="p4">Fourth point on the arc</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc</param>
        public static void DrawArc(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, VectorF p4, CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, p1, p2, p3, p4, null, type.Exclude(CurveType.Pie).Include(CurveType.Arc));

        /// <summary>
        /// Draws an arc passing through three points and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render an arc on</param>
        /// <param name="p1">First point on an arc</param>
        /// <param name="p2">Second point  on the arc</param>
        /// <param name="p3">Third point on the arc</param>
        public static void DrawArc(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, CurveType type = CurveType.Arc) =>
            buffer.DrawArc(p1, p2, p3, null, type.Exclude(CurveType.Pie).Include(CurveType.Arc));

        /// <summary>
        /// Draws an arc passing through five points and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render an arc on</param>
        /// <param name="p1">First point on an arc</param>
        /// <param name="p2">Second point  on the arc</param>
        /// <param name="p3">Third point on the arc</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="p5">Fifth point</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawArc(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3,
            VectorF p4, VectorF p5, IReadContext context, CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, p1, p2, p3, p4, p5, context, type.Exclude(CurveType.Pie).Include(CurveType.Arc));

        /// <summary>
        /// Draws an arc passing through five points and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render an arc on</param>
        /// <param name="p1">First point on an arc</param>
        /// <param name="p2">Second point  on the arc</param>
        /// <param name="p3">Third point on the arc</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="p5">Fifth point</param>
        public static void DrawArc(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3,
            VectorF p4, VectorF p5, CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, p1, p2, p3, p4, p5, null, type.Exclude(CurveType.Pie).Include(CurveType.Arc));
        #endregion

        #region DRAW PIE
        /// <summary>
        /// Draws a pie specified by the bounding area and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="buffer">buffer which to render a pie on</param>
        /// <param name="x">X cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="width">Width of a bounding area where the pie is to be drawn -> pie's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the pie is to be drawn ->pie's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. If type includes NoSweepAngle option otherwise effective end angle is start angle + end angle</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the pie</param>
        /// <param name="type"> Defines the type of an pie along with other supplimentary options on how to draw it</param>
        public static void DrawPie(this IWritable buffer, float x, float y, float width, float height,
            float startAngle, float endAngle, IReadContext context, CurveType type = CurveType.Pie) =>
            RenderArcOrPie(buffer, x, y, width, height, startAngle, endAngle, context, type.Exclude(CurveType.Arc).Include(CurveType.Pie));

        /// <summary>
        /// Draws a pie specified by the bounding area and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="buffer">buffer which to render a pie on</param>
        /// <param name="x">X cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="width">Width of a bounding area where the pie is to be drawn -> pie's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the pie is to be drawn ->pie's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. If type includes NoSweepAngle option otherwise effective end angle is start angle + end angle</param>
        /// <param name="angle">Angle to apply rotation while rendering the pie</param>
        /// <param name="type"> Defines the type of an pie along with other supplimentary options on how to draw it</param>
        public static void DrawPie(this IWritable buffer, float x, float y, float width, float height, float startAngle, float endAngle,
            CurveType type = CurveType.Pie) =>
            buffer.DrawPie(x, y, width, height, startAngle, endAngle, null, type.Exclude(CurveType.Arc).Include(CurveType.Pie));

        /// <summary>
        /// Draws a pie passing through two specified points p1, p2 and has a center p3 and angle of rotation if supplied.
        /// If Curvetype has option ThirdPointOnArc then p3 will be a point on arc rather than center and different arc gets generated. 
        /// </summary>
        /// <param name="buffer">buffer which to render a pie on</param>
        /// <param name="p1">First point on the pie</param>
        /// <param name="p2">Second point which must be on the pie</param>
        /// <param name="p3">Third point on the pie</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the pie</param>
        /// <param name="type"> Defines the type of an pie along with other supplimentary options on how to draw it</param>
        public static void DrawPie(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, IReadContext context, CurveType type = CurveType.Pie) =>
            RenderArcOrPie(buffer, p1, p2, p3, context, type.Exclude(CurveType.Arc).Include(CurveType.Pie));

        /// <summary>
        /// Draws a pie passing through two specified points p1, p2 and has a center p3 and angle of rotation if supplied.
        /// If Curvetype has option ThirdPointOnArc then p3 will be a point on arc rather than center and different arc gets generated. 
        /// </summary>
        /// <param name="buffer">buffer which to render a pie on</param>
        /// <param name="p1">First point on the pie</param>
        /// <param name="p2">Second point which must be on the pie</param>
        /// <param name="p3">Third point on the pie</param>
        /// <param name="angle">Angle to apply rotation while rendering the pie</param>
        /// <param name="type"> Defines the type of an pie along with other supplimentary options on how to draw it</param>
        public static void DrawPie(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, CurveType type = CurveType.Pie) =>
            buffer.DrawPie(p1, p2, p3, null, type.Exclude(CurveType.Arc).Include(CurveType.Pie));


        /// <summary>
        /// Draws a pie passing through four points and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render an arc on</param>
        /// <param name="p1">First point on the pie</param>
        /// <param name="p2">Second point which must be on the pie</param>
        /// <param name="p3">Third point on the pie</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc</param>
        public static void DrawPie(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3,
            VectorF p4, IReadContext context, CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, p1, p2, p3, p4, context, type.Exclude(CurveType.Arc).Include(CurveType.Pie));

        /// <summary>
        /// Draws a pie passing through four points and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render an arc on</param>
        /// <param name="p1">First point on the pie</param>
        /// <param name="p2">Second point which must be on the pie</param>
        /// <param name="p3">Third point on the pie</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc</param>
        public static void DrawPie(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3,
            VectorF p4, CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, p1, p2, p3, p4, null, type.Exclude(CurveType.Arc).Include(CurveType.Pie));


        /// <summary>
        /// Draws a pie passing through five points and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render an arc on</param>
        /// <param name="p1">First point on the pie</param>
        /// <param name="p2">Second point which must be on the pie</param>
        /// <param name="p3">Third point on the pie</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="p5">Fifth point</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc</param>
        public static void DrawPie(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3,
            VectorF p4, VectorF p5, IReadContext context, CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, p1, p2, p3, p4, p5, context, type.Exclude(CurveType.Arc).Include(CurveType.Pie));

        /// <summary>
        /// Draws a pie passing through five points and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render an arc on</param>
        /// <param name="p1">First point on the pie</param>
        /// <param name="p2">Second point which must be on the pie</param>
        /// <param name="p3">Third point on the pie</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="p5">Fifth point</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc</param>
        public static void DrawPie(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3,
            VectorF p4, VectorF p5, CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, p1, p2, p3, p4, p5, null, type.Exclude(CurveType.Arc).Include(CurveType.Pie));
        #endregion

        #region DRAW CURVE
        /// <summary>
        /// Renders a curve object.
        /// </summary>
        /// <param name="buffer">Buffer which to render a curve on</param>
        /// <param name="Curve">Cureve object to render</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawCurve(this IWritable buffer, ICurve Curve, IReadContext context = null)
        {
            buffer.Render(Curve, context);
        }
        #endregion

        #region DRAW CONIC
        /// <summary>
        /// Renders a conic object.
        /// </summary>
        /// <param name="conic">Conic object to render</param>
        /// <param name="buffer">Buffer which to render a curve on</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="drawEndsOnly">If true, only out line of conic will be drawn and filling will not be performed.</param>
        public static void DrawConic(this IWritable buffer, IConic conic, IReadContext context = null, bool drawEndsOnly = false)
        {
            if (buffer == null)
                return;

            if (drawEndsOnly)
            {
                buffer.Settings.FillCommand |= FillCommand.DrawEndsOnly;
                buffer.Settings.FillCommand &= ~FillCommand.DrawLineOnly;
            }
            else
            {
                buffer.Settings.FillCommand &= ~FillCommand.DrawEndsOnly;
                buffer.Settings.FillCommand &= ~FillCommand.DrawLineOnly;
            }
            buffer.Render(conic, context);
        }
        #endregion

        #region DRAW BEZIER
        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawBezier(this IWritable buffer, IReadContext context, params float[] points) =>
            RenderBezier(buffer, points, BezierType.Cubic, context);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="type">BezierType enum determines the type of bezier i.e Cubic - group of 4 points or multiple(group of 4 or 7 or 10 so on...)</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawBezier(this IWritable buffer, BezierType type, IReadContext context, params float[] points) =>
            RenderBezier(buffer, points, type, context);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        public static void DrawBezier(this IWritable buffer, params float[] points) =>
            RenderBezier(buffer, points, BezierType.Cubic, null);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="type">BezierType enum determines the type of bezier i.e Cubic - group of 4 points or multiple(group of 4 or 7 or 10 so on...)</param>
        public static void DrawBezier(this IWritable buffer, BezierType type, params float[] points) =>
            RenderBezier(buffer, points, type, null);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in integers - each group of two subsequent values forms one point i.e x & y</param>
        public static void DrawBezier(this IWritable buffer, params int[] points) =>
            RenderBezier(buffer, points.Select(p => (float)p), BezierType.Cubic, null);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in integers - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="type">BezierType enum determines the type of bezier i.e Cubic - group of 4 points or multiple(group of 4 or 7 or 10 so on...)</param>
        public static void DrawBezier(this IWritable buffer, BezierType type, params int[] points) =>
            RenderBezier(buffer, points.Select(p => (float)p), type, null);
        #endregion

        #region DRAW TRIANGLE
        /// <summary>
        /// Renders a triangle formed by three points specified by x1,y1 & x2,y2 & x3,y3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a triangle on</param>
        /// <param name="x1">X corodinate of the first point</param>
        /// <param name="y1">Y corodinate of the first point</param>
        /// <param name="x2">X corodinate of the second point</param>
        /// <param name="y2">Y corodinate of the second point</param>
        /// <param name="x3">X corodinate of the third point</param>
        /// <param name="y3">Y corodinate of the third point</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the traingle</param>
        public static void DrawTriangle(this IWritable buffer, float x1, float y1, float x2, float y2, float x3, float y3, IReadContext context) =>
            RenderTriangle(buffer, x1, y1, x2, y2, x3, y3, context);

        /// <summary>
        /// Renders a trianle formed by three points specified by x1,y1 & x2,y2 & x3,y3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a triangle on</param>
        /// <param name="x1">X corodinate of the first point</param>
        /// <param name="y1">Y corodinate of the first point</param>
        /// <param name="x2">X corodinate of the second point</param>
        /// <param name="y2">Y corodinate of the second point</param>
        /// <param name="x3">X corodinate of the third point</param>
        /// <param name="y3">Y corodinate of the third point</param>
        /// <param name="angle">Angle to apply rotation while rendering the traingle</param>
        public static void DrawTriangle(this IWritable buffer, float x1, float y1, float x2, float y2, float x3, float y3) =>
            buffer.DrawTriangle(x1, y1, x2, y2, x3, y3, null);

        /// <summary>
        /// Renders a trianle formed by three points specified by points p1, p2, p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a triangle on</param>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">the third point</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle"></param>
        public static void DrawTriangle(this IWritable buffer, Vector p1, Vector p2, Vector p3, IReadContext context) =>
            buffer.DrawTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, context);

        /// <summary>
        /// Renders a trianle formed by three points specified by points p1, p2, p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a triangle on</param>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">the third point</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle"></param>
        public static void DrawTriangle(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, IReadContext context) =>
            buffer.DrawTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, context);

        /// <summary>
        /// Renders a trianle formed by three points specified by points p1, p2, p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a triangle on</param>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">the third point</param>
        /// <param name="angle"></param>
        public static void DrawTriangle(this IWritable buffer, Vector p1, Vector p2, Vector p3) =>
        buffer.DrawTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, null);

        /// <summary>
        /// Renders a trianle formed by three points specified by points p1, p2, p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a triangle on</param>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">the third point</param>
        /// <param name="angle"></param>
        public static void DrawTriangle(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3) =>
            buffer.DrawTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, null);
        #endregion

        #region DRAW SQUARE
        /// <summary>
        /// Renders a rectangle specified by x, y, width, height parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a rectangle on</param>
        /// <param name="x">X cordinate of the rectangle</param>
        /// <param name="y">Y cordinate of the rectangle</param>
        /// <param name="width">Width  and also height of the rectangle/param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rectangle</param>
        public static void DrawSquare(this IWritable buffer, float x, float y, float width, IReadContext context) =>
            RenderRectangle(buffer, x, y, width, width, context);

        /// <summary>
        /// Renders a rectangle specified by x, y, width, height parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a rectangle on</param>
        /// <param name="x">X cordinate of the rectangle</param>
        /// <param name="y">Y cordinate of the rectangle</param>
        /// <param name="width">Width  and also height of the rectangle/param>
        /// <param name="angle">Angle to apply rotation while rendering the rectangle</param>
        public static void DrawSquare(this IWritable buffer, float x, float y, float width) =>
            RenderRectangle(buffer, x, y, width, width, null);
        #endregion

        #region DRAW RECTANGLE
        /// <summary>
        /// Renders a rectangle specified by x, y, width, height parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a rectangle on</param>
        /// <param name="x">X cordinate of the rectangle</param>
        /// <param name="y">Y cordinate of the rectangle</param>
        /// <param name="width">Width of the rectangle/param>
        /// <param name="height">Height the rectangle</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rectangle</param>
        public static void DrawRectangle(this IWritable buffer, float x, float y, float width, float height, IReadContext context) =>
            RenderRectangle(buffer, x, y, width, height, context);

        /// <summary>
        /// Renders a rectangle specified by x, y, width, height parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a rectangle on</param>
        /// <param name="x">X cordinate of the rectangle</param>
        /// <param name="y">Y cordinate of the rectangle</param>
        /// <param name="width">Width of the rectangle/param>
        /// <param name="height">Height the rectangle</param>
        /// <param name="angle">Angle to apply rotation while rendering the rectangle</param>
        public static void DrawRectangle(this IWritable buffer, float x, float y, float width, float height) =>
            RenderRectangle(buffer, x, y, width, height, null);

        /// <summary>
        /// Renders a rectangle specified by r parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a rectangle on</param>
        /// <param name="r">Rectangle to draw</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rectangle</param>
        public static void DrawRectangle(this IWritable buffer, Rectangle r, IReadContext context) =>
            RenderRectangle(buffer, r.X, r.Y, r.Width, r.Height, context);

        /// <summary>
        /// Renders a rectangle specified by r parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a rectangle on</param>
        /// <param name="r">Rectangle to draw</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rectangle</param>
        public static void DrawRectangle(this IWritable buffer, RectangleF r, IReadContext context) =>
           RenderRectangle(buffer, r.X, r.Y, r.Width, r.Height, context);

        /// <summary>
        /// Renders a rectangle specified by r parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a rectangle on</param>
        /// <param name="r">Rectangle to draw</param>
        /// <param name="angle">Angle to apply rotation while rendering the rectangle</param>
        public static void DrawRectangle(this IWritable buffer, Rectangle r) =>
            RenderRectangle(buffer, r.X, r.Y, r.Width, r.Height, null);

        /// <summary>
        /// Renders a rectangle specified by r parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a rectangle on</param>
        /// <param name="r">Rectangle to draw</param>
        /// <param name="angle">Angle to apply rotation while rendering the rectangle</param>
        public static void DrawRectangle(this IWritable buffer, RectangleF r) =>
           RenderRectangle(buffer, r.X, r.Y, r.Width, r.Height, null);
        #endregion

        #region DRAW ROUNDED BOX
        /// <summary>
        /// Renders a rounded box specified by x, y, width, height parameters and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="buffer">buffer which to render a rounded box on</param>
        /// <param name="x">X cordinate of the rounded box</param>
        /// <param name="y">Y cordinate of the rounded box</param>
        /// <param name="width">Width of the rounded box/param>
        /// <param name="height">Height the rounded box</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rounded box</param>
        public static void DrawRoundedBox(this IWritable buffer, float x, float y, float width, float height, float cornerRadius, IReadContext context) =>
           RenderRoundedBox(buffer, x, y, width, height, cornerRadius, context);

        /// <summary>
        /// Renders a rounded box specified by x, y, width, height parameters and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="buffer">buffer which to render a rounded box on</param>
        /// <param name="x">X cordinate of the rounded box</param>
        /// <param name="y">Y cordinate of the rounded box</param>
        /// <param name="width">Width of the rounded box/param>
        /// <param name="height">Height the rounded box</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="angle">Angle to apply rotation while rendering the rounded box</param>
        public static void DrawRoundedBox(this IWritable buffer, float x, float y, float width, float height, float cornerRadius) =>
            RenderRoundedBox(buffer, x, y, width, height, cornerRadius, null);

        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="buffer">buffer which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rounded box</param>
        public static void DrawRoundedBox(this IWritable buffer, RectangleF r, float cornerRadius, IReadContext context) =>
            RenderRoundedBox(buffer, r.X, r.Y, r.Width, r.Height, cornerRadius, context);

        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="buffer">buffer which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rounded box</param>
        public static void DrawRoundedBox(this IWritable buffer, Rectangle r, float cornerRadius, IReadContext context) =>
            RenderRoundedBox(buffer, r.X, r.Y, r.Width, r.Height, cornerRadius, context);

        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="buffer">buffer which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="angle">Angle to apply rotation while rendering the rounded box</param>
        public static void DrawRoundedBox(this IWritable buffer, RectangleF r, float cornerRadius) =>
            RenderRoundedBox(buffer, r.X, r.Y, r.Width, r.Height, cornerRadius, null);

        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="buffer">buffer which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="angle">Angle to apply rotation while rendering the rounded box</param>
        public static void DrawRoundedBox(this IWritable buffer, Rectangle r, float cornerRadius) =>
            RenderRoundedBox(buffer, r.X, r.Y, r.Width, r.Height, cornerRadius, null);
        #endregion

        #region DRAW RHOMBUS
        /// <summary>
        /// Renders a rhombus specified by three points (The fourth point will be calculated) and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a rhombus on</param>
        /// <param name="first">First point</param>
        /// <param name="second">Second point</param>
        /// <param name="third">Third point</param>
        /// <param name="angle">Angle to apply rotation while rendering the rhombus</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawRhombus(this IWritable buffer, VectorF first, VectorF second, VectorF third, IReadContext context) =>
            RenderRhombus(buffer, first, second, third, context);

        /// <summary>
        /// Renders a rhombus specified by three points (The fourth point will be calculated) and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a rhombus on</param>
        /// <param name="first">First point</param>
        /// <param name="second">Second point</param>
        /// <param name="third">Third point</param>
        /// <param name="angle">Angle to apply rotation while rendering the rhombus</param>
        public static void DrawRhombus(this IWritable buffer, VectorF first, VectorF second, VectorF third) =>
            RenderRhombus(buffer, first, second, third, null);

        /// <summary>
        /// Renders a rhombus specified by three points (The fourth point will be calculated) and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="x1">X coordinate of first point</param>
        /// <param name="y1">>Y coordinate of first point</param>
        /// <param name="x2">X coordinate of second point</param>
        /// <param name="y2">Y coordinate of second point</param>
        /// <param name="x3">X coordinate of third point</param>
        /// <param name="y3">Y coordinate of third point</param>
        /// <param name="angle">Angle to apply rotation while rendering the rhombus</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawRhombus(this IWritable buffer, float x1, float y1, float x2, float y2, float x3, float y3,
            IReadContext context) =>
            RenderRhombus(buffer, new VectorF(x1, y1), new VectorF(x2, y2), new VectorF(x3, y3), context);

        /// <summary>
        /// Renders a rhombus specified by three points (The fourth point will be calculated) and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="x1">X coordinate of first point</param>
        /// <param name="y1">>Y coordinate of first point</param>
        /// <param name="x2">X coordinate of second point</param>
        /// <param name="y2">Y coordinate of second point</param>
        /// <param name="x3">X coordinate of third point</param>
        /// <param name="y3">Y coordinate of third point</param>
        /// <param name="angle">Angle to apply rotation while rendering the rhombus</param>
        public static void DrawRhombus(this IWritable buffer, float x1, float y1, float x2, float y2, float x3, float y3) =>
            RenderRhombus(buffer, new VectorF(x1, y1), new VectorF(x2, y2), new VectorF(x3, y3), null);
        #endregion

        #region DRAW TRAPEZIUM
        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="baseLine">A line from where the trapezium start</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, ILine baseLine, float parallelLineDeviation, float skewBy, IReadContext context) =>
            RenderTrapezium(buffer, baseLine, parallelLineDeviation, skewBy, context);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        public static void DrawTrapezium(this IWritable buffer, ILine baseLine, float parallelLineDeviation, float skewBy) =>
            RenderTrapezium(buffer, baseLine, parallelLineDeviation, skewBy, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        public static void DrawTrapezium(this IWritable buffer, ILine baseLine, float parallelLineDeviation) =>
            RenderTrapezium(buffer, baseLine, parallelLineDeviation, 0, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="baseLine">A line from where the trapezium start</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, ILine baseLine, float parallelLineDeviation, IReadContext context) =>
            RenderTrapezium(buffer, baseLine, parallelLineDeviation, 0, context);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by the first four values in values parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="values">An array of float values- first four of which forms a base line from where the trapezium start.
        /// Parallel line deviation should be the 5th i.e values[4] item in values parameter if values has length of 5 otherwise deemed as zero which results in a simple line draw.
        /// 6th item i.e values[5] in values would form parallel Line Size Difference value if the lenght of values is 6 otherwise zero.
        /// </param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, float[] values, IReadContext context)
        {
            if (values.Length < 4)
                return;

            var first = new Line(values[0], values[1], values[2], values[3]);
            float parallelLineDeviation = 30f;
            float skewBy = 0;
            if (values.Length > 4)
                parallelLineDeviation = values[4];
            if (values.Length > 5)
                skewBy = values[5];
            RenderTrapezium(buffer, first, parallelLineDeviation, skewBy, context);
        }

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by the first four values in values parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="values">An array of int values- first four of which forms a base line from where the trapezium start.
        /// Parallel line deviation should be the 5th i.e values[4] item in values parameter if values has length of 5 otherwise deemed as zero which results in a simple line draw.
        /// 6th item i.e values[5] in values would form parallel Line Size Difference value if the lenght of values is 6 otherwise zero.
        /// </param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, int[] values, IReadContext context)
        {
            if (values.Length < 4)
                return;

            var first = new Line(values[0], values[1], values[2], values[3]);
            float deviation = 30f;
            float skewBy = 0;
            if (values.Length > 4)
                deviation = values[4];
            if (values.Length > 5)
                skewBy = values[5];
            RenderTrapezium(buffer, first, deviation, skewBy, context);
        }

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by the first four values in values parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="values">An array of float values- first four of which forms a base line from where the trapezium start.
        /// Parallel line deviation should be the 5th i.e values[4] item in values parameter if values has length of 5 otherwise deemed as zero which results in a simple line draw.
        /// 6th item i.e values[5] in values would form parallel Line Size Difference value if the lenght of values is 6 otherwise zero.
        /// </param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        public static void DrawTrapezium(this IWritable buffer, float[] values) =>
            buffer.DrawTrapezium(values, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by the first four values in values parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="values">An array of int values- first four of which forms a base line from where the trapezium start.
        /// Parallel line deviation should be the 5th i.e values[4] item in values parameter if values has length of 5 otherwise deemed as zero which results in a simple line draw.
        /// 6th item i.e values[5] in values would form parallel Line Size Difference value if the lenght of values is 6 otherwise zero.
        /// </param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, int[] values) =>
            buffer.DrawTrapezium(values, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from parameters x1, y1, x2, y2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, float x1, float y1, float x2, float y2,
            float parallelLineDeviation, float skewBy, IReadContext context) =>
            RenderTrapezium(buffer, new Line(x1, y1, x2, y2), parallelLineDeviation, skewBy, context);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from parameters x1, y1, x2, y2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="deviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        public static void DrawTrapezium(this IWritable buffer, float x1, float y1, float x2, float y2, float deviation, float skewBy) =>
            RenderTrapezium(buffer, new Line(x1, y1, x2, y2), deviation, skewBy, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from parameters x1, y1, x2, y2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, float x1, float y1, float x2, float y2, float parallelLineDeviation, IReadContext context) =>
            RenderTrapezium(buffer, new Line(x1, y1, x2, y2), parallelLineDeviation, 0, context);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from parameters x1, y1, x2, y2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        public static void DrawTrapezium(this IWritable buffer, float x1, float y1, float x2, float y2, float parallelLineDeviation) =>
            RenderTrapezium(buffer, new Line(x1, y1, x2, y2), parallelLineDeviation, 0, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, VectorF p1, VectorF p2, float parallelLineDeviation, float skewBy, IReadContext context) =>
            RenderTrapezium(buffer, new Line(p1, p2), parallelLineDeviation, skewBy, context);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, Vector p1, Vector p2, float parallelLineDeviation, float skewBy, IReadContext context) =>
            RenderTrapezium(buffer, new Line(p1.X, p1.Y, p2.X, p2.Y), parallelLineDeviation, skewBy, context);


        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        public static void DrawTrapezium(this IWritable buffer, VectorF p1, VectorF p2, float parallelLineDeviation, float skewBy) =>
            RenderTrapezium(buffer, new Line(p1, p2), parallelLineDeviation, skewBy, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        public static void DrawTrapezium(this IWritable buffer, Vector p1, Vector p2, float parallelLineDeviation, float skewBy) =>
            RenderTrapezium(buffer, new Line(p1.X, p1.Y, p2.X, p2.Y), parallelLineDeviation, skewBy, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, VectorF p1, VectorF p2, float parallelLineDeviation, IReadContext context) =>
            RenderTrapezium(buffer, new Line(p1, p2), parallelLineDeviation, 0, context);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, Vector p1, Vector p2, float parallelLineDeviation, IReadContext context) =>
            RenderTrapezium(buffer, new Line(p1.X, p1.Y, p2.X, p2.Y), parallelLineDeviation, 0, context);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        public static void DrawTrapezium(this IWritable buffer, VectorF p1, VectorF p2, float parallelLineDeviation) =>
            RenderTrapezium(buffer, new Line(p1, p2), parallelLineDeviation, 0, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        public static void DrawTrapezium(this IWritable buffer, Vector p1, Vector p2, float parallelLineDeviation) =>
            RenderTrapezium(buffer, new Line(p1.X, p1.Y, p2.X, p2.Y), parallelLineDeviation, 0, null);
        #endregion

        #region DRAW POLYGON
        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon  an each group of two subsequent values in polypoints forms a point x,y</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        public static void DrawPolygon(this IWritable buffer, IReadContext context, params float[] polyPoints) =>
            RenderPolygon(buffer, polyPoints, context);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon an each group of two subsequent values in polypoints forms a point x,y</param>
        public static void DrawPolygon(this IWritable buffer, params float[] polyPoints) =>
            RenderPolygon(buffer, polyPoints, null);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon an each group of two subsequent values in polypoints forms a point x,y</param>
        public static void DrawPolygon(this IWritable buffer, params int[] polyPoints) =>
            RenderPolygon(buffer, polyPoints.Select(p => (float)p), null);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon  an each group of two subsequent values in polypoints forms a point x,y</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        public static void DrawPolygon(this IWritable buffer, IReadContext context, params int[] polyPoints) =>
            RenderPolygon(buffer, polyPoints.Select(p => (float)p), context);
        #endregion

        #region DRAW TEXT
        /// <summary>
        /// Draw text on a specified buffer using specified parameters.
        /// </summary>
        /// <param name="buffer">buffer which to render a rhombus on</param>
        /// <param name="font">Font object to get glyphs collection for a given text</param>
        /// <param name="destX">X cordinate of destination point where text gets drawn</param>
        /// <param name="destY">Y cordinate of destination point where text gets drawn</param>
        /// <param name="text">A string of characters to draw</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="drawStyle">A draw style to be used to draw text</param>
        /// <returns>GlyphsData object which contains a draw result information such as glyphs, drawn area etc.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Glyphs DrawText(this IWritable buffer, IFont font, float destX, float destY, string text,
            IReadContext context = null, TextDrawStyle drawStyle = null)
        {
            if (buffer == null || font == null || string.IsNullOrEmpty(text))
                return Glyphs.Empty;
            var info = font.MeasureText(text, destX, destY, drawStyle);
            buffer.Render(info, context);
            return info;
        }

        /// <summary>
        /// Renders a text object which represents a text and a collection of glyphs providing drawing representation of the text. 
        /// </summary>
        /// <param name="buffer">Buffer which to render a rhombus on</param>
        /// <param name="text">A text object to render</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawText(this IWritable buffer, IGlyphs text, IReadContext context = null, int? drawX = null, int? drawY = null)
        {
            buffer.Render(text, context, drawX, drawY);
        }
        #endregion

        #region PORTION
        public static unsafe Size Portion(this ICopyable block, out int[] data, int? x = null, int? y = null, int? w = null, int? h = null)
        {
            if (block == null)
            {
                data = null;
                return Size.Empty;
            }
            var rc = block.CompitibleRc(x, y, w, h);
            data = new int[rc.Width * rc.Height];
            fixed (int* p = data)
            {
                block.CopyTo(rc.X, rc.Y, rc.Width, rc.Height, (IntPtr)p, data.Length, rc.Width, 0, 0);
            }
            return new Size(rc);
        }
        public static unsafe Size Portion(this ICopyable block, out IntPtr result,
            int? x = null, int? y = null, int? w = null, int? h = null)
        {
            if (block == null)
            {
                result = IntPtr.Zero;
                return Size.Empty;
            }
            var rc = block.CompitibleRc(x, y, w, h);
            int[] data = new int[rc.Width * rc.Height];
            fixed (int* p = data)
            {
                result = (IntPtr)p;
                block.CopyTo(rc.X, rc.Y, rc.Width, rc.Height, result, data.Length, rc.Width, 0, 0);
            }
            return new Size(rc);
        }
        #endregion

        #region WRITE IMAGE
        /// <summary>
        /// Writes a given memory block to a file on a given path.
        /// </summary>
        /// <param name="image">Memory block to write to disk file</param>
        /// <param name="path">Path of a file to create and write dat to</param>
        /// <param name="format">Format of the targeted image file</param>
        /// <param name="quality">Resolution quality of the tageted image file</param>
        public static void Write(this IImageProcessor writer, ICopyable image, string path, ImageFormat format, int pitch = 4, int quality = 50)
        {
            try
            {
                using (var stream = File.Open(path, FileMode.OpenOrCreate))
                {
                    writer.Write(image, stream, format, pitch, quality);
                }
            }
            catch { }
        }
        
        /// <summary>
        /// Writes a given memory block to a file on a given path.
        /// </summary>
        /// <param name="image">Memory block to write to disk file</param>
        /// <param name="format">Format of the targeted image file</param>
        /// <param name="quality">Resolution quality of the tageted image file</param>
        public static unsafe void Write(this IImageProcessor writer, ICopyable image, Stream dest, ImageFormat format, int pitch = 4, int quality = 50)
        {
            int[] data = new int[image.Width * image.Height];
            fixed (int* p = data)
            {
                IntPtr pixels = (IntPtr)p;
                image.CopyTo(0, 0, image.Width, image.Height, pixels, image.Length, image.Width, 0, 0);
                writer.Write(pixels, image.Width, image.Height, image.Length, pitch, dest, format, quality);
            }
        }

        /// <summary>
        /// Writes a given memory block to a file on a given path.
        /// </summary>
        /// <param name="pixels">Memory block to write to disk file</param>
        /// <param name="width">Width of memory block</param>
        /// <param name="height"></param>
        /// <param name="path">Path of a file to create and write dat to</param>
        /// <param name="format">Format of the targeted image file</param>
        /// <param name="quality">Resolution quality of the tageted image file</param>
        public static unsafe void Write(this IImageProcessor writer, int[] pixels, int width, int height, string path, ImageFormat format, int pitch = 4, int quality = 50)
        {
            try
            {
                using (var stream = File.Open(path, FileMode.OpenOrCreate))
                {
                    fixed (int* p = pixels)
                    {
                        writer.Write((IntPtr)p, width, height, pixels.Length, pitch, stream, format, quality);
                    }
                }
            }
            catch { }
        }
        #endregion

        #region SAVE AS
        /// <summary>
        /// Saves entire image or a portion of it with or without backgound to a disk file in a specified image format.
        /// </summary>
        /// <param name="image">Image which is to be saved.</param>
        /// <param name="file">Path of a file to create and write data to</param>
        /// <param name="format">Format of the targeted image file</param>
        /// <param name="withBackground"></param>
        /// <param name="portion">Null represents a whole chunk of memory block. Other wise a prtion determined by location X, Y and size Width, Height of the portion rectangle</param>
        /// <param name="pitch">Pitch of the image - default is 4 - R, G, B, A</param>
        /// <param name="quality">Resolution quality of the tageted image file</param>
        public static unsafe void SaveAs(this ICopyable block, string file,
            ImageFormat format = ImageFormat.BMP, Rectangle? portion = null, int pitch = 4, int quality = 50)
        {
            file += "." + format.ToString();
            Size size;

#if Window
            if (format == ImageFormat.BMP)
            {
                if (portion == null)
                    Factory.SaveAsBitmap(block, file);
                else
                {
                    size = block.Portion(out IntPtr ptr, portion?.X, portion?.Y, portion?.Width, portion?.Height);
                    Factory.SaveAsBitmap(ptr, size.Width, size.Height, file);
                }
                return;
            }
#endif
            size = block.Portion(out int[] data, portion?.X, portion?.Y, portion?.Width, portion?.Height);
            Factory.ImageProcessor.Write(data, size.Width, size.Height, file, format, pitch, quality);
        }
        #endregion
    }
    partial class Renderer
    {
        #region RENDER CIRCLE OR ELLIPSE
        static void RenderCircleOrEllipse(this IWritable buffer, float x, float y, float width, float height,
            IReadContext context = null)
        {
            if (buffer == null)
                return;
            var curve = new Curve(x, y, width, height, 0, 0, CurveType.Full, buffer.Settings.Rotation, buffer.Settings.Scale);

            DrawCurve(buffer, curve, context);
        }
        static void RenderCircleOrEllipse(this IWritable buffer, VectorF first, VectorF second, VectorF third, IReadContext context = null,
            CurveType type = CurveType.Full)
        {
            if (buffer == null)
                return;
            type = type.Exclude(CurveType.Arc).Exclude(CurveType.Pie).Exclude(CurveType.ClosedArc);
            type = type.Include(CurveType.Full);
            var curve = new Curve(first, second, third, type, buffer.Settings.Rotation, buffer.Settings.Scale);
            DrawCurve(buffer, curve, context);
        }

        static void RenderCircleOrEllipse(this IWritable buffer, VectorF first, VectorF second, VectorF third, VectorF fourth,
            IReadContext context = null, CurveType type = CurveType.Full)
        {
            if (buffer == null)
                return;
            type = type.Exclude(CurveType.Arc).Exclude(CurveType.Pie).Exclude(CurveType.ClosedArc);
            type = type.Include(CurveType.Full);
            var curve = new Curve(first, second, third, fourth, type, buffer.Settings.Rotation, buffer.Settings.Scale);
            DrawCurve(buffer, curve, context);
        }
        static void RenderCircleOrEllipse(this IWritable buffer, VectorF first, VectorF second, VectorF third, VectorF fourth, VectorF fifth,
            IReadContext context = null)
        {
            if (buffer == null)
                return;
            var curve = new Curve(first, second, third, fourth, fifth, CurveType.Full, buffer.Settings.Rotation, buffer.Settings.Scale);
            DrawCurve(buffer, curve, context);
        }
        #endregion

        #region RENDER ARC - PIE
        /// <summary>
        /// Renders an arc or pie specified by the bounding area and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="buffer">Buffer which to render an arc/pie on</param>
        /// <param name="x">X cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="width">Width of a bounding area where the circle/ellipse is to be drawn -> circle/ellipse's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the circle is to be drawn ->circle/ellipse's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. If type includes NoSweepAngle option otherwise effective end angle is start angle + end angle</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        static void RenderArcOrPie(this IWritable buffer, float x, float y, float width, float height,
            float startAngle, float endAngle, IReadContext context = null, CurveType type = CurveType.Pie)
        {
            if (buffer == null)
                return;

            type = type.Exclude(CurveType.Full);

            var curve = new Curve(x, y, width, height, startAngle, endAngle, type, buffer.Settings.Rotation, buffer.Settings.Scale);
            DrawCurve(buffer, curve, context);
        }

        /// <summary>
        /// Renders an arc or pie specified by three points and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="buffer">Buffer which to render a circle/ellipse on</param>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        static void RenderArcOrPie(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, IReadContext context = null, CurveType type = CurveType.Pie)
        {
            if (buffer == null)
                return;
            type = type.Exclude(CurveType.Full);

            var curve = new Curve(p1, p2, p3, type, buffer.Settings.Rotation, buffer.Settings.Scale);
            DrawCurve(buffer, curve, context);
        }

        /// <summary>
        /// Renders an arc or pie specified by four points and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="buffer">Buffer which to render a circle/ellipse on</param>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        static void RenderArcOrPie(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, VectorF p4, IReadContext context = null, CurveType type = CurveType.Pie)
        {
            if (buffer == null)
                return;
            type = type.Exclude(CurveType.Full);
            var curve = new Curve(p1, p2, p3, p4, type, buffer.Settings.Rotation, buffer.Settings.Scale);
            DrawCurve(buffer, curve, context);
        }

        /// <summary>
        /// Renders an arc or pie specified by five points conic section and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="buffer">Buffer which to render a circle/ellipse on</param>
        /// <param name="first">First point</param>
        /// <param name="second">Second point</param>
        /// <param name="third">Third point</param>
        /// <param name="fourth">Fourth point</param>
        /// <param name="fifth">Fifth point</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        static void RenderArcOrPie(this IWritable buffer, VectorF first, VectorF second, VectorF third, VectorF fourth,
            VectorF fifth, IReadContext context = null, CurveType type = CurveType.Pie)
        {
            if (buffer == null)
                return;
            var curve = new Curve(first, second, third, fourth, fifth, type, buffer.Settings.Rotation, buffer.Settings.Scale);
            DrawCurve(buffer, curve, context);
        }
        #endregion

        #region RENDER BEZIER
        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">Buffer which to render a bezier on</param>
        /// <param name="pts">Defines perimiter of the bezier</param>
        /// <param name="type">BezierType enum determines the type of bezier i.e Cubic - group of 4 points or multiple(group of 4 or 7 or 10 so on...)</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the bezier</param>
        static void RenderBezier(this IWritable buffer, IEnumerable<float> pts, BezierType type = BezierType.Cubic,
            IReadContext context = null)
        {
            if (buffer == null)
                return;

            var bezier = new Bezier(type, pts.ToArray(), null);
            buffer.Render(bezier, context);
        }
        #endregion

        #region RENDER TRINAGLE
        /// <summary>
        /// Renders a trianle formed by three points specified by x1,y1 & x2,y2 & x3,y3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">Buffer which to render a triangle on</param>
        /// <param name="x1">X corodinate of the first point</param>
        /// <param name="y1">Y corodinate of the first point</param>
        /// <param name="x2">X corodinate of the second point</param>
        /// <param name="y2">Y corodinate of the second point</param>
        /// <param name="x3">X corodinate of the third point</param>
        /// <param name="y3">Y corodinate of the third point</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the traingle</param>
        static void RenderTriangle(this IWritable buffer, float x1, float y1, float x2, float y2, float x3, float y3, IReadContext context)
        {
            if (buffer == null)
                return;
            var triangle = new Triangle(x1, y1, x2, y2, x3, y3);
            buffer.Render(triangle, context);
        }
        #endregion

        #region RENDER POLYGON
        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">Buffer which to render a polygon on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon an each group of two subsequent values in polypoints forms a point x,y</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the polygon</param>
        static void RenderPolygon(this IWritable buffer, IEnumerable<float> polyPoints, IReadContext context)
        {
            if (buffer == null)
                return;
            IList<VectorF> points = polyPoints.ToPoints();
            buffer.Render(new Shape(points, "Polygon"), context);
        }
        #endregion

        #region RENDER RECTANGLE
        /// <summary>
        /// Renders a rectangle specified by x, y, width, height parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">Buffer which to render a rectangle on</param>
        /// <param name="x">X cordinate of the rectangle</param>
        /// <param name="y">Y cordinate of the rectangle</param>
        /// <param name="width">Width of the rectangle/param>
        /// <param name="height">Height the rectangle</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rectangle</param>
        static void RenderRectangle(this IWritable buffer, float x, float y, float width, float height, IReadContext context)
        {
            if (buffer == null)
                return;
            buffer.Render(new BoxF(x, y, width, height), context);
        }
        #endregion

        #region RENDER ROUNDED BOX
        /// <summary>
        /// Renders a rounded box specified by x, y, width, height parameters and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="buffer">Buffer which to render a rounded box on</param>
        /// <param name="x">X cordinate of the rounded box</param>
        /// <param name="y">Y cordinate of the rounded box</param>
        /// <param name="width">Width of the rounded box/param>
        /// <param name="height">Height the rounded box</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rounded box</param>
        static void RenderRoundedBox(this IWritable buffer, float x, float y, float width, float height, float cornerRadius,
            IReadContext context)
        {
            if (buffer == null)
                return;

            var pts = Curves.RoundedBoxPoints(x, y, width, height, cornerRadius);
            buffer.Render(new Shape(pts, "RoundBox"), context);
        }
        #endregion

        #region RENDER RHOMBUS
        /// <summary>
        /// Renders a rhombus specified by x, y, width, height parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">Buffer which to render a rhombus on</param>
        /// <param name="x">X cordinate of the bounding rectangle</param>
        /// <param name="y">Y cordinate of the bounding rectangle</param>
        /// <param name="width">Width of the bounding rectangle/param>
        /// <param name="height">Height the bounding rectangle</param>
        /// <param name="angle">Angle to apply rotation while rendering the rhombus</param>
        /// <param name="deviation">If not zero, it replaces the value of width parameter</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        static void RenderRhombus(this IWritable buffer, float x, float y, float width, float height, float? deviation, IReadContext context) =>
            RenderRectangle(buffer, x, y, (deviation ?? width), height, context);

        /// <summary>
        /// Renders a rhombus specified by three points (The fourth point will be calculated) and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">Buffer which to render a rhombus on</param>
        /// <param name="first">First point</param>
        /// <param name="second">Second point</param>
        /// <param name="third">Third point</param>
        /// <param name="angle">Angle to apply rotation while rendering the rhombus</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        static void RenderRhombus(this IWritable buffer, VectorF first, VectorF second, VectorF third, IReadContext context)
        {
            var rhombus = new Tetragon(first, second, third);
            buffer.Render(rhombus, context);
        }
        #endregion

        #region RENDER TRAPEZIUM
        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">Buffer which to render a trapezium on</param>
        /// <param name="baseLine">A line from where the trapezium start</param>
        /// <param name="deviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skeyBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        static void RenderTrapezium(this IWritable buffer, ILine baseLine, float deviation, float skeyBy, IReadContext context)
        {
            if (buffer == null)
                return;
            var trapezium = new Tetragon(baseLine, deviation, buffer.Settings.StrokeMode, skeyBy);
            buffer.Render(trapezium, context);
        }
        #endregion
    }
    partial class Renderer
    {
        #region CREATE LINE ACTION
        /// <summary>
        /// Retuns an action delegate for storing an axial line or pixel information in specified list.
        /// </summary>
        /// <param name="list">A list to accumulate pixels and axial lines resulted from executing an action</param>
        /// <returns>An instance of FillAction delegate</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateAction(this ICollection<VectorF> list, out PixelAction<float> action)
        {
            action = (val1, axis, horizontal) =>
            {
                list.Add(new VectorF(val1, axis, horizontal));
            };
        }

        /// <summary>
        /// Retuns an action delegate for storing an axial line or pixel information in specified list.
        /// </summary>
        /// <param name="list">A list to accumulate pixels and axial lines resulted from executing an action</param>
        /// <returns>An instance of FillAction delegate</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateAction(this ICollection<Vector> list, out PixelAction<int> action)
        {
            action = (val1, axis, horizontal) =>
            {
                list.Add(new Vector(val1, axis, horizontal));
            };
        }

        /// <summary>
        /// Retuns an action delegate for storing an axial line or pixel information in specified list.
        /// </summary>
        /// <param name="list">A list to accumulate pixels and axial lines resulted from executing an action</param>
        /// <returns>An instance of FillAction delegate</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateAction(this ICollection<Vector> list, out VectorAction<int> action)
        {
            action = (x, y) =>
            {
                list.Add(new Vector(x, y));
            };
        }

        /// <summary>
        /// Retuns an action delegate for storing an axial line or pixel information in specified list.
        /// </summary>
        /// <param name="list">A list to accumulate pixels and axial lines resulted from executing an action</param>
        /// <returns>An instance of FillAction delegate</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateAction(this ICollection<VectorF> list, out VectorAction<float> action)
        {
            action = (x, y) =>
            {
                list.Add(new VectorF(x, y));
            };
        }

        #endregion

        #region PROCESS LINE
        /// <summary>
        /// Processes a line using standard line algorithm between two points of a line segment using specified action.
        /// </summary>
        /// <param name="line">A line to render</param>
        /// <param name="action">A FillAction delegate which has routine to do something with the information emerges by using standard line algorithm</param>
        /// <param name="skip">LineSkip option used to filter the lines so that shallow and steep gradients can be processed seperately.</param>
        /// If current stroke value is other than 0, it results in a rendering of a trapezium instead of the line</param>
        public static void Process(this ILine line, PixelAction<float> action, LineCommand lineCommand, SlopeType skip = SlopeType.None)
        {
            if (line == null || !line.Valid)
                return;
            ProcessLine(line.X1, line.Y1, line.X2, line.Y2, action, lineCommand, skip);
        }

        /// <summary>
        /// Processes a line using standard line algorithm between two points of a line segment using specified action.
        /// </summary>
        /// <param name="line">A line to render</param>
        /// <param name="action">A FillAction delegate which has routine to do something with the information emerges by using standard line algorithm</param>
        /// <param name="skip">LineSkip option used to filter the lines so that shallow and steep gradients can be processed seperately.</param>
        /// If current stroke is other than 0, it results in a rendering of a trapezium instead of the line</param>
        public static void Process(this ILine line, PixelAction<int> action, LineCommand lineCommand, SlopeType skip = SlopeType.None)
        {
            if (line == null || !line.Valid)
                return;
            ProcessLine(line.X1.Round(), line.Y1.Round(), line.X2.Round(), line.Y2.Round(), action, lineCommand, skip);
        }

        /// <summary>
        /// Processes a line using GWS line algorithm between two points specified by x1, y1 and x2, y2 using specified action.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="action">A Vector action delegate which has routine to do something with the information emerges by using GWS line algorithm</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessLine(int x1, int y1, int x2, int y2, VectorAction<int> action, LineCommand lineCommand)
        {
            ProcessLine(x1, y1, x2, y2, action.ToPixelAction(), lineCommand);
        }

        /// <summary>
        /// Processes a line using GWS line algorithm between two points specified by x1, y1 and x2, y2 using specified action.
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="action">A Vector action delegate which has routine to do something with the information emerges by using GWS line algorithm</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessLine(float x1, float y1, float x2, float y2, VectorAction<float> action, LineCommand lineCommand)
        {
            ProcessLine(x1, y1, x2, y2, action.ToPixelAction(), lineCommand);
        }
        #endregion

        #region PROCESS LINES
        /// <summary>
        /// Processes a collection of lines using standard line algorithm between two points of a line segment using specified action.
        /// </summary>
        /// <param name="lines">Collection of lines to render</param>
        /// <param name="action">A FillAction delegate which has routine to do something with the information emerges by using standard line algorithm</param>
        /// <param name="skip">LineSkip option used to filter the lines so that shallow and steep gradients can be processed seperately.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Process(this IEnumerable<ILine> lines, PixelAction<float> action, LineCommand lineCommand, SlopeType skip = SlopeType.None)
        {
            if (lines == null || skip == SlopeType.Both)
                return;

            foreach (var l in lines)
                Process(l, action, lineCommand, skip);
        }
        #endregion

        #region SCAN LINES
        /// <summary>
        /// Scans a collection of lines using standard line algorithm between two points of a line segment using specified action.
        /// While scanning each line, the processing will not exceed the boundaries defined by min and max values.
        /// For example if horizontal then Start = MinY and End = MaxY. Let say it is 100 and 300 respectively.
        /// Now while processing a line say 50, 99, 90, 301, position of y1 at 99 and y2 at 301 will be ignored as they do not fall in the range of 100 -300.
        /// </summary>
        /// <param name="lines">Collection of lines to render</param>
        /// <param name="scanAction">A FillAction delegate which has routine to do something with the information emerges by using standard line algorithm</param>
        /// <param name="horizontalScan">If null, steepness of line would be scan mode, else if true, Lines should be scanned from top to bottom otherwise left to right</param>
        public static void ScanLines(this IEnumerable<ILine> lines, PixelAction<float> scanAction, bool? horizontalScan)
        {
            if (lines == null)
                return;

            foreach (var line in lines)
            {
                if (line == null)
                    continue;
                ScanLine(line.X1, line.Y1, line.X2, line.Y2, horizontalScan, scanAction);
            }
        }
        #endregion

        #region POLY FILL SCAN
        /// <summary>
        /// Includes lines in filling operation.
        /// </summary>
        /// <param name="lines">Collection of lines to include in filling operation.</param>
        public static void Scan(this IPolyFill polyFill, params ILine[] lines)
        {
            foreach (var line in lines)
                polyFill.Scan(line.X1, line.Y1, line.X2, line.Y2);
        }

        /// <summary>
        /// Includes lines for filling operation.
        /// </summary>
        /// <param name="lines">Collection of lines to include in filling operation.</param>
        public static void Scan(this IPolyFill polyFill, IEnumerable<ILine> lines)
        {
            if (lines == null)
                return;
            foreach (var line in lines)
                polyFill.Scan(line.X1, line.Y1, line.X2, line.Y2);
        }

        public static void Scan(this IPolyFill polyFill, VectorF a, VectorF b)
        {
            polyFill.Scan(a.X, a.Y, b.X, b.Y);
        }
        public static void Scan(this IPolyFill polyFill, Vector a, Vector b)
        {
            polyFill.Scan(a.X, a.Y, b.X, b.Y);
        }
        #endregion

        #region PROCESS TRIANGLE
        /// <summary>
        /// Scan triangle lines horizontally and performs fill action.
        /// </summary>
        /// <param name="p1">1st point of triangle.</param>
        /// <param name="p2">2st point of triangle.</param>
        /// <param name="p3">3st point of triangle.</param>
        /// <param name="Action">A FillAction delegate which has routine to do something with the axial scan line information provided.</param>
        /// <param name="drawOutLines">>If true, border around filled area will get drawn.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessTriangle(VectorF p1, VectorF p2, VectorF p3, FillAction<float> Action, IDrawSettings Settings, bool drawOutLines = true)
        {
            bool drawOutLinesOnly = Settings.FillMode == FillMode.DrawOutLine;
            drawOutLines = drawOutLines || drawOutLinesOnly;
            SlopeType steep = drawOutLinesOnly ? 0 : SlopeType.Steep;

            if (drawOutLines)
            {
                var pixelAction = Action.ToPixelAction();
                ProcessLine(p1.X, p1.Y, p2.X, p2.Y, pixelAction, Settings.LineCommand, steep);
                ProcessLine(p2.X, p2.Y, p3.X, p3.Y, pixelAction, Settings.LineCommand, steep);
                ProcessLine(p3.X, p3.Y, p1.X, p1.Y, pixelAction, Settings.LineCommand, steep);
            }

            if (drawOutLinesOnly)
                return;


            Vectors.MinMax(Settings.Clip, out _, out float minY, out _, out float maxY, p1, p2, p3);
            using (var polyFill = Factory.newPolyFill())
            {
                Settings.BrushCommand |= BrushCommand.IgnoreAutoCalculatedFillPatten;

                polyFill.Begin(minY.Round(), (int)maxY + 1, Settings.FillCommand | FillCommand.OddEvenPolyFill);
                ScanLine(p1.X, p1.Y, p2.X, p2.Y, true, polyFill.ScanAction);
                ScanLine(p2.X, p2.Y, p3.X, p3.Y, true, polyFill.ScanAction);
                ScanLine(p3.X, p3.Y, p1.X, p1.Y, true, polyFill.ScanAction);

                polyFill.Fill(Action, Settings.LineCommand);
                polyFill.End();
                Settings.BrushCommand &= ~BrushCommand.IgnoreAutoCalculatedFillPatten;
            }
        }
        #endregion

        #region PROCESS QUADRILATERAL
        /// <summary>
        /// Scan Quardilateral lines horizontally and performs fill action.
        /// </summary>
        /// <param name="p1">1st point of triangle.</param>
        /// <param name="p2">2st point of triangle.</param>
        /// <param name="p3">3st point of triangle.</param>
        /// <param name="p4">4nd point of qardilateral.</param>
        /// <param name="Action">A FillAction delegate which has routine to do something with the axial scan line information provided.</param>
        /// <param name="drawOutLines">>If true, border around filled area will get drawn.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessQuardilateral(VectorF p1, VectorF p2, VectorF p3, VectorF p4, FillAction<float> Action, IDrawSettings Settings, bool drawOutLines = true)
        {
            bool drawOutLinesOnly = Settings.FillMode == FillMode.DrawOutLine;
            drawOutLines = drawOutLines || drawOutLinesOnly;
            SlopeType steep = drawOutLinesOnly ? 0 : SlopeType.Steep;

            if (drawOutLines)
            {
                var pixelAction = Action.ToPixelAction();
                ProcessLine(p1.X, p1.Y, p2.X, p2.Y, pixelAction, Settings.LineCommand, steep);
                ProcessLine(p3.X, p3.Y, p4.X, p4.Y, pixelAction, Settings.LineCommand, steep);
                ProcessLine(p1.X, p1.Y, p3.X, p3.Y, pixelAction, Settings.LineCommand, steep);
                ProcessLine(p2.X, p2.Y, p4.X, p4.Y, pixelAction, Settings.LineCommand, steep);
            }

            if (drawOutLinesOnly)
                return;

            Vectors.MinMax(Settings.Clip, out float minX, out float minY, out float maxX, out float maxY, p1, p2, p3, p4);

            using (var PolyFill = Factory.newPolyFill())
            {
                Settings.BrushCommand |= BrushCommand.IgnoreAutoCalculatedFillPatten;
                PolyFill.Begin(minY.Round(), (int)maxY + 1, Settings.FillCommand| FillCommand.OddEvenPolyFill);

                ScanLine(p1.X, p1.Y, p2.X, p2.Y, true, PolyFill.ScanAction);
                ScanLine(p4.X, p4.Y, p3.X, p3.Y, true, PolyFill.ScanAction);

                ScanLine(p1.X, p1.Y, p3.X, p3.Y, true, PolyFill.ScanAction);
                ScanLine(p2.X, p2.Y, p4.X, p4.Y, true, PolyFill.ScanAction);

                PolyFill.Fill(Action, Settings.LineCommand);
                PolyFill.End();
                Settings.BrushCommand &= ~BrushCommand.IgnoreAutoCalculatedFillPatten;
            }
        }
        #endregion

        #region PROCESS RHOMBUS
        /// <summary>
        /// Scan rhombus lines horizontally and performs fill action.
        /// </summary>
        /// <param name="p1">1st point of triangle.</param>
        /// <param name="p2">2st point of triangle.</param>
        /// <param name="p3">3st point of triangle.</param>
        /// <param name="Action">A FillAction delegate which has routine to do something with the axial scan line information provided.</param>
        /// <param name="drawOutLines">>If true, border around filled area will get drawn.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessRhombus(VectorF p1, VectorF p2, VectorF p3, FillAction<float> Action, IDrawSettings Settings, bool drawOutLines = true)
        {
            var p4 = Vectors.FourthPointOfRhombus(p1, p2, p3);
            ProcessQuardilateral(p1, p2, p3, p4, Action, Settings, drawOutLines);
        }
        #endregion

        #region PROCESS CONIC
        /// <summary>
        /// Process conic - notifying each obtained axial scan line by executing specified action.
        /// </summary>
        /// <param name="conic">Conic object to process.</param>
        /// <param name="action">A FillAction delegate which has routine to do something with the axial scan line information provided.</param>
        /// <param name="forDrawingOnly">If true, only ends point of axial line will be drawn otherwise full line.</param>
        public static void Process(this IConic conic, FillAction<float> action, IDrawSettings Settings, bool forDrawingOnly)
        {
            if (action == null)
                return;
            CurveType type = CurveType.Full;
            if (conic is ICurve)
            {
                type = (conic as ICurve).Type;
            }

            bool Full = !(type.HasFlag(CurveType.Arc) || type.HasFlag(CurveType.Pie));

            if (Settings.Stroke == 0 && type.HasFlag(CurveType.Arc))
                forDrawingOnly = true;

            Settings.FillCommand |= FillCommand.DrawEndsOnly;
            Settings.FillCommand |= FillCommand.FillSinglePointLine;

            using (var PolyFill = Factory.newPolyFill())
            {
                int i;
                int a1, a2;
                PolyFill.FillCommand = Settings.FillCommand;
                i = conic.GetBoundary(false, true);
                while (i >= 0)
                {
                    var list = conic.GetDataAt(i, false, true, out a1, out a2);
                    PolyFill.FillLine(list[0], a1, false, action);
                    PolyFill.FillLine(list[1], a2, false, action);
                    i -= 1;
                }

                if (!forDrawingOnly)
                    Settings.FillCommand &= ~FillCommand.DrawEndsOnly;

                Settings.FillCommand &= ~FillCommand.DrawLineOnly;
                PolyFill.FillCommand = Settings.FillCommand;

                i = conic.GetBoundary(true, forDrawingOnly);

                while (i >= 0)
                {
                    var list = conic.GetDataAt(i, true, forDrawingOnly, out a1, out a2);
                    PolyFill.FillLine(list[0], a1, true, action);
                    PolyFill.FillLine(list[1], a2, true, action);
                    i -= 1;
                }
            }
            if (!Full && conic is ICurve)
            {
                var lines = (conic as ICurve).GetClosingLines();
                if (lines != null)
                {
                    var paction = action.ToPixelAction();
                    foreach (var line in lines)
                        Process(line, paction, Settings.LineCommand);
                }
            }
        }
        #endregion

        #region PROCESS OUT LINES
        /// <summary>
        /// Fills the area between specified two collections of lines.
        /// Filling is done by scanning each line of outer perimeter with correspoinding line of inner perimeter at given index.
        /// </summary>
        /// <param name="Outer">Outer perimeter of shape.</param>
        /// <param name="Inner">Inner perimeter of shape.</param>
        /// <param name="action">A FillAction delegate which has routine to do something with the information emerges by using breshenham line algorithm</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessWith(this IList<ILine> Outer, IList<ILine> Inner, IDrawSettings Settings, FillAction<float> action)
        {
            var length = Math.Min(Outer.Count, Inner.Count);
            VectorF p1, p2, p3, p4;
            for (int i = 0; i < length; i++)
            {
                p1 = new VectorF(Outer[i].X1, Outer[i].Y1);
                p2 = new VectorF(Outer[i].X2, Outer[i].Y2);
                p3 = new VectorF(Inner[i].X1, Inner[i].Y1);
                p4 = new VectorF(Inner[i].X2, Inner[i].Y2);
                ProcessQuardilateral(p1, p2, p3, p4, action, Settings, false);
            }
        }
        #endregion
    }
    partial class Renderer
    {
        #region ANIMATED GIF FRAME
        /// <summary>
        /// Load GIF from file.
        /// </summary>
        /// <param name="path">Path of file containing images.</param>
        /// <param name="x">Width of graphic</param>
        /// <param name="y">Height of graphic</param>
        /// <param name="comp">actual color composition</param>
        /// <param name="requiredComposition">Required color composition</param>
        /// <returns>IAnimatedGifFrame containing GIF data.</returns>
        public static AnimatedGifFrame[] GifFromStream(string path, out int x, out int y, out int comp, int requiredComposition = 4)
        {
            AnimatedGifFrame[] frames = null;
            using (Stream ms = File.Open(path, FileMode.Open))
            {
                frames = GifFromStream(ms, out x, out y, out comp, requiredComposition);
            }
            return frames;
        }
        /// <summary>
        /// Load GIF from file.
        /// </summary>
        /// <param name="path">Path of file containing images.</param>
        /// <param name="requiredComposition">Required color composition</param>
        /// <returns></returns>
        public static Tuple<AnimatedGifFrame[], Vector, int> GifFromStream(string path, int requiredComposition = 4)
        {
            AnimatedGifFrame[] frames = null;
            int x, y;
            int comp;
            using (Stream ms = File.Open(path, FileMode.Open))
            {
                frames = GifFromStream(ms, out x, out y, out comp, requiredComposition);
            }
            return Tuple.Create(frames, new Vector(x, y), comp);
        }
        /// <summary>
        /// Load GIF from file.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="x">Width of graphic</param>
        /// <param name="y">Height of graphic</param>
        /// <param name="comp">actual color composition</param>
        /// <param name="requiredComposition">Required color composition</param>
        /// <returns></returns>
        public static AnimatedGifFrame[] GifFromStream(Stream stream, out int x, out int y, out int comp, int requiredComposition) =>
            STBImage.Processor.ReadAnimatedGif(stream, out x, out y, out comp, requiredComposition);
        #endregion

        #region IMAGE READ
        /// <summary>
        /// Read Image from file and return the image with width and height data.
        /// </summary>
        /// <param name="path">File path of image.</param>
        /// <returns>Returns a Pair containing the image byte array and its width and height.</returns>
        public static Lot<byte[], int, int> ReadImage(string path)
        {
            return Factory.ImageProcessor.Read(path);
        }
        /// <summary>
        /// Read Image and return the image with width and height data.
        /// </summary>
        /// <param name="data">Byte array representing image.</param>
        /// <returns>Returns a Pair containing the image byte array and its width and height.</returns>
        public static Lot<byte[], int, int> ReadImage(byte[] data)
        {
            return Factory.ImageProcessor.Read(data);
        }
        #endregion

        #region ROTATE 
        /// <summary>
        /// Returns a rotated and scalled copy of this object.
        /// </summary>
        /// <param name="buffer">Memory block to rotate and scale.</param>
        /// <param name="angle">Angle of rotation to apply.</param>
        /// <param name="antiAliased">If true copy is antialised version of this object otherwise not.</param>
        /// <param name="scale">Scale to apply.</param>
        /// <returns>Rotated and scalled copy of this object.</returns>
        public static ISurface RotateAndScale(this IScalable buffer, Rotation angle, bool antiAliased = true, float scale = 1)
        {
            var sz = buffer.RotateAndScale(out int[] data, angle, antiAliased, scale);
            return Factory.newSurface(data, sz.Width, sz.Height);
        }
        #endregion

        #region FLIP
        /// <summary>
        /// Returns a flipped version of this object.
        /// </summary>
        /// <param name="flipMode"></param>
        /// <returns>Flipped copy of this object.</returns>
        public static ISurface Flip(this IScalable buffer, Flip flipMode)
        {
            var sz = buffer.Flip(out int[] data, flipMode);
            return Factory.newSurface(data, sz.Width, sz.Height);
        }
        #endregion
    }
#endif
}
