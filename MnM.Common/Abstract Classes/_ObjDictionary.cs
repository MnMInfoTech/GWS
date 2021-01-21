/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    public abstract class _ObjDictionary<TObj, TKey>: 
        IObjDictionary<TObj, TKey> where TObj: IID<TKey>
    {
        #region PROPERTIES
        public abstract TObj this[TKey key] { get; }
        protected abstract bool IsDisposed { get; }
        protected abstract IEnumerable<TObj> objects { get; }
        #endregion

        #region COUNT OF
        public virtual int CountOf<T>() where T : TObj
        {
            if (IsDisposed)
                return 0;
            return objects.OfType<T>().Count();
        }
        public virtual int CountOf<T>(Predicate<T> condition) where T : TObj
        {
            if (IsDisposed)
                return 0;
            return objects.OfType<T>().Where(x => condition(x)).Count();
        }
        #endregion

        #region CONTAINS
        public abstract bool Contains(TKey key);
        public bool Contains(TObj shape)
        {
            if (shape == null)
                return false;
            return Contains(shape.ID);
        }
        #endregion

        #region REPLACE
        public virtual void Replace(TObj obj)
        {
            if (Contains(obj.ID))
                Remove(obj.ID);
            Add(obj);
        }
        #endregion

        #region ADD
        public abstract T Add<T>(T obj) where T: TObj;
        #endregion

        #region REMOVE
        public bool Remove(TObj shape)
        {
            if (shape == null)
                return false;
            return Remove(shape.ID);
        }
        public abstract bool Remove(TKey id);
        #endregion

        #region GET
        public virtual TObj Get(TKey key)
        {
            if (!Contains(key))
                return default(TObj);

            return this[key];
        }
        public virtual T Get<T>(TKey key) where T : TObj
        {
            if (!Contains(key))
                return default(T);

            var o = this[key];

            if (o is T)
                return (T)o;

            return default(T);
        }
        public virtual bool Get<T>(TKey key, out T obj) where T : TObj
        {
            obj = default(T);

            if (!Contains(key))
                return false;

            var o = this[key];

            if (o is T)
            {
                obj = (T)o;
                return true;
            }
            return false;
        }
        public virtual bool Get(TKey key, out TObj obj)
        {
            obj = default(TObj);

            if (!Contains(key))
                return false;

            obj = this[key];
            return false;
        }
        public virtual T Get<T>(Predicate<T> condition) where T : TObj
        {
            if (IsDisposed)
                return default(T);
            return objects.OfType<T>().FirstOrDefault(x => condition(x));
        }
        public virtual TObj Get(Predicate<TObj> condition) 
        {
            if (IsDisposed)
                return default(TObj);
            return objects.FirstOrDefault(x => condition(x));
        }
        #endregion

        #region GET ALL
        public virtual IEnumerable<T> GetAll<T>(Predicate<T> condition) where T : TObj
        {
            if (IsDisposed)
                return new T[0];
            return objects.OfType<T>().Where(x => condition(x));
        }
        public virtual IEnumerable<TObj> GetAll(Predicate<TObj> condition) 
        {
            if (IsDisposed)
                return new TObj[0];
            return objects.Where(x => condition(x));
        }
        #endregion

        #region IENUMERATOR
        public IEnumerator<TObj> GetEnumerator()
        {
            foreach (var item in objects)
                yield return item;
        }
        IEnumerator IEnumerable.GetEnumerator() => 
            GetEnumerator();
        #endregion

        #region DISPOSE
        public abstract void Dispose();
        #endregion
    }
}
