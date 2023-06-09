/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public sealed partial class Block : IImageSource, IDisposable, IExResizable
    {
        #region VARIABLES
        int[] Data;
        static ushort id;
        int OriginalWidth, OriginalHeight;
        #endregion

        #region CONSTRUCTORS
        public Block(int width, int height)
        {
            Width = OriginalWidth = width;
            Height = OriginalHeight = height;
            Data = new int[width * height];
            ID = "Block" + (++id);
        }
        public unsafe Block(int[] data, int w, int h, bool makeCopy = false) :
            this(w, h)
        {
            if (data == null)
                return;
            if (makeCopy)
                Array.Copy(data, Data, data.Length);
            else
                Data = data;
        }
        public unsafe Block(int w, int h, byte[] data, bool switchRBChannel = false) :
            this(w, h)
        {
            if (data == null)
                return;
            fixed (byte* src = data)
            {
                CopyBytes(src, switchRBChannel);
            }
        }
        public unsafe Block(IntPtr data, int w, int h, bool switchRBChannel = false) :
            this(w, h)
        {
            CopyBytes((byte*)data, switchRBChannel);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe void CopyBytes(byte* src, bool switchRBChannel = false)
        {
            if (src == null)
                return;
            fixed (int* d = Data)
            {
                byte* dst = (byte*)d;
                var len = Width * Height * 4;
                if (switchRBChannel)
                {
                    for (int i = 0; i < len; i += 4)
                    {
                        dst[i] = src[i + 2];
                        dst[i + 1] = src[i + 1];
                        dst[i + 2] = src[i];
                        dst[i + 3] = src[i + 3];
                    }
                }
                else
                {
                    for (int i = 0; i < len; i += 1)
                    {
                        dst[i] = src[i];
                    }
                }
            }
        }
        #endregion

        #region PROPERTIES
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string ID { get; private set; }
        public bool Valid => Width > 0 && Height > 0;
        unsafe IntPtr ISource<IntPtr>.Source
        {
            get
            {
                fixed (int* p = Data)
                    return (IntPtr)p;
            }
        }
        int IPoint.X => 0;
        int IPoint.Y => 0;
        bool IOriginCompatible.IsOriginBased => true;
        #endregion

        #region RESIZE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object IExResizable.Resize(int w, int h, out bool sucess, ResizeCommand resizeCommand)
        {
            sucess = false;
            if (
              (w == Width && h == Height) ||
              (w == 0 && h == 0))
                return this;

            bool NewInstance = (resizeCommand & ResizeCommand.NewInstance) == ResizeCommand.NewInstance;
            bool SizeOnlyToFit = (resizeCommand & ResizeCommand.SizeOnlyToFit) == ResizeCommand.SizeOnlyToFit;
            bool AutoReSizeContent = !SizeOnlyToFit && (resizeCommand & ResizeCommand.AutoReSizeContent) == ResizeCommand.AutoReSizeContent;
            bool NotLessThanOriginal = !SizeOnlyToFit && (resizeCommand & ResizeCommand.NotLessThanOriginal) == ResizeCommand.NotLessThanOriginal;

            if (SizeOnlyToFit && Width > w && Height > h)
                return this;

            if (SizeOnlyToFit)
            {
                if (w < Width)
                    w = Width;
                if (h < Height)
                    h = Height;
            }
            if (NotLessThanOriginal)
            {
                if (w < OriginalWidth)
                    w = OriginalWidth;
                if (h < OriginalHeight)
                    h = OriginalHeight;
            }

            if (AutoReSizeContent
            )
            {
                Interpolation interpolation = 0;
                if ((resizeCommand & ResizeCommand.AutoReSizeContentBicubic) == ResizeCommand.AutoReSizeContentBicubic)
                {
                    interpolation = Interpolation.Bicubic;
                }
                else if ((resizeCommand & ResizeCommand.AutoReSizeContentBilinear) == ResizeCommand.AutoReSizeContentBilinear)
                {
                    interpolation = Interpolation.Bilinear;
                }
                var t = Factory.ImageProcessor.RotateAndScale
                (
                    ((ISource<IntPtr>)this).Source, Width, Height, 0, 0, Width, Height, interpolation,
                    SizeOnlyToFit, null, new Scale(Width, Height, w, h)
                ).Result;
                w = t.Item1.Width;
                h = t.Item1.Height;

                if (NewInstance)
                    return new Block(t.Item2, w, h);

                Width = w;
                Height = h;
                var Length = Width * Height;
                Data = new int[Length];
                Blocks.Copy(t.Item2, 0, ((ISource<IntPtr>)this).Source, 0, Length);
            }
            else
            {
                if (NewInstance)
                {
                    var data = Data.ResizedData(w, h, Width, Height);
                    return new Block(data, w, h);
                }
                Blocks.ResizedData(ref Data, w, h, Width, Height);
            }
            sucess = true;
            return this;
        }
        #endregion

        #region GET ORIGIN-BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion() => this;
        #endregion

        #region DISPOSE
        public void Dispose()
        {
            Data = null;
        }
        #endregion
    }
}
