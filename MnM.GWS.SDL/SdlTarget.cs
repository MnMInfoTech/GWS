/* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if Window
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using MnM.GWS.SDL;

namespace MnM.GWS
{
#if HideSdlObjects
    partial class NativeFactory
    {
#else
    public
#endif
        sealed partial class SdlTarget : _RenderTarget, IRenderTarget
        {
            #region VARIABLES
            /// <summary>
            /// SDL surface object.
            /// </summary>
            IntPtr Surface;

            /// <summary>
            /// Pixels.
            /// </summary>
            volatile IntPtr source;
            #endregion

            #region CONSTRUCTORS
            public SdlTarget(IRenderWindow window):
                base(window)
            {
                Surface = NativeFactory.GetWindowSurface(window.Handle);
                width = window.Width;
                height = window.Height;
                length = width * height;
                source = Surface.ToObj<SdlSurfaceInfo>().Pixels;
            }
            #endregion

            #region PROPERTIES
            public override unsafe IntPtr Source => source;
            #endregion

            #region UPDATE
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override unsafe void Update(ulong command, IBoundable area)
            {
                if (area == null || !area.Valid)
                    return;
                NativeFactory.UpdateWindow(Window.Handle, new Rect(area));
            }
            #endregion

            #region RESIZE
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            protected override void ResizeSource(int w, int h)
            {
                Surface = NativeFactory.GetWindowSurface(Window.Handle);
                SdlSurfaceInfo sdlSurface = Surface.ToObj<SdlSurfaceInfo>();
                source = sdlSurface.Pixels;
                width = sdlSurface.Width;
                height = sdlSurface.Height;
                length = width * height;
            }
            #endregion
        }
#if HideSdlObjects
    }
#endif
}
#endif
