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
    #endregion

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

    #region IDATA
    public interface IImageData : IBlockable
    {
        /// <summary>
        /// Gets a flag to determine if background buffer support is activated or not.
        /// </summary>
        bool SupportBackgroundBuffer { get; }

        /// <summary>
        /// Gets Internally stored pixels and alpha values for entire memory block.
        /// </summary>
        /// <param name="Pixels">Memory block representing color pixels.</param>
        /// <param name="Alphas">Memory block representing alpha values of border pixels.</param>
        void GetData(out int[] Pixels, out byte[] Alphas, bool BackgroundBuffer = false);
    }
    #endregion

#if (GWS || Window)

    #region IGRAPHICS
    /// <summary>
    /// Representsan object which represents window and offers minimum but sufficient gateway into GWS world. 
    /// </summary>
    public interface IGraphics : IWritable, ICopyable, IDisposed
    {
        /// <summary>
        /// Indicates whether this object is curently inaccessible for writing or not.
        /// this object gets Inaccessible for variety of reasons such as if it is resizing or disposed.
        /// </summary>
        bool Inaccessible { get; }
    }
    #endregion

    #region ILOCK-UNLOCK
    public interface ILockUnlock
    {
        /// <summary>
        /// Locks or unlock area of pixels in this buffer in context of over-writing.
        /// <summary>
        /// <param name="lockPixels"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        void LockUnlock(bool lockPixels, int x, int y, int w, int h);
    }
    #endregion

    #region IIMAGE
    /// <summary>
    /// Represents writable and copiable memory block object which can also render shapes.
    /// </summary>
    public partial interface IImage : IGraphics, IConsolidator, IClearable, 
        IWritableBlock, IResizable, IRenderableBlock, ICloneable, IDisposed, IScalable
    { }
    #endregion

    #region ICANVAS
    public partial interface ICanvas : IImage, IContainer, IUpdatable, IRefreshable, IBackground, IReadable
    {
        event EventHandler<IEventArgs> BackgroundChanged;
    }
    #endregion

    #region IPEN
    /// <summary>
    /// Represents an object from which memory can be read.
    /// </summary>
    public interface IPen : IReadable, ICopyable, ICloneable
    {
        /// <summary>
        /// Type this pen currently represents.
        /// </summary>
        int Type { get; }
    }
    #endregion

    #region IBRUSH
    /// <summary>
    /// Represents a brush with certain fill style and gradient for drawin a shape on screen.
    /// </summary>
    public partial interface IBrush : IPen, ISettingsReceiver, IDisposable, ICloneable2, IResizable
    {
        BrushStyle Style { get; }
    }
    #endregion

    #region ITEXTURE-BRUSH
    public partial interface ITextureBrush : IPen, ISettingsReceiver, IDisposable, ICloneable2, IPixels, IResizable
    { }
    #endregion

    #region IFIXED-PEN
    public interface IFixedBrush : IReadable, IDisposable, IPixels
    { }
    #endregion

    #region IPOLYFILL
    /// <summary>
    /// Fills a polygon structure with specified PolyFill enum option.
    /// </summary>
    public interface IPolyFill : IPolySettings, IDisposable
    {
    #region PROPERTIES
        /// <summary>
        /// Far top boundary to which filling must be confined.
        /// </summary>
        int MinY { get; }

        /// <summary>
        /// Far bottom boundary to which filling must be confined.
        /// </summary>
        int MaxY { get; }

        /// <summary>
        /// Far Left boundary to which filling must be confined.
        /// If it is set to 0 then it becomes non effective.
        /// </summary>
        int MinX { get; set; }

        /// <summary>
        /// Far Right boundary to which filling must be confined.
        /// If it is set to 0 then it becomes non effective.
        /// </summary>
        int MaxX { get; set; }

        /// <summary>
        /// A Scan action delegate to record line pixels to be processed for scan line filling.
        /// </summary>
        PixelAction ScanAction { get; }
    #endregion

    #region BEGIN
        /// <summary>
        /// Sets this object for filling operation.
        /// </summary>
        /// <param name="y">Far top boundary where filling should be considered from</param>
        /// <param name="bottom">Far bottom boundary where filling should be considered upto</param>
        /// <param name="fillPattern">Fill pattern to be used to perform scan line filling</param>
        void Begin(int y, int bottom);
    #endregion

    #region FILL
        /// <summary>
        /// Performs horizontal scan line filling using specified action.
        /// </summary>
        /// <param name="fillAction">Action to be used for filling.</param>
        /// <param name="lineCommand">Line command to be used to draw end pixels</param>
        void Fill(FillAction fillAction);
    #endregion

    #region END
        /// <summary>
        /// Ends current fill operation and resets internal data.
        /// </summary>
        void End();
    #endregion

    #region SCAN
        /// <summary>
        /// Scans a line using standard line algorithm between two points of a line segment using specified action.
        /// Line will be scanned horizontally i.e. from y1 to y2 taking rounded values of both,
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        void Scan(float x1, float y1, float x2, float y2);

        /// <summary>
        /// Includes a point specified by x and y parameters in filling operation.
        /// </summary>
        /// <param name="x">X co-ordinate of point.</param>
        /// <param name="y">Y co-ordinate of point.</param>
        void Scan(float x, int y);

        /// <summary>
        /// Includes the given point in filling operation.
        /// </summary>
        /// <param name="p">The point to include.</param>
        void Scan(VectorF p);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Points"></param>
        /// <param name="Contours"></param>
        void Scan(IList<VectorF> Points, IList<int> Contours = null);
    #endregion

    #region FILL LINE
        /// <summary>
        /// Fills an axial fragmented scan line - using odd - even fill rule exclusively.
        /// </summary>
        /// <param name="data">Collection which contains fragments.</param>
        /// <param name="axis">Axis value of the line. If horizontal is true then it is Y otherwise X axis.</param>
        /// <param name="horizontal">If true, line should be scanned from top to bottom otherwise left to right</param>
        /// <param name="action">A FillAction delegate which has routine to do something with the axial information provided.</param>
        /// <param name="alpha">Alha factor to apply to whole line if supplied at all.</param>
        void FillLine(ICollection<float> data, int axis, bool horizontal, FillAction action, float? alpha = null);
    #endregion
    }
    #endregion

#endif
#if Window && GWS
    #region ITEXTURE
    public partial interface ITexture : ISize, IResizable, IDisposable, IUpdatable
    {
        /// <summary>
        /// Copies portion of data specified by copyX, copyY, copyW, copyH parameters from a given memory block and 
        /// pastes it onto this texture at given loaction specified by dstX and dstY parameters.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dstX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="dstY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyArea">Area in source to copy.</param>
        /// <param name="command">Draw command to to control copy task</param>
        void CopyFrom(IBlockable source, int dstX, int dstY, IRectangle copyArea, Command command = 0);
    }
    public interface ITexture2 : ITexture
    {
        FlipMode Flip { get; set; }
        BlendMode Mode { get; set; }
        byte Alpha { get; set; }
        int ColorMode { get; set; }
        void Bind();
        void Unbind();
    }
    public interface ITextureTarget : ITexture, IRenderTarget
    { }
    #endregion
#endif
}
