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

#if GWS || Window
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    #region IFONT-INFO
    public interface IFontInfo: ICloneable
    {
        FontMode Style { get; }
        float UnitsPerEm { get; }
        bool IntegerPpems { get; }
        string FullName { get; }
        string Description { get; }
        float CellAscent { get; }
        float CellDescent { get; }
        float LineHeight { get; }
        float XHeight { get; }
        float UnderlineSize { get; }
        float UnderlinePosition { get; }
        float StrikeoutSize { get; }
        float StrikeoutPosition { get; }
    }
    #endregion

    #region IFONT
    /// <summary>
    /// Represents a font object to use for text drawing.
    /// </summary>
    public partial interface IFont : ICloneable, IProperty
    {
        #region PROPERTIES
        /// <summary>
        /// Retrieves a glyph object exists for a given character which contains vital information on how to draw the character using the given font on screen.
        /// </summary>
        /// <param name="character">Character to return a glyph for.</param>
        /// <returns></returns>
        IGlyph this[char character] { get; }

        /// <summary>
        /// Gets or sets the size of the font to draw text.
        /// </summary>
        int Size { get; set; }

        /// <summary>
        /// Gets or sets flag to enable or disable application of kerning while measuring and drawng text.
        /// </summary>
        bool EnableKerning { get; set; }

        /// <summary>
        /// Gives an information of this font such as line height, scale, horzontal and vertical bearings etc.
        /// </summary>
        IFontInfo Info { get; }

        /// <summary>
        /// Gets or sets a flag indication whether font hinting is to applied or not while drawing or measuring text.
        /// </summary>
        bool Hinting { get; set; }       
        #endregion

        #region GET KERNING
        /// <summary>
        /// Gives the kerning value for the given character in relation to previous character drawn or measured in sequential draw of given text.
        /// </summary>
        /// <param name="previous">Previous character measured or drawn</param>
        /// <param name="now">Current character for which the kerning is sought for</param>
        /// <returns></returns>
        int GetKerning(char previous, char now);
        #endregion

        #region MEASURE GLYPHS
        /// <summary>
        /// Measures specified glyphs and returns resultant glyphs and area.
        /// </summary>
        /// <param name="Glyphs">Colleciton of glyphs to measure.</param>
        /// <param name="parameters">Various text rendering parameters to assist and influence the measuring process.</param>
        /// <returns></returns>
        IReadOnlyList<IGlyphLineInfo> MeasureGlyphs(IEnumerable<IGlyph> glyphs,
                out IRectangle area, out int glyphCount, out float minHBY, IEnumerable<IParameter> parameters = null);
        #endregion
    }
    #endregion

    #region IHBY
    public interface IHBY
    {
        /// <summary>
        /// Gets the minimum of the glyph's top left side bearing in horizontal layouts. 
        /// </summary>
        float MinHBY { get; }
    }
    #endregion
}

namespace MnM.GWS
{
    partial class Factory
    {
        sealed partial class Font : IFont
        {
            #region VARIABLES
            FontInfo info;
            bool kerning, enableKerning;
            float size;
            int iSize;
            float spaceWidth, lineHeight;
            FontMode style;
            readonly Dictionary<string, IGlyphSlot> Cache;
            const string tostr = "Name:{0}, Size:{1}, XHeight:{2}, Ascent:{3}, Descent:{4}";
            #endregion

