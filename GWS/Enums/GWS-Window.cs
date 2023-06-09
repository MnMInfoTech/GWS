/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if (GWS || Window)
using System;

namespace MnM.GWS
{
#if (!Advance)
#endif
}

namespace MnM.GWS
{
    #region VIEW STATE
    [Flags]
#if DevSupport
    public
#else
    internal
#endif
    enum ViewState : ushort
    {
        /// <summary>
        /// Default state - nothing special.
        /// </summary>
        None = Numbers.Flag0,

        /// <summary>
        /// Tells GWS that redrawing of the object is required.
        /// </summary>
        NeedRefresh = Numbers.Flag1,

        /// <summary>
        /// Tells GWS that object is currently busy in loading or refreshing hence very limited operations are allowed.
        /// </summary>
        Busy = Numbers.Flag2,

        /// <summary>
        /// Tells GWS that since view is disposed rendering process is non executable.
        /// </summary>
        Disposed = Numbers.Flag3,

        /// <summary>
        /// Tells GWS that the view is hidden hence no operations are allowed.
        /// </summary>
        Hidden = Numbers.Flag4,

        /// <summary>
        /// Tells GWS that since view is disabled it can no longer process inputs.
        /// </summary>
        Disabled = Numbers.Flag5,

        /// <summary>
        /// The view is minimized to the taskbar (also known as 'iconified').
        /// </summary>
        Minimized = Numbers.Flag6,

        /// <summary>
        /// The view covers the whole working area, which includes the desktop but not the taskbar and/or panels.
        /// </summary>
        Maximized = Numbers.Flag7,

        /// <summary>
        /// Indicates that while changing location or size or other properties which changes shape of the view,
        /// redraw will not occur i.e view will not immediately reflect those changes.
        /// </summary>
        SuspendedLayout = Numbers.Flag8,

        /// <summary>
        /// Tells GWS that all view being rendered are in-fact rendered first time since their creation.
        /// </summary>
        FullyLoaded = Numbers.Flag9,

        /// <summary>
        /// Tells GWS that rendering process is only to perform erasing 
        /// task of the object keeping in mind that object is moved.
        /// </summary>
        DroppingDrag = Numbers.Flag10,

        /// <summary>
        /// 
        /// </summary>
        Loading = Numbers.Flag11,

        /// <summary>
        /// Indicates that this object is fully obscured by other objects.
        /// </summary>
        FullyObscured = Numbers.Flag12,

        /// <summary>
        /// Indicates that this object is partially obscured by other objects.
        /// </summary>
        PartiallyObscured = Numbers.Flag13,

        /// <summary>
        /// Tells GWS that the object is currently drawing itself on an external surface.
        /// </summary>
        DrawingExternally = Numbers.Flag14,
    }
    #endregion

    #region AFTER STROKE
    [Flags]
    public enum AfterStroke : byte
    {
        /// <summary>
        /// Do nothing.!!!!
        /// </summary>
        None = Numbers.Flag0,

        /// <summary>
        /// Normally Strokes are drawn from the second point on the line.
        /// In some cases we need to rest the points that have been removed.
        /// For example: an Arc which must close the ends of the stroke perimiter lines and so the end points are required.
        /// </summary>
        Reset1st = Numbers.Flag1,

        /// <summary>
        /// Joins the end points of the two lines on the perimeter of the stroke and joins them. 
        /// Example the ends of a stroke drawing an arc must be closed.
        /// </summary>
        JoinEnds = Numbers.Flag2,

        /// <summary>
        /// Used for Strokes that define a perimiter that has a start and end such as an Arc.
        /// </summary>
        Both = Reset1st | JoinEnds,
    }
    #endregion

    #region ROUNDBOX OPTION
    [Flags]
    public enum RoundBoxOption : byte
    {
        Normal = Numbers.Flag0,

        /// <summary>
        /// Left top corner.
        /// </summary>
        LT = Numbers.Flag1,

        /// <summary>
        /// Top right corner.
        /// </summary>
        TR = Numbers.Flag2,

        /// <summary>
        /// Right bottom corner.
        /// </summary>
        RB = Numbers.Flag3,

