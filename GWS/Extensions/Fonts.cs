/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window


using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    public static partial class Fonts
    {
        #region MEASURE TEXT
        /// <summary>
        /// Measures specified glyphs and returns resultant glyphs and area.
        /// </summary>
        /// <param name="Glyphs">Colleciton of glyphs to measure.</param>
        /// <param name="parameters">Various text rendering parameters to assist and influence the measuring process.</param>
        /// <returns></returns>
        public static IGlyphMeasurement MeasureGlyphs
            (this IFont font, IEnumerable<IGlyph> Glyphs, IEnumerable<IParameter> parameters = null)
        {
            var lines = font.MeasureGlyphs(Glyphs, out IRectangle area, out int i, out float minHBY, parameters);
            return new GlyphMeasurement(Glyphs, i, area, lines, minHBY);
        }

        /// <summary>
        /// Measures specified text using given font and returns resultant glyphs and area.
        /// </summary>
        /// <param name="font">Font which the glyphs should be extracted from.</param>
        /// <param name="text">Text for which glyphs are to be acquired from given font.</param>
        /// <param name="command">Text command to control measuring operation.</param>
        /// <param name="maxWidth">Maximum allowed width before text must be wrapped to next line.</param>
        /// <param name="margin">A gap to be left before placing the text on the line.</param>
        /// <returns>Glyphs structure which contains information about constructed glyphs.</returns>
        public static IGlyphMeasurement MeasureText(this IFont font,  string text, IEnumerable<IParameter> parameters = null)
        {
            if (font == null || string.IsNullOrEmpty(text))
                return null;
            var glyphs = new IGlyph[(text + "").Length];

            for (int i = 0; i < glyphs.Length; i++)
                glyphs[i] = font[text[i]];

            return font.MeasureGlyphs(glyphs, parameters);
        }
        #endregion

        #region PROCESS GLYPH LINE/S
        public static bool Process(this IGlyphLine line, RenderAction action, IEnumerable<IParameter> parameters = null)
        {
            if (line == null || !line.Valid)
                return false;
            PrimitiveList<IGlyph> Glyphs = new PrimitiveList<IGlyph>(line.Count);

            #region EXTRACT PARAMETERS
            parameters.ExtractTextDrawParameters
            (
                out _,
                out IRotation Rotation,
                out IScale Scale,
                out TextCommand textCommand,
                out int maxchar,
                out IBounds container,
                out int wrapWidth
            );
            #endregion

            #region INITIALIZE VARIABLES
            float dstX = line.DrawX;
            float dstY = line.DrawY;
            bool hasRotation = Rotation != null && Rotation.HasAngle;
            bool hasCenter = hasRotation && Rotation.Centre != null;
            bool hasScale = Scale != null && Scale.HasScale;
            bool hasSkewScale = Rotation?.Skew != null && Rotation.Skew.HasScale;
            var degree = Rotation?.Angle ?? 0;
            bool hasMaxChar = maxchar > 0;
            int elipseCount = 0;
            var Center = Rotation?.Centre;

            var Rx = line.Width * .5f;
            var Ry = line.Height * .5f;

            if (hasCenter)
                Rotation.RotateDest(dstX + Rx, dstY + Ry, Rx, Ry, out dstX, out dstY);
            else if (hasRotation)
                Rotation.SetCentre(dstX + Rx, dstY + Ry);

            IGlyphRenderer renderer = null;
            int i = -1;
            #endregion

            bool ok = true;

            #region LOOP
            foreach (var item in line)
            {
                ++i;
                float ooffx = 0;
                var glyph = item;
                var c = glyph.Character;
                if (c == ' ' || c == '\r' || c == '\n')
                    continue;

                if (c == line.Dot.Character)
                    goto PROCESS_GLYPH;

                switch (textCommand)
                {
                    case TextCommand.None:
                    default:
                        break;
                    case TextCommand.CharcterOnly:
                        if (!char.IsLetter(c))
                        {
                            ooffx += glyph.Width;
                            continue;
                        }
                        break;
                    case TextCommand.NumberOnly:
                        if (!char.IsDigit(c))
                        {
                            ooffx += glyph.Width;
                            continue;
                        }
                        break;
                    case TextCommand.CharacterAndNumberOnly:
                        if (!char.IsLetterOrDigit(c))
                        {
                            ooffx += glyph.Width;
                            continue;
                        }
                        break;
                    case TextCommand.SpecialCharactersOnly:
                        if (char.IsLetterOrDigit(c))
                        {
                            ooffx += glyph.Width;
                            continue;
                        }
                        break;
                    case TextCommand.CharacterAndSpecialCharactersOnly:
                        if (char.IsDigit(c))
                        {
                            ooffx += glyph.Width;
                            continue;
                        }
                        break;
                    case TextCommand.NumberAndSpecialCharactersOnly:
                        if (char.IsLetter(c))
                        {
                            ooffx += glyph.Width;
                            continue;
                        }
                        break;
                }


                PROCESS_GLYPH:
                if (hasMaxChar && i > maxchar)
                {
                    ++elipseCount;
                    if (elipseCount > 3)
                        break;
                    glyph = line.Dot;
                }
                if (hasRotation || hasScale || hasSkewScale)
                {
                    glyph = glyph.RotateAndScale(degree, Rx, Ry, Scale, Rotation?.Skew);
                }
                if (!glyph.IsProcessed)
                {
                    if (renderer == null)
                        renderer = Factory.newGlyphRenderer();
                    glyph.Refresh(renderer);
                }

                var fx = glyph.X - ooffx;
                var fy = glyph.Y;


                glyph.Offset = new Offset(fx + dstX, fy + dstY);
                Glyphs.Add(glyph);
                //ok = true;

                //var slot = ((IExGlyph)glyph).Slot;
                //var items = slot.Points.DeCompose(slot.Contours, glyph.Height).OfType<IPt>();
                //action(null, items, offset);
                //ok = action(glyph.ScanLines, null, offset);
            }
            #endregion

            renderer?.Dispose();
            action(Glyphs, null, null);
            Rotation?.SetCentre(Center?.Cx, Center?.Cy);
            return ok;
        }

        public static bool Process(this IEnumerable<IGlyphLine> lines, RenderAction action, IEnumerable<IParameter> parameters = null)
        {
            if (lines == null)
                return false;
            PrimitiveList<IGlyph> Glyphs = new PrimitiveList<IGlyph>(lines.Sum(l => l.Count));

            #region EXTRACT PARAMETERS
            parameters.ExtractTextDrawParameters
            (
                out IRenderBounds bounds,
                out IRotation Rotation,
                out IScale Scale,
                out TextCommand textCommand,
                out int maxchar,
                out _,
                out int wrapWidth
            );
            #endregion

            #region INITIALIZE VARIABLES
            bool hasRotation = Rotation != null && Rotation.HasAngle;
            bool hasCenter = hasRotation && Rotation.Centre != null;
            bool hasScale = Scale != null && Scale.HasScale;
            bool hasSkewScale = Rotation?.Skew != null && Rotation.Skew.HasScale;
            var degree = Rotation?.Angle ?? 0;
            var Center = Rotation?.Centre;

            bool hasMaxChar = maxchar > 0;
            int elipseCount = 0;
            float dstX, dstY, Rx, Ry;
            IGlyphRenderer renderer = null;
            var First = lines.First();

            int i = -1;

            Rx = First.Width * 0.5f;
            Ry = First.Height * 0.5f;

            if (hasRotation && !hasCenter)
            {
                Rotation.SetCentre(First.DrawX + Rx, First.DrawY + Ry);
                hasCenter = true;
            }
            #endregion

            bool ok = true;

            #region LOOP
            foreach (var line in lines)
            {
                dstX = line.DrawX;
                dstY = line.DrawY;

                if (hasCenter)
                    Rotation.RotateDest(dstX + Rx, dstY + Ry, Rx, Ry, out dstX, out dstY);

                foreach (var item in line)
                {
                    ++i;
                    float ooffx = 0;
                    var glyph = item;
                    var c = glyph.Character;
                    if (c == ' ' || c == '\r' || c == '\n')
                        continue;

                    if (c == line.Dot.Character)
                        goto PROCESS_GLYPH;

                    switch (textCommand)
                    {
                        case TextCommand.None:
                        default:
                            break;
                        case TextCommand.CharcterOnly:
                            if (!char.IsLetter(c))
                            {
                                ooffx += glyph.Width;
                                continue;
                            }
                            break;
                        case TextCommand.NumberOnly:
                            if (!char.IsDigit(c))
                            {
                                ooffx += glyph.Width;
                                continue;
                            }
                            break;
                        case TextCommand.CharacterAndNumberOnly:
                            if (!char.IsLetterOrDigit(c))
                            {
                                ooffx += glyph.Width;
                                continue;
                            }
                            break;
                        case TextCommand.SpecialCharactersOnly:
                            if (char.IsLetterOrDigit(c))
                            {
                                ooffx += glyph.Width;
                                continue;
                            }
                            break;
                        case TextCommand.CharacterAndSpecialCharactersOnly:
                            if (char.IsDigit(c))
                            {
                                ooffx += glyph.Width;
                                continue;
                            }
                            break;
                        case TextCommand.NumberAndSpecialCharactersOnly:
                            if (char.IsLetter(c))
                            {
                                ooffx += glyph.Width;
                                continue;
                            }
                            break;
                    }
                    PROCESS_GLYPH:
                    if (hasMaxChar && i > maxchar)
                    {
                        ++elipseCount;
                        if (elipseCount > 3)
                            break;
                        glyph = line.Dot;
                    }
                    if (hasRotation || hasScale || hasSkewScale)
                    {
                        glyph = glyph.RotateAndScale(degree, Rx, Ry, Scale, Rotation?.Skew);
                    }
                    if (!glyph.IsProcessed)
                    {
                        if (renderer == null)
                            renderer = Factory.newGlyphRenderer();
                        glyph.Refresh(renderer);
                    }

                    var fx = glyph.X - ooffx;
                    var fy = glyph.Y;

                    glyph.Offset = new Offset(fx + dstX, fy + dstY);
                    Glyphs.Add(glyph);
                }
                i = -1;
            }
            #endregion

            renderer?.Dispose();
            action(Glyphs, null, null);
            Rotation?.SetCentre(Center?.Cx, Center?.Cy);
            return ok;
        }
        #endregion
    }
}
#endif
