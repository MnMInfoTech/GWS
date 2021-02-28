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
            IntPtr SurfaceHandle;

            SdlSurfaceInfo Surface;

            #endregion

            #region CONSTRUCTORS
            public SdlTarget(IRenderWindow window):
                base(window)
            {
                SurfaceHandle = NativeFactory.GetWindowSurface(window.Handle);
                width = window.Width;
                height = window.Height;
                length = width * height;
                Surface = SurfaceHandle.ToObj<SdlSurfaceInfo>();
            }
            #endregion

            #region PROPERTIES
            public override unsafe IntPtr Source => Surface.Pixels;
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
                SurfaceHandle = NativeFactory.GetWindowSurface(Window.Handle);
                Surface = SurfaceHandle.ToObj<SdlSurfaceInfo>();
                width = Surface.Width;
                height = Surface.Height;
                length = width * height;
            }
            #endregion
        }
#if HideSdlObjects
    }
#endif
}
#endif
