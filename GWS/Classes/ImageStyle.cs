/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

#if (GWS || Window)
using System;

namespace MnM.GWS
{
    public interface IImageStyle: ICloneable
    {
        /// <summary>
        /// Get sor sets an alignment of a buffer in a bounding box.
        /// </summary>
        ImagePosition ImageAlignment { get; set; }

        /// <summary>
        /// Gets or sets how buffer is drawn whether scalled or unscalled.
        /// </summary>
        ImageDraw ImageDraw { get; set; }

        /// <summary>
        /// Gets or sets a Buffer image to be drawn to screen.
        /// </summary>
        IImageSource Image { get; set; }
    }

    public class ImageStyle: IImageStyle
    {
        public ImagePosition ImageAlignment { get; set; }
        public ImageDraw ImageDraw { get; set; }
        public IImageSource Image { get; set; }
        public object Clone()
        {
            var imgStyle = new ImageStyle();
            imgStyle.ImageAlignment = ImageAlignment;
            imgStyle.ImageDraw = ImageDraw;
            if (Image is ICloneable)
            {
                imgStyle.Image = ((ICloneable)Image).Clone() as IImageSource;
            }
            else if (Image != null)
            {
                var image = Factory.newCanvas(Image.Width, Image.Height);
                image.DrawImage(Image, 0, 0);
                Image = image;
            }
            return imgStyle;
        }
    }
}
#endif
