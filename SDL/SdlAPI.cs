/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if Window && SDL
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using MnM.GWS.SDL;

namespace MnM.GWS
{
    static class SdlAPI
    {
        #region VARIABLES
        internal readonly static IScreens Screens;
        internal readonly static IScreen Primary;
        internal static uint[] pixelFormats;
        internal static uint pixelFormat;
        internal const int MaxAxisCount = 10;
        internal const int MaxDPadCount = 2;
        internal readonly static OS OS;
        internal readonly static bool Initialized;

        #region EXTERNAL LIBRARY NAMES
#if IPHONE
        public const string libSDL ="__Internal";
        public const string libTTF = "__Internal";
        public const string libFT = "__Internal";

#elif LINUX || ANDROID
        public const string libSDL = "libSDL2.so";
        public const string libTTF = "libSDL2_ttf.so";
        public const string libFT = "libfreetype-6.so";

#elif OSX
        public const string libSDL = "libSDL2.dylib";
        public const string libTTF = "libTTF.dylib";
        public const string libFT = "libfreetype-6.dylib";
#else
        public const string libSDL = "SDL2.dll";
        public const string libTTF = "SDL2_ttf.dll";
        public const string libFT = "libfreetype-6.dll";
        //internal const string libFT = "freetype.dll";
#endif
        #endregion

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
        #endregion

        static SdlAPI()
        {
            try
            {
                WasInit(0);
                Init(SystemFlags.VIDEO);
                Initialized = true;
            }
            catch (System.Exception e)
            {
                throw new Exception(e.Message);
            }
            OS = OS.None;

            if (IntPtr.Size == 4)
                OS |= OS.X86;
            else
                OS |= OS.X64;

            var platform = Operations.IntPtrToString(GetPlatform());
            switch (platform)
            {
                case "Windows":
                default:
                    OS |= OS.Windows;
                    break;
                case "Mac OS X":
                    OS |= OS.MacOsX;
                    break;
                case "Linux":
                    OS |= OS.Linux;
                    break;
                case "iOS":
                    OS |= OS.IOS;
                    break;
                case "Android":
                    OS |= OS.Android;
                    break;
            }

            pixelFormats = GetWindowFormats();
            if (pixelFormats.Length > 0)
                pixelFormat = pixelFormats[0];
            else
                pixelFormat = ARGB8888;

            Screens = new SdlScreens();
            Primary = Screens.Primary;
        }

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

        #region SAVE AS BITMAP
        internal unsafe static bool SaveAsBitmap(ICopyable image, string file, Command command)
        {
            if (image == null)
                return false;
            var Length = image.Width * image.Height;

            fixed (int* destination = new int[Length])
            {
                var dst = (IntPtr)destination;
                image.CopyTo(dst, Length, image.Width, new IParameter[] { command.Replace() });
                var surface = CreateSurface(dst, image.Width, image.Height);
                var raw = OpenFile(UTF8_ToNative(file), UTF8_ToNative("wb"));
                SaveBMPRW(surface, raw, 1);
            }
            return true;
        }
        internal static bool SaveAsBitmap(IntPtr data, int width, int height, string file)
        {
            if (data == IntPtr.Zero)
                return false;

            var surface = CreateSurface(data, width, height);

            var raw = OpenFile(UTF8_ToNative(file), UTF8_ToNative("wb"));
            SaveBMPRW(surface, raw, 1);
            return true;
        }
        #endregion

