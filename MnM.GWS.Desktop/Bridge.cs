using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS.Desktop
{
    public static  class Bridge
    {
        public const Benchmarks.Unit Unit = Benchmarks.Unit.MilliSecond;

        public static System.Drawing.PointF[] ToPointsF(this IEnumerable<int> xyPairs, bool formultipleBezier = false)
        {
            Collection<System.Drawing.PointF> points;
            var len = xyPairs.Count();
            if (formultipleBezier)
            {
                var i = len % 3;
                if (i != 1)
                {
                    len -= i;
                    len += 1;
                }
            }
            var previous = -1;

            points = new Collection<System.Drawing.PointF>(len);
            foreach (var item in xyPairs)
            {
                if (previous == -1)
                    previous = item;
                else
                {
                    points.Add(new System.Drawing.PointF(previous, item));
                    previous = -1;
                }
            }
            return points.ToArray();
        }
        public static System.Drawing.PointF[] ToPointsF(this IEnumerable<float> xyPairs, bool formultipleBezier = false)
        {
            Collection<System.Drawing.PointF> points;
            var len = xyPairs.Count();
            var previous = -1f;

            points = new Collection<System.Drawing.PointF>(len);
            foreach (var item in xyPairs)
            {
                if (previous == -1)
                    previous = item;
                else
                {
                    points.Add(new System.Drawing.PointF(previous, item));
                    previous = -1;
                }
            }
            return points.ToArray();
        }

        public static System.Drawing.PointF[] ToPointsF(params int[] xyPairs) =>
            ToPointsF(xyPairs as IEnumerable<int>);
        public static System.Drawing.PointF[] ToPointsF(params float[] xyPairs) =>
            ToPointsF(xyPairs as IEnumerable<float>);

        public static System.Drawing.PointF[] ToPointsF(bool formultipleBezier, params int[] xyPairs) =>
            ToPointsF(xyPairs as IEnumerable<int>, formultipleBezier);
        public static System.Drawing.PointF[] ToPointsF(bool formultipleBezier, params float[] xyPairs) =>
            ToPointsF(xyPairs as IEnumerable<float>, formultipleBezier);
    }
}
