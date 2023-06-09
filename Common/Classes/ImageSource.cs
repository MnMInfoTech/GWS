/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;

namespace MnM.GWS
{
    #region IIMAGE SOURCE
    /// <summary>
    /// Represents an object which offers data source which can be rendered on target
    /// of type IRenderer. Any object which implements IBlock or ICopyable which both
    /// implements this interface will be dealt automatically by GWS to perform render operation.
    /// </summary>
    public interface IImageSource : IImageData, IObject, IOriginCompatible
    { }
    #endregion

    #region IMAGE SOURCE
    public sealed class ImageSource: IImageSource, IDisposable
    {
        IntPtr data;
        int width, height;

        public ImageSource(IntPtr pixels, int width, int height)
        {
            data = pixels;
            this.width = width;
            this.height = height;
        }

        public IntPtr Source => data;
        public bool Valid => width > 0 && height > 0 && data != IntPtr.Zero;
        public int Width => width;
        public int Height => height;

        int IPoint.X => 0;
        int IPoint.Y => 0;
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion() =>
            this;
        bool IOriginCompatible.IsOriginBased => true;

        public void Dispose()
        {
            data = IntPtr.Zero;
        }
    }
    #endregion
}
