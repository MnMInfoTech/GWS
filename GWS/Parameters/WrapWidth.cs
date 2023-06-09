/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details.
* Author: Mukesh Adhvaryu
*/
#if (GWS || Window)
namespace MnM.GWS
{
    public interface IWrapWidth: IParameter
    {
        /// <summary>
        /// Gets the allowed width before text to be wrapped to next line.
        /// </summary>
        int WrapWidth { get; }
    }

    partial class Parameters
    {
        struct pWrapWidth : IWrapWidth
        {
            int WrapWidth;

            public pWrapWidth(int wrapWidth)
            {
                WrapWidth = wrapWidth;
            }

            int IWrapWidth.WrapWidth => WrapWidth;
        }

        public static IWrapWidth ToWrapWidth(this int wrapWidth) =>
            new pWrapWidth(wrapWidth);
    }
}
#endif
