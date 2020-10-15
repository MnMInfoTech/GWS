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
using System.IO;

namespace MnM.GWS
{
#if AllHidden
    partial class _Factory
    {
#else
    public
#endif
        sealed partial class Font : _Font, IFont
        {
            #region VARIABLES
            const string tostr = "Name:{0}, Size:{1}, XHeight:{2}, Ascent:{3}, Descent:{4}";

            const int Unicode32 = 4, Unicode = 0, Microsoft = 3, UnicodeBmp = 1,
                UnicodeFull = 10, iShort = 0, iLong = 1;
            readonly Dictionary<string, GlyphSlot> cache = new Dictionary<string, GlyphSlot>(400);
            Interpreter interpreter;
            float size;
            int iSize;
            FontMode style;
            int dpi = 96;
            BaseGlyph[] glyphs;
            private bool IntegerPpems;
            private MetricsEntry[] hmetrics;
            private MetricsEntry[] vmetrics;
            private CharacterMap charMap;
            private KerningTable kernTable;
            private MetricsEntry verticalSynthesized;
            private int[] controlValueTable;
            private byte[] prepProgram;
            private bool kerning;
            FontInfo info;
            #endregion

            #region CONSTRUCTORS
            public Font(string path,  int fontSize)
            {
                using (Stream fontStream = System.IO.File.OpenRead(path))
                {
                    ReadFont(fontStream, fontSize);
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Font"/> class.
            /// </summary>
            /// <param name="fontStream">A stream pointing to the font file.</param>
            /// <remarks>
            /// All relevant font data is loaded into memory and retained by the FontFace object.
            /// Once the constructor finishes you are free to close the stream.
            /// </remarks>
            public Font(Stream fontStream,  int fontSize)
            {
                ReadFont(fontStream, fontSize);
            }
            #endregion

            #region PROPERTIES
            public int Dpi
            {
                get => dpi;
                set
                {
                    dpi = Math.Max(32, value);
                }
            }
            public override int Size
            {
                get => iSize;
                set
                {
                    iSize = value;
                    size = ComputePixelSize(Math.Max(3, iSize), Dpi);
                    cache.Clear();
                }
            }

            public FontMode Style => style;
            public override bool Kerning => EnableKerning && kerning;
            public override  bool EnableKerning { get; set; }
            public override IFontInfo Info => info;
            #endregion

            #region READ FONT
            void ReadFont(Stream fontStream,  int fontSize)
            {
                iSize = fontSize;
                size = ComputePixelSize(Math.Max(iSize, 3), Dpi);

                using (var reader = new DataReader(fontStream))
                {
                    var tables = SfntTables.ReadFaceHeader(reader);

                    // read head and maxp tables for font metadata and limits
                    FaceHeader head;
                    SfntTables.ReadHead(reader, tables, out head);
                    SfntTables.ReadMaxp(reader, tables, ref head);
                    var unitsPerEm = head.UnitsPerEm;
                    head.Flags |= HeadFlags.SizeDependentInstructions;
                    IntegerPpems = (head.Flags & HeadFlags.IntegerPpem) != 0;

                    // horizontal metrics header and data
                    SfntTables.SeekToTable(reader, tables, FourCC.Hhea, required: true);
                    var hMetricsHeader = SfntTables.ReadMetricsHeader(reader);
                    SfntTables.SeekToTable(reader, tables, FourCC.Hmtx, required: true);
                    hmetrics = SfntTables.ReadMetricsTable(reader, head.GlyphCount, hMetricsHeader.MetricCount);

                    // font might optionally have vertical metrics
                    if (SfntTables.SeekToTable(reader, tables, FourCC.Vhea))
                    {
                        var vMetricsHeader = SfntTables.ReadMetricsHeader(reader);

                        SfntTables.SeekToTable(reader, tables, FourCC.Vmtx, required: true);
                        vmetrics = SfntTables.ReadMetricsTable(reader, head.GlyphCount, vMetricsHeader.MetricCount);
                    }

                    // OS/2 table has even more metrics
                    var os2Data = SfntTables.ReadOS2(reader, tables);
                    var xHeight = os2Data.XHeight;
                    style = os2Data.Style;

                    // optional PostScript table has random junk  it
                    SfntTables.ReadPost(reader, tables, ref head);
                    var IsFixedWidth = head.IsFixedPitch;

                    // read character-to-glyph mapping tables and kerning table
                    charMap = CharacterMap.ReadCmap(reader, tables);
                    kernTable = KerningTable.ReadKern(reader, tables);

                    // name data
                    var names = SfntTables.ReadNames(reader, tables);
                    var Family = names.TypographicFamilyName ?? names.FamilyName;
                    var Subfamily = names.TypographicSubfamilyName ?? names.SubfamilyName;
                    var FullName = names.FullName;
                    var UniqueID = names.UniqueID;
                    var Version = names.Version;
                    var Description = names.Description;

                    // load glyphs if we have them
                    if (SfntTables.SeekToTable(reader, tables, FourCC.Glyf))
                    {
                        unsafe
                        {
                            // read  the loca table, which tells us the byte offset of each glyph
                            var loca = stackalloc uint[head.GlyphCount];
                            SfntTables.ReadLoca(reader, tables, head.IndexFormat, loca, head.GlyphCount);

                            // we need to know the length of the glyf table because of some weirdness  the loca table:
                            // if a glyph is "missing" (like a space character), then its loca[n] entry is equal to loca[n+1]
                            // if the last glyph  the set is missing, then loca[n] == glyf table length
                            SfntTables.SeekToTable(reader, tables, FourCC.Glyf);
                            var glyfOffset = reader.Position;
                            var glyfLength = tables[SfntTables.FindTable(tables, FourCC.Glyf)].Length;

                            // read  all glyphs
                            glyphs = new BaseGlyph[head.GlyphCount];
                            for (int i = 0; i < glyphs.Length; i++)
                                SfntTables.ReadGlyph(reader, i, 0, glyphs, glyfOffset, glyfLength, loca);
                        }
                    }

                    // embedded bitmaps
                    SbitTable.Read(reader, tables);

                    int cellAscent, cellDescent, lineHeight;

                    // metrics calculations: if the UseTypographicMetrics flag is set, then
                    // we should use the sTypo*** data for line height calculation
                    if (os2Data.UseTypographicMetrics)
                    {
                        // include the line gap  the ascent so that
                        // white space is distributed above the line
                        cellAscent = os2Data.TypographicAscender + os2Data.TypographicLineGap;
                        cellDescent = -os2Data.TypographicDescender;
                        lineHeight = os2Data.TypographicAscender + os2Data.TypographicLineGap - os2Data.TypographicDescender;
                    }
                    else
                    {
                        // otherwise, we need to guess at whether hhea data or os/2 data has better line spacing
                        // this is the recommended procedure based on the OS/2 spec extra notes
                        cellAscent = os2Data.WinAscent;
                        cellDescent = Math.Abs(os2Data.WinDescent);
                        lineHeight = Math.Max(
                            Math.Max(0, hMetricsHeader.LineGap) + hMetricsHeader.Ascender + Math.Abs(hMetricsHeader.Descender),
                            cellAscent + cellDescent
                        );
                    }

                    // give sane defaults for underline and strikeout data if missing
                    var underlineSize = head.UnderlineThickness != 0 ?
                        head.UnderlineThickness : (head.UnitsPerEm + 7) / 14;
                    var underlinePosition = head.UnderlinePosition != 0 ?
                        head.UnderlinePosition : -((head.UnitsPerEm + 5) / 10);
                    var strikeoutSize = os2Data.StrikeoutSize != 0 ?
                        os2Data.StrikeoutSize : underlineSize;
                    var strikeoutPosition = os2Data.StrikeoutPosition != 0 ?
                        os2Data.StrikeoutPosition : head.UnitsPerEm / 3;

                    // create some vertical metrics  case we haven't loaded any
                    verticalSynthesized = new MetricsEntry
                    {
                        FrontSideBearing = os2Data.TypographicAscender,
                        Advance = os2Data.TypographicAscender - os2Data.TypographicDescender
                    };

                    // read  global font program data
                    controlValueTable = SfntTables.ReadCvt(reader, tables);
                    prepProgram = SfntTables.ReadProgram(reader, tables, FourCC.Prep);
                    interpreter = new Interpreter(
                        head.MaxStackSize,
                        head.MaxStorageLocations,
                        head.MaxFunctionDefs,
                        head.MaxInstructionDefs,
                        head.MaxTwilightPoints
                    );

                    // the fpgm table optionally contains a program to run at initialization time
                    var fpgm = SfntTables.ReadProgram(reader, tables, FourCC.Fpgm);
                    if (fpgm != null)
                        interpreter.InitializeFunctionDefs(fpgm);


                    float num = ComputeScale(size, unitsPerEm, IntegerPpems);
                    info = new FontInfo();
                    info.Style = style;
                    info.CellAscent = cellAscent * num;
                    info.CellDescent = cellDescent * num;
                    info.LineHeight = lineHeight * num;
                    info.XHeight = xHeight * num;
                    info.UnderlineSize = underlineSize * num;
                    info.UnderlinePosition = underlinePosition * num;
                    info.StrikeoutSize = strikeoutSize * num;
                    info.StrikeoutPosition = strikeoutPosition * num;
                    info.UnitsPerEm = unitsPerEm;
                    info.FullName = FullName;
                }

                id = Info.FullName;
                kerning = kernTable != null;
            }
            #endregion

            #region GET GLYPH
            public override IGlyph GetGlyph(char character)
            {
                var key = ID + "." + size + "." + character.ToString();
                if (cache.ContainsKey(key))
                    return new Glyph(cache[key]);

                var glyphIndex = charMap.Lookup(character);
                if (glyphIndex < 0)
                    return new Glyph(new GlyphSlot(character, null, null, Info.XHeight));

                // set up the control value table
                var scale = ComputeScale(size, Info.UnitsPerEm, IntegerPpems);
                interpreter.SetControlValueTable(controlValueTable, scale, size, prepProgram);

                // get metrics
                var glyph = glyphs[glyphIndex];
                var horizontal = hmetrics[glyphIndex];
                var vtemp = vmetrics?[glyphIndex];
                if (vtemp == null)
                {
                    var synth = verticalSynthesized;
                    synth.FrontSideBearing -= glyph.MaxY;
                    vtemp = synth;
                }
                var vertical = vtemp.GetValueOrDefault();

                // build and transform the glyph
                IList<VectorF> points = new Collection<VectorF>(32);
                var contours = new Collection<int>(32);
                var transform = Matrixs.CreateScale<Matrix3x2>(scale);
                ComposeGlyphs(glyphIndex, 0, ref transform, points, contours, glyphs);

                // add phantom points; these are used to define the extents of the glyph,
                // and can be modified by hinting instructions
                var pp1 = new VectorF((glyph.MinX - horizontal.FrontSideBearing), 0);
                var pp2 = new VectorF(pp1.X + horizontal.Advance, 0);
                var pp3 = new VectorF(0, (glyph.MaxY + vertical.FrontSideBearing));
                var pp4 = new VectorF(0, pp3.Y - vertical.Advance);


                points.Add(pp1.Multiply(scale));
                points.Add(pp2.Multiply(scale));
                points.Add(pp3.Multiply(scale));
                points.Add(pp4.Multiply(scale));
               
                // hint the glyph's points
                VectorF[] Points = new VectorF[points.Count];
                points.CopyTo(Points, 0);
                var contourArray = contours.ToArray();
                if (Hinting && size > 12)
                    interpreter.HintGlyph(Points, contourArray, glyphs[glyphIndex].Instructions);

                var g = new GlyphSlot(character, Points, contourArray, Info.XHeight);

                cache.Add(key, g);
                return new Glyph(g);
            }
            #endregion

            #region GET KERNING
            public override int GetKerning(char left, char right)
            {
                if (this.kernTable == null)
                    return 0;

                int num = charMap.Lookup(left);
                int num2 = charMap.Lookup(right);

                if (num < 0 || num2 < 0)
                    return 0;

                var k = kernTable.Lookup(num, num2) * ComputeScale(size, Info.UnitsPerEm, IntegerPpems);
                return k.Round();
            }
            #endregion

            #region GLYPH COMPOSITION
            float ComputeScale(float pixelSize, float UnitsPerEm, bool integerPpems)
            {
                if (integerPpems)
                    pixelSize = (float)Math.Round(pixelSize);
                return pixelSize / UnitsPerEm;
            }
            static float ComputePixelSize(float pointSize, int dpi) =>
                pointSize * dpi / 72f;
            static void ComposeGlyphs(int glyphIndex, int startPoint, ref Matrix3x2 transform,
                IList<VectorF> basePoints, Collection<int> baseContours, BaseGlyph[] glyphTable)
            {
                var glyph = glyphTable[glyphIndex];
                var simple = glyph as SimpleGlyph;
                if (simple != null)
                {
                    foreach (var endpoint in simple.ContourEndpoints)
                        baseContours.Add(endpoint + startPoint);
                    foreach (var point in simple.Points)
                        basePoints.Add(new VectorF(point.TransformNormal(transform), point.Quadratic));
                }
                else
                {
                    // otherwise, we have a composite glyph
                    var composite = (CompositeGlyph)glyph;
                    foreach (var subglyph in composite.Subglyphs)
                    {
                        // if we have a scale, update the local transform
                        var local = transform;
                        bool haveScale = (subglyph.Flags & (CompositeGlyphFlags.HaveScale | CompositeGlyphFlags.HaveXYScale | CompositeGlyphFlags.HaveTransform)) != 0;
                        if (haveScale)
                            local = transform * subglyph.Transform;

                        // recursively compose the subglyph into our lists
                        int currentPoints = basePoints.Count;
                        ComposeGlyphs(subglyph.Index, currentPoints, ref local, basePoints, baseContours, glyphTable);

                        // calculate the offset for the subglyph. we have to do offsetting after composing all subglyphs,
                        // because we might need to find the offset based on previously composed points by index
                        VectorF offset;
                        if ((subglyph.Flags & CompositeGlyphFlags.ArgsAreXYValues) != 0)
                        {
                            offset = new VectorF(subglyph.Arg1, subglyph.Arg2);
                            if (haveScale && (subglyph.Flags & CompositeGlyphFlags.ScaledComponentOffset) != 0)
                                offset = offset.TransformNormal(local);
                            else
                                offset = offset.TransformNormal(transform);

                            // if the RoundXYToGrid flag is set, round the offset components
                            if ((subglyph.Flags & CompositeGlyphFlags.RoundXYToGrid) != 0)
                                offset = new VectorF((float)Math.Round(offset.X), (float)Math.Round(offset.Y));
                        }
                        else
                        {
                            // if the offsets are not given  FUnits, then they are point indices
                            //  the currently composed base glyph that we should match up
                            var p1 = basePoints[(int)((uint)subglyph.Arg1 + startPoint)];
                            var p2 = basePoints[(int)((uint)subglyph.Arg2 + currentPoints)];
                            offset = p1.Subtract(p2);
                        }

                        // translate all child points
                        if (offset.X != 0 && offset.Y != 0)
                        {
                            for (int i = currentPoints; i < basePoints.Count; i++)
                                basePoints[i] = basePoints[i].Offset(offset);
                        }
                    }
                }
            }
            #endregion

            #region DISPOSE
            public void Dispose() { }
            #endregion
           
            public override string ToString()
            {
                return string.Format(tostr, Info.FullName, size, Info.XHeight, Info.CellAscent, Info.CellDescent);
            }

            class FontInfo : IFontInfo
            {
                public FontMode Style { get; internal set; }
                public float UnitsPerEm { get; internal set; }
                public bool IntegerPpems { get; internal set; }
                public string FullName { get; internal set; }
                public string Description { get; internal set; }
                public float CellAscent { get; internal set; }
                public float CellDescent { get; internal set; }
                public float LineHeight { get; internal set; }
                public float XHeight { get; internal set; }
                public float UnderlineSize { get; internal set; }
                public float UnderlinePosition { get; internal set; }
                public float StrikeoutSize { get; internal set; }
                public float StrikeoutPosition { get; internal set; }
            }
        }
#if AllHidden
    }
#endif
}
