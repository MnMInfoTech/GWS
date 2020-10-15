/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

#if Window
using System;

namespace MnM.GWS
{
    enum AudioStatus
    {
        SDL_AUDIO_STOPPED,
        SDL_AUDIO_PLAYING,
        SDL_AUDIO_PAUSED
    }

    enum AudioAdjust
    {
        FREQUENCY_CHANGE,
        FORMAT_CHANGE,
        CHANNELS_CHANGE,
        ANY_CHANGE
    }

    enum AudioFormat
    {
        //8-bit support
        AUDIO_S8, //signed 8-bit samples
        AUDIO_U8,//unsigned 8-bit samples
                 //16-bit support
        AUDIO_S16LSB, //signed 16-bit samples  little-endian byte order
        AUDIO_S16MSB, //signed 16-bit samples  big-endian byte order
        AUDIO_S16SYS, //signed 16-bit samples  native byte order
        AUDIO_S16, //AUDIO_S16LSB
        AUDIO_U16LSB, //unsigned 16-bit samples  little-endian byte order
        AUDIO_U16MSB, //unsigned 16-bit samples  big-endian byte order
        AUDIO_U16SYS, //unsigned 16-bit samples  native byte order
        AUDIO_U16, //AUDIO_U16LSB
                   //32-bit support (new to SDL 2.0)
        AUDIO_S32LSB, //32-bit integer samples  little-endian byte order
        AUDIO_S32MSB, //32-bit integer samples  big-endian byte order
        AUDIO_S32SYS, //32-bit integer samples  native byte order
        AUDIO_S32, //AUDIO_S32LSB
                   //float support(new to SDL 2.0)
        AUDIO_F32LSB, //32-bit floating point samples  little-endian byte order
        AUDIO_F32MSB, //32-bit floating point samples  big-endian byte order
        AUDIO_F32SYS, //32-bit floating point samples  native byte order
        AUDIO_F32, //AUDIO_F32LSB
    }

    enum EventType
    {
        FIRSTEVENT = 0,
        QUIT = 0x100,
        WINDOWEVENT = 0x200,
        SYSWMEVENT,
        KEYDOWN = 0x300,
        KEYUP,
        TEXTEDITING,
        TEXTINPUT,
        MOUSEMOTION = 0x400,
        MOUSEBUTTONDOWN,
        MOUSEBUTTONUP,
        MOUSEWHEEL,
        JOYAXISMOTION = 0x600,
        JOYBALLMOTION,
        JOYHATMOTION,
        JOYBUTTONDOWN,
        JOYBUTTONUP,
        JOYDEVICEADDED,
        JOYDEVICEREMOVED,
        CONTROLLERAXISMOTION = 0x650,
        CONTROLLERBUTTONDOWN,
        CONTROLLERBUTTONUP,
        CONTROLLERDEVICEADDED,
        CONTROLLERDEVICEREMOVED,
        CONTROLLERDEVICEREMAPPED,

        FINGERDOWN = 0x700,
        FINGERUP,
        FINGERMOTION,
        DOLLARGESTURE = 0x800,
        DOLLARRECORD,
        MULTIGESTURE,
        CLIPBOARDUPDATE = 0x900,
        DROPFILE = 0x1000,
        USEREVENT = 0x8000,
        LASTEVENT = 0xFFFF,

        DROPTEXT,
        DROPBEGIN,
        DROPCOMPLETE,
        AUDIODEVICEADDED = 0x1100,
        AUDIODEVICEREMOVED,
        RENDER_TARGETS_RESET = 0x2000,
        RENDER_DEVICE_RESET,
    }
    enum DisplayIndex
    {
        First = 0,
        Second,
        Third,
        Fourth,
        Fifth,
        Sixth,
        Primary = -1,
        Default = Primary,
    }

    enum WindowFlags
    {
        Default = Resizable,
        FullScreen = 0x00000001,
        OpenGL = 0x00000002,
        Shown = 0x00000004,
        Hidden = 0x00000008,
        NoBorders = 0x00000010,
        Resizable = 0x00000020,
        Minimized = 0x00000040,
        Maximized = 0x00000080,
        GrabInput = 0x00000100,
        InputFocus = 0x00000200,
        MouseFocus = 0x00000400,
        FullScreenDesktop = (FullScreen | 0x00001000),
        Foreign = 0x00000800,
        HighDPI = 0x00002000,
    }
    [Flags]
    enum ImageInitType
    {
        Jpg = 0x00000001,
        Png = 0x00000002,
        Tif = 0x00000004,
        Webp = 0x00000008
    }

    [Flags]
    enum MessageBoxButtonFlags : uint
    {
        ButtonReturnKeyDefault = 0x00000001,
        ButtonEscapeKeyDefault = 0x00000002
    }
    [Flags]
    enum MessageBoxFlags : uint
    {
        Error = 0x00000010,
        Warning = 0x00000020,
        Information = 0x00000040
    }

    enum MessageBoxColorType
    {
        OfBackground,
        OfText,
        OfButtonBorder,
        OfButtonBackground,
        OfButtonSelected,
        OfColorMax
    }

    [Flags]
    enum SystemFlags : uint
    {
        Default = 0,
        TIMER = 0x00000001,
        AUDIO = 0x00000010,
        VIDEO = 0x00000020,
        JOYSTICK = 0x00000200,
        HAPTIC = 0x00001000,
        GAMECONTROLLER = 0x00002000,
        NOPARACHUTE = 0x00100000,
        EVERYTHING = TIMER | AUDIO | VIDEO |
            JOYSTICK | HAPTIC | GAMECONTROLLER
    }

    enum _MouseButton : byte
    {
        Left = 1,
        Middle,
        Right,
        X1,
        X2
    }