        #region SURFACE BINDINGS
        [DllImport(libSDL, EntryPoint = "SDL_LowerBlit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int CopySurface(IntPtr src, Rectangle srcrect, IntPtr dst, Rectangle dstrect);

        [DllImport(libSDL, EntryPoint = "SDL_GetWindowSurface", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal unsafe static extern IntPtr GetWindowSurface(IntPtr window);

        [DllImport(libSDL, EntryPoint = "SDL_UpdateWindowSurface", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int UpdateWindow(IntPtr window);

        [DllImport(libSDL, EntryPoint = "SDL_UpdateWindowSurfaceRects", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int UpdateWindow(IntPtr window, Rectangle[] rects, int numrects);

        internal static int UpdateWindow(IntPtr window, params Rectangle[] rectangles)
        {
            return UpdateWindow(window, rectangles, rectangles.Length);
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

        internal static unsafe IntPtr CreateSurface(IntPtr pixels, int width, int height) =>
            CreateSurface(pixels, width, height, 32, 4 * width, Colours.BMask, Colours.GMask, Colours.RMask, Colours.AMASK);

        [DllImport(libSDL, EntryPoint = "SDL_CreateRGBSurfaceFrom", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern IntPtr CreateSurface(IntPtr pixels, int width, int height, int depth, int pitch, uint Rmask, uint Gmask,
            uint Bmask, uint Amask);

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

        [DllImport(libSDL, EntryPoint = "SDL_GetTextureColourMod", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern int GetTextureColourMod(IntPtr texture, out byte r, out byte g, out byte b);
        internal static int GetTextureColourMod(IntPtr texture)
        {
            GetTextureColourMod(texture, out byte r, out byte g, out byte b);
            return Colours.ToColour(r, g, b);
        }

        [DllImport(libSDL, EntryPoint = "SDL_SetTextureColourMod", CallingConvention = CallingConvention.Cdecl)]
        static extern int SetTextureColourMod(IntPtr texture, byte r, byte g, byte b);
        internal static int SetTextureColourMod(IntPtr texture, int colour)
        {
            Colours.ToRGB(colour, out byte r, out byte g, out byte b);
            return SetTextureColourMod(texture, r, g, b);
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
        internal static extern int LockTexture(IntPtr texture, Rectangle rect, out IntPtr pixels, out int pitch);

        [DllImport(libSDL, EntryPoint = "SDL_LockTexture", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int LockTexture(IntPtr texture, IntPtr rect, out IntPtr pixels, out int pitch);

        [DllImport(libSDL, EntryPoint = "SDL_UnlockTexture", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void UnlockTexture(IntPtr texture);

        [DllImport(libSDL, EntryPoint = "SDL_DestroyTexture", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void DestroyTexture(IntPtr texture);

        [DllImport(libSDL, EntryPoint = "SDL_CreateTexture", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr CreateTexture(IntPtr renderer, uint format, TextureAccess access, int w, int h);

        [DllImport(libSDL, EntryPoint = "SDL_CreateTextureFromSurface", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr CreateTexture(IntPtr renderer, IntPtr surface);

        [DllImport(libSDL, EntryPoint = "SDL_QueryTexture", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int QueryTexture(IntPtr texture, out uint format, out TextureAccess access, out int w, out int h);

        [DllImport(libSDL, EntryPoint = "SDL_UpdateTexture", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int UpdateTexture(IntPtr texture, Rectangle rect, IntPtr pixels, int pitch);

        [DllImport(libSDL, EntryPoint = "SDL_UpdateTexture", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int UpdateTexture(IntPtr texture, IntPtr rect, IntPtr pixels, int pitch);
        #endregion

        #region RENDERER BINDINGS
        [DllImport(libSDL, EntryPoint = "SDL_GetRenderer", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr GetRenderer(IntPtr window);

        [DllImport(libSDL, EntryPoint = "SDL_GetRendererInfo", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int GetRenderInfo(IntPtr renderer, out RendererInfo info);

        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_CreateWindowAndRenderer", ExactSpelling = true)]
        internal static extern int CreateWindowAndRenderer(
            int width, int height, int window_flags, out IntPtr IntPtr, out IntPtr renderer);

        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_CreateSoftwareRenderer", ExactSpelling = true)]
        internal static extern IntPtr CreateSoftwareRenderer(IntPtr surface);

        [DllImport(libSDL, EntryPoint = "SDL_CreateRenderer", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr CreateRenderer(IntPtr window, int index, RendererFlags flags);

        [DllImport(libSDL, EntryPoint = "SDL_DestroyRenderer", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void DestroyRenderer(IntPtr renderer);

        [DllImport(libSDL, EntryPoint = "SDL_RenderSetClipRect", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RendererSetClipRect(IntPtr renderer, IntPtr rect);

        [DllImport(libSDL, EntryPoint = "SDL_GetRendererOutputSize", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int GetRendererOutputSize(IntPtr renderer, out int w, out int h);

        [DllImport(libSDL, EntryPoint = "SDL_RenderGetScale", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RendererGetScale(IntPtr renderer, out float scaleX, out float scaleY);

        [DllImport(libSDL, EntryPoint = "SDL_RenderSetScale", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RendererSetScale(IntPtr renderer, float scaleX, float scaleY);

        [DllImport(libSDL, EntryPoint = "SDL_RenderGetViewport", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RendererGetViewport(IntPtr renderer, out Rectangle rect);

        [DllImport(libSDL, EntryPoint = "SDL_RenderSetViewport", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RendererSetViewport(IntPtr renderer, IntPtr rect);

        [DllImport(libSDL, EntryPoint = "SDL_RenderGetLogicalSize", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RendererGetLogicalSize(IntPtr renderer, out int w, out int h);

        [DllImport(libSDL, EntryPoint = "SDL_RenderSetLogicalSize", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RendererSetLogicalSize(IntPtr renderer, int w, int h);

        [DllImport(libSDL, EntryPoint = "SDL_RenderGetIntegerScale", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool RendererGetIntegerScale(IntPtr renderer);

        [DllImport(libSDL, EntryPoint = "SDL_RenderSetIntegerScale", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RendererSetIntegerScale(IntPtr renderer, int enable);

        [DllImport(libSDL, EntryPoint = "SDL_RenderCopy", CallingConvention = CallingConvention.Cdecl)]
        unsafe internal static extern int RendererCopyTexture(IntPtr renderer, IntPtr texture, IntPtr sourceRC, IntPtr destRC);

        [DllImport(libSDL, EntryPoint = "SDL_RenderCopy", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RendererCopyTexture(IntPtr renderer, IntPtr texture, Rectangle sourceRC, Rectangle destRC);


        [DllImport(libSDL, EntryPoint = "SDL_RenderPresent", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void UpdateRenderer(IntPtr renderer);

        [DllImport(libSDL, EntryPoint = "SDL_RenderClear", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ClearRenderer(IntPtr renderer);

        [DllImport(libSDL, EntryPoint = "SDL_RenderReadPixels", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int ReadPixels(IntPtr renderer, IntPtr rect, uint format, IntPtr pixels, int pitch);

        [DllImport(libSDL, EntryPoint = "SDL_RenderReadPixels", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal unsafe static extern int ReadPixels(IntPtr renderer, IntPtr rect, uint format, int* pixels, int pitch);

        [DllImport(libSDL, EntryPoint = "SDL_RenderReadPixels", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal unsafe static extern int ReadPixels(IntPtr renderer, Rectangle rect, uint format, int* pixels, int pitch);

        [DllImport(libSDL, EntryPoint = "SDL_RenderReadPixels", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal unsafe static extern int ReadPixels(IntPtr renderer, Rectangle rect, uint format, IntPtr pixels, int pitch);

        [DllImport(libSDL, EntryPoint = "SDL_SetRenderDrawColour", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int SetDrawColour(IntPtr renderer, int r, int g, int b, int a);

        internal static int SetDrawColour(IntPtr renderer, int colour)
        {
            int r = (byte)((colour >> Colours.RShift) & 0xFF);
            int g = (byte)((colour >> Colours.GShift) & 0xFF);
            int b = (byte)((colour >> Colours.BShift) & 0xFF);
            int a = (byte)((colour >> Colours.AShift) & 0xFF);
            return SetDrawColour(renderer, r, g, b, a);
        }

        [DllImport(libSDL, EntryPoint = "SDL_SetRenderDrawColour", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int DrawPoint(IntPtr renderer, int x, int y);
        #endregion

        #region WINDOW BINDINGS
        [DllImport(libSDL, EntryPoint = "SDL_GetMouseState", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int GetCursorPosition(out int x, out int y);

        [DllImport(libSDL, EntryPoint = "SDL_GetGlobalMouseState", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int GetCursorPositionGlobal(out int x, out int y);
         

        [DllImport(libSDL, EntryPoint = "SDL_CaptureMouse", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int CaptureMouse(bool enabled);

        [DllImport(libSDL, EntryPoint = "SDL_GetScancodeFromKey", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern Scancode GetScancodeFromKey(Keycode key);

        [DllImport(libSDL, EntryPoint = "SDL_GetWindowFromID", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr GetWindowFromID(int id);

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

        [DllImport(libSDL, EntryPoint = "SDL_WarpMouseGlobal", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SetCursorPosition(int x, int y);

        [DllImport(libSDL, EntryPoint = "SDL_WarpMouseInWindow", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SetCursorPosition(IntPtr window, int x, int y);

        [DllImport(libSDL, EntryPoint = "SDL_SetHint", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int SetHint(string name, string value);

        [DllImport(libSDL, EntryPoint = "SDL_SetHintWithPriority", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int SetHintWithPriority(string name, string value, SdlHintPriority priority);

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

        [DllImport(libSDL, EntryPoint = "SDL_CreateColourCursor", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern IntPtr CreateColourCursor(IntPtr surface, int hot_x, int hot_y);

        [DllImport(libSDL, EntryPoint = "SDL_GetWindowID", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int WindowID(IntPtr window);

        [DllImport(libSDL, EntryPoint = "SDL_SetCursor", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern void SetWindowCursor(IntPtr IntPtr);

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

        [DllImport(libSDL, EntryPoint = "SDL_GetDisplayDPI", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int GetDisplayDPI(int displayIndex, out float ddpi, out float hdpi, out float vdpi);

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

        #region MESSAGE BOX
        [DllImport(libSDL, EntryPoint = "SDL_ShowMessageBox", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int ShowMessageBox(MessageBoxData data, ref int buttonID);
        #endregion

        #region GET WINDOWS FLAGS
        [DllImport(libSDL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SDL_GetWindowFlags", ExactSpelling = true)]
        internal static extern WindowFlags GetWindowFlags(IntPtr window);
        #endregion

        #region SCREEN BINDING
        [DllImport(libSDL, EntryPoint = "SDL_PixelFormatEnumToMasks", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern bool PixelFormatToMasks(uint format, out int bpp, out uint rmask, out uint gmask, out uint bmask, out uint amask);

        [DllImport(libSDL, EntryPoint = "SDL_GetNumVideoDisplays", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int GetNumVideoDisplays();

        [DllImport(libSDL, EntryPoint = "SDL_GetNumDisplayModes", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int GetNumDisplayModes(int displayIndex);

        [DllImport(libSDL, EntryPoint = "SDL_GetDisplayMode", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int GetDisplayMode(int displayIndex, int modeIndex, out DisplayMode mode);
        #endregion

        class SdlScreens : IScreens
        {
            #region variables
            const string libSDL = SdlAPI.libSDL;

            static readonly IScreen[] devices;
            static SdlScreen primary;
            static readonly object screenLock = new object();
            #endregion

            #region static constructor
            static SdlScreens()
            {
                int displays = GetNumVideoDisplays();
                devices = new IScreen[displays];
                for (int d = 0; d < displays; d++)
                {
                    Rectangle bounds;
                    SdlAPI.GetDisplayBounds(d, out bounds);
                    SdlAPI.GetCurrentDisplayMode(d, out DisplayMode dm);

                    int total = GetNumDisplayModes(d);
                    var list = new IResolution[total];

                    for (int m = 0; m < total; m++)
                    {
                        GetDisplayMode(d, m, out DisplayMode sdm);
                        list[m] = new SdlResolution(bounds.X, bounds.Y, sdm.Width, sdm.Height, sdm.Format, sdm.RefreshRate);
                    }

                    var current_resolution = new SdlResolution(bounds.X, bounds.Y, dm.Width, dm.Height, dm.Format, dm.RefreshRate);
                    var device = new SdlScreen(current_resolution, d == 0, list, d);

                    devices[d] = (device);
                    if (d == 0)
                        primary = device;
                }
            }
            #endregion

            public IScreen Primary => primary;
            public IScreen this[int index]
            {
                get
                {
                    if (index == (int)DisplayIndex.Primary)
                        return primary;
                    else if ((int)index >= 0 && (int)index < devices.Length)
                        return devices[(int)index];
                    return primary;
                }
            }
            public int Count => devices.Length;

            public IScreen FromPoint(int x, int y)
            {
                for (DisplayIndex i = DisplayIndex.First; i < DisplayIndex.Sixth; i++)
                {
                    var display = this[(int)i];
                    if (display != null)
                    {
                        if (display.Contains(x, y))
                        {
                            return display;
                        }
                    }
                }
                return null;
            }

           

            #region readable list
            public IEnumerator<IScreen> GetEnumerator()
            {
                for (int i = 0; i < devices.Length; i++)
                    yield return devices[i];
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                for (int i = 0; i < devices.Length; i++)
                    yield return devices[i];
            }
            #endregion

            /// <summary>
            /// Defines a display screen on the underlying system, and provides
            /// methods to query and change its display parameters.
            /// </summary>
            class SdlScreen : IScreen
            {
                #region variables
                IResolution original;
                bool isPrimary;
                IResolution[] resolutions;
                internal object monitorID;
                #endregion

                #region constructors
                internal SdlScreen(IResolution currentResolution, bool primary, IResolution[] resolutions, object id)
                {
                    // Todo: Consolidate current resolution with bounds? Can they fall out of sync right now?
                    this.Resolution = currentResolution;
                    this.original = currentResolution;
                    IsPrimary = primary;
                    this.resolutions = (resolutions);
                    this.monitorID = id;
                }
                #endregion

                #region properties
                public int X =>
                    Resolution.X;
                public int Y =>
                    Resolution.Y;
                public int Width =>
                    Resolution.Width;
                public int Height =>
                    Resolution.Height;
                public int BitsPerPixel =>
                    Resolution.BitsPerPixel;
                public float RefreshRate =>
                    Resolution.RefreshRate;
                public bool IsPrimary
                {
                    get { return isPrimary; }
                    internal set
                    {
                        if (value && primary != null && primary != this)
                        {
                            primary.IsPrimary = false;
                        }

                        lock (screenLock)
                        {
                            isPrimary = value;
                            if (value)
                            {
                                primary = this;
                            }
                        }
                    }
                }
                public IResolution Resolution { get; private set; }
                #endregion

                #region methods
                public void ChangeResolution(int resolutionIndex)
                {
                    if (resolutionIndex >= resolutions.Length)
                        return;
                    var resolution = resolutions[resolutionIndex];

                    if (!resolution.Valid)
                        RestoreResolution();

                    if (resolution == this.Resolution)
                        return;

                    //effect.FadeOut();

                    if (changeResolution(this, resolution))
                    {
                        this.Resolution = resolution;
                    }
                    else
                    {
                        throw new System.Exception(string.Format("Device {0}: Failed to change resolution to {1}.",
                            this, resolution));
                    }

                    //effect.FadeIn();
                }
                public void RestoreResolution()
                {
                    if (original.Valid)
                    {
                        //effect.FadeOut();

                        if (restoreResolution(this))
                            Resolution = original;
                        else
                            throw new System.Exception(string.Format("Device {0}: Failed to restore resolution.", this));
                        //effect.FadeIn();
                    }
                }

                public bool Contains(float x, float y)
                {
                    return x >= Resolution.X &&
                         y >= Resolution.Y &&
                         x <= (Resolution.X + Resolution.Width) &&
                         y <= (Resolution.Y + Resolution.Height);
                }

                public void Dispose() { }
                public override string ToString()
                {
                    return string.Format("{0}: {1} ({2} modes available)", IsPrimary ? "Primary" : "Secondary",
                       Resolution, resolutions.Length);
                }
                #endregion

                #region static methods
                static bool changeResolution(IScreen screen, IResolution resolution)
                {
                    //Windows.Factory.UseFullscreenDesktop = true;
                    return true;
                }
                static bool restoreResolution(IScreen screen)
                {
                    //Windows.Factory.UseFullscreenDesktop = true;
                    return true;
                }
                #endregion

                #region interface
                public IResolution this[int index] => resolutions[index];
                public int Count => resolutions.Length;
                public IEnumerator<IResolution> GetEnumerator()
                {
                    for (int i = 0; i < resolutions.Length; i++)
                    {
                        yield return this[i];
                    }
                }
                IEnumerator IEnumerable.GetEnumerator()
                {
                    for (int i = 0; i < resolutions.Length; i++)
                    {
                        yield return this[i];
                    }
                }
                #endregion
            }
        }
    }
}
#endif