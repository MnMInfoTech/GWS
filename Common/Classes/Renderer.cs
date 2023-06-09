/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
#if DevSupport
    public
#else
    internal
#endif

    abstract partial class Renderer: IExRenderer
    {
#if DevSupport
        protected
#else
        internal
#endif
        volatile ViewState viewState;
#if DevSupport
        protected
#else
        internal
#endif
        IPoint UniversalDrawOffset;

        public abstract int Width { get; }
        public abstract int Height { get; }
        public virtual bool Valid => Width > 0 && Height > 0;
        ViewState IExViewState.ViewState => viewState;
        IPoint IExRenderer.UniversalDrawOffset 
        { 
            get => UniversalDrawOffset; 
            set => UniversalDrawOffset = value;
        }
        public abstract RenderAction CreateRenderAction(IEnumerable<IParameter> Parameters = null);

        #region SET STATE
        void IExUpdatable<ViewState, ModifyCommand>.Update(ViewState state, ModifyCommand command)
        {
            switch (command)
            {
                case ModifyCommand.Replace:
                default:
                    this.viewState = state;
                    break;
                case ModifyCommand.Add:
                    this.viewState |= state;
                    break;
                case ModifyCommand.Remove:
                    this.viewState &= ~state;
                    break;
            }
        }
        #endregion
    }
}
