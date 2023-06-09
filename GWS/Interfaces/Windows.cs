/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    #region IVIRTUAL-WINDOW
    public partial interface IWindow : IParent 
    { }
    #endregion

    #region IEx-WINDOW
    internal partial interface IExWindow : IWindow, IExParent, IDisposable
    {
    }
    #endregion

    #region IRENDER-WINDOW
    /// <summary>
    /// Represents an object which represents window and offers minimum but sufficient gateway into GWS world. 
    /// </summary>
    public partial interface IRenderWindow : IID<string>, IUpdatable, IRefreshable, IVisible, IPoint, IHandle,
        ICopyable, IHitTestable, ICursorManager, IInputStateHolder, IMessageBoxHost, IMemoryOccupier, IFocus
    {
        /// <summary>
        /// Gets the flag this window is created with.
        /// </summary>
        GwsWindowFlags Flags { get; }
    }
    #endregion

    #region IEx-RENDER-WINDOW
    internal partial interface IExRenderWindow : IRenderWindow, IExResizable, IDisposable, IExViewState, IViewHolder<IExView>, IExEventPusher, IAdvancePaintable
    { }
    #endregion

    #region RENDER-WINDOW HOLDER
    public interface IRenderWindowHolder
    {
        IRenderWindow RenderWindow { get; }
    }
    #endregion

    #region ADVANCED RENDER-WINDOW HOLDER
    internal partial interface IExRenderWindowHolder : IRenderWindowHolder
    {
        new IExRenderWindow RenderWindow { get; }
    }
    #endregion

    #region IMESSAGE-BOX HOST
    /// <summary>
    /// Represents an object which lets users to create and use messagebox.
    /// </summary>
    public interface IMessageBoxHost
    {
        /// <summary>
        /// Allows users to create new messabox and returs user chosen result to process further.
        /// </summary>
        /// <param name="title">Title of the messagebox to be displayed./</param>
        /// <param name="x">X co-ordinate of the location of the messagebox.</param>
        /// <param name="y">Y co-ordinate of the location of the messagebox.</param>
        /// <param name="buttons">Buttons to be included in the messagebox.</param>
        /// <returns>Messabox result as per the choice made by the user.</returns>
        MsgBoxResult ShowMessageBox(string title, int x, int y, string text, MsgBoxButtons buttons = MsgBoxButtons.OkCancel);


        /// <summary>
        /// Allows users to create new inputbox and returs user chosen result to process further.
        /// </summary>
        /// <param name="title">Title of the messagebox to be displayed./</param>
        /// <param name="x">X co-ordinate of the location of the messagebox.</param>
        /// <param name="y">Y co-ordinate of the location of the messagebox.</param>
        /// <param name="buttons">Buttons to be included in the messagebox.</param>
        /// <returns>Messabox result as per the choice made by the user.</returns>
        Lot<MsgBoxResult, string> ShowInputBox(string title, int x, int y, string text, MsgBoxButtons buttons = MsgBoxButtons.OkCancel);
    }
    #endregion

    #region OS WINDOW
    public interface IOSMinimalWindow
    {
        /// <summary>
        /// Gets the type of underlying operating system.
        /// </summary>
        OS OS { get; }

        /// <summary>
        /// Gets or sets the transparency of the Window.
        /// </summary>
        float Transparency { get; set; }

        /// <summary>
        /// Gets or sets the screen properties.
        /// </summary>
        IScreen Screen { get; }

#if Window
        #region PROPERTIES
        /// <summary>
        /// Gets the latest error occured while interacting with by the operating system.
        /// </summary>
        string LastError { get; }

        /// <summary>
        /// Pixel format as used by the windowing system e.g. SDL.
        /// </summary>
        uint PixelFormat { get; }

        /// <summary>
        /// Return an OpenGL context attached with this window.
        /// Only avalable if GWSWindowFlags contains OpenGL flag.
        /// </summary>
        IGLContext GLContext { get; }

        /// <summary>
        /// Gets the ISound object for audio playback.
        /// </summary>
        ISound Sound { get; }
        #endregion

        #region CHANGE SCREEN
        /// <summary>
        /// Move Window to the screen with givn number.
        /// </summary>
        /// <param name="screenIndex">Number of the Screen to move to.</param>
        void ChangeScreen(int screenIndex);
        #endregion

        #region TEXTURE
        /// <summary>
        /// Crates a new texture from a given window.
        /// </summary>
        /// <param name="w">Width of the texture</param>
        /// <param name="h">Height of the texture</param>
        /// <param name="isPrimary">Defines if its a primary one for the window</param>
        /// <param name="textureAccess">Defines the way texture can be accessed. Default is streaming.</param>
        /// <returns></returns>
        ITexture newTexture(int? w = null, int? h = null,
            bool isPrimary = false, TextureAccess? textureAccess = null, RendererFlags? rendererFlags = null);
        #endregion

        #region WAV PLAYER
        /// <summary>
        /// Createa new Wav player.
        /// </summary>
        /// <returns></returns>
        ISound newWavPlayer();
        #endregion
#endif
    }

    public interface IOSWindow : IOSMinimalWindow, IHandle, IOverlap, IID<string>, ITextDisplayer, IInputStateHolder,
        IShowable, IHideable, IPoint, ISize, IMessageBoxHost, ILocationHolder, ICursorManager, IFocusable, IMemoryOccupier,
        IRenderTargetHolder
    {
        /// <summary>
        /// Indicates if this window is disposed or not.
        /// </summary>
        bool IsDisposed { get; }
    }

