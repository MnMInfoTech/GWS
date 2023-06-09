/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;

namespace MnM.GWS
{
#if !Advance
    #region COMMAND
    [Flags]
    internal enum Command : ulong
    {
        /// <summary>
        /// No specific command.
        /// </summary>
        Default = Numbers.Flag0,

        #region INDEPENDENT COMMAND
        /// <summary>
        /// Tells GWS to overwrite destination pixel only if it is transparent.
        /// Usage: FillAction, DrawAction delegates, IImageReceiver.DrawImage.
        /// </summary>
        Backdrop = Numbers.Flag1,

        /// <summary>
        /// Tells GWS to skip something while copying i.e. FrontBuffer or BackBuffer or Screen etc.. 
        /// Usage: ICopyable.CopyTo,IUploadable.CopyScreen, Blocks.Copy.
        /// </summary>
        Skip = Numbers.Flag2,

        /// <summary>
        /// Tells GWS to ignore internal clip set on graphics object while rendering. Pro version only.
        /// Usage: FillAction, DrawAction and PointAction delegates, IGraphics.DrawImage .
        /// </summary>
        NoClip = Numbers.Flag3,

        ///// <summary>
        ///// Tells GWS to invert colour pixel before it is displayed.
        ///// Usage: FillAction, DrawAction delegates, IImageReceiver.DrawImage.
        ///// IUpdatable.U
        ///// </summary>
        InvertColour = Numbers.Flag4,
        #endregion

        #region UPDATE COMMAND
        /// <summary>
        /// Tells GWS to skips update process entirely. 
        /// Usage: IUpdatable.Update - When update is managed externally.
        /// </summary>
        SkipDisplayUpdate = Numbers.Flag5,

        /// <summary>
        /// Tells GWS to wipe all temporary drawings while refreshing screen.
        /// If used stand alone, it will remove all screen ZOrder flags.
        /// Usage: IUpdatable.Update - Pro version only.
        /// </summary>
        RestoreView = Numbers.Flag6,

        /// <summary>
        /// Tells GWS to reconstruct view from scratch by consolidating all layers excluding hidden pixels. 
        /// Pro version only.
        /// Caution: Expansive operation use only when needed.
        /// Usage: IUpdatable.Update
        /// </summary>
        UpdateView = Numbers.Flag7,

        /// <summary>
        /// Tells GWS to update screen without copying data without consolidating view layers.  
        /// Usage: IUpdatable.Update.
        /// </summary>
        UpdateScreenOnly = Numbers.Flag8,

        /// <summary>
        /// Tells GWS to fill screen pixels which are empty with background pen. 
        /// Usage: IUpdatable.Update. 
        /// Pro version only.
        /// </summary>
        RestoreScreen = RestoreView | Skip,
        #endregion

        #region FILL OPTIONS
        /// <summary>
        /// Fill the shape as it physically appears with its original area.
        /// </summary>
        OriginalFill = Numbers.Flag9,

        /// <summary>
        /// Positions stroke on the outside of the theoretical border of the shape. 
        /// Expands the rectangle containing the shape but does not change the internal area of the shape.
        /// </summary>
        StrokeOuter = Numbers.Flag10,

        /// <summary>
        /// Positions stroke so that the theoretical border of the shape is on the outside. 
        /// Shrinks the space inside the shape and maintains the size of the enclosing rectangle.
        /// </summary>
        StrokeInner = Numbers.Flag11,

        /// <summary>
        /// If stroke is applied, it draws the pixel boundary between outer and inner child shape.
        /// </summary>
        DrawOutLines = Numbers.Flag12,

        /// <summary>
        /// If stroke is applied, it draws the pixel boundary of outer shape 
        /// and fills inner child shape, giving a fill of hollow shape.
        /// </summary>
        FillOddLines = Numbers.Flag13,

        /// <summary>
        /// Tells GWS to use flood-fill rule to fill polygon area.
        /// </summary>
        FloodFill = Numbers.Flag14,

        /// <summary>
        /// Tells GWS to skip filling axial lines which define a body of the shape.
        /// </summary>
        SkipFill = Numbers.Flag15,

        /// <summary>
        /// Tells GWS to skip drawing lines which define an edge of the shape.
        /// </summary>
        SkipDraw = Numbers.Flag16,

        /// <summary>
        /// Tells GWs to render polygon exactly opposite of odd-even method i.e.
        /// ignoring odd sequences instead of even sequences of portions of the given axial line.
        /// </summary>
        XORFill = SkipDraw | SkipFill | Skip,
        #endregion

        #region LINE OPTIONS
        /// <summary>
        /// Tells GWS that the line should be drawn without using anti-aliasing.
        /// </summary>
        Bresenham = Numbers.Flag17,

