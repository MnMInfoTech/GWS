/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;

namespace MnM.GWS
{
    #region BENCHMARK UNIT
    public enum Unit
    {
        MilliSecond,
        Tick,
        Second,
        MicroSecond
    }
    #endregion

    #region MOUSE STATE
    /// <summary>
    /// Represents the state of the mouse.
    /// </summary>
    public enum MouseState
    {
        /// <summary>
        /// No state.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Mouse button up
        /// </summary>
        Up = 0x1,

        /// <summary>
        /// Mouse button down
        /// </summary>
        Down = 0x2,

        /// <summary>
        /// Mouse is moving
        /// </summary>
        Move = 0x4,

        /// <summary>
        /// mouse is clicked
        /// </summary>
        Click = 0x8,

        /// <summary>
        /// Mouse is double clicked
        /// </summary>
        DoubleClick = 0x10,

        /// <summary>
        /// Mouse wheel motion is ongoing.
        /// </summary>
        Wheel = 0x20,

        /// <summary>
        /// Mouse is entered in some container premises
        /// </summary>
        Enter = 0x40,

        /// <summary>
        /// Mouse has left some container premises
        /// </summary>
        Leave = 0x80,

        /// <summary>
        /// Mouse has started a draw operation on something
        /// </summary>
        DragBegin = 0x100,

        /// <summary>
        /// Mouse is dragging something currently
        /// </summary>
        Drag = DragBegin | 0x200,

        /// <summary>
        /// Mouse has finished dragging something
        /// </summary>
        DragEnd = Drag | 0x400
    }
    #endregion

    #region MOUSE BUTTON
    public enum MouseButton
    {
        None = 0,
        /// <summary>
        /// The left mouse button.
        /// </summary>
        Left = 1,
        /// <summary>
        /// The middle mouse button.
        /// </summary>
        Middle,
        /// <summary>
        /// The right mouse button.
        /// </summary>
        Right,
        /// <summary>
        /// The first extra mouse button.
        /// </summary>
        Button1,
        /// <summary>
        /// The second extra mouse button.
        /// </summary>
        Button2,
        /// <summary>
        /// The third extra mouse button.
        /// </summary>
        Button3,
        /// <summary>
        /// The fourth extra mouse button.
        /// </summary>
        Button4,
        /// <summary>
        /// The fifth extra mouse button.
        /// </summary>
        Button5,
        /// <summary>
        /// The sixth extra mouse button.
        /// </summary>
        Button6,
        /// <summary>
        /// The seventh extra mouse button.
        /// </summary>
        Button7,
        /// <summary>
        /// The eigth extra mouse button.
        /// </summary>
        Button8,
        /// <summary>
        /// The ninth extra mouse button.
        /// </summary>
        Button9,
        /// <summary>
        /// Indicates the last available mouse button.
        /// </summary>
        LastButton
    }
    #endregion

    #region KEY STATE
    /// <summary>
    /// Represents state of the keyboard key.
    /// </summary>
    public enum KeyState
    {
        /// <summary>
        /// No key is acted upon
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Key is up
        /// </summary>
        Up = 0x1,

        /// <summary>
        /// Key is down
        /// </summary>
        Down = 0x2,

        /// <summary>
        /// Just after down before up for preview and do something about it.
        /// </summary>
        Preview = Down | 0x4
    }
    #endregion

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

    #region IMAGE FORMAT
    public enum ImageFormat
    {
        JPG = 2,
        BMP = 0,
        HDR = 3,
        TGA = 4,
        PNG = 5,
    }
    #endregion

