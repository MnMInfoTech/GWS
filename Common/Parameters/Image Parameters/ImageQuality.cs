/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    public interface IImageQuality: IInLineParameter
    {
        byte Quality { get; }
    }

    partial class Parameters
    {
        struct ImageQuality : IImageQuality
        {
            readonly byte Quality;

            public ImageQuality(byte quality)
            {
                Quality = quality;
            }
            byte IImageQuality.Quality => Quality;

            public override string ToString() =>
                Quality.ToString();
        }

        #region TO IMAGE QUALITY
        public static IImageQuality ToImageQuality(this byte quality) =>
            new ImageQuality(quality);
        #endregion
    }
}
