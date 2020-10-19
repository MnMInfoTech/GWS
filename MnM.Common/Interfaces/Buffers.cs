/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.IO;

namespace MnM.GWS
{
#if (GWS || Window)
    #region IREADABLE
    public interface IReadable : IID, ISize, IReadContext
    {
        /// <summary>
        /// Reads a pixel after applying applying offset and rotation transformation (if exists) to get the correct co-ordinate.
        /// </summary>
        /// <param name="x">X co-ordinate of the location to read pixel from.</param>
        /// <param name="y">Y co-ordinate of the location to read pixel from.</param>
        /// <returns>Pixel value.</returns>
        int ReadPixel(int x, int y);

        /// <summary>
        /// Reads an axial line after applying applying offset and rotation transformation (if exists).
        /// </summary>
        /// <param name="start">Start of an axial line to read from this object - X co-ordinate if horizontal otherwise Y co-ordinate.</param>
        /// <param name="end">End of an axial line to read from this object - Y co-ordinate if not horizontal otherwise X co-ordinate.</param>
        /// <param name="axis">Axis value of line to read from this object -  Y co-ordinate if horizontal otherwise X co-ordinate.</param>
        /// <param name="horizontal">Direction of axial line if true then horizontal otherwise vertiacal.</param>
        /// <param name="pixels">Resultant memory block.</param>
        /// <param name="srcIndex">Location in the resultant memory block from where reading shoud start.</param>
        /// <param name="length">Length up to which the block should be read.</param>
        void ReadLine(int start, int end, int axis, bool horizontal, out int[] pixels, out int srcIndex, out int length);
    }
    #endregion

    #region IWRITEABLE
    public interface IWritable : IID, ISize, IDrawController, IInvalidatable, IForeground, IDisposed, ICloneable
    {
        #region PROPERTIES
        /// <summary>
        /// Length of this memory block.
        /// </summary>
        int Length { get; }
        #endregion

        #region WRITE PIXEL
        /// <summary>
        /// Writes pixel to this block at given axial position using specified color.
        /// </summary>
        /// <param name="val">Position on axis - X cordinate if horizontal otherwise Y.</param>
        /// <param name="axis">Position of axis -Y cordinate if horizontal otherwise X.</param>
        /// <param name="horizontal">Axis orientation - horizontal if true otherwise vertical.</param>
        /// <param name="color">Color to write at given location.</param>
        ///<param name="Alpha">Value by which blending should happen if at all it is supplied.</param>
        void WritePixel(int val, int axis, bool horizontal, int color, float? Alpha);

        /// <summary>
        /// Writes pixel to this block at given axial position using specified color.
        /// </summary>
        /// <param name="val">Position on axis - X cordinate if horizontal otherwise Y.</param>
        /// <param name="axis">Position on axis - X cordinate if horizontal otherwise Y.</param>
        /// <param name="horizontal">xis orientation - horizontal if true otherwise vertical.</param>
        /// <param name="color">colour of pixel.</param>
        void WritePixel(float val, int axis, bool horizontal, int color);
        #endregion

        #region WRITE LINE
        /// <summary>
        /// Writes line to the this block at given position specified by x and y parameters by reading specified source
        /// starting from give source index upto the length specified.
        /// </summary>
        /// <param name="source">Source memory block to copy data from.</param>
        /// <param name="srcIndex">Location in source memory block from which copy should begin.</param>
        /// <param name="srcW">Width of source memory block.</param>
        /// <param name="length">Length up to which source should be read for writing.</param>
        /// <param name="horizontal"></param>
        /// <param name="x">X co-ordinate of the location where writing begins.</param>
        /// <param name="y">Y co-ordinate of the location where writing begins.</param>
        ///<param name="Alpha">Value by which blending should happen if at all it is supplied</param>
        unsafe void WriteLine(int* source, int srcIndex, int srcW, int length, bool horizontal, int x, int y, float? Alpha);