#if (DevSupport || DLLSupport)
    public
#else
    internal
#endif
    interface IExOSWindow : IOSWindow, IEventParser, IDisposable
#if Window
        , IEventPoller
#endif
    {
        new ModifierKeys ModifierKey { get; set; }
        new MouseButton MouseButton { get; set; }

        bool PsuedoConstructor(IRenderWindow window, out GwsWindowFlags resultFlags,
            string title = null,
            int? width = null, int? height = null,
            int? x = null, int? y = null,
            GwsWindowFlags? flags = null,
            RendererFlags? renderFlags = null);
    }
    #endregion

    #region ICURSOR-MANAGABLE WINDOW 
    public interface ICursorManager
    {
        #region SET CURSOR TYPE
        /// <summary>
        /// Chooses a system cursor using specified cursor type.
        /// </summary>
        /// <param name="cursor"></param>
        void SetCursorType(CursorType cursor);
        #endregion

        #region GET/ SET CURSOR POSITION
        /// <summary>
        /// Gets window's cusor's position as in x and y coordinates.
        /// </summary>
        /// <param name="x">X coordinate of the location where cursor should be placed</param>
        /// <param name="y">Y coordinate of the location where cursor should be placed</param>
        void GetCursorPos(out int x, out int y, bool global = false);

        /// <summary>
        /// Sets window's cusor's position to specified x and y coordinates.
        /// </summary>
        /// <param name="x">X coordinate of the location where cursor should be placed</param>
        /// <param name="y">Y coordinate of the location where cursor should be placed</param>
        void SetCursorPos(int x, int y, bool global = false);
        #endregion

        #region SHOW CURSOR
        ///// <summary>
        ///// Set the cursor to visible.
        ///// </summary>
        //void ShowCursor();
        #endregion

        #region HIDE CURSOR
        ///// <summary>
        ///// Sets the cursor to hidden.
        ///// </summary>
        //void HideCursor();
        #endregion
    }
    #endregion

    #region SCREEN
    /// <summary>
    /// Object used to get the parameters of the screen.
    /// </summary>
    public partial interface IScreen : IPoint, ISize, IDisposable, IHitTestable
    {
        /// <summary>
        /// Returns true if each display is a Mirror of the Primary and this Display is the Primary.
        /// </summary>
        bool IsPrimary { get; }

        /// <summary>
        /// X offset of visible screen.
        /// </summary>
        int BitsPerPixel { get; }
    }
    #endregion

    #region SCREENS
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
    }
    #endregion

#if Window
#if SDL
    #region ISCREEN
    /// <summary>
    /// Object used to get the parameters of the screen.
    /// </summary>
    partial interface IScreen : IEnumerable<IResolution>
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
    #endregion

    #region IRESOLUTION
    /// <summary>
    /// Object that contains the specifications of one Display.
    /// </summary>
    public interface IResolution : IPoint, ISize
    {
        /// <summary>
        /// True if Screen present.
        /// </summary>
        bool Valid { get; }

        /// <summary>
        /// X offset of visible screen.
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
#endif

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

    #region ITEXTURE
    public partial interface ITexture : ISize, IDisposable, IUpdatable, ICopyable, ICopy, ISizeHolder
    { }
    public interface ITexture2 : ITexture
    {
        FlipMode Flip { get; set; }
        BlendMode Mode { get; set; }
        byte Alpha { get; set; }
        int ColourMode { get; set; }
        void Bind();
        void Unbind();
    }
    #endregion
#endif
}
#endif
