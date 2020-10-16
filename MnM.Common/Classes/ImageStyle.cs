/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if (GWS || Window)
    using System;
    public sealed class ImageStyle: ICloneable
    {
        /// <summary>
        /// Get sor sets an alignment of a buffer in a bounding box.
        /// </summary>
        public ImagePosition Alignment { get; set; }

        /// <summary>
        /// Gets or sets how buffer is drawn whether scalled or unscalled.
        /// </summary>
        public ImageDraw Draw { get; set; }

        /// <summary>
        /// Gets or sets a Buffer image to be drawn to screen.
        /// </summary>
        public IBlock Image { get; set; }

        /// <summary>
        /// Clones this object and returns it.
        /// </summary>
        /// <returns></returns>
        public ImageStyle Clone()
        {
            var i = new ImageStyle();
            i.Alignment = Alignment;
            i.Draw = Draw;
            i.Image = Image.Clone() as IBlock;
            return i;
        }

        object ICloneable.Clone() => Clone();
    }
#endif
}
