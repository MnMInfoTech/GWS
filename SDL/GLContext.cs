/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if Window && SDL

using System;

namespace MnM.GWS
{
    public enum GlContextAttribute
    {
        RedSize = 0,
        GreenSize = 1,
        BlueSize = 2,
        AlphaSize = 3,
        BufferSize = 4,
        DoubleBuffer = 5,
        DepthSize = 6,
        StencilSize = 7,
        AccumRedSize = 8,
        AccumGreenSize = 9,
        AccumBlueSize = 10,
        AccumAlphaSize = 11,
        Stereo = 12,
        MultiSampleBuffers = 13,
        MultiSampleSamples = 14,
        AcceleratedVisual = 15,
        SwapControl = 16,
        ContextMajorVersion = 17,
        ContextMinorVersion = 18,
        ContextEGL = 19,
        ContextFlags = 20,
        ContextProfileMAsk = 21,
        ShareWithCurrentContext = 22,
    }

    partial class Factory
    {
        sealed class GLContext : IGLContext
        {
            #region variables
            IntPtr window;
            static IntPtr current;
            IRenderWindow texture;
            #endregion

            #region constructors
            GLContext() { }
            internal GLContext(IHandle window)
            {
                this.window = window.Handle;
                Handle = SdlAPI.CreateGLContext(window.Handle);
            }
            #endregion

            #region PROPERTIES
            public IntPtr Handle { get; private set; }
            public int SwapInterval
            {
                get
                {
                    MakeCurrent();
                    return SdlAPI.GetGLSwapInterval();
                }
                set
                {
                    MakeCurrent();
                    SdlAPI.SetGLSwapInterval(value);
                }
            }
            public bool IsCurrent =>
                current == Handle;
            public int this[int attribute]
            {
                get
                {
                    MakeCurrent();
                    return SdlAPI.GetGLAttribute((GlContextAttribute)attribute);
                }
                set
                {
                    MakeCurrent();
                    SdlAPI.SetGLAttribute((GlContextAttribute)attribute, value);
                }
            }
            #endregion

            #region RESET ATTRIBUTES
            public void Reset() =>
                SdlAPI.ResetGLAttributes();
            #endregion

            #region GET CURRENT
            public void GetCurrent() =>
                SdlAPI.GetCurrentGLContext();
            #endregion

            #region GET FUNCTION
            public IntPtr GetFunction(string fxName) =>
                SdlAPI.GetGLFunction(fxName);
            #endregion

            #region BIND - UNBIND
            public unsafe void BindTexture(IRenderWindow texture, float? width = null, float? height = null)
            {
                float w, h;

                w = width ?? texture.Width;
                h = height ?? texture.Height;

                SdlAPI.BindGLTexture(texture.Handle, &w, &h);
                this.texture = texture;
            }
            public void UnbindTexture()
            {
                if (texture != null)
                {
                    SdlAPI.UnbindGLTexture(texture.Handle);
                    texture = null;
                }
            }
            #endregion

            #region MAKE CURRENT
            public void MakeCurrent()
            {
                if (!IsCurrent)
                    SdlAPI.MakeGLCurrent(window, Handle);
                current = Handle;
            }
            #endregion

            #region SWAP
            public void Swap()
            {
                MakeCurrent();
                SdlAPI.SwapGLWindow(window);
            }
            #endregion

            public void Dispose() =>
                SdlAPI.DestroyGLContext(Handle);
        }
    }
}
#endif