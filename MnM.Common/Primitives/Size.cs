/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System.Runtime.InteropServices;

namespace MnM.GWS
{
#if (GWS || Window)
    /// <summary>
    /// Represents dimension in terms of width and height.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Size : ISize
    {
        /// <summary>
        /// Value of Width.
        /// </summary>
        public int Width;

        /// <summary>
        /// Value of height.
        /// </summary>
        public int Height;

        /// <summary>
        /// Empty instance of this object.
        /// </summary>
        public readonly static Size Empty = new Size();

        /// <summary>
        /// Creates new instance using given values of width and height.
        /// </summary>
        /// <param name="width">Width value.</param>
        /// <param name="height">Height value.</param>
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }
        public Size(Rectangle rc) :
            this(rc.Width, rc.Height)
        { }

        int ISize.Width => Width;
        int ISize.Height => Height;

        /// <summary>
        /// Implicity converts given size object to SizeF instance.
        /// </summary>
        /// <param name="size">Size object to convert.</param>
        public static implicit operator SizeF(Size size) =>
            new SizeF(size.Width, size.Height);

        /// <summary>
        /// Implicity converts given size object to Vector instance.
        /// </summary>
        /// <param name="size">Size object to convert.</param>
        public static implicit operator Vector(Size size) =>
            new Vector(size.Width, size.Height);

        /// <summary>
        /// Explicitely converts given size object to Rectangle instance.
        /// </summary>
        /// <param name="s"></param>
        public static explicit operator Rectangle(Size s)=>
            new Rectangle(0, 0, s.Width,s.Height);

        /// <summary>
        /// Implicity converts given size object to boolean value.
        /// Coverts to true if values are valid otherwise false.
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator bool(Size s) =>
            !(s.Width < 1 || int.MinValue == (s.Width) || s.Height < 1 || int.MinValue == (s.Height));
    }
#endif
}
