/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if GWS || Window
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public static partial class Fonts
    {
        #region MEASURE TEXT
        public static void MeasureText(this IFont font, string text, float destX, float destY, 
            out RectangleF Area, out IList<IGlyph> ResultGlyphs, out float minHBY, TextDrawStyle drawStyle = null)
        {
            Area = RectangleF.Empty;
            minHBY = 0;
            ResultGlyphs = null;

            if (font == null || string.IsNullOrEmpty(text))
                return;

            IList<IGlyph> Glyphs = new IGlyph[(text + "").Length];

            for (int i = 0; i < Glyphs.Count; i++)
                Glyphs[i] = font.GetGlyph(text[i]);

            MeasureGlyphs(font, Glyphs, destX, destY, out Area,  out ResultGlyphs, out minHBY, drawStyle);
        }
    
        public static void MeasureGlyphs(this IFont font, IList<IGlyph> Glyphs, float destX, float destY, 
            out RectangleF Area, out IList<IGlyph> ResultGlyphs, out float minHBY,  TextDrawStyle drawStyle = null)
        {
            Area = RectangleF.Empty;
            minHBY = 0;
            ResultGlyphs = null;

            if (font == null)
                return;

            var DrawStyle = drawStyle ?? new TextDrawStyle();
            DrawStyle.LineHeight = font.Info.LineHeight.Ceiling();

            float lineHeight = DrawStyle.LineHeight;

            float newx, newy, right, bottom, minX, minY, kerning;

            for (int i = 0; i < Glyphs.Count; i++)
            {
                Glyphs[i].X = 0;
                Glyphs[i].Y = 0;
            }
            minHBY = Glyphs.Min(g => g.Bounds.Y);
            if (minHBY < 0)
                destY += -minHBY;

            if (Glyphs[0].Bounds.X < 0)
                destX += -Glyphs[0].Bounds.X;

            newx = right = minX = destX;
            newy = bottom = minY = destY;

            bool start = true;
            kerning = 0;

            for (int i = 0; i < Glyphs.Count; i++)
            {
                IGlyph g = Glyphs[i];
                lineHeight = Math.Max(lineHeight, g.Bounds.Height);

                if (IsSpace(Glyphs, i))
                {
                    switch (DrawStyle.Breaker)
                    {
                        case TextBreaker.None:
                        case TextBreaker.Word:
                            if (DrawStyle.Delimiter == BreakDelimiter.Word)
                                goto case TextBreaker.SingleWord;
                            break;
                        case TextBreaker.Line:
                        default:
                            if (!start)
                                newx += g.Bounds.Width;
                            goto mks;
                        case TextBreaker.SingleWord:
                            newx = destX;
                            newy += lineHeight;
                            start = true;
                            break;
                    }
                }
                else if (IsCR(Glyphs, i) || IsLF(Glyphs, i))
                {
                    if (IsPreviousCR(Glyphs, i))
                        goto last;
                    else
                    {
                        newx = destX;
                        newy += lineHeight;
                        goto mks;
                    }
                }

                kerning = GetKerning(Glyphs, font, i);

            mks:
                g.X = (newx + kerning).Round();
                g.Y = newy.Round();

                minX = Math.Min(g.X, minX);
                minY = Math.Min(g.Y, minY);

                newx = g.X + g.Bounds.Width;
                right = Math.Max(newx, right);
                bottom = Math.Max(bottom, g.Y + g.Bounds.Height);
            last:
                start = false;
            }

            Area = RectangleF.FromLTRB(minX, minY, right, bottom);
            ResultGlyphs = Glyphs;
        }

        static bool IsSpace(IList<IGlyph> glyphs, int index) =>
            glyphs[index].Character == ' ';
        static bool IsCR(IList<IGlyph> glyphs, int index) =>
            glyphs[index].Character == '\r';
        static bool IsPreviousCR(IList<IGlyph> glyphs, int index)
        {
            if (index == 0)
                return false;
            return IsCR(glyphs, index - 1);
        }
        static bool IsLF(IList<IGlyph> glyphs, int index) =>
            glyphs[index].Character == '\n';
        static int GetKerning(IList<IGlyph> glyphs, IFont font, int i)
        {
            if (font == null || i == 0)
                return 0;
            var c = glyphs[i].Character;
            if (font.Kerning && i > 0 && c != 0)
                return font.GetKerning(glyphs[i - 1].Character, c);
            return 0;
        }
        #endregion

        #region FONT
        public static byte[] FontFromFile(string path)
        {
            if (!File.Exists(path))
                return null;
            var name = System.IO.Path.GetFileName(path);
            return File.ReadAllBytes(path);
        }
        #endregion
    }
#endif
}
