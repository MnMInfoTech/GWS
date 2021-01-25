/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if GWS || Window

using System;

namespace MnM.GWS
{
    #region FILL MODE
    ///<summary>
    /// Represents various option to create a brush to fill a surface.
    /// </summary>
    public enum FillMode
    {
        /// <summary>
        /// Fill the shape as it physically appears with its original area.
        /// </summary>
        Original,
        /// <summary>
        /// If stroke is applied, it fills the inner child shape. Outer shape is non existant.
        /// </summary>
        Inner,
        /// <summary>
        /// If stroke is applied, it draws the pixel boundary between outer and inner child shape.
        /// </summary>
        DrawOutLine,
        /// <summary>
        /// If stroke is applied, it fills the pixel boundary between outer and inner child shape.
        /// </summary>
        FillOutLine,
        /// <summary>
        /// If stroke is applied, it draws the pixel boundary of outer shape and fills inner child shape, giving a fill of hollow shape.
        /// </summary>
        ExceptOutLine,
        /// <summary>
        /// If stroke is applied, it fills the outer shape wholly which results in inner child shape non existant.
        /// </summary>
        Outer,
    }
    #endregion

    #region STROKE MODE
    [Flags]
    public enum StrokeMode
    {
        /// <summary>
        /// Positions stroke so that its middle is the thoeretical border of the shape. 
        /// Both expands the rectangle that contains the shape and shrinks the vissible space inside the stroke.
        /// </summary>
        StrokeMiddle = 0x0,

        /// <summary>
        /// Positions stroke on the outside of the theoretical border of the shape. 
        /// Expands the rectangle containing the shape but does not change the internal area of the shpe.
        /// </summary>
        StrokeOuter = 0x1,

        /// <summary>
        /// Positions stroke so that the theoretical border of the shape is on the outside. 
        /// Shrinks the space inside the shape and maintains the size of the enclosing rectangle.
        /// </summary>
        StrokeInner = 0x2,
    }
    #endregion

    #region POINT JOIN
    [Flags]
    public enum PointJoin
    {
        /// <summary>
        /// No Join required.!!!!
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Ensures each point is connected to its neighbours.
        /// </summary>
        ConnectEach = 0x1,
        /// <summary>
        /// Used in Bezier to prevent errors in stroke drawing.
        /// </summary>
        RemoveLast = 0x2,
        /// <summary>
        /// Only take a point in to account if it is a minimum distance away.
        /// </summary>
        AvoidTooClose = 0x4,
        /// <summary>
        /// Ignores repeated points and join the remainder.
        /// </summary>
        NoRepeat = 0x8,
        /// <summary>
        /// Connects the end point to the start point.
        /// </summary>
        ConnectEnds = 0x10,
        /// <summary>
        /// Connect each point as long as it is not a repeated point.
        /// </summary>
        CircularJoinOpen = ConnectEach | NoRepeat,
        /// <summary>
        /// Used for closed shapes ignore repeated points
        /// </summary>
        CircularJoin = ConnectEach | ConnectEnds | NoRepeat,
        /// <summary>
        /// Connects end with start point.
        /// </summary>
        PieJoin = CircularJoin,
        /// <summary>
        /// Connects each point but do not connect end to start
        /// </summary>
        ArcJoin = CircularJoinOpen | NoRepeat | RemoveLast,
        /// <summary>
        /// Connect each point and join end to start.
        /// </summary>
        CloseArcJoin = ConnectEach | ConnectEnds,
        /// <summary>
        /// Ensures the firt and last point in the Bezier are not connected.
        /// </summary>
        BezierJoin = ConnectEach | RemoveLast,
        /// <summary>
        /// Joins end point to start point.
        /// </summary>
        PolygonJoin = CircularJoin
    }
    #endregion

    #region AFTER STROKE
    [Flags]
    public enum AfterStroke
    {
        /// <summary>
        /// Do nothing.!!!!
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Normally Strokes are drawn from the second point on the line.
        /// In some cases we need to rest the points that have been removed.
        /// For example: an Arc which must close the ends of the stroke perimiter lines and so the end points are required.
        /// </summary>
        Reset1st = 0x4,
        /// <summary>
        /// Joins the end points of the two lines on the perimeter of the stroke and joins them. 
        /// Example the ends of a stroke drawing an arc must be closed.
        /// </summary>
        JoinEnds = 0x8,
        /// <summary>
        /// Used for Strokes that define a perimiter that has a start and end such as an Arc.
        /// </summary>
        Both = Reset1st | JoinEnds,
    }
    #endregion

