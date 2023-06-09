/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public static unsafe partial class Blocks
    {
        #region CONSTS
        const uint AMASK = Colours.AMASK;
        const uint RBMASK = Colours.RBMASK;
        const uint GMASK = Colours.GMASK;
        const uint AGMASK = AMASK | GMASK;
        const uint ONEALPHA = Colours.ONEALPHA;
        const int Inversion = Colours.Inversion;
        const uint AlphaRemoval = Colours.AlphaRemoval;
        const byte ONE = 1;
        const byte ZERO = 0;
        const byte TWO = 2;
        const byte MAX = 255;
        const int NOCOLOR = 0;
        const int Transparent = Colours.Transparent;
        const uint White = Colours.UWhite;
        const int Big = Vectors.Big;
        const int BigExp = Vectors.BigExp;
        #endregion

        #region CORRECT REGION
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CorrectRegion(ref int x, ref int y, ref int w, ref int h, int srcW, int srcH,
            ref int dstX, ref int dstY, int dstW, int dstLen, out int srcIndex, out int dstIndex, bool positiveLocation = true)
        {
            int dstH = dstLen / dstW;
            srcIndex = dstIndex = 0;

            #region CO-ORDINATES CORRECTION 
            if (x < 0)
            {
                w += x;
                dstX -= x;
                x = 0;
            }
            if (y < 0)
            {
                h += y;
                dstY -= y;
                y = 0;
            }
            if (x == int.MaxValue)
                x = 0;
            if (y == int.MaxValue)
                y = 0;

            if (dstX == int.MaxValue)
                dstX = 0;
            if (dstY == int.MaxValue)
                dstY = 0;

            var effectiveW = srcW;
            if (effectiveW > dstW)
                effectiveW = dstW;
            var effectiveH = srcH;
            if (effectiveH > dstH)
                effectiveH = dstH;

            if (w > effectiveW)
                w = effectiveW;

            if (h > effectiveH)
                h = effectiveH;

            if (x + w > srcW)
                w = (srcW - x);
            if (y + h > srcH)
                h = (srcH - y);

            if (positiveLocation)
            {
                if (dstX < 0)
                {
                    x -= dstX;
                    w += dstX;
                    dstX = 0;
                }
                if (dstY < 0)
                {
                    y -= dstY;
                    h += dstY;
                    dstY = 0;
                }
            }

            if (dstX + w > dstW)
                w = (dstW - dstX);
            if (dstY + h > dstH)
                h = (dstH - dstY);

            if (w < 0 || h < 0)
                return false;
            #endregion

            srcIndex = x + y * srcW;
            dstIndex = dstX + dstY * dstW;
            return true;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CorrectRegion(int srcW, int srcH, IBounds copyArea, IBounds clip,
        ref int dstX, ref int dstY, int dstW, int dstH, out int x, out int y, out int w, out int h, 
        out int srcIndex, out int dstIndex)
        {
            srcIndex = dstIndex = 0;

            x = 0;
            y = 0;
            w = srcW;
            h = srcH;

            if (copyArea != null && copyArea.Valid)
            {
                copyArea.GetBounds(out x, out y, out w, out h);

                if (x < 0)
                {
                    w += x;
                    dstX -= x;
                    x = 0;
                }
                if (y < 0)
                {
                    h += y;
                    dstY -= y;
                    y = 0;
                }
            }

            if (clip != null && clip.Valid)
            {
                clip.GetBounds(out int clipX, out int clipY, out int clipW, out int clipH);
                var clipR = clipX + clipW;
                var clipB = clipY + clipH;

                if (
                    dstY > clipB ||
                    dstX > clipR && dstY > clipB
                   )
                    return false;
                var oldW = w;
                var dstEX = dstX + w;
                if (dstX < clipX)
                    dstX = clipX;
                if (dstEX > clipR)
                    dstEX = clipR;
                w = dstEX - dstX;
                if (oldW - w >= 0)
                    x += (oldW - w);
                if (w < 0)
                    return false;

                var oldH = h;
                var dstEY = dstY + h;
                if (dstY < clipY)
                    dstY = clipY;
                if (dstEY > clipB)
                    dstEY = clipB;
                h = dstEY - dstY;
                if (oldH - h >= 0)
                    y += (oldH - h);
                if (h < 0)
                    return false;
            }

            var effectiveW = srcW;
            if (effectiveW > dstW)
                effectiveW = dstW;
            var effectiveH = srcH;
            if (effectiveH > dstH)
                effectiveH = dstH;

            if (w > effectiveW)
                w = effectiveW;

            if (h > effectiveH)
                h = effectiveH;

            if (x + w > srcW)
                w = (srcW - x);
            if (y + h > srcH)
                h = (srcH - y);

            if (dstX + w > dstW)
                w = (dstW - dstX);
            if (dstY + h > dstH)
                h = (dstH - dstY);

            if (w < 0 || h < 0)
                return false;

            srcIndex = x + y * srcW;
            dstIndex = dstX + dstY * dstW;
            return true;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CorrectLength(int viewW, int viewH, bool horizontal, ref int dstX, ref int dstY, ref int copyLength)
        {
            if (horizontal)
            {
                if (dstY < 0 || dstY >= viewH)
                    return false;
                if (dstX < 0)
                {
                    copyLength += dstX;
                    dstX = 0;
                }
                if (copyLength < 0)
                    return false;
                if (dstX + copyLength > viewW)
                    copyLength = viewW - dstX;
            }
            else
            {
                if (dstX < 0 || dstX >= viewW)
                    return false;

                if (dstY < 0)
                {
                    copyLength += dstY;
                    dstY = 0;
                }
                if (copyLength < 0)
                    return false;

                if (dstY + copyLength > viewH)
                    copyLength = viewH - dstY;
            }
            if (copyLength == 0)
                copyLength = 1;
            return true;
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IBounds CopyBlock(IBounds copyArea, int srcLen, int srcW, int srcH,
            int dstX, int dstY, int dstW, int dstLen, BlockCopy action, CopyCommand command)
        {
            copyArea.GetBounds(out int copyX, out int copyY, out int w, out int h);
            CorrectRegion(ref copyX, ref copyY, ref w, ref h, srcW, srcH, ref dstX, ref dstY, dstW, dstLen, out int srcIndex, out int dstIndex);
            if (w <= 0)
                return Rectangle.Empty;
            int i = 0;
            int copyW;
            int copyH = h;

            while (i < copyH)
            {
                copyW = w;
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
            return new Rectangle(dstX, dstY, w, i);
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IBounds CopyBlock(IBounds copyArea, int srcLen, int srcW, int srcH,
            int dstX, int dstY, int dstW, int dstLen, ConditionalCopy action, int condition, NumCriteria criteria)
        {
            copyArea.GetBounds(out int copyX, out int copyY, out int w, out int h);
            CorrectRegion(ref copyX, ref copyY, ref w, ref h, srcW, srcH, ref dstX, ref dstY, dstW, dstLen, out int srcIndex, out int dstIndex);
            if (w <= 0)
                return Rectangle.Empty;
            int i = 0;
            int copyW;
            int copyH = h;

            while (i < copyH)
            {
                copyW = w;
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
            return new Rectangle(dstX, dstY, w, i);
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IBounds CopyBlock(int* src, IBounds copyArea, int srcLen, int srcW, int srcH,
            int* dst, int dstX, int dstY, int dstW, int dstLen, CopyCommand command = 0)
        {
            copyArea.GetBounds(out int copyX, out int copyY, out int w, out int h);
            CorrectRegion(ref copyX, ref copyY, ref w, ref h, srcW, srcH, ref dstX, ref dstY, dstW, dstLen, out int srcIndex, out int dstIndex);
            if (w <= 0)
                return Rectangle.Empty;
            int i = 0;
            int copyW;
            int copyH = h;

            byte transparency = copyArea is ITransparency ? ((ITransparency)copyArea).Transparency : Application.NO;
            while (i <= copyH)
            {
                copyW = w;
                if (srcIndex + copyW >= srcLen)
                    copyW -= (srcIndex + copyW - srcLen);
                if (copyW <= 0)
                    break;

                if (dstIndex + copyW >= dstLen)
                    copyW -= (dstIndex + copyW - dstLen);

                if (copyW <= 0)
                    break;
                Copy(src, srcIndex, dst, dstIndex, copyW, command, 1, 1, transparency);
                srcIndex += srcW;
                dstIndex += dstW;
                ++i;
            }
            return new Rectangle(dstX, dstY, w, i);
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IBounds CopyBlock(byte* src, IBounds copyArea, int srcLen, int srcW, int srcH,
            byte* dst, int dstX, int dstY, int dstW, int dstLen, CopyCommand command = 0)
        {
            copyArea.GetBounds(out int copyX, out int copyY, out int w, out int h);
            CorrectRegion(ref copyX, ref copyY, ref w, ref h, srcW, srcH, ref dstX, ref dstY, dstW, dstLen, out int srcIndex, out int dstIndex);
            if (w <= 0)
                return Rectangle.Empty;
            int i = 0;
            int copyW;
            int copyH = h;

            while (i < copyH)
            {
                copyW = w;
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
            return new Rectangle(dstX, dstY, w, i);
        }

        /// <summary>
        /// Copies data from one  memory block to another.
        /// </summary>
        /// <param name="source">Souece block to copy data from.</param>
        /// <param name="copyArea">Area to copy.</param>
        /// <param name="srcLen">Specifies the length of the source pointer.</param>
        /// <param name="srcW">Specifies the current width by which the pixel reading should be wrapped to the next line.</param>
        /// <param name="srcH">Specifies the current height of the source block.</param>
        /// <param name="destination">Destination block to copy data to.</param>
        /// <param name="dstX">Specifies the X coordinate where the paste operation should commence.</param>
        /// <param name="dstY">Specifies the Y coordinate from where the paste operation should commence.</param>
        /// <param name="dstW">Specifies the current width by which the pixel writing should be wrapped to the next line.</param>
        /// <param name="dstLen">Specifies the current length of the destination pointer.</param>
        /// <param name="command">Draw command to control the copy operation.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IBounds CopyBlock(IntPtr source, IBounds copyArea, int srcLen, int srcW, int srcH,
            IntPtr destination, int dstX, int dstY, int dstW, int dstLen, CopyCommand command = 0)
        {
            int* src = (int*)source;
            int* dst = (int*)destination;

            copyArea.GetBounds(out int copyX, out int copyY, out int w, out int h);
            CorrectRegion(ref copyX, ref copyY, ref w, ref h, srcW, srcH, ref dstX, ref dstY, dstW, dstLen, out int srcIndex, out int dstIndex);
            if (w <= 0)
                return  Rectangle.Empty;
            int i = 0;
            int copyW;
            int copyH = h;

            byte transparency = copyArea is ITransparency ? 
                ((ITransparency)copyArea).Transparency : Application.NO;
            while (i <= copyH)
            {
                copyW = w;
                if (srcIndex + copyW >= srcLen)
                    copyW -= (srcIndex + copyW - srcLen);
                if (copyW <= 0)
                    break;

                if (dstIndex + copyW >= dstLen)
                    copyW -= (dstIndex + copyW - dstLen);

                if (copyW <= 0)
                    break;
                Copy(src, srcIndex, dst, dstIndex, copyW, command, 1, 1, transparency);
                srcIndex += srcW;
                dstIndex += dstW;
                ++i;
            }
            return new Rectangle(dstX, dstY, w, i);
        }
        #endregion

        #region COPY VALUE TO BLOCK
        /// <summary>
        /// Copies value to the specified memory block.
        /// </summary>
        /// <param name="value">Value to copy.</param>
        /// <param name="copyArea">Area to copy.</param>
        /// <param name="dstW">The width of the block.</param>
        /// <param name="dstH">The height of the block.</param>
        /// <param name="conditionValue">Value of condition to use to qualify copy operation.</param>
        /// <param name="criteria">Numeric criteria to control copy operation.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IBounds CopyBlock(int value, IBounds copyArea, IntPtr destination, 
            int dstW, int dstH, int conditionValue = 0, NumCriteria criteria = 0)
        {
            int copyX, copyY;
            copyArea.GetBounds(out copyX, out copyY, out int w, out int h);
            Rectangles.CompitibleRc(dstW, dstH, ref copyX, ref copyY, ref w, ref h);

            if (w <= 0)
                return Rectangle.Empty;

            int dstLen = dstW * dstH;
            int i = 0;
            int copyW = w;
            int copyH = h;
            int dstIndex = copyX + copyY * dstW;
            int* dst = (int*)destination;
            while (i < copyH)
            {
                copyW = w;
                if (dstIndex + copyW >= dstLen)
                    copyW -= (dstIndex + copyW - dstLen);

                if (copyW <= 0)
                    break;
                CopyValue(value, dst, dstIndex, copyW, conditionValue, criteria);
                dstIndex += dstW;
                ++i;
            }
            return new Rectangle(copyX, copyY, copyW, i);
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
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IBounds CopyBlock(byte value, IBounds copyArea, IntPtr destination, 
            int dstW, int dstH, byte conditionValue = 0, NumCriteria criteria = 0)
        {
            int copyX, copyY;
            copyArea.GetBounds(out copyX, out copyY, out int w, out int h);
            Rectangles.CompitibleRc(dstW, dstH, ref copyX, ref copyY, ref w, ref h);

            if (w <= 0)
                return Rectangle.Empty;
            int dstLen = dstW * dstH;
            byte* dst = (byte*)destination;

            int i = 0;
            int copyW;
            int copyH = h;

            int dstIndex = copyX + copyY * dstW;

            while (i < copyH)
            {
                copyW = w;
                if (dstIndex + copyW >= dstLen)
                    copyW -= (dstIndex + copyW - dstLen);

                if (copyW <= 0)
                    break;
                CopyValue(value, dst, dstIndex, copyW, conditionValue, criteria);
                dstIndex += dstW;
                ++i;
            }
            return new Rectangle(copyX, copyY, w, i);
        }
        #endregion

        #region COPY BLOCK ARRAY
        /// <summary>
        /// Copies data from one block to another.
        /// </summary>
        /// <typeparam name="T">Type of elements in the array block.</typeparam>
        /// <param name="src">Souece memory block to copy data from.</param>
        /// <param name="copyArea">Area to copy.</param>
        /// <param name="srcW">Specifies the current width by which the pixel reading should be wrapped to the next line.</param>
        /// <param name="srcH">Specifies the current height of the source block.</param>
        /// <param name="dst">Destination memory block to copy data to.</param>
        /// <param name="dstX">Specifies the X coordinate where the paste operation should commence.</param>
        /// <param name="dstY">Specifies the Y coordinate from where the paste operation should commence.</param>
        /// <param name="dstW">Specifies the current width by which the pixel writing should be wrapped to the next line.</param>
        /// <param name="dstLen">Specifies the current length of the destination pointer.</param>
        /// <returns>Area covered by copy operation.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IBounds CopyBlock<T>(T[] src, IBounds copyArea, 
            int srcW, int srcH, T[] dst, int dstX, int dstY, int dstW)
        {
            copyArea.GetBounds(out int copyX, out int copyY, out int w, out int h);
            CorrectRegion(ref copyX, ref copyY, ref w, ref h, srcW, srcH, ref dstX, ref dstY, 
                dstW, dst.Length, out int srcIndex, out int dstIndex);
            if (w <= 0)
                return Rectangle.Empty;
            int copyW;
            int copyH = h;
            int i = 0;
            int srcLen = src.Length;
            int dstLen = dst.Length;
            if (src != null)
            {
                while (i < copyH)
                {
                    copyW = w;
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
            }
            else
            {
                var arr = new T[w];
                while (i < copyH)
                {
                    copyW = w;
                    if (srcIndex + copyW >= srcLen)
                        copyW -= (srcIndex + copyW - srcLen);
                    if (copyW <= 0)
                        break;

                    if (dstIndex + copyW >= dstLen)
                        copyW -= (dstIndex + copyW - dstLen);

                    if (copyW <= 0)
                        break;
                    Array.Copy(arr, 0, dst, dstIndex, copyW);
                    srcIndex += srcW;
                    dstIndex += dstW;
                    ++i;
                }
            }
            return new Rectangle(dstX, dstY, w, i);
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
        /// Applicable flags: Opaque, Backdrop, InvertCanvasColour</param>
        /// <param name="srcAlphas">Alpha channel information of the source block.</param>
        /// <param name="dstCounter">Counter by which destination index moves to next position for copy.</param>
        /// <param name="srcCounter">Counter by which source index moves to next position for copy.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(int* src, int srcIndex, int* dst, int dstIndex, int length,
            CopyCommand command = 0, int dstCounter = 1, int srcCounter = 1, byte transparency = 0)
        {
            if (length == 0)
                return;
            bool Back = (command & CopyCommand.Backdrop) == CopyCommand.Backdrop;
            bool Clear = src == null;

            if (dstCounter <= 0)
                dstCounter = 1;
            if (srcCounter <= 0)
                srcCounter = 1;

            int srcColour = 0, dstColour;
            if (Clear)
            {
                if (Back)
                {
                    for (int i = 0; i < length; i++, dstIndex += dstCounter)
                    {
                        if (dst[dstIndex] != NOCOLOR)
                            continue;
                        dst[dstIndex] = srcColour;
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++, dstIndex += dstCounter)
                    {
                        dst[dstIndex] = srcColour;
                    }
                }
                return;
            }

            bool hasTransparency = transparency != ZERO;
            bool transparent;
            byte opacity = (byte)(MAX - transparency);
            hasTransparency = opacity > ONE && opacity != MAX;
            uint C1, C2, RB, AG, invAlpha, alpha;
            bool RGBOnly = (command & CopyCommand.CopyRGBOnly) == CopyCommand.CopyRGBOnly;
            bool SwitchRB = (command & CopyCommand.SwapRedBlueChannel) == CopyCommand.SwapRedBlueChannel;
            bool Opaque = (command & CopyCommand.CopyOpaque) == CopyCommand.CopyOpaque;

            byte r, g, b, iDelta;

            for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
            {
                srcColour = src[srcIndex];
                dstColour = dst[dstIndex];
                if (srcColour == NOCOLOR)
                {
                    if (Opaque)
                    {
                        dst[dstIndex] = srcColour;
                    }
                    continue;
                }
                if (RGBOnly)
                    srcColour = (MAX << Colours.AShift) | (srcColour & Inversion);
                iDelta = (byte)((srcColour >> Colours.AShift) & 0xFF);
                transparent = hasTransparency;

                if (SwitchRB)
                {
                    b = (byte)((srcColour >> Colours.RShift) & 0xFF);
                    g = (byte)((srcColour >> Colours.GShift) & 0xFF);
                    r = (byte)((srcColour >> Colours.BShift) & 0xFF);
                    srcColour = (iDelta << Colours.AShift)
                            | ((r & 0xFF) << Colours.RShift)
                            | ((g & 0xFF) << Colours.GShift)
                            | ((b & 0xFF) << Colours.BShift);
                }
                alpha = iDelta;

                if (alpha < TWO || alpha == MAX || dstColour == NOCOLOR)
                    goto ASSIGN;

                if (Back) alpha = (MAX - alpha);

                BLEND:
                //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-colour-fast?
                C1 = (uint)dstColour;
                C2 = (uint)srcColour;
                invAlpha = MAX - alpha;
                RB = ((invAlpha * (C1 & RBMASK)) + (alpha * (C2 & RBMASK))) >> 8;
                AG = (invAlpha * ((C1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((C2 & GMASK) >> 8)));
                srcColour = (int)((RB & RBMASK) | (AG & AGMASK));

                ASSIGN:
                if (transparent)
                {
                    transparent = false;
                    alpha = opacity;
                    goto BLEND;
                }
                dst[dstIndex] = srcColour;
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
        /// Applicable flags: Opaque, Backdrop, InvertCanvasColour</param>
        /// <param name="dstCounter">Counter by which destination index moves to next position for copy.</param>
        /// <param name="srcCounter">Counter by which source index moves to next position for copy.</param>
        /// <param name="ignoreValue">If provided, positions in destination block where value is as same as this value will not get overwritten.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(byte* src, int srcIndex, byte* dst, int dstIndex, int length,
            CopyCommand command = 0, int dstCounter = 1, int srcCounter = 1)
        {
            if (length == 0)
                return;
            bool Back = (command & CopyCommand.Backdrop) == CopyCommand.Backdrop;
            bool Clear = src == null;

            if (dstCounter <= 0)
                dstCounter = 1;
            if (srcCounter <= 0)
                srcCounter = 1;

            byte srcByte = 0, dstColour;

            if (Clear)
            {
                for (int i = 0; i < length; i++, dstIndex += dstCounter)
                {
                    dstColour = dst[dstIndex];
                    if (Back && dstColour != ZERO)
                        continue;
                    dst[dstIndex] = srcByte;
                }
            }
            else
            {
                for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                {
                    srcByte = src[srcIndex];
                    dstColour = dst[dstIndex];
                    if (Back && dstColour != ZERO)
                        continue;
                    dst[dstIndex] = srcByte;
                }
            }
        }

        /// <summary>
        /// Copies source memory block to destination memory block.
        /// </summary>
        /// <param name="src">Source memory block</param>
        /// <param name="srcIndex">Index in source from where copy operation should start</param>
        /// <param name="dst">Destination memory block to copy data to.</param>
        /// <param name="dstIndex">Index in destination where paste operation should start</param>
        /// <param name="length">Length of pixels to be copied</param>
        /// <param name="command">Command to control copy operation.
        /// Applicable flags: Opaque, Backdrop, InvertCanvasColour</param>
        /// <param name="srcAlphas">Alpha channel information of the source block.</param>
        /// <param name="dstCounter">Counter by which destination index moves to next position for copy.</param>
        /// <param name="srcCounter">Counter by which source index moves to next position for copy.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(IntPtr src, int srcIndex, IntPtr dst, int dstIndex, int length,
            CopyCommand command = 0, int dstCounter = 1, int srcCounter = 1, byte transparency = 0)
        {
            Copy((int*)src, srcIndex, (int*)dst, dstIndex, length, command, dstCounter, srcCounter, transparency);
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
        /// <param name="length">Length of pixels to be copied. Applicable flags: Opaque, Backdrop, InvertCanvasColour</param>
        /// <param name="conditionValue">Value of condition to use to qualify copy.</param>
        /// <param name="criteria">Numeric criteria to control copy operation.</param>
        /// <param name="srcAlphas">Alpha channel information of the source block.</param>
        /// <param name="invert">If true, value will be inverted before copying it to the destination.</param>
        /// <param name="dstCounter">Counter by which destination index moves to next position for copy.</param>
        /// <param name="srcCounter">Counter by which source index moves to next position for copy.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(int* src, int srcIndex, int* dst, int dstIndex, 
            int length, int conditionValue, NumCriteria criteria, byte* srcAlphas = null,
            bool invert = false, int dstCounter = 1, int srcCounter = 1, byte transparency = 0)
        {
            if (length == 0)
                return;
            bool Clear = src == null;

            if (dstCounter <= 0)
                dstCounter = 1;
            if (srcCounter <= 0)
                srcCounter = 1;

            int srcColour = 0, dstColour;
            bool NoAlphas = srcAlphas == null;

            if (Clear)
            {
                if (invert)
                    srcColour ^= Inversion;
                for (int i = 0; i < length; i++, dstIndex += dstCounter)
                {
                    dstColour = dst[dstIndex];

                    switch (criteria)
                    {
                        case NumCriteria.Equal:
                        default:
                            if (dstColour != conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotEqual:
                            if (dstColour == conditionValue)
                                continue;
                            break;
                        case NumCriteria.GreaterThan:
                            if (dstColour <= conditionValue)
                                continue;
                            break;
                        case NumCriteria.LessThan:
                            if (dstColour >= conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotGreaterThan:
                            if (dstColour > conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotLessThan:
                            if (dstColour < conditionValue)
                                continue;
                            break;
                    }

                    dst[dstIndex] = srcColour;
                }
                return;
            }

            uint C1, C2, RB, AG, invAlpha, alpha;
            bool Back = conditionValue == 0 && criteria == NumCriteria.Equal;
            bool hasTransparency = transparency != ZERO;

            if (NoAlphas)
            {
                for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                {
                    srcColour = src[srcIndex];
                    if (invert)
                        srcColour ^= Inversion;

                    dstColour = dst[dstIndex];

                    switch (criteria)
                    {
                        case NumCriteria.Equal:
                        default:
                            if (dstColour != conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotEqual:
                            if (dstColour == conditionValue)
                                continue;
                            break;
                        case NumCriteria.GreaterThan:
                            if (dstColour <= conditionValue)
                                continue;
                            break;
                        case NumCriteria.LessThan:
                            if (dstColour >= conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotGreaterThan:
                            if (dstColour > conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotLessThan:
                            if (dstColour < conditionValue)
                                continue;
                            break;
                    }

                    if (hasTransparency)
                    {
                        alpha = transparency;
                        C1 = (uint)dstColour;
                        C2 = (uint)srcColour;
                        //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-colour-fast?
                        invAlpha = MAX - alpha;
                        RB = ((invAlpha * (C1 & RBMASK)) + (alpha * (C2 & RBMASK))) >> 8;
                        AG = (invAlpha * ((C1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((C2 & GMASK) >> 8)));
                        srcColour = (int)((RB & RBMASK) | (AG & AGMASK));
                    }
                    dst[dstIndex] = srcColour;
                }
            }
            else
            {
                bool transparent;
                for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                {
                    srcColour = src[srcIndex];
                    dstColour = dst[dstIndex];
                    alpha = srcAlphas[srcIndex];
                    transparent = hasTransparency;

                    switch (criteria)
                    {
                        case NumCriteria.Equal:
                        default:
                            if (dstColour != conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotEqual:
                            if (dstColour == conditionValue)
                                continue;
                            break;
                        case NumCriteria.GreaterThan:
                            if (dstColour <= conditionValue)
                                continue;
                            break;
                        case NumCriteria.LessThan:
                            if (dstColour >= conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotGreaterThan:
                            if (dstColour > conditionValue)
                                continue;
                            break;
                        case NumCriteria.NotLessThan:
                            if (dstColour < conditionValue)
                                continue;
                            break;
                    }

                    if (alpha < TWO || alpha == MAX || dstColour == NOCOLOR)
                        goto AssignColour;

                    Blend:
                    if (Back) alpha = (MAX - alpha);
                    C1 = (uint)dstColour;
                    C2 = (uint)srcColour;
                    //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-colour-fast?
                    invAlpha = MAX - alpha;
                    RB = ((invAlpha * (C1 & RBMASK)) + (alpha * (C2 & RBMASK))) >> 8;
                    AG = (invAlpha * ((C1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((C2 & GMASK) >> 8)));
                    srcColour = (int)((RB & RBMASK) | (AG & AGMASK));

                AssignColour:
                    if (invert)
                        srcColour ^= Inversion;
                    dst[dstIndex] = srcColour;
                    if (transparent)
                    {
                        transparent = false;
                        alpha = transparency;
                        goto Blend;
                    }
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
        /// <param name="length">Length of pixels to be copied. Applicable flags: Opaque, Backdrop, InvertCanvasColour</param>
        /// <param name="conditionValue">Value of condition to use to qualify copy.</param>
        /// <param name="criteria">Numeric criteria to control copy operation.</param>
        /// <param name="invert">If true, value will be inverted before copying it to the destination.</param>
        /// <param name="dstCounter">Counter by which destination index moves to next position for copy.</param>
        /// <param name="srcCounter">Counter by which source index moves to next position for copy.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(byte* src, int srcIndex, byte* dst,
            int dstIndex, int length, byte conditionValue, NumCriteria criteria,
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
                    srcByte = MAX;

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
                        srcByte = (byte)(MAX - srcByte);

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
        /// <param name="length">Length of pixels to be copied. Applicable flags: Opaque, Backdrop, InvertCanvasColour</param>
        /// <param name="conditionValue">Value of condition to use to qualify copy.</param>
        /// <param name="criteria">Numeric criteria to control copy operation.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void CopyValue(int value, int* dst, int dstIndex, int length,
            int conditionValue, NumCriteria criteria = 0)
        {
            if (length == 0)
                return;
            int srcColour = 0;
            srcColour = value;
            int last = dstIndex + length;
            if (criteria == 0)
            {
                for (int i = dstIndex; i < last; i++)
                    dst[i] = value;
                return;
            }
            int dstValue;
            for (int i = dstIndex; i < last; i++)
            {
                dstValue = dst[i];

                switch (criteria)
                {
                    case NumCriteria.Equal:
                    default:
                        if (dstValue != conditionValue)
                            continue;
                        break;
                    case NumCriteria.NotEqual:
                        if (dstValue == conditionValue)
                            continue;
                        break;
                    case NumCriteria.GreaterThan:
                        if (dstValue <= conditionValue)
                            continue;
                        break;
                    case NumCriteria.LessThan:
                        if (dstValue >= conditionValue)
                            continue;
                        break;
                    case NumCriteria.NotGreaterThan:
                        if (dstValue > conditionValue)
                            continue;
                        break;
                    case NumCriteria.NotLessThan:
                        if (dstValue < conditionValue)
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
        /// <param name="length">Length of pixels to be copied. Applicable flags: Opaque, Backdrop, InvertCanvasColour</param>
        /// <param name="conditionValue">Value of condition to use to qualify copy.</param>
        /// <param name="criteria">Numeric criteria to control copy operation.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void CopyValue(byte value, byte* dst, int dstIndex, int length,
            byte conditionValue, NumCriteria criteria = 0)
        {
            if (length == 0)
                return;
            int srcColour = 0;
            srcColour = value;
            int last = dstIndex + length;
            if (criteria == 0)
            {
                for (int i = dstIndex; i < last; i++)
                    dst[i] = value;
                return;
            }

            byte dstColour;
            for (int i = dstIndex; i < last; i++)
            {
                dstColour = dst[i];

                switch (criteria)
                {
                    case NumCriteria.Equal:
                    default:
                        if (dstColour != conditionValue)
                            continue;
                        break;
                    case NumCriteria.NotEqual:
                        if (dstColour == conditionValue)
                            continue;
                        break;
                    case NumCriteria.GreaterThan:
                        if (dstColour <= conditionValue)
                            continue;
                        break;
                    case NumCriteria.LessThan:
                        if (dstColour >= conditionValue)
                            continue;
                        break;
                    case NumCriteria.NotGreaterThan:
                        if (dstColour > conditionValue)
                            continue;
                        break;
                    case NumCriteria.NotLessThan:
                        if (dstColour < conditionValue)
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
            CopyCommand command = 0, int dstCounter = 1, int srcCounter = 1, int? ignoreValue = null)
        {
            if (destinations == null || destinations.Length == 0 || length == 0)
                return;
            int count = destinations.Length;
            bool Back = (command & CopyCommand.Backdrop) == CopyCommand.Backdrop;
            bool Clear = src == null;
            var exclude = ignoreValue ?? 0;
            bool ignore = ignoreValue != null;

            if (dstCounter <= 0)
                dstCounter = 1;
            if (srcCounter <= 0)
                srcCounter = 1;
            byte srcByte = 0, dstColour;

            if (Clear)
            {
                for (int i = 0; i < length; i++, dstIndex += dstCounter)
                {
                    foreach (var dst in destinations)
                    {
                        dstColour = dst[dstIndex];
                        if ((Back && dstColour != ZERO) ||
                            (ignore && dstColour == exclude))
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
                        dstColour = dst[dstIndex];
                        if ((Back && dstColour != ZERO) ||
                            (ignore && dstColour == exclude))
                            continue;
                        dst[dstIndex] = srcByte;
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
        /// Applicable flags: Opaque, Backdrop, InvertCanvasColour</param>
        /// <param name="srcAlphas">Alpha channel information of the source block.</param>
        /// <param name="useDstIndexForAlphas">If true, indices of destination block will be used to read source alpha information.</param>
        /// <param name="dstCounter">Counter by which destination index moves to next position for copy.</param>
        /// <param name="srcCounter">Counter by which source index moves to next position for copy.</param>
        /// <param name="ignoreValue">If provided, positions in destination block where value is as same as this value will not get overwritten.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(int* src, int srcIndex, int*[] destinations, int dstIndex, int length,
            CopyCommand command = 0, byte* srcAlphas = null,
            bool useDstIndexForAlphas = false, int dstCounter = 1, int srcCounter = 1, int? ignoreValue = null)
        {
            if (length == 0 || destinations == null || destinations.Length == 0)
                return;
            bool Back = (command & CopyCommand.Backdrop) == CopyCommand.Backdrop;
            bool Clear = src == null;
            var exclude = ignoreValue ?? 0;
            bool ignore = ignoreValue != null;

            if (dstCounter <= 0)
                dstCounter = 1;
            if (srcCounter <= 0)
                srcCounter = 1;

            int srcColour = 0, dstColour;

            bool HasAlphas = srcAlphas != null;
            if (!HasAlphas)
            {
                if (Clear)
                {
                    for (int i = 0; i < length; i++, dstIndex += dstCounter)
                    {
                        foreach (var dst in destinations)
                        {
                            dstColour = dst[dstIndex];
                            if ((Back && dstColour != 0) ||
                            (ignore && dstColour == exclude))
                                continue;
                            dst[dstIndex] = srcColour;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                    {
                        srcColour = src[srcIndex];
                        foreach (var dst in destinations)
                        {
                            dstColour = dst[dstIndex];
                            if (srcColour == 0 || (Back && dstColour != 0) ||
                            (ignore && dstColour == exclude))
                                continue;
                            dst[dstIndex] = srcColour;
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
                    for (int i = 0; i < length; i++, dstIndex += dstCounter)
                    {
                        foreach (var dst in destinations)
                        {
                            dstColour = dst[dstIndex];
                            alphaIdx = useDstIndexForAlphas ? dstIndex : srcIndex;

                            if ((Back && dstColour != 0) ||
                            (ignore && dstColour == exclude))
                                continue;
                            dst[dstIndex] = srcColour;
                            srcAlphas[alphaIdx] = 0;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++, dstIndex += dstCounter, srcIndex += srcCounter)
                    {
                        srcColour = src[srcIndex];
                        foreach (var dst in destinations)
                        {
                            dstColour = dst[dstIndex];
                            alphaIdx = useDstIndexForAlphas ? dstIndex : srcIndex;
                            alpha = srcAlphas[alphaIdx];

                            if (srcColour == 0 || (Back && dstColour != 0 && alpha == 0) ||
                            (ignore && dstColour == exclude))
                                continue;

                            if (alpha == 0 || alpha == 255 || dstColour == 0)
                                goto AssignColour;

                            if (Back) alpha = (255 - alpha);
                            C1 = (uint)dstColour;
                            C2 = (uint)srcColour;
                            //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-colour-fast?
                            invAlpha = 255 - (uint)alpha;
                            RB = ((invAlpha * (C1 & Colours.RBMASK)) + (alpha * (C2 & Colours.RBMASK))) >> 8;
                            AG = (invAlpha * ((C1 & Colours.AGMASK) >> 8)) + (alpha * (Colours.ONEALPHA | ((C2 & Colours.GMASK) >> 8)));
                            srcColour = (int)((RB & Colours.RBMASK) | (AG & Colours.AGMASK));
                        AssignColour:
                            dst[dstIndex] = srcColour;
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
        public static T[] ResizedData<T>(this T[] source, int newWidth, int newHeight, 
            int oldWidth, int oldHeight, bool clear = false)
        {
            T[] result = new T[newWidth * newHeight];
            if (clear)
            {
                return result;
            }
            CopyBlock(source, new Rectangle(0, 0, oldWidth, oldHeight), oldWidth, oldHeight, result, 0, 0, newWidth);
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
        public static void ResizedData<T>(ref T[] source, int newWidth, int newHeight,
            int oldWidth, int oldHeight, bool clear = false)
        {
            T[] result = new T[newWidth * newHeight];
            if (clear)
            {
                source = result;
                return;
            }
            CopyBlock(source, new Rectangle(0, 0, oldWidth, oldHeight), oldWidth, oldHeight, result, 0, 0, newWidth);
            source = result;
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
        public static T[] ResizedData<T>(this T[] source, int newWidth,
            int newHeight, int oldWidth, int oldHeight, T defaultValue, bool clear = false)
        {
            int len = newWidth * newHeight;
            T[] result = System.Linq.Enumerable.Repeat(defaultValue, len).ToArray();
            if (clear)
            {
                return result;
            }
            CopyBlock(source, new Rectangle(0, 0, oldWidth, oldHeight), oldWidth, oldHeight, result, 0, 0, newWidth);
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
        public static void ResizedData<T>(ref T[] source, int newWidth,
            int newHeight, int oldWidth, int oldHeight, T defaultValue, bool clear = false)
        {
            int len = newWidth * newHeight;
            T[] result = System.Linq.Enumerable.Repeat(defaultValue, len).ToArray();
            if (clear)
            {
                source = result;
                return;
            }
            CopyBlock(source, new Rectangle(0, 0, oldWidth, oldHeight), oldWidth, oldHeight, result, 0, 0, newWidth);
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
        public static unsafe IntPtr ResizedData(IntPtr source, int srcLen, int newWidth,
            int newHeight, int oldWidth, int oldHeight, bool clear = false)
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
                CopyBlock(src, new Rectangle(0, 0, copyW, copyH), srcLen, oldWidth, oldHeight, dst,
                    0, 0, newWidth, result.Length, 0);
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

        #region FLIP
        public static unsafe Size FlipData(int* src, int srcW, int srcH, out IntPtr result, FlipMode flipMode)
        {
            int dstW = srcW;
            int dstH = srcH;
            int dstLen = srcW * srcH;
            int srcLen = dstLen;

            int* dst;
            fixed (int* d = new int[dstW * dstH])
                dst = d;

            int i = 0;
            if (flipMode == FlipMode.Horizontal)
            {
                for (var y = srcH - 1; y >= 0; y--)
                {
                    for (var x = 0; x < srcW; x++)
                    {
                        var srcInd = y * srcW + x;
                        dst[i] = src[srcInd];
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
                        dst[i] = src[srcInd];
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
    }
}
