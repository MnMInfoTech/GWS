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
// Author: Manan Adhvaryu.
#if (GWS || Window)
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace MnM.GWS
{
    partial class Factory
    {
        partial class Font
        {
            #region VARIABLES
            Interpreter interpreter;
            BaseGlyph[] glyphs;
            bool IntegerPpems;
            MetricsEntry[] hmetrics;
            MetricsEntry[] vmetrics;
            CharacterMap charMap;
            KerningTable kernTable;
            MetricsEntry verticalSynthesized;
            int[] controlValueTable;
            byte[] prepProgram;
            const int Unicode32 = 4, Unicode = 0, Microsoft = 3, UnicodeBmp = 1,
                UnicodeFull = 10, iShort = 0, iLong = 1;
            #endregion

            #region GLYPH COMPOSITION
            float ComputeScale(float pixelSize, float UnitsPerEm, bool integerPpems)
            {
                if (integerPpems)
                    pixelSize = (float)Math.Round(pixelSize);
                return pixelSize / UnitsPerEm;
            }
            static float ComputePixelSize(float pointSize, int dpi) =>
                pointSize * dpi / 64f;
            static void ComposeGlyphs(int glyphIndex, int startPoint, ref Matrix3x2 transform,
                IList<VectorF> basePoints, PrimitiveList<int> baseContours, BaseGlyph[] glyphTable)
            {
                var glyph = glyphTable[glyphIndex];
                var simple = glyph as SimpleGlyph;
                if (simple != null)
                {
                    foreach (var endpoint in simple.ContourEndpoints)
                        baseContours.Add(endpoint + startPoint);
                    foreach (var point in simple.Points)
                        basePoints.Add(new VectorF(point.TransformNormal(transform), point.Kind));
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

            #region READ FONT
            void ReadFont(Stream fontStream, int fontSize)
            {
                iSize = fontSize;
                size = ComputePixelSize(Math.Max(iSize, 3), Application.DPI);

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
                kerning = kernTable != null;

                this.lineHeight = this['I'].Height;
                this.spaceWidth = this[' '].Width;
            }
            #endregion

            #region SBIT TABLE
            class SbitTable
            {
                public unsafe static SbitTable Read(DataReader reader, TableRecord[] tables)
                {
                    if (!SfntTables.SeekToTable(reader, tables, FourCC.Eblc))
                        return null;

                    // skip version
                    var baseOffset = reader.Position;
                    reader.Skip(sizeof(int));

                    // load each strike table
                    var count = reader.ReadInt32BE();
                    if (count > MaxBitmapStrikes)
                        throw new Exception("Too many bitmap strikes  font.");

                    var sizeTableHeaders = stackalloc BitmapSizeTable[count];
                    for (int i = 0; i < count; i++)
                    {
                        sizeTableHeaders[i].SubTableOffset = reader.ReadUInt32BE();
                        sizeTableHeaders[i].SubTableSize = reader.ReadUInt32BE();
                        sizeTableHeaders[i].SubTableCount = reader.ReadUInt32BE();

                        // skip colourRef, metrics entries, start and end glyph indices
                        reader.Skip(sizeof(uint) + sizeof(ushort) * 2 + 12 * 2);

                        sizeTableHeaders[i].PpemX = reader.ReadByte();
                        sizeTableHeaders[i].PpemY = reader.ReadByte();
                        sizeTableHeaders[i].BitDepth = reader.ReadByte();
                        sizeTableHeaders[i].Flags = (BitmapSizeFlags)reader.ReadByte();
                    }

                    // read index subtables
                    var indexSubTables = stackalloc IndexSubTable[count];
                    for (int i = 0; i < count; i++)
                    {
                        reader.Seek(baseOffset + sizeTableHeaders[i].SubTableOffset);
                        indexSubTables[i] = new IndexSubTable
                        {
                            FirstGlyph = reader.ReadUInt16BE(),
                            LastGlyph = reader.ReadUInt16BE(),
                            Offset = reader.ReadUInt32BE()
                        };
                    }

                    // read the actual data for each strike table
                    for (int i = 0; i < count; i++)
                    {
                        // read the subtable header
                        reader.Seek(baseOffset + sizeTableHeaders[i].SubTableOffset + indexSubTables[i].Offset);
                        var indexFormat = reader.ReadUInt16BE();
                        var imageFormat = reader.ReadUInt16BE();
                        var imageDataOffset = reader.ReadUInt32BE();


                    }

                    return null;
                }

                struct BitmapSizeTable
                {
                    public uint SubTableOffset;
                    public uint SubTableSize;
                    public uint SubTableCount;
                    public byte PpemX;
                    public byte PpemY;
                    public byte BitDepth;
                    public BitmapSizeFlags Flags;
                }

                struct IndexSubTable
                {
                    public ushort FirstGlyph;
                    public ushort LastGlyph;
                    public uint Offset;
                }

                [Flags]
                enum BitmapSizeFlags
                {
                    None,
                    Horizontal,
                    Vertical
                }

                const int MaxBitmapStrikes = 1024;
            }
            #endregion

            #region KERNING TABLE
            class KerningTable : ICloneable
            {
                Dictionary<uint, int> table;

                KerningTable(Dictionary<uint, int> table)
                {
                    this.table = table;
                }
                KerningTable()
                {

                }
                public object Clone()
                {
                    var k = new KerningTable();
                    k.table = new Dictionary<uint, int>(table);
                    return k;
                }
                public int Lookup(int left, int right)
                {
                    var key = ((uint)left << 16) | (uint)right;
                    int value;
                    if (table.TryGetValue(key, out value))
                        return value;
                    return 0;
                }

                public static KerningTable ReadKern(DataReader reader, TableRecord[] tables)
                {
                    // kern table is optional
                    if (!SfntTables.SeekToTable(reader, tables, FourCC.Kern))
                        return null;

                    // skip version
                    reader.Skip(sizeof(short));

                    // read each subtable and accumulate kerning values
                    var tableData = new Dictionary<uint, int>();
                    var subtableCount = reader.ReadUInt16BE();
                    for (int i = 0; i < subtableCount; i++)
                    {
                        // skip version
                        var currentOffset = reader.Position;
                        reader.Skip(sizeof(short));

                        var length = reader.ReadUInt16BE();
                        var coverage = reader.ReadUInt16BE();

                        // we (and Windows) only support Format 0 tables
                        // only care about tables with horizontal kerning data
                        var kc = (KernCoverage)coverage;
                        if ((coverage & FormatMask) == 0 && (kc & KernCoverage.Horizontal) != 0 && (kc & KernCoverage.CrossStream) == 0)
                        {
                            // read the number of entries; skip over the rest of the header
                            var entryCount = reader.ReadUInt16BE();
                            reader.Skip(sizeof(short) * 3);

                            var isMin = (kc & KernCoverage.Minimum) != 0;
                            var isOverride = (kc & KernCoverage.Override) != 0;

                            // read  each entry and accumulate its kerning data
                            for (int j = 0; j < entryCount; j++)
                            {
                                var left = reader.ReadUInt16BE();
                                var right = reader.ReadUInt16BE();
                                var value = reader.ReadInt16BE();

                                // look up the current value, if we have one; if not, start at zero
                                int current = 0;
                                var key = ((uint)left << 16) | right;
                                tableData.TryGetValue(key, out current);

                                if (isMin)
                                {
                                    if (current < value)
                                        tableData[key] = value;
                                }
                                else if (isOverride)
                                    tableData[key] = value;
                                else
                                    tableData[key] = current + value;
                            }
                        }

                        // jump to the next subtable
                        reader.Seek(currentOffset + length);
                    }

                    return new KerningTable(tableData);
                }

                const uint FormatMask = 0xFFFF0000;

                [Flags]
                enum KernCoverage
                {
                    None = 0,
                    Horizontal = 0x1,
                    Minimum = 0x2,
                    CrossStream = 0x4,
                    Override = 0x8
                }
            }
            #endregion

            #region INTERPRETER
            class Interpreter : ICloneable
            {
                GraphicsState state;
                GraphicsState cvtState;
                ExecutionStack stack;
                InstructionStream[] functions;
                InstructionStream[] instructionDefs;
                float[] controlValueTable;
                int[] storage;
                int[] contours;
                float scale;
                int ppem;
                int callStackSize;
                float fdotp;
                float roundThreshold;
                float roundPhase;
                float roundPeriod;
                Zone zp0, zp1, zp2;
                Zone points, twilight;

                public Interpreter(int maxStack, int maxStorage, int maxFunctions, int maxInstructionDefs, int maxTwilightPoints)
                {
                    stack = new ExecutionStack(maxStack);
                    storage = new int[maxStorage];
                    functions = new InstructionStream[maxFunctions];
                    instructionDefs = new InstructionStream[maxInstructionDefs > 0 ? 256 : 0];
                    state = new GraphicsState();
                    cvtState = new GraphicsState();
                    VectorF[] data = new VectorF[maxTwilightPoints];
                    twilight = new Zone(data, isTwilight: true);
                }
                Interpreter()
                {

                }
                public object Clone()
                {
                    Interpreter i = new Interpreter();
                    i.state = state;
                    i.cvtState = cvtState;
                    i.stack = stack.Clone() as ExecutionStack;
                    i.functions = functions?.ToArray();
                    i.instructionDefs = instructionDefs?.ToArray();
                    i.controlValueTable = controlValueTable?.ToArray();
                    i.storage = storage?.ToArray();
                    i.contours = contours?.ToArray();
                    i.scale = scale;
                    i.ppem = ppem;
                    i.callStackSize = callStackSize;
                    i.fdotp = fdotp;
                    i.roundThreshold = roundThreshold;
                    i.roundPhase = roundPhase;
                    i.roundPeriod = roundPhase;
                    i.zp0 = zp0;
                    i.zp1 = zp1;
                    i.zp2 = zp2;
                    i.points = points;
                    i.twilight = twilight;
                    return i;
                }
                public void InitializeFunctionDefs(byte[] instructions) => Execute(new InstructionStream(instructions), false, true);

                public void SetControlValueTable(int[] cvt, float scale, float ppem, byte[] cvProgram)
                {
                    if (this.scale == scale || cvt == null)
                        return;

                    if (controlValueTable == null)
                        controlValueTable = new float[cvt.Length];
                    for (int i = 0; i < cvt.Length; i++)
                        controlValueTable[i] = cvt[i] * scale;

                    this.scale = scale;
                    this.ppem = (int)Math.Round(ppem);
                    zp0 = zp1 = zp2 = points;
                    state.Reset();
                    stack.Clear();

                    if (cvProgram != null)
                    {
                        Execute(new InstructionStream(cvProgram), false, false);

                        // save off the CVT graphics state so that we can restore it for each glyph we hint
                        if ((state.InstructionControl & InstructionControlFlags.UseDefaultGraphicsState) != 0)
                            cvtState.Reset();
                        else
                        {
                            // always reset a few fields; copy the reset
                            cvtState = state;
                            cvtState.Freedom = VectorF.UnitX;
                            cvtState.Projection = VectorF.UnitX;
                            cvtState.DualProjection = VectorF.UnitX;
                            cvtState.RoundState = RoundMode.ToGrid;
                            cvtState.Loop = 1;
                        }
                    }
                }

                public void HintGlyph(VectorF[] glyphPoints, int[] contours, byte[] instructions)
                {
                    if (instructions == null || instructions.Length == 0)
                        return;

                    // check if the CVT program disabled hinting
                    if ((state.InstructionControl & InstructionControlFlags.InhibitGridFitting) != 0)
                        return;

                    // TODO: composite glyphs
                    // TODO: round the phantom points?

                    // save contours and points
                    this.contours = contours;
                    zp0 = zp1 = zp2 = points = new Zone(glyphPoints, isTwilight: false);

                    // reset all of our shared state
                    state = cvtState;
                    callStackSize = 0;
                    debugList.Clear();
                    stack.Clear();
                    OnVectorsUpdated();

                    // normalize the round state settings
                    switch (state.RoundState)
                    {
                        case RoundMode.Super: SetSuperRound(1.0f); break;
                        case RoundMode.Super45: SetSuperRound(Sqrt2Over2); break;
                    }

                    Execute(new InstructionStream(instructions), false, false);
                }

                public void HintGlyph(IList<VectorF> glyphPoints, int[] contours, byte[] instructions)
                {
                    HintGlyph(glyphPoints.Select(x => new VectorF(x)).ToArray(), contours, instructions);
                }
                PrimitiveList<OpCode> debugList = new PrimitiveList<OpCode>();

                void Execute(InstructionStream stream, bool inFunction, bool allowFunctionDefs)
                {
                    // dispatch each instruction  the stream
                    while (!stream.Done)
                    {
                        var opcode = stream.NextOpCode();
                        debugList.Add(opcode);
                        switch (opcode)
                        {
                            // ==== PUSH INSTRUCTIONS ====
                            case OpCode.NPUSHB:
                            case OpCode.PUSHB1:
                            case OpCode.PUSHB2:
                            case OpCode.PUSHB3:
                            case OpCode.PUSHB4:
                            case OpCode.PUSHB5:
                            case OpCode.PUSHB6:
                            case OpCode.PUSHB7:
                            case OpCode.PUSHB8:
                                {
                                    var count = opcode == OpCode.NPUSHB ? stream.NextByte() : opcode - OpCode.PUSHB1 + 1;
                                    for (int i = 0; i < count; i++)
                                        stack.Push(stream.NextByte());
                                }
                                break;
                            case OpCode.NPUSHW:
                            case OpCode.PUSHW1:
                            case OpCode.PUSHW2:
                            case OpCode.PUSHW3:
                            case OpCode.PUSHW4:
                            case OpCode.PUSHW5:
                            case OpCode.PUSHW6:
                            case OpCode.PUSHW7:
                            case OpCode.PUSHW8:
                                {
                                    var count = opcode == OpCode.NPUSHW ? stream.NextByte() : opcode - OpCode.PUSHW1 + 1;
                                    for (int i = 0; i < count; i++)
                                        stack.Push(stream.NextWord());
                                }
                                break;

                            // ==== STORAGE MANAGEMENT ====
                            case OpCode.RS:
                                {
                                    var loc = CheckIndex(stack.Pop(), storage.Length);
                                    stack.Push(storage[loc]);
                                }
                                break;
                            case OpCode.WS:
                                {
                                    var value = stack.Pop();
                                    var loc = CheckIndex(stack.Pop(), storage.Length);
                                    storage[loc] = value;
                                }
                                break;

                            // ==== CONTROL VALUE TABLE ====
                            case OpCode.WCVTP:
                                {
                                    var value = stack.PopFloat();
                                    var loc = CheckIndex(stack.Pop(), controlValueTable.Length);
                                    controlValueTable[loc] = value;
                                }
                                break;
                            case OpCode.WCVTF:
                                {
                                    var value = stack.Pop();
                                    var loc = CheckIndex(stack.Pop(), controlValueTable.Length);
                                    controlValueTable[loc] = value * scale;
                                }
                                break;
                            case OpCode.RCVT: stack.Push(ReadCvt()); break;

                            // ==== STATE VECTORS ====
                            case OpCode.SVTCA0:
                            case OpCode.SVTCA1:
                                {
                                    var axis = opcode - OpCode.SVTCA0;
                                    SetFreedomVectorToAxis(axis);
                                    SetProjectionVectorToAxis(axis);
                                }
                                break;
                            case OpCode.SFVTPV: state.Freedom = state.Projection; OnVectorsUpdated(); break;
                            case OpCode.SPVTCA0:
                            case OpCode.SPVTCA1: SetProjectionVectorToAxis(opcode - OpCode.SPVTCA0); break;
                            case OpCode.SFVTCA0:
                            case OpCode.SFVTCA1: SetFreedomVectorToAxis(opcode - OpCode.SFVTCA0); break;
                            case OpCode.SPVTL0:
                            case OpCode.SPVTL1:
                            case OpCode.SFVTL0:
                            case OpCode.SFVTL1: SetVectorToLine(opcode - OpCode.SPVTL0, false); break;
                            case OpCode.SDPVTL0:
                            case OpCode.SDPVTL1: SetVectorToLine(opcode - OpCode.SDPVTL0, true); break;
                            case OpCode.SPVFS:
                            case OpCode.SFVFS:
                                {
                                    var y = stack.Pop();
                                    var x = stack.Pop();
                                    var vec = new VectorF(F2Dot14ToFloat(x), F2Dot14ToFloat(y)).Normalize();
                                    if (opcode == OpCode.SFVFS)
                                        state.Freedom = vec;
                                    else
                                    {
                                        state.Projection = vec;
                                        state.DualProjection = vec;
                                    }
                                    OnVectorsUpdated();
                                }
                                break;
                            case OpCode.GPV:
                            case OpCode.GFV:
                                {
                                    var vec = opcode == OpCode.GPV ? state.Projection : state.Freedom;
                                    stack.Push(FloatToF2Dot14(vec.X));
                                    stack.Push(FloatToF2Dot14(vec.Y));
                                }
                                break;

                            // ==== GRAPHICS STATE ====
                            case OpCode.SRP0: state.Rp0 = stack.Pop(); break;
                            case OpCode.SRP1: state.Rp1 = stack.Pop(); break;
                            case OpCode.SRP2: state.Rp2 = stack.Pop(); break;
                            case OpCode.SZP0: zp0 = GetZoneFromStack(); break;
                            case OpCode.SZP1: zp1 = GetZoneFromStack(); break;
                            case OpCode.SZP2: zp2 = GetZoneFromStack(); break;
                            case OpCode.SZPS: zp0 = zp1 = zp2 = GetZoneFromStack(); break;
                            case OpCode.RTHG: state.RoundState = RoundMode.ToHalfGrid; break;
                            case OpCode.RTG: state.RoundState = RoundMode.ToGrid; break;
                            case OpCode.RTDG: state.RoundState = RoundMode.ToDoubleGrid; break;
                            case OpCode.RDTG: state.RoundState = RoundMode.DownToGrid; break;
                            case OpCode.RUTG: state.RoundState = RoundMode.UpToGrid; break;
                            case OpCode.ROFF: state.RoundState = RoundMode.Off; break;
                            case OpCode.SROUND: state.RoundState = RoundMode.Super; SetSuperRound(1.0f); break;
                            case OpCode.S45ROUND: state.RoundState = RoundMode.Super45; SetSuperRound(Sqrt2Over2); break;
                            case OpCode.INSTCTRL:
                                {
                                    var selector = stack.Pop();
                                    if (selector >= 1 && selector <= 2)
                                    {
                                        // value is false if zero, otherwise shift the right bit into the flags
                                        var bit = 1 << (selector - 1);
                                        if (stack.Pop() == 0)
                                            state.InstructionControl = (InstructionControlFlags)((int)state.InstructionControl & ~bit);
                                        else
                                            state.InstructionControl = (InstructionControlFlags)((int)state.InstructionControl | bit);
                                    }
                                }
                                break;
                            case OpCode.SCANCTRL: /* instruction unspported */ stack.Pop(); break;
                            case OpCode.SCANTYPE: /* instruction unspported */ stack.Pop(); break;
                            case OpCode.SANGW: /* instruction unspported */ stack.Pop(); break;
                            case OpCode.SLOOP: state.Loop = stack.Pop(); break;
                            case OpCode.SMD: state.MinDistance = stack.PopFloat(); break;
                            case OpCode.SCVTCI: state.ControlValueCutIn = stack.PopFloat(); break;
                            case OpCode.SSWCI: state.SingleWidthCutIn = stack.PopFloat(); break;
                            case OpCode.SSW: state.SingleWidthValue = stack.Pop() * scale; break;
                            case OpCode.FLIPON: state.AutoFlip = true; break;
                            case OpCode.FLIPOFF: state.AutoFlip = false; break;
                            case OpCode.SDB: state.DeltaBase = stack.Pop(); break;
                            case OpCode.SDS: state.DeltaShift = stack.Pop(); break;

                            // ==== POINT MEASUREMENT ====
                            case OpCode.GC0: stack.Push(Project(zp2.GetCurrent(stack.Pop()))); break;
                            case OpCode.GC1: stack.Push(DualProject(zp2.GetOriginal(stack.Pop()))); break;
                            case OpCode.SCFS:
                                {
                                    var value = stack.PopFloat();
                                    var index = stack.Pop();
                                    var point = zp2.GetCurrent(index);
                                    MovePoint(zp2, index, value - Project(point));

                                    // moving twilight points moves their "original" value also
                                    if (zp2.IsTwilight)
                                        zp2.Original[index] = zp2.Current[index];
                                }
                                break;
                            case OpCode.MD0:
                                {
                                    var p1 = zp1.GetOriginal(stack.Pop());
                                    var p2 = zp0.GetOriginal(stack.Pop());
                                    stack.Push(DualProject(p2.Subtract(p1)));
                                }
                                break;
                            case OpCode.MD1:
                                {
                                    var p1 = zp1.GetCurrent(stack.Pop());
                                    var p2 = zp0.GetCurrent(stack.Pop());
                                    stack.Push(Project(p2.Subtract(p1)));
                                }
                                break;
                            case OpCode.MPS: // MPS should return point size, but we assume DPI so it's the same as pixel size
                            case OpCode.MPPEM: stack.Push(ppem); break;
                            case OpCode.AA: /* deprecated instruction */ stack.Pop(); break;

                            // ==== POINT MODIFICATION ====
                            case OpCode.FLIPPT:
                                {
                                    for (int i = 0; i < state.Loop; i++)
                                    {
                                        var index = stack.Pop();
                                        if ((points.Current[index].Kind & PointKind.Control) == PointKind.Control)
                                            points.Current[index] = new VectorF(points.Current[index], PointKind.Normal);
                                        else
                                            points.Current[index] = new VectorF(points.Current[index], PointKind.Control);
                                    }
                                    state.Loop = 1;
                                }
                                break;
                            case OpCode.FLIPRGON:
                                {
                                    var end = stack.Pop();
                                    for (int i = stack.Pop(); i <= end; i++)
                                        points.Current[i] = new VectorF(points.Current[i], PointKind.Normal);
                                }
                                break;
                            case OpCode.FLIPRGOFF:
                                {
                                    var end = stack.Pop();
                                    for (int i = stack.Pop(); i <= end; i++)
                                        points.Current[i] = new VectorF(points.Current[i], PointKind.Control);
                                }
                                break;
                            case OpCode.SHP0:
                            case OpCode.SHP1:
                                {
                                    Zone zone;
                                    int point;
                                    var displacement = ComputeDisplacement((int)opcode, out zone, out point);
                                    ShiftPoints(displacement);
                                }
                                break;
                            case OpCode.SHPIX:
                                ShiftPoints(state.Freedom.Multiply(stack.PopFloat()));
                                break;
                            case OpCode.SHC0:
                            case OpCode.SHC1:
                                {
                                    Zone zone;
                                    int point;
                                    var displacement = ComputeDisplacement((int)opcode, out zone, out point);
                                    var touch = GetTouchState();
                                    var contour = stack.Pop();
                                    var start = contour == 0 ? 0 : contours[contour - 1] + 1;
                                    var count = zp2.IsTwilight ? zp2.Current.Length : contours[contour] + 1;

                                    for (int i = start; i < count; i++)
                                    {
                                        // don't move the reference point
                                        if (zone.Current != zp2.Current || point != i)
                                        {
                                            zp2.Current[i] = zp2.Current[i].Add(displacement);
                                            zp2.TouchState[i] |= touch;
                                        }
                                    }
                                }
                                break;
                            case OpCode.SHZ0:
                            case OpCode.SHZ1:
                                {
                                    Zone zone;
                                    int point;
                                    var displacement = ComputeDisplacement((int)opcode, out zone, out point);
                                    var count = 0;
                                    if (zp2.IsTwilight)
                                        count = zp2.Current.Length;
                                    else if (contours.Length > 0)
                                        count = contours[contours.Length - 1] + 1;

                                    for (int i = 0; i < count; i++)
                                    {
                                        // don't move the reference point
                                        if (zone.Current != zp2.Current || point != i)
                                            zp2.Current[i] = zp2.Current[i].Add(displacement);
                                    }
                                }
                                break;
                            case OpCode.MIAP0:
                            case OpCode.MIAP1:
                                {
                                    var distance = ReadCvt();
                                    var pointIndex = stack.Pop();

                                    // this instruction is used  the CVT to set up twilight points with original values
                                    if (zp0.IsTwilight)
                                    {
                                        var original = state.Freedom.Multiply(distance);
                                        zp0.Original[pointIndex] = original;
                                        zp0.Current[pointIndex] = original;
                                    }

                                    // current position of the point along the projection vector
                                    var point = zp0.GetCurrent(pointIndex);
                                    var currentPos = Project(point);
                                    if (opcode == OpCode.MIAP1)
                                    {
                                        // only use the CVT if we are above the cut- point
                                        if (Math.Abs(distance - currentPos) > state.ControlValueCutIn)
                                            distance = currentPos;
                                        distance = Round(distance);
                                    }

                                    MovePoint(zp0, pointIndex, distance - currentPos);
                                    state.Rp0 = pointIndex;
                                    state.Rp1 = pointIndex;
                                }
                                break;
                            case OpCode.MDAP0:
                            case OpCode.MDAP1:
                                {
                                    var pointIndex = stack.Pop();
                                    var point = zp0.GetCurrent(pointIndex);
                                    var distance = 0.0f;
                                    if (opcode == OpCode.MDAP1)
                                    {
                                        distance = Project(point);
                                        distance = Round(distance) - distance;
                                    }

                                    MovePoint(zp0, pointIndex, distance);
                                    state.Rp0 = pointIndex;
                                    state.Rp1 = pointIndex;
                                }
                                break;
                            case OpCode.MSIRP0:
                            case OpCode.MSIRP1:
                                {
                                    var targetDistance = stack.PopFloat();
                                    var pointIndex = stack.Pop();

                                    // if we're operating on the twilight zone, initialize the points
                                    if (zp1.IsTwilight)
                                    {
                                        zp1.Original[pointIndex] =
                                            zp0.Original[state.Rp0].Add((state.Freedom.Multiply(targetDistance)).Divide(fdotp));
                                        zp1.Current[pointIndex] = zp1.Original[pointIndex];
                                    }

                                    var currentDistance = Project(zp1.GetCurrent(pointIndex).Subtract(zp0.GetCurrent(state.Rp0)));
                                    MovePoint(zp1, pointIndex, targetDistance - currentDistance);

                                    state.Rp1 = state.Rp0;
                                    state.Rp2 = pointIndex;
                                    if (opcode == OpCode.MSIRP1)
                                        state.Rp0 = pointIndex;
                                }
                                break;
                            case OpCode.IP:
                                {
                                    var originalBase = zp0.GetOriginal(state.Rp1);
                                    var currentBase = zp0.GetCurrent(state.Rp1);
                                    var originalRange = DualProject(zp1.GetOriginal(state.Rp2).Subtract(originalBase));
                                    var currentRange = Project(zp1.GetCurrent(state.Rp2).Subtract(currentBase));

                                    for (int i = 0; i < state.Loop; i++)
                                    {
                                        var pointIndex = stack.Pop();
                                        var point = zp2.GetCurrent(pointIndex);
                                        var currentDistance = Project(point.Subtract(currentBase));
                                        var originalDistance = DualProject(zp2.GetOriginal(pointIndex).Subtract(originalBase));

                                        var newDistance = 0.0f;
                                        if (originalDistance != 0.0f)
                                        {
                                            // a range of 0.0f is invalid according to the spec (would result  a div by zero)
                                            if (originalRange == 0.0f)
                                                newDistance = originalDistance;
                                            else
                                                newDistance = originalDistance * currentRange / originalRange;
                                        }

                                        MovePoint(zp2, pointIndex, newDistance - currentDistance);
                                    }
                                    state.Loop = 1;
                                }
                                break;
                            case OpCode.ALIGNRP:
                                {
                                    for (int i = 0; i < state.Loop; i++)
                                    {
                                        var pointIndex = stack.Pop();
                                        var p1 = zp1.GetCurrent(pointIndex);
                                        var p2 = zp0.GetCurrent(state.Rp0);
                                        MovePoint(zp1, pointIndex, -Project(p1.Subtract(p2)));
                                    }
                                    state.Loop = 1;
                                }
                                break;
                            case OpCode.ALIGNPTS:
                                {
                                    var p1 = stack.Pop();
                                    var p2 = stack.Pop();
                                    var distance = Project(zp0.GetCurrent(p2).Subtract(zp1.GetCurrent(p1))) / 2;
                                    MovePoint(zp1, p1, distance);
                                    MovePoint(zp0, p2, -distance);
                                }
                                break;
                            case OpCode.UTP: zp0.TouchState[stack.Pop()] &= ~GetTouchState(); break;
                            case OpCode.IUP0:
                            case OpCode.IUP1:
                                unsafe
                                {
                                    // bail if no contours (empty outline)
                                    if (contours.Length == 0)
                                        break;

                                    fixed (VectorF* currentPtr = points.Current) fixed (VectorF* originalPtr = points.Original)
                                    {
                                        // opcode controls whether we care about X or Y direction
                                        // do some pointer trickery so we can operate on the
                                        // points  a direction-agnostic manner
                                        TouchState touchMask;
                                        byte* current;
                                        byte* original;
                                        if (opcode == OpCode.IUP0)
                                        {
                                            touchMask = TouchState.Y;
                                            current = (byte*)&currentPtr->Y;
                                            original = (byte*)&originalPtr->Y;
                                        }
                                        else
                                        {
                                            touchMask = TouchState.X;
                                            current = (byte*)&currentPtr->X;
                                            original = (byte*)&originalPtr->X;
                                        }

                                        var point = 0;
                                        for (int i = 0; i < contours.Length; i++)
                                        {
                                            var endPoint = contours[i];
                                            var firstPoint = point;
                                            var firstTouched = -1;
                                            var lastTouched = -1;

                                            for (; point <= endPoint; point++)
                                            {
                                                // check whether this point has been touched
                                                if ((points.TouchState[point] & touchMask) != 0)
                                                {
                                                    // if this is the first touched point  the contour, note it and continue
                                                    if (firstTouched < 0)
                                                    {
                                                        firstTouched = point;
                                                        lastTouched = point;
                                                        continue;
                                                    }

                                                    // otherwise, interpolate all untouched points
                                                    // between this point and our last touched point
                                                    InterpolatePoints(current, original, lastTouched + 1, point - 1, lastTouched, point);
                                                    lastTouched = point;
                                                }
                                            }

                                            // check if we had any touched points at all  this contour
                                            if (firstTouched >= 0)
                                            {
                                                // there are two cases left to handle:
                                                // 1. there was only one touched point  the whole contour, in
                                                //    which case we want to shift everything relative to that one
                                                // 2. several touched points,  which case handle the gap from the
                                                //    beginning to the first touched point and the gap from the last
                                                //    touched point to the end of the contour
                                                if (lastTouched == firstTouched)
                                                {
                                                    var delta = *GetPoint(current, lastTouched) - *GetPoint(original, lastTouched);
                                                    if (delta != 0.0f)
                                                    {
                                                        for (int j = firstPoint; j < lastTouched; j++)
                                                            *GetPoint(current, j) += delta;
                                                        for (int j = lastTouched + 1; j <= endPoint; j++)
                                                            *GetPoint(current, j) += delta;
                                                    }
                                                }
                                                else
                                                {
                                                    InterpolatePoints(current, original, lastTouched + 1, endPoint, lastTouched, firstTouched);
                                                    if (firstTouched > 0)
                                                        InterpolatePoints(current, original, firstPoint, firstTouched - 1, lastTouched, firstTouched);
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case OpCode.ISECT:
                                {
                                    // move point P to the intersection of lines A and B
                                    var b1 = zp0.GetCurrent(stack.Pop());
                                    var b0 = zp0.GetCurrent(stack.Pop());
                                    var a1 = zp1.GetCurrent(stack.Pop());
                                    var a0 = zp1.GetCurrent(stack.Pop());
                                    var index = stack.Pop();

                                    // calculate intersection using determinants: https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection#Given_two_points_on_each_line
                                    var da = a0.Subtract(a1);
                                    var db = b0.Subtract(b1);
                                    var den = (da.X * db.Y) - (da.Y * db.X);
                                    if (Math.Abs(den) <= Epsilon)
                                    {
                                        // parallel lines; spec says to put the ppoint "into the middle of the two lines"
                                        zp2.Current[index] = (a0.Add(a1).Add(b0.Add(b1))).Divide(4);
                                    }
                                    else
                                    {
                                        var t = (a0.X * a1.Y) - (a0.Y * a1.X);
                                        var u = (b0.X * b1.Y) - (b0.Y * b1.X);
                                        var p = new VectorF((t * db.X) - (da.X * u), (t * db.Y) - (da.Y * u));
                                        zp2.Current[index] = p.Divide(den);
                                    }
                                    zp2.TouchState[index] = TouchState.Both;
                                }
                                break;

                            // ==== STACK MANAGEMENT ====
                            case OpCode.DUP: stack.Duplicate(); break;
                            case OpCode.POP: stack.Pop(); break;
                            case OpCode.CLEAR: stack.Clear(); break;
                            case OpCode.SWAP: stack.Swap(); break;
                            case OpCode.DEPTH: stack.Depth(); break;
                            case OpCode.CINDEX: stack.Copy(); break;
                            case OpCode.MINDEX: stack.Move(); break;
                            case OpCode.ROLL: stack.Roll(); break;

                            // ==== FLOW CONTROL ====
                            case OpCode.IF:
                                {
                                    // value is false; jump to the next else block or endif marker
                                    // otherwise, we don't have to do anything; we'll keep executing this block
                                    if (!stack.PopBool())
                                    {
                                        int indent = 1;
                                        while (indent > 0)
                                        {
                                            opcode = SkipNext(ref stream);
                                            switch (opcode)
                                            {
                                                case OpCode.IF: indent++; break;
                                                case OpCode.EIF: indent--; break;
                                                case OpCode.ELSE:
                                                    if (indent == 1)
                                                        indent = 0;
                                                    break;
                                            }
                                        }
                                    }
                                }
                                break;
                            case OpCode.ELSE:
                                {
                                    // assume we hit the true statement of some previous if block
                                    // if we had hit false, we would have jumped over this
                                    int indent = 1;
                                    while (indent > 0)
                                    {
                                        opcode = SkipNext(ref stream);
                                        switch (opcode)
                                        {
                                            case OpCode.IF: indent++; break;
                                            case OpCode.EIF: indent--; break;
                                        }
                                    }
                                }
                                break;
                            case OpCode.EIF: /* nothing to do */ break;
                            case OpCode.JROT:
                            case OpCode.JROF:
                                {
                                    if (stack.PopBool() == (opcode == OpCode.JROT))
                                        stream.Jump(stack.Pop() - 1);
                                    else
                                        stack.Pop();    // ignore the offset
                                }
                                break;
                            case OpCode.JMPR: stream.Jump(stack.Pop() - 1); break;

                            // ==== LOGICAL OPS ====
                            case OpCode.LT:
                                {
                                    var b = stack.Pop();
                                    var a = stack.Pop();
                                    stack.Push(a < b);
                                }
                                break;
                            case OpCode.LTEQ:
                                {
                                    var b = stack.Pop();
                                    var a = stack.Pop();
                                    stack.Push(a <= b);
                                }
                                break;
                            case OpCode.GT:
                                {
                                    var b = stack.Pop();
                                    var a = stack.Pop();
                                    stack.Push(a > b);
                                }
                                break;
                            case OpCode.GTEQ:
                                {
                                    var b = stack.Pop();
                                    var a = stack.Pop();
                                    stack.Push(a >= b);
                                }
                                break;
                            case OpCode.EQ:
                                {
                                    var b = stack.Pop();
                                    var a = stack.Pop();
                                    stack.Push(a == b);
                                }
                                break;
                            case OpCode.NEQ:
                                {
                                    var b = stack.Pop();
                                    var a = stack.Pop();
                                    stack.Push(a != b);
                                }
                                break;
                            case OpCode.AND:
                                {
                                    var b = stack.PopBool();
                                    var a = stack.PopBool();
                                    stack.Push(a && b);
                                }
                                break;
                            case OpCode.OR:
                                {
                                    var b = stack.PopBool();
                                    var a = stack.PopBool();
                                    stack.Push(a || b);
                                }
                                break;
                            case OpCode.NOT: stack.Push(!stack.PopBool()); break;
                            case OpCode.ODD:
                                {
                                    var value = (int)Round(stack.PopFloat());
                                    stack.Push(value % 2 != 0);
                                }
                                break;
                            case OpCode.EVEN:
                                {
                                    var value = (int)Round(stack.PopFloat());
                                    stack.Push(value % 2 == 0);
                                }
                                break;

                            // ==== ARITHMETIC ====
                            case OpCode.ADD:
                                {
                                    var b = stack.Pop();
                                    var a = stack.Pop();
                                    stack.Push(a + b);
                                }
                                break;
                            case OpCode.SUB:
                                {
                                    var b = stack.Pop();
                                    var a = stack.Pop();
                                    stack.Push(a - b);
                                }
                                break;
                            case OpCode.DIV:
                                {
                                    var b = stack.Pop();
                                    if (b == 0)
                                        throw new Exception("Division by zero.");

                                    var a = stack.Pop();
                                    var result = ((long)a << 6) / b;
                                    stack.Push((int)result);
                                }
                                break;
                            case OpCode.MUL:
                                {
                                    var b = stack.Pop();
                                    var a = stack.Pop();
                                    var result = ((long)a * b) >> 6;
                                    stack.Push((int)result);
                                }
                                break;
                            case OpCode.ABS: stack.Push(Math.Abs(stack.Pop())); break;
                            case OpCode.NEG: stack.Push(-stack.Pop()); break;
                            case OpCode.FLOOR: stack.Push(stack.Pop() & ~63); break;
                            case OpCode.CEILING: stack.Push((stack.Pop() + 63) & ~63); break;
                            case OpCode.MAX: stack.Push(Math.Max(stack.Pop(), stack.Pop())); break;
                            case OpCode.MIN: stack.Push(Math.Min(stack.Pop(), stack.Pop())); break;

                            // ==== FUNCTIONS ====
                            case OpCode.FDEF:
                                {
                                    if (!allowFunctionDefs || inFunction)
                                        throw new Exception("Can't define functions here.");

                                    functions[stack.Pop()] = stream;
                                    while (SkipNext(ref stream) != OpCode.ENDF) ;
                                }
                                break;
                            case OpCode.IDEF:
                                {
                                    if (!allowFunctionDefs || inFunction)
                                        throw new Exception("Can't define functions here.");

                                    instructionDefs[stack.Pop()] = stream;
                                    while (SkipNext(ref stream) != OpCode.ENDF) ;
                                }
                                break;
                            case OpCode.ENDF:
                                {
                                    if (!inFunction)
                                        throw new Exception("Found invalid ENDF marker outside of a function definition.");
                                    return;
                                }
                            case OpCode.CALL:
                            case OpCode.LOOPCALL:
                                {
                                    callStackSize++;
                                    if (callStackSize > MaxCallStack)
                                        throw new Exception("Stack overflow; infinite recursion?");

                                    var function = functions[stack.Pop()];
                                    var count = opcode == OpCode.LOOPCALL ? stack.Pop() : 1;
                                    for (int i = 0; i < count; i++)
                                        Execute(function, true, false);
                                    callStackSize--;
                                }
                                break;

                            // ==== ROUNDING ====
                            // we don't have "engine compensation" so the variants are unnecessary
                            case OpCode.ROUND0:
                            case OpCode.ROUND1:
                            case OpCode.ROUND2:
                            case OpCode.ROUND3:
                                stack.Push(Round(stack.PopFloat()));
                                break;
                            case OpCode.NROUND0:
                            case OpCode.NROUND1:
                            case OpCode.NROUND2:
                            case OpCode.NROUND3: break;

                            // ==== DELTA EXCEPTIONS ====
                            case OpCode.DELTAC1:
                            case OpCode.DELTAC2:
                            case OpCode.DELTAC3:
                                {
                                    var last = stack.Pop();
                                    for (int i = 1; i <= last; i++)
                                    {
                                        var cvtIndex = stack.Pop();
                                        var arg = stack.Pop();

                                        // upper 4 bits of the 8-bit arg is the relative ppem
                                        // the opcode specifies the base to add to the ppem
                                        var triggerPpem = (arg >> 4) & 0xF;
                                        triggerPpem += (opcode - OpCode.DELTAC1) * 16;
                                        triggerPpem += state.DeltaBase;

                                        // if the current ppem matches the trigger, apply the exception
                                        if (ppem == triggerPpem)
                                        {
                                            // the lower 4 bits of the arg is the amount to shift
                                            // it's encoded such that 0 isn't an allowable value (who wants to shift by 0 anyway?)
                                            var amount = (arg & 0xF) - 8;
                                            if (amount >= 0)
                                                amount++;
                                            amount *= 1 << (6 - state.DeltaShift);

                                            // update the CVT
                                            CheckIndex(cvtIndex, controlValueTable.Length);
                                            controlValueTable[cvtIndex] += F26Dot6ToFloat(amount);
                                        }
                                    }
                                }
                                break;
                            case OpCode.DELTAP1:
                            case OpCode.DELTAP2:
                            case OpCode.DELTAP3:
                                {
                                    var last = stack.Pop();
                                    for (int i = 1; i <= last; i++)
                                    {
                                        var pointIndex = stack.Pop();
                                        var arg = stack.Pop();

                                        // upper 4 bits of the 8-bit arg is the relative ppem
                                        // the opcode specifies the base to add to the ppem
                                        var triggerPpem = (arg >> 4) & 0xF;
                                        triggerPpem += state.DeltaBase;
                                        if (opcode != OpCode.DELTAP1)
                                            triggerPpem += (opcode - OpCode.DELTAP2 + 1) * 16;

                                        // if the current ppem matches the trigger, apply the exception
                                        if (ppem == triggerPpem)
                                        {
                                            // the lower 4 bits of the arg is the amount to shift
                                            // it's encoded such that 0 isn't an allowable value (who wants to shift by 0 anyway?)
                                            var amount = (arg & 0xF) - 8;
                                            if (amount >= 0)
                                                amount++;
                                            amount *= 1 << (6 - state.DeltaShift);

                                            MovePoint(zp0, pointIndex, F26Dot6ToFloat(amount));
                                        }
                                    }
                                }
                                break;

                            // ==== MISCELLANEOUS ====
                            case OpCode.DEBUG: stack.Pop(); break;
                            case OpCode.GETINFO:
                                {
                                    var selector = stack.Pop();
                                    var result = 0;
                                    if ((selector & 0x1) != 0)
                                    {
                                        // pretend we are MS Rasterizer v35
                                        result = 35;
                                    }

                                    // TODO: rotation and stretching
                                    //if ((selector & 0x2) != 0)
                                    //if ((selector & 0x4) != 0)

                                    // we're always rendering  grayscale
                                    if ((selector & 0x20) != 0)
                                        result |= 1 << 12;

                                    // TODO: ClearType flags

                                    stack.Push(result);
                                }
                                break;

                            default:
                                if (opcode >= OpCode.MIRP)
                                    MoveIndirectRelative(opcode - OpCode.MIRP);
                                else if (opcode >= OpCode.MDRP)
                                    MoveDirectRelative(opcode - OpCode.MDRP);
                                else
                                {
                                    // check if this is a runtime-defined opcode
                                    var index = (int)opcode;
                                    if (index > instructionDefs.Length || !instructionDefs[index].IsValid)
                                        throw new Exception("Unknown opcode  font program.");

                                    callStackSize++;
                                    if (callStackSize > MaxCallStack)
                                        throw new Exception("Stack overflow; infinite recursion?");

                                    Execute(instructionDefs[index], true, false);
                                    callStackSize--;
                                }
                                break;
                        }
                    }
                }

                int CheckIndex(int index, int length)
                {
                    if (index < 0 || index >= length)
                        throw new Exception();
                    return index;
                }

                float ReadCvt() => controlValueTable[CheckIndex(stack.Pop(), controlValueTable.Length)];

                void OnVectorsUpdated()
                {
                    fdotp = state.Freedom.Dot(state.Projection);
                    if (Math.Abs(fdotp) < Epsilon)
                        fdotp = 1.0f;
                }

                void SetFreedomVectorToAxis(int axis)
                {
                    state.Freedom = axis == 0 ? VectorF.UnitY : VectorF.UnitX;
                    OnVectorsUpdated();
                }

                void SetProjectionVectorToAxis(int axis)
                {
                    state.Projection = axis == 0 ? VectorF.UnitY : VectorF.UnitX;
                    state.DualProjection = state.Projection;

                    OnVectorsUpdated();
                }

                void SetVectorToLine(int mode, bool dual)
                {
                    // mode here should be as follows:
                    // 0: SPVTL0
                    // 1: SPVTL1
                    // 2: SFVTL0
                    // 3: SFVTL1
                    var index1 = stack.Pop();
                    var index2 = stack.Pop();
                    var p1 = zp2.GetCurrent(index1);
                    var p2 = zp1.GetCurrent(index2);

                    var line = p2.Subtract(p1);
                    if (line.LengthSquared() == 0)
                    {
                        // invalid; just set to whatever
                        if (mode >= 2)
                            state.Freedom = VectorF.UnitX;
                        else
                        {
                            state.Projection = VectorF.UnitX;
                            state.DualProjection = VectorF.UnitX;
                        }
                    }
                    else
                    {
                        // if mode is 1 or 3, we want a perpendicular vector
                        if ((mode & 0x1) != 0)
                            line = new VectorF(-line.Y, line.X);
                        line = line.Normalize();

                        if (mode >= 2)
                            state.Freedom = line;
                        else
                        {
                            state.Projection = line;
                            state.DualProjection = line;
                        }
                    }

                    // set the dual projection vector using original points
                    if (dual)
                    {
                        p1 = zp2.GetOriginal(index1);
                        p2 = zp2.GetOriginal(index2);
                        line = p2.Subtract(p1);

                        if (line.LengthSquared() == 0)
                            state.DualProjection = VectorF.UnitX;
                        else
                        {
                            if ((mode & 0x1) != 0)
                                line = new VectorF(-line.Y, line.X);

                            state.DualProjection = line.Normalize();
                        }
                    }

                    OnVectorsUpdated();
                }

                Zone GetZoneFromStack()
                {
                    switch (stack.Pop())
                    {
                        case 0: return twilight;
                        case 1: return points;
                        default: throw new Exception("Invalid zone pointer.");
                    }
                }

                void SetSuperRound(float period)
                {
                    // mode is a bunch of packed flags
                    // bits 7-6 are the period multiplier
                    var mode = stack.Pop();
                    switch (mode & 0xC0)
                    {
                        case 0: roundPeriod = period / 2; break;
                        case 0x40: roundPeriod = period; break;
                        case 0x80: roundPeriod = period * 2; break;
                        default: throw new Exception("Unknown rounding period multiplier.");
                    }

                    // bits 5-4 are the phase
                    switch (mode & 0x30)
                    {
                        case 0: roundPhase = 0; break;
                        case 0x10: roundPhase = roundPeriod / 4; break;
                        case 0x20: roundPhase = roundPeriod / 2; break;
                        case 0x30: roundPhase = roundPeriod * 3 / 4; break;
                    }

                    // bits 3-0 are the threshold
                    if ((mode & 0xF) == 0)
                        roundThreshold = roundPeriod - 1;
                    else
                        roundThreshold = ((mode & 0xF) - 4) * roundPeriod / 8;
                }

                void MoveIndirectRelative(int flags)
                {
                    // this instruction tries to make the current distance between a given point
                    // and the reference point rp0 be equivalent to the same distance  the original outline
                    // there are a bunch of flags that control how that distance is measured
                    var cvt = ReadCvt();
                    var pointIndex = stack.Pop();

                    if (Math.Abs(cvt - state.SingleWidthValue) < state.SingleWidthCutIn)
                    {
                        if (cvt >= 0)
                            cvt = state.SingleWidthValue;
                        else
                            cvt = -state.SingleWidthValue;
                    }

                    // if we're looking at the twilight zone we need to prepare the points there
                    var originalReference = zp0.GetOriginal(state.Rp0);
                    if (zp1.IsTwilight)
                    {
                        var initialValue = originalReference.Add(state.Freedom.Multiply(cvt));
                        zp1.Original[pointIndex] = initialValue;
                        zp1.Current[pointIndex] = initialValue;
                    }

                    var point = zp1.GetCurrent(pointIndex);
                    var originalDistance = DualProject(zp1.GetOriginal(pointIndex).Subtract(originalReference));
                    var currentDistance = Project(point.Subtract(zp0.GetCurrent(state.Rp0)));

                    if (state.AutoFlip && Math.Sign(originalDistance) != Math.Sign(cvt))
                        cvt = -cvt;

                    // if bit 2 is set, round the distance and look at the cut- value
                    var distance = cvt;
                    if ((flags & 0x4) != 0)
                    {
                        // only perform cut- tests when both points are  the same zone
                        if (zp0.IsTwilight == zp1.IsTwilight && Math.Abs(cvt - originalDistance) > state.ControlValueCutIn)
                            cvt = originalDistance;
                        distance = Round(cvt);
                    }

                    // if bit 3 is set, constrain to the minimum distance
                    if ((flags & 0x8) != 0)
                    {
                        if (originalDistance >= 0)
                            distance = Math.Max(distance, state.MinDistance);
                        else
                            distance = Math.Min(distance, -state.MinDistance);
                    }

                    // move the point
                    MovePoint(zp1, pointIndex, distance - currentDistance);
                    state.Rp1 = state.Rp0;
                    state.Rp2 = pointIndex;
                    if ((flags & 0x10) != 0)
                        state.Rp0 = pointIndex;
                }

                void MoveDirectRelative(int flags)
                {
                    // determine the original distance between the two reference points
                    var pointIndex = stack.Pop();
                    var p1 = zp0.GetOriginal(state.Rp0);
                    var p2 = zp1.GetOriginal(pointIndex);
                    var originalDistance = DualProject(p2.Subtract(p1));

                    // single width cutin test
                    if (Math.Abs(originalDistance - state.SingleWidthValue) < state.SingleWidthCutIn)
                    {
                        if (originalDistance >= 0)
                            originalDistance = state.SingleWidthValue;
                        else
                            originalDistance = -state.SingleWidthValue;
                    }

                    // if bit 2 is set, perform rounding
                    var distance = originalDistance;
                    if ((flags & 0x4) != 0)
                        distance = Round(distance);

                    // if bit 3 is set, constrain to the minimum distance
                    if ((flags & 0x8) != 0)
                    {
                        if (originalDistance >= 0)
                            distance = Math.Max(distance, state.MinDistance);
                        else
                            distance = Math.Min(distance, -state.MinDistance);
                    }

                    // move the point
                    originalDistance = Project(zp1.GetCurrent(pointIndex).Subtract(zp0.GetCurrent(state.Rp0)));
                    MovePoint(zp1, pointIndex, distance - originalDistance);
                    state.Rp1 = state.Rp0;
                    state.Rp2 = pointIndex;
                    if ((flags & 0x10) != 0)
                        state.Rp0 = pointIndex;
                }

                VectorF ComputeDisplacement(int mode, out Zone zone, out int point)
                {
                    // compute displacement of the reference point
                    if ((mode & 1) == 0)
                    {
                        zone = zp1;
                        point = state.Rp2;
                    }
                    else
                    {
                        zone = zp0;
                        point = state.Rp1;
                    }

                    var distance = Project(zone.GetCurrent(point).Subtract(zone.GetOriginal(point)));
                    return state.Freedom.Multiply(distance).Divide(fdotp);
                }

                TouchState GetTouchState()
                {
                    var touch = TouchState.None;
                    if (state.Freedom.X != 0)
                        touch = TouchState.X;
                    if (state.Freedom.Y != 0)
                        touch |= TouchState.Y;

                    return touch;
                }

                void ShiftPoints(VectorF displacement)
                {
                    var touch = GetTouchState();
                    for (int i = 0; i < state.Loop; i++)
                    {
                        var pointIndex = stack.Pop();
                        zp2.Current[pointIndex] = zp2.Current[pointIndex].Add(displacement);
                        zp2.TouchState[pointIndex] |= touch;
                    }
                    state.Loop = 1;
                }

                void MovePoint(Zone zone, int index, float distance)
                {
                    var point = zone.GetCurrent(index).Add(state.Freedom.Multiply(distance).Divide(fdotp));
                    var touch = GetTouchState();
                    zone.Current[index] = point;
                    zone.TouchState[index] |= touch;
                }

                float Round(float value)
                {
                    switch (state.RoundState)
                    {
                        case RoundMode.ToGrid:
                            return value >= 0 ? (float)(Math.Round(value)) : -(float)(Math.Round(-value));
                        case RoundMode.ToHalfGrid:
                            return value >= 0 ? (float)Math.Floor(value) + 0.5f : -((float)Math.Floor(-value) + 0.5f);
                        case RoundMode.ToDoubleGrid:
                            return value >= 0 ? (float)(Math.Round(value * 2, MidpointRounding.AwayFromZero) / 2) : -(float)(Math.Round(-value * 2, MidpointRounding.AwayFromZero) / 2);
                        case RoundMode.DownToGrid:
                            return value >= 0 ? (float)Math.Floor(value) : -(float)Math.Floor(-value);
                        case RoundMode.UpToGrid:
                            return value >= 0 ? (float)Math.Ceiling(value) : -(float)Math.Ceiling(-value);
                        case RoundMode.Super:
                        case RoundMode.Super45:
                            float result;
                            if (value >= 0)
                            {
                                result = value - roundPhase + roundThreshold;
                                result = (float)Math.Truncate(result / roundPeriod) * roundPeriod;
                                result += roundPhase;
                                if (result < 0)
                                    result = roundPhase;
                            }
                            else
                            {
                                result = -value - roundPhase + roundThreshold;
                                result = -(float)Math.Truncate(result / roundPeriod) * roundPeriod;
                                result -= roundPhase;
                                if (result > 0)
                                    result = -roundPhase;
                            }
                            return result;

                        default: return value;
                    }
                }

                float Project(VectorF point) => point.Dot(state.Projection);
                float DualProject(VectorF point) => point.Dot(state.DualProjection);

                static OpCode SkipNext(ref InstructionStream stream)
                {
                    // grab the next opcode, and if it's one of the push instructions skip over its arguments
                    var opcode = stream.NextOpCode();
                    switch (opcode)
                    {
                        case OpCode.NPUSHB:
                        case OpCode.PUSHB1:
                        case OpCode.PUSHB2:
                        case OpCode.PUSHB3:
                        case OpCode.PUSHB4:
                        case OpCode.PUSHB5:
                        case OpCode.PUSHB6:
                        case OpCode.PUSHB7:
                        case OpCode.PUSHB8:
                            {
                                var count = opcode == OpCode.NPUSHB ? stream.NextByte() : opcode - OpCode.PUSHB1 + 1;
                                for (int i = 0; i < count; i++)
                                    stream.NextByte();
                            }
                            break;
                        case OpCode.NPUSHW:
                        case OpCode.PUSHW1:
                        case OpCode.PUSHW2:
                        case OpCode.PUSHW3:
                        case OpCode.PUSHW4:
                        case OpCode.PUSHW5:
                        case OpCode.PUSHW6:
                        case OpCode.PUSHW7:
                        case OpCode.PUSHW8:
                            {
                                var count = opcode == OpCode.NPUSHW ? stream.NextByte() : opcode - OpCode.PUSHW1 + 1;
                                for (int i = 0; i < count; i++)
                                    stream.NextWord();
                            }
                            break;
                    }

                    return opcode;
                }

                static unsafe void InterpolatePoints(byte* current, byte* original, int start, int end, int ref1, int ref2)
                {
                    if (start > end)
                        return;

                    // figure out how much the two reference points
                    // have been shifted from their original positions
                    float delta1, delta2;
                    var lower = *GetPoint(original, ref1);
                    var upper = *GetPoint(original, ref2);
                    if (lower > upper)
                    {
                        var temp = lower;
                        lower = upper;
                        upper = temp;

                        delta1 = *GetPoint(current, ref2) - lower;
                        delta2 = *GetPoint(current, ref1) - upper;
                    }
                    else
                    {
                        delta1 = *GetPoint(current, ref1) - lower;
                        delta2 = *GetPoint(current, ref2) - upper;
                    }

                    var lowerCurrent = delta1 + lower;
                    var upperCurrent = delta2 + upper;
                    var scale = (upperCurrent - lowerCurrent) / (upper - lower);

                    for (int i = start; i <= end; i++)
                    {
                        // three cases: if it's to the left of the lower reference point or to
                        // the right of the upper reference point, do a shift based on that ref point.
                        // otherwise, interpolate between the two of them
                        var pos = *GetPoint(original, i);
                        if (pos <= lower)
                            pos += delta1;
                        else if (pos >= upper)
                            pos += delta2;
                        else
                            pos = lowerCurrent + (pos - lower) * scale;
                        *GetPoint(current, i) = pos;
                    }
                }

                static float F2Dot14ToFloat(int value) => (short)value / 16384.0f;
                static int FloatToF2Dot14(float value) => (int)(uint)(short)Math.Round(value * 16384.0f);
                static float F26Dot6ToFloat(int value) => value / 64.0f;
                static int FloatToF26Dot6(float value) => (int)Math.Round(value * 64.0f);

                unsafe static float* GetPoint(byte* data, int index) => (float*)(data + sizeof(VectorF) * index);

                static readonly float Sqrt2Over2 = (float)(Math.Sqrt(2) / 2);

                const int MaxCallStack = 128;
                const float Epsilon = 0.000001f;

                struct InstructionStream
                {
                    byte[] instructions;
                    int ip;

                    public bool IsValid => instructions != null;
                    public bool Done => ip >= instructions.Length;

                    public InstructionStream(byte[] instructions)
                    {
                        this.instructions = instructions;
                        ip = 0;
                    }

                    public int NextByte()
                    {
                        if (Done)
                            throw new Exception();
                        return instructions[ip++];
                    }

                    public OpCode NextOpCode() => (OpCode)NextByte();
                    public int NextWord() => (short)(ushort)(NextByte() << 8 | NextByte());
                    public void Jump(int offset) => ip += offset;
                }

                struct GraphicsState
                {
                    public VectorF Freedom;
                    public VectorF DualProjection;
                    public VectorF Projection;
                    public InstructionControlFlags InstructionControl;
                    public RoundMode RoundState;
                    public float MinDistance;
                    public float ControlValueCutIn;
                    public float SingleWidthCutIn;
                    public float SingleWidthValue;
                    public int DeltaBase;
                    public int DeltaShift;
                    public int Loop;
                    public int Rp0;
                    public int Rp1;
                    public int Rp2;
                    public bool AutoFlip;

                    public void Reset()
                    {
                        Freedom = VectorF.UnitX;
                        Projection = VectorF.UnitX;
                        DualProjection = VectorF.UnitX;
                        InstructionControl = InstructionControlFlags.None;
                        RoundState = RoundMode.ToGrid;
                        MinDistance = 1.0f;
                        ControlValueCutIn = 17.0f / 16.0f;
                        SingleWidthCutIn = 0.0f;
                        SingleWidthValue = 0.0f;
                        DeltaBase = 9;
                        DeltaShift = 3;
                        Loop = 1;
                        Rp0 = Rp1 = Rp2 = 0;
                        AutoFlip = true;
                    }
                }

                class ExecutionStack : ICloneable
                {
                    int[] s;
                    int count;

                    public ExecutionStack(int maxStack)
                    {
                        s = new int[maxStack];
                    }
                    ExecutionStack()
                    {

                    }
                    public object Clone()
                    {
                        var i = new ExecutionStack();
                        i.s = s.ToArray();
                        i.count = count;
                        return i;
                    }
                    public int Peek() => Peek(0);
                    public bool PopBool() => Pop() != 0;
                    public float PopFloat() => F26Dot6ToFloat(Pop());
                    public void Push(bool value) => Push(value ? 1 : 0);
                    public void Push(float value) => Push(FloatToF26Dot6(value));

                    public void Clear() => count = 0;
                    public void Depth() => Push(count);
                    public void Duplicate() => Push(Peek());
                    public void Copy() => Copy(Pop() - 1);
                    public void Copy(int index) => Push(Peek(index));
                    public void Move() => Move(Pop() - 1);
                    public void Roll() => Move(2);

                    public void Move(int index)
                    {
                        var val = Peek(index);
                        for (int i = count - index - 1; i < count - 1; i++)
                            s[i] = s[i + 1];
                        s[count - 1] = val;
                    }

                    public void Swap()
                    {
                        if (count < 2)
                            throw new Exception();

                        var tmp = s[count - 1];
                        s[count - 1] = s[count - 2];
                        s[count - 2] = tmp;
                    }

                    public void Push(int value)
                    {
                        if (count == s.Length)
                            throw new Exception();
                        s[count++] = value;
                    }

                    public int Pop()
                    {
                        if (count == 0)
                            throw new Exception();
                        return s[--count];
                    }

                    public int Peek(int index)
                    {
                        if (index < 0 || index >= count)
                            throw new Exception();
                        return s[count - index - 1];
                    }
                }

                struct Zone
                {
                    public VectorF[] Current;
                    public VectorF[] Original;
                    public TouchState[] TouchState;
                    public bool IsTwilight;

                    public Zone(VectorF[] points, bool isTwilight)
                    {
                        IsTwilight = isTwilight;
                        Current = points;
                        Original = (VectorF[])points.Clone();
                        TouchState = new TouchState[points.Length];
                    }
                    public VectorF GetCurrent(int index) => Current[index];
                    public VectorF GetOriginal(int index) => Original[index];
                }

                enum RoundMode
                {
                    ToHalfGrid,
                    ToGrid,
                    ToDoubleGrid,
                    DownToGrid,
                    UpToGrid,
                    Off,
                    Super,
                    Super45
                }

                [Flags]
                enum InstructionControlFlags
                {
                    None,
                    InhibitGridFitting = 0x1,
                    UseDefaultGraphicsState = 0x2
                }

                [Flags]
                enum TouchState
                {
                    None = 0,
                    X = 0x1,
                    Y = 0x2,
                    Both = X | Y
                }

                enum OpCode : byte
                {
                    SVTCA0,
                    SVTCA1,
                    SPVTCA0,
                    SPVTCA1,
                    SFVTCA0,
                    SFVTCA1,
                    SPVTL0,
                    SPVTL1,
                    SFVTL0,
                    SFVTL1,
                    SPVFS,
                    SFVFS,
                    GPV,
                    GFV,
                    SFVTPV,
                    ISECT,
                    SRP0,
                    SRP1,
                    SRP2,
                    SZP0,
                    SZP1,
                    SZP2,
                    SZPS,
                    SLOOP,
                    RTG,
                    RTHG,
                    SMD,
                    ELSE,
                    JMPR,
                    SCVTCI,
                    SSWCI,
                    SSW,
                    DUP,
                    POP,
                    CLEAR,
                    SWAP,
                    DEPTH,
                    CINDEX,
                    MINDEX,
                    ALIGNPTS,
                    /* unused: 0x28 */
                    UTP = 0x29,
                    LOOPCALL,
                    CALL,
                    FDEF,
                    ENDF,
                    MDAP0,
                    MDAP1,
                    IUP0,
                    IUP1,
                    SHP0,
                    SHP1,
                    SHC0,
                    SHC1,
                    SHZ0,
                    SHZ1,
                    SHPIX,
                    IP,
                    MSIRP0,
                    MSIRP1,
                    ALIGNRP,
                    RTDG,
                    MIAP0,
                    MIAP1,
                    NPUSHB,
                    NPUSHW,
                    WS,
                    RS,
                    WCVTP,
                    RCVT,
                    GC0,
                    GC1,
                    SCFS,
                    MD0,
                    MD1,
                    MPPEM,
                    MPS,
                    FLIPON,
                    FLIPOFF,
                    DEBUG,
                    LT,
                    LTEQ,
                    GT,
                    GTEQ,
                    EQ,
                    NEQ,
                    ODD,
                    EVEN,
                    IF,
                    EIF,
                    AND,
                    OR,
                    NOT,
                    DELTAP1,
                    SDB,
                    SDS,
                    ADD,
                    SUB,
                    DIV,
                    MUL,
                    ABS,
                    NEG,
                    FLOOR,
                    CEILING,
                    ROUND0,
                    ROUND1,
                    ROUND2,
                    ROUND3,
                    NROUND0,
                    NROUND1,
                    NROUND2,
                    NROUND3,
                    WCVTF,
                    DELTAP2,
                    DELTAP3,
                    DELTAC1,
                    DELTAC2,
                    DELTAC3,
                    SROUND,
                    S45ROUND,
                    JROT,
                    JROF,
                    ROFF,
                    /* unused: 0x7B */
                    RUTG = 0x7C,
                    RDTG,
                    SANGW,
                    AA,
                    FLIPPT,
                    FLIPRGON,
                    FLIPRGOFF,
                    /* unused: 0x83 - 0x84 */
                    SCANCTRL = 0x85,
                    SDPVTL0,
                    SDPVTL1,
                    GETINFO,
                    IDEF,
                    ROLL,
                    MAX,
                    MIN,
                    SCANTYPE,
                    INSTCTRL,
                    /* unused: 0x8F - 0xAF */
                    PUSHB1 = 0xB0,
                    PUSHB2,
                    PUSHB3,
                    PUSHB4,
                    PUSHB5,
                    PUSHB6,
                    PUSHB7,
                    PUSHB8,
                    PUSHW1,
                    PUSHW2,
                    PUSHW3,
                    PUSHW4,
                    PUSHW5,
                    PUSHW6,
                    PUSHW7,
                    PUSHW8,
                    MDRP,           // range of 32 values, 0xC0 - 0xDF,
                    MIRP = 0xE0     // range of 32 values, 0xE0 - 0xFF
                }
            }
            #endregion

            #region CHARACTER MAP
            class CharacterMap : ICloneable
            {
                Dictionary<Character, int> table;

                CharacterMap()
                {

                }
                CharacterMap(Dictionary<Character, int> table)
                {
                    this.table = table;
                }
                public object Clone()
                {
                    var cm = new CharacterMap();
                    cm.table = new Dictionary<Character, int>(table);
                    return cm;
                }
                public int Lookup(Character codePoint)
                {
                    int index;
                    if (table.TryGetValue(codePoint, out index))
                        return index;
                    return -1;
                }

                public static CharacterMap ReadCmap(DataReader reader, TableRecord[] tables)
                {
                    SfntTables.SeekToTable(reader, tables, FourCC.Cmap, required: true);

                    // skip version
                    var cmapOffset = reader.Position;
                    reader.Skip(sizeof(short));

                    // read all of the subtable headers
                    var subtableCount = reader.ReadUInt16BE();
                    var subtableHeaders = new CmapSubtableHeader[subtableCount];
                    for (int i = 0; i < subtableHeaders.Length; i++)
                    {
                        subtableHeaders[i] = new CmapSubtableHeader
                        {
                            PlatformID = reader.ReadUInt16BE(),
                            EncodingID = reader.ReadUInt16BE(),
                            Offset = reader.ReadUInt32BE()
                        };
                    }

                    // search for a "full" Unicode table first
                    var chosenSubtableOffset = 0u;
                    for (int i = 0; i < subtableHeaders.Length; i++)
                    {
                        var platform = subtableHeaders[i].PlatformID;
                        var encoding = subtableHeaders[i].EncodingID;
                        if ((platform == Microsoft && encoding == UnicodeFull) ||
                            (platform == Unicode && encoding == Unicode32))
                        {

                            chosenSubtableOffset = subtableHeaders[i].Offset;
                            break;
                        }
                    }

                    // if no full unicode table, just grab the first
                    // one that supports any flavor of Unicode
                    if (chosenSubtableOffset == 0)
                    {
                        for (int i = 0; i < subtableHeaders.Length; i++)
                        {
                            var platform = subtableHeaders[i].PlatformID;
                            var encoding = subtableHeaders[i].EncodingID;
                            if ((platform == Microsoft && encoding == UnicodeBmp) ||
                                 platform == Unicode)
                            {

                                chosenSubtableOffset = subtableHeaders[i].Offset;
                                break;
                            }
                        }
                    }

                    // no unicode support at all is an error
                    if (chosenSubtableOffset == 0)
                        throw new Exception("Font does not support Unicode.");

                    // jump to our chosen table and find out what format it's in
                    reader.Seek(cmapOffset + chosenSubtableOffset);
                    var format = reader.ReadUInt16BE();
                    switch (format)
                    {
                        case 4: return ReadCmapFormat4(reader);
                        default: throw new Exception("Unsupported cmap format.");
                    }
                }

                unsafe static CharacterMap ReadCmapFormat4(DataReader reader)
                {
                    // skip over length and language
                    reader.Skip(sizeof(short) * 2);

                    // figure out how many segments we have
                    var segmentCount = reader.ReadUInt16BE() / 2;
                    if (segmentCount > MaxSegments)
                        throw new Exception("Too many cmap segments.");

                    // skip over searchRange, entrySelector, and rangeShift
                    reader.Skip(sizeof(short) * 3);

                    // read  segment ranges
                    var endCount = stackalloc int[segmentCount];
                    for (int i = 0; i < segmentCount; i++)
                        endCount[i] = reader.ReadUInt16BE();

                    reader.Skip(sizeof(short));     // padding

                    var startCount = stackalloc int[segmentCount];
                    for (int i = 0; i < segmentCount; i++)
                        startCount[i] = reader.ReadUInt16BE();

                    var idDelta = stackalloc int[segmentCount];
                    for (int i = 0; i < segmentCount; i++)
                        idDelta[i] = reader.ReadInt16BE();

                    // build table from each segment
                    var table = new Dictionary<Character, int>();
                    for (int i = 0; i < segmentCount; i++)
                    {
                        // read the "idRangeOffset" for the current segment
                        // if nonzero, we need to jump into the glyphIdArray to figure out the mapping
                        // the layout is bizarre; see the OpenType spec for details
                        var idRangeOffset = reader.ReadUInt16BE();
                        if (idRangeOffset != 0)
                        {
                            var currentOffset = reader.Position;
                            reader.Seek(currentOffset + idRangeOffset - sizeof(ushort));

                            var end = endCount[i];
                            var delta = idDelta[i];
                            for (var codepoint = startCount[i]; codepoint <= end; codepoint++)
                            {
                                var glyphId = reader.ReadUInt16BE();
                                if (glyphId != 0)
                                {
                                    var glyphIndex = (glyphId + delta) & 0xFFFF;
                                    if (glyphIndex != 0)
                                        table.Add((Character)codepoint, glyphIndex);
                                }
                            }

                            reader.Seek(currentOffset);
                        }
                        else
                        {
                            // otherwise, do a straight iteration through the segment
                            var end = endCount[i];
                            var delta = idDelta[i];
                            for (var codepoint = startCount[i]; codepoint <= end; codepoint++)
                            {
                                var glyphIndex = (codepoint + delta) & 0xFFFF;
                                if (glyphIndex != 0)
                                    table.Add((Character)codepoint, glyphIndex);
                            }
                        }
                    }

                    return new CharacterMap(table);
                }

                const int MaxSegments = 1024;

                struct CmapSubtableHeader
                {
                    public int PlatformID;
                    public int EncodingID;
                    public uint Offset;
                }
            }
            #endregion

            #region DATA READER
            unsafe sealed class DataReader : IDisposable
            {
                readonly Stream stream;
                readonly byte[] buffer;
                readonly GCHandle handle;
                readonly byte* start;
                readonly int maxReadLength;
                int readOffset;
                int writeOffset;

                public uint Position => (uint)(stream.Position - (writeOffset - readOffset));

                public DataReader(Stream stream, int maxReadLength = 4096)
                {
                    this.stream = stream;
                    this.maxReadLength = maxReadLength;

                    buffer = new byte[maxReadLength * 2];
                    handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    start = (byte*)handle.AddrOfPinnedObject();
                }

                public void Dispose()
                {
                    if (handle.IsAllocated)
                        handle.Free();
                }

                public byte ReadByte() => *Read(1);
                public sbyte ReadSByte() => *(sbyte*)Read(1);
                public short ReadInt16() => *(short*)Read(sizeof(short));
                public int ReadInt32() => *(int*)Read(sizeof(int));
                public ushort ReadUInt16() => *(ushort*)Read(sizeof(ushort));
                public uint ReadUInt32() => *(uint*)Read(sizeof(uint));
                public short ReadInt16BE() => (short)htons(ReadUInt16());
                public int ReadInt32BE() => (int)htonl(ReadUInt32());
                public ushort ReadUInt16BE() => htons(ReadUInt16());
                public uint ReadUInt32BE() => htonl(ReadUInt32());

                public byte[] ReadBytes(int count)
                {
                    var result = new byte[count];
                    int index = 0;
                    while (count > 0)
                    {
                        var readCount = Math.Min(count, maxReadLength);
                        Marshal.Copy(new IntPtr(Read(readCount)), result, index, readCount);

                        count -= readCount;
                        index += readCount;
                    }
                    return result;
                }

                public void Seek(uint position)
                {
                    // if the position is within our buffer we can reuse part of it
                    // otherwise, just clear everything out and jump to the right spot
                    var current = stream.Position;
                    if (position < current - writeOffset || position >= current)
                    {
                        readOffset = 0;
                        writeOffset = 0;
                        stream.Position = position;
                    }
                    else
                    {
                        readOffset = (int)(position - current + writeOffset);
                        CheckWrapAround(0);
                    }
                }

                public void Skip(int count)
                {
                    readOffset += count;
                    if (readOffset < writeOffset)
                        CheckWrapAround(0);
                    else
                    {
                        // we've skipped everything  our buffer; clear it out
                        // and then skip any remaining data by seeking the stream
                        var seekCount = readOffset - writeOffset;
                        if (seekCount > 0)
                            stream.Position += seekCount;

                        readOffset = 0;
                        writeOffset = 0;
                    }
                }

                byte* Read(int count)
                {
                    // we'll be returning a pointer to a contiguous block of memory
                    // at least count bytes large, starting at the current offset
                    var result = start + readOffset;
                    readOffset += count;

                    if (readOffset >= writeOffset)
                    {
                        if (count > maxReadLength)
                            throw new InvalidOperationException("Tried to read more data than the max read length.");

                        // we need to read at least this many bytes, but we'll try for more (could be zero)
                        var need = readOffset - writeOffset;
                        while (need > 0)
                        {
                            // try to read  a chunk of maxReadLength bytes (unless that would push past the end of our space)
                            int read = stream.Read(buffer, writeOffset, Math.Min(maxReadLength, buffer.Length - writeOffset));
                            if (read <= 0)
                                throw new EndOfStreamException();

                            writeOffset += read;
                            need -= read;
                        }

                        if (CheckWrapAround(count))
                            result = start;
                    }

                    // most of the time we'll have plenty of data  the buffer
                    // so we'll fall through here and get the pointer quickly
                    return result;
                }

                bool CheckWrapAround(int dataCount)
                {
                    // if we've gone past the max read length, we can no longer ensure
                    // that future read calls of maxReadLength size will be able to get a
                    // contiguous buffer, so wrap back to the beginning
                    if (readOffset >= maxReadLength)
                    {
                        // back copy any buffered data so that it doesn't get lost
                        var copyCount = writeOffset - readOffset + dataCount;
                        if (copyCount > 0)
                            System.Buffer.BlockCopy(buffer, readOffset - dataCount, buffer, 0, copyCount);

                        readOffset = dataCount;
                        writeOffset = copyCount;
                        return true;
                    }

                    return false;
                }

                static uint htonl(uint value)
                {
                    // this branch is constant at JIT time and will be optimized out
                    if (!BitConverter.IsLittleEndian)
                        return value;

                    var ptr = (byte*)&value;
                    return (uint)(ptr[0] << 24 | ptr[1] << 16 | ptr[2] << 8 | ptr[3]);
                }

                static ushort htons(ushort value)
                {
                    // this branch is constant at JIT time and will be optimized out
                    if (!BitConverter.IsLittleEndian)
                        return value;

                    var ptr = (byte*)&value;
                    return (ushort)(ptr[0] << 8 | ptr[1]);
                }
            }
            #endregion

            #region SFNT TABLE
            unsafe static class SfntTables
            {
                public static uint[] ReadTTCHeader(DataReader reader)
                {
                    // read the file header; if we have a collection, we want to
                    // figure out where all the different faces are  the file
                    // if we don't have a collection, there's just one font  the file
                    var tag = reader.ReadUInt32();
                    if (tag != FourCC.Ttcf)
                        return new[] { 0u };

                    // font file is a TrueType collection; read the TTC header
                    reader.Skip(4);     // version number
                    var count = reader.ReadUInt32BE();
                    if (count == 0 || count > MaxFontsInCollection)
                        throw new Exception("Invalid TTC header");

                    var offsets = new uint[count];
                    for (int i = 0; i < count; i++)
                        offsets[i] = reader.ReadUInt32BE();

                    return offsets;
                }

                public static TableRecord[] ReadFaceHeader(DataReader reader)
                {
                    var tag = reader.ReadUInt32BE();
                    if (tag != TTFv1 && tag != TTFv2 && tag != FourCC.True)
                        throw new Exception("Unknown or unsupported sfnt version.");

                    var tableCount = reader.ReadUInt16BE();
                    reader.Skip(6); // skip the rest of the header

                    // read each font table descriptor
                    var tables = new TableRecord[tableCount];
                    for (int i = 0; i < tableCount; i++)
                    {
                        tables[i] = new TableRecord
                        {
                            Tag = reader.ReadUInt32(),
                            CheckSum = reader.ReadUInt32BE(),
                            Offset = reader.ReadUInt32BE(),
                            Length = reader.ReadUInt32BE(),
                        };
                    }

                    return tables;
                }

                public static void ReadHead(DataReader reader, TableRecord[] tables, out FaceHeader header)
                {
                    SeekToTable(reader, tables, FourCC.Head, required: true);

                    // 'head' table contains global information for the font face
                    // we only care about a few fields  it
                    reader.Skip(sizeof(int) * 4);   // version, revision, checksum, magic number

                    header = new FaceHeader
                    {
                        Flags = (HeadFlags)reader.ReadUInt16BE(),
                        UnitsPerEm = reader.ReadUInt16BE()
                    };
                    if (header.UnitsPerEm == 0)
                        throw new Exception("Invalid 'head' table.");

                    // skip over created and modified times, bounding box,
                    // deprecated style bits, direction hints, and size hints
                    reader.Skip(sizeof(long) * 2 + sizeof(short) * 7);

                    header.IndexFormat = reader.ReadInt16BE();
                }

                public static void ReadMaxp(DataReader reader, TableRecord[] tables, ref FaceHeader header)
                {
                    SeekToTable(reader, tables, FourCC.Maxp, required: true);

                    if (reader.ReadInt32BE() != 0x00010000)
                        throw new Exception("Font contains an old style maxp table.");

                    header.GlyphCount = reader.ReadUInt16BE();
                    if (header.GlyphCount > MaxGlyphs)
                        throw new Exception("Font contains too many glyphs.");

                    // skip maxPoints, maxContours, maxCompositePoints, maxCompositeContours, maxZones
                    reader.Skip(sizeof(short) * 5);

                    header.MaxTwilightPoints = reader.ReadUInt16BE();
                    header.MaxStorageLocations = reader.ReadUInt16BE();
                    header.MaxFunctionDefs = reader.ReadUInt16BE();
                    header.MaxInstructionDefs = reader.ReadUInt16BE();
                    header.MaxStackSize = reader.ReadUInt16BE();

                    // sanity checking
                    if (header.MaxTwilightPoints > MaxTwilightPoints || header.MaxStorageLocations > MaxStorageLocations ||
                        header.MaxFunctionDefs > MaxFunctionDefs || header.MaxInstructionDefs > MaxFunctionDefs ||
                        header.MaxStackSize > MaxStackSize)
                        throw new Exception("Font programs have limits that are larger than built- sanity checks.");
                }

                public static MetricsHeader ReadMetricsHeader(DataReader reader)
                {
                    // skip over version
                    reader.Skip(sizeof(int));

                    var header = new MetricsHeader
                    {
                        Ascender = reader.ReadInt16BE(),
                        Descender = reader.ReadInt16BE(),
                        LineGap = reader.ReadInt16BE()
                    };

                    // skip over advanceWidthMax, minLsb, minRsb, xMaxExtent, caretSlopeRise,
                    // caretSlopeRun, caretOffset, 4 reserved entries, and metricDataFormat
                    reader.Skip(sizeof(short) * 12);

                    header.MetricCount = reader.ReadUInt16BE();
                    return header;
                }

                public static MetricsEntry[] ReadMetricsTable(DataReader reader, int glyphCount, int metricCount)
                {
                    var results = new MetricsEntry[glyphCount];
                    for (int i = 0; i < metricCount; i++)
                    {
                        results[i] = new MetricsEntry
                        {
                            Advance = reader.ReadUInt16BE(),
                            FrontSideBearing = reader.ReadInt16BE()
                        };
                    }

                    // there might be an additional array of fsb-only entries
                    var extraCount = glyphCount - metricCount;
                    var lastAdvance = results[metricCount - 1].Advance;
                    for (int i = 0; i < extraCount; i++)
                    {
                        results[i + metricCount] = new MetricsEntry
                        {
                            Advance = lastAdvance,
                            FrontSideBearing = reader.ReadInt16BE()
                        };
                    }

                    return results;
                }

                public static OS2Data ReadOS2(DataReader reader, TableRecord[] tables)
                {
                    SeekToTable(reader, tables, FourCC.OS_2, required: true);

                    // skip over version, xAvgCharWidth
                    reader.Skip(sizeof(short) * 2);

                    var result = new OS2Data
                    {
                        Weight = (FontWeight)reader.ReadUInt16BE(),
                        Stretch = (FontStretch)reader.ReadUInt16BE()
                    };

                    // skip over fsType, ySubscriptXSize, ySubscriptYSize, ySubscriptXOffset, ySubscriptYOffset,
                    // ySuperscriptXSize, ySuperscriptYSize, ySuperscriptXOffset, ySuperscriptXOffset
                    reader.Skip(sizeof(short) * 9);

                    result.StrikeoutSize = reader.ReadInt16BE();
                    result.StrikeoutPosition = reader.ReadInt16BE();

                    // skip over sFamilyClass, panose[10], ulUnicodeRange1-4, achVendID[4]
                    reader.Skip(sizeof(short) + sizeof(int) * 4 + 14);

                    // check various style flags
                    var fsSelection = (FsSelectionFlags)reader.ReadUInt16BE();
                    result.Style = (fsSelection & FsSelectionFlags.Italic) != 0 ? FontMode.Italic :
                                    (fsSelection & FsSelectionFlags.Bold) != 0 ? FontMode.Bold :
                                    (fsSelection & FsSelectionFlags.Oblique) != 0 ? FontMode.Oblique :
                                    FontMode.Regular;
                    result.IsWWSFont = (fsSelection & FsSelectionFlags.WWS) != 0;
                    result.UseTypographicMetrics = (fsSelection & FsSelectionFlags.UseTypoMetrics) != 0;

                    // skip over usFirstCharIndex, usLastCharIndex
                    reader.Skip(sizeof(short) * 2);

                    result.TypographicAscender = reader.ReadInt16BE();
                    result.TypographicDescender = reader.ReadInt16BE();
                    result.TypographicLineGap = reader.ReadInt16BE();
                    result.WinAscent = reader.ReadUInt16BE();
                    result.WinDescent = reader.ReadUInt16BE();

                    // skip over ulCodePageRange1-2
                    reader.Skip(sizeof(int) * 2);

                    result.XHeight = reader.ReadInt16BE();
                    result.CapHeight = reader.ReadInt16BE();

                    return result;
                }

                public static void ReadPost(DataReader reader, TableRecord[] tables, ref FaceHeader header)
                {
                    if (!SeekToTable(reader, tables, FourCC.Post))
                        return;

                    // skip over version and italicAngle
                    reader.Skip(sizeof(int) * 2);

                    header.UnderlinePosition = reader.ReadInt16BE();
                    header.UnderlineThickness = reader.ReadInt16BE();
                    header.IsFixedPitch = reader.ReadUInt32BE() != 0;
                }

                public static void ReadLoca(DataReader reader, TableRecord[] tables, int format, uint* table, int count)
                {
                    SeekToTable(reader, tables, FourCC.Loca, required: true);

                    if (format == iShort)
                    {
                        // values are ushort, divided by 2, so we need to shift back
                        for (int i = 0; i < count; i++)
                            *table++ = (uint)(reader.ReadUInt16BE() << 1);
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                            *table++ = reader.ReadUInt32BE();
                    }
                }

                public unsafe static NameData ReadNames(DataReader reader, TableRecord[] tables)
                {
                    if (!SeekToTable(reader, tables, FourCC.Name))
                        return default(NameData);

                    // read header
                    var currentOffset = reader.Position;
                    var format = reader.ReadUInt16BE();
                    var count = reader.ReadUInt16BE();
                    var dataOffset = currentOffset + reader.ReadUInt16BE();

                    // read name records, filtering out non-Unicode and platforms we don't know about
                    var stringData = stackalloc StringData[count];
                    var stringDataCount = 0;
                    for (int i = 0; i < count; i++)
                    {
                        var platform = reader.ReadUInt16BE();
                        var encoding = reader.ReadUInt16BE();
                        var language = reader.ReadUInt16BE();
                        var name = reader.ReadUInt16BE();
                        var length = reader.ReadUInt16BE();
                        var offset = reader.ReadUInt16BE();

                        // we only support Unicode strings
                        if (platform == Microsoft)
                        {
                            if (encoding != UnicodeBmp && encoding != UnicodeFull)
                                continue;

                            //if (language != CultureInfo.CurrentCulture.LCID)
                            //    continue;
                        }
                        else if (platform != Unicode)
                            continue;

                        stringData[stringDataCount++] = new StringData
                        {
                            Name = name,
                            Offset = offset,
                            Length = length
                        };
                    }

                    // find strings we care about and extract them from the blob
                    var nameData = new NameData();
                    for (int i = 0; i < stringDataCount; i++)
                    {
                        var data = stringData[i];
                        switch (data.Name)
                        {
                            case NameID.FamilyName: nameData.FamilyName = ExtractString(reader, dataOffset, data); break;
                            case NameID.SubfamilyName: nameData.SubfamilyName = ExtractString(reader, dataOffset, data); break;
                            case NameID.UniqueID: nameData.UniqueID = ExtractString(reader, dataOffset, data); break;
                            case NameID.FullName: nameData.FullName = ExtractString(reader, dataOffset, data); break;
                            case NameID.Version: nameData.Version = ExtractString(reader, dataOffset, data); break;
                            case NameID.Description: nameData.Description = ExtractString(reader, dataOffset, data); break;
                            case NameID.TypographicFamilyName: nameData.TypographicFamilyName = ExtractString(reader, dataOffset, data); break;
                            case NameID.TypographicSubfamilyName: nameData.TypographicSubfamilyName = ExtractString(reader, dataOffset, data); break;
                        }
                    }

                    return nameData;
                }

                public static int[] ReadCvt(DataReader reader, TableRecord[] tables)
                {
                    var index = FindTable(tables, FourCC.Cvt);
                    if (index == -1)
                        return null;

                    reader.Seek(tables[index].Offset);

                    var results = new int[tables[index].Length / sizeof(short)];
                    for (int i = 0; i < results.Length; i++)
                        results[i] = reader.ReadInt16BE();

                    return results;
                }

                public static byte[] ReadProgram(DataReader reader, TableRecord[] tables, FourCC tag)
                {
                    var index = FindTable(tables, tag);
                    if (index == -1)
                        return null;

                    reader.Seek(tables[index].Offset);
                    return reader.ReadBytes((int)tables[index].Length);
                }

                public static int FindTable(TableRecord[] tables, FourCC tag)
                {
                    var index = -1;
                    for (int i = 0; i < tables.Length; i++)
                    {
                        if (tables[i].Tag == tag)
                        {
                            index = i;
                            break;
                        }
                    }

                    return index;
                }

                public static bool SeekToTable(DataReader reader, TableRecord[] tables, FourCC tag, bool required = false)
                {
                    // check if we have the desired table and that it's not empty
                    var index = FindTable(tables, tag);
                    if (index == -1 || tables[index].Length == 0)
                    {
                        if (required)
                            throw new Exception($"Missing or empty '{tag}' table.");
                        return false;
                    }

                    // seek to the appropriate offset
                    reader.Seek(tables[index].Offset);
                    return true;
                }

                public static void ReadGlyph(
                    DataReader reader, int glyphIndex, int recursionDepth,
                    BaseGlyph[] glyphTable, uint glyfOffset, uint glyfLength, uint* loca
                )
                {
                    // check if this glyph has already been loaded; this can happen
                    // if we're recursively loading subglyphs as part of a composite
                    if (glyphTable[glyphIndex] != null)
                        return;

                    // prevent bad font data from causing infinite recursion
                    if (recursionDepth > MaxRecursion)
                        throw new Exception("Bad font data; infinite composite recursion.");

                    // check if this glyph doesn't have any actual data
                    GlyphHeader header;
                    var offset = loca[glyphIndex];
                    if ((glyphIndex < glyphTable.Length - 1 && offset == loca[glyphIndex + 1]) || offset >= glyfLength)
                    {
                        // this is an empty glyph, so synthesize a header
                        header = default(GlyphHeader);
                    }
                    else
                    {
                        // seek to the right spot and load the header
                        reader.Seek(glyfOffset + loca[glyphIndex]);
                        header = new GlyphHeader
                        {
                            ContourCount = reader.ReadInt16BE(),
                            MinX = reader.ReadInt16BE(),
                            MinY = reader.ReadInt16BE(),
                            MaxX = reader.ReadInt16BE(),
                            MaxY = reader.ReadInt16BE()
                        };

                        if (header.ContourCount < -1 || header.ContourCount > MaxContours)
                            throw new Exception("Invalid number of contours for glyph.");
                    }

                    if (header.ContourCount > 0)
                    {
                        // positive contours means a simple glyph
                        glyphTable[glyphIndex] = ReadSimpleGlyph(reader, header.ContourCount);
                    }
                    else if (header.ContourCount == -1)
                    {
                        // -1 means composite glyph
                        var composite = ReadCompositeGlyph(reader);
                        var subglyphs = composite.Subglyphs;

                        // read each subglyph recrusively
                        for (int i = 0; i < subglyphs.Length; i++)
                            ReadGlyph(reader, subglyphs[i].Index, recursionDepth + 1, glyphTable, glyfOffset, glyfLength, loca);

                        glyphTable[glyphIndex] = composite;
                    }
                    else
                    {
                        // no data, so synthesize an empty glyph
                        glyphTable[glyphIndex] = new SimpleGlyph
                        {
                            Points = new VectorF[0],
                            ContourEndpoints = new int[0]
                        };
                    }

                    // save bounding box
                    var glyph = glyphTable[glyphIndex];
                    glyph.MinX = header.MinX;
                    glyph.MinY = header.MinY;
                    glyph.MaxX = header.MaxX;
                    glyph.MaxY = header.MaxY;
                }

                static SimpleGlyph ReadSimpleGlyph(DataReader reader, int contourCount)
                {
                    // read contour endpoints
                    var contours = new int[contourCount];
                    var lastEndpoint = reader.ReadUInt16BE();
                    contours[0] = lastEndpoint;
                    for (int i = 1; i < contours.Length; i++)
                    {
                        var endpoint = reader.ReadUInt16BE();
                        contours[i] = endpoint;
                        if (contours[i] <= lastEndpoint)
                            throw new Exception("Glyph contour endpoints are unordered.");

                        lastEndpoint = endpoint;
                    }

                    // the last contour's endpoint is the number of points  the glyph
                    var pointCount = lastEndpoint + 1;
                    var points = new VectorF[pointCount];

                    // read instruction data
                    var instructionLength = reader.ReadUInt16BE();
                    var instructions = reader.ReadBytes(instructionLength);

                    // read flags
                    var flags = new SimpleGlyphFlags[pointCount];
                    int flagIndex = 0;
                    while (flagIndex < flags.Length)
                    {
                        var f = (SimpleGlyphFlags)reader.ReadByte();
                        flags[flagIndex++] = f;

                        // if Repeat is set, this flag data is repeated n more times
                        if ((f & SimpleGlyphFlags.Repeat) != 0)
                        {
                            var count = reader.ReadByte();
                            for (int i = 0; i < count; i++)
                                flags[flagIndex++] = f;
                        }
                    }

                    // Read points, first doing all X coordinates and then all Y coordinates.
                    // The point packing is insane; coords are either 1 byte or 2; they're
                    // deltas from previous point, and flags let you repeat identical points.
                    var x = 0;
                    for (int i = 0; i < points.Length; i++)
                    {
                        var f = flags[i];
                        var delta = 0;

                        if ((f & SimpleGlyphFlags.ShortX) != 0)
                        {
                            delta = reader.ReadByte();
                            if ((f & SimpleGlyphFlags.SameX) == 0)
                                delta = -delta;
                        }
                        else if ((f & SimpleGlyphFlags.SameX) == 0)
                            delta = reader.ReadInt16BE();

                        x += delta;
                        points[i] = new VectorF(x, points[i].Y, points[i].Kind);
                    }

                    var y = 0;
                    for (int i = 0; i < points.Length; i++)
                    {
                        var f = flags[i];
                        var delta = 0;

                        if ((f & SimpleGlyphFlags.ShortY) != 0)
                        {
                            delta = reader.ReadByte();
                            if ((f & SimpleGlyphFlags.SameY) == 0)
                                delta = -delta;
                        }
                        else if ((f & SimpleGlyphFlags.SameY) == 0)
                            delta = reader.ReadInt16BE();

                        y += delta;
                        points[i] = new VectorF(points[i].X, y,
                            ((f & SimpleGlyphFlags.OnCurve) != 0 ? PointKind.Normal : PointKind.Control));
                    }

                    return new SimpleGlyph
                    {
                        Points = points,
                        ContourEndpoints = contours,
                        Instructions = instructions
                    };
                }

                static CompositeGlyph ReadCompositeGlyph(DataReader reader)
                {
                    // we need to keep reading glyphs for as long as
                    // our flags tell us that there are more to read
                    var subglyphs = new PrimitiveList<Subglyph>();

                    CompositeGlyphFlags flags;
                    do
                    {
                        flags = (CompositeGlyphFlags)reader.ReadUInt16BE();

                        var subglyph = new Subglyph { Flags = flags };
                        subglyph.Index = reader.ReadUInt16BE();

                        // read  args; they vary  size based on flags
                        if ((flags & CompositeGlyphFlags.ArgsAreWords) != 0)
                        {
                            subglyph.Arg1 = reader.ReadInt16BE();
                            subglyph.Arg2 = reader.ReadInt16BE();
                        }
                        else
                        {
                            subglyph.Arg1 = reader.ReadSByte();
                            subglyph.Arg2 = reader.ReadSByte();
                        }

                        // figure out the transform; we can either have no scale, a uniform
                        // scale, two independent scales, or a full 2x2 transform matrix
                        // transform components are  2.14 fixed point format
                        var transform = Matrix3x2.Identity;
                        if ((flags & CompositeGlyphFlags.HaveScale) != 0)
                        {
                            var scale = reader.ReadInt16BE() / F2Dot14ToFloat;
                            transform.M00 = scale;
                            transform.M11 = scale;
                        }
                        else if ((flags & CompositeGlyphFlags.HaveXYScale) != 0)
                        {
                            transform.M00 = reader.ReadInt16BE() / F2Dot14ToFloat;
                            transform.M11 = reader.ReadInt16BE() / F2Dot14ToFloat;
                        }
                        else if ((flags & CompositeGlyphFlags.HaveTransform) != 0)
                        {
                            transform.M00 = reader.ReadInt16BE() / F2Dot14ToFloat;
                            transform.M01 = reader.ReadInt16BE() / F2Dot14ToFloat;
                            transform.M10 = reader.ReadInt16BE() / F2Dot14ToFloat;
                            transform.M11 = reader.ReadInt16BE() / F2Dot14ToFloat;
                        }

                        subglyph.Transform = transform;
                        subglyphs.Add(subglyph);

                    } while ((flags & CompositeGlyphFlags.MoreComponents) != 0);

                    var result = new CompositeGlyph { Subglyphs = subglyphs.ToArray() };

                    // if we have instructions, read them now
                    if ((flags & CompositeGlyphFlags.HaveInstructions) != 0)
                    {
                        var instructionLength = reader.ReadUInt16BE();
                        result.Instructions = reader.ReadBytes(instructionLength);
                    }

                    return result;
                }

                static string ExtractString(DataReader reader, uint baseOffset, StringData data)
                {
                    reader.Seek(baseOffset + data.Offset);

                    var bytes = reader.ReadBytes(data.Length);
                    return Encoding.BigEndianUnicode.GetString(bytes);
                }

                // most of these limits are arbitrary; they can be increased if you
                // run into a font  the wild that is constrained by them
                const uint TTFv1 = 0x10000;
                const uint TTFv2 = 0x20000;
                const int MaxGlyphs = short.MaxValue;
                const int MaxContours = 256;
                const int MaxRecursion = 128;
                const int MaxFontsInCollection = 64;
                const int MaxStackSize = 16384;
                const int MaxTwilightPoints = short.MaxValue;
                const int MaxFunctionDefs = 4096;
                const int MaxStorageLocations = 16384;
                const float F2Dot14ToFloat = 16384.0f;

                [Flags]
                enum SimpleGlyphFlags
                {
                    None = 0,
                    OnCurve = 0x1,
                    ShortX = 0x2,
                    ShortY = 0x4,
                    Repeat = 0x8,
                    SameX = 0x10,
                    SameY = 0x20
                }

                [Flags]
                enum FsSelectionFlags
                {
                    Italic = 0x1,
                    Bold = 0x20,
                    Regular = 0x40,
                    UseTypoMetrics = 0x80,
                    WWS = 0x100,
                    Oblique = 0x200
                }

                struct GlyphHeader
                {
                    public short ContourCount;
                    public short MinX;
                    public short MinY;
                    public short MaxX;
                    public short MaxY;
                }

                struct StringData
                {
                    public ushort Name;
                    public ushort Offset;
                    public ushort Length;
                }

                static class NameID
                {
                    public const int FamilyName = 1;
                    public const int SubfamilyName = 2;
                    public const int UniqueID = 3;
                    public const int FullName = 4;
                    public const int Version = 5;
                    public const int Description = 10;
                    public const int TypographicFamilyName = 16;
                    public const int TypographicSubfamilyName = 17;
                }
            }
            #endregion

            #region CHARACTER
            struct Character : IComparable<Character>, IEquatable<Character>
            {
                readonly int value;

                #region constructors
                /// <summary>
                /// Initializes a new instance of the <see cref="Character"/> struct.
                /// </summary>
                /// <param name="codePoint">The 32-bit value of the codepoint.</param>
                public Character(int codePoint)
                {
                    value = codePoint;
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="Character"/> struct.
                /// </summary>
                /// <param name="character">The 16-bit value of the codepoint.</param>
                public Character(char character)
                {
                    value = character;
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="Character"/> struct.
                /// </summary>
                /// <param name="highSurrogate">The first member of a surrogate pair representing the codepoint.</param>
                /// <param name="lowSurrogate">The second member of a surrogate pair representing the codepoint.</param>
                public Character(char highSurrogate, char lowSurrogate)
                {
                    value = char.ConvertToUtf32(highSurrogate, lowSurrogate);
                }
                #endregion

                #region IComparable/ IEquatable
                /// <summary>
                /// Compares this instance to the specified value.
                /// </summary>
                /// <param name="other">The value to compare.</param>
                /// <returns>A signed number indicating the relative values of this instance and <paramref name="other"/>.</returns>
                public int CompareTo(Character other) => value.CompareTo(other.value);

                /// <summary>
                /// Returns a value indicating whether this instance is equal to the specified object.
                /// </summary>
                /// <param name="obj">The object to compare.</param>
                /// <returns><c>true</c> if this instance equals <paramref name="other"/>; otherwise, <c>false</c>.</returns>
                public bool Equals(Character other) => value.Equals(other.value);

                /// <summary>
                /// Returns a value indicating whether this instance is equal to the specified object.
                /// </summary>
                /// <param name="obj">The object to compare.</param>
                /// <returns><c>true</c> if this instance equals <paramref name="obj"/>; otherwise, <c>false</c>.</returns>
                public override bool Equals(object obj)
                {
                    var codepoint = obj as Character?;
                    if (codepoint == null)
                        return false;

                    return Equals(codepoint);
                }

                /// <summary>
                /// Returns the hash code for this instance.
                /// </summary>
                /// <returns>The instance's hashcode.</returns>
                public override int GetHashCode() => value.GetHashCode();
                #endregion

                #region operator overload
                /// <summary>
                /// Implements the equality operator.
                /// </summary>
                /// <param name="left">The left hand side of the operator.</param>
                /// <param name="right">The right hand side of the operator.</param>
                /// <returns>The result of the operator.</returns>
                public static bool operator ==(Character left, Character right) => left.Equals(right);

                /// <summary>
                /// Implements the inequality operator.
                /// </summary>
                /// <param name="left">The left hand side of the operator.</param>
                /// <param name="right">The right hand side of the operator.</param>
                /// <returns>The result of the operator.</returns>
                public static bool operator !=(Character left, Character right) => !left.Equals(right);

                /// <summary>
                /// Implements the less-than operator.
                /// </summary>
                /// <param name="left">The left hand side of the operator.</param>
                /// <param name="right">The right hand side of the operator.</param>
                /// <returns>The result of the operator.</returns>
                public static bool operator <(Character left, Character right) => left.value < right.value;

                /// <summary>
                /// Implements the greater-than operator.
                /// </summary>
                /// <param name="left">The left hand side of the operator.</param>
                /// <param name="right">The right hand side of the operator.</param>
                /// <returns>The result of the operator.</returns>
                public static bool operator >(Character left, Character right) => left.value > right.value;

                /// <summary>
                /// Implements the less-than-or-equal-to operator.
                /// </summary>
                /// <param name="left">The left hand side of the operator.</param>
                /// <param name="right">The right hand side of the operator.</param>
                /// <returns>The result of the operator.</returns>
                public static bool operator <=(Character left, Character right) => left.value <= right.value;

                /// <summary>
                /// Implements the greater-than-or-equal-to operator.
                /// </summary>
                /// <param name="left">The left hand side of the operator.</param>
                /// <param name="right">The right hand side of the operator.</param>
                /// <returns>The result of the operator.</returns>
                public static bool operator >=(Character left, Character right) => left.value >= right.value;

                /// <summary>
                /// Implements an explicit conversion from integer to <see cref="Character"/>.
                /// </summary>
                /// <param name="codePoint">The codepoint value.</param>
                public static explicit operator Character(int codePoint) => new Character(codePoint);

                /// <summary>
                /// Implements an implicit conversion from character to <see cref="Character"/>.
                /// </summary>
                /// <param name="character">The character value.</param>
                public static implicit operator Character(char character) => new Character(character);

                /// <summary>
                /// Implements an explicit conversion from <see cref="Character"/> to character.
                /// </summary>
                /// <param name="codePoint">The codepoint value.</param>
                public static explicit operator char(Character codePoint) => (char)codePoint.value;
                #endregion

                /// <summary>
                /// Converts the value to its equivalent string representation.
                /// </summary>
                /// <returns></returns>
                public override string ToString() => $"{value} ({(char)value})";
            }
            #endregion

            #region TABLE RECORD
            struct TableRecord
            {
                public FourCC Tag;
                public uint CheckSum;
                public uint Offset;
                public uint Length;

                public override string ToString() => Tag.ToString();
            }
            #endregion

            #region FACE HEADER
            struct FaceHeader
            {
                public HeadFlags Flags;
                public int UnitsPerEm;
                public int IndexFormat;
                public int UnderlinePosition;
                public int UnderlineThickness;
                public bool IsFixedPitch;
                public int GlyphCount;
                public int MaxTwilightPoints;
                public int MaxStorageLocations;
                public int MaxFunctionDefs;
                public int MaxInstructionDefs;
                public int MaxStackSize;
            }
            #endregion

            #region METRICSHEADER
            struct MetricsHeader
            {
                public int Ascender;
                public int Descender;
                public int LineGap;
                public int MetricCount;
            }
            #endregion

            #region METRICSENTRY
            struct MetricsEntry
            {
                public int Advance;
                public int FrontSideBearing;
            }
            #endregion

            #region OS2 DATA
            struct OS2Data
            {
                public FontWeight Weight;
                public FontStretch Stretch;
                public FontMode Style;
                public int StrikeoutSize;
                public int StrikeoutPosition;
                public int TypographicAscender;
                public int TypographicDescender;
                public int TypographicLineGap;
                public int WinAscent;
                public int WinDescent;
                public bool UseTypographicMetrics;
                public bool IsWWSFont;
                public int XHeight;
                public int CapHeight;
            }
            #endregion

            #region NAME DATA
            struct NameData
            {
                public string FamilyName;
                public string SubfamilyName;
                public string UniqueID;
                public string FullName;
                public string Version;
                public string Description;
                public string TypographicFamilyName;
                public string TypographicSubfamilyName;
            }
            #endregion

            #region FOUR CC
            struct FourCC
            {
                uint value;

                public FourCC(uint value)
                {
                    this.value = value;
                }

                public FourCC(string str)
                {
                    if (str.Length != 4)
                        throw new InvalidOperationException("Invalid FourCC code");
                    value = str[0] | ((uint)str[1] << 8) | ((uint)str[2] << 16) | ((uint)str[3] << 24);
                }

                public override string ToString()
                {
                    return new string(new[] {
                    (char)(value & 0xff),
                    (char)((value >> 8) & 0xff),
                    (char)((value >> 16) & 0xff),
                    (char)(value >> 24)
                });
                }

                public static implicit operator FourCC(string value) => new FourCC(value);
                public static implicit operator FourCC(uint value) => new FourCC(value);
                public static implicit operator uint(FourCC fourCC) => fourCC.value;

                public static readonly FourCC Otto = "OTTO";
                public static readonly FourCC True = "true";
                public static readonly FourCC Ttcf = "ttcf";
                public static readonly FourCC Typ1 = "typ1";
                public static readonly FourCC Head = "head";
                public static readonly FourCC Maxp = "maxp";
                public static readonly FourCC Post = "post";
                public static readonly FourCC OS_2 = "OS/2";
                public static readonly FourCC Hhea = "hhea";
                public static readonly FourCC Hmtx = "hmtx";
                public static readonly FourCC Vhea = "vhea";
                public static readonly FourCC Vmtx = "vmtx";
                public static readonly FourCC Loca = "loca";
                public static readonly FourCC Glyf = "glyf";
                public static readonly FourCC Cmap = "cmap";
                public static readonly FourCC Kern = "kern";
                public static readonly FourCC Name = "name";
                public static readonly FourCC Cvt = "cvt ";
                public static readonly FourCC Fpgm = "fpgm";
                public static readonly FourCC Prep = "prep";
                public static readonly FourCC Eblc = "EBLC";
            }
            #endregion

            #region BASE GLYPH
            abstract class BaseGlyph : ICloneable
            {
                public byte[] Instructions;
                public int MinX;
                public int MinY;
                public int MaxX;
                public int MaxY;

                protected abstract BaseGlyph newInstance();
                public virtual object Clone()
                {
                    var g = newInstance();
                    g.Instructions = Instructions?.ToArray();
                    g.MinX = MinX;
                    g.MinY = MinY;
                    g.MaxX = MaxX;
                    g.MaxY = MaxY;
                    return g;
                }
            }
            #endregion

            #region SIMPLE GLYPH
            class SimpleGlyph : BaseGlyph
            {
                public VectorF[] Points;
                public int[] ContourEndpoints;

                protected override BaseGlyph newInstance() =>
                    new SimpleGlyph();
                public override object Clone()
                {
                    var g = base.Clone() as SimpleGlyph;

                    g.Points = Points.ToArray();
                    g.ContourEndpoints = ContourEndpoints.ToArray();
                    return g;
                }
            }
            #endregion

            #region SUB GLYPH
            struct Subglyph
            {
                public Matrix3x2 Transform;
                public CompositeGlyphFlags Flags;
                public int Index;
                public int Arg1;
                public int Arg2;
            }
            #endregion

            #region COMPOSITE GLYPH
            class CompositeGlyph : BaseGlyph
            {
                public Subglyph[] Subglyphs;

                protected override BaseGlyph newInstance() =>
                    new CompositeGlyph();
                public override object Clone()
                {
                    var c = base.Clone() as CompositeGlyph;
                    c.Subglyphs = Subglyphs.ToArray();
                    return c;
                }
            }
            #endregion

            #region COMPOSITE GLYPH FLAGS
            [Flags]
            enum CompositeGlyphFlags
            {
                None = 0,
                ArgsAreWords = 0x1,
                ArgsAreXYValues = 0x2,
                RoundXYToGrid = 0x4,
                HaveScale = 0x8,
                MoreComponents = 0x20,
                HaveXYScale = 0x40,
                HaveTransform = 0x80,
                HaveInstructions = 0x100,
                UseMetrics = 0x200,
                ScaledComponentOffset = 0x800
            }
            #endregion

            #region HEAD FLAGS
            [Flags]
            enum HeadFlags
            {
                None = 0,
                SimpleBaseline = 0x1,
                SimpleLsb = 0x2,
                SizeDependentInstructions = 0x4,
                IntegerPpem = 0x8,
                InstructionsAlterAdvance = 0x10
            }
            #endregion

            #region FONT INFO
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

                public object Clone()
                {
                    var i = new FontInfo();
                    i.Style = Style;
                    i.UnitsPerEm = UnitsPerEm;
                    i.IntegerPpems = IntegerPpems;
                    i.FullName = FullName;
                    i.Description = Description;
                    i.CellAscent = CellAscent;
                    i.CellDescent = CellDescent;
                    i.LineHeight = LineHeight;
                    i.XHeight = XHeight;
                    i.UnderlineSize = UnderlineSize;
                    i.UnderlinePosition = UnderlinePosition;
                    i.StrikeoutSize = StrikeoutSize;
                    i.StrikeoutPosition = StrikeoutPosition;
                    return i;
                }
            }
            #endregion
        }
    }
}
#endif