    [Flags]
    enum ButtonFlags
    {
        Left = 1 << (_MouseButton.Left - 1),
        Middle = 1 << (_MouseButton.Middle - 1),
        Right = 1 << (_MouseButton.Right - 1),
        X1 = 1 << (_MouseButton.X1 - 1),
        X2 = 1 << (_MouseButton.X2 - 1),
    }
    enum EventAction
    {
        Add,
        Peek,
        Get
    }
    enum EventState
    {
        Query = -1,
        Ignore = 0,
        Disable = 0,
        Enable = 1
    }
    enum SysWMType
    {
        Unknown = 0,
        Windows,
        X11,
        Wayland,
        DirectFB,
        Cocoa,
        UIKit,
    }

    /// <summary>
    /// The available keyboard keys.
    /// </summary>
    enum SdlKeys : int
    {
        /// <summary>A key outside the known keys.</summary>
        Unknown = 0,

        // Modifiers
        /// <summary>The left shift key.</summary>
        ShiftLeft,
        /// <summary>The left shift key (equivalent to ShiftLeft).</summary>
        LShift = ShiftLeft,
        /// <summary>The right shift key.</summary>
        ShiftRight,
        /// <summary>The right shift key (equivalent to ShiftRight).</summary>
        RShift = ShiftRight,
        /// <summary>The left control key.</summary>
        ControlLeft,
        /// <summary>The left control key (equivalent to ControlLeft).</summary>
        LControl = ControlLeft,
        /// <summary>The right control key.</summary>
        ControlRight,
        /// <summary>The right control key (equivalent to ControlRight).</summary>
        RControl = ControlRight,
        /// <summary>The left alt key.</summary>
        AltLeft,
        /// <summary>The left alt key (equivalent to AltLeft.</summary>
        LAlt = AltLeft,
        /// <summary>The right alt key.</summary>
        AltRight,
        /// <summary>The right alt key (equivalent to AltRight).</summary>
        RAlt = AltRight,
        /// <summary>The left win key.</summary>
        WinLeft,
        /// <summary>The left win key (equivalent to WinLeft).</summary>
        LWin = WinLeft,
        /// <summary>The right win key.</summary>
        WinRight,
        /// <summary>The right win key (equivalent to WinRight).</summary>
        RWin = WinRight,
        /// <summary>The menu key.</summary>
        Menu,

        // Function keys (hopefully enough for most keyboards - mine has 26)
        // <keysymdef.h> on X11 reports up to 35 function keys.
        /// <summary>The F1 key.</summary>
        F1,
        /// <summary>The F2 key.</summary>
        F2,
        /// <summary>The F3 key.</summary>
        F3,
        /// <summary>The F4 key.</summary>
        F4,
        /// <summary>The F5 key.</summary>
        F5,
        /// <summary>The F6 key.</summary>
        F6,
        /// <summary>The F7 key.</summary>
        F7,
        /// <summary>The F8 key.</summary>
        F8,
        /// <summary>The F9 key.</summary>
        F9,
        /// <summary>The F10 key.</summary>
        F10,
        /// <summary>The F11 key.</summary>
        F11,
        /// <summary>The F12 key.</summary>
        F12,
        /// <summary>The F13 key.</summary>
        F13,
        /// <summary>The F14 key.</summary>
        F14,
        /// <summary>The F15 key.</summary>
        F15,
        /// <summary>The F16 key.</summary>
        F16,
        /// <summary>The F17 key.</summary>
        F17,
        /// <summary>The F18 key.</summary>
        F18,
        /// <summary>The F19 key.</summary>
        F19,
        /// <summary>The F20 key.</summary>
        F20,
        /// <summary>The F21 key.</summary>
        F21,
        /// <summary>The F22 key.</summary>
        F22,
        /// <summary>The F23 key.</summary>
        F23,
        /// <summary>The F24 key.</summary>
        F24,
        /// <summary>The F25 key.</summary>
        F25,
        /// <summary>The F26 key.</summary>
        F26,
        /// <summary>The F27 key.</summary>
        F27,
        /// <summary>The F28 key.</summary>
        F28,
        /// <summary>The F29 key.</summary>
        F29,
        /// <summary>The F30 key.</summary>
        F30,
        /// <summary>The F31 key.</summary>
        F31,
        /// <summary>The F32 key.</summary>
        F32,
        /// <summary>The F33 key.</summary>
        F33,
        /// <summary>The F34 key.</summary>
        F34,
        /// <summary>The F35 key.</summary>
        F35,

        // Direction arrows
        /// <summary>The up arrow key.</summary>
        Up,
        /// <summary>The down arrow key.</summary>
        Down,
        /// <summary>The left arrow key.</summary>
        Left,
        /// <summary>The right arrow key.</summary>
        Right,

        /// <summary>The enter key.</summary>
        Enter,
        /// <summary>The escape key.</summary>
        Escape,
        /// <summary>The space key.</summary>
        Space,
        /// <summary>The tab key.</summary>
        Tab,
        /// <summary>The backspace key.</summary>
        BackSpace,
        /// <summary>The backspace key (equivalent to BackSpace).</summary>
        Back = BackSpace,
        /// <summary>The insert key.</summary>
        Insert,
        /// <summary>The delete key.</summary>
        Delete,
        /// <summary>The page up key.</summary>
        PageUp,
        /// <summary>The page down key.</summary>
        PageDown,
        /// <summary>The home key.</summary>
        Home,
        /// <summary>The end key.</summary>
        End,
        /// <summary>The caps lock key.</summary>
        CapsLock,
        /// <summary>The scroll lock key.</summary>
        ScrollLock,
        /// <summary>The print screen key.</summary>
        PrintScreen,
        /// <summary>The pause key.</summary>
        Pause,
        /// <summary>The num lock key.</summary>
        NumLock,

