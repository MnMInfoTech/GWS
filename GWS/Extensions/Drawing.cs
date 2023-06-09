/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    partial class Drawing
    {
        #region RENDER
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
#if DevSupport
        public
#else
    internal
#endif
        static bool Render(this IRenderer Renderer, IObject Shape, IEnumerable<IParameter> parameters)
        {
            var renderer = (IExRenderer)Renderer;
            bool IsImage = (Shape is IImageSource);

            parameters.Extract(out IExSession session);
            var OCommand = session.Command;
            session.Command |= Command.SkipDisplayUpdate;

            if ((session.Command & Command.Calculate) == Command.Calculate)
            {
                bool HasAbsBoundary = session.Boundaries.Count > 0 &&
                    session.Boundaries.OfType<IExBoundary>().Any(bdr => bdr.Kind == BoundaryKind.AbsBoundary);

                if (!HasAbsBoundary)
                {
                    session.Boundaries.Add(Factory.newBoundary(BoundaryKind.AbsBoundary));
                }
            }

            if (session.Boundaries != null)
            {
                foreach (var boundary in session.Boundaries.OfType<IExBoundary>())
                    boundary.Clear();
            }

            if (IsImage)
            {
                var action = Renderer.CreateRenderAction(session);
                action(null, null, (IImageSource)Shape);
                goto Exit;
            }
            if (Shape is IExDraw)
            {
                ((IExDraw)Shape).Draw(session, renderer);
                goto End;
            }
            if (Shape is IDraw)
            {
                ((IDraw)Shape).Draw(session, Renderer);
                goto End;
            }
            if (Shape is IPolygonCollection)
            {
                renderer.RenderPolygons((IPolygonCollection)Shape, session);
                goto End;
            }
            if (Shape is IPolygonalF)
            {
                renderer.RenderPolygon((IPolygonalF)Shape, session);
                goto End;
            }

            Factory.factory.HandleRenderingOfUnknownObject(Shape, Renderer, session);
            End:
            (session.PenContext as ISettings)?.FlushSettings();

            Exit:
            session.RenderBounds?.Clear();
            session.Command = OCommand;
            return true;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool RenderWithUpdate(this IExRenderer Renderer, IObject Shape, IEnumerable<IParameter> parameters)
        {
            parameters.Extract(out IExSession info);
            var Command = info.Command;

            Render(Renderer, Shape, parameters);

            if ((Command & Command.SkipDisplayUpdate) != Command.SkipDisplayUpdate
                && Renderer is IUpdatable
                && info.Boundaries.Count > 0)
            {
                var updateCommand = Command.ToEnum<UpdateCommand>();
                ((IUpdatable)Renderer).Update(info.Boundaries[0], updateCommand);
            }
            info.Command = Command;
            return true;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Render(this IExRenderer graphics, IList<ILine>[] Data, IEnumerable<IParameter> parameters,  bool Screen, 
            FillCommand FillMode, bool isBezier = false)
        {
            bool OriginalFill = FillMode == 0 || (FillMode & FillCommand.OriginalFill) == FillCommand.OriginalFill;
            bool DrawOutLine = (FillMode & FillCommand.DrawOutLines) == FillCommand.DrawOutLines;

            bool SkipLineDraw = (FillMode & FillCommand.SkipDraw) == FillCommand.SkipDraw;
            //bool Screen = (FillMode & FillMode.Screen) == FillMode.Screen;

            bool FloodFill = (FillMode & FillCommand.FloodFill) == FillCommand.FloodFill &&
                (FillMode & FillCommand.OriginalFill) != FillCommand.OriginalFill;

            using (var polyFill = graphics.newPolyFill(parameters, FillMode))
            {
                IEnumerable<ILine> Data20 = null;
                if (Data[2] != null || Data[0] != null)
                    Data20 = Data[2].AppendItems(Data[0]);
                else
                    SkipLineDraw = true;

                if (FloodFill)
                {
                    bool HasInner = Data[3] != null;
                    if (HasInner)
                    {
                        int length = Math.Min(Data[1].Count, Data[3].Count);
                        if (isBezier)
                            --length;
                        VectorF p1, p2, p3, p4;

                        if (!SkipLineDraw && Screen)
                            polyFill.RenderAny(null, Data20);

                        for (int i = 0; i < length; i++)
                        {
                            p1 = new VectorF(Data[1][i].X1, Data[1][i].Y1);
                            p2 = new VectorF(Data[1][i].X2, Data[1][i].Y2);
                            p3 = new VectorF(Data[3][i].X1, Data[3][i].Y1);
                            p4 = new VectorF(Data[3][i].X2, Data[3][i].Y2);
                            polyFill.ProcessQuardilateral(p1, p2, p3, p4, FillMode, false);
                        }
                        if (!SkipLineDraw && !Screen)
                            polyFill.RenderAny(null, Data20);
                    }
                }
                polyFill.Begin();
                polyFill.Scan(Data[1].AppendItems(Data[3]));
                return polyFill.RenderAny(polyFill.GetScanLines(), Data20);
            }
        }
        #endregion

        #region SET PEN
        /// <summary>
        /// Sets pen of this object using specified shape and writeable object settings.
        /// </summary>
        /// <param name="Shape">Shape which to set the pen for.</param>
        /// <param name="InParameters">Collection of parameters to control rendering process.</param>
        /// has shape id and destination info.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISession SetPen(this IEnumerable<IParameter> InParameters,
            IObject Shape, IRectangle suppliedBounds = null)
        {
            InParameters.Extract(out IExSession session);
            IRotation ORotation = session.Rotation;

            if (Shape is IDegreeHolder)
            {
                if (session.Rotation == null)
                    session.Rotation = new Rotation();
                session.Rotation.SetDegree(((IDegreeHolder)Shape)?.Degree?.Angle);
            }

            bool SizeToFit = (session.Command & Command.SizeToFit) == Command.SizeToFit;
            bool CalculateMode = !SizeToFit && (session.Command & Command.Calculate) == Command.Calculate;

            IPen Pen = null;
            if (suppliedBounds != null)
                session.RenderBounds?.Set(suppliedBounds);

            if (!session.RenderBounds.Valid)
                session.RenderBounds.Set(Shape.ToArea());

            session.RenderBounds.Set(session.RenderBounds.Scale(session.Rotation, session.Scale));

            if (!CalculateMode)
            {
                session.RenderBounds.GetBounds(out int x, out int y, out int w, out int h);
                IPenContext Context = session.PenContext;
                if (Context == null)
                {
                    Pen = Rgba.Foreground;
                    goto EXIT;
                }
                Pen = Context.ToPen(w, h);
            }
            EXIT:
            if (!CalculateMode)
            {
                if (Pen is ISettings)
                    (Pen as ISettings)?.ReceiveSettings(session);
            }
            session.Rotation = ORotation;
            if (!CalculateMode)
            {
                session.PenContext = Pen;
                //OutParameters = OutParameters.AppendItem(Pen);
            }
            return session;
        }
        #endregion

        #region WRITE POLYGON
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool RenderPolygon(this IExRenderer graphics, IPolygonalF Shape,
            IEnumerable<IParameter> parameters = null)
        {
            if (Shape == null)
                return false;

            parameters.Extract(out IExSession session);

            string Type = Shape.GetType().Name + "";

            IEnumerable<VectorF> Original = Shape.GetPoints().ToArray(), Outer, Inner;
            if (Original == null)
                return true;

            var bounds = Shape.ToArea();
            bool HasScale = session.Scale?.HasScale ?? false;
            bool HasRotation = session.Rotation != null && session.Rotation.Valid;
            float Cx = 0, Cy = 0;

            if (HasScale)
            {
                Original = Original.Scale(session.Scale, bounds.Center());
            }

            bool isRotated = HasRotation && session.Rotation.EffectiveCenter(bounds, out Cx, out Cy);
            bool OriginalFill = (session.Command & Command.OriginalFill) == Command.OriginalFill;
            bool IsBezier = Shape is IBezier;
            FillCommand FillMode = session.Command.ToEnum<FillCommand>();

            if (session.Stroke == 0 || OriginalFill)
            {
                if (isRotated)
                {
                    Original = Original.Rotate(session.Rotation, Cx, Cy);
                }
                if (IsBezier)
                {
                    Original = Curves.GetBezierPoints(12, ((IBezier)Shape).Option, Original.ToArray());
                }

                if (Original is IList<VectorF>)
                    Outer = Inner = Original as IList<VectorF>;
                else
                    Outer = Inner = Original.ToArray();
            }
            else
            {
                if (IsBezier)
                    Original = Curves.GetBezierPoints(16, ((IBezier)Shape).Option, Original.ToArray());

                Original.StrokePoints(Type, session.Stroke, session.Command.ToEnum<FillCommand>(), out Outer, out Inner, out bool _);

                bounds = Outer.HybridBounds(Inner).Expand();
                if (isRotated)
                {
                    Outer = Outer.Rotate(session.Rotation, Cx, Cy);
                    Inner = Inner.Rotate(session.Rotation, Cx, Cy);
                }
            }
            session.SetPen(bounds);
            Factory.ShapeParser.ResetFillOptions(ref FillMode, Type, session.Stroke);
            session.UpdateWith(FillMode, ModifyCommand.Replace);
            var Data = GetDrawParams(session.Command, session.Stroke, Type, Original, Outer, Inner);
            return graphics.Render(Data, session, (session.Command & Command.Screen) == Command.Screen, session.Command.ToEnum<FillCommand>(), IsBezier);
        }
        #endregion

        #region WRITE POLYGONS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool RenderPolygons(this IExRenderer graphics, IPolygonCollection polygons,
            IEnumerable<IParameter> parameters = null)
        {
            if (polygons == null)
                return false;
            var items = parameters;
            if (polygons.Parameters != null)
                items = items.AppendItems(polygons.Parameters);

            parameters.Extract(out IExSession session);

            string Type = "Polygon";
            PrimitiveList<ILine>[] Data = new PrimitiveList<ILine>[4];
            for (int m = 0; m < 4; m++)
            {
                Data[m] = new PrimitiveList<ILine>(10);
            }
            var notifiable = (IExBoundary)Factory.newBoundary();
            bool OriginalFill = (session.Command & Command.OriginalFill) == Command.OriginalFill;
            Rectangle o = Rectangle.Empty;
            var FillMode = session.Command.ToEnum<FillCommand>();

            foreach (var Shape in polygons)
            {
                if (Shape == null)
                    continue;

                IEnumerable<VectorF> Original = Shape.GetPoints().ToArray();
                IEnumerable<VectorF> Outer, Inner;

                var bounds = Original.ToArea();
                bool HasScale = session.Scale?.HasScale ?? false;
                bool HasRotation = session.Rotation != null && session.Rotation.Valid;
                float Cx = 0, Cy = 0;

                if (HasScale)
                    Original = Original.Scale(session.Scale, bounds.Center());

                bool isRotated = HasRotation && session.Rotation.EffectiveCenter(bounds, out Cx, out Cy);
                bool IsBezier = Shape is IBezier;

                if (session.Stroke == 0 || OriginalFill)
                {
                    if (isRotated)
                        Original = Original.Rotate(session.Rotation, Cx, Cy);
                    if (IsBezier)
                        Original = Curves.GetBezierPoints(8, ((IBezier)Shape).Option, Original.ToArray());

                    if (Original is IList<VectorF>)
                        Outer = Inner = Original as IList<VectorF>;
                    else
                        Outer = Inner = Original.ToList();
                }
                else
                {
                    if (IsBezier)
                        Original = Curves.GetBezierPoints(8, ((IBezier)Shape).Option, Original.ToArray());

                    Original.StrokePoints(Type, session.Stroke, FillMode, out Outer, out Inner, out bool swapped);

                    bounds = Outer.HybridBounds(Inner).Expand();
                    if (isRotated)
                    {
                        Outer = Outer.Rotate(session.Rotation, Cx, Cy);
                        Inner = Inner.Rotate(session.Rotation, Cx, Cy);
                    }
                }
                notifiable.Update(bounds.Expand(1));

                var ItemData = GetDrawParams(session.Command, session.Stroke, Type, Original, Outer, Inner);
                for (int n = 0; n < ItemData.Length; n++)
                {
                    if (ItemData[n] != null)
                        Data[n].AddRange(ItemData[n]);
                }
            }

            Factory.ShapeParser.ResetFillOptions(ref FillMode, Type, session.Stroke);
            session.UpdateWith(FillMode, ModifyCommand.Replace);

            session.SetPen(notifiable);
            return graphics.Render(Data, session, (session.Command & Command.Screen) == Command.Screen, 
                session.Command.ToEnum<FillCommand>(), false);
        }
        #endregion

        #region CREATE VECTOR ACTION
        /// <summary>
        /// Retuns an action delegate for storing an axial line or pixel information in specified list.
        /// </summary>
        /// <param name="list">A list to accumulate pixels and axial lines resulted from executing an action</param>
        /// <returns>An instance of FillAction delegate</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CreateAction(this ICollection<VectorF> list, out VectorAction<float> action)
        {
            action = (x, y) =>
            {
                list.Add(new VectorF(x, y));
            };
        }
        #endregion

        #region FILL
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines">Lines which forms the perimeter of a shape</param>
        /// <param name="action">RenderAction delegate to propogate fill operation.</param>
        /// <param name="y">Vertical start of filling operation.</param>
        /// <param name="bottom">Vertical end of filling operation.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Fill(this IEnumerable<ILine> lines, RenderAction action)
        {
            using (var polyFill = Factory.newPolyFill(action))
            {
                return polyFill.DoAll(lines);
            }
        }

        /// <summary>
        /// Fills and sraws the area covered by specified lines.
        /// </summary>
        /// <param name="lines">Lines which forms the perimeter of a shape</param>
        /// <param name="graphics">Graphics object to operate upon.</param>
        /// <param name="parameters">Collection of parameters to control render operation.</param>
        /// <param name="y">Vertical start of filling operation.</param>
        /// <param name="bottom">Vertical end of filling operation.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Fill(this IEnumerable<ILine> lines, IRenderer graphics,
            IEnumerable<IParameter> parameters)
        {
            using (var polyFill = graphics.newPolyFill(parameters))
            {
                return polyFill.DoAll(lines);
            }
        }

        /// <summary>
        /// Fills and draws the area between specified two collections of lines.
        /// Filling is done by scanning each line of outer perimeter with correspoinding line of inner perimeter at given index.
        /// </summary>
        /// <param name="Outer">Outer perimeter of shape.</param>
        /// <param name="Inner">Inner perimeter of shape.</param>
        /// <param name="action">FillAction delegate to propogate fill operation.</param>
        /// <param name="fillMode"></param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Fill(this IList<ILine> Outer, IList<ILine> Inner,
            RenderAction action, FillCommand fillMode, bool isBezier = false)
        {
            int length = Outer.Count;

            bool HasInner = Inner != null;
            if (HasInner)
            {
                length = Math.Min(Outer.Count, Inner.Count);
                if (isBezier)
                    --length;
            }
            VectorF p1, p2, p3, p4;
            using (var polyFill = Factory.newPolyFill(action))
            {
                if (HasInner)
                {
                    for (int i = 0; i < length; i++)
                    {
                        p1 = new VectorF(Outer[i].X1, Outer[i].Y1);
                        p2 = new VectorF(Outer[i].X2, Outer[i].Y2);
                        p3 = new VectorF(Inner[i].X1, Inner[i].Y1);
                        p4 = new VectorF(Inner[i].X2, Inner[i].Y2);
                        polyFill.ProcessQuardilateral(p1, p2, p3, p4, fillMode, false);
                    }
                }
                else
                {
                    return polyFill.DoAll(Outer);
                }
            }
            return false;
        }
        #endregion

        #region PROCESS TRIANGLE
        /// <summary>
        /// Scan triangle lines horizontally and performs fill action.
        /// </summary>
        /// <param name="p1">1st point of triangle.</param>
        /// <param name="p2">2st point of triangle.</param>
        /// <param name="p3">3st point of triangle.</param>
        /// <param name="fillMode"></param>
        /// <param name="drawOutLines">>If true, border around filled area will get drawn.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ProcessTriangle(this IPolygonFiller polyFill, VectorF p1, VectorF p2, VectorF p3,
            FillCommand fillMode, bool drawOutLines = true)
        {
            bool drawOutLinesOnly = (fillMode & FillCommand.DrawOutLines) == FillCommand.DrawOutLines;
            polyFill.Begin();
            polyFill.Scan(new ILine[]
             {
                new Line(p1.X, p1.Y, p2.X, p2.Y),
                new Line(p2.X, p2.Y, p3.X, p3.Y),
                new Line(p3.X, p3.Y, p1.X, p1.Y)
             });

            FillCommand polyState =
                drawOutLinesOnly ? FillCommand.DrawOutLinesOnly :
                drawOutLines ? FillCommand.DrawOutLines : 0;
            var r = polyFill.Render(polyState);
            polyFill.End();
            return r;
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ProcessQuardilateral(this IPolygonFiller polyFill, VectorF p1, VectorF p2,
            VectorF p3, VectorF p4, FillCommand fillMode, bool drawOutLines = true)
        {
            bool drawOutLinesOnly = (fillMode & FillCommand.DrawOutLines) == FillCommand.DrawOutLines;
            polyFill.Begin();

            polyFill.Scan(new ILine[]
            {
                new Line(p1.X, p1.Y, p2.X, p2.Y),
                new Line(p2.X, p2.Y, p4.X, p4.Y),
                new Line(p4.X, p4.Y, p3.X, p3.Y),
                new Line(p3.X, p3.Y, p1.X, p1.Y)
             });


            FillCommand polyState =
                drawOutLinesOnly ? FillCommand.DrawOutLinesOnly :
                drawOutLines ? FillCommand.DrawOutLines : 0;

            var r = polyFill.Render(polyState);
            polyFill.End();
            return r;
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ProcessRhombus(this IPolygonFiller polyFill, VectorF p1, VectorF p2, VectorF p3,
            FillCommand fillMode, bool drawOutLines = true)
        {
            var p4 = Vectors.FourthPointOfRhombus(p1, p2, p3);
            return polyFill.ProcessQuardilateral(p1, p2, p3, p4, fillMode, drawOutLines);
        }
        #endregion

        #region PROCESS CONIC
        /// <summary>
        /// Process conic - notifying each obtained axial scan line by executing specified action.
        /// </summary>
        /// <param name="conic">Conic object to process.</param>
        /// <param name="action">A FillAction delegate which has routine to do something with the axial scan line information provided.</param>
        /// <param name="forDrawingOnly">If true, only ends point of axial line will be drawn otherwise full line.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ProcessConic(this IPolygonFiller polyFill, IConic conic, float Stroke,
            bool forDrawingOnly)
        {
            CurveType type = CurveType.Full;
            if (conic is ICurve)
                type = ((ICurve)conic).Type;

            bool Full = (type & CurveType.Arc) != CurveType.Arc && (type & CurveType.Pie) != CurveType.Pie;

            if (Stroke == 0 && ((type & CurveType.Arc) == CurveType.Arc))
                forDrawingOnly = true;

            var scanLines = conic.AllLines(true, forDrawingOnly);
            var points = conic.AllPoints();

            IEnumerable<IScanPoint> pts = points;

            if (!Full && conic is ICurve)
                pts = pts.AppendItems(((ICurve)conic).GetClosingLines());

            if (forDrawingOnly)
                return polyFill.RenderAny(null, pts);
            else
                return polyFill.RenderAny(scanLines, pts);
        }
        #endregion

        #region GET CONIC AXIS LINES
        /// <summary>
        /// 
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="pointsOnly"></param>
        /// <returns></returns>
        public static IReadOnlyList<IAxisLine> AllLines(this IConic conic, bool? horizontal = null,
            bool drawOutLinesOnly = false)
        {
            bool hasHoz = horizontal == null || horizontal == true;
            bool hasVert = horizontal == null || horizontal == false;
            bool both = hasHoz && hasVert;

            bool forDrawingOnly = drawOutLinesOnly;
            bool hoz = hasHoz;
            LineFill draw = hoz ? LineFill.Horizontal : LineFill.Vertical;
            draw |= forDrawingOnly ? LineFill.EndsOnly : LineFill.WithEnds;
            int i;
            int a1, a2;
            PrimitiveList<IAxisLine> scanLines = new PrimitiveList<IAxisLine>(20);

            LOOP:
            i = conic.GetBoundary(hoz, forDrawingOnly);
            while (i >= 0)
            {
                var list = conic.GetDataAt(i, hoz, forDrawingOnly, out a1, out a2);

                IAxisLine l1 = null, l2 = null;

                if (list[0].Count == 1)
                    l1 = new AxisLine(list[0][0], a1, draw);
                else if (list[0].Count == 2)
                    l1 = new AxisLineF(list[0][0], list[0][1], a1, draw);
                else if (list[0].Count != 0)
                    l1 = new OddEvenLineF(list[0], a1, draw);

                if (list[1].Count == 1)
                    l2 = new AxisLine(list[1][0], a2, draw);
                else if (list[1].Count == 2)
                    l2 = new AxisLineF(list[1][0], list[1][1], a2, draw);
                else if (list[1].Count != 0)
                    l2 = new OddEvenLineF(list[1], a2, draw);

                if (l1 != null)
                    scanLines.Add(l1);

                if (l2 != null)
                    scanLines.Add(l2);

                i -= 1;
            }
            if (both)
            {
                both = false;
                hoz = !hoz;
                draw = hoz ? LineFill.Horizontal : LineFill.Vertical;
                draw |= LineFill.EndsOnly;
                forDrawingOnly = true;
                goto LOOP;
            }

            return scanLines;
        }
        #endregion

        #region GET CONIC AXIS POINTS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static IReadOnlyList<IAxisPoint> AllPoints(this IConic conic, bool sort = false)
        {
            int i;
            int a1, a2;
            var points = new PrimitiveList<IAxisPoint>(20);
            bool hoz = true;
            LineFill draw = LineFill.Horizontal | LineFill.EndsOnly;

            IReadOnlyList<IAxisPoint> result;

            LOOP:
            i = conic.GetBoundary(hoz, true);
            while (i >= 0)
            {
                var list = conic.GetDataAt(i, hoz, true, out a1, out a2);

                foreach (var p in list[0])
                    points.Add(new AxisPoint(p, a1, draw));

                foreach (var p in list[1])
                    points.Add(new AxisPoint(p, a2, draw));

                i -= 1;
            }
            if (hoz)
            {
                hoz = false;
                draw = LineFill.Vertical | LineFill.EndsOnly;
                goto LOOP;
            }

            if (sort)
            {
                result = points.OrderBy(p => Math.Atan2(
                    (p.Draw & LineFill.Vertical) != LineFill.Vertical ? p.Axis : p.Val,
                    (p.Draw & LineFill.Vertical) != LineFill.Vertical ? p.Val : p.Axis)).ToArray();

            }
            else
                result = points;
            return result;
        }
        #endregion

        #region MEASURE GLYPHS
        public static IReadOnlyList<IGlyphLineInfo> MeasureGlyphs(this IEnumerable<IGlyph> glyphs,
            out IRectangle area, out int glyphCount, out float minHBY, TextCommand commands = 0, int maxWidth = 0)
        {
            var lines = new PrimitiveList<IGlyphLineInfo>();

            float dstX = 0, dstY = 0, x = float.MaxValue, y = float.MaxValue, r = 0, b = 0;

            float newX, newY, kerning, currentX, currentY;
            float lineHeight = glyphs.Max(g => g.Height);
            float spaceWidth = glyphs.Average(g => g.Width);

            float lh = lineHeight;
            lh += 2;
            float w = 0, h = 0;

            minHBY = glyphs.Min(g => g.MinHBY);
            if (minHBY < 0)
                dstY -= minHBY;
            else
                dstY += minHBY;

            newX = dstX;
            newY = dstY;
            bool begin = true;
            kerning = 0;
            bool Word = (commands & TextCommand.Word) == TextCommand.Word;
            bool Line = (commands & TextCommand.Line) == TextCommand.Line;
            bool SingleWord = (commands & TextCommand.SingleWord) == TextCommand.SingleWord;
            bool SingleChar = (commands & TextCommand.SingleChar) == TextCommand.SingleChar;
            bool CharDelimiter = (commands & TextCommand.CharacterDelimiter) == TextCommand.CharacterDelimiter;
            bool WordDelimiter = (commands & TextCommand.WordDelimiter) == TextCommand.WordDelimiter;

            int index = 0;
            int i = -1;
            int rnCount = 0;

            IGlyph previous = Factory.SystemFont[(char)0];
            foreach (var current in glyphs.OfType<IExGlyph>())
            {
                ++i;
                current.X = 0;
                current.Y = 0;
                w += current.Width;
                bool IsSpace = current.Character == ' ';
                bool IsCRLF = current.Character == '\r' || current.Character == '\n';
                if (maxWidth != 0 && newX + current.Width > maxWidth)
                {
                    lines.Add(new GlyphLineInfo(ref index, i, dstX, dstY,
                        ref w, ref h, ref newX, ref newY, ref rnCount, lh));
                    goto mks;
                }

                if (IsSpace)
                {
                    if (Word || SingleWord)
                    {
                        if (WordDelimiter || SingleWord)
                        {
                            lines.Add(new GlyphLineInfo(ref index, i, dstX, dstY,
                                ref w, ref h, ref newX, ref newY, ref rnCount, lh));
                            begin = true;
                        }
                    }
                    else if (Line || commands == 0)
                    {
                        if (!begin)
                            newX += current.Width;
                        goto mks;

                    }
                }
                if (SingleChar)
                {
                    lines.Add(new GlyphLineInfo(ref index, i, dstX, dstY,
                        ref w, ref h, ref newX, ref newY, ref rnCount, lh));
                }
                else if (IsCRLF)
                {
                    w += spaceWidth;
                    ++rnCount;
                    goto mks;
                }
                else if (!IsCRLF && (previous.Character == '\r' || previous.Character == '\n'))
                {
                    w -= current.Width;
                    lines.Add(new GlyphLineInfo(ref index, i, dstX, dstY,
                        ref w, ref h, ref newX, ref newY, ref rnCount, lh));
                    w = current.Width;
                }
                //if (Kerning && i != 0)
                //    kerning = GetKerning(this, current, previous);

                mks:
                currentX = newX + kerning;
                currentY = dstY;

                if (newX < x)
                    x = newX;

                if (newY < y)
                    y = newY;

                newX += current.Width;
                if (newX > r)
                    r = newX;

                if (h < current.Height)
                    h = current.Height;

                if (current.Slot.Min.Y < 0)
                    h -= current.Slot.Min.Y;

                if (newY + h > b)
                    b = newY + h;

                current.X = currentX;
                current.Y = currentY;
                begin = false;
                previous = current;
            }
            if (lines.Count > 0)
            {
                lines.Add(new GlyphLineInfo(ref index, i, dstX, dstY,
                    ref w, ref h, ref newX, ref newY, ref rnCount, lh));

            }
            area = Rectangle.FromLTRB(x, y, r, b);

            glyphCount = i;
            return lines;
        }
        #endregion

        #region STROKING
        /// <summary>
        /// Returns inner and outer perimeter resulted due to stroking of specified points.
        /// </summary>
        /// <param name="Points">Points to stroke.</param>
        /// <param name="shapeType">Type of shape the points belong to.</param>
        /// <param name="stroke">Amount of stroke.</param>
        /// <param name="Command">Stroke mode to apply.</param>
        /// <param name="outerP">Outer perimeter returned.</param>
        /// <param name="innerP">Inner perimeter returned.</param>
        /// <param name="Swapped">Returns true if outer and inner are interchanged.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void StrokePoints(this IEnumerable<VectorF> Points, string shapeType, float stroke, FillCommand Command,
            out IEnumerable<VectorF> outerP, out IEnumerable<VectorF> innerP, out bool Swapped)
        {
            Swapped = false;
            var afterStroke = Factory.ShapeParser.GetAfterStroke(shapeType);
            var join = Factory.ShapeParser.GetStrokeJoin(shapeType);
            var points = Points.Clean(join);

            bool reset1st = (afterStroke & AfterStroke.Reset1st) == AfterStroke.Reset1st;
            bool StrokeInner = (Command & FillCommand.StrokeInner) == FillCommand.StrokeInner;
            bool StrokeOuter = (Command & FillCommand.StrokeOuter) == FillCommand.StrokeOuter && !StrokeInner;

            if (StrokeOuter)
            {
                outerP = points.StrokePoints(stroke, join, reset1st);
                innerP = points;
                var i = innerP.ToArea();
                var o = outerP.ToArea();
                if (!o.Contains(i))
                {
                    outerP = points.StrokePoints(-stroke, join, reset1st);
                    Swapped = true;
                }
            }
            else if (StrokeInner)
            {
                innerP = points.StrokePoints(-stroke, join, reset1st);
                outerP = points;
                var i = innerP.ToArea();
                var o = outerP.ToArea();
                if (!o.Contains(i))
                {
                    innerP = points.StrokePoints(stroke, join, reset1st);
                    Swapped = true;
                }
            }
            else
            {
                var half = (stroke / 2f);
                outerP = points.StrokePoints(half, join, reset1st);
                innerP = points.StrokePoints(-half, join, reset1st);
                var i = innerP.ToArea();
                var o = outerP.ToArea();
                if (!o.Contains(i))
                {
                    Numbers.Swap(ref outerP, ref innerP);
                    Swapped = true;
                }
            }
        }
        #endregion

        #region GET LINES
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void GetLines(string ShapeName, IEnumerable<VectorF> Outer, IEnumerable<VectorF> Inner,
            float stroke, out IList<ILine> outerLines, out IList<ILine> innerLines,
            bool dontCloseEnds = false)
        {
            var join = PointJoin.ConnectEach | Factory.ShapeParser.GetStrokeJoin(ShapeName);

            if (stroke == 0)
            {
                outerLines = Outer.ToLines(join);
                innerLines = null;
                return;
            }
            bool close = (Factory.ShapeParser.GetAfterStroke(ShapeName) & AfterStroke.JoinEnds) == AfterStroke.JoinEnds;

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
        }
        #endregion

        #region GET DRAW PARAMS
        /// <summary>
        /// Gets array of collection of lines accroding to crrent stroke settings for the given original points.
        /// </summary>
        /// <param name="fillMode"></param>
        /// <param name="Stroke"></param>
        /// <param name="ShapeName"></param>
        /// <param name="Original"></param>
        /// <param name="Outer"></param>
        /// <param name="Inner"></param>
        /// <returns></returns>
        static IList<ILine>[] GetDrawParams(Command Command, float Stroke, string ShapeName,
            IEnumerable<VectorF> Original, IEnumerable<VectorF> Outer, IEnumerable<VectorF> Inner)
        {
            IList<ILine>[] Data = new IList<ILine>[4];
            bool OriginalFill = (Command & Command.OriginalFill) == Command.OriginalFill;

            float stroke = OriginalFill ? 0 : Stroke;

            IList<ILine> outer, inner, outer1, inner1 = null;
            bool StrokeInner = (Command & Command.StrokeInner) == Command.StrokeInner;
            bool StrokeOuter = (Command & Command.StrokeOuter) == Command.StrokeOuter && !StrokeInner;

            var avoidClosingEnds = StrokeOuter || StrokeInner;
            avoidClosingEnds = avoidClosingEnds && (ShapeName == "Bezier" || ShapeName == "Arc");
            if (stroke == 0)
            {
                var join = PointJoin.ConnectEach | Factory.ShapeParser.GetStrokeJoin(ShapeName);

                outer = Original.ToLines(join);
                inner = null;
                outer1 = outer;
            }
            else
            {
                GetLines(ShapeName, Outer, Inner, stroke, out outer, out inner, avoidClosingEnds);
                outer1 = outer;
                inner1 = inner;
            }

            if (Stroke == 0 && StrokeInner)
                goto Original;
            bool DrawOutLine = (Command & Command.DrawOutLines) == Command.DrawOutLines;
            bool Odd = (Command & Command.XORFill) == Command.XORFill;
            bool ExceptOutLine = !Odd && (Command & Command.FillOddLines) == Command.FillOddLines;

            bool FillOutLine = !ExceptOutLine && !DrawOutLine;

            if (FillOutLine)
            {
                Data[0] = (outer1 ?? outer);
                Data[2] = (inner1 ?? inner);
                if (ShapeName == "Bezier" && stroke == 0)
                    return Data;
                Data[1] = outer;
                Data[3] = inner;
                return Data;
            }

            if (DrawOutLine)
            {
                Data[0] = (outer1 ?? outer);
                Data[2] = (inner1 ?? inner);
                return Data;
            }

            if (ExceptOutLine)
            {
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
                return Data;
            }

            if (StrokeInner)
            {
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
                return Data;
            }

            Original:
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
            return Data;
        }
        #endregion

        #region TO AREA
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle ToArea(this IObject shape)
        {
            ToArea(shape, out int x, out int y, out int w, out int h);
            return new Rectangle(x, y, w, h);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ToArea(this IObject shape, out int x, out int y, out int w, out int h)
        {
            x = 0; y = 0; w = 0; h = 0;

            if (!shape.Valid)
                return;

            if (shape is IBoundsF)
            {
                ((IBoundsF)shape).GetBounds(out float fx, out float fy, out float fw, out float fh);

                x = (int)fx;
                y = (int)fy;
                w = (int)fw;
                h = (int)fh;

                if (fw - w != 0)
                    ++w;
                if (fh - h != 0)
                    ++h;
                return;
            }
            if (shape is IBounds)
            {
                ((IBounds)shape).GetBounds(out x, out y, out w, out h);
                return;
            }

            x = shape.X;
            y = shape.Y;
            w = shape.Width;
            h = shape.Height;
        }
        #endregion

        #region TO PEN
        public static IPen ToPen(this IPenContext context,
            int? w = null, int? h = null, ResizeCommand resizeMode = 0)
        {
            return Factory.factory.ToPen(context, w, h, resizeMode);
        }
        #endregion

        #region POLY FILL
        /// <summary>
        /// Create new instance of object implementing IPolyFill interface for rendering purposes.
        /// </summary>
        /// <param name="graphics">Graphics object to render to.</param>
        /// <param name="parameters">Session object to control rendering process.</param>
        /// <returns>Instance of IPolyFill object.</returns>
        public static IPolygonFiller newPolyFill(this IRenderer graphics, IEnumerable<IParameter> parameters)
        {
            return newPolyFill(graphics, parameters);
        }

        /// <summary>
        /// Create new instance of object implementing IPolyFill interface for rendering purposes.
        /// </summary>
        /// <param name="graphics">Graphics object to render to.</param>
        /// <param name="parameters">Session object to control rendering process.</param>
        /// <returns>Instance of IPolyFill object.</returns>
        public static IPolygonFiller newPolyFill(this IRenderer graphics,
            IEnumerable<IParameter> parameters, FillCommand polyFillCommand)
        {
            var polyFill = Factory.factory.newPolyFill
             (
                 graphics.CreateRenderAction(parameters)
             );

            polyFill.Mode = polyFillCommand;
            return polyFill;
        }

        /// <summary>
        /// Create new instance of object implementing IPolyFill interface for rendering purposes.
        /// </summary>
        /// <param name="graphics">Graphics object to render to.</param>
        /// <param name="parameters">Session object to control rendering process.</param>
        /// <returns>Instance of IPolyFill object.</returns>
        internal static IPolygonFiller newPolyFill(this IRenderer graphics,
            IEnumerable<IParameter> parameters, Command polyFillCommand)
        {
            var polyFill = Factory.factory.newPolyFill
             (
                 graphics.CreateRenderAction(parameters)
             );

            polyFill.Mode = polyFillCommand.ToEnum<FillCommand>();
            return polyFill;
        }
        #endregion

        #region CLEAR
        /// <summary>
        /// Clears screen using parameters supplied.
        /// </summary>
        /// <returns>No of pixels cleared.</returns>
        public static void Clear(this IClearable @object, params IParameter[] parameters)
        {
            if (@object == null || parameters == null)
                return;
            @object.Clear(parameters);
        }
        #endregion

        #region ROTATE AND SCALE
        /// <summary>
        /// Returns a rotated and scalled memory block of this object alongwith size of it.
        /// </summary>
        /// <param name="parameters">Collection of parameters to affect and control rotation and scaling operation.</param>
        /// <param name="Data">Resultant rotated and scalled data.</param>
        /// <returns>Rotated and scalled copy of this object.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISize RotateAndScale(this IRotatableSource source,
            out IntPtr Data, params IParameter[] parameters)
        {
            var r = source.RotateAndScale(parameters);
            Data = r.Item2;
            return r.Item1;
        }

        /// <summary>
        /// Returns a rotated and scalled copy of this object.
        /// </summary>
        /// <param name="graphics">Memory block to rotate and scale.</param>
        /// <param name="angle">Angle of rotation to apply.</param>
        /// <param name="antiAliased">If true copy is antialised version of this object otherwise not.</param>
        /// <param name="scale">Scale to apply.</param>
        /// <returns>Rotated and scalled copy of this object.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ICanvas RotateAndScale(this IRotatableSource graphics, params IParameter[] parameters)
        {
            var sz = graphics.RotateAndScale(out IntPtr data, parameters);
            return Factory.newCanvas(data, sz.Width, sz.Height);
        }
        #endregion

        #region TAKE SCREEN SHOT
        /// <summary>
        /// Takes a screen shot of this object to a surface and saves that surface to the specified path
        /// as an image as per desired image format.
        /// </summary>
        /// <param name="screen">An object capable of rendering its mirror image on other surface.</param>
        /// <param name="parameters">Parameters to influence and control the operation of taking screen-shot.. 
        /// Expected parameters are:
        /// 1. ScreenShotType
        /// 2. Clip to crop the result image. Clip location must be in relation to location of DrawnBounds
        /// of this object.
        /// </param>        
        /// <param name="path"></param>
        public static void TakeScreenShot(this ICopyableObject screen, IEnumerable<IParameter> parameters,
            string path)
        {
            var surface = screen.TakeScreenShot(parameters);
            surface.SaveAs(parameters.AppendItems
                (ImageFormat.PNG.ToParameter(), Command.SwapRedBlueChannel.Add()), path);
        }

        /// <summary>
        /// Takes a screen shot of this object to a surface and saves that surface to the specified path
        /// as an image as per desired image format.
        /// </summary>
        /// <param name="screen">An object capable of rendering its mirror image on other surface.</param>
        /// <param name="parameters">Parameters to influence and control the operation of taking screen-shot.. 
        /// Expected parameters are:
        /// 1. ScreenShotType
        /// 2. Clip to crop the result image. Clip location must be in relation to location of DrawnBounds
        /// of this object.
        /// </param>        
        /// <param name="path"></param>
        public static void TakeScreenShot(this ICopyableObject screen,
            string path, params IParameter[] parameters)
        {
            var surface = screen.TakeScreenShot(parameters);
            surface.SaveAs(parameters.AppendItems
                (ImageFormat.PNG.ToParameter(), Command.SwapRedBlueChannel.Add()), path);
        }

        /// <summary>
        /// Takes a screen shot of this window to a given surface.
        /// </summary>
        /// <param name="screen">An object capable of rendering its mirror image on other surface.</param>
        /// <param name="parameters">Parameters to influence and control the operation of taking screen-shot.. 
        /// Expected parameters are:
        /// 1. ScreenShotType
        /// 2. Clip to crop the result image. Clip location must be in relation to location of DrawnBounds
        /// of this object.
        /// </param>        
        /// <returns>Instance of ISurface object which will have resultant screen-shot image.</returns>
        public static ICanvas TakeScreenShot(this ICopyableObject screen,
            IEnumerable<IParameter> parameters = null)
        {
            var Surface = Factory.newCanvas(screen.Width + 1, screen.Height + 1);
            screen.TakeScreenShot(Surface, parameters);
            return Surface;
        }
        #endregion
    }
    partial class Drawing
    {
        #region DRAW SHAPE
        /// <summary>
        /// Represents an object which can be drawn drawn directly on its host surface.
        /// In order to be drawn automatically, the Shape must inherit either IDraw or IPolygonalF interface
        /// or must derive from either ObjItem<T> or Widget<T>.
        /// If none of the above is the case then,
        /// You must override IFactory.HandleRenderingOfUnknownObject(Shape, renderer, parameters) method
        /// and provide custom rendering routine for your Shape,
        /// </summary>
        /// <param name="graphics">Buffer which shape is to be drawn on.</param>
        /// <param name="Shape">Shape to be drawn..</param>
        /// <param name="parameters">Collection of parameters to influence or control this drawing process.</param>
        /// <returns>True if operation is handled within this method otherwise returns false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Draw(this IRenderer renderer, IObject Shape, IEnumerable<IParameter> parameters) =>
          RenderWithUpdate((IExRenderer)renderer, Shape, parameters);

        /// <summary>
        /// Represents an object which can be drawn drawn directly on its host surface.
        /// In order to be drawn automatically, the Shape must inherit either IDraw or IPolygonalF interface
        /// or must derive from either ObjItem<T> or Widget<T>.
        /// If none of the above is the case then,
        /// You must override IFactory.HandleRenderingOfUnknownObject(Shape, renderer, parameters) method
        /// and provide custom rendering routine for your Shape,
        /// </summary>
        /// <param name="graphics">Buffer which shape is to be drawn on.</param>
        /// <param name="Shape">Shape to be drawn..</param>
        /// <param name="parameters">Collection of parameters to influence or control this drawing process.</param>
        /// <returns>True if operation is handled within this method otherwise returns false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Draw(this IRenderer renderer, IObject Shape, params IParameter[] parameters) =>
           RenderWithUpdate((IExRenderer)renderer, Shape, parameters);

        /// <summary>
        /// Represents an object which can be drawn drawn directly on its host surface.
        /// In order to be drawn automatically, the Shape must inherit either IDraw or IPolygonalF interface
        /// or must derive from either ObjItem<T> or Widget<T>.
        /// If none of the above is the case then,
        /// You must override IFactory.HandleRenderingOfUnknownObject(Shape, renderer, parameters) method
        /// and provide custom rendering routine for your Shape,
        /// </summary>
        /// <param name="graphics">Buffer which shape is to be drawn on.</param>
        /// <param name="Shape">Shape to be drawn..</param>
        /// <returns>True if operation is handled within this method otherwise returns false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Draw(this IRenderer renderer, IObject Shape) =>
           RenderWithUpdate((IExRenderer)renderer, Shape, null);
        #endregion

        #region DRAW SHAPES
        /// <summary>
        /// Renders multiple elements on this object. 
        /// In order to be drawn automatically, the Shape must inherit either IDraw or IPolygonalF interface
        /// or must derive from either ObjItem<T> or Widget<T>.
        /// If none of the above is the case then,
        /// You must override IFactory.HandleRenderingOfUnknownObject(Shape, renderer, parameters) method
        /// and provide custom rendering routine for your Shape,
        /// </summary>
        /// <param name="Shapes">Array of renderable elements.</param>
        /// <param name="parameters">Array of Settings associated with respective element in the array of renderables.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawShapes(this IRenderer renderer,
            IEnumerable<IParameter> parameters, IEnumerable<IObject> Shapes)
        {
            parameters.Extract(out IExSession session);
            var Boundary = Factory.newBoundary(session.Type);
            Boundary.Accumulative = true;
            IEnumerable<IParameter> Parameters = parameters.AppendItems(Boundary);
            var graphics = (IExRenderer)renderer;
            foreach (var Renderable in Shapes)
            {
                graphics.Render(Renderable, Parameters);
            }
            if (Boundary.Valid && (session.Command & Command.SkipDisplayUpdate) != Command.SkipDisplayUpdate && graphics is IUpdatable)
            {
                ((IUpdatable)graphics).Update(Boundary, session.Command.ToEnum<UpdateCommand>() | UpdateCommand.UpdateScreenOnly);
            }
        }

        /// <summary>
        /// Renders multiple elements on this object. 
        /// In order to be drawn automatically, the Shape must inherit either IDraw or IPolygonalF interface
        /// or must derive from either ObjItem<T> or Widget<T>.
        /// If none of the above is the case then,
        /// You must override IFactory.HandleRenderingOfUnknownObject(Shape, renderer, parameters) method
        /// and provide custom rendering routine for your Shape,
        /// </summary>
        /// <param name="Shapes">Array of renderable elements.</param>
        /// <param name="parameters">Array of Settings associated with respective element in the array of renderables.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Draw(this IRenderer graphics, IEnumerable<IObject> Shapes,
            params IParameter[] parameters) =>
           DrawShapes(graphics, parameters, Shapes);
        #endregion

        #region WRITE PIXEL/S
        /// <summary>
        /// Writes pixel to the this block at given co-ordinates of location using specified colour.
        /// </summary>
        /// <param name="x">X co-ordinate on 2d graphics memory block</param>
        /// <param name="y">Y co-ordinate on 2d graphics memory block</param>
        /// <param name="colour">colour of pixel.</param>
        /// <param name="Session">Session object to control rendering operation.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WritePixel(this IRenderer graphics, float x, float y, params IParameter[] parameters)
        {
            var items = new IScanPoint[] { new VectorF(x, y, PointKind.Break) };

            var action = graphics.CreateRenderAction(parameters);
            action(null, items, null);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WritePixels(this IRenderer graphics, float[] co_ordinates, params IParameter[] parameters)
        {
            var items = co_ordinates.ToVectorF(PointKind.Break).OfType<IScanPoint>();

            var action = graphics.CreateRenderAction(parameters);
            action(null, items, null);
        }


        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WritePixels(this IRenderer graphics, int[] co_ordinates, params IParameter[] parameters)
        {
            var items = co_ordinates.ToVectorF(PointKind.Break).OfType<IScanPoint>();

            var action = graphics.CreateRenderAction(parameters);
            action(null, items, null);
        }
        #endregion

        #region WRITE LINES
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLines(this IRenderer graphics, float[] co_ordinates, params IParameter[] parameters)
        {
            var items = co_ordinates.ToVectorF().OfType<IScanPoint>();

            var action = graphics.CreateRenderAction(parameters);
            action(null, items, null);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteLines(this IRenderer graphics, int[] co_ordinates, params IParameter[] parameters)
        {
            var items = co_ordinates.ToVectorF().OfType<IScanPoint>();

            var action = graphics.CreateRenderAction(parameters);
            action(null, items, null);
        }
        #endregion

        #region DRAW BORDER
        public static void DrawBorder(this IRenderer graphics, IBounds perimeter, LineCommand lineCommand = 0, int shrink = 0, UpdateCommand updateCommand = 0)
        {
            if (perimeter == null)
                return;

            IPenContext pen;

            if (graphics is IPen)
                pen = (IPen)graphics;
            else if (graphics is IBackgroundContextSetter)
                pen = ((IBackgroundContextSetter)graphics).BackgroundPen ?? Rgba.Transparent;
            else
                pen = Rgba.Black;

            Command command = 0;
            command = command.UpdateWith(lineCommand, ModifyCommand.Add).UpdateWith(updateCommand, ModifyCommand.Add);
            command |= Command.InvertColour;
            var boundary = Factory.newBoundary();
            var parameters = new IParameter[] { pen, command.Replace(), boundary };
            perimeter.GetBounds(out int x1, out int y1, out int w, out int h);
            int x2 = x1 + w;
            int y2 = y1 + h;

            if (shrink != 0)
            {
                x1 += shrink;
                y1 += shrink;
                x2 -= shrink;
                y2 -= shrink;
            }
            var action = graphics.CreateRenderAction(parameters);
            action(null, new ILine[]
            {
                new Line(x1, y1, x1, y2),
                new Line(x1, y2, x2, y2),
                new Line(x2, y2, x2, y1),
                new Line(x2, y1, x1, y1),
            }, null
            );
            if (graphics is IUpdatable)
                ((IUpdatable)graphics).Update(boundary, command.ToEnum<UpdateCommand>());
        }
        #endregion

        #region DRAW LINE
        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="parameters">Various settings parameters to influence the rendering process.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine(this IRenderer renderer, float x1, float y1, float x2, float y2,
            IEnumerable<IParameter> parameters) =>
            ((IExRenderer)renderer).RenderWithUpdate(new Line(x1, y1, x2, y2), parameters);

        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="parameters">Various settings parameters to influence the rendering process.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine(this IRenderer renderer, float x1, float y1, float x2, float y2,
            params IParameter[] parameters) =>
            ((IExRenderer)renderer).RenderWithUpdate(new Line(x1, y1, x2, y2), parameters);

        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by x1, y1 and x2, y2.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine(this IRenderer renderer, float x1, float y1, float x2, float y2) =>
            ((IExRenderer)renderer).RenderWithUpdate(new Line(x1, y1, x2, y2), null);

        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by points p1 & p2.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="line">Line to draw</param>
        /// <param name="parameters">Various settings parameters to influence the rendering process.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine(this IRenderer renderer, ILine line, IEnumerable<IParameter> parameters) =>
            ((IExRenderer)renderer).RenderWithUpdate(line, parameters);

        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by points p1 & p2.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="line">Line to draw</param>
        /// <param name="parameters">Various settings parameters to influence the rendering process.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawLine(this IRenderer renderer, ILine line, params IParameter[] parameters) =>
            ((IExRenderer)renderer).RenderWithUpdate(line, parameters);

        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by points p1 & p2.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="line">Line to draw</param>
        public static void DrawLine(this IRenderer renderer, ILine line) =>
            ((IExRenderer)renderer).RenderWithUpdate(line, null);

        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by points p1 & p2.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="p1">Start point of line segment</param>
        /// <param name="p2">end point of line segment</param>
        /// <param name="parameters">Various settings parameters to influence the rendering process.</param>
        public static void DrawLine(this IRenderer renderer, VectorF p1, VectorF p2,
            params IParameter[] parameters) =>
            ((IExRenderer)renderer).RenderWithUpdate(new Line(p1, p2), parameters);

        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by points p1 & p2.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="p1">Start point of line segment</param>
        /// <param name="p2">end point of line segment</param>
        /// <param name="parameters">Various settings parameters to influence the rendering process.</param>
        public static void DrawLine(this IRenderer renderer, Vector p1, Vector p2, params IParameter[] parameters) =>
            ((IExRenderer)renderer).RenderWithUpdate(new Line(p1, p2), parameters);

        /// <summary>
        /// Renders a line segment using standard line algorithm between two points specified by points p1 & x2, y2.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="p1">Start point of line segment</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="parameters">Various settings parameters to influence the rendering process.</param>
        public static void DrawLine(this IRenderer renderer, VectorF p1, float x2, float y2,
            params IParameter[] parameters) =>
            ((IExRenderer)renderer).RenderWithUpdate(new Line(p1.X, p1.Y, x2, y2), parameters);
        #endregion

        #region DRAW LINES
        /// <summary>
        /// Renders a collection of line segments using standard line algorithm.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="lines">A collection of lines</param>
        /// <param name="parameters">Various settings parameters to influence the rendering process.</param>
        public static void DrawLines(this IRenderer renderer, IEnumerable<ILine> lines,
            params IParameter[] parameters)
        {
            if (lines == null)
                return;
            var graphics = ((IExRenderer)renderer);
            foreach (var l in lines)
                graphics.RenderWithUpdate(l, parameters);
        }

        /// <summary>
        /// Renders a collection of line segments using standard line algorithm.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="lines">A collection of lines</param>
        /// <param name="parameters">Various settings parameters to influence the rendering process.</param>
        public static void DrawLines(this IRenderer renderer, IEnumerable<ILine> lines,
            IEnumerable<IParameter> parameters)
        {
            if (lines == null)
                return;
            var graphics = ((IExRenderer)renderer);
            foreach (var l in lines)
              graphics.RenderWithUpdate(l, parameters);
        }

        /// <summary>
        /// Renders a collection of line segments using standard line algorithm.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="points">A collection of points to create lines from.</param>
        /// <param name="parameters">Various settings parameters to influence the rendering process.</param>
        public static void DrawLines(this IRenderer renderer, IEnumerable<VectorF> points,
           IEnumerable<IParameter> parameters, bool connectEach = true)
        {
            var lines = points.ToLines(connectEach ? PointJoin.ConnectEach | PointJoin.ConnectEnds : 0);
            if (lines == null || lines.Count == 0)
                return;
            var graphics = ((IExRenderer)renderer);
            foreach (var l in lines)
                graphics.RenderWithUpdate(l, parameters);
        }

        /// <summary>
        /// Renders a collection of line segments using standard line algorithm.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="lines">A collection of lines</param>
        /// <param name="parameters">Various settings parameters to influence the rendering process.</param>
        public static void DrawLines(this IRenderer renderer,
            IEnumerable<IParameter> parameters, params ILine[] lines)
        {
            if (lines == null)
                return;
            var graphics = ((IExRenderer)renderer);
            foreach (var l in lines)
                graphics.RenderWithUpdate(l, parameters);
        }

        /// <summary>
        /// Renders a collection of line segments using standard line algorithm.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="lines">A collection of lines</param>
        public static void DrawLines(this IRenderer renderer, params ILine[] lines)
        {
            if (lines == null)
                return;
            var graphics = ((IExRenderer)renderer);
            foreach (var l in lines)
                graphics.RenderWithUpdate(l, null);
        }

        /// <summary>
        /// Renders line segments by first creating them from an array of integer values specified using standard line algorithm.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="parameters">Various settings parameters to influence the rendering process.</param>
        /// <param name="connectEach">If true then each line segment will be connected to the previous and next one</param>
        /// <param name="values">An interger array of values.Each subsequent four elements get converted to a line segment
        /// For example if values are int[]{23, 56, 98, 205} creates Line(X1 = 23, Y1 = 56, X2 = 98,Y2 = 205) </param>
        public static void DrawLines(this IRenderer renderer,
            IEnumerable<IParameter> parameters, bool connectEach, params int[] values)
        {
            var lines = (values).ToVectorF().ToLines(connectEach ? 
                PointJoin.ConnectEach | PointJoin.ConnectEnds : 0);
            if (lines == null || lines.Count == 0)
                return;
            var graphics = ((IExRenderer)renderer);

            foreach (var l in lines)
                graphics.RenderWithUpdate(l, parameters);
        }

        /// <summary>
        /// Renders line segments by first creating them from an array of float values specified using standard line algorithm.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="parameters">Various settings parameters to influence the rendering process.</param>
        /// <param name="connectEach">If true then each line segment will be connected to the previous and next one</param>
        /// <param name="values">A float array of values.Each subsequent four elements get converted to a line segment
        /// For example if values are int[]{23.33f, 56.67f, 98.45f, 205.21f} creates Line(X1 = 23.33f, Y1 = 56.67f, X2 = 98.45f,Y2 = 205.21f) </param>
        public static void DrawLines(this IRenderer renderer,
            IEnumerable<IParameter> parameters, bool connectEach, params float[] values)
        {
            var lines = (values).ToVectorF().ToLines(connectEach ? PointJoin.ConnectEach | PointJoin.ConnectEnds : 0);
            if (lines == null || lines.Count == 0)
                return;
            var graphics = ((IExRenderer)renderer);
            foreach (var l in lines)
                graphics.RenderWithUpdate(l, parameters);
        }

        /// <summary>
        /// Renders line segments by first creating them from an array of integer values specified using standard line algorithm.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="connectEach">If true then each line segment will be connected to the previous and next one</param>
        /// <param name="angle">Angle to apply rotation on x1, y1, x2, y2 of each line segment before rendering the line segment</param>
        /// <param name="values">An interger array of values.Each subsequent four elements get converted to a line segment
        /// For example if values are int[]{23, 56, 98, 205} creates Line(X1 = 23, Y1 = 56, X2 = 98,Y2 = 205) </param>
        public static void DrawLines(this IRenderer renderer, bool connectEach, params int[] values)
        {
            var lines = (values).ToVectorF().ToLines(connectEach ?
                PointJoin.ConnectEach | PointJoin.ConnectEnds : 0);
            if (lines == null || lines.Count == 0)
                return;
            var graphics = ((IExRenderer)renderer);
            foreach (var l in lines)
                graphics.RenderWithUpdate(l, null);
        }

        /// <summary>
        /// Renders line segments by first creating them from an array of float values specified using standard line algorithm.
        /// </summary>
        /// <param name="graphics">graphics which to render a line on</param>
        /// <param name="connectEach">If true then each line segment will be connected to the previous and next one</param>
        /// <param name="values">A float array of values.Each subsequent four elements get converted to a line segment
        /// For example if values are int[]{23.33f, 56.67f, 98.45f, 205.21f} creates Line(X1 = 23.33f, Y1 = 56.67f, X2 = 98.45f,Y2 = 205.21f) </param>
        public static void DrawLines(this IRenderer renderer, bool connectEach, params float[] values)
        {
            var lines = (values).ToVectorF().ToLines(connectEach ?
                PointJoin.ConnectEach | PointJoin.ConnectEnds : 0);
            if (lines == null || lines.Count == 0)
                return;
            var graphics = ((IExRenderer)renderer);
            foreach (var l in lines)
                graphics.RenderWithUpdate(l, null);
        }
        #endregion

        #region DRAW CIRCLE
        /// <summary>
        /// Draws a circle specified by the bounding area and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a circle/ellipse on</param>
        /// <param name="x">X cordinate of a bounding area where the circle is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle is to be drawn</param>
        /// <param name="width">Width of a bounding area where the circle is to be drawn -> circle's minor X axis = Width/2</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCircle(this IRenderer graphics, float x, float y, float width,
            params IParameter[] parameters) =>
           RenderCircleOrEllipse(graphics, x, y, width, width, parameters);

        /// <summary>
        /// Draws a circle specified by the bounding area and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a circle/ellipse on</param>
        /// <param name="x">X cordinate of a bounding area where the circle is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle is to be drawn</param>
        /// <param name="width">Width of a bounding area where the circle is to be drawn -> circle's minor X axis = Width/2</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCircle(this IRenderer graphics, float x, float y, float width) =>
           RenderCircleOrEllipse(graphics, x, y, width, width);

        /// <summary>
        /// Draws a circle specified by the center point and another point which provides a start location and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a circle/ellipse on</param>
        /// <param name="pointOnCircle">A point on a circle which you want</param>
        /// <param name="centerOfCircle">Center of a circle</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCircle(this IRenderer graphics, VectorF pointOnCircle, VectorF centerOfCircle,
            params IParameter[] parameters)
        {
            DrawCircle(graphics, pointOnCircle, centerOfCircle, (IEnumerable<IParameter>)parameters);
        }

        /// <summary>
        /// Draws a circle specified by the center point and another point which provides a start location and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a circle/ellipse on</param>
        /// <param name="pointOnCircle">A point on a circle which you want</param>
        /// <param name="centerOfCircle">Center of a circle</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCircle(this IRenderer graphics, VectorF pointOnCircle, VectorF centerOfCircle,
            IEnumerable<IParameter> parameters)
        {
            Curves.GetCircleData(pointOnCircle, centerOfCircle, out float x, out float y, out float w);
            RenderCircleOrEllipse(graphics, x, y, w, w, parameters);
        }

        /// <summary>
        /// Draws a circle specified by the center point and another point which provides a start location and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a circle/ellipse on</param>
        /// <param name="pointOnCircle">A point on a circle which you want</param>
        /// <param name="centerOfCircle">Center of a circle</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCircle(this IRenderer graphics, VectorF pointOnCircle, VectorF centerOfCircle)
        {
            Curves.GetCircleData(pointOnCircle, centerOfCircle, out float x, out float y, out float w);
            RenderCircleOrEllipse(graphics, x, y, w, w);
        }
        #endregion

        #region DRAW ELLIPSE
        /// <summary>
        /// Draws an ellipse specified by the bounding area and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a ellipse on</param>
        /// <param name="x">X cordinate of a bounding area where the ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the ellipse is to be drawn</param>
        /// <param name="width">Width of a bounding area where the ellipse is to be drawn -> ellipse's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the ellipse is to be drawn -> ellipse's minor Y axis = Height/2</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawEllipse(this IRenderer graphics, float x, float y, float width, float height,
            params IParameter[] parameters) =>
           RenderCircleOrEllipse(graphics, x, y, width, height, parameters);

        /// <summary>
        /// Draws an ellipse specified by the bounding area and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a ellipse on</param>
        /// <param name="x">X cordinate of a bounding area where the ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the ellipse is to be drawn</param>
        /// <param name="width">Width of a bounding area where the ellipse is to be drawn -> ellipse's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the ellipse is to be drawn -> ellipse's minor Y axis = Height/2</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawEllipse(this IRenderer graphics, IEnumerable<IParameter> parameters,
            float x, float y, float width, float height) =>
           RenderCircleOrEllipse(graphics, x, y, width, height, parameters);

        /// <summary>
        /// Draws an ellipse specified by the bounding area and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a ellipse on</param>
        /// <param name="x">X cordinate of a bounding area where the ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the ellipse is to be drawn</param>
        /// <param name="width">Width of a bounding area where the ellipse is to be drawn -> ellipse's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the ellipse is to be drawn -> ellipse's minor Y axis = Height/2</param>
        public static void DrawEllipse(this IRenderer graphics, float x, float y, float width, float height) =>
           RenderCircleOrEllipse(graphics, x, y, width, height);
        #endregion

        #region DRAW ARC
        /// <summary>
        /// Draws an arc specified by the bounding area and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="graphics">graphics which to render an arc on</param>
        /// <param name="x">X cordinate of a bounding area where the arc is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the arc is to be drawn</param>
        /// <param name="width">Width of a bounding area where the arc is to be drawn -> arc's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the arc is to be drawn ->arc's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. If type includes NoSweepAngle option otherwise effective end angle is start angle + end angle</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered. Such as fitting, third point on ellipse or on center etc.</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawArc(this IRenderer graphics, float x, float y, float width, float height,
            float startAngle, float endAngle, params IParameter[] parameters) =>
           RenderArcOrPie(graphics, x, y, width, height, startAngle, endAngle, parameters, CurveType.Arc);

        /// <summary>
        /// Draws an arc specified by the bounding area and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="graphics">graphics which to render an arc on</param>
        /// <param name="x">X cordinate of a bounding area where the arc is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the arc is to be drawn</param>
        /// <param name="width">Width of a bounding area where the arc is to be drawn -> arc's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the arc is to be drawn ->arc's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. If type includes NoSweepAngle option otherwise effective end angle is start angle + end angle</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered. Such as fitting, third point on ellipse or on center etc.</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawArc(this IRenderer graphics, IEnumerable<IParameter> parameters,
            float x, float y, float width, float height, float startAngle, float endAngle) =>
           RenderArcOrPie(graphics, x, y, width, height, startAngle, endAngle, parameters, CurveType.Arc);

        /// <summary>
        /// Draws an arc specified by the bounding area and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="graphics">graphics which to render an arc on</param>
        /// <param name="x">X cordinate of a bounding area where the arc is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the arc is to be drawn</param>
        /// <param name="width">Width of a bounding area where the arc is to be drawn -> arc's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the arc is to be drawn ->arc's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. If type includes NoSweepAngle option otherwise effective end angle is start angle + end angle</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered. Such as fitting, third point on ellipse or on center etc.</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawArc(this IRenderer graphics, float x, float y, float width, float height,
            float startAngle, float endAngle) =>
           RenderArcOrPie(graphics, x, y, width, height, startAngle, endAngle, null, CurveType.Arc);
        #endregion

        #region DRAW PIE
        /// <summary>
        /// Draws a pie specified by the bounding area and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="graphics">graphics which to render a pie on</param>
        /// <param name="x">X cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="width">Width of a bounding area where the pie is to be drawn -> pie's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the pie is to be drawn ->pie's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. If type includes NoSweepAngle option otherwise effective end angle is start angle + end angle</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered. Such as fitting, third point on ellipse or on center etc.</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawPie(this IRenderer graphics, float x, float y, float width, float height,
            float startAngle, float endAngle, params IParameter[] parameters) =>
           RenderArcOrPie(graphics, x, y, width, height, startAngle, endAngle, parameters, CurveType.Pie);

        /// <summary>
        /// Draws a pie specified by the bounding area and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="graphics">graphics which to render a pie on</param>
        /// <param name="x">X cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="width">Width of a bounding area where the pie is to be drawn -> pie's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the pie is to be drawn ->pie's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. If type includes NoSweepAngle option otherwise effective end angle is start angle + end angle</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered. Such as fitting, third point on ellipse or on center etc.</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawPie(this IRenderer graphics, IEnumerable<IParameter> parameters,
            float x, float y, float width, float height,
            float startAngle, float endAngle) =>
           RenderArcOrPie(graphics, x, y, width, height, startAngle, endAngle, parameters, CurveType.Pie);

        /// <summary>
        /// Draws a pie specified by the bounding area and angle of rotation if supplied using various option supplied throuh CurveType enum.
        /// </summary>
        /// <param name="graphics">graphics which to render a pie on</param>
        /// <param name="x">X cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the pie is to be drawn</param>
        /// <param name="width">Width of a bounding area where the pie is to be drawn -> pie's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the pie is to be drawn ->pie's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. If type includes NoSweepAngle option otherwise effective end angle is start angle + end angle</param>
        /// <param name="type"> Defines the type of an pie along with other supplimentary options on how to draw it</param>
        public static void DrawPie(this IRenderer graphics, float x, float y, float width, float height,
            float startAngle, float endAngle) =>
           RenderArcOrPie(graphics, x, y, width, height, startAngle, endAngle, null, CurveType.Pie);
        #endregion

        #region DRAW CURVE
        /// <summary>
        /// Renders a curve object.
        /// </summary>
        /// <param name="graphics">Buffer which to render a curve on</param>
        /// <param name="Curve">Cureve object to render</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCurve(this IRenderer renderer, ICurve Curve, params IParameter[] parameters) =>
           ((IExRenderer)renderer).RenderWithUpdate(Curve, parameters);

        /// <summary>
        /// Creates a new circle or ellipse or pie or an arc specified by the bounding area, start and end angles and angle of rotation if supplied.
        /// </summary>
        /// <param name="x">X cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="y">Y cordinate of a bounding area where the circle/ellipse is to be drawn</param>
        /// <param name="width">Width of a bounding area where the circle/ellipse is to be drawn -> circle/ellipse's minor X axis = Width/2</param>
        /// <param name="height">Height of a bounding area where the circle is to be drawn ->circle/ellipse's minor Y axis = Height/2</param>
        /// <param name="startAngle">Start angle from where a curve start</param>
        /// <param name="endAngle">End Angle where a curve stops. If type includes NoSweepAngle option otherwise effective end angle is start angle + end angle</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered. Such as fitting, third point on ellipse or on center etc.</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCurve(this IRenderer graphics, float x, float y, float width, float height,
            float startAngle = 0, float endAngle = 0, CurveType type = CurveType.Pie,
            params IParameter[] parameters)
        {
            var curve = new Curve(x, y, width, height, startAngle, endAngle, type);
            DrawCurve(graphics, curve, parameters);
        }

        /// <summary>
        /// Creates a ellipse or pie or arc using the specified 3 points and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered. Such as fitting, third point on ellipse or on center etc.</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCurve(this IRenderer graphics, VectorF p1, VectorF p2, VectorF p3,
            CurveType type = CurveType.Pie, params IParameter[] parameters)
        {
            var curve = new Curve(p1, p2, p3, type);
            DrawCurve(graphics, curve, parameters);
        }

        /// <summary>
        /// Creates a ellipse or pie or arc using the specified 4 points and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Third point</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered. Such as fitting, third point on ellipse or on center etc.</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCurve(this IRenderer graphics, VectorF p1, VectorF p2, VectorF p3, VectorF p4,
            CurveType type = CurveType.Full, params IParameter[] parameters)
        {
            var curve = new Curve(p1, p2, p3, p4, type);
            DrawCurve(graphics, curve, parameters);
        }

        /// <summary> 
        /// Creates a ellipse or pie or arc using the specified five points conic section and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="p5">Fifth point</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered. Such as fitting, third point on ellipse or on center etc.</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCurve(this IRenderer graphics, VectorF p1, VectorF p2, VectorF p3, VectorF p4, VectorF p5, CurveType type = CurveType.Pie, params IParameter[] parameters)
        {
            var curve = new Curve(p1, p2, p3, p4, type);
            DrawCurve(graphics, curve, parameters);
        }

        /// <summary>
        /// Renders a curve object.
        /// </summary>
        /// <param name="graphics">Buffer which to render a curve on</param>
        /// <param name="Curve">Cureve object to render</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCurve(this IRenderer renderer, IEnumerable<IParameter> parameters, ICurve Curve)
        {
            ((IExRenderer)renderer).RenderWithUpdate(Curve, parameters);
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
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered. Such as fitting, third point on ellipse or on center etc.</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCurve(this IRenderer graphics, IEnumerable<IParameter> parameters,
            float x, float y, float width, float height,
            float startAngle = 0, float endAngle = 0, CurveType type = CurveType.Pie)
        {
            var curve = new Curve(x, y, width, height, startAngle, endAngle, type);
            DrawCurve(graphics, parameters, curve);
        }

        /// <summary>
        /// Creates a ellipse or pie or arc using the specified 3 points and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered. Such as fitting, third point on ellipse or on center etc.</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCurve(this IRenderer graphics, IEnumerable<IParameter> parameters,
            VectorF p1, VectorF p2, VectorF p3, CurveType type = CurveType.Pie)
        {
            var curve = new Curve(p1, p2, p3, type);
            DrawCurve(graphics, parameters, curve);
        }

        /// <summary>
        /// Creates a ellipse or pie or arc using the specified 4 points and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Third point</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered. Such as fitting, third point on ellipse or on center etc.</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCurve(this IRenderer graphics, IEnumerable<IParameter> parameters,
            VectorF p1, VectorF p2, VectorF p3, VectorF p4, CurveType type = CurveType.Full)
        {
            var curve = new Curve(p1, p2, p3, p4, type);
            DrawCurve(graphics, parameters, curve);
        }

        /// <summary> 
        /// Creates a ellipse or pie or arc using the specified five points conic section and angle of rotation if supplied.
        /// </summary>
        /// <param name="p1">First point</param>
        /// <param name="p2">Second point</param>
        /// <param name="p3">Third point</param>
        /// <param name="p4">Fourth point</param>
        /// <param name="p5">Fifth point</param>
        /// <param name="type"> Since intention is to draw ellipse, only supplimentary options on how to draw it will be considered. Such as fitting, third point on ellipse or on center etc.</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCurve(this IRenderer graphics, IEnumerable<IParameter> parameters,
            VectorF p1, VectorF p2, VectorF p3, VectorF p4, VectorF p5,
            CurveType type = CurveType.Pie)
        {
            var curve = new Curve(p1, p2, p3, p4, type);
            DrawCurve(graphics, parameters, curve);
        }
        #endregion

        #region DRAW CONIC
        /// <summary>
        /// Renders a conic object.
        /// </summary>
        /// <param name="conic">Conic object to render</param>
        /// <param name="graphics">Buffer which to render a curve on</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawConic(this IRenderer renderer, IConic conic, params IParameter[] parameters) =>
            ((IExRenderer)renderer).RenderWithUpdate(conic, parameters);

        /// <summary>
        /// Renders a conic object.
        /// </summary>
        /// <param name="conic">Conic object to render</param>
        /// <param name="graphics">Buffer which to render a curve on</param>
        /// <param name="drawEndsOnly">If true, only out line of conic will be drawn and filling will not be performed.</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawConic(this IRenderer renderer, IEnumerable<IParameter> parameters, IConic conic) =>
            ((IExRenderer)renderer).RenderWithUpdate(conic, parameters);
        #endregion

        #region DRAW BEZIER
        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawBezier(this IRenderer graphics, IParameter[] parameters, params float[] points) =>
           RenderBezier(graphics, points, BezierType.Cubic, parameters);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawBezier(this IRenderer graphics, IEnumerable<IParameter> parameters, params float[] points) =>
           RenderBezier(graphics, points, BezierType.Cubic, parameters);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawBezier(this IRenderer graphics, float[] points, params IParameter[] parameters) =>
           RenderBezier(graphics, points, BezierType.Cubic, parameters);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawBezier(this IRenderer graphics, float[] points, IEnumerable<IParameter> parameters) =>
            RenderBezier(graphics, points, BezierType.Cubic, parameters);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawBezier(this IRenderer graphics, IEnumerable<float> points, params IParameter[] parameters) =>
           RenderBezier(graphics, points, BezierType.Cubic, parameters);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawBezier(this IRenderer graphics, IEnumerable<float> points,
            IEnumerable<IParameter> parameters) =>
          RenderBezier(graphics, points, BezierType.Cubic, parameters);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="type">BezierType enum determines the type of bezier i.e Cubic - group of 4 points or multiple(group of 4 or 7 or 10 so on...)</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawBezier(this IRenderer graphics, BezierType type, IParameter[] parameters, params float[] points) =>
          RenderBezier(graphics, points, type, parameters);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="type">BezierType enum determines the type of bezier i.e Cubic - group of 4 points or multiple(group of 4 or 7 or 10 so on...)</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawBezier(this IRenderer graphics, BezierType type, IEnumerable<IParameter> parameters, params float[] points) =>
           RenderBezier(graphics, points, type, parameters);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="type">BezierType enum determines the type of bezier i.e Cubic - group of 4 points or multiple(group of 4 or 7 or 10 so on...)</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawBezier(this IRenderer graphics, BezierType type, float[] points, params IParameter[] parameters) =>
          RenderBezier(graphics, points, type, parameters);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="type">BezierType enum determines the type of bezier i.e Cubic - group of 4 points or multiple(group of 4 or 7 or 10 so on...)</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawBezier(this IRenderer graphics, BezierType type,
            float[] points, IEnumerable<IParameter> parameters) =>
           RenderBezier(graphics, points, type, parameters);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="type">BezierType enum determines the type of bezier i.e Cubic - group of 4 points or multiple(group of 4 or 7 or 10 so on...)</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawBezier(this IRenderer graphics, BezierType type, IEnumerable<float> points,
            params IParameter[] parameters) =>
          RenderBezier(graphics, points, type, parameters);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="type">BezierType enum determines the type of bezier i.e Cubic - group of 4 points or multiple(group of 4 or 7 or 10 so on...)</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawBezier(this IRenderer graphics, BezierType type, IEnumerable<float> points,
            IEnumerable<IParameter> parameters) =>
           RenderBezier(graphics, points, type, parameters);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        public static void DrawBezier(this IRenderer graphics, params float[] points) =>
           RenderBezier(graphics, points, BezierType.Cubic, null);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in float - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="type">BezierType enum determines the type of bezier i.e Cubic - group of 4 points or multiple(group of 4 or 7 or 10 so on...)</param>
        public static void DrawBezier(this IRenderer graphics, BezierType type, params float[] points) =>
           RenderBezier(graphics, points, type, null);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in integers - each group of two subsequent values forms one point i.e x & y</param>
        public static void DrawBezier(this IRenderer graphics, params int[] points) =>
           RenderBezier(graphics, points.Select(p => (float)p), BezierType.Cubic, null);

        /// <summary>
        /// Renders a bezier defined by points and specified by type and an angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a bezier on</param>
        /// <param name="points">Defines perimiter of the bezier as values in integers - each group of two subsequent values forms one point i.e x & y</param>
        /// <param name="type">BezierType enum determines the type of bezier i.e Cubic - group of 4 points or multiple(group of 4 or 7 or 10 so on...)</param>
        public static void DrawBezier(this IRenderer graphics, BezierType type, params int[] points) =>
           RenderBezier(graphics, points.Select(p => (float)p), type, null);
        #endregion

        #region DRAW TRIANGLE
        /// <summary>
        /// Renders a triangle formed by three points specified by x1,y1 & x2,y2 & x3,y3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a triangle on</param>
        /// <param name="x1">X corodinate of the first point</param>
        /// <param name="y1">Y corodinate of the first point</param>
        /// <param name="x2">X corodinate of the second point</param>
        /// <param name="y2">Y corodinate of the second point</param>
        /// <param name="x3">X corodinate of the third point</param>
        /// <param name="y3">Y corodinate of the third point</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTriangle(this IRenderer graphics,
            float x1, float y1, float x2, float y2, float x3, float y3, params IParameter[] parameters) =>
            RenderTriangle(graphics, x1, y1, x2, y2, x3, y3, parameters);

        /// <summary>
        /// Renders a triangle formed by three points specified by x1,y1 & x2,y2 & x3,y3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a triangle on</param>
        /// <param name="x1">X corodinate of the first point</param>
        /// <param name="y1">Y corodinate of the first point</param>
        /// <param name="x2">X corodinate of the second point</param>
        /// <param name="y2">Y corodinate of the second point</param>
        /// <param name="x3">X corodinate of the third point</param>
        /// <param name="y3">Y corodinate of the third point</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTriangle(this IRenderer graphics,
            float x1, float y1, float x2, float y2, float x3, float y3, IEnumerable<IParameter> parameters) =>
           RenderTriangle(graphics, x1, y1, x2, y2, x3, y3, parameters);

        /// <summary>
        /// Renders a trianle formed by three points specified by x1,y1 & x2,y2 & x3,y3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a triangle on</param>
        /// <param name="x1">X corodinate of the first point</param>
        /// <param name="y1">Y corodinate of the first point</param>
        /// <param name="x2">X corodinate of the second point</param>
        /// <param name="y2">Y corodinate of the second point</param>
        /// <param name="x3">X corodinate of the third point</param>
        /// <param name="y3">Y corodinate of the third point</param>
        public static void DrawTriangle(this IRenderer graphics, float x1, float y1,
            float x2, float y2, float x3, float y3) =>
           graphics.DrawTriangle(x1, y1, x2, y2, x3, y3, null);

        /// <summary>
        /// Renders a trianle formed by three points specified by points p1, p2, p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a triangle on</param>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">the third point</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTriangle(this IRenderer graphics, Vector p1, Vector p2, Vector p3,
            params IParameter[] parameters) =>
            graphics.DrawTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, parameters);

        /// <summary>
        /// Renders a trianle formed by three points specified by points p1, p2, p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a triangle on</param>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">the third point</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTriangle(this IRenderer graphics, Vector p1, Vector p2, Vector p3,
            IEnumerable<IParameter> parameters) =>
            graphics.DrawTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, parameters);

        /// <summary>
        /// Renders a trianle formed by three points specified by points p1, p2, p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a triangle on</param>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">the third point</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTriangle(this IRenderer graphics, VectorF p1, VectorF p2, VectorF p3,
            params IParameter[] parameters) =>
            graphics.DrawTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, parameters);

        /// <summary>
        /// Renders a trianle formed by three points specified by points p1, p2, p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a triangle on</param>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">the third point</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTriangle(this IRenderer graphics, VectorF p1, VectorF p2, VectorF p3,
            IEnumerable<IParameter> parameters) =>
            graphics.DrawTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, parameters);

        /// <summary>
        /// Renders a trianle formed by three points specified by points p1, p2, p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a triangle on</param>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">the third point</param>
        public static void DrawTriangle(this IRenderer graphics, Vector p1, Vector p2, Vector p3) =>
            graphics.DrawTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, null);

        /// <summary>
        /// Renders a trianle formed by three points specified by points p1, p2, p3 and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a triangle on</param>
        /// <param name="p1">The first point</param>
        /// <param name="p2">The second point</param>
        /// <param name="p3">the third point</param>
        public static void DrawTriangle(this IRenderer graphics, VectorF p1, VectorF p2, VectorF p3) =>
            graphics.DrawTriangle(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, null);
        #endregion

        #region DRAW SQUARE
        /// <summary>
        /// Renders a rectangle specified by x, y, width, height parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a rectangle on</param>
        /// <param name="x">X cordinate of the rectangle</param>
        /// <param name="y">Y cordinate of the rectangle</param>
        /// <param name="width">Width  and also height of the rectangle/param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawSquare(this IRenderer graphics, float x, float y, float width, params IParameter[] parameters) =>
           RenderRectangle(graphics, x, y, width, width, parameters);

        /// <summary>
        /// Renders a rectangle specified by x, y, width, height parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a rectangle on</param>
        /// <param name="x">X cordinate of the rectangle</param>
        /// <param name="y">Y cordinate of the rectangle</param>
        /// <param name="width">Width  and also height of the rectangle/param>
        public static void DrawSquare(this IRenderer graphics, float x, float y, float width) =>
           RenderRectangle(graphics, x, y, width, width, null);
        #endregion

        #region DRAW RECTANGLE
        /// <summary>
        /// Renders a rectangle specified by x, y, width, height parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a rectangle on</param>
        /// <param name="x">X cordinate of the rectangle</param>
        /// <param name="y">Y cordinate of the rectangle</param>
        /// <param name="width">Width of the rectangle/param>
        /// <param name="height">Height the rectangle</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRectangle(this IRenderer graphics, float x, float y, float width,
            float height, params IParameter[] parameters) =>
           RenderRectangle(graphics, x, y, width, height, parameters);

        /// <summary>
        /// Renders a rectangle specified by x, y, width, height parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a rectangle on</param>
        /// <param name="x">X cordinate of the rectangle</param>
        /// <param name="y">Y cordinate of the rectangle</param>
        /// <param name="width">Width of the rectangle/param>
        /// <param name="height">Height the rectangle</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRectangle(this IRenderer graphics, float x, float y, float width, float height, IEnumerable<IParameter> parameters) =>
           RenderRectangle(graphics, x, y, width, height, parameters);

        /// <summary>
        /// Renders a rectangle specified by x, y, width, height parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a rectangle on</param>
        /// <param name="x">X cordinate of the rectangle</param>
        /// <param name="y">Y cordinate of the rectangle</param>
        /// <param name="width">Width of the rectangle/param>
        /// <param name="height">Height the rectangle</param>
        public static void DrawRectangle(this IRenderer graphics, float x, float y, float width, float height) =>
           RenderRectangle(graphics, x, y, width, height, null);

        /// <summary>
        /// Renders a rectangle specified by r parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a rectangle on</param>
        /// <param name="r">Rectangle to draw</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRectangle(this IRenderer graphics, Rectangle r, params IParameter[] parameters) =>
           RenderRectangle(graphics, r.X, r.Y, r.Width, r.Height, parameters);

        /// <summary>
        /// Renders a rectangle specified by r parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a rectangle on</param>
        /// <param name="r">Rectangle to draw</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRectangle(this IRenderer graphics, Rectangle r, IEnumerable<IParameter> parameters) =>
           RenderRectangle(graphics, r.X, r.Y, r.Width, r.Height, parameters);

        /// <summary>
        /// Renders a rectangle specified by r parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a rectangle on</param>
        /// <param name="r">Rectangle to draw</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRectangle(this IRenderer graphics, RectangleF r, params IParameter[] parameters) =>
          RenderRectangle(graphics, r.X, r.Y, r.Width, r.Height, parameters);

        /// <summary>
        /// Renders a rectangle specified by r parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a rectangle on</param>
        /// <param name="r">Rectangle to draw</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRectangle(this IRenderer graphics, RectangleF r, IEnumerable<IParameter> parameters) =>
          RenderRectangle(graphics, r.X, r.Y, r.Width, r.Height, parameters);

        /// <summary>
        /// Renders a rectangle specified by r parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a rectangle on</param>
        /// <param name="r">Rectangle to draw</param>
        public static void DrawRectangle(this IRenderer graphics, Rectangle r) =>
           RenderRectangle(graphics, r.X, r.Y, r.Width, r.Height, null);

        /// <summary>
        /// Renders a rectangle specified by r parameters and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a rectangle on</param>
        /// <param name="r">Rectangle to draw</param>
        public static void DrawRectangle(this IRenderer graphics, RectangleF r) =>
            RenderRectangle(graphics, r.X, r.Y, r.Width, r.Height, null);
        #endregion

        #region DRAW ROUNDED BOX
        /// <summary>
        /// Renders a rounded box specified by x, y, width, height parameters and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="x">X cordinate of the rounded box</param>
        /// <param name="y">Y cordinate of the rounded box</param>
        /// <param name="width">Width of the rounded box/param>
        /// <param name="height">Height the rounded box</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        public static void DrawRoundedBox(this IRenderer graphics, float x, float y, float width, float height,
            float cornerRadius, RoundBoxOption option, params IParameter[] parameters) =>
          RenderRoundedBox(graphics, x, y, width, height, cornerRadius, parameters, option);

        /// <summary>
        /// Renders a rounded box specified by x, y, width, height parameters and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="x">X cordinate of the rounded box</param>
        /// <param name="y">Y cordinate of the rounded box</param>
        /// <param name="width">Width of the rounded box/param>
        /// <param name="height">Height the rounded box</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        public static void DrawRoundedBox(this IRenderer graphics, float x, float y, float width, float height,
            float cornerRadius, RoundBoxOption option, IEnumerable<IParameter> parameters) =>
          RenderRoundedBox(graphics, x, y, width, height, cornerRadius, parameters, option);

        /// <summary>
        /// Renders a rounded box specified by x, y, width, height parameters and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="x">X cordinate of the rounded box</param>
        /// <param name="y">Y cordinate of the rounded box</param>
        /// <param name="width">Width of the rounded box/param>
        /// <param name="height">Height the rounded box</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRoundedBox(this IRenderer graphics, float x, float y, float width, float height,
            float cornerRadius, IEnumerable<IParameter> parameters) =>
          RenderRoundedBox(graphics, x, y, width, height, cornerRadius, parameters);

        /// <summary>
        /// Renders a rounded box specified by x, y, width, height parameters and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="x">X cordinate of the rounded box</param>
        /// <param name="y">Y cordinate of the rounded box</param>
        /// <param name="width">Width of the rounded box/param>
        /// <param name="height">Height the rounded box</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRoundedBox(this IRenderer graphics, float x, float y, float width, float height,
            float cornerRadius, params IParameter[] parameters) =>
          RenderRoundedBox(graphics, x, y, width, height, cornerRadius, parameters);

        /// <summary>
        /// Renders a rounded box specified by x, y, width, height parameters and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="x">X cordinate of the rounded box</param>
        /// <param name="y">Y cordinate of the rounded box</param>
        /// <param name="width">Width of the rounded box/param>
        /// <param name="height">Height the rounded box</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        public static void DrawRoundedBox(this IRenderer graphics, float x, float y, float width, float height,
            float cornerRadius, RoundBoxOption option = RoundBoxOption.Normal) =>
           RenderRoundedBox(graphics, x, y, width, height, cornerRadius, null, option);

        /// <summary>
        /// Renders a rounded box specified by x, y, width, height parameters and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="x">X cordinate of the rounded box</param>
        /// <param name="y">Y cordinate of the rounded box</param>
        /// <param name="width">Width of the rounded box/param>
        /// <param name="height">Height the rounded box</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        public static void DrawRoundedBox(this IRenderer graphics, float x, float y, float width, float height, float cornerRadius) =>
           RenderRoundedBox(graphics, x, y, width, height, cornerRadius, null);

        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRoundedBox(this IRenderer graphics, IRectangleF r, float cornerRadius, params IParameter[] parameters) =>
          RenderRoundedBox(graphics, r.X, r.Y, r.Width, r.Height, cornerRadius, parameters);

        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRoundedBox(this IRenderer graphics, IRectangleF r, float cornerRadius, IEnumerable<IParameter> parameters) =>
          RenderRoundedBox(graphics, r.X, r.Y, r.Width, r.Height, cornerRadius, parameters);

        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRoundedBox(this IRenderer graphics, IRectangle r, float cornerRadius, params IParameter[] parameters) =>
          RenderRoundedBox(graphics, r.X, r.Y, r.Width, r.Height, cornerRadius, parameters);

        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRoundedBox(this IRenderer graphics, IRectangle r, float cornerRadius, IEnumerable<IParameter> parameters) =>
           RenderRoundedBox(graphics, r.X, r.Y, r.Width, r.Height, cornerRadius, parameters);


        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        public static void DrawRoundedBox(this IRenderer graphics, IRectangleF r, float cornerRadius) =>
            RenderRoundedBox(graphics, r.X, r.Y, r.Width, r.Height, cornerRadius, null);

        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        public static void DrawRoundedBox(this IRenderer graphics, IRectangle r, float cornerRadius) =>
          RenderRoundedBox(graphics, r.X, r.Y, r.Width, r.Height, cornerRadius, null);
        #endregion

        #region DRAW ROUNDED BOX
        /// <summary>
        /// Renders a rounded box specified by x, y, width, height parameters and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="x">X cordinate of the rounded box</param>
        /// <param name="y">Y cordinate of the rounded box</param>
        /// <param name="width">Width of the rounded box/param>
        /// <param name="height">Height the rounded box</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        public static void DrawCapsule(this IRenderer graphics, float x, float y, float width, float height,
            float cornerRadius, params IParameter[] parameters) =>
          RenderCapsule(graphics, x, y, width, height, cornerRadius, parameters);

        /// <summary>
        /// Renders a rounded box specified by x, y, width, height parameters and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="x">X cordinate of the rounded box</param>
        /// <param name="y">Y cordinate of the rounded box</param>
        /// <param name="width">Width of the rounded box/param>
        /// <param name="height">Height the rounded box</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        public static void DrawCapsule(this IRenderer graphics, float x, float y, float width, float height,
            float cornerRadius, IEnumerable<IParameter> parameters) =>
          RenderCapsule(graphics, x, y, width, height, cornerRadius, parameters);

        /// <summary>
        /// Renders a rounded box specified by x, y, width, height parameters and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="x">X cordinate of the rounded box</param>
        /// <param name="y">Y cordinate of the rounded box</param>
        /// <param name="width">Width of the rounded box/param>
        /// <param name="height">Height the rounded box</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        public static void DrawCapsule(this IRenderer graphics, float x, float y, float width, float height,
            float cornerRadius) =>
           RenderRoundedBox(graphics, x, y, width, height, cornerRadius, null);

        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCapsule(this IRenderer graphics, IRectangleF r, float cornerRadius, params IParameter[] parameters) =>
          RenderRoundedBox(graphics, r.X, r.Y, r.Width, r.Height, cornerRadius, parameters);

        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCapsule(this IRenderer graphics, IRectangleF r, float cornerRadius, IEnumerable<IParameter> parameters) =>
          RenderRoundedBox(graphics, r.X, r.Y, r.Width, r.Height, cornerRadius, parameters);

        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCapsule(this IRenderer graphics, IRectangle r, float cornerRadius, params IParameter[] parameters) =>
          RenderRoundedBox(graphics, r.X, r.Y, r.Width, r.Height, cornerRadius, parameters);

        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawCapsule(this IRenderer graphics, IRectangle r, float cornerRadius, IEnumerable<IParameter> parameters) =>
           RenderRoundedBox(graphics, r.X, r.Y, r.Width, r.Height, cornerRadius, parameters);


        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        public static void DrawCapsule(this IRenderer graphics, IRectangleF r, float cornerRadius) =>
            RenderRoundedBox(graphics, r.X, r.Y, r.Width, r.Height, cornerRadius, null);

        /// <summary>
        /// Renders a rounded box specified by r parameter and angle of rotation if supplied and a hull convex of circle determined by corner radius at all four corners.
        /// </summary>
        /// <param name="graphics">graphics which to render a rounded box on</param>
        /// <param name="r">Base rectange to construct the rounded box from</param>
        /// <param name="cornerRadius">Radius of a circle - convex hull of which is to be drawn on each corner</param>
        public static void DrawCapsule(this IRenderer graphics, IRectangle r, float cornerRadius) =>
          RenderRoundedBox(graphics, r.X, r.Y, r.Width, r.Height, cornerRadius, null);
        #endregion

        #region DRAW RHOMBUS
        /// <summary>
        /// Renders a rhombus specified by three points (The fourth point will be calculated) and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a rhombus on</param>
        /// <param name="first">First point</param>
        /// <param name="second">Second point</param>
        /// <param name="third">Third point</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRhombus(this IRenderer graphics, VectorF first, VectorF second, VectorF third, params IParameter[] parameters) =>
          RenderRhombus(graphics, first, second, third, parameters);

        /// <summary>
        /// Renders a rhombus specified by three points (The fourth point will be calculated) and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a rhombus on</param>
        /// <param name="first">First point</param>
        /// <param name="second">Second point</param>
        /// <param name="third">Third point</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRhombus(this IRenderer graphics, VectorF first, VectorF second, VectorF third, IEnumerable<IParameter> parameters) =>
            RenderRhombus(graphics, first, second, third, parameters);

        /// <summary>
        /// Renders a rhombus specified by three points (The fourth point will be calculated) and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a rhombus on</param>
        /// <param name="first">First point</param>
        /// <param name="second">Second point</param>
        /// <param name="third">Third point</param>
        public static void DrawRhombus(this IRenderer graphics, VectorF first, VectorF second, VectorF third) =>
           RenderRhombus(graphics, first, second, third, null);

        /// <summary>
        /// Renders a rhombus specified by three points (The fourth point will be calculated) and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="x1">X coordinate of first point</param>
        /// <param name="y1">>Y coordinate of first point</param>
        /// <param name="x2">X coordinate of second point</param>
        /// <param name="y2">Y coordinate of second point</param>
        /// <param name="x3">X coordinate of third point</param>
        /// <param name="y3">Y coordinate of third point</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRhombus(this IRenderer graphics, float x1, float y1, float x2, float y2, float x3, float y3,
            params IParameter[] parameters) =>
          RenderRhombus(graphics, new VectorF(x1, y1), new VectorF(x2, y2), new VectorF(x3, y3), parameters);

        /// <summary>
        /// Renders a rhombus specified by three points (The fourth point will be calculated) and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="x1">X coordinate of first point</param>
        /// <param name="y1">>Y coordinate of first point</param>
        /// <param name="x2">X coordinate of second point</param>
        /// <param name="y2">Y coordinate of second point</param>
        /// <param name="x3">X coordinate of third point</param>
        /// <param name="y3">Y coordinate of third point</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawRhombus(this IRenderer graphics, float x1, float y1, float x2, float y2, float x3, float y3,
            IEnumerable<IParameter> parameters) =>
            RenderRhombus(graphics, new VectorF(x1, y1), new VectorF(x2, y2), new VectorF(x3, y3), parameters);


        /// <summary>
        /// Renders a rhombus specified by three points (The fourth point will be calculated) and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="x1">X coordinate of first point</param>
        /// <param name="y1">>Y coordinate of first point</param>
        /// <param name="x2">X coordinate of second point</param>
        /// <param name="y2">Y coordinate of second point</param>
        /// <param name="x3">X coordinate of third point</param>
        /// <param name="y3">Y coordinate of third point</param>
        public static void DrawRhombus(this IRenderer graphics, float x1, float y1, float x2, float y2, float x3, float y3) =>
           RenderRhombus(graphics, new VectorF(x1, y1), new VectorF(x2, y2), new VectorF(x3, y3), null);
        #endregion

        #region DRAW TRAPEZIUM
        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="baseLine">A line from where the trapezium start</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTrapezium(this IRenderer graphics, ILine baseLine, float parallelLineDeviation, float skewBy, params IParameter[] parameters) =>
           RenderTrapezium(graphics, baseLine, parallelLineDeviation, skewBy, parameters);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="baseLine">A line from where the trapezium start</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTrapezium(this IRenderer graphics, ILine baseLine, float parallelLineDeviation, float skewBy, IEnumerable<IParameter> parameters) =>
           RenderTrapezium(graphics, baseLine, parallelLineDeviation, skewBy, parameters);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        public static void DrawTrapezium(this IRenderer graphics, ILine baseLine, float parallelLineDeviation, float skewBy) =>
           RenderTrapezium(graphics, baseLine, parallelLineDeviation, skewBy, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        public static void DrawTrapezium(this IRenderer graphics, ILine baseLine, float parallelLineDeviation) =>
           RenderTrapezium(graphics, baseLine, parallelLineDeviation, 0, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="baseLine">A line from where the trapezium start</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTrapezium(this IRenderer graphics, ILine baseLine, float parallelLineDeviation, params IParameter[] parameters) =>
          RenderTrapezium(graphics, baseLine, parallelLineDeviation, 0, parameters);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line, parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="baseLine">A line from where the trapezium start</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTrapezium(this IRenderer graphics, ILine baseLine, float parallelLineDeviation, IEnumerable<IParameter> parameters) =>
           RenderTrapezium(graphics, baseLine, parallelLineDeviation, 0, parameters);


        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by the first four values in values parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="values">An array of float values- first four of which forms a base line from where the trapezium start.
        /// Parallel line deviation should be the 5th i.e values[4] item in values parameter if values has length of 5 otherwise deemed as zero which results in a simple line draw.
        /// 6th item i.e values[5] in values would form parallel Line Size Difference value if the lenght of values is 6 otherwise zero.
        /// </param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTrapezium(this IRenderer graphics, float[] values, params IParameter[] parameters) =>
            DrawTrapezium(graphics, values, (IEnumerable<IParameter>)parameters);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by the first four values in values parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="values">An array of float values- first four of which forms a base line from where the trapezium start.
        /// Parallel line deviation should be the 5th i.e values[4] item in values parameter if values has length of 5 otherwise deemed as zero which results in a simple line draw.
        /// 6th item i.e values[5] in values would form parallel Line Size Difference value if the lenght of values is 6 otherwise zero.
        /// </param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTrapezium(this IRenderer graphics, float[] values, IEnumerable<IParameter> parameters)
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
            RenderTrapezium(graphics, first, parallelLineDeviation, skewBy, parameters);
        }

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by the first four values in values parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="values">An array of int values- first four of which forms a base line from where the trapezium start.
        /// Parallel line deviation should be the 5th i.e values[4] item in values parameter if values has length of 5 otherwise deemed as zero which results in a simple line draw.
        /// 6th item i.e values[5] in values would form parallel Line Size Difference value if the lenght of values is 6 otherwise zero.
        /// </param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTrapezium(this IRenderer graphics, int[] values, IEnumerable<IParameter> parameters)
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
            RenderTrapezium(graphics, first, deviation, skewBy, parameters);
        }

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by the first four values in values parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="values">An array of int values- first four of which forms a base line from where the trapezium start.
        /// Parallel line deviation should be the 5th i.e values[4] item in values parameter if values has length of 5 otherwise deemed as zero which results in a simple line draw.
        /// 6th item i.e values[5] in values would form parallel Line Size Difference value if the lenght of values is 6 otherwise zero.
        /// </param>
        /// <param name="angle">Angle to apply rotation while rendering the trapezium</param>
        /// <param name="settings">A pen settings which to create a graphics pen from</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTrapezium(this IRenderer graphics, int[] values, params IParameter[] parameters) =>
            DrawTrapezium(graphics, values, (IEnumerable<IParameter>)parameters);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by the first four values in values parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="values">An array of float values- first four of which forms a base line from where the trapezium start.
        /// Parallel line deviation should be the 5th i.e values[4] item in values parameter if values has length of 5 otherwise deemed as zero which results in a simple line draw.
        /// 6th item i.e values[5] in values would form parallel Line Size Difference value if the lenght of values is 6 otherwise zero.
        /// </param>
        public static void DrawTrapezium(this IRenderer graphics, float[] values) =>
           graphics.DrawTrapezium(values, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by the first four values in values parameter and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="values">An array of int values- first four of which forms a base line from where the trapezium start.
        /// Parallel line deviation should be the 5th i.e values[4] item in values parameter if values has length of 5 otherwise deemed as zero which results in a simple line draw.
        /// 6th item i.e values[5] in values would form parallel Line Size Difference value if the lenght of values is 6 otherwise zero.
        /// </param>
        public static void DrawTrapezium(this IRenderer graphics, int[] values) =>
           graphics.DrawTrapezium(values, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from parameters x1, y1, x2, y2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTrapezium(this IRenderer graphics, float x1, float y1, float x2, float y2,
            float parallelLineDeviation, float skewBy, params IParameter[] parameters) =>
           RenderTrapezium(graphics, new Line(x1, y1, x2, y2), parallelLineDeviation, skewBy, parameters);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from parameters x1, y1, x2, y2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="deviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        public static void DrawTrapezium(this IRenderer graphics, float x1, float y1, float x2, float y2,
            float deviation, float skewBy) =>
           RenderTrapezium(graphics, new Line(x1, y1, x2, y2), deviation, skewBy, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from parameters x1, y1, x2, y2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTrapezium(this IRenderer graphics, float x1, float y1, float x2, float y2,
            float parallelLineDeviation, params IParameter[] parameters) =>
           RenderTrapezium(graphics, new Line(x1, y1, x2, y2), parallelLineDeviation, 0, parameters);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from parameters x1, y1, x2, y2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        public static void DrawTrapezium(this IRenderer graphics, float x1, float y1, float x2, float y2,
            float parallelLineDeviation) =>
            RenderTrapezium(graphics, new Line(x1, y1, x2, y2), parallelLineDeviation, 0, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTrapezium(this IRenderer graphics, VectorF p1, VectorF p2,
            float parallelLineDeviation, float skewBy, params IParameter[] parameters) =>
           RenderTrapezium(graphics, new Line(p1, p2), parallelLineDeviation, skewBy, parameters);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTrapezium(this IRenderer graphics, Vector p1, Vector p2,
            float parallelLineDeviation, float skewBy, params IParameter[] parameters) =>
           RenderTrapezium(graphics, new Line(p1.X, p1.Y, p2.X, p2.Y), parallelLineDeviation, skewBy, parameters);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        public static void DrawTrapezium(this IRenderer graphics, VectorF p1, VectorF p2, float parallelLineDeviation, float skewBy) =>
           RenderTrapezium(graphics, new Line(p1, p2), parallelLineDeviation, skewBy, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="skewBy">A change in parallel line size to tilt the trapezium</param>
        public static void DrawTrapezium(this IRenderer graphics, Vector p1, Vector p2, float parallelLineDeviation, float skewBy) =>
           RenderTrapezium(graphics, new Line(p1.X, p1.Y, p2.X, p2.Y), parallelLineDeviation, skewBy, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        public static void DrawTrapezium(this IRenderer graphics, VectorF p1, VectorF p2,
            float parallelLineDeviation, params IParameter[] parameters) =>
          RenderTrapezium(graphics, new Line(p1, p2), parallelLineDeviation, 0, parameters);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawTrapezium(this IRenderer graphics, Vector p1, Vector p2,
            float parallelLineDeviation, params IParameter[] parameters) =>
          RenderTrapezium(graphics, new Line(p1.X, p1.Y, p2.X, p2.Y), parallelLineDeviation, 0, parameters);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        public static void DrawTrapezium(this IRenderer graphics, VectorF p1, VectorF p2, float parallelLineDeviation) =>
           RenderTrapezium(graphics, new Line(p1, p2), parallelLineDeviation, 0, null);

        /// <summary>
        /// Renders a trapezium (defined as per the definition in British English) specified by a base line formed from points p1 & p2, 
        /// parallel line deviation and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a trapezium on</param>
        /// <param name="p1">A start point of a base line</param>
        /// <param name="p2">An end point of a base line</param>
        /// <param name="parallelLineDeviation">A deviation from a base line to form a parallel line to construct a trapezium</param>
        public static void DrawTrapezium(this IRenderer graphics, Vector p1, Vector p2, float parallelLineDeviation) =>
           RenderTrapezium(graphics, new Line(p1.X, p1.Y, p2.X, p2.Y), parallelLineDeviation, 0, null);
        #endregion

        #region DRAW POLYGON
        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon  an each group of two subsequent values in polypoints forms a point x,y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawPolygon(this IRenderer graphics, IParameter[] parameters, params float[] polyPoints) =>
          RenderPolygon(graphics, polyPoints, parameters);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon  an each group of two subsequent values in polypoints forms a point x,y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawPolygon(this IRenderer graphics, IEnumerable<IParameter> parameters, params float[] polyPoints) =>
            RenderPolygon(graphics, polyPoints, parameters);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon  an each group of two subsequent values in polypoints forms a point x,y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawPolygon(this IRenderer graphics, float[] polyPoints, params IParameter[] parameters) =>
           RenderPolygon(graphics, polyPoints, parameters);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon  an each group of two subsequent values in polypoints forms a point x,y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawPolygon(this IRenderer graphics, float[] polyPoints, IEnumerable<IParameter> parameters) =>
           RenderPolygon(graphics, polyPoints, parameters);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon  an each group of two subsequent values in polypoints forms a point x,y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawPolygon(this IRenderer graphics, IEnumerable<float> polyPoints,
            params IParameter[] parameters) =>
          RenderPolygon(graphics, polyPoints, parameters);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon  an each group of two subsequent values in polypoints forms a point x,y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawPolygon(this IRenderer graphics, IEnumerable<float> polyPoints,
            IEnumerable<IParameter> parameters) =>
           RenderPolygon(graphics, polyPoints, parameters);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon an each group of two subsequent values in polypoints forms a point x,y</param>
        public static void DrawPolygon(this IRenderer graphics, params float[] polyPoints) =>
            RenderPolygon(graphics, polyPoints, null);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon an each group of two subsequent values in polypoints forms a point x,y</param>
        public static void DrawPolygon(this IRenderer graphics, params int[] polyPoints) =>
           RenderPolygon(graphics, polyPoints.Select(p => (float)p), null);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon  an each group of two subsequent values in polypoints forms a point x,y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawPolygon(this IRenderer graphics,
            IParameter[] parameters, params int[] polyPoints) =>
           RenderPolygon(graphics, polyPoints.Select(p => (float)p), parameters);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon  an each group of two subsequent values in polypoints forms a point x,y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawPolygon(this IRenderer graphics,
           IEnumerable<IParameter> parameters, params int[] polyPoints) =>
          RenderPolygon(graphics, polyPoints.Select(p => (float)p), parameters);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon  an each group of two subsequent values in polypoints forms a point x,y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawPolygon(this IRenderer graphics,
            int[] polyPoints, params IParameter[] parameters) =>
           RenderPolygon(graphics, polyPoints.Select(p => (float)p), parameters);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon  an each group of two subsequent values in polypoints forms a point x,y</param>
        public static void DrawPolygon(this IRenderer graphics,
            int[] polyPoints, IEnumerable<IParameter> parameters) =>
            RenderPolygon(graphics, polyPoints.Select(p => (float)p), parameters);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon  an each group of two subsequent values in polypoints forms a point x,y</param>
        public static void DrawPolygon(this IRenderer graphics,
            IEnumerable<int> polyPoints, params IParameter[] parameters) =>
            RenderPolygon(graphics, polyPoints.Select(p => (float)p), parameters);

        /// <summary>
        /// Renders a polygon specified by a collection of points and angle of rotation if supplied.
        /// </summary>
        /// <param name="graphics">graphics which to render a polygom on</param>
        /// <param name="polyPoints">A collection of points which forms perimeter of the polygon  an each group of two subsequent values in polypoints forms a point x,y</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        public static void DrawPolygon(this IRenderer graphics,
            IEnumerable<int> polyPoints, IEnumerable<IParameter> parameters) =>
           RenderPolygon(graphics, polyPoints.Select(p => (float)p), parameters);
        #endregion

        #region DRAW TEXT
        /// <summary>
        /// Draw text on a specified graphics using specified parameters.
        /// </summary>
        /// <param name="graphics">graphics which to render a rhombus on</param>
        /// <param name="font">Font object to get glyphs collection for a given text</param>
        /// <param name="text">A string of characters to draw</param>
        /// <returns>GlyphsData object which contains a draw result information such as glyphs, drawn area etc.</returns>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IText DrawText(this IRenderer graphics, IFont font, string text,
            IParameter[] parameters) =>
           RenderText(graphics, font, text, parameters);

        /// <summary>
        /// Draw text on a specified graphics using specified parameters.
        /// </summary>
        /// <param name="graphics">graphics which to render a rhombus on</param>
        /// <param name="font">Font object to get glyphs collection for a given text</param>
        /// <param name="text">A string of characters to draw</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        /// <returns>GlyphsData object which contains a draw result information such as glyphs, drawn area etc.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IText DrawText(this IRenderer graphics, IFont font, string text,
            IEnumerable<IParameter> parameters) =>
           RenderText(graphics, font, text, parameters);

        /// <summary>
        /// Draw text on a specified graphics using specified parameters.
        /// </summary>
        /// <param name="graphics">graphics which to render a rhombus on</param>
        /// <param name="font">Font object to get glyphs collection for a given text</param>
        /// <param name="text">A string of characters to draw</param>
        /// <returns>GlyphsData object which contains a draw result information such as glyphs, drawn area etc.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IText DrawText(this IRenderer graphics, IFont font, string text) =>
           RenderText(graphics, font, text, null);

        /// <summary>
        /// Draw text on a specified graphics using specified parameters.
        /// </summary>
        /// <param name="graphics">graphics which to render a rhombus on</param>
        /// <param name="font">Font object to get glyphs collection for a given text</param>
        /// <param name="text">A string of characters to draw</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        /// <returns>GlyphsData object which contains a draw result information such as glyphs, drawn area etc.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IText DrawText(this IRenderer graphics, IFont font, string text, int x, int y,
            params IParameter[] parameters) =>
           RenderText(graphics, font, text, parameters.AppendItem(new Location(x, y)));

        /// <summary>
        /// Draw text on a specified graphics using specified parameters.
        /// </summary>
        /// <param name="graphics">graphics which to render a rhombus on</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        /// <param name="font">Font object to get glyphs collection for a given text</param>
        /// <param name="text">A string of characters to draw</param>
        /// <returns>GlyphsData object which contains a draw result information such as glyphs, drawn area etc.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IText DrawText(this IRenderer graphics,
            IEnumerable<IParameter> parameters, IFont font, string text, int x, int y) =>
           RenderText(graphics, font, text, parameters.AppendItem(new Location(x, y)));

        /// <summary>
        /// Renders a text object which represents a text and a collection of glyphs providing drawing representation of the text. 
        /// </summary>
        /// <param name="graphics">Buffer which to render a rhombus on</param>
        /// <param name="text">A text object to render</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawText(this IRenderer renderer, IText text, params IParameter[] parameters) =>
            ((IExRenderer)renderer).RenderWithUpdate(text, parameters);

        /// <summary>
        /// Renders a text object which represents a text and a collection of glyphs providing drawing representation of the text. 
        /// </summary>
        /// <param name="graphics">Buffer which to render a rhombus on</param>
        /// <param name="text">A text object to render</param>
        /// <param name="parameters">Various parameters to influence and control drawing operation.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawText(this IRenderer renderer, IText text, IEnumerable<IParameter> parameters) =>
            ((IExRenderer)renderer).RenderWithUpdate(text, parameters);
        #endregion

        #region RENDER CIRCLE OR ELLIPSE
        static void RenderCircleOrEllipse(this IRenderer renderer, float x, float y, float width, float height)
        {
            var curve = new Ellipse(x, y, width, height);
            ((IExRenderer)renderer).RenderWithUpdate(curve, null);
        }
        static void RenderCircleOrEllipse(this IRenderer renderer, float x, float y, float width, float height,
            IEnumerable<IParameter> parameters)
        {
            var curve = new Ellipse(x, y, width, height);
            ((IExRenderer)renderer).RenderWithUpdate(curve, parameters);
        }
        #endregion

        #region RENDER ARC - PIE
        static void RenderArcOrPie(this IRenderer renderer, float x, float y, float width, float height,
            float startAngle, float endAngle, IEnumerable<IParameter> parameters, CurveType type = CurveType.Pie)
        {
            bool sweepAngle = (type & CurveType.NoSweepAngle) != CurveType.NoSweepAngle;
            bool negativeMotion = (type & CurveType.AntiClock) == CurveType.AntiClock;
            bool pieAngle = (type & CurveType.AbsolutePieAngle) != CurveType.AbsolutePieAngle;

            IObject shape;
            if ((type & CurveType.Arc) == CurveType.Arc)
            {
                shape = new Arc(x, y, width, height, startAngle, endAngle, negativeMotion, sweepAngle, pieAngle);
            }
            else
            {
                shape = new Pie(x, y, width, height, startAngle, endAngle, negativeMotion, sweepAngle, pieAngle);
            }
            ((IExRenderer)renderer).RenderWithUpdate(shape, parameters);
        }
        #endregion

        #region RENDER BEZIER
        static void RenderBezier(this IRenderer renderer, IEnumerable<float> pts, BezierType type,
            IEnumerable<IParameter> parameters)
        {
            Bezier bezier = new Bezier(type, pts.ToArray(), default(IList<VectorF>));
            ((IExRenderer)renderer).RenderWithUpdate(bezier, parameters);
        }
        #endregion

        #region RENDER TRINAGLE
        static void RenderTriangle(this IRenderer renderer, float x1, float y1, float x2, float y2, float x3, float y3, IEnumerable<IParameter> parameters)
        {
            Triangle triangle = new Triangle(x1, y1, x2, y2, x3, y3);
            ((IExRenderer)renderer).RenderWithUpdate(triangle, parameters);
        }
        #endregion

        #region RENDER POLYGON
        static void RenderPolygon(this IRenderer renderer, IEnumerable<float> polyPoints,
            IEnumerable<IParameter> parameters)
        {
            IList<VectorF> points = polyPoints.ToVectorF();
            var polygon = new Polygon(points);
            ((IExRenderer)renderer).RenderWithUpdate(polygon, parameters);
        }
        #endregion

        #region RENDER RECTANGLE
        static void RenderRectangle(this IRenderer renderer, float x, float y, float width, float height, IEnumerable<IParameter> parameters)
        {
            BoxF box = new BoxF(x, y, width, height);
            ((IExRenderer)renderer).RenderWithUpdate(box, parameters);
        }
        static void RenderRectangle(this IRenderer renderer, int x, int y, int width, int height,
            IEnumerable<IParameter> parameters)
        {
            Box box = new Box(x, y, width, height);
            ((IExRenderer)renderer).RenderWithUpdate(box, parameters);
        }
        #endregion

        #region RENDER ROUNDED BOX
        static void RenderRoundedBox(this IRenderer renderer, float x, float y, float width, float height, float cornerRadius, IEnumerable<IParameter> parameters, RoundBoxOption option = 0)
        {
            var shape = new RoundBox(x, y, width, height, cornerRadius, option);
            ((IExRenderer)renderer).RenderWithUpdate(shape, parameters);
        }

        #endregion

        #region RENDER ROUNDED BOX
        static void RenderCapsule(this IRenderer renderer, float x, float y, float width, float height, 
            float cornerRadius, IEnumerable<IParameter> parameters)
        {
            var shape = new Capsule(x, y, width, height, cornerRadius);
            ((IExRenderer)renderer).RenderWithUpdate(shape, parameters);
        }
        #endregion

        #region RENDER RHOMBUS
        static void RenderRhombus(this IRenderer renderer, VectorF first, VectorF second, VectorF third,
            IEnumerable<IParameter> parameters)
        {
            var rhombus = new Tetragon(first, second, third);
            ((IExRenderer)renderer).RenderWithUpdate(rhombus, parameters);
        }
        #endregion

        #region RENDER TRAPEZIUM
        static void RenderTrapezium(this IRenderer renderer, ILine baseLine, float deviation,
            float skeyBy, IEnumerable<IParameter> parameters)
        {
            FillCommand fillMode = 0;
            if (parameters != null)
                fillMode = (parameters.LastOrDefault(p => p is IModifier<FillCommand>)
                    as IModifier<FillCommand>)?.Modify(fillMode) ?? 0;
            Tetragon trapezium = new Tetragon(baseLine, deviation, fillMode, skeyBy);
            ((IExRenderer)renderer).RenderWithUpdate(trapezium, parameters);
        }
        #endregion

        #region RENDER TEXT
        static IText RenderText(this IRenderer renderer, IFont font, string text,
          IEnumerable<IParameter> parameters)
        {
            if (renderer == null || font == null || string.IsNullOrEmpty(text))
                return null;
            var element = new Text(text);

            ((IExRenderer)renderer).RenderWithUpdate(element, parameters.AppendItem(font));
            return element;
        }
        #endregion
    }
}
#endif
