/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

using System;

namespace MnM.GWS
{
#if Texts || Collections || GWS || Window
    using System;

    #region CASE CONVERSION
    /// <summary>
    /// Enum CaseConversion
    /// </summary>
    public enum CaseConversion
    {
        /// <summary>
        /// The none
        /// </summary>
        None,

        /// <summary>
        /// The title i.e "Hello Are You?"
        /// </summary>
        Title,

        /// <summary>
        /// The sentence i.e "Hhow are you?"
        /// </summary>
        Sentence,

        /// <summary>
        /// The upper i.e "HOW ARE YOU?"
        /// </summary>
        Upper,

        /// <summary>
        /// The lower i.e "how are you?"
        /// </summary>
        Lower,
        /// <summary>
        /// The toggle i.e if original text is: "How are you?" it becomes "hOW ARE YOU?"
        /// </summary>
        Toggle
    }
    #endregion
#endif

#if Texts
    #region FORMAT TYPE
    /// <summary>
    /// Enum FormatType
    /// </summary>
    public enum TextFormatType
    {
        /// <summary>
        /// The array
        /// </summary>
        Array,
        /// <summary>
        /// The normal
        /// </summary>
        Normal,
        /// <summary>
        /// The group
        /// </summary>
        Group,

        VirtualTextBox,

        TextBox
    }
    #endregion

    #region WORD SELECTION
    /// <summary>
    /// Enum WordSelection
    /// </summary>
    public enum WordSelection
    {
        /// <summary>
        /// The current
        /// </summary>
        Current,
        /// <summary>
        /// The previous
        /// </summary>
        Previous = -1,
        /// <summary>
        /// The next
        /// </summary>
        Next = 1,
    }
    #endregion
#endif
#if Texts || GWS || Window
    #region IMAGE POSITION
    /// <summary>
    /// Enum ImagePosition
    /// </summary>
    public enum ImagePosition
    {
        /// <summary>
        /// The before text
        /// </summary>
        BeforeText = 0x0,
        /// <summary>
        /// The after text
        /// </summary>
        AfterText = 0x1,
        /// <summary>
        /// The above text
        /// </summary>
        AboveText = 0x2,
        /// <summary>
        /// The below text
        /// </summary>
        BelowText = 0x4,
        /// <summary>
        /// The overlay
        /// </summary>
        Overlay = 0x8,
    }
    #endregion

    #region IMAGE DRAW
    /// <summary>
    /// Enum ImageDraw
    /// </summary>
    public enum ImageDraw
    {
        /// <summary>
        /// The un scaled
        /// </summary>
        UnScaled = 0x1,
        /// <summary>
        /// The scaled
        /// </summary>
        Scaled = 0x2,
        /// <summary>
        /// The disabled
        /// </summary>
        Disabled = 0x4
    }
    #endregion

    #region CONTENT ALIGNMENT
    /// <summary>
    /// Enum for specifying alignment of content in respect of host.
    /// </summary>
    public enum ContentAlignment
    {
        /// <summary>
        /// Places an element on top-left of the container.
        /// </summary>
        TopLeft = 1,

        /// <summary>
        /// Places an element on top-center of the container.
        /// </summary>
        TopCenter = 2,

        /// <summary>
        /// Places an element on top- right of the container.
        /// </summary>
        TopRight = 4,

        /// <summary>
        /// Places an element on middle-left of the container.
        /// </summary>
        MiddleLeft = 16,

        /// <summary>
        /// Places an element on middle-center of the container.
        /// </summary>
        MiddleCenter = 32,

        /// <summary>
        /// Places an element on middle-right of the container.
        /// </summary>
        MiddleRight = 64,

        /// <summary>
        /// Places an element on bottom-left of the container.
        /// </summary>
        BottomLeft = 256,

        /// <summary>
        /// Places an element on bottom-center of the container.
        /// </summary>
        BottomCenter = 512,

        /// <summary>
        /// Places an element on bottom-right of the container.
        /// </summary>
        BottomRight = 1024
    }
    #endregion

    #region TEXT BREAKER
    /// <summary>
    /// Breaks a block of text according to option chosen.
    /// </summary>
    public enum TextBreaker
    {
        /// <summary>
        /// No breaking.
        /// </summary>
        None,
        /// <summary>
        /// Breaks the text by word which means every word will get written in a separate line.
        /// </summary>
        Word,
        /// <summary>
        /// Breaks the text by new line character i.e /r or /n or /rn. Basically, carriage return characters.
        /// </summary>
        Line,
        /// <summary>
        /// Breakes the line on first word only.
        /// </summary>
        SingleWord
    }
    #endregion

    #region FONT MODE
    [Flags]
    public enum FontMode
    {
        Regular = 0x00,
        StrikeThrough = 0x01,
        Underline = 0x02,
        Bold = 0x04,
        Italic = 0x08,
        Oblique = 0x10,
    }
    #endregion

    #region TEXT STYLE
    [Flags]
    public enum TextStyle
    {
        None,
        Strikeout = FontMode.StrikeThrough,
        Underline = FontMode.Underline,
        Both = Strikeout | Underline,
        OutLine = 0x10,
    }
    #endregion

    #region BREAK DELIMITER
    public enum BreakDelimiter
    {
        None,
        Character,
        Word
    }
    #endregion

#if Advanced
    [System.Flags]
    public enum CaretState
    {
        Right = 0x0,
        Left = 0x1,
        Mouse = 0x2,
        Key = 0x4,
        Selection = 0x8,
        WordSelection = 0x10,
        Backward = 0x20,
        Forward = 0x40,
        Horizontal = 0x80,
        Vertical = 0x100,
        SelectionClear = 0x200,

        MouseDrag = Mouse | 0x400,
        MouseProxy = Mouse | 0x800,
        MouseDirect = Mouse | 0x1000,

        KeyLeft = Key | Horizontal | Backward | 0x2000,
        KeyRight = Key | Horizontal | Forward | 0x4000,
        KeyUp = MouseProxy | Vertical | Backward | 0x8000,
        KeyDn = MouseProxy | Vertical | Forward | 0x10000,
        KeyPgUp = MouseProxy | Vertical | Backward | 0x20000,
        KeyPgDn = MouseProxy | Vertical | Forward | 0x40000,
        KeyHome = Key | Vertical | Backward | 0x80000,
        KeyEnd = Key | Vertical | Forward | 0x100000,

        XForward = Mouse | Horizontal | Forward,
        XBackward = Mouse | Horizontal | Backward,
        YForward = Mouse | Vertical | Forward,
        YBackward = Mouse | Vertical | Backward,
    }
#endif
#endif

}