        // Special keys
        /// <summary>The clear key (Keypad5 with NumLock disabled, on typical keyboards).</summary>
        Clear,
        /// <summary>The sleep key.</summary>
        Sleep,
        /*LogOff,
        Help,
        Undo,
        Redo,
        New,
        Open,
        Close,
        Reply,
        Forward,
        Send,
        Spell,
        Save,
        Calculator,

        // Folders and applications
        Documents,
        Pictures,
        Music,
        MediaPlayer,
        Mail,
        Browser,
        Messenger,

        // Multimedia keys
        Mute,
        PlayPause,
        Stop,
        VolumeUp,
        VolumeDown,
        TrackPrevious,
        TrackNext,*/

        // Keypad keys
        /// <summary>The keypad 0 key.</summary>
        Keypad0,
        /// <summary>The keypad 1 key.</summary>
        Keypad1,
        /// <summary>The keypad 2 key.</summary>
        Keypad2,
        /// <summary>The keypad 3 key.</summary>
        Keypad3,
        /// <summary>The keypad 4 key.</summary>
        Keypad4,
        /// <summary>The keypad 5 key.</summary>
        Keypad5,
        /// <summary>The keypad 6 key.</summary>
        Keypad6,
        /// <summary>The keypad 7 key.</summary>
        Keypad7,
        /// <summary>The keypad 8 key.</summary>
        Keypad8,
        /// <summary>The keypad 9 key.</summary>
        Keypad9,
        /// <summary>The keypad divide key.</summary>
        KeypadDivide,
        /// <summary>The keypad multiply key.</summary>
        KeypadMultiply,
        /// <summary>The keypad subtract key.</summary>
        KeypadSubtract,
        /// <summary>The keypad minus key (equivalent to KeypadSubtract).</summary>
        KeypadMinus = KeypadSubtract,
        /// <summary>The keypad add key.</summary>
        KeypadAdd,
        /// <summary>The keypad plus key (equivalent to KeypadAdd).</summary>
        KeypadPlus = KeypadAdd,
        /// <summary>The keypad decimal key.</summary>
        KeypadDecimal,
        /// <summary>The keypad period key (equivalent to KeypadDecimal).</summary>
        KeypadPeriod = KeypadDecimal,
        /// <summary>The keypad enter key.</summary>
        KeypadEnter,

        // Letters
        /// <summary>The A key.</summary>
        A,
        /// <summary>The B key.</summary>
        B,
        /// <summary>The C key.</summary>
        C,
        /// <summary>The D key.</summary>
        D,
        /// <summary>The E key.</summary>
        E,
        /// <summary>The F key.</summary>
        F,
        /// <summary>The G key.</summary>
        G,
        /// <summary>The H key.</summary>
        H,
        /// <summary>The I key.</summary>
        I,
        /// <summary>The J key.</summary>
        J,
        /// <summary>The K key.</summary>
        K,
        /// <summary>The L key.</summary>
        L,
        /// <summary>The M key.</summary>
        M,
        /// <summary>The N key.</summary>
        N,
        /// <summary>The O key.</summary>
        O,
        /// <summary>The P key.</summary>
        P,
        /// <summary>The Q key.</summary>
        Q,
        /// <summary>The R key.</summary>
        R,
        /// <summary>The S key.</summary>
        S,
        /// <summary>The T key.</summary>
        T,
        /// <summary>The U key.</summary>
        U,
        /// <summary>The V key.</summary>
        V,
        /// <summary>The W key.</summary>
        W,
        /// <summary>The X key.</summary>
        X,
        /// <summary>The Y key.</summary>
        Y,
        /// <summary>The Z key.</summary>
        Z,

        // Numbers
        /// <summary>The number 0 key.</summary>
        Number0,
        /// <summary>The number 1 key.</summary>
        Number1,
        /// <summary>The number 2 key.</summary>
        Number2,
        /// <summary>The number 3 key.</summary>
        Number3,
        /// <summary>The number 4 key.</summary>
        Number4,
        /// <summary>The number 5 key.</summary>
        Number5,
        /// <summary>The number 6 key.</summary>
        Number6,
        /// <summary>The number 7 key.</summary>
        Number7,
        /// <summary>The number 8 key.</summary>
        Number8,
        /// <summary>The number 9 key.</summary>
        Number9,

        // Symbols
        /// <summary>The tilde key.</summary>
        Tilde,
        /// <summary>The grave key (equivaent to Tilde).</summary>
        Grave = Tilde,
        /// <summary>The minus key.</summary>
        Minus,
        //Equal,
        /// <summary>The plus key.</summary>
        Plus,
        /// <summary>The left bracket key.</summary>
        BracketLeft,
        /// <summary>The left bracket key (equivalent to BracketLeft).</summary>
        LBracket = BracketLeft,
        /// <summary>The right bracket key.</summary>
        BracketRight,
        /// <summary>The right bracket key (equivalent to BracketRight).</summary>
        RBracket = BracketRight,
        /// <summary>The semicolon key.</summary>
        Semicolon,
        /// <summary>The quote key.</summary>
        Quote,
        /// <summary>The comma key.</summary>
        Comma,
        /// <summary>The period key.</summary>
        Period,
        /// <summary>The slash key.</summary>
        Slash,
        /// <summary>The backslash key.</summary>
        BackSlash,
        /// <summary>The secondary backslash key.</summary>
        NonUSBackSlash,
        /// <summary>Indicates the last available keyboard key.</summary>
        LastKey
    }

    /// <summary>
    /// Enumerates modifier keys.
    /// </summary>
    [Flags]
    enum SdlKeyModifiers : byte
    {
        /// <summary>
        /// The alt key modifier (option on Mac).
        /// </summary>
        Alt = 1 << 0,

        /// <summary>
        /// The control key modifier.
        /// </summary>
        Control = 1 << 1,

        /// <summary>
        /// The shift key modifier.
        /// </summary>
        Shift = 1 << 2
    }

    /// <summary>
    /// Enumerates available buttons for a <c>GamePad</c> device.
    /// </summary>
    [Flags]
    enum PadButtons
    {
        /// <summary>
        /// DPad up direction button
        /// </summary>
        DPadUp = 1 << 0,

