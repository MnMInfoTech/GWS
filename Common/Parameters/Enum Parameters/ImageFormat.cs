/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    public interface IImageFormat: IProperty<ImageFormat>
    {
        ImageFormat Format { get; }
    }

    partial class Parameters
    {
        struct pImageFormat: IImageFormat
        {
            ImageFormat Format;

            public pImageFormat(ImageFormat format)
            {
                Format = format;
            }
            ImageFormat IImageFormat.Format => Format;
            object IValue.Value => Format;
            ImageFormat IValue<ImageFormat>.Value => Format;

            public override string ToString() =>
                Format.ToString();
        }

        #region TO IMAGE FORMAT
        public static IImageFormat ToParameter(this ImageFormat format) =>
            new pImageFormat(format);
        #endregion
    }
}
