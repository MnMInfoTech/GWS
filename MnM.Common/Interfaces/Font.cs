/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if (GWS || Window)
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
    public interface IFont : IID
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
    public interface IGlyph : IDrawable, ICloneable, ILocation
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

    public interface IGlyphs : IDrawable, IEnumerable<IGlyph>, IRecognizable
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
        void Process(IList<VectorF> points, IList<int> contours, FillAction<int> action, int width, int height);
    }
#endif
}
