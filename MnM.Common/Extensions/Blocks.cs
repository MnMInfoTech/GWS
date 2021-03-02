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
        /// Copies data from one  memory block to another: routine of copying must be provided through action delegate.
        /// </summary>
        /// <param name="copyArea">Area to copy.</param>
        /// <param name="srcLen">Specifies the length of the source pointer.</param>
        /// <param name="srcW">Specifies the current width by which the pixel reading should be wrapped to the next line.</param>
        /// <param name="srcH">Specifies the current height of the source block.</param>
        /// <param name="dstX">Specifies the X coordinate where the paste operation should commence.</param>
        /// <param name="dstY">Specifies the Y coordinate from where the paste operation should commence.</param>
        /// <param name="dstW">Specifies the current width by which the pixel writing should be wrapped to the next line.</param>
        /// <param name="dstLen">Specifies the current length of the destination pointer.</param>
        /// <param name="action">BlockCopy action delegate to execute copy operation.</param>
        /// <param name="command">Draw command to control the copy operation.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Perimeter CopyBlock(IBoundable copyArea, int srcLen, int srcW, int srcH,
            int dstX, int dstY, int dstW, int dstLen, BlockCopy action, ulong command)
        {
            copyArea.GetBounds(out int copyX, out int copyY, out int copyW, out int copyH);
            CorrectRegion(ref copyX, ref copyY, ref copyW, ref copyH, srcW, srcH, ref dstX, ref dstY, dstW, dstLen, out int srcIndex, out int dstIndex);
            if (copyW <= 0)
                return Perimeter.Empty;
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
            return new Perimeter(copyArea, dstX, dstY, copyW, i);
        }

        /// <summary>
        /// Copies data from one  memory block to another: routine of copying must be provided through action delegate.
        /// </summary>
        /// <param name="copyArea">Area to copy.</param>
        /// <param name="srcLen">Specifies the length of the source pointer.</param>
        /// <param name="srcW">Specifies the current width by which the pixel reading should be wrapped to the next line.</param>
        /// <param name="srcH">Specifies the current height of the source block.</param>
        /// <param name="dstX">Specifies the X coordinate where the paste operation should commence.</param>
        /// <param name="dstY">Specifies the Y coordinate from where the paste operation should commence.</param>
        /// <param name="dstW">Specifies the current width by which the pixel writing should be wrapped to the next line.</param>
        /// <param name="dstLen">Specifies the current length of the destination pointer.</param>
        /// <param name="action">BlockCopy action delegate to execute copy operation.</param>
        /// <param name="command">Draw command to control the copy operation.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Perimeter CopyBlock(IBoundable copyArea, int srcLen, int srcW, int srcH,
            int dstX, int dstY, int dstW, int dstLen, ConditionalCopy action, int condition, NumCriteria criteria)
        {
            copyArea.GetBounds(out int copyX, out int copyY, out int copyW, out int copyH);
            CorrectRegion(ref copyX, ref copyY, ref copyW, ref copyH, srcW, srcH, ref dstX, ref dstY, dstW, dstLen, out int srcIndex, out int dstIndex);
            if (copyW <= 0)
                return Perimeter.Empty;
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

                action(srcIndex, dstIndex, copyW, copyX, copyY + i, condition, criteria);
                srcIndex += srcW;
                dstIndex += dstW;
                ++i;
            }
            return new Perimeter(copyArea, dstX, dstY, copyW, i);
        }
        #endregion

        #region COPY BLOCK POINTER
        /// <summary>
        /// Copies data from one  memory block to another.
        /// </summary>
        /// <param name="src">Souece block to copy data from.</param>
        /// <param name="copyArea">Area to copy.</param>
        /// <param name="srcLen">Specifies the length of the source pointer.</param>
        /// <param name="srcW">Specifies the current width by which the pixel reading should be wrapped to the next line.</param>
        /// <param name="srcH">Specifies the current height of the source block.</param>
        /// <param name="dst">Destination block to copy data to.</param>
        /// <param name="dstX">Specifies the X coordinate where the paste operation should commence.</param>
        /// <param name="dstY">Specifies the Y coordinate from where the paste operation should commence.</param>
        /// <param name="dstW">Specifies the current width by which the pixel writing should be wrapped to the next line.</param>
        /// <param name="dstLen">Specifies the current length of the destination pointer.</param>
        /// <param name="command">Draw command to control the copy operation.</param>
        /// <param name="srcAlphas">Alpha channel information of the source block.</param>
        /// <returns>Area covered by copy operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Perimeter CopyBlock(int* src, IBoundable copyArea, int srcLen, int srcW, int srcH,
            int* dst, int dstX, int dstY, int dstW, int dstLen, ulong command = 0, byte* srcAlphas = null)
        {
            copyArea.GetBounds(out int copyX, out int copyY, out int copyW, out int copyH);
            CorrectRegion(ref copyX, ref copyY, ref copyW, ref copyH, srcW, srcH, ref dstX, ref dstY, dstW, dstLen, out int srcIndex, out int dstIndex);
            if (copyW <= 0)
                return Perimeter.Empty;
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
            return new Perimeter(copyArea, dstX, dstY, copyW, i);
        }

        /// <summary>
        /// Copies data from one  memory block to another.
        /// </summary>
        /// <param name="src">Souece block to copy data from.</param>
        /// <param name="copyArea">Area to copy.</param>
        /// <param name="srcLen">Specifies the length of the source pointer.</param>
        /// <param name="srcW">Specifies the current width by which the pixel reading should be wrapped to the next line.</param>
        /// <param name="srcH">Specifies the current height of the source block.</param>
        /// <param name="dst">Destination block to copy data to.</param>
        /// <param name="dstX">Specifies the X coordinate where the paste operation should commence.</param>
        /// <param name="dstY">Specifies the Y coordinate from where the paste operation should commence.</param>
        /// <param name="dstW">Specifies the current width by which the pixel writing should be wrapped to the next line.</param>
        /// <param name="dstLen">Specifies the current length of the destination pointer.</param>
        /// <param name="command">Draw command to control the copy operation.</param>
        /// <returns>Area covered by copy operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Perimeter CopyBlock(byte* src, IBoundable copyArea, int srcLen, int srcW, int srcH,
            byte* dst, int dstX, int dstY, int dstW, int dstLen, ulong command = 0)
        {
            copyArea.GetBounds(out int copyX, out int copyY, out int copyW, out int copyH);
            CorrectRegion(ref copyX, ref copyY, ref copyW, ref copyH, srcW, srcH, ref dstX, ref dstY, dstW, dstLen, out int srcIndex, out int dstIndex);
            if (copyW <= 0)
                return Perimeter.Empty;
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
            return new Perimeter(copyArea, dstX, dstY, copyW, i);
        }
        #endregion

        #region COPY VALUE TO BLOCK
        /// <summary>
        /// Copies value to the specified memory block.
        /// </summary>
        /// <param name="value">Value to copy.</param>
        /// <param name="copyArea">Area to copy.</param>
        /// <param name="dst">Destination block to copy data to.</param>
        /// <param name="dstW">The width of the block.</param>
        /// <param name="dstH">The height of the block.</param>
        /// <param name="conditionValue">Value of condition to use to qualify copy operation.</param>
        /// <param name="criteria">Numeric criteria to control copy operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Perimeter CopyBlock(int value, IBoundable copyArea, IntPtr destination, int dstW, int dstH, int conditionValue, NumCriteria criteria = 0)
        {
            var area = Rects.CompitiblePerimeter(dstW, dstH, copyArea);
            int copyW = area.W;
            int copyH = area.H;

            if (copyW <= 0)
                return Perimeter.Empty;
            int dstLen = dstW * dstH;
            int i = 0;
            int dstIndex = area.X + area.Y * dstW;
            int* dst = (int*)destination;
            while (i < copyH)
            {
                if (dstIndex + copyW >= dstLen)
                    copyW -= (dstIndex + copyW - dstLen);

                if (copyW <= 0)
                    break;
                CopyValue(value, dst, dstIndex, copyW, conditionValue, criteria);
                dstIndex += dstW;
                ++i;
            }
            return new Perimeter(copyArea, area.X, area.Y, copyW, i);
        }

        /// <summary>
        /// Copies value to the specified memory block.
        /// </summary>
        /// <param name="value">Value to copy.</param>
        /// <param name="copyArea">Area to copy.</param>
        /// <param name="dst">Destination block to copy data to.</param>
        /// <param name="dstW">The width of the block.</param>
        /// <param name="dstH">The height of the block.</param>
        /// <param name="conditionValue">Value of condition to use to qualify copy operation.</param>
        /// <param name="criteria">Numeric criteria to control copy operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Perimeter CopyBlock(byte value, IBoundable copyArea, IntPtr destination, int dstW, int dstH, byte conditionValue, NumCriteria criteria = 0)
        {
            var area = Rects.CompitiblePerimeter(dstW, dstH, copyArea);
            int copyW = area.W;
            int copyH = area.H;

            if (copyW <= 0)
                return Perimeter.Empty;
            int dstLen = dstW * dstH;
            byte* dst = (byte*)destination;

            int i = 0;
            int dstIndex = area.X + area.Y * dstW;

            while (i < copyH)
            {
                if (dstIndex + copyW >= dstLen)
                    copyW -= (dstIndex + copyW - dstLen);

                if (copyW <= 0)
                    break;
                CopyValue(value, dst, dstIndex, copyW, conditionValue, criteria);
                dstIndex += dstW;
                ++i;
            }
            return new Perimeter(copyArea, area.X, area.Y, copyW, i);
        }
        #endregion

        #region COPY BLOCK ARRAY
        /// <summary>
        /// Copies data from one block to another.
        /// </summary>
        /// <typeparam name="T">Type of elements in the array block.</typeparam>
        /// <param name="src">Souece memory block to copy data from.</param>
        /// <param name="copyArea">Area to copy.</param>
        /// <param name="srcLen">Specifies the length of the source pointer.</param>
        /// <param name="srcW">Specifies the current width by which the pixel reading should be wrapped to the next line.</param>
        /// <param name="srcH">Specifies the current height of the source block.</param>
        /// <param name="dst">Destination memory block to copy data to.</param>
        /// <param name="dstX">Specifies the X coordinate where the paste operation should commence.</param>
        /// <param name="dstY">Specifies the Y coordinate from where the paste operation should commence.</param>
        /// <param name="dstW">Specifies the current width by which the pixel writing should be wrapped to the next line.</param>
        /// <param name="dstLen">Specifies the current length of the destination pointer.</param>
        /// <returns>Area covered by copy operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Perimeter CopyBlock<T>(T[] src, IBoundable copyArea, int srcLen, int srcW, int srcH,
            T[] dst, int dstX, int dstY, int dstW, int dstLen)
        {
            copyArea.GetBounds(out int copyX, out int copyY, out int copyW, out int copyH);
            CorrectRegion(ref copyX, ref copyY, ref copyW, ref copyH, srcW, srcH, ref dstX, ref dstY, dstW, dstLen, out int srcIndex, out int dstIndex);
            if (copyW <= 0)
                return Perimeter.Empty;
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
            return new Perimeter(copyArea, dstX, dstY, copyW, i);
        }
        #endregion

        #region COPY MEMORY
        /// <summary>
        /// Copies source memory block to destination memory block.
        /// </summary>
        /// <param name="src">Source memory block</param>
        /// <param name="srcIndex">Index in source from where copy operation should start</param>
        /// <param name="dst">Destination memory block to copy data to.</param>
        /// <param name="dstIndex">Index in destination where paste operation should start</param>
        /// <param name="length">Length of pixels to be copied</param>
        /// <param name="command">Command to control copy operation.
        /// Applicable flags: Opaque, Backdrop, InvertCanvasColor</param>
        /// <param name="srcAlphas">Alpha channel information of the source block.</param>
        /// <param name="dstCounter">Counter by which destination index moves to next position for copy.</param>
        /// <param name="srcCounter">Counter by which source index moves to next position for copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(int* src, int srcIndex, int* dst, int dstIndex, int length,
            ulong command = Command.Opaque, byte* srcAlphas = null, int dstCounter = 1, int srcCounter = 1)
        {
            if (length == 0)
                return;
            bool Opaque = (command & Command.Opaque) == Command.Opaque;
            bool Back = (command & Command.Backdrop) == Command.Backdrop;
            bool Invert = (command & Command.InvertColor) == Command.InvertColor;
            bool Clear = src == null;
            bool HasAlphas = srcAlphas != null;

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

                    if (Back)
                    {
                        for (int i = 0; i < length; i++, dstIndex += dstCounter)
                        {
                            if (dst[dstIndex] != 0)
                                continue;
                            dst[dstIndex] = srcColor;
                            srcAlphas[dstIndex] = 0;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < length; i++, dstIndex += dstCounter)
                        {
                            dst[dstIndex] = srcColor;
                            srcAlphas[dstIndex] = 0;
                        }
                    }
                }
                else
                {
                    if(Invert && Back)
                    {
                        for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                        {
                            if (dst[dstIndex] != 0)
                                continue;
                            dst[dstIndex] = src[srcIndex] ^ Colors.Inversion;
                        }
                    }
                    if (Back)
                    {
                        for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                        {
                            if (dst[dstIndex] != 0)
                                continue;
                            dst[dstIndex] = src[srcIndex];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                        {
                            dst[dstIndex] = src[srcIndex];
                        }
                    }
                }
            }
            else
            {
                if (!HasAlphas)
                {
                    if (Clear)
                    {
                        if (Invert)
                            srcColor ^= Colors.Inversion;
                        if (Back)
                        {
                            for (int i = 0; i < length; i++, dstIndex += dstCounter)
                            {
                                if (dst[dstIndex] != 0)
                                    continue;
                                dst[dstIndex] = srcColor;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < length; i++, dstIndex += dstCounter)
                            {
                                dst[dstIndex] = srcColor;
                            }
                        }
                    }
                    else
                    {
                        if (Invert && Back)
                        {
                            for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                            {
                                if (src[srcIndex] == 0 || dst[dstIndex] != 0)
                                    continue;
                                dst[dstIndex] = src[srcIndex] ^ Colors.Inversion;
                            }
                        }
                        else if (Back)
                        {
                            for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                            {
                                if (src[srcIndex] == 0 || dst[dstIndex] != 0)
                                    continue;
                                dst[dstIndex] = src[srcIndex];
                            }
                        }
                        else
                        {
                            for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                            {
                                if (src[srcIndex] == 0)
                                    continue;
                                dst[dstIndex] = src[srcIndex];
                            }
                        }
                    }
                }
                else
                {
                    uint C1, C2, RB, AG, invAlpha, alpha;
                    if (Clear)
                    {
                        if (Invert)
                            srcColor ^= Colors.Inversion;

                        if (Back)
                        {
                            for (int i = 0; i < length; i++, dstIndex += dstCounter)
                            {
                                if (dst[dstIndex] != 0)
                                    continue;
                                dst[dstIndex] = srcColor;
                                srcAlphas[srcIndex] = 0;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < length; i++, dstIndex += dstCounter)
                            {
                                dst[dstIndex] = srcColor;
                                srcAlphas[srcIndex] = 0;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                        {
                            srcColor = src[srcIndex];
                            dstColor = dst[dstIndex];
                            alpha = srcAlphas[srcIndex];

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
        /// <param name="dstCounter">Counter by which destination index moves to next position for copy.</param>
        /// <param name="srcCounter">Counter by which source index moves to next position for copy.</param>
        /// <param name="ignoreValue">If provided, positions in destination block where value is as same as this value will not get overwritten.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(byte* src, int srcIndex, byte* dst, int dstIndex, int length,
            ulong command = Command.Opaque, int dstCounter = 1, int srcCounter = 1)
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

            byte srcByte = 0, dstColor;

            if (Opaque)
            {
                if (Clear)
                {
                    if (Invert)
                        srcByte = 1;

                    for (int i = 0; i < length; i++, dstIndex += dstCounter)
                    {
                        dstColor = dst[dstIndex];
                        if (Back && dstColor != 0)
                            continue;
                        dst[dstIndex] = srcByte;
                    }
                }
                else
                {
                    if (Opaque)
                    {
                        for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                        {
                            dst[dstIndex] = src[srcIndex];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                        {
                            srcByte = src[srcIndex];
                            if (Invert)
                                srcByte = (byte)(255 - srcByte);
                            dstColor = dst[dstIndex];
                            if (Back && dstColor != 0)
                                continue;
                            dst[dstIndex] = srcByte;
                        }
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
                        dstColor = dst[dstIndex];
                        if (Back && dstColor != 0)
                            continue;
                        dst[dstIndex] = srcByte;
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                    {
                        srcByte = src[srcIndex];
                        dstColor = dst[dstIndex];
                        if (srcByte == 0 || 
                            (Back && dstColor != 0))
                            continue;
                        if (Invert)
                            srcByte = (byte)(255 - srcByte);
                        dst[dstIndex] = srcByte;
                    }
                }
            }
        }
        #endregion

        #region COPY MEMORY CONDITIONAL
        /// <summary>
        /// Copies source memory block to destination memory block satisfying given condition.
        /// </summary>
        /// <param name="src">Source memory block</param>
        /// <param name="srcIndex">Index in source from where copy operation should start</param>
        /// <param name="dst">Destination memory block to copy data to.</param>
        /// <param name="dstIndex">Index in destination where paste operation should start</param>
        /// <param name="length">Length of pixels to be copied. Applicable flags: Opaque, Backdrop, InvertCanvasColor</param>
        /// <param name="conditionValue">Value of condition to use to qualify copy.</param>
        /// <param name="criteria">Numeric criteria to control copy operation.</param>
        /// <param name="srcAlphas">Alpha channel information of the source block.</param>
        /// <param name="invert">If true, value will be inverted before copying it to the destination.</param>
        /// <param name="useDstIndexForAlphas">If true, indices of destination block will be used to read source alpha information.</param>
        /// <param name="dstCounter">Counter by which destination index moves to next position for copy.</param>
        /// <param name="srcCounter">Counter by which source index moves to next position for copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(int* src, int srcIndex, int* dst, int dstIndex, int length, int conditionValue, NumCriteria criteria, 
           byte* srcAlphas = null, bool invert = false, int dstCounter = 1, int srcCounter = 1)
        {
            if (length == 0)
                return;
            bool Clear = src == null;

            if (dstCounter <= 0)
                dstCounter = 1;
            if (srcCounter <= 0)
                srcCounter = 1;

            int srcColor = 0, dstColor;
            bool NoAlphas = srcAlphas == null;
            
            if (Clear)
            {
                if (invert)
                    srcColor ^= Colors.Inversion;
                for (int i = 0; i < length; i++, dstIndex += dstCounter)
                {
                    dstColor = dst[dstIndex];

                    switch (criteria)
                    {
                        case NumCriteria.Equal:
                        default:
                            if (dstColor != conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotEqual:
                            if (dstColor == conditionValue)
                                continue;
                            break;
                        case NumCriteria.GreaterThan:
                            if (dstColor <= conditionValue)
                                continue;
                            break;
                        case NumCriteria.LessThan:
                            if (dstColor >= conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotGreaterThan:
                            if (dstColor > conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotLessThan:
                            if (dstColor < conditionValue)
                                continue;
                            break;
                    }

                    dst[dstIndex] = srcColor;
                }
            }
            else if(NoAlphas)
            {
                for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                {
                    srcColor = src[srcIndex];
                    if (invert)
                        srcColor ^= Colors.Inversion;

                    dstColor = dst[dstIndex];

                    switch (criteria)
                    {
                        case NumCriteria.Equal:
                        default:
                            if (dstColor != conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotEqual:
                            if (dstColor == conditionValue)
                                continue;
                            break;
                        case NumCriteria.GreaterThan:
                            if (dstColor <= conditionValue)
                                continue;
                            break;
                        case NumCriteria.LessThan:
                            if (dstColor >= conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotGreaterThan:
                            if (dstColor > conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotLessThan:
                            if (dstColor < conditionValue)
                                continue;
                            break;
                    }

                    dst[dstIndex] = srcColor;
                }
            }
            else
            {
                uint C1, C2, RB, AG, invAlpha, alpha;
                bool Back = conditionValue == 0 && criteria == NumCriteria.Equal;
                for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                {
                    srcColor = src[srcIndex];
                    dstColor = dst[dstIndex];
                    alpha = srcAlphas[srcIndex];

                    switch (criteria)
                    {
                        case NumCriteria.Equal:
                        default:
                            if (dstColor != conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotEqual:
                            if (dstColor == conditionValue)
                                continue;
                            break;
                        case NumCriteria.GreaterThan:
                            if (dstColor <= conditionValue)
                                continue;
                            break;
                        case NumCriteria.LessThan:
                            if (dstColor >= conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotGreaterThan:
                            if (dstColor > conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotLessThan:
                            if (dstColor < conditionValue)
                                continue;
                            break;
                    }

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
                    if (invert)
                        srcColor ^= Colors.Inversion;
                    dst[dstIndex] = srcColor;
                }
            }
        }

        /// <summary>
        /// Copies source memory block to destination memory block satisfying given condition.
        /// </summary>
        /// <param name="src">Source memory block</param>
        /// <param name="srcIndex">Index in source from where copy operation should start</param>
        /// <param name="dst">Destination memory block to copy data to.</param>
        /// <param name="dstIndex">Index in destination where paste operation should start</param>
        /// <param name="length">Length of pixels to be copied. Applicable flags: Opaque, Backdrop, InvertCanvasColor</param>
        /// <param name="conditionValue">Value of condition to use to qualify copy.</param>
        /// <param name="criteria">Numeric criteria to control copy operation.</param>
        /// <param name="invert">If true, value will be inverted before copying it to the destination.</param>
        /// <param name="dstCounter">Counter by which destination index moves to next position for copy.</param>
        /// <param name="srcCounter">Counter by which source index moves to next position for copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(byte* src, int srcIndex, byte* dst, int dstIndex, int length, byte conditionValue, NumCriteria criteria,
           bool invert = false, int dstCounter = 1, int srcCounter = 1)
        {
            if (length == 0)
                return;
            bool Clear = src == null;

            if (dstCounter <= 0)
                dstCounter = 1;
            if (srcCounter <= 0)
                srcCounter = 1;

            byte srcByte = 0, dstByte;
            if (Clear)
            {
                if (invert)
                    srcByte = 255;

                for (int i = 0; i < length; i++, dstIndex += dstCounter)
                {
                    dstByte = dst[dstIndex];

                    switch (criteria)
                    {
                        case NumCriteria.Equal:
                        default:
                            if (dstByte != conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotEqual:
                            if (dstByte == conditionValue)
                                continue;
                            break;
                        case NumCriteria.GreaterThan:
                            if (dstByte <= conditionValue)
                                continue;
                            break;
                        case NumCriteria.LessThan:
                            if (dstByte >= conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotGreaterThan:
                            if (dstByte > conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotLessThan:
                            if (dstByte < conditionValue)
                                continue;
                            break;
                    }

                    dst[dstIndex] = srcByte;
                }
            }
            else
            {
                for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                {
                    srcByte = src[srcIndex];
                    if (invert)
                        srcByte = (byte)(255 - srcByte);

                    dstByte = dst[dstIndex];

                    switch (criteria)
                    {
                        case NumCriteria.Equal:
                        default:
                            if (dstByte != conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotEqual:
                            if (dstByte == conditionValue)
                                continue;
                            break;
                        case NumCriteria.GreaterThan:
                            if (dstByte <= conditionValue)
                                continue;
                            break;
                        case NumCriteria.LessThan:
                            if (dstByte >= conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotGreaterThan:
                            if (dstByte > conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotLessThan:
                            if (dstByte < conditionValue)
                                continue;
                            break;
                    }

                    dst[dstIndex] = srcByte;
                }
            }
        }
        #endregion

        #region COPY VALUE
        /// <summary>
        /// Copies source memory block to destination memory block satisfying given condition.
        /// </summary>
        /// <param name="value">Value to be copied to across the destination block.</param>
        /// <param name="dst">Destination memory block to copy data to.</param>
        /// <param name="dstIndex">Index in destination where paste operation should start</param>
        /// <param name="length">Length of pixels to be copied. Applicable flags: Opaque, Backdrop, InvertCanvasColor</param>
        /// <param name="conditionValue">Value of condition to use to qualify copy.</param>
        /// <param name="criteria">Numeric criteria to control copy operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void CopyValue(int value, int* dst, int dstIndex, int length, int conditionValue, NumCriteria criteria = 0)
        {
            if (length == 0)
                return;
            int srcColor = 0;
            srcColor = value;
            int last = dstIndex + length;
            if(criteria == 0)
            {
                for (int i = dstIndex; i < last; i++)
                    dst[i] = value;
                return;
            }
            int dstColor;
            for (int i = dstIndex; i < last; i++)
            {
                dstColor = dst[i];

                switch (criteria)
                {
                    case NumCriteria.Equal:
                    default:
                        if (dstColor != conditionValue)
                            continue;
                        break;
                    case NumCriteria.NotEqual:
                        if (dstColor == conditionValue)
                            continue;
                        break;
                    case NumCriteria.GreaterThan:
                        if (dstColor <= conditionValue)
                            continue;
                        break;
                    case NumCriteria.LessThan:
                        if (dstColor >= conditionValue)
                            continue;
                        break;
                    case NumCriteria.NotGreaterThan:
                        if (dstColor > conditionValue)
                            continue;
                        break;
                    case NumCriteria.NotLessThan:
                        if (dstColor < conditionValue)
                            continue;
                        break;
                }
                dst[i] = value;
            }
        }

        /// <summary>
        /// Copies source memory block to destination memory block satisfying given condition.
        /// </summary>
        /// <param name="value">Value to be copied to across the destination block.</param>
        /// <param name="dst">Destination memory block to copy data to.</param>
        /// <param name="dstIndex">Index in destination where paste operation should start</param>
        /// <param name="length">Length of pixels to be copied. Applicable flags: Opaque, Backdrop, InvertCanvasColor</param>
        /// <param name="conditionValue">Value of condition to use to qualify copy.</param>
        /// <param name="criteria">Numeric criteria to control copy operation.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void CopyValue(byte value, byte* dst, int dstIndex, int length, byte conditionValue, NumCriteria criteria = 0)
        {
            if (length == 0)
                return;
            int srcColor = 0;
            srcColor = value;
            int last = dstIndex + length;
            if (criteria == 0)
            {
                for (int i = dstIndex; i < last; i++)
                    dst[i] = value;
                return;
            }

            byte dstColor;
            for (int i = dstIndex; i < last; i++)
            {
                dstColor = dst[i];

                switch (criteria)
                {
                    case NumCriteria.Equal:
                    default:
                        if (dstColor != conditionValue)
                            continue;
                        break;
                    case NumCriteria.NotEqual:
                        if (dstColor == conditionValue)
                            continue;
                        break;
                    case NumCriteria.GreaterThan:
                        if (dstColor <= conditionValue)
                            continue;
                        break;
                    case NumCriteria.LessThan:
                        if (dstColor >= conditionValue)
                            continue;
                        break;
                    case NumCriteria.NotGreaterThan:
                        if (dstColor > conditionValue)
                            continue;
                        break;
                    case NumCriteria.NotLessThan:
                        if (dstColor < conditionValue)
                            continue;
                        break;
                }

                dst[i] = value;
            }
        }
        #endregion

        #region CLEAR MULTIPLE ARRAYS OF SAME SIZE
        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcIndex"></param>
        /// <param name="destinations">Array of byte arrays of same size.</param>
        /// <param name="dstIndex"></param>
        /// <param name="length"></param>
        /// <param name="command"></param>
        /// <param name="dstCounter"></param>
        /// <param name="srcCounter"></param>
        /// <param name="ignoreValue"></param>
        public static unsafe void Copy(byte* src, int srcIndex, byte*[] destinations, int dstIndex, int length,
            ulong command = Command.Opaque, int dstCounter = 1, int srcCounter = 1, int? ignoreValue = null)
        {
            if(destinations == null || destinations.Length == 0 || length == 0)
                return;
            int count = destinations.Length;
            bool Opaque = (command & Command.Opaque) == Command.Opaque;
            bool Back = (command & Command.Backdrop) == Command.Backdrop;
            bool Invert = (command & Command.InvertColor) == Command.InvertColor;
            bool Clear = src == null;
            var exclude = ignoreValue ?? 0;
            bool ignore = ignoreValue != null;

            if (dstCounter <= 0)
                dstCounter = 1;
            if (srcCounter <= 0)
                srcCounter = 1;
            byte srcByte = 0, dstColor;

            if (Opaque)
            {
                if (Clear)
                {
                    if (Invert)
                        srcByte = 1;

                    for (int i = 0; i < length; i++, dstIndex += dstCounter)
                    {
                        foreach (var dst in destinations)
                        {
                            dstColor = dst[dstIndex];
                            if ((Back && dstColor != 0) ||
                                (ignore && dstColor == exclude))
                                continue;
                            dst[dstIndex] = srcByte;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                    {
                        srcByte = src[srcIndex];
                        if (Invert)
                            srcByte = (byte)(255 - srcByte);
                        foreach (var dst in destinations)
                        {
                            dstColor = dst[dstIndex];
                            if ((Back && dstColor != 0) ||
                                (ignore && dstColor == exclude))
                                continue;
                            dst[dstIndex] = srcByte;
                        }
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
                        foreach (var dst in destinations)
                        {
                            dstColor = dst[dstIndex];
                            if ((Back && dstColor != 0) ||
                                (ignore && dstColor == exclude))
                                continue;
                            dst[dstIndex] = srcByte;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                    {
                        srcByte = src[srcIndex];
                        foreach (var dst in destinations)
                        {
                            dstColor = dst[dstIndex];
                            if (srcByte == 0 ||
                                (Back && dstColor != 0) ||
                                (ignore && dstColor == exclude))
                                continue;
                            if (Invert)
                                srcByte = (byte)(255 - srcByte);
                            dst[dstIndex] = srcByte;
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
        /// <param name="destinations">Array of Destination memory blocks to copy data to.</param>
        /// <param name="dstIndex">Index in destination where paste operation should start</param>
        /// <param name="length">Length of pixels to be copied</param>
        /// <param name="command">Command to control copy operation.
        /// Applicable flags: Opaque, Backdrop, InvertCanvasColor</param>
        /// <param name="srcAlphas">Alpha channel information of the source block.</param>
        /// <param name="useDstIndexForAlphas">If true, indices of destination block will be used to read source alpha information.</param>
        /// <param name="dstCounter">Counter by which destination index moves to next position for copy.</param>
        /// <param name="srcCounter">Counter by which source index moves to next position for copy.</param>
        /// <param name="ignoreValue">If provided, positions in destination block where value is as same as this value will not get overwritten.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(int* src, int srcIndex, int*[] destinations, int dstIndex, int length,
            ulong command = Command.Opaque, byte* srcAlphas = null,
            bool useDstIndexForAlphas = false, int dstCounter = 1, int srcCounter = 1, int? ignoreValue = null)
        {
            if (length == 0 || destinations == null || destinations.Length == 0)
                return;
            bool Opaque = (command & Command.Opaque) == Command.Opaque;
            bool Back = (command & Command.Backdrop) == Command.Backdrop;
            bool Invert = (command & Command.InvertColor) == Command.InvertColor;
            bool Clear = src == null;
            var exclude = ignoreValue ?? 0;
            bool ignore = ignoreValue != null;

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
                        foreach (var dst in destinations)
                        {
                            dstColor = dst[dstIndex];

                            if ((Back && dst[dstIndex] != 0) ||
                                (ignore && dstColor == exclude))
                                continue;
                            dst[dstIndex] = srcColor;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                    {
                        srcColor = src[srcIndex];
                        if (Invert)
                            srcColor ^= Colors.Inversion;

                        foreach (var dst in destinations)
                        {
                            dstColor = dst[dstIndex];

                            if ((Back && dstColor != 0) ||
                                (ignore && dstColor == exclude))
                                continue;
                            dst[dstIndex] = srcColor;
                        }
                    }
                }
            }
            else
            {
                bool HasAlphas = srcAlphas != null;
                if (!HasAlphas)
                {
                    if (Clear)
                    {
                        if (Invert)
                            srcColor ^= Colors.Inversion;
                        for (int i = 0; i < length; i++, dstIndex += dstCounter)
                        {
                            foreach (var dst in destinations)
                            {
                                dstColor = dst[dstIndex];
                                if ((Back && dstColor != 0) ||
                                (ignore && dstColor == exclude))
                                    continue;
                                dst[dstIndex] = srcColor;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                        {
                            srcColor = src[srcIndex];
                            foreach (var dst in destinations)
                            {
                                dstColor = dst[dstIndex];
                                if (srcColor == 0 || (Back && dstColor != 0) ||
                                (ignore && dstColor == exclude))
                                    continue;
                                if (Invert)
                                    srcColor ^= Colors.Inversion;
                                dst[dstIndex] = srcColor;
                            }
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
                            foreach (var dst in destinations)
                            {
                                dstColor = dst[dstIndex];
                                alphaIdx = useDstIndexForAlphas ? dstIndex : srcIndex;

                                if ((Back && dstColor != 0) ||
                                (ignore && dstColor == exclude))
                                    continue;
                                dst[dstIndex] = srcColor;
                                srcAlphas[alphaIdx] = 0;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                        {
                            srcColor = src[srcIndex];
                            foreach (var dst in destinations)
                            {
                                dstColor = dst[dstIndex];
                                alphaIdx = useDstIndexForAlphas ? dstIndex : srcIndex;
                                alpha = srcAlphas[alphaIdx];

                                if (srcColor == 0 || (Back && dstColor != 0 && alpha == 0) ||
                                (ignore && dstColor == exclude))
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
            CopyBlock(source, new Perimeter(0, 0, copyW, copyH), source.Length, oldWidth, oldHeight, result, 0, 0, newWidth, result.Length);

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
            CopyBlock(source, new Perimeter(0, 0, oldWidth, oldHeight), oldWidth * oldHeight, oldWidth, oldHeight, result, 0, 0, newWidth, result.Length);
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
                CopyBlock(src, new Perimeter(0, 0, copyW, copyH), srcLen, oldWidth, oldHeight, dst, 0, 0, newWidth, result.Length, 0);
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
    }
}