        /// <summary>
        /// A gap of one pixel will be left after each drawn pixel.
        /// </summary>
        DottedLine = Numbers.Flag18,

        /// <summary>
        /// A gap of two pixel will be left after each pair of two drawn pixel.
        /// </summary>
        DashedLine = Numbers.Flag19,

        /// <summary>
        /// A gap of one pixel will be left after the first two drawn pixel.
        /// And then next two pixels will be drawn normally.
        /// This pattern gets repeated subsequently.
        /// </summary>
        DashDotDashLine = Numbers.Flag20,

        /// <summary>
        /// Tells GWS to smoothen edges (joint location between lines) in continuous line drawing.
        /// </summary>
        LineCap = Numbers.Flag21,

        /// <summary>
        /// Tells GWS to draw end pixel after lone pixel loop finished.
        /// </summary>
        DrawEndPixel = Numbers.Flag22,
        #endregion

        #region BRUSH COMMAND
        /// <summary>
        /// Tells GWS to prevent auto-sizing of brush according to the size of shape being rendered.
        /// Usage: ISession.SetPen by changing command of settings object.
        /// </summary>
        BrushNoSizeToFit = Numbers.Flag23,

        /// <summary>
        /// Tells GWS to prevent auto-location match of brush according to the location of shape being rendered.
        /// Usage: ISession.SetPen by changing command of settings object.
        /// </summary>
        BrushNoAutoPosition = Numbers.Flag24,

        /// <summary>
        /// Tells GWS to take exact position of writing co-ordinates and read pixel from the brush at exact same position.
        /// Usage: ISettings.SetPen by changing command of settings object.
        /// </summary>
        BrushFollowCanvas = Numbers.Flag25,

        /// <summary>
        /// Tells GWS to invert the rotation angle of the brush in opposite direction then that of shape rotation.
        /// Usage: ISession.SetPen by changing command of settings object.
        /// </summary>
        BrushInvertRotation = Numbers.Flag26,
        #endregion

        #region COPY COMMAND
        /// <summary>
        /// Tells GWS to copy only view which involves its child controls only.
        /// This is useful when you have transparent window which displays 
        /// desktop controls and/or desktop images as well. Then this option allows to ignore them.
        /// </summary>
        CopyContentOnly = Numbers.Flag27,

        /// <summary>
        /// Tells GWS to copy currently displayed screen including menus and tooltips or other temporary objects.
        /// Usage: command option in IUploadable.CopyScreen only when external target buffer is chosen.
        /// </summary>
        CopyScreen = Numbers.Flag28,

        /// <summary>
        /// Tells GWS that currently an image data is being saved after copying to external file.
        /// </summary>
        CopyRGBOnly = Numbers.Flag29,

        /// <summary>
        /// Tells GWS that currently an image data is being saved after copying to external file.
        /// </summary>
        CopyBackground = Numbers.Flag30,

        /// <summary>
        /// Tells GWS while copying data, swap Red channel with Blue that of source colour before copying to target.
        /// </summary>
        SwapRedBlueChannel = Numbers.Flag31,

        /// <summary>
        /// Tells GWS that currently an image data copy operation must be opaque i.e.
        /// To copy all data even if the source colour is empty.
        /// </summary>
        CopyOpaque = Numbers.Flag32,
        #endregion

        #region CLEAR COMMAND
        /// <summary>
        /// Tells GWS to skip front (control) buffer from any involvement in any operation.
        /// </summary>
        SkipDesktop = Numbers.Flag33,
        #endregion

        #region CONTROL DRAW
        /// <summary>
        /// Tells GWS that the current draw on buffer is origin based 
        /// i.e location of the draw is (0, 0) + Minimum point of shape perimeter.
        /// </summary>
        OriginBased = Numbers.Flag34,

        /// <summary>
        /// Tells GWS that for the purpose of the current draw on buffer,
        /// the buffer must be resized to accommodate all of shape drawing area.
        /// Or to shrink or expand an image to fit the area specified for copy.
        /// </summary>
        SizeToFit = Numbers.Flag35,

        /// <summary>
        /// Tells GWS to draw edges of a shape even if it is drawn on backdrop.
        /// Usage: Writable.WritePixel, Renderer.ProcessLine.
        /// </summary>
        KeepBorderOnBackdrop = Numbers.Flag36,

        /// <summary>
        /// Tells GWS that current rendering operation should be treated as transparent draw operation.
        /// i.e. do everything except filling pixel with colour.
        /// </summary>
        Transparent = Numbers.Flag37,

        /// <summary>
        /// Tells GWS that current operation must skip any data scaling or rotation tasks
        /// even if valid rotation and scale are supplied.
        /// </summary>
        SkipRotateScale = Numbers.Flag38,

