/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
#if GWS || Window
    #region IOFFSET
    public interface IOffset : ISettable
    {
        /// <summary>
        /// Gets X co-ordinate of the location this object.
        /// </summary>
        int X { get; }

        /// <summary>
        /// Gets Y co-ordinate of the location of this object.
        /// </summary>
        int Y { get; }
    }
    #endregion

    #region IDRAW-INFO - SETTINGS
    public interface IRecentlyDrawn
    {
        /// <summary>
        /// Gets recently drawn area since last rendering operation.
        /// </summary>
        Rectangle RecentlyDrawn { get; }
    }

    public interface IRecentlyDrawn2: IRecentlyDrawn
    {
        /// <summary>
        /// Gets recently drawn area since last rendering operation.
        /// </summary>
        new Rectangle RecentlyDrawn { get; set; }
    }

    public interface IShapeDrawInfo
    {
        /// <summary>
        /// Gets ID of current reader (IBufferPen) associated with current rendering process.
        /// </summary>
        string PenID { get; }

        /// <summary>
        /// Gets ID of current writer (IWriteableBlock) associated with current rendering process.
        /// </summary>
        string BufferID { get; }

        /// <summary>
        /// Gets ID of current shape associated with current rendering process.
        /// </summary>
        string ShapeID { get; }
    }

    public interface IShapeDrawInfo2: IShapeDrawInfo
    {
        /// <summary>
        /// Gets or sets ID of current reader (IBufferPen) associated with current rendering process.
        /// </summary>
        new string PenID { get; set; }
    }

    /// <summary>
    /// Represents an object which 
    /// </summary>
    public interface IDrawInfo : IRenderInfo, IOffset, IRotatable, IShapeDrawInfo, IRecentlyDrawn
    {
        /// <summary>
        /// Gets fill mode settings for this object.
        /// </summary> 
        FillMode FillMode { get; }

        /// <summary>
        /// Gets stroke mode settings for this object.
        /// </summary>
        StrokeMode StrokeMode { get; }

        /// <summary>
        /// Gets stroke value settings for this object.
        /// </summary>
        float Stroke { get; }

        /// <summary>
        /// Gets scale vector for scale transformation.
        /// </summary>
        VectorF Scale { get; }

        /// <summary>
        /// Gets the size of current buffer on which shape is being rendered.
        /// </summary>
        Size Clip { get; }

        /// <summary>
        /// Gets bounds of current shape associated with current rendering process.
        /// </summary>
        Rectangle Bounds { get; }
    }

    /// <summary>
    /// Reprsents an object which represents location and draw parameters information as well.
    /// It also facilitates modification of location and draw parameters.
    /// </summary>
    public interface IDrawSettings : IDrawInfo, IRenderInfo2, IShapeDrawInfo2, IForeground, IReadContext
    {
        /// <summary>
        /// Gets or sets fill mode settings for this object.
        /// </summary>
        new FillMode FillMode { get; set; }

        /// <summary>
        /// Gets or sets stroke mode settings for this object.
        /// </summary>
        new StrokeMode StrokeMode { get; set; }

        /// <summary>
        /// Gets or sets stroke value settings for this object.
        /// </summary>
        new float Stroke { get; set; }

        /// <summary>
        /// Gets or sets X co-ordinate of the location this object.
        /// </summary>
        new int X { get; set; }

        /// <summary>
        /// Gets or sets Y co-ordinate of the location of this object.
        /// </summary>
        new int Y { get; set; }

        /// <summary>
        /// Gets or sets rotation object to perform rotation transfromation this object.
        /// </summary>
        new Rotation Rotation { get; set; }

        /// <summary>
        /// Gets or sets scale vector for scalling transformation.
        /// </summary>
        new VectorF Scale { get; set; }

        /// <summary>
        /// Gets or sets the size of clipping for the current shape rendering.
        /// </summary>
        new Size Clip { get; set; }

        /// <summary>
        /// Gets or sets bounds of current shape associated with current rendering process.
        /// </summary>
        new Rectangle Bounds { get; set; }

        /// <summary>
        /// Gets or sets a flag which determines if settings should be flushed or not after each element rendering.
        /// </summary>
        bool FreezeSettings { get; set; }
       
        /// <summary>
        /// Flushes all settings and set them to default values.
        /// </summary>
        void Flush();
    }

    public interface IDrawSettings2 : IDrawSettings, IClippable
    { }
    #endregion

    #region IBASICDRAWIFO
    public interface IBasicDrawInfo
    {
        /// <summary>
        /// Gets command to apply on buffers while writing them for rendering a shape.
        /// </summary>
        DrawCommand DrawCommand { get; }

        /// <summary>
        /// Gets line draw settings for this object.
        /// </summary>
        LineCommand LineCommand { get; }
    }
    #endregion

    #region  IBASICDRAWINFO2
    public interface IBasicDrawInfo2: IBasicDrawInfo
    {
        /// <summary>
        /// Gets or sets command to apply on buffers while writing them for rendering a shape.
        /// </summary>
        new DrawCommand DrawCommand { get; set; }

        /// <summary>
        /// Gets or sets line draw settings for this object.
        /// </summary>
        new LineCommand LineCommand { get; set; }
    }
    #endregion
    
    #region IRENDERINFO
    public interface IRenderInfo : IID<int>, ISettable, ISettings, IBasicDrawInfo
    {
        /// <summary>
        /// Gets fill pattern settings for this object.
        /// </summary>
        FillCommand FillCommand { get; }


        /// <summary>
        /// Gets command to apply on brushes while reading them for rendering a shape.
        /// </summary>
        BrushCommand BrushCommand { get; }
    }
    #endregion

    #region IRENDERINFO2
    public interface IRenderInfo2 : IRenderInfo, IBasicDrawInfo2
    {
        /// <summary>
        /// Gets or sets fill command for this object.
        /// </summary>
        new FillCommand FillCommand { get; set; }

        /// <summary>
        /// Gets command to apply on brushes while reading them for rendering a shape.
        /// </summary>
        new BrushCommand BrushCommand { get; set; }


        /// <summary>
        /// Extract currently effective fill parameters from this object.
        /// </summary>
        /// <param name="CheckForCloseness"></param>
        /// <param name="LineOnly"></param>
        /// <param name="EndsOnly"></param>
        void GetFillParameters(out bool CheckForCloseness, out bool LineOnly, out bool EndsOnly);
    }
    #endregion

#if Advanced
    #region DRAWINFO2
    public interface IDrawInfo2 : IDrawInfo, IVisible, IRecentlyDrawn
    {
        /// <summary>
        /// Gets the tab index of the shape.
        /// </summary>
        int TabIndex { get; }

        /// <summary>
        /// Gets the zorder of the shape.
        /// </summary>
        int ZOrder { get; }

        /// <summary>
        /// Gets the page which the shape blelogs to.
        /// </summary>
        int Page { get; }
    }
    #endregion
#endif

    #region IDRAWNINFO
    public interface IDrawnInfo
    {
        IRenderable Shape { get; }
#if Advanced
        IDrawInfo2 Info { get; }
#else
        IDrawInfo Info { get; }
#endif
    }
    #endregion
#endif
}