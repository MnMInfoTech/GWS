/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MnM.GWS
{
    #region IMAGE PROCESSOR
    /// <summary>
    /// Represents an object to facilitate image data processing. GWS uses default image reader derived from STBImage. 
    /// </summary>
    public interface IImageProcessor : IAttachment
    {
        /// <summary>
        /// Reads a memory stream and provides processed data to be used for creating memory buffer.
        /// Uses implementation of STB image reading.
        /// for more info on STBImage visit: https://github.com/nothings/stb
        /// </summary>
        /// <param name="stream">Strem to process</param>
        /// <returns>
        /// Pair.Item1 - data in bytes array
        /// Pair.Item2 - Width information.
        /// Pair.Item3 - Height information.
        /// </returns>
        Task<Tuple<byte[], int, int>> Read(Stream stream);

        /// <summary>
        /// Writes a given memory block to a file on a given stream.
        /// Uses implementation of STB image reading.
        /// for more info on STBImage visit: https://github.com/nothings/stb
        /// </summary>
        /// <param name="pixels">Memory block to write to disk file.</param>
        /// <param name="width">Width of memory block.</param>
        /// <param name="height">Height of memory block.</param>
        /// <param name="dest">Destination stream where image to be writtwn.</param>
        Task<bool> Write(IntPtr pixels, int width, int height, Stream dest, IEnumerable<IParameter> parameters = null);

        /// <summary>
        /// Gets a grey-scalled image from given source data.
        /// </summary>
        /// <param name="Source">Memory block to get grey-scalled image from..</param>
        /// <param name="width">Width of the source data block.</param>
        /// <param name="height">Height of the source data block.</param>
        /// <param name="greyScale">Grey-scale enum option to get variety of different grey-scale results.</param>
        /// <returns></returns>
        Task<Tuple<IntPtr, bool>> GreyScaleImage
           (IntPtr Source, ref int width, ref int height, GreyScale greyScale = GreyScale.JPEG);

        /// <summary>
        /// Rotates and scales memory block with specified rotation and scale factor with or without anti-aliasing.
        /// Source: https://www.drdobbs.com/architecture-and-design/fast-bitmap-rotation-and-scaling/184416337
        /// Original method does not do anti-alising but this one does.
        /// </summary>
        /// <param name="source">source data which to scale and rotate from.</param>
        /// <param name="srcW"></param>
        /// <param name="srcH"></param>
        /// <param name="x">Optional X co-ordinate of location where rotation and scaling starts from.</param>
        /// <param name="y">Optional Y co-ordinate of location where rotation and scaling starts from.</param>
        /// <param name="w">Optional Width of portion area up to which horizontal rotation and scaling spans.</param>
        /// <param name="h">Optional Width of portion area up to which vertical rotation and scaling spans.</param>
        /// <param name="interpolation">Interpolation mode to be used while rotating and scaling.</param>
        /// <param name="SizeToFit">If true, destination image size will be adjusted to fit the rotated size.
        /// othereise destination image size remains the same as source image size.</param>
        /// <param name="rotation">Rotation object to apply for this operation</param>
        /// <param name="scale">Scale object to apply for this operation.</param>
        /// <param name="BackgroundPen">Background pen to be used to provide pixels in case source colour is empty.</param>
        /// <param name="InvertBkgColour">If true, Correspoing background-pen colour would be inverted while using. </param>
        /// <returns>Resultant rotated and scalled data along with its size.</returns>
        Task<Tuple<ISize, IntPtr>> RotateAndScale(
            IntPtr source,
            int srcW, int srcH,
            int x, int y, int w, int h,
            Interpolation interpolation,
            bool SizeToFit,
            IRotation rotation,
            IScale scale,
            IPen BackgroundPen = null,
            bool InvertBkgColour = false
        );

        /// <summary>
        /// Returns resized version of source data.
        /// </summary>
        /// <param name="Source">Pixels of source data.</param>
        /// <param name="srcW">Width of source data.</param>
        /// <param name="srcH">Height of source data.</param>
        /// <param name="newWidth">New width of result data expected after resizing.</param>
        /// <param name="newHeight">New height of result data expected after resizing.</param>
        /// <param name="srcCopyX">Optional X co-ordinate of location where resizing starts from.</param>
        /// <param name="srcCopyY">Optional Y co-ordinate of location where resizing starts from.</param>
        /// <param name="srcCopyW">Optional Width of portion area up to which horizontal resizing spans.</param>
        /// <param name="srcCopyH">Optional Width of portion area up to which vertical resizing spans.</param>
        /// <param name="BackgroundPen">Background pen to be used to provide pixels in case source colour is empty.</param>
        /// <param name="InvertBkgColour">If true, Correspoing background-pen colour would be inverted while using. </param>
        /// <returns>True if resizing is happend along with result otherwise false along with original data.</returns>
        Task<bool> Resize
        (
            IntPtr Source,
            int srcW, int srcH,
            int newWidth, int newHeight,
            Interpolation interpolation,
            out IntPtr result,
            int? srcCopyX = null, int? srcCopyY = null,
            int? srcCopyW = null, int? srcCopyH = null,
            IPen BackgroundPen = null,
            bool InvertBkgColour = false
        );

        /// <summary>
        /// Load GIF from file.
        /// Uses implementation of STB image reading.
        /// for more info on STBImage visit: https://github.com/nothings/stb
        /// </summary>
        /// <param name="stream">Stream to process</param>
        /// <param name="comp">actual colour composition</param>
        /// <param name="requiredComposition">Required colour composition</param>
        /// <returns></returns>
        IAnimatedGifFrame[] ReadAnimatedGif(Stream stream, out int w, out int h, out int comp, int requiredComposition);
    }
    #endregion

    #region IANIMATED-GIF-FRAME
    /// <summary>
    /// Represents an object which holds animeted GIF image information.
    /// </summary>
    public interface IAnimatedGifFrame
    {
        /// <summary>
        /// Data of the image in byte array.
        /// </summary>
        byte[] Data { get; }

        /// <summary>
        /// Delay unit to be used to change a frame.
        /// </summary>
        int Delay { get; }
    }
    #endregion
}
namespace MnM.GWS
{
#if DevSupport
    public
#else
    internal
#endif
    abstract class ImageProcessor : IImageProcessor
    {
        #region VARIABLES
        static readonly int[] ConvolveX = { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
        static readonly int[] ConvolveY = { -1, -1, -1, 0, 0, 0, 1, 1, 1 };
        static int KernelSize = 3;
        const int Inversion = Colours.Inversion;
        const int Big = Vectors.Big;
        const int BigExp = Vectors.BigExp;
        const float Half = .5f;

        const byte ZERO = 0;
        const byte ONE = 1;
        const byte TWO = 2;
        const byte THREE = 3;
        const byte MAX = 255;

        const float Epsilon = .0001f;
        const float OneBy3 = 1f / 3f;
        const float OneBy6 = 1f / 6f;
        const float TwoBy3 = 2f / 3f;
        #endregion

        #region READ
        public abstract Task<Tuple<byte[], int, int>> Read(Stream stream);

#if (GWS || Window)
        public unsafe IObject ReadShape(string path, out int width, out int height)
        {
            PrimitiveList<VectorF> edges = new PrimitiveList<VectorF>(1000);
            using (var stream = File.Open(path, FileMode.Open))
            {
                var t = Read(stream).Result;
                var len = t.Item1.Length;
                width = height = 0;

                fixed (byte* b = t.Item1)
                {
                    var i = 0;
                    if
                    (
                        b[i++] != 'G'
                        || b[i++] != 'W'
                        || b[i++] != 'S'
                    )
                    {
                        return new Polygon(edges);
                    }
                    ushort x, y;
                    byte dx, dy;
                    PointKind kind;
                    byte p1, p2;

                    p1 = b[i++];
                    p2 = b[i++];
                    width = (ushort)(p1 + (p2 << 8));

                    p1 = b[i++];
                    p2 = b[i++];
                    height = (ushort)(p1 + (p2 << 8));

                    while (i < len - 7)
                    {
                        p1 = b[i++];
                        p2 = b[i++];
                        x = (ushort)(p1 + (p2 << 8));

                        p1 = b[i++];
                        p2 = b[i++];
                        y = (ushort)(p1 + (p2 << 8));

                        dx = b[++i];
                        dy = b[++i];
                        kind = (PointKind)b[++i];

                        edges.Add(new VectorF(x + Colours.Alphas[dx], y + Colours.Alphas[dy], kind));
                    }
                    return new Polygon(edges);
                }
            }
        }
        #endif

        #endregion

        #region WRITE
        public async Task<bool> Write(IntPtr pixels, int width, int height,
            Stream dest, IEnumerable<IParameter> parameters = null)
        {
            parameters.ExtractImageSaveParameters(
                out ImageFormat format,
                out GreyScale greyScale,
                out byte quality,
                out byte pitch,
                out byte transparency);

            var data = pixels;

            if (greyScale != 0)
            {
                var res = await GreyScaleImage(pixels, ref width, ref height, greyScale);
                if (res.Item2)
                    data = res.Item1;
            }
            int len = width * height;

            return await PerformImageWriting
            (
                data,
                width,
                height,
                len,
                dest,
                format,
                quality,
                pitch,
                transparency
            );
        }

        protected abstract Task<bool> PerformImageWriting
        (
            IntPtr data,
            int w, int h,
            int len,
            Stream dsest,
            ImageFormat format,
            byte quality,
            byte pitch,
            byte transparency
        );
        #endregion

        #region GREY IMAGE
        public unsafe Task<Tuple<IntPtr, bool>> GreyScaleImage
            (IntPtr Source, ref int width, ref int height, GreyScale greyScale = GreyScale.JPEG)
        {
            #region INITIALIZE RATIO VARIABLES
            int RF, GF, BF;
            int SGF = 0, SBF = 0;
            switch (greyScale)
            {
                case GreyScale.NONE:
                    return Task.FromResult(Tuple.Create(Source, false));
                case GreyScale.JPEG:
                default:
                    RF = (.3f * Big).Round();
                    GF = (.59f * Big).Round();
                    BF = (.11f * Big).Round();
                    break;
                case GreyScale.CIE:
                    RF = (.2126f * Big).Round();
                    GF = (.7152f * Big).Round();
                    BF = (.0722f * Big).Round();
                    break;
                case GreyScale.AVG:
                    RF = (.333f * Big).Round();
                    GF = (.333f * Big).Round();
                    BF = (.333f * Big).Round();
                    break;
            }

            bool BlackAndWhite = greyScale == GreyScale.BLACK_WHITE;
            bool Sepia = greyScale == GreyScale.SEPIA;
            if (Sepia)
            {
                SGF = (.95f * Big).Round();
                SBF = (.82f * Big).Round();
            }
            #endregion

            #region INITIALIZE VARIABLES
            var result = new int[width * height];
            int len = width * height;
            int length = len * 4;
            var s = (byte*)Source;
            int m = 0;
            byte r, g, b, a;
            #endregion

            fixed (int* res = result)
            {
                byte* d = (byte*)res;

                #region GEREY CONVERSION LOOP
                while (m < length)
                {
                    int grayScale = (((s[m] * RF) + (s[m + 1] * GF) + (s[m + 2] * BF)) >> BigExp);
                    if (grayScale == 0)
                    {
                        m += 4;
                        continue;
                    }
                    r = g = b = (byte)grayScale;
                    a = s[m + 3];
                    if (BlackAndWhite)
                    {
                        if (grayScale < 127)
                        {
                            r = g = b = 0;
                            a = 255;
                        }
                        else
                            r = g = b = a = 255;
                    }
                    else if (Sepia)
                    {
                        g = (byte)(g * SGF >> BigExp);
                        b = (byte)(b * SBF >> BigExp);
                    }
                    d[m++] = r;
                    d[m++] = g;
                    d[m++] = b;
                    d[m++] = a;
                }
                #endregion

                switch (greyScale)
                {
                    case GreyScale.NONE:
                    case GreyScale.JPEG:
                    case GreyScale.CIE:
                    case GreyScale.SEPIA:
                    case GreyScale.AVG:
                    case GreyScale.BLACK_WHITE:
                    default:
                        return Task.FromResult(Tuple.Create((IntPtr)res, true));
                    case GreyScale.SEBEL_EDGES:
                        return SebelEdgeDetection(res, width, height);
                }
            }
        }
        #endregion

        #region SEBEL EDGE DETECTION
        unsafe Task<Tuple<IntPtr, bool>> SebelEdgeDetection(int* input, int width, int height)
        {
            #region SOBLE EDGE DETECTION ROUTINE
            int* convolveX, convolveY;
            fixed (int* p = ConvolveX)
                convolveX = p;
            fixed (int* p = ConvolveY)
                convolveY = p;

            int HalfSize = KernelSize / 2;
            int NHalfSize = -HalfSize;
            var xlast = width - NHalfSize - 1;
            var ylast = height - NHalfSize - 1;
            IntPtr Edges;
            int index, i;
            int len = width * height;

            fixed (int* output = new int[len])
            {
                fixed (int* diffx = new int[len])
                {
                    fixed (int* diffy = new int[len])
                    {
                        fixed (int* mag = new int[len])
                        {
                            int valx, valy;

                            index = HalfSize + HalfSize * width;

                            for (int y = HalfSize; y < ylast; y++, index += width)
                            {
                                i = index;
                                for (int x = HalfSize; x < xlast; x++, i++)
                                {
                                    valx = 0;
                                    valy = 0;
                                    for (int x1 = 0; x1 < KernelSize; x1++)
                                    {
                                        for (int y1 = 0; y1 < KernelSize; y1++)
                                        {
                                            int pos = (y1 * KernelSize + x1);
                                            int imPos = (x + (x1 - HalfSize)) +
                                                ((y + (y1 - HalfSize)) * width);

                                            valx += ((input[imPos] & 0xff) * convolveX[pos]);
                                            valy += ((input[imPos] & 0xff) * convolveY[pos]);
                                        }
                                    }
                                    diffx[i] = valx;
                                    diffy[i] = valy;
                                    mag[i] = (int)(Math.Sqrt((valx * valx) + (valy * valy)));
                                }
                            }

                            index = 1 + width;

                            for (int y = 1; y < height - 1; y++, index += width)
                            {
                                i = index;

                                for (int x = 1; x < width - 1; x++, i++)
                                {
                                    if (input[i] == 0)
                                    {
                                        continue;
                                    }
                                    int dx = diffx[i] > 0 ? 1 : -1;
                                    int dy = diffy[i] > 0 ? 1 : -1;

                                    int a1, a2, b1, b2, A, B, point, val;
                                    int difx = diffx[i];
                                    int dify = diffy[i];
                                    if (difx < 0)
                                        difx = -difx;
                                    if (dify < 0)
                                        dify = -dify;

                                    int dhy = dy * height;

                                    if (difx > dify)
                                    {
                                        a1 = mag[i + dx];
                                        a2 = mag[i + dx - dhy];
                                        b1 = mag[i - dx];
                                        b2 = mag[i - dx + dhy];
                                        A = (difx - dify) * a1 + dify * a2;
                                        B = (difx - dify) * b1 + dify * b2;
                                        point = mag[i] * difx;
                                        val = point >= A && point > B ? difx : 0;
                                    }
                                    else
                                    {
                                        a1 = mag[i - dhy];
                                        a2 = mag[i + dx - dhy];
                                        b1 = mag[i + dhy];
                                        b2 = mag[i - dx + dhy];
                                        A = (dify - difx) * a1 + difx * a2;
                                        B = (dify - difx) * b1 + difx * b2;
                                        point = mag[i] * dify;
                                        val = point >= A && point > B ? dify : 0;
                                    }
                                    int colour = (int)(0xff000000 | (val << 16 | val << 8 | val));
                                    output[i] = colour ^ Colours.Inversion;
                                }
                            }
                        }
                    }
                }
                Edges = (IntPtr)output;
            }
            #endregion

            return Task.FromResult(Tuple.Create(Edges, true));
        }
        #endregion

        #region GWS EDGE DETECTION
        unsafe IntPtr GWSEdgeDetection(int* src, ref int width, ref int height)
        {
            int len = width * height;
            var eData = new byte[len * 5];
            int index = 0;
            int i = 0;
            var h = (ushort)(height);
            var w = (ushort)(width);
            int count = 0;

            eData[count++] = (byte)('G');
            eData[count++] = (byte)('W');
            eData[count++] = (byte)('S');

            eData[count++] = (byte)width;
            eData[count++] = (byte)(width >> 8);

            eData[count++] = (byte)height;
            eData[count++] = (byte)(height >> 8);

            int c1, c2 = 0;

            for (ushort y = 0; y < h; y++, index += width)
            {
                i = index;
                for (ushort x = 0; x < w; x++, i++)
                {
                    c1 = src[i];
                    if (c2 == c1)
                        goto NEXT;

                    eData[count++] = (byte)x;
                    eData[count++] = (byte)(x >> 8);

                    eData[count++] = (byte)y;
                    eData[count++] = (byte)(y >> 8);

                    eData[count++] = 0;

                    NEXT:
                    c2 = c1;
                }
            }

            count += 7;
            Array.Resize(ref eData, count);
            count /= 5;
            if (count <= width)
            {
                width = count;
                height = 1;
            }
            else
            {
                var ww = (float)Math.Sqrt(count);
                width = (int)ww;
                height = (int)ww;
                if (ww - height != 0)
                    ++height;
            }
            fixed (byte* bdd = eData)
                return (IntPtr)bdd;
        }
        #endregion

        #region ROTATE AND SCALE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<Tuple<ISize, IntPtr>> RotateAndScale
        (
            IntPtr source,
            int srcW, int srcH,
            int x, int y, int w, int h,
            Interpolation interpolation,
            bool SizeToFit,
            IRotation rotation,
            IScale scale,
            IPen BackgroundPen = null,
            bool InvertBkgColour = false
        )
        {
            bool NoRotation = rotation == null || !rotation.Valid;
            bool NoSkew = NoRotation || rotation.Skew == null || !rotation.Skew.HasScale;
            bool NoScale = scale == null || !scale.HasScale;
            bool HasAngle = !NoRotation && rotation.HasAngle;

            if (
                x == 0 && y == 0
                && w == srcW && h == srcH
                && NoRotation
                && NoScale)
            {
                return Tuple.Create((ISize)new Size(w, h), source);
            }

            IntPtr Pixels = source;

            #region PEN VARIABLES
            bool IsColourPen = BackgroundPen == null || BackgroundPen is IColour;
            BackgroundPen = BackgroundPen ?? Rgba.Transparent;
            var bkgColour = IsColourPen ? ((IColour)BackgroundPen).Colour : Colours.Transparent;
            if (InvertBkgColour)
                bkgColour ^= Inversion;
            #endregion

            #region SIZE VARIABLES INTIALIZATION
            int dstW = srcW;
            int dstH = srcH;
            int newWidth = srcW;
            int newHeight = srcH;

            int anyX = x, anyY = y;
            Rectangles.Scale(ref anyX, ref anyY, ref newWidth, ref newHeight, rotation, scale);

            if (SizeToFit)
            {
                dstW = newWidth;
                dstH = newHeight;
                if (HasAngle)
                    Rectangles.RotateRectangle(ref anyX, ref anyY, ref dstW, ref dstH, rotation.Angle);
            }
            #endregion

            #region HANDLE SCALING
            var degree = HasAngle ? rotation.Angle : 0;
            if (!NoSkew && rotation.Skew.Type == SkewType.Diagonal)
                degree += rotation.Skew.Degree;

            float ScaleX = newWidth / (float)srcW;
            float ScaleY = newHeight / (float)srcH;

            bool DifferentScale = Math.Round(ScaleX, 2) != Math.Round(ScaleY, 2);
            float Scale = ScaleX;

            if (DifferentScale)
            {
                var ok = await Resize(source, srcW, srcH, newWidth, newHeight,
                    interpolation, out IntPtr result, x, y, w, h,
                    BackgroundPen, InvertBkgColour);
                Scale = 1;
                if (ok)
                {
                    if (!NoRotation && !NoSkew)
                        return Tuple.Create((ISize)new Size(newWidth, newHeight), result);

                    Pixels = result;
                    srcW = w = newWidth;
                    srcH = h = newHeight;
                    x = y = 0;

                }
            }
            #endregion

            var rotatedData = await RotateAndScale(
                Pixels, srcW, srcH, dstW, dstH, interpolation, x, y, w, h, degree, Scale,
                IsColourPen, BackgroundPen, bkgColour, InvertBkgColour);

            return Tuple.Create((ISize)new Size(dstW, dstH), rotatedData);
        }

        unsafe Task<IntPtr> RotateAndScale(IntPtr Pixels, int srcW, int srcH, int dstW, int dstH,
            Interpolation interpolation, int x, int y, int w, int h,
            float degree, float Scale, bool IsColourPen, IPen BackgroundPen,
            int bkgColour, bool InvertBkgColour)
        {
            #region INITIALIZE VARIABLES
            float dstXf = 0;
            float dstYf = 0;
            int dstXi = 0;
            int dstYi = 0;
            int Sini = 0, Cosi = 0;
            float Sin = 0, Cos = 0;
            bool Aliased = interpolation == 0;
            #endregion

            #region ROTATION INITIALIZATION
            Angles.SinCos(degree, out Sin, out Cos);
            Angles.SinCos(degree, out Sini, out Cosi);

            if (Scale != 0 && Scale != 1)
            {
                Angles.SinCos(degree, out Sin, out Cos);
                Sin *= 1 / Scale;
                Cos *= 1 / Scale;

                Sini = (Sin * Big).Round();
                Cosi = (Cos * Big).Round();
            }

            int srcLen = w * h;
            int srcCx = w / 2;
            int srcCy = h / 2;
            int dstCx = dstW / 2;
            int dstCy = dstH / 2;

            if (Aliased)
            {
                dstXi = -(dstCx * Cosi + dstCy * Sini);
                dstYi = -(dstCy * Cosi - dstCx * Sini);
            }
            else
            {
                dstXf = srcCx - (dstCx * Cos + dstCy * Sin);
                dstYf = srcCy - (dstCy * Cos - dstCx * Sin);
            }
            #endregion

            #region LOOP VARIABLES
            float x3 = 0, y3 = 0;
            int x0 = 0, y0 = 0, xi = 0, yi = 0;
            uint colour = 0;
            int dstIndex = 0;
            int d = 0;
            int s = 0;
            int RIGHT = x + w;
            int BOTTOM = y + h;
            uint c2, c3, c4;
            #endregion

            #region LOOP
            uint[] DST = new uint[dstW * dstH];
            fixed (uint* dst = DST)
            {
                uint* src = (uint*)Pixels;
                for (int jy = 0; jy < dstH; jy++, dstIndex += dstW)
                {
                    if (Aliased)
                    {
                        xi = dstXi;
                        yi = dstYi;
                    }
                    else
                    {
                        x3 = dstXf;
                        y3 = dstYf;
                    }

                    d = dstIndex;

                    for (int jx = 0; jx < dstW; jx++, d++)
                    {
                        if (Aliased)
                        {
                            x0 = srcCx + (xi >> BigExp);
                            y0 = srcCy + (yi >> BigExp);
                        }
                        else
                        {
                            x0 = (int)x3;
                            y0 = (int)y3;
                        }

                        if
                        (
                            x0 < x || y0 < y
                            || x0 >= RIGHT || y0 >= BOTTOM
                        )
                        {
                            goto HORIZONTAL_INCREMENT;
                        }

                        s = x0 + (y0 * srcW);
                        colour = src[s];
                        c2 = src[s + 1];
                        c3 = src[s + srcW];
                        c4 = src[s + srcW + 1];

                        if (!IsColourPen)
                        {
                            bkgColour = BackgroundPen.ReadPixel(x0, y0);
                            if (InvertBkgColour)
                                bkgColour ^= Inversion;
                        }
                        if (Aliased)
                        {
                            goto ASSIGN;
                        }

                        float Dx = x3 - x0;
                        float Dy = y3 - y0;

                        if (Dx == 0 && Dy == 0)
                            goto ASSIGN;

                        var bcolour = (uint)bkgColour;
                        colour = Colours.Blend(colour, c2, c3, c4, Dx, Dy, bcolour);

                        ASSIGN:
                        dst[d] = colour;

                        HORIZONTAL_INCREMENT:
                        #region HORIZONTAL INCREMENT
                        if (Aliased)
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
                    if (Aliased)
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
                return Task.FromResult((IntPtr)dst);
            }
            #endregion
        }
        #endregion

        #region RESIZE
        public unsafe Task<bool> Resize
        (
            IntPtr Source,
            int srcW, int srcH,
            int newWidth, int newHeight,
            Interpolation interpolation,
            out IntPtr result,
            int? srcCopyX = null, int? srcCopyY = null,
            int? srcCopyW = null, int? srcCopyH = null,
            IPen BackgroundPen = null,
            bool InvertBkgColour = false
        )
        {
            result = IntPtr.Zero;

            #region BLEND VARIABLES
            var RShift = Colours.RShift;
            var GShift = Colours.GShift;
            var BShift = Colours.BShift;
            var AShift = Colours.AShift;
            int colour, c1, c2, c3, c4;
            int R, G, B, A;
            int r, g, b, a;
            #endregion

            #region PEN VARIABLES
            bool IsColourPen = BackgroundPen == null || BackgroundPen is IColour;
            var bkgColour = IsColourPen ? ((IColour)BackgroundPen).Colour : Colours.Transparent;
            if (InvertBkgColour)
                bkgColour ^= Inversion;
            bool HasBrush = !IsColourPen && BackgroundPen != null;
            #endregion

            #region COPY && SCALE VARIABLES
            int LEFT = (srcCopyX ?? 0);
            int TOP = (srcCopyY ?? 0);
            int copyW = srcCopyW ?? srcW;
            int copyH = srcCopyH ?? srcH;

            int RIGHT = LEFT + copyW;
            int BOTTOM = TOP + copyH;

            float scaleX = (float)srcW / newWidth;
            float scaleY = (float)srcH / newHeight;

            if (scaleX == 0)
                scaleX = 1;
            if (scaleY == 0)
                scaleY = 1;

            bool horizontalScale = scaleX != 1;
            bool VerticalScale = scaleY != 1;

            int xRatio = (scaleX * Big).Round();
            int yRatio = (scaleY * Big).Round();
            #endregion

            if (!horizontalScale && !VerticalScale &&
                LEFT == 0 && TOP == 0 && copyW == srcW && copyH == srcH)
                return Task.FromResult(false);

            #region LOCATION VARIABLES
            int resLen = newWidth * newHeight;
            int dstIndex = 0;
            int srcIdx, dstIdx;
            int srcLen = srcW * srcH;
            int x0, y0, xFactor, yFactor;
            float x3, y3;
            float xDiff, yDiff;
            int span;
            int dx, dy = 0;
            bool success;
            #endregion

            int[] res = new int[resLen];
            int* src = (int*)Source;
            fixed (int* dst = res)
            {
                if ((byte)interpolation > 1)
                    goto COMPLEX_INTERPOLATION;

                #region NO ITERPOLATION OR BI-LINEAR
                if (interpolation == 0)
                {
                    for (int i = 0; i < newHeight; i++, dstIndex += newWidth, dy++)
                    {
                        dstIdx = dstIndex;
                        y0 = (i * yRatio) >> BigExp;
                        xFactor = xRatio;

                        for (int j = 0; j < newWidth; j++, dstIdx++, xFactor += xRatio)
                        {
                            x0 = xFactor >> BigExp;
                            if
                            (
                                x0 < LEFT || y0 < TOP
                                || x0 >= RIGHT || y0 >= BOTTOM
                            )
                            {
                                continue;
                            }
                            srcIdx = (x0 + y0 * srcW);
                            dst[dstIdx] = src[srcIdx];
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < newHeight; i++, dstIndex += newWidth, dy++)
                    {
                        dstIdx = dstIndex;
                        y0 = (i * yRatio) >> BigExp;
                        xFactor = xRatio;
                        y3 = i * scaleY;
                        x3 = scaleX;
                        yDiff = y3 - y0;
                        dx = 0;

                        for (int j = 0; j < newWidth; j++, dstIdx++, xFactor += xRatio, x3 += scaleX, dx++)
                        {
                            x0 = xFactor >> BigExp;
                            if
                            (
                                x0 < LEFT || y0 < TOP
                                || x0 >= RIGHT || y0 >= BOTTOM
                            )
                            {
                                continue;
                            }
                            srcIdx = (x0 + y0 * srcW);
                            if (src[srcIdx - 1] == 0 && src[srcIdx - srcW] == 0)
                                continue;

                            c1 = src[srcIdx];
                            c3 = src[srcIdx + srcW];
                            c2 = src[++srcIdx];
                            c4 = src[srcIdx + srcW];
                            xDiff = x3 - x0;
                            if (HasBrush)
                                bkgColour = BackgroundPen.ReadPixel(dx, dy);
                            colour = Colours.Blend(c1, c2, c3, c4, xDiff, yDiff, bkgColour);
                            dst[dstIdx] = colour;
                        }
                    }
                }
                #endregion

                goto EXIT;

                COMPLEX_INTERPOLATION:

                #region COMPLEX INTERPOLATION
                switch (interpolation)
                {
                    case Interpolation.None:
                    case Interpolation.Box:
                    case Interpolation.Bilinear:
                    case Interpolation.Triangle:
                    default:
                        span = ONE;
                        break;
                    case Interpolation.Bicubic:
                    case Interpolation.Catmull_Rom:
                    case Interpolation.Hermite:
                    case Interpolation.MitchellNetravali:
                    case Interpolation.Robidoux:
                    case Interpolation.RobidouxSharp:
                    case Interpolation.RobidouxSoft:
                    case Interpolation.Spline:
                        span = TWO;
                        break;
                    case Interpolation.Lancoz3:
                    case Interpolation.Welch:
                        span = THREE;
                        break;
                }

                yFactor = yRatio;
                y3 = scaleY;

                for (int i = 0; i < newHeight; i++, dstIndex += newWidth, dy++, yFactor += yRatio, y3 += scaleY)
                {
                    dstIdx = dstIndex;
                    y0 = yFactor >> BigExp;
                    xFactor = xRatio;
                    x3 = scaleX;
                    yDiff = y3 - y0;
                    dx = 0;
                    for (int j = 0; j < newWidth; j++, dstIdx++, xFactor += xRatio, x3 += scaleX, dx++)
                    {
                        x0 = xFactor >> BigExp;
                        if
                        (
                            x0 < LEFT || y0 < TOP
                            || x0 >= RIGHT || y0 >= BOTTOM
                        )
                        {
                            continue;
                        }
                        srcIdx = (x0 + y0 * srcW);
                        if (src[srcIdx] == 0)
                            continue;

                        xDiff = x3 - x0;
                        r = g = b = a = 0;

                        for (int yy = -span; yy <= span; yy++)
                        {
                            // Get Y cooefficient
                            var y = yDiff - yy;
                            if (y < 0)
                                y = -y;
                            var yHalf = Half * y;
                            var ySqr = y * y;
                            var yCube = ySqr * y;
                            success = GetInterpolatedValue(interpolation, y,
                                ySqr, yCube, yHalf, out float coefficientY);
                            if (!success)
                                continue;

                            int interpolatedY = y0 + yy;
                            if (interpolatedY < 0)
                                interpolatedY = 0;

                            if (interpolatedY > srcH - 1)
                                interpolatedY = srcH - 1;

                            for (int xx = -span; xx <= span; xx++)
                            {
                                var x = xx - xDiff;
                                if (x < 0)
                                    x = -x;
                                var xSqr = x * x;
                                var xCube = xSqr * x;
                                var xHalf = Half * x;
                                success = GetInterpolatedValue(interpolation, x,
                                    xSqr, xCube, xHalf, out float coefficientX);

                                if (!success)
                                    continue;

                                int kernel2 = (int)(coefficientX * coefficientY * Big);

                                int interpolatedX = x0 + xx;
                                if (interpolatedX < 0)
                                    interpolatedX = 0;

                                if (interpolatedX > srcW - 1)
                                    interpolatedX = srcW - 1;

                                var ipIndex = interpolatedX + interpolatedY * srcW;
                                colour = src[ipIndex];
                                if (colour == 0)
                                {
                                    if (HasBrush)
                                        colour = BackgroundPen.ReadPixel(dx, dy);
                                    colour = bkgColour;
                                }
                                R = ((colour >> RShift) & 0xFF);
                                G = ((colour >> GShift) & 0xFF);
                                B = ((colour >> BShift) & 0xFF);
                                A = ((colour >> AShift) & 0xFF);

                                r += kernel2 * R;
                                g += kernel2 * G;
                                b += kernel2 * B;
                                a += kernel2 * A;
                            }
                        }

                        r >>= BigExp;
                        g >>= BigExp;
                        b >>= BigExp;
                        a >>= BigExp;

                        if (a < ZERO) a = ZERO;
                        if (a < TWO) continue;
                        if (a > MAX) a = MAX;

                        if (r < ZERO) r = -r;
                        if (g < ZERO) g = -g;
                        if (b < ZERO) b = -b;
                        if (r > MAX) r = MAX;
                        if (g > MAX) g = MAX;
                        if (b > MAX) b = MAX;

                        colour = (a << AShift)
                             | ((r & 0xFF) << RShift)
                             | ((g & 0xFF) << GShift)
                             | ((b & 0xFF) << BShift);

                        dst[dstIdx] = colour;

                    }
                }
                #endregion

                EXIT:
                result = (IntPtr)dst;
                return Task.FromResult(true);
            }
        }
        #endregion

        #region READ ANIMATED GIF
        public abstract IAnimatedGifFrame[] ReadAnimatedGif(Stream stream, out int w, out int h, out int c, int rc);
        #endregion

        #region INTERPOLATION 
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool GetInterpolatedValue(Interpolation interpolation, float x,
            float xSqr, float xCube, float halfX, out float result)
        {
            result = 0;
            float temp;

            switch (interpolation)
            {
                case Interpolation.None:
                default:
                case Interpolation.Box:
                    if (x <= 0.5f)
                        result = 1;
                    break;

                case Interpolation.Bilinear:
                    result = x;
                    break;
                case Interpolation.Bicubic:
                    if (x <= 1)
                        result = (x + halfX - 2.5f) * xSqr + 1;
                    else if (x < 2)
                        result = ((((2.5f - halfX) * x) - 4) * x) + 2;
                    break;

                case Interpolation.Catmull_Rom:
                    if (x < 1)
                        result = (9 * xCube - 16 * xSqr + 6) * OneBy6;
                    else if (x < 2)
                        result = (-3 * xCube + 15 * xSqr - 24 * x + 12) * OneBy6;
                    break;
                case Interpolation.Hermite:
                    if (x < 1)
                        result = (12 * xCube - 18 * xSqr + 6) * OneBy6;
                    break;

                case Interpolation.Lancoz3:
                case Interpolation.Welch:
                    if (x < 3)
                    {
                        float x1 = interpolation == Interpolation.Lancoz3 ?
                            x * OneBy3 : 1.0f - (xSqr * 0.111111112f);

                        if (x <= Epsilon)
                        {
                            x = 1;
                            goto LENCOZ;
                        }
                        x *= Angles.PI;
                        x = (float)(Math.Sin(x) / x);
                        temp = x;
                        if (temp < 0)
                            temp = -temp;
                        if (temp < Epsilon)
                            x = 0;

                        LENCOZ:
                        if (interpolation == Interpolation.Welch)
                            goto WELCH_LENCOZ_FINAL;
                        if (x1 <= Epsilon)
                        {
                            x1 = 1;
                            goto WELCH_LENCOZ_FINAL;
                        }

                        x1 *= Angles.PI;
                        x1 = (float)(Math.Sin(x1) / x1);
                        temp = x1;
                        if (temp < 0)
                            temp = -temp;
                        if (temp < Epsilon)
                            x1 = 0;

                        WELCH_LENCOZ_FINAL:
                        result = x * x1;
                    }
                    break;

                case Interpolation.MitchellNetravali:
                    if (x < 1)
                        result = (7 * xCube - 12 * xSqr + 5.33333349f) * OneBy6;
                    else if (x < 2)
                        result = (-2.33333325F * xCube + 12 * xSqr - 20 * x + 10.666667f) * OneBy6;
                    break;
                case Interpolation.Robidoux:
                case Interpolation.RobidouxSharp:
                case Interpolation.RobidouxSoft:
                    switch (interpolation)
                    {
                        case Interpolation.Robidoux:
                            if (x < 1)
                                result = (6.7038f * xCube - 11.5962f * xSqr + 5.2436f) * OneBy6;
                            else if (x < 2)
                                result = (-2.2436f * xCube + 11.5962f * xSqr - 19.4616f * x + 10.4872f) * OneBy6;
                            break;
                        case Interpolation.RobidouxSharp:
                            if (x < 1)
                                result = (7.428f * xCube - 12.42f * xSqr + 5.476f) * OneBy6;
                            else if (x < 2)
                                result = (-2.47599983f * xCube + 12.6419992f * xSqr - 20.855999f * x + 10.952f) * OneBy6;
                            break;
                        case Interpolation.RobidouxSoft:
                            if (x < 1)
                                result = (4.9224f * xCube - 8.8836f * xSqr + 4.6408f) * OneBy6;
                            else if (x < 2)
                                result = (-1.6408f * xCube + 8.8836f * xSqr - 15.8448f * x + 9.2816f) * OneBy6;
                            break;
                        default:
                            break;
                    }
                    break;
                case Interpolation.Spline:
                    if (x < 1)
                        result = (3 * xCube - 6 * xSqr + 4) * OneBy6;
                    else if (x < 2)
                        result = (-xCube + 6 * xSqr - 12 * x + 8) * OneBy6;
                    break;

                case Interpolation.Triangle:
                    if (x < 1)
                        result = 1 - x;
                    break;
            }

            var val = result;
            if (val < 0)
                val = -val;
            return val >= Epsilon;
        }
        #endregion

        public abstract void Dispose();
    }
}
