/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if(GWS || Window)

namespace MnM.GWS
{
    public struct APoint
    {
        public readonly float Val;
        public readonly int Axis;
        public readonly byte Horizontal;
        public readonly int Color;

        public APoint(float val, int axis, byte horizontal, int color)
        {
            Val = val;
            Axis = axis;
            Horizontal = horizontal;
            Color = color;
        }
    }
}
#endif