        /// <summary>
        /// Bottom left corner.
        /// </summary>
        BL = Numbers.Flag4,

        Bresenham = Numbers.Flag5,

        /// <summary>
        /// Left top & right top corners
        /// </summary>
        LTRT = LT | TR,

        /// <summary>
        /// Bottom left & right bottom
        /// </summary>
        LBRB = BL | RB,

        /// <summary>
        /// Left top & right bottm
        /// </summary>
        LTRB = LT | RB,

        /// <summary>
        /// Left bottom & right bottom
        /// </summary>
        LBRT = BL | TR,
    }
    #endregion

    #region BEZIER TYPE
    public enum BezierType : byte
    {
        Cubic = 4,
        Quadratric = 3,
        Multiple = 7,
    }
    #endregion

    #region CURVE TYPE
    [Flags]
    public enum CurveType : ushort
    {
        /// <summary>
        /// Represents full ellipse or circle.
        /// </summary>
        Full = Numbers.Flag0,

        /// <summary>
        /// Represents an arc with start and sweep/end angles.
        /// </summary>
        Arc = Numbers.Flag1,

        /// <summary>
        /// Represents a pie with start and sweep/end angles.
        /// </summary>
        Pie = Numbers.Flag2,

        /// <summary>
        /// By default, end angle is sweeped i.e end angle += start angle.
        /// If this option is added, arc or pie will be created using absolute end angle value.
        /// </summary>
        NoSweepAngle = Numbers.Flag3,

        /// <summary>
        /// Draws an arc or pie cutting it in anti clock wise motion.
        /// i.e. if start-angle 0 and end-angle = 90, this option will draw an arc or pie from 90 to 360.
        /// </summary>
        AntiClock = Numbers.Flag4,

        /// <summary>
        /// Fills an area of Arc betwwen start and end points.
        /// </summary>
        ClosedArc = Arc | Numbers.Flag5,

        /// <summary>
        /// Draws an arc or pie or ellipse fitting the boundary defined by three points given. 
        /// </summary>
        Fitting = Numbers.Flag6,

        /// <summary>
        /// Only applicable when ellipse/ arc / pie is created using theree points. And Fitting flag is not included.
        /// This flag will make sure that the third point will not be treated as center of ellipsoid but rather a point on ellipsoid itself.
        /// </summary>
        ThirdPointOnEllipse = Numbers.Flag7,

        /// <summary>
        /// Only applicable when ellipse/ arc / pie is created using four points. And Fitting flag is not included.
        /// This flag will make sure that the fourth point will not be treated a point on ellipsoid itself but rather as center of ellipsoid.
        /// This will also mean that first and second points are guranteed on ellipse and third one can not be so.
        /// </summary>
        FourthPointIsCenter = Numbers.Flag8,

        /// <summary>
        /// Only applicable in stroked curve. 
        /// If applied, this option will swap centres of outer curve and inner curve which
        /// in effect creates cross stroking effcet i.e outer curve is cut by inner curve arcline and 
        /// inner curve is cut by outer curve arcline.
        /// </summary>
        CrossStroke = Numbers.Flag9,

        /// <summary>
        /// 
        /// </summary>
        AbsolutePieAngle = Numbers.Flag10,
    }
    #endregion

    #region LINE DIRECTION
    [Flags]
    public enum LineDirection : byte
    {
        /// <summary>
        /// Diagonal line.
        /// </summary>
        Diagonal = Numbers.Flag0,

        /// <summary>
        /// Horizontal line i.e. y1 = y2 or difference between them is 0.0001f
        /// </summary>
        Horizontal = Numbers.Flag1,

        /// <summary>
        /// Vertical line i.e. x1 = x2 or difference between them is 0.0001f
        /// </summary>        
        Vertical = Numbers.Flag2,

        /// <summary>
        /// Just a point i.e. deifference between x1 and x2 as well as y1 and y2 is less than 0.0001f.
        /// </summary>
        Point = Numbers.Flag3,
    }
    #endregion

    #region CONIC TYPE
    public enum ConicType : byte
    {
        Ellipse,
        Hyperbola,
        Parabola,
    }
    #endregion

