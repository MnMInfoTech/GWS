/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    public abstract partial class _ObjCollection: _ObjDictionary<IRenderable, string>, IObjCollection
    {
        #region VARIABLES
        protected readonly IWritable Parent;
        protected bool isDisposed;
        #endregion

        #region CONSTRUCTORS
        public _ObjCollection(IWritable buffer)
        {
            Parent = buffer;
            ID = "ObjectCollection".NewID();
        }
        #endregion

        #region PROPERTIES
        protected override bool IsDisposed =>
            isDisposed;

        public
#if Advanced
            IRenderInfo2
#else
            IRenderInfo
#endif
        this[IRenderable shape] => GetInfo(shape?.ID);
        public string ID { get; protected set; }
        public bool AddMode { get; protected set; }
        public abstract int Count { get; }
        public IEnumerable<IRenderable> Items => objects;
        public abstract
#if Advanced
            IEnumerable<IRenderInfo2>
#else
            IEnumerable<IRenderInfo>
#endif
            InfoItems
        { get; }
        #endregion

        #region IS DRAW POSSIBLE
        protected bool IsDrawPossible(IRenderable shape) =>
           Parent != null;
        #endregion

        #region IS ADDABLE
        protected virtual bool IsAddable(IRenderable shape) =>
            shape != null && shape.ID != null;
        #endregion

        #region ADD SHAPE
        public T Add<T>(T Shape, IContext context)
            where T : IRenderable
        {
            if (!IsAddable(Shape))
                return Shape;

            AddMode = !Contains(Shape);
            if (AddMode)
            {
                var info = NewDrawInfo(Shape);
                AddInternal(Shape, info);
            }

            Parent.Render(Shape, context);
            AddMode = false;
            return Shape;
        }
        public sealed override T Add<T>(T Shape) =>
            Add<T>(Shape, null);

#if Advanced
        protected abstract void AddInternal(IRenderable Shape, IRenderInfo2 info);
#else
        protected abstract void AddInternal(IRenderable Shape, IRenderInfo info);
#endif
        public void AddRange<T>(IEnumerable<T> controls) where T: IRenderable
        {
            foreach (var item in controls)
            {
                Add(item);
            }
        }
        #endregion

        #region REMOVE
        public sealed override bool Remove(string shapeID)
        {
            if (!Contains(shapeID))
                return false;
            var info = GetInfo(shapeID);
            if (!RemoveInternal(info))
                return false;
            return true;
        }
        protected abstract bool RemoveInternal
            (
#if Advanced
            IRenderInfo2
#else
        IRenderInfo
#endif
            info);
        #endregion

        #region REMOVE ALL
        public virtual void RemoveAll()
        {
            Clear();
        }
        #endregion

        #region NEW DRAWINFO
        protected abstract

#if Advanced
            IRenderInfo2
#else
            IRenderInfo
#endif
        newDrawInfo(IRenderable shape);
        public abstract
#if Advanced
            IRenderInfo2
#else
            IRenderInfo
#endif
            GetInfo(string Shape);

        public
#if Advanced
            IRenderInfo2
#else
            IRenderInfo
#endif
            NewDrawInfo(IRenderable Shape)
        {
            if (Contains(Shape.ID))
                return GetInfo(Shape.ID);
            if (Shape.ID == null)
                return null;
            var info = newDrawInfo(Shape);
            return info;
        }
        public
#if Advanced
            IRenderInfo2
#else
            IRenderInfo
#endif
        NewDrawInfo(string shapeID)
        {
            if (Contains(shapeID + ""))
                return GetInfo(shapeID);

            if (shapeID == null)
                return null;
            var shape = Get(shapeID);
            var info = newDrawInfo(shape);
            return info;
        }
        #endregion

        #region REFRESH
        public virtual void Refresh(IRenderable shape)
        {
            if (Parent == null || !Contains(shape))
                return;

            Parent.Render(shape);

            var info = GetInfo(shape.ID);
            SetCurrentPage(info, true);

            foreach (var item in Items)
            {
                var i = GetInfo(item.ID);
                if (i == null)
                    continue;
                if (IsDrawable(i, info))
                    Parent.Render(item);
            }
        }
        #endregion

        #region SET CURRENT PAGE
        protected abstract void SetCurrentPage(
#if Advanced
            IRenderInfo2
#else
        IRenderInfo
#endif
            info, bool silent);
        protected abstract bool IsDrawable(
#if Advanced
            IRenderInfo2
#else
        IRenderInfo
#endif
            item,
#if Advanced
            IRenderInfo2
#else
        IRenderInfo
#endif
            compareWith);
        #endregion

        #region CLEAR 
        public abstract void Clear();
        #endregion

        #region QUERY
        public abstract IEnumerable<T> Query<T>(Predicate<
#if Advanced
            IRenderInfo2
#else
            IRenderInfo
#endif
            > condition) where T : IRenderable;
        public abstract IList<IDrawnInfo> QueryDraw(Predicate<
#if Advanced
            IRenderInfo2
#else
            IRenderInfo
#endif
            > condition);

        public abstract T QueryFirst<T>(Predicate<
#if Advanced
            IRenderInfo2
#else
            IRenderInfo
#endif
            > condition) where T : IRenderable;
        public abstract IDrawnInfo QueryFirstDraw(Predicate<
#if Advanced
            IRenderInfo2
#else
            IRenderInfo
#endif
            > condition);
        #endregion
    }
}
