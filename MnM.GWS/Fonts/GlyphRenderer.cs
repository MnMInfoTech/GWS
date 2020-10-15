/*Copyright(c) 2015 Michael Popoloski

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
 the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
#if AllHidden
    partial class _Factory
    {
#else
    public
#endif
        class GlyphRenderer : IGlyphRenderer
        {
            #region variables
            const float EPSILON = .0001f;
            int[] scanlines;                // one scanline per Y, points into cell buffer
            int[] curveLevels;
            VectorF[] bezierArc;            // points on a bezier arc
            Cell[] cells;
            VectorF activePoint;            // subpixel position of active point
            float activeArea;               // running total of the active cell's area
            float activeCoverage;           // ditto for coverage
            int cellX, cellY;               // pixel position of the active cell
            int cellCount;                  // number of cells  active use
            bool cellActive;                // whether the current cell has active data
            IList<VectorF> Points;
            IList<int> Contours;
            int iw, ih;
            FillAction<int> action;
            #endregion

            #region constructor
            static GlyphRenderer() { }
            #endregion

            #region methods
            public void Process(IList<VectorF> points, IList<int> contours, FillAction<int> action, int width, int height)
            {
                if (contours == null)
                    Contours = new int[] { points.Count - 1 };
                else
                    Contours = contours;

                iw = width;
                ih = height;

                if (points is IList<VectorF>)
                    Points = points as IList<VectorF>;
                else
                    Points = points.ToArray();

                this.action = action;

                cellCount = 0;
                activeArea = 0.0f;
                activeCoverage = 0.0f;
                cellActive = false;
                if (cells == null)
                {
                    cells = new Cell[1024];
                    curveLevels = new int[32];
                    bezierArc = new VectorF[curveLevels.Length * 3 + 1];
                    //curveCubicLevels = new int[49];
                    //bezierCubicArc = new IPointF[curveCubicLevels.Length * 3 + 1];

                    scanlines = new int[ih];
                }
                else if (ih >= scanlines.Length)
                    scanlines = new int[ih];

                for (int i = 0; i < ih; i++)
                    scanlines[i] = -1;

                // check for an empty outline, which obviously results  an empty render
                if (iw <= 0 || ih <= 0)
                    return;

                if (Points.Count <= 0)
                    return;

                Decompose();
                Process();
            }

            void FillHLine(int x, int y, float alpha, int len)
            {
                if (alpha < 0)
                    alpha = -alpha;

                if (alpha < EPSILON)
                    return;
                if (len == 0)
                    return;
                Angles.Rotate180(x, y, ih, out x, out y);
                action(x,  y, true, x + len, alpha);
            }
            void RenderScanline(int scanline, float x1, float y1, float x2, float y2)
            {
                var ex1 = (int)x1;
                var ex2 = (int)x2;
                var fx1 = x1 - ex1;
                var fx2 = x2 - ex2;

                var dx = x2 - x1;
                var dy = y2 - y1;

                if (dy == 0)
                {
                    SetCurrentCell(ex2, scanline);
                    return;
                }

                if (ex1 == ex2)
                {
                    activeArea += (fx1 + fx2) * dy;
                    activeCoverage += dy;
                    return;
                }

                var dist = (1 - fx1) * dy;
                var first = 1f;
                var increment = 1;
                if (dx < 0)
                {
                    dist = fx1 * dy;
                    first = 0.0f;
                    increment = -1;
                    dx = -dx;
                }

                var delta = dist / dx;
                activeArea += (fx1 + first) * delta;
                activeCoverage += delta;

                ex1 += increment;
                SetCurrentCell(ex1, scanline);
                y1 += delta;

                if (ex1 != ex2)
                {
                    dist = y2 - y1 + delta;
                    delta = dist / dx;

                    while (ex1 != ex2)
                    {
                        activeArea += delta;
                        activeCoverage += delta;
                        y1 += delta;
                        ex1 += increment;
                        SetCurrentCell(ex1, scanline);
                    }
                }

                delta = y2 - y1;
                activeArea += (fx2 + 1f - first) * delta;
                activeCoverage += delta;
            }

            void Decompose()
            {
                var firstIndex = 0;

                for (int i = 0; i < Contours.Count; i++)
                {
                    // decompose the contour into drawing commands
                    int lastIndex = Contours[i];
                    var pointIndex = firstIndex;
                    var start = Points[pointIndex];
                    var end = Points[lastIndex];
                    if (start.Quadratic !=0)
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
                            start = new VectorF((start.X + end.X) / 2f, (start.Y + end.Y) / 2f, start.Quadratic);
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
                                LineTo(point);
                                break;

                            case 1:
                                VectorF control = point;
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
                                    var p = new VectorF((control.X + next.X) / 2f, (control.Y + next.Y) / 2f, control.Quadratic);
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
                        LineTo(start);
                    // next contour starts where this one left off
                    firstIndex = lastIndex + 1;
                }
            }
            void Process()
            {
                if (cellActive)
                    RetireActiveCell();

                // if we rendered nothing, there's nothing to do
                if (cellCount == 0)
                    return;

                for (int y = 0; y < ih; y++)
                {
                    var x = 0;
                    var coverage = 0.0f;
                    var index = scanlines[y];

                    while (index != -1)
                    {
                        // cap off the previous span, if we had one
                        var cell = cells[index];
                        if (cell.X > x && coverage != 0.0f)
                            FillHLine(x, y, coverage, cell.X - x);

                        coverage += cell.Coverage;

                        var area = coverage - (cell.Area / 2f);
                        if (area != 0.0f && cell.X >= 0)
                            FillHLine(cell.X, y, area, 1);

                        x = cell.X + 1;
                        index = cell.Next;
                    }

                    // finish off the trailing span
                    if (coverage != 0.0f)
                        FillHLine(x, y, coverage, (iw - x));
                }
            }

            void MoveTo(VectorF point)
            {
                // record current cell, if any
                if (cellActive)
                    RetireActiveCell();

                // calculate cell coordinates
                activePoint = point;
                cellX = Math.Max(-1, Math.Min((int)activePoint.X, iw));
                cellY = (int)activePoint.Y;

                // activate if this is a valid cell location
                cellActive = cellX < iw && cellY < ih;
                activeArea = 0.0f;
                activeCoverage = 0.0f;
            }
            void LineTo(VectorF point)
            {
                // figure out which scanlines this line crosses
                var startScanline = (int)activePoint.Y;
                var endScanline = (int)point.Y;

                // vertical clipping
                if (Math.Min(startScanline, endScanline) >= ih ||
                    Math.Max(startScanline, endScanline) < 0)
                {
                    // just save this position since it's outside our bounds and continue
                    activePoint = point;
                    return;
                }

                // render the line
                var vector = new VectorF(point.X - activePoint.X, point.Y - activePoint.Y, point.Quadratic);
                var fringeStart = activePoint.Y - startScanline;
                var fringeEnd = point.Y - endScanline;

                if (startScanline == endScanline)
                {
                    // this is a horizontal line
                    RenderScanline(startScanline, activePoint.X, fringeStart, point.X, fringeEnd);
                }
                else if (vector.X == 0)
                {
                    // this is a vertical line
                    var x = (int)activePoint.X;
                    var xarea = (activePoint.X - x) * 2f;

                    // check if we're scanning up or down
                    var first = 1f;
                    var increment = 1;
                    if (vector.Y < 0)
                    {
                        first = 0.0f;
                        increment = -1;
                    }

                    // first cell fringe
                    var deltaY = (first - fringeStart);
                    activeArea += xarea * deltaY;
                    activeCoverage += deltaY;
                    startScanline += increment;
                    SetCurrentCell(x, startScanline);

                    // any other cells covered by the line
                    deltaY = first + first - 1f;
                    var area = xarea * deltaY;
                    while (startScanline != endScanline)
                    {
                        activeArea += area;
                        activeCoverage += deltaY;
                        startScanline += increment;
                        SetCurrentCell(x, startScanline);
                    }

                    // ending fringe
                    deltaY = fringeEnd - 1f + first;
                    activeArea += xarea * deltaY;
                    activeCoverage += deltaY;
                }
                else
                {
                    // diagonal line
                    // check if we're scanning up or down
                    var dist = (1f - fringeStart) * vector.X;
                    var first = 1f;
                    var increment = 1;
                    if (vector.Y < 0)
                    {
                        dist = fringeStart * vector.X;
                        first = 0.0f;
                        increment = -1;
                        vector = new VectorF(vector.X, -vector.Y, vector.Quadratic);
                    }

                    // render the first scanline
                    var delta = dist / vector.Y;
                    var x = activePoint.X + delta;
                    RenderScanline(startScanline, activePoint.X, fringeStart, x, first);
                    startScanline += increment;
                    SetCurrentCell((int)x, startScanline);

                    // step along the line
                    if (startScanline != endScanline)
                    {
                        delta = vector.X / vector.Y;
                        while (startScanline != endScanline)
                        {
                            var x2 = x + delta;
                            RenderScanline(startScanline, x, 1f - first, x2, first);
                            x = x2;

                            startScanline += increment;
                            SetCurrentCell((int)x, startScanline);
                        }
                    }

                    // last scanline
                    RenderScanline(startScanline, x, 1f - first, point.X, fringeEnd);
                }

                activePoint = point;
            }
            void CurveTo(VectorF control, VectorF point)
            {
                var levels = curveLevels;
                var arc = bezierArc;
                arc[0] = point;
                arc[1] = control;
                arc[2] = activePoint;

                var dx = Math.Abs(arc[2].X + arc[0].X - 2 * arc[1].X);
                var dy = Math.Abs(arc[2].Y + arc[0].Y - 2 * arc[1].Y);

                if (dx < dy)
                    dx = dy;

                // short cut for small arcs
                if (dx < 0.25f)
                {
                    LineTo(arc[0]);
                    return;
                }

                int level = 0;
                do
                {
                    dx /= 4.0f;
                    level++;
                } while (dx > 0.25f);

                int top = 0;
                int i = 0;
                levels[0] = level;

                while (top >= 0)
                {
                    level = levels[top];
                    if (level > 0)
                    {
                        // split the arc
                        arc[i + 4] = arc[i + 2];
                        var b = arc[i + 1];
                        var a = new VectorF((arc[i + 2].X + b.X) / 2f, (arc[i + 2].Y + b.Y) / 2f, arc[i + 2].Quadratic);
                        arc[i + 3] = a;

                        b = new VectorF((arc[i].X + b.X) / 2f, (arc[i].Y + b.Y) / 2f, arc[i].Quadratic);
                        arc[i + 1] = b;
                        a = new VectorF((a.X + b.X) / 2f, (a.Y + b.Y) / 2f, a.Quadratic);
                        arc[i + 2] = a;
                        i += 2;
                        top++;
                        levels[top] = levels[top - 1] = level - 1;
                    }
                    else
                    {
                        LineTo(arc[i]);
                        top--;
                        i -= 2;
                    }
                }
            }

            void SetCurrentCell(int x, int y)
            {
                // all cells on the left of the clipping region go to the minX - 1 position
                x = Math.Min(x, iw);
                x = Math.Max(x, -1);

                // moving to a new cell?
                if (x != cellX || y != cellY)
                {
                    if (cellActive)
                        RetireActiveCell();

                    activeArea = 0.0f;
                    activeCoverage = 0.0f;
                    cellX = x;
                    cellY = y;
                }

                cellActive = cellX < iw && cellY < ih;
            }
            void RetireActiveCell()
            {
                // cells with no coverage have nothing to do
                if (activeArea == 0.0f && activeCoverage == 0.0f)
                    return;

                // find the right spot to add or insert this cell
                var x = cellX;
                var y = cellY;
                if (y < 0)
                    y = 0;
                var cell = scanlines[y];
                if (cell == -1 || cells[cell].X > x)
                {
                    // no cells at all on this scanline yet, or the first one
                    // is already beyond our X value, so grab a new one
                    cell = GetNewCell(x, cell);
                    scanlines[y] = cell;
                    return;
                }

                while (cells[cell].X != x)
                {
                    var next = cells[cell].Next;
                    if (next == -1 || cells[next].X > x)
                    {
                        // either we reached the end of the chain  this
                        // scanline, or the next cell has a larger X
                        next = GetNewCell(x, next);
                        cells[cell].Next = next;
                        return;
                    }

                    // move to next cell
                    cell = next;
                }

                // we found a cell with identical coords, so adjust its coverage
                cells[cell].Area += activeArea;
                cells[cell].Coverage += activeCoverage;
            }
            int GetNewCell(int x, int next)
            {
                // resize our array if we've run out of room
                if (cellCount == cells.Length)
                    Array.Resize(ref cells, (int)(cells.Length * 1.5));

                var index = cellCount++;
                cells[index].X = x;
                cells[index].Next = next;
                cells[index].Area = activeArea;
                cells[index].Coverage = activeCoverage;

                return index;
            }

            public void Dispose()
            {
                scanlines = null;
                curveLevels = null;
                bezierArc = null;
                cells = null;
            }
            #endregion

            struct Cell
            {
                public int X;
                public int Next;
                public float Coverage;
                public float Area;
            }
        }
#if AllHidden
    }
#endif
}
