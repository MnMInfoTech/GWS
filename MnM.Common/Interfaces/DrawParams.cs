/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS
{
    #region IDRAWPARAM
    /// <summary>
    /// Marker interface - represents any object which has settings.
    /// </summary>
    public interface IDrawParams
    { }
    #endregion

#if GWS || Window

    #region IRECENTLYDRAWN
    public interface IRecentlyDrawn
    {
        /// <summary>
        /// Gets recently drawn area since last rendering operation.
        /// </summary>
        IBoundary RecentlyDrawn { get; }
    }
    #endregion

    #region IPOLYDRAW INFO
    public interface IPolyInfo: IDrawParams
    {
        /// <summary>
        /// Gets or sets command to apply on buffers while writing them for rendering a shape.
        /// </summary>
        Command Command { get; set; }

        /// <summary>
        /// Gets the size of current buffer on which shape is being rendered.
        /// </summary>
        Size Clip { get; set; }
    }
    #endregion

    #region SETTINGS0
    public interface ISettings0: IPolyInfo, IRecentlyDrawn
    {
        /// <summary>
        /// Gets or sets an ID of current shape associated with current rendering process.
        /// </summary>
        string ShapeID { get; }
    }
    #endregion

    #region ISETTINGS
    /// <summary>
    /// Reprsents an object which represents location and draw parameters information as well.
    /// It also facilitates modification of location and draw parameters.
    /// </summary>
    public partial interface ISettings : ISettings0, ISettingsReceiver, IPoint, IBounds, IRotatable
    {  
        /// <summary>
        /// Gets or sets an ID of current shape associated with current rendering process.
        /// </summary>
        new string ShapeID { get; set; }

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
        /// Gets or sets bounds of current shape associated with current rendering process.
        /// </summary>
        new IRectangle Bounds { get; set; }

        /// <summary>
        /// Gets or sets the supplied foreground context to be used for rendering.
        /// </summary>
        IPenContext PenContext { get; set; }

        ///// <summary>
        ///// Gets pe created using current PenContext.
        ///// </summary>
        //IReadable Pen { get; }

        /// <summary>
        /// Cleans existing draw command by removing run time temporary redering options.
        /// </summary>
        void CleanCommand();

        /// <summary>
        /// Flushes all parameters and bring this object to default state.
        /// </summary>
        void Flush();
    }
    #endregion
#endif
}