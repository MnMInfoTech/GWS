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
    #region KEY
    public enum Key
    {
        Modifiers = -65536,
        None = 0,
        LButton = 1,
        RButton = 2,
        Cancel = 3,
        MButton = 4,
        XButton1 = 5,
        XButton2 = 6,
        Back = 8,
        Tab = 9,
        LineFeed = 10,
        Clear = 12,
        Return = 13,
        Enter = 13,
        ShiftKey = 16,
        ControlKey = 17,
        Menu = 18,
        Pause = 19,
        Capital = 20,
        CapsLock = 20,
        KanaMode = 21,
        HanguelMode = 21,
        HangulMode = 21,
        JunjaMode = 23,
        FinalMode = 24,
        HanjaMode = 25,
        KanjiMode = 25,
        Escape = 27,
        IMEConvert = 28,
        IMENonconvert = 29,
        IMEAccept = 30,
        IMEAceept = 30,
        IMEModeChange = 31,
        Space = 32,
        Prior = 33,
        PageUp = 33,
        Next = 34,
        PageDown = 34,
        End = 35,
        Home = 36,
        Left = 37,
        Up = 38,
        Right = 39,
        Down = 40,
        Select = 41,
        Print = 42,
        Execute = 43,
        Snapshot = 44,
        PrintScreen = 44,
        Insert = 45,
        Delete = 46,
        Help = 47,
        D0 = 48,
        D1 = 49,
        D2 = 50,
        D3 = 51,
        D4 = 52,
        D5 = 53,
        D6 = 54,
        D7 = 55,
        D8 = 56,
        D9 = 57,
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,
        LWin = 91,
        RWin = 92,
        Apps = 93,
        Sleep = 95,
        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,
        Multiply = 106,
        Add = 107,
        Separator = 108,
        Subtract = 109,
        Decimal = 110,
        Divide = 111,
        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,
        F13 = 124,
        F14 = 125,
        F15 = 126,
        F16 = 127,
        F17 = 128,
        F18 = 129,
        F19 = 130,
        F20 = 131,
        F21 = 132,
        F22 = 133,
        F23 = 134,
        F24 = 135,
        NumLock = 144,
        Scroll = 145,
        LShiftKey = 160,
        RShiftKey = 161,
        LControlKey = 162,
        RControlKey = 163,
        LMenu = 164,
        RMenu = 165,
        BrowserBack = 166,
        BrowserForward = 167,
        BrowserRefresh = 168,
        BrowserStop = 169,
        BrowserSearch = 170,
        BrowserFavorites = 171,
        BrowserHome = 172,
        VolumeMute = 173,
        VolumeDown = 174,
        VolumeUp = 175,
        MediaNextTrack = 176,
        MediaPreviousTrack = 177,
        MediaStop = 178,
        MediaPlayPause = 179,
        LaunchMail = 180,
        SelectMedia = 181,
        LaunchApplication1 = 182,
        LaunchApplication2 = 183,
        OemSemicolon = 186,
        Oem1 = 186,
        Oemplus = 187,
        Oemcomma = 188,
        OemMinus = 189,
        OemPeriod = 190,
        OemQuestion = 191,
        Oem2 = 191,
        OemTilde = 192,
        Oem3 = 192,
        OemOpenBrackets = 219,
        Oem4 = 219,
        OemPipe = 220,
        Oem5 = 220,
        OemCloseBrackets = 221,
        Oem6 = 221,
        OemQuotes = 222,
        Oem7 = 222,
        Oem8 = 223,
        OemSlash = 225,
        OemBackslash = 226,
        Oem102 = 226,
        ProcessKey = 229,
        Packet = 231,
        Attn = 246,
        Crsel = 247,
        Exsel = 248,
        EraseEof = 249,
        Play = 250,
        Zoom = 251,
        NoName = 252,
        Pa1 = 253,
        OemClear = 254,
        KeyCode = 65535,
        Shift = 65536,
        Control = 131072,
        Alt = 262144,
        LAlt = Alt | 1000,
        RALT = Alt | 2000,
        LastKey = 196,
    }
    #endregion

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