        /// <summary>
        /// Tells GWS to draw border while rendering by inverting colour used to rendering body.
        /// </summary>
        InvertedBorder = Numbers.Flag39,
        #endregion

        #region WINDOW DRAW COMMAND
        /// <summary>
        /// Tells GWS to register an area of a container with the system so the child controls can be drawn correctly.
        /// </summary>
        RegisterContainer = Numbers.Flag40,
        #endregion

        #region COMBINATION OF COMMANDS
        /// <summary>
        /// Tells GWS that the current draw on buffer must show all area of the shape.
        /// Adjustments must be made to draw shape origin based and to avoid cutting 
        /// view due to scaling or rotation and if needed, buffer should be resized to
        /// accommodate all of shape drawing area.
        /// </summary>
        FullView = OriginBased | SizeToFit,

        /// <summary>
        /// Tells GWS to calculate and record boundary and not the pixels of shape being rendered. 
        /// Usage: Session's Command parameter to be used in FillAction, DrawAction.
        /// Screen remains unaffected as drawing does not happen.
        /// </summary>
        Calculate = SkipDisplayUpdate | RestoreView | UpdateView,

        /// <summary>
        /// Tells GWS to calculate and record boundary and not the pixels of shape being rendered. 
        /// Usage: Session's Command parameter to be used in FillAction, DrawAction.
        /// Screen remains unaffected as drawing does not happen.
        /// </summary>
        Calculate2 = Transparent | Calculate,

        /// <summary>
        /// Tells GWS to write data directly on screen (which of course temporary in nature)
        /// bypassing the internal storage buffer completely.
        /// Usage: Writable.WritePixel, Writable.WriteLine, ICopyable.CopyTo, ICopyScreen.CopyScreen, IClearable.Clear.
        /// </summary>
        Screen = NoClip | UpdateScreenOnly,

        /// <summary>
        /// Tells GWS to write data directly on screen and also on deep screen layer as well.
        /// Which result in preservation of data for the time being until ReconstructView command is called.
        /// Usage: FillAction, DrawAction and PointAction delegates.
        /// </summary>
        Persistent = Screen | RestoreView,

        /// <summary>
        /// Tells GWS that control being rendered now is the one where mouse pointer just entered.
        /// </summary>
        HoverOn = Numbers.Flag41,// Hover | ON,

        /// <summary>
        /// 
        /// </summary>
        HoverBorder = HoverOn | InvertedBorder | Skip
        #endregion
    }
    #endregion

    #region UPDATE COMMAND
    [Flags]
    public enum UpdateCommand : byte
    {
        /// <summary>
        /// No specific command.
        /// </summary>
        Default = Numbers.Flag0,

        /// <summary>
        /// Tells GWS to skip something while copying i.e. FrontBuffer or BackBuffer or Screen etc.. 
        /// </summary>
        Skip = Numbers.Flag1,

        /// <summary>
        /// Tells GWS to ignore internal clip set on graphics object while rendering. Pro version only.
        /// Usage: FillAction, DrawAction and PointAction delegates, IGraphics.DrawImage .
        /// </summary>
        NoClip = Numbers.Flag2,

        /// <summary>
        /// 
        /// </summary>
        DataCopy = Numbers.Flag3,

        /// <summary>
        /// Tells GWS to skip update process entirely. 
        /// Usage: IUpdatable.Update - When update is managed externally.
        /// </summary>
        SkipDisplayUpdate = Numbers.Flag4,

        /// <summary>
        /// Tells GWS to wipe all temporary drawings while refreshing screen.
        /// If used stand alone, it will remove all screen ZOrder flags.
        /// Usage: IUpdatable.Update - Pro version only.
        /// </summary>
        RestoreView = Numbers.Flag5,

        /// <summary>
        /// Tells GWS to reconstruct view from scratch by consolidating all layers excluding hidden pixels. 
        /// Pro version only.
        /// Caution: Expansive operation use only when needed.
        /// Usage: IUpdatable.Update
        /// </summary>
        UpdateView = Numbers.Flag6,

        /// <summary>
        /// Tells GWS to update screen without copying data without consolidating view layers.  
        /// Usage: IUpdatable.Update.
        /// </summary>
        UpdateScreenOnly = Numbers.Flag7,

        /// <summary>
        /// 
        /// </summary>
        CopyScreen = DataCopy | Skip,

        /// <summary>
        /// Tells GWS to fill screen pixels which are empty with background pen. 
        /// Usage: IUpdatable.Update. 
        /// Pro version only.
        /// </summary>
        RestoreScreen = RestoreView | Skip,
    }
    #endregion

    #region COPY COMMAND
    public enum CopyCommand : byte
    {
        /// <summary>
        /// No specific command.
        /// </summary>
        Default = Numbers.Flag0,