    #region QUAD TYPE
    public enum QuadType : byte
    {
        Rhombus,
        Trapezium,
        Trapezoid,
        RoundBox,
    }
    #endregion

    #region EDGEDETECTION MODE
    public enum EdgeDetectionMode : byte
    {
        Canny,
        Kovalevsky,
        Sobel,
        Prewitt,
        Laplacian,
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
        BeforeText,

        /// <summary>
        /// The after text
        /// </summary>
        AfterText,

        /// <summary>
        /// The above text
        /// </summary>
        AboveText,

        /// <summary>
        /// The below text
        /// </summary>
        BelowText,

        /// <summary>
        /// The overlay
        /// </summary>
        Overlay,
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
        UnScaled,

        /// <summary>
        /// The scaled
        /// </summary>
        Scaled,

        /// <summary>
        /// The disabled
        /// </summary>
        Disabled,
    }
    #endregion

    #region RECT MOVEMENT
    public enum RectMovement : byte
    {
        Up = 38,

        Down = 40,

        Left = 37,

        Right = 39
    }
    #endregion

    #region POINT SHIFT
    /// <summary>
    /// Enum PointShift
    /// </summary>
    public enum PointShift
    {
        /// <summary>
        /// The backward
        /// </summary>
        Backward,
        /// <summary>
        /// The forward
        /// </summary>
        Forward
    }
    #endregion
    //--------------------------------------------------------

    #region MSGBOX BUTTONS
    public enum MsgBoxButtons
    {
        YesNo,
        OkCancel,
        Information,
        Error,
        AbortRetry,
    }
    public enum MsgBoxResult
    {
        None,
        Yes,
        No,
        Ok,
        Cancel,
        Information,
        Error,
        Abort,
        Retry,
        Ignore,
    }
    #endregion

    #region MOUSE STATE
    [Flags]
    /// <summary>
    /// Represents the state of the mouse.
    /// </summary>
    public enum MouseState: byte
    {
        /// <summary>
        /// No state.
        /// </summary>
        None = Numbers.Flag0,

        /// <summary>
        /// Mouse button up
        /// </summary>
        Up = Numbers.Flag1,

        /// <summary>
        /// Mouse button down
        /// </summary>
        Down = Numbers.Flag2,

        /// <summary>
        /// Mouse is moving
        /// </summary>
        Move = Numbers.Flag3,

        /// <summary>
        /// mouse is clicked
        /// </summary>
        Click = Numbers.Flag4,

        /// <summary>
        /// Mouse is double clicked
        /// </summary>
        DoubleClick = Numbers.Flag5,

        /// <summary>
        /// Mouse wheel motion is ongoing.
        /// </summary>
        Wheel = Numbers.Flag6,

        /// <summary>
        /// Mouse is entered in some container premises
        /// </summary>
        Enter = Numbers.Flag7,

        /// <summary>
        /// Mouse has left some container premises
        /// </summary>
        Leave = Numbers.Flag8,
    }
    #endregion

    #region MOUSE BUTTON
    public enum MouseButton : byte
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

    [Flags]
    public enum KeyState : byte
    {
        /// <summary>
        /// No key is acted upon
        /// </summary>
        None = Numbers.Flag0,

        /// <summary>
        /// Key is up
        /// </summary>
        Up = Numbers.Flag1,

        /// <summary>
        /// Key is down
        /// </summary>
        Down = Numbers.Flag2,

        /// <summary>
        /// Just after down before up for preview and do something about it.
        /// </summary>
        Preview = Down | Numbers.Flag3
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

    #region MODIFIER KEYS
    [Flags]
    public enum ModifierKeys : byte
    {
        None = Numbers.Flag0,

        /// <summary>
        /// Control key in keyboard.
        /// </summary>
        Ctrl = Numbers.Flag1,

        /// <summary>
        /// Alt key in keyboard.
        /// </summary>
        Alt = Numbers.Flag2,

        /// <summary>
        /// Shift key in keyboard.
        /// </summary>
        Shift = Numbers.Flag3,

        /// <summary>
        /// GUI key in keyboard (mostly window key).
        /// </summary>
        GUI = Numbers.Flag4,

        /// <summary>
        /// AltGr key in keyboard.
        /// </summary>
        AltGR = Numbers.Flag5,

