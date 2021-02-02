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
    public abstract partial class _Image : IImage
    {
        #region VARIABLES
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
        /// Stores indices of written pixels to prevent over-writing.
        /// </summary>
        protected readonly HashSet<int> DrawnIndices = new HashSet<int>();

        /// <summary>
        /// Original width when this object is created.
        /// </summary>
        protected readonly int originalWidth;

        /// <summary>
        /// Original height when this object is created.
        /// </summary>
        protected readonly int originalHeight;

        /// <summary>
        /// Represents byte value of 0.
        /// </summary>
        protected const byte o = 0;
        #endregion

        #region CONSTRUCTORS
        protected _Image(int width, int height)
        {
            this.width = width;
            this.height = height;
            length = width * height;
            originalHeight = height;
            originalWidth = width;
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
        protected abstract unsafe int* Screen(bool DirectScreen);
        public virtual string TypeName => "Image";
        public string Name => TypeName + ID;
        public virtual bool Inaccessible => IsResizing || isDisposed;

        protected abstract unsafe int* Pen { get; }
        public IPoint CopyPoint { get; set; }
        public ISize CopySize { get; set; }
        #endregion

        #region RENDER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract bool Render(IRenderable Renderable, ISettings Settings = null, bool? suspendUpdate = null);
        #endregion

        #region DRAW
        public virtual bool Draw(IWritable buffer, ISettings Settings)
        {
            var rc = this.CompitibleRc(CopyPoint?.X, CopyPoint?.Y, CopySize?.Width, CopySize?.Height);
            buffer.DrawImage(this, Settings.Boundary.DstX, Settings.Boundary.DstY, rc.X, rc.Y, rc.Width, rc.Height, Settings.Command);
            return true;
        }
        #endregion

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
        public abstract IRectangle WriteBlock(IntPtr source, int srcW, int srcH, int dstX, int dstY, IRectangle copyArea, Command Command, IntPtr alphaBytes = default);
        #endregion

        #region RESIZE
        public abstract void Resize(int? newWidth = null, int? newHeight = null);
        #endregion

        #region ROTATE 
        /// <summary>
        /// Source: https://www.drdobbs.com/architecture-and-design/fast-bitmap-rotation-and-scaling/184416337
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="antiAliased"></param>
        public unsafe Size RotateAndScale(out IntPtr result, Rotation rotation, bool antiAliased, float scale = 1f)
        {
            Size size = new Size(width, height);
            if (scale <= 0) scale = 1;
            int dstW = width;
            int dstH = height;
            int srcW = width;
            int srcH = height;
            int srcLen = length;
            uint White = Colors.UWhite;
            int* dst;
            int* Data;
            Command command = 0;

            fixed (int* d = new int[dstW * dstH])
                dst = d;

            fixed (int* p = new int[dstW * dstH])
                Data = p;

            if (this is IContainer)
                command = Command.Screen;

            CopyTo((IntPtr)Data, srcLen, srcW, 0, 0, new Rectangle(0, 0, dstW, dstH), command);

            if (!rotation && scale == 1)
            {
                Blocks.Copy(Data, 0, dst, 0, srcLen);
                result = (IntPtr)dst;
                return size;
            }

            int srcCx = srcW / 2;
            int srcCy = srcH / 2;

            int dstCx = dstW / 2;
            int dstCy = dstH / 2;

            bool intCalculation = !antiAliased;

            float dstXf = 0;
            float dstYf = 0;
            int dstXi = 0;
            int dstYi = 0;
            float x3 = 0, y3 = 0;
            int x0 = 0, y0 = 0, xi = 0, yi = 0;
            int color = 0;

            int Sini = rotation.Sini;
            int Cosi = rotation.Cosi;

            float Sin = rotation.Sin;
            float Cos = rotation.Cos;

            if (scale != 1)
            {
                Sin *= 1f / scale;
                Cos *= 1f / scale;

                Sini = (Sin * Angles.Big).Round();
                Cosi = (Cos * Angles.Big).Round();
            }

            if (intCalculation)
            {
                dstXi = -(dstCx * Cosi + dstCy * Sini);
                dstYi = -(dstCx * -Sini + dstCy * Cosi);
            }
            else
            {
                dstXf = srcCx - (dstCx * Cos + dstCy * Sin);
                dstYf = srcCy - (dstCx * -Sin + dstCy * Cos);
            }

            for (int j = 0; j < dstH; j++)
            {
                if (intCalculation)
                {
                    xi = dstXi;
                    yi = dstYi;
                }
                else
                {
                    x3 = dstXf;
                    y3 = dstYf;
                }

                int* pDst = dst + (dstW * j);

                for (int i = 0; i < dstW; i++)
                {
                    if (intCalculation)
                    {
                        x0 = srcCx + (xi >> Angles.BigExp);
                        y0 = srcCy + (yi >> Angles.BigExp);
                    }
                    else
                    {
                        x0 = (int)x3;
                        y0 = (int)y3;
                    }

                    if (x0 < 0 || y0 < 0 || x0 >= srcW || y0 >= srcH)
                    {
                        pDst++;
                        goto horizotalIncrement;
                    }

                    var index = x0 + (y0 * srcW);

                    if (intCalculation || (x0 - x3 == 0 && y3 - y0 == 0))
                    {
                        color = Data[index];
                        if (color == 0)
                            color = *pDst;
                        goto assignColor;
                    }
                    float Dx = x3 - x0;
                    float Dy = y3 - y0;

                    #region BI-LINEAR INTERPOLATION
                    uint rb, ag, c3 = 0, c4 = 0;
                    int n = index + srcW;
                    bool only2 = (n >= srcLen || n + 1 >= srcLen);

                    uint c1 = (uint)Data[index++];
                    uint c2 = (uint)Data[index];
                    if (!only2)
                    {
                        c3 = (uint)Data[n++];
                        c4 = (uint)Data[n];
                    }
                    if (c1 == 0 || c1 == Colors.Transparent)
                        c1 = White;
                    if (c2 == 0 || c2 == Colors.Transparent)
                        c2 = White;

                    if (c3 == 0 || c3 == Colors.Transparent)
                        c3 = White;
                    if (c4 == 0 || c4 == Colors.Transparent)
                        c4 = White;

                    uint alpha = (uint)(Dx * 255);
                    uint invAlpha = 255 - alpha;

                    if (alpha == 255)
                        c1 = c2;

                    else if (alpha != 0)
                    {
                        rb = ((invAlpha * (c1 & GWS.Colors.RBMASK)) + (alpha * (c2 & GWS.Colors.RBMASK))) >> 8;
                        ag = (invAlpha * ((c1 & GWS.Colors.AGMASK) >> 8)) + (alpha * (GWS.Colors.ONEALPHA | ((c2 & GWS.Colors.GMASK) >> 8)));
                        c1 = ((rb & GWS.Colors.RBMASK) | (ag & GWS.Colors.AGMASK));
                    }
                    if (only2)
                    {
                        color = (int)c1;
                        goto assignColor;
                    }

                    if (alpha == 255)
                        c3 = c4;
                    else if (alpha != 0)
                    {
                        rb = ((invAlpha * (c3 & GWS.Colors.RBMASK)) + (alpha * (c4 & GWS.Colors.RBMASK))) >> 8;
                        ag = (invAlpha * ((c3 & GWS.Colors.AGMASK) >> 8)) + (alpha * (GWS.Colors.ONEALPHA | ((c4 & GWS.Colors.GMASK) >> 8)));
                        c3 = ((rb & GWS.Colors.RBMASK) | (ag & GWS.Colors.AGMASK));
                    }

                    alpha = (uint)(Dy * 255);
                    invAlpha = 255 - alpha;

                    if (alpha == 255)
                        color = (int)c3;
                    else if (alpha != 0)
                    {
                        rb = ((invAlpha * (c1 & GWS.Colors.RBMASK)) + (alpha * (c3 & GWS.Colors.RBMASK))) >> 8;
                        ag = (invAlpha * ((c1 & GWS.Colors.AGMASK) >> 8)) + (alpha * (GWS.Colors.ONEALPHA | ((c3 & GWS.Colors.GMASK) >> 8)));
                        color = (int)((rb & GWS.Colors.RBMASK) | (ag & GWS.Colors.AGMASK));
                    }
                    else
                        color = (int)c1;
                    #endregion

                    assignColor:
                    *pDst++ = color;

                horizotalIncrement:
                    #region HORIZONTAL INCREMENT
                    if (intCalculation)
                    {
                        xi += Cosi;
                        yi -= Sini;
                    }
                    else
                    {
                        x3 += Cos;
                        y3 -= Sin;
                    }
                    #endregion
                }

                #region VERTICAL INCREMENT
                if (intCalculation)
                {
                    dstXi += Sini;
                    dstYi += Cosi;
                }
                else
                {
                    dstXf += Sin;
                    dstYf += Cos;
                }
                #endregion
            }
            result = (IntPtr)dst;
            return size;
        }
        #endregion

        #region FLIP
        public unsafe Size Flip(out IntPtr result, FlipMode flipMode)
        {
            int dstW = width;
            int dstH = height;
            int srcW = width;
            int srcH = height;
            int srcLen = length;
            int* dst;
            int* Data;
            Command command = 0;

            fixed (int* d = new int[dstW * dstH])
                dst = d;

            fixed (int* p = new int[dstW * dstH])
                Data = p;

            if (this is IContainer)
                command = Command.Screen;

            CopyTo((IntPtr)Data, srcLen, srcW, 0, 0, new Rectangle(0, 0, dstW, dstH), command);

            int i = 0;
            if (flipMode == FlipMode.Horizontal)
            {
                for (var y = srcH - 1; y >= 0; y--)
                {
                    for (var x = 0; x < srcW; x++)
                    {
                        var srcInd = y * srcW + x;
                        dst[i] = Data[srcInd];
                        i++;
                    }
                }
            }
            else
            {
                for (var y = 0; y < srcH; y++)
                {
                    for (var x = srcW - 1; x >= 0; x--)
                    {
                        var srcInd = y * srcW + x;
                        dst[i] = Data[srcInd];
                        i++;
                    }
                }
            }
            if (flipMode == FlipMode.Vertical)
                Numbers.Swap(ref srcW, ref srcH);
            result = (IntPtr)dst;
            return new Size(srcW, srcH);
        }
        #endregion

        #region CLONE
        public abstract object Clone();
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            isDisposed = true;
        }
        #endregion
    }
}
