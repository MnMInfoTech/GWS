/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if Window
using System;
using System.Runtime.InteropServices;

using MnM.GWS.SDL;

namespace MnM.GWS
{
    public partial class NativeFactory 
    {
        #region VARIABLES
        static SdlScreens Screens;
        static IScreen Primary;
        static uint[] pixelFormats;
        internal static uint pixelFormat;
        internal const int MaxAxisCount = 10;
        internal const int MaxDPadCount = 2;
        static OS os;
        #endregion

        #region CONSTRUCTORS
        static partial void InitializeWindowingSystem(ref bool initialized)
        {
            initialized = false;
            try
            {
                WasInit(0);
                Init(SystemFlags.VIDEO);
            }
            catch (System.Exception e)
            {
                throw new Exception(e.Message);
            }
            initialized = true;
            os = OS.None;

            if (IntPtr.Size == 4)
                os |= OS.X86;
            else
                os |= OS.X64;

            var platform = Operations.IntPtrToString(GetPlatform());
            switch (platform)
            {
                case "Windows":
                default:
                    os |= OS.Windows;
                    break;
                case "Mac OS X":
                    os |= OS.MacOsX;
                    break;
                case "Linux":
                    os |= OS.Linux;
                    break;
                case "iOS":
                    os |= OS.IOS;
                    break;
                case "Android":
                    os |= OS.Android;
                    break;
            }

            pixelFormats = GetWindowFormats();
            if (pixelFormats.Length > 0)
                pixelFormat = pixelFormats[0];
            else
                pixelFormat = ARGB8888;
        }
        #endregion

        #region PROPERTIES
        partial void GetScreen(ref IScreen screen)
        {
            if (Primary == null)
                Primary = AvailableScreens.Primary;
            screen = Primary;
        }
        partial void GetScreens(ref IScreens screens)
        {
            if (Screens == null)
                Screens = new SdlScreens();
            screens = Screens;
        }
        partial void GetDefaultWinFlag(ref int winflag)
        {
            winflag = (int)WindowFlags.Default;
        }
        partial void GetScreenFlag(ref int winflag)
        {
            winflag = (int)WindowFlags.FullScreen;
        }
        partial void GetPixelFormats(ref uint[] pixelFormats)
        {
            pixelFormats = NativeFactory.pixelFormats;
        }
        partial void GetPixelFormat(ref uint pixelFormat)
        {
            pixelFormat = NativeFactory.pixelFormat;
        }
        partial void GetOS(ref OS os)
        {
            os = NativeFactory.os;
        }
        partial void GetLastError(ref string error)
        {
            error = GetError();
        }
        #endregion

        #region TEXTURE
        partial void newTexture(ref ITexture texture, IRenderWindow window, int? w, int? h, bool isPrimary, TextureAccess? textureAccess)  =>
           texture = new SdlTexture(window, w, h, isPrimary, null, textureAccess);
        partial void newTexture(ref ITexture texture, IRenderWindow window, ICopyable info, bool isPrimary, TextureAccess? textureAccess) =>
            texture = new SdlTexture(window, info, isPrimary, null, textureAccess);
        #endregion

        #region WINDOW
        partial void newWindow(ref IWindow window, string title, int? width, int? height, int? x, int? y, 
            GwsWindowFlags? flags, IScreen display, RendererFlags? renderFlags) =>
            window = new SdlWindow(title, width, height, x, y, flags, display, renderFlags);
        partial void newWindow(ref IWindow window, IExternalTarget control) =>
            window = new SdlWindow(control);
        partial void GetWindowID2(ref int id, IntPtr window) =>
            id = WindowID(window);
        partial void SetCursorPos2(int x, int y) =>
            SetCursorPosition(x, y);
        partial void DisableScreenSaver2() =>
            DisableScreenSaverEx();
        #endregion

        #region RENDER TARGET
        partial void newRenderTarget(ref IRenderTarget target, IRenderWindow window)=>
             target = new SdlWindowSurface(window);
        #endregion

        #region OPENGL CONTEXT
        partial void newGLContext(ref IGLContext glContext, IWindow window) =>
            glContext = GLContext.Create(window);
        #endregion

