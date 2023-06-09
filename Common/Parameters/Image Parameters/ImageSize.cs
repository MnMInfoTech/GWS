/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
    public interface IImageSize: ISize, IInLineParameter, IValid
    { }

    public struct ImageSize: IImageSize
    {
        public int Width, Height;

        public ImageSize(ISize size)
        {
            Width = size?.Width ?? 0;
            Height = size?.Height ?? 0;
        }
        public ImageSize(int w, int h)
        {
            Width = w;
            Height = h;
        }
        public ImageSize(int wh)
        {
            Width = wh;
            Height = wh;
        }
        int ISize.Width => Width;
        int ISize.Height => Height;

        public bool Valid => Width > 0 && Height > 0;
    }
}