            #region CONSTRUCTORS
            public Font(string path, int fontSize) :
                this()
            {
                using (Stream fontStream = System.IO.File.OpenRead(path))
                {
                    ReadFont(fontStream, fontSize);
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="_NativeFont"/> class.
            /// </summary>
            /// <param name="fontStream">A stream pointing to the font file.</param>
            /// <remarks>
            /// All relevant font data is loaded into memory and retained by the FontFace object.
            /// Once the constructor finishes you are free to close the stream.
            /// </remarks>
            public Font(Stream fontStream, int fontSize) :
                this()
            {
                ReadFont(fontStream, fontSize);
            }

            Font()
            {
                Cache = new Dictionary<string, IGlyphSlot>(400);
            }
            Font(Font f) :
                this()
            {
                info = f.info.Clone() as FontInfo;
                EnableKerning = f.EnableKerning;
                Hinting = f.Hinting;
                kerning = f.kerning;
                iSize = f.iSize;
                size = f.size;
                style = f.style;
                interpreter = f.interpreter?.Clone() as Interpreter;
                glyphs = f.glyphs?.Select(g => g.Clone() as BaseGlyph)?.ToArray();
                IntegerPpems = f.IntegerPpems;
                hmetrics = f.hmetrics?.ToArray();
                vmetrics = f.vmetrics?.ToArray();
                charMap = f.charMap?.Clone() as CharacterMap;
                kernTable = f.kernTable?.Clone() as KerningTable;
                verticalSynthesized = f.verticalSynthesized;
                controlValueTable = f.controlValueTable?.ToArray();
                prepProgram = f.prepProgram?.ToArray();
            }
            #endregion

            #region PROPERTIES
            public IGlyph this[char character]
            {
                get
                {
                    var key = size + "." + character.ToString();
                    if (Cache.ContainsKey(key))
                        return new Glyph(Cache[key]);

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
                    IList<VectorF> points = new PrimitiveList<VectorF>(32);
                    var contours = new PrimitiveList<int>(32);
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
                    if(!Cache.ContainsKey(key))
                        Cache.Add(key, g);
                    return new Glyph(g);
                }
            }
            public bool EnableKerning 
            { 
                get => kerning && enableKerning; 
                set => enableKerning = value; 
            }
            public bool Hinting { get; set; }
            public IFontInfo Info => info;
            public int Size
            {
                get => iSize;
                set
                {
                    iSize = value;
                    size = ComputePixelSize(Math.Max(3, iSize), Application.DPI);
                    Cache.Clear();
                    spaceWidth = this[' '].Width;
                    lineHeight = this['I'].Height;
                }
            }
            public FontMode Style => style;
            object IValue.Value => this;
            #endregion

            #region MEASURE GLYPHS
            public IReadOnlyList<IGlyphLineInfo> MeasureGlyphs(IEnumerable<IGlyph> glyphs, 
                out IRectangle area, out int glyphCount, out float minHBY, IEnumerable<IParameter> parameters)
            {
                glyphCount = glyphs.Count();

                #region IF THERE IS NO GLYPH IN COLLECTION EXIT
                if (glyphCount == 0)
                {
                    minHBY = 0;
                    area = Rectangle.Empty;
                    return new IGlyphLineInfo[] { new GlyphLineInfo() };
                }
                #endregion

                #region EXTRACT TEXT PARAMETERS
                parameters.ExtractTextDrawParameters(out TextCommand command, out int maxcharDisplay, out IBounds container, out int maxWidth);
                #endregion

                #region CREATE IGlyphLineInfo COLLECTION TO HOLD DATA
                var lines = new PrimitiveList<IGlyphLineInfo>();
                #endregion

                #region INITIALIZE DSTX, DSTY AND OTHER VARAIBELS

                float dstX = 0, dstY = 0, x = 0, y = 0, r = 0, b = 0;

                float newX, newY, kerning, currentX, currentY;
                float lh = lineHeight;
                if (lh < info.LineHeight)
                    lh = info.LineHeight;
                lh += 2;
                float w = 0, h = 0;
                #endregion

                #region GET HBY AND UPDATE DSTY WITH IT
                minHBY = glyphs.Min(g => g.MinHBY);
                if (minHBY < 0)
                    dstY -= minHBY;
                else
                    dstY += minHBY;
                #endregion

                #region PARSE TEXT COMMAND
                bool Word = (command & TextCommand.Word) == TextCommand.Word;
                bool Line = (command & TextCommand.Line) == TextCommand.Line;
                bool SingleWord = (command & TextCommand.SingleWord) == TextCommand.SingleWord;
                bool SingleChar = (command & TextCommand.SingleChar) == TextCommand.SingleChar;
                bool CharDelimiter = (command & TextCommand.CharacterDelimiter) == TextCommand.CharacterDelimiter;
                bool WordDelimiter = (command & TextCommand.WordDelimiter) == TextCommand.WordDelimiter;
                #endregion

                #region INITIALIZE LOOPVARIABLE SUCH AS KERNING X, Y, INDEX ETC.
                bool Kerning = EnableKerning;
                newX = dstX;
                newY = dstY;
                bool begin = true;
                kerning = 0;


                int index = 0;
                int i = -1;
                int rnCount = 0;

                IGlyph previous = this[(char)0];
                #endregion

                #region LOOP
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
                        else if (Line || command == 0)
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
                    if (Kerning && i != 0)
                        kerning = GetKerning(this, current, previous);

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

                    if (newY + h > b)
                        b = newY + h;

                    current.X = currentX;
                    current.Y = currentY;
                    begin = false;
                    previous = current;
                }
                #endregion

                #region FINIALIZE LINE INFO COLLECTION
                if (lines.Count > 0)
                {
                    lines.Add(new GlyphLineInfo(ref index, i, dstX, dstY,
                        ref w, ref h, ref newX, ref newY, ref rnCount, lh));

                }
                #endregion

                #region DETERMINE TOTAL AREA, GLYPHS' COUNT
                area = Rectangle.FromLTRB(x, y, r, b);
                glyphCount = i;
                #endregion

                return lines;
            }
            #endregion

            #region GET KERNING
            static int GetKerning(IFont font, IGlyph current, IGlyph previous)
            {
                if (previous == null)
                    return 0;
                var c = current.Character;
                if (c != 0)
                    return font.GetKerning(previous.Character, c);
                return 0;
            }
            #endregion

            #region GET GLYPH
            public static IGlyphSlot newGlyphSlot(char character, IList<VectorF> pts, int[] contours, int XHeight) =>
                new GlyphSlot(character, pts, contours, XHeight);
            #endregion

            #region GET KERNING
            public int GetKerning(char left, char right)
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

            #region CLONE
            public object Clone() =>
                new Font(this);
            #endregion

            #region DISPOSE
            public void Dispose() { }
            #endregion

            public override string ToString()
            {
                return string.Format(tostr, Info.FullName, size, Info.XHeight, Info.CellAscent, Info.CellDescent);
            }
        }
    }
}
#endif