    #region FLIP
    [Flags]
    public enum FlipMode
    {
        None = 0x00000000,
        Horizontal = 0x00000001,
        Vertical = 0x00000002
    }
    #endregion

    #region ROUNDBOX OPTION
    [Flags]
    public enum RoundBoxOption
    {
        Normal = 0,
        Left = 0x1,
        Top = 0x2,
        Right = 0x4,
        Bottom = 0x8,
        Banner = 0x10,
    }
    #endregion
    #region BEZIER TYPE
    public enum BezierType
    {
        Cubic = 4,
        Quadratric = 3,
        Multiple = 7,
    }
    #endregion

    #region CURVE TYPE
    [Flags]
    public enum CurveType
    {
        /// <summary>
        /// Represents full ellipse or circle.
        /// </summary>
        Full = 0x0,

        /// <summary>
        /// Represents an arc with start and sweep/end angles.
        /// </summary>
        Arc = 0x1,

        /// <summary>
        /// Represents a pie with start and sweep/end angles.
        /// </summary>
        Pie = 0x2,

        /// <summary>
        /// By default, end angle is sweeped i.e end angle += start angle.
        /// If this option is added, arc or pie will be created using absolute end angle value.
        /// </summary>
        NoSweepAngle = 0x8,

        /// <summary>
        /// Draws an arc or pie cutting it in anti clock wise motion.
        /// i.e. if start-angle 0 and end-angle = 90, this option will draw an arc or pie from 90 to 360.
        /// </summary>
        AntiClock = 0x10,

        /// <summary>
        /// Fills an area of Arc betwwen start and end points.
        /// </summary>
        ClosedArc = Arc | 0x20,

        /// <summary>
        /// Draws an arc or pie or ellipse fitting the boundary defined by three points given. 
        /// </summary>
        Fitting = 0x40,

        /// <summary>
        /// Only applicable when ellipse/ arc / pie is created using theree points. And Fitting flag is not included.
        /// This flag will make sure that the third point will not be treated as center of ellipsoid but rather a point on ellipsoid itself.
        /// </summary>
        ThirdPointOnEllipse = 0x80,

        /// <summary>
        /// Only applicable when ellipse/ arc / pie is created using four points. And Fitting flag is not included.
        /// This flag will make sure that the fourth point will not be treated a point on ellipsoid itself but rather as center of ellipsoid.
        /// This will also mean that first and second points are guranteed on ellipse and third one can not be so.
        /// </summary>
        FourthPointIsCenter = 0x100,

        /// <summary>
        /// Only applicable in stroked curve. 
        /// If applied, this option will swap centers of outer curve and inner curve which
        /// in effect creates cross stroking effcet i.e outer curve is cut by inner curve arcline and 
        /// inner curve is cut by outer curve arcline.
        /// </summary>
        CrossStroke = 0x200,
    }
    #endregion

    #region LINE TYPE
    [Flags]
    public enum LineType
    {
        /// <summary>
        /// Diagonal line.
        /// </summary>
        Diagonal = 0x0,
        /// <summary>
        /// Horizontal line i.e. y1 = y2 or difference between them is 0.0001f
        /// </summary>
        Horizontal = 0x1,
        /// <summary>
        /// Vertical line i.e. x1 = x2 or difference between them is 0.0001f
        /// </summary>
        Vertical = 0x2,
        /// <summary>
        /// Just a point i.e. deifference between x1 and x2 as well as y1 and y2 is less than 0.0001f.
        /// </summary>
        Point = 0x4,
    }
    #endregion

    #region SLOPE TYPE
    [Flags]
    public enum SlopeType
    {
        None = 0x0,
        NonSteep = 0x1,
        Steep = 0x2,
        Both = NonSteep | Steep,
    }
    #endregion

    #region BRUSH TYPE
    [Flags]
    public enum BrushType
    {
        /// <summary>
        /// For texture brushes only.
        /// </summary>
        Texture = -1,

        /// <summary>
        /// Solid fill with first color specified.
        /// </summary>
        Solid = 0,