        /// <summary>
        /// Num-lock key in keyboard.
        /// </summary>
        Num = Numbers.Flag6,

        /// <summary>
        /// Caps-lock key in keyboard.
        /// </summary>
        Caps = Numbers.Flag7,
    }
    #endregion

    #region GAME PAD TYPE
    /// <summary>
    /// Enumerates available <see cref="GamePad"/> types.
    /// </summary>
    public enum GamePadType : byte
    {
        /// <summary>
        /// The <c>GamePad</c> is of an unknown type.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The <c>GamePad</c> is an arcade stick.
        /// </summary>
        ArcadeStick,

        /// <summary>
        /// The <c>GamePad</c> is a dance pad.
        /// </summary>
        DancePad,

        /// <summary>
        /// The <c>GamePad</c> is a flight stick.
        /// </summary>
        FlightStick,

        /// <summary>
        /// The <c>GamePad</c> is a guitar.
        /// </summary>
        Guitar,

        /// <summary>
        /// The <c>GamePad</c> is a driving wheel.
        /// </summary>
        Wheel,

        /// <summary>
        /// The <c>GamePad</c> is an alternate guitar.
        /// </summary>
        AlternateGuitar,

        /// <summary>
        /// The <c>GamePad</c> is a big button pad.
        /// </summary>
        BigButtonPad,

        /// <summary>
        /// The <c>GamePad</c> is a drum kit.
        /// </summary>
        DrumKit,

        /// <summary>
        /// The <c>GamePad</c> is a game pad.
        /// </summary>
        GamePad,

        /// <summary>
        /// The <c>GamePad</c> is an arcade pad.
        /// </summary>
        ArcadePad,

        /// <summary>
        /// The <c>GamePad</c> is a bass guitar.
        /// </summary>
        BassGuitar,
    }
    #endregion

    #region HAT LOCATION
    /// <summary>
    /// Enumerates discrete positions for a joystick hat.
    /// </summary>
    public enum HatLocation : byte
    {
        /// <summary>
        /// The hat is  its centered (neutral) position
        /// </summary>
        Centered = 0,
        /// <summary>
        /// The hat is  its top position.
        /// </summary>
        Up,
        /// <summary>
        /// The hat is  its top-right position.
        /// </summary>
        UpRight,
        /// <summary>
        /// The hat is  its right position.
        /// </summary>
        Right,
        /// <summary>
        /// The hat is  its bottom-right position.
        /// </summary>
        DownRight,
        /// <summary>
        /// The hat is  its bottom position.
        /// </summary>
        Down,
        /// <summary>
        /// The hat is  its bottom-left position.
        /// </summary>
        DownLeft,
        /// <summary>
        /// The hat is  its left position.
        /// </summary>
        Left,
        /// <summary>
        /// The hat is  its top-left position.
        /// </summary>
        UpLeft,
    }
    #endregion

    #region INPUT TYPE
    /// <summary>
    /// The type of the input device.
    /// </summary>
    public enum InputType : byte
    {
        /// <summary>
        /// Device is a keyboard.
        /// </summary>
        Keyboard,
        /// <summary>
        /// Device is a mouse.
        /// </summary>
        Mouse,
        /// <summary>
        /// Device is a Human Interface Device. Joysticks, joypads, pens
        /// and some specific usb keyboards/mice fall into this category.
        /// </summary>
        Hid
    }
    #endregion

    #region JOY STICK HAT
    /// <summary>
    /// Defines available Joystick hats.
    /// </summary>
    public enum JoystickHat : byte
    {
        /// <summary>
        /// The first hat of the Joystick device.
        /// </summary>
        Hat0,
        /// <summary>
        /// The second hat of the Joystick device.
        /// </summary>
        Hat1,
        /// <summary>
        /// The third hat of the Joystick device.
        /// </summary>
        Hat2,
        /// <summary>
        /// The fourth hat of the Joystick device.
        /// </summary>
        Hat3,
        /// <summary>
        /// The last hat of the Joystick device.
        /// </summary>
        Last = Hat3
    }
    #endregion

