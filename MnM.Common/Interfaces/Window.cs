/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    #region IRENDER TARGET
    /// <summary>
    /// Represents an object which has a capability to receive data from copyable source object.
    /// </summary>
    public partial interface IRenderTarget : IPixels, IResizable, IUpdatable, IDisposable2,
        IPaintable, IBackground, IClearable, IReadable, ICopyable
    {
        /// <summary>
        /// Background pen pixels.
        /// </summary>
        int[] BackgroundData { get; }
    }
    #endregion

    #region INATIVE TARGET
    public interface INativeTarget : IRenderTarget, IHandle, IDisposable,
        IShowable, IHideable, IResizable, ITextDisplayer
    {
        INativeForm Form { get; set; }
    }
    #endregion

    #region IEXTERNAL- TARGET
    /// <summary>
    /// Represents an object which is a render target but belongs to external system such as Form.
    /// </summary>
    public interface IExternalTarget : IRenderTarget, IEventPusher, IHandle
    { }
    #endregion

    #region INATIVE-FORM
    /// <summary>
    /// Represents an object which binds to native operating system specfic window such as Microsoft.Window.Forms.Form.
    /// SDL - Window should be the natural choice to represent window instead of this, 
    /// unlesss there are compelling resaons to use this object.
    /// </summary>
    public partial interface INativeForm : ICopyableScreen, IDisposable2, IPaintable, IResizable, IEventPusher, ITextDisplayer
    { }
    #endregion

    #region IRENDER-WINDOW
    /// <summary>
    /// Representsan object which represents window and offers minimum but sufficient gateway into GWS world. 
    /// </summary>
    public partial interface IRenderWindow : IHandle, IBlockable, IDisposable2, IPaintable
    {
        RendererFlags RendererFlags { get; }
    }
    #endregion
}