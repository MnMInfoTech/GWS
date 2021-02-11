/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if GWS || Window
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

    #region IPOLYDRAW INFO
    /// <summary>
    /// Represents an object which has editable command and size clipping.
    /// </summary>
    public interface IPolySettings : IDrawParams, ICommand
    {
        /// <summary>
        /// Gets or sets command to apply on buffers while writing them for rendering a shape.
        /// </summary>
        new Command Command { get; set; }

        /// <summary>
        /// Gets the size of current buffer on which shape is being rendered.
        /// </summary>
        Size Clip { get; set; }
    }
    #endregion

    #region SHAPEID
    /// <summary>
    /// Represents an object which has an information about shape.
    /// </summary>
    public interface IShapeID
    {
        /// <summary>
        /// Gets an ID of current shape associated with current rendering process.
        /// </summary>
        uint ShapeID { get; }
    }
    #endregion

    #region PROCESSID
    /// <summary>
    /// Represents an objet which has an information about current rendering process.
    /// Very important for handling multi-threaded parallel running rendering tasks.
    /// </summary>
    public interface IProcessID
    {
        /// <summary>
        /// Gets GWS assigned ID for the current process.
        /// </summary>
        int ProcessID { get; }
    }
    #endregion

    #region IDSTPOINT
    /// <summary>
    /// Represents an object which has an information about destination.
    /// </summary>
    public interface IDstPoint: IPoint
    {
        /// <summary>
        /// Gets or sets X co-ordinate of the draw location.
        /// </summary>
        new int X { get; set; }

        /// <summary>
        /// Gets or sets Y co-ordinate of the draw location.
        /// </summary>
        new int Y { get; set; }
    }
    #endregion

    #region ICOMMAND
    /// <summary>
    /// Represents an object which has command information for current processing.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets command to apply on buffers while writing them for rendering a shape.
        /// </summary>
        Command Command { get; }
    }
    #endregion

    #region IREADSESSION
    public interface IReadSession: ICloneable
    {
        /// <summary>
        /// Gets or sets option to read data from pen.
        /// </summary>
        ReadChoice Choice { get; set; }
    }
    #endregion

    #region SESSION
    /// <summary>
    /// Represents an object which has information about current rendering process and perimeter formation.
    /// </summary>
    public interface ISession : IShapeID, IBoundary, IDstPoint, IReadSession
    { }
    #endregion

    #region ISETTINGS
    /// <summary>
    /// Represents an object which exposes various settings meant for controling rendering process.
    /// It also facilitates modification of location and draw parameters.
    /// </summary>
    public partial interface ISettings : IDrawParams, IBounds, IRotatable,
        IPolySettings, ISettingsReceiver, IShapeID, IProcessID
    {
        /// <summary>
        /// 
        /// </summary>
        ISession Session { get; }

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

        /// <summary>
        /// Gets or sets an ID of current shape associated with current rendering process.
        /// </summary>
        new uint ShapeID { get; set; }

        /// <summary>
        /// Gets or sets GWS assigned ID for the current process.
        /// </summary>
        new int ProcessID { get; set; }
    }
    #endregion

    #region SETTINGS HOLDER
    /// <summary>
    /// Represents an object which holds public draw-settings.
    /// </summary>
    public interface ISettingsHolder
    {
        /// <summary>
        /// Gets modifiable draw-settings object this object holds.
        /// </summary>
        ISettings Settings { get; }
    }
    #endregion
}
#endif
