/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if (GWS || Window)

namespace MnM.GWS
{
    using System;
    using System.Collections.Generic;

    #region IFONT-INFO
    public interface IFontInfo
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
    public interface IFont
    {
        /// <summary>
        /// Gets or sets the size of the font to draw text.
        /// </summary>
        int Size { get; set; }

        /// <summary>
        /// Gets kerning information associated with the font.
        /// </summary>
        bool Kerning { get; }

        /// <summary>
        /// Gets or sets flag to enable or disable application of kerning while measuring and drawng text.
        /// </summary>
        bool EnableKerning { get; set; }

        /// <summary>
        /// Gives an information of this font such as line height, scale, horzontal and vertical bearings etc.
        /// </summary>
        IFontInfo Info { get; }

        /// <summary>
        /// Gives the kerning value for the given character in relation to previous character drawn or measured in sequential draw of given text.
        /// </summary>
        /// <param name="previous">Previous character measured or drawn</param>
        /// <param name="now">Current character for which the kerning is sought for</param>
        /// <returns></returns>
        int GetKerning(char previous, char now);

        /// <summary>
        /// Retrieves a glyph object exists for a given character which contains vital information on how to draw the character using the given font on screen.
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        IGlyph GetGlyph(char character);

        #region MEASURE GLYPHS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Glyphs">Colleciton of glyphs to measure.</param>
        /// <param name="destX">X co-ordinate of startig location of text.</param>
        /// <param name="destY">Y co-ordinate of startig location of text.</param>
        /// <param name="Area">Caluculated area returned after measuring the text.</param>
        /// <param name="ResultGlyphs">Glyphs returned after measuring the text.</param>
        /// <param name="minHBY">Minimum horizontal bearing returned after measuring the text.</param>
        /// <param name="drawStyle">Draw style to take into account while measuring if supplied.</param>
        void MeasureGlyphs(IList<IGlyph> Glyphs, float destX, float destY,
            out RectangleF Area, out IList<IGlyph> ResultGlyphs, out float minHBY, ITextStyle drawStyle = null);
        #endregion

        /// <summary>
        /// Gets or sets a flag indication whether font hinting is to applied or not while drawing or measuring text.
        /// </summary>
        bool Hinting { get; set; }
    }
    #endregion

    #region IGLYPH
    /// <summary>
    /// Represents a glyph object which contains all the information on how a particual character it represents should be drawn on screen.
    /// </summary>
    public interface IGlyph : IDrawable, ICloneable, IPoint, ISizeF
    {
        /// <summary>
        /// Gets or sets X co-ordinate of position of this glyph.
        /// </summary>
        new int X { get; set; }

        /// <summary>
        /// Gets or sets Y co-ordinate of position of this glyph.
        /// </summary>
        new int Y { get; set; }

        /// <summary>
        /// The character which the glyph represents.
        /// </summary>
        char Character { get; }

        /// <summary>
        /// Gets or sets a flag indicating whether the character is to be drawn only in terms of outline and not to be filled or the otherwise.
        /// </summary>
        bool IsOutLine { get; set; }
    }
    #endregion

    #region IGLYPHSLOT
    /// <summary>
    /// Represents information vital to draw a character for a given font.
    /// the information is directly fetched from the font object.
    /// </summary>
    public interface IGlyphSlot : IRectangleF
    {
        #region PROPERTIES
        /// <summary>
        /// List of Points that forms an outline of the character.
        /// </summary>
        IList<VectorF> Points { get; }

        /// <summary>
        /// List of curve contours
        /// </summary>
        IList<int> Contours { get; }

        /// <summary>
        /// The character this slot represents for drawing on screen.
        /// </summary>
        char Character { get; }

        /// <summary>
        /// XHeight of the slot.
        /// </summary>
        int XHeight { get; }

        /// <summary>
        /// Minimum of points which forms perimiter of the slot.
        /// </summary>
        VectorF Min { get; }

        /// <summary>
        /// Maximum of points which forms the perimeter of the slot.
        /// </summary>
        VectorF Max { get; }

        /// <summary>
        /// Indicates if the slot is initialzed and ready for the process or not.
        /// </summary>
        bool Initialized { get; }
        #endregion
    }
    #endregion

    #region IGLYPHS
    public interface IGlyphs : IDrawable, IEnumerable<IGlyph>, IRectangleF
    {
        /// <summary>
        /// Gets the child glyph at a given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IGlyph this[int index] { get; set; }

        /// <summary>
        /// Idicates number of glyphs present in the collection.
        /// </summary>
        int Count { get; }
    }
    #endregion

    #region IGLYPH-RENDERER
    /// <summary>
    /// Renders a given slot using the action supplied on a given context.
    /// </summary>
    public interface IGlyphRenderer : IDisposable
    {
        /// <summary>
        /// Process the glyph slot taking the action specified.
        /// </summary>
        /// <param name="points">Collection of points to process.</param>
        /// <param name="contours">Collection of contours to be used while processing points.</param>
        /// <param name="action">the action to render result of the processing</param>
        /// <param name="width">Width of the area to be used for the  processing</param>
        /// <param name="height">Height of the area to be used for the  processing</param>
        void Process(IList<VectorF> points, IList<int> contours, FillAction action, int width, int height);
    }
    #endregion

    #region ITEXT DRAW
    public interface ITextStyle : ICloneable
    {
        /// <summary>
        /// Gets or sets the preffered size of the text bounds.
        /// </summary>
        Size PreferredSize { get; set; }

        /// <summary>
        /// Get or sets a flag to specifying whether any case conversion of characters in a text should take place before measuring or drawing.
        /// </summary>
        CaseConversion CaseConversion { get; set; }

        /// <summary>
        /// Gets or sets Alignment of a text in respect of actual bounds of measurement.
        /// </summary>
        ContentAlignment Position { get; set; }

        /// <summary>
        /// Gets or sets a line break option while measuring or drawing a text.
        /// </summary>
        TextBreaker Breaker { get; set; }

        /// <summary>
        /// Gets or sets a break delimiter  while measuring or drawing a text. 
        /// Text can be broke into characters or a word displaed on a single line.
        /// </summary>
        BreakDelimiter Delimiter { get; set; }

        /// <summary>
        /// Gets or sets a text style to be applied for example strike out , underline etc. while drawing a text.
        /// </summary>
        TextStyle TextStyle { get; set; }

        /// <summary>
        /// Preferred line height to be applied while placing charactes on next line.
        /// </summary>
        int LineHeight { get; set; }

        /// <summary>
        /// Draws glyps individually in terms of creating bufferpen fitting individual area of a glyh rather than taking a bufferpen covering an entire area of text.
        /// </summary>
        bool DrawGlyphIndividually { get; set; }

#if Advanced
        /// <summary>
        /// Get sor sets an alignment of a buffer in a bounding box.
        /// </summary>
        ImagePosition ImageAlignment { get; set; }

        /// <summary>
        /// Gets or sets how buffer is drawn whether scalled or unscalled.
        /// </summary>
        ImageDraw ImageDraw { get; set; }

        /// <summary>
        /// Gets or sets a Buffer image to be drawn to screen.
        /// </summary>
        IImage Image { get; set; }
#endif
        /// <summary>
        //  Creates a new instance of this class with the same value as an existing instance.
        /// </summary>
        new ITextStyle Clone();
    }
    #endregion
}
#endif
