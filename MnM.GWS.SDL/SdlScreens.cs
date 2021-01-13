/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using MnM.GWS;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static MnM.GWS.Application;

namespace MnM.GWS
{
#if Window
#if AllHidden
    partial class SdlFactory
    {
#else
    public
#endif
        class SdlScreens : IScreens
        {
    #region variables
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
                    SdlFactory.GetDisplayBounds(d, out Rectangle rc);
                    bounds = new Rectangle(rc.X, rc.Y, rc.Width, rc.Height);

                    SdlFactory.GetCurrentDisplayMode(d, out DisplayMode dm);

                    int total = GetNumDisplayModes(d);
                    var list = new IResolution[total];

                    for (int m = 0; m < total; m++)
                    {
                        GetDisplayMode(d, m, out DisplayMode sdm);
                        list[m] = new SdlResolution(bounds.X, bounds.Y, sdm.Width, sdm.Height, sdm.Format, sdm.RefreshRate);
                    }

                    var current_resolution = new SdlResolution(bounds.X, bounds.Y, dm.Width, dm.Height, dm.Format, dm.RefreshRate);
                    var device = new SdlScreen(current_resolution, d == 0, list, bounds, d);

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
                        if (display.Bounds.Contains(x, y))
                        {
                            return display;
                        }
                    }
                }
                return null;
            }

            static int translateFormat(uint format)
            {
                int bpp;
                uint a, r, g, b;
                PixelFormatToMasks(format, out bpp, out r, out g, out b, out a);
                return bpp;
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
                internal SdlScreen(IResolution currentResolution, bool primary, IResolution[] resolutions, Rectangle bounds, object id)
                {
                    // Todo: Consolidate current resolution with bounds? Can they fall out of sync right now?
                    this.Resolution = currentResolution;
                    this.original = currentResolution;
                    IsPrimary = primary;
                    this.resolutions = (resolutions);
#pragma warning disable 612, 618
                    this.Bounds = bounds.Width ==0|| bounds.Height==0 ? currentResolution.Bounds : bounds;
#pragma warning restore 612, 618
                    this.monitorID = id;
                }
    #endregion

    #region properties
                public Rectangle Bounds { get; internal set; }
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
                public void Dispose() { }
                public override string ToString()
                {
                    return string.Format("{0}: {1} ({2} modes available)", IsPrimary ? "Primary" : "Secondary",
                        Bounds.ToString(), resolutions.Length);
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
                public override string ToString()
                {
                    return String.Format("{0}x{1}@{2}Hz", Bounds, BitsPerPixel, RefreshRate);
                }
            }

    #region sdl binding
            [DllImport(libSDL, EntryPoint = "SDL_PixelFormatEnumToMasks", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            static extern bool PixelFormatToMasks(uint format, out int bpp, out uint rmask, out uint gmask, out uint bmask, out uint amask);

            [DllImport(libSDL, EntryPoint = "SDL_GetNumVideoDisplays", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            static extern int GetNumVideoDisplays();

            [DllImport(libSDL, EntryPoint = "SDL_GetNumDisplayModes", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            static extern int GetNumDisplayModes(int displayIndex);

            [DllImport(libSDL, EntryPoint = "SDL_GetDisplayMode", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
            static extern int GetDisplayMode(int displayIndex, int modeIndex, out DisplayMode mode);
    #endregion
        }
#if AllHidden
    }
#endif
#endif
}