        /// <summary>
        /// DPad down direction button
        /// </summary>
        DPadDown = 1 << 1,

        /// <summary>
        /// DPad left direction button
        /// </summary>
        DPadLeft = 1 << 2,

        /// <summary>
        /// DPad right direction button
        /// </summary>
        DPadRight = 1 << 3,

        /// <summary>
        /// Start button
        /// </summary>
        Start = 1 << 4,

        /// <summary>
        /// Back button
        /// </summary>
        Back = 1 << 5,

        /// <summary>
        /// Left stick button
        /// </summary>
        LeftStick = 1 << 6,

        /// <summary>
        /// Right stick button
        /// </summary>
        RightStick = 1 << 7,

        /// <summary>
        /// Left shoulder button
        /// </summary>
        LeftShoulder = 1 << 8,

        /// <summary>
        /// Right shoulder button
        /// </summary>
        RightShoulder = 1 << 9,

        /// <summary>
        /// Home button
        /// </summary>
        Home = 1 << 11,

        /// <summary>
        /// Home button
        /// </summary>
        BigButton = Home,

        /// <summary>
        /// A button
        /// </summary>
        A = 1 << 12,

        /// <summary>
        /// B button
        /// </summary>
        B = 1 << 13,

        /// <summary>
        /// X button
        /// </summary>
        X = 1 << 14,

        /// <summary>
        /// Y button
        /// </summary>
        Y = 1 << 15,

        /// <summary>
        /// Left thumbstick left direction button
        /// </summary>
        LeftThumbstickLeft = 1 << 21,

        /// <summary>
        /// Right trigger button
        /// </summary>
        RightTrigger = 1 << 22,

        /// <summary>
        /// Left trigger button
        /// </summary>
        LeftTrigger = 1 << 23,

        /// <summary>
        /// Right thumbstick up direction button
        /// </summary>
        RightThumbstickUp = 1 << 24,

        /// <summary>
        /// Right thumbstick down direction button
        /// </summary>
        RightThumbstickDown = 1 << 25,

        /// <summary>
        /// Right stick right direction button
        /// </summary>
        RightThumbstickRight = 1 << 26,

        /// <summary>
        /// Right stick left direction button
        /// </summary>
        RightThumbstickLeft = 1 << 27,

        /// <summary>
        /// Left stick up direction button
        /// </summary>
        LeftThumbstickUp = 1 << 28,

        /// <summary>
        /// Left stick down direction button
        /// </summary>
        LeftThumbstickDown = 1 << 29,

        /// <summary>
        /// Left stick right direction button
        /// </summary>
        LeftThumbstickRight = 1 << 30,
    }

    #region internal
    enum SdlFlag
    {
        /// <summary>
        /// Error
        /// </summary>
        Error = -1,
        /// <summary>
        /// Surccess
        /// </summary>
        Success = 0,
        /// <summary>
        /// Plays  infinite loop.
        /// </summary>
        InfiniteLoop = -1,
        /// <summary>
        /// Error
        /// </summary>
        Error2 = 1,
        /// <summary>
        /// Success
        /// </summary>
        Success2 = 1,
        /// <summary>
        /// None
        /// </summary>
        None = 0,
        /// <summary>
        /// First available channel
        /// </summary>
        FirstFreeChannel = -1,
        /// <summary>
        /// True
        /// </summary>
        TrueValue = 1,
        /// <summary>
        /// False
        /// </summary>
        FalseValue = 0,
    }