    #region VALIDATE TYPE
    /// <summary>
    /// Enum ValidateType
    /// </summary>
    public enum ValidateType
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Allows date in dd-MM-yyyy format
        /// </summary>
        ddMMyyyy = 0x1,
        /// <summary>
        /// Allows date in dd-MM-yy format
        /// </summary>
        ddMMyy = 0x2,
        /// <summary>
        /// Allows date in MM-dd-yyyy format
        /// </summary>
        MMddyyyy = 0x4,
        /// <summary>
        /// Allows date in MM-dd-yy format
        /// </summary>
        MMddyy = 0x8,
        /// <summary>
        /// Allows date in yyyy-MM-dd format
        /// </summary>
        yyyyMMdd = 0x10,
        /// <summary>
        /// Allows date in yy-MM-dd format
        /// </summary>
        yyMMdd = 0x20,
        /// <summary>
        /// Allows time in HH:mm format example 23:59
        /// </summary>
        HHmm = 0x40,
        /// <summary>
        /// Allows time in HH:mm format with AM/PM example 11:59 PM
        /// </summary>
        hhmm12 = 0x80,
        /// <summary>
        /// Allows time in HH:mm:SS format example 23:59:23
        /// </summary>
        HHmmss = 0x100,
        /// <summary>
        /// Allows time in HH:mm:SS format with AM/PM example 11:59:23 PM
        /// </summary>
        hhmmss12 = 0x200,
        /// <summary>
        /// Allows time in HH:mm:SS:MS with miliseconds format example 23:59:23:99
        /// </summary>
        HHmmssms = 0x400,
        /// <summary>
        /// The HHMMSSMS12
        /// </summary>
        hhmmssms12 = 0x800,
        /// <summary>
        /// The dd m myyyy h HMM
        /// </summary>
        ddMMyyyyHHmm = ddMMyyyy | HHmm,
        /// <summary>
        /// The dd m myyyyhhmm12
        /// </summary>
        ddMMyyyyhhmm12 = ddMMyyyy | hhmm12,
        /// <summary>
        /// The dd m myyyy h HMMSS
        /// </summary>
        ddMMyyyyHHmmss = ddMMyyyy | HHmmss,
        /// <summary>
        /// The dd m myyyyhhmmss12
        /// </summary>
        ddMMyyyyhhmmss12 = ddMMyyyy | hhmmss12,
        /// <summary>
        /// The dd m myy h HMM
        /// </summary>
        ddMMyyHHmm = ddMMyy | HHmm,
        /// <summary>
        /// The dd m myyhhmm12
        /// </summary>
        ddMMyyhhmm12 = ddMMyy | hhmm12,
        /// <summary>
        /// The dd m myy h HMMSS
        /// </summary>
        ddMMyyHHmmss = ddMMyy | HHmmss,
        /// <summary>
        /// The dd m myyhhmmss12
        /// </summary>
        ddMMyyhhmmss12 = ddMMyy | hhmmss12,
        /// <summary>
        /// The yyyy m MDD h HMM
        /// </summary>
        yyyyMMddHHmm = yyyyMMdd | HHmm,
        /// <summary>
        /// The yyyy m MDDHHMM12
        /// </summary>
        yyyyMMddhhmm12 = yyyyMMdd | hhmm12,
        /// <summary>
        /// The yyyy m MDD h HMMSS
        /// </summary>
        yyyyMMddHHmmss = yyyyMMdd | HHmmss,
        /// <summary>
        /// The yyyy m MDDHHMMSS12
        /// </summary>
        yyyyMMddhhmmss12 = yyyyMMdd | hhmmss12,
        /// <summary>
        /// Allows characters only. a to z or A to Z
        /// </summary>
        Character = 0x1000,
        /// <summary>
        /// Allows numbers only
        /// </summary>
        Number = 0x2000,
        /// <summary>
        /// Allows characters and numbers only  a to z or A to Z or 0 to 9
        /// </summary>
        CharacterAndNumber = Character | Number,
        /// <summary>
        /// Allows anything except numbers i.e. 0 to 9 can not be written
        /// </summary>
        NotNumber = 0x4000,
        /// <summary>
        /// Allows positive intiger numbers only
        /// </summary>
        PositiveNumber = 0x8000,
        /// <summary>
        /// Allows decimal number only
        /// </summary>
        DecimalNumber = 0x10000,
        /// <summary>
        /// Allows positive decimal numbers only
        /// </summary>
        PositiveDecimalNumber = PositiveNumber | DecimalNumber,
        /// <summary>
        /// Allows numbers with + or - signs
        /// </summary>
        NumberWithPlusAndMinus = 0x20000,
        /// <summary>
        /// Allows number with +,-,/,*,% signs
        /// </summary>
        NumberWithCalculatingSigns = 0x40000,
        /// <summary>
        /// Allows true, false, 0 and -1 only
        /// </summary>
        Boolean = 0x80000
    }
    #endregion

    #region AND/OR
    /// <summary>
    /// Enum AndOr
    /// </summary>
    public enum AndOr
    {
        /// <summary>
        /// The or
        /// </summary>
        OR,
        /// <summary>
        /// The and
        /// </summary>
        AND
    }
    #endregion

    #region CRITERIA
    /// <summary>
    /// Enum Criteria
    /// </summary>
    public enum Criteria
    {
        /// <summary>
        /// The equal
        /// </summary>
        Equal = 0,
        /// <summary>
        /// The greater than
        /// </summary>
        GreaterThan = 1,
        /// <summary>
        /// The less than
        /// </summary>
        LessThan = 2,
        /// <summary>
        /// The occurs
        /// </summary>
        Occurs = 3,
        /// <summary>
        /// The begins with
        /// </summary>
        BeginsWith = 4,
        /// <summary>
        /// The ends with
        /// </summary>
        EndsWith = 5,
        /// <summary>
        /// The occurs no case
        /// </summary>
        OccursNoCase = 6,
        /// <summary>
        /// The begins with no case
        /// </summary>
        BeginsWithNoCase = 7,
        /// <summary>
        /// The ends with no case
        /// </summary>
        EndsWithNoCase = 8,
        /// <summary>
        /// The string equal
        /// </summary>
        StringEqual = 9,
        /// <summary>
        /// The string equal no case
        /// </summary>
        StringEqualNoCase = 10,
        /// <summary>
        /// The string number greater than
        /// </summary>
        StringNumGreaterThan = 11,
        /// <summary>
        /// The string number less than
        /// </summary>
        StringNumLessThan = 12,
        /// <summary>
        /// The not equal
        /// </summary>
        NotEqual = -1,
        /// <summary>
        /// The not greater than
        /// </summary>
        NotGreaterThan = -2,
        /// <summary>
        /// The not less than
        /// </summary>
        NotLessThan = -3,
        /// <summary>
        /// The not occurs
        /// </summary>
        NotOccurs = -4,
        /// <summary>
        /// The not begins with
        /// </summary>
        NotBeginsWith = -5,
        /// <summary>
        /// The not ends with
        /// </summary>
        NotEndsWith = -6,
        /// <summary>
        /// The not occurs no case
        /// </summary>
        NotOccursNoCase = -7,
        /// <summary>
        /// The not begins with no case
        /// </summary>
        NotBeginsWithNoCase = -8,
        /// <summary>
        /// The not ends with no case
        /// </summary>
        NotEndsWithNoCase = -9,
        /// <summary>
        /// The not string equal
        /// </summary>
        NotStrEqual = -10,
        /// <summary>
        /// The not string equal no case
        /// </summary>
        NotStrEqualNoCase = -11,
        /// <summary>
        /// The not string greater than
        /// </summary>
        NotStringGreaterThan = -12,
        /// <summary>
        /// The not string less than
        /// </summary>
        NotStringLessThan = -13
    }
    #endregion

    #region MULTI CRITERIA
    /// <summary>
    /// Enum MultCriteria
    /// </summary>
    public enum MultCriteria
    {
        /// <summary>
        /// The between
        /// </summary>
        Between = 0,
        /// <summary>
        /// The not between
        /// </summary>
        NotBetween = -1,
    }
    #endregion

    #region NUMERIC-CRITERIA
    public enum NumCriteria
    {
        None = 0,

        /// <summary>
        /// The equal
        /// </summary>
        Equal = 1,

        /// <summary>
        /// The greater than
        /// </summary>
        GreaterThan = 2,

        /// <summary>
        /// The less than
        /// </summary>
        LessThan = 3,

        /// <summary>
        /// <summary>
        /// The not equal
        /// </summary>
        NotEqual = -1,

        /// <summary>
        /// The not greater than
        /// </summary>
        NotGreaterThan = -2,

        /// <summary>
        /// The not less than
        /// </summary>
        NotLessThan = -3,
    }
    #endregion

    #region MATCH BY
    /// <summary>
    /// Enum MatchBy
    /// </summary>
    public enum MatchBy
    {
        /// <summary>
        /// The key
        /// </summary>
        Key,
        /// <summary>
        /// The value
        /// </summary>
        Value,

        Item,
    }
    #endregion

    #region BINDING FLAGS TYPE
    /// <summary>
    /// Enum BindingFlagType
    /// </summary>
    public enum BindingFlagType
    {
        /// <summary>
        /// All member
        /// </summary>
        AllMember,
        /// <summary>
        /// The public instance
        /// </summary>
        PublicInstance,
        /// <summary>
        /// The non public instance
        /// </summary>
        NonPublicInstance,
        /// <summary>
        /// The public static
        /// </summary>
        PublicStatic,
        /// <summary>
        /// The non public static
        /// </summary>
        NonPublicStatic,
        /// <summary>
        /// The type initializer
        /// </summary>
        TypeInitializer,
        /// <summary>
        /// The public non inherited
        /// </summary>
        PublicNonInherited,

#if NETSTANDARD2_0
        /// <summary>
        /// The exact matching
        /// </summary>
        ExactMatching,
#endif
        /// <summary>
        /// All static
        /// </summary>
        AllStatic,
        /// <summary>
        /// All properties
        /// </summary>
        AllProperties,
    }
    #endregion

    #region EXTRACT INTERFACES
    /// <summary>
    /// Enum ExtractInterfaces
    /// </summary>
    public enum ExtractInterfaces
    {
        /// <summary>
        /// The these only
        /// </summary>
        TheseOnly,
        /// <summary>
        /// The exclude these
        /// </summary>
        ExcludeThese,
        /// <summary>
        /// All prioritize these
        /// </summary>
        AllPrioritizeThese
    }
    #endregion

    #region EXCLUDE NESTED PARAMS
    /// <summary>
    /// Enum ExcludeNestedParams
    /// </summary>
    public enum ExcludeNestedParams
    {
        /// <summary>
        /// The both genre
        /// </summary>
        BothGenre,
        /// <summary>
        /// The no exclusion
        /// </summary>
        NoExclusion,
        /// <summary>
        /// The host genre
        /// </summary>
        HostGenre,
        /// <summary>
        /// The other genre
        /// </summary>
        OtherGenre,
    }
    #endregion

    #region INSTANCE
    /// <summary>
    /// Enum Instance
    /// </summary>
    public enum Instance
    {
        /// <summary>
        /// The normal
        /// </summary>
        Normal,
        /// <summary>
        /// The reference
        /// </summary>
        Reference,
        /// <summary>
        /// The array
        /// </summary>
        Array,
        /// <summary>
        /// The pointer
        /// </summary>
        Pointer
    }
    #endregion

    #region GET LIST
    /// <summary>
    /// Enum GetList
    /// </summary>
    public enum GetList
    {
        /// <summary>
        /// The mn m list
        /// </summary>
        MnMList,
        /// <summary>
        /// The mn m collection
        /// </summary>
        MnMCollection,
        /// <summary>
        /// The stream
        /// </summary>
        List,
        /// <summary>
        /// The stack
        /// </summary>
        Stack,
        /// <summary>
        /// The queue
        /// </summary>
        Queue,
        /// <summary>
        /// The hash set
        /// </summary>
        HashSet
    }
    #endregion

    #region OPERAND
    /// <summary>
    /// Enum Operand
    /// </summary>
    public enum Operand
    {
        /// <summary>
        /// The left
        /// </summary>
        Left,
        /// <summary>
        /// The right
        /// </summary>
        Right
    }
    #endregion

    #region TIMEFORMAT
    /// <summary>
    /// Enum TimeFormat
    /// </summary>
    public enum TimeFormat
    {
        /// <summary>
        /// The time24
        /// </summary>
        Time24,
        /// <summary>
        /// The time12
        /// </summary>
        Time12
    }
    #endregion

    #region COMPARERETURN
    /// <summary>
    /// Enum CompareReturn
    /// </summary>
    public enum CompareReturn
    {
        /// <summary>
        /// The small
        /// </summary>
        Small,
        /// <summary>
        /// The big
        /// </summary>
        Big,
        /// <summary>
        /// The nearest
        /// </summary>
        Nearest
    }
    #endregion

    #region PRESS STATE
    /// <summary>
    /// Mose state
    /// </summary>
    public enum State
    {
        /// <summary>
        /// Mouse was pressed before and now relaesed
        /// </summary>
        Released = 1,

        /// <summary>
        /// Mouse was in released state and now pressed
        /// </summary>
        Pressed = 2,
    }
    #endregion

    #region EXPR TYPE
    /// <summary>
    /// Enum ExprType
    /// </summary>
    public enum ExprType
    {
        /// <summary>
        /// The none
        /// </summary>
        None,

        /// <summary>
        /// The function
        /// </summary>
        Function,

        /// <summary>
        /// The math
        /// </summary>
        Math,

        /// <summary>
        /// The keyword
        /// </summary>
        Keyword,

        /// <summary>
        /// The class
        /// </summary>
        Class,

        /// <summary>
        /// The array
        /// </summary>
        Array,

        /// <summary>
        /// The new
        /// </summary>
        New,

        /// <summary>
        /// The variable array
        /// </summary>
        VarArray,

        NumericSuffix,

        NameSpace,
    }
    #endregion

    #region MATH OPERATOR
    /// <summary>
    /// Enum MathOperator
    /// </summary>
    public enum MathOperator
    {
        None = 0x0,

        /// <summary>
        /// The add
        /// </summary>
        Add = 0x1,

        /// <summary>
        /// The multiply
        /// </summary>
        Multiply = 0x2,

        /// <summary>
        /// The subtract
        /// </summary>
        Subtract = 0x4,

        /// <summary>
        /// The divide
        /// </summary>
        Divide = 0x8,

        /// <summary>
        /// The modulo
        /// </summary>
        Modulo = 0x10,

        /// <summary>
        /// The negate
        /// </summary>
        Negate = 0x20,

        /// <summary>
        /// The compare
        /// </summary>
        Compare = 0x40,

        All = Add | Multiply | Subtract |
            Divide | Modulo | Negate | Compare,
    }
    #endregion

    #region POSITION
    [Flags]
    public enum Position
    {
        Default = 0x0,
        Left = 0x1,
        Top = 0x2,
        Right = 0x4,
        Bottom = 0x8,
        All = Left | Top | Right | Bottom,
    }
    #endregion

    #region SKEW TYPE
    [Flags]
    public enum SkewType
    {
        None = 0x0,
        Horizontal = 0x1,
        Vertical = 0x2,
        Diagonal = 0x4,
        Downsize = Horizontal | Vertical,
    }
    #endregion

    #region BLOCK-COPY
    public enum CopyCondition
    {
        NotEqual,
        Equal,
    }
    #endregion
}
