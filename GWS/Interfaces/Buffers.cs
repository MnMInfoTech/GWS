/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

#if (GWS || Window)
using System;
using System.Collections.Generic;
#if NoObjectLimit
using gint = System.Int32;
#else
using gint = System.UInt16;
#endif

namespace MnM.GWS
{
    #region IPARENT 
    /// <summary>
    /// Represents an object which represents visual rendering capabilities and offers minimum but sufficient gateway into GWS world. 
    /// </summary>
    public partial interface IParent : IRenderer, IRefreshable, IVisible, 
        IUpdatable, IClearable, IFocus, IPoint, IControls, ILayoutSupport
    { }
    #endregion

    #region IExPARENT
    internal partial interface IExParent : IParent, IExRenderer,
        IExUpdatable<ViewState, ModifyCommand>, IViewHolder<IExView>, IExControls
    { }
    #endregion

    #region IExPARENT HOLDER
    public interface IParentHolder
    {
        IParent Parent { get; }
    }
    #endregion

    #region IExPARENT HOLDER
    internal interface IExParentHolder : IParentHolder
    {
        new IExParent Parent { get; }
    }
    #endregion

    #region IEx VIEW HOLDER
    internal interface IViewHolder<TView>
        where TView : ISource<IntPtr>, IExViewState, IRenderer, IUpdatable, IExUpdatable<ViewState, ModifyCommand>
    {
        TView View { get; }
    }
    #endregion

    #region ICONTROL COLLECTION
    /// <summary>
    /// Object containing a collection of objects of type IRenderable.
    /// </summary>
    public partial interface IControlCollection : IObjectCollection<IControl, IControl>, INotToBeImplementedOutsideGWS
    {
        #region PROPERTIES
        /// <summary>
        /// Gets the number of objects in the collection.
        /// </summary>
        new int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        new IControl this[int index] { get; set; }

        /// <summary>
        /// Returns shape having the given id in this collection if it exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IControl this[string id] { get; }
        #endregion

        #region ADD
        /// <summary>
        /// Adds a given shape to this collection.
        /// </summary>
        /// <param name="shape">Shape to be added.</param>
        /// <param name="parameters">Optional settings or session parameters chosen by the user.</param>
        /// Usage: var triangle = This.Add(new Triangle(0, 0, 100,100, 0, 45), 3, new GwsFillMode(FillMode.Original), null).
        /// <returns>The shape added.</returns>
        IControl Add(IShape shape, params IParameter[] parameters);

        /// <summary>
        /// Adds a given shape to this collection.
        /// </summary>
        /// <param name="parameters">Optional settings or session parameters chosen by the user.</param>
        /// <param name="shape">Shape to be added.</param>
        /// Usage: var triangle = This.Add(new Triangle(0, 0, 100,100, 0, 45), 3, new GwsFillMode(FillMode.Original), null).
        /// <returns>The shape added.</returns>
        IControl Add(IEnumerable<IParameter> parameters, IShape shape);

        /// <summary>
        /// Adds a given shape to this collection.
        /// </summary>
        /// <param name="shape">Shape to be added.</param>
        /// <param name="parameters">Optional settings or session parameters chosen by the user.</param>
        /// Usage: var triangle = This.Add(new Triangle(0, 0, 100,100, 0, 45), 3, new GwsFillMode(FillMode.Original), null).
        /// <returns>The shape added.</returns>
        IControl Add(IControl shape, params IParameter[] parameters);

        /// <summary>
        /// Adds a given shape to this collection.
        /// </summary>
        /// <param name="parameters">Optional settings or session parameters chosen by the user.</param>
        /// <param name="shape">Shape to be added.</param>
        /// Usage: var triangle = This.Add(new Triangle(0, 0, 100,100, 0, 45), 3, new GwsFillMode(FillMode.Original), null).
        /// <returns>The shape added.</returns>
        IControl Add(IEnumerable<IParameter> parameters, IControl shape);
        #endregion

        #region INSERT
        /// <summary>
        /// Insert a given shape object to this collection on a specified position.
        /// </summary>
        /// <param name="shape">Shape to be added.</param>
        /// <param name="position">The position to insert the shape at.</param>
        /// <param name="params">Optional settings or session parameters chosen by the user.</param>
        /// Usage var triangle = This.Insert(new Triangle(0, 0, 100,100, 0, 45), 3, new GwsFillMode(FillMode.Original), null).
        /// <returns>Returns the same Shape object which is inserted.</returns>
        IControl Insert(int position, IShape shape, params IParameter[] @params);

