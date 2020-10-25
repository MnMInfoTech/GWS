/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.IO;

namespace MnM.GWS
{
#if (GWS || Window)
    #region IBLOCK
    /// <summary>
    /// Represents smallest writable and copiable memory block object.
    /// </summary>
    public interface IBlock : IWritable, ICopyable, IBackground
    { }
    #endregion

    #region IIMAGE
    public interface IImage : IBlock, IBasicDrawInfo2, ICloneable
    {
    }
    #endregion

    #region IBUFFER
    /// <summary>
    /// Represents smallest writable and copiable memory block object which can also render shapes.
    /// Settings property of this object controls the flow of writing and rendering data.
    /// </summary>
    public interface IBuffer : IBlock, IID, IDrawController, IDisposed, ICloneable
#if Advanced
        , IElementFinder, IObjectDrawer
#endif
    {
        /// <summary>
        /// Length of this memory block.
        /// </summary>
        new int Length { get; }

#if Advanced
        /// <summary>
        /// Sets Source Alpha values to be read while copying image source.
        /// </summary>
        unsafe byte* SourceAlphas { set; }
#endif
    }
    #endregion

    #region ISURFACE
    public interface ISurface : IBuffer, IScalable, IClearable, IUpdatable, IDisposable, IRenderSession
#if Advanced
        , ICopier
#endif
    { }
    #endregion

    #region ICANVAS
    public interface ICanvas : ISurface, IContainer, IResizable, IRefreshable, IDisposable
    { }
    #endregion

    #region IREADCONTEXT
    /// <summary>
    /// This is a marker interface which represents an object which can be converted to a buffer pen.
    /// </summary>
    public interface IReadContext
    { }
    #endregion

    #region IPEN
    /// <summary>
    /// Represents an object from which memory can be read.
    /// </summary>
    public interface IPen : IReadable, IID, ICopyable, ICloneable
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

    #region IBACKGROUND
    public interface IBackground
    {
        /// <summary>
        /// Gets or sets background for this object.
        /// </summary>
        IReadContext Background { get; set; }
    }
    #endregion

    #region IBACKGROUNDPEN
    public interface IBackgroundPen
    {
        /// <summary>
        /// Gets or sets background for this object.
        /// </summary>
        IPen BackgroundPen { get; }
    }
    #endregion

    #region IFOREGROUND
    public interface IForeground
    {
        /// <summary>
        /// Gets or sets foreground for this object.
        /// </summary>
        IReadContext Foreground { get; set; }
    }
    #endregion

    #region IBRUSH
    /// <summary>
    /// Represents a brush with certain fill style and gradient for drawin a shape on screen.
    /// </summary>
    public interface IBrush : IPen, ICopyable, ISettings, IDisposable, ICloneable2
#if Advanced
        , IResizable
#endif
    {
        BrushStyle Style { get; }
    }
    #endregion

    #region ITEXTURE-BRUSH
    public interface ITextureBrush : IPen, ICopyable, ISettings, IDisposable, ICloneable2
#if Advanced
        , IResizable
#endif
    {
        IntPtr Pixels { get; }
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
    public interface IPens : IObjDictionary<IPen>, IAttachment
    {
        IPen ToPen(IReadContext context, int? w = null, int? h = null);
    }
    #endregion
#endif
}

namespace MnM.GWS
{
#if Window && GWS
    #region IWINDOW-SURFACE
    public interface IWindowSurface : ICanvas, IRenderTarget { }
    #endregion

    #region ITEXTURE
    public interface ITexture : IRenderTarget, IDisposable, IResizable
    {
        bool IsPrimary { get; }
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