        /// <summary>
        /// Tells GWS to overwrite destination pixel only if it is transparent.
        /// Usage: FillAction, DrawAction delegates, IImageReceiver.DrawImage.
        /// </summary>
        Backdrop = Numbers.Flag1,

        /// <summary>
        /// Tells GWS that currently an image data is being saved after copying to external file.
        /// </summary>
        CopyRGBOnly = Numbers.Flag2,

        /// <summary>
        /// Tells GWS while copying data, swap Red channel with Blue that of source colour before copying to target.
        /// </summary>
        SwapRedBlueChannel = Numbers.Flag3,

        /// <summary>
        /// Tells GWS that currently an image data copy operation must be opaque i.e.
        /// To copy all data even if the source colour is empty.
        /// </summary>
        CopyOpaque = Numbers.Flag4,

        /// <summary>
        /// Tells GWS to copy only view which involves its child controls only.
        /// This is useful when you have transparent window which displays 
        /// desktop controls and/or desktop images as well. Then this option allows to ignore them.
        /// </summary>
        CopyContentOnly = Numbers.Flag5,

        /// <summary>
        /// Tells GWS that for the purpose of the current draw on buffer,
        /// the buffer must be resized to accommodate all of shape drawing area.
        /// Or to shrink or expand an image to fit the area specified for copy.
        /// </summary>
        SizeToFit = Numbers.Flag6,
    }
    #endregion

    #region CLEAR COMMAND
    [Flags]
    public enum ClearCommand : byte
    {
        /// <summary>
        /// No specific command.
        /// </summary>
        Default = Numbers.Flag0,

        /// <summary>
        /// Tells GWS to target front (control) buffer for rendering.
        /// Usage: ICopyable.CopyTo,IUploadable.CopyScreen, Blocks.Copy.
        /// </summary>
        SkipDesktop = Numbers.Flag1,

        /// <summary>
        /// Tells GWS to write data directly on screen (which of course temporary in nature)
        /// bypassing the internal storage buffer completely.
        /// Usage: Writable.WritePixel, Writable.WriteLine, ICopyable.CopyTo, ICopyScreen.CopyScreen, IClearable.Clear.
        /// </summary>
        Screen = Numbers.Flag3,

        /// <summary>
        /// Tells GWS to skips update process entirely. 
        /// Usage: IUpdatable.Update - When update is managed externally.
        /// </summary>
        NoScreenUpdate = Numbers.Flag4,
    }
    #endregion
#endif

    #region FILL COMMAND
    [Flags]
    public enum FillCommand : byte
    {
        /// <summary>
        /// No specific command.
        /// </summary>
        Default = Numbers.Flag0,

        /// <summary>
        /// Fill the shape as it physically appears with its original area.
        /// </summary>
        OriginalFill = Numbers.Flag1,

        /// <summary>
        /// Positions stroke so that the theoretical border of the shape is on the outside. 
        /// Shrinks the space inside the shape and maintains the size of the enclosing rectangle.
        /// </summary>
        StrokeInner = Numbers.Flag2,

        /// <summary>
        /// Positions stroke on the outside of the theoretical border of the shape. 
        /// Expands the rectangle containing the shape but does not change the internal area of the shape.
        /// </summary>
        StrokeOuter = Numbers.Flag3,

        /// <summary>
        /// If stroke is applied, it draws the pixel boundary between outer and inner child shape.
        /// </summary>
        DrawOutLines = Numbers.Flag4,

        /// <summary>
        /// If stroke is applied, it draws the pixel boundary of outer shape 
        /// and fills inner child shape, giving a fill of hollow shape.
        /// </summary>
        FillOddLines = Numbers.Flag5,

        /// <summary>
        /// Tells GWS to use flood-fill rule to fill polygon area.
        /// </summary>
        FloodFill = Numbers.Flag6,

        /// <summary>
        /// Tells GWS to skip filling axial lines which define a body of the shape.
        /// </summary>
        SkipFill = Numbers.Flag7,

        /// <summary>
        /// Tells GWS to skip drawing lines which define an edge of the shape.
        /// </summary>
        SkipDraw = Numbers.Flag8,

        /// <summary>
        /// Tells GWs to draw outlines and skip filling.
        /// </summary>
        DrawOutLinesOnly = DrawOutLines | SkipFill,

        /// <summary>
        /// 
        /// </summary>
        XORFill = SkipFill | SkipDraw,
    }
    #endregion

    #region LINE COMMAND
    [Flags]
    public enum LineCommand : byte
    {
        /// <summary>
        /// Tells GWS that the line is invalid.
        /// </summary>
        InValidLine = Numbers.Flag0,

        /// <summary>
        /// Tells GWS that the line is valid.
        /// </summary>
        ValidLine = Numbers.Flag1,

