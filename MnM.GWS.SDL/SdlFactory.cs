/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

#if Window
using System;
using System.Runtime.InteropServices;

#if Advanced
using MnM.GWS.Advanced;
#else
using MnM.GWS.Standard;
#endif

namespace MnM.GWS
{
    public partial class SdlFactory : NativeFactory, IWindowFactory
    {
        #region VARIABLES
        static SdlScreens screens;
        static IScreen primary;
        static readonly uint[] pixelFormats;
        internal static readonly uint pixelFormat;
        static readonly bool initialized;
        internal const int MaxAxisCount = 10;
        internal const int MaxDPadCount = 2;
        static readonly OS os;
        public static readonly new IWindowFactory Instance = new SdlFactory();
        #endregion

        #region CONSTRUCTORS
        static SdlFactory()
        {
            initialized = detectSdl2(out string message);

            if (!initialized)
                throw new Exception(message);

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
        SdlFactory() { }

        static bool detectSdl2(out string message)
        {
            message = "SDL could not Initialized!";
            try
            {
                if (WasInit(0))
                {
                    message = "SDL is Initialized!";
                    return true;
                }
                else
                {
                    if (Init(SystemFlags.VIDEO) == 0)
                    {
                        message = "SDL is Initialized!";
                        return true;
                    }
                    else
                        message = (string.Format("SDL2 init failed with error: {0}", GetError()));
                }
            }
            catch (System.Exception e)
            {
                message = (string.Format("SDL2 init failed with error: {0}", e.Message));
            }
            return false;
        }
        #endregion
         
        #region PROPERTIES
        public IScreens AvailableScreens
        {
            get
            {
                if (screens == null)
                    screens = new SdlScreens();
                return screens;
            }
        }
        public IScreen PrimaryScreen
        {
            get
            {
                if (primary == null)
                    primary = AvailableScreens.Primary;
                return primary;
            }
        }
        public int DefaultWinFlag => (int)WindowFlags.Default;
        public int FullScreenWinFlag => (int)WindowFlags.FullScreen;
        public uint[] PixelFormats => pixelFormats;
        public uint PixelFormat => pixelFormat;
        public bool Initialized => initialized;
        public OS OS => os;
        public string LastError => GetError();
        #endregion

        #region TEXTURE
        public virtual ITexture newTexture(IRenderWindow window, int? w = null, int? h = null, bool isPrimary = false, TextureAccess? textureAccess = null) =>
            new SdlTexture(window, w, h, isPrimary, null, textureAccess);
        public virtual ITexture newTexture(IRenderWindow window, ICopyable info, bool isPrimary = false, TextureAccess? textureAccess = null) =>
            new SdlTexture(window, info, isPrimary, null, textureAccess);
        #endregion

        #region WINDOW
        public IWindow newWindow(string title = null, int? width = null, int? height = null,
            int? x = null, int? y = null, GwsWindowFlags? flags = null, IScreen display = null, RendererFlags? renderFlags = null) =>
            new SdlWindow(title, width, height, x, y, flags, display, renderFlags);
        public IWindow newWindow(IExternalWindow control) =>
            new SdlWindow(control);
        public int GetWindowID(IntPtr window) =>
            WindowID(window);
        public void SetCursorPos(int x, int y) =>
            SetCursorPosition(x, y);
        public void DisableScreenSaver() =>
            DisableScreenSaverEx();
        #endregion

        #region OPENGL CONTEXT
        public IGLContext newGLContext(IWindow window) =>
            GLContext.Create(window);
        #endregion

        #region SAVE AS BITMAP
        public unsafe bool SaveAsBitmap(ICopyable image, string file)
        {
            if (image == null)
                return false;
            image.Portion(out IntPtr data);
            return SaveAsBitmap(data, image.Width, image.Height, file);
        }
        public unsafe bool SaveAsBitmap(IntPtr Pixels, int width, int height, string file)
        {
            if (Pixels == IntPtr.Zero)
                return false;

            var format = Factory.PixelFormat;
            var surface = CreateSurface(Pixels, width, height, Depth(format), Pitch(format) * width, format);

            var raw = OpenFile(UTF8_ToNative(file), UTF8_ToNative("wb"));
            SaveBMPRW(surface, raw, 1);
            return true;
        }
        #endregion

        #region CURSOR
        public int ConvertToSystemCursorID(CursorType cursorType)
        {
            switch (cursorType)
            {
                case CursorType.Arrow:
                case CursorType.Default:
                    return 0;//SDL_SYSTEM_CURSOR_ARROW;
                case CursorType.IBeam:
                    return 1;// SystemCursor.SDL_SYSTEM_CURSOR_IBEAM;
                case CursorType.WaitCursor:
                    return 2;//SystemCursor.SDL_SYSTEM_CURSOR_WAIT;
                case CursorType.Cross:
                    return 3;//SDL_SYSTEM_CURSOR_CROSSHAIR;
                case CursorType.SizeNWSE:
                    return 5;// SystemCursor.SDL_SYSTEM_CURSOR_SIZENWSE;
                case CursorType.SizeNESW:
                    return 6;// SystemCursor.SDL_SYSTEM_CURSOR_SIZENESW;
                case CursorType.SizeWE:
                    return 7;//SystemCursor.SDL_SYSTEM_CURSOR_SIZEWE;
                case CursorType.SizeNS:
                    return 8;//SystemCursor.SDL_SYSTEM_CURSOR_SIZENS;
                case CursorType.SizeAll:
                    return 9;//SystemCursor.SDL_SYSTEM_CURSOR_SIZEALL;
                case CursorType.No:
                    return 10;// SystemCursor.SDL_SYSTEM_CURSOR_NO;
                case CursorType.Hand:
                    return 11;// SystemCursor.SDL_SYSTEM_CURSOR_HAND;
                default:
                    return (int)cursorType;
            }
        }
        public void SetCursor(IntPtr cursor) =>
            SetWindowCursor(cursor);
        #endregion

        #region EVENTS
        public void PushEvent(IEvent e)
        {
            if (!(e is Event))
                return;
            Event evt = (Event)e;
            PushEventEx(evt);
        }
        public void PumpEvents() =>
            pumpEvents();
        public bool PollEvent(out IEvent e)
        {
            Event evt;
            var i = PollEventEx(out evt);
            e = evt;
            return i != 0;
        }
        #endregion

        #region WAV PLAYER
        public ISound newWavPlayer() => new SdlSound();
        #endregion

        protected override void Dispose2()
        {
            base.Dispose2();
            SdlSoundBase.QuitAudio();
        }
    }
    partial class SdlFactory
    {
        const string libSDL = GWS.Application.libSDL;

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

        static byte[] UTF8_ToNative(string s)
        {
            if (s == null)
                return null;
            return System.Text.Encoding.UTF8.GetBytes(s + '\0');
        }
        #endregion

        #region SURFACE BINDINGS
        [DllImport(libSDL, EntryPoint = "SDL_GetWindowSurface", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal unsafe static extern IntPtr GetWindowSurface(IntPtr window);

        [DllImport(libSDL, EntryPoint = "SDL_UpdateWindowSurface", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int UpdateWindowSurface(IntPtr window);

        [DllImport(libSDL, EntryPoint = "SDL_UpdateWindowSurface", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int UpdateWindowSurface(IntPtr window,  Rectangle[] rects, int numrects);
        internal static int UpdateWindowSurface(IntPtr window, params  Rectangle[] rects)
        {
            return UpdateWindowSurface(window, rects, rects.Length);
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
            fixed(int* pixels= data)
            {
               return  CreateSurface((IntPtr)pixels, width, height, Depth(format), Pitch(format) * width, format);
            }
        }
        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_FreeSurface", ExactSpelling = true)]
        internal static extern void FreeSurface(IntPtr surface);
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