        #region SAVE AS BITMAP
        partial void SaveAsBitmap(ref bool success, IntPtr Pixels, int width, int height, string file)
        {
            var format = Factory.PixelFormat;
            var surface = CreateSurface(Pixels, width, height, Depth(format), Pitch(format) * width, format);

            var raw = OpenFile(UTF8_ToNative(file), UTF8_ToNative("wb"));
            SaveBMPRW(surface, raw, 1);
            success = true;
        }
        #endregion

        #region CURSOR
        partial void GetCursorID(ref int systemCursorID, CursorType cursorType)
        {
            switch (cursorType)
            {
                case CursorType.Arrow:
                case CursorType.Default:
                    systemCursorID = 0;//SDL_SYSTEM_CURSOR_ARROW;
                    break;
                case CursorType.IBeam:
                    systemCursorID = 1;// SystemCursor.SDL_SYSTEM_CURSOR_IBEAM;
                    break;
                case CursorType.WaitCursor:
                    systemCursorID = 2;//SystemCursor.SDL_SYSTEM_CURSOR_WAIT;
                    break;
                case CursorType.Cross:
                    systemCursorID = 3;//SDL_SYSTEM_CURSOR_CROSSHAIR;
                    break;
                case CursorType.SizeNWSE:
                    systemCursorID = 5;// SystemCursor.SDL_SYSTEM_CURSOR_SIZENWSE;
                    break;
                case CursorType.SizeNESW:
                    systemCursorID = 6;// SystemCursor.SDL_SYSTEM_CURSOR_SIZENESW;
                    break;
                case CursorType.SizeWE:
                    systemCursorID = 7;//SystemCursor.SDL_SYSTEM_CURSOR_SIZEWE;
                    break;
                case CursorType.SizeNS:
                    systemCursorID = 8;//SystemCursor.SDL_SYSTEM_CURSOR_SIZENS;
                    break;
                case CursorType.SizeAll:
                    systemCursorID = 9;//SystemCursor.SDL_SYSTEM_CURSOR_SIZEALL;
                    break;
                case CursorType.No:
                    systemCursorID = 10;// SystemCursor.SDL_SYSTEM_CURSOR_NO;
                    break;
                case CursorType.Hand:
                    systemCursorID = 11;// SystemCursor.SDL_SYSTEM_CURSOR_HAND;
                    break;
                default:
                    systemCursorID = (int)cursorType;
                    break;
            }
        }

        partial void SetCursor2(IntPtr cursor) =>
            SetWindowCursor(cursor);
        #endregion

        #region EVENTS
        partial void PumpEvents2() =>
            pumpEvents();
        partial void PushEvent2(IEvent e)
        {
            if (!(e is Event))
                return;
            Event evt = (Event)e;
            PushEventEx(evt);
        }
        partial void PollEvent2(ref bool success, ref IEvent e)
        {
            Event ev;
            var i = PollEventEx(out ev);
            e = ev;
            success = i != 0;
        }
        #endregion

        #region WAV PLAYER
        partial void newWavPlayer(ref ISound sound) =>
            sound =  new SdlSound();
        #endregion

        partial void Dispose2()
        {
            SdlSoundBase.QuitAudio();
        }
    }
   
    partial class NativeFactory
    {
        const string libSDL = Application.libSDL;

        #region PIXEL FORMATS
        const uint UnKnown = 0,
          ARGB8888 = 374743044,
          Index1LSB = 374743044,
          Index1MSB = 286261504,
          Index4LSB = 287310080,
          Index4MSB = 303039488,
          Index8 = 304088064,
          RGB332 = 318769153,
          RGB444 = 338757633,
          RGB555 = 355601410,
          BGR555 = 355667714,
          ARGB4444 = 286461698,
          RGBA4444 = 357699586,
          ABGR4444 = 361893890,
          BGRA4444 = 362942466,
          ARGB1555 = 357765122,
          RGBA5551 = 358879234,
          ABGR1555 = 361959426,
          BGRA5551 = 363073538,
          RGB565 = 355799042,
          BGR565 = 359993346,
          RGB24 = 397416451,
          BGR24 = 400562179,
          RGB888 = 372643844,
          RGBX8888 = 373692420,
          BGR888 = 376838148,
          BGRX8888 = 377886724,
          RGBA8888 = 375791620,
          ABGR8888 = 378937348,
          BGRA8888 = 379985924,
          ARGB2101010 = 374808580,
          YV12 = 842094169,
          IYUV = 1448433993,
          YUY2 = 844715353,
          UYVY = 1498831189,
          YVYU = 1431918169;
        #endregion

