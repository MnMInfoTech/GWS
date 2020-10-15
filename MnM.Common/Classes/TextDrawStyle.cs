/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if (GWS || Window)
    using System;
    public sealed class TextDrawStyle : ICloneable
    {
        /// <summary>
        /// Image style to be used for placing an image it at all one is provided.
        /// </summary>
        public ImageStyle ImageStyle { get; set; }

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

        /// <summary>
        //  Creates a new instance of a class with the same value as an existing instance.
        /// </summary>
        /// <returns></returns>
        public TextDrawStyle Clone()
        {
            var d = new TextDrawStyle();
            d.PreferredSize = new Size(PreferredSize.Width, PreferredSize.Height);
            d.CaseConversion = CaseConversion;
            d.Position = Position;
            d.Breaker = Breaker;
            d.Delimiter = Delimiter;
            d.TextStyle = TextStyle;
            d.LineHeight = LineHeight;
            d.ImageStyle = ImageStyle?.Clone() as ImageStyle;
            d.DrawGlyphIndividually = DrawGlyphIndividually;
            return d;
        }

        object ICloneable.Clone() => Clone();

    }
#endif
}