        /// <summary>
        /// Tells GWS that the line should be drawn without using anti-aliasing.
        /// </summary>
        Bresenham = Numbers.Flag2,

        /// <summary>
        /// A gap of one pixel will be left after each drawn pixel.
        /// </summary>
        DottedLine = Numbers.Flag3,

        /// <summary>
        /// A gap of two pixel will be left after each pair of two drawn pixel.
        /// </summary>
        DashedLine = Numbers.Flag4,

        /// <summary>
        /// A gap of one pixel will be left after the first two drawn pixel.
        /// And then next two pixels will be drawn normally.
        /// This pattern gets repeated subsequently.
        /// </summary>
        DashDotDashLine = Numbers.Flag5,

        /// <summary>
        /// Tells GWS to smoothen edges (joint location between lines) in continuous line drawing.
        /// </summary>
        LineCap = Numbers.Flag6,

        /// <summary>
        /// Tells GWS to draw end pixel after lone pixel loop finished.
        /// </summary>
        DrawEndPixel = Numbers.Flag7,

        /// <summary>
        /// Tells GWS that one point of line is at the origin (0, 0)
        /// </summary>
        OriginBasedLine = Numbers.Flag8,
    }
    #endregion

    #region BRUSH COMMAND
    public enum BrushCommand : byte
    {
        None = Numbers.Flag0,
        /// <summary>
        /// Tells GWS to prevent auto-sizing of brush according to the size of shape being rendered.
        /// Usage: ISession.SetPen by changing command of settings object.
        /// </summary>
        BrushNoSizeToFit = Numbers.Flag1,

        /// <summary>
        /// Tells GWS to prevent auto-location match of brush according to the location of shape being rendered.
        /// Usage: ISession.SetPen by changing command of settings object.
        /// </summary>
        BrushNoAutoPosition = Numbers.Flag2,

        /// <summary>
        /// Tells GWS to take exact position of writing co-ordinates and read pixel from the brush at exact same position.
        /// Usage: ISettings.SetPen by changing command of settings object.
        /// </summary>
        BrushFollowCanvas = Numbers.Flag3,

        /// <summary>
        /// Tells GWS to invert the rotation angle of the brush in opposite direction then that of shape rotation.
        /// Usage: ISession.SetPen by changing command of settings object.
        /// </summary>
        BrushInvertRotation = Numbers.Flag4,
    }
    #endregion

    #region SLOPE TYPE
    [Flags]
    public enum SlopeType : byte
    {
        None = Numbers.Flag0,
        NonSteep = Numbers.Flag1,
        Steep = Numbers.Flag2,
        Both = NonSteep | Steep,
    }
    #endregion

    #region GWS OBJ KIND 
    public enum GwsObjKind : byte
    {
        Unspecified,
        Text,
        Minimize,
        Maximize,
        Restore,
        Close,
        Dropdown,
        EndButton,
        ScrollButton,

        AbsBoundary,
        Boundary,
        RotatedBoundary,
        ItemBoundary,
        RotatedItemBoundary,
        TypeRectangle,
        AxisLine,
        AxisLineF,
        OddEvenLine,
        LineScanner,
        PolygonFiller,
        ImageStyle,
        Rotation,
        Degree,
        Skew,
        Scale,
        BrushStyle,

        Serializer,
        ImageProcessor,

        Font,
        Glyph,
        GlyphLine,
        GlyphRenderer,

        VirtualWindow,
        DesktopWindow,
        Image,
        Graphics,
        Canvas,
        View,
        Brush,
        GradientBrush,
        TextureBrush,
        Rgba,
        RenderTarget,
        Texture,

        ShapeCollection,
        ImageCollection,
        WindowCollection,
        AnimationCollection,
        PageCollection,
        ObjectCollection,
        ElementCollection,
        WidgetCollection,
        ViewCollection,
        PenCollection,

        Animation,
        Blinker,
        Circular,
        Sprite,
        SpriteSheet,
        SpriteAnimation,
        DirectShow,
        ToolTip,
        Skewer,
        ObjectMover,
        Caret,

        Element,
        Widget,
        Host,
        Control,
        SimpleLabel,
        SimpleButton,
        SimplePopup,

        WindowBody,

        PageBar,
        DataBar,
        ScrollBar,
        UpDown,
        ToolBar,
        TitleBar,
        ProgessBar,
        Frame,
        CustomBar,

        Page,
        SubPage,

        PageItem,
        ToolBarItem,

        Hover,
        MessageBox,
        Sound,
        Schedule,
        CountDown,
        Timer,

        Behavior,
        Group,
        Series,
        Limit,
        Span,
        Range,

        CheckList,
        ItemList,
        Filter,

