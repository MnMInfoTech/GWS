/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
/*
 * We have used some part of the structs definitions below from the...
 * OpenTK - https://github.com/opentk/opentk
   *Copyright (c) 2006-2019 Stefanos Apostolopoulos for the Open Toolkit project.
   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
// Author: Manan Adhvaryu.

#if Window && SDL
using System;
using System.Runtime.InteropServices;

namespace MnM.GWS.SDL
{
    struct SdlResolution : IResolution
    {
        public readonly bool Valid;

        #region constructors
        internal SdlResolution(float x, float y, float width, float height, uint format, float refreshRate)
        {
            Valid = true;
            var bitsPerPixel = translateFormat(format);
            // Refresh rate may be zero, since this information may not be available on some platforms.
            if (width <= 0)
                throw new ArgumentOutOfRangeException("width", "Must be greater than zero.");
            if (height <= 0)
                throw new ArgumentOutOfRangeException("height", "Must be greater than zero.");
            if (bitsPerPixel <= 0)
                throw new ArgumentOutOfRangeException("bitsPerPixel", "Must be greater than zero.");
            if (refreshRate < 0)
                throw new ArgumentOutOfRangeException("refreshRate", "Must be greater than, or equal to zero.");

            Bounds = new Rectangle((int)x, (int)y, (int)width, (int)height);
            Format = format;
            BitsPerPixel = bitsPerPixel;
            RefreshRate = refreshRate;
        }
        #endregion

        #region properties
        public Rectangle Bounds { get; private set; }
        public int X => Bounds.X;
        public int Y => Bounds.Y;
        public int Width => Bounds.Width;
        public int Height => Bounds.Height;
        public int BitsPerPixel { get; private set; }
        public float RefreshRate { get; private set; }
        public uint Format { get; private set; }
        #endregion

        #region equality
        /// <summary>Determines whether the specified resolutions are equal.</summary>
        /// <param name="obj">The System.Object to check against.</param>
        /// <returns>True if the System.Object is an equal DisplayResolution; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (this.GetType() == obj.GetType())
            {
                SdlResolution res = (SdlResolution)obj;
                return
                    Width == res.Width &&
                    Height == res.Height &&
                    BitsPerPixel == res.BitsPerPixel &&
                    RefreshRate == res.RefreshRate;
            }

            return false;
        }

        /// <summary>Returns a unique hash representing this resolution.</summary>
        /// <returns>A System.Int32 that may serve as a hash code for this resolution.</returns>
        public override int GetHashCode()
        {
#pragma warning disable 612, 618
            return Bounds.GetHashCode() ^ BitsPerPixel ^ RefreshRate.GetHashCode();
#pragma warning restore 612, 618
        }

        /// <summary>
        /// Compares two instances for equality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left equals right; false otherwise.</returns>
        public static bool operator ==(SdlResolution left, SdlResolution right)
        {
            if (((object)left) == null && ((object)right) == null)
            {
                return true;
            }
            else if ((((object)left) == null && ((object)right) != null) ||
                     (((object)left) != null && ((object)right) == null))
            {
                return false;
            }
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two instances for inequality.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns>True, if left does not equal right; false otherwise.</returns>
        public static bool operator !=(SdlResolution left, SdlResolution right)
        {
            return !(left == right);
        }
        #endregion

        bool IResolution.Valid => Valid;

        static int translateFormat(uint format)
        {
            int bpp;
            uint a, r, g, b;
            SdlAPI.PixelFormatToMasks(format, out bpp, out r, out g, out b, out a);
            return bpp;
        }
        public override string ToString()
        {
            return String.Format("W:{0}, H:{1}, BPP: {2} @{3}Hz", Width, Height, BitsPerPixel, RefreshRate);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SdlSurfaceInfo
    {
#region VARIABLES
        public uint Flags;
        public IntPtr Format; //PixelFormat
        public int Width;
        public int Height;
        public int Pitch;
        public volatile IntPtr Pixels;
        public IntPtr UserData;
        public int Locked;
        public IntPtr LockData;
        public IntPtr ClipRect;
        public IntPtr Map;
        public int Refcount;
#endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SdlSurfacePixelFormat
    {
        public uint Format;
        public IntPtr Pallette;
        public uint BitsPerPixel;
        public uint BytesPerPixel;
        public uint Rmask;
        public uint Gmask;
        public uint BMask;
        public uint Amask;
    }
    struct SysWMInfo
    {
        public Version Version;
        public SysWMType Subsystem;
        public SysInfo Info;

        [StructLayout(LayoutKind.Explicit)]
        public struct SysInfo
        {
            [FieldOffset(0)]
            public WindowsInfo Windows;
            [FieldOffset(0)]
            public X11Info X11;
            [FieldOffset(0)]
            public WaylandInfo Wayland;
            [FieldOffset(0)]
            public DirectFBInfo DirectFB;
            [FieldOffset(0)]
            public CocoaInfo Cocoa;
            [FieldOffset(0)]
            public UIKitInfo UIKit;

            public struct WindowsInfo
            {
                public IntPtr Window;
            }

            public struct X11Info
            {
                public IntPtr Display;
                public IntPtr Window;
            }

            public struct WaylandInfo
            {
                public IntPtr Display;
                public IntPtr Surface;
                public IntPtr ShellSurface;
            }

            public struct DirectFBInfo
            {
                public IntPtr Dfb;
                public IntPtr Window;
                public IntPtr Surface;
            }

            public struct CocoaInfo
            {
                public IntPtr Window;
            }

            public struct UIKitInfo
            {
                public IntPtr Window;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    unsafe struct RendererInfo
    {
        public IntPtr name; // const char*
        public uint flags;
        public uint num_texture_formats;
        public fixed uint texture_formats[16];
        public int max_texture_width;
        public int max_texture_height;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MessageBoxButtonData
    {
        public MessageBoxButtonFlags flags;
        public int buttonid;
        public string text; /* The UTF-8 button text */

    }

    [StructLayout(LayoutKind.Sequential)]
    struct MessageBoxColour
    {
        public byte r, g, b;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MessageBoxColourScheme
    {
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = (int)MessageBoxColourType.OfColourMax)]
        [Obsolete]
        public MessageBoxColour[] colours;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct MessageBoxData
    {
        public MessageBoxFlags flags;
        public IntPtr window;               /* Parent window, can be NULL */
        public string title;                /* UTF-8 title */
        public string message;              /* UTF-8 message text */
        public int numbuttons;
        public MessageBoxButtonData[] buttons;
        public MessageBoxColourScheme? colourScheme;  /* Can be NULL to use system settings */
    }

    [StructLayout(LayoutKind.Sequential)]
    struct InternalMessageBoxButtonData
    {
        public MessageBoxButtonFlags flags;
        public int buttonid;
        public IntPtr text; /* The UTF-8 button text */
    }

    [StructLayout(LayoutKind.Sequential)]
    struct InternalMessageBoxData
    {
        public MessageBoxFlags flags;
        public IntPtr window;               /* Parent window, can be NULL */
        public IntPtr title;                /* UTF-8 title */
        public IntPtr message;              /* UTF-8 message text */
        public int numbuttons;
        public IntPtr buttons;
        public IntPtr colourScheme;          /* Can be NULL to use system settings */
    }

    struct Version
    {
        public byte Major;
        public byte Minor;
        public byte Patch;
        public int Number
        {
            get { return 1000 * Major + 100 * Minor + Patch; }
        }
    }

    struct DisplayMode
    {
        public uint Format;
        public int Width;
        public int Height;
        public int RefreshRate;
        public IntPtr DriverData;
    }
}
#endif
