/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;

namespace MnM.GWS
{
    public class SizeEventArgs : EventArgs, ISizeEventArgs
    {
        const string toStr = "width:{0}, height:{1}";

        public SizeEventArgs()
        {

        }
        public SizeEventArgs(ISize e)
        {
            Width = e.Width;
            Height = e.Height;
        }
        public SizeEventArgs(int width, int height)
        {
            Width = width;
            Height = height;
        }
        public override string ToString()
        {
            return string.Format(toStr, Width, Height);
        }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}