        Genre,
        GenreCollection,
        Converter,
        Evalutor,

        LineSegment,

        Conic,
        Box,
        BoxF,
        Square,
        Rectangle,
        Circle,
        Ellipse,
        Arc,
        Pie,
        Curve,
        Line,
        Triangle,
        Polygon,
        RoundBox,
        Rhombus,
        Trapezium,
        Bezier,
        Capsule,
        Cylinder,
    }
    #endregion

    #region BOUNDARY KIND
    public enum BoundaryKind : byte
    {
        Boundary,
        RotatedBoundary,
        AbsBoundary,
        ItemBoundary,
        RotatedItemBoundary,
    }
    #endregion

    #region BENCHMARK UNIT
    public enum TimeUnit : byte
    {
        MilliSecond,
        Second,
        MicroSecond,
        Minute,
        Hour,
        Day,
        Tick,
    }
    #endregion

    #region LINE DRAW
    [Flags]
    public enum LineFill : byte
    {
        /// <summary>
        /// Horizontal line.
        /// Usage: FillAction or PointAction delegate.
        /// Assignment: Axis Line/ Axis Point/ Scan Line 's draw property.
        /// </summary>
        Horizontal = Numbers.Flag0,

        /// <summary>
        /// Vertical line.
        /// Usage: FillAction or PointAction delegate.
        /// Assignment: Axis Line/ Axis Point/ Scan Line 's draw property.
        /// </summary>        
        Vertical = Numbers.Flag1,

        /// <summary>
        /// Tells GWS it it has to draw ends points of a scan line and skip the portion in between from drawing.
        /// Usage: FillAction or PointAction delegate.
        /// Assignment: Axis Line/ Axis Point/ Scan Line 's draw property.
        /// </summary>
        EndsOnly = Numbers.Flag2,

        /// <summary>
        /// Draws middle part of line along with end pixels with anti-aliasing.
        /// Usage: FillAction or PointAction delegate.
        /// Assignment: Axis Line/ Axis Point/ Scan Line 's draw property.
        /// </summary>
        WithEnds = Numbers.Flag3,

        /// <summary>
        /// 
        /// </summary>
        PolyLine = Numbers.Flag4,
    }
    #endregion

    #region SKEW TYPE
    [Flags]
    public enum SkewType : byte
    {
        None = Numbers.Flag0,
        Horizontal = Numbers.Flag1,
        Vertical = Numbers.Flag2,
        Diagonal = Numbers.Flag3,
        Downsize = Horizontal | Vertical,
    }
    #endregion

    #region POINT TYPE
    [Flags]
    public enum PointKind : byte
    {
        /// <summary>
        /// Linear point - part of a line.
        /// </summary>
        Normal = Numbers.Flag0,

        /// <summary>
        /// Control point - part of a bezier definition.
        /// </summary>
        Control = Numbers.Flag1,

        /// <summary>
        /// Point with an edge on left.
        /// </summary>
        Bresenham = Numbers.Flag2,

        /// <summary>
        /// Point with an edge.
        /// </summary>
        Edge = Numbers.Flag3,

        /// <summary>
        /// Empty point suggesting a break in existing contour.
        /// </summary>
        Break = Numbers.Flag4,

        /// <summary>
        /// 
        /// </summary>
        Segment = Break | Numbers.Flag5,
    }
    #endregion

    #region POINT JOIN
    [Flags]
    public enum PointJoin : byte
    {
        /// <summary>
        /// Default option.
        /// </summary>
        None = Numbers.Flag0,

        /// <summary>
        /// Ensures each point is connected to its neighbours.
        /// </summary>
        ConnectEach = Numbers.Flag1,

        /// <summary>
        /// Used in Bezier to prevent errors in stroke drawing.
        /// </summary>
        RemoveLast = Numbers.Flag2,

        /// <summary>
        /// Ignores repeated points and join the remainder.
        /// </summary>
        NoRepeat = Numbers.Flag3,

        /// <summary>
        /// Connects the end point to the start point.
        /// </summary>
        ConnectEnds = Numbers.Flag4,

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

    #region FLIP
    [Flags]
    public enum FlipMode : byte
    {
        None = 0x00000000,
        Horizontal = 0x00000001,
        Vertical = 0x00000002
    }
    #endregion

    #region IMAGE FORMAT
    [Flags]
    public enum ImageFormat : byte
    {
        PNG,
        BMP,
        JPG,
        HDR,
        TGA,
        GWS,
    }
    #endregion

    #region GREY SCALE
    [Flags]
    public enum GreyScale : byte
    {
        /// <summary>
        /// NO GREY_SCALE MEANS COLOR IMAGE
        /// </summary>
        NONE = 0,

