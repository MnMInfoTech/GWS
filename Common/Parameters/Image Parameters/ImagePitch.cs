/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details.
* Author: Mukesh Adhvaryu
*/
namespace MnM.GWS
{
    public  interface IImagePitch : IInLineParameter
    {
        byte Pitch { get; }
    }
    partial class Parameters
    {
        struct pImagePitch : IImagePitch
        {
            readonly byte Pitch;

            public pImagePitch(byte pitch)
            {
                Pitch = pitch;
            }

            byte IImagePitch.Pitch => Pitch;

            public override string ToString() =>
                Pitch.ToString();
        }

        #region TO IMAGE PITCH
        public static IInLineParameter ToImagePitch(this byte pitch) =>
            new pImagePitch(pitch);
        #endregion
    }

}
