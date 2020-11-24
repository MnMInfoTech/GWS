/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    //public sealed class Block : IImage
    //{
    //    #region VARIABLES
    //    int[] Data;
    //    bool IsResizing;
    //    readonly HashSet<int> DrawnIndices = new HashSet<int>();
    //    const byte o = 0;
    //    #endregion

    //    #region CONSTRUCTORS
    //    public Block(int w, int h)
    //    {
    //        Width = w;
    //        Height = h;
    //        Length = w * h;
    //        Data = new int[Length];
    //    }
    //    public Block(int[] data, int w, int h)
    //    {
    //        Width = w;
    //        Height = h;
    //        Length = w * h;
    //        Data = data;
    //    }
    //    public unsafe Block(IntPtr data, int w, int h):
    //        this(w, h)
    //    {
    //        int* src = (int*)data;
    //        fixed (int* dst = Data)
    //            Blocks.Copy(src, 0, dst, 0, Length);
    //    }
    //    public unsafe Block(byte[] data, int w, int h):
    //        this(w /4, h)
    //    {
    //        byte* dst = (byte*)Source;
    //        fixed(byte* src = data)
    //            Blocks.Copy(src, 0, dst, 0, Length);
    //    }
    //    #endregion

    //    #region PROPERTIES
    //    public int Width { get; private set; }
    //    public int Height { get; private set; }
    //    public int Length { get; private set; }
    //    public unsafe IntPtr Source
    //    {
    //        get
    //        {
    //            fixed (int* d = Data)
    //                return (IntPtr)d;
    //        }
    //    }
    //    public Size Clip
    //    {
    //        get => new Size(Width, Height);
    //        set { }
    //    }
    //    #endregion

    //    #region BLEND
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    int Blend(int dstColor, int srcColor, byte alpha)
    //    {
    //        if (alpha == 255)
    //            return srcColor;

    //        //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-color-fast?
    //        uint C1 = (uint)dstColor;
    //        uint C2 = (uint)srcColor;
    //        uint invAlpha = 255 - (uint)alpha;
    //        uint RB = ((invAlpha * (C1 & Colors.RBMASK)) + (alpha * (C2 & Colors.RBMASK))) >> 8;
    //        uint AG = (invAlpha * ((C1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((C2 & Colors.GMASK) >> 8)));
    //        int color = (int)((RB & Colors.RBMASK) | (AG & Colors.AGMASK));
    //        return color;
    //    }
    //    #endregion

    //    #region WRITE PIXEL
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public void WritePixel(int val, int axis, bool horizontal, int srcColor, float? Alpha, DrawCommand command)
    //    {
    //        bool Opaque = command.HasFlag(DrawCommand.Opaque);
    //        if (IsResizing ||( srcColor == 0 && !Opaque))
    //            return;

    //        int i;
    //        int x = horizontal ? val : axis;
    //        int y = horizontal ? axis : val;

    //        if (x < 0 || y < 0 || x >= Width || y >= Height)
    //            return;

    //        i = x + y * Width;
    //        int dc = Data[i];

    //        bool Back = command.HasFlag(DrawCommand.Backdrop);

    //        if (Back && dc != 0)
    //            return;

    //        bool Distinct = command.HasFlag(DrawCommand.Distinct);

    //        if (Distinct)
    //        {
    //            if (DrawnIndices.Contains(i))
    //                return;
    //            DrawnIndices.Add(i);
    //        }
    //        byte alpha;

    //        float delta = Alpha ?? Colors.Alphas[(byte)((srcColor >> Colors.AShift) & 0xFF)];
    //        alpha = (byte)(delta * 255);

    //        if (alpha == 0)
    //            return;

    //        if (alpha != 255)
    //        {
    //            srcColor = Blend(dc, srcColor, alpha);
    //        }
    //        Data[i] = srcColor;
    //    }
    //    #endregion

    //    #region WRITE LINE
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public unsafe void WriteLine(int* pixels, int srcIndex, int srcW, int copyLength, 
    //        bool horizontal, int x, int y, float? Alpha, byte[] imageAlphas, DrawCommand command)
    //    {
    //        #region VARAIBLE INITIALIZATION
    //        if (IsResizing || copyLength <= 0)
    //            return;

    //        int* dst;
    //        fixed (int* d = Data)
    //            dst = d;
    //        int dstIndex = x + y * Width;
    //        int dplus = horizontal ? 1 : Width;
    //        int splus = horizontal || srcW == copyLength ? 1 : srcW;
    //        int last = dstIndex + dplus * copyLength;
    //        int j = srcIndex;

    //        int px = x;
    //        int py = y;
    //        int ix = horizontal ? 1 : 0;
    //        int iy = horizontal ? 0 : 1;

    //        int dc, sc;
    //        var NoBlend = Alpha == null;
    //        byte alpha = !NoBlend ? (byte)(Alpha.Value * 255) : o;
    //        bool Opaque = command.HasFlag(DrawCommand.Opaque);
    //        bool Back = command.HasFlag(DrawCommand.Backdrop);
    //        #endregion

    //        #region WRITING LINE
    //        for (int i = dstIndex; i < last; i += dplus, j += splus, px += ix, py += iy)
    //        {
    //            if (i >= Length) break;

    //            dc = dst[i];
    //            sc = pixels[j];

    //            if (sc == 0 && dc == 0)
    //                continue;

    //            if (sc == 0)
    //            {
    //                if (Opaque)
    //                    dst[i] = sc;
    //                continue;
    //            }

    //            if (Back && dc != 0)
    //                continue;

    //            if (!NoBlend && alpha < 2)
    //                continue;

    //            if (NoBlend || alpha == 255)
    //            {
    //                dst[i] = sc;
    //                continue;
    //            }
    //            dst[i] = Blend(dc, sc, alpha);
    //            continue;
    //        }
    //        #endregion

    //        int w = horizontal ? copyLength : 1;
    //        int h = horizontal ? 1 : copyLength;
    //    }
    //    #endregion

    //    #region COPY TO
    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public unsafe Rectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr dest, int dstLen, int dstW, int dstX, int dstY, DrawCommand command)
    //    {
    //        int* dst = (int*)dest;
    //        int srcLen = Length;
    //        int srcW = Width;
    //        int srcH = Height;
    //        int sc;
    //        int* src;
    //        fixed (int* s = Data)
    //            src = s;

    //        var copy = Rects.CompitibleRc(srcW, srcH, copyX, copyY, copyW, copyH);

    //        copyW = copy.Width;
    //        copyH = copy.Height;
    //        copyX = copy.X;
    //        copyY = copy.Y;

    //        if (copyX < 0)
    //        {
    //            copyW += copyX;
    //            copyX = 0;
    //        }
    //        if (copyY < 0)
    //        {
    //            copyH += copyY;
    //            copyY = 0;
    //        }
    //        var srcIndex = copyX + copyY * srcW;

    //        if (dstX < 0)
    //            dstX = 0;
    //        if (dstY < 0)
    //            dstY = 0;

    //        var dstIndex = dstX + dstY * dstW;

    //        if (copyW > srcW)
    //            copyW = srcW;
    //        if (copyH > srcH)
    //            copyH = srcH;

    //        if (srcIndex + copyW >= srcLen)
    //            copyW -= (srcIndex + copyW - srcLen);

    //        if (copyW <= 0)
    //            return Rectangle.Empty;

    //        if (dstIndex + copyW >= dstLen)
    //            copyW -= (dstIndex + copyW - dstLen);

    //        if (copyW <= 0)
    //            return Rectangle.Empty;

    //        int i = 0;
    //        int x = copyX;
    //        int y = copyY;
    //        int r = copyX + copyW;

    //        while (i < copyH)
    //        {
    //            if (srcIndex + copyW >= srcLen)
    //                copyW -= (srcIndex + copyW - srcLen);
    //            if (copyW <= 0)
    //                break;

    //            if (dstIndex + copyW >= dstLen)
    //                copyW -= (dstIndex + copyW - dstLen);

    //            if (copyW <= 0)
    //                break;

    //            int didx = dstIndex;
    //            int sidx = srcIndex;

    //            for (int j = didx; j < didx + copyW; j++, sidx++)
    //            {
    //                sc = src[sidx];
    //                if (sc == 0)
    //                    continue;
    //                dst[j] = sc;
    //            }
    //            srcIndex += srcW;
    //            dstIndex += dstW;
    //            ++i;
    //        }
    //        var result = new Rectangle(dstX, dstY, copyW, i);
    //        return result;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public unsafe Rectangle CopyTo(IBlockable block, int destX, int destY, int copyX, int copyY, int copyW, int copyH, DrawCommand command)
    //    {
    //        if (block is IPixels)
    //        {
    //            return CopyTo(copyX, copyY, copyW, copyH, ((IImage)block).Source, block.Length, block.Width, destX, destY, command);
    //        }

    //        if (!(block is IWritable))
    //            return Rectangle.Empty;

    //        var surface = (IWritable)block;

    //        var copy = Rects.CompitibleRc(Width, Height, copyX, copyY, copyW, copyH);
    //        Rectangle dstRc;
    //        var x = copy.X;
    //        var y = copy.Y;
    //        copyW = copy.Width;

    //        var b = y + copy.Height;
    //        if (y < 0)
    //        {
    //            b += y;
    //            y = 0;
    //        }
    //        int srcLen = Length;
    //        int srcIndex = x + y * Width;
    //        int srcW = Width;
    //        var dy = destY;
    //        int* dst;
    //        fixed (int* d = Data)
    //            dst = d;
    //        for (int j = y; j <= b; j++)
    //        {
    //            surface.WriteLine(dst, srcIndex, srcW, copyW, true, destX, dy++, null, null, command);
    //            srcIndex += srcW;
    //            if (srcIndex >= srcLen)
    //                break;
    //        }
    //        dstRc = new Rectangle(destX, destY, copyW, dy - destY);

    //        if (dstRc && block is IUpdatable)
    //        {
    //            var updatable = (IUpdatable)surface;
    //            updatable.Invalidate(dstRc.X, dstRc.Y, dstRc.Width, dstRc.Height);
    //            updatable.Update(command);
    //        }

    //        return dstRc;
    //    }
    //    #endregion

    //    #region DRAW IMAGE
    //    public unsafe void DrawImage(IntPtr source, int srcW, int srcH, int dstX, int dstY, int copyX, int copyY, int copyW, int copyH, DrawCommand command)
    //    {
    //        var copy = Rects.CompitibleRc(Width, Height, copyX, copyY, copyW, copyH);
    //        var Opaque = command.HasFlag(DrawCommand.Opaque);

    //        var x = copy.X;
    //        var y = copy.Y;
    //        copyW = copy.Width;

    //        var b = y + copy.Height;
    //        if (y < 0)
    //        {
    //            b += y;
    //            y = 0;
    //        }
    //        int* src = (int*)source;
    //        int srcLen = Length;
    //        int srcIndex = x + y * Width;
    //        var dy = dstY;
    //        int* dst;
    //        fixed (int* d = Data)
    //            dst = d;
    //        int dstIndex = dstX + dstY * Width;
    //        int sidx, didx;

    //        for (int j = y; j <= b; j++)
    //        {
    //            sidx = srcIndex;
    //            didx = dstIndex;
    //            for (int i = 0; i < copyW; i++)
    //            {
    //                int color = src[sidx++];
    //                if (color == 0 && !Opaque)
    //                    continue;
    //                dst[didx++] = color;
    //            }
    //            srcIndex += srcW;
    //            dstIndex += Width;
    //            if (srcIndex >= srcLen)
    //                break;
    //        }
    //    }
    //    #endregion

    //    #region INVALIDATE - UPDATE
    //    public void Invalidate(int x, int y, int width, int height) { }
    //    public void Update() { }
    //    #endregion

    //    #region RESIZE
    //    public void Resize(int? width = null, int? height = null)
    //    {
    //        if ((width == null && height == null) || 
    //            (width == this.Width && height == this.Height))
    //            return;
    //        var w = width ?? this.Width;
    //        var h = height ?? this.Height;
    //        IsResizing = true;
    //        Data = Data.ResizedData(w, h, this.Width, this.Height);
    //        this.Width = w;
    //        this.Height = h;
    //        Length = w * h;
    //        IsResizing = false;
    //    }
    //    #endregion

    //    #region CLONE
    //    public object Clone()
    //    {
    //        var image = new Block(Width, Height);
    //        Array.Copy(Data, image.Data, Length);
    //        return image;
    //    }
    //    #endregion

    //    #region DISPOSE
    //    public void Dispose()
    //    {
    //        Data = null;
    //        Width = Height = Length = 0;
    //    }

    //    public bool IsContainer { get; }
    //    bool IWritable.IsResizing { get; }
    //    public bool IsDrawingChild { get; set; }
    //    public string ShapeID { get; set; }
    //    public Rectangle RecentlyDrawn { get; }

    //    public unsafe void WriteLine(int* pixels, int srcIndex, int srcW, int length, bool horizontal, int x, int y, float? Alpha, byte* imageAlphas, DrawCommand drawCommand)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void WriteShape(IShape shape, IRenderInfo Settings)
    //    {
    //        throw new NotImplementedException();
    //    }
    //    #endregion
    //}
}