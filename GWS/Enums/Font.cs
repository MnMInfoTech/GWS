
#if (GWS || Window)
using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    #region TEXT COMMAND
    [Flags]
    public enum TextCommand : uint
    {
        /// <summary>
        /// The none
        /// </summary>
        None = Numbers.Flag0,

        /// <summary>
        /// Aligns text to left corner - parent rectangle is required to exercise this option.
        /// </summary>
        Left = Numbers.Flag1,

        /// <summary>
        /// Aligns text to top corner - parent rectangle is required to exercise this option
        /// </summary>
        Top = Numbers.Flag2,

        /// <summary>
        /// Aligns text to right corner - parent rectangle is required to exercise this option
        /// </summary>
        Right = Numbers.Flag3,

        /// <summary>
        /// Aligns text to bottom corner - parent rectangle is required to exercise this option
        /// </summary>
        Bottom = Numbers.Flag4,

        /// <summary>
        /// Text case is converted to title style - first character of each word is capital character i.e "How Are You?"
        /// </summary>
        Title = Numbers.Flag5,

        /// <summary>
        /// Text case is converted to sentence style - first character of 1st word is capital character i.e "How are you?"
        /// </summary>
        Sentence = Numbers.Flag6,

        /// <summary>
        /// Text case is converted to upper style - each charcter is capital character i.e "HOW ARE YOU?"
        /// </summary>
        Upper = Numbers.Flag7,

        /// <summary>
        /// Text case is converted to LOWER style - each charcter is non capital character i.e "how are you?"
        /// </summary>
        Lower = Numbers.Flag8,

        /// <summary>
        /// Text case is converted to toggle style - each charcter is converted to lowercase if it is originally in
        /// uppercase and viceversa i.e "How Are You?" becomes "hOW aRE yOU?"
        /// </summary>
        Toggle = Numbers.Flag9,

        /// <summary>
        /// Breaks the text by word which means every word will get written in a separate line.
        /// </summary>
        Word = Numbers.Flag10,

        /// <summary>
        /// Breaks the text by new line character i.e /r or /n or /rn. Basically, carriage return characters.
        /// </summary>
        Line = Numbers.Flag11,

        /// <summary>
        /// Breakes the line on first word only.
        /// </summary>
        SingleWord = Numbers.Flag12,

        /// <summary>
        /// Breaks the line on every single character.
        /// </summary>
        SingleChar = Numbers.Flag13,

        /// <summary>
        /// Breaks the text in next next line on each occurance of character suppied as Character delimiter.
        /// </summary>
        CharacterDelimiter = Numbers.Flag14,

        /// <summary>
        /// Breaks the line on every word.
        /// </summary>
        WordDelimiter = Numbers.Flag15,

        /// <summary>
        /// Draws hollow characters
        /// </summary>
        OutLine = Numbers.Flag16,

        /// <summary>
        /// Draws strike out effect on text.
        /// </summary>
        Strikeout = Numbers.Flag17,

        /// <summary>
        /// Draws underline effect on text.
        /// </summary>
        Underline = Numbers.Flag18,

        /// <summary>
        /// Considers only characters that are letters.
        /// </summary>
        CharcterOnly = Numbers.Flag19,

        /// <summary>
        /// Considers only characters that are numbers.
        /// </summary>
        NumberOnly = Numbers.Flag20,

        /// <summary>
        /// Considers only characters that are numbers or letters.
        /// </summary>
        CharacterAndNumberOnly = Numbers.Flag21,

        /// <summary>
        /// Considers only characters that are special characters such as @, # etc.
        /// </summary>
        SpecialCharactersOnly = Numbers.Flag22,

        /// <summary>
        /// Considers only characters that are letters special characters such as @, # etc.
        /// </summary>
        CharacterAndSpecialCharactersOnly = Numbers.Flag23,

        /// <summary>
        /// Considers only characters that are numbers and special characters such as @, # etc.
        /// </summary>
        NumberAndSpecialCharactersOnly = Numbers.Flag24,

        /// <summary>
        /// Only considers drawing upto the width specified.
        /// </summary>
        FixedWidth = Numbers.Flag25,

        /// <summary>
        /// 
        /// </summary>
        Middle = Top | Bottom,

        /// <summary>
        /// 
        /// </summary>
        Center = Left | Top,

        /// <summary>
        /// Aligns text to top-left corner - parent rectangle is required to exercise this option
        /// </summary>
        TopLeft = Top | Left,

        /// <summary>
        /// Aligns text to top-center corner - parent rectangle is required to exercise this option
        /// </summary>
        TopCenter = Top | Center,

        /// <summary>
        /// Aligns text to top-right corner - parent rectangle is required to exercise this option
        /// </summary>
        TopRight = Top | Right,

        /// <summary>
        /// Places text to middle-left spot - parent rectangle is required to exercise this option
        /// </summary>
        MiddleLeft = Middle | Left,

        /// <summary>
        /// Places text to middle-center spot - parent rectangle is required to exercise this option
        /// </summary>
        MiddleCenter = Middle | Center,

        /// <summary>
        /// Places text to middle-right spot - parent rectangle is required to exercise this option
        /// </summary>
        MiddleRight = Middle | Right,

        /// <summary>
        /// Places text to bottom-left corner - parent rectangle is required to exercise this option
        /// </summary>
        BottomLeft = Bottom | Left,

        /// <summary>
        /// Places text to bottom-center spot - parent rectangle is required to exercise this option
        /// </summary>
        BottomCenter = Bottom | Center,

        /// <summary>
        /// Places text to bottom-right conrner - parent rectangle is required to exercise this option
        /// </summary>
        BottomRight = Bottom | Right,

        /// <summary>
        /// 
        /// </summary>
        Both = Strikeout | Underline,
    }
    #endregion

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
        /// The sentence i.e "How are you?"
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

    #region FONT MODE
    [Flags]
    public enum FontMode
    {
        Regular = Numbers.Flag0,
        StrikeThrough = Numbers.Flag1,
        Underline = Numbers.Flag2,
        Bold = Numbers.Flag3,
        Italic = Numbers.Flag4,
        Oblique = Numbers.Flag5,
    }
    #endregion

    #region FONT WEIGHT
    /// <summary>
    /// Specifies various font weights.
    /// </summary>
    public enum FontWeight : ushort
    {
        /// <summary>
        /// The weight is unknown or unspecified.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Very thin.
        /// </summary>
        Thin = 100,

        /// <summary>
        /// Extra light.
        /// </summary>
        ExtraLight = 200,

        /// <summary>
        /// Light.
        /// </summary>
        Light = 300,

        /// <summary>
        /// Normal.
        /// </summary>
        Normal = 400,

        /// <summary>
        /// Medium.
        /// </summary>
        Medium = 500,

        /// <summary>
        /// Somewhat bold.
        /// </summary>
        SemiBold = 600,

        /// <summary>
        /// Bold.
        /// </summary>
        Bold = 700,

        /// <summary>
        /// Extra bold.
        /// </summary>
        ExtraBold = 800,

        /// <summary>
        /// Extremely bold.
        /// </summary>
        Black = 900
    }
    #endregion

    #region FONT STRETCH
    /// <summary>
    /// Specifies the font stretching level.
    /// </summary>
    public enum FontStretch : byte
    {
        /// <summary>
        /// The stretch is unknown or unspecified.
        /// </summary>
        Unknown,

        /// <summary>
        /// Ultra condensed.
        /// </summary>
        UltraCondensed,

        /// <summary>
        /// Extra condensed.
        /// </summary>
        ExtraCondensed,

        /// <summary>
        /// Condensed.
        /// </summary>
        Condensed,

        /// <summary>
        /// Somewhat condensed.
        /// </summary>
        SemiCondensed,

        /// <summary>
        /// Normal.
        /// </summary>
        Normal,

        /// <summary>
        /// Somewhat expanded.
        /// </summary>
        SemiExpanded,

        /// <summary>
        /// Expanded.
        /// </summary>
        Expanded,

        /// <summary>
        /// Extra expanded.
        /// </summary>
        ExtraExpanded,

        /// <summary>
        /// Ultra expanded.
        /// </summary>
        UltraExpanded
    }
    #endregion
}
#endif