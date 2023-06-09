/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    /// <summary>
    /// Represents a location.
    /// </summary>
    public interface ISize 
    {
        /// <summary>
        /// Gets width of this object.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets height of this object.
        /// </summary>
        int Height { get; }
    }

    public interface IUserSize : ISize, IProperty, IValid
    { }

    /// <summary>
    /// Represents dimension in terms of width and height.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Size : IUserSize
    {
        #region VARIABLES
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
        #endregion

        #region CONSTRUCTORS

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

        /// <summary>
        /// Creates new instance using given values of width and height.
        /// </summary>
        /// <param name="width">Width value.</param>
        /// <param name="height">Height value.</param>
        public Size(float width, float height)
        {
            Width = (int)width;
            Height = (int)height;
            if (width - Width != 0)
                ++Width;
            if (height - Height != 0)
                ++Height;
        }

        public Size(ISize rc) :
            this(rc.Width, rc.Height)
        { }

        public Size(Margin rc) :
            this(rc.X, rc.Y)
        { }
        #endregion

        #region PROPERTIES
        int ISize.Width => Width;
        int ISize.Height => Height;
        object IValue.Value => this;
        bool IValid.Valid => Width > 0 && Height > 0;
        #endregion

        #region OPERATORS
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
        public static explicit operator Rectangle(Size s) =>
            new Rectangle(0, 0, s.Width, s.Height);

        /// <summary>
        /// Implicity converts given size object to boolean value.
        /// Coverts to true if values are valid otherwise false.
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator bool(Size s) =>
            (s.Width > 0 && s.Height > 0);

        public static Size operator +(Size p1, ISize p2) =>
            new Size(p1.Width + p2.Width, p1.Height + p2.Height);

        public static Size operator -(Size p1, ISize p2) =>
            new Size(p1.Width - p2.Width, p1.Height - p2.Height);

        public static Size operator *(Size p1, ISize p2) =>
            new Size(p1.Width * p2.Width, p1.Height * p2.Height);

        public static Size operator /(Size p1, ISize p2) =>
            new Size(p1.Width / p2.Width, p1.Height / p2.Height);

        public static Size operator *(Size p1, int b) =>
            new Size(p1.Width * b, p1.Height * b);

        public static Size operator +(Size p1, int b) =>
            new Size(p1.Width + b, p1.Height + b);

        public static Size operator -(Size p1, int b) =>
            new Size(p1.Width - b, p1.Height - b);

        public static Size operator /(Size p1, int b) =>
            new Size(p1.Width / b, p1.Height / b);

        public static Size operator -(Size p1) =>
            new Point(-p1.Width, -p1.Height);
        #endregion

        public override string ToString()
        {
            return string.Format("Width : {0}, Height: {1}", Width, Height);
        }
    }
}

