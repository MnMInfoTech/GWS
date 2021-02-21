using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MnM.GWS
{
    public abstract class _ProxyCollection<TItem, TSubItem>: IProxyCollection<TItem, TSubItem>
    {
        #region VARIABLES
        protected readonly Collection<TItem> Items;
        #endregion

        #region CONSTRUCTORS
        public _ProxyCollection()
        {
            Items = new Collection<TItem>();
        }
        public _ProxyCollection(int capacity)
        {
            Items = new Collection<TItem>(capacity);
        }
        public _ProxyCollection(IEnumerable<TSubItem> items)
        {
            Items = new Collection<TItem>(items.Select(x => GetItem(x)));
        }
        public _ProxyCollection(IEnumerable<TItem> items)
        {
            Items = new Collection<TItem>(items);
        }
        #endregion

        #region PROPERTIES
        public int Count => Items.Count;
        public bool IsReadOnly => Items.IsReadOnly;
        public TItem this[int index] => Items[index];
        #endregion

        #region ADD
        public virtual void Add(TSubItem subItem)
        {
            Items.Add(GetItem(subItem));
        }
        public virtual void AddRange(IEnumerable<TSubItem> subItem)
        {
            Items.AddRange(subItem.Select(x => GetItem(x)));
        }
        public virtual void AddRange(params TSubItem[] subItem)
        {
            Items.AddRange(subItem.Select(x => GetItem(x)));
        }
        #endregion

        #region CLEAR
        public virtual void Clear()
        {
            Items.Clear();
        }
        #endregion

        #region CONTAINS
        public bool Contains(TSubItem subItem)
        {
            return Items.Any(x => subItem.Equals(GetSubItem(x)));
        }
        public int IndexOf(TSubItem subItem)
        {
            return Items.IndexOf(x => subItem.Equals(GetSubItem(x)));
        }
        #endregion

        #region REMOVE
        public bool Remove(TSubItem subItem)
        {
            Remove(subItem, out int index);
            return index != -1;
        }
        public virtual void Remove(TSubItem subItem, out int index)
        {
            index = Items.IndexOf(x => subItem.Equals(GetSubItem(x)));
            if (index == -1)
                return;
            Items.RemoveAt(index);
        }
        #endregion

        #region ENUMERATOR
        public IEnumerator<TItem> GetEnumerator() =>
            Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
        #endregion

        #region CONVERT
        protected abstract TItem GetItem(TSubItem subItem);
        protected abstract TSubItem GetSubItem(TItem item);
        #endregion
    }
}
