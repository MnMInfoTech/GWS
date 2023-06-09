/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    /// <summary>
    /// Represents a location.
    /// </summary>
    public interface ISizeF
    {
        /// <summary>
        /// Gets width of this object.
        /// </summary>
        float Width { get; }

        /// <summary>
        /// Gets height of this object.
        /// </summary>
        float Height { get; }
    }

    /// <summary>
    /// Represents dimension in terms of width and height.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SizeF: ISizeF
    {
        /// <summary>
        /// Value of Width.
        /// </summary>
        public float Width;

        /// <summary>
        /// Value of height.
        /// </summary>
        public float Height;

        /// <summary>
        /// Empty instance of this object.
        /// </summary>
        public readonly static SizeF Empty = new SizeF();


        /// <summary>
        /// Creates new instance using given values of width and height.
        /// </summary>
        /// <param name="width">Width value.</param>
        /// <param name="height">Height value.</param>
        public SizeF(float width, float height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Returns Size object containing ceiling values of width and height.
        /// </summary>
        /// <returns></returns>
        public Size Ceiling() =>
            new Size(Width.Ceiling(), Height.Ceiling());

        /// <summary>
        /// Returns Size object containing rounding values of width and height.
        /// </summary>
        /// <param name="r">MidpointRounding value.</param>
        /// <returns></returns>
        public Size Round(MidpointRounding r = MidpointRounding.ToEven) =>
            new Size(Width.Round(r), Height.Round(r));

        /// <summary>
        /// Returns Size object containing flooring values of width and height.
        /// </summary>
        /// <returns></returns>
        public Size Floor() =>
            new Size((int)Width, (int)Height);

        /// <summary>
        /// Implicity converts given sizef object to VectorF instance.
        /// </summary>
        /// <param name="sizef">Sizef object to convert.</param>
        public static explicit operator VectorF(SizeF sizef) =>
            new VectorF(sizef.Width, sizef.Height);

        /// <summary>
        /// Explicitely converts given size object to Rectangle instance.
        /// </summary>
        /// <param name="s"></param>
        public static explicit operator RectangleF(SizeF s) =>
            new RectangleF(0, 0, s.Width, s.Height);

        /// <summary>
        /// Indicates if this instance has valid width and height values.
        /// </summary>
        public static implicit operator bool(SizeF sizef) =>
            !(sizef.Width < 1 || float.IsNaN(sizef.Width) || sizef.Height < 1 || float.IsNaN(sizef.Height));

        float ISizeF.Width => Width;
        float ISizeF.Height => Height;
    }
}