        /// <summary>
        /// CREATES JPEG COMPITIBLE GREY IMAGE WITH RATIO R = 0.299F, G = 0.587F, B = 0.114F
        /// </summary>
        JPEG,

        /// <summary>
        ///  CREATES CIE COMPITIBLE GREY IMAGE WITH RATIO R = 0.2126F, G = 0.7152F, B = 0.0722F
        /// </summary>
        CIE,

        /// <summary>
        ///  CREATES SEPIA GREY IMAGE WITH RATIO R = 0.2126F, G = 0.7152F, B = 0.0722F
        /// THEN B *= 0.82F AND G *= 0.95F
        /// </summary>
        SEPIA,

        /// <summary>
        /// CREATES EQUAL RATIO GREY IMAGE WITH RATIO R = 0.333F, G = 0.333F, B = 0.333F
        /// </summary>
        AVG,

        /// <summary>
        /// CREATES BLACK AND WHITE IMAGE WITH RATIO R = 0.2126F, G = 0.7152F, B = 0.0722F
        /// AND THE CONDITION >127 : WHITE ELSE 0
        /// </summary>
        BLACK_WHITE,

        /// <summary>
        ///CREATES BLANK AND WHITE STRUCTURE DEFINITION IMAGE USING SEBEL EDGE DETECTION.
        /// </summary>
        SEBEL_EDGES,
    }
    #endregion

    #region INTERPOLATION
    public enum Interpolation : byte
    {
        None,

        /// <summary>
        /// Tells GWS to interpolate using Bi-linear interpolation algorithm.
        /// </summary>
        Bilinear,

        /// <summary>
        /// Tells GWS to interpolate using Bi-cubic interpolation algorithm.
        /// </summary>
        Bicubic,

        /// <summary>
        /// Tells GWS to interpolate using Box interpolation algorithm.
        /// </summary>
        Box,

        /// <summary>
        /// Tells GWS to interpolate using Catmull-Rom interpolation algorithm.
        /// </summary>
        Catmull_Rom,

        /// <summary>
        /// Tells GWS to interpolate using Hermite interpolation algorithm.
        /// </summary>
        Hermite,

        /// <summary>
        /// Tells GWS to interpolate using Lancoz3 interpolation algorithm.
        /// Uses the result of a sine cardinal function to get the interpolation coefficient.
        /// https://en.wikipedia.org/wiki/Sinc_function
        /// result = Sinc(x) * Sinc(x/3f);
        /// </summary>
        Lancoz3,

        /// <summary>
        /// Tells GWS to interpolate using Welch interpolation algorithm.
        /// Uses the result of a sine cardinal function to get the interpolation coefficient.
        /// https://en.wikipedia.org/wiki/Sinc_function
        /// result = Sinc(x) * 1.0f - (x * x * 0.111111112f);
        /// </summary>
        Welch,

        /// <summary>
        /// Tells GWS to interpolate using Mitchell-Netravali interpolation algorithm.
        /// https://en.wikipedia.org/wiki/Mitchell%E2%80%93Netravali_filters
        /// </summary>
        MitchellNetravali,

        /// <summary>
        /// Tells GWS to interpolate using Robidoux interpolation algorithm.
        /// Returns the result of a B-C filter against the given value.
        /// <see href="http://www.imagemagick.org/Usage/filter/#cubic_bc"/>
        /// B = 0.3782f, C = 0.3109f,
        /// </summary>
        Robidoux,

        /// <summary>
        /// Tells GWS to interpolate using Robidoux interpolation algorithm for sharp edges.
        /// Returns the result of a B-C filter against the given value.
        /// <see href="http://www.imagemagick.org/Usage/filter/#cubic_bc"/>
        /// B = 0.2620f, C = 0.3690f,
        /// </summary>
        RobidouxSharp,

        /// <summary>
        /// Tells GWS to interpolate using Robidoux interpolation algorithm for soft edges.
        /// Returns the result of a B-C filter against the given value.
        /// <see href="http://www.imagemagick.org/Usage/filter/#cubic_bc"/>
        /// B = 0.6796f, C = 0.1602f
        /// </summary>
        RobidouxSoft,

        /// <summary>
        /// Tells GWS to interpolate using Spline interpolation algorithm.
        /// </summary>
        Spline,

        /// <summary>
        /// Tells GWS to interpolate using Triangle interpolation algorithm.
        /// </summary>
        Triangle,
    }
    #endregion

    #region PEN USED
    public enum PenUsed : byte
    {
        Founded,
        InvertedFound,
    }
    #endregion

    #region RESIZE MODE
    [Flags]
    public enum ResizeCommand : byte
    {
        /// <summary>
        /// Tells GWS to do resizing freely.
        /// </summary>
        Free = Numbers.Flag0,