        /// <summary>
        /// Writes an axial line (either horizontal or vertical) to this object using specified parameters
        /// </summary>
        /// <param name="start">Start position of reading from buffer pen on axis i.e X co-ordinate if horizontal otherwise Y</param>
        /// <param name="end">>End position of reading from buffer pen on axis i.e X co-ordinate if horizontal otherwise Y</param>
        /// <param name="axis">Position of reading from buffer pen on axis i.e Y co-ordinate if horizontal otherwise X</param>
        /// <param name="horizontal">Axis orientation - horizontal if true otherwise vertical</param>
        /// <param name="pen">buffer pen which to read pixel from</param>
        ///<param name="Alpha">Value by which blending should happen if at all it is supplied</param>
        void WriteLine(float start, float end, int axis, bool horizontal, IReadable pen, float? Alpha);
        #endregion
    }
    #endregion

    #region ISURFACE
    public interface ISurface : IWritable, ICopyable, IScalable, IBackground, IClearable, IUpdatable
#if Advanced
        , IRenderTarget
#endif
    {
        new int Length { get; }

#if Advanced
        IObjectDraw ObjectDraw { get; }

        /// <summary>
        /// Sets Source Alpha values to be read while copying image source.
        /// </summary>
        unsafe byte* SourceAlphas { set; }

        /// <summary>
        /// Finds an element from this collection if it exists on a given x and y coordinates.
        /// the test is applied on a last drawn area rather than an actual area of each element so if an element is not drawn yet, 
        /// it can not be found!
        /// </summary>
        /// <param name="x">X coordinate to search for</param>
        /// <param name="y">Y coordinate to search for</param>
        /// <returns></returns>
        IRenderable FindElement(int x, int y);

        /// <summary>
        /// Draws focus rectangle i.e. border around specified with dotted invert colors
        /// </summary>
        /// <param name="rectangle">Rectangle to draw focus rectangle around.</param>
        void DrawFocusRect(Rectangle rc);
#endif

        #region BEGIN - END
        /// <summary>
        /// Tells this object to create a rendering session accroding to existing settings before rendering.
        /// </summary>
        /// <param name="renderable">Shape to render on this object</param>
        /// <param name="pen">Appropriate pen if exists to be returned.</param>
        void Begin(IRenderable renderable, out IPen pen);

        /// <summary>
        /// Tells this object to end the rendering session and to finalize settings.
        /// </summary>
        /// <param name="pen">Pen used in rendering a shape.</param>
        void End(IPen pen);
        #endregion

    }
    #endregion

    #region ICANVAS
    public interface ICanvas : ISurface, IContainer, IResizable, IRefreshable, IDisposable
    {
#if Advanced
        IFocusRect FocusRect { get; }
#endif
    }
    #endregion

    #region IREADCONTEXT
    /// <summary>
    /// This is a marker interface which represents an object which can be converted to a buffer pen.
    /// </summary>
    public interface IReadContext
    {
    }
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

    #region IRENDERTARGET
    /// <summary>
    /// Represents an object which has a capability to receive data from copyable source object.
    /// </summary>
    public interface IRenderTarget : ISize, IDisposed
    {
        /// <summary>
        /// Uploads a data block specified by x, y, width and height parameters for update to IUpdateable object.
        /// </summary>
        /// <param name="source">Source from which data to be uploaded.</param>
        /// <param name="srcX">X co-ordinate of source area to upload.</param>
        /// <param name="srcY">Y co-ordinate of source area to upload.</param>
        /// <param name="srcW">Width of source area to upload.</param>
        /// <param name="srcH">Height of source area to upload.</param>
        void CopyFrom(ICopyable source, int srcX, int srcY, int srcW, int srcH);

        /// <summary>
        /// Uploads a data block specified by x, y, width and height parameters for update to IUpdateable object at given destination.
        /// </summary>
        /// <param name="source">Source from which data to be uploaded.</param>
        /// <param name="dstX"></param>
        /// <param name="dstY"></param>
        /// <param name="srcX">X co-ordinate of source area to upload.</param>
        /// <param name="srcY">Y co-ordinate of source area to upload.</param>
        /// <param name="srcW">Width of source area to upload.</param>
        /// <param name="srcH">Height of source area to upload.</param>
        void CopyFrom(ICopyable source, int dstX, int dstY, int srcX, int srcY, int srcW, int srcH);
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

    #region IBACKGROUND
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
