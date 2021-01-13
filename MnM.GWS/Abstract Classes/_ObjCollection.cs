/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
#if GWS || Window
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    public abstract partial class _ObjCollection : _ObjDictionary<IRenderable, string>, IObjCollection
    {
        #region VARIABLES
        protected readonly IImage Window;
        protected readonly IElementFinder Finder;
        protected readonly bool HasElementFinder;
        protected bool isDisposed;
        #endregion

        #region CONSTRUCTORS
        public _ObjCollection(IImage buffer)
        {
            Window = buffer;
            HasElementFinder = buffer is IElementFinder;
            if (HasElementFinder)
                Finder = (IElementFinder)buffer;
            ID = "ObjectCollection".NewID();
        }
        #endregion

        #region PROPERTIES
        protected override bool IsDisposed =>
            isDisposed;

        public ISettings this[IRenderable shape] => GetInfo(shape);
        public string ID { get; protected set; }
        public bool AddMode { get; protected set; }
        public abstract int Count { get; }
        public IEnumerable<IRenderable> Items => objects;
        public abstract IEnumerable<ISettings> InfoItems { get; }
        #endregion

        #region IS DRAW POSSIBLE
        protected bool IsDrawPossible(IRenderable shape) =>
           Window != null;
        #endregion

        #region IS ADDABLE
        protected virtual bool IsAddable(IRenderable shape) =>
            shape != null && shape.ID != null;
        #endregion

        #region ADD SHAPE
        public abstract T Add<T>(T Shape, ISettings settings, bool? suspendUpdate = null) where T : IRenderable;
        public sealed override T Add<T>(T Shape) =>
            Add<T>(Shape, null, true);
        public void AddRange<T>(IEnumerable<T> controls) where T : IRenderable
        {
            foreach (var item in controls)
            {
                Add(item);
            }
        }
        #endregion

        #region REMOVE ALL
        public virtual void RemoveAll()
        {
            Clear();
        }
        #endregion

        #region GET INFO
        public abstract ISettings GetInfo(IRenderable Shape);
        #endregion

        #region REFRESH
        public virtual void Refresh(IRenderable shape)
        {
            if (Window == null || !Contains(shape))
                return;
            var info = GetInfo(shape);

            Window.Render(shape, info);

            SetCurrentPage(info, true);

            foreach (var item in Items)
            {
                var i = GetInfo(item);
                if (i == null)
                    continue;
                if (IsDrawable(i, info))
                    Window.Render(item, info);
            }
        }
        #endregion

        #region SET CURRENT PAGE
        protected abstract void SetCurrentPage(ISettings info, bool silent);
        protected abstract bool IsDrawable(ISettings item, ISettings compareWith);
        #endregion

        #region CLEAR 
        public abstract void Clear();
        #endregion

        #region QUERY
        public abstract IEnumerable<IRenderable> Query(Predicate<ISettings> condition = null);
        public abstract IEnumerable<IShape> QueryDraw(Predicate<ISettings> condition = null);
        public abstract IRenderable QueryFirst(Predicate<ISettings> condition = null);
        public abstract IShape QueryFirstDraw(Predicate<ISettings> condition = null);
        #endregion
    }
}
#endif