        /// <summary>
        /// Tellse GWS to make the object big enough to hold the data with intended width and height.
        /// This means that current width and height is equal or greate than intended width and height,
        /// resize will not be required. In case either width or height is not big enough only that 
        /// will be changed.
        /// </summary>
        SizeOnlyToFit = Numbers.Flag1,

        /// <summary>
        /// Tellse GWS the no matter what, the intended width and height of the object can not be 
        /// less than the width and height when object is initialized at very first time.
        /// </summary>
        NotLessThanOriginal = Numbers.Flag2,

        /// <summary>
        /// Tells GWS to resize content of the object being resized. 
        /// Only applicable when option does not include SizeOnlyToFit.
        /// </summary>
        AutoReSizeContent = Numbers.Flag3,

        /// <summary>
        /// Tells GWS to resize using Bi-linear interpolation algorithm.
        /// Only applicable when resizing image source.
        /// </summary>
        AutoReSizeContentBilinear = AutoReSizeContent | Numbers.Flag4,

        /// <summary>
        /// Tells GWS to resize using Bi-cubic interpolation algorithm.
        /// Only applicable when resizing image source.
        /// </summary>
        AutoReSizeContentBicubic = AutoReSizeContent | Numbers.Flag5,

        /// <summary>
        /// Tells GWS to modify and return iteself and not creating a copy and return that.
        /// New instance is only returned when resize operation is actually done and not 
        /// conditionally terminated influenced by other options.
        /// Only applicable when used in conjusction with IResizeable<T>
        /// Where T is some object for example canvas, shape etc.
        /// </summary>
        NewInstance = Numbers.Flag6,

        StructChangeSelf = Numbers.Flag7,
    }
    #endregion

    #region ITEMSPREAD
    public enum ItemSpread : byte
    {
        Free = 0x0,
        Horizontal,
        Vertical,
    }
    #endregion

    #region RENDER PROCESS RESULT
    public enum RenderProcessResult : byte
    {
        Sucess,
        MoveNextPixel,
        MoveNextFragment,
        MoveNextLine,
        ExitLoop,
    }
    #endregion

    #region AREA FOR
    public enum Purpose : byte
    {
        Copy,
        Erase,
        Clip,
        TextContainer,
        ImageContainer,
    }
    #endregion
}

#if !Advance
namespace MnM.GWS
{
#region OBJ TYPE
    public enum ObjType: byte
    {
        /// <summary>
        /// Tells GWS the objects being drawn on screen is control/shape.
        /// </summary>
        Shape = 0,

        /// <summary>
        /// Tells GWS to ignore type of control/shape/image for the purpose of storing.
        /// </summary>
        UnSpecified = 1,

        /// <summary>
        /// Tells GWS not to wipe exisiting screen data while restoring screen.
        /// </summary>
        Top = 248,
    }
#endregion
}
#endif

#if !(GWS || Window)
namespace MnM.GWS
{
#region VIEW STATE
        [Flags]
        public enum ViewState : ushort
        {
            /// <summary>
            /// Default state - nothing special.
            /// </summary>
            None = Numbers.Flag0,

            /// <summary>
            /// Tells GWS that redrawing of the object is required.
            /// </summary>
            RedrawRequired = Numbers.Flag1,

            /// <summary>
            /// Tells GWS that since view is disposed rendering process is non executable.
            /// </summary>
            Disposed = Numbers.Flag2,

            /// <summary>
            /// Tells GWS that the view is hidden hence no operations are allowed.
            /// </summary>
            Hidden = Numbers.Flag3,

            /// <summary>
            /// Tells GWS that since view is disabled it can no longer process inputs.
            /// </summary>
            Disabled = Numbers.Flag4,

            /// <summary>
            /// Tells GWS that rendering process is only to perform erasing 
            /// task of the object keeping in mind that object is moved.
            /// </summary>
            DroppingDrag = Numbers.Flag5,

            /// <summary>
            /// Indicates thet object is currently has focus.
            /// </summary>
            Focused = Numbers.Flag6,

            /// <summary>
            /// Tells GWS that the object is currently in moving state.
            /// </summary>
            Moving = Numbers.Flag7,

            /// <summary>
            /// Tells GWS that currently dragged object is now just being dropped at the target.
            /// </summary>
            DragDropped = Numbers.Flag8,
        }
#endregion

#region CANVAS-PIXEL
        [Flags]
        public enum CanvasPixel : byte
        {
            /// <summary>
            /// Indicates that pixel belongs background image of view's desktop.
            /// </summary>
            None = Numbers.Flag0,

            /// <summary>
            /// Indicates that pixel belongs to an object and is a part of background of the object 
            /// where the object is an element of collection of objects in view's desktop/ window.
            /// </summary>
            Clip = Numbers.Flag1,
        }
#endregion
}
#endif