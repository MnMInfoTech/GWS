/* Source reference: https://github.com/rds1983/StbSharp
 * StbSharp is C# port of the famous C framework: https://github.com/nothings/stb
 * which has been ported using Sichem - https://github.com/rds1983/Sichem.
 * Lincence : Public Domain */

/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MnM.GWS
{
    partial class Factory
    {
        unsafe sealed class STBImageProcessor : ImageProcessor, IImageProcessor
        {
            #region variables 
            Stream receiver, sender;
            byte[] inBuffer, outBuffer;
            StbImage.stbi_io_callbacks _callbacks;
            internal static readonly STBImageProcessor Instance;
            #endregion

            #region constructor
            static STBImageProcessor()
            {
                Instance = new STBImageProcessor();
            }

            STBImageProcessor()
            {
                inBuffer = new byte[1024];
                _callbacks = new StbImage.stbi_io_callbacks
                {
                    read = readCallback,
                    skip = skipCallback,
                    eof = eof
                };
                outBuffer = new byte[1024];
                DefaultImageBackgroundColour = Colours.Transparent;
            }
            #endregion

            #region private methods
            int skipCallback(void* user, int i)
            {
                return (int)receiver.Seek(i, SeekOrigin.Current);
            }
            int eof(void* user)
            {
                return receiver.CanRead ? 1 : 0;
            }
            int readCallback(void* user, sbyte* data, int size)
            {
                if (size > inBuffer.Length)
                {
                    inBuffer = new byte[size * 2];
                }

                var res = receiver.Read(inBuffer, 0, size);
                Marshal.Copy(inBuffer, 0, new IntPtr(data), size);
                return res;
            }
            int writeCallback(void* context, void* data, int size)
            {
                if (data == null || size <= 0)
                {
                    return 0;
                }

                if (outBuffer.Length < size)
                {
                    outBuffer = new byte[size * 2];
                }

                var bptr = (byte*)data;

                Marshal.Copy(new IntPtr(bptr), outBuffer, 0, size);

                sender.Write(outBuffer, 0, size);

                return size;
            }
            #endregion

            #region Read
            public override Task<Tuple<byte[], int, int>> Read(Stream stream)
            {
                receiver = stream;
                try
                {
                    int x, y, comp;
                    var result = StbImage.stbi_load_from_callbacks(_callbacks, null, &x, &y, &comp, 4);

                    //image.format = (req_comp == 4) ? PixelFormats.RGB888 : PixelFormats.RGB24;

                    if (result == null)
                    {
                        throw new Exception(StbImage.LastError);
                    }

                    // Convert to array
                    var data = new byte[(x) * (y) * 4];
                    Marshal.Copy(new IntPtr(result), data, 0, data.Length);
                    CRuntime.free(result);

                    return Task.FromResult(Tuple.Create(data, x, y));
                }
                finally
                {
                    receiver = null;
                }
            }
            public override IAnimatedGifFrame[] ReadAnimatedGif(Stream stream, out int w, out int h, out int c, int rc)
            {
                try
                {
                    int comp = 0;
                    int reqComp = (int)rc;
                    w = h = comp = 0;

                    var res = new PrimitiveList<IAnimatedGifFrame>();
                    receiver = stream;

                    var context = new StbImage.stbi__context();
                    StbImage.stbi__start_callbacks(context, _callbacks, null);

                    if (StbImage.stbi__gif_test(context) == 0)
                    {
                        throw new Exception("Input stream is not GIF file.");
                    }

                    var g = new StbImage.stbi__gif();
                    int resultc = 0;
                    do
                    {
                        int ccomp;
                        var result = StbImage.stbi__gif_load_next(context, g, &ccomp, reqComp);
                        if (result == null)
                            break;

                        comp = ccomp;
                        resultc = (int)reqComp != 0 ? reqComp : comp;
                        var data = new byte[g.w * g.h * resultc];
                        Marshal.Copy(new IntPtr(result), data, 0, data.Length);
                        CRuntime.free(result);

                        var frame = new AnimatedGifFrame
                        (
                            data,
                            g.delay
                        );
                        res.Add(frame);
                    } while (true);

                    CRuntime.free(g._out_);

                    if (res.Count > 0)
                    {
                        w = g.w;
                        h = g.h;
                    }
                    c = resultc;
                    return res.ToArray();
                }
                finally
                {
                    receiver = null;
                }
            }
            #endregion

            #region Write
            protected override Task<bool> PerformImageWriting
            (
                IntPtr data,
                int w, int h, int len,
                Stream dest,
                ImageFormat format,
                byte quality,
                byte pitch,
                byte transparency
            )
            {
                byte* b = (byte*)data;
                sender = dest;
                try
                {
                    if ((format & ImageFormat.PNG) == ImageFormat.PNG)
                        StbImageWrite.stbi_write_png_to_func(writeCallback, null, w, h, pitch, b, w * pitch);
                    else if ((format & ImageFormat.JPG) == ImageFormat.JPG)
                        StbImageWrite.stbi_write_jpg_to_func(writeCallback, null, w, h, pitch, b, quality);
                    else if ((format & ImageFormat.TGA) == ImageFormat.TGA)
                        StbImageWrite.stbi_write_tga_to_func(writeCallback, null, w, h, pitch, b);
                    else if ((format & ImageFormat.HDR) == ImageFormat.HDR)
                    {
                        var f = new float[len];
                        fixed (float* fptr = f)
                        {
                            for (var i = 0; i < len; ++i)
                                fptr[i] = b[i] / 255.0f;

                            StbImageWrite.stbi_write_hdr_to_func(writeCallback, null, w, h, pitch, fptr);
                        }
                    }
                    else
                        StbImageWrite.stbi_write_bmp_to_func(writeCallback, null, w, h, pitch, b);
                    return Task.FromResult(true);
                }
                finally
                {
                    sender = null;
                }
            }
            #endregion

            #region GetAnimatedGifFrame
            public static AnimatedGifFrame GetAnimatedGifFrame(byte[] data, int delay) =>
                new AnimatedGifFrame(data, delay);
            #endregion

            public override void Dispose()
            {
                receiver = sender = null;
                inBuffer = outBuffer = null;
                _callbacks = null;
            }

            #region interface
            public int DefaultImageBackgroundColour { get; set; }
            #endregion

            #region CRUN TIME
            static unsafe class CRuntime
            {
                public const long DBL_EXP_MASK = 0x7ff0000000000000L;
                public const int DBL_MANT_BITS = 52;
                public const long DBL_SGN_MASK = -1 - 0x7fffffffffffffffL;
                public const long DBL_MANT_MASK = 0x000fffffffffffffL;
                public const long DBL_EXP_CLR_MASK = DBL_SGN_MASK | DBL_MANT_MASK;

                public static void* malloc(ulong size)
                {
                    return malloc((long)size);
                }

                public static void* malloc(long size)
                {
                    var ptr = Marshal.AllocHGlobal((int)size);

                    return ptr.ToPointer();
                }

                public static void memcpy(void* a, void* b, long size)
                {
                    var ap = (byte*)a;
                    var bp = (byte*)b;
                    for (long i = 0; i < size; ++i)
                    {
                        *ap++ = *bp++;
                    }
                }

                public static void memcpy(void* a, void* b, ulong size)
                {
                    memcpy(a, b, (long)size);
                }

                public static void memmove(void* a, void* b, long size)
                {
                    void* temp = null;

                    try
                    {
                        temp = malloc(size);
                        memcpy(temp, b, size);
                        memcpy(a, temp, size);
                    }

                    finally
                    {
                        if (temp != null)
                        {
                            free(temp);
                        }
                    }
                }

                public static void memmove(void* a, void* b, ulong size)
                {
                    memmove(a, b, (long)size);
                }

                public static int memcmp(void* a, void* b, long size)
                {
                    var result = 0;
                    var ap = (byte*)a;
                    var bp = (byte*)b;
                    for (long i = 0; i < size; ++i)
                    {
                        if (*ap != *bp)
                        {
                            result += 1;
                        }

                        ap++;
                        bp++;
                    }

                    return result;
                }

                public static int memcmp(void* a, void* b, ulong size)
                {
                    return memcmp(a, b, (long)size);
                }

                public static int memcmp(byte* a, byte[] b, ulong size)
                {
                    fixed (void* bptr = b)
                    {
                        return memcmp(a, bptr, (long)size);
                    }
                }

                public static void free(void* a)
                {
                    var ptr = new IntPtr(a);
                    Marshal.FreeHGlobal(ptr);
                }

                public static void memset(void* ptr, int value, long size)
                {
                    byte* bptr = (byte*)ptr;
                    var bval = (byte)value;
                    for (long i = 0; i < size; ++i)
                    {
                        *bptr++ = bval;
                    }
                }

                public static void memset(void* ptr, int value, ulong size)
                {
                    memset(ptr, value, (long)size);
                }

                public static uint _lrotl(uint x, int y)
                {
                    return (x << y) | (x >> (32 - y));
                }

                public static void* realloc(void* a, long newSize)
                {
                    if (a == null)
                    {
                        return malloc(newSize);
                    }

                    var ptr = new IntPtr(a);
                    var result = Marshal.ReAllocHGlobal(ptr, new IntPtr(newSize));

                    return result.ToPointer();
                }

                public static void* realloc(void* a, ulong newSize)
                {
                    return realloc(a, (long)newSize);
                }

                public static int abs(int v)
                {
                    return Math.Abs(v);
                }

                /// <summary>
                /// This code had been borrowed from here: https://github.com/MachineCognitis/C.math.NET
                /// </summary>
                /// <param name="number"></param>
                /// <param name="exponent"></param>
                /// <returns></returns>
                public static double frexp(double number, int* exponent)
                {
                    var bits = BitConverter.DoubleToInt64Bits(number);
                    var exp = (int)((bits & DBL_EXP_MASK) >> DBL_MANT_BITS);
                    *exponent = 0;

                    if (exp == 0x7ff || number == 0D)
                        number += number;
                    else
                    {
                        // Not zero and finite.
                        *exponent = exp - 1022;
                        if (exp == 0)
                        {
                            // Subnormal, scale number so that it is  [1, 2).
                            number *= BitConverter.Int64BitsToDouble(0x4350000000000000L); // 2^54
                            bits = BitConverter.DoubleToInt64Bits(number);
                            exp = (int)((bits & DBL_EXP_MASK) >> DBL_MANT_BITS);
                            *exponent = exp - 1022 - 54;
                        }

                        // Set exponent to -1 so that number is  [0.5, 1).
                        number = BitConverter.Int64BitsToDouble((bits & DBL_EXP_CLR_MASK) | 0x3fe0000000000000L);
                    }

                    return number;
                }

                public static double pow(double a, double b)
                {
                    return Math.Pow(a, b);
                }

                public static float fabs(double a)
                {
                    return (float)Math.Abs(a);
                }

                public static double ceil(double a)
                {
                    return Math.Ceiling(a);
                }


                public static double floor(double a)
                {
                    return Math.Floor(a);
                }

                public static double log(double value)
                {
                    return Math.Log(value);
                }

                public static double exp(double value)
                {
                    return Math.Exp(value);
                }

                public static double cos(double value)
                {
                    return Math.Cos(value);
                }

                public static double acos(double value)
                {
                    return Math.Acos(value);
                }

                public static double sin(double value)
                {
                    return Math.Sin(value);
                }

                public static double ldexp(double number, int exponent)
                {
                    return number * Math.Pow(2, exponent);
                }

                public delegate int QSortComparer(void* a, void* b);

                private static void qsortSwap(byte* data, long size, long pos1, long pos2)
                {
                    var a = data + size * pos1;
                    var b = data + size * pos2;

                    for (long k = 0; k < size; ++k)
                    {
                        var tmp = *a;
                        *a = *b;
                        *b = tmp;

                        a++;
                        b++;
                    }
                }

                private static long qsortPartition(byte* data, long size, QSortComparer comparer, long left, long right)
                {
                    void* pivot = data + size * left;
                    var i = left - 1;
                    var j = right + 1;
                    for (; ; )
                    {
                        do
                        {
                            ++i;
                        } while (comparer(data + size * i, pivot) < 0);

                        do
                        {
                            --j;
                        } while (comparer(data + size * j, pivot) > 0);

                        if (i >= j)
                        {
                            return j;
                        }

                        qsortSwap(data, size, i, j);
                    }
                }


                private static void qsortInternal(byte* data, long size, QSortComparer comparer, long left, long right)
                {
                    if (left < right)
                    {
                        var p = qsortPartition(data, size, comparer, left, right);

                        qsortInternal(data, size, comparer, left, p);
                        qsortInternal(data, size, comparer, p + 1, right);
                    }
                }

                public static void qsort(void* data, ulong count, ulong size, QSortComparer comparer)
                {
                    qsortInternal((byte*)data, (long)size, comparer, 0, (long)count - 1);
                }

                public static double sqrt(double val)
                {
                    return Math.Sqrt(val);
                }

                public static double fmod(double x, double y)
                {
                    return x % y;
                }

                public static ulong strlen(sbyte* str)
                {
                    var ptr = str;

                    while (*ptr != '\0')
                    {
                        ptr++;
                    }

                    return ((ulong)ptr - (ulong)str - 1);
                }
            }
            #endregion

            #region STBDXT
            unsafe static class StbDxt
            {
                public static byte[] stb__Expand5 = new byte[32];
                public static byte[] stb__Expand6 = new byte[64];
                public static byte[] stb__OMatch5 = new byte[512];
                public static byte[] stb__OMatch6 = new byte[512];
                public static byte[] stb__QuantRBTab = new byte[256 + 16];
                public static byte[] stb__QuantGTab = new byte[256 + 16];
                public static int init = (int)(1);

                public static void stb__DitherBlock(byte* dest, byte* block)
                {
                    int* err = stackalloc int[8];
                    var ep1 = err;
                    var ep2 = err + 4;
                    int ch;
                    for (ch = 0; ch < 3; ++ch)
                    {
                        var bp = block + ch;
                        var dp = dest + ch;
                        var quantArray = ch == (1) ? stb__QuantGTab : stb__QuantRBTab;
                        fixed (byte* quant = quantArray)
                        {
                            CRuntime.memset(err, 0, (ulong)(8 * sizeof(int)));
                            int y;
                            for (y = 0; (y) < (4); ++y)
                            {
                                dp[0] = quant[bp[0] + ((3 * ep2[1] + 5 * ep2[0]) >> 4)];
                                ep1[0] = bp[0] - dp[0];
                                dp[4] = quant[bp[4] + ((7 * ep1[0] + 3 * ep2[2] + 5 * ep2[1] + ep2[0]) >> 4)];
                                ep1[1] = bp[4] - dp[4];
                                dp[8] = quant[bp[8] + ((7 * ep1[1] + 3 * ep2[3] + 5 * ep2[2] + ep2[1]) >> 4)];
                                ep1[2] = bp[8] - dp[8];
                                dp[12] = quant[bp[12] + ((7 * ep1[2] + 5 * ep2[3] + ep2[2]) >> 4)];
                                ep1[3] = bp[12] - dp[12];
                                bp += 16;
                                dp += 16;
                                var et = ep1;
                                ep1 = ep2;
                                ep2 = et;
                            }
                        }
                    }
                }
                public static byte[] stb_compress_dxt(byte* rgba, int w, int h, bool hasAlpha = true, int mode = 10)
                {
                    var osize = hasAlpha ? 16 : 8;

                    var result = new byte[(w + 3) * (h + 3) / 16 * osize];
                    fixed (byte* resultPtr = result)
                    {
                        var p = resultPtr;

                        byte* block = stackalloc byte[16 * 4];
                        for (var j = 0; j < w; j += 4)
                        {
                            var x = 4;
                            for (var i = 0; i < h; i += 4)
                            {
                                if (j + 3 >= w) x = w - j;
                                int y;
                                for (y = 0; y < 4; ++y)
                                {
                                    if (j + y >= h) break;
                                    CRuntime.memcpy(block + y * 16, rgba + w * 4 * (j + y) + i * 4, x * 4);
                                }
                                int y2;
                                if (x < 4)
                                {
                                    switch (x)
                                    {
                                        case 0:
                                            throw new Exception("Unknown error");
                                        case 1:
                                            for (y2 = 0; y2 < y; ++y2)
                                            {
                                                CRuntime.memcpy(block + y2 * 16 + 1 * 4, block + y2 * 16 + 0 * 4, 4);
                                                CRuntime.memcpy(block + y2 * 16 + 2 * 4, block + y2 * 16 + 0 * 4, 8);
                                            }
                                            break;
                                        case 2:
                                            for (y2 = 0; y2 < y; ++y2)
                                                CRuntime.memcpy(block + y2 * 16 + 2 * 4, block + y2 * 16 + 0 * 4, 8);
                                            break;
                                        case 3:
                                            for (y2 = 0; y2 < y; ++y2)
                                                CRuntime.memcpy(block + y2 * 16 + 3 * 4, block + y2 * 16 + 1 * 4, 4);
                                            break;
                                    }
                                }
                                y2 = 0;
                                for (; y < 4; ++y, ++y2)
                                    CRuntime.memcpy(block + y * 16, block + y2 * 16, 4 * 4);
                                stb_compress_dxt_block(p, block, hasAlpha ? 1 : 0, mode);
                                p += hasAlpha ? 16 : 8;
                            }
                        }
                    }


                    return result;
                }
                public static int stb__Mul8Bit(int a, int b)
                {
                    int t = (int)(a * b + 128);
                    return (int)((t + (t >> 8)) >> 8);
                }

                public static void stb__From16Bit(byte* _out_, ushort v)
                {
                    int rv = (int)((v & 0xf800) >> 11);
                    int gv = (int)((v & 0x07e0) >> 5);
                    int bv = (int)((v & 0x001f) >> 0);
                    _out_[0] = (byte)(stb__Expand5[rv]);
                    _out_[1] = (byte)(stb__Expand6[gv]);
                    _out_[2] = (byte)(stb__Expand5[bv]);
                    _out_[3] = (byte)(0);
                }

                public static ushort stb__As16Bit(int r, int g, int b)
                {
                    return
                        (ushort)
                            ((stb__Mul8Bit((int)(r), (int)(31)) << 11) + (stb__Mul8Bit((int)(g), (int)(63)) << 5) +
                             stb__Mul8Bit((int)(b), (int)(31)));
                }

                public static int stb__Lerp13(int a, int b)
                {
                    return (int)((2 * a + b) / 3);
                }

                public static void stb__Lerp13RGB(byte* _out_, byte* p1, byte* p2)
                {
                    _out_[0] = (byte)(stb__Lerp13((int)(p1[0]), (int)(p2[0])));
                    _out_[1] = (byte)(stb__Lerp13((int)(p1[1]), (int)(p2[1])));
                    _out_[2] = (byte)(stb__Lerp13((int)(p1[2]), (int)(p2[2])));
                }

                public static void stb__PrepareOptTable(byte[] Table, byte[] expand, int size)
                {
                    int i;
                    int mn;
                    int mx;
                    for (i = (int)(0); (i) < (256); i++)
                    {
                        int bestErr = (int)(256);
                        for (mn = (int)(0); (mn) < (size); mn++)
                        {
                            for (mx = (int)(0); (mx) < (size); mx++)
                            {
                                int mine = (int)(expand[mn]);
                                int maxe = (int)(expand[mx]);
                                int err = (int)(CRuntime.abs((int)(stb__Lerp13((int)(maxe), (int)(mine)) - i)));
                                err += (int)(CRuntime.abs((int)(maxe - mine)) * 3 / 100);
                                if ((err) < (bestErr))
                                {
                                    Table[i * 2 + 0] = (byte)(mx);
                                    Table[i * 2 + 1] = (byte)(mn);
                                    bestErr = (int)(err);
                                }
                            }
                        }
                    }
                }

                public static void stb__EvalColours(byte* colour, ushort c0, ushort c1)
                {
                    stb__From16Bit(colour + 0, (ushort)(c0));
                    stb__From16Bit(colour + 4, (ushort)(c1));
                    stb__Lerp13RGB(colour + 8, colour + 0, colour + 4);
                    stb__Lerp13RGB(colour + 12, colour + 4, colour + 0);
                }

                public static uint stb__MatchColoursBlock(byte* block, byte* colour, int dither)
                {
                    uint mask = (uint)(0);
                    int dirr = (int)(colour[0 * 4 + 0] - colour[1 * 4 + 0]);
                    int dirg = (int)(colour[0 * 4 + 1] - colour[1 * 4 + 1]);
                    int dirb = (int)(colour[0 * 4 + 2] - colour[1 * 4 + 2]);
                    int* dots = stackalloc int[16];
                    int* stops = stackalloc int[4];
                    int i;
                    int c0Point;
                    int halfPoint;
                    int c3Point;
                    for (i = (int)(0); (i) < (16); i++)
                    {
                        dots[i] = (int)(block[i * 4 + 0] * dirr + block[i * 4 + 1] * dirg + block[i * 4 + 2] * dirb);
                    }
                    for (i = (int)(0); (i) < (4); i++)
                    {
                        stops[i] = (int)(colour[i * 4 + 0] * dirr + colour[i * 4 + 1] * dirg + colour[i * 4 + 2] * dirb);
                    }
                    c0Point = (int)((stops[1] + stops[3]) >> 1);
                    halfPoint = (int)((stops[3] + stops[2]) >> 1);
                    c3Point = (int)((stops[2] + stops[0]) >> 1);
                    if (dither == 0)
                    {
                        for (i = (int)(15); (i) >= (0); i--)
                        {
                            int dot = (int)(dots[i]);
                            mask <<= 2;
                            if ((dot) < (halfPoint)) mask |= (uint)(((dot) < (c0Point)) ? 1 : 3);
                            else mask |= (uint)(((dot) < (c3Point)) ? 2 : 0);
                        }
                    }
                    else
                    {
                        int* err = stackalloc int[8];
                        int* ep1 = err;
                        int* ep2 = err + 4;
                        int* dp = dots;
                        int y;
                        c0Point <<= 4;
                        halfPoint <<= 4;
                        c3Point <<= 4;
                        for (i = (int)(0); (i) < (8); i++)
                        {
                            err[i] = (int)(0);
                        }
                        for (y = (int)(0); (y) < (4); y++)
                        {
                            int dot;
                            int lmask;
                            int step;
                            dot = (int)((dp[0] << 4) + (3 * ep2[1] + 5 * ep2[0]));
                            if ((dot) < (halfPoint)) step = (int)(((dot) < (c0Point)) ? 1 : 3);
                            else step = (int)(((dot) < (c3Point)) ? 2 : 0);
                            ep1[0] = (int)(dp[0] - stops[step]);
                            lmask = (int)(step);
                            dot = (int)((dp[1] << 4) + (7 * ep1[0] + 3 * ep2[2] + 5 * ep2[1] + ep2[0]));
                            if ((dot) < (halfPoint)) step = (int)(((dot) < (c0Point)) ? 1 : 3);
                            else step = (int)(((dot) < (c3Point)) ? 2 : 0);
                            ep1[1] = (int)(dp[1] - stops[step]);
                            lmask |= (int)(step << 2);
                            dot = (int)((dp[2] << 4) + (7 * ep1[1] + 3 * ep2[3] + 5 * ep2[2] + ep2[1]));
                            if ((dot) < (halfPoint)) step = (int)(((dot) < (c0Point)) ? 1 : 3);
                            else step = (int)(((dot) < (c3Point)) ? 2 : 0);
                            ep1[2] = (int)(dp[2] - stops[step]);
                            lmask |= (int)(step << 4);
                            dot = (int)((dp[3] << 4) + (7 * ep1[2] + 5 * ep2[3] + ep2[2]));
                            if ((dot) < (halfPoint)) step = (int)(((dot) < (c0Point)) ? 1 : 3);
                            else step = (int)(((dot) < (c3Point)) ? 2 : 0);
                            ep1[3] = (int)(dp[3] - stops[step]);
                            lmask |= (int)(step << 6);
                            dp += 4;
                            mask |= (uint)(lmask << (y * 8));
                            {
                                int* et = ep1;
                                ep1 = ep2;
                                ep2 = et;
                            }
                        }
                    }

                    return (uint)(mask);
                }

                public static void stb__OptimizeColoursBlock(byte* block, ushort* pmax16, ushort* pmin16)
                {
                    int mind = (int)(0x7fffffff);
                    int maxd = (int)(-0x7fffffff);
                    byte* minp = null;
                    byte* maxp = null;
                    double magn;
                    int v_r;
                    int v_g;
                    int v_b;
                    int nIterPower = (int)(4);
                    float* covf = stackalloc float[6];
                    float vfr;
                    float vfg;
                    float vfb;
                    int* cov = stackalloc int[6];
                    int* mu = stackalloc int[3];
                    int* min = stackalloc int[3];
                    int* max = stackalloc int[3];
                    int ch;
                    int i;
                    int iter;
                    for (ch = (int)(0); (ch) < (3); ch++)
                    {
                        byte* bp = (block) + ch;
                        int muv;
                        int minv;
                        int maxv;
                        muv = (int)(minv = (int)(maxv = (int)(bp[0])));
                        for (i = (int)(4); (i) < (64); i += (int)(4))
                        {
                            muv += (int)(bp[i]);
                            if ((bp[i]) < (minv)) minv = (int)(bp[i]);
                            else if ((bp[i]) > (maxv)) maxv = (int)(bp[i]);
                        }
                        mu[ch] = (int)((muv + 8) >> 4);
                        min[ch] = (int)(minv);
                        max[ch] = (int)(maxv);
                    }
                    for (i = (int)(0); (i) < (6); i++)
                    {
                        cov[i] = (int)(0);
                    }
                    for (i = (int)(0); (i) < (16); i++)
                    {
                        int r = (int)(block[i * 4 + 0] - mu[0]);
                        int g = (int)(block[i * 4 + 1] - mu[1]);
                        int b = (int)(block[i * 4 + 2] - mu[2]);
                        cov[0] += (int)(r * r);
                        cov[1] += (int)(r * g);
                        cov[2] += (int)(r * b);
                        cov[3] += (int)(g * g);
                        cov[4] += (int)(g * b);
                        cov[5] += (int)(b * b);
                    }
                    for (i = (int)(0); (i) < (6); i++)
                    {
                        covf[i] = (float)(cov[i] / 255.0f);
                    }
                    vfr = ((float)(max[0] - min[0]));
                    vfg = ((float)(max[1] - min[1]));
                    vfb = ((float)(max[2] - min[2]));
                    for (iter = (int)(0); (iter) < (nIterPower); iter++)
                    {
                        float r = (float)(vfr * covf[0] + vfg * covf[1] + vfb * covf[2]);
                        float g = (float)(vfr * covf[1] + vfg * covf[3] + vfb * covf[4]);
                        float b = (float)(vfr * covf[2] + vfg * covf[4] + vfb * covf[5]);
                        vfr = (float)(r);
                        vfg = (float)(g);
                        vfb = (float)(b);
                    }
                    magn = (double)(CRuntime.fabs((double)(vfr)));
                    if ((CRuntime.fabs((double)(vfg))) > (magn)) magn = (double)(CRuntime.fabs((double)(vfg)));
                    if ((CRuntime.fabs((double)(vfb))) > (magn)) magn = (double)(CRuntime.fabs((double)(vfb)));
                    if ((magn) < (4.0f))
                    {
                        v_r = (int)(299);
                        v_g = (int)(587);
                        v_b = (int)(114);
                    }
                    else
                    {
                        magn = (double)(512.0 / magn);
                        v_r = ((int)(vfr * magn));
                        v_g = ((int)(vfg * magn));
                        v_b = ((int)(vfb * magn));
                    }

                    for (i = (int)(0); (i) < (16); i++)
                    {
                        int dot = (int)(block[i * 4 + 0] * v_r + block[i * 4 + 1] * v_g + block[i * 4 + 2] * v_b);
                        if ((dot) < (mind))
                        {
                            mind = (int)(dot);
                            minp = block + i * 4;
                        }
                        if ((dot) > (maxd))
                        {
                            maxd = (int)(dot);
                            maxp = block + i * 4;
                        }
                    }
                    *pmax16 = (ushort)(stb__As16Bit((int)(maxp[0]), (int)(maxp[1]), (int)(maxp[2])));
                    *pmin16 = (ushort)(stb__As16Bit((int)(minp[0]), (int)(minp[1]), (int)(minp[2])));
                }

                public static int stb__sclamp(float y, int p0, int p1)
                {
                    int x = (int)(y);
                    if ((x) < (p0)) return (int)(p0);
                    if ((x) > (p1)) return (int)(p1);
                    return (int)(x);
                }

                public static int stb__RefineBlock(byte* block, ushort* pmax16, ushort* pmin16, uint mask)
                {
                    int* w1Tab = stackalloc int[4];
                    w1Tab[0] = (int)(3);
                    w1Tab[1] = (int)(0);
                    w1Tab[2] = (int)(2);
                    w1Tab[3] = (int)(1);

                    int* prods = stackalloc int[4];
                    prods[0] = (int)(0x090000);
                    prods[1] = (int)(0x000900);
                    prods[2] = (int)(0x040102);
                    prods[3] = (int)(0x010402);

                    float frb;
                    float fg;
                    ushort oldMin;
                    ushort oldMax;
                    ushort min16;
                    ushort max16;
                    int i;
                    int akku = (int)(0);
                    int xx;
                    int xy;
                    int yy;
                    int At1_r;
                    int At1_g;
                    int At1_b;
                    int At2_r;
                    int At2_g;
                    int At2_b;
                    uint cm = (uint)(mask);
                    oldMin = (ushort)(*pmin16);
                    oldMax = (ushort)(*pmax16);
                    if ((mask ^ (mask << 2)) < (4))
                    {
                        int r = (int)(8);
                        int g = (int)(8);
                        int b = (int)(8);
                        for (i = (int)(0); (i) < (16); ++i)
                        {
                            r += (int)(block[i * 4 + 0]);
                            g += (int)(block[i * 4 + 1]);
                            b += (int)(block[i * 4 + 2]);
                        }
                        r >>= 4;
                        g >>= 4;
                        b >>= 4;
                        max16 = (ushort)((stb__OMatch5[r] << 11) | (stb__OMatch6[g] << 5) | stb__OMatch5[b]);
                        min16 = (ushort)((stb__OMatch5[r + 256] << 11) | (stb__OMatch6[g + 256] << 5) | stb__OMatch5[b + 256]);
                    }
                    else
                    {
                        At1_r = (int)(At1_g = (int)(At1_b = (int)(0)));
                        At2_r = (int)(At2_g = (int)(At2_b = (int)(0)));
                        for (i = (int)(0); (i) < (16); ++i, cm >>= 2)
                        {
                            int step = (int)(cm & 3);
                            int w1 = (int)(w1Tab[step]);
                            int r = (int)(block[i * 4 + 0]);
                            int g = (int)(block[i * 4 + 1]);
                            int b = (int)(block[i * 4 + 2]);
                            akku += (int)(prods[step]);
                            At1_r += (int)(w1 * r);
                            At1_g += (int)(w1 * g);
                            At1_b += (int)(w1 * b);
                            At2_r += (int)(r);
                            At2_g += (int)(g);
                            At2_b += (int)(b);
                        }
                        At2_r = (int)(3 * At2_r - At1_r);
                        At2_g = (int)(3 * At2_g - At1_g);
                        At2_b = (int)(3 * At2_b - At1_b);
                        xx = (int)(akku >> 16);
                        yy = (int)((akku >> 8) & 0xff);
                        xy = (int)((akku >> 0) & 0xff);
                        frb = (float)(3.0f * 31.0f / 255.0f / (xx * yy - xy * xy));
                        fg = (float)(frb * 63.0f / 31.0f);
                        max16 = (ushort)(stb__sclamp((float)((At1_r * yy - At2_r * xy) * frb + 0.5f), (int)(0), (int)(31)) << 11);
                        max16 |= (ushort)(stb__sclamp((float)((At1_g * yy - At2_g * xy) * fg + 0.5f), (int)(0), (int)(63)) << 5);
                        max16 |= (ushort)(stb__sclamp((float)((At1_b * yy - At2_b * xy) * frb + 0.5f), (int)(0), (int)(31)) << 0);
                        min16 = (ushort)(stb__sclamp((float)((At2_r * xx - At1_r * xy) * frb + 0.5f), (int)(0), (int)(31)) << 11);
                        min16 |= (ushort)(stb__sclamp((float)((At2_g * xx - At1_g * xy) * fg + 0.5f), (int)(0), (int)(63)) << 5);
                        min16 |= (ushort)(stb__sclamp((float)((At2_b * xx - At1_b * xy) * frb + 0.5f), (int)(0), (int)(31)) << 0);
                    }

                    *pmin16 = (ushort)(min16);
                    *pmax16 = (ushort)(max16);
                    return (int)((oldMin != min16) || (oldMax != max16) ? 1 : 0);
                }

                public static void stb__CompressColourBlock(byte* dest, byte* block, int mode)
                {
                    uint mask;
                    int i;
                    int dither;
                    int refinecount;
                    ushort max16;
                    ushort min16;
                    byte* dblock = stackalloc byte[16 * 4];
                    byte* colour = stackalloc byte[4 * 4];
                    dither = (int)(mode & 1);
                    refinecount = (int)((mode & 2) != 0 ? 2 : 1);
                    for (i = (int)(1); (i) < (16); i++)
                    {
                        if (((uint*)(block))[i] != ((uint*)(block))[0]) break;
                    }
                    if ((i) == (16))
                    {
                        int r = (int)(block[0]);
                        int g = (int)(block[1]);
                        int b = (int)(block[2]);
                        mask = (uint)(0xaaaaaaaa);
                        max16 = (ushort)((stb__OMatch5[r] << 11) | (stb__OMatch6[g] << 5) | stb__OMatch5[b]);
                        min16 = (ushort)((stb__OMatch5[r + 256] << 11) | (stb__OMatch6[g + 256] << 5) | stb__OMatch5[b + 256]);
                    }
                    else
                    {
                        if ((dither) != 0) stb__DitherBlock(dblock, block);
                        stb__OptimizeColoursBlock((dither) != 0 ? dblock : block, &max16, &min16);
                        if (max16 != min16)
                        {
                            stb__EvalColours(colour, (ushort)(max16), (ushort)(min16));
                            mask = (uint)(stb__MatchColoursBlock(block, colour, (int)(dither)));
                        }
                        else mask = (uint)(0);
                        for (i = (int)(0); (i) < (refinecount); i++)
                        {
                            uint lastmask = (uint)(mask);
                            if ((stb__RefineBlock((dither) != 0 ? dblock : block, &max16, &min16, (uint)(mask))) != 0)
                            {
                                if (max16 != min16)
                                {
                                    stb__EvalColours(colour, (ushort)(max16), (ushort)(min16));
                                    mask = (uint)(stb__MatchColoursBlock(block, colour, (int)(dither)));
                                }
                                else
                                {
                                    mask = (uint)(0);
                                    break;
                                }
                            }
                            if ((mask) == (lastmask)) break;
                        }
                    }

                    if ((max16) < (min16))
                    {
                        ushort t = (ushort)(min16);
                        min16 = (ushort)(max16);
                        max16 = (ushort)(t);
                        mask ^= (uint)(0x55555555);
                    }

                    dest[0] = ((byte)(max16));
                    dest[1] = ((byte)(max16 >> 8));
                    dest[2] = ((byte)(min16));
                    dest[3] = ((byte)(min16 >> 8));
                    dest[4] = ((byte)(mask));
                    dest[5] = ((byte)(mask >> 8));
                    dest[6] = ((byte)(mask >> 16));
                    dest[7] = ((byte)(mask >> 24));
                }

                public static void stb__CompressAlphaBlock(byte* dest, byte* src, int stride)
                {
                    int i;
                    int dist;
                    int bias;
                    int dist4;
                    int dist2;
                    int bits;
                    int mask;
                    int mn;
                    int mx;
                    mn = (int)(mx = (int)(src[0]));
                    for (i = (int)(1); (i) < (16); i++)
                    {
                        if ((src[i * stride]) < (mn)) mn = (int)(src[i * stride]);
                        else if ((src[i * stride]) > (mx)) mx = (int)(src[i * stride]);
                    }
                    (dest)[0] = (byte)(mx);
                    (dest)[1] = (byte)(mn);
                    dest += 2;
                    dist = (int)(mx - mn);
                    dist4 = (int)(dist * 4);
                    dist2 = (int)(dist * 2);
                    bias = (int)(((dist) < (8)) ? (dist - 1) : (dist / 2 + 2));
                    bias -= (int)(mn * 7);
                    bits = (int)(0);
                    mask = (int)(0);
                    for (i = (int)(0); (i) < (16); i++)
                    {
                        int a = (int)(src[i * stride] * 7 + bias);
                        int ind;
                        int t;
                        t = (int)(((a) >= (dist4)) ? -1 : 0);
                        ind = (int)(t & 4);
                        a -= (int)(dist4 & t);
                        t = (int)(((a) >= (dist2)) ? -1 : 0);
                        ind += (int)(t & 2);
                        a -= (int)(dist2 & t);
                        ind += (int)((a) >= (dist) ? 1 : 0);
                        ind = (int)(-ind & 7);
                        ind ^= (int)((2) > (ind) ? 1 : 0);
                        mask |= (int)(ind << bits);
                        if ((bits += (int)(3)) >= (8))
                        {
                            *dest++ = (byte)(mask);
                            mask >>= 8;
                            bits -= (int)(8);
                        }
                    }
                }

                public static void stb__InitDXT()
                {
                    int i;
                    for (i = (int)(0); (i) < (32); i++)
                    {
                        stb__Expand5[i] = (byte)((i << 3) | (i >> 2));
                    }
                    for (i = (int)(0); (i) < (64); i++)
                    {
                        stb__Expand6[i] = (byte)((i << 2) | (i >> 4));
                    }
                    for (i = (int)(0); (i) < (256 + 16); i++)
                    {
                        int v = (int)((i - 8) < (0) ? 0 : (i - 8) > (255) ? 255 : i - 8);
                        stb__QuantRBTab[i] = (byte)(stb__Expand5[stb__Mul8Bit((int)(v), (int)(31))]);
                        stb__QuantGTab[i] = (byte)(stb__Expand6[stb__Mul8Bit((int)(v), (int)(63))]);
                    }
                    stb__PrepareOptTable(stb__OMatch5, stb__Expand5, (int)(32));
                    stb__PrepareOptTable(stb__OMatch6, stb__Expand6, (int)(64));
                }

                public static void stb_compress_dxt_block(byte* dest, byte* src, int alpha, int mode)
                {
                    if ((init) != 0)
                    {
                        stb__InitDXT();
                        init = (int)(0);
                    }

                    if ((alpha) != 0)
                    {
                        stb__CompressAlphaBlock(dest, src + 3, (int)(4));
                        dest += 8;
                    }

                    stb__CompressColourBlock(dest, src, (int)(mode));
                }

                public static void stb_compress_bc4_block(byte* dest, byte* src)
                {
                    stb__CompressAlphaBlock(dest, src, (int)(1));
                }

                public static void stb_compress_bc5_block(byte* dest, byte* src)
                {
                    stb__CompressAlphaBlock(dest, src, (int)(2));
                    stb__CompressAlphaBlock(dest + 8, src + 1, (int)(2));
                }
            }
            #endregion

            #region STBIMAGE
            internal static unsafe partial class StbImage
            {
                public static string LastError;

                public const int STBI__ZFAST_BITS = 9;

                public delegate int ReadCallback(void* user, sbyte* data, int size);

                public delegate int SkipCallback(void* user, int n);

                public delegate int EofCallback(void* user);

                public delegate void idct_block_kernel(byte* output, int out_stride, short* data);

                public delegate void YCbCr_to_RGB_kernel(
                    byte* output, byte* y, byte* pcb, byte* pcr, int count, int step);

                public delegate byte* Resampler(byte* a, byte* b, byte* c, int d, int e);

                public static string stbi__g_failure_reason;
                public static int stbi__vertically_flip_on_load;

                public class stbi_io_callbacks
                {
                    public ReadCallback read;
                    public SkipCallback skip;
                    public EofCallback eof;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct img_comp
                {
                    public int id;
                    public int h, v;
                    public int tq;
                    public int hd, ha;
                    public int dc_pred;

                    public int x, y, w2, h2;
                    public byte* data;
                    public void* raw_data;
                    public void* raw_coeff;
                    public byte* linebuf;
                    public short* coeff; // progressive only
                    public int coeff_w, coeff_h; // number of 8x8 coefficient blocks
                }

                public class stbi__jpeg
                {
                    public stbi__context s;
                    public readonly stbi__huffman* huff_dc = (stbi__huffman*)stbi__malloc(4 * sizeof(stbi__huffman));
                    public readonly stbi__huffman* huff_ac = (stbi__huffman*)stbi__malloc(4 * sizeof(stbi__huffman));
                    public readonly ushort*[] dequant;

                    public readonly short*[] fast_ac;

                    // sizes for components, interleaved MCUs
                    public int img_h_max, img_v_max;
                    public int img_mcu_x, img_mcu_y;
                    public int img_mcu_w, img_mcu_h;

                    // definition of jpeg image component
                    public img_comp[] img_comp = new img_comp[4];

                    public uint code_buffer; // jpeg entropy-coded buffer
                    public int code_bits; // number of valid bits
                    public byte marker; // marker seen while filling entropy buffer
                    public int nomore; // flag if we saw a marker so must stop

                    public int progressive;
                    public int spec_start;
                    public int spec_end;
                    public int succ_high;
                    public int succ_low;
                    public int eob_run;
                    public int jfif;
                    public int app14_colour_transform; // Adobe APP14 tag
                    public int rgb;

                    public int scan_n;
                    public int* order = (int*)stbi__malloc(4 * sizeof(int));
                    public int restart_interval, todo;

                    // kernels
                    public idct_block_kernel idct_block_kernel;
                    public YCbCr_to_RGB_kernel YCbCr_to_RGB_kernel;
                    public Resampler resample_row_hv_2_kernel;

                    public stbi__jpeg()
                    {
                        for (var i = 0; i < 4; ++i)
                        {
                            huff_ac[i] = new stbi__huffman();
                            huff_dc[i] = new stbi__huffman();
                        }

                        for (var i = 0; i < img_comp.Length; ++i)
                        {
                            img_comp[i] = new img_comp();
                        }

                        fast_ac = new short*[4];
                        for (var i = 0; i < fast_ac.Length; ++i)
                        {
                            fast_ac[i] = (short*)stbi__malloc((1 << STBI__ZFAST_BITS) * sizeof(short));
                        }

                        dequant = new ushort*[4];
                        for (var i = 0; i < dequant.Length; ++i)
                        {
                            dequant[i] = (ushort*)stbi__malloc(64 * sizeof(ushort));
                        }
                    }
                };

                public class stbi__resample
                {
                    public Resampler resample;
                    public byte* line0;
                    public byte* line1;
                    public int hs;
                    public int vs;
                    public int w_lores;
                    public int ystep;
                    public int ypos;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct stbi__gif_lzw
                {
                    public short prefix;
                    public byte first;
                    public byte suffix;
                }

                public class stbi__gif
                {
                    public int w;
                    public int h;
                    public byte* _out_;
                    public byte* old_out;
                    public int flags;
                    public int bgindex;
                    public int ratio;
                    public int transparent;
                    public int eflags;
                    public int delay;
                    public byte* pal;
                    public byte* lpal;
                    public stbi__gif_lzw* codes;
                    public byte* colour_table;
                    public int parse;
                    public int step;
                    public int lflags;
                    public int start_x;
                    public int start_y;
                    public int max_x;
                    public int max_y;
                    public int cur_x;
                    public int cur_y;
                    public int line_size;

                    public stbi__gif()
                    {
                        codes = (stbi__gif_lzw*)stbi__malloc(4096 * sizeof(stbi__gif_lzw));
                        pal = (byte*)stbi__malloc(256 * 4 * sizeof(byte));
                        lpal = (byte*)stbi__malloc(256 * 4 * sizeof(byte));
                    }
                }

                private static void* stbi__malloc(int size)
                {
                    return CRuntime.malloc((ulong)size);
                }

                private static void* stbi__malloc(ulong size)
                {
                    return stbi__malloc((int)size);
                }

                private static int stbi__err(string str)
                {
                    LastError = str;
                    return 0;
                }

                public static void stbi__gif_parse_colourtable(stbi__context s, byte* pal, int num_entries, int transp)
                {
                    int i;
                    for (i = 0; (i) < (num_entries); ++i)
                    {
                        pal[i * 4 + 2] = stbi__get8(s);
                        pal[i * 4 + 1] = stbi__get8(s);
                        pal[i * 4] = stbi__get8(s);
                        pal[i * 4 + 3] = (byte)(transp == i ? 0 : 255);
                    }
                }

                public static Lot<byte[], int, int> LoadFromMemory(byte[] bytes)
                {
                    byte* result;
                    int x, y, comp;
                    fixed (byte* b = bytes)
                    {
                        result = stbi_load_from_memory(b, bytes.Length, &x, &y, &comp, 4);
                    }

                    //image.format = req_comp == 4 ? PixelFormats.RGB888 : PixelFormats.RGB24;

                    if (result == null)
                    {
                        throw new Exception(LastError);
                    }

                    // Convert to array
                    var data = new byte[x * y * 4];
                    Marshal.Copy(new IntPtr(result), data, 0, data.Length);
                    CRuntime.free(result);
                    return Lot.Create(data, x, y);
                }

            }
            #endregion

            #region STB IMAGE2
            unsafe partial class StbImage
            {
                public class stbi__context
                {
                    public uint img_x;
                    public uint img_y;
                    public int img_n;
                    public int img_out_n;
                    public stbi_io_callbacks io = new stbi_io_callbacks();
                    public void* io_user_data;
                    public int read_from_callbacks;
                    public int buflen;
                    public byte* buffer_start = (byte*)stbi__malloc(128);
                    public byte* img_buffer;
                    public byte* img_buffer_end;
                    public byte* img_buffer_original;
                    public byte* img_buffer_original_end;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct stbi__result_info
                {
                    public int bits_per_channel;
                    public int num_channels;
                    public int channel_order;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct stbi__huffman
                {
                    public fixed byte fast[1 << 9];
                    public fixed ushort code[256];
                    public fixed byte values[256];
                    public fixed byte size[257];
                    public fixed uint maxcode[18];
                    public fixed int delta[17];
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct stbi__zhuffman
                {
                    public fixed ushort fast[1 << 9];
                    public fixed ushort firstcode[16];
                    public fixed int maxcode[17];
                    public fixed ushort firstsymbol[16];
                    public fixed byte size[288];
                    public fixed ushort value[288];
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct stbi__zbuf
                {
                    public byte* zbuffer;
                    public byte* zbuffer_end;
                    public int num_bits;
                    public uint code_buffer;
                    public sbyte* zout;
                    public sbyte* zout_start;
                    public sbyte* zout_end;
                    public int z_expandable;
                    public stbi__zhuffman z_length;
                    public stbi__zhuffman z_distance;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct stbi__pngchunk
                {
                    public uint length;
                    public uint type;
                }

                public class stbi__png
                {
                    public stbi__context s = new stbi__context();
                    public byte* idata;
                    public byte* expanded;
                    public byte* _out_;
                    public int depth;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct stbi__bmp_data
                {
                    public int bpp;
                    public int offset;
                    public int hsz;
                    public uint mr;
                    public uint mg;
                    public uint mb;
                    public uint ma;
                    public uint all_a;
                }

                public const int STBI_default = 0;
                public const int STBI_grey = 1;
                public const int STBI_grey_alpha = 2;
                public const int STBI_rgb = 3;
                public const int STBI_rgb_alpha = 4;
                public const int STBI_ORDER_RGB = 0;
                public const int STBI_ORDER_BGR = 1;
                public const int STBI__SCAN_load = 0;
                public const int STBI__SCAN_type = 1;
                public const int STBI__SCAN_header = 2;
                public const int STBI__F_none = 0;
                public const int STBI__F_sub = 1;
                public const int STBI__F_up = 2;
                public const int STBI__F_avg = 3;
                public const int STBI__F_paeth = 4;
                public const int STBI__F_avg_first = 5;
                public const int STBI__F_paeth_first = 6;
                public static float stbi__h2l_gamma_i = (float)(1.0f / 2.2f);
                public static float stbi__h2l_scale_i = (float)(1.0f);

                public static uint[] stbi__bmask =
                {
            0, 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023, 2047, 4095, 8191, 16383, 32767, 65535
        };

                public static int[] stbi__jbias =
                {
            0, -1, -3, -7, -15, -31, -63, -127, -255, -511, -1023, -2047, -4095, -8191, -16383,
            -32767
        };

                public static byte[] stbi__jpeg_dezigzag =
                {
            0, 1, 8, 16, 9, 2, 3, 10, 17, 24, 32, 25, 18, 11, 4, 5, 12, 19, 26, 33, 40,
            48, 41, 34, 27, 20, 13, 6, 7, 14, 21, 28, 35, 42, 49, 56, 57, 50, 43, 36, 29, 22, 15, 23, 30, 37, 44, 51, 58, 59, 52,
            45, 38, 31, 39, 46, 53, 60, 61, 54, 47, 55, 62, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63
        };

                public static int[] stbi__zlength_base =
                {
            3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27, 31, 35, 43, 51, 59, 67,
            83, 99, 115, 131, 163, 195, 227, 258, 0, 0
        };

                public static int[] stbi__zlength_extra =
                {
            0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5,
            5, 5, 5, 0, 0, 0
        };

                public static int[] stbi__zdist_base =
                {
            1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193, 257, 385, 513, 769,
            1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577, 0, 0
        };

                public static int[] stbi__zdist_extra =
                {
            0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11,
            11, 12, 12, 13, 13
        };

                public static byte[] length_dezigzag = { 16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15 };

                public static byte[] stbi__zdefault_length =
                {
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
            9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
            9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 7, 7,
            7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 8, 8, 8, 8, 8, 8, 8
        };

                public static byte[] stbi__zdefault_distance =
                {
            5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5
        };

                public static byte[] png_sig = { 137, 80, 78, 71, 13, 10, 26, 10 };

                public static byte[] first_row_filter =
                {
            STBI__F_none, STBI__F_sub, STBI__F_none, STBI__F_avg_first,
            STBI__F_paeth_first
        };

                public static byte[] stbi__depth_scale_table = { 0, 0xff, 0x55, 0, 0x11, 0, 0, 0, 0x01 };
                public static int stbi__unpremultiply_on_load = (int)(0);
                public static int stbi__de_iphone_flag = (int)(0);

                public static void stbi__start_mem(stbi__context s, byte* buffer, int len)
                {
                    s.io.read = (null);
                    s.read_from_callbacks = (int)(0);
                    s.img_buffer = s.img_buffer_original = buffer;
                    s.img_buffer_end = s.img_buffer_original_end = buffer + len;
                }

                public static void stbi__start_callbacks(stbi__context s, stbi_io_callbacks c, void* user)
                {
                    s.io = (stbi_io_callbacks)(c);
                    s.io_user_data = user;
                    s.buflen = 128;
                    s.read_from_callbacks = (int)(1);
                    s.img_buffer_original = s.buffer_start;
                    stbi__refill_buffer(s);
                    s.img_buffer_original_end = s.img_buffer_end;
                }

                public static void stbi__rewind(stbi__context s)
                {
                    s.img_buffer = s.img_buffer_original;
                    s.img_buffer_end = s.img_buffer_original_end;
                }

                public static int stbi__addsizes_valid(int a, int b)
                {
                    if ((b) < (0)) return (int)(0);
                    return (a <= 2147483647 - b) ? 1 : 0;
                }

                public static int stbi__mul2sizes_valid(int a, int b)
                {
                    if (((a) < (0)) || ((b) < (0))) return (int)(0);
                    if ((b) == (0)) return (int)(1);
                    return (a <= 2147483647 / b) ? 1 : 0;
                }

                public static int stbi__mad2sizes_valid(int a, int b, int add)
                {
                    return
                        (int)
                            (((stbi__mul2sizes_valid((int)(a), (int)(b))) != 0) && ((stbi__addsizes_valid((int)(a * b), (int)(add))) != 0)
                                ? 1
                                : 0);
                }

                public static int stbi__mad3sizes_valid(int a, int b, int c, int add)
                {
                    return
                        (int)
                            ((((stbi__mul2sizes_valid((int)(a), (int)(b))) != 0) && ((stbi__mul2sizes_valid((int)(a * b), (int)(c))) != 0)) &&
                             ((stbi__addsizes_valid((int)(a * b * c), (int)(add))) != 0)
                                ? 1
                                : 0);
                }

                public static int stbi__mad4sizes_valid(int a, int b, int c, int d, int add)
                {
                    return
                        (int)
                            (((((stbi__mul2sizes_valid((int)(a), (int)(b))) != 0) && ((stbi__mul2sizes_valid((int)(a * b), (int)(c))) != 0)) &&
                              ((stbi__mul2sizes_valid((int)(a * b * c), (int)(d))) != 0)) &&
                             ((stbi__addsizes_valid((int)(a * b * c * d), (int)(add))) != 0)
                                ? 1
                                : 0);
                }

                public static void* stbi__malloc_mad2(int a, int b, int add)
                {
                    if (stbi__mad2sizes_valid((int)(a), (int)(b), (int)(add)) == 0) return (null);
                    return stbi__malloc((ulong)(a * b + add));
                }

                public static void* stbi__malloc_mad3(int a, int b, int c, int add)
                {
                    if (stbi__mad3sizes_valid((int)(a), (int)(b), (int)(c), (int)(add)) == 0) return (null);
                    return stbi__malloc((ulong)(a * b * c + add));
                }

                public static void* stbi__malloc_mad4(int a, int b, int c, int d, int add)
                {
                    if (stbi__mad4sizes_valid((int)(a), (int)(b), (int)(c), (int)(d), (int)(add)) == 0) return (null);
                    return stbi__malloc((ulong)(a * b * c * d + add));
                }

                public static void stbi_set_flip_vertically_on_load(int flag_true_if_should_flip)
                {
                    stbi__vertically_flip_on_load = (int)(flag_true_if_should_flip);
                }

                public static void* stbi__load_main(stbi__context s, int* x, int* y, int* comp, int req_comp, stbi__result_info* ri,
                    int bpc)
                {
                    ri->bits_per_channel = (int)(8);
                    ri->channel_order = (int)(STBI_ORDER_RGB);
                    ri->num_channels = (int)(0);
                    if ((stbi__jpeg_test(s)) != 0) return stbi__jpeg_load(s, x, y, comp, (int)(req_comp), ri);
                    if ((stbi__png_test(s)) != 0) return stbi__png_load(s, x, y, comp, (int)(req_comp), ri);
                    if ((stbi__bmp_test(s)) != 0) return stbi__bmp_load(s, x, y, comp, (int)(req_comp), ri);
                    if ((stbi__gif_test(s)) != 0) return stbi__gif_load(s, x, y, comp, (int)(req_comp), ri);
                    if ((stbi__psd_test(s)) != 0) return stbi__psd_load(s, x, y, comp, (int)(req_comp), ri, (int)(bpc));
                    if ((stbi__tga_test(s)) != 0) return stbi__tga_load(s, x, y, comp, (int)(req_comp), ri);
                    return ((byte*)((ulong)((stbi__err("unknown image type")) != 0 ? ((byte*)null) : (null))));
                }

                public static byte* stbi__convert_16_to_8(ushort* orig, int w, int h, int channels)
                {
                    int i;
                    int img_len = (int)(w * h * channels);
                    byte* reduced;
                    reduced = (byte*)(stbi__malloc((ulong)(img_len)));
                    if ((reduced) == (null)) return ((byte*)((ulong)((stbi__err("outofmem")) != 0 ? ((byte*)null) : (null))));
                    for (i = (int)(0); (i) < (img_len); ++i)
                    {
                        reduced[i] = ((byte)((orig[i] >> 8) & 0xFF));
                    }
                    CRuntime.free(orig);
                    return reduced;
                }

                public static ushort* stbi__convert_8_to_16(byte* orig, int w, int h, int channels)
                {
                    int i;
                    int img_len = (int)(w * h * channels);
                    ushort* enlarged;
                    enlarged = (ushort*)(stbi__malloc((ulong)(img_len * 2)));
                    if ((enlarged) == (null))
                        return (ushort*)((byte*)((ulong)((stbi__err("outofmem")) != 0 ? ((byte*)null) : (null))));
                    for (i = (int)(0); (i) < (img_len); ++i)
                    {
                        enlarged[i] = ((ushort)((orig[i] << 8) + orig[i]));
                    }
                    CRuntime.free(orig);
                    return enlarged;
                }

                public static void stbi__vertical_flip(void* image, int w, int h, int bytes_per_pixel)
                {
                    int row;
                    ulong bytes_per_row = (ulong)(w * bytes_per_pixel);
                    byte* temp = stackalloc byte[2048];
                    byte* bytes = (byte*)(image);
                    for (row = (int)(0); (row) < (h >> 1); row++)
                    {
                        byte* row0 = bytes + (ulong)row * bytes_per_row;
                        byte* row1 = bytes + (ulong)(h - row - 1) * bytes_per_row;
                        ulong bytes_left = (ulong)(bytes_per_row);
                        while ((bytes_left) != 0)
                        {
                            ulong bytes_copy = (ulong)(((bytes_left) < (2048)) ? bytes_left : 2048);
                            CRuntime.memcpy(temp, row0, (ulong)(bytes_copy));
                            CRuntime.memcpy(row0, row1, (ulong)(bytes_copy));
                            CRuntime.memcpy(row1, temp, (ulong)(bytes_copy));
                            row0 += bytes_copy;
                            row1 += bytes_copy;
                            bytes_left -= (ulong)(bytes_copy);
                        }
                    }
                }

                public static byte* stbi__load_and_postprocess_8bit(stbi__context s, int* x, int* y, int* comp, int req_comp)
                {
                    stbi__result_info ri = new stbi__result_info();
                    void* result = stbi__load_main(s, x, y, comp, (int)(req_comp), &ri, (int)(8));
                    if ((result) == (null)) return (null);
                    if (ri.bits_per_channel != 8)
                    {
                        result = stbi__convert_16_to_8((ushort*)(result), (int)(*x), (int)(*y),
                            (int)((req_comp) == (0) ? *comp : req_comp));
                        ri.bits_per_channel = (int)(8);
                    }

                    if ((stbi__vertically_flip_on_load) != 0)
                    {
                        int channels = (int)((req_comp) != 0 ? req_comp : *comp);
                        stbi__vertical_flip(result, (int)(*x), (int)(*y), (int)(channels));
                    }

                    return (byte*)(result);
                }

                public static ushort* stbi__load_and_postprocess_16bit(stbi__context s, int* x, int* y, int* comp, int req_comp)
                {
                    stbi__result_info ri = new stbi__result_info();
                    void* result = stbi__load_main(s, x, y, comp, (int)(req_comp), &ri, (int)(16));
                    if ((result) == (null)) return (null);
                    if (ri.bits_per_channel != 16)
                    {
                        result = stbi__convert_8_to_16((byte*)(result), (int)(*x), (int)(*y),
                            (int)((req_comp) == (0) ? *comp : req_comp));
                        ri.bits_per_channel = (int)(16);
                    }

                    if ((stbi__vertically_flip_on_load) != 0)
                    {
                        int channels = (int)((req_comp) != 0 ? req_comp : *comp);
                        stbi__vertical_flip(result, (int)(*x), (int)(*y), (int)(channels * 2));
                    }

                    return (ushort*)(result);
                }

                public static ushort* stbi_load_16_from_memory(byte* buffer, int len, int* x, int* y, int* channels_in_file,
                    int desired_channels)
                {
                    stbi__context s = new stbi__context();
                    stbi__start_mem(s, buffer, (int)(len));
                    return stbi__load_and_postprocess_16bit(s, x, y, channels_in_file, (int)(desired_channels));
                }

                public static ushort* stbi_load_16_from_callbacks(stbi_io_callbacks clbk, void* user, int* x, int* y,
                    int* channels_in_file, int desired_channels)
                {
                    stbi__context s = new stbi__context();
                    stbi__start_callbacks(s, clbk, user);
                    return stbi__load_and_postprocess_16bit(s, x, y, channels_in_file, (int)(desired_channels));
                }

                public static byte* stbi_load_from_memory(byte* buffer, int len, int* x, int* y, int* comp, int req_comp)
                {
                    stbi__context s = new stbi__context();
                    stbi__start_mem(s, buffer, (int)(len));
                    return stbi__load_and_postprocess_8bit(s, x, y, comp, (int)(req_comp));
                }

                public static byte* stbi_load_from_callbacks(stbi_io_callbacks clbk, void* user, int* x, int* y, int* comp,
                    int req_comp)
                {
                    stbi__context s = new stbi__context();
                    stbi__start_callbacks(s, clbk, user);
                    return stbi__load_and_postprocess_8bit(s, x, y, comp, (int)(req_comp));
                }

                public static void stbi_hdr_to_ldr_gamma(float gamma)
                {
                    stbi__h2l_gamma_i = (float)(1 / gamma);
                }

                public static void stbi_hdr_to_ldr_scale(float scale)
                {
                    stbi__h2l_scale_i = (float)(1 / scale);
                }

                public static void stbi__refill_buffer(stbi__context s)
                {
                    int n = (int)(s.io.read(s.io_user_data, (sbyte*)(s.buffer_start), (int)(s.buflen)));
                    if ((n) == (0))
                    {
                        s.read_from_callbacks = (int)(0);
                        s.img_buffer = s.buffer_start;
                        s.img_buffer_end = s.buffer_start;
                        s.img_buffer_end++;
                        *s.img_buffer = (byte)(0);
                    }
                    else
                    {
                        s.img_buffer = s.buffer_start;
                        s.img_buffer_end = s.buffer_start;
                        s.img_buffer_end += n;
                    }

                }

                public static byte stbi__get8(stbi__context s)
                {
                    if ((s.img_buffer) < (s.img_buffer_end)) return (byte)(*s.img_buffer++);
                    if ((s.read_from_callbacks) != 0)
                    {
                        stbi__refill_buffer(s);
                        return (byte)(*s.img_buffer++);
                    }

                    return (byte)(0);
                }

                public static int stbi__at_eof(stbi__context s)
                {
                    if ((s.io.read) != null)
                    {
                        if (s.io.eof(s.io_user_data) == 0) return (int)(0);
                        if ((s.read_from_callbacks) == (0)) return (int)(1);
                    }

                    return (int)((s.img_buffer) >= (s.img_buffer_end) ? 1 : 0);
                }

                public static void stbi__skip(stbi__context s, int n)
                {
                    if ((n) < (0))
                    {
                        s.img_buffer = s.img_buffer_end;
                        return;
                    }

                    if ((s.io.read) != null)
                    {
                        int blen = (int)(s.img_buffer_end - s.img_buffer);
                        if ((blen) < (n))
                        {
                            s.img_buffer = s.img_buffer_end;
                            s.io.skip(s.io_user_data, (int)(n - blen));
                            return;
                        }
                    }

                    s.img_buffer += n;
                }

                public static int stbi__getn(stbi__context s, byte* buffer, int n)
                {
                    if ((s.io.read) != null)
                    {
                        int blen = (int)(s.img_buffer_end - s.img_buffer);
                        if ((blen) < (n))
                        {
                            int res;
                            int count;
                            CRuntime.memcpy(buffer, s.img_buffer, (ulong)(blen));
                            count = (int)(s.io.read(s.io_user_data, (sbyte*)(buffer) + blen, (int)(n - blen)));
                            res = (int)((count) == (n - blen) ? 1 : 0);
                            s.img_buffer = s.img_buffer_end;
                            return (int)(res);
                        }
                    }

                    if (s.img_buffer + n <= s.img_buffer_end)
                    {
                        CRuntime.memcpy(buffer, s.img_buffer, (ulong)(n));
                        s.img_buffer += n;
                        return (int)(1);
                    }
                    else return (int)(0);
                }

                public static int stbi__get16be(stbi__context s)
                {
                    int z = (int)(stbi__get8(s));
                    return (int)((z << 8) + stbi__get8(s));
                }

                public static uint stbi__get32be(stbi__context s)
                {
                    uint z = (uint)(stbi__get16be(s));
                    return (uint)((z << 16) + stbi__get16be(s));
                }

                public static int stbi__get16le(stbi__context s)
                {
                    int z = (int)(stbi__get8(s));
                    return (int)(z + (stbi__get8(s) << 8));
                }

                public static uint stbi__get32le(stbi__context s)
                {
                    uint z = (uint)(stbi__get16le(s));
                    return (uint)(z + (stbi__get16le(s) << 16));
                }

                public static byte stbi__compute_y(int r, int g, int b)
                {
                    return (byte)(((r * 77) + (g * 150) + (29 * b)) >> 8);
                }

                public static byte* stbi__convert_format(byte* data, int img_n, int req_comp, uint x, uint y)
                {
                    int i;
                    int j;
                    byte* good;
                    if ((req_comp) == (img_n)) return data;
                    good = (byte*)(stbi__malloc_mad3((int)(req_comp), (int)(x), (int)(y), (int)(0)));
                    if ((good) == (null))
                    {
                        CRuntime.free(data);
                        return ((byte*)((ulong)((stbi__err("outofmem")) != 0 ? ((byte*)null) : (null))));
                    }

                    for (j = (int)(0); (j) < ((int)(y)); ++j)
                    {
                        byte* src = data + j * x * img_n;
                        byte* dest = good + j * x * req_comp;
                        switch (((img_n) * 8 + (req_comp)))
                        {
                            case ((1) * 8 + (2)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 1, dest += 2)
                                {
                                    dest[0] = (byte)(src[0]);
                                    dest[1] = (byte)(255);
                                }
                                break;
                            case ((1) * 8 + (3)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 1, dest += 3)
                                {
                                    dest[0] = (byte)(dest[1] = (byte)(dest[2] = (byte)(src[0])));
                                }
                                break;
                            case ((1) * 8 + (4)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 1, dest += 4)
                                {
                                    dest[0] = (byte)(dest[1] = (byte)(dest[2] = (byte)(src[0])));
                                    dest[3] = (byte)(255);
                                }
                                break;
                            case ((2) * 8 + (1)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 2, dest += 1)
                                {
                                    dest[0] = (byte)(src[0]);
                                }
                                break;
                            case ((2) * 8 + (3)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 2, dest += 3)
                                {
                                    dest[0] = (byte)(dest[1] = (byte)(dest[2] = (byte)(src[0])));
                                }
                                break;
                            case ((2) * 8 + (4)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 2, dest += 4)
                                {
                                    dest[0] = (byte)(dest[1] = (byte)(dest[2] = (byte)(src[0])));
                                    dest[3] = (byte)(src[1]);
                                }
                                break;
                            case ((3) * 8 + (4)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 3, dest += 4)
                                {
                                    dest[0] = (byte)(src[0]);
                                    dest[1] = (byte)(src[1]);
                                    dest[2] = (byte)(src[2]);
                                    dest[3] = (byte)(255);
                                }
                                break;
                            case ((3) * 8 + (1)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 3, dest += 1)
                                {
                                    dest[0] = (byte)(stbi__compute_y((int)(src[0]), (int)(src[1]), (int)(src[2])));
                                }
                                break;
                            case ((3) * 8 + (2)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 3, dest += 2)
                                {
                                    dest[0] = (byte)(stbi__compute_y((int)(src[0]), (int)(src[1]), (int)(src[2])));
                                    dest[1] = (byte)(255);
                                }
                                break;
                            case ((4) * 8 + (1)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 4, dest += 1)
                                {
                                    dest[0] = (byte)(stbi__compute_y((int)(src[0]), (int)(src[1]), (int)(src[2])));
                                }
                                break;
                            case ((4) * 8 + (2)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 4, dest += 2)
                                {
                                    dest[0] = (byte)(stbi__compute_y((int)(src[0]), (int)(src[1]), (int)(src[2])));
                                    dest[1] = (byte)(src[3]);
                                }
                                break;
                            case ((4) * 8 + (3)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 4, dest += 3)
                                {
                                    dest[0] = (byte)(src[0]);
                                    dest[1] = (byte)(src[1]);
                                    dest[2] = (byte)(src[2]);
                                }
                                break;
                            default:
                                return ((byte*)((ulong)((stbi__err("0")) != 0 ? ((byte*)null) : (null))));
                        }
                    }
                    CRuntime.free(data);
                    return good;
                }

                public static ushort stbi__compute_y_16(int r, int g, int b)
                {
                    return (ushort)(((r * 77) + (g * 150) + (29 * b)) >> 8);
                }

                public static ushort* stbi__convert_format16(ushort* data, int img_n, int req_comp, uint x, uint y)
                {
                    int i;
                    int j;
                    ushort* good;
                    if ((req_comp) == (img_n)) return data;
                    good = (ushort*)(stbi__malloc((ulong)(req_comp * x * y * 2)));
                    if ((good) == (null))
                    {
                        CRuntime.free(data);
                        return (ushort*)((byte*)((ulong)((stbi__err("outofmem")) != 0 ? ((byte*)null) : (null))));
                    }

                    for (j = (int)(0); (j) < ((int)(y)); ++j)
                    {
                        ushort* src = data + j * x * img_n;
                        ushort* dest = good + j * x * req_comp;
                        switch (((img_n) * 8 + (req_comp)))
                        {
                            case ((1) * 8 + (2)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 1, dest += 2)
                                {
                                    dest[0] = (ushort)(src[0]);
                                    dest[1] = (ushort)(0xffff);
                                }
                                break;
                            case ((1) * 8 + (3)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 1, dest += 3)
                                {
                                    dest[0] = (ushort)(dest[1] = (ushort)(dest[2] = (ushort)(src[0])));
                                }
                                break;
                            case ((1) * 8 + (4)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 1, dest += 4)
                                {
                                    dest[0] = (ushort)(dest[1] = (ushort)(dest[2] = (ushort)(src[0])));
                                    dest[3] = (ushort)(0xffff);
                                }
                                break;
                            case ((2) * 8 + (1)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 2, dest += 1)
                                {
                                    dest[0] = (ushort)(src[0]);
                                }
                                break;
                            case ((2) * 8 + (3)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 2, dest += 3)
                                {
                                    dest[0] = (ushort)(dest[1] = (ushort)(dest[2] = (ushort)(src[0])));
                                }
                                break;
                            case ((2) * 8 + (4)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 2, dest += 4)
                                {
                                    dest[0] = (ushort)(dest[1] = (ushort)(dest[2] = (ushort)(src[0])));
                                    dest[3] = (ushort)(src[1]);
                                }
                                break;
                            case ((3) * 8 + (4)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 3, dest += 4)
                                {
                                    dest[0] = (ushort)(src[0]);
                                    dest[1] = (ushort)(src[1]);
                                    dest[2] = (ushort)(src[2]);
                                    dest[3] = (ushort)(0xffff);
                                }
                                break;
                            case ((3) * 8 + (1)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 3, dest += 1)
                                {
                                    dest[0] = (ushort)(stbi__compute_y_16((int)(src[0]), (int)(src[1]), (int)(src[2])));
                                }
                                break;
                            case ((3) * 8 + (2)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 3, dest += 2)
                                {
                                    dest[0] = (ushort)(stbi__compute_y_16((int)(src[0]), (int)(src[1]), (int)(src[2])));
                                    dest[1] = (ushort)(0xffff);
                                }
                                break;
                            case ((4) * 8 + (1)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 4, dest += 1)
                                {
                                    dest[0] = (ushort)(stbi__compute_y_16((int)(src[0]), (int)(src[1]), (int)(src[2])));
                                }
                                break;
                            case ((4) * 8 + (2)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 4, dest += 2)
                                {
                                    dest[0] = (ushort)(stbi__compute_y_16((int)(src[0]), (int)(src[1]), (int)(src[2])));
                                    dest[1] = (ushort)(src[3]);
                                }
                                break;
                            case ((4) * 8 + (3)):
                                for (i = (int)(x - 1); (i) >= (0); --i, src += 4, dest += 3)
                                {
                                    dest[0] = (ushort)(src[0]);
                                    dest[1] = (ushort)(src[1]);
                                    dest[2] = (ushort)(src[2]);
                                }
                                break;
                            default:
                                return (ushort*)((byte*)((ulong)((stbi__err("0")) != 0 ? ((byte*)null) : (null))));
                        }
                    }
                    CRuntime.free(data);
                    return good;
                }

                public static int stbi__build_huffman(stbi__huffman* h, int* count)
                {
                    int i;
                    int j;
                    int k = (int)(0);
                    int code;
                    for (i = (int)(0); (i) < (16); ++i)
                    {
                        for (j = (int)(0); (j) < (count[i]); ++j)
                        {
                            h->size[k++] = ((byte)(i + 1));
                        }
                    }
                    h->size[k] = (byte)(0);
                    code = (int)(0);
                    k = (int)(0);
                    for (j = (int)(1); j <= 16; ++j)
                    {
                        h->delta[j] = (int)(k - code);
                        if ((h->size[k]) == (j))
                        {
                            while ((h->size[k]) == (j))
                            {
                                h->code[k++] = ((ushort)(code++));
                            }
                            if ((code - 1) >= (1 << j)) return (int)(stbi__err("bad code lengths"));
                        }
                        h->maxcode[j] = (uint)(code << (16 - j));
                        code <<= 1;
                    }
                    h->maxcode[j] = (uint)(0xffffffff);
                    CRuntime.memset(h->fast, (int)(255), (ulong)(1 << 9));
                    for (i = (int)(0); (i) < (k); ++i)
                    {
                        int s = (int)(h->size[i]);
                        if (s <= 9)
                        {
                            int c = (int)(h->code[i] << (9 - s));
                            int m = (int)(1 << (9 - s));
                            for (j = (int)(0); (j) < (m); ++j)
                            {
                                h->fast[c + j] = ((byte)(i));
                            }
                        }
                    }
                    return (int)(1);
                }

                public static void stbi__build_fast_ac(short* fast_ac, stbi__huffman* h)
                {
                    int i;
                    for (i = (int)(0); (i) < (1 << 9); ++i)
                    {
                        byte fast = (byte)(h->fast[i]);
                        fast_ac[i] = (short)(0);
                        if ((fast) < (255))
                        {
                            int rs = (int)(h->values[fast]);
                            int run = (int)((rs >> 4) & 15);
                            int magbits = (int)(rs & 15);
                            int len = (int)(h->size[fast]);
                            if (((magbits) != 0) && (len + magbits <= 9))
                            {
                                int k = (int)(((i << len) & ((1 << 9) - 1)) >> (9 - magbits));
                                int m = (int)(1 << (magbits - 1));
                                if ((k) < (m)) k += (int)((~0U << magbits) + 1);
                                if (((k) >= (-128)) && (k <= 127)) fast_ac[i] = ((short)((k << 8) + (run << 4) + (len + magbits)));
                            }
                        }
                    }
                }

                public static void stbi__grow_buffer_unsafe(stbi__jpeg j)
                {
                    do
                    {
                        int b = (int)((j.nomore) != 0 ? 0 : stbi__get8(j.s));
                        if ((b) == (0xff))
                        {
                            int c = (int)(stbi__get8(j.s));
                            while ((c) == (0xff))
                            {
                                c = (int)(stbi__get8(j.s));
                            }
                            if (c != 0)
                            {
                                j.marker = ((byte)(c));
                                j.nomore = (int)(1);
                                return;
                            }
                        }
                        j.code_buffer |= (uint)(b << (24 - j.code_bits));
                        j.code_bits += (int)(8);
                    } while (j.code_bits <= 24);
                }

                public static int stbi__jpeg_huff_decode(stbi__jpeg j, stbi__huffman* h)
                {
                    uint temp;
                    int c;
                    int k;
                    if ((j.code_bits) < (16)) stbi__grow_buffer_unsafe(j);
                    c = (int)((j.code_buffer >> (32 - 9)) & ((1 << 9) - 1));
                    k = (int)(h->fast[c]);
                    if ((k) < (255))
                    {
                        int s = (int)(h->size[k]);
                        if ((s) > (j.code_bits)) return (int)(-1);
                        j.code_buffer <<= s;
                        j.code_bits -= (int)(s);
                        return (int)(h->values[k]);
                    }

                    temp = (uint)(j.code_buffer >> 16);
                    for (k = (int)(9 + 1); ; ++k)
                    {
                        if ((temp) < (h->maxcode[k])) break;
                    }
                    if ((k) == (17))
                    {
                        j.code_bits -= (int)(16);
                        return (int)(-1);
                    }

                    if ((k) > (j.code_bits)) return (int)(-1);
                    c = (int)(((j.code_buffer >> (32 - k)) & stbi__bmask[k]) + h->delta[k]);
                    j.code_bits -= (int)(k);
                    j.code_buffer <<= k;
                    return (int)(h->values[c]);
                }

                public static int stbi__extend_receive(stbi__jpeg j, int n)
                {
                    uint k;
                    int sgn;
                    if ((j.code_bits) < (n)) stbi__grow_buffer_unsafe(j);
                    sgn = (int)((int)j.code_buffer >> 31);
                    k = (uint)(CRuntime._lrotl(j.code_buffer, (int)(n)));
                    j.code_buffer = (uint)(k & ~stbi__bmask[n]);
                    k &= (uint)(stbi__bmask[n]);
                    j.code_bits -= (int)(n);
                    return (int)(k + (stbi__jbias[n] & ~sgn));
                }

                public static int stbi__jpeg_get_bits(stbi__jpeg j, int n)
                {
                    uint k;
                    if ((j.code_bits) < (n)) stbi__grow_buffer_unsafe(j);
                    k = (uint)(CRuntime._lrotl(j.code_buffer, (int)(n)));
                    j.code_buffer = (uint)(k & ~stbi__bmask[n]);
                    k &= (uint)(stbi__bmask[n]);
                    j.code_bits -= (int)(n);
                    return (int)(k);
                }

                public static int stbi__jpeg_get_bit(stbi__jpeg j)
                {
                    uint k;
                    if ((j.code_bits) < (1)) stbi__grow_buffer_unsafe(j);
                    k = (uint)(j.code_buffer);
                    j.code_buffer <<= 1;
                    --j.code_bits;
                    return (int)(k & 0x80000000);
                }

                public static int stbi__jpeg_decode_block(stbi__jpeg j, short* data, stbi__huffman* hdc, stbi__huffman* hac,
                    short* fac, int b, ushort* dequant)
                {
                    int diff;
                    int dc;
                    int k;
                    int t;
                    if ((j.code_bits) < (16)) stbi__grow_buffer_unsafe(j);
                    t = (int)(stbi__jpeg_huff_decode(j, hdc));
                    if ((t) < (0)) return (int)(stbi__err("bad huffman code"));
                    CRuntime.memset(data, (int)(0), (ulong)(64 * sizeof(short)));
                    diff = (int)((t) != 0 ? stbi__extend_receive(j, (int)(t)) : 0);
                    dc = (int)(j.img_comp[b].dc_pred + diff);
                    j.img_comp[b].dc_pred = (int)(dc);
                    data[0] = ((short)(dc * dequant[0]));
                    k = (int)(1);
                    do
                    {
                        uint zig;
                        int c;
                        int r;
                        int s;
                        if ((j.code_bits) < (16)) stbi__grow_buffer_unsafe(j);
                        c = (int)((j.code_buffer >> (32 - 9)) & ((1 << 9) - 1));
                        r = (int)(fac[c]);
                        if ((r) != 0)
                        {
                            k += (int)((r >> 4) & 15);
                            s = (int)(r & 15);
                            j.code_buffer <<= s;
                            j.code_bits -= (int)(s);
                            zig = (uint)(stbi__jpeg_dezigzag[k++]);
                            data[zig] = ((short)((r >> 8) * dequant[zig]));
                        }
                        else
                        {
                            int rs = (int)(stbi__jpeg_huff_decode(j, hac));
                            if ((rs) < (0)) return (int)(stbi__err("bad huffman code"));
                            s = (int)(rs & 15);
                            r = (int)(rs >> 4);
                            if ((s) == (0))
                            {
                                if (rs != 0xf0) break;
                                k += (int)(16);
                            }
                            else
                            {
                                k += (int)(r);
                                zig = (uint)(stbi__jpeg_dezigzag[k++]);
                                data[zig] = ((short)(stbi__extend_receive(j, (int)(s)) * dequant[zig]));
                            }
                        }
                    } while ((k) < (64));
                    return (int)(1);
                }

                public static int stbi__jpeg_decode_block_prog_dc(stbi__jpeg j, short* data, stbi__huffman* hdc, int b)
                {
                    int diff;
                    int dc;
                    int t;
                    if (j.spec_end != 0) return (int)(stbi__err("can't merge dc and ac"));
                    if ((j.code_bits) < (16)) stbi__grow_buffer_unsafe(j);
                    if ((j.succ_high) == (0))
                    {
                        CRuntime.memset(data, (int)(0), (ulong)(64 * sizeof(short)));
                        t = (int)(stbi__jpeg_huff_decode(j, hdc));
                        diff = (int)((t) != 0 ? stbi__extend_receive(j, (int)(t)) : 0);
                        dc = (int)(j.img_comp[b].dc_pred + diff);
                        j.img_comp[b].dc_pred = (int)(dc);
                        data[0] = ((short)(dc << j.succ_low));
                    }
                    else
                    {
                        if ((stbi__jpeg_get_bit(j)) != 0) data[0] += ((short)(1 << j.succ_low));
                    }

                    return (int)(1);
                }

                public static int stbi__jpeg_decode_block_prog_ac(stbi__jpeg j, short* data, stbi__huffman* hac, short* fac)
                {
                    int k;
                    if ((j.spec_start) == (0)) return (int)(stbi__err("can't merge dc and ac"));
                    if ((j.succ_high) == (0))
                    {
                        int shift = (int)(j.succ_low);
                        if ((j.eob_run) != 0)
                        {
                            --j.eob_run;
                            return (int)(1);
                        }
                        k = (int)(j.spec_start);
                        do
                        {
                            uint zig;
                            int c;
                            int r;
                            int s;
                            if ((j.code_bits) < (16)) stbi__grow_buffer_unsafe(j);
                            c = (int)((j.code_buffer >> (32 - 9)) & ((1 << 9) - 1));
                            r = (int)(fac[c]);
                            if ((r) != 0)
                            {
                                k += (int)((r >> 4) & 15);
                                s = (int)(r & 15);
                                j.code_buffer <<= s;
                                j.code_bits -= (int)(s);
                                zig = (uint)(stbi__jpeg_dezigzag[k++]);
                                data[zig] = ((short)((r >> 8) << shift));
                            }
                            else
                            {
                                int rs = (int)(stbi__jpeg_huff_decode(j, hac));
                                if ((rs) < (0)) return (int)(stbi__err("bad huffman code"));
                                s = (int)(rs & 15);
                                r = (int)(rs >> 4);
                                if ((s) == (0))
                                {
                                    if ((r) < (15))
                                    {
                                        j.eob_run = (int)(1 << r);
                                        if ((r) != 0) j.eob_run += (int)(stbi__jpeg_get_bits(j, (int)(r)));
                                        --j.eob_run;
                                        break;
                                    }
                                    k += (int)(16);
                                }
                                else
                                {
                                    k += (int)(r);
                                    zig = (uint)(stbi__jpeg_dezigzag[k++]);
                                    data[zig] = ((short)(stbi__extend_receive(j, (int)(s)) << shift));
                                }
                            }
                        } while (k <= j.spec_end);
                    }
                    else
                    {
                        short bit = (short)(1 << j.succ_low);
                        if ((j.eob_run) != 0)
                        {
                            --j.eob_run;
                            for (k = (int)(j.spec_start); k <= j.spec_end; ++k)
                            {
                                short* p = &data[stbi__jpeg_dezigzag[k]];
                                if (*p != 0)
                                    if ((stbi__jpeg_get_bit(j)) != 0)
                                        if ((*p & bit) == (0))
                                        {
                                            if ((*p) > (0)) *p += (short)(bit);
                                            else *p -= (short)(bit);
                                        }
                            }
                        }
                        else
                        {
                            k = (int)(j.spec_start);
                            do
                            {
                                int r;
                                int s;
                                int rs = (int)(stbi__jpeg_huff_decode(j, hac));
                                if ((rs) < (0)) return (int)(stbi__err("bad huffman code"));
                                s = (int)(rs & 15);
                                r = (int)(rs >> 4);
                                if ((s) == (0))
                                {
                                    if ((r) < (15))
                                    {
                                        j.eob_run = (int)((1 << r) - 1);
                                        if ((r) != 0) j.eob_run += (int)(stbi__jpeg_get_bits(j, (int)(r)));
                                        r = (int)(64);
                                    }
                                    else
                                    {
                                    }
                                }
                                else
                                {
                                    if (s != 1) return (int)(stbi__err("bad huffman code"));
                                    if ((stbi__jpeg_get_bit(j)) != 0) s = (int)(bit);
                                    else s = (int)(-bit);
                                }
                                while (k <= j.spec_end)
                                {
                                    short* p = &data[stbi__jpeg_dezigzag[k++]];
                                    if (*p != 0)
                                    {
                                        if ((stbi__jpeg_get_bit(j)) != 0)
                                            if ((*p & bit) == (0))
                                            {
                                                if ((*p) > (0)) *p += (short)(bit);
                                                else *p -= (short)(bit);
                                            }
                                    }
                                    else
                                    {
                                        if ((r) == (0))
                                        {
                                            *p = ((short)(s));
                                            break;
                                        }
                                        --r;
                                    }
                                }
                            } while (k <= j.spec_end);
                        }
                    }

                    return (int)(1);
                }

                public static byte stbi__clamp(int x)
                {
                    if (((uint)(x)) > (255))
                    {
                        if ((x) < (0)) return (byte)(0);
                        if ((x) > (255)) return (byte)(255);
                    }

                    return (byte)(x);
                }

                public static void stbi__idct_block(byte* _out_, int out_stride, short* data)
                {
                    int i;
                    int* val = stackalloc int[64];
                    int* v = val;
                    byte* o;
                    short* d = ((short*)data);
                    for (i = (int)(0); (i) < (8); ++i, ++d, ++v)
                    {
                        if ((((((((d[8]) == (0)) && ((d[16]) == (0))) && ((d[24]) == (0))) && ((d[32]) == (0))) && ((d[40]) == (0))) &&
                             ((d[48]) == (0))) && ((d[56]) == (0)))
                        {
                            int dcterm = (int)(d[0] << 2);
                            v[0] =
                                (int)
                                    (v[8] =
                                        (int)(v[16] = (int)(v[24] = (int)(v[32] = (int)(v[40] = (int)(v[48] = (int)(v[56] = (int)(dcterm))))))));
                        }
                        else
                        {
                            int t0;
                            int t1;
                            int t2;
                            int t3;
                            int p1;
                            int p2;
                            int p3;
                            int p4;
                            int p5;
                            int x0;
                            int x1;
                            int x2;
                            int x3;
                            p2 = (int)(d[16]);
                            p3 = (int)(d[48]);
                            p1 = (int)((p2 + p3) * ((int)((0.5411961f) * 4096 + 0.5)));
                            t2 = (int)(p1 + p3 * ((int)((-1.847759065f) * 4096 + 0.5)));
                            t3 = (int)(p1 + p2 * ((int)((0.765366865f) * 4096 + 0.5)));
                            p2 = (int)(d[0]);
                            p3 = (int)(d[32]);
                            t0 = (int)((p2 + p3) << 12);
                            t1 = (int)((p2 - p3) << 12);
                            x0 = (int)(t0 + t3);
                            x3 = (int)(t0 - t3);
                            x1 = (int)(t1 + t2);
                            x2 = (int)(t1 - t2);
                            t0 = (int)(d[56]);
                            t1 = (int)(d[40]);
                            t2 = (int)(d[24]);
                            t3 = (int)(d[8]);
                            p3 = (int)(t0 + t2);
                            p4 = (int)(t1 + t3);
                            p1 = (int)(t0 + t3);
                            p2 = (int)(t1 + t2);
                            p5 = (int)((p3 + p4) * ((int)((1.175875602f) * 4096 + 0.5)));
                            t0 = (int)(t0 * ((int)((0.298631336f) * 4096 + 0.5)));
                            t1 = (int)(t1 * ((int)((2.053119869f) * 4096 + 0.5)));
                            t2 = (int)(t2 * ((int)((3.072711026f) * 4096 + 0.5)));
                            t3 = (int)(t3 * ((int)((1.501321110f) * 4096 + 0.5)));
                            p1 = (int)(p5 + p1 * ((int)((-0.899976223f) * 4096 + 0.5)));
                            p2 = (int)(p5 + p2 * ((int)((-2.562915447f) * 4096 + 0.5)));
                            p3 = (int)(p3 * ((int)((-1.961570560f) * 4096 + 0.5)));
                            p4 = (int)(p4 * ((int)((-0.390180644f) * 4096 + 0.5)));
                            t3 += (int)(p1 + p4);
                            t2 += (int)(p2 + p3);
                            t1 += (int)(p2 + p4);
                            t0 += (int)(p1 + p3);
                            x0 += (int)(512);
                            x1 += (int)(512);
                            x2 += (int)(512);
                            x3 += (int)(512);
                            v[0] = (int)((x0 + t3) >> 10);
                            v[56] = (int)((x0 - t3) >> 10);
                            v[8] = (int)((x1 + t2) >> 10);
                            v[48] = (int)((x1 - t2) >> 10);
                            v[16] = (int)((x2 + t1) >> 10);
                            v[40] = (int)((x2 - t1) >> 10);
                            v[24] = (int)((x3 + t0) >> 10);
                            v[32] = (int)((x3 - t0) >> 10);
                        }
                    }
                    for (i = (int)(0), v = val, o = _out_; (i) < (8); ++i, v += 8, o += out_stride)
                    {
                        int t0;
                        int t1;
                        int t2;
                        int t3;
                        int p1;
                        int p2;
                        int p3;
                        int p4;
                        int p5;
                        int x0;
                        int x1;
                        int x2;
                        int x3;
                        p2 = (int)(v[2]);
                        p3 = (int)(v[6]);
                        p1 = (int)((p2 + p3) * ((int)((0.5411961f) * 4096 + 0.5)));
                        t2 = (int)(p1 + p3 * ((int)((-1.847759065f) * 4096 + 0.5)));
                        t3 = (int)(p1 + p2 * ((int)((0.765366865f) * 4096 + 0.5)));
                        p2 = (int)(v[0]);
                        p3 = (int)(v[4]);
                        t0 = (int)((p2 + p3) << 12);
                        t1 = (int)((p2 - p3) << 12);
                        x0 = (int)(t0 + t3);
                        x3 = (int)(t0 - t3);
                        x1 = (int)(t1 + t2);
                        x2 = (int)(t1 - t2);
                        t0 = (int)(v[7]);
                        t1 = (int)(v[5]);
                        t2 = (int)(v[3]);
                        t3 = (int)(v[1]);
                        p3 = (int)(t0 + t2);
                        p4 = (int)(t1 + t3);
                        p1 = (int)(t0 + t3);
                        p2 = (int)(t1 + t2);
                        p5 = (int)((p3 + p4) * ((int)((1.175875602f) * 4096 + 0.5)));
                        t0 = (int)(t0 * ((int)((0.298631336f) * 4096 + 0.5)));
                        t1 = (int)(t1 * ((int)((2.053119869f) * 4096 + 0.5)));
                        t2 = (int)(t2 * ((int)((3.072711026f) * 4096 + 0.5)));
                        t3 = (int)(t3 * ((int)((1.501321110f) * 4096 + 0.5)));
                        p1 = (int)(p5 + p1 * ((int)((-0.899976223f) * 4096 + 0.5)));
                        p2 = (int)(p5 + p2 * ((int)((-2.562915447f) * 4096 + 0.5)));
                        p3 = (int)(p3 * ((int)((-1.961570560f) * 4096 + 0.5)));
                        p4 = (int)(p4 * ((int)((-0.390180644f) * 4096 + 0.5)));
                        t3 += (int)(p1 + p4);
                        t2 += (int)(p2 + p3);
                        t1 += (int)(p2 + p4);
                        t0 += (int)(p1 + p3);
                        x0 += (int)(65536 + (128 << 17));
                        x1 += (int)(65536 + (128 << 17));
                        x2 += (int)(65536 + (128 << 17));
                        x3 += (int)(65536 + (128 << 17));
                        o[0] = (byte)(stbi__clamp((int)((x0 + t3) >> 17)));
                        o[7] = (byte)(stbi__clamp((int)((x0 - t3) >> 17)));
                        o[1] = (byte)(stbi__clamp((int)((x1 + t2) >> 17)));
                        o[6] = (byte)(stbi__clamp((int)((x1 - t2) >> 17)));
                        o[2] = (byte)(stbi__clamp((int)((x2 + t1) >> 17)));
                        o[5] = (byte)(stbi__clamp((int)((x2 - t1) >> 17)));
                        o[3] = (byte)(stbi__clamp((int)((x3 + t0) >> 17)));
                        o[4] = (byte)(stbi__clamp((int)((x3 - t0) >> 17)));
                    }
                }

                public static byte stbi__get_marker(stbi__jpeg j)
                {
                    byte x;
                    if (j.marker != 0xff)
                    {
                        x = (byte)(j.marker);
                        j.marker = (byte)(0xff);
                        return (byte)(x);
                    }

                    x = (byte)(stbi__get8(j.s));
                    if (x != 0xff) return (byte)(0xff);
                    while ((x) == (0xff))
                    {
                        x = (byte)(stbi__get8(j.s));
                    }
                    return (byte)(x);
                }

                public static void stbi__jpeg_reset(stbi__jpeg j)
                {
                    j.code_bits = (int)(0);
                    j.code_buffer = (uint)(0);
                    j.nomore = (int)(0);
                    j.img_comp[0].dc_pred =
                        (int)(j.img_comp[1].dc_pred = (int)(j.img_comp[2].dc_pred = (int)(j.img_comp[3].dc_pred = (int)(0))));
                    j.marker = (byte)(0xff);
                    j.todo = (int)((j.restart_interval) != 0 ? j.restart_interval : 0x7fffffff);
                    j.eob_run = (int)(0);
                }

                public static int stbi__parse_entropy_coded_data(stbi__jpeg z)
                {
                    stbi__jpeg_reset(z);
                    if (z.progressive == 0)
                    {
                        if ((z.scan_n) == (1))
                        {
                            int i;
                            int j;
                            short* data = stackalloc short[64];
                            int n = (int)(z.order[0]);
                            int w = (int)((z.img_comp[n].x + 7) >> 3);
                            int h = (int)((z.img_comp[n].y + 7) >> 3);
                            for (j = (int)(0); (j) < (h); ++j)
                            {
                                for (i = (int)(0); (i) < (w); ++i)
                                {
                                    int ha = (int)(z.img_comp[n].ha);
                                    if (
                                        stbi__jpeg_decode_block(z, data, (stbi__huffman*)z.huff_dc + z.img_comp[n].hd, (stbi__huffman*)z.huff_ac + ha,
                                            z.fast_ac[ha], (int)(n), (ushort*)z.dequant[z.img_comp[n].tq]) == 0) return (int)(0);
                                    z.idct_block_kernel(z.img_comp[n].data + z.img_comp[n].w2 * j * 8 + i * 8, (int)(z.img_comp[n].w2), data);
                                    if (--z.todo <= 0)
                                    {
                                        if ((z.code_bits) < (24)) stbi__grow_buffer_unsafe(z);
                                        if (!(((z.marker) >= (0xd0)) && ((z.marker) <= 0xd7))) return (int)(1);
                                        stbi__jpeg_reset(z);
                                    }
                                }
                            }
                            return (int)(1);
                        }
                        else
                        {
                            int i;
                            int j;
                            int k;
                            int x;
                            int y;
                            short* data = stackalloc short[64];
                            for (j = (int)(0); (j) < (z.img_mcu_y); ++j)
                            {
                                for (i = (int)(0); (i) < (z.img_mcu_x); ++i)
                                {
                                    for (k = (int)(0); (k) < (z.scan_n); ++k)
                                    {
                                        int n = (int)(z.order[k]);
                                        for (y = (int)(0); (y) < (z.img_comp[n].v); ++y)
                                        {
                                            for (x = (int)(0); (x) < (z.img_comp[n].h); ++x)
                                            {
                                                int x2 = (int)((i * z.img_comp[n].h + x) * 8);
                                                int y2 = (int)((j * z.img_comp[n].v + y) * 8);
                                                int ha = (int)(z.img_comp[n].ha);
                                                if (
                                                    stbi__jpeg_decode_block(z, data, (stbi__huffman*)z.huff_dc + z.img_comp[n].hd,
                                                        (stbi__huffman*)z.huff_ac + ha, z.fast_ac[ha], (int)(n), (ushort*)z.dequant[z.img_comp[n].tq]) == 0)
                                                    return (int)(0);
                                                z.idct_block_kernel(z.img_comp[n].data + z.img_comp[n].w2 * y2 + x2, (int)(z.img_comp[n].w2), data);
                                            }
                                        }
                                    }
                                    if (--z.todo <= 0)
                                    {
                                        if ((z.code_bits) < (24)) stbi__grow_buffer_unsafe(z);
                                        if (!(((z.marker) >= (0xd0)) && ((z.marker) <= 0xd7))) return (int)(1);
                                        stbi__jpeg_reset(z);
                                    }
                                }
                            }
                            return (int)(1);
                        }
                    }
                    else
                    {
                        if ((z.scan_n) == (1))
                        {
                            int i;
                            int j;
                            int n = (int)(z.order[0]);
                            int w = (int)((z.img_comp[n].x + 7) >> 3);
                            int h = (int)((z.img_comp[n].y + 7) >> 3);
                            for (j = (int)(0); (j) < (h); ++j)
                            {
                                for (i = (int)(0); (i) < (w); ++i)
                                {
                                    short* data = z.img_comp[n].coeff + 64 * (i + j * z.img_comp[n].coeff_w);
                                    if ((z.spec_start) == (0))
                                    {
                                        if (stbi__jpeg_decode_block_prog_dc(z, data, (stbi__huffman*)z.huff_dc + z.img_comp[n].hd, (int)(n)) == 0)
                                            return (int)(0);
                                    }
                                    else
                                    {
                                        int ha = (int)(z.img_comp[n].ha);
                                        if (stbi__jpeg_decode_block_prog_ac(z, data, (stbi__huffman*)z.huff_ac + ha, z.fast_ac[ha]) == 0)
                                            return (int)(0);
                                    }
                                    if (--z.todo <= 0)
                                    {
                                        if ((z.code_bits) < (24)) stbi__grow_buffer_unsafe(z);
                                        if (!(((z.marker) >= (0xd0)) && ((z.marker) <= 0xd7))) return (int)(1);
                                        stbi__jpeg_reset(z);
                                    }
                                }
                            }
                            return (int)(1);
                        }
                        else
                        {
                            int i;
                            int j;
                            int k;
                            int x;
                            int y;
                            for (j = (int)(0); (j) < (z.img_mcu_y); ++j)
                            {
                                for (i = (int)(0); (i) < (z.img_mcu_x); ++i)
                                {
                                    for (k = (int)(0); (k) < (z.scan_n); ++k)
                                    {
                                        int n = (int)(z.order[k]);
                                        for (y = (int)(0); (y) < (z.img_comp[n].v); ++y)
                                        {
                                            for (x = (int)(0); (x) < (z.img_comp[n].h); ++x)
                                            {
                                                int x2 = (int)(i * z.img_comp[n].h + x);
                                                int y2 = (int)(j * z.img_comp[n].v + y);
                                                short* data = z.img_comp[n].coeff + 64 * (x2 + y2 * z.img_comp[n].coeff_w);
                                                if (stbi__jpeg_decode_block_prog_dc(z, data, (stbi__huffman*)z.huff_dc + z.img_comp[n].hd, (int)(n)) == 0)
                                                    return (int)(0);
                                            }
                                        }
                                    }
                                    if (--z.todo <= 0)
                                    {
                                        if ((z.code_bits) < (24)) stbi__grow_buffer_unsafe(z);
                                        if (!(((z.marker) >= (0xd0)) && ((z.marker) <= 0xd7))) return (int)(1);
                                        stbi__jpeg_reset(z);
                                    }
                                }
                            }
                            return (int)(1);
                        }
                    }

                }

                public static void stbi__jpeg_dequantize(short* data, ushort* dequant)
                {
                    int i;
                    for (i = (int)(0); (i) < (64); ++i)
                    {
                        data[i] *= (short)(dequant[i]);
                    }
                }

                public static void stbi__jpeg_finish(stbi__jpeg z)
                {
                    if ((z.progressive) != 0)
                    {
                        int i;
                        int j;
                        int n;
                        for (n = (int)(0); (n) < (z.s.img_n); ++n)
                        {
                            int w = (int)((z.img_comp[n].x + 7) >> 3);
                            int h = (int)((z.img_comp[n].y + 7) >> 3);
                            for (j = (int)(0); (j) < (h); ++j)
                            {
                                for (i = (int)(0); (i) < (w); ++i)
                                {
                                    short* data = z.img_comp[n].coeff + 64 * (i + j * z.img_comp[n].coeff_w);
                                    stbi__jpeg_dequantize(data, (ushort*)z.dequant[z.img_comp[n].tq]);
                                    z.idct_block_kernel(z.img_comp[n].data + z.img_comp[n].w2 * j * 8 + i * 8, (int)(z.img_comp[n].w2), data);
                                }
                            }
                        }
                    }

                }

                public static int stbi__process_marker(stbi__jpeg z, int m)
                {
                    int L;
                    switch (m)
                    {
                        case 0xff:
                            return (int)(stbi__err("expected marker"));
                        case 0xDD:
                            if (stbi__get16be(z.s) != 4) return (int)(stbi__err("bad DRI len"));
                            z.restart_interval = (int)(stbi__get16be(z.s));
                            return (int)(1);
                        case 0xDB:
                            L = (int)(stbi__get16be(z.s) - 2);
                            while ((L) > (0))
                            {
                                int q = (int)(stbi__get8(z.s));
                                int p = (int)(q >> 4);
                                int sixteen = (p != 0) ? 1 : 0;
                                int t = (int)(q & 15);
                                int i;
                                if ((p != 0) && (p != 1)) return (int)(stbi__err("bad DQT type"));
                                if ((t) > (3)) return (int)(stbi__err("bad DQT table"));
                                for (i = (int)(0); (i) < (64); ++i)
                                {
                                    z.dequant[t][stbi__jpeg_dezigzag[i]] = ((ushort)((sixteen) != 0 ? stbi__get16be(z.s) : stbi__get8(z.s)));
                                }
                                L -= (int)((sixteen) != 0 ? 129 : 65);
                            }
                            return (int)((L) == (0) ? 1 : 0);
                        case 0xC4:
                            L = (int)(stbi__get16be(z.s) - 2);
                            while ((L) > (0))
                            {
                                byte* v;
                                int* sizes = stackalloc int[16];
                                int i;
                                int n = (int)(0);
                                int q = (int)(stbi__get8(z.s));
                                int tc = (int)(q >> 4);
                                int th = (int)(q & 15);
                                if (((tc) > (1)) || ((th) > (3))) return (int)(stbi__err("bad DHT header"));
                                for (i = (int)(0); (i) < (16); ++i)
                                {
                                    sizes[i] = (int)(stbi__get8(z.s));
                                    n += (int)(sizes[i]);
                                }
                                L -= (int)(17);
                                if ((tc) == (0))
                                {
                                    if (stbi__build_huffman((stbi__huffman*)z.huff_dc + th, sizes) == 0) return (int)(0);
                                    stbi__huffman* h = (stbi__huffman*)z.huff_dc + th;
                                    v = h->values;
                                }
                                else
                                {
                                    if (stbi__build_huffman((stbi__huffman*)z.huff_ac + th, sizes) == 0) return (int)(0);
                                    stbi__huffman* h = (stbi__huffman*)z.huff_ac + th;
                                    v = h->values;
                                }
                                for (i = (int)(0); (i) < (n); ++i)
                                {
                                    v[i] = (byte)(stbi__get8(z.s));
                                }
                                if (tc != 0) stbi__build_fast_ac(z.fast_ac[th], (stbi__huffman*)z.huff_ac + th);
                                L -= (int)(n);
                            }
                            return (int)((L) == (0) ? 1 : 0);
                    }

                    if ((((m) >= (0xE0)) && (m <= 0xEF)) || ((m) == (0xFE)))
                    {
                        L = (int)(stbi__get16be(z.s));
                        if ((L) < (2))
                        {
                            if ((m) == (0xFE)) return (int)(stbi__err("bad COM len"));
                            else return (int)(stbi__err("bad APP len"));
                        }
                        L -= (int)(2);
                        if (((m) == (0xE0)) && ((L) >= (5)))
                        {
                            byte* tag = stackalloc byte[5];
                            tag[0] = (byte)('J');
                            tag[1] = (byte)('F');
                            tag[2] = (byte)('I');
                            tag[3] = (byte)('F');
                            tag[4] = (byte)('\0');
                            int ok = (int)(1);
                            int i;
                            for (i = (int)(0); (i) < (5); ++i)
                            {
                                if (stbi__get8(z.s) != tag[i]) ok = (int)(0);
                            }
                            L -= (int)(5);
                            if ((ok) != 0) z.jfif = (int)(1);
                        }
                        else if (((m) == (0xEE)) && ((L) >= (12)))
                        {
                            byte* tag = stackalloc byte[6];
                            tag[0] = (byte)('A');
                            tag[1] = (byte)('d');
                            tag[2] = (byte)('o');
                            tag[3] = (byte)('b');
                            tag[4] = (byte)('e');
                            tag[5] = (byte)('\0');
                            int ok = (int)(1);
                            int i;
                            for (i = (int)(0); (i) < (6); ++i)
                            {
                                if (stbi__get8(z.s) != tag[i]) ok = (int)(0);
                            }
                            L -= (int)(6);
                            if ((ok) != 0)
                            {
                                stbi__get8(z.s);
                                stbi__get16be(z.s);
                                stbi__get16be(z.s);
                                z.app14_colour_transform = (int)(stbi__get8(z.s));
                                L -= (int)(6);
                            }
                        }
                        stbi__skip(z.s, (int)(L));
                        return (int)(1);
                    }

                    return (int)(stbi__err("unknown marker"));
                }

                public static int stbi__process_scan_header(stbi__jpeg z)
                {
                    int i;
                    int Ls = (int)(stbi__get16be(z.s));
                    z.scan_n = (int)(stbi__get8(z.s));
                    if ((((z.scan_n) < (1)) || ((z.scan_n) > (4))) || ((z.scan_n) > (z.s.img_n)))
                        return (int)(stbi__err("bad SOS component count"));
                    if (Ls != 6 + 2 * z.scan_n) return (int)(stbi__err("bad SOS len"));
                    for (i = (int)(0); (i) < (z.scan_n); ++i)
                    {
                        int id = (int)(stbi__get8(z.s));
                        int which;
                        int q = (int)(stbi__get8(z.s));
                        for (which = (int)(0); (which) < (z.s.img_n); ++which)
                        {
                            if ((z.img_comp[which].id) == (id)) break;
                        }
                        if ((which) == (z.s.img_n)) return (int)(0);
                        z.img_comp[which].hd = (int)(q >> 4);
                        if ((z.img_comp[which].hd) > (3)) return (int)(stbi__err("bad DC huff"));
                        z.img_comp[which].ha = (int)(q & 15);
                        if ((z.img_comp[which].ha) > (3)) return (int)(stbi__err("bad AC huff"));
                        z.order[i] = (int)(which);
                    }
                    {
                        int aa;
                        z.spec_start = (int)(stbi__get8(z.s));
                        z.spec_end = (int)(stbi__get8(z.s));
                        aa = (int)(stbi__get8(z.s));
                        z.succ_high = (int)(aa >> 4);
                        z.succ_low = (int)(aa & 15);
                        if ((z.progressive) != 0)
                        {
                            if ((((((z.spec_start) > (63)) || ((z.spec_end) > (63))) || ((z.spec_start) > (z.spec_end))) ||
                                 ((z.succ_high) > (13))) || ((z.succ_low) > (13))) return (int)(stbi__err("bad SOS"));
                        }
                        else
                        {
                            if (z.spec_start != 0) return (int)(stbi__err("bad SOS"));
                            if ((z.succ_high != 0) || (z.succ_low != 0)) return (int)(stbi__err("bad SOS"));
                            z.spec_end = (int)(63);
                        }
                    }

                    return (int)(1);
                }

                public static int stbi__free_jpeg_components(stbi__jpeg z, int ncomp, int why)
                {
                    int i;
                    for (i = (int)(0); (i) < (ncomp); ++i)
                    {
                        if ((z.img_comp[i].raw_data) != null)
                        {
                            CRuntime.free(z.img_comp[i].raw_data);
                            z.img_comp[i].raw_data = (null);
                            z.img_comp[i].data = (null);
                        }
                        if ((z.img_comp[i].raw_coeff) != null)
                        {
                            CRuntime.free(z.img_comp[i].raw_coeff);
                            z.img_comp[i].raw_coeff = null;
                            z.img_comp[i].coeff = null;
                        }
                        if ((z.img_comp[i].linebuf) != null)
                        {
                            CRuntime.free(z.img_comp[i].linebuf);
                            z.img_comp[i].linebuf = (null);
                        }
                    }
                    return (int)(why);
                }

                public static int stbi__process_frame_header(stbi__jpeg z, int scan)
                {
                    stbi__context s = z.s;
                    int Lf;
                    int p;
                    int i;
                    int q;
                    int h_max = (int)(1);
                    int v_max = (int)(1);
                    int c;
                    Lf = (int)(stbi__get16be(s));
                    if ((Lf) < (11)) return (int)(stbi__err("bad SOF len"));
                    p = (int)(stbi__get8(s));
                    if (p != 8) return (int)(stbi__err("only 8-bit"));
                    s.img_y = (uint)(stbi__get16be(s));
                    if ((s.img_y) == (0)) return (int)(stbi__err("no header height"));
                    s.img_x = (uint)(stbi__get16be(s));
                    if ((s.img_x) == (0)) return (int)(stbi__err("0 width"));
                    c = (int)(stbi__get8(s));
                    if (((c != 3) && (c != 1)) && (c != 4)) return (int)(stbi__err("bad component count"));
                    s.img_n = (int)(c);
                    for (i = (int)(0); (i) < (c); ++i)
                    {
                        z.img_comp[i].data = (null);
                        z.img_comp[i].linebuf = (null);
                    }
                    if (Lf != 8 + 3 * s.img_n) return (int)(stbi__err("bad SOF len"));
                    z.rgb = (int)(0);
                    for (i = (int)(0); (i) < (s.img_n); ++i)
                    {
                        byte* rgb = stackalloc byte[3];
                        rgb[0] = (byte)('R');
                        rgb[1] = (byte)('G');
                        rgb[2] = (byte)('B');
                        z.img_comp[i].id = (int)(stbi__get8(s));
                        if (((s.img_n) == (3)) && ((z.img_comp[i].id) == (rgb[i]))) ++z.rgb;
                        q = (int)(stbi__get8(s));
                        z.img_comp[i].h = (int)(q >> 4);
                        if ((z.img_comp[i].h == 0) || ((z.img_comp[i].h) > (4))) return (int)(stbi__err("bad H"));
                        z.img_comp[i].v = (int)(q & 15);
                        if ((z.img_comp[i].v == 0) || ((z.img_comp[i].v) > (4))) return (int)(stbi__err("bad V"));
                        z.img_comp[i].tq = (int)(stbi__get8(s));
                        if ((z.img_comp[i].tq) > (3)) return (int)(stbi__err("bad TQ"));
                    }
                    if (scan != STBI__SCAN_load) return (int)(1);
                    if (stbi__mad3sizes_valid((int)(s.img_x), (int)(s.img_y), (int)(s.img_n), (int)(0)) == 0)
                        return (int)(stbi__err("too large"));
                    for (i = (int)(0); (i) < (s.img_n); ++i)
                    {
                        if ((z.img_comp[i].h) > (h_max)) h_max = (int)(z.img_comp[i].h);
                        if ((z.img_comp[i].v) > (v_max)) v_max = (int)(z.img_comp[i].v);
                    }
                    z.img_h_max = (int)(h_max);
                    z.img_v_max = (int)(v_max);
                    z.img_mcu_w = (int)(h_max * 8);
                    z.img_mcu_h = (int)(v_max * 8);
                    z.img_mcu_x = (int)((s.img_x + z.img_mcu_w - 1) / z.img_mcu_w);
                    z.img_mcu_y = (int)((s.img_y + z.img_mcu_h - 1) / z.img_mcu_h);
                    for (i = (int)(0); (i) < (s.img_n); ++i)
                    {
                        z.img_comp[i].x = (int)((s.img_x * z.img_comp[i].h + h_max - 1) / h_max);
                        z.img_comp[i].y = (int)((s.img_y * z.img_comp[i].v + v_max - 1) / v_max);
                        z.img_comp[i].w2 = (int)(z.img_mcu_x * z.img_comp[i].h * 8);
                        z.img_comp[i].h2 = (int)(z.img_mcu_y * z.img_comp[i].v * 8);
                        z.img_comp[i].coeff = null;
                        z.img_comp[i].raw_coeff = null;
                        z.img_comp[i].linebuf = (null);
                        z.img_comp[i].raw_data = stbi__malloc_mad2((int)(z.img_comp[i].w2), (int)(z.img_comp[i].h2), (int)(15));
                        if ((z.img_comp[i].raw_data) == (null))
                            return (int)(stbi__free_jpeg_components(z, (int)(i + 1), (int)(stbi__err("outofmem"))));
                        z.img_comp[i].data = (byte*)((((long)z.img_comp[i].raw_data + 15) & ~15));
                        if ((z.progressive) != 0)
                        {
                            z.img_comp[i].coeff_w = (int)(z.img_comp[i].w2 / 8);
                            z.img_comp[i].coeff_h = (int)(z.img_comp[i].h2 / 8);
                            z.img_comp[i].raw_coeff = stbi__malloc_mad3((int)(z.img_comp[i].w2), (int)(z.img_comp[i].h2), (int)(2),
                                (int)(15));
                            if ((z.img_comp[i].raw_coeff) == (null))
                                return (int)(stbi__free_jpeg_components(z, (int)(i + 1), (int)(stbi__err("outofmem"))));
                            z.img_comp[i].coeff = (short*)((((long)z.img_comp[i].raw_coeff + 15) & ~15));
                        }
                    }
                    return (int)(1);
                }

                public static int stbi__decode_jpeg_header(stbi__jpeg z, int scan)
                {
                    int m;
                    z.jfif = (int)(0);
                    z.app14_colour_transform = (int)(-1);
                    z.marker = (byte)(0xff);
                    m = (int)(stbi__get_marker(z));
                    if (!((m) == (0xd8))) return (int)(stbi__err("no SOI"));
                    if ((scan) == (STBI__SCAN_type)) return (int)(1);
                    m = (int)(stbi__get_marker(z));
                    while (!((((m) == (0xc0)) || ((m) == (0xc1))) || ((m) == (0xc2))))
                    {
                        if (stbi__process_marker(z, (int)(m)) == 0) return (int)(0);
                        m = (int)(stbi__get_marker(z));
                        while ((m) == (0xff))
                        {
                            if ((stbi__at_eof(z.s)) != 0) return (int)(stbi__err("no SOF"));
                            m = (int)(stbi__get_marker(z));
                        }
                    }
                    z.progressive = (int)((m) == (0xc2) ? 1 : 0);
                    if (stbi__process_frame_header(z, (int)(scan)) == 0) return (int)(0);
                    return (int)(1);
                }

                public static int stbi__decode_jpeg_image(stbi__jpeg j)
                {
                    int m;
                    for (m = (int)(0); (m) < (4); m++)
                    {
                        j.img_comp[m].raw_data = (null);
                        j.img_comp[m].raw_coeff = (null);
                    }
                    j.restart_interval = (int)(0);
                    if (stbi__decode_jpeg_header(j, (int)(STBI__SCAN_load)) == 0) return (int)(0);
                    m = (int)(stbi__get_marker(j));
                    while (!((m) == (0xd9)))
                    {
                        if (((m) == (0xda)))
                        {
                            if (stbi__process_scan_header(j) == 0) return (int)(0);
                            if (stbi__parse_entropy_coded_data(j) == 0) return (int)(0);
                            if ((j.marker) == (0xff))
                            {
                                while (stbi__at_eof(j.s) == 0)
                                {
                                    int x = (int)(stbi__get8(j.s));
                                    if ((x) == (255))
                                    {
                                        j.marker = (byte)(stbi__get8(j.s));
                                        break;
                                    }
                                }
                            }
                        }
                        else if (((m) == (0xdc)))
                        {
                            int Ld = (int)(stbi__get16be(j.s));
                            uint NL = (uint)(stbi__get16be(j.s));
                            if (Ld != 4) stbi__err("bad DNL len");
                            if (NL != j.s.img_y) stbi__err("bad DNL height");
                        }
                        else
                        {
                            if (stbi__process_marker(j, (int)(m)) == 0) return (int)(0);
                        }
                        m = (int)(stbi__get_marker(j));
                    }
                    if ((j.progressive) != 0) stbi__jpeg_finish(j);
                    return (int)(1);
                }

                public static byte* resample_row_1(byte* _out_, byte* in_near, byte* in_far, int w, int hs)
                {
                    return in_near;
                }

                public static byte* stbi__resample_row_v_2(byte* _out_, byte* in_near, byte* in_far, int w, int hs)
                {
                    int i;
                    for (i = (int)(0); (i) < (w); ++i)
                    {
                        _out_[i] = ((byte)((3 * in_near[i] + in_far[i] + 2) >> 2));
                    }
                    return _out_;
                }

                public static byte* stbi__resample_row_h_2(byte* _out_, byte* in_near, byte* in_far, int w, int hs)
                {
                    int i;
                    byte* input = in_near;
                    if ((w) == (1))
                    {
                        _out_[0] = (byte)(_out_[1] = (byte)(input[0]));
                        return _out_;
                    }

                    _out_[0] = (byte)(input[0]);
                    _out_[1] = ((byte)((input[0] * 3 + input[1] + 2) >> 2));
                    for (i = (int)(1); (i) < (w - 1); ++i)
                    {
                        int n = (int)(3 * input[i] + 2);
                        _out_[i * 2 + 0] = ((byte)((n + input[i - 1]) >> 2));
                        _out_[i * 2 + 1] = ((byte)((n + input[i + 1]) >> 2));
                    }
                    _out_[i * 2 + 0] = ((byte)((input[w - 2] * 3 + input[w - 1] + 2) >> 2));
                    _out_[i * 2 + 1] = (byte)(input[w - 1]);
                    return _out_;
                }

                public static byte* stbi__resample_row_hv_2(byte* _out_, byte* in_near, byte* in_far, int w, int hs)
                {
                    int i;
                    int t0;
                    int t1;
                    if ((w) == (1))
                    {
                        _out_[0] = (byte)(_out_[1] = ((byte)((3 * in_near[0] + in_far[0] + 2) >> 2)));
                        return _out_;
                    }

                    t1 = (int)(3 * in_near[0] + in_far[0]);
                    _out_[0] = ((byte)((t1 + 2) >> 2));
                    for (i = (int)(1); (i) < (w); ++i)
                    {
                        t0 = (int)(t1);
                        t1 = (int)(3 * in_near[i] + in_far[i]);
                        _out_[i * 2 - 1] = ((byte)((3 * t0 + t1 + 8) >> 4));
                        _out_[i * 2] = ((byte)((3 * t1 + t0 + 8) >> 4));
                    }
                    _out_[w * 2 - 1] = ((byte)((t1 + 2) >> 2));
                    return _out_;
                }

                public static byte* stbi__resample_row_generic(byte* _out_, byte* in_near, byte* in_far, int w, int hs)
                {
                    int i;
                    int j;
                    for (i = (int)(0); (i) < (w); ++i)
                    {
                        for (j = (int)(0); (j) < (hs); ++j)
                        {
                            _out_[i * hs + j] = (byte)(in_near[i]);
                        }
                    }
                    return _out_;
                }

                public static void stbi__YCbCr_to_RGB_row(byte* _out_, byte* y, byte* pcb, byte* pcr, int count, int step)
                {
                    int i;
                    for (i = (int)(0); (i) < (count); ++i)
                    {
                        int y_fixed = (int)((y[i] << 20) + (1 << 19));
                        int r;
                        int g;
                        int b;
                        int cr = (int)(pcr[i] - 128);
                        int cb = (int)(pcb[i] - 128);
                        r = (int)(y_fixed + cr * (((int)((1.40200f) * 4096.0f + 0.5f)) << 8));
                        g =
                            (int)
                                (y_fixed + (cr * -(((int)((0.71414f) * 4096.0f + 0.5f)) << 8)) +
                                 ((cb * -(((int)((0.34414f) * 4096.0f + 0.5f)) << 8)) & 0xffff0000));
                        b = (int)(y_fixed + cb * (((int)((1.77200f) * 4096.0f + 0.5f)) << 8));
                        r >>= 20;
                        g >>= 20;
                        b >>= 20;
                        if (((uint)(r)) > (255))
                        {
                            if ((r) < (0)) r = (int)(0);
                            else r = (int)(255);
                        }
                        if (((uint)(g)) > (255))
                        {
                            if ((g) < (0)) g = (int)(0);
                            else g = (int)(255);
                        }
                        if (((uint)(b)) > (255))
                        {
                            if ((b) < (0)) b = (int)(0);
                            else b = (int)(255);
                        }
                        _out_[0] = ((byte)(r));
                        _out_[1] = ((byte)(g));
                        _out_[2] = ((byte)(b));
                        _out_[3] = (byte)(255);
                        _out_ += step;
                    }
                }

                public static void stbi__setup_jpeg(stbi__jpeg j)
                {
                    j.idct_block_kernel = stbi__idct_block;
                    j.YCbCr_to_RGB_kernel = stbi__YCbCr_to_RGB_row;
                    j.resample_row_hv_2_kernel = stbi__resample_row_hv_2;
                }

                public static void stbi__cleanup_jpeg(stbi__jpeg j)
                {
                    stbi__free_jpeg_components(j, (int)(j.s.img_n), (int)(0));
                }

                public static byte stbi__blinn_8x8(byte x, byte y)
                {
                    uint t = (uint)(x * y + 128);
                    return (byte)((t + (t >> 8)) >> 8);
                }

                public static byte* load_jpeg_image(stbi__jpeg z, int* out_x, int* out_y, int* comp, int req_comp)
                {
                    int n;
                    int decode_n;
                    int is_rgb;
                    z.s.img_n = (int)(0);
                    if (((req_comp) < (0)) || ((req_comp) > (4)))
                        return ((byte*)((ulong)((stbi__err("bad req_comp")) != 0 ? ((byte*)null) : (null))));
                    if (stbi__decode_jpeg_image(z) == 0)
                    {
                        stbi__cleanup_jpeg(z);
                        return (null);
                    }

                    n = (int)((req_comp) != 0 ? req_comp : (z.s.img_n) >= (3) ? 3 : 1);
                    is_rgb =
                        (int)(((z.s.img_n) == (3)) && (((z.rgb) == (3)) || (((z.app14_colour_transform) == (0)) && (z.jfif == 0))) ? 1 : 0);
                    if ((((z.s.img_n) == (3)) && ((n) < (3))) && (is_rgb == 0)) decode_n = (int)(1);
                    else decode_n = (int)(z.s.img_n);
                    {
                        int k;
                        uint i;
                        uint j;
                        byte* output;
                        byte** coutput = stackalloc byte*[4];
                        var res_comp = new stbi__resample[4];
                        for (var kkk = 0; kkk < res_comp.Length; ++kkk) res_comp[kkk] = new stbi__resample();
                        for (k = (int)(0); (k) < (decode_n); ++k)
                        {
                            stbi__resample r = res_comp[k];
                            z.img_comp[k].linebuf = (byte*)(stbi__malloc((ulong)(z.s.img_x + 3)));
                            if (z.img_comp[k].linebuf == null)
                            {
                                stbi__cleanup_jpeg(z);
                                return ((byte*)((ulong)((stbi__err("outofmem")) != 0 ? ((byte*)null) : (null))));
                            }
                            r.hs = (int)(z.img_h_max / z.img_comp[k].h);
                            r.vs = (int)(z.img_v_max / z.img_comp[k].v);
                            r.ystep = (int)(r.vs >> 1);
                            r.w_lores = (int)((z.s.img_x + r.hs - 1) / r.hs);
                            r.ypos = (int)(0);
                            r.line0 = r.line1 = z.img_comp[k].data;
                            if (((r.hs) == (1)) && ((r.vs) == (1))) r.resample = resample_row_1;
                            else if (((r.hs) == (1)) && ((r.vs) == (2))) r.resample = stbi__resample_row_v_2;
                            else if (((r.hs) == (2)) && ((r.vs) == (1))) r.resample = stbi__resample_row_h_2;
                            else if (((r.hs) == (2)) && ((r.vs) == (2))) r.resample = z.resample_row_hv_2_kernel;
                            else r.resample = stbi__resample_row_generic;
                        }
                        output = (byte*)(stbi__malloc_mad3((int)(n), (int)(z.s.img_x), (int)(z.s.img_y), (int)(1)));
                        if (output == null)
                        {
                            stbi__cleanup_jpeg(z);
                            return ((byte*)((ulong)((stbi__err("outofmem")) != 0 ? ((byte*)null) : (null))));
                        }
                        for (j = (uint)(0); (j) < (z.s.img_y); ++j)
                        {
                            byte* _out_ = output + n * z.s.img_x * j;
                            for (k = (int)(0); (k) < (decode_n); ++k)
                            {
                                stbi__resample r = res_comp[k];
                                int y_bot = (int)((r.ystep) >= (r.vs >> 1) ? 1 : 0);
                                coutput[k] = r.resample(z.img_comp[k].linebuf, (y_bot) != 0 ? r.line1 : r.line0, (y_bot) != 0 ? r.line0 : r.line1,
                                    (int)(r.w_lores), (int)(r.hs));
                                if ((++r.ystep) >= (r.vs))
                                {
                                    r.ystep = (int)(0);
                                    r.line0 = r.line1;
                                    if ((++r.ypos) < (z.img_comp[k].y)) r.line1 += z.img_comp[k].w2;
                                }
                            }
                            if ((n) >= (3))
                            {
                                byte* y = coutput[0];
                                if ((z.s.img_n) == (3))
                                {
                                    if ((is_rgb) != 0)
                                    {
                                        for (i = (uint)(0); (i) < (z.s.img_x); ++i)
                                        {
                                            _out_[0] = (byte)(y[i]);
                                            _out_[1] = (byte)(coutput[1][i]);
                                            _out_[2] = (byte)(coutput[2][i]);
                                            _out_[3] = (byte)(255);
                                            _out_ += n;
                                        }
                                    }
                                    else
                                    {
                                        z.YCbCr_to_RGB_kernel(_out_, y, coutput[1], coutput[2], (int)(z.s.img_x), (int)(n));
                                    }
                                }
                                else if ((z.s.img_n) == (4))
                                {
                                    if ((z.app14_colour_transform) == (0))
                                    {
                                        for (i = (uint)(0); (i) < (z.s.img_x); ++i)
                                        {
                                            byte m = (byte)(coutput[3][i]);
                                            _out_[0] = (byte)(stbi__blinn_8x8((byte)(coutput[0][i]), (byte)(m)));
                                            _out_[1] = (byte)(stbi__blinn_8x8((byte)(coutput[1][i]), (byte)(m)));
                                            _out_[2] = (byte)(stbi__blinn_8x8((byte)(coutput[2][i]), (byte)(m)));
                                            _out_[3] = (byte)(255);
                                            _out_ += n;
                                        }
                                    }
                                    else if ((z.app14_colour_transform) == (2))
                                    {
                                        z.YCbCr_to_RGB_kernel(_out_, y, coutput[1], coutput[2], (int)(z.s.img_x), (int)(n));
                                        for (i = (uint)(0); (i) < (z.s.img_x); ++i)
                                        {
                                            byte m = (byte)(coutput[3][i]);
                                            _out_[0] = (byte)(stbi__blinn_8x8((byte)(255 - _out_[0]), (byte)(m)));
                                            _out_[1] = (byte)(stbi__blinn_8x8((byte)(255 - _out_[1]), (byte)(m)));
                                            _out_[2] = (byte)(stbi__blinn_8x8((byte)(255 - _out_[2]), (byte)(m)));
                                            _out_ += n;
                                        }
                                    }
                                    else
                                    {
                                        z.YCbCr_to_RGB_kernel(_out_, y, coutput[1], coutput[2], (int)(z.s.img_x), (int)(n));
                                    }
                                }
                                else
                                    for (i = (uint)(0); (i) < (z.s.img_x); ++i)
                                    {
                                        _out_[0] = (byte)(_out_[1] = (byte)(_out_[2] = (byte)(y[i])));
                                        _out_[3] = (byte)(255);
                                        _out_ += n;
                                    }
                            }
                            else
                            {
                                if ((is_rgb) != 0)
                                {
                                    if ((n) == (1))
                                        for (i = (uint)(0); (i) < (z.s.img_x); ++i)
                                        {
                                            *_out_++ = (byte)(stbi__compute_y((int)(coutput[0][i]), (int)(coutput[1][i]), (int)(coutput[2][i])));
                                        }
                                    else
                                    {
                                        for (i = (uint)(0); (i) < (z.s.img_x); ++i, _out_ += 2)
                                        {
                                            _out_[0] = (byte)(stbi__compute_y((int)(coutput[0][i]), (int)(coutput[1][i]), (int)(coutput[2][i])));
                                            _out_[1] = (byte)(255);
                                        }
                                    }
                                }
                                else if (((z.s.img_n) == (4)) && ((z.app14_colour_transform) == (0)))
                                {
                                    for (i = (uint)(0); (i) < (z.s.img_x); ++i)
                                    {
                                        byte m = (byte)(coutput[3][i]);
                                        byte r = (byte)(stbi__blinn_8x8((byte)(coutput[0][i]), (byte)(m)));
                                        byte g = (byte)(stbi__blinn_8x8((byte)(coutput[1][i]), (byte)(m)));
                                        byte b = (byte)(stbi__blinn_8x8((byte)(coutput[2][i]), (byte)(m)));
                                        _out_[0] = (byte)(stbi__compute_y((int)(r), (int)(g), (int)(b)));
                                        _out_[1] = (byte)(255);
                                        _out_ += n;
                                    }
                                }
                                else if (((z.s.img_n) == (4)) && ((z.app14_colour_transform) == (2)))
                                {
                                    for (i = (uint)(0); (i) < (z.s.img_x); ++i)
                                    {
                                        _out_[0] = (byte)(stbi__blinn_8x8((byte)(255 - coutput[0][i]), (byte)(coutput[3][i])));
                                        _out_[1] = (byte)(255);
                                        _out_ += n;
                                    }
                                }
                                else
                                {
                                    byte* y = coutput[0];
                                    if ((n) == (1))
                                        for (i = (uint)(0); (i) < (z.s.img_x); ++i)
                                        {
                                            _out_[i] = (byte)(y[i]);
                                        }
                                    else
                                        for (i = (uint)(0); (i) < (z.s.img_x); ++i)
                                        {
                                            *_out_++ = (byte)(y[i]);
                                            *_out_++ = (byte)(255);
                                        }
                                }
                            }
                        }
                        stbi__cleanup_jpeg(z);
                        *out_x = (int)(z.s.img_x);
                        *out_y = (int)(z.s.img_y);
                        if ((comp) != null) *comp = (int)((z.s.img_n) >= (3) ? 3 : 1);
                        return output;
                    }

                }

                public static void* stbi__jpeg_load(stbi__context s, int* x, int* y, int* comp, int req_comp, stbi__result_info* ri)
                {
                    byte* result;
                    stbi__jpeg j = new stbi__jpeg();
                    j.s = s;
                    stbi__setup_jpeg(j);
                    result = load_jpeg_image(j, x, y, comp, (int)(req_comp));

                    return result;
                }

                public static int stbi__jpeg_test(stbi__context s)
                {
                    int r;
                    stbi__jpeg j = new stbi__jpeg();
                    j.s = s;
                    stbi__setup_jpeg(j);
                    r = (int)(stbi__decode_jpeg_header(j, (int)(STBI__SCAN_type)));
                    stbi__rewind(s);

                    return (int)(r);
                }

                public static int stbi__jpeg_info_raw(stbi__jpeg j, int* x, int* y, int* comp)
                {
                    if (stbi__decode_jpeg_header(j, (int)(STBI__SCAN_header)) == 0)
                    {
                        stbi__rewind(j.s);
                        return (int)(0);
                    }

                    if ((x) != null) *x = (int)(j.s.img_x);
                    if ((y) != null) *y = (int)(j.s.img_y);
                    if ((comp) != null) *comp = (int)((j.s.img_n) >= (3) ? 3 : 1);
                    return (int)(1);
                }

                public static int stbi__jpeg_info(stbi__context s, int* x, int* y, int* comp)
                {
                    int result;
                    stbi__jpeg j = new stbi__jpeg();
                    j.s = s;
                    result = (int)(stbi__jpeg_info_raw(j, x, y, comp));

                    return (int)(result);
                }

                public static int stbi__bitreverse16(int n)
                {
                    n = (int)(((n & 0xAAAA) >> 1) | ((n & 0x5555) << 1));
                    n = (int)(((n & 0xCCCC) >> 2) | ((n & 0x3333) << 2));
                    n = (int)(((n & 0xF0F0) >> 4) | ((n & 0x0F0F) << 4));
                    n = (int)(((n & 0xFF00) >> 8) | ((n & 0x00FF) << 8));
                    return (int)(n);
                }

                public static int stbi__bit_reverse(int v, int bits)
                {
                    return (int)(stbi__bitreverse16((int)(v)) >> (16 - bits));
                }

                public static int stbi__zbuild_huffman(stbi__zhuffman* z, byte* sizelist, int num)
                {
                    int i;
                    int k = (int)(0);
                    int code;
                    int* next_code = stackalloc int[16];
                    int* sizes = stackalloc int[17];
                    CRuntime.memset(sizes, (int)(0), (ulong)(sizeof(int)));
                    CRuntime.memset(((ushort*)(z->fast)), (int)(0), (ulong)((1 << 9) * sizeof(ushort)));
                    for (i = (int)(0); (i) < (num); ++i)
                    {
                        ++sizes[sizelist[i]];
                    }
                    sizes[0] = (int)(0);
                    for (i = (int)(1); (i) < (16); ++i)
                    {
                        if ((sizes[i]) > (1 << i)) return (int)(stbi__err("bad sizes"));
                    }
                    code = (int)(0);
                    for (i = (int)(1); (i) < (16); ++i)
                    {
                        next_code[i] = (int)(code);
                        z->firstcode[i] = ((ushort)(code));
                        z->firstsymbol[i] = ((ushort)(k));
                        code = (int)(code + sizes[i]);
                        if ((sizes[i]) != 0) if ((code - 1) >= (1 << i)) return (int)(stbi__err("bad codelengths"));
                        z->maxcode[i] = (int)(code << (16 - i));
                        code <<= 1;
                        k += (int)(sizes[i]);
                    }
                    z->maxcode[16] = (int)(0x10000);
                    for (i = (int)(0); (i) < (num); ++i)
                    {
                        int s = (int)(sizelist[i]);
                        if ((s) != 0)
                        {
                            int c = (int)(next_code[s] - z->firstcode[s] + z->firstsymbol[s]);
                            ushort fastv = (ushort)((s << 9) | i);
                            z->size[c] = ((byte)(s));
                            z->value[c] = ((ushort)(i));
                            if (s <= 9)
                            {
                                int j = (int)(stbi__bit_reverse((int)(next_code[s]), (int)(s)));
                                while ((j) < (1 << 9))
                                {
                                    z->fast[j] = (ushort)(fastv);
                                    j += (int)(1 << s);
                                }
                            }
                            ++next_code[s];
                        }
                    }
                    return (int)(1);
                }

                public static byte stbi__zget8(stbi__zbuf* z)
                {
                    if ((z->zbuffer) >= (z->zbuffer_end)) return (byte)(0);
                    return (byte)(*z->zbuffer++);
                }

                public static void stbi__fill_bits(stbi__zbuf* z)
                {
                    do
                    {
                        z->code_buffer |= (uint)((uint)(stbi__zget8(z)) << z->num_bits);
                        z->num_bits += (int)(8);
                    } while (z->num_bits <= 24);
                }

                public static uint stbi__zreceive(stbi__zbuf* z, int n)
                {
                    uint k;
                    if ((z->num_bits) < (n)) stbi__fill_bits(z);
                    k = (uint)(z->code_buffer & ((1 << n) - 1));
                    z->code_buffer >>= n;
                    z->num_bits -= (int)(n);
                    return (uint)(k);
                }

                public static int stbi__zhuffman_decode_slowpath(stbi__zbuf* a, stbi__zhuffman* z)
                {
                    int b;
                    int s;
                    int k;
                    k = (int)(stbi__bit_reverse((int)(a->code_buffer), (int)(16)));
                    for (s = (int)(9 + 1); ; ++s)
                    {
                        if ((k) < (z->maxcode[s])) break;
                    }
                    if ((s) == (16)) return (int)(-1);
                    b = (int)((k >> (16 - s)) - z->firstcode[s] + z->firstsymbol[s]);
                    a->code_buffer >>= s;
                    a->num_bits -= (int)(s);
                    return (int)(z->value[b]);
                }

                public static int stbi__zhuffman_decode(stbi__zbuf* a, stbi__zhuffman* z)
                {
                    int b;
                    int s;
                    if ((a->num_bits) < (16)) stbi__fill_bits(a);
                    b = (int)(z->fast[a->code_buffer & ((1 << 9) - 1)]);
                    if ((b) != 0)
                    {
                        s = (int)(b >> 9);
                        a->code_buffer >>= s;
                        a->num_bits -= (int)(s);
                        return (int)(b & 511);
                    }

                    return (int)(stbi__zhuffman_decode_slowpath(a, z));
                }

                public static int stbi__zexpand(stbi__zbuf* z, sbyte* zout, int n)
                {
                    sbyte* q;
                    int cur;
                    int limit;
                    int old_limit;
                    z->zout = zout;
                    if (z->z_expandable == 0) return (int)(stbi__err("output buffer limit"));
                    cur = ((int)(z->zout - z->zout_start));
                    limit = (int)(old_limit = ((int)(z->zout_end - z->zout_start)));
                    while ((cur + n) > (limit))
                    {
                        limit *= (int)(2);
                    }
                    q = (sbyte*)(CRuntime.realloc(z->zout_start, (ulong)(limit)));
                    if ((q) == (null)) return (int)(stbi__err("outofmem"));
                    z->zout_start = q;
                    z->zout = q + cur;
                    z->zout_end = q + limit;
                    return (int)(1);
                }

                public static int stbi__parse_huffman_block(stbi__zbuf* a)
                {
                    sbyte* zout = a->zout;
                    for (; ; )
                    {
                        int z = (int)(stbi__zhuffman_decode(a, &a->z_length));
                        if ((z) < (256))
                        {
                            if ((z) < (0)) return (int)(stbi__err("bad huffman code"));
                            if ((zout) >= (a->zout_end))
                            {
                                if (stbi__zexpand(a, zout, (int)(1)) == 0) return (int)(0);
                                zout = a->zout;
                            }
                            *zout++ = ((sbyte)(z));
                        }
                        else
                        {
                            byte* p;
                            int len;
                            int dist;
                            if ((z) == (256))
                            {
                                a->zout = zout;
                                return (int)(1);
                            }
                            z -= (int)(257);
                            len = (int)(stbi__zlength_base[z]);
                            if ((stbi__zlength_extra[z]) != 0) len += (int)(stbi__zreceive(a, (int)(stbi__zlength_extra[z])));
                            z = (int)(stbi__zhuffman_decode(a, &a->z_distance));
                            if ((z) < (0)) return (int)(stbi__err("bad huffman code"));
                            dist = (int)(stbi__zdist_base[z]);
                            if ((stbi__zdist_extra[z]) != 0) dist += (int)(stbi__zreceive(a, (int)(stbi__zdist_extra[z])));
                            if ((zout - a->zout_start) < (dist)) return (int)(stbi__err("bad dist"));
                            if ((zout + len) > (a->zout_end))
                            {
                                if (stbi__zexpand(a, zout, (int)(len)) == 0) return (int)(0);
                                zout = a->zout;
                            }
                            p = (byte*)(zout - dist);
                            if ((dist) == (1))
                            {
                                byte v = (byte)(*p);
                                if ((len) != 0)
                                {
                                    do *zout++ = (sbyte)(v); while ((--len) != 0);
                                }
                            }
                            else
                            {
                                if ((len) != 0)
                                {
                                    do *zout++ = (sbyte)(*p++); while ((--len) != 0);
                                }
                            }
                        }
                    }
                }

                public static int stbi__compute_huffman_codes(stbi__zbuf* a)
                {
                    stbi__zhuffman z_codelength = new stbi__zhuffman();
                    byte* lencodes = stackalloc byte[286 + 32 + 137];
                    byte* codelength_sizes = stackalloc byte[19];
                    int i;
                    int n;
                    int hlit = (int)(stbi__zreceive(a, (int)(5)) + 257);
                    int hdist = (int)(stbi__zreceive(a, (int)(5)) + 1);
                    int hclen = (int)(stbi__zreceive(a, (int)(4)) + 4);
                    int ntot = (int)(hlit + hdist);
                    CRuntime.memset(((byte*)(codelength_sizes)), (int)(0), (ulong)(19 * sizeof(byte)));
                    for (i = (int)(0); (i) < (hclen); ++i)
                    {
                        int s = (int)(stbi__zreceive(a, (int)(3)));
                        codelength_sizes[length_dezigzag[i]] = ((byte)(s));
                    }
                    if (stbi__zbuild_huffman(&z_codelength, codelength_sizes, (int)(19)) == 0) return (int)(0);
                    n = (int)(0);
                    while ((n) < (ntot))
                    {
                        int c = (int)(stbi__zhuffman_decode(a, &z_codelength));
                        if (((c) < (0)) || ((c) >= (19))) return (int)(stbi__err("bad codelengths"));
                        if ((c) < (16)) lencodes[n++] = ((byte)(c));
                        else
                        {
                            byte fill = (byte)(0);
                            if ((c) == (16))
                            {
                                c = (int)(stbi__zreceive(a, (int)(2)) + 3);
                                if ((n) == (0)) return (int)(stbi__err("bad codelengths"));
                                fill = (byte)(lencodes[n - 1]);
                            }
                            else if ((c) == (17)) c = (int)(stbi__zreceive(a, (int)(3)) + 3);
                            else
                            {
                                c = (int)(stbi__zreceive(a, (int)(7)) + 11);
                            }
                            if ((ntot - n) < (c)) return (int)(stbi__err("bad codelengths"));
                            CRuntime.memset(lencodes + n, (int)(fill), (ulong)(c));
                            n += (int)(c);
                        }
                    }
                    if (n != ntot) return (int)(stbi__err("bad codelengths"));
                    if (stbi__zbuild_huffman(&a->z_length, lencodes, (int)(hlit)) == 0) return (int)(0);
                    if (stbi__zbuild_huffman(&a->z_distance, lencodes + hlit, (int)(hdist)) == 0) return (int)(0);
                    return (int)(1);
                }

                public static int stbi__parse_uncompressed_block(stbi__zbuf* a)
                {
                    byte* header = stackalloc byte[4];
                    int len;
                    int nlen;
                    int k;
                    if ((a->num_bits & 7) != 0) stbi__zreceive(a, (int)(a->num_bits & 7));
                    k = (int)(0);
                    while ((a->num_bits) > (0))
                    {
                        header[k++] = ((byte)(a->code_buffer & 255));
                        a->code_buffer >>= 8;
                        a->num_bits -= (int)(8);
                    }
                    while ((k) < (4))
                    {
                        header[k++] = (byte)(stbi__zget8(a));
                    }
                    len = (int)(header[1] * 256 + header[0]);
                    nlen = (int)(header[3] * 256 + header[2]);
                    if (nlen != (len ^ 0xffff)) return (int)(stbi__err("zlib corrupt"));
                    if ((a->zbuffer + len) > (a->zbuffer_end)) return (int)(stbi__err("read past buffer"));
                    if ((a->zout + len) > (a->zout_end)) if (stbi__zexpand(a, a->zout, (int)(len)) == 0) return (int)(0);
                    CRuntime.memcpy(a->zout, a->zbuffer, (ulong)(len));
                    a->zbuffer += len;
                    a->zout += len;
                    return (int)(1);
                }

                public static int stbi__parse_zlib_header(stbi__zbuf* a)
                {
                    int cmf = (int)(stbi__zget8(a));
                    int cm = (int)(cmf & 15);
                    int flg = (int)(stbi__zget8(a));
                    if ((cmf * 256 + flg) % 31 != 0) return (int)(stbi__err("bad zlib header"));
                    if ((flg & 32) != 0) return (int)(stbi__err("no preset dict"));
                    if (cm != 8) return (int)(stbi__err("bad compression"));
                    return (int)(1);
                }

                public static int stbi__parse_zlib(stbi__zbuf* a, int parse_header)
                {
                    int final;
                    int type;
                    if ((parse_header) != 0) if (stbi__parse_zlib_header(a) == 0) return (int)(0);
                    a->num_bits = (int)(0);
                    a->code_buffer = (uint)(0);
                    do
                    {
                        final = (int)(stbi__zreceive(a, (int)(1)));
                        type = (int)(stbi__zreceive(a, (int)(2)));
                        if ((type) == (0))
                        {
                            if (stbi__parse_uncompressed_block(a) == 0) return (int)(0);
                        }
                        else if ((type) == (3))
                        {
                            return (int)(0);
                        }
                        else
                        {
                            if ((type) == (1))
                            {
                                fixed (byte* b = stbi__zdefault_length)
                                {
                                    if (stbi__zbuild_huffman(&a->z_length, b, (int)(288)) == 0) return (int)(0);
                                }
                                fixed (byte* b = stbi__zdefault_distance)
                                {
                                    if (stbi__zbuild_huffman(&a->z_distance, b, (int)(32)) == 0) return (int)(0);
                                }
                            }
                            else
                            {
                                if (stbi__compute_huffman_codes(a) == 0) return (int)(0);
                            }
                            if (stbi__parse_huffman_block(a) == 0) return (int)(0);
                        }
                    } while (final == 0);
                    return (int)(1);
                }

                public static int stbi__do_zlib(stbi__zbuf* a, sbyte* obuf, int olen, int exp, int parse_header)
                {
                    a->zout_start = obuf;
                    a->zout = obuf;
                    a->zout_end = obuf + olen;
                    a->z_expandable = (int)(exp);
                    return (int)(stbi__parse_zlib(a, (int)(parse_header)));
                }

                public static sbyte* stbi_zlib_decode_malloc_guesssize(sbyte* buffer, int len, int initial_size, int* outlen)
                {
                    stbi__zbuf a = new stbi__zbuf();
                    sbyte* p = (sbyte*)(stbi__malloc((ulong)(initial_size)));
                    if ((p) == (null)) return (null);
                    a.zbuffer = (byte*)(buffer);
                    a.zbuffer_end = (byte*)(buffer) + len;
                    if ((stbi__do_zlib(&a, p, (int)(initial_size), (int)(1), (int)(1))) != 0)
                    {
                        if ((outlen) != null) *outlen = ((int)(a.zout - a.zout_start));
                        return a.zout_start;
                    }
                    else
                    {
                        CRuntime.free(a.zout_start);
                        return (null);
                    }

                }

                public static sbyte* stbi_zlib_decode_malloc(sbyte* buffer, int len, int* outlen)
                {
                    return stbi_zlib_decode_malloc_guesssize(buffer, (int)(len), (int)(16384), outlen);
                }

                public static sbyte* stbi_zlib_decode_malloc_guesssize_headerflag(sbyte* buffer, int len, int initial_size,
                    int* outlen, int parse_header)
                {
                    stbi__zbuf a = new stbi__zbuf();
                    sbyte* p = (sbyte*)(stbi__malloc((ulong)(initial_size)));
                    if ((p) == (null)) return (null);
                    a.zbuffer = (byte*)(buffer);
                    a.zbuffer_end = (byte*)(buffer) + len;
                    if ((stbi__do_zlib(&a, p, (int)(initial_size), (int)(1), (int)(parse_header))) != 0)
                    {
                        if ((outlen) != null) *outlen = ((int)(a.zout - a.zout_start));
                        return a.zout_start;
                    }
                    else
                    {
                        CRuntime.free(a.zout_start);
                        return (null);
                    }

                }

                public static int stbi_zlib_decode_buffer(sbyte* obuffer, int olen, sbyte* ibuffer, int ilen)
                {
                    stbi__zbuf a = new stbi__zbuf();
                    a.zbuffer = (byte*)(ibuffer);
                    a.zbuffer_end = (byte*)(ibuffer) + ilen;
                    if ((stbi__do_zlib(&a, obuffer, (int)(olen), (int)(0), (int)(1))) != 0) return (int)(a.zout - a.zout_start);
                    else return (int)(-1);
                }

                public static sbyte* stbi_zlib_decode_noheader_malloc(sbyte* buffer, int len, int* outlen)
                {
                    stbi__zbuf a = new stbi__zbuf();
                    sbyte* p = (sbyte*)(stbi__malloc((ulong)(16384)));
                    if ((p) == (null)) return (null);
                    a.zbuffer = (byte*)(buffer);
                    a.zbuffer_end = (byte*)(buffer) + len;
                    if ((stbi__do_zlib(&a, p, (int)(16384), (int)(1), (int)(0))) != 0)
                    {
                        if ((outlen) != null) *outlen = ((int)(a.zout - a.zout_start));
                        return a.zout_start;
                    }
                    else
                    {
                        CRuntime.free(a.zout_start);
                        return (null);
                    }

                }

                public static int stbi_zlib_decode_noheader_buffer(sbyte* obuffer, int olen, sbyte* ibuffer, int ilen)
                {
                    stbi__zbuf a = new stbi__zbuf();
                    a.zbuffer = (byte*)(ibuffer);
                    a.zbuffer_end = (byte*)(ibuffer) + ilen;
                    if ((stbi__do_zlib(&a, obuffer, (int)(olen), (int)(0), (int)(0))) != 0) return (int)(a.zout - a.zout_start);
                    else return (int)(-1);
                }

                public static stbi__pngchunk stbi__get_chunk_header(stbi__context s)
                {
                    stbi__pngchunk c = new stbi__pngchunk();
                    c.length = (uint)(stbi__get32be(s));
                    c.type = (uint)(stbi__get32be(s));
                    return (stbi__pngchunk)(c);
                }

                public static int stbi__check_png_header(stbi__context s)
                {
                    int i;
                    for (i = (int)(0); (i) < (8); ++i)
                    {
                        if (stbi__get8(s) != png_sig[i]) return (int)(stbi__err("bad png sig"));
                    }
                    return (int)(1);
                }

                public static int stbi__paeth(int a, int b, int c)
                {
                    int p = (int)(a + b - c);
                    int pa = (int)(CRuntime.abs((int)(p - a)));
                    int pb = (int)(CRuntime.abs((int)(p - b)));
                    int pc = (int)(CRuntime.abs((int)(p - c)));
                    if ((pa <= pb) && (pa <= pc)) return (int)(a);
                    if (pb <= pc) return (int)(b);
                    return (int)(c);
                }

                public static int stbi__create_png_image_raw(stbi__png a, byte* raw, uint raw_len, int out_n, uint x, uint y,
                    int depth, int colour)
                {
                    int bytes = (int)((depth) == (16) ? 2 : 1);
                    stbi__context s = a.s;
                    uint i;
                    uint j;
                    uint stride = (uint)(x * out_n * bytes);
                    uint img_len;
                    uint img_width_bytes;
                    int k;
                    int img_n = (int)(s.img_n);
                    int output_bytes = (int)(out_n * bytes);
                    int filter_bytes = (int)(img_n * bytes);
                    int width = (int)(x);
                    a._out_ = (byte*)(stbi__malloc_mad3((int)(x), (int)(y), (int)(output_bytes), (int)(0)));
                    if (a._out_ == null) return (int)(stbi__err("outofmem"));
                    img_width_bytes = (uint)(((img_n * x * depth) + 7) >> 3);
                    img_len = (uint)((img_width_bytes + 1) * y);
                    if ((raw_len) < (img_len)) return (int)(stbi__err("not enough pixels"));
                    for (j = (uint)(0); (j) < (y); ++j)
                    {
                        byte* cur = a._out_ + stride * j;
                        byte* prior;
                        int filter = (int)(*raw++);
                        if ((filter) > (4)) return (int)(stbi__err("invalid filter"));
                        if ((depth) < (8))
                        {
                            cur += x * out_n - img_width_bytes;
                            filter_bytes = (int)(1);
                            width = (int)(img_width_bytes);
                        }
                        prior = cur - stride;
                        if ((j) == (0)) filter = (int)(first_row_filter[filter]);
                        for (k = (int)(0); (k) < (filter_bytes); ++k)
                        {
                            switch (filter)
                            {
                                case STBI__F_none:
                                    cur[k] = (byte)(raw[k]);
                                    break;
                                case STBI__F_sub:
                                    cur[k] = (byte)(raw[k]);
                                    break;
                                case STBI__F_up:
                                    cur[k] = ((byte)((raw[k] + prior[k]) & 255));
                                    break;
                                case STBI__F_avg:
                                    cur[k] = ((byte)((raw[k] + (prior[k] >> 1)) & 255));
                                    break;
                                case STBI__F_paeth:
                                    cur[k] = ((byte)((raw[k] + stbi__paeth((int)(0), (int)(prior[k]), (int)(0))) & 255));
                                    break;
                                case STBI__F_avg_first:
                                    cur[k] = (byte)(raw[k]);
                                    break;
                                case STBI__F_paeth_first:
                                    cur[k] = (byte)(raw[k]);
                                    break;
                            }
                        }
                        if ((depth) == (8))
                        {
                            if (img_n != out_n) cur[img_n] = (byte)(255);
                            raw += img_n;
                            cur += out_n;
                            prior += out_n;
                        }
                        else if ((depth) == (16))
                        {
                            if (img_n != out_n)
                            {
                                cur[filter_bytes] = (byte)(255);
                                cur[filter_bytes + 1] = (byte)(255);
                            }
                            raw += filter_bytes;
                            cur += output_bytes;
                            prior += output_bytes;
                        }
                        else
                        {
                            raw += 1;
                            cur += 1;
                            prior += 1;
                        }
                        if (((depth) < (8)) || ((img_n) == (out_n)))
                        {
                            int nk = (int)((width - 1) * filter_bytes);
                            switch (filter)
                            {
                                case STBI__F_none:
                                    CRuntime.memcpy(cur, raw, (ulong)(nk));
                                    break;
                                case STBI__F_sub:
                                    for (k = (int)(0); (k) < (nk); ++k)
                                    {
                                        cur[k] = ((byte)((raw[k] + cur[k - filter_bytes]) & 255));
                                    }
                                    break;
                                case STBI__F_up:
                                    for (k = (int)(0); (k) < (nk); ++k)
                                    {
                                        cur[k] = ((byte)((raw[k] + prior[k]) & 255));
                                    }
                                    break;
                                case STBI__F_avg:
                                    for (k = (int)(0); (k) < (nk); ++k)
                                    {
                                        cur[k] = ((byte)((raw[k] + ((prior[k] + cur[k - filter_bytes]) >> 1)) & 255));
                                    }
                                    break;
                                case STBI__F_paeth:
                                    for (k = (int)(0); (k) < (nk); ++k)
                                    {
                                        cur[k] =
                                            ((byte)
                                                ((raw[k] + stbi__paeth((int)(cur[k - filter_bytes]), (int)(prior[k]), (int)(prior[k - filter_bytes]))) &
                                                 255));
                                    }
                                    break;
                                case STBI__F_avg_first:
                                    for (k = (int)(0); (k) < (nk); ++k)
                                    {
                                        cur[k] = ((byte)((raw[k] + (cur[k - filter_bytes] >> 1)) & 255));
                                    }
                                    break;
                                case STBI__F_paeth_first:
                                    for (k = (int)(0); (k) < (nk); ++k)
                                    {
                                        cur[k] = ((byte)((raw[k] + stbi__paeth((int)(cur[k - filter_bytes]), (int)(0), (int)(0))) & 255));
                                    }
                                    break;
                            }
                            raw += nk;
                        }
                        else
                        {
                            switch (filter)
                            {
                                case STBI__F_none:
                                    for (i = (uint)(x - 1);
                                        (i) >= (1);
                                        --i, cur[filter_bytes] = (byte)(255), raw += filter_bytes, cur += output_bytes, prior += output_bytes)
                                    {
                                        for (k = (int)(0); (k) < (filter_bytes); ++k)
                                        {
                                            cur[k] = (byte)(raw[k]);
                                        }
                                    }
                                    break;
                                case STBI__F_sub:
                                    for (i = (uint)(x - 1);
                                        (i) >= (1);
                                        --i, cur[filter_bytes] = (byte)(255), raw += filter_bytes, cur += output_bytes, prior += output_bytes)
                                    {
                                        for (k = (int)(0); (k) < (filter_bytes); ++k)
                                        {
                                            cur[k] = ((byte)((raw[k] + cur[k - output_bytes]) & 255));
                                        }
                                    }
                                    break;
                                case STBI__F_up:
                                    for (i = (uint)(x - 1);
                                        (i) >= (1);
                                        --i, cur[filter_bytes] = (byte)(255), raw += filter_bytes, cur += output_bytes, prior += output_bytes)
                                    {
                                        for (k = (int)(0); (k) < (filter_bytes); ++k)
                                        {
                                            cur[k] = ((byte)((raw[k] + prior[k]) & 255));
                                        }
                                    }
                                    break;
                                case STBI__F_avg:
                                    for (i = (uint)(x - 1);
                                        (i) >= (1);
                                        --i, cur[filter_bytes] = (byte)(255), raw += filter_bytes, cur += output_bytes, prior += output_bytes)
                                    {
                                        for (k = (int)(0); (k) < (filter_bytes); ++k)
                                        {
                                            cur[k] = ((byte)((raw[k] + ((prior[k] + cur[k - output_bytes]) >> 1)) & 255));
                                        }
                                    }
                                    break;
                                case STBI__F_paeth:
                                    for (i = (uint)(x - 1);
                                        (i) >= (1);
                                        --i, cur[filter_bytes] = (byte)(255), raw += filter_bytes, cur += output_bytes, prior += output_bytes)
                                    {
                                        for (k = (int)(0); (k) < (filter_bytes); ++k)
                                        {
                                            cur[k] =
                                                ((byte)
                                                    ((raw[k] + stbi__paeth((int)(cur[k - output_bytes]), (int)(prior[k]), (int)(prior[k - output_bytes]))) &
                                                     255));
                                        }
                                    }
                                    break;
                                case STBI__F_avg_first:
                                    for (i = (uint)(x - 1);
                                        (i) >= (1);
                                        --i, cur[filter_bytes] = (byte)(255), raw += filter_bytes, cur += output_bytes, prior += output_bytes)
                                    {
                                        for (k = (int)(0); (k) < (filter_bytes); ++k)
                                        {
                                            cur[k] = ((byte)((raw[k] + (cur[k - output_bytes] >> 1)) & 255));
                                        }
                                    }
                                    break;
                                case STBI__F_paeth_first:
                                    for (i = (uint)(x - 1);
                                        (i) >= (1);
                                        --i, cur[filter_bytes] = (byte)(255), raw += filter_bytes, cur += output_bytes, prior += output_bytes)
                                    {
                                        for (k = (int)(0); (k) < (filter_bytes); ++k)
                                        {
                                            cur[k] = ((byte)((raw[k] + stbi__paeth((int)(cur[k - output_bytes]), (int)(0), (int)(0))) & 255));
                                        }
                                    }
                                    break;
                            }
                            if ((depth) == (16))
                            {
                                cur = a._out_ + stride * j;
                                for (i = (uint)(0); (i) < (x); ++i, cur += output_bytes)
                                {
                                    cur[filter_bytes + 1] = (byte)(255);
                                }
                            }
                        }
                    }
                    if ((depth) < (8))
                    {
                        for (j = (uint)(0); (j) < (y); ++j)
                        {
                            byte* cur = a._out_ + stride * j;
                            byte* _in_ = a._out_ + stride * j + x * out_n - img_width_bytes;
                            byte scale = (byte)(((colour) == (0)) ? stbi__depth_scale_table[depth] : 1);
                            if ((depth) == (4))
                            {
                                for (k = (int)(x * img_n); (k) >= (2); k -= (int)(2), ++_in_)
                                {
                                    *cur++ = (byte)(scale * (*_in_ >> 4));
                                    *cur++ = (byte)(scale * ((*_in_) & 0x0f));
                                }
                                if ((k) > (0)) *cur++ = (byte)(scale * (*_in_ >> 4));
                            }
                            else if ((depth) == (2))
                            {
                                for (k = (int)(x * img_n); (k) >= (4); k -= (int)(4), ++_in_)
                                {
                                    *cur++ = (byte)(scale * (*_in_ >> 6));
                                    *cur++ = (byte)(scale * ((*_in_ >> 4) & 0x03));
                                    *cur++ = (byte)(scale * ((*_in_ >> 2) & 0x03));
                                    *cur++ = (byte)(scale * ((*_in_) & 0x03));
                                }
                                if ((k) > (0)) *cur++ = (byte)(scale * (*_in_ >> 6));
                                if ((k) > (1)) *cur++ = (byte)(scale * ((*_in_ >> 4) & 0x03));
                                if ((k) > (2)) *cur++ = (byte)(scale * ((*_in_ >> 2) & 0x03));
                            }
                            else if ((depth) == (1))
                            {
                                for (k = (int)(x * img_n); (k) >= (8); k -= (int)(8), ++_in_)
                                {
                                    *cur++ = (byte)(scale * (*_in_ >> 7));
                                    *cur++ = (byte)(scale * ((*_in_ >> 6) & 0x01));
                                    *cur++ = (byte)(scale * ((*_in_ >> 5) & 0x01));
                                    *cur++ = (byte)(scale * ((*_in_ >> 4) & 0x01));
                                    *cur++ = (byte)(scale * ((*_in_ >> 3) & 0x01));
                                    *cur++ = (byte)(scale * ((*_in_ >> 2) & 0x01));
                                    *cur++ = (byte)(scale * ((*_in_ >> 1) & 0x01));
                                    *cur++ = (byte)(scale * ((*_in_) & 0x01));
                                }
                                if ((k) > (0)) *cur++ = (byte)(scale * (*_in_ >> 7));
                                if ((k) > (1)) *cur++ = (byte)(scale * ((*_in_ >> 6) & 0x01));
                                if ((k) > (2)) *cur++ = (byte)(scale * ((*_in_ >> 5) & 0x01));
                                if ((k) > (3)) *cur++ = (byte)(scale * ((*_in_ >> 4) & 0x01));
                                if ((k) > (4)) *cur++ = (byte)(scale * ((*_in_ >> 3) & 0x01));
                                if ((k) > (5)) *cur++ = (byte)(scale * ((*_in_ >> 2) & 0x01));
                                if ((k) > (6)) *cur++ = (byte)(scale * ((*_in_ >> 1) & 0x01));
                            }
                            if (img_n != out_n)
                            {
                                int q;
                                cur = a._out_ + stride * j;
                                if ((img_n) == (1))
                                {
                                    for (q = (int)(x - 1); (q) >= (0); --q)
                                    {
                                        cur[q * 2 + 1] = (byte)(255);
                                        cur[q * 2 + 0] = (byte)(cur[q]);
                                    }
                                }
                                else
                                {
                                    for (q = (int)(x - 1); (q) >= (0); --q)
                                    {
                                        cur[q * 4 + 3] = (byte)(255);
                                        cur[q * 4 + 2] = (byte)(cur[q * 3 + 2]);
                                        cur[q * 4 + 1] = (byte)(cur[q * 3 + 1]);
                                        cur[q * 4 + 0] = (byte)(cur[q * 3 + 0]);
                                    }
                                }
                            }
                        }
                    }
                    else if ((depth) == (16))
                    {
                        byte* cur = a._out_;
                        ushort* cur16 = (ushort*)(cur);
                        for (i = (uint)(0); (i) < (x * y * out_n); ++i, cur16++, cur += 2)
                        {
                            *cur16 = (ushort)((cur[0] << 8) | cur[1]);
                        }
                    }

                    return (int)(1);
                }

                public static int stbi__create_png_image(stbi__png a, byte* image_data, uint image_data_len, int out_n, int depth,
                    int colour, int interlaced)
                {
                    int bytes = (int)((depth) == (16) ? 2 : 1);
                    int out_bytes = (int)(out_n * bytes);
                    byte* final;
                    int p;
                    if (interlaced == 0)
                        return
                            (int)
                                (stbi__create_png_image_raw(a, image_data, (uint)(image_data_len), (int)(out_n), (uint)(a.s.img_x),
                                    (uint)(a.s.img_y), (int)(depth), (int)(colour)));
                    final = (byte*)(stbi__malloc_mad3((int)(a.s.img_x), (int)(a.s.img_y), (int)(out_bytes), (int)(0)));
                    for (p = (int)(0); (p) < (7); ++p)
                    {
                        int* xorig = stackalloc int[7];
                        xorig[0] = (int)(0);
                        xorig[1] = (int)(4);
                        xorig[2] = (int)(0);
                        xorig[3] = (int)(2);
                        xorig[4] = (int)(0);
                        xorig[5] = (int)(1);
                        xorig[6] = (int)(0);
                        int* yorig = stackalloc int[7];
                        yorig[0] = (int)(0);
                        yorig[1] = (int)(0);
                        yorig[2] = (int)(4);
                        yorig[3] = (int)(0);
                        yorig[4] = (int)(2);
                        yorig[5] = (int)(0);
                        yorig[6] = (int)(1);
                        int* xspc = stackalloc int[7];
                        xspc[0] = (int)(8);
                        xspc[1] = (int)(8);
                        xspc[2] = (int)(4);
                        xspc[3] = (int)(4);
                        xspc[4] = (int)(2);
                        xspc[5] = (int)(2);
                        xspc[6] = (int)(1);
                        int* yspc = stackalloc int[7];
                        yspc[0] = (int)(8);
                        yspc[1] = (int)(8);
                        yspc[2] = (int)(8);
                        yspc[3] = (int)(4);
                        yspc[4] = (int)(4);
                        yspc[5] = (int)(2);
                        yspc[6] = (int)(2);
                        int i;
                        int j;
                        int x;
                        int y;
                        x = (int)((a.s.img_x - xorig[p] + xspc[p] - 1) / xspc[p]);
                        y = (int)((a.s.img_y - yorig[p] + yspc[p] - 1) / yspc[p]);
                        if (((x) != 0) && ((y) != 0))
                        {
                            uint img_len = (uint)(((((a.s.img_n * x * depth) + 7) >> 3) + 1) * y);
                            if (
                                stbi__create_png_image_raw(a, image_data, (uint)(image_data_len), (int)(out_n), (uint)(x), (uint)(y),
                                    (int)(depth), (int)(colour)) == 0)
                            {
                                CRuntime.free(final);
                                return (int)(0);
                            }
                            for (j = (int)(0); (j) < (y); ++j)
                            {
                                for (i = (int)(0); (i) < (x); ++i)
                                {
                                    int out_y = (int)(j * yspc[p] + yorig[p]);
                                    int out_x = (int)(i * xspc[p] + xorig[p]);
                                    CRuntime.memcpy(final + out_y * a.s.img_x * out_bytes + out_x * out_bytes, a._out_ + (j * x + i) * out_bytes,
                                        (ulong)(out_bytes));
                                }
                            }
                            CRuntime.free(a._out_);
                            image_data += img_len;
                            image_data_len -= (uint)(img_len);
                        }
                    }
                    a._out_ = final;
                    return (int)(1);
                }

                public static int stbi__compute_transparency(stbi__png z, byte* tc, int out_n)
                {
                    stbi__context s = z.s;
                    uint i;
                    uint pixel_count = (uint)(s.img_x * s.img_y);
                    byte* p = z._out_;
                    if ((out_n) == (2))
                    {
                        for (i = (uint)(0); (i) < (pixel_count); ++i)
                        {
                            p[1] = (byte)((p[0]) == (tc[0]) ? 0 : 255);
                            p += 2;
                        }
                    }
                    else
                    {
                        for (i = (uint)(0); (i) < (pixel_count); ++i)
                        {
                            if ((((p[0]) == (tc[0])) && ((p[1]) == (tc[1]))) && ((p[2]) == (tc[2]))) p[3] = (byte)(0);
                            p += 4;
                        }
                    }

                    return (int)(1);
                }

                public static int stbi__compute_transparency16(stbi__png z, ushort* tc, int out_n)
                {
                    stbi__context s = z.s;
                    uint i;
                    uint pixel_count = (uint)(s.img_x * s.img_y);
                    ushort* p = (ushort*)(z._out_);
                    if ((out_n) == (2))
                    {
                        for (i = (uint)(0); (i) < (pixel_count); ++i)
                        {
                            p[1] = (ushort)((p[0]) == (tc[0]) ? 0 : 65535);
                            p += 2;
                        }
                    }
                    else
                    {
                        for (i = (uint)(0); (i) < (pixel_count); ++i)
                        {
                            if ((((p[0]) == (tc[0])) && ((p[1]) == (tc[1]))) && ((p[2]) == (tc[2]))) p[3] = (ushort)(0);
                            p += 4;
                        }
                    }

                    return (int)(1);
                }

                public static int stbi__expand_png_palette(stbi__png a, byte* palette, int len, int pal_img_n)
                {
                    uint i;
                    uint pixel_count = (uint)(a.s.img_x * a.s.img_y);
                    byte* p;
                    byte* temp_out;
                    byte* orig = a._out_;
                    p = (byte*)(stbi__malloc_mad2((int)(pixel_count), (int)(pal_img_n), (int)(0)));
                    if ((p) == (null)) return (int)(stbi__err("outofmem"));
                    temp_out = p;
                    if ((pal_img_n) == (3))
                    {
                        for (i = (uint)(0); (i) < (pixel_count); ++i)
                        {
                            int n = (int)(orig[i] * 4);
                            p[0] = (byte)(palette[n]);
                            p[1] = (byte)(palette[n + 1]);
                            p[2] = (byte)(palette[n + 2]);
                            p += 3;
                        }
                    }
                    else
                    {
                        for (i = (uint)(0); (i) < (pixel_count); ++i)
                        {
                            int n = (int)(orig[i] * 4);
                            p[0] = (byte)(palette[n]);
                            p[1] = (byte)(palette[n + 1]);
                            p[2] = (byte)(palette[n + 2]);
                            p[3] = (byte)(palette[n + 3]);
                            p += 4;
                        }
                    }

                    CRuntime.free(a._out_);
                    a._out_ = temp_out;
                    return (int)(1);
                }

                public static void stbi_set_unpremultiply_on_load(int flag_true_if_should_unpremultiply)
                {
                    stbi__unpremultiply_on_load = (int)(flag_true_if_should_unpremultiply);
                }

                public static void stbi_convert_iphone_png_to_rgb(int flag_true_if_should_convert)
                {
                    stbi__de_iphone_flag = (int)(flag_true_if_should_convert);
                }

                public static void stbi__de_iphone(stbi__png z)
                {
                    stbi__context s = z.s;
                    uint i;
                    uint pixel_count = (uint)(s.img_x * s.img_y);
                    byte* p = z._out_;
                    if ((s.img_out_n) == (3))
                    {
                        for (i = (uint)(0); (i) < (pixel_count); ++i)
                        {
                            byte t = (byte)(p[0]);
                            p[0] = (byte)(p[2]);
                            p[2] = (byte)(t);
                            p += 3;
                        }
                    }
                    else
                    {
                        if ((stbi__unpremultiply_on_load) != 0)
                        {
                            for (i = (uint)(0); (i) < (pixel_count); ++i)
                            {
                                byte a = (byte)(p[3]);
                                byte t = (byte)(p[0]);
                                if ((a) != 0)
                                {
                                    byte half = (byte)(a / 2);
                                    p[0] = (byte)((p[2] * 255 + half) / a);
                                    p[1] = (byte)((p[1] * 255 + half) / a);
                                    p[2] = (byte)((t * 255 + half) / a);
                                }
                                else
                                {
                                    p[0] = (byte)(p[2]);
                                    p[2] = (byte)(t);
                                }
                                p += 4;
                            }
                        }
                        else
                        {
                            for (i = (uint)(0); (i) < (pixel_count); ++i)
                            {
                                byte t = (byte)(p[0]);
                                p[0] = (byte)(p[2]);
                                p[2] = (byte)(t);
                                p += 4;
                            }
                        }
                    }

                }

                public static int stbi__parse_png_file(stbi__png z, int scan, int req_comp)
                {
                    byte* palette = stackalloc byte[1024];
                    byte pal_img_n = (byte)(0);
                    byte has_trans = (byte)(0);
                    byte* tc = stackalloc byte[3];
                    ushort* tc16 = stackalloc ushort[3];
                    uint ioff = (uint)(0);
                    uint idata_limit = (uint)(0);
                    uint i;
                    uint pal_len = (uint)(0);
                    int first = (int)(1);
                    int k;
                    int interlace = (int)(0);
                    int colour = (int)(0);
                    int is_iphone = (int)(0);
                    stbi__context s = z.s;
                    z.expanded = (null);
                    z.idata = (null);
                    z._out_ = (null);
                    if (stbi__check_png_header(s) == 0) return (int)(0);
                    if ((scan) == (STBI__SCAN_type)) return (int)(1);
                    for (; ; )
                    {
                        stbi__pngchunk c = (stbi__pngchunk)(stbi__get_chunk_header(s));
                        switch (c.type)
                        {
                            case ((('C') << 24) + (('g') << 16) + (('B') << 8) + ('I')):
                                is_iphone = (int)(1);
                                stbi__skip(s, (int)(c.length));
                                break;
                            case ((('I') << 24) + (('H') << 16) + (('D') << 8) + ('R')):
                                {
                                    int comp;
                                    int filter;
                                    if (first == 0) return (int)(stbi__err("multiple IHDR"));
                                    first = (int)(0);
                                    if (c.length != 13) return (int)(stbi__err("bad IHDR len"));
                                    s.img_x = (uint)(stbi__get32be(s));
                                    if ((s.img_x) > (1 << 24)) return (int)(stbi__err("too large"));
                                    s.img_y = (uint)(stbi__get32be(s));
                                    if ((s.img_y) > (1 << 24)) return (int)(stbi__err("too large"));
                                    z.depth = (int)(stbi__get8(s));
                                    if (((((z.depth != 1) && (z.depth != 2)) && (z.depth != 4)) && (z.depth != 8)) && (z.depth != 16))
                                        return (int)(stbi__err("1/2/4/8/16-bit only"));
                                    colour = (int)(stbi__get8(s));
                                    if ((colour) > (6)) return (int)(stbi__err("bad ctype"));
                                    if (((colour) == (3)) && ((z.depth) == (16))) return (int)(stbi__err("bad ctype"));
                                    if ((colour) == (3)) pal_img_n = (byte)(3);
                                    else if ((colour & 1) != 0) return (int)(stbi__err("bad ctype"));
                                    comp = (int)(stbi__get8(s));
                                    if ((comp) != 0) return (int)(stbi__err("bad comp method"));
                                    filter = (int)(stbi__get8(s));
                                    if ((filter) != 0) return (int)(stbi__err("bad filter method"));
                                    interlace = (int)(stbi__get8(s));
                                    if ((interlace) > (1)) return (int)(stbi__err("bad interlace method"));
                                    if ((s.img_x == 0) || (s.img_y == 0)) return (int)(stbi__err("0-pixel image"));
                                    if (pal_img_n == 0)
                                    {
                                        s.img_n = (int)(((colour & 2) != 0 ? 3 : 1) + ((colour & 4) != 0 ? 1 : 0));
                                        if (((1 << 30) / s.img_x / s.img_n) < (s.img_y)) return (int)(stbi__err("too large"));
                                        if ((scan) == (STBI__SCAN_header)) return (int)(1);
                                    }
                                    else
                                    {
                                        s.img_n = (int)(1);
                                        if (((1 << 30) / s.img_x / 4) < (s.img_y)) return (int)(stbi__err("too large"));
                                    }
                                    break;
                                }
                            case ((('P') << 24) + (('L') << 16) + (('T') << 8) + ('E')):
                                {
                                    if ((first) != 0) return (int)(stbi__err("first not IHDR"));
                                    if ((c.length) > (256 * 3)) return (int)(stbi__err("invalid PLTE"));
                                    pal_len = (uint)(c.length / 3);
                                    if (pal_len * 3 != c.length) return (int)(stbi__err("invalid PLTE"));
                                    for (i = (uint)(0); (i) < (pal_len); ++i)
                                    {
                                        palette[i * 4 + 0] = (byte)(stbi__get8(s));
                                        palette[i * 4 + 1] = (byte)(stbi__get8(s));
                                        palette[i * 4 + 2] = (byte)(stbi__get8(s));
                                        palette[i * 4 + 3] = (byte)(255);
                                    }
                                    break;
                                }
                            case ((('t') << 24) + (('R') << 16) + (('N') << 8) + ('S')):
                                {
                                    if ((first) != 0) return (int)(stbi__err("first not IHDR"));
                                    if ((z.idata) != null) return (int)(stbi__err("tRNS after IDAT"));
                                    if ((pal_img_n) != 0)
                                    {
                                        if ((scan) == (STBI__SCAN_header))
                                        {
                                            s.img_n = (int)(4);
                                            return (int)(1);
                                        }
                                        if ((pal_len) == (0)) return (int)(stbi__err("tRNS before PLTE"));
                                        if ((c.length) > (pal_len)) return (int)(stbi__err("bad tRNS len"));
                                        pal_img_n = (byte)(4);
                                        for (i = (uint)(0); (i) < (c.length); ++i)
                                        {
                                            palette[i * 4 + 3] = (byte)(stbi__get8(s));
                                        }
                                    }
                                    else
                                    {
                                        if ((s.img_n & 1) == 0) return (int)(stbi__err("tRNS with alpha"));
                                        if (c.length != (uint)(s.img_n) * 2) return (int)(stbi__err("bad tRNS len"));
                                        has_trans = (byte)(1);
                                        if ((z.depth) == (16))
                                        {
                                            for (k = (int)(0); (k) < (s.img_n); ++k)
                                            {
                                                tc16[k] = ((ushort)(stbi__get16be(s)));
                                            }
                                        }
                                        else
                                        {
                                            for (k = (int)(0); (k) < (s.img_n); ++k)
                                            {
                                                tc[k] = (byte)((byte)(stbi__get16be(s) & 255) * stbi__depth_scale_table[z.depth]);
                                            }
                                        }
                                    }
                                    break;
                                }
                            case ((('I') << 24) + (('D') << 16) + (('A') << 8) + ('T')):
                                {
                                    if ((first) != 0) return (int)(stbi__err("first not IHDR"));
                                    if (((pal_img_n) != 0) && (pal_len == 0)) return (int)(stbi__err("no PLTE"));
                                    if ((scan) == (STBI__SCAN_header))
                                    {
                                        s.img_n = (int)(pal_img_n);
                                        return (int)(1);
                                    }
                                    if (((int)(ioff + c.length)) < ((int)(ioff))) return (int)(0);
                                    if ((ioff + c.length) > (idata_limit))
                                    {
                                        uint idata_limit_old = (uint)(idata_limit);
                                        byte* p;
                                        if ((idata_limit) == (0)) idata_limit = (uint)((c.length) > (4096) ? c.length : 4096);
                                        while ((ioff + c.length) > (idata_limit))
                                        {
                                            idata_limit *= (uint)(2);
                                        }
                                        p = (byte*)(CRuntime.realloc(z.idata, (ulong)(idata_limit)));
                                        if ((p) == (null)) return (int)(stbi__err("outofmem"));
                                        z.idata = p;
                                    }
                                    if (stbi__getn(s, z.idata + ioff, (int)(c.length)) == 0) return (int)(stbi__err("outofdata"));
                                    ioff += (uint)(c.length);
                                    break;
                                }
                            case ((('I') << 24) + (('E') << 16) + (('N') << 8) + ('D')):
                                {
                                    uint raw_len;
                                    uint bpl;
                                    if ((first) != 0) return (int)(stbi__err("first not IHDR"));
                                    if (scan != STBI__SCAN_load) return (int)(1);
                                    if ((z.idata) == (null)) return (int)(stbi__err("no IDAT"));
                                    bpl = (uint)((s.img_x * z.depth + 7) / 8);
                                    raw_len = (uint)(bpl * s.img_y * s.img_n + s.img_y);
                                    z.expanded =
                                        (byte*)
                                            (stbi_zlib_decode_malloc_guesssize_headerflag((sbyte*)(z.idata), (int)(ioff), (int)(raw_len),
                                                (int*)(&raw_len), is_iphone != 0 ? 0 : 1));
                                    if ((z.expanded) == (null)) return (int)(0);
                                    CRuntime.free(z.idata);
                                    z.idata = (null);
                                    if (((((req_comp) == (s.img_n + 1)) && (req_comp != 3)) && (pal_img_n == 0)) || ((has_trans) != 0))
                                        s.img_out_n = (int)(s.img_n + 1);
                                    else s.img_out_n = (int)(s.img_n);
                                    if (
                                        stbi__create_png_image(z, z.expanded, (uint)(raw_len), (int)(s.img_out_n), (int)(z.depth), (int)(colour),
                                            (int)(interlace)) == 0) return (int)(0);
                                    if ((has_trans) != 0)
                                    {
                                        if ((z.depth) == (16))
                                        {
                                            if (stbi__compute_transparency16(z, tc16, (int)(s.img_out_n)) == 0) return (int)(0);
                                        }
                                        else
                                        {
                                            if (stbi__compute_transparency(z, tc, (int)(s.img_out_n)) == 0) return (int)(0);
                                        }
                                    }
                                    if ((((is_iphone) != 0) && ((stbi__de_iphone_flag) != 0)) && ((s.img_out_n) > (2))) stbi__de_iphone(z);
                                    if ((pal_img_n) != 0)
                                    {
                                        s.img_n = (int)(pal_img_n);
                                        s.img_out_n = (int)(pal_img_n);
                                        if ((req_comp) >= (3)) s.img_out_n = (int)(req_comp);
                                        if (stbi__expand_png_palette(z, palette, (int)(pal_len), (int)(s.img_out_n)) == 0) return (int)(0);
                                    }
                                    else if ((has_trans) != 0)
                                    {
                                        ++s.img_n;
                                    }
                                    CRuntime.free(z.expanded);
                                    z.expanded = (null);
                                    return (int)(1);
                                }
                            default:
                                if ((first) != 0) return (int)(stbi__err("first not IHDR"));
                                if ((c.type & (1 << 29)) == (0))
                                {
                                    string invalid_chunk = "XXXX PNG chunk not known";
                                    return (int)(stbi__err(invalid_chunk));
                                }
                                stbi__skip(s, (int)(c.length));
                                break;
                        }
                        stbi__get32be(s);
                    }
                }

                public static void* stbi__do_png(stbi__png p, int* x, int* y, int* n, int req_comp, stbi__result_info* ri)
                {
                    void* result = (null);
                    if (((req_comp) < (0)) || ((req_comp) > (4)))
                        return ((byte*)((ulong)((stbi__err("bad req_comp")) != 0 ? ((byte*)null) : (null))));
                    if ((stbi__parse_png_file(p, (int)(STBI__SCAN_load), (int)(req_comp))) != 0)
                    {
                        if ((p.depth) < (8)) ri->bits_per_channel = (int)(8);
                        else ri->bits_per_channel = (int)(p.depth);
                        result = p._out_;
                        p._out_ = (null);
                        if (((req_comp) != 0) && (req_comp != p.s.img_out_n))
                        {
                            if ((ri->bits_per_channel) == (8))
                                result = stbi__convert_format((byte*)(result), (int)(p.s.img_out_n), (int)(req_comp), (uint)(p.s.img_x),
                                    (uint)(p.s.img_y));
                            else
                                result = stbi__convert_format16((ushort*)(result), (int)(p.s.img_out_n), (int)(req_comp), (uint)(p.s.img_x),
                                    (uint)(p.s.img_y));
                            p.s.img_out_n = (int)(req_comp);
                            if ((result) == (null)) return result;
                        }
                        *x = (int)(p.s.img_x);
                        *y = (int)(p.s.img_y);
                        if ((n) != null) *n = (int)(p.s.img_n);
                    }

                    CRuntime.free(p._out_);
                    p._out_ = (null);
                    CRuntime.free(p.expanded);
                    p.expanded = (null);
                    CRuntime.free(p.idata);
                    p.idata = (null);
                    return result;
                }

                public static void* stbi__png_load(stbi__context s, int* x, int* y, int* comp, int req_comp, stbi__result_info* ri)
                {
                    stbi__png p = new stbi__png();
                    p.s = s;
                    return stbi__do_png(p, x, y, comp, (int)(req_comp), ri);
                }

                public static int stbi__png_test(stbi__context s)
                {
                    int r;
                    r = (int)(stbi__check_png_header(s));
                    stbi__rewind(s);
                    return (int)(r);
                }

                public static int stbi__png_info_raw(stbi__png p, int* x, int* y, int* comp)
                {
                    if (stbi__parse_png_file(p, (int)(STBI__SCAN_header), (int)(0)) == 0)
                    {
                        stbi__rewind(p.s);
                        return (int)(0);
                    }

                    if ((x) != null) *x = (int)(p.s.img_x);
                    if ((y) != null) *y = (int)(p.s.img_y);
                    if ((comp) != null) *comp = (int)(p.s.img_n);
                    return (int)(1);
                }

                public static int stbi__png_info(stbi__context s, int* x, int* y, int* comp)
                {
                    stbi__png p = new stbi__png();
                    p.s = s;
                    return (int)(stbi__png_info_raw(p, x, y, comp));
                }

                public static int stbi__bmp_test_raw(stbi__context s)
                {
                    int r;
                    int sz;
                    if (stbi__get8(s) != 'B') return (int)(0);
                    if (stbi__get8(s) != 'M') return (int)(0);
                    stbi__get32le(s);
                    stbi__get16le(s);
                    stbi__get16le(s);
                    stbi__get32le(s);
                    sz = (int)(stbi__get32le(s));
                    r = (int)((((((sz) == (12)) || ((sz) == (40))) || ((sz) == (56))) || ((sz) == (108))) || ((sz) == (124)) ? 1 : 0);
                    return (int)(r);
                }

                public static int stbi__bmp_test(stbi__context s)
                {
                    int r = (int)(stbi__bmp_test_raw(s));
                    stbi__rewind(s);
                    return (int)(r);
                }

                public static int stbi__high_bit(uint z)
                {
                    int n = (int)(0);
                    if ((z) == (0)) return (int)(-1);
                    if ((z) >= (0x10000))
                    {
                        n += (int)(16);
                        z >>= 16;
                    }

                    if ((z) >= (0x00100))
                    {
                        n += (int)(8);
                        z >>= 8;
                    }

                    if ((z) >= (0x00010))
                    {
                        n += (int)(4);
                        z >>= 4;
                    }

                    if ((z) >= (0x00004))
                    {
                        n += (int)(2);
                        z >>= 2;
                    }

                    if ((z) >= (0x00002))
                    {
                        n += (int)(1);
                        z >>= 1;
                    }

                    return (int)(n);
                }

                public static int stbi__bitcount(uint a)
                {
                    a = (uint)((a & 0x55555555) + ((a >> 1) & 0x55555555));
                    a = (uint)((a & 0x33333333) + ((a >> 2) & 0x33333333));
                    a = (uint)((a + (a >> 4)) & 0x0f0f0f0f);
                    a = (uint)(a + (a >> 8));
                    a = (uint)(a + (a >> 16));
                    return (int)(a & 0xff);
                }

                public static int stbi__shiftsigned(int v, int shift, int bits)
                {
                    int result;
                    int z = (int)(0);
                    if ((shift) < (0)) v <<= -shift;
                    else v >>= shift;
                    result = (int)(v);
                    z = (int)(bits);
                    while ((z) < (8))
                    {
                        result += (int)(v >> z);
                        z += (int)(bits);
                    }
                    return (int)(result);
                }

                public static void* stbi__bmp_parse_header(stbi__context s, stbi__bmp_data* info)
                {
                    int hsz;
                    if ((stbi__get8(s) != 'B') || (stbi__get8(s) != 'M'))
                        return ((byte*)((ulong)((stbi__err("not BMP")) != 0 ? ((byte*)null) : (null))));
                    stbi__get32le(s);
                    stbi__get16le(s);
                    stbi__get16le(s);
                    info->offset = (int)(stbi__get32le(s));
                    info->hsz = (int)(hsz = (int)(stbi__get32le(s)));
                    info->mr = (uint)(info->mg = (uint)(info->mb = (uint)(info->ma = (uint)(0))));
                    if (((((hsz != 12) && (hsz != 40)) && (hsz != 56)) && (hsz != 108)) && (hsz != 124))
                        return ((byte*)((ulong)((stbi__err("unknown BMP")) != 0 ? ((byte*)null) : (null))));
                    if ((hsz) == (12))
                    {
                        s.img_x = (uint)(stbi__get16le(s));
                        s.img_y = (uint)(stbi__get16le(s));
                    }
                    else
                    {
                        s.img_x = (uint)(stbi__get32le(s));
                        s.img_y = (uint)(stbi__get32le(s));
                    }

                    if (stbi__get16le(s) != 1) return ((byte*)((ulong)((stbi__err("bad BMP")) != 0 ? ((byte*)null) : (null))));
                    info->bpp = (int)(stbi__get16le(s));
                    if ((info->bpp) == (1)) return ((byte*)((ulong)((stbi__err("monochrome")) != 0 ? ((byte*)null) : (null))));
                    if (hsz != 12)
                    {
                        int compress = (int)(stbi__get32le(s));
                        if (((compress) == (1)) || ((compress) == (2)))
                            return ((byte*)((ulong)((stbi__err("BMP RLE")) != 0 ? ((byte*)null) : (null))));
                        stbi__get32le(s);
                        stbi__get32le(s);
                        stbi__get32le(s);
                        stbi__get32le(s);
                        stbi__get32le(s);
                        if (((hsz) == (40)) || ((hsz) == (56)))
                        {
                            if ((hsz) == (56))
                            {
                                stbi__get32le(s);
                                stbi__get32le(s);
                                stbi__get32le(s);
                                stbi__get32le(s);
                            }
                            if (((info->bpp) == (16)) || ((info->bpp) == (32)))
                            {
                                if ((compress) == (0))
                                {
                                    if ((info->bpp) == (32))
                                    {
                                        info->mr = (uint)(0xffu << 16);
                                        info->mg = (uint)(0xffu << 8);
                                        info->mb = (uint)(0xffu << 0);
                                        info->ma = (uint)(0xffu << 24);
                                        info->all_a = (uint)(0);
                                    }
                                    else
                                    {
                                        info->mr = (uint)(31u << 10);
                                        info->mg = (uint)(31u << 5);
                                        info->mb = (uint)(31u << 0);
                                    }
                                }
                                else if ((compress) == (3))
                                {
                                    info->mr = (uint)(stbi__get32le(s));
                                    info->mg = (uint)(stbi__get32le(s));
                                    info->mb = (uint)(stbi__get32le(s));
                                    if (((info->mr) == (info->mg)) && ((info->mg) == (info->mb)))
                                    {
                                        return ((byte*)((ulong)((stbi__err("bad BMP")) != 0 ? ((byte*)null) : (null))));
                                    }
                                }
                                else return ((byte*)((ulong)((stbi__err("bad BMP")) != 0 ? ((byte*)null) : (null))));
                            }
                        }
                        else
                        {
                            int i;
                            if ((hsz != 108) && (hsz != 124))
                                return ((byte*)((ulong)((stbi__err("bad BMP")) != 0 ? ((byte*)null) : (null))));
                            info->mr = (uint)(stbi__get32le(s));
                            info->mg = (uint)(stbi__get32le(s));
                            info->mb = (uint)(stbi__get32le(s));
                            info->ma = (uint)(stbi__get32le(s));
                            stbi__get32le(s);
                            for (i = (int)(0); (i) < (12); ++i)
                            {
                                stbi__get32le(s);
                            }
                            if ((hsz) == (124))
                            {
                                stbi__get32le(s);
                                stbi__get32le(s);
                                stbi__get32le(s);
                                stbi__get32le(s);
                            }
                        }
                    }

                    return (void*)(1);
                }

                public static void* stbi__bmp_load(stbi__context s, int* x, int* y, int* comp, int req_comp, stbi__result_info* ri)
                {
                    byte* _out_;
                    uint mr = (uint)(0);
                    uint mg = (uint)(0);
                    uint mb = (uint)(0);
                    uint ma = (uint)(0);
                    uint all_a;
                    byte* pal = stackalloc byte[256 * 4];
                    int psize = (int)(0);
                    int i;
                    int j;
                    int width;
                    int flip_vertically;
                    int pad;
                    int target;
                    stbi__bmp_data info = new stbi__bmp_data();
                    info.all_a = (uint)(255);
                    if ((stbi__bmp_parse_header(s, &info)) == (null)) return (null);
                    flip_vertically = (int)(((int)(s.img_y)) > (0) ? 1 : 0);
                    s.img_y = (uint)(CRuntime.abs((int)(s.img_y)));
                    mr = (uint)(info.mr);
                    mg = (uint)(info.mg);
                    mb = (uint)(info.mb);
                    ma = (uint)(info.ma);
                    all_a = (uint)(info.all_a);
                    if ((info.hsz) == (12))
                    {
                        if ((info.bpp) < (24)) psize = (int)((info.offset - 14 - 24) / 3);
                    }
                    else
                    {
                        if ((info.bpp) < (16)) psize = (int)((info.offset - 14 - info.hsz) >> 2);
                    }

                    s.img_n = (int)((ma) != 0 ? 4 : 3);
                    if (((req_comp) != 0) && ((req_comp) >= (3))) target = (int)(req_comp);
                    else target = (int)(s.img_n);
                    if (stbi__mad3sizes_valid((int)(target), (int)(s.img_x), (int)(s.img_y), (int)(0)) == 0)
                        return ((byte*)((ulong)((stbi__err("too large")) != 0 ? ((byte*)null) : (null))));
                    _out_ = (byte*)(stbi__malloc_mad3((int)(target), (int)(s.img_x), (int)(s.img_y), (int)(0)));
                    if (_out_ == null) return ((byte*)((ulong)((stbi__err("outofmem")) != 0 ? ((byte*)null) : (null))));
                    if ((info.bpp) < (16))
                    {
                        int z = (int)(0);
                        if (((psize) == (0)) || ((psize) > (256)))
                        {
                            CRuntime.free(_out_);
                            return ((byte*)((ulong)((stbi__err("invalid")) != 0 ? ((byte*)null) : (null))));
                        }
                        for (i = (int)(0); (i) < (psize); ++i)
                        {
                            pal[i * 4 + 2] = (byte)(stbi__get8(s));
                            pal[i * 4 + 1] = (byte)(stbi__get8(s));
                            pal[i * 4 + 0] = (byte)(stbi__get8(s));
                            if (info.hsz != 12) stbi__get8(s);
                            pal[i * 4 + 3] = (byte)(255);
                        }
                        stbi__skip(s, (int)(info.offset - 14 - info.hsz - psize * ((info.hsz) == (12) ? 3 : 4)));
                        if ((info.bpp) == (4)) width = (int)((s.img_x + 1) >> 1);
                        else if ((info.bpp) == (8)) width = (int)(s.img_x);
                        else
                        {
                            CRuntime.free(_out_);
                            return ((byte*)((ulong)((stbi__err("bad bpp")) != 0 ? ((byte*)null) : (null))));
                        }
                        pad = (int)((-width) & 3);
                        for (j = (int)(0); (j) < ((int)(s.img_y)); ++j)
                        {
                            for (i = (int)(0); (i) < ((int)(s.img_x)); i += (int)(2))
                            {
                                int v = (int)(stbi__get8(s));
                                int v2 = (int)(0);
                                if ((info.bpp) == (4))
                                {
                                    v2 = (int)(v & 15);
                                    v >>= 4;
                                }
                                _out_[z++] = (byte)(pal[v * 4 + 0]);
                                _out_[z++] = (byte)(pal[v * 4 + 1]);
                                _out_[z++] = (byte)(pal[v * 4 + 2]);
                                if ((target) == (4)) _out_[z++] = (byte)(255);
                                if ((i + 1) == ((int)(s.img_x))) break;
                                v = (int)(((info.bpp) == (8)) ? stbi__get8(s) : v2);
                                _out_[z++] = (byte)(pal[v * 4 + 0]);
                                _out_[z++] = (byte)(pal[v * 4 + 1]);
                                _out_[z++] = (byte)(pal[v * 4 + 2]);
                                if ((target) == (4)) _out_[z++] = (byte)(255);
                            }
                            stbi__skip(s, (int)(pad));
                        }
                    }
                    else
                    {
                        int rshift = (int)(0);
                        int gshift = (int)(0);
                        int bshift = (int)(0);
                        int ashift = (int)(0);
                        int rcount = (int)(0);
                        int gcount = (int)(0);
                        int bcount = (int)(0);
                        int acount = (int)(0);
                        int z = (int)(0);
                        int easy = (int)(0);
                        stbi__skip(s, (int)(info.offset - 14 - info.hsz));
                        if ((info.bpp) == (24)) width = (int)(3 * s.img_x);
                        else if ((info.bpp) == (16)) width = (int)(2 * s.img_x);
                        else width = (int)(0);
                        pad = (int)((-width) & 3);
                        if ((info.bpp) == (24))
                        {
                            easy = (int)(1);
                        }
                        else if ((info.bpp) == (32))
                        {
                            if (((((mb) == (0xff)) && ((mg) == (0xff00))) && ((mr) == (0x00ff0000))) && ((ma) == (0xff000000)))
                                easy = (int)(2);
                        }
                        if (easy == 0)
                        {
                            if (((mr == 0) || (mg == 0)) || (mb == 0))
                            {
                                CRuntime.free(_out_);
                                return ((byte*)((ulong)((stbi__err("bad masks")) != 0 ? ((byte*)null) : (null))));
                            }
                            rshift = (int)(stbi__high_bit((uint)(mr)) - 7);
                            rcount = (int)(stbi__bitcount((uint)(mr)));
                            gshift = (int)(stbi__high_bit((uint)(mg)) - 7);
                            gcount = (int)(stbi__bitcount((uint)(mg)));
                            bshift = (int)(stbi__high_bit((uint)(mb)) - 7);
                            bcount = (int)(stbi__bitcount((uint)(mb)));
                            ashift = (int)(stbi__high_bit((uint)(ma)) - 7);
                            acount = (int)(stbi__bitcount((uint)(ma)));
                        }
                        for (j = (int)(0); (j) < ((int)(s.img_y)); ++j)
                        {
                            if ((easy) != 0)
                            {
                                for (i = (int)(0); (i) < ((int)(s.img_x)); ++i)
                                {
                                    byte a;
                                    _out_[z + 2] = (byte)(stbi__get8(s));
                                    _out_[z + 1] = (byte)(stbi__get8(s));
                                    _out_[z + 0] = (byte)(stbi__get8(s));
                                    z += (int)(3);
                                    a = (byte)((easy) == (2) ? stbi__get8(s) : 255);
                                    all_a |= (uint)(a);
                                    if ((target) == (4)) _out_[z++] = (byte)(a);
                                }
                            }
                            else
                            {
                                int bpp = (int)(info.bpp);
                                for (i = (int)(0); (i) < ((int)(s.img_x)); ++i)
                                {
                                    uint v = (uint)((bpp) == (16) ? (uint)(stbi__get16le(s)) : stbi__get32le(s));
                                    int a;
                                    _out_[z++] = ((byte)((stbi__shiftsigned((int)(v & mr), (int)(rshift), (int)(rcount))) & 255));
                                    _out_[z++] = ((byte)((stbi__shiftsigned((int)(v & mg), (int)(gshift), (int)(gcount))) & 255));
                                    _out_[z++] = ((byte)((stbi__shiftsigned((int)(v & mb), (int)(bshift), (int)(bcount))) & 255));
                                    a = (int)((ma) != 0 ? stbi__shiftsigned((int)(v & ma), (int)(ashift), (int)(acount)) : 255);
                                    all_a |= (uint)(a);
                                    if ((target) == (4)) _out_[z++] = ((byte)((a) & 255));
                                }
                            }
                            stbi__skip(s, (int)(pad));
                        }
                    }

                    if (((target) == (4)) && ((all_a) == (0)))
                        for (i = (int)(4 * s.img_x * s.img_y - 1); (i) >= (0); i -= (int)(4))
                        {
                            _out_[i] = (byte)(255);
                        }
                    if ((flip_vertically) != 0)
                    {
                        byte t;
                        for (j = (int)(0); (j) < ((int)(s.img_y) >> 1); ++j)
                        {
                            byte* p1 = _out_ + j * s.img_x * target;
                            byte* p2 = _out_ + (s.img_y - 1 - j) * s.img_x * target;
                            for (i = (int)(0); (i) < ((int)(s.img_x) * target); ++i)
                            {
                                t = (byte)(p1[i]);
                                p1[i] = (byte)(p2[i]);
                                p2[i] = (byte)(t);
                            }
                        }
                    }

                    if (((req_comp) != 0) && (req_comp != target))
                    {
                        _out_ = stbi__convert_format(_out_, (int)(target), (int)(req_comp), (uint)(s.img_x), (uint)(s.img_y));
                        if ((_out_) == (null)) return _out_;
                    }

                    *x = (int)(s.img_x);
                    *y = (int)(s.img_y);
                    if ((comp) != null) *comp = (int)(s.img_n);
                    return _out_;
                }

                public static int stbi__tga_get_comp(int bits_per_pixel, int is_grey, int* is_rgb16)
                {
                    if ((is_rgb16) != null) *is_rgb16 = (int)(0);
                    switch (bits_per_pixel)
                    {
                        case 8:
                            return (int)(STBI_grey);
                        case 15:
                        case 16:
                            if (((bits_per_pixel) == (16)) && ((is_grey) != 0)) return (int)(STBI_grey_alpha);
                            if ((is_rgb16) != null) *is_rgb16 = (int)(1);
                            return (int)(STBI_rgb);
                        case 24:
                        case 32:
                            return (int)(bits_per_pixel / 8);
                        default:
                            return (int)(0);
                    }

                }

                public static int stbi__tga_info(stbi__context s, int* x, int* y, int* comp)
                {
                    int tga_w;
                    int tga_h;
                    int tga_comp;
                    int tga_image_type;
                    int tga_bits_per_pixel;
                    int tga_colourmap_bpp;
                    int sz;
                    int tga_colourmap_type;
                    stbi__get8(s);
                    tga_colourmap_type = (int)(stbi__get8(s));
                    if ((tga_colourmap_type) > (1))
                    {
                        stbi__rewind(s);
                        return (int)(0);
                    }

                    tga_image_type = (int)(stbi__get8(s));
                    if ((tga_colourmap_type) == (1))
                    {
                        if ((tga_image_type != 1) && (tga_image_type != 9))
                        {
                            stbi__rewind(s);
                            return (int)(0);
                        }
                        stbi__skip(s, (int)(4));
                        sz = (int)(stbi__get8(s));
                        if (((((sz != 8) && (sz != 15)) && (sz != 16)) && (sz != 24)) && (sz != 32))
                        {
                            stbi__rewind(s);
                            return (int)(0);
                        }
                        stbi__skip(s, (int)(4));
                        tga_colourmap_bpp = (int)(sz);
                    }
                    else
                    {
                        if ((((tga_image_type != 2) && (tga_image_type != 3)) && (tga_image_type != 10)) && (tga_image_type != 11))
                        {
                            stbi__rewind(s);
                            return (int)(0);
                        }
                        stbi__skip(s, (int)(9));
                        tga_colourmap_bpp = (int)(0);
                    }

                    tga_w = (int)(stbi__get16le(s));
                    if ((tga_w) < (1))
                    {
                        stbi__rewind(s);
                        return (int)(0);
                    }

                    tga_h = (int)(stbi__get16le(s));
                    if ((tga_h) < (1))
                    {
                        stbi__rewind(s);
                        return (int)(0);
                    }

                    tga_bits_per_pixel = (int)(stbi__get8(s));
                    stbi__get8(s);
                    if (tga_colourmap_bpp != 0)
                    {
                        if ((tga_bits_per_pixel != 8) && (tga_bits_per_pixel != 16))
                        {
                            stbi__rewind(s);
                            return (int)(0);
                        }
                        tga_comp = (int)(stbi__tga_get_comp((int)(tga_colourmap_bpp), (int)(0), (null)));
                    }
                    else
                    {
                        tga_comp =
                            (int)
                                (stbi__tga_get_comp((int)(tga_bits_per_pixel),
                                    (((tga_image_type) == (3))) || (((tga_image_type) == (11))) ? 1 : 0, (null)));
                    }

                    if (tga_comp == 0)
                    {
                        stbi__rewind(s);
                        return (int)(0);
                    }

                    if ((x) != null) *x = (int)(tga_w);
                    if ((y) != null) *y = (int)(tga_h);
                    if ((comp) != null) *comp = (int)(tga_comp);
                    return (int)(1);
                }

                public static int stbi__tga_test(stbi__context s)
                {
                    int res = (int)(0);
                    int sz;
                    int tga_colour_type;
                    stbi__get8(s);
                    tga_colour_type = (int)(stbi__get8(s));
                    if ((tga_colour_type) > (1)) goto errorEnd;
                    sz = (int)(stbi__get8(s));
                    if ((tga_colour_type) == (1))
                    {
                        if ((sz != 1) && (sz != 9)) goto errorEnd;
                        stbi__skip(s, (int)(4));
                        sz = (int)(stbi__get8(s));
                        if (((((sz != 8) && (sz != 15)) && (sz != 16)) && (sz != 24)) && (sz != 32)) goto errorEnd;
                        stbi__skip(s, (int)(4));
                    }
                    else
                    {
                        if ((((sz != 2) && (sz != 3)) && (sz != 10)) && (sz != 11)) goto errorEnd;
                        stbi__skip(s, (int)(9));
                    }

                    if ((stbi__get16le(s)) < (1)) goto errorEnd;
                    if ((stbi__get16le(s)) < (1)) goto errorEnd;
                    sz = (int)(stbi__get8(s));
                    if ((((tga_colour_type) == (1)) && (sz != 8)) && (sz != 16)) goto errorEnd;
                    if (((((sz != 8) && (sz != 15)) && (sz != 16)) && (sz != 24)) && (sz != 32)) goto errorEnd;
                    res = (int)(1);
                    errorEnd:
                    ;
                    stbi__rewind(s);
                    return (int)(res);
                }

                public static void stbi__tga_read_rgb16(stbi__context s, byte* _out_)
                {
                    ushort px = (ushort)(stbi__get16le(s));
                    ushort fiveBitMask = (ushort)(31);
                    int r = (int)((px >> 10) & fiveBitMask);
                    int g = (int)((px >> 5) & fiveBitMask);
                    int b = (int)(px & fiveBitMask);
                    _out_[0] = ((byte)((r * 255) / 31));
                    _out_[1] = ((byte)((g * 255) / 31));
                    _out_[2] = ((byte)((b * 255) / 31));
                }

                public static void* stbi__tga_load(stbi__context s, int* x, int* y, int* comp, int req_comp, stbi__result_info* ri)
                {
                    int tga_offset = (int)(stbi__get8(s));
                    int tga_indexed = (int)(stbi__get8(s));
                    int tga_image_type = (int)(stbi__get8(s));
                    int tga_is_RLE = (int)(0);
                    int tga_palette_start = (int)(stbi__get16le(s));
                    int tga_palette_len = (int)(stbi__get16le(s));
                    int tga_palette_bits = (int)(stbi__get8(s));
                    int tga_x_origin = (int)(stbi__get16le(s));
                    int tga_y_origin = (int)(stbi__get16le(s));
                    int tga_width = (int)(stbi__get16le(s));
                    int tga_height = (int)(stbi__get16le(s));
                    int tga_bits_per_pixel = (int)(stbi__get8(s));
                    int tga_comp;
                    int tga_rgb16 = (int)(0);
                    int tga_inverted = (int)(stbi__get8(s));
                    byte* tga_data;
                    byte* tga_palette = (null);
                    int i;
                    int j;
                    byte* raw_data = stackalloc byte[4];
                    raw_data[0] = (byte)(0);

                    int RLE_count = (int)(0);
                    int RLE_repeating = (int)(0);
                    int read_next_pixel = (int)(1);
                    if ((tga_image_type) >= (8))
                    {
                        tga_image_type -= (int)(8);
                        tga_is_RLE = (int)(1);
                    }

                    tga_inverted = (int)(1 - ((tga_inverted >> 5) & 1));
                    if ((tga_indexed) != 0) tga_comp = (int)(stbi__tga_get_comp((int)(tga_palette_bits), (int)(0), &tga_rgb16));
                    else tga_comp = (int)(stbi__tga_get_comp((int)(tga_bits_per_pixel), (tga_image_type) == (3) ? 1 : 0, &tga_rgb16));
                    if (tga_comp == 0) return ((byte*)((ulong)((stbi__err("bad format")) != 0 ? ((byte*)null) : (null))));
                    *x = (int)(tga_width);
                    *y = (int)(tga_height);
                    if ((comp) != null) *comp = (int)(tga_comp);
                    if (stbi__mad3sizes_valid((int)(tga_width), (int)(tga_height), (int)(tga_comp), (int)(0)) == 0)
                        return ((byte*)((ulong)((stbi__err("too large")) != 0 ? ((byte*)null) : (null))));
                    tga_data = (byte*)(stbi__malloc_mad3((int)(tga_width), (int)(tga_height), (int)(tga_comp), (int)(0)));
                    if (tga_data == null) return ((byte*)((ulong)((stbi__err("outofmem")) != 0 ? ((byte*)null) : (null))));
                    stbi__skip(s, (int)(tga_offset));
                    if (((tga_indexed == 0) && (tga_is_RLE == 0)) && (tga_rgb16 == 0))
                    {
                        for (i = (int)(0); (i) < (tga_height); ++i)
                        {
                            int row = (int)((tga_inverted) != 0 ? tga_height - i - 1 : i);
                            byte* tga_row = tga_data + row * tga_width * tga_comp;
                            stbi__getn(s, tga_row, (int)(tga_width * tga_comp));
                        }
                    }
                    else
                    {
                        if ((tga_indexed) != 0)
                        {
                            stbi__skip(s, (int)(tga_palette_start));
                            tga_palette = (byte*)(stbi__malloc_mad2((int)(tga_palette_len), (int)(tga_comp), (int)(0)));
                            if (tga_palette == null)
                            {
                                CRuntime.free(tga_data);
                                return ((byte*)((ulong)((stbi__err("outofmem")) != 0 ? ((byte*)null) : (null))));
                            }
                            if ((tga_rgb16) != 0)
                            {
                                byte* pal_entry = tga_palette;
                                for (i = (int)(0); (i) < (tga_palette_len); ++i)
                                {
                                    stbi__tga_read_rgb16(s, pal_entry);
                                    pal_entry += tga_comp;
                                }
                            }
                            else if (stbi__getn(s, tga_palette, (int)(tga_palette_len * tga_comp)) == 0)
                            {
                                CRuntime.free(tga_data);
                                CRuntime.free(tga_palette);
                                return ((byte*)((ulong)((stbi__err("bad palette")) != 0 ? ((byte*)null) : (null))));
                            }
                        }
                        for (i = (int)(0); (i) < (tga_width * tga_height); ++i)
                        {
                            if ((tga_is_RLE) != 0)
                            {
                                if ((RLE_count) == (0))
                                {
                                    int RLE_cmd = (int)(stbi__get8(s));
                                    RLE_count = (int)(1 + (RLE_cmd & 127));
                                    RLE_repeating = (int)(RLE_cmd >> 7);
                                    read_next_pixel = (int)(1);
                                }
                                else if (RLE_repeating == 0)
                                {
                                    read_next_pixel = (int)(1);
                                }
                            }
                            else
                            {
                                read_next_pixel = (int)(1);
                            }
                            if ((read_next_pixel) != 0)
                            {
                                if ((tga_indexed) != 0)
                                {
                                    int pal_idx = (int)(((tga_bits_per_pixel) == (8)) ? stbi__get8(s) : stbi__get16le(s));
                                    if ((pal_idx) >= (tga_palette_len))
                                    {
                                        pal_idx = (int)(0);
                                    }
                                    pal_idx *= (int)(tga_comp);
                                    for (j = (int)(0); (j) < (tga_comp); ++j)
                                    {
                                        raw_data[j] = (byte)(tga_palette[pal_idx + j]);
                                    }
                                }
                                else if ((tga_rgb16) != 0)
                                {
                                    stbi__tga_read_rgb16(s, raw_data);
                                }
                                else
                                {
                                    for (j = (int)(0); (j) < (tga_comp); ++j)
                                    {
                                        raw_data[j] = (byte)(stbi__get8(s));
                                    }
                                }
                                read_next_pixel = (int)(0);
                            }
                            for (j = (int)(0); (j) < (tga_comp); ++j)
                            {
                                tga_data[i * tga_comp + j] = (byte)(raw_data[j]);
                            }
                            --RLE_count;
                        }
                        if ((tga_inverted) != 0)
                        {
                            for (j = (int)(0); (j * 2) < (tga_height); ++j)
                            {
                                int index1 = (int)(j * tga_width * tga_comp);
                                int index2 = (int)((tga_height - 1 - j) * tga_width * tga_comp);
                                for (i = (int)(tga_width * tga_comp); (i) > (0); --i)
                                {
                                    byte temp = (byte)(tga_data[index1]);
                                    tga_data[index1] = (byte)(tga_data[index2]);
                                    tga_data[index2] = (byte)(temp);
                                    ++index1;
                                    ++index2;
                                }
                            }
                        }
                        if (tga_palette != (null))
                        {
                            CRuntime.free(tga_palette);
                        }
                    }

                    if (((tga_comp) >= (3)) && (tga_rgb16 == 0))
                    {
                        byte* tga_pixel = tga_data;
                        for (i = (int)(0); (i) < (tga_width * tga_height); ++i)
                        {
                            byte temp = (byte)(tga_pixel[0]);
                            tga_pixel[0] = (byte)(tga_pixel[2]);
                            tga_pixel[2] = (byte)(temp);
                            tga_pixel += tga_comp;
                        }
                    }

                    if (((req_comp) != 0) && (req_comp != tga_comp))
                        tga_data = stbi__convert_format(tga_data, (int)(tga_comp), (int)(req_comp), (uint)(tga_width),
                            (uint)(tga_height));
                    tga_palette_start =
                        (int)(tga_palette_len = (int)(tga_palette_bits = (int)(tga_x_origin = (int)(tga_y_origin = (int)(0)))));
                    return tga_data;
                }

                public static int stbi__psd_test(stbi__context s)
                {
                    int r = (((stbi__get32be(s)) == (0x38425053))) ? 1 : 0;
                    stbi__rewind(s);
                    return (int)(r);
                }

                public static int stbi__psd_decode_rle(stbi__context s, byte* p, int pixelCount)
                {
                    int count;
                    int nleft;
                    int len;
                    count = (int)(0);
                    while ((nleft = (int)(pixelCount - count)) > (0))
                    {
                        len = (int)(stbi__get8(s));
                        if ((len) == (128))
                        {
                        }
                        else if ((len) < (128))
                        {
                            len++;
                            if ((len) > (nleft)) return (int)(0);
                            count += (int)(len);
                            while ((len) != 0)
                            {
                                *p = (byte)(stbi__get8(s));
                                p += 4;
                                len--;
                            }
                        }
                        else if ((len) > (128))
                        {
                            byte val;
                            len = (int)(257 - len);
                            if ((len) > (nleft)) return (int)(0);
                            val = (byte)(stbi__get8(s));
                            count += (int)(len);
                            while ((len) != 0)
                            {
                                *p = (byte)(val);
                                p += 4;
                                len--;
                            }
                        }
                    }
                    return (int)(1);
                }

                public static void* stbi__psd_load(stbi__context s, int* x, int* y, int* comp, int req_comp, stbi__result_info* ri,
                    int bpc)
                {
                    int pixelCount;
                    int channelCount;
                    int compression;
                    int channel;
                    int i;
                    int bitdepth;
                    int w;
                    int h;
                    byte* _out_;
                    if (stbi__get32be(s) != 0x38425053)
                        return ((byte*)((ulong)((stbi__err("not PSD")) != 0 ? ((byte*)null) : (null))));
                    if (stbi__get16be(s) != 1) return ((byte*)((ulong)((stbi__err("wrong version")) != 0 ? ((byte*)null) : (null))));
                    stbi__skip(s, (int)(6));
                    channelCount = (int)(stbi__get16be(s));
                    if (((channelCount) < (0)) || ((channelCount) > (16)))
                        return ((byte*)((ulong)((stbi__err("wrong channel count")) != 0 ? ((byte*)null) : (null))));
                    h = (int)(stbi__get32be(s));
                    w = (int)(stbi__get32be(s));
                    bitdepth = (int)(stbi__get16be(s));
                    if ((bitdepth != 8) && (bitdepth != 16))
                        return ((byte*)((ulong)((stbi__err("unsupported bit depth")) != 0 ? ((byte*)null) : (null))));
                    if (stbi__get16be(s) != 3)
                        return ((byte*)((ulong)((stbi__err("wrong colour format")) != 0 ? ((byte*)null) : (null))));
                    stbi__skip(s, (int)(stbi__get32be(s)));
                    stbi__skip(s, (int)(stbi__get32be(s)));
                    stbi__skip(s, (int)(stbi__get32be(s)));
                    compression = (int)(stbi__get16be(s));
                    if ((compression) > (1)) return ((byte*)((ulong)((stbi__err("bad compression")) != 0 ? ((byte*)null) : (null))));
                    if (stbi__mad3sizes_valid((int)(4), (int)(w), (int)(h), (int)(0)) == 0)
                        return ((byte*)((ulong)((stbi__err("too large")) != 0 ? ((byte*)null) : (null))));
                    if (((compression == 0) && ((bitdepth) == (16))) && ((bpc) == (16)))
                    {
                        _out_ = (byte*)(stbi__malloc_mad3((int)(8), (int)(w), (int)(h), (int)(0)));
                        ri->bits_per_channel = (int)(16);
                    }
                    else _out_ = (byte*)(stbi__malloc((ulong)(4 * w * h)));
                    if (_out_ == null) return ((byte*)((ulong)((stbi__err("outofmem")) != 0 ? ((byte*)null) : (null))));
                    pixelCount = (int)(w * h);
                    if ((compression) != 0)
                    {
                        stbi__skip(s, (int)(h * channelCount * 2));
                        for (channel = (int)(0); (channel) < (4); channel++)
                        {
                            byte* p;
                            p = _out_ + channel;
                            if ((channel) >= (channelCount))
                            {
                                for (i = (int)(0); (i) < (pixelCount); i++, p += 4)
                                {
                                    *p = (byte)((channel) == (3) ? 255 : 0);
                                }
                            }
                            else
                            {
                                if (stbi__psd_decode_rle(s, p, (int)(pixelCount)) == 0)
                                {
                                    CRuntime.free(_out_);
                                    return ((byte*)((ulong)((stbi__err("corrupt")) != 0 ? ((byte*)null) : (null))));
                                }
                            }
                        }
                    }
                    else
                    {
                        for (channel = (int)(0); (channel) < (4); channel++)
                        {
                            if ((channel) >= (channelCount))
                            {
                                if (((bitdepth) == (16)) && ((bpc) == (16)))
                                {
                                    ushort* q = ((ushort*)(_out_)) + channel;
                                    ushort val = (ushort)((channel) == (3) ? 65535 : 0);
                                    for (i = (int)(0); (i) < (pixelCount); i++, q += 4)
                                    {
                                        *q = (ushort)(val);
                                    }
                                }
                                else
                                {
                                    byte* p = _out_ + channel;
                                    byte val = (byte)((channel) == (3) ? 255 : 0);
                                    for (i = (int)(0); (i) < (pixelCount); i++, p += 4)
                                    {
                                        *p = (byte)(val);
                                    }
                                }
                            }
                            else
                            {
                                if ((ri->bits_per_channel) == (16))
                                {
                                    ushort* q = ((ushort*)(_out_)) + channel;
                                    for (i = (int)(0); (i) < (pixelCount); i++, q += 4)
                                    {
                                        *q = ((ushort)(stbi__get16be(s)));
                                    }
                                }
                                else
                                {
                                    byte* p = _out_ + channel;
                                    if ((bitdepth) == (16))
                                    {
                                        for (i = (int)(0); (i) < (pixelCount); i++, p += 4)
                                        {
                                            *p = ((byte)(stbi__get16be(s) >> 8));
                                        }
                                    }
                                    else
                                    {
                                        for (i = (int)(0); (i) < (pixelCount); i++, p += 4)
                                        {
                                            *p = (byte)(stbi__get8(s));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if ((channelCount) >= (4))
                    {
                        if ((ri->bits_per_channel) == (16))
                        {
                            for (i = (int)(0); (i) < (w * h); ++i)
                            {
                                ushort* pixel = (ushort*)(_out_) + 4 * i;
                                if ((pixel[3] != 0) && (pixel[3] != 65535))
                                {
                                    float a = (float)(pixel[3] / 65535.0f);
                                    float ra = (float)(1.0f / a);
                                    float inv_a = (float)(65535.0f * (1 - ra));
                                    pixel[0] = ((ushort)(pixel[0] * ra + inv_a));
                                    pixel[1] = ((ushort)(pixel[1] * ra + inv_a));
                                    pixel[2] = ((ushort)(pixel[2] * ra + inv_a));
                                }
                            }
                        }
                        else
                        {
                            for (i = (int)(0); (i) < (w * h); ++i)
                            {
                                byte* pixel = _out_ + 4 * i;
                                if ((pixel[3] != 0) && (pixel[3] != 255))
                                {
                                    float a = (float)(pixel[3] / 255.0f);
                                    float ra = (float)(1.0f / a);
                                    float inv_a = (float)(255.0f * (1 - ra));
                                    pixel[0] = ((byte)(pixel[0] * ra + inv_a));
                                    pixel[1] = ((byte)(pixel[1] * ra + inv_a));
                                    pixel[2] = ((byte)(pixel[2] * ra + inv_a));
                                }
                            }
                        }
                    }

                    if (((req_comp) != 0) && (req_comp != 4))
                    {
                        if ((ri->bits_per_channel) == (16))
                            _out_ = (byte*)(stbi__convert_format16((ushort*)(_out_), (int)(4), (int)(req_comp), (uint)(w), (uint)(h)));
                        else _out_ = stbi__convert_format(_out_, (int)(4), (int)(req_comp), (uint)(w), (uint)(h));
                        if ((_out_) == (null)) return _out_;
                    }

                    if ((comp) != null) *comp = (int)(4);
                    *y = (int)(h);
                    *x = (int)(w);
                    return _out_;
                }

                public static int stbi__gif_test_raw(stbi__context s)
                {
                    int sz;
                    if ((((stbi__get8(s) != 'G') || (stbi__get8(s) != 'I')) || (stbi__get8(s) != 'F')) || (stbi__get8(s) != '8'))
                        return (int)(0);
                    sz = (int)(stbi__get8(s));
                    if ((sz != '9') && (sz != '7')) return (int)(0);
                    if (stbi__get8(s) != 'a') return (int)(0);
                    return (int)(1);
                }

                public static int stbi__gif_test(stbi__context s)
                {
                    int r = (int)(stbi__gif_test_raw(s));
                    stbi__rewind(s);
                    return (int)(r);
                }

                public static int stbi__gif_header(stbi__context s, stbi__gif g, int* comp, int is_info)
                {
                    byte version;
                    if ((((stbi__get8(s) != 'G') || (stbi__get8(s) != 'I')) || (stbi__get8(s) != 'F')) || (stbi__get8(s) != '8'))
                        return (int)(stbi__err("not GIF"));
                    version = (byte)(stbi__get8(s));
                    if ((version != '7') && (version != '9')) return (int)(stbi__err("not GIF"));
                    if (stbi__get8(s) != 'a') return (int)(stbi__err("not GIF"));
                    stbi__g_failure_reason = "";
                    g.w = (int)(stbi__get16le(s));
                    g.h = (int)(stbi__get16le(s));
                    g.flags = (int)(stbi__get8(s));
                    g.bgindex = (int)(stbi__get8(s));
                    g.ratio = (int)(stbi__get8(s));
                    g.transparent = (int)(-1);
                    if (comp != null) *comp = (int)(4);
                    if ((is_info) != 0) return (int)(1);
                    if ((g.flags & 0x80) != 0) stbi__gif_parse_colourtable(s, g.pal, (int)(2 << (g.flags & 7)), (int)(-1));
                    return (int)(1);
                }

                public static int stbi__gif_info_raw(stbi__context s, int* x, int* y, int* comp)
                {
                    stbi__gif g = new stbi__gif();
                    if (stbi__gif_header(s, g, comp, (int)(1)) == 0)
                    {
                        stbi__rewind(s);
                        return (int)(0);
                    }

                    if ((x) != null) *x = (int)(g.w);
                    if ((y) != null) *y = (int)(g.h);

                    return (int)(1);
                }

                public static void stbi__out_gif_code(stbi__gif g, ushort code)
                {
                    byte* p;
                    byte* c;
                    if ((g.codes[code].prefix) >= (0)) stbi__out_gif_code(g, (ushort)(g.codes[code].prefix));
                    if ((g.cur_y) >= (g.max_y)) return;
                    p = &g._out_[g.cur_x + g.cur_y];
                    c = &g.colour_table[g.codes[code].suffix * 4];
                    if ((c[3]) >= (128))
                    {
                        p[0] = (byte)(c[2]);
                        p[1] = (byte)(c[1]);
                        p[2] = (byte)(c[0]);
                        p[3] = (byte)(c[3]);
                    }

                    g.cur_x += (int)(4);
                    if ((g.cur_x) >= (g.max_x))
                    {
                        g.cur_x = (int)(g.start_x);
                        g.cur_y += (int)(g.step);
                        while (((g.cur_y) >= (g.max_y)) && ((g.parse) > (0)))
                        {
                            g.step = (int)((1 << g.parse) * g.line_size);
                            g.cur_y = (int)(g.start_y + (g.step >> 1));
                            --g.parse;
                        }
                    }

                }

                public static byte* stbi__process_gif_raster(stbi__context s, stbi__gif g)
                {
                    byte lzw_cs;
                    int len;
                    int init_code;
                    uint first;
                    int codesize;
                    int codemask;
                    int avail;
                    int oldcode;
                    int bits;
                    int valid_bits;
                    int clear;
                    stbi__gif_lzw* p;
                    lzw_cs = (byte)(stbi__get8(s));
                    if ((lzw_cs) > (12)) return (null);
                    clear = (int)(1 << lzw_cs);
                    first = (uint)(1);
                    codesize = (int)(lzw_cs + 1);
                    codemask = (int)((1 << codesize) - 1);
                    bits = (int)(0);
                    valid_bits = (int)(0);
                    for (init_code = (int)(0); (init_code) < (clear); init_code++)
                    {
                        ((stbi__gif_lzw*)(g.codes))[init_code].prefix = (short)(-1);
                        ((stbi__gif_lzw*)(g.codes))[init_code].first = ((byte)(init_code));
                        ((stbi__gif_lzw*)(g.codes))[init_code].suffix = ((byte)(init_code));
                    }
                    avail = (int)(clear + 2);
                    oldcode = (int)(-1);
                    len = (int)(0);
                    for (; ; )
                    {
                        if ((valid_bits) < (codesize))
                        {
                            if ((len) == (0))
                            {
                                len = (int)(stbi__get8(s));
                                if ((len) == (0)) return g._out_;
                            }
                            --len;
                            bits |= (int)((int)(stbi__get8(s)) << valid_bits);
                            valid_bits += (int)(8);
                        }
                        else
                        {
                            int code = (int)(bits & codemask);
                            bits >>= codesize;
                            valid_bits -= (int)(codesize);
                            if ((code) == (clear))
                            {
                                codesize = (int)(lzw_cs + 1);
                                codemask = (int)((1 << codesize) - 1);
                                avail = (int)(clear + 2);
                                oldcode = (int)(-1);
                                first = (uint)(0);
                            }
                            else if ((code) == (clear + 1))
                            {
                                stbi__skip(s, (int)(len));
                                while ((len = (int)(stbi__get8(s))) > (0))
                                {
                                    stbi__skip(s, (int)(len));
                                }
                                return g._out_;
                            }
                            else if (code <= avail)
                            {
                                if ((first) != 0) return ((byte*)((ulong)((stbi__err("no clear code")) != 0 ? ((byte*)null) : (null))));
                                if ((oldcode) >= (0))
                                {
                                    p = (stbi__gif_lzw*)g.codes + avail++;
                                    if ((avail) > (4096)) return ((byte*)((ulong)((stbi__err("too many codes")) != 0 ? ((byte*)null) : (null))));
                                    p->prefix = ((short)(oldcode));
                                    p->first = (byte)(g.codes[oldcode].first);
                                    p->suffix = (byte)(((code) == (avail)) ? p->first : g.codes[code].first);
                                }
                                else if ((code) == (avail))
                                    return ((byte*)((ulong)((stbi__err("illegal code  raster")) != 0 ? ((byte*)null) : (null))));
                                stbi__out_gif_code(g, (ushort)(code));
                                if (((avail & codemask) == (0)) && (avail <= 0x0FFF))
                                {
                                    codesize++;
                                    codemask = (int)((1 << codesize) - 1);
                                }
                                oldcode = (int)(code);
                            }
                            else
                            {
                                return ((byte*)((ulong)((stbi__err("illegal code  raster")) != 0 ? ((byte*)null) : (null))));
                            }
                        }
                    }
                }

                public static void stbi__fill_gif_background(stbi__gif g, int x0, int y0, int x1, int y1)
                {
                    int x;
                    int y;
                    byte* c = (byte*)g.pal + g.bgindex;
                    for (y = (int)(y0); (y) < (y1); y += (int)(4 * g.w))
                    {
                        for (x = (int)(x0); (x) < (x1); x += (int)(4))
                        {
                            byte* p = &g._out_[y + x];
                            p[0] = (byte)(c[2]);
                            p[1] = (byte)(c[1]);
                            p[2] = (byte)(c[0]);
                            p[3] = (byte)(0);
                        }
                    }
                }

                public static byte* stbi__gif_load_next(stbi__context s, stbi__gif g, int* comp, int req_comp)
                {
                    int i;
                    byte* prev_out = null;
                    if (((g._out_) == null) && (stbi__gif_header(s, g, comp, (int)(0)) == 0)) return null;
                    if (stbi__mad3sizes_valid((int)(g.w), (int)(g.h), (int)(4), (int)(0)) == 0)
                        return ((byte*)((ulong)((stbi__err("too large")) != 0 ? ((byte*)null) : (null))));
                    prev_out = g._out_;
                    g._out_ = (byte*)(stbi__malloc_mad3((int)(4), (int)(g.w), (int)(g.h), (int)(0)));
                    if ((g._out_) == null) return ((byte*)((ulong)((stbi__err("outofmem")) != 0 ? ((byte*)null) : (null))));
                    switch ((g.eflags & 0x1C) >> 2)
                    {
                        case 0:
                            stbi__fill_gif_background(g, (int)(0), (int)(0), (int)(4 * g.w), (int)(4 * g.w * g.h));
                            break;
                        case 1:
                            if ((prev_out) != null) CRuntime.memcpy(g._out_, prev_out, (ulong)(4 * g.w * g.h));
                            g.old_out = prev_out;
                            break;
                        case 2:
                            if ((prev_out) != null) CRuntime.memcpy(g._out_, prev_out, (ulong)(4 * g.w * g.h));
                            stbi__fill_gif_background(g, (int)(g.start_x), (int)(g.start_y), (int)(g.max_x), (int)(g.max_y));
                            break;
                        case 3:
                            if ((g.old_out) != null)
                            {
                                for (i = (int)(g.start_y); (i) < (g.max_y); i += (int)(4 * g.w))
                                {
                                    CRuntime.memcpy(&g._out_[i + g.start_x], &g.old_out[i + g.start_x], (ulong)(g.max_x - g.start_x));
                                }
                            }
                            break;
                    }

                    for (; ; )
                    {
                        switch (stbi__get8(s))
                        {
                            case 0x2C:
                                {
                                    int prev_trans = (int)(-1);
                                    int x;
                                    int y;
                                    int w;
                                    int h;
                                    byte* o;
                                    x = (int)(stbi__get16le(s));
                                    y = (int)(stbi__get16le(s));
                                    w = (int)(stbi__get16le(s));
                                    h = (int)(stbi__get16le(s));
                                    if (((x + w) > (g.w)) || ((y + h) > (g.h)))
                                        return ((byte*)((ulong)((stbi__err("bad Image Descriptor")) != 0 ? ((byte*)null) : (null))));
                                    g.line_size = (int)(g.w * 4);
                                    g.start_x = (int)(x * 4);
                                    g.start_y = (int)(y * g.line_size);
                                    g.max_x = (int)(g.start_x + w * 4);
                                    g.max_y = (int)(g.start_y + h * g.line_size);
                                    g.cur_x = (int)(g.start_x);
                                    g.cur_y = (int)(g.start_y);
                                    g.lflags = (int)(stbi__get8(s));
                                    if ((g.lflags & 0x40) != 0)
                                    {
                                        g.step = (int)(8 * g.line_size);
                                        g.parse = (int)(3);
                                    }
                                    else
                                    {
                                        g.step = (int)(g.line_size);
                                        g.parse = (int)(0);
                                    }
                                    if ((g.lflags & 0x80) != 0)
                                    {
                                        stbi__gif_parse_colourtable(s, g.lpal, (int)(2 << (g.lflags & 7)),
                                            (int)((g.eflags & 0x01) != 0 ? g.transparent : -1));
                                        g.colour_table = (byte*)(g.lpal);
                                    }
                                    else if ((g.flags & 0x80) != 0)
                                    {
                                        if (((g.transparent) >= (0)) && (g.eflags & 0x01) != 0)
                                        {
                                            prev_trans = (int)(g.pal[g.transparent * 4 + 3]);
                                            g.pal[g.transparent * 4 + 3] = (byte)(0);
                                        }
                                        g.colour_table = (byte*)(g.pal);
                                    }
                                    else return ((byte*)((ulong)((stbi__err("missing colour table")) != 0 ? ((byte*)null) : (null))));
                                    o = stbi__process_gif_raster(s, g);
                                    if ((o) == (null)) return (null);
                                    if (prev_trans != -1) g.pal[g.transparent * 4 + 3] = ((byte)(prev_trans));
                                    return o;
                                }
                            case 0x21:
                                {
                                    int len;
                                    if ((stbi__get8(s)) == (0xF9))
                                    {
                                        len = (int)(stbi__get8(s));
                                        if ((len) == (4))
                                        {
                                            g.eflags = (int)(stbi__get8(s));
                                            g.delay = (int)(stbi__get16le(s));
                                            g.transparent = (int)(stbi__get8(s));
                                        }
                                        else
                                        {
                                            stbi__skip(s, (int)(len));
                                            break;
                                        }
                                    }
                                    while ((len = (int)(stbi__get8(s))) != 0)
                                    {
                                        stbi__skip(s, (int)(len));
                                    }
                                    break;
                                }
                            case 0x3B:
                                return null;
                            default:
                                return ((byte*)((ulong)((stbi__err("unknown code")) != 0 ? ((byte*)null) : (null))));
                        }
                    }
                }

                public static void* stbi__gif_load(stbi__context s, int* x, int* y, int* comp, int req_comp, stbi__result_info* ri)
                {
                    byte* u = null;
                    stbi__gif g = new stbi__gif();

                    u = stbi__gif_load_next(s, g, comp, (int)(req_comp));
                    if ((u) != null)
                    {
                        *x = (int)(g.w);
                        *y = (int)(g.h);
                        if (((req_comp) != 0) && (req_comp != 4))
                            u = stbi__convert_format(u, (int)(4), (int)(req_comp), (uint)(g.w), (uint)(g.h));
                    }
                    else if ((g._out_) != null) CRuntime.free(g._out_);

                    return u;
                }

                public static int stbi__gif_info(stbi__context s, int* x, int* y, int* comp)
                {
                    return (int)(stbi__gif_info_raw(s, x, y, comp));
                }

                public static int stbi__bmp_info(stbi__context s, int* x, int* y, int* comp)
                {
                    void* p;
                    stbi__bmp_data info = new stbi__bmp_data();
                    info.all_a = (uint)(255);
                    p = stbi__bmp_parse_header(s, &info);
                    stbi__rewind(s);
                    if ((p) == (null)) return (int)(0);
                    if ((x) != null) *x = (int)(s.img_x);
                    if ((y) != null) *y = (int)(s.img_y);
                    if ((comp) != null) *comp = (int)((info.ma) != 0 ? 4 : 3);
                    return (int)(1);
                }

                public static int stbi__psd_info(stbi__context s, int* x, int* y, int* comp)
                {
                    int channelCount;
                    int dummy;
                    if (x == null) x = &dummy;
                    if (y == null) y = &dummy;
                    if (comp == null) comp = &dummy;
                    if (stbi__get32be(s) != 0x38425053)
                    {
                        stbi__rewind(s);
                        return (int)(0);
                    }

                    if (stbi__get16be(s) != 1)
                    {
                        stbi__rewind(s);
                        return (int)(0);
                    }

                    stbi__skip(s, (int)(6));
                    channelCount = (int)(stbi__get16be(s));
                    if (((channelCount) < (0)) || ((channelCount) > (16)))
                    {
                        stbi__rewind(s);
                        return (int)(0);
                    }

                    *y = (int)(stbi__get32be(s));
                    *x = (int)(stbi__get32be(s));
                    if (stbi__get16be(s) != 8)
                    {
                        stbi__rewind(s);
                        return (int)(0);
                    }

                    if (stbi__get16be(s) != 3)
                    {
                        stbi__rewind(s);
                        return (int)(0);
                    }

                    *comp = (int)(4);
                    return (int)(1);
                }

                public static int stbi__info_main(stbi__context s, int* x, int* y, int* comp)
                {
                    if ((stbi__jpeg_info(s, x, y, comp)) != 0) return (int)(1);
                    if ((stbi__png_info(s, x, y, comp)) != 0) return (int)(1);
                    if ((stbi__gif_info(s, x, y, comp)) != 0) return (int)(1);
                    if ((stbi__bmp_info(s, x, y, comp)) != 0) return (int)(1);
                    if ((stbi__psd_info(s, x, y, comp)) != 0) return (int)(1);
                    if ((stbi__tga_info(s, x, y, comp)) != 0) return (int)(1);
                    return (int)(stbi__err("unknown image type"));
                }

                public static int stbi_info_from_memory(byte* buffer, int len, int* x, int* y, int* comp)
                {
                    stbi__context s = new stbi__context();
                    stbi__start_mem(s, buffer, (int)(len));
                    return (int)(stbi__info_main(s, x, y, comp));
                }

                public static int stbi_info_from_callbacks(stbi_io_callbacks c, void* user, int* x, int* y, int* comp)
                {
                    stbi__context s = new stbi__context();
                    stbi__start_callbacks(s, c, user);
                    return (int)(stbi__info_main(s, x, y, comp));
                }
            }
            #endregion


            public static unsafe partial class StbImageResize
            {
                public delegate float stbir__kernel_fn(float x, float scale);

                public delegate float stbir__support_fn(float scale);

                public class stbir__filter_info
                {
                    public stbir__kernel_fn kernel;
                    public stbir__support_fn support;

                    public stbir__filter_info(stbir__kernel_fn k, stbir__support_fn s)
                    {
                        kernel = k;
                        support = s;
                    }
                }

                public class stbir__info
                {
                    public void* input_data;
                    public int input_w;
                    public int input_h;
                    public int input_stride_bytes;
                    public void* output_data;
                    public int output_w;
                    public int output_h;
                    public int output_stride_bytes;
                    public float s0;
                    public float t0;
                    public float s1;
                    public float t1;
                    public float horizontal_shift;
                    public float vertical_shift;
                    public float horizontal_scale;
                    public float vertical_scale;
                    public int channels;
                    public int alpha_channel;
                    public uint flags;
                    public int type;
                    public int horizontal_filter;
                    public int vertical_filter;
                    public int edge_horizontal;
                    public int edge_vertical;
                    public int colourspace;
                    public stbir__contributors* horizontal_contributors;
                    public float* horizontal_coefficients;
                    public stbir__contributors* vertical_contributors;
                    public float* vertical_coefficients;
                    public int decode_buffer_pixels;
                    public float* decode_buffer;
                    public float* horizontal_buffer;
                    public int horizontal_coefficient_width;
                    public int vertical_coefficient_width;
                    public int horizontal_filter_pixel_width;
                    public int vertical_filter_pixel_width;
                    public int horizontal_filter_pixel_margin;
                    public int vertical_filter_pixel_margin;
                    public int horizontal_num_contributors;
                    public int vertical_num_contributors;
                    public int ring_buffer_length_bytes;
                    public int ring_buffer_num_entries;
                    public int ring_buffer_first_scanline;
                    public int ring_buffer_last_scanline;
                    public int ring_buffer_begin_index;
                    public float* ring_buffer;
                    public float* encode_buffer;
                    public int horizontal_contributors_size;
                    public int horizontal_coefficients_size;
                    public int vertical_contributors_size;
                    public int vertical_coefficients_size;
                    public int decode_buffer_size;
                    public int horizontal_buffer_size;
                    public int ring_buffer_size;
                    public int encode_buffer_size;
                }

                [StructLayout(LayoutKind.Explicit)]
                public struct stbir__FP32
                {
                    [FieldOffset(0)] public uint u;
                    [FieldOffset(0)] public float f;
                }

                public static stbir__filter_info[] stbir__filter_info_table =
                {
                    new stbir__filter_info(null, stbir__support_zero),
                    new stbir__filter_info(stbir__filter_trapezoid, stbir__support_trapezoid),
                    new stbir__filter_info(stbir__filter_triangle, stbir__support_one),
                    new stbir__filter_info(stbir__filter_cubic, stbir__support_two),
                    new stbir__filter_info(stbir__filter_catmullrom, stbir__support_two),
                    new stbir__filter_info(stbir__filter_mitchell, stbir__support_two),
                };

                public static byte stbir__linear_to_srgb_uchar(float _in_)
                {
                    var almostone = new stbir__FP32 { u = 0x3f7fffff };
                    var minval = new stbir__FP32 { u = (127 - 13) << 23 };
                    uint tab;
                    uint bias;
                    uint scale;
                    uint t;
                    var f = new stbir__FP32();
                    if (!(_in_ > minval.f)) _in_ = minval.f;
                    if (_in_ > almostone.f) _in_ = almostone.f;
                    f.f = _in_;
                    tab = fp32_to_srgb8_tab4[(f.u - minval.u) >> 20];
                    bias = (tab >> 16) << 9;
                    scale = tab & 0xffff;
                    t = (f.u >> 12) & 0xff;
                    return (byte)((bias + scale * t) >> 16);
                }


                //
                [StructLayout(LayoutKind.Sequential)]
                public struct stbir__contributors
                {
                    public int n0;
                    public int n1;
                }

                public const int STBIR_EDGE_CLAMP = 1;
                public const int STBIR_EDGE_REFLECT = 2;
                public const int STBIR_EDGE_WRAP = 3;
                public const int STBIR_EDGE_ZERO = 4;
                public const int STBIR_FILTER_DEFAULT = 0;
                public const int STBIR_FILTER_BOX = 1;
                public const int STBIR_FILTER_TRIANGLE = 2;
                public const int STBIR_FILTER_CUBICBSPLINE = 3;
                public const int STBIR_FILTER_CATMULLROM = 4;
                public const int STBIR_FILTER_MITCHELL = 5;
                public const int STBIR_COLORSPACE_LINEAR = 0;
                public const int STBIR_COLORSPACE_SRGB = 1;
                public const int STBIR_MAX_COLORSPACES = 2;
                public const int STBIR_TYPE_UINT8 = 0;
                public const int STBIR_TYPE_UINT16 = 1;
                public const int STBIR_TYPE_UINT32 = 2;
                public const int STBIR_TYPE_FLOAT = 3;
                public const int STBIR_MAX_TYPES = 4;
                public static byte[] stbir__type_size = { 1, 2, 4, 4 };

                public static float[] stbir__srgb_uchar_to_linear_float =
                {
            0.000000f, 0.000304f, 0.000607f, 0.000911f, 0.001214f,
            0.001518f, 0.001821f, 0.002125f, 0.002428f, 0.002732f, 0.003035f, 0.003347f, 0.003677f, 0.004025f, 0.004391f,
            0.004777f, 0.005182f, 0.005605f, 0.006049f, 0.006512f, 0.006995f, 0.007499f, 0.008023f, 0.008568f, 0.009134f,
            0.009721f, 0.010330f, 0.010960f, 0.011612f, 0.012286f, 0.012983f, 0.013702f, 0.014444f, 0.015209f, 0.015996f,
            0.016807f, 0.017642f, 0.018500f, 0.019382f, 0.020289f, 0.021219f, 0.022174f, 0.023153f, 0.024158f, 0.025187f,
            0.026241f, 0.027321f, 0.028426f, 0.029557f, 0.030713f, 0.031896f, 0.033105f, 0.034340f, 0.035601f, 0.036889f,
            0.038204f, 0.039546f, 0.040915f, 0.042311f, 0.043735f, 0.045186f, 0.046665f, 0.048172f, 0.049707f, 0.051269f,
            0.052861f, 0.054480f, 0.056128f, 0.057805f, 0.059511f, 0.061246f, 0.063010f, 0.064803f, 0.066626f, 0.068478f,
            0.070360f, 0.072272f, 0.074214f, 0.076185f, 0.078187f, 0.080220f, 0.082283f, 0.084376f, 0.086500f, 0.088656f,
            0.090842f, 0.093059f, 0.095307f, 0.097587f, 0.099899f, 0.102242f, 0.104616f, 0.107023f, 0.109462f, 0.111932f,
            0.114435f, 0.116971f, 0.119538f, 0.122139f, 0.124772f, 0.127438f, 0.130136f, 0.132868f, 0.135633f, 0.138432f,
            0.141263f, 0.144128f, 0.147027f, 0.149960f, 0.152926f, 0.155926f, 0.158961f, 0.162029f, 0.165132f, 0.168269f,
            0.171441f, 0.174647f, 0.177888f, 0.181164f, 0.184475f, 0.187821f, 0.191202f, 0.194618f, 0.198069f, 0.201556f,
            0.205079f, 0.208637f, 0.212231f, 0.215861f, 0.219526f, 0.223228f, 0.226966f, 0.230740f, 0.234551f, 0.238398f,
            0.242281f, 0.246201f, 0.250158f, 0.254152f, 0.258183f, 0.262251f, 0.266356f, 0.270498f, 0.274677f, 0.278894f,
            0.283149f, 0.287441f, 0.291771f, 0.296138f, 0.300544f, 0.304987f, 0.309469f, 0.313989f, 0.318547f, 0.323143f,
            0.327778f, 0.332452f, 0.337164f, 0.341914f, 0.346704f, 0.351533f, 0.356400f, 0.361307f, 0.366253f, 0.371238f,
            0.376262f, 0.381326f, 0.386430f, 0.391573f, 0.396755f, 0.401978f, 0.407240f, 0.412543f, 0.417885f, 0.423268f,
            0.428691f, 0.434154f, 0.439657f, 0.445201f, 0.450786f, 0.456411f, 0.462077f, 0.467784f, 0.473532f, 0.479320f,
            0.485150f, 0.491021f, 0.496933f, 0.502887f, 0.508881f, 0.514918f, 0.520996f, 0.527115f, 0.533276f, 0.539480f,
            0.545725f, 0.552011f, 0.558340f, 0.564712f, 0.571125f, 0.577581f, 0.584078f, 0.590619f, 0.597202f, 0.603827f,
            0.610496f, 0.617207f, 0.623960f, 0.630757f, 0.637597f, 0.644480f, 0.651406f, 0.658375f, 0.665387f, 0.672443f,
            0.679543f, 0.686685f, 0.693872f, 0.701102f, 0.708376f, 0.715694f, 0.723055f, 0.730461f, 0.737911f, 0.745404f,
            0.752942f, 0.760525f, 0.768151f, 0.775822f, 0.783538f, 0.791298f, 0.799103f, 0.806952f, 0.814847f, 0.822786f,
            0.830770f, 0.838799f, 0.846873f, 0.854993f, 0.863157f, 0.871367f, 0.879622f, 0.887923f, 0.896269f, 0.904661f,
            0.913099f, 0.921582f, 0.930111f, 0.938686f, 0.947307f, 0.955974f, 0.964686f, 0.973445f, 0.982251f, 0.991102f, 1.0f
        };

                public static uint[] fp32_to_srgb8_tab4 =
                {
            0x0073000d, 0x007a000d, 0x0080000d, 0x0087000d, 0x008d000d, 0x0094000d,
            0x009a000d, 0x00a1000d, 0x00a7001a, 0x00b4001a, 0x00c1001a, 0x00ce001a, 0x00da001a, 0x00e7001a, 0x00f4001a,
            0x0101001a, 0x010e0033, 0x01280033, 0x01410033, 0x015b0033, 0x01750033, 0x018f0033, 0x01a80033, 0x01c20033,
            0x01dc0067, 0x020f0067, 0x02430067, 0x02760067, 0x02aa0067, 0x02dd0067, 0x03110067, 0x03440067, 0x037800ce,
            0x03df00ce, 0x044600ce, 0x04ad00ce, 0x051400ce, 0x057b00c5, 0x05dd00bc, 0x063b00b5, 0x06970158, 0x07420142,
            0x07e30130, 0x087b0120, 0x090b0112, 0x09940106, 0x0a1700fc, 0x0a9500f2, 0x0b0f01cb, 0x0bf401ae, 0x0ccb0195,
            0x0d950180, 0x0e56016e, 0x0f0d015e, 0x0fbc0150, 0x10630143, 0x11070264, 0x1238023e, 0x1357021d, 0x14660201,
            0x156601e9, 0x165a01d3, 0x174401c0, 0x182401af, 0x18fe0331, 0x1a9602fe, 0x1c1502d2, 0x1d7e02ad, 0x1ed4028d,
            0x201a0270, 0x21520256, 0x227d0240, 0x239f0443, 0x25c003fe, 0x27bf03c4, 0x29a10392, 0x2b6a0367, 0x2d1d0341,
            0x2ebe031f, 0x304d0300, 0x31d105b0, 0x34a80555, 0x37520507, 0x39d504c5, 0x3c37048b, 0x3e7c0458, 0x40a8042a,
            0x42bd0401, 0x44c20798, 0x488e071e, 0x4c1c06b6, 0x4f76065d, 0x52a50610, 0x55ac05cc, 0x5892058f, 0x5b590559,
            0x5e0c0a23, 0x631c0980, 0x67db08f6, 0x6c55087f, 0x70940818, 0x74a007bd, 0x787d076c, 0x7c330723
        };

                public static int stbir__min(int a, int b)
                {
                    return (int)((a) < (b) ? a : b);
                }

                public static int stbir__max(int a, int b)
                {
                    return (int)((a) > (b) ? a : b);
                }

                public static float stbir__saturate(float x)
                {
                    if ((x) < (0)) return (float)(0);
                    if ((x) > (1)) return (float)(1);
                    return (float)(x);
                }

                public static float stbir__srgb_to_linear(float f)
                {
                    if (f <= 0.04045f) return (float)(f / 12.92f);
                    else return (float)(CRuntime.pow((double)((f + 0.055f) / 1.055f), (double)(2.4f)));
                }

                public static float stbir__linear_to_srgb(float f)
                {
                    if (f <= 0.0031308f) return (float)(f * 12.92f);
                    else return (float)(1.055f * (float)(CRuntime.pow((double)(f), (double)(1 / 2.4f))) - 0.055f);
                }

                public static float stbir__filter_trapezoid(float x, float scale)
                {
                    float halfscale = (float)(scale / 2);
                    float t = (float)(0.5f + halfscale);
                    x = ((float)(CRuntime.fabs((double)(x))));
                    if ((x) >= (t)) return (float)(0);
                    else
                    {
                        float r = (float)(0.5f - halfscale);
                        if (x <= r) return (float)(1);
                        else return (float)((t - x) / scale);
                    }

                }

                public static float stbir__support_trapezoid(float scale)
                {
                    return (float)(0.5f + scale / 2);
                }

                public static float stbir__filter_triangle(float x, float s)
                {
                    x = ((float)(CRuntime.fabs((double)(x))));
                    if (x <= 1.0f) return (float)(1 - x);
                    else return (float)(0);
                }

                public static float stbir__filter_cubic(float x, float s)
                {
                    x = ((float)(CRuntime.fabs((double)(x))));
                    if ((x) < (1.0f)) return (float)((4 + x * x * (3 * x - 6)) / 6);
                    else if ((x) < (2.0f)) return (float)((8 + x * (-12 + x * (6 - x))) / 6);
                    return (float)(0.0f);
                }

                public static float stbir__filter_catmullrom(float x, float s)
                {
                    x = ((float)(CRuntime.fabs((double)(x))));
                    if ((x) < (1.0f)) return (float)(1 - x * x * (2.5f - 1.5f * x));
                    else if ((x) < (2.0f)) return (float)(2 - x * (4 + x * (0.5f * x - 2.5f)));
                    return (float)(0.0f);
                }

                public static float stbir__filter_mitchell(float x, float s)
                {
                    x = ((float)(CRuntime.fabs((double)(x))));
                    if ((x) < (1.0f)) return (float)((16 + x * x * (21 * x - 36)) / 18);
                    else if ((x) < (2.0f)) return (float)((32 + x * (-60 + x * (36 - 7 * x))) / 18);
                    return (float)(0.0f);
                }

                public static float stbir__support_zero(float s)
                {
                    return (float)(0);
                }

                public static float stbir__support_one(float s)
                {
                    return (float)(1);
                }

                public static float stbir__support_two(float s)
                {
                    return (float)(2);
                }

                public static int stbir__use_upsampling(float ratio)
                {
                    return (int)((ratio) > (1) ? 1 : 0);
                }

                public static int stbir__use_width_upsampling(stbir__info stbir_info)
                {
                    return (int)(stbir__use_upsampling((float)(stbir_info.horizontal_scale)));
                }

                public static int stbir__use_height_upsampling(stbir__info stbir_info)
                {
                    return (int)(stbir__use_upsampling((float)(stbir_info.vertical_scale)));
                }

                public static int stbir__get_filter_pixel_width(int filter, float scale)
                {
                    if ((stbir__use_upsampling((float)(scale))) != 0)
                        return (int)(CRuntime.ceil((double)(stbir__filter_info_table[filter].support((float)(1 / scale)) * 2)));
                    else return (int)(CRuntime.ceil((double)(stbir__filter_info_table[filter].support((float)(scale)) * 2 / scale)));
                }

                public static int stbir__get_filter_pixel_margin(int filter, float scale)
                {
                    return (int)(stbir__get_filter_pixel_width((int)(filter), (float)(scale)) / 2);
                }

                public static int stbir__get_coefficient_width(int filter, float scale)
                {
                    if ((stbir__use_upsampling((float)(scale))) != 0)
                        return (int)(CRuntime.ceil((double)(stbir__filter_info_table[filter].support((float)(1 / scale)) * 2)));
                    else return (int)(CRuntime.ceil((double)(stbir__filter_info_table[filter].support((float)(scale)) * 2)));
                }

                public static int stbir__get_contributors(float scale, int filter, int input_size, int output_size)
                {
                    if ((stbir__use_upsampling((float)(scale))) != 0) return (int)(output_size);
                    else return (int)(input_size + stbir__get_filter_pixel_margin((int)(filter), (float)(scale)) * 2);
                }

                public static int stbir__get_total_horizontal_coefficients(stbir__info info)
                {
                    return
                        (int)
                            (info.horizontal_num_contributors *
                             stbir__get_coefficient_width((int)(info.horizontal_filter), (float)(info.horizontal_scale)));
                }

                public static int stbir__get_total_vertical_coefficients(stbir__info info)
                {
                    return
                        (int)
                            (info.vertical_num_contributors *
                             stbir__get_coefficient_width((int)(info.vertical_filter), (float)(info.vertical_scale)));
                }

                public static stbir__contributors* stbir__get_contributor(stbir__contributors* contributors, int n)
                {
                    return &contributors[n];
                }

                public static float* stbir__get_coefficient(float* coefficients, int filter, float scale, int n, int c)
                {
                    int width = (int)(stbir__get_coefficient_width((int)(filter), (float)(scale)));
                    return &coefficients[width * n + c];
                }

                public static int stbir__edge_wrap_slow(int edge, int n, int max)
                {
                    switch (edge)
                    {
                        case STBIR_EDGE_ZERO:
                            return (int)(0);
                        case STBIR_EDGE_CLAMP:
                            if ((n) < (0)) return (int)(0);
                            if ((n) >= (max)) return (int)(max - 1);
                            return (int)(n);
                        case STBIR_EDGE_REFLECT:
                            {
                                if ((n) < (0))
                                {
                                    if ((n) < (max)) return (int)(-n);
                                    else return (int)(max - 1);
                                }
                                if ((n) >= (max))
                                {
                                    int max2 = (int)(max * 2);
                                    if ((n) >= (max2)) return (int)(0);
                                    else return (int)(max2 - n - 1);
                                }
                                return (int)(n);
                            }
                        case STBIR_EDGE_WRAP:
                            if ((n) >= (0)) return (int)(n % max);
                            else
                            {
                                int m = (int)((-n) % max);
                                if (m != 0) m = (int)(max - m);
                                return (int)(m);
                            }

                        default:
                            ;
                            return (int)(0);
                    }

                }

                public static int stbir__edge_wrap(int edge, int n, int max)
                {
                    if (((n) >= (0)) && ((n) < (max))) return (int)(n);
                    return (int)(stbir__edge_wrap_slow((int)(edge), (int)(n), (int)(max)));
                }

                public static void stbir__calculate_sample_range_upsample(int n, float out_filter_radius, float scale_ratio,
                    float out_shift, int* in_first_pixel, int* in_last_pixel, float* in_center_of_out)
                {
                    float out_pixel_center = (float)((float)(n) + 0.5f);
                    float out_pixel_influence_lowerbound = (float)(out_pixel_center - out_filter_radius);
                    float out_pixel_influence_upperbound = (float)(out_pixel_center + out_filter_radius);
                    float in_pixel_influence_lowerbound = (float)((out_pixel_influence_lowerbound + out_shift) / scale_ratio);
                    float in_pixel_influence_upperbound = (float)((out_pixel_influence_upperbound + out_shift) / scale_ratio);
                    *in_center_of_out = (float)((out_pixel_center + out_shift) / scale_ratio);
                    *in_first_pixel = ((int)(CRuntime.floor((double)(in_pixel_influence_lowerbound + 0.5))));
                    *in_last_pixel = ((int)(CRuntime.floor((double)(in_pixel_influence_upperbound - 0.5))));
                }

                public static void stbir__calculate_sample_range_downsample(int n, float in_pixels_radius, float scale_ratio,
                    float out_shift, int* out_first_pixel, int* out_last_pixel, float* out_center_of_in)
                {
                    float in_pixel_center = (float)((float)(n) + 0.5f);
                    float in_pixel_influence_lowerbound = (float)(in_pixel_center - in_pixels_radius);
                    float in_pixel_influence_upperbound = (float)(in_pixel_center + in_pixels_radius);
                    float out_pixel_influence_lowerbound = (float)(in_pixel_influence_lowerbound * scale_ratio - out_shift);
                    float out_pixel_influence_upperbound = (float)(in_pixel_influence_upperbound * scale_ratio - out_shift);
                    *out_center_of_in = (float)(in_pixel_center * scale_ratio - out_shift);
                    *out_first_pixel = ((int)(CRuntime.floor((double)(out_pixel_influence_lowerbound + 0.5))));
                    *out_last_pixel = ((int)(CRuntime.floor((double)(out_pixel_influence_upperbound - 0.5))));
                }

                public static void stbir__calculate_coefficients_upsample(stbir__info stbir_info, int filter, float scale,
                    int in_first_pixel, int in_last_pixel, float in_center_of_out, stbir__contributors* contributor,
                    float* coefficient_group)
                {
                    int i;
                    float total_filter = (float)(0);
                    float filter_scale;
                    contributor->n0 = (int)(in_first_pixel);
                    contributor->n1 = (int)(in_last_pixel);
                    for (i = (int)(0); i <= in_last_pixel - in_first_pixel; i++)
                    {
                        float in_pixel_center = (float)((float)(i + in_first_pixel) + 0.5f);
                        coefficient_group[i] =
                            (float)(stbir__filter_info_table[filter].kernel((float)(in_center_of_out - in_pixel_center), (float)(1 / scale)));
                        if (((i) == (0)) && (coefficient_group[i] == 0))
                        {
                            contributor->n0 = (int)(++in_first_pixel);
                            i--;
                            continue;
                        }
                        total_filter += (float)(coefficient_group[i]);
                    }
                    filter_scale = (float)(1 / total_filter);
                    for (i = (int)(0); i <= in_last_pixel - in_first_pixel; i++)
                    {
                        coefficient_group[i] *= (float)(filter_scale);
                    }
                    for (i = (int)(in_last_pixel - in_first_pixel); (i) >= (0); i--)
                    {
                        if ((coefficient_group[i]) != 0) break;
                        contributor->n1 = (int)(contributor->n0 + i - 1);
                    }
                }

                public static void stbir__calculate_coefficients_downsample(stbir__info stbir_info, int filter, float scale_ratio,
                    int out_first_pixel, int out_last_pixel, float out_center_of_in, stbir__contributors* contributor,
                    float* coefficient_group)
                {
                    int i;
                    contributor->n0 = (int)(out_first_pixel);
                    contributor->n1 = (int)(out_last_pixel);
                    for (i = (int)(0); i <= out_last_pixel - out_first_pixel; i++)
                    {
                        float out_pixel_center = (float)((float)(i + out_first_pixel) + 0.5f);
                        float x = (float)(out_pixel_center - out_center_of_in);
                        coefficient_group[i] =
                            (float)(stbir__filter_info_table[filter].kernel((float)(x), (float)(scale_ratio)) * scale_ratio);
                    }
                    for (i = (int)(out_last_pixel - out_first_pixel); (i) >= (0); i--)
                    {
                        if ((coefficient_group[i]) != 0) break;
                        contributor->n1 = (int)(contributor->n0 + i - 1);
                    }
                }

                public static void stbir__normalize_downsample_coefficients(stbir__info stbir_info, stbir__contributors* contributors,
                    float* coefficients, int filter, float scale_ratio, float shift, int input_size, int output_size)
                {
                    int num_contributors =
                        (int)(stbir__get_contributors((float)(scale_ratio), (int)(filter), (int)(input_size), (int)(output_size)));
                    int num_coefficients = (int)(stbir__get_coefficient_width((int)(filter), (float)(scale_ratio)));
                    int i;
                    int j;
                    int skip;
                    for (i = (int)(0); (i) < (output_size); i++)
                    {
                        float scale;
                        float total = (float)(0);
                        for (j = (int)(0); (j) < (num_contributors); j++)
                        {
                            if (((i) >= (contributors[j].n0)) && (i <= contributors[j].n1))
                            {
                                float coefficient =
                                    (float)
                                        (*
                                            stbir__get_coefficient(coefficients, (int)(filter), (float)(scale_ratio), (int)(j),
                                                (int)(i - contributors[j].n0)));
                                total += (float)(coefficient);
                            }
                            else if ((i) < (contributors[j].n0)) break;
                        }
                        scale = (float)(1 / total);
                        for (j = (int)(0); (j) < (num_contributors); j++)
                        {
                            if (((i) >= (contributors[j].n0)) && (i <= contributors[j].n1))
                                *
                                    stbir__get_coefficient(coefficients, (int)(filter), (float)(scale_ratio), (int)(j),
                                        (int)(i - contributors[j].n0)) *= (float)(scale);
                            else if ((i) < (contributors[j].n0)) break;
                        }
                    }
                    for (j = (int)(0); (j) < (num_contributors); j++)
                    {
                        int range;
                        int max;
                        int width;
                        skip = (int)(0);
                        while ((*stbir__get_coefficient(coefficients, (int)(filter), (float)(scale_ratio), (int)(j), (int)(skip))) ==
                               (0))
                        {
                            skip++;
                        }
                        contributors[j].n0 += (int)(skip);
                        while ((contributors[j].n0) < (0))
                        {
                            contributors[j].n0++;
                            skip++;
                        }
                        range = (int)(contributors[j].n1 - contributors[j].n0 + 1);
                        max = (int)(stbir__min((int)(num_coefficients), (int)(range)));
                        width = (int)(stbir__get_coefficient_width((int)(filter), (float)(scale_ratio)));
                        for (i = (int)(0); (i) < (max); i++)
                        {
                            if ((i + skip) >= (width)) break;
                            *stbir__get_coefficient(coefficients, (int)(filter), (float)(scale_ratio), (int)(j), (int)(i)) =
                                (float)
                                    (*stbir__get_coefficient(coefficients, (int)(filter), (float)(scale_ratio), (int)(j), (int)(i + skip)));
                        }
                        continue;
                    }
                    for (i = (int)(0); (i) < (num_contributors); i++)
                    {
                        contributors[i].n1 = (int)(stbir__min((int)(contributors[i].n1), (int)(output_size - 1)));
                    }
                }

                public static void stbir__calculate_filters(stbir__info stbir_info, stbir__contributors* contributors,
                    float* coefficients, int filter, float scale_ratio, float shift, int input_size, int output_size)
                {
                    int n;
                    int total_contributors =
                        (int)(stbir__get_contributors((float)(scale_ratio), (int)(filter), (int)(input_size), (int)(output_size)));
                    if ((stbir__use_upsampling((float)(scale_ratio))) != 0)
                    {
                        float out_pixels_radius = (float)(stbir__filter_info_table[filter].support((float)(1 / scale_ratio)) * scale_ratio);
                        for (n = (int)(0); (n) < (total_contributors); n++)
                        {
                            float in_center_of_out;
                            int in_first_pixel;
                            int in_last_pixel;
                            stbir__calculate_sample_range_upsample((int)(n), (float)(out_pixels_radius), (float)(scale_ratio),
                                (float)(shift), &in_first_pixel, &in_last_pixel, &in_center_of_out);
                            stbir__calculate_coefficients_upsample(stbir_info, (int)(filter), (float)(scale_ratio), (int)(in_first_pixel),
                                (int)(in_last_pixel), (float)(in_center_of_out), stbir__get_contributor(contributors, (int)(n)),
                                stbir__get_coefficient(coefficients, (int)(filter), (float)(scale_ratio), (int)(n), (int)(0)));
                        }
                    }
                    else
                    {
                        float in_pixels_radius = (float)(stbir__filter_info_table[filter].support((float)(scale_ratio)) / scale_ratio);
                        for (n = (int)(0); (n) < (total_contributors); n++)
                        {
                            float out_center_of_in;
                            int out_first_pixel;
                            int out_last_pixel;
                            int n_adjusted = (int)(n - stbir__get_filter_pixel_margin((int)(filter), (float)(scale_ratio)));
                            stbir__calculate_sample_range_downsample((int)(n_adjusted), (float)(in_pixels_radius), (float)(scale_ratio),
                                (float)(shift), &out_first_pixel, &out_last_pixel, &out_center_of_in);
                            stbir__calculate_coefficients_downsample(stbir_info, (int)(filter), (float)(scale_ratio), (int)(out_first_pixel),
                                (int)(out_last_pixel), (float)(out_center_of_in), stbir__get_contributor(contributors, (int)(n)),
                                stbir__get_coefficient(coefficients, (int)(filter), (float)(scale_ratio), (int)(n), (int)(0)));
                        }
                        stbir__normalize_downsample_coefficients(stbir_info, contributors, coefficients, (int)(filter),
                            (float)(scale_ratio), (float)(shift), (int)(input_size), (int)(output_size));
                    }

                }

                public static float* stbir__get_decode_buffer(stbir__info stbir_info)
                {
                    return &stbir_info.decode_buffer[stbir_info.horizontal_filter_pixel_margin * stbir_info.channels];
                }

                public static void stbir__decode_scanline(stbir__info stbir_info, int n)
                {
                    int c;
                    int channels = (int)(stbir_info.channels);
                    int alpha_channel = (int)(stbir_info.alpha_channel);
                    int type = (int)(stbir_info.type);
                    int colourspace = (int)(stbir_info.colourspace);
                    int input_w = (int)(stbir_info.input_w);
                    ulong input_stride_bytes = (ulong)(stbir_info.input_stride_bytes);
                    float* decode_buffer = stbir__get_decode_buffer(stbir_info);
                    int edge_horizontal = (int)(stbir_info.edge_horizontal);
                    int edge_vertical = (int)(stbir_info.edge_vertical);
                    ulong in_buffer_row_offset =
                        (ulong)(stbir__edge_wrap((int)(edge_vertical), (int)(n), (int)(stbir_info.input_h)) * (int)input_stride_bytes);
                    void* input_data = (sbyte*)(stbir_info.input_data) + in_buffer_row_offset;
                    int max_x = (int)(input_w + stbir_info.horizontal_filter_pixel_margin);
                    int decode = (int)((type) * (STBIR_MAX_COLORSPACES) + (colourspace));
                    int x = (int)(-stbir_info.horizontal_filter_pixel_margin);
                    if (((edge_vertical) == (STBIR_EDGE_ZERO)) && (((n) < (0)) || ((n) >= (stbir_info.input_h))))
                    {
                        for (; (x) < (max_x); x++)
                        {
                            for (c = (int)(0); (c) < (channels); c++)
                            {
                                decode_buffer[x * channels + c] = (float)(0);
                            }
                        }
                        return;
                    }

                    switch (decode)
                    {
                        case ((STBIR_TYPE_UINT8) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_LINEAR)):
                            for (; (x) < (max_x); x++)
                            {
                                int decode_pixel_index = (int)(x * channels);
                                int input_pixel_index = (int)(stbir__edge_wrap((int)(edge_horizontal), (int)(x), (int)(input_w)) * channels);
                                for (c = (int)(0); (c) < (channels); c++)
                                {
                                    decode_buffer[decode_pixel_index + c] = (float)(((float)(((byte*)(input_data))[input_pixel_index + c])) / 255);
                                }
                            }
                            break;
                        case ((STBIR_TYPE_UINT8) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_SRGB)):
                            for (; (x) < (max_x); x++)
                            {
                                int decode_pixel_index = (int)(x * channels);
                                int input_pixel_index = (int)(stbir__edge_wrap((int)(edge_horizontal), (int)(x), (int)(input_w)) * channels);
                                for (c = (int)(0); (c) < (channels); c++)
                                {
                                    decode_buffer[decode_pixel_index + c] =
                                        (float)(stbir__srgb_uchar_to_linear_float[((byte*)(input_data))[input_pixel_index + c]]);
                                }
                                if ((stbir_info.flags & (1 << 1)) == 0)
                                    decode_buffer[decode_pixel_index + alpha_channel] =
                                        (float)(((float)(((byte*)(input_data))[input_pixel_index + alpha_channel])) / 255);
                            }
                            break;
                        case ((STBIR_TYPE_UINT16) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_LINEAR)):
                            for (; (x) < (max_x); x++)
                            {
                                int decode_pixel_index = (int)(x * channels);
                                int input_pixel_index = (int)(stbir__edge_wrap((int)(edge_horizontal), (int)(x), (int)(input_w)) * channels);
                                for (c = (int)(0); (c) < (channels); c++)
                                {
                                    decode_buffer[decode_pixel_index + c] =
                                        (float)(((float)(((ushort*)(input_data))[input_pixel_index + c])) / 65535);
                                }
                            }
                            break;
                        case ((STBIR_TYPE_UINT16) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_SRGB)):
                            for (; (x) < (max_x); x++)
                            {
                                int decode_pixel_index = (int)(x * channels);
                                int input_pixel_index = (int)(stbir__edge_wrap((int)(edge_horizontal), (int)(x), (int)(input_w)) * channels);
                                for (c = (int)(0); (c) < (channels); c++)
                                {
                                    decode_buffer[decode_pixel_index + c] =
                                        (float)(stbir__srgb_to_linear((float)(((float)(((ushort*)(input_data))[input_pixel_index + c])) / 65535)));
                                }
                                if ((stbir_info.flags & (1 << 1)) == 0)
                                    decode_buffer[decode_pixel_index + alpha_channel] =
                                        (float)(((float)(((ushort*)(input_data))[input_pixel_index + alpha_channel])) / 65535);
                            }
                            break;
                        case ((STBIR_TYPE_UINT32) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_LINEAR)):
                            for (; (x) < (max_x); x++)
                            {
                                int decode_pixel_index = (int)(x * channels);
                                int input_pixel_index = (int)(stbir__edge_wrap((int)(edge_horizontal), (int)(x), (int)(input_w)) * channels);
                                for (c = (int)(0); (c) < (channels); c++)
                                {
                                    decode_buffer[decode_pixel_index + c] =
                                        ((float)(((double)(((uint*)(input_data))[input_pixel_index + c])) / 4294967295));
                                }
                            }
                            break;
                        case ((STBIR_TYPE_UINT32) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_SRGB)):
                            for (; (x) < (max_x); x++)
                            {
                                int decode_pixel_index = (int)(x * channels);
                                int input_pixel_index = (int)(stbir__edge_wrap((int)(edge_horizontal), (int)(x), (int)(input_w)) * channels);
                                for (c = (int)(0); (c) < (channels); c++)
                                {
                                    decode_buffer[decode_pixel_index + c] =
                                        (float)(stbir__srgb_to_linear((float)(((double)(((uint*)(input_data))[input_pixel_index + c])) / 4294967295)));
                                }
                                if ((stbir_info.flags & (1 << 1)) == 0)
                                    decode_buffer[decode_pixel_index + alpha_channel] =
                                        ((float)(((double)(((uint*)(input_data))[input_pixel_index + alpha_channel])) / 4294967295));
                            }
                            break;
                        case ((STBIR_TYPE_FLOAT) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_LINEAR)):
                            for (; (x) < (max_x); x++)
                            {
                                int decode_pixel_index = (int)(x * channels);
                                int input_pixel_index = (int)(stbir__edge_wrap((int)(edge_horizontal), (int)(x), (int)(input_w)) * channels);
                                for (c = (int)(0); (c) < (channels); c++)
                                {
                                    decode_buffer[decode_pixel_index + c] = (float)(((float*)(input_data))[input_pixel_index + c]);
                                }
                            }
                            break;
                        case ((STBIR_TYPE_FLOAT) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_SRGB)):
                            for (; (x) < (max_x); x++)
                            {
                                int decode_pixel_index = (int)(x * channels);
                                int input_pixel_index = (int)(stbir__edge_wrap((int)(edge_horizontal), (int)(x), (int)(input_w)) * channels);
                                for (c = (int)(0); (c) < (channels); c++)
                                {
                                    decode_buffer[decode_pixel_index + c] =
                                        (float)(stbir__srgb_to_linear((float)(((float*)(input_data))[input_pixel_index + c])));
                                }
                                if ((stbir_info.flags & (1 << 1)) == 0)
                                    decode_buffer[decode_pixel_index + alpha_channel] =
                                        (float)(((float*)(input_data))[input_pixel_index + alpha_channel]);
                            }
                            break;
                        default:
                            ;
                            break;
                    }

                    if ((stbir_info.flags & (1 << 0)) == 0)
                    {
                        for (x = (int)(-stbir_info.horizontal_filter_pixel_margin); (x) < (max_x); x++)
                        {
                            int decode_pixel_index = (int)(x * channels);
                            float alpha = (float)(decode_buffer[decode_pixel_index + alpha_channel]);
                            if (stbir_info.type != STBIR_TYPE_FLOAT)
                            {
                                alpha += (float)((float)(1) / (1 << 20) / (1 << 20) / (1 << 20) / (1 << 20));
                                decode_buffer[decode_pixel_index + alpha_channel] = (float)(alpha);
                            }
                            for (c = (int)(0); (c) < (channels); c++)
                            {
                                if ((c) == (alpha_channel)) continue;
                                decode_buffer[decode_pixel_index + c] *= (float)(alpha);
                            }
                        }
                    }

                    if ((edge_horizontal) == (STBIR_EDGE_ZERO))
                    {
                        for (x = (int)(-stbir_info.horizontal_filter_pixel_margin); (x) < (0); x++)
                        {
                            for (c = (int)(0); (c) < (channels); c++)
                            {
                                decode_buffer[x * channels + c] = (float)(0);
                            }
                        }
                        for (x = (int)(input_w); (x) < (max_x); x++)
                        {
                            for (c = (int)(0); (c) < (channels); c++)
                            {
                                decode_buffer[x * channels + c] = (float)(0);
                            }
                        }
                    }

                }

                public static float* stbir__get_ring_buffer_entry(float* ring_buffer, int index, int ring_buffer_length)
                {
                    return &ring_buffer[index * ring_buffer_length];
                }

                public static float* stbir__add_empty_ring_buffer_entry(stbir__info stbir_info, int n)
                {
                    int ring_buffer_index;
                    float* ring_buffer;
                    stbir_info.ring_buffer_last_scanline = (int)(n);
                    if ((stbir_info.ring_buffer_begin_index) < (0))
                    {
                        ring_buffer_index = (int)(stbir_info.ring_buffer_begin_index = (int)(0));
                        stbir_info.ring_buffer_first_scanline = (int)(n);
                    }
                    else
                    {
                        ring_buffer_index =
                            (int)
                                ((stbir_info.ring_buffer_begin_index +
                                  (stbir_info.ring_buffer_last_scanline - stbir_info.ring_buffer_first_scanline)) %
                                 stbir_info.ring_buffer_num_entries);
                    }

                    ring_buffer = stbir__get_ring_buffer_entry(stbir_info.ring_buffer, (int)(ring_buffer_index),
                        (int)(stbir_info.ring_buffer_length_bytes / sizeof(float)));
                    CRuntime.memset(ring_buffer, (int)(0), (ulong)(stbir_info.ring_buffer_length_bytes));
                    return ring_buffer;
                }

                public static void stbir__resample_horizontal_upsample(stbir__info stbir_info, int n, float* output_buffer)
                {
                    int x;
                    int k;
                    int output_w = (int)(stbir_info.output_w);
                    int kernel_pixel_width = (int)(stbir_info.horizontal_filter_pixel_width);
                    int channels = (int)(stbir_info.channels);
                    float* decode_buffer = stbir__get_decode_buffer(stbir_info);
                    stbir__contributors* horizontal_contributors = stbir_info.horizontal_contributors;
                    float* horizontal_coefficients = stbir_info.horizontal_coefficients;
                    int coefficient_width = (int)(stbir_info.horizontal_coefficient_width);
                    for (x = (int)(0); (x) < (output_w); x++)
                    {
                        int n0 = (int)(horizontal_contributors[x].n0);
                        int n1 = (int)(horizontal_contributors[x].n1);
                        int out_pixel_index = (int)(x * channels);
                        int coefficient_group = (int)(coefficient_width * x);
                        int coefficient_counter = (int)(0);
                        switch (channels)
                        {
                            case 1:
                                for (k = (int)(n0); k <= n1; k++)
                                {
                                    int in_pixel_index = (int)(k * 1);
                                    float coefficient = (float)(horizontal_coefficients[coefficient_group + coefficient_counter++]);
                                    output_buffer[out_pixel_index + 0] += (float)(decode_buffer[in_pixel_index + 0] * coefficient);
                                }
                                break;
                            case 2:
                                for (k = (int)(n0); k <= n1; k++)
                                {
                                    int in_pixel_index = (int)(k * 2);
                                    float coefficient = (float)(horizontal_coefficients[coefficient_group + coefficient_counter++]);
                                    output_buffer[out_pixel_index + 0] += (float)(decode_buffer[in_pixel_index + 0] * coefficient);
                                    output_buffer[out_pixel_index + 1] += (float)(decode_buffer[in_pixel_index + 1] * coefficient);
                                }
                                break;
                            case 3:
                                for (k = (int)(n0); k <= n1; k++)
                                {
                                    int in_pixel_index = (int)(k * 3);
                                    float coefficient = (float)(horizontal_coefficients[coefficient_group + coefficient_counter++]);
                                    output_buffer[out_pixel_index + 0] += (float)(decode_buffer[in_pixel_index + 0] * coefficient);
                                    output_buffer[out_pixel_index + 1] += (float)(decode_buffer[in_pixel_index + 1] * coefficient);
                                    output_buffer[out_pixel_index + 2] += (float)(decode_buffer[in_pixel_index + 2] * coefficient);
                                }
                                break;
                            case 4:
                                for (k = (int)(n0); k <= n1; k++)
                                {
                                    int in_pixel_index = (int)(k * 4);
                                    float coefficient = (float)(horizontal_coefficients[coefficient_group + coefficient_counter++]);
                                    output_buffer[out_pixel_index + 0] += (float)(decode_buffer[in_pixel_index + 0] * coefficient);
                                    output_buffer[out_pixel_index + 1] += (float)(decode_buffer[in_pixel_index + 1] * coefficient);
                                    output_buffer[out_pixel_index + 2] += (float)(decode_buffer[in_pixel_index + 2] * coefficient);
                                    output_buffer[out_pixel_index + 3] += (float)(decode_buffer[in_pixel_index + 3] * coefficient);
                                }
                                break;
                            default:
                                for (k = (int)(n0); k <= n1; k++)
                                {
                                    int in_pixel_index = (int)(k * channels);
                                    float coefficient = (float)(horizontal_coefficients[coefficient_group + coefficient_counter++]);
                                    int c;
                                    for (c = (int)(0); (c) < (channels); c++)
                                    {
                                        output_buffer[out_pixel_index + c] += (float)(decode_buffer[in_pixel_index + c] * coefficient);
                                    }
                                }
                                break;
                        }
                    }
                }

                public static void stbir__resample_horizontal_downsample(stbir__info stbir_info, int n, float* output_buffer)
                {
                    int x;
                    int k;
                    int input_w = (int)(stbir_info.input_w);
                    int output_w = (int)(stbir_info.output_w);
                    int kernel_pixel_width = (int)(stbir_info.horizontal_filter_pixel_width);
                    int channels = (int)(stbir_info.channels);
                    float* decode_buffer = stbir__get_decode_buffer(stbir_info);
                    stbir__contributors* horizontal_contributors = stbir_info.horizontal_contributors;
                    float* horizontal_coefficients = stbir_info.horizontal_coefficients;
                    int coefficient_width = (int)(stbir_info.horizontal_coefficient_width);
                    int filter_pixel_margin = (int)(stbir_info.horizontal_filter_pixel_margin);
                    int max_x = (int)(input_w + filter_pixel_margin * 2);
                    switch (channels)
                    {
                        case 1:
                            for (x = (int)(0); (x) < (max_x); x++)
                            {
                                int n0 = (int)(horizontal_contributors[x].n0);
                                int n1 = (int)(horizontal_contributors[x].n1);
                                int in_x = (int)(x - filter_pixel_margin);
                                int in_pixel_index = (int)(in_x * 1);
                                int max_n = (int)(n1);
                                int coefficient_group = (int)(coefficient_width * x);
                                for (k = (int)(n0); k <= max_n; k++)
                                {
                                    int out_pixel_index = (int)(k * 1);
                                    float coefficient = (float)(horizontal_coefficients[coefficient_group + k - n0]);
                                    output_buffer[out_pixel_index + 0] += (float)(decode_buffer[in_pixel_index + 0] * coefficient);
                                }
                            }
                            break;
                        case 2:
                            for (x = (int)(0); (x) < (max_x); x++)
                            {
                                int n0 = (int)(horizontal_contributors[x].n0);
                                int n1 = (int)(horizontal_contributors[x].n1);
                                int in_x = (int)(x - filter_pixel_margin);
                                int in_pixel_index = (int)(in_x * 2);
                                int max_n = (int)(n1);
                                int coefficient_group = (int)(coefficient_width * x);
                                for (k = (int)(n0); k <= max_n; k++)
                                {
                                    int out_pixel_index = (int)(k * 2);
                                    float coefficient = (float)(horizontal_coefficients[coefficient_group + k - n0]);
                                    output_buffer[out_pixel_index + 0] += (float)(decode_buffer[in_pixel_index + 0] * coefficient);
                                    output_buffer[out_pixel_index + 1] += (float)(decode_buffer[in_pixel_index + 1] * coefficient);
                                }
                            }
                            break;
                        case 3:
                            for (x = (int)(0); (x) < (max_x); x++)
                            {
                                int n0 = (int)(horizontal_contributors[x].n0);
                                int n1 = (int)(horizontal_contributors[x].n1);
                                int in_x = (int)(x - filter_pixel_margin);
                                int in_pixel_index = (int)(in_x * 3);
                                int max_n = (int)(n1);
                                int coefficient_group = (int)(coefficient_width * x);
                                for (k = (int)(n0); k <= max_n; k++)
                                {
                                    int out_pixel_index = (int)(k * 3);
                                    float coefficient = (float)(horizontal_coefficients[coefficient_group + k - n0]);
                                    output_buffer[out_pixel_index + 0] += (float)(decode_buffer[in_pixel_index + 0] * coefficient);
                                    output_buffer[out_pixel_index + 1] += (float)(decode_buffer[in_pixel_index + 1] * coefficient);
                                    output_buffer[out_pixel_index + 2] += (float)(decode_buffer[in_pixel_index + 2] * coefficient);
                                }
                            }
                            break;
                        case 4:
                            for (x = (int)(0); (x) < (max_x); x++)
                            {
                                int n0 = (int)(horizontal_contributors[x].n0);
                                int n1 = (int)(horizontal_contributors[x].n1);
                                int in_x = (int)(x - filter_pixel_margin);
                                int in_pixel_index = (int)(in_x * 4);
                                int max_n = (int)(n1);
                                int coefficient_group = (int)(coefficient_width * x);
                                for (k = (int)(n0); k <= max_n; k++)
                                {
                                    int out_pixel_index = (int)(k * 4);
                                    float coefficient = (float)(horizontal_coefficients[coefficient_group + k - n0]);
                                    output_buffer[out_pixel_index + 0] += (float)(decode_buffer[in_pixel_index + 0] * coefficient);
                                    output_buffer[out_pixel_index + 1] += (float)(decode_buffer[in_pixel_index + 1] * coefficient);
                                    output_buffer[out_pixel_index + 2] += (float)(decode_buffer[in_pixel_index + 2] * coefficient);
                                    output_buffer[out_pixel_index + 3] += (float)(decode_buffer[in_pixel_index + 3] * coefficient);
                                }
                            }
                            break;
                        default:
                            for (x = (int)(0); (x) < (max_x); x++)
                            {
                                int n0 = (int)(horizontal_contributors[x].n0);
                                int n1 = (int)(horizontal_contributors[x].n1);
                                int in_x = (int)(x - filter_pixel_margin);
                                int in_pixel_index = (int)(in_x * channels);
                                int max_n = (int)(n1);
                                int coefficient_group = (int)(coefficient_width * x);
                                for (k = (int)(n0); k <= max_n; k++)
                                {
                                    int c;
                                    int out_pixel_index = (int)(k * channels);
                                    float coefficient = (float)(horizontal_coefficients[coefficient_group + k - n0]);
                                    for (c = (int)(0); (c) < (channels); c++)
                                    {
                                        output_buffer[out_pixel_index + c] += (float)(decode_buffer[in_pixel_index + c] * coefficient);
                                    }
                                }
                            }
                            break;
                    }

                }

                public static void stbir__decode_and_resample_upsample(stbir__info stbir_info, int n)
                {
                    stbir__decode_scanline(stbir_info, (int)(n));
                    if ((stbir__use_width_upsampling(stbir_info)) != 0)
                        stbir__resample_horizontal_upsample(stbir_info, (int)(n), stbir__add_empty_ring_buffer_entry(stbir_info, (int)(n)));
                    else
                        stbir__resample_horizontal_downsample(stbir_info, (int)(n),
                            stbir__add_empty_ring_buffer_entry(stbir_info, (int)(n)));
                }

                public static void stbir__decode_and_resample_downsample(stbir__info stbir_info, int n)
                {
                    stbir__decode_scanline(stbir_info, (int)(n));
                    CRuntime.memset(stbir_info.horizontal_buffer, (int)(0),
                        (ulong)(stbir_info.output_w * stbir_info.channels * sizeof(float)));
                    if ((stbir__use_width_upsampling(stbir_info)) != 0)
                        stbir__resample_horizontal_upsample(stbir_info, (int)(n), stbir_info.horizontal_buffer);
                    else stbir__resample_horizontal_downsample(stbir_info, (int)(n), stbir_info.horizontal_buffer);
                }

                public static float* stbir__get_ring_buffer_scanline(int get_scanline, float* ring_buffer, int begin_index,
                    int first_scanline, int ring_buffer_num_entries, int ring_buffer_length)
                {
                    int ring_buffer_index = (int)((begin_index + (get_scanline - first_scanline)) % ring_buffer_num_entries);
                    return stbir__get_ring_buffer_entry(ring_buffer, (int)(ring_buffer_index), (int)(ring_buffer_length));
                }

                public static void stbir__encode_scanline(stbir__info stbir_info, int num_pixels, void* output_buffer,
                    float* encode_buffer, int channels, int alpha_channel, int decode)
                {
                    int x;
                    int n;
                    int num_nonalpha;
                    ushort* nonalpha = stackalloc ushort[64];
                    if ((stbir_info.flags & (1 << 0)) == 0)
                    {
                        for (x = (int)(0); (x) < (num_pixels); ++x)
                        {
                            int pixel_index = (int)(x * channels);
                            float alpha = (float)(encode_buffer[pixel_index + alpha_channel]);
                            float reciprocal_alpha = (float)((alpha) != 0 ? 1.0f / alpha : 0);
                            for (n = (int)(0); (n) < (channels); n++)
                            {
                                if (n != alpha_channel) encode_buffer[pixel_index + n] *= (float)(reciprocal_alpha);
                            }
                        }
                    }

                    for (x = (int)(0), num_nonalpha = (int)(0); (x) < (channels); ++x)
                    {
                        if ((x != alpha_channel) || ((stbir_info.flags & (1 << 1)) != 0)) nonalpha[num_nonalpha++] = (ushort)(x);
                    }
                    switch (decode)
                    {
                        case ((STBIR_TYPE_UINT8) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_LINEAR)):
                            for (x = (int)(0); (x) < (num_pixels); ++x)
                            {
                                int pixel_index = (int)(x * channels);
                                for (n = (int)(0); (n) < (channels); n++)
                                {
                                    int index = (int)(pixel_index + n);
                                    ((byte*)(output_buffer))[index] = ((byte)((int)((stbir__saturate((float)(encode_buffer[index])) * 255) + 0.5)));
                                }
                            }
                            break;
                        case ((STBIR_TYPE_UINT8) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_SRGB)):
                            for (x = (int)(0); (x) < (num_pixels); ++x)
                            {
                                int pixel_index = (int)(x * channels);
                                for (n = (int)(0); (n) < (num_nonalpha); n++)
                                {
                                    int index = (int)(pixel_index + nonalpha[n]);
                                    ((byte*)(output_buffer))[index] = (byte)(stbir__linear_to_srgb_uchar((float)(encode_buffer[index])));
                                }
                                if ((stbir_info.flags & (1 << 1)) == 0)
                                    ((byte*)(output_buffer))[pixel_index + alpha_channel] =
                                        ((byte)((int)((stbir__saturate((float)(encode_buffer[pixel_index + alpha_channel])) * 255) + 0.5)));
                            }
                            break;
                        case ((STBIR_TYPE_UINT16) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_LINEAR)):
                            for (x = (int)(0); (x) < (num_pixels); ++x)
                            {
                                int pixel_index = (int)(x * channels);
                                for (n = (int)(0); (n) < (channels); n++)
                                {
                                    int index = (int)(pixel_index + n);
                                    ((ushort*)(output_buffer))[index] =
                                        ((ushort)((int)((stbir__saturate((float)(encode_buffer[index])) * 65535) + 0.5)));
                                }
                            }
                            break;
                        case ((STBIR_TYPE_UINT16) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_SRGB)):
                            for (x = (int)(0); (x) < (num_pixels); ++x)
                            {
                                int pixel_index = (int)(x * channels);
                                for (n = (int)(0); (n) < (num_nonalpha); n++)
                                {
                                    int index = (int)(pixel_index + nonalpha[n]);
                                    ((ushort*)(output_buffer))[index] =
                                        ((ushort)
                                            ((int)((stbir__linear_to_srgb((float)(stbir__saturate((float)(encode_buffer[index])))) * 65535) + 0.5)));
                                }
                                if ((stbir_info.flags & (1 << 1)) == 0)
                                    ((ushort*)(output_buffer))[pixel_index + alpha_channel] =
                                        ((ushort)((int)((stbir__saturate((float)(encode_buffer[pixel_index + alpha_channel])) * 65535) + 0.5)));
                            }
                            break;
                        case ((STBIR_TYPE_UINT32) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_LINEAR)):
                            for (x = (int)(0); (x) < (num_pixels); ++x)
                            {
                                int pixel_index = (int)(x * channels);
                                for (n = (int)(0); (n) < (channels); n++)
                                {
                                    int index = (int)(pixel_index + n);
                                    ((uint*)(output_buffer))[index] =
                                        ((uint)((((double)(stbir__saturate((float)(encode_buffer[index])))) * 4294967295) + 0.5));
                                }
                            }
                            break;
                        case ((STBIR_TYPE_UINT32) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_SRGB)):
                            for (x = (int)(0); (x) < (num_pixels); ++x)
                            {
                                int pixel_index = (int)(x * channels);
                                for (n = (int)(0); (n) < (num_nonalpha); n++)
                                {
                                    int index = (int)(pixel_index + nonalpha[n]);
                                    ((uint*)(output_buffer))[index] =
                                        ((uint)
                                            ((((double)(stbir__linear_to_srgb((float)(stbir__saturate((float)(encode_buffer[index])))))) * 4294967295) +
                                             0.5));
                                }
                                if ((stbir_info.flags & (1 << 1)) == 0)
                                    ((uint*)(output_buffer))[pixel_index + alpha_channel] =
                                        ((uint)
                                            ((int)((((double)(stbir__saturate((float)(encode_buffer[pixel_index + alpha_channel])))) * 4294967295) + 0.5)));
                            }
                            break;
                        case ((STBIR_TYPE_FLOAT) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_LINEAR)):
                            for (x = (int)(0); (x) < (num_pixels); ++x)
                            {
                                int pixel_index = (int)(x * channels);
                                for (n = (int)(0); (n) < (channels); n++)
                                {
                                    int index = (int)(pixel_index + n);
                                    ((float*)(output_buffer))[index] = (float)(encode_buffer[index]);
                                }
                            }
                            break;
                        case ((STBIR_TYPE_FLOAT) * (STBIR_MAX_COLORSPACES) + (STBIR_COLORSPACE_SRGB)):
                            for (x = (int)(0); (x) < (num_pixels); ++x)
                            {
                                int pixel_index = (int)(x * channels);
                                for (n = (int)(0); (n) < (num_nonalpha); n++)
                                {
                                    int index = (int)(pixel_index + nonalpha[n]);
                                    ((float*)(output_buffer))[index] = (float)(stbir__linear_to_srgb((float)(encode_buffer[index])));
                                }
                                if ((stbir_info.flags & (1 << 1)) == 0)
                                    ((float*)(output_buffer))[pixel_index + alpha_channel] = (float)(encode_buffer[pixel_index + alpha_channel]);
                            }
                            break;
                        default:
                            ;
                            break;
                    }

                }

                public static void stbir__resample_vertical_upsample(stbir__info stbir_info, int n, int in_first_scanline,
                    int in_last_scanline, float in_center_of_out)
                {
                    int x;
                    int k;
                    int output_w = (int)(stbir_info.output_w);
                    stbir__contributors* vertical_contributors = stbir_info.vertical_contributors;
                    float* vertical_coefficients = stbir_info.vertical_coefficients;
                    int channels = (int)(stbir_info.channels);
                    int alpha_channel = (int)(stbir_info.alpha_channel);
                    int type = (int)(stbir_info.type);
                    int colourspace = (int)(stbir_info.colourspace);
                    int ring_buffer_entries = (int)(stbir_info.ring_buffer_num_entries);
                    void* output_data = stbir_info.output_data;
                    float* encode_buffer = stbir_info.encode_buffer;
                    int decode = (int)((type) * (STBIR_MAX_COLORSPACES) + (colourspace));
                    int coefficient_width = (int)(stbir_info.vertical_coefficient_width);
                    int coefficient_counter;
                    int contributor = (int)(n);
                    float* ring_buffer = stbir_info.ring_buffer;
                    int ring_buffer_begin_index = (int)(stbir_info.ring_buffer_begin_index);
                    int ring_buffer_first_scanline = (int)(stbir_info.ring_buffer_first_scanline);
                    int ring_buffer_last_scanline = (int)(stbir_info.ring_buffer_last_scanline);
                    int ring_buffer_length = (int)(stbir_info.ring_buffer_length_bytes / sizeof(float));
                    int n0;
                    int n1;
                    int output_row_start;
                    int coefficient_group = (int)(coefficient_width * contributor);
                    n0 = (int)(vertical_contributors[contributor].n0);
                    n1 = (int)(vertical_contributors[contributor].n1);
                    output_row_start = (int)(n * stbir_info.output_stride_bytes);
                    CRuntime.memset(encode_buffer, (int)(0), (ulong)(output_w * sizeof(float) * channels));
                    coefficient_counter = (int)(0);
                    switch (channels)
                    {
                        case 1:
                            for (k = (int)(n0); k <= n1; k++)
                            {
                                int coefficient_index = (int)(coefficient_counter++);
                                float* ring_buffer_entry = stbir__get_ring_buffer_scanline((int)(k), ring_buffer, (int)(ring_buffer_begin_index),
                                    (int)(ring_buffer_first_scanline), (int)(ring_buffer_entries), (int)(ring_buffer_length));
                                float coefficient = (float)(vertical_coefficients[coefficient_group + coefficient_index]);
                                for (x = (int)(0); (x) < (output_w); ++x)
                                {
                                    int in_pixel_index = (int)(x * 1);
                                    encode_buffer[in_pixel_index + 0] += (float)(ring_buffer_entry[in_pixel_index + 0] * coefficient);
                                }
                            }
                            break;
                        case 2:
                            for (k = (int)(n0); k <= n1; k++)
                            {
                                int coefficient_index = (int)(coefficient_counter++);
                                float* ring_buffer_entry = stbir__get_ring_buffer_scanline((int)(k), ring_buffer, (int)(ring_buffer_begin_index),
                                    (int)(ring_buffer_first_scanline), (int)(ring_buffer_entries), (int)(ring_buffer_length));
                                float coefficient = (float)(vertical_coefficients[coefficient_group + coefficient_index]);
                                for (x = (int)(0); (x) < (output_w); ++x)
                                {
                                    int in_pixel_index = (int)(x * 2);
                                    encode_buffer[in_pixel_index + 0] += (float)(ring_buffer_entry[in_pixel_index + 0] * coefficient);
                                    encode_buffer[in_pixel_index + 1] += (float)(ring_buffer_entry[in_pixel_index + 1] * coefficient);
                                }
                            }
                            break;
                        case 3:
                            for (k = (int)(n0); k <= n1; k++)
                            {
                                int coefficient_index = (int)(coefficient_counter++);
                                float* ring_buffer_entry = stbir__get_ring_buffer_scanline((int)(k), ring_buffer, (int)(ring_buffer_begin_index),
                                    (int)(ring_buffer_first_scanline), (int)(ring_buffer_entries), (int)(ring_buffer_length));
                                float coefficient = (float)(vertical_coefficients[coefficient_group + coefficient_index]);
                                for (x = (int)(0); (x) < (output_w); ++x)
                                {
                                    int in_pixel_index = (int)(x * 3);
                                    encode_buffer[in_pixel_index + 0] += (float)(ring_buffer_entry[in_pixel_index + 0] * coefficient);
                                    encode_buffer[in_pixel_index + 1] += (float)(ring_buffer_entry[in_pixel_index + 1] * coefficient);
                                    encode_buffer[in_pixel_index + 2] += (float)(ring_buffer_entry[in_pixel_index + 2] * coefficient);
                                }
                            }
                            break;
                        case 4:
                            for (k = (int)(n0); k <= n1; k++)
                            {
                                int coefficient_index = (int)(coefficient_counter++);
                                float* ring_buffer_entry = stbir__get_ring_buffer_scanline((int)(k), ring_buffer, (int)(ring_buffer_begin_index),
                                    (int)(ring_buffer_first_scanline), (int)(ring_buffer_entries), (int)(ring_buffer_length));
                                float coefficient = (float)(vertical_coefficients[coefficient_group + coefficient_index]);
                                for (x = (int)(0); (x) < (output_w); ++x)
                                {
                                    int in_pixel_index = (int)(x * 4);
                                    encode_buffer[in_pixel_index + 0] += (float)(ring_buffer_entry[in_pixel_index + 0] * coefficient);
                                    encode_buffer[in_pixel_index + 1] += (float)(ring_buffer_entry[in_pixel_index + 1] * coefficient);
                                    encode_buffer[in_pixel_index + 2] += (float)(ring_buffer_entry[in_pixel_index + 2] * coefficient);
                                    encode_buffer[in_pixel_index + 3] += (float)(ring_buffer_entry[in_pixel_index + 3] * coefficient);
                                }
                            }
                            break;
                        default:
                            for (k = (int)(n0); k <= n1; k++)
                            {
                                int coefficient_index = (int)(coefficient_counter++);
                                float* ring_buffer_entry = stbir__get_ring_buffer_scanline((int)(k), ring_buffer, (int)(ring_buffer_begin_index),
                                    (int)(ring_buffer_first_scanline), (int)(ring_buffer_entries), (int)(ring_buffer_length));
                                float coefficient = (float)(vertical_coefficients[coefficient_group + coefficient_index]);
                                for (x = (int)(0); (x) < (output_w); ++x)
                                {
                                    int in_pixel_index = (int)(x * channels);
                                    int c;
                                    for (c = (int)(0); (c) < (channels); c++)
                                    {
                                        encode_buffer[in_pixel_index + c] += (float)(ring_buffer_entry[in_pixel_index + c] * coefficient);
                                    }
                                }
                            }
                            break;
                    }

                    stbir__encode_scanline(stbir_info, (int)(output_w), (sbyte*)(output_data) + output_row_start, encode_buffer,
                        (int)(channels), (int)(alpha_channel), (int)(decode));
                }

                public static void stbir__resample_vertical_downsample(stbir__info stbir_info, int n, int in_first_scanline,
                    int in_last_scanline, float in_center_of_out)
                {
                    int x;
                    int k;
                    int output_w = (int)(stbir_info.output_w);
                    int output_h = (int)(stbir_info.output_h);
                    stbir__contributors* vertical_contributors = stbir_info.vertical_contributors;
                    float* vertical_coefficients = stbir_info.vertical_coefficients;
                    int channels = (int)(stbir_info.channels);
                    int ring_buffer_entries = (int)(stbir_info.ring_buffer_num_entries);
                    void* output_data = stbir_info.output_data;
                    float* horizontal_buffer = stbir_info.horizontal_buffer;
                    int coefficient_width = (int)(stbir_info.vertical_coefficient_width);
                    int contributor = (int)(n + stbir_info.vertical_filter_pixel_margin);
                    float* ring_buffer = stbir_info.ring_buffer;
                    int ring_buffer_begin_index = (int)(stbir_info.ring_buffer_begin_index);
                    int ring_buffer_first_scanline = (int)(stbir_info.ring_buffer_first_scanline);
                    int ring_buffer_last_scanline = (int)(stbir_info.ring_buffer_last_scanline);
                    int ring_buffer_length = (int)(stbir_info.ring_buffer_length_bytes / sizeof(float));
                    int n0;
                    int n1;
                    n0 = (int)(vertical_contributors[contributor].n0);
                    n1 = (int)(vertical_contributors[contributor].n1);
                    for (k = (int)(n0); k <= n1; k++)
                    {
                        int coefficient_index = (int)(k - n0);
                        int coefficient_group = (int)(coefficient_width * contributor);
                        float coefficient = (float)(vertical_coefficients[coefficient_group + coefficient_index]);
                        float* ring_buffer_entry = stbir__get_ring_buffer_scanline((int)(k), ring_buffer, (int)(ring_buffer_begin_index),
                            (int)(ring_buffer_first_scanline), (int)(ring_buffer_entries), (int)(ring_buffer_length));
                        switch (channels)
                        {
                            case 1:
                                for (x = (int)(0); (x) < (output_w); x++)
                                {
                                    int in_pixel_index = (int)(x * 1);
                                    ring_buffer_entry[in_pixel_index + 0] += (float)(horizontal_buffer[in_pixel_index + 0] * coefficient);
                                }
                                break;
                            case 2:
                                for (x = (int)(0); (x) < (output_w); x++)
                                {
                                    int in_pixel_index = (int)(x * 2);
                                    ring_buffer_entry[in_pixel_index + 0] += (float)(horizontal_buffer[in_pixel_index + 0] * coefficient);
                                    ring_buffer_entry[in_pixel_index + 1] += (float)(horizontal_buffer[in_pixel_index + 1] * coefficient);
                                }
                                break;
                            case 3:
                                for (x = (int)(0); (x) < (output_w); x++)
                                {
                                    int in_pixel_index = (int)(x * 3);
                                    ring_buffer_entry[in_pixel_index + 0] += (float)(horizontal_buffer[in_pixel_index + 0] * coefficient);
                                    ring_buffer_entry[in_pixel_index + 1] += (float)(horizontal_buffer[in_pixel_index + 1] * coefficient);
                                    ring_buffer_entry[in_pixel_index + 2] += (float)(horizontal_buffer[in_pixel_index + 2] * coefficient);
                                }
                                break;
                            case 4:
                                for (x = (int)(0); (x) < (output_w); x++)
                                {
                                    int in_pixel_index = (int)(x * 4);
                                    ring_buffer_entry[in_pixel_index + 0] += (float)(horizontal_buffer[in_pixel_index + 0] * coefficient);
                                    ring_buffer_entry[in_pixel_index + 1] += (float)(horizontal_buffer[in_pixel_index + 1] * coefficient);
                                    ring_buffer_entry[in_pixel_index + 2] += (float)(horizontal_buffer[in_pixel_index + 2] * coefficient);
                                    ring_buffer_entry[in_pixel_index + 3] += (float)(horizontal_buffer[in_pixel_index + 3] * coefficient);
                                }
                                break;
                            default:
                                for (x = (int)(0); (x) < (output_w); x++)
                                {
                                    int in_pixel_index = (int)(x * channels);
                                    int c;
                                    for (c = (int)(0); (c) < (channels); c++)
                                    {
                                        ring_buffer_entry[in_pixel_index + c] += (float)(horizontal_buffer[in_pixel_index + c] * coefficient);
                                    }
                                }
                                break;
                        }
                    }
                }

                public static void stbir__buffer_loop_upsample(stbir__info stbir_info)
                {
                    int y;
                    float scale_ratio = (float)(stbir_info.vertical_scale);
                    float out_scanlines_radius =
                        (float)(stbir__filter_info_table[stbir_info.vertical_filter].support((float)(1 / scale_ratio)) * scale_ratio);
                    for (y = (int)(0); (y) < (stbir_info.output_h); y++)
                    {
                        float in_center_of_out = (float)(0);
                        int in_first_scanline = (int)(0);
                        int in_last_scanline = (int)(0);
                        stbir__calculate_sample_range_upsample((int)(y), (float)(out_scanlines_radius), (float)(scale_ratio),
                            (float)(stbir_info.vertical_shift), &in_first_scanline, &in_last_scanline, &in_center_of_out);
                        if ((stbir_info.ring_buffer_begin_index) >= (0))
                        {
                            while ((in_first_scanline) > (stbir_info.ring_buffer_first_scanline))
                            {
                                if ((stbir_info.ring_buffer_first_scanline) == (stbir_info.ring_buffer_last_scanline))
                                {
                                    stbir_info.ring_buffer_begin_index = (int)(-1);
                                    stbir_info.ring_buffer_first_scanline = (int)(0);
                                    stbir_info.ring_buffer_last_scanline = (int)(0);
                                    break;
                                }
                                else
                                {
                                    stbir_info.ring_buffer_first_scanline++;
                                    stbir_info.ring_buffer_begin_index =
                                        (int)((stbir_info.ring_buffer_begin_index + 1) % stbir_info.ring_buffer_num_entries);
                                }
                            }
                        }
                        if ((stbir_info.ring_buffer_begin_index) < (0))
                            stbir__decode_and_resample_upsample(stbir_info, (int)(in_first_scanline));
                        while ((in_last_scanline) > (stbir_info.ring_buffer_last_scanline))
                        {
                            stbir__decode_and_resample_upsample(stbir_info, (int)(stbir_info.ring_buffer_last_scanline + 1));
                        }
                        stbir__resample_vertical_upsample(stbir_info, (int)(y), (int)(in_first_scanline), (int)(in_last_scanline),
                            (float)(in_center_of_out));
                    }
                }

                public static void stbir__empty_ring_buffer(stbir__info stbir_info, int first_necessary_scanline)
                {
                    int output_stride_bytes = (int)(stbir_info.output_stride_bytes);
                    int channels = (int)(stbir_info.channels);
                    int alpha_channel = (int)(stbir_info.alpha_channel);
                    int type = (int)(stbir_info.type);
                    int colourspace = (int)(stbir_info.colourspace);
                    int output_w = (int)(stbir_info.output_w);
                    void* output_data = stbir_info.output_data;
                    int decode = (int)((type) * (STBIR_MAX_COLORSPACES) + (colourspace));
                    float* ring_buffer = stbir_info.ring_buffer;
                    int ring_buffer_length = (int)(stbir_info.ring_buffer_length_bytes / sizeof(float));
                    if ((stbir_info.ring_buffer_begin_index) >= (0))
                    {
                        while ((first_necessary_scanline) > (stbir_info.ring_buffer_first_scanline))
                        {
                            if (((stbir_info.ring_buffer_first_scanline) >= (0)) &&
                                ((stbir_info.ring_buffer_first_scanline) < (stbir_info.output_h)))
                            {
                                int output_row_start = (int)(stbir_info.ring_buffer_first_scanline * output_stride_bytes);
                                float* ring_buffer_entry = stbir__get_ring_buffer_entry(ring_buffer, (int)(stbir_info.ring_buffer_begin_index),
                                    (int)(ring_buffer_length));
                                stbir__encode_scanline(stbir_info, (int)(output_w), (sbyte*)(output_data) + output_row_start, ring_buffer_entry,
                                    (int)(channels), (int)(alpha_channel), (int)(decode));
                            }
                            if ((stbir_info.ring_buffer_first_scanline) == (stbir_info.ring_buffer_last_scanline))
                            {
                                stbir_info.ring_buffer_begin_index = (int)(-1);
                                stbir_info.ring_buffer_first_scanline = (int)(0);
                                stbir_info.ring_buffer_last_scanline = (int)(0);
                                break;
                            }
                            else
                            {
                                stbir_info.ring_buffer_first_scanline++;
                                stbir_info.ring_buffer_begin_index =
                                    (int)((stbir_info.ring_buffer_begin_index + 1) % stbir_info.ring_buffer_num_entries);
                            }
                        }
                    }

                }

                public static void stbir__buffer_loop_downsample(stbir__info stbir_info)
                {
                    int y;
                    float scale_ratio = (float)(stbir_info.vertical_scale);
                    int output_h = (int)(stbir_info.output_h);
                    float in_pixels_radius =
                        (float)(stbir__filter_info_table[stbir_info.vertical_filter].support((float)(scale_ratio)) / scale_ratio);
                    int pixel_margin = (int)(stbir_info.vertical_filter_pixel_margin);
                    int max_y = (int)(stbir_info.input_h + pixel_margin);
                    for (y = (int)(-pixel_margin); (y) < (max_y); y++)
                    {
                        float out_center_of_in;
                        int out_first_scanline;
                        int out_last_scanline;
                        stbir__calculate_sample_range_downsample((int)(y), (float)(in_pixels_radius), (float)(scale_ratio),
                            (float)(stbir_info.vertical_shift), &out_first_scanline, &out_last_scanline, &out_center_of_in);
                        if (((out_last_scanline) < (0)) || ((out_first_scanline) >= (output_h))) continue;
                        stbir__empty_ring_buffer(stbir_info, (int)(out_first_scanline));
                        stbir__decode_and_resample_downsample(stbir_info, (int)(y));
                        if ((stbir_info.ring_buffer_begin_index) < (0))
                            stbir__add_empty_ring_buffer_entry(stbir_info, (int)(out_first_scanline));
                        while ((out_last_scanline) > (stbir_info.ring_buffer_last_scanline))
                        {
                            stbir__add_empty_ring_buffer_entry(stbir_info, (int)(stbir_info.ring_buffer_last_scanline + 1));
                        }
                        stbir__resample_vertical_downsample(stbir_info, (int)(y), (int)(out_first_scanline), (int)(out_last_scanline),
                            (float)(out_center_of_in));
                    }
                    stbir__empty_ring_buffer(stbir_info, (int)(stbir_info.output_h));
                }

                public static void stbir__setup(stbir__info info, int input_w, int input_h, int output_w, int output_h, int channels)
                {
                    info.input_w = (int)(input_w);
                    info.input_h = (int)(input_h);
                    info.output_w = (int)(output_w);
                    info.output_h = (int)(output_h);
                    info.channels = (int)(channels);
                }

                public static void stbir__calculate_transform(stbir__info info, float s0, float t0, float s1, float t1,
                    float* transform)
                {
                    info.s0 = (float)(s0);
                    info.t0 = (float)(t0);
                    info.s1 = (float)(s1);
                    info.t1 = (float)(t1);
                    if ((transform) != null)
                    {
                        info.horizontal_scale = (float)(transform[0]);
                        info.vertical_scale = (float)(transform[1]);
                        info.horizontal_shift = (float)(transform[2]);
                        info.vertical_shift = (float)(transform[3]);
                    }
                    else
                    {
                        info.horizontal_scale = (float)(((float)(info.output_w) / info.input_w) / (s1 - s0));
                        info.vertical_scale = (float)(((float)(info.output_h) / info.input_h) / (t1 - t0));
                        info.horizontal_shift = (float)(s0 * info.output_w / (s1 - s0));
                        info.vertical_shift = (float)(t0 * info.output_h / (t1 - t0));
                    }

                }

                public static void stbir__choose_filter(stbir__info info, int h_filter, int v_filter)
                {
                    if ((h_filter) == (0))
                        h_filter =
                            (int)
                                ((stbir__use_upsampling((float)(info.horizontal_scale))) != 0 ? STBIR_FILTER_CATMULLROM : STBIR_FILTER_MITCHELL);
                    if ((v_filter) == (0))
                        v_filter =
                            (int)
                                ((stbir__use_upsampling((float)(info.vertical_scale))) != 0 ? STBIR_FILTER_CATMULLROM : STBIR_FILTER_MITCHELL);
                    info.horizontal_filter = (int)(h_filter);
                    info.vertical_filter = (int)(v_filter);
                }

                public static uint stbir__calculate_memory(stbir__info info)
                {
                    int pixel_margin =
                        (int)(stbir__get_filter_pixel_margin((int)(info.horizontal_filter), (float)(info.horizontal_scale)));
                    int filter_height =
                        (int)(stbir__get_filter_pixel_width((int)(info.vertical_filter), (float)(info.vertical_scale)));
                    info.horizontal_num_contributors =
                        (int)
                            (stbir__get_contributors((float)(info.horizontal_scale), (int)(info.horizontal_filter), (int)(info.input_w),
                                (int)(info.output_w)));
                    info.vertical_num_contributors =
                        (int)
                            (stbir__get_contributors((float)(info.vertical_scale), (int)(info.vertical_filter), (int)(info.input_h),
                                (int)(info.output_h)));
                    info.ring_buffer_num_entries = (int)(filter_height + 1);
                    info.horizontal_contributors_size = (int)(info.horizontal_num_contributors * sizeof(stbir__contributors));
                    info.horizontal_coefficients_size = (int)(stbir__get_total_horizontal_coefficients(info) * sizeof(float));
                    info.vertical_contributors_size = (int)(info.vertical_num_contributors * sizeof(stbir__contributors));
                    info.vertical_coefficients_size = (int)(stbir__get_total_vertical_coefficients(info) * sizeof(float));
                    info.decode_buffer_size = (int)((info.input_w + pixel_margin * 2) * info.channels * sizeof(float));
                    info.horizontal_buffer_size = (int)(info.output_w * info.channels * sizeof(float));
                    info.ring_buffer_size = (int)(info.output_w * info.channels * info.ring_buffer_num_entries * sizeof(float));
                    info.encode_buffer_size = (int)(info.output_w * info.channels * sizeof(float));
                    if ((stbir__use_height_upsampling(info)) != 0) info.horizontal_buffer_size = (int)(0);
                    else info.encode_buffer_size = (int)(0);
                    return
                        (uint)
                            (info.horizontal_contributors_size + info.horizontal_coefficients_size + info.vertical_contributors_size +
                             info.vertical_coefficients_size + info.decode_buffer_size + info.horizontal_buffer_size + info.ring_buffer_size +
                             info.encode_buffer_size);
                }

                public static int stbir__resize_allocated(stbir__info info, void* input_data, int input_stride_in_bytes,
                    void* output_data, int output_stride_in_bytes, int alpha_channel, uint flags, int type, int edge_horizontal,
                    int edge_vertical, int colourspace, void* tempmem, ulong tempmem_size_in_bytes)
                {
                    ulong memory_required = (ulong)(stbir__calculate_memory(info));
                    int width_stride_input =
                        (int)((input_stride_in_bytes) != 0 ? input_stride_in_bytes : info.channels * info.input_w * stbir__type_size[type]);
                    int width_stride_output =
                        (int)((output_stride_in_bytes) != 0 ? output_stride_in_bytes : info.channels * info.output_w * stbir__type_size[type]);
                    if (((info.channels) < (0)) || ((info.channels) > (64))) return (int)(0);
                    if ((info.horizontal_filter) >= (6)) return (int)(0);
                    if ((info.vertical_filter) >= (6)) return (int)(0);
                    if ((alpha_channel) < (0)) flags |= (uint)((1 << 1) | (1 << 0));
                    if (((flags & (1 << 1)) == 0) || ((flags & (1 << 0)) == 0))
                        if ((alpha_channel) >= (info.channels)) return (int)(0);
                    if (tempmem == null) return (int)(0);
                    if ((tempmem_size_in_bytes) < (memory_required)) return (int)(0);
                    CRuntime.memset(tempmem, (int)(0), (ulong)(tempmem_size_in_bytes));
                    info.input_data = input_data;
                    info.input_stride_bytes = (int)(width_stride_input);
                    info.output_data = output_data;
                    info.output_stride_bytes = (int)(width_stride_output);
                    info.alpha_channel = (int)(alpha_channel);
                    info.flags = (uint)(flags);
                    info.type = (int)(type);
                    info.edge_horizontal = (int)(edge_horizontal);
                    info.edge_vertical = (int)(edge_vertical);
                    info.colourspace = (int)(colourspace);
                    info.horizontal_coefficient_width =
                        (int)(stbir__get_coefficient_width((int)(info.horizontal_filter), (float)(info.horizontal_scale)));
                    info.vertical_coefficient_width =
                        (int)(stbir__get_coefficient_width((int)(info.vertical_filter), (float)(info.vertical_scale)));
                    info.horizontal_filter_pixel_width =
                        (int)(stbir__get_filter_pixel_width((int)(info.horizontal_filter), (float)(info.horizontal_scale)));
                    info.vertical_filter_pixel_width =
                        (int)(stbir__get_filter_pixel_width((int)(info.vertical_filter), (float)(info.vertical_scale)));
                    info.horizontal_filter_pixel_margin =
                        (int)(stbir__get_filter_pixel_margin((int)(info.horizontal_filter), (float)(info.horizontal_scale)));
                    info.vertical_filter_pixel_margin =
                        (int)(stbir__get_filter_pixel_margin((int)(info.vertical_filter), (float)(info.vertical_scale)));
                    info.ring_buffer_length_bytes = (int)(info.output_w * info.channels * sizeof(float));
                    info.decode_buffer_pixels = (int)(info.input_w + info.horizontal_filter_pixel_margin * 2);
                    info.horizontal_contributors = (stbir__contributors*)(tempmem);
                    info.horizontal_coefficients =
                        (float*)(((byte*)(info.horizontal_contributors)) + info.horizontal_contributors_size);
                    info.vertical_contributors =
                        (stbir__contributors*)(((byte*)(info.horizontal_coefficients)) + info.horizontal_coefficients_size);
                    info.vertical_coefficients = (float*)(((byte*)(info.vertical_contributors)) + info.vertical_contributors_size);
                    info.decode_buffer = (float*)(((byte*)(info.vertical_coefficients)) + info.vertical_coefficients_size);
                    if ((stbir__use_height_upsampling(info)) != 0)
                    {
                        info.horizontal_buffer = null;
                        info.ring_buffer = (float*)(((byte*)(info.decode_buffer)) + info.decode_buffer_size);
                        info.encode_buffer = (float*)(((byte*)(info.ring_buffer)) + info.ring_buffer_size);
                    }
                    else
                    {
                        info.horizontal_buffer = (float*)(((byte*)(info.decode_buffer)) + info.decode_buffer_size);
                        info.ring_buffer = (float*)(((byte*)(info.horizontal_buffer)) + info.horizontal_buffer_size);
                        info.encode_buffer = null;
                    }

                    info.ring_buffer_begin_index = (int)(-1);
                    stbir__calculate_filters(info, info.horizontal_contributors, info.horizontal_coefficients,
                        (int)(info.horizontal_filter), (float)(info.horizontal_scale), (float)(info.horizontal_shift),
                        (int)(info.input_w), (int)(info.output_w));
                    stbir__calculate_filters(info, info.vertical_contributors, info.vertical_coefficients, (int)(info.vertical_filter),
                        (float)(info.vertical_scale), (float)(info.vertical_shift), (int)(info.input_h), (int)(info.output_h));
                    if ((stbir__use_height_upsampling(info)) != 0) stbir__buffer_loop_upsample(info);
                    else stbir__buffer_loop_downsample(info);
                    return (int)(1);
                }

                public static int stbir__resize_arbitrary(void* alloc_context, void* input_data, int input_w, int input_h,
                    int input_stride_in_bytes, void* output_data, int output_w, int output_h, int output_stride_in_bytes, float s0,
                    float t0, float s1, float t1, float* transform, int channels, int alpha_channel, uint flags, int type, int h_filter,
                    int v_filter, int edge_horizontal, int edge_vertical, int colourspace)
                {
                    stbir__info info = new stbir__info();
                    int result;
                    ulong memory_required;
                    void* extra_memory;
                    stbir__setup(info, (int)(input_w), (int)(input_h), (int)(output_w), (int)(output_h), (int)(channels));
                    stbir__calculate_transform(info, (float)(s0), (float)(t0), (float)(s1), (float)(t1), transform);
                    stbir__choose_filter(info, (int)(h_filter), (int)(v_filter));
                    memory_required = (ulong)(stbir__calculate_memory(info));
                    extra_memory = CRuntime.malloc((ulong)(memory_required));
                    if (extra_memory == null) return (int)(0);
                    result =
                        (int)
                            (stbir__resize_allocated(info, input_data, (int)(input_stride_in_bytes), output_data,
                                (int)(output_stride_in_bytes), (int)(alpha_channel), (uint)(flags), (int)(type), (int)(edge_horizontal),
                                (int)(edge_vertical), (int)(colourspace), extra_memory, (ulong)(memory_required)));
                    CRuntime.free(extra_memory);
                    return (int)(result);
                }

                public static int stbir_resize_uint8(byte* input_pixels, int input_w, int input_h, int input_stride_in_bytes,
                    byte* output_pixels, int output_w, int output_h, int output_stride_in_bytes, int num_channels)
                {
                    return
                        (int)
                            (stbir__resize_arbitrary(null, input_pixels, (int)(input_w), (int)(input_h), (int)(input_stride_in_bytes),
                                output_pixels, (int)(output_w), (int)(output_h), (int)(output_stride_in_bytes), (float)(0), (float)(0),
                                (float)(1), (float)(1), null, (int)(num_channels), (int)(-1), (uint)(0), (int)(STBIR_TYPE_UINT8),
                                (int)(STBIR_FILTER_DEFAULT), (int)(STBIR_FILTER_DEFAULT), (int)(STBIR_EDGE_CLAMP), (int)(STBIR_EDGE_CLAMP),
                                (int)(STBIR_COLORSPACE_LINEAR)));
                }

                public static int stbir_resize_float(float* input_pixels, int input_w, int input_h, int input_stride_in_bytes,
                    float* output_pixels, int output_w, int output_h, int output_stride_in_bytes, int num_channels)
                {
                    return
                        (int)
                            (stbir__resize_arbitrary(null, input_pixels, (int)(input_w), (int)(input_h), (int)(input_stride_in_bytes),
                                output_pixels, (int)(output_w), (int)(output_h), (int)(output_stride_in_bytes), (float)(0), (float)(0),
                                (float)(1), (float)(1), null, (int)(num_channels), (int)(-1), (uint)(0), (int)(STBIR_TYPE_FLOAT),
                                (int)(STBIR_FILTER_DEFAULT), (int)(STBIR_FILTER_DEFAULT), (int)(STBIR_EDGE_CLAMP), (int)(STBIR_EDGE_CLAMP),
                                (int)(STBIR_COLORSPACE_LINEAR)));
                }

                public static int stbir_resize_uint8_srgb(byte* input_pixels, int input_w, int input_h, int input_stride_in_bytes,
                    byte* output_pixels, int output_w, int output_h, int output_stride_in_bytes, int num_channels, int alpha_channel,
                    int flags)
                {
                    return
                        (int)
                            (stbir__resize_arbitrary(null, input_pixels, (int)(input_w), (int)(input_h), (int)(input_stride_in_bytes),
                                output_pixels, (int)(output_w), (int)(output_h), (int)(output_stride_in_bytes), (float)(0), (float)(0),
                                (float)(1), (float)(1), null, (int)(num_channels), (int)(alpha_channel), (uint)(flags),
                                (int)(STBIR_TYPE_UINT8),
                                (int)(STBIR_FILTER_DEFAULT), (int)(STBIR_FILTER_DEFAULT), (int)(STBIR_EDGE_CLAMP), (int)(STBIR_EDGE_CLAMP),
                                (int)(STBIR_COLORSPACE_SRGB)));
                }

                public static int stbir_resize_uint8_srgb_edgemode(byte* input_pixels, int input_w, int input_h,
                    int input_stride_in_bytes, byte* output_pixels, int output_w, int output_h, int output_stride_in_bytes,
                    int num_channels, int alpha_channel, int flags, int edge_wrap_mode)
                {
                    return
                        (int)
                            (stbir__resize_arbitrary(null, input_pixels, (int)(input_w), (int)(input_h), (int)(input_stride_in_bytes),
                                output_pixels, (int)(output_w), (int)(output_h), (int)(output_stride_in_bytes), (float)(0), (float)(0),
                                (float)(1), (float)(1), null, (int)(num_channels), (int)(alpha_channel), (uint)(flags),
                                (int)(STBIR_TYPE_UINT8),
                                (int)(STBIR_FILTER_DEFAULT), (int)(STBIR_FILTER_DEFAULT), (int)(edge_wrap_mode), (int)(edge_wrap_mode),
                                (int)(STBIR_COLORSPACE_SRGB)));
                }

                public static int stbir_resize_uint8_generic(byte* input_pixels, int input_w, int input_h, int input_stride_in_bytes,
                    byte* output_pixels, int output_w, int output_h, int output_stride_in_bytes, int num_channels, int alpha_channel,
                    int flags, int edge_wrap_mode, int filter, int space, void* alloc_context)
                {
                    return
                        (int)
                            (stbir__resize_arbitrary(alloc_context, input_pixels, (int)(input_w), (int)(input_h),
                                (int)(input_stride_in_bytes), output_pixels, (int)(output_w), (int)(output_h), (int)(output_stride_in_bytes),
                                (float)(0), (float)(0), (float)(1), (float)(1), null, (int)(num_channels), (int)(alpha_channel),
                                (uint)(flags),
                                (int)(STBIR_TYPE_UINT8), (int)(filter), (int)(filter), (int)(edge_wrap_mode), (int)(edge_wrap_mode),
                                (int)(space)));
                }

                public static int stbir_resize_uint16_generic(ushort* input_pixels, int input_w, int input_h,
                    int input_stride_in_bytes, ushort* output_pixels, int output_w, int output_h, int output_stride_in_bytes,
                    int num_channels, int alpha_channel, int flags, int edge_wrap_mode, int filter, int space, void* alloc_context)
                {
                    return
                        (int)
                            (stbir__resize_arbitrary(alloc_context, input_pixels, (int)(input_w), (int)(input_h),
                                (int)(input_stride_in_bytes), output_pixels, (int)(output_w), (int)(output_h), (int)(output_stride_in_bytes),
                                (float)(0), (float)(0), (float)(1), (float)(1), null, (int)(num_channels), (int)(alpha_channel),
                                (uint)(flags),
                                (int)(STBIR_TYPE_UINT16), (int)(filter), (int)(filter), (int)(edge_wrap_mode), (int)(edge_wrap_mode),
                                (int)(space)));
                }

                public static int stbir_resize_float_generic(float* input_pixels, int input_w, int input_h, int input_stride_in_bytes,
                    float* output_pixels, int output_w, int output_h, int output_stride_in_bytes, int num_channels, int alpha_channel,
                    int flags, int edge_wrap_mode, int filter, int space, void* alloc_context)
                {
                    return
                        (int)
                            (stbir__resize_arbitrary(alloc_context, input_pixels, (int)(input_w), (int)(input_h),
                                (int)(input_stride_in_bytes), output_pixels, (int)(output_w), (int)(output_h), (int)(output_stride_in_bytes),
                                (float)(0), (float)(0), (float)(1), (float)(1), null, (int)(num_channels), (int)(alpha_channel),
                                (uint)(flags),
                                (int)(STBIR_TYPE_FLOAT), (int)(filter), (int)(filter), (int)(edge_wrap_mode), (int)(edge_wrap_mode),
                                (int)(space)));
                }

                public static int stbir_resize(void* input_pixels, int input_w, int input_h, int input_stride_in_bytes,
                    void* output_pixels, int output_w, int output_h, int output_stride_in_bytes, int datatype, int num_channels,
                    int alpha_channel, int flags, int edge_mode_horizontal, int edge_mode_vertical, int filter_horizontal,
                    int filter_vertical, int space, void* alloc_context)
                {
                    return
                        (int)
                            (stbir__resize_arbitrary(alloc_context, input_pixels, (int)(input_w), (int)(input_h),
                                (int)(input_stride_in_bytes), output_pixels, (int)(output_w), (int)(output_h), (int)(output_stride_in_bytes),
                                (float)(0), (float)(0), (float)(1), (float)(1), null, (int)(num_channels), (int)(alpha_channel),
                                (uint)(flags),
                                (int)(datatype), (int)(filter_horizontal), (int)(filter_vertical), (int)(edge_mode_horizontal),
                                (int)(edge_mode_vertical), (int)(space)));
                }

                public static int stbir_resize_subpixel(void* input_pixels, int input_w, int input_h, int input_stride_in_bytes,
                    void* output_pixels, int output_w, int output_h, int output_stride_in_bytes, int datatype, int num_channels,
                    int alpha_channel, int flags, int edge_mode_horizontal, int edge_mode_vertical, int filter_horizontal,
                    int filter_vertical, int space, void* alloc_context, float x_scale, float y_scale, float x_offset, float y_offset)
                {
                    float* transform = stackalloc float[4];
                    transform[0] = (float)(x_scale);
                    transform[1] = (float)(y_scale);
                    transform[2] = (float)(x_offset);
                    transform[3] = (float)(y_offset);
                    return
                        (int)
                            (stbir__resize_arbitrary(alloc_context, input_pixels, (int)(input_w), (int)(input_h),
                                (int)(input_stride_in_bytes), output_pixels, (int)(output_w), (int)(output_h), (int)(output_stride_in_bytes),
                                (float)(0), (float)(0), (float)(1), (float)(1), transform, (int)(num_channels), (int)(alpha_channel),
                                (uint)(flags), (int)(datatype), (int)(filter_horizontal), (int)(filter_vertical), (int)(edge_mode_horizontal),
                                (int)(edge_mode_vertical), (int)(space)));
                }

                public static int stbir_resize_region(void* input_pixels, int input_w, int input_h, int input_stride_in_bytes,
                    void* output_pixels, int output_w, int output_h, int output_stride_in_bytes, int datatype, int num_channels,
                    int alpha_channel, int flags, int edge_mode_horizontal, int edge_mode_vertical, int filter_horizontal,
                    int filter_vertical, int space, void* alloc_context, float s0, float t0, float s1, float t1)
                {
                    return
                        (int)
                            (stbir__resize_arbitrary(alloc_context, input_pixels, (int)(input_w), (int)(input_h),
                                (int)(input_stride_in_bytes), output_pixels, (int)(output_w), (int)(output_h), (int)(output_stride_in_bytes),
                                (float)(s0), (float)(t0), (float)(s1), (float)(t1), null, (int)(num_channels), (int)(alpha_channel),
                                (uint)(flags), (int)(datatype), (int)(filter_horizontal), (int)(filter_vertical), (int)(edge_mode_horizontal),
                                (int)(edge_mode_vertical), (int)(space)));
                }
            }
            public static unsafe partial class StbImageWrite
            {
                public static int stbi_write_tga_with_rle = 1;

                public delegate int WriteCallback(void* context, void* data, int size);

                public class stbi__write_context
                {
                    public WriteCallback func;
                    public void* context;
                }

                public static void stbi__start_write_callbacks(stbi__write_context s, WriteCallback c, void* context)
                {
                    s.func = c;
                    s.context = context;
                }

                public static void stbiw__writefv(stbi__write_context s, string fmt, params object[] v)
                {
                    var vindex = 0;
                    for (var i = 0; i < fmt.Length; ++i)
                    {
                        var c = fmt[i];
                        switch (c)
                        {
                            case ' ':
                                break;
                            case '1':
                                {
                                    var x = (byte)((int)v[vindex++] & 0xff);
                                    s.func(s.context, &x, 1);
                                    break;
                                }
                            case '2':
                                {
                                    var x = (int)v[vindex++];
                                    var b = stackalloc byte[2];
                                    b[0] = (byte)(x & 0xff);
                                    b[1] = (byte)((x >> 8) & 0xff);
                                    s.func(s.context, b, 2);
                                    break;
                                }
                            case '4':
                                {
                                    var x = (int)v[vindex++];
                                    var b = stackalloc byte[4];
                                    b[0] = (byte)(x & 0xff);
                                    b[1] = (byte)((x >> 8) & 0xff);
                                    b[2] = (byte)((x >> 16) & 0xff);
                                    b[3] = (byte)((x >> 24) & 0xff);
                                    s.func(s.context, b, 4);
                                    break;
                                }
                        }
                    }
                }

                public static void stbiw__writef(stbi__write_context s, string fmt, params object[] v)
                {
                    stbiw__writefv(s, fmt, v);
                }

                public static int stbiw__outfile(stbi__write_context s, int rgb_dir, int vdir, int x, int y, int comp,
                    int expand_mono, void* data, int alpha, int pad, string fmt, params object[] v)
                {
                    if ((y < 0) || (x < 0))
                    {
                        return 0;
                    }

                    stbiw__writefv(s, fmt, v);
                    stbiw__write_pixels(s, rgb_dir, vdir, x, y, comp, data, alpha, pad, expand_mono);
                    return 1;
                }

                public static int stbi_write_bmp_to_func(WriteCallback func,
                    void* context,
                    int x,
                    int y,
                    int comp,
                    void* data
                    )
                {
                    var s = new stbi__write_context();
                    stbi__start_write_callbacks(s, func, context);
                    return stbi_write_bmp_core(s, x, y, comp, data);
                }

                public static int stbi_write_tga_to_func(WriteCallback func,
                    void* context,
                    int x,
                    int y,
                    int comp,
                    void* data
                    )
                {
                    var s = new stbi__write_context();
                    stbi__start_write_callbacks(s, func, context);
                    return stbi_write_tga_core(s, x, y, comp, data);
                }

                public static int stbi_write_hdr_to_func(WriteCallback func,
                    void* context,
                    int x,
                    int y,
                    int comp,
                    float* data
                    )
                {
                    stbi__write_context s = new stbi__write_context();
                    stbi__start_write_callbacks(s, func, context);
                    return stbi_write_hdr_core(s, x, y, comp, data);
                }

                public static int stbi_write_png_to_func(WriteCallback func,
                    void* context,
                    int x,
                    int y,
                    int comp,
                    void* data,
                    int stride_bytes
                    )
                {
                    int len;
                    var png = stbi_write_png_to_mem((byte*)(data), stride_bytes, x, y, comp, &len);
                    if (png == null) return 0;
                    func(context, png, len);
                    CRuntime.free(png);
                    return 1;
                }

                public static int stbi_write_jpg_to_func(WriteCallback func,
                    void* context,
                    int x,
                    int y,
                    int comp,
                    void* data,
                    int quality
                    )
                {
                    stbi__write_context s = new stbi__write_context();
                    stbi__start_write_callbacks(s, func, context);
                    return stbi_write_jpg_core(s, x, y, comp, data, quality);
                }

                public static int stbi_write_hdr_core(stbi__write_context s, int x, int y, int comp, float* data)
                {
                    if ((y <= 0) || (x <= 0) || (data == null))
                    {
                        return 0;
                    }

                    var scratch = (byte*)(CRuntime.malloc((ulong)(x * 4)));

                    int i;
                    var header = "#?RADIANCE\n# Written by stb_image_write.h\nFORMAT=32-bit_rle_rgbe\n";
                    var bytes = Encoding.UTF8.GetBytes(header);
                    fixed (byte* ptr = bytes)
                    {
                        s.func(s.context, ((sbyte*)ptr), bytes.Length);
                    }

                    var str = string.Format("EXPOSURE=          1.0000000000000\n\n-Y {0} +X {1}\n", y, x);
                    bytes = Encoding.UTF8.GetBytes(str);
                    fixed (byte* ptr = bytes)
                    {
                        s.func(s.context, ((sbyte*)ptr), bytes.Length);
                    }
                    for (i = 0; i < y; i++)
                    {
                        stbiw__write_hdr_scanline(s, x, comp, scratch, data + comp * i * x);
                    }
                    CRuntime.free(scratch);
                    return 1;
                }
            }
            unsafe partial class StbImageWrite
            {
                public static ushort[] lengthc =
                {
            3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27, 31, 35, 43, 51, 59, 67, 83, 99,
            115, 131, 163, 195, 227, 258, 259
        };

                public static byte[] lengtheb =
                {
            0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0
        };

                public static ushort[] distc =
                {
            1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193, 257, 385, 513, 769, 1025,
            1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577, 32768
        };

                public static byte[] disteb =
                {
            0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12,
            12, 13, 13
        };

                public static uint[] crc_table =
                {
            0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419, 0x706AF48F, 0xE963A535,
            0x9E6495A3, 0x0eDB8832, 0x79DCB8A4, 0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07, 0x90BF1D91,
            0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE, 0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7, 0x136C9856,
            0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9, 0xFA0F3D63, 0x8D080DF5, 0x3B6E20C8, 0x4C69105E,
            0xD56041E4, 0xA2677172, 0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B, 0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6,
            0xACBCF940, 0x32D86CE3, 0x45DF5C75, 0xDCD60DCF, 0xABD13D59, 0x26D930AC, 0x51DE003A, 0xC8D75180, 0xBFD06116,
            0x21B4F4B5, 0x56B3C423, 0xCFBA9599, 0xB8BDA50F, 0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924, 0x2F6F7C87,
            0x58684C11, 0xC1611DAB, 0xB6662D3D, 0x76DC4190, 0x01DB7106, 0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F,
            0x9FBFE4A5, 0xE8B8D433, 0x7807C9A2, 0x0F00F934, 0x9609A88E, 0xE10E9818, 0x7F6A0DBB, 0x086D3D2D, 0x91646C97,
            0xE6635C01, 0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED, 0x1B01A57B, 0x8208F4C1, 0xF50FC457,
            0x65B0D9C6, 0x12B7E950, 0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3, 0xFBD44C65, 0x4DB26158,
            0x3AB551CE, 0xA3BC0074, 0xD4BB30E2, 0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB, 0x4369E96A, 0x346ED9FC,
            0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5, 0xAA0A4C5F, 0xDD0D7CC9, 0x5005713C, 0x270241AA, 0xBE0B1010,
            0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F, 0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4,
            0x59B33D17, 0x2EB40D81, 0xB7BD5C3B, 0xC0BA6CAD, 0xEDB88320, 0x9ABFB3B6, 0x03B6E20C, 0x74B1D29A, 0xEAD54739,
            0x9DD277AF, 0x04DB2615, 0x73DC1683, 0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8, 0xE40ECF0B, 0x9309FF9D,
            0x0A00AE27, 0x7D079EB1, 0xF00F9344, 0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB, 0x196C3671,
            0x6E6B06E7, 0xFED41B76, 0x89D32BE0, 0x10DA7A5A, 0x67DD4ACC, 0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
            0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1, 0xA6BC5767, 0x3FB506DD, 0x48B2364B, 0xD80D2BDA,
            0xAF0A1B4C, 0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF, 0x4669BE79, 0xCB61B38C, 0xBC66831A,
            0x256FD2A0, 0x5268E236, 0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F, 0xC5BA3BBE, 0xB2BD0B28, 0x2BB45A92,
            0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31, 0x2CD99E8B, 0x5BDEAE1D, 0x9B64C2B0, 0xEC63F226, 0x756AA39C, 0x026D930A,
            0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713, 0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B,
            0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21, 0x86D3D2D4, 0xF1D4E242, 0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B,
            0x6FB077E1, 0x18B74777, 0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C, 0x8F659EFF, 0xF862AE69, 0x616BFFD3,
            0x166CCF45, 0xA00AE278, 0xD70DD2EE, 0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7, 0x4969474D, 0x3E6E77DB,
            0xAED16A4A, 0xD9D65ADC, 0x40DF0B66, 0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9, 0xBDBDF21C,
            0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605, 0xCDD70693, 0x54DE5729, 0x23D967BF, 0xB3667A2E, 0xC4614AB8,
            0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B, 0x2D02EF8D
        };

                public static byte[] stbiw__jpg_ZigZag =
                {
            0, 1, 5, 6, 14, 15, 27, 28, 2, 4, 7, 13, 16, 26, 29, 42, 3, 8, 12, 17, 25,
            30, 41, 43, 9, 11, 18, 24, 31, 40, 44, 53, 10, 19, 23, 32, 39, 45, 52, 54, 20, 22, 33, 38, 46, 51, 55, 60, 21, 34, 37,
            47, 50, 56, 59, 61, 35, 36, 48, 49, 57, 58, 62, 63
        };

                public static byte[] std_dc_luminance_nrcodes = { 0, 0, 1, 5, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 };
                public static byte[] std_dc_luminance_values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
                public static byte[] std_ac_luminance_nrcodes = { 0, 0, 2, 1, 3, 3, 2, 4, 3, 5, 5, 4, 4, 0, 0, 1, 0x7d };

                public static byte[] std_ac_luminance_values =
                {
            0x01, 0x02, 0x03, 0x00, 0x04, 0x11, 0x05, 0x12, 0x21, 0x31, 0x41, 0x06,
            0x13, 0x51, 0x61, 0x07, 0x22, 0x71, 0x14, 0x32, 0x81, 0x91, 0xa1, 0x08, 0x23, 0x42, 0xb1, 0xc1, 0x15, 0x52, 0xd1,
            0xf0, 0x24, 0x33, 0x62, 0x72, 0x82, 0x09, 0x0a, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2a,
            0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x53, 0x54, 0x55, 0x56,
            0x57, 0x58, 0x59, 0x5a, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79,
            0x7a, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9a, 0xa2,
            0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 0xaa, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xc2, 0xc3,
            0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xe1, 0xe2, 0xe3,
            0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 0xfa
        };

                public static byte[] std_dc_chrominance_nrcodes = { 0, 0, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0 };
                public static byte[] std_dc_chrominance_values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
                public static byte[] std_ac_chrominance_nrcodes = { 0, 0, 2, 1, 2, 4, 4, 3, 4, 7, 5, 4, 4, 0, 1, 2, 0x77 };

                public static byte[] std_ac_chrominance_values =
                {
            0x00, 0x01, 0x02, 0x03, 0x11, 0x04, 0x05, 0x21, 0x31, 0x06, 0x12,
            0x41, 0x51, 0x07, 0x61, 0x71, 0x13, 0x22, 0x32, 0x81, 0x08, 0x14, 0x42, 0x91, 0xa1, 0xb1, 0xc1, 0x09, 0x23, 0x33,
            0x52, 0xf0, 0x15, 0x62, 0x72, 0xd1, 0x0a, 0x16, 0x24, 0x34, 0xe1, 0x25, 0xf1, 0x17, 0x18, 0x19, 0x1a, 0x26, 0x27,
            0x28, 0x29, 0x2a, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4a, 0x53, 0x54,
            0x55, 0x56, 0x57, 0x58, 0x59, 0x5a, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x73, 0x74, 0x75, 0x76, 0x77,
            0x78, 0x79, 0x7a, 0x82, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98,
            0x99, 0x9a, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 0xaa, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9,
            0xba, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 0xc8, 0xc9, 0xca, 0xd2, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda,
            0xe2, 0xe3, 0xe4, 0xe5, 0xe6, 0xe7, 0xe8, 0xe9, 0xea, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 0xfa
        };

                public static ushort[,] YDC_HT =
                {
            {0, 2}, {2, 3}, {3, 3}, {4, 3}, {5, 3}, {6, 3}, {14, 4}, {30, 5}, {62, 6}, {126, 7},
            {254, 8}, {510, 9}
        };

                public static ushort[,] UVDC_HT =
                {
            {0, 2}, {1, 2}, {2, 2}, {6, 3}, {14, 4}, {30, 5}, {62, 6}, {126, 7}, {254, 8},
            {510, 9}, {1022, 10}, {2046, 11}
        };

                public static ushort[,] YAC_HT =
                {
            {10, 4}, {0, 2}, {1, 2}, {4, 3}, {11, 4}, {26, 5}, {120, 7}, {248, 8}, {1014, 10},
            {65410, 16}, {65411, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {12, 4}, {27, 5}, {121, 7}, {502, 9},
            {2038, 11}, {65412, 16}, {65413, 16}, {65414, 16}, {65415, 16}, {65416, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0},
            {0, 0}, {28, 5}, {249, 8}, {1015, 10}, {4084, 12}, {65417, 16}, {65418, 16}, {65419, 16}, {65420, 16}, {65421, 16},
            {65422, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {58, 6}, {503, 9}, {4085, 12}, {65423, 16}, {65424, 16},
            {65425, 16}, {65426, 16}, {65427, 16}, {65428, 16}, {65429, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0},
            {59, 6}, {1016, 10}, {65430, 16}, {65431, 16}, {65432, 16}, {65433, 16}, {65434, 16}, {65435, 16}, {65436, 16},
            {65437, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {122, 7}, {2039, 11}, {65438, 16}, {65439, 16},
            {65440, 16}, {65441, 16}, {65442, 16}, {65443, 16}, {65444, 16}, {65445, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0},
            {0, 0}, {123, 7}, {4086, 12}, {65446, 16}, {65447, 16}, {65448, 16}, {65449, 16}, {65450, 16}, {65451, 16},
            {65452, 16}, {65453, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {250, 8}, {4087, 12}, {65454, 16},
            {65455, 16}, {65456, 16}, {65457, 16}, {65458, 16}, {65459, 16}, {65460, 16}, {65461, 16}, {0, 0}, {0, 0}, {0, 0},
            {0, 0}, {0, 0}, {0, 0}, {504, 9}, {32704, 15}, {65462, 16}, {65463, 16}, {65464, 16}, {65465, 16}, {65466, 16},
            {65467, 16}, {65468, 16}, {65469, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {505, 9}, {65470, 16},
            {65471, 16}, {65472, 16}, {65473, 16}, {65474, 16}, {65475, 16}, {65476, 16}, {65477, 16}, {65478, 16}, {0, 0},
            {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {506, 9}, {65479, 16}, {65480, 16}, {65481, 16}, {65482, 16}, {65483, 16},
            {65484, 16}, {65485, 16}, {65486, 16}, {65487, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {1017, 10},
            {65488, 16}, {65489, 16}, {65490, 16}, {65491, 16}, {65492, 16}, {65493, 16}, {65494, 16}, {65495, 16}, {65496, 16},
            {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {1018, 10}, {65497, 16}, {65498, 16}, {65499, 16}, {65500, 16},
            {65501, 16}, {65502, 16}, {65503, 16}, {65504, 16}, {65505, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0},
            {2040, 11}, {65506, 16}, {65507, 16}, {65508, 16}, {65509, 16}, {65510, 16}, {65511, 16}, {65512, 16}, {65513, 16},
            {65514, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {65515, 16}, {65516, 16}, {65517, 16}, {65518, 16},
            {65519, 16}, {65520, 16}, {65521, 16}, {65522, 16}, {65523, 16}, {65524, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0},
            {2041, 11}, {65525, 16}, {65526, 16}, {65527, 16}, {65528, 16}, {65529, 16}, {65530, 16}, {65531, 16}, {65532, 16},
            {65533, 16}, {65534, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}
        };

                public static ushort[,] UVAC_HT =
                {
            {0, 2}, {1, 2}, {4, 3}, {10, 4}, {24, 5}, {25, 5}, {56, 6}, {120, 7}, {500, 9},
            {1014, 10}, {4084, 12}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {11, 4}, {57, 6}, {246, 8}, {501, 9},
            {2038, 11}, {4085, 12}, {65416, 16}, {65417, 16}, {65418, 16}, {65419, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0},
            {0, 0}, {26, 5}, {247, 8}, {1015, 10}, {4086, 12}, {32706, 15}, {65420, 16}, {65421, 16}, {65422, 16}, {65423, 16},
            {65424, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {27, 5}, {248, 8}, {1016, 10}, {4087, 12}, {65425, 16},
            {65426, 16}, {65427, 16}, {65428, 16}, {65429, 16}, {65430, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0},
            {58, 6}, {502, 9}, {65431, 16}, {65432, 16}, {65433, 16}, {65434, 16}, {65435, 16}, {65436, 16}, {65437, 16},
            {65438, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {59, 6}, {1017, 10}, {65439, 16}, {65440, 16},
            {65441, 16}, {65442, 16}, {65443, 16}, {65444, 16}, {65445, 16}, {65446, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0},
            {0, 0}, {121, 7}, {2039, 11}, {65447, 16}, {65448, 16}, {65449, 16}, {65450, 16}, {65451, 16}, {65452, 16},
            {65453, 16}, {65454, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {122, 7}, {2040, 11}, {65455, 16},
            {65456, 16}, {65457, 16}, {65458, 16}, {65459, 16}, {65460, 16}, {65461, 16}, {65462, 16}, {0, 0}, {0, 0}, {0, 0},
            {0, 0}, {0, 0}, {0, 0}, {249, 8}, {65463, 16}, {65464, 16}, {65465, 16}, {65466, 16}, {65467, 16}, {65468, 16},
            {65469, 16}, {65470, 16}, {65471, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {503, 9}, {65472, 16},
            {65473, 16}, {65474, 16}, {65475, 16}, {65476, 16}, {65477, 16}, {65478, 16}, {65479, 16}, {65480, 16}, {0, 0},
            {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {504, 9}, {65481, 16}, {65482, 16}, {65483, 16}, {65484, 16}, {65485, 16},
            {65486, 16}, {65487, 16}, {65488, 16}, {65489, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {505, 9},
            {65490, 16}, {65491, 16}, {65492, 16}, {65493, 16}, {65494, 16}, {65495, 16}, {65496, 16}, {65497, 16}, {65498, 16},
            {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {506, 9}, {65499, 16}, {65500, 16}, {65501, 16}, {65502, 16},
            {65503, 16}, {65504, 16}, {65505, 16}, {65506, 16}, {65507, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0},
            {2041, 11}, {65508, 16}, {65509, 16}, {65510, 16}, {65511, 16}, {65512, 16}, {65513, 16}, {65514, 16}, {65515, 16},
            {65516, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {16352, 14}, {65517, 16}, {65518, 16}, {65519, 16},
            {65520, 16}, {65521, 16}, {65522, 16}, {65523, 16}, {65524, 16}, {65525, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0},
            {1018, 10}, {32707, 15}, {65526, 16}, {65527, 16}, {65528, 16}, {65529, 16}, {65530, 16}, {65531, 16}, {65532, 16},
            {65533, 16}, {65534, 16}, {0, 0}, {0, 0}, {0, 0}, {0, 0}, {0, 0}
        };

                public static int[] YQT =
                {
            16, 11, 10, 16, 24, 40, 51, 61, 12, 12, 14, 19, 26, 58, 60, 55, 14, 13, 16, 24, 40, 57, 69,
            56, 14, 17, 22, 29, 51, 87, 80, 62, 18, 22, 37, 56, 68, 109, 103, 77, 24, 35, 55, 64, 81, 104, 113, 92, 49, 64, 78,
            87, 103, 121, 120, 101, 72, 92, 95, 98, 112, 100, 103, 99
        };

                public static int[] UVQT =
                {
            17, 18, 24, 47, 99, 99, 99, 99, 18, 21, 26, 66, 99, 99, 99, 99, 24, 26, 56, 99, 99, 99, 99,
            99, 47, 66, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99,
            99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99
        };

                public static float[] aasf =
                {
            1.0f*2.828427125f, 1.387039845f*2.828427125f, 1.306562965f*2.828427125f,
            1.175875602f*2.828427125f, 1.0f*2.828427125f, 0.785694958f*2.828427125f, 0.541196100f*2.828427125f,
            0.275899379f*2.828427125f
        };

                public static byte[] head0 =
                {
            0xFF, 0xD8, 0xFF, 0xE0, 0, 0x10, (byte) ('J'), (byte) ('F'), (byte) ('I'), (byte) ('F'),
            0, 1, 1, 0, 0, 1, 0, 1, 0, 0, 0xFF, 0xDB, 0, 0x84, 0
        };

                public static byte[] head2 = { 0xFF, 0xDA, 0, 0xC, 3, 1, 0, 2, 0x11, 3, 0x11, 0, 0x3F, 0 };

                public static void stbiw__putc(stbi__write_context s, byte c)
                {
                    s.func(s.context, &c, (int)(1));
                }

                public static void stbiw__write3(stbi__write_context s, byte a, byte b, byte c, byte d)
                {
                    if (d == 0)
                    {
                        byte* arr = stackalloc byte[3];
                        arr[0] = a;
                        arr[1] = b;
                        arr[2] = c;
                        s.func(s.context, arr, 3);
                    }
                    else
                    {
                        byte* arr = stackalloc byte[4];
                        arr[0] = a;
                        arr[1] = b;
                        arr[2] = c;
                        arr[3] = d;
                        s.func(s.context, arr, 4);
                    }
                }
                public static void stbiw__write_pixel(stbi__write_context s, int rgb_dir, int comp, int write_alpha, int expand_mono,
                    byte* d)
                {
                    byte* bg = stackalloc byte[3];
                    bg[0] = 0;
                    bg[1] = 0;
                    bg[2] = 0;

                    byte* px = stackalloc byte[3];
                    int k;
                    if ((write_alpha) < (0)) s.func(s.context, &d[comp - 1], (int)(1));
                    switch (comp)
                    {
                        case 1:
                        case 2:
                            if ((expand_mono) != 0) stbiw__write3(s, (byte)(d[0]), (byte)(d[0]), (byte)(d[0]), 0);
                            else s.func(s.context, d, (int)(1));
                            break;
                        case 3:
                        case 4:
                            if (comp == 4 && write_alpha == 0)
                            {
                                for (k = 0; (k) < 3; ++k)
                                {
                                    px[k] = (byte)(bg[k] + ((d[k] - bg[k]) * d[3]) / 255);
                                }
                                stbiw__write3(s, (px[1 + rgb_dir]), (px[1]), (px[1 - rgb_dir]), 0);
                                break;
                            }
                            stbiw__write3(s, (byte)(d[1 + rgb_dir]), (byte)(d[1]), (byte)(d[1 - rgb_dir]), 0);
                            break;
                    }

                    if ((write_alpha) > (0))
                        s.func(s.context, &d[comp - 1], (int)(1));
                }

                public static void stbiw__write_pixels(stbi__write_context s, int rgb_dir, int vdir, int x, int y, int comp,
                    void* data, int write_alpha, int scanline_pad, int expand_mono)
                {
                    uint zero = (uint)(0);
                    int i;
                    int j;
                    int j_end;
                    if (y <= 0) return;
                    if ((vdir) < (0))
                    {
                        j_end = (int)(-1);
                        j = (int)(y - 1);
                    }
                    else
                    {
                        j_end = (int)(y);
                        j = (int)(0);
                    }

                    for (; j != j_end; j += (int)(vdir))
                    {
                        for (i = (int)(0); (i) < (x); ++i)
                        {
                            byte* d = (byte*)(data) + (j * x + i) * comp;
                            stbiw__write_pixel(s, (int)(rgb_dir), (int)(comp), (int)(write_alpha), (int)(expand_mono), d);
                        }
                        s.func(s.context, &zero, (int)(scanline_pad));
                    }
                }

                public static int stbi_write_bmp_core(stbi__write_context s, int x, int y, int comp, void* data)
                {
                    int pad = (int)((-x * 3) & 3);
                    return
                        (int)
                            (stbiw__outfile(s, (int)(-1), (int)(-1), (int)(x), (int)(y), (int)(comp), (int)(1), data, (int)(0),
                                (int)(pad), "11 4 22 44 44 22 444444", (int)('B'), (int)('M'), (int)(14 + 40 + (x * 3 + pad) * y), (int)(0),
                                (int)(0), (int)(14 + 40), (int)(40), (int)(x), (int)(y), (int)(1), (int)(24), (int)(0), (int)(0),
                                (int)(0), (int)(0), (int)(0), (int)(0)));
                }

                public static int stbi_write_tga_core(stbi__write_context s, int x, int y, int comp, void* data)
                {
                    int has_alpha = (((comp) == (2)) || ((comp) == (4))) ? 1 : 0;
                    int colourbytes = (int)((has_alpha) != 0 ? comp - 1 : comp);
                    int format = (int)((colourbytes) < (2) ? 3 : 2);
                    if (((y) < (0)) || ((x) < (0))) return (int)(0);
                    if (stbi_write_tga_with_rle == 0)
                    {
                        return
                            (int)
                                (stbiw__outfile(s, (int)(-1), (int)(-1), (int)(x), (int)(y), (int)(comp), (int)(0), data, (int)(has_alpha),
                                    (int)(0), "111 221 2222 11", (int)(0), (int)(0), (int)(format), (int)(0), (int)(0), (int)(0), (int)(0),
                                    (int)(0), (int)(x), (int)(y), (int)((colourbytes + has_alpha) * 8), (int)(has_alpha * 8)));
                    }
                    else
                    {
                        int i;
                        int j;
                        int k;
                        stbiw__writef(s, "111 221 2222 11", (int)(0), (int)(0), (int)(format + 8), (int)(0), (int)(0), (int)(0),
                            (int)(0), (int)(0), (int)(x), (int)(y), (int)((colourbytes + has_alpha) * 8), (int)(has_alpha * 8));
                        for (j = (int)(y - 1); (j) >= (0); --j)
                        {
                            byte* row = (byte*)(data) + j * x * comp;
                            int len;
                            for (i = (int)(0); (i) < (x); i += (int)(len))
                            {
                                byte* begin = row + i * comp;
                                int diff = (int)(1);
                                len = (int)(1);
                                if ((i) < (x - 1))
                                {
                                    ++len;
                                    diff = (int)(CRuntime.memcmp(begin, row + (i + 1) * comp, (ulong)(comp)));
                                    if ((diff) != 0)
                                    {
                                        byte* prev = begin;
                                        for (k = (int)(i + 2); ((k) < (x)) && ((len) < (128)); ++k)
                                        {
                                            if ((CRuntime.memcmp(prev, row + k * comp, (ulong)(comp))) != 0)
                                            {
                                                prev += comp;
                                                ++len;
                                            }
                                            else
                                            {
                                                --len;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (k = (int)(i + 2); ((k) < (x)) && ((len) < (128)); ++k)
                                        {
                                            if (CRuntime.memcmp(begin, row + k * comp, (ulong)(comp)) == 0)
                                            {
                                                ++len;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                if ((diff) != 0)
                                {
                                    byte header = (byte)((len - 1) & 0xff);
                                    s.func(s.context, &header, (int)(1));
                                    for (k = (int)(0); (k) < (len); ++k)
                                    {
                                        stbiw__write_pixel(s, (int)(-1), (int)(comp), (int)(has_alpha), (int)(0), begin + k * comp);
                                    }
                                }
                                else
                                {
                                    byte header = (byte)((len - 129) & 0xff);
                                    s.func(s.context, &header, (int)(1));
                                    stbiw__write_pixel(s, (int)(-1), (int)(comp), (int)(has_alpha), (int)(0), begin);
                                }
                            }
                        }
                    }

                    return (int)(1);
                }

                public static void stbiw__linear_to_rgbe(byte* rgbe, float* linear)
                {
                    int exponent;
                    float maxcomp =
                        (float)
                            ((linear[0]) > ((linear[1]) > (linear[2]) ? (linear[1]) : (linear[2]))
                                ? (linear[0])
                                : ((linear[1]) > (linear[2]) ? (linear[1]) : (linear[2])));
                    if ((maxcomp) < (1e-32f))
                    {
                        rgbe[0] = (byte)(rgbe[1] = (byte)(rgbe[2] = (byte)(rgbe[3] = (byte)(0))));
                    }
                    else
                    {
                        float normalize = (float)((float)(CRuntime.frexp((double)(maxcomp), &exponent)) * 256.0f / maxcomp);
                        rgbe[0] = ((byte)(linear[0] * normalize));
                        rgbe[1] = ((byte)(linear[1] * normalize));
                        rgbe[2] = ((byte)(linear[2] * normalize));
                        rgbe[3] = ((byte)(exponent + 128));
                    }

                }

                public static void stbiw__write_run_data(stbi__write_context s, int length, byte databyte)
                {
                    byte lengthbyte = (byte)((length + 128) & 0xff);
                    s.func(s.context, &lengthbyte, (int)(1));
                    s.func(s.context, &databyte, (int)(1));
                }

                public static void stbiw__write_dump_data(stbi__write_context s, int length, byte* data)
                {
                    byte lengthbyte = (byte)((length) & 0xff);
                    s.func(s.context, &lengthbyte, (int)(1));
                    s.func(s.context, data, (int)(length));
                }

                public static void stbiw__write_hdr_scanline(stbi__write_context s, int width, int ncomp, byte* scratch,
                    float* scanline)
                {
                    byte* scanlineheader = stackalloc byte[4];
                    scanlineheader[0] = (byte)(2);
                    scanlineheader[1] = (byte)(2);
                    scanlineheader[2] = (byte)(0);
                    scanlineheader[3] = (byte)(0);

                    byte* rgbe = stackalloc byte[4];
                    float* linear = stackalloc float[3];
                    int x;
                    scanlineheader[2] = (byte)((width & 0xff00) >> 8);
                    scanlineheader[3] = (byte)(width & 0x00ff);
                    if (((width) < (8)) || ((width) >= (32768)))
                    {
                        for (x = (int)(0); (x) < (width); x++)
                        {
                            switch (ncomp)
                            {
                                case 4:
                                case 3:
                                    linear[2] = (float)(scanline[x * ncomp + 2]);
                                    linear[1] = (float)(scanline[x * ncomp + 1]);
                                    linear[0] = (float)(scanline[x * ncomp + 0]);
                                    break;
                                default:
                                    linear[0] = (float)(linear[1] = (float)(linear[2] = (float)(scanline[x * ncomp + 0])));
                                    break;
                            }
                            stbiw__linear_to_rgbe(rgbe, linear);
                            s.func(s.context, rgbe, (int)(4));
                        }
                    }
                    else
                    {
                        int c;
                        int r;
                        for (x = (int)(0); (x) < (width); x++)
                        {
                            switch (ncomp)
                            {
                                case 4:
                                case 3:
                                    linear[2] = (float)(scanline[x * ncomp + 2]);
                                    linear[1] = (float)(scanline[x * ncomp + 1]);
                                    linear[0] = (float)(scanline[x * ncomp + 0]);
                                    break;
                                default:
                                    linear[0] = (float)(linear[1] = (float)(linear[2] = (float)(scanline[x * ncomp + 0])));
                                    break;
                            }
                            stbiw__linear_to_rgbe(rgbe, linear);
                            scratch[x + width * 0] = (byte)(rgbe[0]);
                            scratch[x + width * 1] = (byte)(rgbe[1]);
                            scratch[x + width * 2] = (byte)(rgbe[2]);
                            scratch[x + width * 3] = (byte)(rgbe[3]);
                        }
                        s.func(s.context, scanlineheader, (int)(4));
                        for (c = (int)(0); (c) < (4); c++)
                        {
                            byte* comp = &scratch[width * c];
                            x = (int)(0);
                            while ((x) < (width))
                            {
                                r = (int)(x);
                                while ((r + 2) < (width))
                                {
                                    if (((comp[r]) == (comp[r + 1])) && ((comp[r]) == (comp[r + 2]))) break;
                                    ++r;
                                }
                                if ((r + 2) >= (width)) r = (int)(width);
                                while ((x) < (r))
                                {
                                    int len = (int)(r - x);
                                    if ((len) > (128)) len = (int)(128);
                                    stbiw__write_dump_data(s, (int)(len), &comp[x]);
                                    x += (int)(len);
                                }
                                if ((r + 2) < (width))
                                {
                                    while (((r) < (width)) && ((comp[r]) == (comp[x])))
                                    {
                                        ++r;
                                    }
                                    while ((x) < (r))
                                    {
                                        int len = (int)(r - x);
                                        if ((len) > (127)) len = (int)(127);
                                        stbiw__write_run_data(s, (int)(len), (byte)(comp[x]));
                                        x += (int)(len);
                                    }
                                }
                            }
                        }
                    }

                }

                public static void* stbiw__sbgrowf(void** arr, int increment, int itemsize)
                {
                    int m = (int)(*arr != null ? 2 * ((int*)(*arr) - 2)[0] + increment : increment + 1);
                    void* p = CRuntime.realloc(*arr != null ? ((int*)(*arr) - 2) : ((int*)(0)), (ulong)(itemsize * m + sizeof(int) * 2));
                    if ((p) != null)
                    {
                        if (*arr == null) ((int*)(p))[1] = (int)(0);
                        *arr = (void*)((int*)(p) + 2);
                        ((int*)(*arr) - 2)[0] = (int)(m);
                    }

                    return *arr;
                }

                public static byte* stbiw__zlib_flushf(byte* data, uint* bitbuffer, int* bitcount)
                {
                    while ((*bitcount) >= (8))
                    {
                        if ((((data) == null) || ((((int*)(data) - 2)[1] + (1)) >= (((int*)(data) - 2)[0]))))
                        {
                            stbiw__sbgrowf((void**)(&(data)), (int)(1), sizeof(byte));
                        }
                        (data)[((int*)(data) - 2)[1]++] = ((byte)((*bitbuffer) & 0xff));
                        *bitbuffer >>= 8;
                        *bitcount -= (int)(8);
                    }
                    return data;
                }

                public static int stbiw__zlib_bitrev(int code, int codebits)
                {
                    int res = (int)(0);
                    while ((codebits--) != 0)
                    {
                        res = (int)((res << 1) | (code & 1));
                        code >>= 1;
                    }
                    return (int)(res);
                }

                public static uint stbiw__zlib_countm(byte* a, byte* b, int limit)
                {
                    int i;
                    for (i = (int)(0); ((i) < (limit)) && ((i) < (258)); ++i)
                    {
                        if (a[i] != b[i]) break;
                    }
                    return (uint)(i);
                }

                public static uint stbiw__zhash(byte* data)
                {
                    uint hash = (uint)(data[0] + (data[1] << 8) + (data[2] << 16));
                    hash ^= (uint)(hash << 3);
                    hash += (uint)(hash >> 5);
                    hash ^= (uint)(hash << 4);
                    hash += (uint)(hash >> 17);
                    hash ^= (uint)(hash << 25);
                    hash += (uint)(hash >> 6);
                    return (uint)(hash);
                }

                public static byte* stbi_zlib_compress(byte* data, int data_len, int* out_len, int quality)
                {
                    uint bitbuf = (uint)(0);
                    int i;
                    int j;
                    int bitcount = (int)(0);
                    byte* _out_ = null;
                    byte*** hash_table = (byte***)(CRuntime.malloc((ulong)(16384 * sizeof(byte**))));
                    if ((quality) < (5)) quality = (int)(5);
                    if ((((_out_) == null) || ((((int*)(_out_) - 2)[1] + (1)) >= (((int*)(_out_) - 2)[0]))))
                    {
                        stbiw__sbgrowf((void**)(&(_out_)), (int)(1), sizeof(byte));
                    }

                    (_out_)[((int*)(_out_) - 2)[1]++] = (byte)(0x78);
                    if ((((_out_) == null) || ((((int*)(_out_) - 2)[1] + (1)) >= (((int*)(_out_) - 2)[0]))))
                    {
                        stbiw__sbgrowf((void**)(&(_out_)), (int)(1), sizeof(byte));
                    }

                    (_out_)[((int*)(_out_) - 2)[1]++] = (byte)(0x5e);
                    {
                        bitbuf |= (uint)((1) << bitcount);
                        bitcount += (int)(1);
                        _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                    }

                    {
                        bitbuf |= (uint)((1) << bitcount);
                        bitcount += (int)(2);
                        _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                    }

                    for (i = (int)(0); (i) < (16384); ++i)
                    {
                        hash_table[i] = null;
                    }
                    i = (int)(0);
                    while ((i) < (data_len - 3))
                    {
                        int h = (int)(stbiw__zhash(data + i) & (16384 - 1));
                        int best = (int)(3);
                        byte* bestloc = null;
                        byte** hlist = hash_table[h];
                        int n = (int)(hlist != null ? ((int*)(hlist) - 2)[1] : 0);
                        for (j = (int)(0); (j) < (n); ++j)
                        {
                            if ((hlist[j] - data) > (i - 32768))
                            {
                                int d = (int)(stbiw__zlib_countm(hlist[j], data + i, (int)(data_len - i)));
                                if ((d) >= (best))
                                {
                                    best = (int)(d);
                                    bestloc = hlist[j];
                                }
                            }
                        }
                        if (((hash_table[h]) != null) && ((((int*)(hash_table[h]) - 2)[1]) == (2 * quality)))
                        {
                            CRuntime.memmove(hash_table[h], hash_table[h] + quality, (ulong)(sizeof(byte*) * quality));
                            ((int*)(hash_table[h]) - 2)[1] = (int)(quality);
                        }
                        if ((((hash_table[h]) == null) || ((((int*)(hash_table[h]) - 2)[1] + (1)) >= (((int*)(hash_table[h]) - 2)[0]))))
                        {
                            stbiw__sbgrowf((void**)(&(hash_table[h])), (int)(1), sizeof(byte*));
                        }
                        (hash_table[h])[((int*)(hash_table[h]) - 2)[1]++] = (data + i);
                        if ((bestloc) != null)
                        {
                            h = (int)(stbiw__zhash(data + i + 1) & (16384 - 1));
                            hlist = hash_table[h];
                            n = (int)(hlist != null ? ((int*)(hlist) - 2)[1] : 0);
                            for (j = (int)(0); (j) < (n); ++j)
                            {
                                if ((hlist[j] - data) > (i - 32767))
                                {
                                    int e = (int)(stbiw__zlib_countm(hlist[j], data + i + 1, (int)(data_len - i - 1)));
                                    if ((e) > (best))
                                    {
                                        bestloc = null;
                                        break;
                                    }
                                }
                            }
                        }
                        if ((bestloc) != null)
                        {
                            int d = (int)(data + i - bestloc);
                            for (j = (int)(0); (best) > (lengthc[j + 1] - 1); ++j)
                            {
                            }
                            if (j + 257 <= 143)
                            {
                                bitbuf |= (uint)((stbiw__zlib_bitrev((int)(0x30 + (j + 257)), (int)(8))) << bitcount);
                                bitcount += (int)(8);
                                _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                            }
                            else if (j + 257 <= 255)
                            {
                                bitbuf |= (uint)((stbiw__zlib_bitrev((int)(0x190 + (j + 257) - 144), (int)(9))) << bitcount);
                                bitcount += (int)(9);
                                _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                            }
                            else if (j + 257 <= 279)
                            {
                                bitbuf |= (uint)((stbiw__zlib_bitrev((int)(0 + (j + 257) - 256), (int)(7))) << bitcount);
                                bitcount += (int)(7);
                                _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                            }
                            else
                            {
                                bitbuf |= (uint)((stbiw__zlib_bitrev((int)(0xc0 + (j + 257) - 280), (int)(8))) << bitcount);
                                bitcount += (int)(8);
                                _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                            }
                            if ((lengtheb[j]) != 0)
                            {
                                bitbuf |= (uint)((best - lengthc[j]) << bitcount);
                                bitcount += (int)(lengtheb[j]);
                                _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                            }
                            for (j = (int)(0); (d) > (distc[j + 1] - 1); ++j)
                            {
                            }
                            {
                                bitbuf |= (uint)((stbiw__zlib_bitrev((int)(j), (int)(5))) << bitcount);
                                bitcount += (int)(5);
                                _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                            }
                            if ((disteb[j]) != 0)
                            {
                                bitbuf |= (uint)((d - distc[j]) << bitcount);
                                bitcount += (int)(disteb[j]);
                                _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                            }
                            i += (int)(best);
                        }
                        else
                        {
                            if (data[i] <= 143)
                            {
                                bitbuf |= (uint)((stbiw__zlib_bitrev((int)(0x30 + (data[i])), (int)(8))) << bitcount);
                                bitcount += (int)(8);
                                _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                            }
                            else
                            {
                                bitbuf |= (uint)((stbiw__zlib_bitrev((int)(0x190 + (data[i]) - 144), (int)(9))) << bitcount);
                                bitcount += (int)(9);
                                _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                            }
                            ++i;
                        }
                    }
                    for (; (i) < (data_len); ++i)
                    {
                        if (data[i] <= 143)
                        {
                            bitbuf |= (uint)((stbiw__zlib_bitrev((int)(0x30 + (data[i])), (int)(8))) << bitcount);
                            bitcount += (int)(8);
                            _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                        }
                        else
                        {
                            bitbuf |= (uint)((stbiw__zlib_bitrev((int)(0x190 + (data[i]) - 144), (int)(9))) << bitcount);
                            bitcount += (int)(9);
                            _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                        }
                    }
                    if (256 <= 143)
                    {
                        bitbuf |= (uint)((stbiw__zlib_bitrev((int)(0x30 + (256)), (int)(8))) << bitcount);
                        bitcount += (int)(8);
                        _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                    }
                    else if (256 <= 255)
                    {
                        bitbuf |= (uint)((stbiw__zlib_bitrev((int)(0x190 + (256) - 144), (int)(9))) << bitcount);
                        bitcount += (int)(9);
                        _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                    }
                    else if (256 <= 279)
                    {
                        bitbuf |= (uint)((stbiw__zlib_bitrev((int)(0 + (256) - 256), (int)(7))) << bitcount);
                        bitcount += (int)(7);
                        _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                    }
                    else
                    {
                        bitbuf |= (uint)((stbiw__zlib_bitrev((int)(0xc0 + (256) - 280), (int)(8))) << bitcount);
                        bitcount += (int)(8);
                        _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                    }

                    while ((bitcount) != 0)
                    {
                        bitbuf |= (uint)((0) << bitcount);
                        bitcount += (int)(1);
                        _out_ = stbiw__zlib_flushf(_out_, &bitbuf, &bitcount);
                    }
                    for (i = (int)(0); (i) < (16384); ++i)
                    {
                        if ((hash_table[i]) != null)
                        {
                            CRuntime.free(((int*)(hash_table[i]) - 2));
                        }
                    }
                    CRuntime.free(hash_table);
                    {
                        uint s1 = (uint)(1);
                        uint s2 = (uint)(0);
                        int blocklen = (int)(data_len % 5552);
                        j = (int)(0);
                        while ((j) < (data_len))
                        {
                            for (i = (int)(0); (i) < (blocklen); ++i)
                            {
                                s1 += (uint)(data[j + i]);
                                s2 += (uint)(s1);
                            }
                            s1 %= (uint)(65521);
                            s2 %= (uint)(65521);
                            j += (int)(blocklen);
                            blocklen = (int)(5552);
                        }
                        if ((((_out_) == null) || ((((int*)(_out_) - 2)[1] + (1)) >= (((int*)(_out_) - 2)[0]))))
                        {
                            stbiw__sbgrowf((void**)(&(_out_)), (int)(1), sizeof(byte));
                        }
                        (_out_)[((int*)(_out_) - 2)[1]++] = ((byte)((s2 >> 8) & 0xff));
                        if ((((_out_) == null) || ((((int*)(_out_) - 2)[1] + (1)) >= (((int*)(_out_) - 2)[0]))))
                        {
                            stbiw__sbgrowf((void**)(&(_out_)), (int)(1), sizeof(byte));
                        }
                        (_out_)[((int*)(_out_) - 2)[1]++] = ((byte)((s2) & 0xff));
                        if ((((_out_) == null) || ((((int*)(_out_) - 2)[1] + (1)) >= (((int*)(_out_) - 2)[0]))))
                        {
                            stbiw__sbgrowf((void**)(&(_out_)), (int)(1), sizeof(byte));
                        }
                        (_out_)[((int*)(_out_) - 2)[1]++] = ((byte)((s1 >> 8) & 0xff));
                        if ((((_out_) == null) || ((((int*)(_out_) - 2)[1] + (1)) >= (((int*)(_out_) - 2)[0]))))
                        {
                            stbiw__sbgrowf((void**)(&(_out_)), (int)(1), sizeof(byte));
                        }
                        (_out_)[((int*)(_out_) - 2)[1]++] = ((byte)((s1) & 0xff));
                    }

                    *out_len = (int)(((int*)(_out_) - 2)[1]);
                    CRuntime.memmove(((int*)(_out_) - 2), _out_, (ulong)(*out_len));
                    return (byte*)((int*)(_out_) - 2);
                }

                public static uint stbiw__crc32(byte* buffer, int len)
                {
                    uint crc = (uint)(~0u);
                    int i;
                    for (i = (int)(0); (i) < (len); ++i)
                    {
                        crc = (uint)((crc >> 8) ^ crc_table[buffer[i] ^ (crc & 0xff)]);
                    }
                    return (uint)(~crc);
                }

                public static void stbiw__wpcrc(byte** data, int len)
                {
                    uint crc = (uint)(stbiw__crc32(*data - len - 4, (int)(len + 4)));
                    (*data)[0] = ((byte)(((crc) >> 24) & 0xff));
                    (*data)[1] = ((byte)(((crc) >> 16) & 0xff));
                    (*data)[2] = ((byte)(((crc) >> 8) & 0xff));
                    (*data)[3] = ((byte)((crc) & 0xff));
                    (*data) += 4;
                }

                public static byte stbiw__paeth(int a, int b, int c)
                {
                    int p = (int)(a + b - c);
                    int pa = (int)(CRuntime.abs((int)(p - a)));
                    int pb = (int)(CRuntime.abs((int)(p - b)));
                    int pc = (int)(CRuntime.abs((int)(p - c)));
                    if ((pa <= pb) && (pa <= pc)) return (byte)((a) & 0xff);
                    if (pb <= pc) return (byte)((b) & 0xff);
                    return (byte)((c) & 0xff);
                }

                public static byte* stbi_write_png_to_mem(byte* pixels, int stride_bytes, int x, int y, int n, int* out_len)
                {
                    int* ctype = stackalloc int[5];
                    ctype[0] = (int)(-1);
                    ctype[1] = (int)(0);
                    ctype[2] = (int)(4);
                    ctype[3] = (int)(2);
                    ctype[4] = (int)(6);

                    byte* sig = stackalloc byte[8];
                    sig[0] = (byte)(137);
                    sig[1] = (byte)(80);
                    sig[2] = (byte)(78);
                    sig[3] = (byte)(71);
                    sig[4] = (byte)(13);
                    sig[5] = (byte)(10);
                    sig[6] = (byte)(26);
                    sig[7] = (byte)(10);

                    byte* _out_;
                    byte* o;
                    byte* filt;
                    byte* zlib;
                    sbyte* line_buffer;
                    int i;
                    int j;
                    int k;
                    int p;
                    int zlen;
                    if ((stride_bytes) == (0)) stride_bytes = (int)(x * n);
                    filt = (byte*)(CRuntime.malloc((ulong)((x * n + 1) * y)));
                    if (filt == null) return null;
                    line_buffer = (sbyte*)(CRuntime.malloc((ulong)(x * n)));
                    if (line_buffer == null)
                    {
                        CRuntime.free(filt);
                        return null;
                    }

                    for (j = (int)(0); (j) < (y); ++j)
                    {
                        int* mapping = stackalloc int[5];
                        mapping[0] = (int)(0);
                        mapping[1] = (int)(1);
                        mapping[2] = (int)(2);
                        mapping[3] = (int)(3);
                        mapping[4] = (int)(4);
                        int* firstmap = stackalloc int[5];
                        firstmap[0] = (int)(0);
                        firstmap[1] = (int)(1);
                        firstmap[2] = (int)(0);
                        firstmap[3] = (int)(5);
                        firstmap[4] = (int)(6);
                        int* mymap = (j != 0) ? mapping : firstmap;
                        int best = (int)(0);
                        int bestval = (int)(0x7fffffff);
                        for (p = (int)(0); (p) < (2); ++p)
                        {
                            for (k = (int)((p) != 0 ? best : 0); (k) < (5); ++k)
                            {
                                int type = (int)(mymap[k]);
                                int est = (int)(0);
                                byte* z = pixels + stride_bytes * j;
                                for (i = (int)(0); (i) < (n); ++i)
                                {
                                    switch (type)
                                    {
                                        case 0:
                                            line_buffer[i] = (sbyte)(z[i]);
                                            break;
                                        case 1:
                                            line_buffer[i] = (sbyte)(z[i]);
                                            break;
                                        case 2:
                                            line_buffer[i] = (sbyte)(z[i] - z[i - stride_bytes]);
                                            break;
                                        case 3:
                                            line_buffer[i] = (sbyte)(z[i] - (z[i - stride_bytes] >> 1));
                                            break;
                                        case 4:
                                            line_buffer[i] = ((sbyte)(z[i] - stbiw__paeth((int)(0), (int)(z[i - stride_bytes]), (int)(0))));
                                            break;
                                        case 5:
                                            line_buffer[i] = (sbyte)(z[i]);
                                            break;
                                        case 6:
                                            line_buffer[i] = (sbyte)(z[i]);
                                            break;
                                    }
                                }
                                for (i = (int)(n); (i) < (x * n); ++i)
                                {
                                    switch (type)
                                    {
                                        case 0:
                                            line_buffer[i] = (sbyte)(z[i]);
                                            break;
                                        case 1:
                                            line_buffer[i] = (sbyte)(z[i] - z[i - n]);
                                            break;
                                        case 2:
                                            line_buffer[i] = (sbyte)(z[i] - z[i - stride_bytes]);
                                            break;
                                        case 3:
                                            line_buffer[i] = (sbyte)(z[i] - ((z[i - n] + z[i - stride_bytes]) >> 1));
                                            break;
                                        case 4:
                                            line_buffer[i] =
                                                (sbyte)(z[i] - stbiw__paeth((int)(z[i - n]), (int)(z[i - stride_bytes]), (int)(z[i - stride_bytes - n])));
                                            break;
                                        case 5:
                                            line_buffer[i] = (sbyte)(z[i] - (z[i - n] >> 1));
                                            break;
                                        case 6:
                                            line_buffer[i] = (sbyte)(z[i] - stbiw__paeth((int)(z[i - n]), (int)(0), (int)(0)));
                                            break;
                                    }
                                }
                                if ((p) != 0) break;
                                for (i = (int)(0); (i) < (x * n); ++i)
                                {
                                    est += (int)(CRuntime.abs((int)(line_buffer[i])));
                                }
                                if ((est) < (bestval))
                                {
                                    bestval = (int)(est);
                                    best = (int)(k);
                                }
                            }
                        }
                        filt[j * (x * n + 1)] = ((byte)(best));
                        CRuntime.memmove(filt + j * (x * n + 1) + 1, line_buffer, (ulong)(x * n));
                    }
                    CRuntime.free(line_buffer);
                    zlib = stbi_zlib_compress(filt, (int)(y * (x * n + 1)), &zlen, (int)(8));
                    CRuntime.free(filt);
                    if (zlib == null) return null;
                    _out_ = (byte*)(CRuntime.malloc((ulong)(8 + 12 + 13 + 12 + zlen + 12)));
                    if (_out_ == null) return null;
                    *out_len = (int)(8 + 12 + 13 + 12 + zlen + 12);
                    o = _out_;
                    CRuntime.memmove(o, sig, (ulong)(8));
                    o += 8;
                    (o)[0] = ((byte)(((13) >> 24) & 0xff));
                    (o)[1] = ((byte)(((13) >> 16) & 0xff));
                    (o)[2] = ((byte)(((13) >> 8) & 0xff));
                    (o)[3] = ((byte)((13) & 0xff));
                    (o) += 4;
                    (o)[0] = ((byte)(("IHDR"[0]) & 0xff));
                    (o)[1] = ((byte)(("IHDR"[1]) & 0xff));
                    (o)[2] = ((byte)(("IHDR"[2]) & 0xff));
                    (o)[3] = ((byte)(("IHDR"[3]) & 0xff));
                    (o) += 4;
                    (o)[0] = ((byte)(((x) >> 24) & 0xff));
                    (o)[1] = ((byte)(((x) >> 16) & 0xff));
                    (o)[2] = ((byte)(((x) >> 8) & 0xff));
                    (o)[3] = ((byte)((x) & 0xff));
                    (o) += 4;
                    (o)[0] = ((byte)(((y) >> 24) & 0xff));
                    (o)[1] = ((byte)(((y) >> 16) & 0xff));
                    (o)[2] = ((byte)(((y) >> 8) & 0xff));
                    (o)[3] = ((byte)((y) & 0xff));
                    (o) += 4;
                    *o++ = (byte)(8);
                    *o++ = ((byte)((ctype[n]) & 0xff));
                    *o++ = (byte)(0);
                    *o++ = (byte)(0);
                    *o++ = (byte)(0);
                    stbiw__wpcrc(&o, (int)(13));
                    (o)[0] = ((byte)(((zlen) >> 24) & 0xff));
                    (o)[1] = ((byte)(((zlen) >> 16) & 0xff));
                    (o)[2] = ((byte)(((zlen) >> 8) & 0xff));
                    (o)[3] = ((byte)((zlen) & 0xff));
                    (o) += 4;
                    (o)[0] = ((byte)(("IDAT"[0]) & 0xff));
                    (o)[1] = ((byte)(("IDAT"[1]) & 0xff));
                    (o)[2] = ((byte)(("IDAT"[2]) & 0xff));
                    (o)[3] = ((byte)(("IDAT"[3]) & 0xff));
                    (o) += 4;
                    CRuntime.memmove(o, zlib, (ulong)(zlen));
                    o += zlen;
                    CRuntime.free(zlib);
                    stbiw__wpcrc(&o, (int)(zlen));
                    (o)[0] = ((byte)(((0) >> 24) & 0xff));
                    (o)[1] = ((byte)(((0) >> 16) & 0xff));
                    (o)[2] = ((byte)(((0) >> 8) & 0xff));
                    (o)[3] = ((byte)((0) & 0xff));
                    (o) += 4;
                    (o)[0] = ((byte)(("IEND"[0]) & 0xff));
                    (o)[1] = ((byte)(("IEND"[1]) & 0xff));
                    (o)[2] = ((byte)(("IEND"[2]) & 0xff));
                    (o)[3] = ((byte)(("IEND"[3]) & 0xff));
                    (o) += 4;
                    stbiw__wpcrc(&o, (int)(0));
                    return _out_;
                }

                public static void stbiw__jpg_writeBits(stbi__write_context s, int* bitBufP, int* bitCntP, ushort bs0, ushort bs1)
                {
                    int bitBuf = (int)(*bitBufP);
                    int bitCnt = (int)(*bitCntP);
                    bitCnt += (int)(bs1);
                    bitBuf |= (int)(bs0 << (24 - bitCnt));
                    while ((bitCnt) >= (8))
                    {
                        byte c = (byte)((bitBuf >> 16) & 255);
                        stbiw__putc(s, (byte)(c));
                        if ((c) == (255))
                        {
                            stbiw__putc(s, (byte)(0));
                        }
                        bitBuf <<= 8;
                        bitCnt -= (int)(8);
                    }
                    *bitBufP = (int)(bitBuf);
                    *bitCntP = (int)(bitCnt);
                }

                public static void stbiw__jpg_DCT(float* d0p, float* d1p, float* d2p, float* d3p, float* d4p, float* d5p, float* d6p,
                    float* d7p)
                {
                    float d0 = (float)(*d0p);
                    float d1 = (float)(*d1p);
                    float d2 = (float)(*d2p);
                    float d3 = (float)(*d3p);
                    float d4 = (float)(*d4p);
                    float d5 = (float)(*d5p);
                    float d6 = (float)(*d6p);
                    float d7 = (float)(*d7p);
                    float z1;
                    float z2;
                    float z3;
                    float z4;
                    float z5;
                    float z11;
                    float z13;
                    float tmp0 = (float)(d0 + d7);
                    float tmp7 = (float)(d0 - d7);
                    float tmp1 = (float)(d1 + d6);
                    float tmp6 = (float)(d1 - d6);
                    float tmp2 = (float)(d2 + d5);
                    float tmp5 = (float)(d2 - d5);
                    float tmp3 = (float)(d3 + d4);
                    float tmp4 = (float)(d3 - d4);
                    float tmp10 = (float)(tmp0 + tmp3);
                    float tmp13 = (float)(tmp0 - tmp3);
                    float tmp11 = (float)(tmp1 + tmp2);
                    float tmp12 = (float)(tmp1 - tmp2);
                    d0 = (float)(tmp10 + tmp11);
                    d4 = (float)(tmp10 - tmp11);
                    z1 = (float)((tmp12 + tmp13) * 0.707106781f);
                    d2 = (float)(tmp13 + z1);
                    d6 = (float)(tmp13 - z1);
                    tmp10 = (float)(tmp4 + tmp5);
                    tmp11 = (float)(tmp5 + tmp6);
                    tmp12 = (float)(tmp6 + tmp7);
                    z5 = (float)((tmp10 - tmp12) * 0.382683433f);
                    z2 = (float)(tmp10 * 0.541196100f + z5);
                    z4 = (float)(tmp12 * 1.306562965f + z5);
                    z3 = (float)(tmp11 * 0.707106781f);
                    z11 = (float)(tmp7 + z3);
                    z13 = (float)(tmp7 - z3);
                    *d5p = (float)(z13 + z2);
                    *d3p = (float)(z13 - z2);
                    *d1p = (float)(z11 + z4);
                    *d7p = (float)(z11 - z4);
                    *d0p = (float)(d0);
                    *d2p = (float)(d2);
                    *d4p = (float)(d4);
                    *d6p = (float)(d6);
                }

                public static void stbiw__jpg_calcBits(int val, ushort* bits)
                {
                    int tmp1 = (int)((val) < (0) ? -val : val);
                    val = (int)((val) < (0) ? val - 1 : val);
                    bits[1] = (ushort)(1);
                    while ((tmp1 >>= 1) != 0)
                    {
                        ++bits[1];
                    }
                    bits[0] = (ushort)(val & ((1 << bits[1]) - 1));
                }

                public static int stbiw__jpg_processDU(stbi__write_context s, int* bitBuf, int* bitCnt, float* CDU, float* fdtbl,
                    int DC, ushort[,] HTDC, ushort[,] HTAC)
                {
                    ushort* EOB = stackalloc ushort[2];
                    EOB[0] = (ushort)(HTAC[0x00, 0]);
                    EOB[1] = (ushort)(HTAC[0x00, 1]);

                    ushort* M16zeroes = stackalloc ushort[2];
                    M16zeroes[0] = (ushort)(HTAC[0xF0, 0]);
                    M16zeroes[1] = (ushort)(HTAC[0xF0, 1]);

                    int dataOff;
                    int i;
                    int diff;
                    int end0pos;
                    int* DU = stackalloc int[64];
                    for (dataOff = (int)(0); (dataOff) < (64); dataOff += (int)(8))
                    {
                        stbiw__jpg_DCT(&CDU[dataOff], &CDU[dataOff + 1], &CDU[dataOff + 2], &CDU[dataOff + 3], &CDU[dataOff + 4],
                            &CDU[dataOff + 5], &CDU[dataOff + 6], &CDU[dataOff + 7]);
                    }
                    for (dataOff = (int)(0); (dataOff) < (8); ++dataOff)
                    {
                        stbiw__jpg_DCT(&CDU[dataOff], &CDU[dataOff + 8], &CDU[dataOff + 16], &CDU[dataOff + 24], &CDU[dataOff + 32],
                            &CDU[dataOff + 40], &CDU[dataOff + 48], &CDU[dataOff + 56]);
                    }
                    for (i = (int)(0); (i) < (64); ++i)
                    {
                        float v = (float)(CDU[i] * fdtbl[i]);
                        DU[stbiw__jpg_ZigZag[i]] = ((int)((v) < (0) ? v - 0.5f : v + 0.5f));
                    }
                    diff = (int)(DU[0] - DC);
                    if ((diff) == (0))
                    {
                        stbiw__jpg_writeBits(s, bitBuf, bitCnt, HTDC[0, 0], HTDC[0, 1]);
                    }
                    else
                    {
                        ushort* bits = stackalloc ushort[2];
                        stbiw__jpg_calcBits((int)(diff), bits);
                        stbiw__jpg_writeBits(s, bitBuf, bitCnt, HTDC[bits[1], 0], HTDC[bits[1], 1]);
                        stbiw__jpg_writeBits(s, bitBuf, bitCnt, bits[0], bits[1]);
                    }

                    end0pos = (int)(63);
                    for (; ((end0pos) > (0)) && ((DU[end0pos]) == (0)); --end0pos)
                    {
                    }
                    if ((end0pos) == (0))
                    {
                        stbiw__jpg_writeBits(s, bitBuf, bitCnt, EOB[0], EOB[1]);
                        return (int)(DU[0]);
                    }

                    for (i = (int)(1); i <= end0pos; ++i)
                    {
                        int startpos = (int)(i);
                        int nrzeroes;
                        ushort* bits = stackalloc ushort[2];
                        for (; ((DU[i]) == (0)) && (i <= end0pos); ++i)
                        {
                        }
                        nrzeroes = (int)(i - startpos);
                        if ((nrzeroes) >= (16))
                        {
                            int lng = (int)(nrzeroes >> 4);
                            int nrmarker;
                            for (nrmarker = (int)(1); nrmarker <= lng; ++nrmarker)
                            {
                                stbiw__jpg_writeBits(s, bitBuf, bitCnt, M16zeroes[0], M16zeroes[1]);
                            }
                            nrzeroes &= (int)(15);
                        }
                        stbiw__jpg_calcBits((int)(DU[i]), bits);
                        stbiw__jpg_writeBits(s, bitBuf, bitCnt, HTAC[(nrzeroes << 4) + bits[1], 0], HTAC[(nrzeroes << 4) + bits[1], 1]);
                        stbiw__jpg_writeBits(s, bitBuf, bitCnt, bits[0], bits[1]);
                    }
                    if (end0pos != 63)
                    {
                        stbiw__jpg_writeBits(s, bitBuf, bitCnt, EOB[0], EOB[1]);
                    }

                    return (int)(DU[0]);
                }

                public static int stbi_write_jpg_core(stbi__write_context s, int width, int height, int comp, void* data, int quality)
                {
                    int row;
                    int col;
                    int i;
                    int k;
                    float* fdtbl_Y = stackalloc float[64];
                    float* fdtbl_UV = stackalloc float[64];
                    byte* YTable = stackalloc byte[64];
                    byte* UVTable = stackalloc byte[64];
                    if (((((data == null) || (width == 0)) || (height == 0)) || ((comp) > (4))) || ((comp) < (1)))
                    {
                        return (int)(0);
                    }

                    quality = (int)((quality) != 0 ? quality : 90);
                    quality = (int)((quality) < (1) ? 1 : (quality) > (100) ? 100 : quality);
                    quality = (int)((quality) < (50) ? 5000 / quality : 200 - quality * 2);
                    for (i = (int)(0); (i) < (64); ++i)
                    {
                        int uvti;
                        int yti = (int)((YQT[i] * quality + 50) / 100);
                        YTable[stbiw__jpg_ZigZag[i]] = ((byte)((yti) < (1) ? 1 : (yti) > (255) ? 255 : yti));
                        uvti = (int)((UVQT[i] * quality + 50) / 100);
                        UVTable[stbiw__jpg_ZigZag[i]] = ((byte)((uvti) < (1) ? 1 : (uvti) > (255) ? 255 : uvti));
                    }
                    for (row = (int)(0), k = (int)(0); (row) < (8); ++row)
                    {
                        for (col = (int)(0); (col) < (8); ++col, ++k)
                        {
                            fdtbl_Y[k] = (float)(1 / (YTable[stbiw__jpg_ZigZag[k]] * aasf[row] * aasf[col]));
                            fdtbl_UV[k] = (float)(1 / (UVTable[stbiw__jpg_ZigZag[k]] * aasf[row] * aasf[col]));
                        }
                    }
                    {
                        byte* head1 = stackalloc byte[24];
                        head1[0] = (byte)(0xFF);
                        head1[1] = (byte)(0xC0);
                        head1[2] = (byte)(0);
                        head1[3] = (byte)(0x11);
                        head1[4] = (byte)(8);
                        head1[5] = (byte)(height >> 8);
                        head1[6] = (byte)((height) & 0xff);
                        head1[7] = (byte)(width >> 8);
                        head1[8] = (byte)((width) & 0xff);
                        head1[9] = (byte)(3);
                        head1[10] = (byte)(1);
                        head1[11] = (byte)(0x11);
                        head1[12] = (byte)(0);
                        head1[13] = (byte)(2);
                        head1[14] = (byte)(0x11);
                        head1[15] = (byte)(1);
                        head1[16] = (byte)(3);
                        head1[17] = (byte)(0x11);
                        head1[18] = (byte)(1);
                        head1[19] = (byte)(0xFF);
                        head1[20] = (byte)(0xC4);
                        head1[21] = (byte)(0x01);
                        head1[22] = (byte)(0xA2);
                        head1[23] = (byte)(0);

                        fixed (byte* h = head0)
                        {
                            s.func(s.context, h, head0.Length);
                        }


                        s.func(s.context, YTable, 64);
                        stbiw__putc(s, (byte)(1));
                        s.func(s.context, UVTable, 64);
                        s.func(s.context, head1, 24);

                        fixed (byte* d = &std_dc_luminance_nrcodes[1])
                        {
                            s.func(s.context, d, std_dc_chrominance_nrcodes.Length - 1);
                        }

                        fixed (byte* d = std_dc_luminance_values)
                        {
                            s.func(s.context, d, std_dc_chrominance_values.Length);
                        }

                        stbiw__putc(s, (byte)(0x10));

                        fixed (byte* a = &std_ac_luminance_nrcodes[1])
                        {
                            s.func(s.context, a, std_ac_luminance_nrcodes.Length - 1);
                        }

                        fixed (byte* a = std_ac_luminance_values)
                        {
                            s.func(s.context, a, std_ac_luminance_values.Length);
                        }

                        stbiw__putc(s, (byte)(1));

                        fixed (byte* c = &std_dc_chrominance_nrcodes[1])
                        {
                            s.func(s.context, c, std_dc_chrominance_nrcodes.Length - 1);
                        }

                        fixed (byte* c = std_dc_chrominance_values)
                        {
                            s.func(s.context, c, std_dc_chrominance_values.Length);
                        }

                        stbiw__putc(s, (byte)(0x11));


                        fixed (byte* c = &std_ac_chrominance_nrcodes[1])
                        {
                            s.func(s.context, c, std_ac_chrominance_nrcodes.Length - 1);
                        }

                        fixed (byte* c = std_ac_chrominance_values)
                        {
                            s.func(s.context, c, std_ac_chrominance_values.Length);
                        }

                        fixed (byte* c = head2)
                        {
                            s.func(s.context, c, head2.Length);
                        }
                    }

                    {
                        ushort* fillBits = stackalloc ushort[2];
                        fillBits[0] = (ushort)(0x7F);
                        fillBits[1] = (ushort)(7);
                        byte* imageData = (byte*)(data);
                        int DCY = (int)(0);
                        int DCU = (int)(0);
                        int DCV = (int)(0);
                        int bitBuf = (int)(0);
                        int bitCnt = (int)(0);
                        int ofsG = (int)((comp) > (2) ? 1 : 0);
                        int ofsB = (int)((comp) > (2) ? 2 : 0);
                        int x;
                        int y;
                        int pos;
                        float* YDU = stackalloc float[64];
                        float* UDU = stackalloc float[64];
                        float* VDU = stackalloc float[64];

                        for (y = (int)(0); (y) < (height); y += (int)(8))
                        {
                            for (x = (int)(0); (x) < (width); x += (int)(8))
                            {
                                for (row = (int)(y), pos = (int)(0); (row) < (y + 8); ++row)
                                {
                                    for (col = (int)(x); (col) < (x + 8); ++col, ++pos)
                                    {
                                        int p = (int)(row * width * comp + col * comp);
                                        float r;
                                        float g;
                                        float b;
                                        if ((row) >= (height))
                                        {
                                            p -= (int)(width * comp * (row + 1 - height));
                                        }
                                        if ((col) >= (width))
                                        {
                                            p -= (int)(comp * (col + 1 - width));
                                        }
                                        r = (float)(imageData[p + ofsB]);
                                        g = (float)(imageData[p + ofsG]);
                                        b = (float)(imageData[p + 0]);
                                        YDU[pos] = (float)(+0.29900f * r + 0.58700f * g + 0.11400f * b - 128);
                                        UDU[pos] = (float)(-0.16874f * r - 0.33126f * g + 0.50000f * b);
                                        VDU[pos] = (float)(+0.50000f * r - 0.41869f * g - 0.08131f * b);
                                    }
                                }

                                DCY = (int)(stbiw__jpg_processDU(s, &bitBuf, &bitCnt, YDU, fdtbl_Y, (int)(DCY), YDC_HT, YAC_HT));
                                DCU = (int)(stbiw__jpg_processDU(s, &bitBuf, &bitCnt, UDU, fdtbl_UV, (int)(DCU), UVDC_HT, UVAC_HT));
                                DCV = (int)(stbiw__jpg_processDU(s, &bitBuf, &bitCnt, VDU, fdtbl_UV, (int)(DCV), UVDC_HT, UVAC_HT));
                            }
                        }
                        stbiw__jpg_writeBits(s, &bitBuf, &bitCnt, fillBits[0], fillBits[1]);
                    }

                    stbiw__putc(s, (byte)(0xFF));
                    stbiw__putc(s, (byte)(0xD9));
                    return (int)(1);
                }
            }
            public static unsafe partial class StbVorbis
            {
                public class Residue
                {
                    public uint begin;
                    public uint end;
                    public uint part_size;
                    public byte classifications;
                    public byte classbook;
                    public byte** classdata;
                    public short[,] residue_books;
                }

                public class stb_vorbis
                {
                    public uint sample_rate;
                    public int channels;
                    public uint setup_memory_required;
                    public uint temp_memory_required;
                    public uint setup_temp_memory_required;
                    public byte* stream;
                    public byte* stream_start;
                    public byte* stream_end;
                    public uint stream_len;
                    public byte push_mode;
                    public uint first_audio_page_offset;
                    public ProbedPage p_first = new ProbedPage();
                    public ProbedPage p_last = new ProbedPage();
                    public stb_vorbis_alloc alloc = new stb_vorbis_alloc();
                    public int setup_offset;
                    public int temp_offset;
                    public int eof;

                    public int error;
                    public int[] blocksize = new int[2];
                    public int blocksize_0;
                    public int blocksize_1;
                    public int codebook_count;
                    public Codebook* codebooks;
                    public int floor_count;
                    public ushort[] floor_types = new ushort[64];
                    public Floor* floor_config;
                    public int residue_count;
                    public ushort[] residue_types = new ushort[64];
                    public Residue[] residue_config;
                    public int mapping_count;
                    public Mapping* mapping;
                    public int mode_count;
                    public Mode* mode_config = (Mode*)CRuntime.malloc(64 * sizeof(Mode));
                    public uint total_samples;
                    public float*[] channel_buffers = new float*[16];
                    public float*[] outputs = new float*[16];
                    public float*[] previous_window = new float*[16];
                    public int previous_length;
                    public short*[] finalY = new short*[16];
                    public uint current_loc;
                    public int current_loc_valid;
                    public float*[] A = new float*[2];
                    public float*[] B = new float*[2];
                    public float*[] C = new float*[2];
                    public float*[] window = new float*[2];
                    public ushort*[] bit_reverse = new ushort*[2];
                    public uint serial;
                    public int last_page;
                    public int segment_count;
                    public byte* segments = (byte*)CRuntime.malloc(255 * sizeof(byte));
                    public byte page_flag;
                    public byte bytes_in_seg;
                    public byte first_decode;
                    public int next_seg;
                    public int last_seg;
                    public int last_seg_which;
                    public uint acc;
                    public int valid_bits;
                    public int packet_bytes;
                    public int end_seg_with_known_loc;
                    public uint known_loc_for_packet;
                    public int discard_samples_deferred;
                    public uint samples_output;
                    public int page_crc_tests;
                    public CRCscan[] scan = new CRCscan[4];
                    public int channel_buffer_start;
                    public int channel_buffer_end;
                }

                public static sbyte[,] channel_position =
                {
            {0, 0, 0, 0, 0, 0},
            {2 | 4 | 1, 0, 0, 0, 0, 0},
            {2 | 1, 4 | 1, 0, 0, 0, 0},
            {2 | 1, 2 | 4 | 1, 4 | 1, 0, 0, 0},
            {2 | 1, 4 | 1, 2 | 1, 4 | 1, 0, 0},
            {2 | 1, 2 | 4 | 1, 4 | 1, 2 | 1, 4 | 1, 0},
            {2 | 1, 2 | 4 | 1, 4 | 1, 2 | 1, 4 | 1, 2 | 4 | 1}
        };

                public static uint get_bits(stb_vorbis f, int n)
                {
                    uint z;
                    if (f.valid_bits < 0) return 0;
                    if (f.valid_bits < n)
                    {
                        if (n > 24)
                        {
                            z = get_bits(f, 24);
                            z += get_bits(f, n - 24) << 24;
                            return z;
                        }

                        if (f.valid_bits == 0) f.acc = 0;
                        while (f.valid_bits < n)
                        {
                            var z2 = get8_packet_raw(f);
                            if (z2 == -1)
                            {
                                f.valid_bits = -1;
                                return 0;
                            }

                            f.acc += (uint)(z2 << f.valid_bits);
                            f.valid_bits += 8;
                        }
                    }

                    if (f.valid_bits < 0) return 0;
                    z = (uint)(f.acc & ((1 << n) - 1));
                    f.acc >>= n;
                    f.valid_bits -= n;
                    return z;
                }

                public static short[] decode_vorbis_from_memory(byte[] input, out int sampleRate, out int chan)
                {
                    short* result = null;
                    int length = 0;
                    fixed (byte* b = input)
                    {
                        int c, s;
                        length = stb_vorbis_decode_memory(b, input.Length, &c, &s, ref result);

                        chan = c;
                        sampleRate = s;
                    }

                    var output = new short[length];
                    Marshal.Copy(new IntPtr(result), output, 0, output.Length);
                    CRuntime.free(result);

                    return output;
                }
            }
            unsafe partial class StbVorbis
            {
                [StructLayout(LayoutKind.Sequential)]
                public struct stb_vorbis_alloc
                {
                    public sbyte* alloc_buffer;
                    public int alloc_buffer_length_in_bytes;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct stb_vorbis_info
                {
                    public uint sample_rate;
                    public int channels;
                    public uint setup_memory_required;
                    public uint setup_temp_memory_required;
                    public uint temp_memory_required;
                    public int max_frame_size;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct Codebook
                {
                    public int dimensions;
                    public int entries;
                    public byte* codeword_lengths;
                    public float minimum_value;
                    public float delta_value;
                    public byte value_bits;
                    public byte lookup_type;
                    public byte sequence_p;
                    public byte sparse;
                    public uint lookup_values;
                    public float* multiplicands;
                    public uint* codewords;
                    public fixed short fast_huffman[(1 << 10)];
                    public uint* sorted_codewords;
                    public int* sorted_values;
                    public int sorted_entries;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct Floor0
                {
                    public byte order;
                    public ushort rate;
                    public ushort bark_map_size;
                    public byte amplitude_bits;
                    public byte amplitude_offset;
                    public byte number_of_books;
                    public fixed byte book_list[16];
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct Floor1
                {
                    public byte partitions;
                    public fixed byte partition_class_list[32];
                    public fixed byte class_dimensions[16];
                    public fixed byte class_subclasses[16];
                    public fixed byte class_masterbooks[16];
                    public fixed short subclass_books[16 * 8];
                    public fixed ushort Xlist[31 * 8 + 2];
                    public fixed byte sorted_order[31 * 8 + 2];
                    public fixed byte neighbors[(31 * 8 + 2) * 2];
                    public byte floor1_multiplier;
                    public byte rangebits;
                    public int values;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct Floor
                {
                    public Floor0 floor0;
                    public Floor1 floor1;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct MappingChannel
                {
                    public byte magnitude;
                    public byte angle;
                    public byte mux;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct Mapping
                {
                    public ushort coupling_steps;
                    public MappingChannel* chan;
                    public byte submaps;
                    public fixed byte submap_floor[15];
                    public fixed byte submap_residue[15];
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct Mode
                {
                    public byte blockflag;
                    public byte mapping;
                    public ushort windowtype;
                    public ushort transformtype;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct CRCscan
                {
                    public uint goal_crc;
                    public int bytes_left;
                    public uint crc_so_far;
                    public int bytes_done;
                    public uint sample_loc;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct ProbedPage
                {
                    public uint page_start;
                    public uint page_end;
                    public uint last_decoded_sample;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct stbv__floor_ordering
                {
                    public ushort x;
                    public ushort id;
                }

                public const int VORBIS__no_error = 0;
                public const int VORBIS_need_more_data = 1;
                public const int VORBIS_invalid_api_mixing = 2;
                public const int VORBIS_outofmem = 3;
                public const int VORBIS_feature_not_supported = 4;
                public const int VORBIS_too_many_channels = 5;
                public const int VORBIS_file_open_failure = 6;
                public const int VORBIS_seek_without_length = 7;
                public const int VORBIS_unexpected_eof = 10;
                public const int VORBIS_seek_invalid = 11;
                public const int VORBIS_invalid_setup = 20;
                public const int VORBIS_invalid_stream = 21;
                public const int VORBIS_missing_capture_pattern = 30;
                public const int VORBIS_invalid_stream_structure_version = 31;
                public const int VORBIS_continued_packet_flag_invalid = 32;
                public const int VORBIS_incorrect_stream_serial_number = 33;
                public const int VORBIS_invalid_first_page = 34;
                public const int VORBIS_bad_packet_type = 35;
                public const int VORBIS_cant_find_last_page = 36;
                public const int VORBIS_seek_failed = 37;
                public const int VORBIS_packet_id = 1;
                public const int VORBIS_packet_comment = 3;
                public const int VORBIS_packet_setup = 5;
                public static uint[] _crc_table = new uint[256];
                public static sbyte[] log2_4 = { 0, 1, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4 };
                public static byte[] ogg_page_header = { 0x4f, 0x67, 0x67, 0x53 };

                public static float[] inverse_db_table =
                {
            1.0649863e-07f, 1.1341951e-07f, 1.2079015e-07f, 1.2863978e-07f,
            1.3699951e-07f, 1.4590251e-07f, 1.5538408e-07f, 1.6548181e-07f, 1.7623575e-07f, 1.8768855e-07f, 1.9988561e-07f,
            2.1287530e-07f, 2.2670913e-07f, 2.4144197e-07f, 2.5713223e-07f, 2.7384213e-07f, 2.9163793e-07f, 3.1059021e-07f,
            3.3077411e-07f, 3.5226968e-07f, 3.7516214e-07f, 3.9954229e-07f, 4.2550680e-07f, 4.5315863e-07f, 4.8260743e-07f,
            5.1396998e-07f, 5.4737065e-07f, 5.8294187e-07f, 6.2082472e-07f, 6.6116941e-07f, 7.0413592e-07f, 7.4989464e-07f,
            7.9862701e-07f, 8.5052630e-07f, 9.0579828e-07f, 9.6466216e-07f, 1.0273513e-06f, 1.0941144e-06f, 1.1652161e-06f,
            1.2409384e-06f, 1.3215816e-06f, 1.4074654e-06f, 1.4989305e-06f, 1.5963394e-06f, 1.7000785e-06f, 1.8105592e-06f,
            1.9282195e-06f, 2.0535261e-06f, 2.1869758e-06f, 2.3290978e-06f, 2.4804557e-06f, 2.6416497e-06f, 2.8133190e-06f,
            2.9961443e-06f, 3.1908506e-06f, 3.3982101e-06f, 3.6190449e-06f, 3.8542308e-06f, 4.1047004e-06f, 4.3714470e-06f,
            4.6555282e-06f, 4.9580707e-06f, 5.2802740e-06f, 5.6234160e-06f, 5.9888572e-06f, 6.3780469e-06f, 6.7925283e-06f,
            7.2339451e-06f, 7.7040476e-06f, 8.2047000e-06f, 8.7378876e-06f, 9.3057248e-06f, 9.9104632e-06f, 1.0554501e-05f,
            1.1240392e-05f, 1.1970856e-05f, 1.2748789e-05f, 1.3577278e-05f, 1.4459606e-05f, 1.5399272e-05f, 1.6400004e-05f,
            1.7465768e-05f, 1.8600792e-05f, 1.9809576e-05f, 2.1096914e-05f, 2.2467911e-05f, 2.3928002e-05f, 2.5482978e-05f,
            2.7139006e-05f, 2.8902651e-05f, 3.0780908e-05f, 3.2781225e-05f, 3.4911534e-05f, 3.7180282e-05f, 3.9596466e-05f,
            4.2169667e-05f, 4.4910090e-05f, 4.7828601e-05f, 5.0936773e-05f, 5.4246931e-05f, 5.7772202e-05f, 6.1526565e-05f,
            6.5524908e-05f, 6.9783085e-05f, 7.4317983e-05f, 7.9147585e-05f, 8.4291040e-05f, 8.9768747e-05f, 9.5602426e-05f,
            0.00010181521f, 0.00010843174f, 0.00011547824f, 0.00012298267f, 0.00013097477f, 0.00013948625f, 0.00014855085f,
            0.00015820453f, 0.00016848555f, 0.00017943469f, 0.00019109536f, 0.00020351382f, 0.00021673929f, 0.00023082423f,
            0.00024582449f, 0.00026179955f, 0.00027881276f, 0.00029693158f, 0.00031622787f, 0.00033677814f, 0.00035866388f,
            0.00038197188f, 0.00040679456f, 0.00043323036f, 0.00046138411f, 0.00049136745f, 0.00052329927f, 0.00055730621f,
            0.00059352311f, 0.00063209358f, 0.00067317058f, 0.00071691700f, 0.00076350630f, 0.00081312324f, 0.00086596457f,
            0.00092223983f, 0.00098217216f, 0.0010459992f, 0.0011139742f, 0.0011863665f, 0.0012634633f, 0.0013455702f,
            0.0014330129f, 0.0015261382f, 0.0016253153f, 0.0017309374f, 0.0018434235f, 0.0019632195f, 0.0020908006f,
            0.0022266726f, 0.0023713743f, 0.0025254795f, 0.0026895994f, 0.0028643847f, 0.0030505286f, 0.0032487691f,
            0.0034598925f, 0.0036847358f, 0.0039241906f, 0.0041792066f, 0.0044507950f, 0.0047400328f, 0.0050480668f,
            0.0053761186f, 0.0057254891f, 0.0060975636f, 0.0064938176f, 0.0069158225f, 0.0073652516f, 0.0078438871f,
            0.0083536271f, 0.0088964928f, 0.009474637f, 0.010090352f, 0.010746080f, 0.011444421f, 0.012188144f, 0.012980198f,
            0.013823725f, 0.014722068f, 0.015678791f, 0.016697687f, 0.017782797f, 0.018938423f, 0.020169149f, 0.021479854f,
            0.022875735f, 0.024362330f, 0.025945531f, 0.027631618f, 0.029427276f, 0.031339626f, 0.033376252f, 0.035545228f,
            0.037855157f, 0.040315199f, 0.042935108f, 0.045725273f, 0.048696758f, 0.051861348f, 0.055231591f, 0.058820850f,
            0.062643361f, 0.066714279f, 0.071049749f, 0.075666962f, 0.080584227f, 0.085821044f, 0.091398179f, 0.097337747f,
            0.10366330f, 0.11039993f, 0.11757434f, 0.12521498f, 0.13335215f, 0.14201813f, 0.15124727f, 0.16107617f, 0.17154380f,
            0.18269168f, 0.19456402f, 0.20720788f, 0.22067342f, 0.23501402f, 0.25028656f, 0.26655159f, 0.28387361f, 0.30232132f,
            0.32196786f, 0.34289114f, 0.36517414f, 0.38890521f, 0.41417847f, 0.44109412f, 0.46975890f, 0.50028648f, 0.53279791f,
            0.56742212f, 0.60429640f, 0.64356699f, 0.68538959f, 0.72993007f, 0.77736504f, 0.82788260f, 0.88168307f, 0.9389798f,
            1.0f
        };

                public static int[,] channel_selector = { { 0, 0 }, { 1, 0 }, { 2, 4 } };

                public static int error(stb_vorbis f, int e)
                {
                    f.error = (e);
                    if ((f.eof == 0) && (e != VORBIS_need_more_data))
                    {
                        f.error = (e);
                    }

                    throw new Exception("Vorbis Error: " + e);
                }

                public static void* make_block_array(void* mem, int count, int size)
                {
                    int i;
                    void** p = (void**)(mem);
                    sbyte* q = (sbyte*)(p + count);
                    for (i = (int)(0); (i) < (count); ++i)
                    {
                        p[i] = q;
                        q += size;
                    }
                    return p;
                }

                public static void* setup_malloc(stb_vorbis f, int sz)
                {
                    sz = (int)((sz + 3) & ~3);
                    f.setup_memory_required += (uint)(sz);
                    if ((f.alloc.alloc_buffer) != null)
                    {
                        void* p = f.alloc.alloc_buffer + f.setup_offset;
                        if ((f.setup_offset + sz) > (f.temp_offset)) return (null);
                        f.setup_offset += (int)(sz);
                        return p;
                    }

                    return (sz) != 0 ? CRuntime.malloc((ulong)(sz)) : (null);
                }

                public static void setup_free(stb_vorbis f, void* p)
                {
                    if ((f.alloc.alloc_buffer) != null) return;
                    CRuntime.free(p);
                }

                public static void* setup_temp_malloc(stb_vorbis f, int sz)
                {
                    sz = (int)((sz + 3) & ~3);
                    if ((f.alloc.alloc_buffer) != null)
                    {
                        if ((f.temp_offset - sz) < (f.setup_offset)) return (null);
                        f.temp_offset -= (int)(sz);
                        return f.alloc.alloc_buffer + f.temp_offset;
                    }

                    return CRuntime.malloc((ulong)(sz));
                }

                public static void setup_temp_free(stb_vorbis f, void* p, int sz)
                {
                    if ((f.alloc.alloc_buffer) != null)
                    {
                        f.temp_offset += (int)((sz + 3) & ~3);
                        return;
                    }

                    CRuntime.free(p);
                }

                public static void crc32_init()
                {
                    int i;
                    int j;
                    uint s;
                    for (i = (int)(0); (i) < (256); i++)
                    {
                        for (s = (uint)((uint)(i) << 24), j = (int)(0); (j) < (8); ++j)
                        {
                            s = (uint)((s << 1) ^ ((s) >= (1U << 31) ? 0x04c11db7 : 0));
                        }
                        _crc_table[i] = (uint)(s);
                    }
                }

                public static uint crc32_update(uint crc, byte b)
                {
                    return (uint)((crc << 8) ^ _crc_table[b ^ (crc >> 24)]);
                }

                public static uint bit_reverse(uint n)
                {
                    n = (uint)(((n & 0xAAAAAAAA) >> 1) | ((n & 0x55555555) << 1));
                    n = (uint)(((n & 0xCCCCCCCC) >> 2) | ((n & 0x33333333) << 2));
                    n = (uint)(((n & 0xF0F0F0F0) >> 4) | ((n & 0x0F0F0F0F) << 4));
                    n = (uint)(((n & 0xFF00FF00) >> 8) | ((n & 0x00FF00FF) << 8));
                    return (uint)((n >> 16) | (n << 16));
                }

                public static float square(float x)
                {
                    return (float)(x * x);
                }

                public static int ilog(int n)
                {
                    if ((n) < (0)) return (int)(0);
                    if ((n) < (1 << 14))
                        if ((n) < (1 << 4)) return (int)(0 + log2_4[n]);
                        else if ((n) < (1 << 9)) return (int)(5 + log2_4[n >> 5]);
                        else return (int)(10 + log2_4[n >> 10]);
                    else if ((n) < (1 << 24))
                        if ((n) < (1 << 19)) return (int)(15 + log2_4[n >> 15]);
                        else return (int)(20 + log2_4[n >> 20]);
                    else if ((n) < (1 << 29)) return (int)(25 + log2_4[n >> 25]);
                    else return (int)(30 + log2_4[n >> 30]);
                }

                public static float float32_unpack(uint x)
                {
                    uint mantissa = (uint)(x & 0x1fffff);
                    uint sign = (uint)(x & 0x80000000);
                    uint exp = (uint)((x & 0x7fe00000) >> 21);
                    double res = (double)((sign) != 0 ? -(double)(mantissa) : (double)(mantissa));
                    return (float)(CRuntime.ldexp((double)((float)(res)), (int)(exp - 788)));
                }

                public static void add_entry(Codebook* c, uint huff_code, int symbol, int count, int len, uint* values)
                {
                    if (c->sparse == 0)
                    {
                        c->codewords[symbol] = (uint)(huff_code);
                    }
                    else
                    {
                        c->codewords[count] = (uint)(huff_code);
                        c->codeword_lengths[count] = (byte)(len);
                        values[count] = (uint)(symbol);
                    }
                }

                public static int compute_codewords(Codebook* c, byte* len, int n, uint* values)
                {
                    int i;
                    int k;
                    int m = (int)(0);
                    uint* available = stackalloc uint[32];
                    CRuntime.memset(available, 0, 32 * sizeof(uint));
                    for (k = (int)(0); (k) < (n); ++k)
                    {
                        if ((len[k]) < (255)) break;
                    }
                    if ((k) == (n))
                    {
                        return (int)(1);
                    }

                    add_entry(c, (uint)(0), (int)(k), (int)(m++), (int)(len[k]), values);
                    for (i = (int)(1); i <= len[k]; ++i)
                    {
                        available[i] = (uint)(1U << (32 - i));
                    }
                    for (i = (int)(k + 1); (i) < (n); ++i)
                    {
                        uint res;
                        int z = (int)(len[i]);
                        int y;
                        if ((z) == (255)) continue;
                        while (((z) > (0)) && (available[z] == 0))
                        {
                            --z;
                        }
                        if ((z) == (0))
                        {
                            return (int)(0);
                        }
                        res = (uint)(available[z]);
                        available[z] = (uint)(0);
                        add_entry(c, (uint)(bit_reverse((uint)(res))), (int)(i), (int)(m++), (int)(len[i]), values);
                        if (z != len[i])
                        {
                            for (y = (int)(len[i]); (y) > (z); --y)
                            {
                                available[y] = (uint)(res + (1 << (32 - y)));
                            }
                        }
                    }
                    return (int)(1);
                }

                public static void compute_accelerated_huffman(Codebook* c)
                {
                    int i;
                    int len;
                    for (i = (int)(0); (i) < (1 << 10); ++i)
                    {
                        c->fast_huffman[i] = (short)(-1);
                    }
                    len = (int)((c->sparse) != 0 ? c->sorted_entries : c->entries);
                    if ((len) > (32767)) len = (int)(32767);
                    for (i = (int)(0); (i) < (len); ++i)
                    {
                        if (c->codeword_lengths[i] <= 10)
                        {
                            uint z = (uint)((c->sparse) != 0 ? bit_reverse((uint)(c->sorted_codewords[i])) : c->codewords[i]);
                            while ((z) < (1 << 10))
                            {
                                c->fast_huffman[z] = (short)(i);
                                z += (uint)(1 << c->codeword_lengths[i]);
                            }
                        }
                    }
                }

                public static int uint32_compare(void* p, void* q)
                {
                    uint x = (uint)(*(uint*)(p));
                    uint y = (uint)(*(uint*)(q));
                    return (int)((x) < (y) ? -1 : ((x) > (y) ? 1 : 0));
                }

                public static int include_in_sort(Codebook* c, byte len)
                {
                    if ((c->sparse) != 0)
                    {
                        return (int)(1);
                    }

                    if ((len) == (255)) return (int)(0);
                    if ((len) > (10)) return (int)(1);
                    return (int)(0);
                }

                public static void compute_sorted_huffman(Codebook* c, byte* lengths, uint* values)
                {
                    int i;
                    int len;
                    if (c->sparse == 0)
                    {
                        int k = (int)(0);
                        for (i = (int)(0); (i) < (c->entries); ++i)
                        {
                            if ((include_in_sort(c, (byte)(lengths[i]))) != 0)
                                c->sorted_codewords[k++] = (uint)(bit_reverse((uint)(c->codewords[i])));
                        }
                    }
                    else
                    {
                        for (i = (int)(0); (i) < (c->sorted_entries); ++i)
                        {
                            c->sorted_codewords[i] = (uint)(bit_reverse((uint)(c->codewords[i])));
                        }
                    }

                    CRuntime.qsort(c->sorted_codewords, (ulong)(c->sorted_entries), (ulong)(sizeof(uint)), uint32_compare);
                    c->sorted_codewords[c->sorted_entries] = (uint)(0xffffffff);
                    len = (int)((c->sparse) != 0 ? c->sorted_entries : c->entries);
                    for (i = (int)(0); (i) < (len); ++i)
                    {
                        int huff_len = (int)((c->sparse) != 0 ? lengths[values[i]] : lengths[i]);
                        if ((include_in_sort(c, (byte)(huff_len))) != 0)
                        {
                            uint code = (uint)(bit_reverse((uint)(c->codewords[i])));
                            int x = (int)(0);
                            int n = (int)(c->sorted_entries);
                            while ((n) > (1))
                            {
                                int m = (int)(x + (n >> 1));
                                if (c->sorted_codewords[m] <= code)
                                {
                                    x = (int)(m);
                                    n -= (int)(n >> 1);
                                }
                                else
                                {
                                    n >>= 1;
                                }
                            }
                            if ((c->sparse) != 0)
                            {
                                c->sorted_values[x] = (int)(values[i]);
                                c->codeword_lengths[x] = (byte)(huff_len);
                            }
                            else
                            {
                                c->sorted_values[x] = (int)(i);
                            }
                        }
                    }
                }

                public static int vorbis_validate(byte* data)
                {
                    byte* vorbis = stackalloc byte[6];
                    vorbis[0] = (byte)('v');
                    vorbis[1] = (byte)('o');
                    vorbis[2] = (byte)('r');
                    vorbis[3] = (byte)('b');
                    vorbis[4] = (byte)('i');
                    vorbis[5] = (byte)('s');

                    return (int)((CRuntime.memcmp(data, vorbis, (ulong)(6))) == (0) ? 1 : 0);
                }

                public static int lookup1_values(int entries, int dim)
                {
                    int r =
                        (int)(CRuntime.floor((double)(CRuntime.exp((double)((float)(CRuntime.log((double)((float)(entries)))) / dim)))));
                    if ((int)(CRuntime.floor((double)(CRuntime.pow((double)((float)(r) + 1), (double)(dim))))) <= entries) ++r;
                    return (int)(r);
                }

                public static void compute_twiddle_factors(int n, float* A, float* B, float* C)
                {
                    int n4 = (int)(n >> 2);
                    int n8 = (int)(n >> 3);
                    int k;
                    int k2;
                    for (k = (int)(k2 = (int)(0)); (k) < (n4); ++k, k2 += (int)(2))
                    {
                        A[k2] = ((float)(CRuntime.cos((double)(4 * k * 3.14159265358979323846264f / n))));
                        A[k2 + 1] = ((float)(-CRuntime.sin((double)(4 * k * 3.14159265358979323846264f / n))));
                        B[k2] = (float)((float)(CRuntime.cos((double)((k2 + 1) * 3.14159265358979323846264f / n / 2))) * 0.5f);
                        B[k2 + 1] = (float)((float)(CRuntime.sin((double)((k2 + 1) * 3.14159265358979323846264f / n / 2))) * 0.5f);
                    }
                    for (k = (int)(k2 = (int)(0)); (k) < (n8); ++k, k2 += (int)(2))
                    {
                        C[k2] = ((float)(CRuntime.cos((double)(2 * (k2 + 1) * 3.14159265358979323846264f / n))));
                        C[k2 + 1] = ((float)(-CRuntime.sin((double)(2 * (k2 + 1) * 3.14159265358979323846264f / n))));
                    }
                }

                public static void compute_window(int n, float* window)
                {
                    int n2 = (int)(n >> 1);
                    int i;
                    for (i = (int)(0); (i) < (n2); ++i)
                    {
                        window[i] =
                            ((float)
                                (CRuntime.sin(
                                    (double)
                                        (0.5 * 3.14159265358979323846264f *
                                         square((float)(CRuntime.sin((double)((i - 0 + 0.5) / n2 * 0.5 * 3.14159265358979323846264f))))))));
                    }
                }

                public static void compute_bitreverse(int n, ushort* rev)
                {
                    int ld = (int)(ilog((int)(n)) - 1);
                    int i;
                    int n8 = (int)(n >> 3);
                    for (i = (int)(0); (i) < (n8); ++i)
                    {
                        rev[i] = (ushort)((bit_reverse((uint)(i)) >> (32 - ld + 3)) << 2);
                    }
                }

                public static int init_blocksize(stb_vorbis f, int b, int n)
                {
                    int n2 = (int)(n >> 1);
                    int n4 = (int)(n >> 2);
                    int n8 = (int)(n >> 3);
                    f.A[b] = (float*)(setup_malloc(f, (int)(sizeof(float) * n2)));
                    f.B[b] = (float*)(setup_malloc(f, (int)(sizeof(float) * n2)));
                    f.C[b] = (float*)(setup_malloc(f, (int)(sizeof(float) * n4)));
                    if (((f.A[b] == null) || (f.B[b] == null)) || (f.C[b] == null)) return (int)(error(f, (int)(VORBIS_outofmem)));
                    compute_twiddle_factors((int)(n), f.A[b], f.B[b], f.C[b]);
                    f.window[b] = (float*)(setup_malloc(f, (int)(sizeof(float) * n2)));
                    if (f.window[b] == null) return (int)(error(f, (int)(VORBIS_outofmem)));
                    compute_window((int)(n), f.window[b]);
                    f.bit_reverse[b] = (ushort*)(setup_malloc(f, (int)(sizeof(ushort) * n8)));
                    if (f.bit_reverse[b] == null) return (int)(error(f, (int)(VORBIS_outofmem)));
                    compute_bitreverse((int)(n), f.bit_reverse[b]);
                    return (int)(1);
                }

                public static void neighbors(ushort* x, int n, int* plow, int* phigh)
                {
                    int low = (int)(-1);
                    int high = (int)(65536);
                    int i;
                    for (i = (int)(0); (i) < (n); ++i)
                    {
                        if (((x[i]) > (low)) && ((x[i]) < (x[n])))
                        {
                            *plow = (int)(i);
                            low = (int)(x[i]);
                        }
                        if (((x[i]) < (high)) && ((x[i]) > (x[n])))
                        {
                            *phigh = (int)(i);
                            high = (int)(x[i]);
                        }
                    }
                }

                public static int point_compare(void* p, void* q)
                {
                    stbv__floor_ordering* a = (stbv__floor_ordering*)(p);
                    stbv__floor_ordering* b = (stbv__floor_ordering*)(q);
                    return (int)((a->x) < (b->x) ? -1 : ((a->x) > (b->x) ? 1 : 0));
                }

                public static byte get8(stb_vorbis z)
                {
                    if ((1) != 0)
                    {
                        if ((z.stream) >= (z.stream_end))
                        {
                            z.eof = (int)(1);
                            return (byte)(0);
                        }
                        return (byte)(*z.stream++);
                    }

                }

                public static uint get32(stb_vorbis f)
                {
                    uint x;
                    x = (uint)(get8(f));
                    x += (uint)(get8(f) << 8);
                    x += (uint)(get8(f) << 16);
                    x += (uint)((uint)(get8(f)) << 24);
                    return (uint)(x);
                }

                public static int getn(stb_vorbis z, byte* data, int n)
                {
                    if ((1) != 0)
                    {
                        if ((z.stream + n) > (z.stream_end))
                        {
                            z.eof = (int)(1);
                            return (int)(0);
                        }
                        CRuntime.memcpy(data, z.stream, (ulong)(n));
                        z.stream += n;
                        return (int)(1);
                    }

                }

                public static void skip(stb_vorbis z, int n)
                {
                    if ((1) != 0)
                    {
                        z.stream += n;
                        if ((z.stream) >= (z.stream_end)) z.eof = (int)(1);
                        return;
                    }

                }

                public static int set_file_offset(stb_vorbis f, uint loc)
                {
                    if ((f.push_mode) != 0) return (int)(0);
                    f.eof = (int)(0);
                    if ((1) != 0)
                    {
                        if (((f.stream_start + loc) >= (f.stream_end)) || ((f.stream_start + loc) < (f.stream_start)))
                        {
                            f.stream = f.stream_end;
                            f.eof = (int)(1);
                            return (int)(0);
                        }
                        else
                        {
                            f.stream = f.stream_start + loc;
                            return (int)(1);
                        }
                    }

                }

                public static int capture_pattern(stb_vorbis f)
                {
                    if (0x4f != get8(f)) return (int)(0);
                    if (0x67 != get8(f)) return (int)(0);
                    if (0x67 != get8(f)) return (int)(0);
                    if (0x53 != get8(f)) return (int)(0);
                    return (int)(1);
                }

                public static int start_page_no_capturepattern(stb_vorbis f)
                {
                    uint loc0;
                    uint loc1;
                    uint n;
                    if (0 != get8(f)) return (int)(error(f, (int)(VORBIS_invalid_stream_structure_version)));
                    f.page_flag = (byte)(get8(f));
                    loc0 = (uint)(get32(f));
                    loc1 = (uint)(get32(f));
                    get32(f);
                    n = (uint)(get32(f));
                    f.last_page = (int)(n);
                    get32(f);
                    f.segment_count = (int)(get8(f));
                    if (getn(f, f.segments, (int)(f.segment_count)) == 0) return (int)(error(f, (int)(VORBIS_unexpected_eof)));
                    f.end_seg_with_known_loc = (int)(-2);
                    if ((loc0 != ~0U) || (loc1 != ~0U))
                    {
                        int i;
                        for (i = (int)(f.segment_count - 1); (i) >= (0); --i)
                        {
                            if ((f.segments[i]) < (255)) break;
                        }
                        if ((i) >= (0))
                        {
                            f.end_seg_with_known_loc = (int)(i);
                            f.known_loc_for_packet = (uint)(loc0);
                        }
                    }

                    if ((f.first_decode) != 0)
                    {
                        int i;
                        int len;
                        ProbedPage p = new ProbedPage();
                        len = (int)(0);
                        for (i = (int)(0); (i) < (f.segment_count); ++i)
                        {
                            len += (int)(f.segments[i]);
                        }
                        len += (int)(27 + f.segment_count);
                        p.page_start = (uint)(f.first_audio_page_offset);
                        p.page_end = (uint)(p.page_start + len);
                        p.last_decoded_sample = (uint)(loc0);
                        f.p_first = (ProbedPage)(p);
                    }

                    f.next_seg = (int)(0);
                    return (int)(1);
                }

                public static int start_page(stb_vorbis f)
                {
                    if (capture_pattern(f) == 0) return (int)(error(f, (int)(VORBIS_missing_capture_pattern)));
                    return (int)(start_page_no_capturepattern(f));
                }

                public static int start_packet(stb_vorbis f)
                {
                    while ((f.next_seg) == (-1))
                    {
                        if (start_page(f) == 0) return (int)(0);
                        if ((f.page_flag & 1) != 0) return (int)(error(f, (int)(VORBIS_continued_packet_flag_invalid)));
                    }
                    f.last_seg = (int)(0);
                    f.valid_bits = (int)(0);
                    f.packet_bytes = (int)(0);
                    f.bytes_in_seg = (byte)(0);
                    return (int)(1);
                }

                public static int maybe_start_packet(stb_vorbis f)
                {
                    if ((f.next_seg) == (-1))
                    {
                        int x = (int)(get8(f));
                        if ((f.eof) != 0) return (int)(0);
                        if (0x4f != x) return (int)(error(f, (int)(VORBIS_missing_capture_pattern)));
                        if (0x67 != get8(f)) return (int)(error(f, (int)(VORBIS_missing_capture_pattern)));
                        if (0x67 != get8(f)) return (int)(error(f, (int)(VORBIS_missing_capture_pattern)));
                        if (0x53 != get8(f)) return (int)(error(f, (int)(VORBIS_missing_capture_pattern)));
                        if (start_page_no_capturepattern(f) == 0) return (int)(0);
                        if ((f.page_flag & 1) != 0)
                        {
                            f.last_seg = (int)(0);
                            f.bytes_in_seg = (byte)(0);
                            return (int)(error(f, (int)(VORBIS_continued_packet_flag_invalid)));
                        }
                    }

                    return (int)(start_packet(f));
                }

                public static int next_segment(stb_vorbis f)
                {
                    int len;
                    if ((f.last_seg) != 0) return (int)(0);
                    if ((f.next_seg) == (-1))
                    {
                        f.last_seg_which = (int)(f.segment_count - 1);
                        if (start_page(f) == 0)
                        {
                            f.last_seg = (int)(1);
                            return (int)(0);
                        }
                        if ((f.page_flag & 1) == 0) return (int)(error(f, (int)(VORBIS_continued_packet_flag_invalid)));
                    }

                    len = (int)(f.segments[f.next_seg++]);
                    if ((len) < (255))
                    {
                        f.last_seg = (int)(1);
                        f.last_seg_which = (int)(f.next_seg - 1);
                    }

                    if ((f.next_seg) >= (f.segment_count)) f.next_seg = (int)(-1);
                    f.bytes_in_seg = (byte)(len);
                    return (int)(len);
                }

                public static int get8_packet_raw(stb_vorbis f)
                {
                    if (f.bytes_in_seg == 0)
                    {
                        if ((f.last_seg) != 0) return (int)(-1);
                        else if (next_segment(f) == 0) return (int)(-1);
                    }

                    --f.bytes_in_seg;
                    ++f.packet_bytes;
                    return (int)(get8(f));
                }

                public static int get8_packet(stb_vorbis f)
                {
                    int x = (int)(get8_packet_raw(f));
                    f.valid_bits = (int)(0);
                    return (int)(x);
                }

                public static void flush_packet(stb_vorbis f)
                {
                    while (get8_packet_raw(f) != (-1))
                    {
                    }
                }

                public static void prep_huffman(stb_vorbis f)
                {
                    if (f.valid_bits <= 24)
                    {
                        if ((f.valid_bits) == (0)) f.acc = (uint)(0);
                        do
                        {
                            int z;
                            if (((f.last_seg) != 0) && (f.bytes_in_seg == 0)) return;
                            z = (int)(get8_packet_raw(f));
                            if ((z) == (-1)) return;
                            f.acc += (uint)((uint)(z) << f.valid_bits);
                            f.valid_bits += (int)(8);
                        } while (f.valid_bits <= 24);
                    }

                }

                public static int codebook_decode_scalar_raw(stb_vorbis f, Codebook* c)
                {
                    int i;
                    prep_huffman(f);
                    if (((c->codewords) == (null)) && ((c->sorted_codewords) == (null))) return (int)(-1);
                    if (((c->entries) > (8) ? c->sorted_codewords != (null) : c->codewords == null))
                    {
                        uint code = (uint)(bit_reverse((uint)(f.acc)));
                        int x = (int)(0);
                        int n = (int)(c->sorted_entries);
                        int len;
                        while ((n) > (1))
                        {
                            int m = (int)(x + (n >> 1));
                            if (c->sorted_codewords[m] <= code)
                            {
                                x = (int)(m);
                                n -= (int)(n >> 1);
                            }
                            else
                            {
                                n >>= 1;
                            }
                        }
                        if (c->sparse == 0) x = (int)(c->sorted_values[x]);
                        len = (int)(c->codeword_lengths[x]);
                        if ((f.valid_bits) >= (len))
                        {
                            f.acc >>= len;
                            f.valid_bits -= (int)(len);
                            return (int)(x);
                        }
                        f.valid_bits = (int)(0);
                        return (int)(-1);
                    }

                    for (i = (int)(0); (i) < (c->entries); ++i)
                    {
                        if ((c->codeword_lengths[i]) == (255)) continue;
                        if ((c->codewords[i]) == (f.acc & ((1 << c->codeword_lengths[i]) - 1)))
                        {
                            if ((f.valid_bits) >= (c->codeword_lengths[i]))
                            {
                                f.acc >>= c->codeword_lengths[i];
                                f.valid_bits -= (int)(c->codeword_lengths[i]);
                                return (int)(i);
                            }
                            f.valid_bits = (int)(0);
                            return (int)(-1);
                        }
                    }
                    error(f, (int)(VORBIS_invalid_stream));
                    f.valid_bits = (int)(0);
                    return (int)(-1);
                }

                public static int codebook_decode_scalar(stb_vorbis f, Codebook* c)
                {
                    int i;
                    if ((f.valid_bits) < (10)) prep_huffman(f);
                    i = (int)(f.acc & ((1 << 10) - 1));
                    i = (int)(c->fast_huffman[i]);
                    if ((i) >= (0))
                    {
                        f.acc >>= c->codeword_lengths[i];
                        f.valid_bits -= (int)(c->codeword_lengths[i]);
                        if ((f.valid_bits) < (0))
                        {
                            f.valid_bits = (int)(0);
                            return (int)(-1);
                        }
                        return (int)(i);
                    }

                    return (int)(codebook_decode_scalar_raw(f, c));
                }

                public static int codebook_decode_start(stb_vorbis f, Codebook* c)
                {
                    int z = (int)(-1);
                    if ((c->lookup_type) == (0)) error(f, (int)(VORBIS_invalid_stream));
                    else
                    {
                        z = (int)(codebook_decode_scalar(f, c));
                        if ((c->sparse) != 0)
                            if ((z) < (0))
                            {
                                if (f.bytes_in_seg == 0) if ((f.last_seg) != 0) return (int)(z);
                                error(f, (int)(VORBIS_invalid_stream));
                            }
                    }

                    return (int)(z);
                }

                public static int codebook_decode(stb_vorbis f, Codebook* c, float* output, int len)
                {
                    int i;
                    int z = (int)(codebook_decode_start(f, c));
                    if ((z) < (0)) return (int)(0);
                    if ((len) > (c->dimensions)) len = (int)(c->dimensions);
                    z *= (int)(c->dimensions);
                    if ((c->sequence_p) != 0)
                    {
                        float last = (float)(0);
                        for (i = (int)(0); (i) < (len); ++i)
                        {
                            float val = (float)((c->multiplicands[z + i]) + last);
                            output[i] += (float)(val);
                            last = (float)(val + c->minimum_value);
                        }
                    }
                    else
                    {
                        float last = (float)(0);
                        for (i = (int)(0); (i) < (len); ++i)
                        {
                            output[i] += (float)((c->multiplicands[z + i]) + last);
                        }
                    }

                    return (int)(1);
                }

                public static int codebook_decode_step(stb_vorbis f, Codebook* c, float* output, int len, int step)
                {
                    int i;
                    int z = (int)(codebook_decode_start(f, c));
                    float last = (float)(0);
                    if ((z) < (0)) return (int)(0);
                    if ((len) > (c->dimensions)) len = (int)(c->dimensions);
                    z *= (int)(c->dimensions);
                    for (i = (int)(0); (i) < (len); ++i)
                    {
                        float val = (float)((c->multiplicands[z + i]) + last);
                        output[i * step] += (float)(val);
                        if ((c->sequence_p) != 0) last = (float)(val);
                    }
                    return (int)(1);
                }

                public static int codebook_decode_deinterleave_repeat(stb_vorbis f, Codebook* c, float** outputs, int ch,
                    int* c_inter_p, int* p_inter_p, int len, int total_decode)
                {
                    int c_inter = (int)(*c_inter_p);
                    int p_inter = (int)(*p_inter_p);
                    int i;
                    int z;
                    int effective = (int)(c->dimensions);
                    if ((c->lookup_type) == (0)) return (int)(error(f, (int)(VORBIS_invalid_stream)));
                    while ((total_decode) > (0))
                    {
                        float last = (float)(0);
                        z = (int)(codebook_decode_scalar(f, c));
                        if ((z) < (0))
                        {
                            if (f.bytes_in_seg == 0) if ((f.last_seg) != 0) return (int)(0);
                            return (int)(error(f, (int)(VORBIS_invalid_stream)));
                        }
                        if ((c_inter + p_inter * ch + effective) > (len * ch))
                        {
                            effective = (int)(len * ch - (p_inter * ch - c_inter));
                        }
                        {
                            z *= (int)(c->dimensions);
                            if ((c->sequence_p) != 0)
                            {
                                for (i = (int)(0); (i) < (effective); ++i)
                                {
                                    float val = (float)((c->multiplicands[z + i]) + last);
                                    if ((outputs[c_inter]) != null) outputs[c_inter][p_inter] += (float)(val);
                                    if ((++c_inter) == (ch))
                                    {
                                        c_inter = (int)(0);
                                        ++p_inter;
                                    }
                                    last = (float)(val);
                                }
                            }
                            else
                            {
                                for (i = (int)(0); (i) < (effective); ++i)
                                {
                                    float val = (float)((c->multiplicands[z + i]) + last);
                                    if ((outputs[c_inter]) != null) outputs[c_inter][p_inter] += (float)(val);
                                    if ((++c_inter) == (ch))
                                    {
                                        c_inter = (int)(0);
                                        ++p_inter;
                                    }
                                }
                            }
                        }
                        total_decode -= (int)(effective);
                    }
                    *c_inter_p = (int)(c_inter);
                    *p_inter_p = (int)(p_inter);
                    return (int)(1);
                }

                public static int predict_point(int x, int x0, int x1, int y0, int y1)
                {
                    int dy = (int)(y1 - y0);
                    int adx = (int)(x1 - x0);
                    int err = (int)(CRuntime.abs((int)(dy)) * (x - x0));
                    int off = (int)(err / adx);
                    return (int)((dy) < (0) ? y0 - off : y0 + off);
                }

                public static void draw_line(float* output, int x0, int y0, int x1, int y1, int n)
                {
                    int dy = (int)(y1 - y0);
                    int adx = (int)(x1 - x0);
                    int ady = (int)(CRuntime.abs((int)(dy)));
                    int _base_;
                    int x = (int)(x0);
                    int y = (int)(y0);
                    int err = (int)(0);
                    int sy;
                    _base_ = (int)(dy / adx);
                    if ((dy) < (0)) sy = (int)(_base_ - 1);
                    else sy = (int)(_base_ + 1);
                    ady -= (int)(CRuntime.abs((int)(_base_)) * adx);
                    if ((x1) > (n)) x1 = (int)(n);
                    if ((x) < (x1))
                    {
                        output[x] *= (float)(inverse_db_table[y]);
                        for (++x; (x) < (x1); ++x)
                        {
                            err += (int)(ady);
                            if ((err) >= (adx))
                            {
                                err -= (int)(adx);
                                y += (int)(sy);
                            }
                            else y += (int)(_base_);
                            output[x] *= (float)(inverse_db_table[y]);
                        }
                    }

                }

                public static int residue_decode(stb_vorbis f, Codebook* book, float* target, int offset, int n, int rtype)
                {
                    int k;
                    if ((rtype) == (0))
                    {
                        int step = (int)(n / book->dimensions);
                        for (k = (int)(0); (k) < (step); ++k)
                        {
                            if (codebook_decode_step(f, book, target + offset + k, (int)(n - offset - k), (int)(step)) == 0)
                                return (int)(0);
                        }
                    }
                    else
                    {
                        for (k = (int)(0); (k) < (n);)
                        {
                            if (codebook_decode(f, book, target + offset, (int)(n - k)) == 0) return (int)(0);
                            k += (int)(book->dimensions);
                            offset += (int)(book->dimensions);
                        }
                    }

                    return (int)(1);
                }

                public static void decode_residue(stb_vorbis f, float** residue_buffers, int ch, int n, int rn, byte* do_not_decode)
                {
                    int i;
                    int j;
                    int pass;
                    Residue r = f.residue_config[rn];
                    int rtype = (int)(f.residue_types[rn]);
                    int c = (int)(r.classbook);
                    int classwords = (int)(f.codebooks[c].dimensions);
                    int n_read = (int)(r.end - r.begin);
                    int part_read = (int)(n_read / r.part_size);
                    int temp_alloc_point = (int)((f).temp_offset);
                    byte*** part_classdata =
                        (byte***)
                            (make_block_array(
                                (f.alloc.alloc_buffer != null
                                    ? setup_temp_malloc(f, (int)(f.channels * (sizeof(void*) + (part_read * sizeof(byte*)))))
                                    : CRuntime.malloc((ulong)(f.channels * (sizeof(void*) + (part_read * sizeof(byte*)))))), (int)(f.channels),
                                (int)(part_read * sizeof(byte*))));
                    for (i = (int)(0); (i) < (ch); ++i)
                    {
                        if (do_not_decode[i] == 0) CRuntime.memset(residue_buffers[i], (int)(0), (ulong)(sizeof(float) * n));
                    }
                    if (((rtype) == (2)) && (ch != 1))
                    {
                        for (j = (int)(0); (j) < (ch); ++j)
                        {
                            if (do_not_decode[j] == 0) break;
                        }
                        if ((j) == (ch)) goto done;
                        for (pass = (int)(0); (pass) < (8); ++pass)
                        {
                            int pcount = (int)(0);
                            int class_set = (int)(0);
                            if ((ch) == (2))
                            {
                                while ((pcount) < (part_read))
                                {
                                    int z2 = (int)(r.begin + pcount * r.part_size);
                                    int c_inter = (int)(z2 & 1);
                                    int p_inter = (int)(z2 >> 1);
                                    if ((pass) == (0))
                                    {
                                        Codebook* c2 = f.codebooks + r.classbook;
                                        int q;
                                        q = (int)(codebook_decode_scalar(f, c2));
                                        if ((c2->sparse) != 0) q = (int)(c2->sorted_values[q]);
                                        if ((q) == (-1)) goto done;
                                        part_classdata[0][class_set] = r.classdata[q];
                                    }
                                    for (i = (int)(0); ((i) < (classwords)) && ((pcount) < (part_read)); ++i, ++pcount)
                                    {
                                        int z3 = (int)(r.begin + pcount * r.part_size);
                                        int c2 = (int)(part_classdata[0][class_set][i]);
                                        int b = (int)(r.residue_books[c2, pass]);
                                        if ((b) >= (0))
                                        {
                                            Codebook* book = f.codebooks + b;
                                            if (
                                                codebook_decode_deinterleave_repeat(f, book, (float**)(residue_buffers), (int)(ch), &c_inter, &p_inter,
                                                    (int)(n), (int)(r.part_size)) == 0) goto done;
                                        }
                                        else
                                        {
                                            z3 += (int)(r.part_size);
                                            c_inter = (int)(z3 & 1);
                                            p_inter = (int)(z3 >> 1);
                                        }
                                    }
                                    ++class_set;
                                }
                            }
                            else if ((ch) == (1))
                            {
                                while ((pcount) < (part_read))
                                {
                                    int z2 = (int)(r.begin + pcount * r.part_size);
                                    int c_inter = (int)(0);
                                    int p_inter = (int)(z2);
                                    if ((pass) == (0))
                                    {
                                        Codebook* c2 = f.codebooks + r.classbook;
                                        int q;
                                        q = (int)(codebook_decode_scalar(f, c2));
                                        if ((c2->sparse) != 0) q = (int)(c2->sorted_values[q]);
                                        if ((q) == (-1)) goto done;
                                        part_classdata[0][class_set] = r.classdata[q];
                                    }
                                    for (i = (int)(0); ((i) < (classwords)) && ((pcount) < (part_read)); ++i, ++pcount)
                                    {
                                        int z3 = (int)(r.begin + pcount * r.part_size);
                                        int c2 = (int)(part_classdata[0][class_set][i]);
                                        int b = (int)(r.residue_books[c, pass]);
                                        if ((b) >= (0))
                                        {
                                            Codebook* book = f.codebooks + b;
                                            if (
                                                codebook_decode_deinterleave_repeat(f, book, (float**)(residue_buffers), (int)(ch), &c_inter, &p_inter,
                                                    (int)(n), (int)(r.part_size)) == 0) goto done;
                                        }
                                        else
                                        {
                                            z3 += (int)(r.part_size);
                                            c_inter = (int)(0);
                                            p_inter = (int)(z3);
                                        }
                                    }
                                    ++class_set;
                                }
                            }
                            else
                            {
                                while ((pcount) < (part_read))
                                {
                                    int z2 = (int)(r.begin + pcount * r.part_size);
                                    int c_inter = (int)(z2 % ch);
                                    int p_inter = (int)(z2 / ch);
                                    if ((pass) == (0))
                                    {
                                        Codebook* c2 = f.codebooks + r.classbook;
                                        int q;
                                        q = (int)(codebook_decode_scalar(f, c2));
                                        if ((c2->sparse) != 0) q = (int)(c2->sorted_values[q]);
                                        if ((q) == (-1)) goto done;
                                        part_classdata[0][class_set] = r.classdata[q];
                                    }
                                    for (i = (int)(0); ((i) < (classwords)) && ((pcount) < (part_read)); ++i, ++pcount)
                                    {
                                        int z3 = (int)(r.begin + pcount * r.part_size);
                                        int c2 = (int)(part_classdata[0][class_set][i]);
                                        int b = (int)(r.residue_books[c, pass]);
                                        if ((b) >= (0))
                                        {
                                            Codebook* book = f.codebooks + b;
                                            if (
                                                codebook_decode_deinterleave_repeat(f, book, (float**)(residue_buffers), (int)(ch), &c_inter, &p_inter,
                                                    (int)(n), (int)(r.part_size)) == 0) goto done;
                                        }
                                        else
                                        {
                                            z3 += (int)(r.part_size);
                                            c_inter = (int)(z3 % ch);
                                            p_inter = (int)(z3 / ch);
                                        }
                                    }
                                    ++class_set;
                                }
                            }
                        }
                        goto done;
                    }

                    for (pass = (int)(0); (pass) < (8); ++pass)
                    {
                        int pcount = (int)(0);
                        int class_set = (int)(0);
                        while ((pcount) < (part_read))
                        {
                            if ((pass) == (0))
                            {
                                for (j = (int)(0); (j) < (ch); ++j)
                                {
                                    if (do_not_decode[j] == 0)
                                    {
                                        Codebook* c2 = f.codebooks + r.classbook;
                                        int temp;
                                        temp = (int)(codebook_decode_scalar(f, c2));
                                        if ((c2->sparse) != 0) temp = (int)(c2->sorted_values[temp]);
                                        if ((temp) == (-1)) goto done;
                                        part_classdata[j][class_set] = r.classdata[temp];
                                    }
                                }
                            }
                            for (i = (int)(0); ((i) < (classwords)) && ((pcount) < (part_read)); ++i, ++pcount)
                            {
                                for (j = (int)(0); (j) < (ch); ++j)
                                {
                                    if (do_not_decode[j] == 0)
                                    {
                                        int c2 = (int)(part_classdata[j][class_set][i]);
                                        int b = (int)(r.residue_books[c, pass]);
                                        if ((b) >= (0))
                                        {
                                            float* target = residue_buffers[j];
                                            int offset = (int)(r.begin + pcount * r.part_size);
                                            int n2 = (int)(r.part_size);
                                            Codebook* book = f.codebooks + b;
                                            if (residue_decode(f, book, target, (int)(offset), (int)(n2), (int)(rtype)) == 0) goto done;
                                        }
                                    }
                                }
                            }
                            ++class_set;
                        }
                    }
                    done:
                    ;

                    CRuntime.free(part_classdata);
                    (f).temp_offset = (int)(temp_alloc_point);
                }

                public static void imdct_step3_iter0_loop(int n, float* e, int i_off, int k_off, float* A)
                {
                    float* ee0 = e + i_off;
                    float* ee2 = ee0 + k_off;
                    int i;
                    for (i = (int)(n >> 2); (i) > (0); --i)
                    {
                        float k00_20;
                        float k01_21;
                        k00_20 = (float)(ee0[0] - ee2[0]);
                        k01_21 = (float)(ee0[-1] - ee2[-1]);
                        ee0[0] += (float)(ee2[0]);
                        ee0[-1] += (float)(ee2[-1]);
                        ee2[0] = (float)(k00_20 * A[0] - k01_21 * A[1]);
                        ee2[-1] = (float)(k01_21 * A[0] + k00_20 * A[1]);
                        A += 8;
                        k00_20 = (float)(ee0[-2] - ee2[-2]);
                        k01_21 = (float)(ee0[-3] - ee2[-3]);
                        ee0[-2] += (float)(ee2[-2]);
                        ee0[-3] += (float)(ee2[-3]);
                        ee2[-2] = (float)(k00_20 * A[0] - k01_21 * A[1]);
                        ee2[-3] = (float)(k01_21 * A[0] + k00_20 * A[1]);
                        A += 8;
                        k00_20 = (float)(ee0[-4] - ee2[-4]);
                        k01_21 = (float)(ee0[-5] - ee2[-5]);
                        ee0[-4] += (float)(ee2[-4]);
                        ee0[-5] += (float)(ee2[-5]);
                        ee2[-4] = (float)(k00_20 * A[0] - k01_21 * A[1]);
                        ee2[-5] = (float)(k01_21 * A[0] + k00_20 * A[1]);
                        A += 8;
                        k00_20 = (float)(ee0[-6] - ee2[-6]);
                        k01_21 = (float)(ee0[-7] - ee2[-7]);
                        ee0[-6] += (float)(ee2[-6]);
                        ee0[-7] += (float)(ee2[-7]);
                        ee2[-6] = (float)(k00_20 * A[0] - k01_21 * A[1]);
                        ee2[-7] = (float)(k01_21 * A[0] + k00_20 * A[1]);
                        A += 8;
                        ee0 -= 8;
                        ee2 -= 8;
                    }
                }

                public static void imdct_step3_inner_r_loop(int lim, float* e, int d0, int k_off, float* A, int k1)
                {
                    int i;
                    float k00_20;
                    float k01_21;
                    float* e0 = e + d0;
                    float* e2 = e0 + k_off;
                    for (i = (int)(lim >> 2); (i) > (0); --i)
                    {
                        k00_20 = (float)(e0[-0] - e2[-0]);
                        k01_21 = (float)(e0[-1] - e2[-1]);
                        e0[-0] += (float)(e2[-0]);
                        e0[-1] += (float)(e2[-1]);
                        e2[-0] = (float)((k00_20) * A[0] - (k01_21) * A[1]);
                        e2[-1] = (float)((k01_21) * A[0] + (k00_20) * A[1]);
                        A += k1;
                        k00_20 = (float)(e0[-2] - e2[-2]);
                        k01_21 = (float)(e0[-3] - e2[-3]);
                        e0[-2] += (float)(e2[-2]);
                        e0[-3] += (float)(e2[-3]);
                        e2[-2] = (float)((k00_20) * A[0] - (k01_21) * A[1]);
                        e2[-3] = (float)((k01_21) * A[0] + (k00_20) * A[1]);
                        A += k1;
                        k00_20 = (float)(e0[-4] - e2[-4]);
                        k01_21 = (float)(e0[-5] - e2[-5]);
                        e0[-4] += (float)(e2[-4]);
                        e0[-5] += (float)(e2[-5]);
                        e2[-4] = (float)((k00_20) * A[0] - (k01_21) * A[1]);
                        e2[-5] = (float)((k01_21) * A[0] + (k00_20) * A[1]);
                        A += k1;
                        k00_20 = (float)(e0[-6] - e2[-6]);
                        k01_21 = (float)(e0[-7] - e2[-7]);
                        e0[-6] += (float)(e2[-6]);
                        e0[-7] += (float)(e2[-7]);
                        e2[-6] = (float)((k00_20) * A[0] - (k01_21) * A[1]);
                        e2[-7] = (float)((k01_21) * A[0] + (k00_20) * A[1]);
                        e0 -= 8;
                        e2 -= 8;
                        A += k1;
                    }
                }

                public static void imdct_step3_inner_s_loop(int n, float* e, int i_off, int k_off, float* A, int a_off, int k0)
                {
                    int i;
                    float A0 = (float)(A[0]);
                    float A1 = (float)(A[0 + 1]);
                    float A2 = (float)(A[0 + a_off]);
                    float A3 = (float)(A[0 + a_off + 1]);
                    float A4 = (float)(A[0 + a_off * 2 + 0]);
                    float A5 = (float)(A[0 + a_off * 2 + 1]);
                    float A6 = (float)(A[0 + a_off * 3 + 0]);
                    float A7 = (float)(A[0 + a_off * 3 + 1]);
                    float k00;
                    float k11;
                    float* ee0 = e + i_off;
                    float* ee2 = ee0 + k_off;
                    for (i = (int)(n); (i) > (0); --i)
                    {
                        k00 = (float)(ee0[0] - ee2[0]);
                        k11 = (float)(ee0[-1] - ee2[-1]);
                        ee0[0] = (float)(ee0[0] + ee2[0]);
                        ee0[-1] = (float)(ee0[-1] + ee2[-1]);
                        ee2[0] = (float)((k00) * A0 - (k11) * A1);
                        ee2[-1] = (float)((k11) * A0 + (k00) * A1);
                        k00 = (float)(ee0[-2] - ee2[-2]);
                        k11 = (float)(ee0[-3] - ee2[-3]);
                        ee0[-2] = (float)(ee0[-2] + ee2[-2]);
                        ee0[-3] = (float)(ee0[-3] + ee2[-3]);
                        ee2[-2] = (float)((k00) * A2 - (k11) * A3);
                        ee2[-3] = (float)((k11) * A2 + (k00) * A3);
                        k00 = (float)(ee0[-4] - ee2[-4]);
                        k11 = (float)(ee0[-5] - ee2[-5]);
                        ee0[-4] = (float)(ee0[-4] + ee2[-4]);
                        ee0[-5] = (float)(ee0[-5] + ee2[-5]);
                        ee2[-4] = (float)((k00) * A4 - (k11) * A5);
                        ee2[-5] = (float)((k11) * A4 + (k00) * A5);
                        k00 = (float)(ee0[-6] - ee2[-6]);
                        k11 = (float)(ee0[-7] - ee2[-7]);
                        ee0[-6] = (float)(ee0[-6] + ee2[-6]);
                        ee0[-7] = (float)(ee0[-7] + ee2[-7]);
                        ee2[-6] = (float)((k00) * A6 - (k11) * A7);
                        ee2[-7] = (float)((k11) * A6 + (k00) * A7);
                        ee0 -= k0;
                        ee2 -= k0;
                    }
                }

                public static void iter_54(float* z)
                {
                    float k00;
                    float k11;
                    float k22;
                    float k33;
                    float y0;
                    float y1;
                    float y2;
                    float y3;
                    k00 = (float)(z[0] - z[-4]);
                    y0 = (float)(z[0] + z[-4]);
                    y2 = (float)(z[-2] + z[-6]);
                    k22 = (float)(z[-2] - z[-6]);
                    z[-0] = (float)(y0 + y2);
                    z[-2] = (float)(y0 - y2);
                    k33 = (float)(z[-3] - z[-7]);
                    z[-4] = (float)(k00 + k33);
                    z[-6] = (float)(k00 - k33);
                    k11 = (float)(z[-1] - z[-5]);
                    y1 = (float)(z[-1] + z[-5]);
                    y3 = (float)(z[-3] + z[-7]);
                    z[-1] = (float)(y1 + y3);
                    z[-3] = (float)(y1 - y3);
                    z[-5] = (float)(k11 - k22);
                    z[-7] = (float)(k11 + k22);
                }

                public static void imdct_step3_inner_s_loop_ld654(int n, float* e, int i_off, float* A, int base_n)
                {
                    int a_off = (int)(base_n >> 3);
                    float A2 = (float)(A[0 + a_off]);
                    float* z = e + i_off;
                    float* _base_ = z - 16 * n;
                    while ((z) > (_base_))
                    {
                        float k00;
                        float k11;
                        k00 = (float)(z[-0] - z[-8]);
                        k11 = (float)(z[-1] - z[-9]);
                        z[-0] = (float)(z[-0] + z[-8]);
                        z[-1] = (float)(z[-1] + z[-9]);
                        z[-8] = (float)(k00);
                        z[-9] = (float)(k11);
                        k00 = (float)(z[-2] - z[-10]);
                        k11 = (float)(z[-3] - z[-11]);
                        z[-2] = (float)(z[-2] + z[-10]);
                        z[-3] = (float)(z[-3] + z[-11]);
                        z[-10] = (float)((k00 + k11) * A2);
                        z[-11] = (float)((k11 - k00) * A2);
                        k00 = (float)(z[-12] - z[-4]);
                        k11 = (float)(z[-5] - z[-13]);
                        z[-4] = (float)(z[-4] + z[-12]);
                        z[-5] = (float)(z[-5] + z[-13]);
                        z[-12] = (float)(k11);
                        z[-13] = (float)(k00);
                        k00 = (float)(z[-14] - z[-6]);
                        k11 = (float)(z[-7] - z[-15]);
                        z[-6] = (float)(z[-6] + z[-14]);
                        z[-7] = (float)(z[-7] + z[-15]);
                        z[-14] = (float)((k00 + k11) * A2);
                        z[-15] = (float)((k00 - k11) * A2);
                        iter_54(z);
                        iter_54(z - 8);
                        z -= 16;
                    }
                }

                public static void inverse_mdct(float* buffer, int n, stb_vorbis f, int blocktype)
                {
                    int n2 = (int)(n >> 1);
                    int n4 = (int)(n >> 2);
                    int n8 = (int)(n >> 3);
                    int l;
                    int ld;
                    int save_point = (int)((f).temp_offset);
                    float* buf2 =
                        (float*)
                            (f.alloc.alloc_buffer != null
                                ? setup_temp_malloc(f, (int)(n2 * sizeof(float)))
                                : CRuntime.malloc((ulong)(n2 * sizeof(float))));
                    float* u = (null);
                    float* v = (null);
                    float* A = f.A[blocktype];
                    {
                        float* d;
                        float* e;
                        float* AA;
                        float* e_stop;
                        d = &buf2[n2 - 2];
                        AA = A;
                        e = &buffer[0];
                        e_stop = &buffer[n2];
                        while (e != e_stop)
                        {
                            d[1] = (float)(e[0] * AA[0] - e[2] * AA[1]);
                            d[0] = (float)(e[0] * AA[1] + e[2] * AA[0]);
                            d -= 2;
                            AA += 2;
                            e += 4;
                        }
                        e = &buffer[n2 - 3];
                        while ((d) >= (buf2))
                        {
                            d[1] = (float)(-e[2] * AA[0] - -e[0] * AA[1]);
                            d[0] = (float)(-e[2] * AA[1] + -e[0] * AA[0]);
                            d -= 2;
                            AA += 2;
                            e -= 4;
                        }
                    }

                    u = buffer;
                    v = buf2;
                    {
                        float* AA = &A[n2 - 8];
                        float* d0;
                        float* d1;
                        float* e0;
                        float* e1;
                        e0 = &v[n4];
                        e1 = &v[0];
                        d0 = &u[n4];
                        d1 = &u[0];
                        while ((AA) >= (A))
                        {
                            float v40_20;
                            float v41_21;
                            v41_21 = (float)(e0[1] - e1[1]);
                            v40_20 = (float)(e0[0] - e1[0]);
                            d0[1] = (float)(e0[1] + e1[1]);
                            d0[0] = (float)(e0[0] + e1[0]);
                            d1[1] = (float)(v41_21 * AA[4] - v40_20 * AA[5]);
                            d1[0] = (float)(v40_20 * AA[4] + v41_21 * AA[5]);
                            v41_21 = (float)(e0[3] - e1[3]);
                            v40_20 = (float)(e0[2] - e1[2]);
                            d0[3] = (float)(e0[3] + e1[3]);
                            d0[2] = (float)(e0[2] + e1[2]);
                            d1[3] = (float)(v41_21 * AA[0] - v40_20 * AA[1]);
                            d1[2] = (float)(v40_20 * AA[0] + v41_21 * AA[1]);
                            AA -= 8;
                            d0 += 4;
                            d1 += 4;
                            e0 += 4;
                            e1 += 4;
                        }
                    }

                    ld = (int)(ilog((int)(n)) - 1);
                    imdct_step3_iter0_loop((int)(n >> 4), u, (int)(n2 - 1 - n4 * 0), (int)(-(n >> 3)), A);
                    imdct_step3_iter0_loop((int)(n >> 4), u, (int)(n2 - 1 - n4 * 1), (int)(-(n >> 3)), A);
                    imdct_step3_inner_r_loop((int)(n >> 5), u, (int)(n2 - 1 - n8 * 0), (int)(-(n >> 4)), A, (int)(16));
                    imdct_step3_inner_r_loop((int)(n >> 5), u, (int)(n2 - 1 - n8 * 1), (int)(-(n >> 4)), A, (int)(16));
                    imdct_step3_inner_r_loop((int)(n >> 5), u, (int)(n2 - 1 - n8 * 2), (int)(-(n >> 4)), A, (int)(16));
                    imdct_step3_inner_r_loop((int)(n >> 5), u, (int)(n2 - 1 - n8 * 3), (int)(-(n >> 4)), A, (int)(16));
                    l = (int)(2);
                    for (; (l) < ((ld - 3) >> 1); ++l)
                    {
                        int k0 = (int)(n >> (l + 2));
                        int k0_2 = (int)(k0 >> 1);
                        int lim = (int)(1 << (l + 1));
                        int i;
                        for (i = (int)(0); (i) < (lim); ++i)
                        {
                            imdct_step3_inner_r_loop((int)(n >> (l + 4)), u, (int)(n2 - 1 - k0 * i), (int)(-k0_2), A, (int)(1 << (l + 3)));
                        }
                    }
                    for (; (l) < (ld - 6); ++l)
                    {
                        int k0 = (int)(n >> (l + 2));
                        int k1 = (int)(1 << (l + 3));
                        int k0_2 = (int)(k0 >> 1);
                        int rlim = (int)(n >> (l + 6));
                        int r;
                        int lim = (int)(1 << (l + 1));
                        int i_off;
                        float* A0 = A;
                        i_off = (int)(n2 - 1);
                        for (r = (int)(rlim); (r) > (0); --r)
                        {
                            imdct_step3_inner_s_loop((int)(lim), u, (int)(i_off), (int)(-k0_2), A0, (int)(k1), (int)(k0));
                            A0 += k1 * 4;
                            i_off -= (int)(8);
                        }
                    }
                    imdct_step3_inner_s_loop_ld654((int)(n >> 5), u, (int)(n2 - 1), A, (int)(n));
                    {
                        ushort* bitrev = f.bit_reverse[blocktype];
                        float* d0 = &v[n4 - 4];
                        float* d1 = &v[n2 - 4];
                        while ((d0) >= (v))
                        {
                            int k4;
                            k4 = (int)(bitrev[0]);
                            d1[3] = (float)(u[k4 + 0]);
                            d1[2] = (float)(u[k4 + 1]);
                            d0[3] = (float)(u[k4 + 2]);
                            d0[2] = (float)(u[k4 + 3]);
                            k4 = (int)(bitrev[1]);
                            d1[1] = (float)(u[k4 + 0]);
                            d1[0] = (float)(u[k4 + 1]);
                            d0[1] = (float)(u[k4 + 2]);
                            d0[0] = (float)(u[k4 + 3]);
                            d0 -= 4;
                            d1 -= 4;
                            bitrev += 2;
                        }
                    }

                    {
                        float* C = f.C[blocktype];
                        float* d;
                        float* e;
                        d = v;
                        e = v + n2 - 4;
                        while ((d) < (e))
                        {
                            float a02;
                            float a11;
                            float b0;
                            float b1;
                            float b2;
                            float b3;
                            a02 = (float)(d[0] - e[2]);
                            a11 = (float)(d[1] + e[3]);
                            b0 = (float)(C[1] * a02 + C[0] * a11);
                            b1 = (float)(C[1] * a11 - C[0] * a02);
                            b2 = (float)(d[0] + e[2]);
                            b3 = (float)(d[1] - e[3]);
                            d[0] = (float)(b2 + b0);
                            d[1] = (float)(b3 + b1);
                            e[2] = (float)(b2 - b0);
                            e[3] = (float)(b1 - b3);
                            a02 = (float)(d[2] - e[0]);
                            a11 = (float)(d[3] + e[1]);
                            b0 = (float)(C[3] * a02 + C[2] * a11);
                            b1 = (float)(C[3] * a11 - C[2] * a02);
                            b2 = (float)(d[2] + e[0]);
                            b3 = (float)(d[3] - e[1]);
                            d[2] = (float)(b2 + b0);
                            d[3] = (float)(b3 + b1);
                            e[0] = (float)(b2 - b0);
                            e[1] = (float)(b1 - b3);
                            C += 4;
                            d += 4;
                            e -= 4;
                        }
                    }

                    {
                        float* d0;
                        float* d1;
                        float* d2;
                        float* d3;
                        float* B = f.B[blocktype] + n2 - 8;
                        float* e = buf2 + n2 - 8;
                        d0 = &buffer[0];
                        d1 = &buffer[n2 - 4];
                        d2 = &buffer[n2];
                        d3 = &buffer[n - 4];
                        while ((e) >= (v))
                        {
                            float p0;
                            float p1;
                            float p2;
                            float p3;
                            p3 = (float)(e[6] * B[7] - e[7] * B[6]);
                            p2 = (float)(-e[6] * B[6] - e[7] * B[7]);
                            d0[0] = (float)(p3);
                            d1[3] = (float)(-p3);
                            d2[0] = (float)(p2);
                            d3[3] = (float)(p2);
                            p1 = (float)(e[4] * B[5] - e[5] * B[4]);
                            p0 = (float)(-e[4] * B[4] - e[5] * B[5]);
                            d0[1] = (float)(p1);
                            d1[2] = (float)(-p1);
                            d2[1] = (float)(p0);
                            d3[2] = (float)(p0);
                            p3 = (float)(e[2] * B[3] - e[3] * B[2]);
                            p2 = (float)(-e[2] * B[2] - e[3] * B[3]);
                            d0[2] = (float)(p3);
                            d1[1] = (float)(-p3);
                            d2[2] = (float)(p2);
                            d3[1] = (float)(p2);
                            p1 = (float)(e[0] * B[1] - e[1] * B[0]);
                            p0 = (float)(-e[0] * B[0] - e[1] * B[1]);
                            d0[3] = (float)(p1);
                            d1[0] = (float)(-p1);
                            d2[3] = (float)(p0);
                            d3[0] = (float)(p0);
                            B -= 8;
                            e -= 8;
                            d0 += 4;
                            d2 += 4;
                            d1 -= 4;
                            d3 -= 4;
                        }
                    }

                    CRuntime.free(buf2);
                    (f).temp_offset = (int)(save_point);
                }

                public static float* get_window(stb_vorbis f, int len)
                {
                    len <<= 1;
                    if ((len) == (f.blocksize_0)) return f.window[0];
                    if ((len) == (f.blocksize_1)) return f.window[1];
                    return (null);
                }

                public static int do_floor(stb_vorbis f, Mapping* map, int i, int n, float* target, short* finalY, byte* step2_flag)
                {
                    int n2 = (int)(n >> 1);
                    int s = (int)(map->chan[i].mux);
                    int floor;
                    floor = (int)(map->submap_floor[s]);
                    if ((f.floor_types[floor]) == (0))
                    {
                        return (int)(error(f, (int)(VORBIS_invalid_stream)));
                    }
                    else
                    {
                        Floor1* g = &f.floor_config[floor].floor1;
                        int j;
                        int q;
                        int lx = (int)(0);
                        int ly = (int)(finalY[0] * g->floor1_multiplier);
                        for (q = (int)(1); (q) < (g->values); ++q)
                        {
                            j = (int)(g->sorted_order[q]);
                            if ((finalY[j]) >= (0))
                            {
                                int hy = (int)(finalY[j] * g->floor1_multiplier);
                                int hx = (int)(g->Xlist[j]);
                                if (lx != hx) draw_line(target, (int)(lx), (int)(ly), (int)(hx), (int)(hy), (int)(n2));
                                lx = (int)(hx);
                                ly = (int)(hy);
                            }
                        }
                        if ((lx) < (n2))
                        {
                            for (j = (int)(lx); (j) < (n2); ++j)
                            {
                                target[j] *= (float)(inverse_db_table[ly]);
                            }
                        }
                    }

                    return (int)(1);
                }

                public static int vorbis_decode_initial(stb_vorbis f, int* p_left_start, int* p_left_end, int* p_right_start,
                    int* p_right_end, int* mode)
                {
                    Mode* m;
                    int i;
                    int n;
                    int prev;
                    int next;
                    int window_center;
                    f.channel_buffer_start = (int)(f.channel_buffer_end = (int)(0));
                    retry:
                    ;
                    if ((f.eof) != 0) return (int)(0);
                    if (maybe_start_packet(f) == 0) return (int)(0);
                    if (get_bits(f, (int)(1)) != 0)
                    {
                        if (((f).push_mode) != 0) return (int)(error(f, (int)(VORBIS_bad_packet_type)));
                        while ((-1) != get8_packet(f))
                        {
                        }
                        goto retry;
                    }

                    i = (int)(get_bits(f, (int)(ilog((int)(f.mode_count - 1)))));
                    if ((i) == (-1)) return (int)(0);
                    if ((i) >= (f.mode_count)) return (int)(0);
                    *mode = (int)(i);
                    m = (Mode*)f.mode_config + i;
                    if ((m->blockflag) != 0)
                    {
                        n = (int)(f.blocksize_1);
                        prev = (int)(get_bits(f, (int)(1)));
                        next = (int)(get_bits(f, (int)(1)));
                    }
                    else
                    {
                        prev = (int)(next = (int)(0));
                        n = (int)(f.blocksize_0);
                    }

                    window_center = (int)(n >> 1);
                    if (((m->blockflag) != 0) && (prev == 0))
                    {
                        *p_left_start = (int)((n - f.blocksize_0) >> 2);
                        *p_left_end = (int)((n + f.blocksize_0) >> 2);
                    }
                    else
                    {
                        *p_left_start = (int)(0);
                        *p_left_end = (int)(window_center);
                    }

                    if (((m->blockflag) != 0) && (next == 0))
                    {
                        *p_right_start = (int)((n * 3 - f.blocksize_0) >> 2);
                        *p_right_end = (int)((n * 3 + f.blocksize_0) >> 2);
                    }
                    else
                    {
                        *p_right_start = (int)(window_center);
                        *p_right_end = (int)(n);
                    }

                    return (int)(1);
                }

                public static int vorbis_decode_packet_rest(stb_vorbis f, int* len, Mode* m, int left_start, int left_end,
                    int right_start, int right_end, int* p_left)
                {
                    Mapping* map;
                    int i;
                    int j;
                    int k;
                    int n;
                    int n2;
                    int* zero_channel = stackalloc int[256];
                    int* really_zero_channel = stackalloc int[256];
                    n = (int)(f.blocksize[m->blockflag]);
                    map = &f.mapping[m->mapping];
                    n2 = (int)(n >> 1);
                    for (i = (int)(0); (i) < (f.channels); ++i)
                    {
                        int s = (int)(map->chan[i].mux);
                        int floor;
                        zero_channel[i] = (int)(0);
                        floor = (int)(map->submap_floor[s]);
                        if ((f.floor_types[floor]) == (0))
                        {
                            return (int)(error(f, (int)(VORBIS_invalid_stream)));
                        }
                        else
                        {
                            Floor1* g = &f.floor_config[floor].floor1;
                            if ((get_bits(f, (int)(1))) != 0)
                            {
                                short* finalY;
                                byte* step2_flag = stackalloc byte[256];
                                int* range_list = stackalloc int[4];
                                range_list[0] = (int)(256);
                                range_list[1] = (int)(128);
                                range_list[2] = (int)(86);
                                range_list[3] = (int)(64);
                                int range = (int)(range_list[g->floor1_multiplier - 1]);
                                int offset = (int)(2);
                                finalY = f.finalY[i];
                                finalY[0] = (short)(get_bits(f, (int)(ilog((int)(range)) - 1)));
                                finalY[1] = (short)(get_bits(f, (int)(ilog((int)(range)) - 1)));
                                for (j = (int)(0); (j) < (g->partitions); ++j)
                                {
                                    int pclass = (int)(g->partition_class_list[j]);
                                    int cdim = (int)(g->class_dimensions[pclass]);
                                    int cbits = (int)(g->class_subclasses[pclass]);
                                    int csub = (int)((1 << cbits) - 1);
                                    int cval = (int)(0);
                                    if ((cbits) != 0)
                                    {
                                        Codebook* c = f.codebooks + g->class_masterbooks[pclass];
                                        cval = (int)(codebook_decode_scalar(f, c));
                                        if ((c->sparse) != 0) cval = (int)(c->sorted_values[cval]);
                                    }
                                    for (k = (int)(0); (k) < (cdim); ++k)
                                    {
                                        int book = (int)(g->subclass_books[pclass * 8 + (cval & csub)]);
                                        cval = (int)(cval >> cbits);
                                        if ((book) >= (0))
                                        {
                                            int temp;
                                            Codebook* c = f.codebooks + book;
                                            temp = (int)(codebook_decode_scalar(f, c));
                                            if ((c->sparse) != 0) temp = (int)(c->sorted_values[temp]);
                                            finalY[offset++] = (short)(temp);
                                        }
                                        else finalY[offset++] = (short)(0);
                                    }
                                }
                                if ((f.valid_bits) == (-1))
                                {
                                    zero_channel[i] = (int)(1);
                                    goto error;
                                }
                                step2_flag[0] = (byte)(step2_flag[1] = (byte)(1));
                                for (j = (int)(2); (j) < (g->values); ++j)
                                {
                                    int low;
                                    int high;
                                    int pred;
                                    int highroom;
                                    int lowroom;
                                    int room;
                                    int val;
                                    low = (int)(g->neighbors[j * 2 + 0]);
                                    high = (int)(g->neighbors[j * 2 + 1]);
                                    pred =
                                        (int)
                                            (predict_point((int)(g->Xlist[j]), (int)(g->Xlist[low]), (int)(g->Xlist[high]), (int)(finalY[low]),
                                                (int)(finalY[high])));
                                    val = (int)(finalY[j]);
                                    highroom = (int)(range - pred);
                                    lowroom = (int)(pred);
                                    if ((highroom) < (lowroom)) room = (int)(highroom * 2);
                                    else room = (int)(lowroom * 2);
                                    if ((val) != 0)
                                    {
                                        step2_flag[low] = (byte)(step2_flag[high] = (byte)(1));
                                        step2_flag[j] = (byte)(1);
                                        if ((val) >= (room))
                                            if ((highroom) > (lowroom)) finalY[j] = (short)(val - lowroom + pred);
                                            else finalY[j] = (short)(pred - val + highroom - 1);
                                        else if ((val & 1) != 0) finalY[j] = (short)(pred - ((val + 1) >> 1));
                                        else finalY[j] = (short)(pred + (val >> 1));
                                    }
                                    else
                                    {
                                        step2_flag[j] = (byte)(0);
                                        finalY[j] = (short)(pred);
                                    }
                                }
                                for (j = (int)(0); (j) < (g->values); ++j)
                                {
                                    if (step2_flag[j] == 0) finalY[j] = (short)(-1);
                                }
                            }
                            else
                            {
                                zero_channel[i] = (int)(1);
                            }
                            error:
                            ;
                        }
                    }
                    CRuntime.memcpy(really_zero_channel, zero_channel, (ulong)(sizeof(int) * f.channels));
                    for (i = (int)(0); (i) < (map->coupling_steps); ++i)
                    {
                        if ((zero_channel[map->chan[i].magnitude] == 0) || (zero_channel[map->chan[i].angle] == 0))
                        {
                            zero_channel[map->chan[i].magnitude] = (int)(zero_channel[map->chan[i].angle] = (int)(0));
                        }
                    }
                    for (i = (int)(0); (i) < (map->submaps); ++i)
                    {
                        float** residue_buffers = stackalloc float*[16];
                        int r;
                        byte* do_not_decode = stackalloc byte[256];
                        int ch = (int)(0);
                        for (j = (int)(0); (j) < (f.channels); ++j)
                        {
                            if ((map->chan[j].mux) == (i))
                            {
                                if ((zero_channel[j]) != 0)
                                {
                                    do_not_decode[ch] = (byte)(1);
                                    residue_buffers[ch] = (null);
                                }
                                else
                                {
                                    do_not_decode[ch] = (byte)(0);
                                    residue_buffers[ch] = f.channel_buffers[j];
                                }
                                ++ch;
                            }
                        }
                        r = (int)(map->submap_residue[i]);
                        decode_residue(f, residue_buffers, (int)(ch), (int)(n2), (int)(r), do_not_decode);
                    }
                    for (i = (int)(map->coupling_steps - 1); (i) >= (0); --i)
                    {
                        int n3 = (int)(n >> 1);
                        float* m3 = f.channel_buffers[map->chan[i].magnitude];
                        float* a = f.channel_buffers[map->chan[i].angle];
                        for (j = (int)(0); (j) < (n3); ++j)
                        {
                            float a2;
                            float m2;
                            if ((m3[j]) > (0))
                                if ((a[j]) > (0))
                                {
                                    m2 = (float)(m3[j]);
                                    a2 = (float)(m3[j] - a[j]);
                                }
                                else
                                {
                                    a2 = (float)(m3[j]);
                                    m2 = (float)(m3[j] + a[j]);
                                }
                            else if ((a[j]) > (0))
                            {
                                m2 = (float)(m3[j]);
                                a2 = (float)(m3[j] + a[j]);
                            }
                            else
                            {
                                a2 = (float)(m3[j]);
                                m2 = (float)(m3[j] - a[j]);
                            }
                            m3[j] = (float)(m2);
                            a[j] = (float)(a2);
                        }
                    }
                    for (i = (int)(0); (i) < (f.channels); ++i)
                    {
                        if ((really_zero_channel[i]) != 0)
                        {
                            CRuntime.memset(f.channel_buffers[i], (int)(0), (ulong)(sizeof(float) * n2));
                        }
                        else
                        {
                            do_floor(f, map, (int)(i), (int)(n), f.channel_buffers[i], f.finalY[i], (null));
                        }
                    }
                    for (i = (int)(0); (i) < (f.channels); ++i)
                    {
                        inverse_mdct(f.channel_buffers[i], (int)(n), f, (int)(m->blockflag));
                    }
                    flush_packet(f);
                    if ((f.first_decode) != 0)
                    {
                        f.current_loc = (uint)(-n2);
                        f.discard_samples_deferred = (int)(n - right_end);
                        f.current_loc_valid = (int)(1);
                        f.first_decode = (byte)(0);
                    }
                    else if ((f.discard_samples_deferred) != 0)
                    {
                        if ((f.discard_samples_deferred) >= (right_start - left_start))
                        {
                            f.discard_samples_deferred -= (int)(right_start - left_start);
                            left_start = (int)(right_start);
                            *p_left = (int)(left_start);
                        }
                        else
                        {
                            left_start += (int)(f.discard_samples_deferred);
                            *p_left = (int)(left_start);
                            f.discard_samples_deferred = (int)(0);
                        }
                    }
                    else if (((f.previous_length) == (0)) && ((f.current_loc_valid) != 0))
                    {
                    }

                    if ((f.last_seg_which) == (f.end_seg_with_known_loc))
                    {
                        if (((f.current_loc_valid) != 0) && ((f.page_flag & 4) != 0))
                        {
                            uint current_end = (uint)(f.known_loc_for_packet - (n - right_end));
                            if ((current_end) < (f.current_loc + (right_end - left_start)))
                            {
                                if ((current_end) < (f.current_loc))
                                {
                                    *len = (int)(0);
                                }
                                else
                                {
                                    *len = (int)(current_end - f.current_loc);
                                }
                                *len += (int)(left_start);
                                if ((*len) > (right_end)) *len = (int)(right_end);
                                f.current_loc += (uint)(*len);
                                return (int)(1);
                            }
                        }
                        f.current_loc = (uint)(f.known_loc_for_packet - (n2 - left_start));
                        f.current_loc_valid = (int)(1);
                    }

                    if ((f.current_loc_valid) != 0) f.current_loc += (uint)(right_start - left_start);
                    if ((f.alloc.alloc_buffer) != null)
                        *len = (int)(right_end);
                    return (int)(1);
                }

                public static int vorbis_decode_packet(stb_vorbis f, int* len, int* p_left, int* p_right)
                {
                    int mode;
                    int left_end;
                    int right_end;
                    if (vorbis_decode_initial(f, p_left, &left_end, p_right, &right_end, &mode) == 0) return (int)(0);
                    return
                        (int)
                            (vorbis_decode_packet_rest(f, len, (Mode*)f.mode_config + mode, (int)(*p_left), (int)(left_end),
                                (int)(*p_right), (int)(right_end), p_left));
                }

                public static int vorbis_finish_frame(stb_vorbis f, int len, int left, int right)
                {
                    int prev;
                    int i;
                    int j;
                    if ((f.previous_length) != 0)
                    {
                        int i2;
                        int j2;
                        int n = (int)(f.previous_length);
                        float* w = get_window(f, (int)(n));
                        for (i2 = (int)(0); (i2) < (f.channels); ++i2)
                        {
                            for (j2 = (int)(0); (j2) < (n); ++j2)
                            {
                                f.channel_buffers[i2][left + j2] =
                                    (float)(f.channel_buffers[i2][left + j2] * w[j2] + f.previous_window[i2][j2] * w[n - 1 - j2]);
                            }
                        }
                    }

                    prev = (int)(f.previous_length);
                    f.previous_length = (int)(len - right);
                    for (i = (int)(0); (i) < (f.channels); ++i)
                    {
                        for (j = (int)(0); (right + j) < (len); ++j)
                        {
                            f.previous_window[i][j] = (float)(f.channel_buffers[i][right + j]);
                        }
                    }
                    if (prev == 0) return (int)(0);
                    if ((len) < (right)) right = (int)(len);
                    f.samples_output += (uint)(right - left);
                    return (int)(right - left);
                }

                public static int vorbis_pump_first_frame(stb_vorbis f)
                {
                    int len;
                    int right;
                    int left;
                    int res;
                    res = (int)(vorbis_decode_packet(f, &len, &left, &right));
                    if ((res) != 0) vorbis_finish_frame(f, (int)(len), (int)(left), (int)(right));
                    return (int)(res);
                }

                public static int is_whole_packet_present(stb_vorbis f, int end_page)
                {
                    int s = (int)(f.next_seg);
                    int first = (int)(1);
                    byte* p = f.stream;
                    if (s != -1)
                    {
                        for (; (s) < (f.segment_count); ++s)
                        {
                            p += f.segments[s];
                            if ((f.segments[s]) < (255)) break;
                        }
                        if ((end_page) != 0) if ((s) < (f.segment_count - 1)) return (int)(error(f, (int)(VORBIS_invalid_stream)));
                        if ((s) == (f.segment_count)) s = (int)(-1);
                        if ((p) > (f.stream_end)) return (int)(error(f, (int)(VORBIS_need_more_data)));
                        first = (int)(0);
                    }

                    for (; (s) == (-1);)
                    {
                        byte* q;
                        int n;
                        if ((p + 26) >= (f.stream_end)) return (int)(error(f, (int)(VORBIS_need_more_data)));
                        if ((CRuntime.memcmp(p, ogg_page_header, (ulong)(4))) != 0) return (int)(error(f, (int)(VORBIS_invalid_stream)));
                        if (p[4] != 0) return (int)(error(f, (int)(VORBIS_invalid_stream)));
                        if ((first) != 0)
                        {
                            if ((f.previous_length) != 0) if ((p[5] & 1) != 0) return (int)(error(f, (int)(VORBIS_invalid_stream)));
                        }
                        else
                        {
                            if ((p[5] & 1) == 0) return (int)(error(f, (int)(VORBIS_invalid_stream)));
                        }
                        n = (int)(p[26]);
                        q = p + 27;
                        p = q + n;
                        if ((p) > (f.stream_end)) return (int)(error(f, (int)(VORBIS_need_more_data)));
                        for (s = (int)(0); (s) < (n); ++s)
                        {
                            p += q[s];
                            if ((q[s]) < (255)) break;
                        }
                        if ((end_page) != 0) if ((s) < (n - 1)) return (int)(error(f, (int)(VORBIS_invalid_stream)));
                        if ((s) == (n)) s = (int)(-1);
                        if ((p) > (f.stream_end)) return (int)(error(f, (int)(VORBIS_need_more_data)));
                        first = (int)(0);
                    }

                    return (int)(1);
                }

                public static int start_decoder(stb_vorbis f)
                {
                    byte* header = stackalloc byte[6];
                    byte x;
                    byte y;
                    int len;
                    int i;
                    int j;
                    int k;
                    int max_submaps = (int)(0);
                    int longest_floorlist = (int)(0);
                    if (start_page(f) == 0) return (int)(0);
                    if ((f.page_flag & 2) == 0) return (int)(error(f, (int)(VORBIS_invalid_first_page)));
                    if ((f.page_flag & 4) != 0) return (int)(error(f, (int)(VORBIS_invalid_first_page)));
                    if ((f.page_flag & 1) != 0) return (int)(error(f, (int)(VORBIS_invalid_first_page)));
                    if (f.segment_count != 1) return (int)(error(f, (int)(VORBIS_invalid_first_page)));
                    if (f.segments[0] != 30) return (int)(error(f, (int)(VORBIS_invalid_first_page)));
                    if (get8(f) != VORBIS_packet_id) return (int)(error(f, (int)(VORBIS_invalid_first_page)));
                    if (getn(f, header, (int)(6)) == 0) return (int)(error(f, (int)(VORBIS_unexpected_eof)));
                    if (vorbis_validate(header) == 0) return (int)(error(f, (int)(VORBIS_invalid_first_page)));
                    if (get32(f) != 0) return (int)(error(f, (int)(VORBIS_invalid_first_page)));
                    f.channels = (int)(get8(f));
                    if (f.channels == 0) return (int)(error(f, (int)(VORBIS_invalid_first_page)));
                    if ((f.channels) > (16)) return (int)(error(f, (int)(VORBIS_too_many_channels)));
                    f.sample_rate = (uint)(get32(f));
                    if (f.sample_rate == 0) return (int)(error(f, (int)(VORBIS_invalid_first_page)));
                    get32(f);
                    get32(f);
                    get32(f);
                    x = (byte)(get8(f));
                    {
                        int log0;
                        int log1;
                        log0 = (int)(x & 15);
                        log1 = (int)(x >> 4);
                        f.blocksize_0 = (int)(1 << log0);
                        f.blocksize_1 = (int)(1 << log1);
                        if (((log0) < (6)) || ((log0) > (13))) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        if (((log1) < (6)) || ((log1) > (13))) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        if ((log0) > (log1)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                    }

                    x = (byte)(get8(f));
                    if ((x & 1) == 0) return (int)(error(f, (int)(VORBIS_invalid_first_page)));
                    if (start_page(f) == 0) return (int)(0);
                    if (start_packet(f) == 0) return (int)(0);
                    do
                    {
                        len = (int)(next_segment(f));
                        skip(f, (int)(len));
                        f.bytes_in_seg = (byte)(0);
                    } while ((len) != 0);
                    if (start_packet(f) == 0) return (int)(0);
                    if (((f).push_mode) != 0)
                    {
                        if (is_whole_packet_present(f, (int)(1)) == 0)
                        {
                            if ((f.error) == (VORBIS_invalid_stream)) f.error = (int)(VORBIS_invalid_setup);
                            return (int)(0);
                        }
                    }

                    crc32_init();
                    if (get8_packet(f) != VORBIS_packet_setup) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                    for (i = (int)(0); (i) < (6); ++i)
                    {
                        header[i] = (byte)(get8_packet(f));
                    }
                    if (vorbis_validate(header) == 0) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                    f.codebook_count = (int)(get_bits(f, (int)(8)) + 1);
                    f.codebooks = (Codebook*)(setup_malloc(f, (int)(sizeof(Codebook) * f.codebook_count)));
                    if ((f.codebooks) == (null)) return (int)(error(f, (int)(VORBIS_outofmem)));
                    CRuntime.memset(f.codebooks, (int)(0), (ulong)(sizeof(Codebook) * f.codebook_count));
                    for (i = (int)(0); (i) < (f.codebook_count); ++i)
                    {
                        uint* values;
                        int ordered;
                        int sorted_count;
                        int total = (int)(0);
                        byte* lengths;
                        Codebook* c = f.codebooks + i;
                        x = (byte)(get_bits(f, (int)(8)));
                        if (x != 0x42) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        x = (byte)(get_bits(f, (int)(8)));
                        if (x != 0x43) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        x = (byte)(get_bits(f, (int)(8)));
                        if (x != 0x56) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        x = (byte)(get_bits(f, (int)(8)));
                        c->dimensions = (int)((get_bits(f, (int)(8)) << 8) + x);
                        x = (byte)(get_bits(f, (int)(8)));
                        y = (byte)(get_bits(f, (int)(8)));
                        c->entries = (int)((get_bits(f, (int)(8)) << 16) + (y << 8) + x);
                        ordered = (int)(get_bits(f, (int)(1)));
                        c->sparse = (byte)((ordered) != 0 ? 0 : get_bits(f, (int)(1)));
                        if (((c->dimensions) == (0)) && (c->entries != 0)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        if ((c->sparse) != 0) lengths = (byte*)(setup_temp_malloc(f, (int)(c->entries)));
                        else lengths = c->codeword_lengths = (byte*)(setup_malloc(f, (int)(c->entries)));
                        if (lengths == null) return (int)(error(f, (int)(VORBIS_outofmem)));
                        if ((ordered) != 0)
                        {
                            int current_entry = (int)(0);
                            int current_length = (int)(get_bits(f, (int)(5)) + 1);
                            while ((current_entry) < (c->entries))
                            {
                                int limit = (int)(c->entries - current_entry);
                                int n = (int)(get_bits(f, (int)(ilog((int)(limit)))));
                                if ((current_entry + n) > (c->entries))
                                {
                                    return (int)(error(f, (int)(VORBIS_invalid_setup)));
                                }
                                CRuntime.memset(lengths + current_entry, (int)(current_length), (ulong)(n));
                                current_entry += (int)(n);
                                ++current_length;
                            }
                        }
                        else
                        {
                            for (j = (int)(0); (j) < (c->entries); ++j)
                            {
                                int present = (int)((c->sparse) != 0 ? get_bits(f, (int)(1)) : 1);
                                if ((present) != 0)
                                {
                                    lengths[j] = (byte)(get_bits(f, (int)(5)) + 1);
                                    ++total;
                                    if ((lengths[j]) == (32)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                                }
                                else
                                {
                                    lengths[j] = (byte)(255);
                                }
                            }
                        }
                        if (((c->sparse) != 0) && ((total) >= (c->entries >> 2)))
                        {
                            if ((c->entries) > ((int)(f.setup_temp_memory_required))) f.setup_temp_memory_required = (uint)(c->entries);
                            c->codeword_lengths = (byte*)(setup_malloc(f, (int)(c->entries)));
                            if ((c->codeword_lengths) == (null)) return (int)(error(f, (int)(VORBIS_outofmem)));
                            CRuntime.memcpy(c->codeword_lengths, lengths, (ulong)(c->entries));
                            setup_temp_free(f, lengths, (int)(c->entries));
                            lengths = c->codeword_lengths;
                            c->sparse = (byte)(0);
                        }
                        if ((c->sparse) != 0)
                        {
                            sorted_count = (int)(total);
                        }
                        else
                        {
                            sorted_count = (int)(0);
                            for (j = (int)(0); (j) < (c->entries); ++j)
                            {
                                if (((lengths[j]) > (10)) && (lengths[j] != 255)) ++sorted_count;
                            }
                        }
                        c->sorted_entries = (int)(sorted_count);
                        values = (null);
                        if (c->sparse == 0)
                        {
                            c->codewords = (uint*)(setup_malloc(f, (int)(sizeof(uint) * c->entries)));
                            if (c->codewords == null) return (int)(error(f, (int)(VORBIS_outofmem)));
                        }
                        else
                        {
                            uint size;
                            if ((c->sorted_entries) != 0)
                            {
                                c->codeword_lengths = (byte*)(setup_malloc(f, (int)(c->sorted_entries)));
                                if (c->codeword_lengths == null) return (int)(error(f, (int)(VORBIS_outofmem)));
                                c->codewords = (uint*)(setup_temp_malloc(f, (int)(sizeof(uint) * c->sorted_entries)));
                                if (c->codewords == null) return (int)(error(f, (int)(VORBIS_outofmem)));
                                values = (uint*)(setup_temp_malloc(f, (int)(sizeof(uint) * c->sorted_entries)));
                                if (values == null) return (int)(error(f, (int)(VORBIS_outofmem)));
                            }
                            size = (uint)(c->entries + (sizeof(uint) + sizeof(uint)) * c->sorted_entries);
                            if ((size) > (f.setup_temp_memory_required)) f.setup_temp_memory_required = (uint)(size);
                        }
                        if (compute_codewords(c, lengths, (int)(c->entries), values) == 0)
                        {
                            if ((c->sparse) != 0) setup_temp_free(f, values, (int)(0));
                            return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        }
                        if ((c->sorted_entries) != 0)
                        {
                            c->sorted_codewords = (uint*)(setup_malloc(f, (int)(sizeof(uint) * (c->sorted_entries + 1))));
                            if ((c->sorted_codewords) == (null)) return (int)(error(f, (int)(VORBIS_outofmem)));
                            c->sorted_values = (int*)(setup_malloc(f, (int)(sizeof(int) * (c->sorted_entries + 1))));
                            if ((c->sorted_values) == (null)) return (int)(error(f, (int)(VORBIS_outofmem)));
                            ++c->sorted_values;
                            c->sorted_values[-1] = (int)(-1);
                            compute_sorted_huffman(c, lengths, values);
                        }
                        if ((c->sparse) != 0)
                        {
                            setup_temp_free(f, values, (int)(sizeof(uint) * c->sorted_entries));
                            setup_temp_free(f, c->codewords, (int)(sizeof(uint) * c->sorted_entries));
                            setup_temp_free(f, lengths, (int)(c->entries));
                            c->codewords = (null);
                        }
                        compute_accelerated_huffman(c);
                        c->lookup_type = (byte)(get_bits(f, (int)(4)));
                        if ((c->lookup_type) > (2)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        if ((c->lookup_type) > (0))
                        {
                            ushort* mults;
                            c->minimum_value = (float)(float32_unpack((uint)(get_bits(f, (int)(32)))));
                            c->delta_value = (float)(float32_unpack((uint)(get_bits(f, (int)(32)))));
                            c->value_bits = (byte)(get_bits(f, (int)(4)) + 1);
                            c->sequence_p = (byte)(get_bits(f, (int)(1)));
                            if ((c->lookup_type) == (1))
                            {
                                c->lookup_values = (uint)(lookup1_values((int)(c->entries), (int)(c->dimensions)));
                            }
                            else
                            {
                                c->lookup_values = (uint)(c->entries * c->dimensions);
                            }
                            if ((c->lookup_values) == (0)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                            mults = (ushort*)(setup_temp_malloc(f, (int)(sizeof(ushort) * c->lookup_values)));
                            if ((mults) == (null)) return (int)(error(f, (int)(VORBIS_outofmem)));
                            for (j = (int)(0); (j) < ((int)(c->lookup_values)); ++j)
                            {
                                int q = (int)(get_bits(f, (int)(c->value_bits)));
                                if ((q) == (-1))
                                {
                                    setup_temp_free(f, mults, (int)(sizeof(ushort) * c->lookup_values));
                                    return (int)(error(f, (int)(VORBIS_invalid_setup)));
                                }
                                mults[j] = (ushort)(q);
                            }
                            if ((c->lookup_type) == (1))
                            {
                                int len2;
                                int sparse = (int)(c->sparse);
                                float last = (float)(0);
                                if ((sparse) != 0)
                                {
                                    if ((c->sorted_entries) == (0)) goto skip;
                                    c->multiplicands = (float*)(setup_malloc(f, (int)(sizeof(float) * c->sorted_entries * c->dimensions)));
                                }
                                else c->multiplicands = (float*)(setup_malloc(f, (int)(sizeof(float) * c->entries * c->dimensions)));
                                if ((c->multiplicands) == (null))
                                {
                                    setup_temp_free(f, mults, (int)(sizeof(ushort) * c->lookup_values));
                                    return (int)(error(f, (int)(VORBIS_outofmem)));
                                }
                                len2 = (int)((sparse) != 0 ? c->sorted_entries : c->entries);
                                for (j = (int)(0); (j) < (len2); ++j)
                                {
                                    uint z = (uint)((sparse) != 0 ? c->sorted_values[j] : j);
                                    uint div = (uint)(1);
                                    for (k = (int)(0); (k) < (c->dimensions); ++k)
                                    {
                                        int off = (int)((z / div) % c->lookup_values);
                                        float val = (float)(mults[off]);
                                        val = (float)(mults[off] * c->delta_value + c->minimum_value + last);
                                        c->multiplicands[j * c->dimensions + k] = (float)(val);
                                        if ((c->sequence_p) != 0) last = (float)(val);
                                        if ((k + 1) < (c->dimensions))
                                        {
                                            if ((div) > (uint.MaxValue / c->lookup_values))
                                            {
                                                setup_temp_free(f, mults, (int)(sizeof(ushort) * c->lookup_values));
                                                return (int)(error(f, (int)(VORBIS_invalid_setup)));
                                            }
                                            div *= (uint)(c->lookup_values);
                                        }
                                    }
                                }
                                c->lookup_type = (byte)(2);
                            }
                            else
                            {
                                float last = (float)(0);
                                c->multiplicands = (float*)(setup_malloc(f, (int)(sizeof(float) * c->lookup_values)));
                                if ((c->multiplicands) == (null))
                                {
                                    setup_temp_free(f, mults, (int)(sizeof(ushort) * c->lookup_values));
                                    return (int)(error(f, (int)(VORBIS_outofmem)));
                                }
                                for (j = (int)(0); (j) < ((int)(c->lookup_values)); ++j)
                                {
                                    float val = (float)(mults[j] * c->delta_value + c->minimum_value + last);
                                    c->multiplicands[j] = (float)(val);
                                    if ((c->sequence_p) != 0) last = (float)(val);
                                }
                            }
                            skip:
                            ;
                            setup_temp_free(f, mults, (int)(sizeof(ushort) * c->lookup_values));
                        }
                    }
                    x = (byte)(get_bits(f, (int)(6)) + 1);
                    for (i = (int)(0); (i) < (x); ++i)
                    {
                        uint z = (uint)(get_bits(f, (int)(16)));
                        if (z != 0) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                    }
                    f.floor_count = (int)(get_bits(f, (int)(6)) + 1);
                    f.floor_config = (Floor*)(setup_malloc(f, (int)(f.floor_count * sizeof(Floor))));
                    if ((f.floor_config) == (null)) return (int)(error(f, (int)(VORBIS_outofmem)));
                    for (i = (int)(0); (i) < (f.floor_count); ++i)
                    {
                        f.floor_types[i] = (ushort)(get_bits(f, (int)(16)));
                        if ((f.floor_types[i]) > (1)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        if ((f.floor_types[i]) == (0))
                        {
                            Floor0* g = &f.floor_config[i].floor0;
                            g->order = (byte)(get_bits(f, (int)(8)));
                            g->rate = (ushort)(get_bits(f, (int)(16)));
                            g->bark_map_size = (ushort)(get_bits(f, (int)(16)));
                            g->amplitude_bits = (byte)(get_bits(f, (int)(6)));
                            g->amplitude_offset = (byte)(get_bits(f, (int)(8)));
                            g->number_of_books = (byte)(get_bits(f, (int)(4)) + 1);
                            for (j = (int)(0); (j) < (g->number_of_books); ++j)
                            {
                                g->book_list[j] = (byte)(get_bits(f, (int)(8)));
                            }
                            return (int)(error(f, (int)(VORBIS_feature_not_supported)));
                        }
                        else
                        {
                            stbv__floor_ordering* p = stackalloc stbv__floor_ordering[31 * 8 + 2];
                            Floor1* g = &f.floor_config[i].floor1;
                            int max_class = (int)(-1);
                            g->partitions = (byte)(get_bits(f, (int)(5)));
                            for (j = (int)(0); (j) < (g->partitions); ++j)
                            {
                                g->partition_class_list[j] = (byte)(get_bits(f, (int)(4)));
                                if ((g->partition_class_list[j]) > (max_class)) max_class = (int)(g->partition_class_list[j]);
                            }
                            for (j = (int)(0); j <= max_class; ++j)
                            {
                                g->class_dimensions[j] = (byte)(get_bits(f, (int)(3)) + 1);
                                g->class_subclasses[j] = (byte)(get_bits(f, (int)(2)));
                                if ((g->class_subclasses[j]) != 0)
                                {
                                    g->class_masterbooks[j] = (byte)(get_bits(f, (int)(8)));
                                    if ((g->class_masterbooks[j]) >= (f.codebook_count)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                                }
                                for (k = (int)(0); (k) < (1 << g->class_subclasses[j]); ++k)
                                {
                                    g->subclass_books[j * 8 + k] = (short)(get_bits(f, (int)(8)) - 1);
                                    if ((g->subclass_books[j * 8 + k]) >= (f.codebook_count)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                                }
                            }
                            g->floor1_multiplier = (byte)(get_bits(f, (int)(2)) + 1);
                            g->rangebits = (byte)(get_bits(f, (int)(4)));
                            g->Xlist[0] = (ushort)(0);
                            g->Xlist[1] = (ushort)(1 << g->rangebits);
                            g->values = (int)(2);
                            for (j = (int)(0); (j) < (g->partitions); ++j)
                            {
                                int c = (int)(g->partition_class_list[j]);
                                for (k = (int)(0); (k) < (g->class_dimensions[c]); ++k)
                                {
                                    g->Xlist[g->values] = (ushort)(get_bits(f, (int)(g->rangebits)));
                                    ++g->values;
                                }
                            }
                            for (j = (int)(0); (j) < (g->values); ++j)
                            {
                                p[j].x = (ushort)(g->Xlist[j]);
                                p[j].id = (ushort)(j);
                            }
                            CRuntime.qsort(p, (ulong)(g->values), (ulong)(sizeof(stbv__floor_ordering)), point_compare);
                            for (j = (int)(0); (j) < (g->values); ++j)
                            {
                                g->sorted_order[j] = ((byte)(p[j].id));
                            }
                            for (j = (int)(2); (j) < (g->values); ++j)
                            {
                                int low;
                                int hi;
                                neighbors(g->Xlist, (int)(j), &low, &hi);
                                g->neighbors[j * 2 + 0] = (byte)(low);
                                g->neighbors[j * 2 + 1] = (byte)(hi);
                            }
                            if ((g->values) > (longest_floorlist)) longest_floorlist = (int)(g->values);
                        }
                    }
                    f.residue_count = (int)(get_bits(f, (int)(6)) + 1);
                    f.residue_config = new Residue[f.residue_count];
                    for (i = 0; i < f.residue_config.Length; ++i)
                    {
                        f.residue_config[i] = new Residue();
                    }
                    if ((f.residue_config) == (null)) return (int)(error(f, (int)(VORBIS_outofmem)));
                    for (i = (int)(0); (i) < (f.residue_count); ++i)
                    {
                        byte* residue_cascade = stackalloc byte[64];
                        Residue r = f.residue_config[i];
                        f.residue_types[i] = (ushort)(get_bits(f, (int)(16)));
                        if ((f.residue_types[i]) > (2)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        r.begin = (uint)(get_bits(f, (int)(24)));
                        r.end = (uint)(get_bits(f, (int)(24)));
                        if ((r.end) < (r.begin)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        r.part_size = (uint)(get_bits(f, (int)(24)) + 1);
                        r.classifications = (byte)(get_bits(f, (int)(6)) + 1);
                        r.classbook = (byte)(get_bits(f, (int)(8)));
                        if ((r.classbook) >= (f.codebook_count)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        for (j = (int)(0); (j) < (r.classifications); ++j)
                        {
                            byte high_bits = (byte)(0);
                            byte low_bits = (byte)(get_bits(f, (int)(3)));
                            if ((get_bits(f, (int)(1))) != 0) high_bits = (byte)(get_bits(f, (int)(5)));
                            residue_cascade[j] = (byte)(high_bits * 8 + low_bits);
                        }
                        r.residue_books = new short[r.classifications, 8];
                        if ((r.residue_books) == (null)) return (int)(error(f, (int)(VORBIS_outofmem)));
                        for (j = (int)(0); (j) < (r.classifications); ++j)
                        {
                            for (k = (int)(0); (k) < (8); ++k)
                            {
                                if ((residue_cascade[j] & (1 << k)) != 0)
                                {
                                    r.residue_books[j, k] = (short)(get_bits(f, (int)(8)));
                                    if ((r.residue_books[j, k]) >= (f.codebook_count)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                                }
                                else
                                {
                                    r.residue_books[j, k] = (short)(-1);
                                }
                            }
                        }
                        r.classdata = (byte**)(setup_malloc(f, (int)(sizeof(byte*) * f.codebooks[r.classbook].entries)));
                        if (r.classdata == null) return (int)(error(f, (int)(VORBIS_outofmem)));
                        CRuntime.memset(r.classdata, (int)(0), (ulong)(sizeof(byte*) * f.codebooks[r.classbook].entries));
                        for (j = (int)(0); (j) < (f.codebooks[r.classbook].entries); ++j)
                        {
                            int classwords = (int)(f.codebooks[r.classbook].dimensions);
                            int temp = (int)(j);
                            r.classdata[j] = (byte*)(setup_malloc(f, (int)(sizeof(byte) * classwords)));
                            if ((r.classdata[j]) == (null)) return (int)(error(f, (int)(VORBIS_outofmem)));
                            for (k = (int)(classwords - 1); (k) >= (0); --k)
                            {
                                r.classdata[j][k] = (byte)(temp % r.classifications);
                                temp /= (int)(r.classifications);
                            }
                        }
                    }
                    f.mapping_count = (int)(get_bits(f, (int)(6)) + 1);
                    f.mapping = (Mapping*)(setup_malloc(f, (int)(f.mapping_count * sizeof(Mapping))));
                    if ((f.mapping) == (null)) return (int)(error(f, (int)(VORBIS_outofmem)));
                    CRuntime.memset(f.mapping, (int)(0), (ulong)(f.mapping_count * sizeof(Mapping)));
                    for (i = (int)(0); (i) < (f.mapping_count); ++i)
                    {
                        Mapping* m = f.mapping + i;
                        int mapping_type = (int)(get_bits(f, (int)(16)));
                        if (mapping_type != 0) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        m->chan = (MappingChannel*)(setup_malloc(f, (int)(f.channels * sizeof(MappingChannel))));
                        if ((m->chan) == (null)) return (int)(error(f, (int)(VORBIS_outofmem)));
                        if ((get_bits(f, (int)(1))) != 0) m->submaps = (byte)(get_bits(f, (int)(4)) + 1);
                        else m->submaps = (byte)(1);
                        if ((m->submaps) > (max_submaps)) max_submaps = (int)(m->submaps);
                        if ((get_bits(f, (int)(1))) != 0)
                        {
                            m->coupling_steps = (ushort)(get_bits(f, (int)(8)) + 1);
                            for (k = (int)(0); (k) < (m->coupling_steps); ++k)
                            {
                                m->chan[k].magnitude = (byte)(get_bits(f, (int)(ilog((int)(f.channels - 1)))));
                                m->chan[k].angle = (byte)(get_bits(f, (int)(ilog((int)(f.channels - 1)))));
                                if ((m->chan[k].magnitude) >= (f.channels)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                                if ((m->chan[k].angle) >= (f.channels)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                                if ((m->chan[k].magnitude) == (m->chan[k].angle)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                            }
                        }
                        else m->coupling_steps = (ushort)(0);
                        if ((get_bits(f, (int)(2))) != 0) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        if ((m->submaps) > (1))
                        {
                            for (j = (int)(0); (j) < (f.channels); ++j)
                            {
                                m->chan[j].mux = (byte)(get_bits(f, (int)(4)));
                                if ((m->chan[j].mux) >= (m->submaps)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                            }
                        }
                        else
                            for (j = (int)(0); (j) < (f.channels); ++j)
                            {
                                m->chan[j].mux = (byte)(0);
                            }
                        for (j = (int)(0); (j) < (m->submaps); ++j)
                        {
                            get_bits(f, (int)(8));
                            m->submap_floor[j] = (byte)(get_bits(f, (int)(8)));
                            m->submap_residue[j] = (byte)(get_bits(f, (int)(8)));
                            if ((m->submap_floor[j]) >= (f.floor_count)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                            if ((m->submap_residue[j]) >= (f.residue_count)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        }
                    }
                    f.mode_count = (int)(get_bits(f, (int)(6)) + 1);
                    for (i = (int)(0); (i) < (f.mode_count); ++i)
                    {
                        Mode* m = (Mode*)f.mode_config + i;
                        m->blockflag = (byte)(get_bits(f, (int)(1)));
                        m->windowtype = (ushort)(get_bits(f, (int)(16)));
                        m->transformtype = (ushort)(get_bits(f, (int)(16)));
                        m->mapping = (byte)(get_bits(f, (int)(8)));
                        if (m->windowtype != 0) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        if (m->transformtype != 0) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                        if ((m->mapping) >= (f.mapping_count)) return (int)(error(f, (int)(VORBIS_invalid_setup)));
                    }
                    flush_packet(f);
                    f.previous_length = (int)(0);
                    for (i = (int)(0); (i) < (f.channels); ++i)
                    {
                        f.channel_buffers[i] = (float*)(setup_malloc(f, (int)(sizeof(float) * f.blocksize_1)));
                        f.previous_window[i] = (float*)(setup_malloc(f, (int)(sizeof(float) * f.blocksize_1 / 2)));
                        f.finalY[i] = (short*)(setup_malloc(f, (int)(sizeof(short) * longest_floorlist)));
                        if ((((f.channel_buffers[i]) == (null)) || ((f.previous_window[i]) == (null))) || ((f.finalY[i]) == (null)))
                            return (int)(error(f, (int)(VORBIS_outofmem)));
                    }
                    if (init_blocksize(f, (int)(0), (int)(f.blocksize_0)) == 0) return (int)(0);
                    if (init_blocksize(f, (int)(1), (int)(f.blocksize_1)) == 0) return (int)(0);
                    f.blocksize[0] = (int)(f.blocksize_0);
                    f.blocksize[1] = (int)(f.blocksize_1);
                    {
                        uint imdct_mem = (uint)(f.blocksize_1 * sizeof(float) >> 1);
                        uint classify_mem;
                        int i2;
                        int max_part_read = (int)(0);
                        for (i2 = (int)(0); (i2) < (f.residue_count); ++i2)
                        {
                            Residue r = f.residue_config[i2];
                            int n_read = (int)(r.end - r.begin);
                            int part_read = (int)(n_read / r.part_size);
                            if ((part_read) > (max_part_read)) max_part_read = (int)(part_read);
                        }
                        classify_mem = (uint)((f.channels * (sizeof(void*) + max_part_read * sizeof(char*))));
                        f.temp_memory_required = (uint)(classify_mem);
                        if ((imdct_mem) > (f.temp_memory_required)) f.temp_memory_required = (uint)(imdct_mem);
                    }

                    f.first_decode = (byte)(1);
                    if ((f.alloc.alloc_buffer) != null)
                    {
                    }

                    f.first_audio_page_offset = (uint)(stb_vorbis_get_file_offset(f));
                    return (int)(1);
                }

                public static void vorbis_deinit(stb_vorbis p)
                {
                    int i;
                    int j;
                    if ((p.residue_config) != null)
                    {
                        for (i = (int)(0); (i) < (p.residue_count); ++i)
                        {
                            Residue r = p.residue_config[i];
                            if ((r.classdata) != null)
                            {
                                for (j = (int)(0); (j) < (p.codebooks[r.classbook].entries); ++j)
                                {
                                    setup_free(p, r.classdata[j]);
                                }
                                setup_free(p, r.classdata);
                            }
                        }
                    }

                    if ((p.codebooks) != null)
                    {
                        for (i = (int)(0); (i) < (p.codebook_count); ++i)
                        {
                            Codebook* c = p.codebooks + i;
                            setup_free(p, c->codeword_lengths);
                            setup_free(p, c->multiplicands);
                            setup_free(p, c->codewords);
                            setup_free(p, c->sorted_codewords);
                            setup_free(p, c->sorted_values != null ? c->sorted_values - 1 : (null));
                        }
                        setup_free(p, p.codebooks);
                    }

                    setup_free(p, p.floor_config);
                    if ((p.mapping) != null)
                    {
                        for (i = (int)(0); (i) < (p.mapping_count); ++i)
                        {
                            setup_free(p, p.mapping[i].chan);
                        }
                        setup_free(p, p.mapping);
                    }

                    for (i = (int)(0); ((i) < (p.channels)) && ((i) < (16)); ++i)
                    {
                        setup_free(p, p.channel_buffers[i]);
                        setup_free(p, p.previous_window[i]);
                        setup_free(p, p.finalY[i]);
                    }
                    for (i = (int)(0); (i) < (2); ++i)
                    {
                        setup_free(p, p.A[i]);
                        setup_free(p, p.B[i]);
                        setup_free(p, p.C[i]);
                        setup_free(p, p.window[i]);
                        setup_free(p, p.bit_reverse[i]);
                    }
                }

                public static void stb_vorbis_close(stb_vorbis p)
                {
                    if ((p) == (null)) return;
                    vorbis_deinit(p);
                }

                public static void vorbis_init(stb_vorbis p, stb_vorbis_alloc* z)
                {
                    if ((z) != null)
                    {
                        p.alloc = (stb_vorbis_alloc)(*z);
                        p.alloc.alloc_buffer_length_in_bytes = (int)((p.alloc.alloc_buffer_length_in_bytes + 3) & ~3);
                        p.temp_offset = (int)(p.alloc.alloc_buffer_length_in_bytes);
                    }

                    p.eof = (int)(0);
                    p.error = (int)(VORBIS__no_error);
                    p.stream = (null);
                    p.codebooks = (null);
                    p.page_crc_tests = (int)(-1);
                }

                public static int stb_vorbis_get_sample_offset(stb_vorbis f)
                {
                    if ((f.current_loc_valid) != 0) return (int)(f.current_loc);
                    else return (int)(-1);
                }

                public static stb_vorbis_info stb_vorbis_get_info(stb_vorbis f)
                {
                    stb_vorbis_info d = new stb_vorbis_info();
                    d.channels = (int)(f.channels);
                    d.sample_rate = (uint)(f.sample_rate);
                    d.setup_memory_required = (uint)(f.setup_memory_required);
                    d.setup_temp_memory_required = (uint)(f.setup_temp_memory_required);
                    d.temp_memory_required = (uint)(f.temp_memory_required);
                    d.max_frame_size = (int)(f.blocksize_1 >> 1);
                    return (stb_vorbis_info)(d);
                }

                public static int stb_vorbis_get_error(stb_vorbis f)
                {
                    int e = (int)(f.error);
                    f.error = (int)(VORBIS__no_error);
                    return (int)(e);
                }

                public static stb_vorbis vorbis_alloc(stb_vorbis f)
                {
                    stb_vorbis p = new stb_vorbis();
                    return p;
                }

                public static void stb_vorbis_flush_pushdata(stb_vorbis f)
                {
                    f.previous_length = (int)(0);
                    f.page_crc_tests = (int)(0);
                    f.discard_samples_deferred = (int)(0);
                    f.current_loc_valid = (int)(0);
                    f.first_decode = (byte)(0);
                    f.samples_output = (uint)(0);
                    f.channel_buffer_start = (int)(0);
                    f.channel_buffer_end = (int)(0);
                }

                public static int vorbis_search_for_page_pushdata(stb_vorbis f, byte* data, int data_len)
                {
                    int i;
                    int n;
                    for (i = (int)(0); (i) < (f.page_crc_tests); ++i)
                    {
                        f.scan[i].bytes_done = (int)(0);
                    }
                    if ((f.page_crc_tests) < (4))
                    {
                        if ((data_len) < (4)) return (int)(0);
                        data_len -= (int)(3);
                        for (i = (int)(0); (i) < (data_len); ++i)
                        {
                            if ((data[i]) == (0x4f))
                            {
                                if ((0) == (CRuntime.memcmp(data + i, ogg_page_header, (ulong)(4))))
                                {
                                    int j;
                                    int len;
                                    uint crc;
                                    if (((i + 26) >= (data_len)) || ((i + 27 + data[i + 26]) >= (data_len)))
                                    {
                                        data_len = (int)(i);
                                        break;
                                    }
                                    len = (int)(27 + data[i + 26]);
                                    for (j = (int)(0); (j) < (data[i + 26]); ++j)
                                    {
                                        len += (int)(data[i + 27 + j]);
                                    }
                                    crc = (uint)(0);
                                    for (j = (int)(0); (j) < (22); ++j)
                                    {
                                        crc = (uint)(crc32_update((uint)(crc), (byte)(data[i + j])));
                                    }
                                    for (; (j) < (26); ++j)
                                    {
                                        crc = (uint)(crc32_update((uint)(crc), (byte)(0)));
                                    }
                                    n = (int)(f.page_crc_tests++);
                                    f.scan[n].bytes_left = (int)(len - j);
                                    f.scan[n].crc_so_far = (uint)(crc);
                                    f.scan[n].goal_crc = (uint)(data[i + 22] + (data[i + 23] << 8) + (data[i + 24] << 16) + (data[i + 25] << 24));
                                    if ((data[i + 27 + data[i + 26] - 1]) == (255)) f.scan[n].sample_loc = uint.MaxValue;
                                    else
                                        f.scan[n].sample_loc = (uint)(data[i + 6] + (data[i + 7] << 8) + (data[i + 8] << 16) + (data[i + 9] << 24));
                                    f.scan[n].bytes_done = (int)(i + j);
                                    if ((f.page_crc_tests) == (4)) break;
                                }
                            }
                        }
                    }

                    for (i = (int)(0); (i) < (f.page_crc_tests);)
                    {
                        uint crc;
                        int j;
                        int n2 = (int)(f.scan[i].bytes_done);
                        int m = (int)(f.scan[i].bytes_left);
                        if ((m) > (data_len - n2)) m = (int)(data_len - n2);
                        crc = (uint)(f.scan[i].crc_so_far);
                        for (j = (int)(0); (j) < (m); ++j)
                        {
                            crc = (uint)(crc32_update((uint)(crc), (byte)(data[n2 + j])));
                        }
                        f.scan[i].bytes_left -= (int)(m);
                        f.scan[i].crc_so_far = (uint)(crc);
                        if ((f.scan[i].bytes_left) == (0))
                        {
                            if ((f.scan[i].crc_so_far) == (f.scan[i].goal_crc))
                            {
                                data_len = (int)(n2 + m);
                                f.page_crc_tests = (int)(-1);
                                f.previous_length = (int)(0);
                                f.next_seg = (int)(-1);
                                f.current_loc = (uint)(f.scan[i].sample_loc);
                                f.current_loc_valid = f.current_loc != ~0U ? 1 : 0;
                                return (int)(data_len);
                            }
                            f.scan[i] = (CRCscan)(f.scan[--f.page_crc_tests]);
                        }
                        else
                        {
                            ++i;
                        }
                    }
                    return (int)(data_len);
                }

                public static int stb_vorbis_decode_frame_pushdata(stb_vorbis f, byte* data, int data_len, int* channels,
                    ref float*[] output, int* samples)
                {
                    int i;
                    int len;
                    int right;
                    int left;
                    if (((f).push_mode) == 0) return (int)(error(f, (int)(VORBIS_invalid_api_mixing)));
                    if ((f.page_crc_tests) >= (0))
                    {
                        *samples = (int)(0);
                        return (int)(vorbis_search_for_page_pushdata(f, data, (int)(data_len)));
                    }

                    f.stream = data;
                    f.stream_end = data + data_len;
                    f.error = (int)(VORBIS__no_error);
                    if (is_whole_packet_present(f, (int)(0)) == 0)
                    {
                        *samples = (int)(0);
                        return (int)(0);
                    }

                    if (vorbis_decode_packet(f, &len, &left, &right) == 0)
                    {
                        int error2 = (int)(f.error);
                        if ((error2) == (VORBIS_bad_packet_type))
                        {
                            f.error = (int)(VORBIS__no_error);
                            while (get8_packet(f) != (-1))
                            {
                                if ((f.eof) != 0) break;
                            }
                            *samples = (int)(0);
                            return (int)(f.stream - data);
                        }
                        if ((error2) == (VORBIS_continued_packet_flag_invalid))
                        {
                            if ((f.previous_length) == (0))
                            {
                                f.error = (int)(VORBIS__no_error);
                                while (get8_packet(f) != (-1))
                                {
                                    if ((f.eof) != 0) break;
                                }
                                *samples = (int)(0);
                                return (int)(f.stream - data);
                            }
                        }
                        stb_vorbis_flush_pushdata(f);
                        f.error = (int)(error2);
                        *samples = (int)(0);
                        return (int)(1);
                    }

                    len = (int)(vorbis_finish_frame(f, (int)(len), (int)(left), (int)(right)));
                    for (i = (int)(0); (i) < (f.channels); ++i)
                    {
                        f.outputs[i] = f.channel_buffers[i] + left;
                    }
                    if ((channels) != null) *channels = (int)(f.channels);
                    *samples = (int)(len);
                    output = f.outputs;
                    return (int)(f.stream - data);
                }

                public static stb_vorbis stb_vorbis_open_pushdata(byte* data, int data_len, int* data_used, int* error,
                    stb_vorbis_alloc* alloc)
                {
                    stb_vorbis f;
                    stb_vorbis p = new stb_vorbis();
                    vorbis_init(p, alloc);
                    p.stream = data;
                    p.stream_end = data + data_len;
                    p.push_mode = (byte)(1);
                    if (start_decoder(p) == 0)
                    {
                        if ((p.eof) != 0) *error = (int)(VORBIS_need_more_data);
                        else *error = (int)(p.error);
                        return (null);
                    }

                    f = vorbis_alloc(p);
                    if ((f) != null)
                    {
                        f = (stb_vorbis)(p);
                        *data_used = ((int)(f.stream - data));
                        *error = (int)(0);
                        return f;
                    }
                    else
                    {
                        vorbis_deinit(p);
                        return (null);
                    }

                }

                public static uint stb_vorbis_get_file_offset(stb_vorbis f)
                {
                    if ((f.push_mode) != 0) return (uint)(0);
                    if ((1) != 0) return (uint)(f.stream - f.stream_start);
                }

                public static uint vorbis_find_page(stb_vorbis f, uint* end, uint* last)
                {
                    for (; ; )
                    {
                        int n;
                        if ((f.eof) != 0) return (uint)(0);
                        n = (int)(get8(f));
                        if ((n) == (0x4f))
                        {
                            uint retry_loc = (uint)(stb_vorbis_get_file_offset(f));
                            int i2;
                            if ((retry_loc - 25) > (f.stream_len)) return (uint)(0);
                            for (i2 = (int)(1); (i2) < (4); ++i2)
                            {
                                if (get8(f) != ogg_page_header[i2]) break;
                            }
                            if ((f.eof) != 0) return (uint)(0);
                            if ((i2) == (4))
                            {
                                byte* header = stackalloc byte[27];
                                uint i;
                                uint crc;
                                uint goal;
                                uint len;
                                for (i = (uint)(0); (i) < (4); ++i)
                                {
                                    header[i] = (byte)(ogg_page_header[i]);
                                }
                                for (; (i) < (27); ++i)
                                {
                                    header[i] = (byte)(get8(f));
                                }
                                if ((f.eof) != 0) return (uint)(0);
                                if (header[4] != 0) goto invalid;
                                goal = (uint)(header[22] + (header[23] << 8) + (header[24] << 16) + (header[25] << 24));
                                for (i = (uint)(22); (i) < (26); ++i)
                                {
                                    header[i] = (byte)(0);
                                }
                                crc = (uint)(0);
                                for (i = (uint)(0); (i) < (27); ++i)
                                {
                                    crc = (uint)(crc32_update((uint)(crc), (byte)(header[i])));
                                }
                                len = (uint)(0);
                                for (i = (uint)(0); (i) < (header[26]); ++i)
                                {
                                    int s = (int)(get8(f));
                                    crc = (uint)(crc32_update((uint)(crc), (byte)(s)));
                                    len += (uint)(s);
                                }
                                if (((len) != 0) && ((f.eof) != 0)) return (uint)(0);
                                for (i = (uint)(0); (i) < (len); ++i)
                                {
                                    crc = (uint)(crc32_update((uint)(crc), (byte)(get8(f))));
                                }
                                if ((crc) == (goal))
                                {
                                    if ((end) != null) *end = (uint)(stb_vorbis_get_file_offset(f));
                                    if ((last) != null)
                                    {
                                        if ((header[5] & 0x04) != 0) *last = (uint)(1);
                                        else *last = (uint)(0);
                                    }
                                    set_file_offset(f, (uint)(retry_loc - 1));
                                    return (uint)(1);
                                }
                            }
                            invalid:
                            ;
                            set_file_offset(f, (uint)(retry_loc));
                        }
                    }
                }

                public static int get_seek_page_info(stb_vorbis f, ProbedPage* z)
                {
                    byte* header = stackalloc byte[27];
                    byte* lacing = stackalloc byte[255];
                    int i;
                    int len;
                    z->page_start = (uint)(stb_vorbis_get_file_offset(f));
                    getn(f, header, (int)(27));
                    if ((((header[0] != 'O') || (header[1] != 'g')) || (header[2] != 'g')) || (header[3] != 'S')) return (int)(0);
                    getn(f, lacing, (int)(header[26]));
                    len = (int)(0);
                    for (i = (int)(0); (i) < (header[26]); ++i)
                    {
                        len += (int)(lacing[i]);
                    }
                    z->page_end = (uint)(z->page_start + 27 + header[26] + len);
                    z->last_decoded_sample = (uint)(header[6] + (header[7] << 8) + (header[8] << 16) + (header[9] << 24));
                    set_file_offset(f, (uint)(z->page_start));
                    return (int)(1);
                }

                public static int go_to_page_before(stb_vorbis f, uint limit_offset)
                {
                    uint previous_safe;
                    uint end;
                    if (((limit_offset) >= (65536)) && ((limit_offset - 65536) >= (f.first_audio_page_offset)))
                        previous_safe = (uint)(limit_offset - 65536);
                    else previous_safe = (uint)(f.first_audio_page_offset);
                    set_file_offset(f, (uint)(previous_safe));
                    while ((vorbis_find_page(f, &end, (null))) != 0)
                    {
                        if (((end) >= (limit_offset)) && ((stb_vorbis_get_file_offset(f)) < (limit_offset))) return (int)(1);
                        set_file_offset(f, (uint)(end));
                    }
                    return (int)(0);
                }

                public static int seek_to_sample_coarse(stb_vorbis f, uint sample_number)
                {
                    ProbedPage left = new ProbedPage();
                    ProbedPage right = new ProbedPage();
                    ProbedPage mid = new ProbedPage();
                    int i;
                    int start_seg_with_known_loc;
                    int end_pos;
                    int page_start;
                    uint delta;
                    uint stream_length;
                    uint padding;
                    double offset = 0;
                    double bytes_per_sample = 0;
                    int probe = (int)(0);
                    stream_length = (uint)(stb_vorbis_stream_length_in_samples(f));
                    if ((stream_length) == (0)) return (int)(error(f, (int)(VORBIS_seek_without_length)));
                    if ((sample_number) > (stream_length)) return (int)(error(f, (int)(VORBIS_seek_invalid)));
                    padding = (uint)((f.blocksize_1 - f.blocksize_0) >> 2);
                    if ((sample_number) < (padding)) sample_number = (uint)(0);
                    else sample_number -= (uint)(padding);
                    left = (ProbedPage)(f.p_first);
                    while ((left.last_decoded_sample) == (~0U))
                    {
                        set_file_offset(f, (uint)(left.page_end));
                        if (get_seek_page_info(f, &left) == 0) goto error;
                    }
                    right = (ProbedPage)(f.p_last);
                    if (sample_number <= left.last_decoded_sample)
                    {
                        if ((stb_vorbis_seek_start(f)) != 0) return (int)(1);
                        return (int)(0);
                    }

                    while (left.page_end != right.page_start)
                    {
                        delta = (uint)(right.page_start - left.page_end);
                        if (delta <= 65536)
                        {
                            set_file_offset(f, (uint)(left.page_end));
                        }
                        else
                        {
                            if ((probe) < (2))
                            {
                                if ((probe) == (0))
                                {
                                    double data_bytes = (double)(right.page_end - left.page_start);
                                    bytes_per_sample = (double)(data_bytes / right.last_decoded_sample);
                                    offset = (double)(left.page_start + bytes_per_sample * (sample_number - left.last_decoded_sample));
                                }
                                else
                                {
                                    double error2 = (double)(((double)(sample_number) - mid.last_decoded_sample) * bytes_per_sample);
                                    if (((error2) >= (0)) && ((error2) < (8000))) error2 = (double)(8000);
                                    if (((error2) < (0)) && ((error2) > (-8000))) error2 = (double)(-8000);
                                    offset += (double)(error2 * 2);
                                }
                                if ((offset) < (left.page_end)) offset = (double)(left.page_end);
                                if ((offset) > (right.page_start - 65536)) offset = (double)(right.page_start - 65536);
                                set_file_offset(f, (uint)(offset));
                            }
                            else
                            {
                                set_file_offset(f, (uint)(left.page_end + (delta / 2) - 32768));
                            }
                            if (vorbis_find_page(f, (null), (null)) == 0) goto error;
                        }
                        for (; ; )
                        {
                            if (get_seek_page_info(f, &mid) == 0) goto error;
                            if (mid.last_decoded_sample != ~0U) break;
                            set_file_offset(f, (uint)(mid.page_end));
                        }
                        if ((mid.page_start) == (right.page_start)) break;
                        if ((sample_number) < (mid.last_decoded_sample)) right = (ProbedPage)(mid);
                        else left = (ProbedPage)(mid);
                        ++probe;
                    }
                    page_start = (int)(left.page_start);
                    set_file_offset(f, (uint)(page_start));
                    if (start_page(f) == 0) return (int)(error(f, (int)(VORBIS_seek_failed)));
                    end_pos = (int)(f.end_seg_with_known_loc);
                    for (; ; )
                    {
                        for (i = (int)(end_pos); (i) > (0); --i)
                        {
                            if (f.segments[i - 1] != 255) break;
                        }
                        start_seg_with_known_loc = (int)(i);
                        if (((start_seg_with_known_loc) > (0)) || ((f.page_flag & 1) == 0)) break;
                        if (go_to_page_before(f, (uint)(page_start)) == 0) goto error;
                        page_start = (int)(stb_vorbis_get_file_offset(f));
                        if (start_page(f) == 0) goto error;
                        end_pos = (int)(f.segment_count - 1);
                    }
                    f.current_loc_valid = (int)(0);
                    f.last_seg = (int)(0);
                    f.valid_bits = (int)(0);
                    f.packet_bytes = (int)(0);
                    f.bytes_in_seg = (byte)(0);
                    f.previous_length = (int)(0);
                    f.next_seg = (int)(start_seg_with_known_loc);
                    for (i = (int)(0); (i) < (start_seg_with_known_loc); i++)
                    {
                        skip(f, (int)(f.segments[i]));
                    }
                    if (vorbis_pump_first_frame(f) == 0) return (int)(0);
                    if ((f.current_loc) > (sample_number)) return (int)(error(f, (int)(VORBIS_seek_failed)));
                    return (int)(1);
                    error:
                    ;
                    stb_vorbis_seek_start(f);
                    return (int)(error(f, (int)(VORBIS_seek_failed)));
                }

                public static int peek_decode_initial(stb_vorbis f, int* p_left_start, int* p_left_end, int* p_right_start,
                    int* p_right_end, int* mode)
                {
                    int bits_read;
                    int bytes_read;
                    if (vorbis_decode_initial(f, p_left_start, p_left_end, p_right_start, p_right_end, mode) == 0) return (int)(0);
                    bits_read = (int)(1 + ilog((int)(f.mode_count - 1)));
                    if ((f.mode_config[*mode].blockflag) != 0) bits_read += (int)(2);
                    bytes_read = (int)((bits_read + 7) / 8);
                    f.bytes_in_seg += (byte)(bytes_read);
                    f.packet_bytes -= (int)(bytes_read);
                    skip(f, (int)(-bytes_read));
                    if ((f.next_seg) == (-1)) f.next_seg = (int)(f.segment_count - 1);
                    else f.next_seg--;
                    f.valid_bits = (int)(0);
                    return (int)(1);
                }

                public static int stb_vorbis_seek_frame(stb_vorbis f, uint sample_number)
                {
                    uint max_frame_samples;
                    if (((f).push_mode) != 0) return (int)(error(f, (int)(VORBIS_invalid_api_mixing)));
                    if (seek_to_sample_coarse(f, (uint)(sample_number)) == 0) return (int)(0);
                    max_frame_samples = (uint)((f.blocksize_1 * 3 - f.blocksize_0) >> 2);
                    while ((f.current_loc) < (sample_number))
                    {
                        int left_start;
                        int left_end;
                        int right_start;
                        int right_end;
                        int mode;
                        int frame_samples;
                        if (peek_decode_initial(f, &left_start, &left_end, &right_start, &right_end, &mode) == 0)
                            return (int)(error(f, (int)(VORBIS_seek_failed)));
                        frame_samples = (int)(right_start - left_start);
                        if ((f.current_loc + frame_samples) > (sample_number))
                        {
                            return (int)(1);
                        }
                        else if ((f.current_loc + frame_samples + max_frame_samples) > (sample_number))
                        {
                            vorbis_pump_first_frame(f);
                        }
                        else
                        {
                            f.current_loc += (uint)(frame_samples);
                            f.previous_length = (int)(0);
                            maybe_start_packet(f);
                            flush_packet(f);
                        }
                    }
                    return (int)(1);
                }

                public static int stb_vorbis_seek(stb_vorbis f, uint sample_number)
                {
                    if (stb_vorbis_seek_frame(f, (uint)(sample_number)) == 0) return (int)(0);
                    if (sample_number != f.current_loc)
                    {
                        int n;
                        uint frame_start = (uint)(f.current_loc);
                        float*[] output = null;
                        stb_vorbis_get_frame_float(f, &n, ref output);
                        f.channel_buffer_start += (int)(sample_number - frame_start);
                    }

                    return (int)(1);
                }

                public static int stb_vorbis_seek_start(stb_vorbis f)
                {
                    if (((f).push_mode) != 0)
                    {
                        return (int)(error(f, (int)(VORBIS_invalid_api_mixing)));
                    }

                    set_file_offset(f, (uint)(f.first_audio_page_offset));
                    f.previous_length = (int)(0);
                    f.first_decode = (byte)(1);
                    f.next_seg = (int)(-1);
                    return (int)(vorbis_pump_first_frame(f));
                }

                public static uint stb_vorbis_stream_length_in_samples(stb_vorbis f)
                {
                    uint restore_offset;
                    uint previous_safe;
                    uint end;
                    uint last_page_loc;
                    if (((f).push_mode) != 0) return (uint)(error(f, (int)(VORBIS_invalid_api_mixing)));
                    if (f.total_samples == 0)
                    {
                        uint last;
                        uint lo;
                        uint hi;
                        sbyte* header = stackalloc sbyte[6];
                        restore_offset = (uint)(stb_vorbis_get_file_offset(f));
                        if (((f.stream_len) >= (65536)) && ((f.stream_len - 65536) >= (f.first_audio_page_offset)))
                            previous_safe = (uint)(f.stream_len - 65536);
                        else previous_safe = (uint)(f.first_audio_page_offset);
                        set_file_offset(f, (uint)(previous_safe));
                        if (vorbis_find_page(f, &end, &last) == 0)
                        {
                            f.error = (int)(VORBIS_cant_find_last_page);
                            f.total_samples = (uint)(0xffffffff);
                            goto done;
                        }
                        last_page_loc = (uint)(stb_vorbis_get_file_offset(f));
                        while (last == 0)
                        {
                            set_file_offset(f, (uint)(end));
                            if (vorbis_find_page(f, &end, &last) == 0)
                            {
                                break;
                            }
                            previous_safe = (uint)(last_page_loc + 1);
                            last_page_loc = (uint)(stb_vorbis_get_file_offset(f));
                        }
                        set_file_offset(f, (uint)(last_page_loc));
                        getn(f, (byte*)(header), (int)(6));
                        lo = (uint)(get32(f));
                        hi = (uint)(get32(f));
                        if (((lo) == (0xffffffff)) && ((hi) == (0xffffffff)))
                        {
                            f.error = (int)(VORBIS_cant_find_last_page);
                            f.total_samples = (uint)(0xffffffff);
                            goto done;
                        }
                        if ((hi) != 0) lo = (uint)(0xfffffffe);
                        f.total_samples = (uint)(lo);
                        f.p_last.page_start = (uint)(last_page_loc);
                        f.p_last.page_end = (uint)(end);
                        f.p_last.last_decoded_sample = (uint)(lo);
                        done:
                        ;
                        set_file_offset(f, (uint)(restore_offset));
                    }

                    return (uint)((f.total_samples) == (0xffffffff) ? 0 : f.total_samples);
                }

                public static float stb_vorbis_stream_length_in_seconds(stb_vorbis f)
                {
                    return (float)(stb_vorbis_stream_length_in_samples(f) / (float)(f.sample_rate));
                }

                public static int stb_vorbis_get_frame_float(stb_vorbis f, int* channels, ref float*[] output)
                {
                    int len;
                    int right;
                    int left;
                    int i;
                    if (((f).push_mode) != 0) return (int)(error(f, (int)(VORBIS_invalid_api_mixing)));
                    if (vorbis_decode_packet(f, &len, &left, &right) == 0)
                    {
                        f.channel_buffer_start = (int)(f.channel_buffer_end = (int)(0));
                        return (int)(0);
                    }

                    len = (int)(vorbis_finish_frame(f, (int)(len), (int)(left), (int)(right)));
                    for (i = (int)(0); (i) < (f.channels); ++i)
                    {
                        f.outputs[i] = f.channel_buffers[i] + left;
                    }
                    f.channel_buffer_start = (int)(left);
                    f.channel_buffer_end = (int)(left + len);
                    if ((channels) != null) *channels = (int)(f.channels);
                    output = f.outputs;
                    return (int)(len);
                }

                public static stb_vorbis stb_vorbis_open_memory(byte* data, int len, int* error, stb_vorbis_alloc* alloc)
                {
                    stb_vorbis f;
                    stb_vorbis p = new stb_vorbis();
                    if ((data) == (null)) return (null);
                    vorbis_init(p, alloc);
                    p.stream = data;
                    p.stream_end = data + len;
                    p.stream_start = p.stream;
                    p.stream_len = (uint)(len);
                    p.push_mode = (byte)(0);
                    if ((start_decoder(p)) != 0)
                    {
                        f = vorbis_alloc(p);
                        if ((f) != null)
                        {
                            f = (stb_vorbis)(p);
                            vorbis_pump_first_frame(f);
                            if ((error) != null) *error = (int)(VORBIS__no_error);
                            return f;
                        }
                    }

                    if ((error) != null) *error = (int)(p.error);
                    vorbis_deinit(p);
                    return (null);
                }

                public static void copy_samples(short* dest, float* src, int len)
                {
                    int i;
                    for (i = (int)(0); (i) < (len); ++i)
                    {
                        int v = ((int)((src[i]) * (1 << (15))));
                        if (((uint)(v + 32768)) > (65535)) v = (int)((v) < (0) ? -32768 : 32767);
                        dest[i] = (short)(v);
                    }
                }

                public static void compute_samples(int mask, short* output, int num_c, float*[] data, int d_offset, int len)
                {
                    float* buffer = stackalloc float[32];
                    int i;
                    int j;
                    int o;
                    int n = (int)(32);
                    for (o = (int)(0); (o) < (len); o += (int)(32))
                    {
                        CRuntime.memset(buffer, (int)(0), (ulong)(sizeof(float) * 32));
                        if ((o + n) > (len)) n = (int)(len - o);
                        for (j = (int)(0); (j) < (num_c); ++j)
                        {
                            if ((channel_position[num_c, j] & mask) != 0)
                            {
                                for (i = (int)(0); (i) < (n); ++i)
                                {
                                    buffer[i] += (float)(data[j][d_offset + o + i]);
                                }
                            }
                        }
                        for (i = (int)(0); (i) < (n); ++i)
                        {
                            int v = ((int)((buffer[i]) * (1 << (15))));
                            if (((uint)(v + 32768)) > (65535)) v = (int)((v) < (0) ? -32768 : 32767);
                            output[o + i] = (short)(v);
                        }
                    }
                }

                public static void compute_stereo_samples(short* output, int num_c, float*[] data, int d_offset, int len)
                {
                    float* buffer = stackalloc float[32];
                    int i;
                    int j;
                    int o;
                    int n = (int)(32 >> 1);
                    for (o = (int)(0); (o) < (len); o += (int)(32 >> 1))
                    {
                        int o2 = (int)(o << 1);
                        CRuntime.memset(buffer, (int)(0), (ulong)(sizeof(float) * 32));
                        if ((o + n) > (len)) n = (int)(len - o);
                        for (j = (int)(0); (j) < (num_c); ++j)
                        {
                            int m = (int)(channel_position[num_c, j] & (2 | 4));
                            if ((m) == (2 | 4))
                            {
                                for (i = (int)(0); (i) < (n); ++i)
                                {
                                    buffer[i * 2 + 0] += (float)(data[j][d_offset + o + i]);
                                    buffer[i * 2 + 1] += (float)(data[j][d_offset + o + i]);
                                }
                            }
                            else if ((m) == (2))
                            {
                                for (i = (int)(0); (i) < (n); ++i)
                                {
                                    buffer[i * 2 + 0] += (float)(data[j][d_offset + o + i]);
                                }
                            }
                            else if ((m) == (4))
                            {
                                for (i = (int)(0); (i) < (n); ++i)
                                {
                                    buffer[i * 2 + 1] += (float)(data[j][d_offset + o + i]);
                                }
                            }
                        }
                        for (i = (int)(0); (i) < (n << 1); ++i)
                        {
                            int v = ((int)((buffer[i]) * (1 << (15))));
                            if (((uint)(v + 32768)) > (65535)) v = (int)((v) < (0) ? -32768 : 32767);
                            output[o2 + i] = (short)(v);
                        }
                    }
                }

                public static void convert_samples_short(int buf_c, short** buffer, int b_offset, int data_c, float*[] data,
                    int d_offset, int samples)
                {
                    int i;
                    if (((buf_c != data_c) && (buf_c <= 2)) && (data_c <= 6))
                    {
                        for (i = (int)(0); (i) < (buf_c); ++i)
                        {
                            compute_samples((int)(channel_selector[buf_c, i]), buffer[i] + b_offset, (int)(data_c), data, (int)(d_offset),
                                (int)(samples));
                        }
                    }
                    else
                    {
                        int limit = (int)((buf_c) < (data_c) ? buf_c : data_c);
                        for (i = (int)(0); (i) < (limit); ++i)
                        {
                            copy_samples(buffer[i] + b_offset, data[i] + d_offset, (int)(samples));
                        }
                        for (; (i) < (buf_c); ++i)
                        {
                            CRuntime.memset(buffer[i] + b_offset, (int)(0), (ulong)(sizeof(short) * samples));
                        }
                    }

                }

                public static int stb_vorbis_get_frame_short(stb_vorbis f, int num_c, short** buffer, int num_samples)
                {
                    float*[] output = null;
                    int len = (int)(stb_vorbis_get_frame_float(f, (null), ref output));
                    if ((len) > (num_samples)) len = (int)(num_samples);
                    if ((len) != 0)
                        convert_samples_short((int)(num_c), buffer, (int)(0), (int)(f.channels), output, (int)(0), (int)(len));
                    return (int)(len);
                }

                public static void convert_channels_short_interleaved(int buf_c, short* buffer, int data_c, float*[] data,
                    int d_offset, int len)
                {
                    int i;
                    if (((buf_c != data_c) && (buf_c <= 2)) && (data_c <= 6))
                    {
                        for (i = (int)(0); (i) < (buf_c); ++i)
                        {
                            compute_stereo_samples(buffer, (int)(data_c), data, (int)(d_offset), (int)(len));
                        }
                    }
                    else
                    {
                        int limit = (int)((buf_c) < (data_c) ? buf_c : data_c);
                        int j;
                        for (j = (int)(0); (j) < (len); ++j)
                        {
                            for (i = (int)(0); (i) < (limit); ++i)
                            {
                                float f = (float)(data[i][d_offset + j]);
                                int v = ((int)((f) * (1 << (15))));
                                if (((uint)(v + 32768)) > (65535)) v = (int)((v) < (0) ? -32768 : 32767);
                                *buffer++ = (short)(v);
                            }
                            for (; (i) < (buf_c); ++i)
                            {
                                *buffer++ = (short)(0);
                            }
                        }
                    }

                }

                public static int stb_vorbis_get_frame_short_interleaved(stb_vorbis f, int num_c, short* buffer, int num_shorts)
                {
                    float*[] output = null;
                    int len;
                    if ((num_c) == (1)) return (int)(stb_vorbis_get_frame_short(f, (int)(num_c), &buffer, (int)(num_shorts)));
                    len = (int)(stb_vorbis_get_frame_float(f, (null), ref output));
                    if ((len) != 0)
                    {
                        if ((len * num_c) > (num_shorts)) len = (int)(num_shorts / num_c);
                        convert_channels_short_interleaved((int)(num_c), buffer, (int)(f.channels), output, (int)(0), (int)(len));
                    }

                    return (int)(len);
                }

                public static int stb_vorbis_get_samples_short_interleaved(stb_vorbis f, int channels, short* buffer, int num_shorts)
                {
                    float*[] outputs = null;
                    int len = (int)(num_shorts / channels);
                    int n = (int)(0);
                    int z = (int)(f.channels);
                    if ((z) > (channels)) z = (int)(channels);
                    while ((n) < (len))
                    {
                        int k = (int)(f.channel_buffer_end - f.channel_buffer_start);
                        if ((n + k) >= (len)) k = (int)(len - n);
                        if ((k) != 0)
                            convert_channels_short_interleaved((int)(channels), buffer, (int)(f.channels), f.channel_buffers,
                                (int)(f.channel_buffer_start), (int)(k));
                        buffer += k * channels;
                        n += (int)(k);
                        f.channel_buffer_start += (int)(k);
                        if ((n) == (len)) break;
                        if (stb_vorbis_get_frame_float(f, (null), ref outputs) == 0) break;
                    }
                    return (int)(n);
                }

                public static int stb_vorbis_get_samples_short(stb_vorbis f, int channels, short** buffer, int len)
                {
                    float*[] outputs = null;
                    int n = (int)(0);
                    int z = (int)(f.channels);
                    if ((z) > (channels)) z = (int)(channels);
                    while ((n) < (len))
                    {
                        int k = (int)(f.channel_buffer_end - f.channel_buffer_start);
                        if ((n + k) >= (len)) k = (int)(len - n);
                        if ((k) != 0)
                            convert_samples_short((int)(channels), buffer, (int)(n), (int)(f.channels), f.channel_buffers,
                                (int)(f.channel_buffer_start), (int)(k));
                        n += (int)(k);
                        f.channel_buffer_start += (int)(k);
                        if ((n) == (len)) break;
                        if (stb_vorbis_get_frame_float(f, (null), ref outputs) == 0) break;
                    }
                    return (int)(n);
                }

                public static int stb_vorbis_decode_memory(byte* mem, int len, int* channels, int* sample_rate, ref short* output)
                {
                    int data_len;
                    int offset;
                    int total;
                    int limit;
                    int error;
                    short* data;
                    stb_vorbis v = stb_vorbis_open_memory(mem, (int)(len), &error, (null));
                    if ((v) == (null)) return (int)(-1);
                    limit = (int)(v.channels * 4096);
                    *channels = (int)(v.channels);
                    if ((sample_rate) != null) *sample_rate = (int)(v.sample_rate);
                    offset = (int)(data_len = (int)(0));
                    total = (int)(limit);
                    data = (short*)(CRuntime.malloc((ulong)(total * sizeof(short))));
                    if ((data) == (null))
                    {
                        stb_vorbis_close(v);
                        return (int)(-2);
                    }

                    for (; ; )
                    {
                        int n = (int)(stb_vorbis_get_frame_short_interleaved(v, (int)(v.channels), data + offset, (int)(total - offset)));
                        if ((n) == (0)) break;
                        data_len += (int)(n);
                        offset += (int)(n * v.channels);
                        if ((offset + limit) > (total))
                        {
                            short* data2;
                            total *= (int)(2);
                            data2 = (short*)(CRuntime.realloc(data, (ulong)(total * sizeof(short))));
                            if ((data2) == (null))
                            {
                                CRuntime.free(data);
                                stb_vorbis_close(v);
                                return (int)(-2);
                            }
                            data = data2;
                        }
                    }
                    output = data;
                    stb_vorbis_close(v);
                    return (int)(data_len);
                }

                public static int stb_vorbis_get_samples_float_interleaved(stb_vorbis f, int channels, float* buffer, int num_floats)
                {
                    float*[] outputs = null;
                    int len = (int)(num_floats / channels);
                    int n = (int)(0);
                    int z = (int)(f.channels);
                    if ((z) > (channels)) z = (int)(channels);
                    while ((n) < (len))
                    {
                        int i;
                        int j;
                        int k = (int)(f.channel_buffer_end - f.channel_buffer_start);
                        if ((n + k) >= (len)) k = (int)(len - n);
                        for (j = (int)(0); (j) < (k); ++j)
                        {
                            for (i = (int)(0); (i) < (z); ++i)
                            {
                                *buffer++ = (float)(f.channel_buffers[i][f.channel_buffer_start + j]);
                            }
                            for (; (i) < (channels); ++i)
                            {
                                *buffer++ = (float)(0);
                            }
                        }
                        n += (int)(k);
                        f.channel_buffer_start += (int)(k);
                        if ((n) == (len)) break;
                        if (stb_vorbis_get_frame_float(f, (null), ref outputs) == 0) break;
                    }
                    return (int)(n);
                }

                public static int stb_vorbis_get_samples_float(stb_vorbis f, int channels, float** buffer, int num_samples)
                {
                    float*[] outputs = null;
                    int n = (int)(0);
                    int z = (int)(f.channels);
                    if ((z) > (channels)) z = (int)(channels);
                    while ((n) < (num_samples))
                    {
                        int i;
                        int k = (int)(f.channel_buffer_end - f.channel_buffer_start);
                        if ((n + k) >= (num_samples)) k = (int)(num_samples - n);
                        if ((k) != 0)
                        {
                            for (i = (int)(0); (i) < (z); ++i)
                            {
                                CRuntime.memcpy(buffer[i] + n, f.channel_buffers[i] + f.channel_buffer_start, (ulong)(sizeof(float) * k));
                            }
                            for (; (i) < (channels); ++i)
                            {
                                CRuntime.memset(buffer[i] + n, (int)(0), (ulong)(sizeof(float) * k));
                            }
                        }
                        n += (int)(k);
                        f.channel_buffer_start += (int)(k);
                        if ((n) == (num_samples)) break;
                        if (stb_vorbis_get_frame_float(f, (null), ref outputs) == 0) break;
                    }
                    return (int)(n);
                }
            }
            enum ImageProcessingMode
            {
                Read,
                Write,
            }
        }

        /// <summary>
        /// Represents an object which holds animeted GIF image information.
        /// </summary>
        public struct AnimatedGifFrame : IAnimatedGifFrame
        {
            public AnimatedGifFrame(byte[] data, int delay)
            {
                Data = data;
                Delay = delay;
            }

            /// <summary>
            /// Data of the image in byte array.
            /// </summary>
            public byte[] Data { get; private set; }

            /// <summary>
            /// Delay unit to be used to change a frame.
            /// </summary>
            public int Delay { get; private set; }
        }
    }
}
