/* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if Window
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using MnM.GWS.SDL;

namespace MnM.GWS
{
#if HideSdlObjects
    partial class NativeFactory
    {
#else
    public
#endif
        sealed class SdlTarget : IRenderTarget
        {
#region VARIABLES
            /// <summary>
            /// Width of this object.
            /// </summary>
            int width;

            /// <summary>
            /// Height of this object
            /// </summary>
            int height;

            /// <summary>
            /// Length of one dimensional memory block this object represents.
            /// </summary>
            int length;

            /// <summary>
            /// Render Window - attached to this target.
            /// </summary>
            readonly IRenderWindow Window;

            /// <summary>
            /// SDL surface object.
            /// </summary>
            IntPtr Surface;

            /// <summary>
            /// Pixels.
            /// </summary>
            volatile IntPtr source;
#if Advanced
            /// <summary>
            /// this array of byte will be used by Canvas object for direct screen.
            /// </summary>
            volatile byte[] flags;
#endif
#endregion

#region CONSTRUCTORS
            public SdlTarget(IRenderWindow window)
            {
                Window = window;
                this.Surface = GetHandle(Window.Handle);
                width = window.Width;
                height = window.Height;
                length = width * height;
                source = Surface.ToObj<SdlSurfaceInfo>().Pixels;
#if Advanced
                flags = new byte[length];
#endif
            }
#endregion

#region PROPERTIES
            public unsafe IntPtr Source => source;
            public int Width => width;
            public int Height => height;
            public int Length => length;
            public bool IsDisposed { get; private set; }
            unsafe int* Screen => (int*)Source;
#if Advanced
            public unsafe IntPtr Flags
            {
                get
                {
                    fixed (byte* b = flags)
                        return (IntPtr)b;
                }
            }
#endif
#endregion

#region GET HANDLE
            IntPtr GetHandle(IntPtr handle) =>
            NativeFactory.GetWindowSurface(handle);
#endregion

#region UPDATE
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe void Update<T>(ulong command, params T[] boundables) where T : IBoundable
            {
                if (boundables.Length == 0)
                    return;
                var items = boundables.Where(p => p.Valid).Select(p => new Rect(p)).ToArray();
                NativeFactory.UpdateWindow(Window.Handle, items, items.Length);
            }
#endregion

#region RESIZE
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe void Resize(int? newWidth = null, int? newHeight = null)
            {
                if ((newWidth == null && newHeight == null) ||
                    (newWidth == width && newHeight == height))
                    return;

                Surface = NativeFactory.GetWindowSurface(Window.Handle);
                SdlSurfaceInfo sdlSurface = Surface.ToObj<SdlSurfaceInfo>();
                source = sdlSurface.Pixels;
                int oldWidth = width;
                int oldHeight = height;
                width = sdlSurface.Width;
                height = sdlSurface.Height;
                length = width * height;
#if Advanced
                flags = flags.ResizedData(width, height, oldWidth, oldHeight);
#endif
            }
#endregion

#region RAISE PAINT
            public void InvokePaint(ulong command = 0, int processID = 0) =>
            Window.InvokePaint(command, processID);
#endregion

#region DISPOSE
            public void Dispose()
            {
                if (!Window.IsDisposed)
                    return;
                IsDisposed = true;
#if Advanced
                flags = null;
#endif
            }
#endregion
        }
#if HideSdlObjects
    }
#endif
}
#endif
