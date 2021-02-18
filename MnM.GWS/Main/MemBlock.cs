/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public class MemBlock: IMemBlock
    {
        #region VARIABLES
        public volatile int X, Y;
        public int Width, Height;
        volatile int[] Data;
        #endregion

        #region PROPERTIES
        public unsafe IntPtr Source
        {
            get
            {
                fixed (int* p = Data)
                    return (IntPtr)p;
            }
        }
        int ISize.Width => Width;
        int ISize.Height => Height;
        public int Length => Data.Length;
        int IPoint.X => X;
        int IPoint.Y => Y;
        public bool Valid => Data.Length > 0;
        #endregion

        #region CHANGE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Change(IntPtr source, int srcLen, int srcW, int srcH, int x, int y, int w, int h)
        {
            var rc = Rects.CompitibleRc(srcW, srcH, x, y, w, h);
            X = rc.X;
            Y = rc.Y;
            Data = new int[rc.Width * rc.Height];
            fixed (int* d = Data)
                Blocks.CopyBlock((int*)source, rc, srcLen, srcW, srcH, d, 0, 0, rc.Width, Data.Length, 0);
        }
        #endregion
    }
}
