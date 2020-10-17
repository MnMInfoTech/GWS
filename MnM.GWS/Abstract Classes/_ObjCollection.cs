/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
    public abstract class _ObjCollection:
        _ObjDictionary<IRenderable, string>, IObjCollection
    {
        #region VARIABLES
        public readonly ISurface Parent;
        protected bool isDisposed;
        #endregion

        #region CONSTRUCTORS
        public _ObjCollection(ISurface buffer)
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
            IDrawInfo2
#else
            IDrawInfo
#endif
        this[IRenderable shape] => GetInfo(shape?.ID);
        public string ID { get; protected set; }
        public bool AddMode { get; protected set; }
        public abstract int Count { get; }
        public IEnumerable<IRenderable> Items => objects;
        ISurface IObjCollection<IRenderable>.Parent => Parent;
        public abstract
#if Advanced
            IEnumerable<IDrawInfo2>
#else
            IEnumerable<IDrawInfo>
#endif
            InfoItems
        { get; }
        #endregion

        #region IS DRAW POSSIBLE
        protected bool IsDrawPossible(IRenderable shape) =>
           Parent != null && Parent.Settings.ShapeID != shape.ID;
        #endregion

        #region ADD SHAPE
        public T Add<T>(T Shape, IReadContext context, int? drawX = null, int? drawY = null)
            where T : IRenderable
        {
            if (Shape == null)
                return Shape;

            AddMode = !Contains(Shape);
            if (AddMode)
            {
                var info = NewDrawInfo(Shape);
                AddInternal(Shape, info);

                if (Shape is IHostable)
                    ((IHostable)Shape).Assign(Parent);
            }
            Parent.Render(Shape, context, drawX, drawY);
            AddMode = false;
            return Shape;
        }
        public override T Add<T>(T Shape) 
        {
            if (Shape == null || Shape.ID == null)
                return Shape;
            AddMode = !Contains(Shape);

            if (AddMode)
            {
                var info = NewDrawInfo(Shape);
                AddInternal(Shape, info);
            }
            if(!(Shape is IShowable2))
                Parent.Render(Shape);
            AddMode = false;
            return Shape;
        }

#if Advanced
        protected abstract void AddInternal(IRenderable Shape, IDrawInfo2 info);
#else
        protected abstract void AddInternal(IRenderable Shape, IDrawInfo info);
#endif
        public void AddRange<T>(IEnumerable<T> controls) where T: IRenderable
        {
            foreach (var item in controls)
            {
                Add(item);
            }
        }
        #endregion

        #region DRAW
        public void Draw(IRenderable renderable)
        {
            if (!Contains(renderable))
                return;
            Parent.Render(renderable);
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
            IDrawInfo2
#else
        IDrawInfo
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
            IDrawInfo2
#else
            IDrawInfo
#endif
        newDrawInfo(IRenderable shape);
        public abstract
#if Advanced
            IDrawInfo2
#else
            IDrawInfo
#endif
            GetInfo(string Shape);

        public
#if Advanced
            IDrawInfo2
#else
            IDrawInfo
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
            IDrawInfo2
#else
            IDrawInfo
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
        public void Refresh(IRenderable shape)
        {
            if (Parent == null || !Contains(shape))
                return;

            Parent.Draw(shape);
            var info = GetInfo(shape.ID);
            SetCurrentPage(info, true);

            foreach (var item in Items)
            {
                var i = GetInfo(item.ID);
                if (i == null)
                    continue;
                if (IsDrawable(i, info))
                    Parent.Draw(item);
            }
        }
        protected abstract void SetCurrentPage(
#if Advanced
            IDrawInfo2
#else
        IDrawInfo
#endif
            info, bool silent);
        protected abstract bool IsDrawable(
#if Advanced
            IDrawInfo2
#else
        IDrawInfo
#endif
            item,
#if Advanced
            IDrawInfo2
#else
        IDrawInfo
#endif
            compareWith);
        #endregion

        #region CLEAR 
        public abstract void Clear();
        #endregion

        #region QUERY
        public abstract IEnumerable<T> Query<T>(Predicate<
#if Advanced
            IDrawInfo2
#else
            IDrawInfo
#endif
            > condition) where T : IRenderable;
        public abstract IList<IDrawnInfo> QueryDraw(Predicate<
#if Advanced
            IDrawInfo2
#else
            IDrawInfo
#endif
            > condition);

        public abstract T QueryFirst<T>(Predicate<
#if Advanced
            IDrawInfo2
#else
            IDrawInfo
#endif
            > condition) where T : IRenderable;
        public abstract IDrawnInfo QueryFirstDraw(Predicate<
#if Advanced
            IDrawInfo2
#else
            IDrawInfo
#endif
            > condition);
        #endregion

        #region ADVANCED VERSION IMPLEMENTATION
#if Advanced
        public virtual int PageCount { get; protected set; }
        public virtual int CurrentPage { get; protected set; }
        public MouseDrag MouseDrag { get; protected set; }
        public IEventPusher ActiveObject { get; protected set; }
        public IRenderable HoveredItem { get; protected set; }
        public IRenderable DraggedItem { get; protected set; }
        public virtual Vector DragLocation { get; protected set; }

        public abstract void ShowAll();
        public abstract void PushEvent(IEventInfo e);
        public abstract event EventHandler<IEventInfo> EventPushed;
        public abstract void SetCurrentPage(int page, bool silent = false);
        public abstract void SetPages(int noOfPages);
        public abstract void Move(IRenderable shape, int? drawX = null, int? drawY = null);
        public abstract void Resize(IRenderable shape, float size, bool clear = false);
        public abstract void Resize(IRenderable shape, float width, float height, bool clear = false);
        public abstract bool Focus(IRenderable shape);
        public abstract void BringToFront(IRenderable shape);
        public abstract void SendToBack(IRenderable shape);
        public abstract void Enable(IRenderable shape);
        public abstract void Disable(IRenderable shape);
        public abstract void Show(IRenderable shape);
        public abstract void Hide(IRenderable shape);
        public abstract void HideAll();
        public abstract bool MoveToPage(IRenderable shape, int pageNumber);
        public abstract int ChangeZOrder(IRenderable shape, int newOrder);
        public abstract int ChangeTabIndex(IFocusable shape, int newTabIndex);
        public abstract event EventHandler<IEventArgs> PageChanged;

#endif
        #endregion
    }
}
