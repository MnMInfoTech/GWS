/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

//Author: Manan Adhvaryu
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace MnM.GWS
{
    public abstract class _ObjCollection<T> : IObjCollection where T: IGraphics
    {
        #region VARIABLES
        protected readonly IGraphics Graphics;
        #endregion

        #region CONSTRUCTOR
        public _ObjCollection(T canvas)
        {
            Graphics = canvas;
        }
        #endregion

        #region PROPERTIES
        public ISettings this[IRenderable shape]
        {
            get
            {
                if (shape == null || shape.ID == 0)
                    return null;
                if (!Dict.ContainsKey(shape.ID))
                    return null;
                return Dict[shape.ID].Settings;
            }
        }
        public IRenderable this[uint shapeID]
        {
            get
            {
                if (shapeID == 0)
                    return null;
                if (!Dict.ContainsKey(shapeID))
                    return null;
                return Dict[shapeID].Renderable;
            }
        }
        public IRenderable this[string name]
        {
            get
            {
                if (name == null)
                    return null;
                var shape = Dict.Values.FirstOrDefault(p => p.Renderable.Name == name);
                if (shape == null)
                    return null;
                return shape.Renderable;
            }
        }
        public int Count =>
            Dict.Count;
        protected abstract Dictionary<uint, Shape> Dict {get;} 
        #endregion

        #region IS DRAWABLE
        protected virtual bool IsDrawable(ISettings item, ISettings compareWith) =>
            item.ShapeID == compareWith.ShapeID;
        #endregion

        #region IS ADDABLE
        protected virtual bool IsAddable(IRenderable shape) =>
            shape != null && shape.ID != 0;
        #endregion

        #region ADD
        public U Add<U>(U Shape, ISettings Settings, bool? suspendUpdate = null) where U : IRenderable
        {
            if (!IsAddable(Shape))
                return Shape;
            bool  AddMode = !Contains(Shape);

            if (AddMode)
            {
                var info = new Settings(Shape.ID, null);
                if (Settings != null)
                    info.Receive(Settings);

                Dict.Add(Shape.ID, new Shape(Shape, info));
                if (Shape is IChild)
                    ((IChild)Shape).Graphics = Graphics;
            }
            Settings = this[Shape];
            var OriginalCommand = Settings.Command;
            if (AddMode)
            {
                if (suspendUpdate == true)
                    Settings.Command |= Command.SuspendUpdate;
                else if (suspendUpdate == false)
                    Settings.Command &= ~Command.SuspendUpdate;
                Settings.Command |= Command.AddMode;
            }
            Graphics.Render(Shape, Settings);
            Settings.Command = OriginalCommand;
            return Shape;
        }
        public U Add<U>(U shape) where U : IRenderable =>
            Add(shape, null, null);
        public void AddRange<U>(IEnumerable<U> controls) where U : IRenderable
        {
            foreach (var item in controls)
                Add(item);
        }
        #endregion

        #region REMOVE
        public bool Remove(uint shapeID)
        {
            if (shapeID == 0)
                return false;
            bool ok = Dict.Remove(shapeID);
            if (ok)
                Graphics.Render(Dict.Values);
            return ok;
        }
        public bool Remove(IRenderable item)
        {
            if (item == null || item.ID == 0)
                return false;
            return Remove(item.ID);
        }
        public void RemoveAll()
        {
            var items = Query();
            foreach (var item in items)
                Remove(item);
        }
        #endregion

        #region CONTAINS
        public bool Contains(IRenderable item)
        {
            if (item == null || item.ID == 0)
                return false;
            return Dict.ContainsKey(item.ID);
        }
        public bool Contains(uint key)
        {
            if (key == 0)
                return false;
            return Dict.ContainsKey(key);
        }
        #endregion

        #region QUERY
        public virtual IEnumerable<IRenderable> Query(Predicate<ISettings> condition = null)
        {
            var fx = condition ?? (s => true);
            return Dict.Values.Where(x => x.Renderable.ID != 0 &&
                fx(x.Settings)).Select(x => x.Renderable);
        }
        public virtual IEnumerable<IShape> QueryDraw(Predicate<ISettings> condition = null)
        {
            var fx = condition ?? (s => true);
            return Dict.Values.Where(x => fx(x.Settings));
        }
        public virtual IRenderable QueryFirst(Predicate<ISettings> condition = null)
        {
            var fx = condition ?? (s => true);

            return Dict.Values.Where(x =>
                fx(x.Settings)).Select(x => x.Renderable).FirstOrDefault();
        }
        public virtual IShape QueryFirstDraw(Predicate<ISettings> condition = null)
        {
            var fx = condition ?? (s => true);
            return Dict.Values.Where(x =>
                fx(x.Settings)).FirstOrDefault();
        }
        #endregion

        #region ENUMERATOR
        public IEnumerator<IRenderable> GetEnumerator()
        {
            foreach (var item in Dict.Values)
                yield return item.Renderable;
        }
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion

        #region DISPOSE
        public void Dispose()
        {
            (Graphics as IClearable)?.Clear(0, 0, Graphics.Width, Graphics.Height);
            Dict.Clear();
        }
        #endregion
    }
}