        /// <summary>
        /// Fill changes colour along the horizontal.
        /// </summary>
        Horizontal = 1,

        /// <summary>
        /// Fill changes color along the vertical.
        /// </summary>
        Vertical = 2,

        /// <summary>
        /// Fill changes color along the forward diagonal
        /// </summary>
        ForwardDiagonal = 3,

        /// <summary>
        /// Fill changes color along the backward diagonal.
        /// </summary>
        BackwardDiagonal = 4,

        /// <summary>
        /// Fill changes color from Left to right for the part of the shape in the top half of the enclosing rectangle.
        /// Fill goes right to left for bottom half of the enclosing rectangle.
        /// </summary>
        HorizontalSwitch = 5,

        /// <summary>
        /// Fill changes colour from left to central vertical and then from right to central vertical.
        /// </summary>
        HorizontalCentral = 6,

        /// <summary>
        /// Fill changes colour from top to central horizontal and then from bottom to central horizontal.
        /// </summary>
        VerticalCentral = 7,

        /// <summary>
        /// Fill changes colour from top left to central Forward diagonal and then from bottom right to central forward diagonal.
        /// </summary>
        DiagonalCentral = 8,

        /// <summary>
        /// Fill changes color as it rotates around the centre of the rectangle enclosing the shape - starts at 0 degree. 
        /// The color is symetric along the back diagonal.
        /// </summary>
        Conical = 9,

        /// <summary>
        /// Fill changes color as it rotates around the centre, however operation starts at 90 degree. 
        /// The color is symetric along the back diagonal.
        /// </summary>
        Conical2 = 10,

        /// <summary>
        /// Fill changes color as it goes from outer most perimeter towards center referencing a biggest possible circle within perimeter. 
        /// </summary>
        Circular = 13,

        /// <summary>
        /// Fill changes color as it goes from outer most perimeter towards center referencing a biggest possible ellipse within perimeter. 
        /// </summary>
        Elliptical = 14,

        /// <summary>
        /// Fill changes color as it rotates around the centre in rectangular fashion. 
        /// </summary>
        Rectangular = 15,

        MiddleCircular = 16,
    }
    #endregion

    #region ANIMATION MODE
    /// <summary>
    /// Defines a mode of animating a shape on regular interval.
    /// </summary>
    [Flags]
    public enum AnimationMode
    {
        /// <summary>
        /// No animation!
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Animates an object by applying different fill mode that GWS provides on sequential manner -i.e. one by one and in cycle.
        /// </summary>
        ByFillMode = 0x1,

        /// <summary>
        /// Animates an object by increasing or descring the size of stroke by unit specified and once reached max level, reversing it with the same unit. 
        /// </summary>
        ByStrokeSize = 0x2,

        /// <summary>
        /// Animates an object by offsetting its location by unit specified and once reached max level, reversing it with the same unit. 
        /// </summary>
        ByLocation = 0x4,

        /// <summary>
        /// Animates an object by applying a rotation by increasing or decreasing a value of an angle specified and once reached 360 - or 0 level, reversing it with the same unit. 
        /// </summary>
        ByRotation = 0x8,

        /// <summary>
        /// Animates an object by applying gradient styles one by one that GWS provides and once it reaches the last option, reversing it one by one. 
        /// </summary>
        ByGradient = 0x10,

        /// <summary>
        /// Animates an object by showing and hiding it on regular interval. 
        /// </summary>
        ByBlinking = 0x20,

        /// <summary>
        /// Animates an object by any other animation that user has specified on regular interval.The user has to provide animation routine here.
        /// </summary>
        UserDefined = 0x40,
    }
    #endregion

    #region GAME PAD TYPE
    /// <summary>
    /// Enumerates available <see cref="GamePad"/> types.
    /// </summary>
    public enum GamePadType
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
    public enum InputType
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
    public enum JoystickHat
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
    public enum JoystickPowerLevel
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
    public enum JoystickType
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

    #region FONT WEIGHT
    /// <summary>
    /// Specifies various font weights.
    /// </summary>
    public enum FontWeight
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
    public enum FontStretch
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

    #region RENDERER FLAGS
    [Flags]
    public enum RendererFlags : uint
    {
        Default = 0x0000000,
        Software = 0x00000001,
        Accelarated = 0x00000002,
        PresentSync = 0x00000004,
        Texture = 0x00000008
    }
    #endregion

