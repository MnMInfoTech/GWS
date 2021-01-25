/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public static partial class Renderer
    {
        #region INSTANCE VARIABLE
        internal const bool Initialize = true;
        #endregion

        #region RENDER
        /// <summary>
        /// Renders any element on this given object. This renderer has a built-in support for the following kind of elements:
        /// 1. IDrawable
        /// 2. IFigurable
        /// 3. IShape
        /// Please note that in case your element does not implement any of the above, you must provide your own rendering routine.
        /// Once you have handled it return true otherwise false.
        /// </summary>
        /// <param name="Renderable">Renderable object which is to be rendered</param>
        /// <param name="Settings">A context which can be a Pen, Rgba color, Brush or RenderInfo object.</param>
        /// <returns>Returns true if this renderer was able to successfully render the element otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Render(this IWritable writable, out long ExecutionTime, IRenderable Renderable, ISettings Settings = null, bool? suspendUpdate = null)
        {
            Stopwatch Watch = new Stopwatch();
            var description = Renderable.ID + " rendering ";
            string message = Benchmarks.Execute(() => 
            writable.Render(Renderable, Settings, suspendUpdate), Watch, out ExecutionTime, description);
            Watch = null;
            return message;
        }

        /// <summary>
        /// Renders multiple elements on this object. This renderer has a built-in support for the following kind of elements:
        /// </summary>
        /// <param name="Renderables">Array of renderable elements.</param>
        /// <param name="SettingsList">Array of Settings associated with respective element in the array of renderables.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Render(this IWritable writable, IEnumerable<IRenderable> Renderables, params ISettings[] SettingsList)
        {
            int slen = SettingsList.Length;
            var Boundary = Factory.newBoundary();
            int i = 0;
            int j = -1;
            ISettings Settings;
            foreach (var Renderable in Renderables)
            {
                Settings = i < slen ? SettingsList[i] : null;
                if(!writable.Render(Renderable, Settings, true))
                    break;
                Boundary.Merge(Settings.RecentlyDrawn);
                ++i;
                ++j;
            }

            if (Boundary.Valid && writable is IUpdatable)
                ((IUpdatable)writable).Update(Command.UpdateScreenOnly | Command.Animate, Boundary);
            return j != -1;
        }

        /// <summary>
        /// Renders multiple elements on this object. This renderer has a built-in support for the following kind of elements:
        /// </summary>
        /// <param name="Shapes">Array of renderable shapes.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Render(this IWritable writable, IEnumerable<IShape> Shapes)
        {
            var Boundary = Factory.newBoundary();
            int j = -1;
            foreach (var Shape in Shapes)
            {
                if (!writable.Render(Shape.Renderable, Shape.Settings, true))
                    break;
                Boundary.Merge(Shape.Settings.RecentlyDrawn);
                ++j;
            }

            if (Boundary.Valid && writable is IUpdatable)
                ((IUpdatable)writable).Update(Command.UpdateScreenOnly | Command.Animate, Boundary);
        }

        /// <summary>
        /// Renders multiple elements on this object. This renderer has a built-in support for the following kind of elements:
        /// </summary>
        /// <param name="ExecutionTime">Time to complete rendering process for all shapes in miliseconds.</param>
        /// <param name="Renderables">Array of renderable elements.</param>
        /// <param name="SettingsList">Array of Settings associated with respective element in the array of renderables.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RenderWithBenchmark(this IWritable writable, out long ExecutionTime,
            IEnumerable<IRenderable> Renderables, params ISettings[] SettingsList)
        {
            Stopwatch Watch = new Stopwatch();
            var description = string.Join(",", Renderables.Select(r => r.ID)) + " rendering ";
            string message = Benchmarks.Execute(()=>
                writable.Render(Renderables, SettingsList), Watch, out ExecutionTime, description);
            Watch = null;
            return message;
        }

        /// <summary>
        /// Renders multiple elements on this object. This renderer has a built-in support for the following kind of elements:
        /// </summary>
        /// <param name="Renderables">Array of renderable elements.</param>
        /// <param name="SettingsList">Array of Settings associated with respective element in the array of renderables.</param>
        /// <returns>Message which contains IDs of shapes and total executon time to render them all.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RenderWithBenchmark(this IWritable writable, IEnumerable<IRenderable> Renderables, params ISettings[] SettingsList)
        {
            Stopwatch Watch = new Stopwatch();
            var description = string.Join(",", Renderables.Select(r => r.ID)) + " rendering ";
            string message;
            Benchmarks.Execute(() => 
                writable.Render(Renderables, SettingsList), Watch, out message, description);
            Watch = null;
            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writable"></param>
        /// <param name="shape"></param>
        /// <param name="Settings"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(this IWritable writable, IFigure shape, ISettings Settings) =>
            WriteFigure(writable, shape, Settings);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writable"></param>
        /// <param name="shape"></param>
        /// <param name="Settings"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static partial void WriteFigure(this IWritable writable, IFigure shape, ISettings Settings);
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
        public static void Fill(this IEnumerable<ILine> lines, FillAction action, int y, int bottom, Command command)
        {
            using (var polyFill = Factory.newPolyFill())
            {
                polyFill.Begin(y, bottom);
                polyFill.Command = command | Command.OddEvenPolyFill;
                polyFill.Scan(lines);
                polyFill.Fill(action);
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
        public static void ProcessLine(int x1, int y1, int x2, int y2, PixelAction action,
            Command command, SlopeType skip = SlopeType.None, Size clip = default(Size))
        {
            if (skip == SlopeType.Both)
                return;
            var type = Lines.Type(x1, y1, x2, y2, out SlopeType nskip);
            if (type == LineType.Point || nskip == skip)
                return;
            if (!Lines.DrawParams(x1, y1, x2, y2, out bool horizontalScan, out int m,
                out int step, out int min, out int max, out int value, clip))
                return;
            var Draw = command;
            Draw &= ~Command.Breshenham;
            Draw &= ~Command.Distinct;
            var Dot = Draw.HasFlag(Command.Dot);
            var Dash = Draw.HasFlag(Command.Dash);
            var DashDotDot = Draw.HasFlag(Command.DashDotDash);

            int i = 0;

            while (min != max)
            {
                var val = value >> Vectors.BigExp;
                if (Dot)
                {
                    if (i == 0)
                    {
                        action(val, min, horizontalScan, Draw);
                        i = 1;
                    }
                    else
                        i = 0;
                }
                else if (Dash)
                {
                    if (i < 2)
                    {
                        action(val, min, horizontalScan, Draw);
                        ++i;
                    }
                    else if (i == 3)
                        i = 0;
                    else
                        ++i;
                }
                else if (DashDotDot)
                {
                    if (i < 2)
                    {
                        action(val, min, horizontalScan, Draw);
                        ++i;
                    }
                    else if (i == 3)
                    {
                        action(val, min, horizontalScan, Draw);
                        i = 0;
                    }
                    else
                        ++i;
                }
                else
                    action(val, min, horizontalScan, Draw);

                value += m;
                min += step;
            }
            action(value >> Vectors.BigExp, min, horizontalScan, Draw);
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
        public static void ProcessLine(float x1, float y1, float x2, float y2, PixelAction action,
            Command command, SlopeType skip = SlopeType.None, Size clip = default(Size))
        {
            if (skip == SlopeType.Both)
                return;

            var type = Lines.Type(x1, y1, x2, y2, out SlopeType nskip);
            if (type == LineType.Point || nskip == skip)
                return;
            var horizontalScan = nskip == SlopeType.Steep ? true : false;

            if (type == LineType.Horizontal || type == LineType.Vertical)
            {
                ProcessLine(x1.Ceiling(), y1.Ceiling(), x2.Ceiling(), y2.Ceiling(), action, command, 0, clip);
                return;
            }

            if (!Lines.DrawParams(x1, y1, x2, y2, horizontalScan, false, out float m,
                out int step, out int min, out int max, out float val, out _, clip))
                return;
            var Draw = command;
            Draw &= ~Command.Breshenham;
            Draw &= ~Command.Distinct;

            var Dot = Draw.HasFlag(Command.Dot);
            var Dash = Draw.HasFlag(Command.Dash);
            var DashDotDot = Draw.HasFlag(Command.DashDotDash);
            int i = 0;
            while (min != max)
            {
                if (Dot)
                {
                    if (i == 0)
                    {
                        action(val, min, horizontalScan, Draw);
                        i = 1;
                    }
                    else
                        i = 0;
                }
                else if (Dash)
                {
                    if (i < 2)
                    {
                        action(val, min, horizontalScan, Draw);
                        ++i;
                    }
                    else if (i == 3)
                        i = 0;
                    else
                        ++i;
                }
                else if (DashDotDot)
                {
                    if (i < 2)
                    {
                        action(val, min, horizontalScan, Draw);
                        ++i;
                    }
                    else if (i == 3)
                    {
                        action(val, min, horizontalScan, Draw);
                        i = 0;
                    }
                    else
                        ++i;
                }
                else
                {
                    action(val, min, horizontalScan, Draw);
                }
                val += m;
                min += step;
            }

            action(val, min, horizontalScan, Draw);
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
        public static void ScanLine(float x1, float y1, float x2, float y2, bool? steep, PixelAction action)
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
                action(val, min, horizontalScan, 0);

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

                var close1 = Factory.newLine(l1.X1, l1.Y1, l2.X1, l2.Y1);
                l1 = outerLines.Last();
                l2 = innerLines.Last();
                var close2 = Factory.newLine(l1.X2, l1.Y2, l2.X2, l2.Y2);
                innerLines.Add(close1);
                outerLines.Add(close2);
            }
            if (Factory.ShapeParser.NoNeedToSwapPerimeters(ShapeName))
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
        public static IList<ILine>[] GetDrawParams(FillMode fillMode, Command fillCommand, float Stroke, string ShapeName,
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

                    if (fillCommand.HasFlag(Command.Outlininig))
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

        #region TOAREA
        public static Rectangle ToArea(this IRenderable shape, Size? clip = null)
        {
            int minX, minY, maxX, maxY;

            minX = minY = int.MaxValue;
            maxX = maxY = int.MinValue;
            bool pointFound = false;
            bool sizeFound = false;
            bool ok = false;

            if (shape is IPointF)
            {
                minX = (int)((IPointF)shape).X;
                minY = (int)((IPointF)shape).Y;
                pointFound = true;
            }
            else if (shape is IPoint)
            {
                minX = ((IPoint)shape).X;
                minY = ((IPoint)shape).Y;
                pointFound = true;
            }

            if (pointFound && shape is ISizeF)
            {
                float r = ((ISizeF)shape).Width;
                float b = ((ISizeF)shape).Height;
                maxX = (int)r;
                maxY = (int)b;
                if (b - maxX != 0)
                    ++maxX;
                if (b - maxY != 0)
                    ++maxY;

                if (maxX != 0 && maxY != 0)
                {
                    maxX += minX;
                    maxY += minY;
                    sizeFound = true;
                }
            }
            else if (pointFound && shape is ISize)
            {
                minX = ((ISize)shape).Width;
                minY = ((ISize)shape).Height;
                if (maxX != 0 && maxY != 0)
                {
                    maxX += minX;
                    maxY += minY;
                    sizeFound = true;
                }
            }
            if (pointFound && sizeFound)
            {
                ok = true;
                goto Clipping;
            }

            if (shape is IFigurable)
            {
                return ((IFigurable)shape).Perimeter().ToArea().Expand();
            }
            else if (shape is IEnumerable<VectorF>)
            {
                return ((IEnumerable<VectorF>)shape).ToArea().Expand();
            }
            else
                return Rectangle.Empty;
            Clipping:
            if (clip != null && (clip.Value.Width > 0 && clip.Value.Height > 0))
            {
                minX = Math.Max(minX, 0);
                minY = Math.Max(minY, 0);
                maxX = Math.Max(maxX, 0);
                maxY = Math.Max(maxY, 0);

                if (maxX > clip.Value.Width)
                    maxX = clip.Value.Width;
                if (maxY > clip.Value.Height)
                    maxY = clip.Value.Height;
            }
            if (!ok)
                return Rectangle.Empty;

            return Rectangle.FromLTRB(minX, minY, maxX, maxY, true);
        }
        #endregion

        #region GET PEN
        /// <summary>
        /// Gets an appropriate pen to render the given shape.
        /// </summary>
        /// <param name="buffer">Buffer with background.</param>
        /// <param name="shape">Shape which to get a pen for.</param>
        /// <param name="Settings">RenderInfo object.</param>
        /// <param name="suppliedBounds">If supplied, size of pen is that of size of boounds otherwise, it will be size of bounds of shape.</param>
        /// <param name="suppliedBounds"></param>
        /// <param name="Control"></param>
        /// <returns></returns>
        public static IReadable GetPen(this IWritable buffer, IRenderable shape, ISettings Settings)
        {
            IReadable Pen;
            if (Settings.Bounds == null || !Settings.Bounds.Valid)
                Settings.Bounds = shape.ToArea();

            if (Settings.Clip)
                Settings.Bounds = Settings.Bounds.Clamp(Settings.Clip);

            else
                Settings.Bounds = Settings.Bounds.Clamp(Vectors.UHD8kWidth, Vectors.UHD8kHeight);

            var w = Settings.Bounds.Width + 1;
            var h = Settings.Bounds.Height + 1;

            IPenContext PenContext = Settings.PenContext;
            bool Inverted = false;

            if (PenContext != null)
                goto mks;

            if (PenContext == null && buffer is IBackground)
            {
                PenContext = ((IBackground)buffer).Background;
                Inverted = true;
            }

        mks:
            Pen = PenContext.ToPen(w, h);
            if (shape is IRotatable)
                Settings.Rotation = ((IRotatable)shape).Rotation;

            (Pen as ISettingsReceiver)?.Receive(Settings);
            if (Inverted)
                Pen.Invert = true;
            Settings.PenContext = Pen;
            return Pen;
        }
        #endregion

        #region WRITE PIXEL
        ///// <summary>
        ///// Writes pixel to the this block at given co-ordinates of location using specified color.
        ///// </summary>
        ///// <param name="buffer">Memory block to write pixel to.</param>
        ///// <param name="x">X cordinate on 2d buffer memory block</param>
        ///// <param name="y">Y cordinate on 2d buffer memory block</param>
        ///// <param name="color">colour of pixel.</param>
        ///// <param name="dstOffsetX">X co-ordinate value of any offset to apply while writing.</param>
        ///// <param name="dstOffsetY">Y co-ordinate value of any offset to apply while writing.</param>
        ///// <param name="Command">Command to control pixel writing.</param>
        ///// <param name="ShapeID">ID of shape which pixel is being written for.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static void WritePixel(this IWritable buffer, float x, float y, int color, int dstOffsetX, int dstOffsetY, Command Command, string ShapeID)
        //{
        //    x += dstOffsetX;
        //    y += dstOffsetY;

        //    int x0 = (int)x;
        //    int y0 = (int)y;

        //    bool Antialiased = (Command & Command.Breshenham) != (Command.Breshenham);

        //    if (!Antialiased)
        //    {
        //        buffer.WritePixel(x0, y0, true, color, null, Command, ShapeID);
        //        return;
        //    }

        //    float alpha1 = x - x0;
        //    float alpha2 = y - y0;

        //    if (alpha1 == 0 || alpha2 == 0)
        //    {
        //        bool horizontal = alpha1 != 0 ? true : false;
        //        if (horizontal)
        //            buffer.WritePixel(x, y0, true, color, 0, 0, Command, ShapeID);
        //        else
        //            buffer.WritePixel(y, x0, false, color, 0, 0, Command, ShapeID);
        //        return;
        //    }
        //    else
        //    {
        //        var invAlpha1 = 1 - alpha1;
        //        var invAlpha2 = 1 - alpha2;

        //        buffer.WritePixel(x0, y0, true, color, invAlpha1 * invAlpha2, Command, ShapeID);
        //        buffer.WritePixel(x0 + 1, y0, true, color, alpha1 * invAlpha2, Command, ShapeID);
        //        buffer.WritePixel(y0 + 1, x0, false, color, invAlpha1 * alpha2, Command, ShapeID);
        //        buffer.WritePixel(y0 + 1, x0 + 1, false, color, alpha1 * alpha2, Command, ShapeID);
        //    }
        //}
        #endregion

        #region CREATE FILL ACTION
        /// <summary>
        /// Retuns an action delegate for rendering a line on specified buffer target using specified buffer pen.
        /// </summary>
        /// <param name="buffer">Buffer which to render a memory block on</param>
        /// <param name="pen">Buffer pen which to read pixeld from</param>
        /// <param name="dstOffsetX">X co-ordinate value of any offset to apply while writing.</param>
        /// <param name="dstOffsetY">Y co-ordinate value of any offset to apply while writing.</param>
        /// <returns>An instance of FillAction delegate</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void CreateFillAction(this IWritable buffer, IReadable pen, out FillAction action,
            int dstOffsetX, int dstOffsetY, string ShapeID, INotifier boundary)
        {
            action = (start, axis, Horizontal, end, Alpha, Command) =>
            {
                if (float.IsNaN(start) && float.IsNaN(end))
                    return;

                if (start > end)
                {
                    float temp = end;
                    end = start;
                    start = temp;
                }

                bool NotSoClose = true;

                int iStart = (int)start;
                int iEnd = (int)end;

                int Start = iStart;
                int End = iEnd;

                if (start - iStart != 0)
                    ++Start;

                if (end - iEnd != 0)
                    ++End;

                bool IsPoint = start == end;
                var CheckForCloseness = (Command & Command.CheckForCloseness) == (Command.CheckForCloseness);
                var LineOnly = (Command & Command.DrawLineOnly) == (Command.DrawLineOnly);
                var EndsOnly = (Command & Command.DrawEndsOnly) == (Command.DrawEndsOnly) || IsPoint;
                var CalculateOnly = (Command & Command.CalculateOnly) == (Command.CalculateOnly);
                bool Antialiased = (Command & Command.Breshenham) != (Command.Breshenham);

                int Length = End - Start;
                if (CheckForCloseness)
                    NotSoClose = (end - start) >= .5f;
                else
                    NotSoClose = true;

                if (!EndsOnly && NotSoClose)
                {
                    int srcIndex = 0;
                    int[] source;
                    byte[] sourceAlphas;

                    if (!CalculateOnly)
                        pen.ReadLine(Start, End, axis, Horizontal, out source, out srcIndex, out Length, out sourceAlphas);

                    int dstX = Horizontal ? Start : axis;
                    int dstY = Horizontal ? axis : Start;
                    fixed (int* src = source)
                    {
                        fixed (byte* srcAlphas = sourceAlphas)
                        {
                            buffer.WriteLine(src, srcIndex, Length, Length, Horizontal,
                            dstX + dstOffsetX, dstY + dstOffsetY, Alpha, srcAlphas, Command, ShapeID, boundary);
                        }
                    }
                }

                if (!LineOnly)
                {
                    int x, y, color;

                    x = Horizontal ? iStart : axis;
                    y = Horizontal ? axis : iStart;
                    color = pen.ReadPixel(x, y);

                    x += dstOffsetX;
                    y += dstOffsetY;
                    float alpha = start - iStart;

                    if (alpha == 0 || !Antialiased)
                    {
                        buffer.WritePixel(x, y, true, color, null, Command, ShapeID, boundary);
                    }
                    else if (Horizontal)
                    {
                        buffer.WritePixel(x, y, true, color, 1 - alpha, Command, ShapeID, boundary);
                        buffer.WritePixel(x + 1, y, true, color, alpha, Command, ShapeID, boundary);
                    }
                    else
                    {
                        buffer.WritePixel(y, x, false, color, 1 - alpha, Command, ShapeID, boundary);
                        buffer.WritePixel(y + 1, x, false, color, alpha, Command, ShapeID, boundary);
                    }

                    if (NotSoClose)
                    {
                        x = Horizontal ? iEnd : axis;
                        y = Horizontal ? axis : iEnd;
                        color = pen.ReadPixel(x, y);
                        x += dstOffsetX;
                        y += dstOffsetY;
                        alpha = end - iEnd;

                        if (alpha == 0 || !Antialiased)
                        {
                            buffer.WritePixel(x, y, true, color, null, Command, ShapeID, boundary);
                        }
                        else if (Horizontal)
                        {
                            buffer.WritePixel(x, y, true, color, 1 - alpha, Command, ShapeID, boundary);
                            buffer.WritePixel(x + 1, y, true, color, alpha, Command, ShapeID, boundary);
                        }
                        else
                        {
                            buffer.WritePixel(y, x, false, color, 1 - alpha, Command, ShapeID, boundary);
                            buffer.WritePixel(y + 1, x, false, color, alpha, Command, ShapeID, boundary);
                        }
                    }
                }
            };
        }
        #endregion

        #region CREATE PIXEL ACTION
        /// <summary>
        /// Retuns an action delegate for rendering an axial line or pixel on specified buffer target using specified buffer pen.
        /// </summary>
        /// <param name="buffer">Buffer which to render a memory block on</param>
        /// <param name="pen">Buffer pen which to read pixeld from</param>
        /// <returns>An instance of FillAction delegate</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreatePixelAction(this IWritable buffer, IReadable pen, out PixelAction action,
            int dstOffsetX, int dstOffsetY, string ShapeID, INotifier boundary)
        {
            action = (val, axis, horizontal, Command) =>
            {
                int intVal = (int)val;

                float alpha = val - intVal;

                int x = horizontal ? intVal : axis;
                int y = horizontal ? axis : intVal;

                int color = pen.ReadPixel(x, y);

                x += dstOffsetX;
                y += dstOffsetY;
                bool Antialiased = (Command & Command.Breshenham) != (Command.Breshenham);

                if (alpha == 0 || !Antialiased)
                {
                    buffer.WritePixel(x, y, true, color, null, Command, ShapeID, boundary);
                }
                else if (horizontal)
                {
                    buffer.WritePixel(x, y, true, color, 1 - alpha, Command, ShapeID, boundary);
                    buffer.WritePixel(x + 1, y, true, color, alpha, Command, ShapeID, boundary);
                }
                else
                {
                    buffer.WritePixel(y, x, false, color, 1 - alpha, Command, ShapeID, boundary);
                    buffer.WritePixel(y + 1, x, false, color, alpha, Command, ShapeID, boundary);
                }
            };
        }
        #endregion

        #region TO PIXEL ACTION
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PixelAction ToPixelAction(this FillAction action)
        {
            return (v, axis, h, cmd) => action(v, axis, h, v, null, cmd);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PixelAction ToPixelAction(this VectorAction<int> action)
        {
            return (val, axis, h, cmd) =>
            {
                int iVal = (int)val;
                if (val - iVal >= 0.5f)
                    ++iVal;

                action(h ? iVal : axis, h ? axis : iVal);
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PixelAction ToPixelAction(this VectorAction<float> action)
        {
            return (v, axis, h, cmd) => action(h ? v : axis, h ? axis : v);
        }
        #endregion

        #region COPY TO
        /// <summary>
        /// Copies block to another by taking an area from a 1D array representing a rectangele to the given destination block.
        /// </summary>
        /// <param name="block">buffer which to render this memory block on</param>
        /// <param name="dstX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="dstY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyX">Top left x co-ordinate of area in source to cop.</param>
        /// <param name="copyY">Top left y co-ordinate of area in source to copy</param>
        /// <param name="copyW">Width of area in the source to copy.</param>
        /// <param name="copyH">Height of area in the source to copy</param>
        /// <param name="Command">Draw command to control the copy operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IRectangle CopyTo(this IBlockable source, IBlockable block, int dstX, int dstY, int copyX, int copyY, int copyW, int copyH, Command Command = 0)
        {
            #region INITIALIZE VARIABLES
            IRectangle dstRc = Rectangle.Empty;
            int* src = null;
            byte* srcAlphas = null;
            string ID = (source as IID)?.ID;
            int srcLen = source.Length;
            int srcW = source.Width;
            int srcH = source.Height;
            int dstW = block.Width;
            int dstH = block.Height;
            int dstLen = block.Length;
            var copy = source.CompitibleRc(copyX, copyY, copyW, copyH);
            copyX = copy.X;
            copyY = copy.Y;
            copyW = copy.Width;
            copyH = copy.Height;
            bool BackgroundBuffer = (Command & Command.BackgroundBuffer) == Command.BackgroundBuffer;
            #endregion

            #region EXTRACT DATA FROM SOURCE
            if (source is ITextureBrush)
            {
                src = (int*)((ITextureBrush)source).Source;
            }
            else if (source is IFixedBrush)
            {
                fixed (int* p = ((IFixedBrush)source).PenData)
                    src = p;
            }
            if (source is IImageData)
            {
                IImageData image = (IImageData)source;
                image.GetData(out int[] _src, out byte[] _srcAlphas, 
                    BackgroundBuffer && image.SupportBackgroundBuffer);
                fixed (int* p = _src)
                    src = p;
                fixed (byte* p = _srcAlphas)
                    srcAlphas = p;
            }
            else if (source is IPixels)
            {
                src = (int*)((IPixels)source).Source;
                srcAlphas = null;
            }
            else if (source is ICopyable)
            {
                srcLen = copyW * copyH;
                srcW = copyW;
                srcH = copyH;
                int[] temp = new int[srcLen];
                fixed (int* p = temp)
                {
                    ((ICopyable)source).CopyTo(copyX, copyY, copyW, copyH, (IntPtr)p, srcLen, srcW, 0, 0, Command);
                    src = p;
                }
                copyX = copyY = 0;
            }
            #endregion

            #region IPASTABLE COPY
            if (block is IPastable)
            {
                if (src != null)
                {
                    dstRc = ((IPastable)block).CopyFrom((IntPtr)src, srcW, srcH, dstX, dstY, copyX, copyY, copyW, copyH, Command, null, (IntPtr)srcAlphas);
                    goto Update;
                }
            }
            #endregion

            #region IPIXELS COPY
            if (block is IPixels)
            {
                int* dst = (int*)((IPixels)block).Source;
                if (src != null)
                {
                    BlockCopy action = (sidx, didx, len, dx, dy, cmd) =>
                        Blocks.Copy(src, sidx, dst, didx, len, cmd, srcAlphas);
                    dstRc = Blocks.CopyBlock(copyX, copyY, copyW, copyH, srcLen, srcW, srcH, dstX, dstY, dstW, dstLen, null, Command);
                    goto Update;
                }
            }
            #endregion

            #region IWRITABLE 
            if (!(block is IWritable))
                return dstRc;

            var writable = (IWritable)block;
            var boundary = Factory.newBoundary();
            int x, y, r, b, srcIndex, copyLen;

            if (src != null)
            {
                srcIndex = copyX + copyY * srcW;
                copyLen = copyW;
                y = dstY;
                x = dstX;
                b = y + copyH;
                if (copyX + copyLen > srcW)
                    copyLen -= (copyX + copyLen - srcW);
                if (y < 0)
                {
                    b += y;
                    y = 0;
                }
                while (y < b)
                {
                    writable.WriteLine(src, srcIndex, srcW, copyLen, true, x, y++, null, srcAlphas, Command, null, boundary);
                    srcIndex += srcW;
                }
                dstRc = boundary.GetBounds();
            }
            else if (source is IReadable)
            {
                var Pen = (IReadable)source;
                x = copyX;
                r = x + copyW;
                y = copyY;
                b = y + copyH;
                if (y < 0)
                {
                    b += y;
                    y = 0;
                }
                var dy = dstY;
                int[] pixels;
                byte[] pixelAlphas;
                while (y < b)
                {
                    Pen.ReadLine(x, r, y, true, out pixels, out srcIndex, out copyLen, out pixelAlphas);
                    fixed (int* p = pixels)
                        src = p;
                    fixed (byte* p = pixelAlphas)
                        srcAlphas = p;
                    writable.WriteLine(src, srcIndex, copyLen, copyLen, true, dstX, dstY++, null, srcAlphas, Command, null, boundary);
                    ++y;
                }
                dstRc = boundary.GetBounds();
            }
            #endregion

        Update:
            #region UPDATE
            if (dstRc.Valid && block is IUpdatable)
                ((IUpdatable)block).Update(Command, dstRc);
            return dstRc;
            #endregion
        }

        /// <summary>
        /// Copies a memory block to the given destination.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="block"></param>
        /// <param name="block">buffer which to render this memory block on</param>
        /// <param name="dstX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="dstY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="area">Area to copy from source to the block</param>
        /// <param name="command">Draw command to control the copy operation.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRectangle CopyTo(this IBlockable source, IBlockable block, int dstX, int dstY, Rectangle area, Command command = 0) =>
            source.CopyTo(block, dstX, dstY, area.X, area.Y, area.Width, area.Height, command);

        /// <summary>
        /// Copies block to another by taking an area from a 1D array representing a rectangele to the given destination block.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="block"></param>
        /// <param name="area"></param>
        /// <param name="command">Draw command to control the copy operation.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IRectangle CopyTo(this IBlockable source, IBlockable block, Rectangle area, Command command = 0) =>
            source.CopyTo(block, area.X, area.Y, area.X, area.Y, area.Width, area.Height, command);

        /// <summary>
        /// Copies block to another by taking an area from a 1D array representing a rectangele to the given destination block.
        /// </summary>
        /// <param name = "result" > buffer which to render this memory block on</param>
        /// <param name = "x" > Top left x co-ordinate of area in source to cop.</param>
        /// <param name = "y" > Top left y co-ordinate of area in source to copy</param>
        /// <param name = "w" > Width of area in the source to copy.</param>
        /// <param name = "h" > Height of area in the source to copy</param>
        /// <param name = "Command" > Draw command to control the copy operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ISize CopyTo(this IBlockable source, out int[] result, int x, int y, int w, int h, Command Command = Command.Screen)
        {
            #region INITIALIZE VARIABLES
            var copy = source.CompitibleRc(x, y, w, h);
            int* src = null;
            byte* srcAlphas = null;
            string ID = (source as IID)?.ID;
            int srcLen = source.Length;
            int srcW = source.Width;
            int srcH = source.Height;
            int dstW = w;
            int dstH = h;
            int dstLen = w * h;
            x = copy.X;
            y = copy.Y;
            w = copy.Width;
            h = copy.Height;
            result = new int[dstLen];
            int* dst;
            fixed (int* p = result)
                dst = p;
            bool BackgroundBuffer = (Command & Command.BackgroundBuffer) == Command.BackgroundBuffer;
            #endregion

            if (source is ICopyable)
            {
                srcLen = w * h;
                srcW = w;
                srcH = h;
                return ((ICopyable)source).CopyTo(x, y, w, h, (IntPtr)dst, srcLen, srcW, 0, 0, Command);
            }

            #region EXTRACT DATA FROM SOURCE
            if (source is ITextureBrush)
            {
                src = (int*)((ITextureBrush)source).Source;
            }
            else if (source is IFixedBrush)
            {
                fixed (int* p = ((IFixedBrush)source).PenData)
                    src = p;
            }
            if (source is IImageData)
            {
                IImageData image = (IImageData)source;
                image.GetData(out int[] _src, out byte[] _srcAlphas,
                    BackgroundBuffer && image.SupportBackgroundBuffer);
                fixed (int* p = _src)
                    src = p;
                fixed (byte* p = _srcAlphas)
                    srcAlphas = p;
            }
            else if (source is IPixels)
            {
                src = (int*)((IPixels)source).Source;
                srcAlphas = null;
            }
            #endregion

            if (src != null)
            {
                BlockCopy action = (sidx, didx, len, dx, dy, cmd) =>
                    Blocks.Copy(src, sidx, dst, didx, len, cmd, srcAlphas);
                return Blocks.CopyBlock(x, y, w, h, srcLen, srcW, srcH, 0, 0, dstW, dstLen, null, Command);
            }
            return Rectangle.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ISize CopyTo(this IBlockable block, out IntPtr Data, int x, int y, int w, int h, Command command = Command.Screen)
        {
            var rc = CopyTo(block, out int[] data, x, y, w, h, command);
            if (data == null)
                Data = IntPtr.Zero;
            else
            {
                fixed (int* p = data)
                    Data = (IntPtr)p;
            }
            return rc;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Write(this IImageProcessor writer, ICopyable image, Stream dest, ImageFormat format, int pitch = 4, int quality = 50)
        {
            int[] data = new int[image.Width * image.Height];
            fixed (int* p = data)
            {
                IntPtr pixels = (IntPtr)p;
                image.CopyTo(0, 0, image.Width, image.Height, pixels, image.Length, image.Width, 0, 0, 0, null);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// <param name="block">Image which is to be saved.</param>
        /// <param name="file">Path of a file to create and write data to</param>
        /// <param name="format">Format of the targeted image file</param>
        /// <param name="portion">Null represents a whole chunk of memory block. Other wise a prtion determined by location X, Y and size Width, Height of the portion rectangle</param>
        /// <param name="pitch">Pitch of the image - default is 4 - R, G, B, A</param>
        /// <param name="quality">Resolution quality of the tageted image file</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void SaveAs(this IBlockable block, string file,
            ImageFormat format = ImageFormat.BMP, Command command = Command.Screen, Rectangle? portion = null, int pitch = 4, int quality = 50)
        {
            file += "." + format.ToString();
            ISize size;
            Rectangle rc = Rects.CompitibleRc(block, portion?.X, portion?.Y, portion?.Width, portion?.Height);
#if Window
            if (format == ImageFormat.BMP)
            {
                if (portion == null)
                    Factory.SaveAsBitmap(block, file, command);
                else
                {
                    size = block.CopyTo(out IntPtr ptr, rc.X, rc.Y, rc.Width, rc.Height, command);
                    Factory.SaveAsBitmap(ptr, size.Width, size.Height, file);
                }
                return;
            }
#endif
            size = block.CopyTo(out int[] data, rc.X, rc.Y, rc.Width, rc.Height, command);
            Factory.ImageProcessor.Write(data, size.Width, size.Height, file, format, pitch, quality);
        }
        #endregion
    }
    partial class Renderer
    {
        #region DRAW IMAGE
        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangele to the given destination.
        /// </summary>
        /// <param name="block">buffer which to render a memory block on</param>
        /// <param name="source">1D array interpreted as a 2D array of Pixels with specified srcW width</param>
        /// <param name="srcW">Width of the entire source</param>
        /// <param name="dstX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="dstY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyX">Top left x co-ordinate of area in source to cop.</param>
        /// <param name="copyY">Top left y co-ordinate of area in source to copy</param>
        /// <param name="copyW">Width of area in the source to copy.</param>
        /// <param name="copyH">Height of area in the source to copy</param>
        /// <param name="command">Draw command to control image drawig operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IRectangle DrawImage(this IBlockable block, IntPtr source, int srcW, int srcH, int dstX, int dstY,
            int copyX, int copyY, int copyW, int copyH, Command command, string ShapeID)
        {
            IRectangle dstRc;
            if (block is IPixels)
            {
                IntPtr dest = ((IPixels)block).Source;
                int* src = (int*)source;
                int* dst = (int*)dest;
                BlockCopy action = (srcIndex, dstIndex, copyLength, x, y, cmd) => 
                    Blocks.Copy(src, srcIndex, dst, dstIndex, copyLength, cmd);
                dstRc = Blocks.CopyBlock(copyX, copyY, copyW, copyH, srcW * srcH, srcW, srcH, dstX, dstY, block.Width, block.Length, action, command);
            }
            else if (block is IPastable)
            {
                dstRc = ((IPastable)block).CopyFrom(source, srcW, srcH, dstX, dstY, copyX, copyY, copyW, copyH, command, ShapeID);
            }
            else
            {
                using (var image = Factory.newImage(source, srcW, srcH))
                {
                   dstRc = image.CopyTo(block, dstX, dstY, copyX, copyY, copyW, copyH, command);
                }
            }
            if(dstRc.Valid && block is IUpdatable)
                ((IUpdatable)block).Update(command, dstRc);
            return dstRc;
        }

        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangele to the given destination.
        /// </summary>
        /// <param name="block">buffer which to render a memory block on</param>
        /// <param name="source">1D array interpreted as a 2D array of Pixels with specified srcW width</param>
        /// <param name="srcW">Width of the entire source</param>
        /// <param name="dstX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="dstY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="updateImmediate">If true, Update method will immediately called and screen will get updated otherwise not.</param>
        public static unsafe void DrawImage(this IBlockable block, IntPtr source, int srcW, int srcH, int dstX, int dstY, Command command, string ShapeID) =>
            block.DrawImage(source, srcW, srcH, dstX, dstY, 0, 0, srcW, srcH, command, ShapeID);

        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangele to the given destination.
        /// </summary>
        /// <param name="block">buffer which to render a memory block on</param>
        /// <param name="source">1D array interpreted as a 2D array of Pixels with specified srcW width</param>
        /// <param name="srcW">Width of the entire source</param>
        /// <param name="dstX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="dstY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyX">Top left x co-ordinate of area in source to cop.</param>
        /// <param name="copyY">Top left y co-ordinate of area in source to copy</param>
        /// <param name="copyW">Width of area in the source to copy.</param>
        /// <param name="copyH">Height of area in the source to copy</param>
        /// <param name="updateImmediate">If true, Update method will immediately called and screen will get updated otherwise not.</param>
        public unsafe static void DrawImage(this IBlockable block, byte[] source, int srcW, int srcH, int dstX, int dstY,
            int copyX, int copyY, int copyW, int copyH, Command command, string ShapeID)
        {
            var srcLen = source.Length / 4;
            var rc = Rects.CompitibleRc(srcW, srcLen / srcW, copyX, copyY, copyW, copyH);
            fixed (byte* b = source)
            {
                block.DrawImage((IntPtr)b, srcW, srcH, dstX, dstY, rc.X, rc.Y, rc.Width, rc.Height, command, ShapeID);
            }
        }

        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangele to the given destination.
        /// </summary>
        /// <param name="block">buffer which to render a memory block on</param>
        /// <param name="source">1D array interpreted as a 2D array of Pixels with specified srcW width</param>
        /// <param name="srcW">Width of the entire source</param>
        /// <param name="dstX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="dstY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyX">Top left x co-ordinate of area in source to cop.</param>
        /// <param name="copyY">Top left y co-ordinate of area in source to copy</param>
        /// <param name="copyW">Width of area in the source to copy.</param>
        /// <param name="copyH">Height of area in the source to copy</param>
        /// <param name="updateImmediate">If true, Update method will immediately called and screen will get updated otherwise not.</param>
        public unsafe static void DrawImage(this IBlockable block, int[] source, int srcW, int srcH, int dstX, int dstY,
            int copyX, int copyY, int copyW, int copyH, Command command, string ShapeID)
        {
            var rc = Rects.CompitibleRc(srcW, source.Length / srcW, copyX, copyY, copyW, copyH);
            var srcLen = source.Length;
            fixed (int* src = source)
            {
                block.DrawImage((IntPtr)src, srcW, srcH, dstX, dstY, rc.X, rc.Y, rc.Width, rc.Height, command, ShapeID);
            }
        }

        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangele to the given destination.
        /// </summary>
        /// <param name="block">buffer which to render a memory block on</param>
        /// <param name="source">1D array interpreted as a 2D array of Pixels with specified srcW width</param>
        /// <param name="dstX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="dstY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="updateImmediate">If true, Update method will immediately called and screen will get updated otherwise not.</param>
        public static unsafe void DrawImage(this IBlockable block, IBlockable source, int dstX, int dstY, Command command = 0) =>
            source.CopyTo(block, dstX, dstY, 0, 0, source.Width, source.Height, command);

        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangele to the given destination.
        /// </summary>
        /// <param name="block">buffer which to render a memory block on</param>
        /// <param name="source">1D array interpreted as a 2D array of Pixels with specified srcW width</param>
        /// <param name="dstX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="dstY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyX">Top left x co-ordinate of area in source to cop.</param>
        /// <param name="copyY">Top left y co-ordinate of area in source to copy</param>
        /// <param name="copyW">Width of area in the source to copy.</param>
        /// <param name="copyH">Height of area in the source to copy</param>
        /// <param name="updateImmediate">If true, Update method will immediately called and screen will get updated otherwise not.</param>
        public static unsafe void DrawImage(this IBlockable block, IBlockable source, int dstX, int dstY,
            int copyX, int copyY, int copyW, int copyH, Command command = 0) =>
            source.CopyTo(block, dstX, dstY, copyX, copyY, copyW, copyH, command);

        /// <summary>
        /// Draws an image by taking an area from a source - capable of being copied to the given destination buffer.
        /// </summary>
        /// <param name="block">buffer which to render a memory block on</param>
        /// <param name="source">Source - capable of being copied to any buffer</param>
        /// <param name="dstX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="dstY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyRc">An Area from source to be copied</param>
        /// <param name="updateImmediate">If true, Update method will immediately called and screen will get updated otherwise not.</param>
        public static void DrawImage(this IBlockable block, IBlockable source, int dstX, int dstY, Rectangle copyRc, Command command = 0)
        {
            source.CopyTo(block, dstX, dstY, copyRc.X, copyRc.Y, copyRc.Width, copyRc.Height, command);
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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine(this IWritable buffer, float x1, float y1, float x2, float y2, ISettings settings)
        {
            if (buffer == null)
                return;
            var line = Factory.newLine(x1, y1, x2, y2);
            buffer.Render(line, settings);
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
            var line = Factory.newLine(x1, y1, x2, y2);
            buffer.Render(line, Factory.newSettings());
        }
        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by points p1 & p2.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="line">Line to draw</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine(this IWritable buffer, ILine line, ISettings settings)
        {
            if (buffer == null)
                return;
            buffer.Render(line, settings);
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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 before rendering the line segment</param>
        public static void DrawLine(this IWritable buffer, VectorF p1, VectorF p2, ISettings settings = null) =>
            buffer.DrawLine(p1.X, p1.Y, p2.X, p2.Y, settings);

        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by points p1 & p2.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="p1">Start point of line segment</param>
        /// <param name="p2">end point of line segment</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 before rendering the line segment</param>
        public static void DrawLine(this IWritable buffer, Vector p1, Vector p2, ISettings settings = null) =>
            buffer.DrawLine(p1.X, p1.Y, p2.X, p2.Y, settings);

        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by points p1 & x2, y2.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="p1">Start point of line segment</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 before rendering the line segment</param>
        public static void DrawLine(this IWritable buffer, VectorF p1, float x2, float y2, ISettings settings = null) =>
            buffer.DrawLine(p1.X, p1.Y, x2, y2, settings);

        /// <summary>
        /// Renders a collection of line segments using standard line algorithm.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="lines">A collection of lines</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawLines(this IWritable buffer, IEnumerable<ILine> lines, ISettings settings = null)
        {
            foreach (var l in lines)
            {
                DrawLine(buffer, l, settings);
            }
        }

        /// <summary>
        /// Renders a collection of line segments using standard line algorithm.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="lines">A collection of lines</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawLines(this IWritable buffer, IEnumerable<VectorF> points, ISettings settings = null, bool connectEach = true)
        {
            VectorF previous = VectorF.Empty;
            VectorF first = VectorF.Empty;

            foreach (var p in points)
            {
                if (!first)
                    first = p;
                if (!previous)
                {
                    previous = p;
                    continue;
                }
                DrawLine(buffer, previous, p, settings);
                if (!connectEach)
                    previous = Vector.Empty;
            }
            if (connectEach && first && previous)
            {
                DrawLine(buffer, previous, first, settings);
            }
        }

        /// <summary>
        /// Renders a collection of line segments using standard line algorithm.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="lines">A collection of lines</param>
        public static void DrawLines(this IWritable buffer, ISettings settings, params ILine[] lines) =>
            buffer.DrawLines(lines as IEnumerable<ILine>, settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="connectEach">If true then each line segment will be connected to the previous and next one</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 of each line segment before rendering the line segment</param>
        /// <param name="values">An interger array of values.Each subsequent four elements get converted to a line segment
        /// For example if values are int[]{23, 56, 98, 205} creates Line(X1 = 23, Y1 = 56, X2 = 98,Y2 = 205) </param>
        public static void DrawLines(this IWritable buffer, ISettings settings, bool connectEach, params int[] values)
        {
            var points = (values).ToPointsF();
            if (connectEach && points.Count > 2)
                points.Add(points[0]);

            if (connectEach)
            {
                for (int i = 1; i < points.Count; i++)
                    buffer.DrawLine(points[i - 1], points[i], settings);
            }
            else
            {
                for (int i = 1; i < points.Count; i += 2)
                    buffer.DrawLine(points[i - 1], points[i], settings);
            }
        }

        /// <summary>
        /// Renders line segments by first creating them from an array of float values specified using standard line algorithm.
        /// </summary>
        /// <param name="buffer">buffer which to render a line on</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="connectEach">If true then each line segment will be connected to the previous and next one</param>
        /// <param name="angle"></param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 of each line segment before rendering the line segment</param>
        /// <param name="values">A float array of values.Each subsequent four elements get converted to a line segment
        /// For example if values are int[]{23.33f, 56.67f, 98.45f, 205.21f} creates Line(X1 = 23.33f, Y1 = 56.67f, X2 = 98.45f,Y2 = 205.21f) </param>
        public static void DrawLines(this IWritable buffer, ISettings settings, bool connectEach, params float[] values)
        {
            var points = values.ToPoints();
            if (connectEach && points.Count > 2)
                points.Add(points[0]);

            if (connectEach)
            {
                for (int i = 1; i < points.Count; i++)
                    buffer.DrawLine(points[i - 1], points[i], settings);
            }
            for (int i = 1; i < points.Count; i += 2)
            {
                buffer.DrawLine(points[i - 1], points[i], settings);
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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the circle</param>
        public static void DrawCircle(this IWritable buffer, float x, float y, float width, ISettings settings) =>
            RenderCircleOrEllipse(buffer, x, y, width, width, settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the circle</param>
        public static void DrawCircle(this IWritable buffer, VectorF pointOnCircle, VectorF centerOfCircle, ISettings settings)
        {
            Curves.GetCircleData(pointOnCircle, centerOfCircle, out float x, out float y, out float w);
            RenderCircleOrEllipse(buffer, x, y, w, w, settings);
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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawEllipse(this IWritable buffer, float x, float y, float width, float height, ISettings settings) =>
            RenderCircleOrEllipse(buffer, x, y, width, height, settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered.
        /// Such as fitting, third point on ellipse or on center etc.</param>
        public static void DrawEllipse(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, ISettings settings, CurveType type = CurveType.Full) =>
            RenderCircleOrEllipse(buffer, p1, p2, p3, settings, type);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered.
        /// Such as fitting, third point on ellipse or on center etc.</param>
        public static void DrawEllipse(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, VectorF p4, ISettings settings, CurveType type = CurveType.Full) =>
            RenderCircleOrEllipse(buffer, p1, p2, p3, p4, settings, type);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawEllipse(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, VectorF p4, VectorF p5, ISettings settings) =>
            RenderCircleOrEllipse(buffer, p1, p2, p3, p4, p5, settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="type"> Defines the type of an arc along with other supplimentary options on how to draw it</param>
        public static void DrawArc(this IWritable buffer, float x, float y, float width, float height, float startAngle, float endAngle,
            ISettings settings, CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, x, y, width, height, startAngle, endAngle, settings, type);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawArc(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, ISettings settings, CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, p1, p2, p3, settings, type.Exclude(CurveType.Pie).Include(CurveType.Arc));

        /// <summary>
        /// Draws an arc passing through four points and rotates it by an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render an arc on</param>
        /// <param name="p1">First point on an arc</param>
        /// <param name="p2">Second point  on the arc</param>
        /// <param name="p3">Third point on the arc</param>
        /// <param name="p4">Fourth point on the arc</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawArc(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, VectorF p4, ISettings settings,
            CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, p1, p2, p3, p4, settings, type.Exclude(CurveType.Pie).Include(CurveType.Arc));

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawArc(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3,
            VectorF p4, VectorF p5, ISettings settings, CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, p1, p2, p3, p4, p5, settings, type.Exclude(CurveType.Pie).Include(CurveType.Arc));

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the pie</param>
        /// <param name="type"> Defines the type of an pie along with other supplimentary options on how to draw it</param>
        public static void DrawPie(this IWritable buffer, float x, float y, float width, float height,
            float startAngle, float endAngle, ISettings settings, CurveType type = CurveType.Pie) =>
            RenderArcOrPie(buffer, x, y, width, height, startAngle, endAngle, settings, type.Exclude(CurveType.Arc).Include(CurveType.Pie));

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the pie</param>
        /// <param name="type"> Defines the type of an pie along with other supplimentary options on how to draw it</param>
        public static void DrawPie(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, ISettings settings, CurveType type = CurveType.Pie) =>
            RenderArcOrPie(buffer, p1, p2, p3, settings, type.Exclude(CurveType.Arc).Include(CurveType.Pie));

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc</param>
        public static void DrawPie(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3,
            VectorF p4, ISettings settings, CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, p1, p2, p3, p4, settings, type.Exclude(CurveType.Arc).Include(CurveType.Pie));

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc</param>
        public static void DrawPie(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3,
            VectorF p4, VectorF p5, ISettings settings, CurveType type = CurveType.Arc) =>
            RenderArcOrPie(buffer, p1, p2, p3, p4, p5, settings, type.Exclude(CurveType.Arc).Include(CurveType.Pie));

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawCurve(this IWritable buffer, ICurve Curve, ISettings settings = null)
        {
            buffer.Render(Curve, settings);
        }
        #endregion

        #region DRAW CONIC
        /// <summary>
        /// Renders a conic object.
        /// </summary>
        /// <param name="conic">Conic object to render</param>
        /// <param name="buffer">Buffer which to render a curve on</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="drawEndsOnly">If true, only out line of conic will be drawn and filling will not be performed.</param>
        public static void DrawConic(this IWritable buffer, IConic conic, ISettings settings, bool drawEndsOnly = false)
        {
            if (buffer == null) 
                return;
            if (settings == null)
                settings = Factory.newSettings();

            if (drawEndsOnly)
            {
                settings.Command |= Command.DrawEndsOnly;
                settings.Command &= ~Command.DrawLineOnly;
            }
            else
            {
                settings.Command &= ~Command.DrawEndsOnly;
                settings.Command &= ~Command.DrawLineOnly;
            }
            buffer.Render(conic, settings);
        }
        #endregion

        #region DRAW BEZIER
        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawBezier(this IWritable buffer, ISettings settings, params float[] points) =>
            RenderBezier(buffer, points, BezierType.Cubic, settings);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="type">BezierType enum determines the type of bezier i.e Cubic - group of 4 points or multiple(group of 4 or 7 or 10 so on...)</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawBezier(this IWritable buffer, BezierType type, ISettings settings, params float[] points) =>
            RenderBezier(buffer, points, type, settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the traingle</param>
        public static void DrawTriangle(this IWritable buffer, float x1, float y1, float x2, float y2, float x3, float y3, ISettings settings) =>
            RenderTriangle(buffer, x1, y1, x2, y2, x3, y3, settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle"></param>
        public static void DrawTriangle(this IWritable buffer, Vector p1, Vector p2, Vector p3, ISettings settings) =>
            buffer.DrawTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, settings);

        /// <summary>
        /// Renders a trianle formed by three points specified by points p1, p2, p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a triangle on</param>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">the third point</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle"></param>
        public static void DrawTriangle(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, ISettings settings) =>
            buffer.DrawTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rectangle</param>
        public static void DrawSquare(this IWritable buffer, float x, float y, float width, ISettings settings) =>
            RenderRectangle(buffer, x, y, width, width, settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rectangle</param>
        public static void DrawRectangle(this IWritable buffer, float x, float y, float width, float height, ISettings settings) =>
            RenderRectangle(buffer, x, y, width, height, settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rectangle</param>
        public static void DrawRectangle(this IWritable buffer, Rectangle r, ISettings settings) =>
            RenderRectangle(buffer, r.X, r.Y, r.Width, r.Height, settings);

        /// <summary>
        /// Renders a rectangle specified by r parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a rectangle on</param>
        /// <param name="r">Rectangle to draw</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rectangle</param>
        public static void DrawRectangle(this IWritable buffer, RectangleF r, ISettings settings) =>
           RenderRectangle(buffer, r.X, r.Y, r.Width, r.Height, settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rounded box</param>
        public static void DrawRoundedBox(this IWritable buffer, float x, float y, float width, float height, float cornerRadius, ISettings settings, RoundBoxOption option) =>
           RenderRoundedBox(buffer, x, y, width, height, cornerRadius, settings, option);

        /// <summary>
        /// Renders a rounded box specified by x, y, width, height parameters and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="buffer">buffer which to render a rounded box on</param>
        /// <param name="x">X cordinate of the rounded box</param>
        /// <param name="y">Y cordinate of the rounded box</param>
        /// <param name="width">Width of the rounded box/param>
        /// <param name="height">Height the rounded box</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rounded box</param>
        public static void DrawRoundedBox(this IWritable buffer, float x, float y, float width, float height, float cornerRadius, ISettings settings) =>
           RenderRoundedBox(buffer, x, y, width, height, cornerRadius, settings);

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
        public static void DrawRoundedBox(this IWritable buffer, float x, float y, float width, float height, float cornerRadius, RoundBoxOption option) =>
            RenderRoundedBox(buffer, x, y, width, height, cornerRadius, null, option);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rounded box</param>
        public static void DrawRoundedBox(this IWritable buffer, RectangleF r, float cornerRadius, ISettings settings) =>
            RenderRoundedBox(buffer, r.X, r.Y, r.Width, r.Height, cornerRadius, settings);

        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="buffer">buffer which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rounded box</param>
        public static void DrawRoundedBox(this IWritable buffer, Rectangle r, float cornerRadius, ISettings settings) =>
            RenderRoundedBox(buffer, r.X, r.Y, r.Width, r.Height, cornerRadius, settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawRhombus(this IWritable buffer, VectorF first, VectorF second, VectorF third, ISettings settings) =>
            RenderRhombus(buffer, first, second, third, settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawRhombus(this IWritable buffer, float x1, float y1, float x2, float y2, float x3, float y3,
            ISettings settings) =>
            RenderRhombus(buffer, new VectorF(x1, y1), new VectorF(x2, y2), new VectorF(x3, y3), settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, ILine baseLine, float parallelLineDeviation, float skewBy, ISettings settings) =>
            RenderTrapezium(buffer, baseLine, parallelLineDeviation, skewBy, settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, ILine baseLine, float parallelLineDeviation, ISettings settings) =>
            RenderTrapezium(buffer, baseLine, parallelLineDeviation, 0, settings);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by the first four values in values parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="values">An array of float values- first four of which forms a base line from where the trapezium start.
        /// Parallel line deviation should be the 5th i.e values[4] item in values parameter if values has length of 5 otherwise deemed as zero which results in a simple line draw.
        /// 6th item i.e values[5] in values would form parallel Line Size Difference value if the lenght of values is 6 otherwise zero.
        /// </param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, float[] values, ISettings settings)
        {
            if (values.Length < 4)
                return;

            var first = Factory.newLine(values[0], values[1], values[2], values[3]);
            float parallelLineDeviation = 30f;
            float skewBy = 0;
            if (values.Length > 4)
                parallelLineDeviation = values[4];
            if (values.Length > 5)
                skewBy = values[5];
            RenderTrapezium(buffer, first, parallelLineDeviation, skewBy, settings);
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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, int[] values, ISettings settings)
        {
            if (values.Length < 4)
                return;

            var first = Factory.newLine(values[0], values[1], values[2], values[3]);
            float deviation = 30f;
            float skewBy = 0;
            if (values.Length > 4)
                deviation = values[4];
            if (values.Length > 5)
                skewBy = values[5];
            RenderTrapezium(buffer, first, deviation, skewBy, settings);
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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, float x1, float y1, float x2, float y2,
            float parallelLineDeviation, float skewBy, ISettings settings) =>
            RenderTrapezium(buffer, Factory.newLine(x1, y1, x2, y2), parallelLineDeviation, skewBy, settings);

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
            RenderTrapezium(buffer, Factory.newLine(x1, y1, x2, y2), deviation, skewBy, null);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, float x1, float y1, float x2, float y2, float parallelLineDeviation, ISettings settings) =>
            RenderTrapezium(buffer, Factory.newLine(x1, y1, x2, y2), parallelLineDeviation, 0, settings);

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
            RenderTrapezium(buffer, Factory.newLine(x1, y1, x2, y2), parallelLineDeviation, 0, null);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, VectorF p1, VectorF p2, float parallelLineDeviation, float skewBy, ISettings settings) =>
            RenderTrapezium(buffer, Factory.newLine(p1, p2), parallelLineDeviation, skewBy, settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, Vector p1, Vector p2, float parallelLineDeviation, float skewBy, ISettings settings) =>
            RenderTrapezium(buffer, Factory.newLine(p1.X, p1.Y, p2.X, p2.Y), parallelLineDeviation, skewBy, settings);


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
            RenderTrapezium(buffer, Factory.newLine(p1, p2), parallelLineDeviation, skewBy, null);

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
            RenderTrapezium(buffer, Factory.newLine(p1.X, p1.Y, p2.X, p2.Y), parallelLineDeviation, skewBy, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, VectorF p1, VectorF p2, float parallelLineDeviation, ISettings settings) =>
            RenderTrapezium(buffer, Factory.newLine(p1, p2), parallelLineDeviation, 0, settings);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawTrapezium(this IWritable buffer, Vector p1, Vector p2, float parallelLineDeviation, ISettings settings) =>
            RenderTrapezium(buffer, Factory.newLine(p1.X, p1.Y, p2.X, p2.Y), parallelLineDeviation, 0, settings);

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
            RenderTrapezium(buffer, Factory.newLine(p1, p2), parallelLineDeviation, 0, null);

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
            RenderTrapezium(buffer, Factory.newLine(p1.X, p1.Y, p2.X, p2.Y), parallelLineDeviation, 0, null);
        #endregion

        #region DRAW POLYGON
        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">buffer which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon  an each group of two subsequent values in polypoints forms a point x,y</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        public static void DrawPolygon(this IWritable buffer, ISettings settings, params float[] polyPoints) =>
            RenderPolygon(buffer, polyPoints, settings);

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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        public static void DrawPolygon(this IWritable buffer, ISettings settings, params int[] polyPoints) =>
            RenderPolygon(buffer, polyPoints.Select(p => (float)p), settings);
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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="drawStyle">A draw style to be used to draw text</param>
        /// <returns>GlyphsData object which contains a draw result information such as glyphs, drawn area etc.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IGlyphs DrawText(this IWritable buffer, IFont font, float destX, float destY, string text,
            ISettings settings = null, TextDrawStyle drawStyle = null)
        {
            if (buffer == null || font == null || string.IsNullOrEmpty(text))
                return null;
            var info = font.MeasureText(text, destX, destY, drawStyle);
            buffer.Render(info, settings);
            return info;
        }

        /// <summary>
        /// Renders a text object which represents a text and a collection of glyphs providing drawing representation of the text. 
        /// </summary>
        /// <param name="buffer">Buffer which to render a rhombus on</param>
        /// <param name="text">A text object to render</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawText(this IWritable buffer, IGlyphs text, ISettings settings = null)
        {
            buffer.Render(text, settings);
        }
        /// <summary>
        /// Renders a text object which represents a text and a collection of glyphs providing drawing representation of the text. 
        /// </summary>
        /// <param name="buffer">Buffer which to render a rhombus on</param>
        /// <param name="text">A text object to render</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawText(this IWritable buffer, IGlyphs text, int dstX, int dstY, ISettings settings = null)
        {
            if (settings == null)
                settings = Factory.newSettings();

            settings.X = dstX;
            settings.Y = dstY;
            buffer.Render(text, settings);
        }
        #endregion

        #region DRAW FOCUS RECT
        /// <summary>
        /// Draws focus rectangle i.e. border around specified with dotted invert colors
        /// </summary>
        /// <param name="rc">Rectangle to draw focus around.</param>
        public static void DrawFocusRect(this IImage block, Rectangle rc, ISettings info)
        {
            if (rc == null)
                return;
            int X = rc.X;
            int Y = rc.Y;
            int W = rc.Width;
            int H = rc.Height;
            var Settings = info ?? Factory.newSettings("FocusRect");
            Settings.Receive(null, true);
            info.FillMode = FillMode.DrawOutLine;
            info.Command = Command.Dot | Command.InvertColor
#if Advanced
             | Command.NoBrushAutoSizing
#endif
             ;
            Settings.PenContext = (block as IBackground)?.Background;
            block.DrawRectangle(X, Y, W, H, Settings);
        }
        #endregion

        #region ROTATE & FLIP
        /// <summary>
        /// Source: https://www.drdobbs.com/architecture-and-design/fast-bitmap-rotation-and-scaling/184416337
        /// Returns a rotated and scalled memory block of this object alongwith size of it.
        /// </summary>
        /// <param name="rotation">Angle of rotation to apply.</param>
        /// <param name="antiAliased">If true copy is antialised version of this object otherwise not.</param>
        /// <param name="scale">Scale to apply.</param>
        /// <returns>Rotated and scalled copy of this object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Size RotateAndScale(this IBlockable source, out IntPtr data, Rotation rotation, bool antiAliased = true, float scale = 1)
        {
            if(source is IScalable)
            {
                return ((IScalable)source).RotateAndScale(out data, rotation, antiAliased, scale);
            }
            int dstW = source.Width;
            int dstH = source.Height;
            int dstLen = source.Length;

            Size size = new Size(dstW, dstH);
            if (scale <= 0) scale = 1;
            int srcW = dstW;
            int srcH = dstH;
            int srcLen = dstLen;
            int* src;

            int* dst;
            fixed (int* p = new int[dstW * dstH])
                dst = p;
            data = IntPtr.Zero;

            if (!rotation && scale == 1)
            {
                if (source is IPixels)
                {
                    Blocks.Copy((int*)((IPixels)source).Source, 0, dst, 0, dstW, Command.Opaque);
                    data = (IntPtr)dst;
                }
                else if(source is ICopyable)
                {
                    data = (IntPtr)dst;
                    ((ICopyable)source).CopyTo(0, 0, srcW, srcH, data, dstLen, dstW, 0, 0, Command.Opaque, null);
                }
                return size;
            }

            fixed (int* p = new int[srcW * srcH])
                src = p;
          
            if (source is IPixels)
            {
                Blocks.Copy((int*)((IPixels)source).Source, 0, src, 0, srcW, Command.Opaque);
            }
            else if (source is ICopyable)
            {
                ((ICopyable)source).CopyTo(0, 0, srcW, srcH, (IntPtr)src, srcLen, srcW, 0, 0, 0, null);
            }
            else
            {
                data = IntPtr.Zero;
                return Size.Empty;
            }
            int srcCx = srcW / 2;
            int srcCy = srcH / 2;

            int dstCx = dstW / 2;
            int dstCy = dstH / 2;

            bool intCalculation = !antiAliased;

            float dstXf = 0;
            float dstYf = 0;
            int dstXi = 0;
            int dstYi = 0;
            float x3 = 0, y3 = 0;
            int x0 = 0, y0 = 0, xi = 0, yi = 0;
            int color = 0;

            int Sini = rotation.Sini;
            int Cosi = rotation.Cosi;

            float Sin = rotation.Sin;
            float Cos = rotation.Cos;

            if (scale != 1)
            {
                Sin *= 1f / scale;
                Cos *= 1f / scale;

                Sini = (Sin * Angles.Big).Round();
                Cosi = (Cos * Angles.Big).Round();
            }

            if (intCalculation)
            {
                dstXi = -(dstCx * Cosi + dstCy * Sini);
                dstYi = -(dstCx * -Sini + dstCy * Cosi);
            }
            else
            {
                dstXf = srcCx - (dstCx * Cos + dstCy * Sin);
                dstYf = srcCy - (dstCx * -Sin + dstCy * Cos);
            }

            for (int j = 0; j < dstH; j++)
            {
                if (intCalculation)
                {
                    xi = dstXi;
                    yi = dstYi;
                }
                else
                {
                    x3 = dstXf;
                    y3 = dstYf;
                }

                int* pDst = dst + (dstW * j);

                for (int i = 0; i < dstW; i++)
                {
                    if (intCalculation)
                    {
                        x0 = srcCx + (xi >> Angles.BigExp);
                        y0 = srcCy + (yi >> Angles.BigExp);
                    }
                    else
                    {
                        x0 = (int)x3;
                        y0 = (int)y3;
                    }

                    if (x0 < 0 || y0 < 0 || x0 >= srcW || y0 >= srcH)
                    {
                        pDst++;
                        goto horizotalIncrement;
                    }

                    var index = x0 + (y0 * srcW);

                    if (intCalculation || (x0 - x3 == 0 && y3 - y0 == 0))
                    {
                        color = src[index];
                        if (color == 0)
                            color = *pDst;
                        goto assignColor;
                    }
                    float Dx = x3 - x0;
                    float Dy = y3 - y0;

                    #region BI-LINEAR INTERPOLATION
                    uint rb, ag, c3 = 0, c4 = 0;
                    int n = index + srcW;
                    bool only2 = (n >= srcLen || n + 1 >= srcLen);

                    uint c1 = (uint)src[index++];
                    uint c2 = (uint)src[index];
                    if (!only2)
                    {
                        c3 = (uint)src[n++];
                        c4 = (uint)src[n];
                    }
                    if (c1 == 0 || c1 == Colors.Transparent)
                        c1 = Colors.UWhite;
                    if (c2 == 0 || c2 == Colors.Transparent)
                        c2 = Colors.UWhite;

                    if (c3 == 0 || c3 == Colors.Transparent)
                        c3 = Colors.UWhite;
                    if (c4 == 0 || c4 == Colors.Transparent)
                        c4 = Colors.UWhite;

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
                    if (only2)
                    {
                        color = (int)c1;
                        goto assignColor;
                    }

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

                    assignColor:
                    *pDst++ = color;

                horizotalIncrement:
                    #region HORIZONTAL INCREMENT
                    if (intCalculation)
                    {
                        xi += Cosi;
                        yi -= Sini;
                    }
                    else
                    {
                        x3 += Cos;
                        y3 -= Sin;
                    }
                    #endregion
                }

                #region VERTICAL INCREMENT
                if (intCalculation)
                {
                    dstXi += Sini;
                    dstYi += Cosi;
                }
                else
                {
                    dstXf += Sin;
                    dstYf += Cos;
                }
                #endregion
            }
            data = (IntPtr)dst;
            return size;
        }

        /// <summary>
        /// Returns a flipped version of this object alogwith size of it.
        /// </summary>
        /// <param name="flipMode"></param>
        /// <returns>Flipped copy of this object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Size Flip(this IBlockable source, out IntPtr data, FlipMode flipMode)
        {
            if (source is IScalable)
            {
                return ((IScalable)source).Flip(out data, flipMode);
            }
            int dstW = source.Width;
            int dstH = source.Height;
            int dstLen = source.Length;

            int srcW = dstW;
            int srcH = dstH;
            int srcLen = dstLen;
            int* src;

            int* dst;
            fixed (int* p = new int[dstW * dstH])
                dst = p;
            data = IntPtr.Zero;

            fixed (int* p = new int[srcW * srcH])
                src = p;
            if (source is IPixels)
            {
                Blocks.Copy((int*)((IPixels)source).Source, 0, src, 0, srcW,  Command.Opaque);
            }
            else if (source is ICopyable)
            {
                ((ICopyable)source).CopyTo(0, 0, srcW, srcH, (IntPtr)src, srcLen, srcW, 0, 0, 0, null);
            }
            else
            {
                data = IntPtr.Zero;
                return Size.Empty;
            }
            int i = 0;
            if (flipMode == FlipMode.Horizontal)
            {
                for (var y = srcH - 1; y >= 0; y--)
                {
                    for (var x = 0; x < srcW; x++)
                    {
                        var srcInd = y * srcW + x;
                        dst[i] = src[srcInd];
                        i++;
                    }
                }
            }
            else
            {
                for (var y = 0; y < srcH; y++)
                {
                    for (var x = srcW - 1; x >= 0; x--)
                    {
                        var srcInd = y * srcW + x;
                        dst[i] = src[srcInd];
                        i++;
                    }
                }
            }

            if (flipMode == FlipMode.Vertical)
                Numbers.Swap(ref srcW, ref srcH);
            data = (IntPtr)dst;
            return new Size(srcW, srcH);
        }
        #endregion
    }
    partial class Renderer
    {
        #region RENDER CIRCLE OR ELLIPSE
        static void RenderCircleOrEllipse(this IWritable buffer, float x, float y, float width, float height,
            ISettings settings = null)
        {
            if (buffer == null)
                return;
            if (settings == null)
                settings = Factory.newSettings();

            var curve = Factory.newCurve(x, y, width, height, 0, 0, CurveType.Full, settings.Rotation, settings.Scale);
            DrawCurve(buffer, curve, settings);
        }
        static void RenderCircleOrEllipse(this IWritable buffer, VectorF first, VectorF second, VectorF third, ISettings settings = null,
            CurveType type = CurveType.Full)
        { 
            if (buffer == null)
                return;
            type = type.Exclude(CurveType.Arc).Exclude(CurveType.Pie).Exclude(CurveType.ClosedArc);
            type = type.Include(CurveType.Full);
            if (settings == null)
                settings = Factory.newSettings();
            var curve = Factory.newCurve(first, second, third, type, settings.Rotation, settings.Scale);
            DrawCurve(buffer, curve, settings);
        }

        static void RenderCircleOrEllipse(this IWritable buffer, VectorF first, VectorF second, VectorF third, VectorF fourth,
            ISettings settings = null, CurveType type = CurveType.Full)
        {
            if (buffer == null)
                return;
            type = type.Exclude(CurveType.Arc).Exclude(CurveType.Pie).Exclude(CurveType.ClosedArc);
            type = type.Include(CurveType.Full);
            if (settings == null)
                settings = Factory.newSettings();
            var curve = Factory.newCurve(first, second, third, fourth, type, settings.Rotation, settings.Scale);
            DrawCurve(buffer, curve, settings);
        }
        static void RenderCircleOrEllipse(this IWritable buffer, VectorF first, VectorF second, VectorF third, VectorF fourth, VectorF fifth,
            ISettings settings = null)
        {
            if (buffer == null)
                return;
            if (settings == null)
                settings = Factory.newSettings();
            ICurve curve = Factory.newCurve(first, second, third, fourth, fifth, CurveType.Full, settings.Rotation, settings.Scale);
            DrawCurve(buffer, curve, settings);
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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        static void RenderArcOrPie(this IWritable buffer, float x, float y, float width, float height,
            float startAngle, float endAngle, ISettings settings = null, CurveType type = CurveType.Pie)
        {
            if (buffer == null)
                return;

            type = type.Exclude(CurveType.Full);
            string name = type.HasFlag(CurveType.Arc) ? "Arc" : "Pie";
            if (settings == null)
                settings = Factory.newSettings();

            var curve = Factory.newCurve(x, y, width, height, startAngle, endAngle, type, settings.Rotation, settings.Scale);
            DrawCurve(buffer, curve, settings);
        }

        /// <summary>
        /// Renders an arc or pie specified by three points and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="buffer">Buffer which to render a circle/ellipse on</param>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        static void RenderArcOrPie(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, ISettings settings = null, CurveType type = CurveType.Pie)
        {
            if (buffer == null)
                return;
            type = type.Exclude(CurveType.Full);
            string name = type.HasFlag(CurveType.Arc) ? "Arc" : "Pie";
            if (settings == null)
                settings = Factory.newSettings();

            var curve = Factory.newCurve(p1, p2, p3, type, settings.Rotation, settings.Scale);
            DrawCurve(buffer, curve, settings);
        }

        /// <summary>
        /// Renders an arc or pie specified by four points and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="buffer">Buffer which to render a circle/ellipse on</param>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        static void RenderArcOrPie(this IWritable buffer, VectorF p1, VectorF p2, VectorF p3, VectorF p4, ISettings settings = null, CurveType type = CurveType.Pie)
        {
            if (buffer == null)
                return;
            type = type.Exclude(CurveType.Full);
            string name = type.HasFlag(CurveType.Arc) ? "Arc" : "Pie";
            if (settings == null)
                settings = Factory.newSettings();
            var curve = Factory.newCurve(p1, p2, p3, p4, type, settings.Rotation, settings.Scale);
            DrawCurve(buffer, curve, settings);
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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the arc/pie</param>
        /// <param name="type"> Defines the type of curve for example an arc or pie etc. along with other supplimentary options on how to draw it</param>
        static void RenderArcOrPie(this IWritable buffer, VectorF first, VectorF second, VectorF third, VectorF fourth,
            VectorF fifth, ISettings settings = null, CurveType type = CurveType.Pie)
        {
            if (buffer == null)
                return;
            string name = type.HasFlag(CurveType.Arc) ? "Arc" : "Pie";
            if (settings == null)
                settings = Factory.newSettings();
            var curve = Factory.newCurve(first, second, third, fourth, fifth, type, settings.Rotation, settings.Scale);
            DrawCurve(buffer, curve, settings);
        }
        #endregion

        #region RENDER BEZIER
        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">Buffer which to render a bezier on</param>
        /// <param name="pts">Defines perimiter of the bezier</param>
        /// <param name="type">BezierType enum determines the type of bezier i.e Cubic - group of 4 points or multiple(group of 4 or 7 or 10 so on...)</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the bezier</param>
        static void RenderBezier(this IWritable buffer, IEnumerable<float> pts, BezierType type = BezierType.Cubic,
            ISettings settings = null)
        {
            if (buffer == null)
                return;

            IBezier bezier = Factory.newBezier(type, pts.ToArray(), default(IList<VectorF>));
            buffer.Render(bezier, settings);
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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the traingle</param>
        static void RenderTriangle(this IWritable buffer, float x1, float y1, float x2, float y2, float x3, float y3, ISettings settings)
        {
            if (buffer == null)
                return;
            ITriangle triangle = Factory.newTriangle(x1, y1, x2, y2, x3, y3);
            buffer.Render(triangle, settings);
        }
        #endregion

        #region RENDER POLYGON
        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">Buffer which to render a polygon on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon an each group of two subsequent values in polypoints forms a point x,y</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the polygon</param>
        static void RenderPolygon(this IWritable buffer, IEnumerable<float> polyPoints, ISettings settings)
        {
            if (buffer == null)
                return;
            IList<VectorF> points = polyPoints.ToPoints();
            var polygon = Factory.newShape(points, "Polygon");
            buffer.Render(polygon, settings);
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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rectangle</param>
        static void RenderRectangle(this IWritable buffer, float x, float y, float width, float height, ISettings settings)
        {
            if (buffer == null)
                return;
            IBoxF box = Factory.newBoxF(x, y, width, height);
            buffer.Render(box, settings);
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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        /// <param name="angle">Angle to apply rotation while rendering the rounded box</param>
        static void RenderRoundedBox(this IWritable buffer, float x, float y, float width, float height, float cornerRadius,
            ISettings settings, RoundBoxOption option = 0)
        {
            if (buffer == null)
                return;

            var shape = Factory.newRoundBox(x, y, width, height, cornerRadius, option);
            buffer.Render(shape, settings);
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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        static void RenderRhombus(this IWritable buffer, float x, float y, float width, float height, float? deviation, ISettings settings) =>
            RenderRectangle(buffer, x, y, (deviation ?? width), height, settings);

        /// <summary>
        /// Renders a rhombus specified by three points (The fourth point will be calculated) and angle of rotation if supplied.
        /// </summary>
        /// <param name="buffer">Buffer which to render a rhombus on</param>
        /// <param name="first">First point</param>
        /// <param name="second">Second point</param>
        /// <param name="third">Third point</param>
        /// <param name="angle">Angle to apply rotation while rendering the rhombus</param>
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        static void RenderRhombus(this IWritable buffer, VectorF first, VectorF second, VectorF third, ISettings settings)
        {
            var rhombus = Factory.newTetragon(first, second, third);
            buffer.Render(rhombus, settings);
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
        /// <param name="settings">A pen settings which to create a buffer pen from</param>
        static void RenderTrapezium(this IWritable buffer, ILine baseLine, float deviation, float skeyBy, ISettings settings)
        {
            if (buffer == null)
                return;
            if (settings == null)
                settings = Factory.newSettings();
            ITetragon trapezium = Factory.newTetragon(baseLine, deviation, settings.StrokeMode, skeyBy);
            buffer.Render(trapezium, settings);
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
        public static void CreateAction(this ICollection<VectorF> list, out PixelAction action)
        {
            action = (val1, axis, horizontal, cmd) =>
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
        public static void CreateAction(this ICollection<Vector> list, out PixelAction action)
        {
            action = (val, axis, horizontal, cmd) =>
            {
                int iVal = (int)val;
                if (val - iVal >= 0.5f)
                    ++iVal;
                list.Add(new Vector(iVal, axis, horizontal));
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
        public static void Process(this ILine line, PixelAction action, Command lineCommand, SlopeType skip = SlopeType.None, Size clip = default(Size))
        {
            if (line == null || !line.Valid)
                return;
            ProcessLine(line.X1, line.Y1, line.X2, line.Y2, action, lineCommand, skip, clip);
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
        public static void ProcessLine(int x1, int y1, int x2, int y2, VectorAction<int> action, Command lineCommand, Size clip = default(Size))
        {
            ProcessLine(x1, y1, x2, y2, action.ToPixelAction(), lineCommand, 0, clip);
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
        public static void ProcessLine(float x1, float y1, float x2, float y2, VectorAction<float> action, Command lineCommand, Size clip = default(Size))
        {
            ProcessLine(x1, y1, x2, y2, action.ToPixelAction(), lineCommand, 0, clip);
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
        public static void Process(this IEnumerable<ILine> lines, PixelAction action, Command lineCommand, 
            SlopeType skip = SlopeType.None, Size clip = default(Size))
        {
            if (lines == null || skip == SlopeType.Both)
                return;

            foreach (var l in lines)
                Process(l, action, lineCommand, skip, clip);
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
        public static void ScanLines(this IEnumerable<ILine> lines, PixelAction scanAction, bool? horizontalScan)
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
        public static void ProcessTriangle(this IPolyFill polyFill, VectorF p1, VectorF p2, VectorF p3, FillAction Action,
            FillMode fillMode, Command drawCommand, bool drawOutLines = true)
        {
            bool drawOutLinesOnly = fillMode == FillMode.DrawOutLine;
            drawOutLines = drawOutLines || drawOutLinesOnly;
            SlopeType steep = drawOutLinesOnly ? 0 : SlopeType.Steep;

            if (drawOutLines)
            {
                var pixelAction = Action.ToPixelAction();
                ProcessLine(p1.X, p1.Y, p2.X, p2.Y, pixelAction, drawCommand, steep);
                ProcessLine(p2.X, p2.Y, p3.X, p3.Y, pixelAction, drawCommand, steep);
                ProcessLine(p3.X, p3.Y, p1.X, p1.Y, pixelAction, drawCommand, steep);
            }

            if (drawOutLinesOnly)
                return;

            Vectors.MinMax(polyFill.Clip, out _, out float minY, out _, out float maxY, p1, p2, p3);
            polyFill.Command |= Command.IgnoreAutoCalculatedFillPatten;

            polyFill.Begin(minY.Round(), (int)maxY + 1);
            polyFill.Command |= Command.OddEvenPolyFill;

            ScanLine(p1.X, p1.Y, p2.X, p2.Y, true, polyFill.ScanAction);
            ScanLine(p2.X, p2.Y, p3.X, p3.Y, true, polyFill.ScanAction);
            ScanLine(p3.X, p3.Y, p1.X, p1.Y, true, polyFill.ScanAction);

            polyFill.Fill(Action);
            polyFill.End();
            polyFill.Command &= ~Command.IgnoreAutoCalculatedFillPatten;
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
        public static void ProcessQuardilateral(this IPolyFill polyFill, VectorF p1, VectorF p2, VectorF p3, VectorF p4,
            FillAction Action, FillMode fillMode, bool drawOutLines = true)
        {
            bool drawOutLinesOnly = fillMode == FillMode.DrawOutLine;
            drawOutLines = drawOutLines || drawOutLinesOnly;
            SlopeType steep = drawOutLinesOnly ? 0 : SlopeType.Steep;
            var fillCommand = polyFill.Command;
            var clip = polyFill.Clip;

            if (drawOutLines)
            {
                var pixelAction = Action.ToPixelAction();
                ProcessLine(p1.X, p1.Y, p2.X, p2.Y, pixelAction, fillCommand, steep);
                ProcessLine(p3.X, p3.Y, p4.X, p4.Y, pixelAction, fillCommand, steep);
                ProcessLine(p1.X, p1.Y, p3.X, p3.Y, pixelAction, fillCommand, steep);
                ProcessLine(p2.X, p2.Y, p4.X, p4.Y, pixelAction, fillCommand, steep);
            }

            if (drawOutLinesOnly)
                return;

            Vectors.MinMax(clip, out float minX, out float minY, out float maxX, out float maxY, p1, p2, p3, p4);

            polyFill.Begin(minY.Round(), (int)maxY + 1);

            ScanLine(p1.X, p1.Y, p2.X, p2.Y, true, polyFill.ScanAction);
            ScanLine(p4.X, p4.Y, p3.X, p3.Y, true, polyFill.ScanAction);

            ScanLine(p1.X, p1.Y, p3.X, p3.Y, true, polyFill.ScanAction);
            ScanLine(p2.X, p2.Y, p4.X, p4.Y, true, polyFill.ScanAction);

            polyFill.Fill(Action);
            polyFill.End();
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
        public static void ProcessRhombus(this IPolyFill polyFill, VectorF p1, VectorF p2, VectorF p3,
            FillAction Action, FillMode fillMode, bool drawOutLines = true)
        {
            var p4 = Vectors.FourthPointOfRhombus(p1, p2, p3);
            polyFill.ProcessQuardilateral(p1, p2, p3, p4, Action, fillMode, drawOutLines);
        }
        #endregion

        #region PROCESS CONIC
        /// <summary>
        /// Process conic - notifying each obtained axial scan line by executing specified action.
        /// </summary>
        /// <param name="conic">Conic object to process.</param>
        /// <param name="action">A FillAction delegate which has routine to do something with the axial scan line information provided.</param>
        /// <param name="forDrawingOnly">If true, only ends point of axial line will be drawn otherwise full line.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessConic(this IPolyFill polyFill, IConic conic, FillAction action, float Stroke, bool forDrawingOnly)
        {
            if (action == null)
                return;
            CurveType type = CurveType.Full;
            if (conic is ICurve)
                type = ((ICurve)conic).Type;

            bool Full = !(type.HasFlag(CurveType.Arc) || type.HasFlag(CurveType.Pie));
            if (!Full && conic is ICurve)
            {
                var lines = (conic as ICurve).GetClosingLines();
                if (lines != null)
                {
                    var paction = action.ToPixelAction();
                    foreach (var line in lines)
                        Process(line, paction, polyFill.Command);
                }
            }

            if (Stroke == 0 && type.HasFlag(CurveType.Arc))
                forDrawingOnly = true;
            var fillCommand = polyFill.Command;

            polyFill.Command |= Command.DrawEndsOnly;
            polyFill.Command |= Command.FillSinglePointLine;

            int i;
            int a1, a2;

            i = conic.GetBoundary(false, true);
            while (i >= 0)
            {
                var list = conic.GetDataAt(i, false, true, out a1, out a2);
                polyFill.FillLine(list[0], a1, false, action);
                polyFill.FillLine(list[1], a2, false, action);
                i -= 1;
            }

            if (!forDrawingOnly)
                polyFill.Command &= ~Command.DrawEndsOnly;

            polyFill.Command &= ~Command.DrawLineOnly;
            i = conic.GetBoundary(true, forDrawingOnly);

            while (i >= 0)
            {
                var list = conic.GetDataAt(i, true, forDrawingOnly, out a1, out a2);
                polyFill.FillLine(list[0], a1, true, action);
                polyFill.FillLine(list[1], a2, true, action);
                i -= 1;
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
        public static void ProcessWith(this IList<ILine> Outer, IList<ILine> Inner, FillAction action,
            FillMode fillMode, Command fillCommand, Size clip)
        {
            var length = Math.Min(Outer.Count, Inner.Count);
            VectorF p1, p2, p3, p4;
            using (var polyFill = Factory.newPolyFill())
            {
                polyFill.Command = fillCommand | Command.OddEvenPolyFill | Command.Outlininig;
                polyFill.Clip = clip;
                for (int i = 0; i < length; i++)
                {
                    p1 = new VectorF(Outer[i].X1, Outer[i].Y1);
                    p2 = new VectorF(Outer[i].X2, Outer[i].Y2);
                    p3 = new VectorF(Inner[i].X1, Inner[i].Y1);
                    p4 = new VectorF(Inner[i].X2, Inner[i].Y2);
                    polyFill.ProcessQuardilateral(p1, p2, p3, p4, action, fillMode, false);
                }
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IImage RotateAndScale(this IScalable buffer, Rotation angle, bool antiAliased = true, float scale = 1)
        {
            var sz = buffer.RotateAndScale(out IntPtr data, angle, antiAliased, scale);
            return Factory.newImage(data, sz.Width, sz.Height);
        }
        #endregion

        #region FLIP
        /// <summary>
        /// Returns a flipped version of this object.
        /// </summary>
        /// <param name="flipMode"></param>
        /// <returns>Flipped copy of this object.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IImage Flip(this IScalable buffer, FlipMode flipMode)
        {
            var sz = buffer.Flip(out IntPtr data, flipMode);
            return Factory.newImage(data, sz.Width, sz.Height);
        }
        #endregion
    }
}
#endif
