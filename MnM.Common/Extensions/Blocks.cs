/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public static partial class Blocks
    {
        #region CORRECT REGION
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CorrectRegion(ref int copyX, ref int copyY, ref int copyW, ref int copyH, int srcW, int srcH,
            ref int dstX, ref int dstY, int dstW, int dstLen, out int srcIndex, out int dstIndex)
        {
            #region VARIABLE INITIALIZATION
            if (copyX < 0)
            {
                copyW += copyX;
                copyX = 0;
            }
            if (copyY < 0)
            {
                copyH += copyY;
                copyY = 0;
            }

            var w = Math.Min(copyW, srcW);
            var h = Math.Min(copyH, srcH);
            var right = copyX + w;
            var bottom = copyY + h;
            if (right > srcW)
                right = srcW;
            if (bottom > srcH)
                bottom = srcH;
            copyW = right - copyX;
            copyH = bottom - copyY;
            int srcLen = srcW * srcH;
            int dstH = dstLen / dstW;
            if (dstY + copyH >= dstH)
                copyH -= (dstY + copyH - dstH);
            #endregion

            #region CORRECT COPY AND PASTE PARAMETERS
            srcIndex = copyX + copyY * srcW;

            if (dstX < 0)
                dstX = 0;
            if (dstY < 0)
                dstY = 0;

            dstIndex = dstX + dstY * dstW;

            if (copyW > dstW)
                copyW = dstW;

            if (srcIndex + copyW >= srcLen)
                copyW -= (srcIndex + copyW - srcLen);

            if (copyW <= 0)
                return;

            if (dstIndex + copyW >= dstLen)
                copyW -= (dstIndex + copyW - dstLen);
            #endregion
        }
        #endregion

        #region COPY BLOCK
        /// <summary>
        /// 
        /// </summary>
        /// <param name="copyX"></param>
        /// <param name="copyY"></param>
        /// <param name="copyW"></param>
        /// <param name="copyH"></param>
        /// <param name="srcLen"></param>
        /// <param name="srcW"></param>
        /// <param name="srcH"></param>
        /// <param name="dstX"></param>
        /// <param name="dstY"></param>
        /// <param name="dstW"></param>
        /// <param name="dstLen"></param>
        /// <param name="action"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Rectangle CopyBlock(int copyX, int copyY, int copyW, int copyH, int srcLen, int srcW, int srcH,
            int dstX, int dstY, int dstW, int dstLen, BlockCopy action, Command command)
        {
            CorrectRegion(ref copyX, ref copyY, ref copyW, ref copyH, srcW, srcH, ref dstX, ref dstY, dstW, dstLen, out int srcIndex, out int dstIndex);
            if (copyW <= 0)
                return Rectangle.Empty;
            int i = 0;
            while (i < copyH)
            {
                if (srcIndex + copyW >= srcLen)
                    copyW -= (srcIndex + copyW - srcLen);
                if (copyW <= 0)
                    break;

                if (dstIndex + copyW >= dstLen)
                    copyW -= (dstIndex + copyW - dstLen);

                if (copyW <= 0)
                    break;

                action(srcIndex, dstIndex, copyW, copyX, copyY + i, command);
                srcIndex += srcW;
                dstIndex += dstW;
                ++i;
            }
            return new Rectangle(dstX, dstY, copyW, i);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="copyX"></param>
        /// <param name="copyY"></param>
        /// <param name="copyW"></param>
        /// <param name="copyH"></param>
        /// <param name="srcLen"></param>
        /// <param name="srcW"></param>
        /// <param name="srcH"></param>
        /// <param name="dst"></param>
        /// <param name="dstX"></param>
        /// <param name="dstY"></param>
        /// <param name="dstW"></param>
        /// <param name="dstLen"></param>
        /// <param name="command"></param>
        /// <param name="srcAlphas"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IRectangle CopyBlock(int* src, int copyX, int copyY, int copyW, int copyH, int srcLen, int srcW, int srcH,
            int* dst, int dstX, int dstY, int dstW, int dstLen, Command command, byte* srcAlphas = null)
        {
            CorrectRegion(ref copyX, ref copyY, ref copyW, ref copyH, srcW, srcH, ref dstX, ref dstY, dstW, dstLen, out int srcIndex, out int dstIndex);
            if (copyW <= 0)
                return Rectangle.Empty;
            int i = 0;

            while (i < copyH)
            {
                if (srcIndex + copyW >= srcLen)
                    copyW -= (srcIndex + copyW - srcLen);
                if (copyW <= 0)
                    break;

                if (dstIndex + copyW >= dstLen)
                    copyW -= (dstIndex + copyW - dstLen);

                if (copyW <= 0)
                    break;
                Copy(src, srcIndex, dst, dstIndex, copyW, command, srcAlphas);
                srcIndex += srcW;
                dstIndex += dstW;
                ++i;
            }
            return new Rectangle(dstX, dstY, copyW, i);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="copyX"></param>
        /// <param name="copyY"></param>
        /// <param name="copyW"></param>
        /// <param name="copyH"></param>
        /// <param name="srcLen"></param>
        /// <param name="srcW"></param>
        /// <param name="srcH"></param>
        /// <param name="dst"></param>
        /// <param name="dstX"></param>
        /// <param name="dstY"></param>
        /// <param name="dstW"></param>
        /// <param name="dstLen"></param>
        /// <param name="command"></param>
        /// <param name="srcAlphas"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IRectangle CopyBlock(byte* src, int copyX, int copyY, int copyW, int copyH, int srcLen, int srcW, int srcH,
            byte* dst, int dstX, int dstY, int dstW, int dstLen, Command command)
        {
            CorrectRegion(ref copyX, ref copyY, ref copyW, ref copyH, srcW, srcH, ref dstX, ref dstY, dstW, dstLen, out int srcIndex, out int dstIndex);
            if (copyW <= 0)
                return Rectangle.Empty;
            int i = 0;

            while (i < copyH)
            {
                if (srcIndex + copyW >= srcLen)
                    copyW -= (srcIndex + copyW - srcLen);
                if (copyW <= 0)
                    break;

                if (dstIndex + copyW >= dstLen)
                    copyW -= (dstIndex + copyW - dstLen);

                if (copyW <= 0)
                    break;
                Copy(src, srcIndex, dst, dstIndex, copyW, command);
                srcIndex += srcW;
                dstIndex += dstW;
                ++i;
            }
            return new Rectangle(dstX, dstY, copyW, i);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="copyX"></param>
        /// <param name="copyY"></param>
        /// <param name="copyW"></param>
        /// <param name="copyH"></param>
        /// <param name="srcLen"></param>
        /// <param name="srcW"></param>
        /// <param name="srcH"></param>
        /// <param name="dst"></param>
        /// <param name="dstX"></param>
        /// <param name="dstY"></param>
        /// <param name="dstW"></param>
        /// <param name="dstLen"></param>
        /// <param name="command"></param>
        /// <param name="srcAlphas"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IRectangle CopyBlock<T>(T[] src, int copyX, int copyY, int copyW, int copyH, int srcLen, int srcW, int srcH,
            T[] dst, int dstX, int dstY, int dstW, int dstLen)
        {
            CorrectRegion(ref copyX, ref copyY, ref copyW, ref copyH, srcW, srcH, ref dstX, ref dstY, dstW, dstLen, out int srcIndex, out int dstIndex);
            if (copyW <= 0)
                return Rectangle.Empty;
            int i = 0;

            while (i < copyH)
            {
                if (srcIndex + copyW >= srcLen)
                    copyW -= (srcIndex + copyW - srcLen);
                if (copyW <= 0)
                    break;

                if (dstIndex + copyW >= dstLen)
                    copyW -= (dstIndex + copyW - dstLen);

                if (copyW <= 0)
                    break;
                Array.Copy(src, srcIndex, dst, dstIndex, copyW);
                srcIndex += srcW;
                dstIndex += dstW;
                ++i;
            }
            return new Rectangle(dstX, dstY, copyW, i);
        }
        #endregion

        #region COPY MEMORY
        /// <summary>
        /// Copies source memory block to destination memory block.
        /// </summary>
        /// <param name="src">Source memory block</param>
        /// <param name="srcIndex">Index in source from where copy operation should start</param>
        /// <param name="dst"></param>
        /// <param name="dstIndex">Index in destination where paste operation should start</param>
        /// <param name="length">Length of pixels to be copied</param>
        /// <param name="command">Command to control copy operation.
        /// Applicable flags: Opaque, Backdrop, InvertCanvasColor</param>
        /// <param name="alphas"></param>
        /// <param name="useDstIndexForAlphas"></param>
        /// <param name="dstCounter">Counter by which destination index moves to next position for copy.</param>
        /// <param name="srcCounter">Counter by which source index moves to next position for copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(int* src, int srcIndex, int* dst, int dstIndex, int length,
            Command command = Command.Opaque, byte* alphas = null, bool useDstIndexForAlphas = false, int dstCounter = 1, int srcCounter = 1)
        {
            if (length == 0)
                return;
            bool Opaque = (command & Command.Opaque) == Command.Opaque;
            bool Back = (command & Command.Backdrop) == Command.Backdrop;
            bool Invert = (command & Command.InvertColor) == Command.InvertColor;
            bool Clear = src == null;

            if (dstCounter <= 0)
                dstCounter = 1;
            if (srcCounter <= 0)
                srcCounter = 1;

            int srcColor = 0, dstColor;

            if (Opaque)
            {
                if (Clear)
                {
                    if (Invert)
                        srcColor ^= Colors.Inversion;

                    for (int i = 0; i < length; i++, dstIndex += dstCounter)
                    {
                        if (Back && dst[dstIndex] != 0)
                            continue;
                        dst[dstIndex] = srcColor;
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                    {
                        srcColor = src[srcIndex];
                        if (Invert)
                            srcColor ^= Colors.Inversion;
                        if (Back && dst[dstIndex] != 0)
                            continue;
                        dst[dstIndex] = srcColor;
                    }
                }
            }
            else
            {
                bool HasAlphas = alphas != null;
                if (!HasAlphas)
                {
                    if (Clear)
                    {
                        if (Invert)
                            srcColor ^= Colors.Inversion;
                        for (int i = 0; i < length; i++, dstIndex += dstCounter)
                        {
                            dstColor = dst[dstIndex];
                            if ((Back && dstColor != 0))
                                continue;
                            dst[dstIndex] = srcColor;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                        {
                            srcColor = src[srcIndex];
                            dstColor = dst[dstIndex];
                            if (srcColor == 0 || (Back && dstColor != 0))
                                continue;
                            if (Invert)
                                srcColor ^= Colors.Inversion;
                            dst[dstIndex] = srcColor;
                        }
                    }
                }
                else
                {
                    int alphaIdx;
                    uint C1, C2, RB, AG, invAlpha, alpha;
                    if (Clear)
                    {
                        if (Invert)
                            srcColor ^= Colors.Inversion;

                        for (int i = 0; i < length; i++, dstIndex += dstCounter)
                        {
                            dstColor = dst[dstIndex];
                            alphaIdx = useDstIndexForAlphas ? dstIndex : srcIndex;

                            if (Back && dstColor != 0)
                                continue;
                            dst[dstIndex] = srcColor;
                            alphas[alphaIdx] = 0;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                        {
                            srcColor = src[srcIndex];
                            dstColor = dst[dstIndex];
                            alphaIdx = useDstIndexForAlphas ? dstIndex : srcIndex;
                            alpha = alphas[alphaIdx];

                            if (srcColor == 0 || (Back && dstColor != 0 && alpha == 0))
                                continue;

                            if (alpha == 0 || alpha == 255 || dstColor == 0)
                                goto AssignColor;

                            if (Back) alpha = (255 - alpha);
                            C1 = (uint)dstColor;
                            C2 = (uint)srcColor;
                            //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-color-fast?
                            invAlpha = 255 - (uint)alpha;
                            RB = ((invAlpha * (C1 & Colors.RBMASK)) + (alpha * (C2 & Colors.RBMASK))) >> 8;
                            AG = (invAlpha * ((C1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((C2 & Colors.GMASK) >> 8)));
                            srcColor = (int)((RB & Colors.RBMASK) | (AG & Colors.AGMASK));
                        AssignColor:
                            if (Invert)
                                srcColor ^= Colors.Inversion;
                            dst[dstIndex] = srcColor;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Copies source memory block to destination memory block.
        /// </summary>
        /// <param name="src">Source memory block</param>
        /// <param name="srcIndex">Index in source from where copy operation should start</param>
        /// <param name="dst"></param>
        /// <param name="dstIndex">Index in destination where paste operation should start</param>
        /// <param name="length">Length of pixels to be copied</param>
        /// <param name="command">Command to control copy operation.
        /// Applicable flags: Opaque, Backdrop, InvertCanvasColor</param>
        /// <param name="alphas"></param>
        /// <param name="useDstIndexForAlphas"></param>
        /// <param name="dstCounter">Counter by which destination index moves to next position for copy.</param>
        /// <param name="srcCounter">Counter by which source index moves to next position for copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(byte* src, int srcIndex, byte* dst, int dstIndex, int length,
            Command command = Command.Opaque, int dstCounter = 1, int srcCounter = 1)
        {
            if (length == 0)
                return;
            bool Opaque = (command & Command.Opaque) == Command.Opaque;
            bool Back = (command & Command.Backdrop) == Command.Backdrop;
            bool Invert = (command & Command.InvertColor) == Command.InvertColor;
            bool Clear = src == null;

            if (dstCounter <= 0)
                dstCounter = 1;
            if (srcCounter <= 0)
                srcCounter = 1;

            byte srcByte = 0, dstByte;

            if (Opaque)
            {
                if (Clear)
                {
                    if (Invert)
                        srcByte = 1;

                    for (int i = 0; i < length; i++, dstIndex += dstCounter)
                    {
                        if (Back && dst[dstIndex] != 0)
                            continue;
                        dst[dstIndex] = srcByte;
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                    {
                        srcByte = src[srcIndex];
                        if (Invert)
                            srcByte = (byte)(255 - srcByte);
                        if (Back && dst[dstIndex] != 0)
                            continue;
                        dst[dstIndex] = srcByte;
                    }
                }
            }
            else
            {
                if (Clear)
                {
                    if (Invert)
                        srcByte = 1;
                    for (int i = 0; i < length; i++, dstIndex += dstCounter)
                    {
                        dstByte = dst[dstIndex];
                        if ((Back && dstByte != 0))
                            continue;
                        dst[dstIndex] = srcByte;
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                    {
                        srcByte = src[srcIndex];
                        dstByte = dst[dstIndex];
                        if (srcByte == 0 || (Back && dstByte != 0))
                            continue;
                        if (Invert)
                            srcByte = (byte)(255 - srcByte);
                        dst[dstIndex] = srcByte;
                    }
                }
            }
        }
        #endregion

        #region COPY ARRAY
        public static void Copy<T>(T[] source, ref T[] destination, int length)
        {
            if (length > source.Length)
                length = source.Length;
            if (destination == null || length < destination.Length)
                destination = new T[length];
            Array.Copy(source, destination, length);
        }
        #endregion

        #region RESIZE DATA
        /// <summary>
        /// Resizes Array the image is stored in returning: a truncated image, an image with transparent border to right and bottom or an image that is transparent..
        /// </summary>
        /// <param name="source">Source memory block</param>
        /// <param name="newWidth">Required Width.</param>
        /// <param name="newHeight">Required Height.</param>
        /// <param name="oldWidth">Original Width.</param>
        /// <param name="oldHeight">Original Height.</param>
        /// <param name="clear">If true a trasparrent image is returned in the new size.</param>
        /// <returns>Returns an int array of transparrent colour or the image truncated/padded to fit the new rectangle.</returns>
        public static T[] ResizedData<T>(this T[] source, int newWidth, int newHeight, ref int oldWidth, ref int oldHeight, bool clear = false)
        {
            T[] result = new T[newWidth * newHeight];

            if (clear)
            {
                oldWidth = newWidth;
                oldHeight = newHeight;
                result = new T[newWidth * newHeight];
                return result;
            }

            int copyW = newWidth;
            int copyH = newHeight;
            CopyBlock(source, 0, 0, copyW, copyH, source.Length, oldWidth, oldHeight, result, 0, 0, newWidth, result.Length);

            oldWidth = newWidth;
            oldHeight = newHeight;

            return result;
        }

        /// <summary>
        /// Resizes Array the image is stored in returning: a truncated image, an image with transparent border to right and bottom or an image that is transparent..
        /// </summary>
        /// <param name="source">Source memory block</param>
        /// <param name="newWidth">Required Width.</param>
        /// <param name="newHeight">Required Height.</param>
        /// <param name="oldWidth">Original Width.</param>
        /// <param name="oldHeight">Original Height.</param>
        /// <param name="clear">If true a trasparrent image is returned in the new size.</param>
        /// <returns>Returns an int array of transparrent colour or the image truncated/padded to fit the new rectangle.</returns>
        public static T[] ResizedData<T>(this T[] source, int newWidth, int newHeight, int oldWidth, int oldHeight, bool clear = false)
        {
            T[] result = new T[newWidth * newHeight];
            if (clear)
            {
                result = new T[newWidth * newHeight];
                return result;
            }
            int copyW = newWidth;
            int copyH = newHeight;
            CopyBlock(source, 0, 0, copyW, copyH, source.Length, oldWidth, oldHeight, result, 0, 0, newWidth, result.Length);
            return result;
        }

        /// <summary>
        /// Resizes an usafe Memory Array the image is stored in returning: a truncated image, an image with transparent border to right and bottom or an image that is transparent..
        /// </summary>
        /// <param name="src">Pointer to original image array</param>
        /// <param name="srcLen">Length of int array.</param>
        /// <param name="newWidth">Required Width.</param>
        /// <param name="newHeight">Required Height.</param>
        /// <param name="oldWidth">Original Width.</param>
        /// <param name="oldHeight">Original Height.</param>
        /// <param name="clear">If true a trasparrent image is returned in the new size.</param>
        /// <returns>Returns pointer to int array of transparrent colour or the image truncated/padded to fit the new rectangle.</returns>
        public static unsafe IntPtr ResizedData(IntPtr source, int srcLen, int newWidth, int newHeight, ref int oldWidth, ref int oldHeight, bool clear = false)
        {
            int[] result = new int[newWidth * newHeight];

            if (clear)
            {
                oldWidth = newWidth;
                oldHeight = newHeight;

                goto mks;
            }

            int* src = (int*)source;
            int copyW = newWidth;
            int copyH = newHeight;

            fixed (int* res = result)
            {
                int* dst = res;
                CopyBlock(src, 0, 0, copyW, copyH, srcLen, oldWidth, oldHeight, dst, 0, 0, newWidth, result.Length, 0);
            }
            oldWidth = newWidth;
            oldHeight = newHeight;

        mks:
            fixed (int* p = result)
            {
                return (IntPtr)p;
            }
        }
        #endregion

        #region DUPLICATE
        public static void Mirror<T>(this T[] data, int w, int h, Position mirror)
        {
            int srcIdx, dstIdx;
            var hh = h / 2;
            var wh = w / 2;

            switch (mirror)
            {
                case Position.Bottom:
                default:
                    srcIdx = hh * w;
                    dstIdx = (hh) * w;
                    while (srcIdx < data.Length && dstIdx >= 0)
                    {
                        Array.Copy(data, srcIdx, data, dstIdx, w);
                        srcIdx += w;
                        dstIdx -= w;
                    }
                    break;
                case Position.Top:
                    srcIdx = (hh) * w;
                    dstIdx = (hh) * w + 1;
                    while (dstIdx < data.Length && srcIdx >= 0)
                    {
                        Array.Copy(data, srcIdx, data, dstIdx, w);
                        srcIdx -= w;
                        dstIdx += w;
                    }
                    break;
                case Position.Right:
                    srcIdx = wh;
                    dstIdx = 0;
                    while (dstIdx + wh < data.Length)
                    {
                        Array.Copy(data, srcIdx, data, dstIdx, wh);
                        Array.Reverse(data, dstIdx, wh);
                        srcIdx += w;
                        dstIdx += w;
                    }
                    break;
                case Position.Left:
                    srcIdx = 0;
                    dstIdx = wh + 1;
                    while (dstIdx + wh < data.Length)
                    {
                        Array.Copy(data, srcIdx, data, dstIdx, wh);
                        Array.Reverse(data, dstIdx, wh);
                        srcIdx += w;
                        dstIdx += w;
                    }
                    break;
            }
        }
        #endregion

#if GWS || Window
        #region ROTATE MEMORY
        /// <summary>
        /// Source: https://www.drdobbs.com/architecture-and-design/fast-bitmap-rotation-and-scaling/184416337
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcW"></param>
        /// <param name="srcH"></param>
        /// <param name="rotation"></param>
        /// <param name="antiAliased"></param>
        public static unsafe int[] RotateAndScale(IntPtr source, int srcW, int srcH, Rotation rotation, bool antiAliased, float scale = 1f)
        {
            int* src = (int*)source;
            int srcLen = srcW * srcH;

            int dstW = srcW;
            int dstH = srcH;

            if (scale <= 0) scale = 1;

            int[] result = new int[dstW * dstH];

            if (!rotation && scale == 1)
            {
                fixed (int* dst = result)
                {
                    Copy(src, 0, dst, 0, srcLen);
                }
                return result;
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

            fixed (int* dst = result)
            {
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
                            color = src[index];
                            goto assignColor;
                        }
                        float Dx = x3 - x0;
                        float Dy = y3 - y0;

                        #region BI-LINEAR INTERPOLATION
                        uint rb, ag, c3 = 0, c4 = 0;
                        int n = index + srcW;
                        bool only2 = (n >= srcLen || n + 1 >= srcLen);

                        uint c1 = (uint)src[index++];
                        uint c2 = (uint)src[index];
                        if (!only2)
                        {
                            c3 = (uint)src[n++];
                            c4 = (uint)src[n];
                        }
                        if (c1 == 0 || c1 == Colors.Transparent)
                            c1 = Colors.UWhite;
                        if (c2 == 0 || c2 == Colors.Transparent)
                            c2 = Colors.UWhite;

                        if (c3 == 0 || c3 == Colors.Transparent)
                            c3 = Colors.UWhite;
                        if (c4 == 0 || c4 == Colors.Transparent)
                            c4 = Colors.UWhite;

                        uint alpha = (uint)(Dx * 255);
                        uint invAlpha = 255 - alpha;

                        if (alpha == 255)
                            c1 = c2;

                        else if (alpha != 0)
                        {
                            rb = ((invAlpha * (c1 & Colors.RBMASK)) + (alpha * (c2 & Colors.RBMASK))) >> 8;
                            ag = (invAlpha * ((c1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((c2 & Colors.GMASK) >> 8)));
                            c1 = ((rb & Colors.RBMASK) | (ag & Colors.AGMASK));
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
                            rb = ((invAlpha * (c3 & Colors.RBMASK)) + (alpha * (c4 & Colors.RBMASK))) >> 8;
                            ag = (invAlpha * ((c3 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((c4 & Colors.GMASK) >> 8)));
                            c3 = ((rb & Colors.RBMASK) | (ag & Colors.AGMASK));
                        }

                        alpha = (uint)(Dy * 255);
                        invAlpha = 255 - alpha;

                        if (alpha == 255)
                            color = (int)c3;
                        else if (alpha != 0)
                        {
                            rb = ((invAlpha * (c1 & Colors.RBMASK)) + (alpha * (c3 & Colors.RBMASK))) >> 8;
                            ag = (invAlpha * ((c1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((c3 & Colors.GMASK) >> 8)));
                            color = (int)((rb & Colors.RBMASK) | (ag & Colors.AGMASK));
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

                    #region VRTICAL INCREMENT
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
            }

            return result;
        }
        #endregion

        #region FLIP
        public static unsafe int[] Flip(IntPtr source, int srcW, int srcH, FlipMode flip)
        {
            int[] result = new int[srcW * srcH];
            int i = 0;
            int* src = (int*)source;

            if (flip == FlipMode.Horizontal)
            {
                for (var y = srcH - 1; y >= 0; y--)
                {
                    for (var x = 0; x < srcW; x++)
                    {
                        var srcInd = y * srcW + x;
                        result[i] = src[srcInd];
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
                        result[i] = src[srcInd];
                        i++;
                    }
                }
            }
            return result;
        }
        #endregion
#endif
    }
}
