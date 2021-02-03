/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

//Author: Manan Adhvaryu
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public abstract class _Canvas: _Image, ICanvas
    {
        #region VARIABLES
        /// <summary>
        /// 
        /// </summary>
        protected bool Invert;
        
        /// <summary>
        /// 
        /// </summary>
        protected bool UseTarget;

        /// <summary>
        /// 
        /// </summary>
        protected ReadChoice choice;

        /// <summary>
        /// 
        /// </summary>
        IPenContext penContext;

        /// <summary>
        /// 
        /// </summary>
        int[] PenData;
        #endregion

        #region CONSTRUCTOR
        public _Canvas(int w, int h) :
            base(w, h)
        { }
        #endregion

        #region PROPERTIES
        protected abstract Dictionary<uint, Shape> Items { get; }
        public int ObjectCount => Items.Count;
        public ISettings this[IRenderable shape]
        {
            get
            {
                if (shape == null || shape.ID == 0)
                    return null;
                if (!Items.ContainsKey(shape.ID))
                    return null;
                return Items[shape.ID].Settings;
            }
        }
        public IRenderable this[uint shapeID]
        {
            get
            {
                if (shapeID == 0)
                    return null;
                if (!Items.ContainsKey(shapeID))
                    return null;
                return Items[shapeID].Renderable;
            }
        }
        public IRenderable this[string name]
        {
            get
            {
                if (name == null)
                    return null;
                var shape = Items.Values.FirstOrDefault(p => p.Renderable.Name == name);
                if (shape == null)
                    return null;
                return shape.Renderable;
            }
        }
        protected sealed override unsafe int* Pen
        {
            get
            {
                if (PenData == null)
                    return null;
                fixed (int* p = PenData)
                    return p;
            }
        }
        public IPenContext Background
        {
            set
            {
                penContext = value;
                ChangeBackground(width, height);
                Clear(0, 0, width, height, Command.Backdrop);
            }
        }
        public sealed override string TypeName => "Canvas";
        public ReadChoice Choice
        {
            get => choice;
            set
            {
                choice = value;
                Invert = (choice & ReadChoice.InvertColor) == ReadChoice.InvertColor;
                UseTarget = (choice & ReadChoice.ScreenData) == ReadChoice.ScreenData;
            }
        }
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
            bool AddMode = !Contains(Shape);

            if (AddMode)
            {
                var info = newSettings(Shape.ID);
                if (Settings != null)
                    info.Receive(Settings);

                Items.Add(Shape.ID, new Shape(Shape, info));
                if (Shape is IChild)
                    ((IChild)Shape).Graphics = this;
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
            Render(Shape, Settings);
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

        #region NEW SETTINGS
        protected abstract Settings newSettings(uint shapeID);
        #endregion

        #region REMOVE
        public virtual bool Remove(uint shapeID)
        {
            if (shapeID == 0)
                return false;
            bool ok = Items.Remove(shapeID);
            if (ok)
                this.Render(Items.Values);
            return ok;
        }
        public bool Remove(IRenderable item)
        {
            if (item == null || item.ID == 0)
                return false;
            return Remove(item.ID);
        }
        public virtual void RemoveAll()
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
            return Items.ContainsKey(item.ID);
        }
        public bool Contains(uint key)
        {
            if (key == 0)
                return false;
            return Items.ContainsKey(key);
        }
        #endregion

        #region QUERY
        public virtual IEnumerable<IRenderable> Query(Predicate<ISettings> condition = null)
        {
            var fx = condition ?? (s => true);
            return Items.Values.Where(x => x.Renderable.ID != 0 &&
                fx(x.Settings)).Select(x => x.Renderable);
        }
        public virtual IEnumerable<IShape> QueryDraw(Predicate<ISettings> condition = null)
        {
            var fx = condition ?? (s => true);
            return Items.Values.Where(x => fx(x.Settings));
        }
        public virtual IRenderable QueryFirst(Predicate<ISettings> condition = null)
        {
            var fx = condition ?? (s => true);

            return Items.Values.Where(x =>
                fx(x.Settings)).Select(x => x.Renderable).FirstOrDefault();
        }
        public virtual IShape QueryFirstDraw(Predicate<ISettings> condition = null)
        {
            var fx = condition ?? (s => true);
            return Items.Values.Where(x =>
                fx(x.Settings)).FirstOrDefault();
        }
        #endregion

        #region ENUMERATOR
        public IEnumerator<IRenderable> GetEnumerator()
        {
            foreach (var item in Items.Values)
                yield return item.Renderable;
        }
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        #endregion

        #region READ PIXEL
        public unsafe int ReadPixel(int x, int y)
        {
            if (Length == 0)
                return 0;
            int i = x + y * Width;
            if (i >= Length)
                i = 0;
            var srcColor = UseTarget ? Screen(true)[i] : Pen[i];
            if (Invert)
                srcColor ^= Colors.Inversion;
            return srcColor;
        }
        #endregion

        #region READ LINE
        public unsafe void ReadLine(int start, int end, int axis, bool horizontal, out int[] pixels, out int srcIndex, out int length)
        {
            if (start > end)
            {
                int temp = end;
                end = start;
                start = end;
            }
            length = end - start;
            if (start < 0)
            {
                length += start;
                start = 0;
            }
            int srcCounter = horizontal ? 1 : Width;
            pixels = new int[length];
            int* dst;
            fixed (int* p = pixels)
                dst = p;
            int* src = UseTarget ? Screen(true) : Pen;
            srcIndex = start + axis * Width;
            int srcColor;

            for (int i = 0; i < length; i++)
            {
                srcColor = src[srcIndex];
                if (Invert)
                    srcColor ^= Colors.Inversion;
                dst[i] = srcColor;
                srcIndex += srcCounter;
            }
            srcIndex = 0;
        }
        #endregion

        #region REFRESH
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void Refresh(Command command = 0)
        {
            if (ObjectCount == 0)
                return;
            Clear(0, 0, Width, Height, command | Command.SuspendUpdate);
            this.Render(QueryDraw());
        }
        #endregion

        #region UPDATE
        public abstract void Update(Command command = Command.None, IRectangle boundary = null);
        #endregion

        #region BACK GROUND CHANGED
        protected unsafe void ChangeBackground(int w, int h)
        {
            if (penContext == null)
            {
                PenData = null;
                goto RaiseEvent;
            }
            int* pen = null;
            int len = w * h;
            PenData = new int[len];
            fixed (int* p = PenData)
                pen = p;
            var brush = penContext.ToPen(w, h);
            if (brush != null)
                brush.CopyTo((IntPtr)pen, len, w, 0, 0, new Rectangle(0, 0, w, h), Command.Opaque);
            RaiseEvent:
            BackgroundChanged?.Invoke(this, Factory.EmptyArgs);
        }
        public event EventHandler<IEventArgs> BackgroundChanged;
        #endregion
    }
}