        #region MISC
        internal static int Pitch(uint format)
        {
            if (IsFourCC(format))
            {
                if ((format == YUY2) ||
                        (format == UYVY) ||
                        (format == YVYU))
                {
                    return 2;
                }
                return 1;
            }
            return (byte)((uint)format & 0xFF);
        }
        internal static bool IsFourCC(uint format) =>
            (format == 0) && (GetFlag(format) != 1);

        internal static byte GetFlag(uint format) =>
            (byte)(((uint)format >> 28) & 0x0F);
        internal static int Depth(uint format) =>
                 (byte)(((uint)format >> 8) & 0xFF);

        internal static byte[] UTF8_ToNative(string s)
        {
            if (s == null)
                return null;
            return System.Text.Encoding.UTF8.GetBytes(s + '\0');
        }
        #endregion

        #region SURFACE BINDINGS
        [DllImport(libSDL, EntryPoint = "SDL_LowerBlit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int CopySurface(IntPtr src, Rectangle srcrect, IntPtr dst, Rectangle dstrect);

        [DllImport(libSDL, EntryPoint = "SDL_GetWindowSurface", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal unsafe static extern IntPtr GetWindowSurface(IntPtr window);

        [DllImport(libSDL, EntryPoint = "SDL_UpdateWindowSurface", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int UpdateWindowSurface(IntPtr window);

        [DllImport(libSDL, EntryPoint = "SDL_UpdateWindowSurfaceRects", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int UpdateWindowSurfaceRects(IntPtr window, Rectangle[] rects, int numrects);

        internal static int UpdateWindow(IntPtr window, params Rectangle[] rectangles)
        {
            return UpdateWindowSurfaceRects(window, rectangles, rectangles.Length);
        }

        [DllImport(libSDL, EntryPoint = "SDL_CreateRGBSurfaceWithFormatFrom", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal unsafe static extern IntPtr CreateSurface(IntPtr pixels, int width, int height, int depth, int pitch, uint format);


        [DllImport(libSDL, EntryPoint = "SDL_CreateRGBSurfaceWithFormat", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr CreateSurface(uint flags, int width, int height, int depth, uint format);
        internal static IntPtr CreateSurface(int width, int height) =>
            CreateSurface(0, width, height, Depth(pixelFormat), pixelFormat);
        internal static unsafe IntPtr CreateSurface(IntPtr pixels, int width, int height, uint format) =>
            CreateSurface(pixels, width, height, Depth(format), Pitch(format) * width, format);
        internal static unsafe IntPtr CreateSurface(int[] data, int width, int height, uint format)
        {
            fixed (int* pixels = data)
            {
                return CreateSurface((IntPtr)pixels, width, height, Depth(format), Pitch(format) * width, format);
            }
        }

        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_FreeSurface", ExactSpelling = true)]
        internal static extern void FreeSurface(IntPtr surface);

        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_SetClipRect", ExactSpelling = true)]
        internal static extern int ClipSurface(IntPtr surface, Rectangle rect);
        #endregion

        #region TEXTURE BINDINGS
        [DllImport(libSDL, EntryPoint = "SDL_GetTextureAlphaMod", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int GetTextureAlpha(IntPtr texture, out byte alpha);
        internal static byte GetTextureAlpha(IntPtr texture)
        {
            GetTextureAlpha(texture, out byte a);
            return a;
        }

        [DllImport(libSDL, EntryPoint = "SDL_SetTextureAlphaMod", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SetTextureAlpha(IntPtr texture, byte alpha);

        [DllImport(libSDL, EntryPoint = "SDL_GetTextureColorMod", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int GetTextureColorMod(IntPtr texture, out byte r, out byte g, out byte b);
        internal static int GetTextureColorMod(IntPtr texture)
        {
            GetTextureColorMod(texture, out byte r, out byte g, out byte b);
            return Colors.ToColor(r, g, b);
        }

        [DllImport(libSDL, EntryPoint = "SDL_SetTextureColorMod", CallingConvention = CallingConvention.Cdecl)]
        static extern int SetTextureColorMod(IntPtr texture, byte r, byte g, byte b);
        internal static int SetTextureColorMod(IntPtr texture, int color)
        {
            Colors.ToRGB(color, out byte r, out byte g, out byte b);
            return SetTextureColorMod(texture, r, g, b);
        }

        [DllImport(libSDL, EntryPoint = "SDL_GetTextureBlendMode", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int GetTextureBlendMode(IntPtr texture, out BlendMode blendMode);
        internal static BlendMode GetTextureBlendMode(IntPtr texture)
        {
            GetTextureBlendMode(texture, out BlendMode mode);
            return mode;
        }

        [DllImport(libSDL, EntryPoint = "SDL_SetTextureBlendMode", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SetTextureBlendMod(IntPtr texture, BlendMode blendMode);

        [DllImport(libSDL, EntryPoint = "SDL_GetRenderTarget", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetRenderTarget(IntPtr texture);

        [DllImport(libSDL, EntryPoint = "SDL_SetRenderTarget", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SetRenderTarget(IntPtr renderer, IntPtr texture);

        [DllImport(libSDL, EntryPoint = "SDL_LockTexture", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int LockTexture(IntPtr texture, IntPtr rect, out IntPtr pixels, out int pitch);

        [DllImport(libSDL, EntryPoint = "SDL_UnlockTexture", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void UnlockTexture(IntPtr texture);

        [DllImport(libSDL, EntryPoint = "SDL_DestroyTexture", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void DestroyTexture(IntPtr texture);

        internal static int RenderCopyTexture(IntPtr renderer, IntPtr texture, Rectangle sourceRC, Rectangle destRC)
        {
            var srcRc = new Rectangle(sourceRC);
            var dstRc = new Rectangle(destRC);
            IntPtr dstH = IntPtr.Zero;
            IntPtr srcH = IntPtr.Zero;

            if (srcRc)
                srcH = srcRc.ToPtr();
            if (dstRc)
                dstH = dstRc.ToPtr();

            int i = RenderCopyTexture(renderer, texture, srcH, dstH);
            if (srcH != IntPtr.Zero)
                srcH.FreePtr();
            if (dstH != IntPtr.Zero)
                dstH.FreePtr();
            return i;
        }

        [DllImport(libSDL, EntryPoint = "SDL_CreateTexture", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr CreateTexture(IntPtr renderer, uint format, TextureAccess access, int w, int h);

        [DllImport(libSDL, EntryPoint = "SDL_CreateTextureFromSurface", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr CreateTexture(IntPtr renderer, IntPtr surface);

        [DllImport(libSDL, EntryPoint = "SDL_QueryTexture", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int QueryTexture(IntPtr texture, out uint format, out TextureAccess access, out int w, out int h);

        [DllImport(libSDL, EntryPoint = "SDL_UpdateTexture", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int UpdateTexture(IntPtr texture, Rectangle rect, IntPtr pixels, int pitch);
        #endregion

        #region RENDERER BINDINGS
        [DllImport(libSDL, EntryPoint = "SDL_GetRenderer", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr GetRenderer(IntPtr window);

        [DllImport(libSDL, EntryPoint = "SDL_GetRendererInfo", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int GetRenderInfo(IntPtr renderer, out RendererInfo info);

        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_CreateWindowAndRenderer", ExactSpelling = true)]
        internal static extern int CreateWindowAndRenderer(
            int width, int height, int window_flags, out IntPtr IntPtr, out IntPtr renderer);

        [DllImport(libSDL, EntryPoint = "SDL_DestroyRenderer", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void DestroyRenderer(IntPtr renderer);

        [DllImport(libSDL, EntryPoint = "SDL_RenderSetClipRect", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RenderSetClipRect(IntPtr renderer, IntPtr rect);

        [DllImport(libSDL, EntryPoint = "SDL_GetRendererOutputSize", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int GetRendererOutputSize(IntPtr renderer, out int w, out int h);

        [DllImport(libSDL, EntryPoint = "SDL_RenderGetScale", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RenderGetScale(IntPtr renderer, out float scaleX, out float scaleY);

        [DllImport(libSDL, EntryPoint = "SDL_RenderSetScale", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RenderSetScale(IntPtr renderer, float scaleX, float scaleY);

        [DllImport(libSDL, EntryPoint = "SDL_RenderGetViewport", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RenderGetViewport(IntPtr renderer, out Rectangle rect);

        [DllImport(libSDL, EntryPoint = "SDL_RenderSetViewport", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RenderSetViewport(IntPtr renderer, IntPtr rect);

        [DllImport(libSDL, EntryPoint = "SDL_RenderGetLogicalSize", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RenderGetLogicalSize(IntPtr renderer, out int w, out int h);

        [DllImport(libSDL, EntryPoint = "SDL_RenderSetLogicalSize", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RenderSetLogicalSize(IntPtr renderer, int w, int h);

        [DllImport(libSDL, EntryPoint = "SDL_RenderGetIntegerScale", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool RenderGetIntegerScale(IntPtr renderer);

        [DllImport(libSDL, EntryPoint = "SDL_RenderSetIntegerScale", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RenderSetIntegerScale(IntPtr renderer, int enable);

        [DllImport(libSDL, EntryPoint = "SDL_CreateRenderer", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr CreateRenderer(IntPtr window, int index, RendererFlags flags);

        [DllImport(libSDL, EntryPoint = "SDL_RenderCopy", CallingConvention = CallingConvention.Cdecl)]
        unsafe internal static extern int RenderCopyTexture(IntPtr renderer, IntPtr texture, IntPtr sourceRC, IntPtr destRC);

        [DllImport(libSDL, EntryPoint = "SDL_RenderPresent", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void UpdateRenderer(IntPtr renderer);

        [DllImport(libSDL, EntryPoint = "SDL_RenderClear", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ClearRenderer(IntPtr renderer);

        [DllImport(libSDL, EntryPoint = "SDL_RenderReadPixels", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int ReadPixels(IntPtr renderer, IntPtr rect, uint format, IntPtr pixels, int pitch);

        [DllImport(libSDL, EntryPoint = "SDL_RenderReadPixels", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal unsafe static extern int ReadPixels(IntPtr renderer, IntPtr rect, uint format, int* pixels, int pitch);
        #endregion

        #region WINDOW - RENDERER BINDINGS
        [DllImport(libSDL, EntryPoint = "SDL_CaptureMouse", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int CaptureMouse(bool enabled);

        [DllImport(libSDL, EntryPoint = "SDL_GetScancodeFromKey", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern Scancode GetScancodeFromKey(Keycode key);

        [DllImport(libSDL, EntryPoint = "SDL_GetWindowFromID", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr GetWindowFromID(int id);

        [DllImport(libSDL, EntryPoint = "SDL_WarpMouseInWindow", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void WarpMouseInWindow(IntPtr window, int x, int y);

        [DllImport(libSDL, EntryPoint = "SDL_ShowWindow", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void ShowWindow(IntPtr IntPtr);

        [DllImport(libSDL, EntryPoint = "SDL_HideWindow", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void HideWindow(IntPtr IntPtr);

        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_RestoreWindow", ExactSpelling = true)]
        internal static extern void RestoreWindow(IntPtr IntPtr);

        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_RaiseWindow", ExactSpelling = true)]
        internal static extern void RaiseWindow(IntPtr IntPtr);

        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_SetWindowInputFocus", ExactSpelling = true)]
        internal static extern int SetFocus(IntPtr window);

        [DllImport(libSDL, EntryPoint = "SDL_SetWindowFullscreen", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int SetWindowFullscreen(IntPtr IntPtr, uint flags);

        [DllImport(libSDL, EntryPoint = "SDL_MinimizeWindow", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void MinimizeWindow(IntPtr IntPtr);

        [DllImport(libSDL, EntryPoint = "SDL_MaximizeWindow", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void MaximizeWindow(IntPtr IntPtr);

        [DllImport(libSDL, EntryPoint = "SDL_GetWindowPosition", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void GetWindowPosition(IntPtr IntPtr, out int x, out int y);

        [DllImport(libSDL, EntryPoint = "SDL_SetWindowPosition", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void SetWindowPosition(IntPtr IntPtr, int x, int y);

        [DllImport(libSDL, EntryPoint = "SDL_GetWindowSize", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void GetWindowSize(IntPtr window, out int w, out int h);

        [DllImport(libSDL, EntryPoint = "SDL_SetWindowSize", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void SetWindowSize(IntPtr IntPtr, int w, int h);

        [DllImport(libSDL, EntryPoint = "SDL_GetWindowTitle", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr GetWindowTitlePrivate(IntPtr IntPtr);

        [DllImport(libSDL, EntryPoint = "SDL_SetWindowTitle", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void SetWindowTitle(IntPtr IntPtr, string title);

        [DllImport(libSDL, EntryPoint = "SDL_SetWindowOpacity", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SetWindowOpacity(IntPtr IntPtr, float opacity);

        [DllImport(libSDL, EntryPoint = "SDL_SetWindowBordered", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void SetWindowBordered(IntPtr IntPtr, bool bordered);

        [DllImport(libSDL, EntryPoint = "SDL_ShowCursor", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int ShowCursor(int toggle);

        [DllImport(libSDL, EntryPoint = "SDL_FreeCursor", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void FreeCursor(IntPtr IntPtr);

        [DllImport(libSDL, EntryPoint = "SDL_SetWindowGrab", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void SetWindowGrab(IntPtr IntPtr, bool grabbed);

        [DllImport(libSDL, EntryPoint = "SDL_SetRelativeMouseMode", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int SetRelativeMouseMode(bool enabled);

        [DllImport(libSDL, EntryPoint = "SDL_WarpMouseInWindow", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetCursorPos(IntPtr IntPtr, int x, int y);

        [DllImport(libSDL, EntryPoint = "SDL_SetHint", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int SetHint(string name, string value);

        [DllImport(libSDL, EntryPoint = "SDL_PumpEvents", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void pumpEvents();

        [DllImport(libSDL, EntryPoint = "SDL_free", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void Free(IntPtr memblock);

        [DllImport(libSDL, EntryPoint = "SDL_RWFromFile", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr OpenFile(byte[] file, byte[] mode);

        [DllImport(libSDL, EntryPoint = "SDL_SaveBMP_RW", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SaveBMPRW(IntPtr surface, IntPtr src, int freesrc);

        [DllImport(libSDL, EntryPoint = "SDL_PollEvent", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int PollEventEx(out Event e);

        [DllImport(libSDL, EntryPoint = "SDL_PushEvent", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int PushEventEx(ref Event @event);

        [DllImport(libSDL, EntryPoint = "SDL_CreateColorCursor", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr CreateColorCursor(IntPtr surface, int hot_x, int hot_y);

        [DllImport(libSDL, EntryPoint = "SDL_GetWindowID", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int WindowID(IntPtr window);

        [DllImport(libSDL, EntryPoint = "SDL_SetCursor", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void SetWindowCursor(IntPtr IntPtr);

        [DllImport(libSDL, EntryPoint = "SDL_WarpMouseGlobal", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SetCursorPosition(int x, int y);


        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_CreateWindow", ExactSpelling = true)]
        internal static extern IntPtr CreateWindow(string title, int x, int y, int w, int h, int flags);

        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_CreateWindowFrom", ExactSpelling = true)]
        internal static extern IntPtr CreateWindowFrom(IntPtr external);

        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_DestroyWindow", ExactSpelling = true)]
        internal static extern void DestroyWindow(IntPtr IntPtr);

        [DllImport(libSDL, EntryPoint = "SDL_CreateSystemCursor", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr CreateSystemCursor(int id);

        [DllImport(libSDL, EntryPoint = "SDL_GetDisplayBounds", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int GetDisplayBounds(int displayIndex, out Rectangle rect);

        [DllImport(libSDL, EntryPoint = "SDL_GetCurrentDisplayMode", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int GetCurrentDisplayMode(int displayIndex, out DisplayMode mode);

        [DllImport(libSDL, EntryPoint = "SDL_GetTicks", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint GetTicks();

        [DllImport(libSDL, EntryPoint = "SDL_DisableScreenSaver", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void DisableScreenSaverEx();
        #endregion

        #region SDL PLATFORM

        [DllImport(libSDL, EntryPoint = "SDL_GetPlatform", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr GetPlatform();

        [DllImport(libSDL, EntryPoint = "SDL_GetError", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern IntPtr getErrorInternal();
        public static string GetError() => Operations.IntPtrToString(getErrorInternal());

        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_WasInit", ExactSpelling = true)]
        static extern bool WasInit(SystemFlags flags);

        [DllImport(libSDL, EntryPoint = "SDL_Init", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int Init(SystemFlags flags);

        [DllImport(libSDL, EntryPoint = "SDL_PushEvent", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int PushEventEx(Event @event);

        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_CreateWindow", ExactSpelling = true)]
        internal static extern IntPtr CreateWindow(string title, int x, int y, int w, int h, WindowFlags flags);

        unsafe static uint[] GetWindowFormats()
        {
            CreateWindowAndRenderer(50, 50, (int)WindowFlags.Hidden, out IntPtr w, out IntPtr r);
            GetRenderInfo(r, out RendererInfo ri);
            var len = ri.num_texture_formats;

            var formats = new uint[len];
            for (int i = 0; i < len; i++)
            {
                formats[i] = ri.texture_formats[i];
            }

            DestroyRenderer(r);
            DestroyWindow(w);
            return formats;
        }

        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_Delay", ExactSpelling = true)]
        internal static extern void Sleep(int ms);
        #endregion

        #region SDL GL CONTEXT
        [DllImport(libSDL, EntryPoint = "SDL_GL_CreateContext", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr CreateGLContext(IntPtr window);

        [DllImport(libSDL, EntryPoint = "SDL_GL_DeleteContext", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void DestroyGLContext(IntPtr context);

        [DllImport(libSDL, EntryPoint = "SDL_GL_GetAttribute", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int GetGLAttribute(GlContextAttribute attr, out int value);

        internal static int GetGLAttribute(GlContextAttribute attr)
        {
            GetGLAttribute(attr, out int value);
            return value;
        }

        [DllImport(libSDL, EntryPoint = "SDL_GL_GetCurrentContext", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr GetCurrentGLContext();

        [DllImport(libSDL, EntryPoint = "SDL_GL_GetDrawableSize", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void GetGLDrawableSize(IntPtr IntPtr, out int w, out int h);

        [DllImport(libSDL, EntryPoint = "SDL_GL_GetProcAddress", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr GetGLProcAddress(IntPtr proc);

        internal static IntPtr GetGLFunction(string proc)
        {
            IntPtr p = Marshal.StringToHGlobalAnsi(proc);
            try
            {
                return GetGLProcAddress(p);
            }
            finally
            {
                Marshal.FreeHGlobal(p);
            }
        }

        [DllImport(libSDL, EntryPoint = "SDL_GL_GetSwapInterval", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int GetGLSwapInterval();

        [DllImport(libSDL, EntryPoint = "SDL_GL_MakeCurrent", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int MakeGLCurrent(IntPtr window, IntPtr context);

        [DllImport(libSDL, EntryPoint = "SDL_GL_SetAttribute", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int SetGLAttribute(GlContextAttribute attr, int value);

        [DllImport(libSDL, EntryPoint = "SDL_GL_SetSwapInterval", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int SetGLSwapInterval(int interval);

        [DllImport(libSDL, EntryPoint = "SDL_GL_SwapWindow", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void SwapGLWindow(IntPtr window);

        [DllImport(libSDL, EntryPoint = "SDL_GL_ResetAttributes", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void ResetGLAttributes();

        [DllImport(libSDL, EntryPoint = "SDL_GL_BindTexture", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static unsafe extern int BindGLTexture(IntPtr sdlTexture, float* texw, float* texh);

        [DllImport(libSDL, EntryPoint = "SDL_GL_UnbindTexture", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static unsafe extern int UnbindGLTexture(IntPtr sdlTexture);
        #endregion
    }
}
#endif