    enum Keycode
    {
        UNKNOWN = 0,
        RETURN = '\r',
        ESCAPE = 27, // '\033' octal
        BACKSPACE = '\b',
        TAB = '\t',
        SPACE = ' ',
        EXCLAIM = '!',
        QUOTEDBL = '"',
        HASH = '#',
        PERCENT = '%',
        DOLLAR = '$',
        AMPERSAND = '&',
        QUOTE = '\'',
        LEFTPAREN = '(',
        RIGHTPAREN = ')',
        ASTERISK = '*',
        PLUS = '+',
        COMMA = ',',
        MINUS = '-',
        PERIOD = '.',
        SLASH = '/',
        Num0 = '0',
        Num1 = '1',
        Num2 = '2',
        Num3 = '3',
        Num4 = '4',
        Num5 = '5',
        Num6 = '6',
        Num7 = '7',
        Num8 = '8',
        Num9 = '9',
        COLON = ':',
        SEMICOLON = ';',
        LESS = '<',
        EQUALS = '=',
        GREATER = '>',
        QUESTION = '?',
        AT = '@',
        LEFTBRACKET = '[',
        BACKSLASH = '\\',
        RIGHTBRACKET = ']',
        CARET = '^',
        UNDERSCORE = '_',
        BACKQUOTE = '`',
        a = 'a',
        b = 'b',
        c = 'c',
        d = 'd',
        e = 'e',
        f = 'f',
        g = 'g',
        h = 'h',
        i = 'i',
        j = 'j',
        k = 'k',
        l = 'l',
        m = 'm',
        n = 'n',
        o = 'o',
        p = 'p',
        q = 'q',
        r = 'r',
        s = 's',
        t = 't',
        u = 'u',
        v = 'v',
        w = 'w',
        x = 'x',
        y = 'y',
        z = 'z',
        CAPSLOCK = (1 << 30) | (int)Scancode.CAPSLOCK,
        F1 = (1 << 30) | (int)Scancode.F1,
        F2 = (1 << 30) | (int)Scancode.F2,
        F3 = (1 << 30) | (int)Scancode.F3,
        F4 = (1 << 30) | (int)Scancode.F4,
        F5 = (1 << 30) | (int)Scancode.F5,
        F6 = (1 << 30) | (int)Scancode.F6,
        F7 = (1 << 30) | (int)Scancode.F7,
        F8 = (1 << 30) | (int)Scancode.F8,
        F9 = (1 << 30) | (int)Scancode.F9,
        F10 = (1 << 30) | (int)Scancode.F10,
        F11 = (1 << 30) | (int)Scancode.F11,
        F12 = (1 << 30) | (int)Scancode.F12,
        PRINTSCREEN = (1 << 30) | (int)Scancode.PRINTSCREEN,
        SCROLLLOCK = (1 << 30) | (int)Scancode.SCROLLLOCK,
        PAUSE = (1 << 30) | (int)Scancode.PAUSE,
        INSERT = (1 << 30) | (int)Scancode.INSERT,
        HOME = (1 << 30) | (int)Scancode.HOME,
        PAGEUP = (1 << 30) | (int)Scancode.PAGEUP,
        DELETE = 127, // '\177' octal
        END = (1 << 30) | (int)Scancode.END,
        PAGEDOWN = (1 << 30) | (int)Scancode.PAGEDOWN,
        RIGHT = (1 << 30) | (int)Scancode.RIGHT,
        LEFT = (1 << 30) | (int)Scancode.LEFT,
        DOWN = (1 << 30) | (int)Scancode.DOWN,
        UP = (1 << 30) | (int)Scancode.UP,
        NUMLOCKCLEAR = (1 << 30) | (int)Scancode.NUMLOCKCLEAR,
        KP_DIVIDE = (1 << 30) | (int)Scancode.KP_DIVIDE,
        KP_MULTIPLY = (1 << 30) | (int)Scancode.KP_MULTIPLY,
        KP_MINUS = (1 << 30) | (int)Scancode.KP_MINUS,
        KP_PLUS = (1 << 30) | (int)Scancode.KP_PLUS,
        KP_ENTER = (1 << 30) | (int)Scancode.KP_ENTER,
        KP_1 = (1 << 30) | (int)Scancode.KP_1,
        KP_2 = (1 << 30) | (int)Scancode.KP_2,
        KP_3 = (1 << 30) | (int)Scancode.KP_3,
        KP_4 = (1 << 30) | (int)Scancode.KP_4,
        KP_5 = (1 << 30) | (int)Scancode.KP_5,
        KP_6 = (1 << 30) | (int)Scancode.KP_6,
        KP_7 = (1 << 30) | (int)Scancode.KP_7,
        KP_8 = (1 << 30) | (int)Scancode.KP_8,
        KP_9 = (1 << 30) | (int)Scancode.KP_9,
        KP_0 = (1 << 30) | (int)Scancode.KP_0,
        KP_PERIOD = (1 << 30) | (int)Scancode.KP_PERIOD,
        APPLICATION = (1 << 30) | (int)Scancode.APPLICATION,
        POWER = (1 << 30) | (int)Scancode.POWER,
        KP_EQUALS = (1 << 30) | (int)Scancode.KP_EQUALS,
        F13 = (1 << 30) | (int)Scancode.F13,
        F14 = (1 << 30) | (int)Scancode.F14,
        F15 = (1 << 30) | (int)Scancode.F15,
        F16 = (1 << 30) | (int)Scancode.F16,
        F17 = (1 << 30) | (int)Scancode.F17,
        F18 = (1 << 30) | (int)Scancode.F18,
        F19 = (1 << 30) | (int)Scancode.F19,
        F20 = (1 << 30) | (int)Scancode.F20,
        F21 = (1 << 30) | (int)Scancode.F21,
        F22 = (1 << 30) | (int)Scancode.F22,
        F23 = (1 << 30) | (int)Scancode.F23,
        F24 = (1 << 30) | (int)Scancode.F24,
        EXECUTE = (1 << 30) | (int)Scancode.EXECUTE,
        HELP = (1 << 30) | (int)Scancode.HELP,
        MENU = (1 << 30) | (int)Scancode.MENU,
        SELECT = (1 << 30) | (int)Scancode.SELECT,
        STOP = (1 << 30) | (int)Scancode.STOP,
        AGAIN = (1 << 30) | (int)Scancode.AGAIN,
        UNDO = (1 << 30) | (int)Scancode.UNDO,
        CUT = (1 << 30) | (int)Scancode.CUT,
        COPY = (1 << 30) | (int)Scancode.COPY,
        PASTE = (1 << 30) | (int)Scancode.PASTE,
        FIND = (1 << 30) | (int)Scancode.FIND,
        MUTE = (1 << 30) | (int)Scancode.MUTE,
        VOLUMEUP = (1 << 30) | (int)Scancode.VOLUMEUP,
        VOLUMEDOWN = (1 << 30) | (int)Scancode.VOLUMEDOWN,
        KP_COMMA = (1 << 30) | (int)Scancode.KP_COMMA,
        KP_EQUALSAS400 = (1 << 30) | (int)Scancode.KP_EQUALSAS400,
        ALTERASE = (1 << 30) | (int)Scancode.ALTERASE,
        SYSREQ = (1 << 30) | (int)Scancode.SYSREQ,
        CANCEL = (1 << 30) | (int)Scancode.CANCEL,
        CLEAR = (1 << 30) | (int)Scancode.CLEAR,
        PRIOR = (1 << 30) | (int)Scancode.PRIOR,
        RETURN2 = (1 << 30) | (int)Scancode.RETURN2,
        SEPARATOR = (1 << 30) | (int)Scancode.SEPARATOR,
        OUT = (1 << 30) | (int)Scancode.OUT,
        OPER = (1 << 30) | (int)Scancode.OPER,
        CLEARAGAIN = (1 << 30) | (int)Scancode.CLEARAGAIN,
        CRSEL = (1 << 30) | (int)Scancode.CRSEL,
        EXSEL = (1 << 30) | (int)Scancode.EXSEL,
        KP_00 = (1 << 30) | (int)Scancode.KP_00,
        KP_000 = (1 << 30) | (int)Scancode.KP_000,
        THOUSANDSSEPARATOR = (1 << 30) | (int)Scancode.THOUSANDSSEPARATOR,
        DECIMALSEPARATOR = (1 << 30) | (int)Scancode.DECIMALSEPARATOR,
        CURRENCYUNIT = (1 << 30) | (int)Scancode.CURRENCYUNIT,
        CURRENCYSUBUNIT = (1 << 30) | (int)Scancode.CURRENCYSUBUNIT,
        KP_LEFTPAREN = (1 << 30) | (int)Scancode.KP_LEFTPAREN,
        KP_RIGHTPAREN = (1 << 30) | (int)Scancode.KP_RIGHTPAREN,
        KP_LEFTBRACE = (1 << 30) | (int)Scancode.KP_LEFTBRACE,
        KP_RIGHTBRACE = (1 << 30) | (int)Scancode.KP_RIGHTBRACE,
        KP_TAB = (1 << 30) | (int)Scancode.KP_TAB,
        KP_BACKSPACE = (1 << 30) | (int)Scancode.KP_BACKSPACE,
        KP_A = (1 << 30) | (int)Scancode.KP_A,
        KP_B = (1 << 30) | (int)Scancode.KP_B,
        KP_C = (1 << 30) | (int)Scancode.KP_C,
        KP_D = (1 << 30) | (int)Scancode.KP_D,
        KP_E = (1 << 30) | (int)Scancode.KP_E,
        KP_F = (1 << 30) | (int)Scancode.KP_F,
        KP_XOR = (1 << 30) | (int)Scancode.KP_XOR,
        KP_POWER = (1 << 30) | (int)Scancode.KP_POWER,
        KP_PERCENT = (1 << 30) | (int)Scancode.KP_PERCENT,
        KP_LESS = (1 << 30) | (int)Scancode.KP_LESS,
        KP_GREATER = (1 << 30) | (int)Scancode.KP_GREATER,
        KP_AMPERSAND = (1 << 30) | (int)Scancode.KP_AMPERSAND,
        KP_DBLAMPERSAND = (1 << 30) | (int)Scancode.KP_DBLAMPERSAND,
        KP_VERTICALBAR = (1 << 30) | (int)Scancode.KP_VERTICALBAR,
        KP_DBLVERTICALBAR = (1 << 30) | (int)Scancode.KP_DBLVERTICALBAR,
        KP_COLON = (1 << 30) | (int)Scancode.KP_COLON,
        KP_HASH = (1 << 30) | (int)Scancode.KP_HASH,
        KP_SPACE = (1 << 30) | (int)Scancode.KP_SPACE,
        KP_AT = (1 << 30) | (int)Scancode.KP_AT,
        KP_EXCLAM = (1 << 30) | (int)Scancode.KP_EXCLAM,
        KP_MEMSTORE = (1 << 30) | (int)Scancode.KP_MEMSTORE,
        KP_MEMRECALL = (1 << 30) | (int)Scancode.KP_MEMRECALL,
        KP_MEMCLEAR = (1 << 30) | (int)Scancode.KP_MEMCLEAR,
        KP_MEMADD = (1 << 30) | (int)Scancode.KP_MEMADD,
        KP_MEMSUBTRACT = (1 << 30) | (int)Scancode.KP_MEMSUBTRACT,
        KP_MEMMULTIPLY = (1 << 30) | (int)Scancode.KP_MEMMULTIPLY,
        KP_MEMDIVIDE = (1 << 30) | (int)Scancode.KP_MEMDIVIDE,
        KP_PLUSMINUS = (1 << 30) | (int)Scancode.KP_PLUSMINUS,
        KP_CLEAR = (1 << 30) | (int)Scancode.KP_CLEAR,
        KP_CLEARENTRY = (1 << 30) | (int)Scancode.KP_CLEARENTRY,
        KP_BINARY = (1 << 30) | (int)Scancode.KP_BINARY,
        KP_OCTAL = (1 << 30) | (int)Scancode.KP_OCTAL,
        KP_DECIMAL = (1 << 30) | (int)Scancode.KP_DECIMAL,
        KP_HEXADECIMAL = (1 << 30) | (int)Scancode.KP_HEXADECIMAL,
        LCTRL = (1 << 30) | (int)Scancode.LCTRL,
        LSHIFT = (1 << 30) | (int)Scancode.LSHIFT,
        LALT = (1 << 30) | (int)Scancode.LALT,
        LGUI = (1 << 30) | (int)Scancode.LGUI,
        RCTRL = (1 << 30) | (int)Scancode.RCTRL,
        RSHIFT = (1 << 30) | (int)Scancode.RSHIFT,
        RALT = (1 << 30) | (int)Scancode.RALT,
        RGUI = (1 << 30) | (int)Scancode.RGUI,
        MODE = (1 << 30) | (int)Scancode.MODE,
        AUDIONEXT = (1 << 30) | (int)Scancode.AUDIONEXT,
        AUDIOPREV = (1 << 30) | (int)Scancode.AUDIOPREV,
        AUDIOSTOP = (1 << 30) | (int)Scancode.AUDIOSTOP,
        AUDIOPLAY = (1 << 30) | (int)Scancode.AUDIOPLAY,
        AUDIOMUTE = (1 << 30) | (int)Scancode.AUDIOMUTE,
        MEDIASELECT = (1 << 30) | (int)Scancode.MEDIASELECT,
        WWW = (1 << 30) | (int)Scancode.WWW,
        MAIL = (1 << 30) | (int)Scancode.MAIL,
        CALCULATOR = (1 << 30) | (int)Scancode.CALCULATOR,
        COMPUTER = (1 << 30) | (int)Scancode.COMPUTER,
        AC_SEARCH = (1 << 30) | (int)Scancode.AC_SEARCH,
        AC_HOME = (1 << 30) | (int)Scancode.AC_HOME,
        AC_BACK = (1 << 30) | (int)Scancode.AC_BACK,
        AC_FORWARD = (1 << 30) | (int)Scancode.AC_FORWARD,
        AC_STOP = (1 << 30) | (int)Scancode.AC_STOP,
        AC_REFRESH = (1 << 30) | (int)Scancode.AC_REFRESH,
        AC_BOOKMARKS = (1 << 30) | (int)Scancode.AC_BOOKMARKS,
        BRIGHTNESSDOWN = (1 << 30) | (int)Scancode.BRIGHTNESSDOWN,
        BRIGHTNESSUP = (1 << 30) | (int)Scancode.BRIGHTNESSUP,
        DISPLAYSWITCH = (1 << 30) | (int)Scancode.DISPLAYSWITCH,
        KBDILLUMTOGGLE = (1 << 30) | (int)Scancode.KBDILLUMTOGGLE,
        KBDILLUMDOWN = (1 << 30) | (int)Scancode.KBDILLUMDOWN,
        KBDILLUMUP = (1 << 30) | (int)Scancode.KBDILLUMUP,
        EJECT = (1 << 30) | (int)Scancode.EJECT,
        SLEEP = (1 << 30) | (int)Scancode.SLEEP
    }
    [Flags]
    enum Keymod : ushort
    {
        NONE = 0x0000,
        LSHIFT = 0x0001,
        RSHIFT = 0x0002,
        LCTRL = 0x0040,
        RCTRL = 0x0080,
        LALT = 0x0100,
        RALT = 0x0200,
        LGUI = 0x0400,
        RGUI = 0x0800,
        NUM = 0x1000,
        CAPS = 0x2000,
        MODE = 0x4000,
        RESERVED = 0x8000,
        CTRL = (LCTRL | RCTRL),
        SHIFT = (LSHIFT | RSHIFT),
        ALT = (LALT | RALT),
        GUI = (LGUI | RGUI)
    }
    enum Scancode
    {
        UNKNOWN = 0,
        A = 4,
        B = 5,
        C = 6,
        D = 7,
        E = 8,
        F = 9,
        G = 10,
        H = 11,
        I = 12,
        J = 13,
        K = 14,
        L = 15,
        M = 16,
        N = 17,
        O = 18,
        P = 19,
        Q = 20,
        R = 21,
        S = 22,
        T = 23,
        U = 24,
        V = 25,
        W = 26,
        X = 27,
        Y = 28,
        Z = 29,
        Num1 = 30,
        Num2 = 31,
        Num3 = 32,
        Num4 = 33,
        Num5 = 34,
        Num6 = 35,
        Num7 = 36,
        Num8 = 37,
        Num9 = 38,
        Num0 = 39,
        RETURN = 40,
        ESCAPE = 41,
        BACKSPACE = 42,
        TAB = 43,
        SPACE = 44,
        MINUS = 45,
        EQUALS = 46,
        LEFTBRACKET = 47,
        RIGHTBRACKET = 48,
        BACKSLASH = 49,
        NONUSHASH = 50,
        SEMICOLON = 51,
        APOSTROPHE = 52,
        GRAVE = 53,
        COMMA = 54,
        PERIOD = 55,
        SLASH = 56,
        CAPSLOCK = 57,
        F1 = 58,
        F2 = 59,
        F3 = 60,
        F4 = 61,
        F5 = 62,
        F6 = 63,
        F7 = 64,
        F8 = 65,
        F9 = 66,
        F10 = 67,
        F11 = 68,
        F12 = 69,
        PRINTSCREEN = 70,
        SCROLLLOCK = 71,
        PAUSE = 72,
        INSERT = 73,
        HOME = 74,
        PAGEUP = 75,
        DELETE = 76,
        END = 77,
        PAGEDOWN = 78,
        RIGHT = 79,
        LEFT = 80,
        DOWN = 81,
        UP = 82,
        NUMLOCKCLEAR = 83,
        KP_DIVIDE = 84,
        KP_MULTIPLY = 85,
        KP_MINUS = 86,
        KP_PLUS = 87,
        KP_ENTER = 88,
        KP_1 = 89,
        KP_2 = 90,
        KP_3 = 91,
        KP_4 = 92,
        KP_5 = 93,
        KP_6 = 94,
        KP_7 = 95,
        KP_8 = 96,
        KP_9 = 97,
        KP_0 = 98,
        KP_PERIOD = 99,
        NONUSBACKSLASH = 100,
        APPLICATION = 101,
        POWER = 102,
        KP_EQUALS = 103,
        F13 = 104,
        F14 = 105,
        F15 = 106,
        F16 = 107,
        F17 = 108,
        F18 = 109,
        F19 = 110,
        F20 = 111,
        F21 = 112,
        F22 = 113,
        F23 = 114,
        F24 = 115,
        EXECUTE = 116,
        HELP = 117,
        MENU = 118,
        SELECT = 119,
        STOP = 120,
        AGAIN = 121,
        UNDO = 122,
        CUT = 123,
        COPY = 124,
        PASTE = 125,
        FIND = 126,
        MUTE = 127,
        VOLUMEUP = 128,
        VOLUMEDOWN = 129,
        // not sure whether there's a reason to enable these
        //  LOCKINGCAPSLOCK = 130,
        //  LOCKINGNUMLOCK = 131,
        //  LOCKINGSCROLLLOCK = 132,
        KP_COMMA = 133,
        KP_EQUALSAS400 = 134,
        INTERNATIONAL1 = 135,
        INTERNATIONAL2 = 136,
        INTERNATIONAL3 = 137,
        INTERNATIONAL4 = 138,
        INTERNATIONAL5 = 139,
        INTERNATIONAL6 = 140,
        INTERNATIONAL7 = 141,
        INTERNATIONAL8 = 142,
        INTERNATIONAL9 = 143,
        LANG1 = 144,
        LANG2 = 145,
        LANG3 = 146,
        LANG4 = 147,
        LANG5 = 148,
        LANG6 = 149,
        LANG7 = 150,
        LANG8 = 151,
        LANG9 = 152,
        ALTERASE = 153,
        SYSREQ = 154,
        CANCEL = 155,
        CLEAR = 156,
        PRIOR = 157,
        RETURN2 = 158,
        SEPARATOR = 159,
        OUT = 160,
        OPER = 161,
        CLEARAGAIN = 162,
        CRSEL = 163,
        EXSEL = 164,
        KP_00 = 176,
        KP_000 = 177,
        THOUSANDSSEPARATOR = 178,
        DECIMALSEPARATOR = 179,
        CURRENCYUNIT = 180,
        CURRENCYSUBUNIT = 181,
        KP_LEFTPAREN = 182,
        KP_RIGHTPAREN = 183,
        KP_LEFTBRACE = 184,
        KP_RIGHTBRACE = 185,
        KP_TAB = 186,
        KP_BACKSPACE = 187,
        KP_A = 188,
        KP_B = 189,
        KP_C = 190,
        KP_D = 191,
        KP_E = 192,
        KP_F = 193,
        KP_XOR = 194,
        KP_POWER = 195,
        KP_PERCENT = 196,
        KP_LESS = 197,
        KP_GREATER = 198,
        KP_AMPERSAND = 199,
        KP_DBLAMPERSAND = 200,
        KP_VERTICALBAR = 201,
        KP_DBLVERTICALBAR = 202,
        KP_COLON = 203,
        KP_HASH = 204,
        KP_SPACE = 205,
        KP_AT = 206,
        KP_EXCLAM = 207,
        KP_MEMSTORE = 208,
        KP_MEMRECALL = 209,
        KP_MEMCLEAR = 210,
        KP_MEMADD = 211,
        KP_MEMSUBTRACT = 212,
        KP_MEMMULTIPLY = 213,
        KP_MEMDIVIDE = 214,
        KP_PLUSMINUS = 215,
        KP_CLEAR = 216,
        KP_CLEARENTRY = 217,
        KP_BINARY = 218,
        KP_OCTAL = 219,
        KP_DECIMAL = 220,
        KP_HEXADECIMAL = 221,
        LCTRL = 224,
        LSHIFT = 225,
        LALT = 226,
        LGUI = 227,
        RCTRL = 228,
        RSHIFT = 229,
        RALT = 230,
        RGUI = 231,
        MODE = 257,
        // These come from the USB consumer page (0x0C)
        AUDIONEXT = 258,
        AUDIOPREV = 259,
        AUDIOSTOP = 260,
        AUDIOPLAY = 261,
        AUDIOMUTE = 262,
        MEDIASELECT = 263,
        WWW = 264,
        MAIL = 265,
        CALCULATOR = 266,
        COMPUTER = 267,
        AC_SEARCH = 268,
        AC_HOME = 269,
        AC_BACK = 270,
        AC_FORWARD = 271,
        AC_STOP = 272,
        AC_REFRESH = 273,
        AC_BOOKMARKS = 274,
        // These come from other sources, and are mostly mac related
        BRIGHTNESSDOWN = 275,
        BRIGHTNESSUP = 276,
        DISPLAYSWITCH = 277,
        KBDILLUMTOGGLE = 278,
        KBDILLUMDOWN = 279,
        KBDILLUMUP = 280,
        EJECT = 281,
        SLEEP = 282,
        APP1 = 283,
        APP2 = 284,
        // This is not a key, simply marks the number of scancodes
        // so that you know how big to make your arrays.
        SDL_NUM_SCANCODES = 512
    }