    #region GAME CONTROLLER AXIS
    public enum GameControllerAxis : byte
    {
        Invalid = 0xff,
        LeftX = 0,
        LeftY,
        RightX,
        RightY,
        TriggerLeft,
        TriggerRight,
        Max
    }
    #endregion

    #region GAME CONTROLLER BUTTON
    public enum GameControllerButton : byte
    {
        INVALID = 0xff,
        A = 0,
        B,
        X,
        Y,
        BACK,
        GUIDE,
        START,
        LEFTSTICK,
        RIGHTSTICK,
        LEFTSHOULDER,
        RIGHTSHOULDER,
        DPAD_UP,
        DPAD_DOWN,
        DPAD_LEFT,
        DPAD_RIGHT,
        Max
    }
    #endregion

    #region JOY STICK POWER LEVEL
    public enum JoystickPowerLevel : sbyte
    {
        Unknown = -1,
        Empty,
        Low,
        Medium,
        Full,
        Wired,
        Max
    }
    #endregion

    #region JOY STICK TYPE
    public enum JoystickType : byte
    {
        Unknown,
        GameController,
        Wheel,
        Stick,
        FlightStick,
        DancePad,
        Guitar,
        DrumKit,
        ArcadePad
    }
    #endregion

    #region RENDERER FLAGS
    [Flags]
    public enum RendererFlags : byte
    {
        Default = 0x0000000,
        Software = 0x00000001,
        Accelarated = 0x00000002,
        PresentSync = 0x00000004,
        Texture = 0x00000008
    }
    #endregion

    #region RENDERING HINT
    public enum RenderingHint : byte
    {
        Nearest = 0,
        Liner = 1,
        Best = 2,
    }
    #endregion

    #region BLEND MODE
    [Flags]
    public enum BlendMode
    {
        None = 0x00000000,
        Blend = 0x00000001,
        Add = 0x00000002,
        Mod = 0x00000004,
        Invalid = 0x7FFFFFFF
    }
    #endregion

    #region TEXTURE ACCESS
    public enum TextureAccess : byte
    {
        Static,
        Streaming,
        Target,
    }
    #endregion

    #region GWS WINDOW FLAGS
    /// <summary>
    /// Window creation flags. For SDL we have mapped all options available here.
    /// For different windowing system of your choice. You will have to map and design your underlying window to reflect these options.
    /// </summary>
    [Flags]
    public enum GwsWindowFlags : ushort
    {
        /// <summary>
        /// None means default
        /// </summary>
        None = Numbers.Flag0,

        /// <summary>
        /// Enables window to be resizeable.
        /// </summary>
        Resizable = Numbers.Flag1,

        /// <summary>
        /// Sets window on full screen mode.
        /// </summary>
        FullScreen = Numbers.Flag2,

        /// <summary>
        /// Sets rendering context of window explicitely to OpenGL.
        /// </summary>
        OpenGL = Numbers.Flag3,

        /// <summary>
        /// Sets window as displayed.
        /// </summary>
        Shown = Numbers.Flag4,

        /// <summary>
        /// Sets window hidden.
        /// </summary>
        Hidden = Numbers.Flag5,

        /// <summary>
        /// Cretes widow without border.
        /// </summary>
        NoBorders = Numbers.Flag6,

        /// <summary>
        /// Sets window in minimized state.
        /// </summary>
        Minimized = Numbers.Flag7,

        /// <summary>
        /// Sets window in maximized state.
        /// </summary>
        Maximized = Numbers.Flag8,

        /// <summary>
        /// Sets window focusable and capable receiving inputs.
        /// </summary>
        InputFocus = Numbers.Flag9,

        /// <summary>
        /// Set mouse focus to window.
        /// </summary>
        MouseFocus = Numbers.Flag10,

        /// <summary>
        /// Tells GWS that window should be created in high-DPI mode if supported (for SDL it is possible in version>= SDL 2.0.1)
        /// </summary>
        HighDPI = Numbers.Flag11,

        /// <summary>
        /// Tells GWS to create a primary view of the window to host multiple virtual windows.
        /// </summary>
        MultiWindow = Numbers.Flag12,

        /// <summary>
        /// Tells GWS to create a window and then immediately show it.
        /// </summary>
        ShowImmediate = Numbers.Flag13,

