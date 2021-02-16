/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    #region ICONTEXT
    /// <summary>
    /// This is a marker interface which represents an object which can be converted to GWS Entity.
    /// </summary>
    public interface IContext
    { }
    #endregion

    #region IRENDER TARGET
    /// <summary>
    /// Represents an object which has a capability to receive data from copyable source object.
    /// </summary>
    public partial interface IRenderTarget : IPixels, IResizable, IUpdatable, IDisposable2, IPaintable
    {
#if Advanced
        /// <summary>
        /// Int32 array pointer to be used by Canvas object for animation.
        /// i.e. anything which is drawn with Command.Animation.
        /// </summary>
        IntPtr Flags { get; }
#endif
    }
    #endregion

    #region INATIVE TARGET
    public interface INativeTarget : IRenderTarget, IHandle, IDisposable,
        IShowable, IHideable, IResizable, ITextDisplayer
    {
        INativeForm Form { set; }
    }
    #endregion

    #region IEXTERNAL- TARGET
    /// <summary>
    /// Represents an object which is a render target but belongs to external system such as Form.
    /// </summary>
    public interface IExternalTarget : IRenderTarget, IEventPusher, IHandle
    { }
    #endregion

#if (GWS || Window)
    #region INATIVE-FORM
    /// <summary>
    /// Represents an object which binds to native operating system specfic window such as Microsoft.Window.Forms.Form.
    /// SDL - Window should be the natural choice to represent window instead of this, 
    /// unlesss there are compelling resaons to use this object.
    /// </summary>
    public interface INativeForm : ICopyable, IResizable, IEventPusher, ITextDisplayer, IPaintable, IDisposable2
    { }
    #endregion

    #region IFORM
    /// <summary>
    /// Represents an object which represents window.
    /// </summary>
    public partial interface IForm : INativeForm, IGraphics, ICopyableScreen,
        IContainer, IUpdatable, IRefreshable, IClearable, IWritableBlock, IDisposable2,
        IShowable, IHideable, IBackground, IMinimalEvents, IScalable,
        IMinimalWindowEvents, IReadable
    {
    }
    #endregion

    #region IHOST
    public partial interface IHost : IForm, IFocusable, IRenderWindow
    {
        /// <summary> 
        /// Gets area of this object.
        /// </summary> 
        Rectangle Bounds { get; }
    }
    #endregion

    #region IRENDER-WINDOW
    /// <summary>
    /// Representsan object which represents window and offers minimum but sufficient gateway into GWS world. 
    /// </summary>
    public partial interface IRenderWindow : IHandle, ICopyable, IDisposable2, IPaintable
    {
        RendererFlags RendererFlags { get; }
    }
    #endregion

    #region IELEMENT
    /// <summary>
    /// Represents an object which has a place in GWS object eco system.
    /// This is an entry point interface to be in the GWS object eco system.
    /// A minimum required interface to inherit in order to make your control work in the GWS.
    /// It must have an ID, a name Name and area to work upon.
    /// </summary>
    public partial interface IElement : IRenderable, IPoint, ISize, IMinSizable, IBounds
    {
    }
    #endregion

    #region ICHILD
    public interface IChild 
    {
        /// <summary>
        /// Gets or sets Parent window this object belongs to.
        /// </summary>
        IGraphics Graphics { get; set; }
    }
    #endregion

    #region IFOREGROUND
    public interface IForeground
    {
        /// <summary>
        /// Sets foreground for this object.
        /// </summary>
        IPenContext Foreground { get; set; }
    }
    #endregion

    #region IHOVER-BACKGROUND
    public interface IHoverBackground
    {
        /// <summary>
        /// Sets background for this object.
        /// </summary>
        IPenContext HoverBackground { get; set; }
    }
    #endregion

    #region IHOVR-FOREGROUND
    public interface IHoverForeground
    {
        /// <summary>
        /// Sets hovering foreground for this object.
        /// </summary>
        IPenContext HoverForeground { get; set; }
    }
    #endregion

    #region IPOPUP-ITEM
    public interface IPopupItem : IVisible
    {
        string Text { get; }
    }
    #endregion
#endif