        /// <summary>
        /// Insert a given shape object to this collection on a specified position.
        /// </summary>
        /// <param name="parameters">Optional settings or session parameters chosen by the user.</param>
        /// Usage var triangle = This.Insert(new Triangle(0, 0, 100,100, 0, 45), 3, new GwsFillMode(FillMode.Original), null).
        /// <param name="shape">Shape to be added.</param>
        /// <param name="position">The position to insert the shape at.</param>
        /// <returns>Returns the same Shape object which is inserted.</returns>
        IControl Insert(IEnumerable<IParameter> parameters, int position, IShape shape);

        /// <summary>
        /// Insert a given shape object to this collection on a specified position.
        /// </summary>
        /// <param name="shape">Shape to be added.</param>
        /// <param name="position">The position to insert the shape at.</param>
        /// <param name="params">Optional settings or session parameters chosen by the user.</param>
        /// Usage var triangle = This.Insert(new Triangle(0, 0, 100,100, 0, 45), 3, new GwsFillMode(FillMode.Original), null).
        /// <returns>Returns the same Shape object which is inserted.</returns>
        IControl Insert(int position, IControl shape, params IParameter[] @params);

        /// <summary>
        /// Insert a given shape object to this collection on a specified position.
        /// </summary>
        /// <param name="parameters">Optional settings or session parameters chosen by the user.</param>
        /// Usage var triangle = This.Insert(new Triangle(0, 0, 100,100, 0, 45), 3, new GwsFillMode(FillMode.Original), null).
        /// <param name="shape">Shape to be added.</param>
        /// <param name="position">The position to insert the shape at.</param>
        /// <returns>Returns the same Shape object which is inserted.</returns>
        IControl Insert(IEnumerable<IParameter> parameters, int position, IControl shape);
        #endregion

        #region ADD RANGE
        /// <summary>
        ///Adds the elements of the specified collection to the end of this collection.
        /// </summary>
        /// <param name="items">The collection whose elements should be added to the end of this collection.</param>
        void AddRange(IEnumerable<IShape> items);

        /// <summary>
        ///Adds the elements of the specified collection to the end of this collection.
        /// </summary>
        /// <param name="items">The collection whose elements should be added to the end of this collection.</param>
        void AddRange(params IShape[] items);
        #endregion

        #region REFRESH
        /// <summary>
        /// Refreshes shape with given id on screen.
        /// </summary>
        /// <param name="shapeID"></param>
        void Refresh(IControl widget);
        #endregion
    }
    #endregion

    #region IExCONTROL COLLECTION
    internal partial interface IExControlCollection : IControlCollection, IExObjectCollection<IControl, IExControl>
    {
        new IExControl this[int index] { get; set; }
    }
    #endregion

    #region IOBJECT FINDER
    public partial interface IObjectFinder : IObjectChecker, IExist<IObject>
    { }
    #endregion

    #region ICANVAS
    /// <summary>
    /// Represents writable and copiable memory block object which can also render shapes.
    /// </summary>
    public partial interface ICanvas : IImage, IUpdatable, ICopyable, IRotatableSource,
        IClearable, IEdgeExtractable, IDisposable, IRenderTargetHolder, ICloneable, ISizeHolder
    { }
    #endregion

    #region IEx-CANVAS
    unsafe internal partial interface IExCanvas : ICanvas, IExResizable, IExRenderer, IExUpdatable<ViewState, ModifyCommand>, IExtracter<IntPtr>
    { }
    #endregion

    #region IVIEW 
    public partial interface IView : IParent, IImageData, IUpdatable, ICopyable, IRotatableSource, IMemoryOccupier,
        IClearable, IEdgeExtractable, IDisposable, IRenderTargetHolder, ICursorManager, IObjectFinder
    { }
    #endregion

    #region IEx VIEW
    unsafe partial interface IExView : IExParent, IView, IExRenderWindowHolder, IExResizable, IExtracter<IntPtr>
    {
        int* Pixels { get; }
        int OriginalWidth { get; }
        int OriginalHeight { get; }
    }
    #endregion

    #region ICONTAINER
    public interface IControls  
    {
        /// <summary>
        /// Gets collection of shapes associated with this host.
        /// </summary>
        IControlCollection Controls { get; }
    }
    #endregion

    #region IEx-CONTAINER
    internal interface IExControls: IControls
    {
        /// <summary>
        /// Gets collection of shapes associated with this host.
        /// </summary>
        new IExControlCollection Controls { get; }
    }
    #endregion
}
#endif
