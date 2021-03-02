/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Collections.Generic;
using System.IO;

namespace MnM.GWS
{
    #region IPENCONTEXT
    /// <summary>
    /// This is a marker interface which represents an object which can be converted to a buffer pen.
    /// </summary>
    public interface IPenContext : IContext
    { }
    #endregion

    #region IPIXELS
    public interface IPixels : IBlockable
    {
        IntPtr Source { get; }
    }
    #endregion

    #region IMEMBLOCK
    public interface IMemBlock : IPixels, IPoint
    {
        bool Valid { get; }

        void Change(IntPtr source, int srcLen, int srcW, int srcH, int x, int y, int w, int h);
    }
    #endregion

    #region IMULTI BUFFERED
    public interface IMultiBuffered : IBlockable
    {
        /// <summary>
        /// Gets Internally stored pixels and alpha values for entire memory block.
        /// </summary>
        /// <param name="Pixels">Memory block representing color pixels.</param>
        /// <param name="Alphas">Memory block representing alpha values of border pixels.</param>
        void GetData(out int[] Pixels, out byte[] Alphas, bool SecondBuffer = false);
    }
    #endregion

    #region IMAGE PROCESSOR
    /// <summary>
    /// Represents an object to facilitate image data processing. GWS uses default image reader derived from STBImage. 
    /// for more info on STBImage visit: https://github.com/nothings/stb
    /// </summary>
    public interface IImageProcessor : IAttachment
    {
        /// <summary>
        /// Reads a file located on a given path on disk or network drive and provides a processed data to be used for creating memory buffer. 
        /// </summary>
        /// <param name="path">Path of a file located on disk or network drive</param>
        /// <returns>
        /// Pair.Item1 - data in bytes array
        /// Pair.Item2 - Width information.
        /// Pair.Item3 - Height information.
        /// </returns>
        Lot<byte[], int, int> Read(string path);

        /// <summary>
        /// Reads a memory stream and providesa processed data to be used for creating memory buffer.
        /// </summary>
        /// <param name="stream">Strem to process</param>
        /// <returns>
        /// Pair.Item1 - data in bytes array
        /// Pair.Item2 - Width information.
        /// Pair.Item3 - Height information.
        /// </returns>
        Lot<byte[], int, int> Read(Stream stream);

        /// <summary>
        /// Reads a byte array and providesa processed data to be used for creating memory buffer.
        /// </summary>
        /// <param name="stream">Strem to process</param>
        /// <returns>
        /// Pair.Item1 - data in bytes array
        /// Pair.Item2 - Width information.
        /// Pair.Item3 - Height information.
        /// </returns>
        Lot<byte[], int, int> Read(byte[] stream);

        /// <summary>
        /// Writes a given memory block to a file on a given stream.
        /// </summary>
        /// <param name="pixels">Memory block to write to disk file.</param>
        /// <param name="width">Width of memory block.</param>
        /// <param name="height">Height of memory block.</param>
        /// <param name="len">Length of memory block</param>
        /// <param name="pitch">Pitch to be used for reading memory block. Default use 4 - R, G, B, A</param>
        /// <param name="dest">Destination stream where image to be writtwn.</param>
        /// <param name="format">Format of the targeted image file</param>
        /// <param name="quality">Resolution quality of the tageted image file</param>
        void Write(IntPtr pixels, int width, int height, int len, int pitch, Stream dest, ImageFormat format, int quality = 50);
    }
    #endregion

    #region IANIMATED-GIF-FRAME
    /// <summary>
    /// Represents an object which holds animeted GIF image information.
    /// </summary>
    public interface IAnimatedGifFrame
    {       /// <summary>
            /// Data of the image in byte array.
            /// </summary>
        byte[] Data { get; }

        /// <summary>
        /// Delay unit to be used to change a frame.
        /// </summary>
        int Delay { get; }
    }
    #endregion
}