#if Window
    #region IWINDOW
    /// <summary>
    /// Addional properties and methods of Windows that buffers do not have.
    /// </summary>
    public partial interface IWindow : IHost, IWindowable, IWindowID, IWindowEvents, IEventProcessor, IBackground
    {
        /// <summary>
        /// Gets the flag this window is created with.
        /// </summary>
        GwsWindowFlags GwsWindowFlags { get; }

        /// <summary>
        /// Return an OpenGL context attached with this window.
        /// Only avalable if GWSWindowFlags contains OpenGL flag.
        /// </summary>
        IGLContext GLContext { get; }

        /// <summary>
        /// Pixel format as used by the windowing system e.g. SDL.
        /// </summary>
        uint PixelFormat { get; }

        /// <summary>
        /// Gets or sets the screen properties.
        /// </summary>
        IScreen Screen { get; set; }

        /// <summary>
        /// Gets or sets the transparency of the Window.
        /// </summary>
        float Transparency { get; set; }

        /// <summary>
        /// Gets the current window state enum: Normal, Minimised...
        /// </summary>
        WindowState WindowState { get; }

        /// <summary>
        /// Gets the enum documenting the display properties of the border:Fixed, Resizable...
        /// </summary>
        WindowBorder WindowBorder { get; }

        /// <summary>
        /// Gets or sets points on the window and their properties.
        /// </summary>
        VectorF Scale { get; set; }

        ///// <summary>
        ///// Returns True if Window has been initialised
        ///// </summary>
        //new bool Valid { get; }

        /// <summary>
        /// Gets or sets the visibility state of the cursor.
        /// </summary>
        bool CursorVisible { get; set; }

        /// <summary>
        /// Gets the ISound object for audio playback.
        /// </summary>
        ISound Sound { get; }

        /// <summary>
        /// Close the window and manage memory.
        /// </summary>
        void Close();

        /// <summary>
        /// Sets the Cursor to given (x,y) co-ordinates. 
        /// </summary>
        /// <param name="x">x ordinate of cursor position.</param>
        /// <param name="y">y ordinate of cursor position.</param>
        void SetCursor(int x, int y);

        /// <summary>
        /// Set the cursor to visible.
        /// </summary>
        void ShowCursor();

        /// <summary>
        /// Sets the cursor to hidden.
        /// </summary>
        void HideCursor();

        /// <summary>
        /// Move Window to the screen with givn number.
        /// </summary>
        /// <param name="screenIndex">Number of the Screen to move to.</param>
        void ChangeScreen(int screenIndex);

        /// <summary>
        /// Sets mouse pointer within the Window area.
        /// </summary>
        /// <param name="flag">!!!!</param>
        void ContainMouse(bool flag);

        /// <summary>
        /// Changes window state: Normal, Maximised etc
        /// </summary>
        /// <param name="state">Windows star Enum describing state.</param>
        void ChangeState(WindowState state);

        /// <summary>
        /// Change the border stting ENum so the the border can be drawn 
        /// </summary>
        /// <param name="border">Enum for new border state.</param>
        void ChangeBorder(WindowBorder border);

        new event EventHandler<IDrawEventArgs> Paint;
    }
    public interface IWindowID
    {
        /// <summary>
        /// Returns the Window Handle id given by the operating system.
        /// </summary>
        int WindowID { get; }
    }
    #endregion

    #region ISCREEN/S
    /// <summary>
    /// Object used to get the parameters of the screen.
    /// </summary>
    public interface IScreen : ISize, IEnumerable<IResolution>, IDisposable
    {
        /// <summary>
        /// Returns the resolution of each display
        /// </summary>
        /// <param name="index">Display number</param>
        /// <returns></returns>
        IResolution this[int index] { get; }

        /// <summary>
        /// Returns the number of Displays.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns the visible bounds of the Display.
        /// </summary>
        Rectangle Bounds { get; }
        /// <summary>
        /// Returns true if each display is a Mirror of the Primary and this Display is the Primary.
        /// </summary>
        bool IsPrimary { get; }
        /// <summary>
        /// Returns the Resolution object for display.
        /// </summary>
        IResolution Resolution { get; }

        /// <summary>
        /// Change the Resolution of the Display.
        /// </summary>
        /// <param name="resolutionIndex"></param>
        void ChangeResolution(int resolutionIndex);

        /// <summary>
        /// Restore resolution to the previous values before it was changed.
        /// </summary>
        void RestoreResolution();
    }

    public interface IScreens : IEnumerable<IScreen>
    {
        /// <summary>
        /// Returns an screen information for the indexed display.
        /// </summary>
        /// <param name="index">Display number.</param>
        /// <returns></returns>
        IScreen this[int index] { get; }
        /// <summary>
        /// Returns the number of Displays available.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns Primary Display number. All displays mirror the Primary.
        /// </summary>
        IScreen Primary { get; }
        /// <summary>
        /// Returns IScreen for the Display containing Point (x,y).
        /// Each Display is different in this case.
        /// </summary>
        /// <param name="x">x ordinate of point of interest.</param>
        /// <param name="y">y ordinate of point of interest.</param>
        /// <returns>Display information of the Display containing (X,Y).</returns>
        IScreen FromPoint(int x, int y);
    }
    #endregion

    #region IRESOLUTION
    /// <summary>
    /// Object that contains the specifications of one Display.
    /// </summary>
    public interface IResolution : ISize
    {
        /// <summary>
        /// True if Screen present.
        /// </summary>
        bool Valid { get; }
        /// <summary>
        /// Defines the visible rectangle that forms the Screen.
        /// </summary>
        Rectangle Bounds { get; }

        /// <summary>
        /// X offset of visible screen.
        /// </summary>

        int X { get; }
        /// <summary>
        /// Y offset of visible screen
        /// </summary>

        int Y { get; }
        /// <summary>
        /// Gets the Colour resolution of the Screen.
        /// </summary>

        int BitsPerPixel { get; }
        /// <summary>
        /// Gets the refresh rate of the Display (if relevant)
        /// </summary>
        float RefreshRate { get; }

        /// <summary>
        /// Returns the coulor format used by the display.
        /// </summary>
        uint Format { get; }
    }
    #endregion

    #region GLCONTEXT
    /// <summary>
    /// Represents an object which serves as an Open GL context.
    /// </summary>
    public interface IGLContext
    {
        int SwapInterval { get; set; }
        int this[int type] { get; set; }
        bool IsCurrent { get; }

        IntPtr GetFunction(string fxName);

        void BindTexture(IRenderWindow texture, float? width = null, float? height = null);
        void UnbindTexture();
        void MakeCurrent();
        void Swap();
    }
    #endregion
#endif
}