    #region RENDERING HINT
    public enum RenderingHint
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
    public enum TextureAccess
    {
        Static,
        Streaming,
        Target,
    }
    #endregion

    #region INTER POLATION
    public enum Interpolation
    {
        NearestNeighbor,
        BiLinear,
        Bicubic,
    }
    #endregion

    #region CONIC TYPE
    public enum ConicType
    {
        Ellipse,
        Hyperbola,
        Parabola,
    }
    #endregion

    #region QUAD TYPE
    public enum QuadType
    {
        Rhombus,
        Trapezium,
        Trapezoid,
        RoundBox,
    }
    #endregion

    #region GWS WINDOW FLAGS
    /// <summary>
    /// Window creation flags. For SDL we have mapped all options available here.
    /// For different windowing system of your choice. You will have to map and design your underlying window to reflect these options.
    /// </summary>
    [Flags]
    public enum GwsWindowFlags
    {
        /// <summary>
        /// None means default
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Enables window to be resizeable.
        /// </summary>
        Resizable = 0x1,
        /// <summary>
        /// Sets window on full screen mode.
        /// </summary>
        FullScreen = 0x2,
        /// <summary>
        /// Sets rendering context of window explicitely to OpenGL.
        /// </summary>
        OpenGL = 0x4,
        /// <summary>
        /// Sets window as displayed.
        /// </summary>
        Shown = 0x8,
        /// <summary>
        /// Sets window hidden.
        /// </summary>
        Hidden = 0x10,
        /// <summary>
        /// Cretes widow without border.
        /// </summary>
        NoBorders = 0x20,
        /// <summary>
        /// Sets window in minimized state.
        /// </summary>
        Minimized = 0x40,
        /// <summary>
        /// Sets window in maximized state.
        /// </summary>
        Maximized = 0x80,
        /// <summary>
        /// Sets window to capture mouse
        /// </summary>
        GrabInput = 0x100,
        /// <summary>
        /// Sets window focusable and capable receiving inputs.
        /// </summary>
        InputFocus = 0x200,
        /// <summary>
        /// Set mouse focus to window.
        /// </summary>
        MouseFocus = 0x400,
        /// <summary>
        /// Sets window as full screen desktop! (SDL has some issues with this).
        /// </summary>
        FullScreenDesktop = 0x800,
        /// <summary>
        /// Use this only if you are attaching a foreign window handle to this window.
        /// </summary>
        Foreign = 0x1000,
        /// <summary>
        /// Tells GWS that window should be created in high-DPI mode if supported (for SDL it is possible in version>= SDL 2.0.1)
        /// </summary>
        HighDPI = 0x2000,
        /// <summary>
        /// Sets window in MultiThread environment.
        /// </summary>
        MultiThread = 0x4000,
        /// <summary>
        /// Enables DirectRender option for window.
        /// For SDL, built in support is provided, if you want to use non SDL windowing then you must design surface - implementing
        /// IWindowSurface in your chosen system and then make it capable to render directly on screen of the window.
        /// </summary>
        EnableDirectRender = 0x8000,
    }
    #endregion

    #region OS
    /// <summary>
    /// Operating System Information enum.
    /// </summary>
    [Flags]
    public enum OS
    {
        /// <summary>
        /// Not determined.
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Operating system is Windows 
        /// </summary>
        Windows = 0x1,
        /// <summary>
        /// Operating system is Android 
        /// </summary>
        Android = 0x2,
        /// <summary>
        /// Operating system is IPhone 
        /// </summary>
        IPhone = 0x4,
        /// <summary>
        /// Operating system is Linux 
        /// </summary>
        Linux = 0x8,
        /// <summary>
        /// Operating system is MacOsX 
        /// </summary>
        MacOsX = 0x10,
        /// <summary>
        /// Operating system is IOS 
        /// </summary>
        IOS = 0x20,
        /// <summary>
        /// Operating system architecture is 32 bit 
        /// </summary>
        X86 = 0x40,
        /// <summary>
        /// Operating system architecture is 64 bit 
        /// </summary>
        X64 = 0x80,
        /// <summary>
        /// Operating system architecture is ARM 
        /// </summary>
        Arm = 0x100,
    }
    #endregion

    #region WINDOW BORDER
    public enum WindowBorder
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

