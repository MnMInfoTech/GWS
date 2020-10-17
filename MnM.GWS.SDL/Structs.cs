/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

#if Window
using System;

/* Unmerged change from project 'GWSDemoDesktop'
Before:
using System.Runtime.InteropServices;
using static MnM.GWS.Application;
After:
using System.Runtime.InteropServices;

using MnM.GWS;

using static MnM.GWS.Application;
*/
using System.Runtime.InteropServices;

/*
 * We have used some part of the structs definitions below from the...
 
 1.
 * OpenTK - https://github.com/opentk/opentk
   *Copyright (c) 2006-2019 Stefanos Apostolopoulos for the Open Toolkit project.
   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
    The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 2.
 * AND - C# Wrapper for SDL2 - https://github.com/flibitijibibo/SDL2-CS/blob/master/src/SDL2.cs
 *
 * Copyright (c) 2013-2020 Ethan Lee.
 *
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages arising from
 * the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. If you use this software in a
 * product, an acknowledgment in the product documentation would be
 * appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not be
 * misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source distribution.
 *
 * Ethan "flibitijibibo" Lee <flibitijibibo@flibitijibibo.com>
 *
 */
namespace MnM
{
    namespace GWS
    {
        [StructLayout(LayoutKind.Sequential)]
        unsafe struct SurfaceInfo
        {
            #region VARIABLES
            public uint Flags;
            public IntPtr Format; //PixelFormat
            public int Width;
            public int Height;
            public int Pitch;
            public IntPtr Pixels;
            public IntPtr UserData;
            public int Locked;
            public IntPtr LockData;
            public IntPtr ClipRect;
            public IntPtr Map;
            public int Refcount;
            #endregion
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
        struct MessageBoxColor
        {
            public byte r, g, b;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MessageBoxColorScheme
        {
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = (int)MessageBoxColorType.OfColorMax)]
            public MessageBoxColor[] colors;
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
            public MessageBoxColorScheme? colorScheme;  /* Can be NULL to use system settings */
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
            public IntPtr colorScheme;          /* Can be NULL to use system settings */
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
}
#endif
