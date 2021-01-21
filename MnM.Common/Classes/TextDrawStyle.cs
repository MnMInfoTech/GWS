/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if (GWS || Window)

namespace MnM.GWS
{
    using System;

    using MnM.GWS.Advanced;

    public class TextDrawStyle : ITextStyle
    {
        /// <summary>
        /// Gets or sets the preffered size of the text bounds.
        /// </summary>
        public Size PreferredSize { get; set; }

        /// <summary>
        /// Get or sets a flag to specifying whether any case conversion of characters in a text should take place before measuring or drawing.
        /// </summary>
        public CaseConversion CaseConversion { get; set; }

        /// <summary>
        /// Gets or sets Alignment of a text in respect of actual bounds of measurement.
        /// </summary>
        public ContentAlignment Position { get; set; }

        /// <summary>
        /// Gets or sets a line break option while measuring or drawing a text.
        /// </summary>
        public TextBreaker Breaker { get; set; }

        /// <summary>
        /// Gets or sets a break delimiter  while measuring or drawing a text. 
        /// Text can be broke into characters or a word displaed on a single line.
        /// </summary>
        public BreakDelimiter Delimiter { get; set; }

        /// <summary>
        /// Gets or sets a text style to be applied for example strike out , underline etc. while drawing a text.
        /// </summary>
        public TextStyle TextStyle { get; set; }

        /// <summary>
        /// Preferred line height to be applied while placing charactes on next line.
        /// </summary>
        public int LineHeight { get; set; }

        /// <summary>
        /// Draws glyps individually in terms of creating bufferpen fitting individual area of a glyh rather than taking a bufferpen covering an entire area of text.
        /// </summary>
        public bool DrawGlyphIndividually { get; set; }

#if Advanced
        /// <summary>
        /// Get sor sets an alignment of a buffer in a bounding box.
        /// </summary>
        public ImagePosition ImageAlignment { get; set; }

        /// <summary>
        /// Gets or sets how buffer is drawn whether scalled or unscalled.
        /// </summary>
        public ImageDraw ImageDraw { get; set; }

        /// <summary>
        /// Gets or sets a Buffer image to be drawn to screen.
        /// </summary>
        public ISurface Image { get; set; }

#endif
        /// <summary>
        //  Creates a new instance of a class with the same value as an existing instance.
        /// </summary>
        /// <returns></returns>
        public ITextStyle Clone()
        {
            var txtStyle = new TextDrawStyle();
            txtStyle.PreferredSize = PreferredSize;
            txtStyle.CaseConversion = CaseConversion;
            txtStyle.Position = Position;
            txtStyle.Breaker = Breaker;
            txtStyle.Delimiter = Delimiter;
            txtStyle.TextStyle = TextStyle;
            txtStyle.LineHeight = LineHeight;
            txtStyle.DrawGlyphIndividually = DrawGlyphIndividually;
#if Advanced
            txtStyle.ImageAlignment = ImageAlignment;
            txtStyle.ImageDraw = ImageDraw;
            txtStyle.Image = Image.Clone() as ISurface;
#endif
            return txtStyle;
        }

        object ICloneable.Clone() => Clone();

    }
}
#endif
