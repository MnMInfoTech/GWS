/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    public struct SinCos
    {
        public readonly float Sin;
        public readonly float Cos;

        public SinCos(float angle)
        {
            Angles.SinCos(angle, out Sin, out Cos);
        }
        public SinCos(float angle, float deviation)
        {
            Angles.SinCos(angle, out Sin, out Cos);
            Sin += deviation;
            Cos += deviation;
        }
        public SinCos(float angle, float sinOffset, float cosOffset)
        {
            Angles.SinCos(angle, out Sin, out Cos);
            Sin += sinOffset;
            Cos += cosOffset;
        }
    }
}
