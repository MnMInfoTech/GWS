/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

namespace MnM.GWS
{
    #region ITHICKNESS
    public interface IThickness : IInLineParameter
    {
        sbyte Thickness { get; }
    }
    #endregion

    partial class Parameters
    {
        #region THICKNESS
        public struct pThickness : IThickness
        {
            readonly sbyte Thickness;

            public pThickness(sbyte thickness)
            {
                Thickness = thickness;
            }

            sbyte IThickness.Thickness => Thickness;
            public static implicit operator sbyte(pThickness thickness) =>
                thickness.Thickness;

            public override string ToString() =>
                Thickness.ToString();
        }
        #endregion

        #region TO THICKNESS
        public static IThickness ToThickness(this sbyte thickness) =>
            new pThickness(thickness);
        public static IThickness ToThickness(this byte thickness) =>
            new pThickness((sbyte)thickness);
        public static IThickness ToThickness(this int thickness) =>
            new pThickness((sbyte)thickness);
        #endregion
    }
}
