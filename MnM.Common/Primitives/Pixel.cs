/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System.Runtime.InteropServices;

namespace MnM.GWS.Advanced
{
#if (GWS || Window)
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Pixel
    {
        #region VARIABLES
        public int Color;
        public int X, Y;
        public byte Alpha;
        #endregion

        public Pixel(int x, int y, int color, byte alpha)
        {
            X = x;
            Y = y;
            Color = color;
            Alpha = alpha;
        }
    }
#endif
}