    #region WINDOW STATE
    public enum WindowState
    {
        /// <summary>
        /// The window is  its normal state.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// The window is minimized to the taskbar (also known as 'iconified').
        /// </summary>
        Minimized,
        /// <summary>
        /// The window covers the whole working area, which includes the desktop but not the taskbar and/or panels.
        /// </summary>
        Maximized,
    }
    #endregion

    #region CURSOR TYPE
    public enum CursorType
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

    #region POPUP POSITION
    public enum PopupPosition
    {
        None,
        Bottom,
        Top
    }
    #endregion

    #region CLOSE REASON
    public enum CloseReason
    {
        AppFocusChange = 0,
        AppClicked = 1,
        ItemClicked = 2,
        Keyboard = 3,
        CloseCalled = 4,
    }
    #endregion

    #region MOUSE DRAG
    public enum MouseDrag
    {
        None = 0,
        Begun,
        Continue,
        Ended,
    }
    #endregion

    #region SOUND
    public enum SoundStatus
    {
        /// <summary>Sound is not playing</summary>
        Stopped,

        /// <summary>Sound is paused</summary>
        Paused,

        /// <summary>Sound is playing</summary>
        Playing
    }
    #endregion

#if GWS || Window || Advanced
    #region GWS EVENT
    /// <summary>
    /// Represents GwsEvent
    /// </summary>
    public enum GwsEvent
    {
        /// <summary>
        /// Just a place holder at 0 index.
        /// </summary>
        First = 0,

        /// <summary>
        /// System has quitted
        /// </summary>
        Quit,

        /// <summary>
        /// System is handling window event
        /// </summary>
        WindowEvent,

        /// <summary>
        /// System is handling window event
        /// </summary>
        SysWmEvent,

        /// <summary>
        /// System has thrown key down event
        /// </summary>
        KeyDown,

        /// <summary>
        /// System has thrown key up event
        /// </summary>
        KeyUp,

        /// <summary>
        /// System has indicated an act of entering some text
        /// </summary>
        TextInput,

        /// <summary>
        /// System has indicated mouse motion
        /// </summary>
        MouseMotion,

        /// <summary>
        /// System has indicated mouse down
        /// </summary>
        MouseDown,

        /// <summary>
        /// System has indicated mouse up
        /// </summary>
        MouseUp,

        /// <summary>
        /// System has indicated mouse wheel motion
        /// </summary>
        MouseWheel,

        /// <summary>
        /// System has indicated joy axis motion
        /// </summary>
        JoyAxisMotion,

        /// <summary>
        /// System has indicated joy ball motion
        /// </summary>
        JoyBallMotion,

        /// <summary>
        /// System has indicated joy hat motion
        /// </summary>
        JoyHatMotion,

        /// <summary>
        /// System has indicated joy button down
        /// </summary>
        JoyButtonDown,

        /// <summary>
        /// System has indicated joy button up
        /// </summary>
        JoyButtonUp,

        /// <summary>
        /// System has indicated a joy device has been added
        /// </summary>
        JoystickAdded,

        /// <summary>
        /// System has indicated a joy device has been removed
        /// </summary>
        JoystickRemoved,

        /// <summary>
        /// System has indicated a controller axis motion
        /// </summary>
        ControllerAxisMotion,

        /// <summary>
        /// System has indicated controller button down
        /// </summary>
        ControllerButtonDown,

        /// <summary>
        /// System has indicated controller button up
        /// </summary>
        ControllerButtonUp,

        /// <summary>
        /// System has indicated a controller device has been added
        /// </summary>
        ControllerAdded,

        /// <summary>
        /// System has indicated a controller device has been removed
        /// </summary>
        ControllerRemoved,

        /// <summary>
        /// System has indicated a controller device has been mapped
        /// </summary>
        ControllerMapped,

        /// <summary>
        /// System has indicated a fingure down on screen
        /// </summary>
        FingerDown,

        /// <summary>
        /// System has indicated a fingure up on screen
        /// </summary>
        FingerUp,

        /// <summary>
        /// System has indicated a fingure motion on screen
        /// </summary>
        FingerMotion,

        /// <summary>
        /// System has indicated a dollar gesture on screen
        /// </summary>
        DollarGesture,

        DollarRecord,

