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

    #region IRECENTLYDRAWN
    public interface IRecentlyDrawn
    {
        /// <summary>
        /// Gets recently drawn area since last rendering operation.
        /// </summary>
        Rectangle RecentlyDrawn { get; set; }
    }
    #endregion

    #region IPOLYDRAW INFO
    public interface IPolyInfo
    {
        /// <summary>
        /// Gets or sets command to apply on buffers while writing them for rendering a shape.
        /// </summary>
        DrawCommand Command { get; set; }

        /// <summary>
        /// Gets the size of current buffer on which shape is being rendered.
        /// </summary>
        Size Clip { get; set; }
    }
    #endregion

    #region IRENDERINFO
    /// <summary>
    /// Reprsents an object which represents location and draw parameters information as well.
    /// It also facilitates modification of location and draw parameters.
    /// </summary>
    public interface IRenderInfo : IPolyInfo, IOffset, IRotatable, ISettings, IContext
    {
        /// <summary>
        /// Gets ID of current reader (IBufferPen) associated with current rendering process.
        /// </summary>
        string PenID { get; }

        /// <summary>
        /// Gets ID of current shape associated with current rendering process.
        /// </summary>
        string ShapeID { get; set; }

        /// <summary>
        /// Gets X co-ordinate of the location this object.
        /// </summary>
        new int X { get; set; }

        /// <summary>
        /// Gets Y co-ordinate of the location of this object.
        /// </summary>
        new int Y { get; set; }

        /// <summary>
        /// Gets or sets fill mode settings for this object.
        /// </summary> 
        FillMode FillMode { get; set; }

        /// <summary>
        /// Gets stroke mode settings for this object.
        /// </summary>
        StrokeMode StrokeMode { get; set; }

        /// <summary>
        /// Gets stroke value settings for this object.
        /// </summary>
        float Stroke { get; set; }

        /// <summary>
        /// Gets or sets Rotaion object for rotate, skew and transform operations.
        /// </summary>
        new Rotation Rotation { get; set; }

        /// <summary>
        /// Gets scale vector for scale transformation.
        /// </summary>
        VectorF Scale { get; set; }

        /// <summary>
        /// Gets bounds of current shape associated with current rendering process.
        /// </summary>
        Rectangle Bounds { get; set; }

        /// <summary>
        /// Gets or sets the supplied foreground context to be used for rendering.
        /// </summary>
        IPenContext Foreground { get; set; }

        /// <summary>
        /// Cleans existing draw command by removing run time temporary redering options.
        /// </summary>
        void CleanCommand();

        /// <summary>
        /// Adds multiple draw commands to this this object.
        /// </summary>
        /// <param name="commands"></param>
        void AddCommands(params DrawCommand[] commands);

        /// Removes multiple draw commands to this this object.
        void RemoveCommands(params DrawCommand[] commands);
    }
    #endregion

#if Advanced
    #region RENDERINFO2
    public interface IRenderInfo2 : IRenderInfo, IVisible//,IClippable
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
        IRenderInfo2 Info { get; }
#else
        IRenderInfo Info { get; }
#endif
    }
    #endregion
#endif
}