        /// <summary>
        /// 
        /// </summary>
        HostingMouse = Numbers.Flag14,
    }
    #endregion

    #region OS
    /// <summary>
    /// Operating System Information enum.
    /// </summary>
    [Flags]
    public enum OS : ushort
    {
        /// <summary>
        /// Not determined.
        /// </summary>
        None = Numbers.Flag0,

        /// <summary>
        /// Operating system is Windows 
        /// </summary>
        Windows = Numbers.Flag1,

        /// <summary>
        /// Operating system is Android 
        /// </summary>
        Android = Numbers.Flag2,

        /// <summary>
        /// Operating system is IPhone 
        /// </summary>
        IPhone = Numbers.Flag3,

        /// <summary>
        /// Operating system is Linux 
        /// </summary>
        Linux = Numbers.Flag4,

        /// <summary>
        /// Operating system is MacOsX 
        /// </summary>
        MacOsX = Numbers.Flag5,

        /// <summary>
        /// Operating system is IOS 
        /// </summary>
        IOS = Numbers.Flag6,

        /// <summary>
        /// Operating system architecture is 32 bit 
        /// </summary>
        X86 = Numbers.Flag7,

        /// <summary>
        /// Operating system architecture is 64 bit 
        /// </summary>
        X64 = Numbers.Flag8,

        /// <summary>
        /// Operating system architecture is ARM 
        /// </summary>
        Arm = Numbers.Flag9,
    }
    #endregion

    #region WINDOW BORDER
    public enum WindowBorder : byte
    {
        /// <summary>
        /// The window has a resizable border. A window with a resizable border can be resized by the user or programmatically.
        /// </summary>
        Resizable = 0,
        /// <summary>
        /// The window has a fixed border. A window with a fixed border can only be resized programmatically.
        /// </summary>
        Fixed,
        /// <summary>
        /// The window does not have a border. A window with a hidden border can only be resized programmatically.
        /// </summary>
        Hidden
    }
    #endregion

    #region CURSOR TYPE
    public enum CursorType : byte
    {
        /// <summary>
        /// The arrow
        /// </summary>
        Arrow,
        /// <summary>
        /// The default
        /// </summary>
        Default,
        /// <summary>
        /// The application starting
        /// </summary>
        AppStarting,
        /// <summary>
        /// The cross
        /// </summary>
        Cross,
        /// <summary>
        /// The hand
        /// </summary>
        Hand,
        /// <summary>
        /// The help
        /// </summary>
        Help,
        /// <summary>
        /// The h split
        /// </summary>
        HSplit,
        /// <summary>
        /// The i beam
        /// </summary>
        IBeam,
        /// <summary>
        /// The no
        /// </summary>
        No,
        /// <summary>
        /// The no move2 d
        /// </summary>
        NoMove2D,
        /// <summary>
        /// The no move horiz
        /// </summary>
        NoMoveHoriz,
        /// <summary>
        /// The no move vert
        /// </summary>
        NoMoveVert,
        /// <summary>
        /// The pan east
        /// </summary>
        PanEast,
        /// <summary>
        /// The pan ne
        /// </summary>
        PanNE,
        /// <summary>
        /// The pan north
        /// </summary>
        PanNorth,
        /// <summary>
        /// The pan nw
        /// </summary>
        PanNW,
        /// <summary>
        /// The pan se
        /// </summary>
        PanSE,
        /// <summary>
        /// The pan south
        /// </summary>
        PanSouth,
        /// <summary>
        /// The pan sw
        /// </summary>
        PanSW,
        /// <summary>
        /// The pan west
        /// </summary>
        PanWest,
        /// <summary>
        /// The size all
        /// </summary>
        SizeAll,
        /// <summary>
        /// The size nesw
        /// </summary>
        SizeNESW,
        /// <summary>
        /// The size ns
        /// </summary>
        SizeNS,
        /// <summary>
        /// The size nwse
        /// </summary>
        SizeNWSE,
        /// <summary>
        /// The size we
        /// </summary>
        SizeWE,
        /// <summary>
        /// Up arrow
        /// </summary>
        UpArrow,
        /// <summary>
        /// The v split
        /// </summary>
        VSplit,
        /// <summary>
        /// The wait cursor
        /// </summary>
        WaitCursor,