        /// <summary>
        /// System has indicated multi gestures of fingure movements on screen
        /// </summary>
        MultiGesture,

        /// <summary>
        /// System has indicated an update of clipboard
        /// </summary>
        ClipBoardUpdate,

        /// <summary>
        /// System has indicated a file drop
        /// </summary>
        DropFile,

        /// <summary>
        /// Any user instigated event
        /// </summary>
        UserEvent,

        /// <summary>
        /// System has indicated a text drop on some window
        /// </summary>
        DropText,

        /// <summary>
        /// System has indicated a beginning of drop operation on some window
        /// </summary>
        DropBegin,

        /// <summary>
        /// System has indicated a completion of drop operation on some window
        /// </summary>
        DropComplete,

        /// <summary>
        /// System has indicated an audio device is added
        /// </summary>
        AudioDeviceAdded,

        /// <summary>
        /// System has indicated an audio device is removed
        /// </summary>
        AudioDeviceRemoved,

        /// <summary>
        /// System has indicated a render target on current window is reset
        /// </summary>
        RENDER_TARGETS_RESET,

        /// <summary>
        /// System has indicated a render device on current window is reset
        /// </summary>
        RENDER_DEVICE_RESET,

        /// <summary>
        /// Window is shown
        /// </summary>
        Shown,

        /// <summary>
        /// Window is shown
        /// </summary>
        Hidden,

        /// <summary>
        /// Window is exposed sort of shown
        /// </summary>
        Exposed,

        /// <summary>
        /// Window is moved
        /// </summary>
        Moved,

        /// <summary>
        /// Window is resized
        /// </summary>
        Resized,

        /// <summary>
        /// Window's size is changed
        /// </summary>
        SizeChanged,

        /// <summary>
        /// Window is minimized
        /// </summary>
        Minimized,

        /// <summary>
        /// Window is maximized
        /// </summary>
        Maximized,

        /// <summary>
        /// Window is restored
        /// </summary>
        Restored,

        /// <summary>
        /// Mouse has entered in the window.
        /// </summary>
        Enter,

        /// <summary>
        /// Mouse has left the window.
        /// </summary>
        Leave,

        /// <summary>
        /// Window has gained the focus
        /// </summary>
        FocusGained,

        /// <summary>
        /// Window has left the focus
        /// </summary>
        FocusLost,

        /// <summary>
        /// Window is closing
        /// </summary>
        Close,

        /// <summary>
        /// Repaint the window.
        /// </summary>
        Paint,

        /// <summary>
        /// Mouse is clicked outside window.
        /// </summary>
        AppClick,

        /// <summary>
        /// 
        /// </summary>
        MouseClick,

        /// <summary>
        /// 
        /// </summary>
        FirstShown,

        /// <summary>
        /// Place holder last index in screen.
        /// </summary>
        LASTEVENT,
    }
    #endregion

    #region WINDOW EVENT ID
    /// <summary>
    /// Represents the current event.
    /// </summary>
    public enum WindowEventID : byte
    {
        /// <summary>
        /// No event
        /// </summary>
        NONE,

        /// <summary>
        /// Window is shown
        /// </summary>
        Shown,

        /// <summary>
        /// Window was made hidden
        /// </summary>
        Hidden,

        /// <summary>
        /// Window is exposed - sort of shown
        /// </summary>
        Exposed,

        /// <summary>
        /// Window is moved
        /// </summary>
        Moved,

        /// <summary>
        /// Window is resized
        /// </summary>
        Resized,

        /// <summary>
        /// Window's size is changed
        /// </summary>
        SizeChanged,

        /// <summary>
        /// Window is minimized
        /// </summary>
        MiniMized,

        /// <summary>
        /// Window is maximized
        /// </summary>
        Maximized,

        /// <summary>
        /// Window is restored to its previous state.
        /// </summary>
        Restored,

        /// <summary>
        /// Mouse has entered in the window.
        /// </summary>
        Enter,

        /// <summary>
        /// Mouse has left the window
        /// </summary>
        Leave,

        /// <summary>
        /// Window receives focus
        /// </summary>
        FocusGained,

        /// <summary>
        /// Window has lost focus
        /// </summary>
        FocusLost,

        /// <summary>
        /// Window is colsed
        /// </summary>
        Close,
    }
    #endregion
#endif
}
#endif

