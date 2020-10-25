/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public static partial class Lines
    {
        #region ROTATE
        public static ILine Rotate(this ILine l, Rotation angle, bool? antiClock = null)
        {
            if (angle)
                return Factory.newLine(l.X1, l.Y1, l.X2, l.Y2, angle, antiClock);
            return Factory.newLine(l.X1, l.Y1, l.X2, l.Y2);
        }
        #endregion

        #region OFFSET & SHRINK
        public static ILine Offset(this ILine l, float offsetX, float offsetY) =>
            Factory.newLine(l.X1 + offsetX, l.Y1 + offsetY, l.X2 + offsetX, l.Y2 + offsetY);
        #endregion

        #region MOVE
        public static ILine Move(this ILine l, float move)
        {
            var p = Vectors.Move(l.X1, l.Y1, l.X2, l.Y2, move);
            return Factory.newLine(p.X, p.Y, l.X2, l.Y2);
        }
        public static ILine Extend(this ILine l, float deviation)
        {
            Vectors.Extend(l.X1, l.Y1, l.X2, l.Y2, out VectorF a, out VectorF b, deviation, true);
            return Factory.newLine(a, b);
        }
        #endregion

        #region PERPENDICULAR
        public static VectorF Perpendicular(this ILine line, VectorF point) =>
            Perpendicular(line.X1, line.Y1, line.X2, line.Y2, point.X, point.Y);

        public static ILine PerpendicularLine(this ILine line, VectorF p)
        {
            var p1 = line.Perpendicular(p);
            return Factory.newLine(p, p1);
        }
        #endregion

        #region CREATE AXIS
        public static ILine Axis(this ILine l)
        {
            Axis(l.X1, l.Y1, l.X2, l.Y2, out float x1, out float y1, out float x2, out float y2);
            return Factory.newLine(x1, y1, x2, y2);
        }
        #endregion

        #region MIRROR LINE
        public static ILine Mirror(this ILine l, bool startPointCommon = false)
        {
            var m = -l.M;
            var c = l.C;
            float nx1, ny1, nx2, ny2;
            if (startPointCommon)
            {
                ny1 = l.Y1;
                nx1 = l.X1;
                nx2 = l.X2;
                ny2 = m * nx2 + c;
            }
            else
            {
                c = l.Y2 - m * l.X2;
                ny1 = l.Y2;
                nx1 = l.X2;
                nx2 = l.X1;
                ny2 = m * nx2 + c;
            }
            return Factory.newLine(nx1, ny1, nx2, ny2);
        }
        #endregion

        #region JOIN ENDS
        public static ILine[] JoinEnds(this ILine l1, ILine l2, bool opposite = false)
        {
            if (!l2.Valid)
                return new ILine[] { Factory.newLine(l1.X1, l1.Y1, l1.X2, l1.Y2) };

            var v1 = new VectorF(l1.X1, l1.Y1);
            var v2 = new VectorF(l1.X2, l1.Y2);
            var v3 = new VectorF(l2.X1, l2.Y1);
            var v4 = new VectorF(l2.X2, l2.Y2);
            var distance1 = v1.Distance(v3);
            var distance2 = v1.Distance(v4);
           
            if (opposite)
            {
                if (distance1 < distance2)
                    Numbers.Swap(ref v3, ref v4);
            }
            else
            {
                if(distance1> distance2)
                    Numbers.Swap(ref v3, ref v4);
            }
            return new ILine[] { Factory.newLine(v1, v3), Factory.newLine(v2, v4) };
        }
        #endregion

        #region STROKE SIDES
        public static IList<ILine> StrokedSides(this ILine line, float stroke, Rotation angle = default(Rotation), bool expandCentrally = false)
        {
            if (!line.Valid)
            {
                return new ILine[0];
            }
            ILine first, second;

            if (!expandCentrally)
            {
                first = Factory.newLine(line);
                second = Factory.newLine(first, stroke);
            }
            else
            {
                first = Factory.newLine(line, -stroke / 2f);
                second = Factory.newLine(line, stroke / 2f);
            }
            if (angle)
            {
                first = first.Rotate(angle);
                second = second.Rotate(angle);
            }

            var third = Factory.newLine(first.X1, first.Y1, second.X1, second.Y1);
            var fourth = Factory.newLine(first.X2, first.Y2, second.X2, second.Y2);
            return new ILine[] { first, second, third, fourth };
        }
        #endregion

        #region PROCESS LINE
        /// <summary>
        /// Processes a thick line using standard line algorithm between two points specified by x1, y1 and x2, y2 using specified action and stroke.
        /// </summary>
        /// <param name="p1">Start point of line segment./param>
        /// <param name="p2">End point of line segment. corordinate of start point</param>
        /// <param name="action">A FillAction delegate which has routine to do something with the information emerges by using standard line algorithm</param>
        /// <returns>True if segment is valid and processed otherwise false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ProcessLine(VectorF p1, VectorF p2, PixelAction<float> action, LineCommand lineCommand) =>
            Renderer.ProcessLine(p1.X, p1.Y, p2.X, p2.Y, action, lineCommand);
        #endregion
    }
}
