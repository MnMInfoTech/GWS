/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if GWS || Window
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    #region IDRAWEVENTARGS
    /// <summary>
    /// Represents an argument object to expose underlying graphics object to draw on.
    /// </summary>
    public interface IDrawEventArgs : IEventArgs, IDisposable
    {
        /// <summary>
        /// Gets an underlying surface object to draw.
        /// </summary>
        IRenderer Renderer { get; }

        ISession Session { get; }
    }
    #endregion

    #region IEx-DRAWEVENTARGS
    internal interface IExDrawEventArgs : IDrawEventArgs
    {
        /// <summary>
        /// Gets or sets an underlying surface object to draw.
        /// </summary>
        new IExRenderer Renderer { get; set; }

        new IExSession Session { get; set; }

        void Reset();
    }
    #endregion

    #region GWS-DRAWEVENTARGS
#if DevSupport
    public
#else
    internal
#endif
    class DrawEventArgs : EventArgs, IExDrawEventArgs
    {
        #region VARIABLES
        internal IExRenderer renderer;
        IExSession session;
        #endregion

        #region CONSTEUCTORS
#if DevSupport
        public
#else
        protected internal
#endif
        DrawEventArgs()
        {
            session = new Session();
        }

        public DrawEventArgs(IRenderer graphics)
        {
            renderer = (IExRenderer)graphics;
            session = new Session();
        }
        public DrawEventArgs(IRenderer graphics, IEnumerable<IParameter> parameters)
        {
            renderer = (IExRenderer)graphics;
            parameters.Extract(out session);
        }
        #endregion

        #region PROPERTIES
        public ISession Session => session;
        public IRenderer Renderer => renderer;
        IExRenderer IExDrawEventArgs.Renderer { get => renderer; set => renderer = value; }
        IExSession IExDrawEventArgs.Session { get => session; set => session = value; }
        #endregion

        #region RESET
        void IExDrawEventArgs.Reset() =>
            Reset();
        protected virtual void Reset()
        {
            renderer = null;
            session.Clear();
        }
        #endregion

        #region DISPOSE
        void IDisposable.Dispose() => 
            Dispose();
        protected virtual void Dispose()
        {
            renderer = null;
            session = null;
        }
        #endregion
    }
    #endregion
}
#endif
