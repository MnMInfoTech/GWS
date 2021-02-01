/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System;

namespace MnM.GWS
{
    public sealed class FixedBrush : IFixedBrush
    {
        #region VARIABLES
        public bool Invert;
        readonly public int Width;
        readonly public int Height;
        readonly public int Length;
        ReadChoice choice;
        #endregion

        #region CONSTRUCTOR
        public FixedBrush(IPenContext context, IDrawParams Settings, int width, int height)
        {
            initialize(context, Settings, width, height, ref Width, ref Height, ref Length);
        }
        public unsafe FixedBrush(IReadable pen, IDrawParams Settings)
        {
            initialize(pen, Settings, pen.Width, pen.Height, ref Width, ref Height, ref Length);
        }
        unsafe void initialize(IPenContext context, IDrawParams Settings, int width, int height, ref int Width, ref int Height, ref int Length)
        {
            if(context == null)
            {
                PenData = new int[0];
                Width = Height = Length = 0;
                return;
            }
            Width = width;
            Height = height;
            Length = Width * Height;

            IReadable pen;
            if (context is IReadable)
                pen = (IReadable)context;
            else
                pen = context.ToPen(width, height);
            if (Settings != null)
                (pen as ISettingsReceiver)?.Receive(Settings);

            PenData = new int[Length];
            fixed (int* p = PenData)
                pen.CopyTo((IntPtr)p, Length, Width, 0, 0, new Rectangle(0, 0, Width, Height), 0);
            if (Settings != null)
                (pen as ISettingsReceiver)?.Receive(Settings, true);

        }
        #endregion

        #region PROPERTIES
        int ISize.Width => Width;
        int ISize.Height => Height;
        int ILength.Length => Length;
        public ReadChoice Choice
        {
            get => choice;
            set
            {
                choice = value;
                Invert = (choice & ReadChoice.InvertColor) == ReadChoice.InvertColor;
            }
        }
        public int[] PenData { get; private set; }
        unsafe IntPtr IPixels.Source
        {
            get
            {
                fixed (int* p = PenData)
                    return (IntPtr)p;
            }
        }
        #endregion

        #region READ PIXEL
        public int ReadPixel(int x, int y)
        {
            if (Length == 0)
                return 0;
            int i = x + y * Width;
            if (i >= Length)
                i = 0;
            var srcColor = PenData[i];
            if (Invert)
                srcColor ^= Colors.Inversion;
            return srcColor;
        }
        #endregion

        #region READ LINE
        public unsafe void ReadLine(int start, int end, int axis, bool horizontal, out int[] pixels, out int srcIndex, out int length)
        {
            if (start > end)
            {
                int temp = end;
                end = start;
                start = end;
            }
            length = end - start;
            if (start < 0)
            {
                length += start;
                start = 0;
            }
            if (horizontal && !Invert)
            {
                srcIndex = start + axis * Width;
                if (srcIndex >= Length)
                    srcIndex %= Length;
                pixels = PenData;
            }
            else
            {
                int srcCounter = horizontal ? 1 : Width;
                pixels = new int[length];
                int* dst;
                fixed (int* p = pixels)
                    dst = p;
                int* src;
                fixed (int* p = PenData)
                    src = p;
                srcIndex = start + axis * Width;
                int srcColor;

                for (int i = 0; i < length; i++)
                {
                    srcColor = src[srcIndex];
                    if (Invert)
                        srcColor ^= Colors.Inversion;
                    dst[i] = srcColor;
                    srcIndex += srcCounter;
                }
                srcIndex = 0;
            }
        }
        #endregion

        #region COPY TO
        public unsafe IRectangle CopyTo(IntPtr destination, int dstLen, int dstW, int dstX, int dstY, IRectangle copyArea,
            Command command)
        {
            int copyX = copyArea.X;
            int copyY = copyArea.Y;
            int copyW = copyArea.Width;
            int copyH = copyArea.Height;

            int* src;
            fixed (int* p = PenData)
                src = p;
            var dst = (int*)destination;
            return Blocks.CopyBlock(0, 0, copyW, copyH, Length, copyW, copyH, dstX, dstY, dstW, dstLen,
                      (srcIndex, dstIndex, w, x, y, cmd) =>
                      Blocks.Copy(src, srcIndex, dst, dstIndex, w, cmd, null, true), command);
        }
        #endregion

        #region DISPOSE
        public void Dispose()
        {
            PenData = null;
        }
        #endregion
    }
}
#endif