        Custom,
    }
    #endregion

    #region SOUND
    public enum SoundStatus : byte
    {
        /// <summary>Sound is not playing</summary>
        Stopped,

        /// <summary>Sound is paused</summary>
        Paused,

        /// <summary>Sound is playing</summary>
        Playing
    }
    #endregion

    #region GWS EVENTS
    /// <summary>
    /// Represents GwsEvent
    /// </summary>
    public enum GwsEvent : byte
    {
        /// <summary>
        /// Just a place holder at 0 index.
        /// </summary>
        First = 0,

        /// <summary>
        /// System has quitted
        /// </summary>
        Quit = 1,

        /// <summary>
        /// System is handling window event
        /// </summary>
        WindowEvent = 2,

        /// <summary>
        /// System is handling window event
        /// </summary>
        SysWmEvent = 3,

        /// <summary>
        /// System has thrown key down event
        /// </summary>
        KeyDown = 4,

        /// <summary>
        /// System has thrown key up event
        /// </summary>
        KeyUp = 5,

        /// <summary>
        /// System has indicated an act of entering some text
        /// </summary>
        KeyPress = 6,

        /// <summary>
        /// System has indicated mouse motion
        /// </summary>
        MouseMotion = 7,

        /// <summary>
        /// System has indicated mouse down
        /// </summary>
        MouseDown = 8,

        /// <summary>
        /// System has indicated mouse up
        /// </summary>
        MouseUp = 9,

        /// <summary>
        /// System has indicated mouse wheel motion
        /// </summary>
        MouseWheel = 10,

        /// <summary>
        /// System has indicated joy axis motion
        /// </summary>
        JoyAxisMotion = 11,

        /// <summary>
        /// System has indicated joy ball motion
        /// </summary>
        JoyBallMotion = 12,

        /// <summary>
        /// System has indicated joy hat motion
        /// </summary>
        JoyHatMotion = 13,

        /// <summary>
        /// System has indicated joy button down
        /// </summary>
        JoyButtonDown = 14,

        /// <summary>
        /// System has indicated joy button up
        /// </summary>
        JoyButtonUp = 15,

        /// <summary>
        /// System has indicated a joy device has been added
        /// </summary>
        JoystickAdded = 16,

        /// <summary>
        /// System has indicated a joy device has been removed
        /// </summary>
        JoystickRemoved = 17,

        /// <summary>
        /// System has indicated a controller axis motion
        /// </summary>
        ControllerAxisMotion = 18,

        /// <summary>
        /// System has indicated controller button down
        /// </summary>
        ControllerButtonDown = 19,

        /// <summary>
        /// System has indicated controller button up
        /// </summary>
        ControllerButtonUp = 20,

        /// <summary>
        /// System has indicated a controller device has been added
        /// </summary>
        ControllerAdded = 21,

        /// <summary>
        /// System has indicated a controller device has been removed
        /// </summary>
        ControllerRemoved = 22,

        /// <summary>
        /// System has indicated a controller device has been mapped
        /// </summary>
        ControllerMapped = 23,

        /// <summary>
        /// System has indicated a fingure down on screen
        /// </summary>
        FingerDown = 24,

        /// <summary>
        /// System has indicated a fingure up on screen
        /// </summary>
        FingerUp = 25,

        /// <summary>
        /// System has indicated a fingure motion on screen
        /// </summary>
        FingerMotion = 26,

        /// <summary>
        /// System has indicated a dollar gesture on screen
        /// </summary>
        DollarGesture = 27,

        DollarRecord = 28,

        /// <summary>
        /// System has indicated multi gestures of fingure movements on screen
        /// </summary>
        MultiGesture = 29,

        /// <summary>
        /// System has indicated an update of clipboard
        /// </summary>
        ClipBoardUpdate = 30,

        /// <summary>
        /// System has indicated a file drop
        /// </summary>
        DropFile = 31,

        /// <summary>
        /// Any user instigated event
        /// </summary>
        UserEvent = 32,

        /// <summary>
        /// System has indicated a text drop on some window
        /// </summary>
        DropText = 33,

