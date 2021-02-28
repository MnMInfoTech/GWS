/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    public static partial class Command
    {
        /// <summary>
        /// Discard all changes made to the command and set it to previous value
        /// No draw command and also no animation.
        /// Overwrite pixel only if source pixel is non transparent.
        /// </summary>
        public const ulong None = 0x0;

        /// <summary>
        /// Normal operation - no check before overwriting destination pixel.
        /// Usage: Writable.WritePixel; Writable.WriteLine; Blocks.Copy.
        /// </summary>
        public const ulong Opaque = 0x1;

        /// <summary>
        /// Overwrite destination pixel only if it is transparent.
        /// Usage: Writable.WritePixel; Writable.WriteLine; Blocks.Copy.
        /// </summary>
        public const ulong Backdrop = 0x2;

        /// <summary>
        /// Tells GWS to invert color pixel before it is displayed.
        /// Usage: Writable.WritePixel; Writable.WriteLine; Blocks.Copy.
        /// </summary>
        public const ulong InvertColor = 0x4;

        /// <summary>
        /// Overwrite destination pixel only if it is non transparent.
        /// Usage: Writable.WritePixel; Writable.WriteLine; Blocks.Copy.
        /// </summary>
        public const ulong Masking = 0x8;

        /// <summary>
        /// Excludes Backgroud Pen pixels from rendering on screen.
        /// Usage: ICopyable.CopyTo; IConsolidator.Consolidate.
        /// </summary>
        public const ulong SkipBackground = 0x10;

        /// <summary>
        /// Exclusively uses breshenham algorithm with integer arithmetic and no anti-aliasing.  
        /// Usage: Writable.WritePixel; Renderer.ProcessLine.
        /// </summary>
        public const ulong Breshenham = 0x20;

        /// <summary>
        /// Sets renderer's Distinct property true to prevent any pixel in the line being redrawn.
        /// Usage: Writable.WritePixel; Renderer.ProcessLine.
        /// </summary>
        public const ulong Distinct = 0x40;

        /// <summary>
        /// A gap of one pixel will be left after each drawn pixel.
        /// Usage: Writable.WritePixel; Renderer.ProcessLine.
        /// </summary>
        public const ulong Dot = 0x80;

        /// <summary>
        /// A gap of two pixel will be left after each pair of two drawn pixel.
        /// Usage: Writable.WritePixel; Renderer.ProcessLine.
        /// </summary>
        public const ulong Dash = 0x100;

        /// <summary>
        /// A gap of one pixel will be left after the first two drawn pixel.
        /// And then next two pixels will be drawn normally.
        /// This pattern gets repeated susequently.
        /// Usage: Writable.WritePixel; Renderer.ProcessLine.
        /// </summary>
        public const ulong DashDotDash = 0x200;

        /// <summary>
        /// Keeps current fill rule active to fill out-lines.
        /// Usage: FillAction's Command parameter; Renderer.WriteLine's Command parameter; PolyFill.Command.
        /// </summary>
        public const ulong KeepFillRuleForStroking = 0x400;

        /// <summary>
        /// Ignores auto calculated FillPattern based on other settings such as fill mode; stroke mode etc.
        /// Usage: FillAction's Command parameter; Renderer.WriteLine's Command parameter; PolyFill.Command.
        /// </summary>
        public const ulong IgnoreAutoCalculatedFillPatten = 0x800;

        /// <summary>
        /// Tells GWS to use Odd-Even rule for polygon filling.
        /// Usage: FillAction's Command parameter; Renderer.WriteLine's Command parameter; PolyFill.Command.
        /// </summary>
        public const ulong OddEven = 0x1000;

        /// <summary>
        /// Polygon drawing using line sequence match rule.
        /// Usage: FillAction's Command parameter; Renderer.WriteLine's Command parameter; PolyFill.Command.
        /// </summary>
        public const ulong Outlininig = 0x2000;

        /// <summary>
        /// Tells GWS it it has to draw ends points of a scan line and skip the portion in between from drawing.
        /// Usage: FillAction's Command parameter; Renderer.WriteLine's Command parameter; PolyFill.Command.
        /// </summary>
        public const ulong DrawEndsOnly = 0x4000;

        /// <summary>
        /// Draws middle part only skip end pixels.
        /// Usage: FillAction's Command parameter; Renderer.WriteLine's Command parameter; PolyFill.Command.
        /// </summary>
        public const ulong DrawLineOnly = 0x8000;

        /// <summary>
        /// Tells GWS if scan line fragments is to be drawn without sorting it first. 
        /// Use this only if you have already sorted the scan line! 
        /// If that is not the case then it can give unexpected result.
        /// Usage: FillAction's Command parameter; Renderer.WriteLine's Command parameter; PolyFill.Command.
        /// </summary>
        public const ulong NoSorting = 0x10000;

        /// <summary>
        /// Skips draw if end points are close enough.
        /// Usage: FillAction's Command parameter; Renderer.WriteLine's Command parameter; PolyFill.Command.
        /// </summary>
        public const ulong CheckForCloseness = 0x20000;

        /// <summary>
        /// If axial line has onlt one endpoint and this flag exists; the point gets filled otherwise not.
        /// Usage: FillAction's Command parameter; Renderer.WriteLine's Command parameter; PolyFill.Command.
        /// </summary>
        public const ulong FillSinglePointLine = 0x40000;

        /// <summary>
        /// Standard Odd-Even rule polygon filling. floating start and end points will be rounded to next integer.
        /// Also line too short i.e start and end points are too close will be ignored.
        /// Usage: FillAction's Command parameter; Renderer.WriteLine's Command parameter; PolyFill.Command.
        /// </summary>
        public const ulong OddEvenPolyFill = OddEven | DrawLineOnly | CheckForCloseness;

        /// <summary>
        /// Standard Odd-Even rule polygon outlining. floating start and end points will be rounded to next integer.
        /// Also line too short i.e start and end points are too close will be ignored.
        /// Usage: FillAction's Command parameter; Renderer.WriteLine's Command parameter; PolyFill.Command.
        /// </summary>
        public const ulong OddEvenPolyDraw = OddEven | DrawEndsOnly | CheckForCloseness;

        /// <summary>
        /// Tells GWS to Suspend immediate update of screen instead to mark area for later update. Use this option carefully.
        /// Usage: IUpdatable.Update method.
        /// </summary>
        public const ulong InvalidateOnly = 0x80000;

        /// <summary>
        /// Prevents auto-sizing of brush according to the size of shape being rendered.
        /// Usage: Renderer.GetPen method - via Settings.Command property.
        /// </summary>
        public const ulong NoBrushAutoSizing = 0x100000;

        /// <summary>
        /// Prevents auto-location match of brush according to the location of shape being rendered.
        /// Usage: Renderer.GetPen method - via Settings.Command property.
        /// </summary>
        public const ulong NoBrushAutoPositioning = 0x200000;

        /// <summary>
        /// Tells GWS to take exact position of writing co-ordinates and read pixel from the brush at exact same position.
        /// Usage: Renderer.GetPen method - via Settings.Command property.
        /// </summary>
        public const ulong BrushFollowCanvas = 0x400000;

        /// <summary>
        /// Inverts the rotation angle of the brush in opposite direction then that of shape rotation.
        /// Usage: Renderer.GetPen method - via Settings.Command property.
        /// </summary>
        public const ulong InvertBrushRotation = 0x800000;

        /// <summary>
        /// Calculates the rendering area but does not draw shape.
        /// Usage: Writable.WritePixel; Writable.WriteLine.
        /// </summary>
        public const ulong CalculateOnly = 0x1000000;

        /// <summary>
        /// Uploads data directly on screen (which of course temporary in nature) bypassing the internal buffer completely.
        /// Usage: Writable.WritePixel; Writable.WriteLine; ICopyable.CopyTo; IConsolidator.Consolidate; IClearable.Clear.
        /// </summary>
        public const ulong WriteToScreen = 0x2000000;

        /// <summary>
        /// Tells GWS to apply animation.
        /// Usage: Writable.WritePixel; Writable.WriteLine; IUpdatable.Update.
        /// </summary>
        public const ulong WriteAnimation = 0x4000000;

        /// <summary>
        /// Erases specified shape from the memory block. Use this option while drawing. Advanced version only.
        /// Usage: Writable.WritePixel; Writable.WriteLine.
        /// </summary>
        public const ulong EraseObject = 0x8000000;

        /// <summary>
        /// Restores drawing after a specified shape is removed or erased from memory block. Advanced version only.
        /// Usage: Writable.WritePixel; Writable.WriteLine.
        /// </summary>
        public const ulong RestoreObject = 0x10000000;

        /// <summary>
        /// Tells GWS that shape is being drawn first time.
        /// Usage: Writable.Render; IObjCollection.Add.
        /// </summary>
        public const ulong AddObject = 0x20000000;

        /// <summary>
        /// Gets or sets a flag to determine that rendering of shape is done in second buffer or not. Pro version only.
        /// Usage: Writable.WritePixel; Writable.WriteLine; ICopyable.CopyTo; IConsolidator.Consolidate; IClearable.Clear.
        /// </summary>
        public const ulong SecondBuffer = 0x40000000;

        /// <summary>
        /// Pushes drawing of added controls to background and brings background to the front. Pro version only.
        /// Usage: ICopyable.CopyTo; IConsolidator.Consolidate.
        /// </summary>
        public const ulong SwapZOrder = 0x80000000;

        /// <summary>
        /// Excludes everything drawn on background buffer for rendering on screen. Pro version only.
        /// Usage: ICopyable.CopyTo; IConsolidator.Consolidate.
        /// </summary>
        public const ulong Skip2ndBuffer = 0x100000000;

        /// <summary>
        /// Excludes everything drawn on main buffer i.e mainly drawing of permanent controls for rendering on screen. Pro version only.
        /// Usage: ICopyable.CopyTo; IConsolidator.Consolidate.
        /// </summary>
        public const ulong Skip1stBuffer = 0x200000000;

        /// <summary>
        /// Copies background pen data only.
        /// Usage: ICopyable.CopyTo; IConsolidator.Consolidate.
        /// </summary>
        public const ulong CopyBackgroundOnly = Skip2ndBuffer | Skip1stBuffer;

        /// <summary>
        /// When resized; the image inside is also resizes to fit the size without losing quality. Pro version only.
        /// </summary>
        public const ulong NoQualityLoss = 0x400000000;

        /// <summary>
        /// Updates screen without copying data from underlying buffer.  
        /// Usage: IUpdatable.Update.
        /// </summary>
        public const ulong UpdateScreenOnly = 0x800000000;

        /// <summary>
        /// Copies data from underlying buffer but does not update screen.
        /// Usage: IUpdatable.Update.
        /// </summary>
        public const ulong UpdateBufferOnly = 0x1000000000;

        /// <summary>
        /// Wipes all temporary drawings while refreshing screen.
        /// Usage: IUpdatable.Update.
        /// </summary>
        public const ulong WipeScreen = 0x2000000000;

        /// <summary>
        /// Wipe latest animation drawing.
        /// Usage: IUpdatable.Update.
        /// </summary>
        public const ulong WipeAnimation = 0x4000000000;

        /// <summary>
        /// Copies directly from screen.
        /// Usage: ICopyable.CopyTo; IConsolidator.Cosolidate
        /// </summary>
        public const ulong CopyFromScreen = 0x8000000000;

        /// <summary>
        /// Clears screen directly ignoring underlying buffer.
        /// Usage: IClearable.Clear
        /// </summary>
        public const ulong ClearScreen = 0x10000000000;

        /// <summary>
        /// Skips update process entirely. 
        /// Usage: When update is externally managed only when rendering directly on screen.
        /// </summary>
        public const ulong SkipUpdate = 0x20000000000;
    }
    public static partial class ObjType
    {
        public const byte None = 0;
        public const byte Animation = 1;
        public const byte Blinker = 2;
        public const byte ToolTop = 3;
        public const byte Popup = 4;
    }
}
