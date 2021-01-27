/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public abstract class _Image : IImage
    {
        #region VARIABLES
        /// <summary>
        /// color pixels of this object.
        /// </summary>
        protected volatile int[] Data;

        /// <summary>
        /// Width of this object.
        /// </summary>
        protected int width;

        /// <summary>
        /// Height of this object
        /// </summary>
        protected int height;

        /// <summary>
        /// Length of one dimensional memory block this object represents.
        /// </summary>
        protected int length;

        /// <summary>
        /// Indicated if this object is disposed or not.
        /// </summary>
        protected bool isDisposed;

        /// <summary>
        /// ID of this object.
        /// </summary>
        protected int id;

        /// <summary>
        /// Indicates if this object is currently being resized or not.
        /// </summary>
        protected volatile bool IsResizing;

        /// <summary>
        /// Indicates if this canvas supports background buffer.
        /// </summary>
        protected volatile bool supportsBackBuffer;

        /// <summary>
        /// Stores indices of written pixels to prevent over-writing.
        /// </summary>
        protected readonly HashSet<int> DrawnIndices = new HashSet<int>();

        protected readonly int originalWidth, originalHeight;

        readonly string typeName;

        /// <summary>
        /// Represents byte value of 0.
        /// </summary>
        protected const byte o = 0;

        protected const byte l = 1;
        #endregion

        #region CONSTRUCTORS
        protected _Image(int width, int height)
        {
            this.width = width;
            this.height = height;
            length = width * height;
            Data = new int[length];
            originalHeight = height;
            originalWidth = width;
        }
        protected unsafe _Image(int[] data, int w, int h, bool makeCopy = false) :
            this(w, h, null)
        {
            if (data == null)
                return;
            if (makeCopy)
                Array.Copy(data, 0, Data, 0, length);
            else
                Data = data;
        }
        protected unsafe _Image(int w, int h, byte[] data) :
            this(w / 4, h)
        {
            if (data == null)
                return;
            fixed (byte* src = data)
            {
                fixed (int* dst = Data)
                    Blocks.Copy((int*)src, 0, dst, 0, length);
            }
        }
        protected unsafe _Image(IntPtr data, int w, int h) :
            this(w, h)
        {
            int* src = (int*)data;
            fixed (int* dst = Data)
                Blocks.Copy(src, 0, dst, 0, length);
        }
        #endregion

        #region PROPERTIES
        public int ID
        {
            get
            {
                if (id == 0)
                    id = this.NewID();
                return id;
            }
        }
        public bool IsDisposed => isDisposed;
        public int Width => width;
        public int Height => height;
        public int Length => length;
        protected virtual unsafe int* Screen(bool DirectScreen)
        {
            fixed (int* p = Data)
                return p;
        }
        public virtual string TypeName => "Image";
        public string Name => TypeName + ID;

        protected abstract unsafe int* Pen { get; }
#if Advanced
        public abstract bool Clipped { get; }
        public abstract IRectangle ClipRectangle { get; set; }
        public abstract IEventPusher ActiveObject { get; }
        bool IImageData.SupportBackgroundBuffer =>
            supportsBackBuffer;
#endif
        #endregion

        #region RENDER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract bool Render(IRenderable Renderable, ISettings Settings = null, bool? suspendUpdate = null);
        #endregion
#if Advanced
        #region GET DATA
        public virtual void GetData(out int[] Pixels, out byte[] Alphas, bool BackgroundBuffer = false)
        {
            Pixels = Data;
            Alphas = null;
        }
        #endregion
#endif
        #region WRITE PIXEL
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe virtual void WritePixel(int val, int axis, bool horizontal, int srcColor, float? Alpha, Command Command, INotifier RecentlyDrawn)
        {
            bool Opaque = (Command & Command.Opaque) == Command.Opaque;
            if (IsResizing || isDisposed || srcColor == 0 && !Opaque)
                return;

            int i;
            int dstX = horizontal ? val : axis;
            int dstY = horizontal ? axis : val;
        
            int ShapeID = RecentlyDrawn.ShapeID;
            dstX += RecentlyDrawn.DstX;
            dstY += RecentlyDrawn.DstY;

            if (dstX < 0 || dstY < 0 || dstX >= width || dstY >= height)
                return;

            bool CalculateOnly = (Command & Command.CalculateOnly) == (Command.CalculateOnly);

            if (CalculateOnly)
                goto Notify;

            bool Backdrop = (Command & Command.Backdrop) == (Command.Backdrop);
            bool Distinct = (Command & Command.Distinct) == (Command.Distinct);
            bool DirectScreen = (Command & Command.Screen) == Command.Screen;

            i = dstX + dstY * width;

            if (Distinct)
            {
                if (DrawnIndices.Contains(i))
                    return;
                DrawnIndices.Add(i);
            }
            int* dst = Screen(DirectScreen);

            int dstColor = dst[i];
            if (Backdrop && dstColor != 0)
                return;

            float delta = Alpha ?? Colors.Alphas[(byte)((srcColor >> Colors.AShift) & 0xFF)];
            byte alpha = (byte)(delta * 255);

            if (alpha == 0)
                return;

            if (alpha == 255)
                goto AssignColor;

            if (dstColor == 0 && Pen != null)
            {
                dstColor = Pen[i];
            }
            uint C1 = (uint)dstColor;
            uint C2 = (uint)srcColor;
            uint invAlpha = (uint)(255 - alpha);
            uint RB = ((invAlpha * (C1 & Colors.RBMASK)) + (alpha * (C2 & Colors.RBMASK))) >> 8;
            uint AG = (invAlpha * ((C1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((C2 & Colors.GMASK) >> 8)));

            srcColor = (int)((RB & Colors.RBMASK) | (AG & Colors.AGMASK));

        AssignColor:
            dst[i] = srcColor;

        Notify:
            RecentlyDrawn.Notify(dstX, dstY, dstX + 1, dstY + 1);
        }
        #endregion

        #region WRITE LINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe virtual void WriteLine(int* src, int srcIndex, int srcW, int copyLength, bool horizontal,
            int dstX, int dstY, float? Alpha, byte* srcAlphas, Command Command, INotifier RecentlyDrawn)
        {
            if (IsResizing || isDisposed)
                return;
            int ShapeID = RecentlyDrawn.ShapeID;
            dstX += RecentlyDrawn.DstX;
            dstY += RecentlyDrawn.DstY;

            #region CORRECT LINE
            if (horizontal)
            {
                if (dstY < 0 || dstY >= height)
                    return;
                if (dstX < 0)
                {
                    copyLength += dstX;
                    dstX = 0;
                }
                if (copyLength < 0)
                    return;
                if (dstX + copyLength > width)
                    copyLength = width - dstX;
            }
            else
            {
                if (dstX < 0 || dstX >= Width)
                    return;

                if (dstY < 0)
                {
                    copyLength += dstY;
                    dstY = 0;
                }
                if (copyLength < 0)
                    return;
                if (dstY + copyLength > height)
                    copyLength = height - dstY;
            }
            #endregion

            bool CalculateOnly = (Command & Command.CalculateOnly) == (Command.CalculateOnly);
            bool DirectOnScreen = (Command & Command.Screen) == (Command.Screen);

            if (CalculateOnly)
                goto Notify;

            #region VARAIBLE INITIALIZATION
            int dstIndex = dstX + dstY * width;
            int dplus = horizontal ? 1 : width;
            int splus = horizontal || srcW == copyLength ? 1 : srcW;
            int last = dstIndex + dplus * copyLength;
            int j = srcIndex;

            int x = dstX;
            int y = dstY;
            int ix = horizontal ? 1 : 0;
            int iy = horizontal ? 0 : 1;

            int dstColor, srcColor;
            var NoBlend = Alpha == null;
            byte alpha = !NoBlend ? (byte)(Alpha.Value * 255) : o;
            bool NegligibleAlpha = !NoBlend && alpha < 2;

            bool Backdrop = (Command & Command.Backdrop) == (Command.Backdrop);
            bool Opaque = (Command & Command.Opaque) == (Command.Opaque);
            bool InvertColor = (Command & Command.InvertColor) == (Command.InvertColor);
            int* dst = Screen(DirectOnScreen);
            int* pen = this.Pen;
            bool HasBkg = pen != null;
            #endregion

            #region LOOP
            for (int i = dstIndex; i < last; i += dplus, j += splus, x += ix, y += iy)
            {
                if (i >= length) break;

                dstColor = dst[i];
                srcColor = src[j];

                if ((!Opaque && srcColor == 0) ||
                    (srcColor == 0 && dstColor == 0) ||
                    (!NoBlend && alpha < 2))
                    continue;

                if (Backdrop && dstColor != 0)
                {
                    continue;
                }

                if (alpha == 255 || NoBlend)
                {
                    goto AssignColor;
                }

                if (dstColor == 0 && HasBkg)
                    dstColor = pen[i];

                uint C1 = (uint)dstColor;
                uint C2 = (uint)srcColor;
                uint invAlpha = (uint)(255 - alpha);
                uint RB = ((invAlpha * (C1 & Colors.RBMASK)) + (alpha * (C2 & Colors.RBMASK))) >> 8;
                uint AG = (invAlpha * ((C1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((C2 & Colors.GMASK) >> 8)));

                srcColor = (int)((RB & Colors.RBMASK) | (AG & Colors.AGMASK));

            AssignColor:
                dst[i] = srcColor;
            }
        #endregion

        Notify:
            #region NOTIFY
            var r = dstX + (horizontal ? copyLength : 1);
            var b = dstY + (horizontal ? 1 : copyLength);
            RecentlyDrawn.Notify(dstX, dstY, r, b);
            #endregion
        }
        #endregion

        #region COPY TO
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe virtual IRectangle CopyTo(IntPtr destination, int dstLen,
            int dstW, int dstX, int dstY, IRectangle copyArea, Command Command = Command.None)
        {
            if (IsResizing || isDisposed)
                return Rectangle.Empty;

            #region COMMAND PARSING
            bool SkipPenBackground = (Command & Command.SkipPenBackground) == Command.SkipPenBackground;
            bool InvertColor = (Command & Command.InvertColor) == Command.InvertColor;
            bool Opaque = (Command & Command.Opaque) == Command.Opaque;
            bool DirectScreen = (Command & Command.Screen) == Command.Screen;
            #endregion

            #region VARIABLE INITIALIZATION
            int copyX = copyArea.X;
            int copyY = copyArea.Y;
            int copyW = copyArea.Width;
            int copyH = copyArea.Height;
            int* texture = (int*)destination;
            int srcColor, dstColor;
            int* data = Screen(DirectScreen);
            int srcIndex, dstIndex;
            int* pen = this.Pen;
            bool hasBkg = pen != null;
            int penColor = InvertColor ? 0 ^ Colors.Inversion : 0;
            #endregion

            #region CORRECT COPY AND PASTE PARAMETERS
            Blocks.CorrectRegion(ref copyX, ref copyY, ref copyW, ref copyH, width, height,
                ref dstX, ref dstY, dstW, dstLen, out srcIndex, out dstIndex);

            int x = copyX;
            int y = copyY;
            int r = copyX + copyW;
            int position = 0;
            #endregion

            #region SIGLE BUFFER DRAWING
            for (position = 0; position < copyH; position++, srcIndex += width, dstIndex += dstW)
            {
                int dIndex = dstIndex;
                int sIndex = srcIndex;

                for (int j = dIndex; j < dIndex + copyW; j++, sIndex++)
                {
                    if (!SkipPenBackground)
                    {
                        if (hasBkg)
                            penColor = pen[j];
                        texture[j] = penColor;
                    }

                    dstColor = texture[j];
                    srcColor = data[sIndex];
                    if (srcColor == 0 && !Opaque)
                        continue;
                    if (InvertColor)
                        srcColor ^= Colors.Inversion;
                    texture[j] = srcColor;
                }
            }
            #endregion

            return new Rectangle(copyX, copyY, copyW, position);
        }
        #endregion

        #region CONSOLIDATE
        public unsafe virtual IRectangle Consolidate(IntPtr destination, int dstLen,
            int dstW, int dstX, int dstY, IRectangle copyArea, IImageData BackgroundBuffer, Command Command = Command.None, IntPtr? Pen = null)
        {
            if (IsResizing || isDisposed)
                return Rectangle.Empty;

            #region COMMAND PARSING
            bool SkipBackgroundDraw = BackgroundBuffer == null;
            bool SkipPenBackground = (Command & Command.SkipPenBackground) == Command.SkipPenBackground;
            bool InvertColor = (Command & Command.InvertColor) == Command.InvertColor;
            bool Opaque = (Command & Command.Opaque) == Command.Opaque;
            bool DirectScreen = (Command & Command.Screen) == Command.Screen;
            #endregion

            #region VARIABLE INITIALIZATION
            int copyX = copyArea.X;
            int copyY = copyArea.Y;
            int copyW = copyArea.Width;
            int copyH = copyArea.Height;
            int ShapeID = 0;
            if (copyArea is IShapeID)
                ShapeID = ((IShapeID)copyArea).ShapeID;
            int* texture = (int*)destination;
            int srcColor, dstColor;
            int srcIndex, dstIndex;
            int* pen = null;
            if (Pen == null)
                pen = this.Pen;
            else
                pen = (int*)Pen.Value;

            bool hasBkg = pen != null;
            int penColor = InvertColor ? 0 ^ Colors.Inversion : 0;
            #endregion

            #region CORRECT COPY AND PASTE PARAMETERS
            Blocks.CorrectRegion(ref copyX, ref copyY, ref copyW, ref copyH, width, height,
                ref dstX, ref dstY, dstW, dstLen, out srcIndex, out dstIndex);

            int x = copyX;
            int y = copyY;
            int r = copyX + copyW;
            int position = 0;
            int* data = Screen(DirectScreen);
            #endregion

            #region SIGLE BUFFER DRAWING
            for (position = 0; position < copyH; position++, srcIndex += width, dstIndex += dstW)
            {
                int dIndex = dstIndex;
                int sIndex = srcIndex;

                for (int j = dIndex; j < dIndex + copyW; j++, sIndex++)
                {
                    if (!SkipPenBackground)
                    {
                        if (hasBkg)
                            penColor = pen[j];
                        texture[j] = penColor;
                    }

                    dstColor = texture[j];
                    srcColor = data[sIndex];
                    if (srcColor == 0 && !Opaque)
                        continue;
                    if (InvertColor)
                        srcColor ^= Colors.Inversion;
                    texture[j] = srcColor;
                }
            }
            #endregion

            return new Rectangle(copyX, copyY, copyW, position);
        }
        #endregion

        #region CLEAR
        public abstract IRectangle Clear(int clearX, int clearY, int clearW, int clearH, Command command = Command.None);
        #endregion

        #region COPY FROM
        public abstract IRectangle CopyFrom(IntPtr source, int srcW, int srcH, int dstX, int dstY, IRectangle copyArea, Command Command, IntPtr alphaBytes = default);
        #endregion

        #region RESIZE
        public abstract void Resize(int? newWidth = null, int? newHeight = null);
        #endregion

        #region CLONE
        public abstract object Clone();
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            isDisposed = true;
            Data = null;
        }
        #endregion
    }
}
