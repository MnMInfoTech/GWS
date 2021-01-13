/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

using System;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
#if GWS || Window
 public static partial class Fonts
    {
    #region MEASURE TEXT
        public static IGlyphs MeasureText(this IFont font, string text, float destX, float destY, ITextStyle drawStyle = null)
        {
            if (font == null || string.IsNullOrEmpty(text))
                return null;
            var Glyphs = new IGlyph[(text + "").Length];

            for (int i = 0; i < Glyphs.Length; i++)
                Glyphs[i] = font.GetGlyph(text[i]);

            return MeasureGlyphs(font, Glyphs, destX, destY, drawStyle);
        }
        public static IGlyphs MeasureGlyphs(this IFont font, IList<IGlyph> Glyphs, float destX, float destY, ITextStyle drawStyle = null)
        {
            if (font == null)
                return null;

            font.MeasureGlyphs(Glyphs, destX, destY, out RectangleF Area, out IList<IGlyph> ResultGlyphs, out float minHBY, drawStyle);

            return Factory.newGlyphs(default(string), Area, ResultGlyphs, minHBY);
        }
    #endregion

    #region PARSE GLYPH
        public static IList<VectorF> ParseGlyph(this IEnumerable<VectorF> Points, IList<int> Contours, int glyphHeight, int offsetX, int offsetY)
        {
            IList<VectorF> points = new Collection<VectorF>();
            VectorF Start = VectorF.Empty;
            Decompose(Points.ToArray(), Contours, ref points, ref Start);

            points = points.Select(p => p.FlipVertical(glyphHeight).Offset(offsetX, offsetY)).ToArray();
            return points;
        }

        static void MoveTo(ref VectorF Start, VectorF p)
        {
            Start = p;
        }

        static void LineTo<T>(ref T points, ref VectorF Start, VectorF End) where T : ICollection<VectorF>
        {
            points.Add(Start);
            points.Add(End);
            Start = End;
        }

        static void CurveTo<T>(ref T points, ref VectorF Start, VectorF controlPoint1, VectorF controlPoint2, VectorF endPoint) where T : ICollection<VectorF>
        {
            if (!Start)
                return;
            Curves.GetBezierPoints(4, ref points, Start, controlPoint1, controlPoint2, endPoint);
            Start = endPoint;
        }
        static void CurveTo<T>(ref T points, ref VectorF Start, VectorF controlPoint1, VectorF endPoint) where T : ICollection<VectorF>
        {
            if (!Start)
                return;
            Curves.GetBezierPoints(4, ref points, Start, controlPoint1, endPoint);
            Start = endPoint;
        }
    #endregion

    #region DECOMPOSE
        static void Decompose<T>(VectorF[] Points, IList<int> Contours, ref T points, ref VectorF Start) where T: ICollection<VectorF>
        {
            var firstIndex = 0;

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
                        start = new VectorF((start.X + end.X) / 2f, (start.Y + end.Y) / 2f, start.Quadratic);
                    }
                    pointIndex--;
                }

                // let's draw this contour
                MoveTo(ref Start, start);

                var needClose = true;
                while (pointIndex < lastIndex)
                {
                    var point = Points[++pointIndex];
                    switch (point.Quadratic)
                    {
                        case 0:
                        default:
                            LineTo(ref points, ref Start, point);
                            break;

                        case 1:
                            VectorF control = point;
                            var done = false;
                            while (pointIndex < lastIndex)
                            {
                                var next = Points[++pointIndex];
                                if (next.Quadratic == 0)
                                {
                                    CurveTo(ref points, ref Start, control, next);
                                    done = true;
                                    break;
                                }

                                if (next.Quadratic == 0)
                                    throw new Exception("Bad outline data.");
                                var p = new VectorF((control.X + next.X) / 2f, (control.Y + next.Y) / 2f, control.Quadratic);
                                CurveTo(ref points, ref Start, control, p);
                                control = next;
                            }

                            if (!done)
                            {
                                // if we hit this point, we're ready to close out the contour
                                CurveTo(ref points, ref Start, control, start);
                                needClose = false;
                            }
                            break;
                    }
                }

                if (needClose)
                    LineTo(ref points, ref Start, start);
                // next contour starts where this one left off
                firstIndex = lastIndex + 1;
            }
        }

    #endregion
}
#endif
}
