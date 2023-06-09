/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if (GWS  || Window)
using System;

namespace MnM.GWS
{
    public interface ISizeEventArgs : IEventArgs, ISize
    {
    }
    internal interface IExSizeEventArgs : ISizeEventArgs
    {
        void Reset(int w, int h);
    }
    public class SizeEventArgs : EventArgs, IExSizeEventArgs
    {
        const string toStr = "width:{0}, height:{1}";
        int width, height;

        public SizeEventArgs()
        {

        }
        public SizeEventArgs(ISize e)
        {
            width = e.Width;
            height = e.Height;
        }
        public SizeEventArgs(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public override string ToString()
        {
            return string.Format(toStr, width, height);
        }

        public int Width => width;
        public int Height => height;

        void IExSizeEventArgs.Reset(int w, int h)
        {
            width = w;
            height = h;
        }
    }
}
#endif