        /// <summary>
        /// System has indicated a beginning of drop operation on some window
        /// </summary>
        DropBegin = 34,

        /// <summary>
        /// System has indicated a completion of drop operation on some window
        /// </summary>
        DropComplete = 35,

        /// <summary>
        /// System has indicated an audio device is added
        /// </summary>
        AudioDeviceAdded = 36,

        /// <summary>
        /// System has indicated an audio device is removed
        /// </summary>
        AudioDeviceRemoved = 37,

        /// <summary>
        /// System has indicated a render target on current window is reset
        /// </summary>
        RENDER_TARGETS_RESET = 38,

        /// <summary>
        /// System has indicated a render device on current window is reset
        /// </summary>
        RENDER_DEVICE_RESET = 39,

        /// <summary>
        /// Window is shown
        /// </summary>
        Shown = 40,

        /// <summary>
        /// Window is shown
        /// </summary>
        Hidden = 41,

        /// <summary>
        /// Window is exposed sort of shown
        /// </summary>
        Exposed = 42,

        /// <summary>
        /// Window is moved
        /// </summary>
        Moved = 43,

        /// <summary>
        /// Window is resized
        /// </summary>
        Resized = 44,

        /// <summary>
        /// Window's size is changed
        /// </summary>
        SizeChanged = 45,

        /// <summary>
        /// Window is minimized
        /// </summary>
        Minimized = 46,

        /// <summary>
        /// Window is maximized
        /// </summary>
        Maximized = 47,

        /// <summary>
        /// Window is restored
        /// </summary>
        Restored = 48,

        /// <summary>
        /// Mouse has entered in the window.
        /// </summary>
        MouseEnter = 49,

        /// <summary>
        /// Mouse has left the window.
        /// </summary>
        MouseLeave = 50,

        /// <summary>
        /// Window has gained the focus
        /// </summary>
        GotFocus = 51,

        /// <summary>
        /// Window has left the focus
        /// </summary>
        LostFocus = 52,

        /// <summary>
        /// Window is closing
        /// </summary>
        Close = 53,

        /// <summary>
        /// Repaint the window.
        /// </summary>
        Paint = 54,

        /// <summary>
        /// 
        /// </summary>
        MouseClick = 55,

        /// <summary>
        /// 
        /// </summary>
        Load = 56,

        /// <summary>
        /// Represents Drag event which can have any of the following status:
        /// Begin, Continue, End.
        /// </summary>
        MouseDrag = 57,

        /// <summary>
        /// Place holder last index in screen.
        /// </summary>
        LASTEVENT = 58,
    }
    #endregion

    #region WINDOW EVENT ID
    /// <summary>
    /// Represents the current event.
    /// </summary>
    [Flags]
    public enum WindowEvent : byte
    {
        /// <summary>
        /// No event
        /// </summary>
        NONE = 0,

        /// <summary>
        /// Window is shown
        /// </summary>
        Shown = 1,

        /// <summary>
        /// Window was made hidden
        /// </summary>
        Hidden = 2,

        /// <summary>
        /// Window is exposed - sort of shown
        /// </summary>
        Exposed = 3,

        /// <summary>
        /// Window is moved
        /// </summary>
        Moved = 4,

        /// <summary>
        /// Window is resized
        /// </summary>
        Resized = 5,

        /// <summary>
        /// Window's size is changed
        /// </summary>
        SizeChanged = 6,

        /// <summary>
        /// Window is minimized
        /// </summary>
        MiniMized = 7,

        /// <summary>
        /// Window is maximized
        /// </summary>
        Maximized = 8,

        /// <summary>
        /// Window is restored to its previous state.
        /// </summary>
        Restored = 9,

        /// <summary>
        /// Mouse has entered in the window.
        /// </summary>
        MouseEnter = 10,

        /// <summary>
        /// Mouse has left the window
        /// </summary>
        MouseLeave = 11,

        /// <summary>
        /// Window receives focus
        /// </summary>
        GotFocus = 12,

        /// <summary>
        /// Window has lost focus
        /// </summary>
        LostFocus = 13,

        /// <summary>
        /// Window is colsed
        /// </summary>
        Close = 14,
    }
    #endregion
}
#endif

