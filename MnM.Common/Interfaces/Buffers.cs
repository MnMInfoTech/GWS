/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;
using System.IO;

namespace MnM.GWS
{
#if (GWS || Window)

    #region IPIXELS
    public interface IPixels : IBlockable
    {
        IntPtr Source { get; }
    }
    #endregion

    #region IBLOCK
    public interface IBlock : IID, IWritable, ICopyable, ICloneable, IResizable, IDisposable
    { }
#endregion

    #region IIMAGE
    /// <summary>
    /// Represents smallest writable and copiable memory block object.
    /// </summary>
    public interface IImage : IBlock, IPixels, IAreaDrawable
    {
    }
    #endregion

    #region ISURFACE
    /// <summary>
    /// Represents writable and copiable memory block object which can also render shapes.
    /// </summary>
    public interface ISurface : IBlock, IUpdatable, IBackground, IDisposable
#if Advanced
       , IBrushSource, IMixableBlock, IObjectAware
#endif
    { }
    #endregion

    #region ICANVAS
    public interface ICanvas : ISurface, IContainer, IRefreshable
    {
#if Advanced
        /// <summary>
        /// Gets or sets a flag to indicate if this object supports back ground buffer.
        /// </summary>
        //bool SupportsBackBuffer { get; set; }
#endif
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

    #region IANIMATED-GIF-FRAME
    /// <summary>
    /// Represents an object which holds animeted GIF image information.
    /// </summary>
    public interface IAnimatedGifFrame
    {        /// <summary>
             /// Data of the image in byte array.
             /// </summary>
        byte[] Data { get; }

        /// <summary>
        /// Delay unit to be used to change a frame.
        /// </summary>
        int Delay { get; }
    }
    #endregion

    #region IBRUSH
    /// <summary>
    /// Represents a brush with certain fill style and gradient for drawin a shape on screen.
    /// </summary>
    public interface IBrush : IPen, ISettings, IDisposable, ICloneable2
#if Advanced
        , IResizable
#endif
    {
        BrushStyle Style { get; }
    }
    #endregion

    #region ITEXTURE-BRUSH
    public interface ITextureBrush : IPen, ISettings, IDisposable, ICloneable2, IPixels
#if Advanced
        , IResizable
#endif
    { }
    #endregion

    #region IBRUSHSOURCE
    public interface IBrushSource
    {
        /// <summary>
        /// Creates appropriate texture brush from this object.
        /// </summary>
        /// <param name="copyArea"></param>
        /// <returns></returns>
        ITextureBrush ToBrush(Rectangle? copyArea = null);
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

    #region IPENS
    public interface IPens : IObjDictionary<IReadable>, IAttachment
    {
        IReadable ToPen(IPenContext context, int? w = null, int? h = null);
    }
    #endregion

    #region IPOLYFILL
    /// <summary>
    /// Fills a polygon structure with specified PolyFill enum option.
    /// </summary>
    public interface IPolyFill : IPolyInfo, IDisposable
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
        PixelAction<float> ScanAction { get; }
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
        void Fill(FillAction<float> fillAction);
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
        void FillLine(ICollection<float> data, int axis, bool horizontal, FillAction<float> action, float? alpha = null);
        #endregion
    }
    #endregion

#endif
}

namespace MnM.GWS
{
#if Window && GWS
    #region IWINDOW-SURFACE
    public interface IWindowSurface : IWritable, IRenderTarget { }
    #endregion

    #region ITEXTURE
    public interface ITexture : ISize, IResizable, IDisposable
    {
        /// <summary>
        /// Copies portion of data specified by copyX, copyY, copyW, copyH parameters from a given memory block and 
        /// pastes it onto this texture at given loaction specified by dstX and dstY parameters.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dstX">Top Left x co-ordinate of destination on buffer</param>
        /// <param name="dstY">Top left y co-ordinate of destination on buffer</param>
        /// <param name="copyX">Top left x co-ordinate of area in source to cop.</param>
        /// <param name="copyY">Top left y co-ordinate of area in source to copy</param>
        /// <param name="copyW">Width of area in the source to copy.</param>
        /// <param name="copyH">Height of area in the source to copy</param>
        /// <param name="updateImmediate">If true, Update method will immediately called and screen will get updated otherwise not.</param>
        void CopyFrom(IBlockable source, int dstX, int dstY, int copyX, int copyY, int copyW, int copyH, bool updateImmediate = true);

        /// <summary>
        /// Uploads a portion of this texture specified by rectangle area to the screen.
        /// </summary>
        /// <param name="area"></param>
        void Upload(Rectangle area);
    }
    public interface ITexture2 : ITexture
    {
        Flip Flip { get; set; }
        BlendMode Mode { get; set; }
        byte Alpha { get; set; }
        int ColorMode { get; set; }
        void Bind();
        void Unbind();
    }
    #endregion
#endif
}
