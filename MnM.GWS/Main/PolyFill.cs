/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
#if GWS || Window
#if AllHidden
    partial class NativeFactory
    {
#else
    public
#endif
        sealed class PolyFill : _PolyFill, IPolyFill
        {
    #region VARIABLES
            Collection<float>[] Results;
            VectorF Start;
            Collection<VectorF> points;
    #endregion

    #region BEGIN - END
            public override void Begin(int y, int bottom)
            {
                base.Begin(y, bottom);
                Results = new Collection<float>[MaxY - MinY + 4];
                points = new Collection<VectorF>(Results.Length);
            }
            public override void End()
            {
                base.End();
                Results = null;
                points = null;
            }
    #endregion

    #region FILL
            public override void Fill(FillAction fillAction)
            {
                if (fillAction == null)
                    return;

                if (points != null)
                {
                    var pixelAction = fillAction.ToPixelAction();
                    for (int i = 1; i < points.Count; i++)
                    {
                        Scan(points[i - 1].X, points[i - 1].Y, points[i].X, points[i].Y);
                        Renderer.ProcessLine(points[i - 1].X, points[i - 1].Y, points[i].X, points[i].Y, pixelAction, drawCommand);
                    }
                }

                for (int i = MinY; i < MaxY; i++)
                    FillLine(Results[i - MinY], i, true, fillAction);
            }
    #endregion

    #region FILL SCAN LINE
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void FillLine(ICollection<float> data, int axis, bool horizontal, FillAction action, float? alpha = null)
            {
                if (data == null || data.Count == 0)
                    return;

                var collection = data.ToArray();

                if (collection.Length == 1)
                {
                    if (FillSinglePoint)
                        action(collection[0], axis, horizontal, collection[0], alpha, drawCommand);
                    return;
                }

                if (collection.Length == 2)
                {
                    action(collection[0], axis, horizontal, collection[1], alpha, drawCommand);
                    return;
                }

                var Even = collection.Length % 2 == 0;

                if (Sorting)
                    Array.Sort(collection);

                for (int i = 1; i < collection.Length; i++)
                {
                    action(collection[i - 1], axis, horizontal, collection[i], alpha, drawCommand);
                    if (EndsOnly)
                        continue;
                    if (Even)
                        ++i;
                }
            }
    #endregion

    #region SCAN
            public override void Scan(float x, int y)
            {
                if (!y.IsWithIn(MinY, MaxY))
                    return;
                if (Results[y - MinY] == null)
                    Results[y - MinY] = new Collection<float>(3);
                Results[y - MinY].Add(x);
            }
            public override void Scan(VectorF p)
            {
                if (!Start)
                {
                    Start = p;
                    points.Add(p);
                    return;
                }
                points.Add(Start);
                Start = p;
            }
            public override void Scan(IList<VectorF> Points, IList<int> Contours = null)
            {
                var firstIndex = 0;
                if (Contours == null || Contours.Count == 0)
                    Contours = new int[] { Points.Count - 1 };

                for (int i = 0; i < Contours.Count; i++)
                {
                    // decompose the contour into drawing commands
                    int lastIndex = Contours[i];
                    var pointIndex = firstIndex;
                    var start = Points[pointIndex];
                    var end = Points[lastIndex];
                    if (start.Quadratic != 0)
                    {
                        // if first point is a control point, try using the last point
                        if (end.Quadratic == 0)
                        {
                            start = end;
                            lastIndex--;
                        }
                        else
                        {
                            // if they're both control points, start at the middle
                            start = (start + end) / 2f;
                        }
                        pointIndex--;
                    }

                    // let's draw this contour
                    MoveTo(start);

                    var needClose = true;
                    while (pointIndex < lastIndex)
                    {
                        var point = Points[++pointIndex];
                        switch (point.Quadratic)
                        {
                            case 0:
                            default:
                                Scan(point);
                                break;

                            case 1:
                                var control = point;
                                var done = false;
                                while (pointIndex < lastIndex)
                                {
                                    var next = Points[++pointIndex];
                                    if (next.Quadratic == 0)
                                    {
                                        CurveTo(control, next);
                                        done = true;
                                        break;
                                    }

                                    if (next.Quadratic == 0)
                                        throw new Exception("Bad outline data.");
                                    var p = (control + next) / 2f;
                                    CurveTo(control, p);
                                    control = next;
                                }

                                if (!done)
                                {
                                    // if we hit this point, we're ready to close out the contour
                                    CurveTo(control, start);
                                    needClose = false;
                                }
                                break;
                        }
                    }

                    if (needClose)
                        Scan(start);
                    // next contour starts where this one left off
                    firstIndex = lastIndex + 1;
                }
            }
    #endregion

    #region NOTIFY SCAN RESULT
            protected override void NotifyScanResult(float value, int axis, bool horizontal, Command command)
            {
                Numbers.Confine(MinX, MaxX, ref value);

                int i = axis - MinY;
                if (i < 0 || i >= Results.Length)
                    return;
                if (Results[i] == null)
                    Results[i] = new Collection<float>(10);

                Results[i].Add(value);
            }
    #endregion

    #region MOVE TO
            void MoveTo(VectorF p)
            {
                Start = p;
            }
    #endregion

    #region CURVE TO
            void CurveTo(VectorF controlPoint1, VectorF controlPoint2, VectorF endPoint)
            {
                if (!Start)
                    return;
                Curves.GetBezierPoints(4, ref points, Start, controlPoint1, controlPoint2, endPoint);
                Start = endPoint;
            }
            void CurveTo(VectorF controlPoint1, VectorF endPoint)
            {
                if (!Start)
                    return;
                Collection<VectorF> list = new Collection<VectorF>(100);
                Curves.GetBezierPoints(4, ref points, Start, controlPoint1, endPoint);
                Start = endPoint;
            }
    #endregion
        }
#if AllHidden
    }
#endif
#endif
}