    [Flags]
    enum GamePadAxes : byte
    {
        LeftX = 1 << 0,
        LeftY = 1 << 1,
        LeftTrigger = 1 << 2,
        RightX = 1 << 3,
        RightY = 1 << 4,
        RightTrigger = 1 << 5,
    }

    enum ConfigurationType
    {
        Unmapped = 0,
        Axis,
        Button,
        Hat
    }
    enum GameControllerBindType : byte
    {
        None = 0,
        Button,
        Axis,
        Hat
    }
    [Flags]
    enum HatPosition : byte
    {
        Centered = 0x00,
        Up = 0x01,
        Right = 0x02,
        Down = 0x04,
        Left = 0x08,
        RightUp = Right | Up,
        RightDown = Right | Down,
        LeftUp = Left | Up,
        LeftDown = Left | Down
    }
    enum HapticEffects : ushort
    {
        SDL_HAPTIC_CONSTANT = (1 << 0),
        SDL_HAPTIC_SINE = (1 << 1),
        SDL_HAPTIC_LEFTRIGHT = (1 << 2),
        SDL_HAPTIC_TRIANGLE = (1 << 3),
        SDL_HAPTIC_SAWTOOTHUP = (1 << 4),
        SDL_HAPTIC_SAWTOOTHDOWN = (1 << 5),
        SDL_HAPTIC_SPRING = (1 << 7),
        SDL_HAPTIC_DAMPER = (1 << 8),
        SDL_HAPTIC_INERTIA = (1 << 9),
        SDL_HAPTIC_FRICTION = (1 << 10),
        SDL_HAPTIC_CUSTOM = (1 << 11),
        SDL_HAPTIC_GAIN = (1 << 12),
        SDL_HAPTIC_AUTOCENTER = (1 << 13),
        SDL_HAPTIC_STATUS = (1 << 14),
        SDL_HAPTIC_PAUSE = (1 << 15),
    }

    enum HapticDirectionType : byte
    {
        SDL_HAPTIC_POLAR = 0,
        SDL_HAPTIC_CARTESIAN = 1,
        SDL_HAPTIC_SPHERICAL = 2,
    }

    enum HapticRunEffect : uint
    {
        SDL_HAPTIC_INFINITY = 4292967295U,
    }
    #endregion
}
#endif
