using System;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    public class KeyCollection<TKey, TItem>: _KeyCollection<TKey, TItem>, IAddRange<TItem>, IArrayHolder<TItem>
    {
        Func<TItem, TKey> KeyGetter;

        #region CONSTRUCTORS
        public KeyCollection(Func<TItem, TKey> keyGetter)
        {
            KeyGetter = keyGetter;
        }
        public KeyCollection(int capacity, Func<TItem, TKey> keyGetter) :
            base(capacity)
        {
            KeyGetter = keyGetter;
        }
        #endregion

        #region PROPERTIES
        public TItem First => Data[0];
        public TItem Last => Data[Length - 1];
        public int ResizeUnit
        {
            get => resizeUnit;
            set => resizeUnit = value;
        }
        TItem[] IArrayHolder<TItem>.Data => Data;
        #endregion

        protected override TKey KeyOf(TItem item) => KeyGetter(item);

        #region COPY TO    
        public int CopyTo(ref TItem[] target, ISpan range, int arrayIndex)
        {
            if (target == null)
                target = new TItem[0];


            int len = target.Length; TItem[] array;

            if (arrayIndex >= 0 && arrayIndex <= target.Length)
            {
                int resize;

                if (this is IArrayHolder<TItem>)
                {
                    int start = 0;

                    array = (this as IArrayHolder<TItem>).Data;
                    var count = (this as IArrayHolder<TItem>).Count;

                    if (range != null)
                    {
                        range = new Span(range, count);
                        start = range.Start; count = range.Count;
                    }
                    if (arrayIndex + count > target.Length)
                    {
                        resize = arrayIndex + count - target.Length;
                        Array.Resize(ref target, target.Length + resize);
                    }
                    Array.Copy(array, start, target, arrayIndex, count);
                }
                else if (this is IList<TItem> || this is IReadOnlyList<TItem>)
                {
                    IList<TItem> list = this as IList<TItem>;

                    if (range != null)
                    {
                        Span r = new Span(range, list.Count);
                        if (arrayIndex + r.Count > target.Length)
                        {
                            resize = arrayIndex + r.Count - target.Length;
                            Array.Resize(ref target, target.Length + resize);
                        }
                        foreach (var i in r.ToEnumerable())
                        {
                            target[arrayIndex] = list[i]; arrayIndex++;
                        }
                    }
                    else
                    {
                        if (arrayIndex + list.Count > target.Length)
                        {
                            resize = arrayIndex + list.Count - target.Length;
                            Array.Resize(ref target, target.Length + resize);
                        }
                        for (int i = 0; i < list.Count; i++)
                        {
                            target[arrayIndex] = list[i]; arrayIndex++;
                        }
                    }
                }
                else if (this is IReadOnlyList<TItem>)
                {
                    IReadOnlyList<TItem> list = this as IReadOnlyList<TItem>;

                    if (range != null)
                    {
                        Span r = new Span(range, list.Count);
                        if (arrayIndex + r.Count > target.Length)
                        {
                            resize = arrayIndex + r.Count - target.Length;
                            Array.Resize(ref target, target.Length + resize);
                        }
                        foreach (var i in r.ToEnumerable())
                        {
                            target[arrayIndex] = list[i]; arrayIndex++;
                        }
                    }
                    else
                    {
                        if (arrayIndex + list.Count > target.Length)
                        {
                            resize = arrayIndex + list.Count - target.Length;
                            Array.Resize(ref target, target.Length + resize);
                        }
                        for (int i = 0; i < list.Count; i++)
                        {
                            target[arrayIndex] = list[i]; arrayIndex++;
                        }
                    }
                }
                else
                {
                    if (range == null)
                    {
                        array = Enumerable.ToArray(this);
                    }
                    else
                    {
                        array = Data.Skip(range.Start).Take(range.Count).ToArray();
                    }
                    resize = arrayIndex + array.Length - target.Length;
                    Array.Resize(ref target, target.Length + resize);
                    Array.Copy(array, 0, target, arrayIndex, array.Length);
                }
            }
            return target.Length - len;
        }
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            if (arrayIndex + Length > array.Length)
                Array.Resize(ref array, arrayIndex + Length);
            Array.Copy(Data, 0, array, arrayIndex, Length);
        }
        #endregion

        #region CONTAINS
        public override bool Exists(TKey key)
        {
            return base.Exists(key);
        }
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(TItem item)
        {
            if (item == null)
                return false;
            return KeyItems.ContainsKey(KeyOf(item));
        }
        #endregion
    }
}
