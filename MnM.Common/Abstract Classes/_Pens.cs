/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;
using System.Collections.Generic;

namespace MnM.GWS
{
#if (GWS || Window)
    public abstract class _Pens : _ObjDictionary<IReadable, string>, IPens
    {
        readonly Dictionary<string, IReadable> items =
            new Dictionary<string, IReadable>(4);

        public sealed override IReadable this[string key]
        {
            get
            {
                if (key == null || !items.ContainsKey(key))
                    return null;
                return items[key];
            }
        }
        protected sealed override IEnumerable<IReadable> objects =>
            items.Values;

        public sealed override bool Contains(string key)
        {
            if (key == null)
                return false;
            return items.ContainsKey(key);
        }
        public sealed override T Add<T>(T obj)
        {
            if (obj == null)
                return default(T);
            if (Contains(obj))
                return obj;
            items.Add(obj.ID, obj);
            return obj;
        }
        public sealed override bool Remove(string id)
        {
            if (id == null)
                return false;
            return items.Remove(id);
        }

        public override void Dispose()
        {
            if (!IsDisposed)
                return;
            foreach (var item in objects)
                (item as IDisposable)?.Dispose();
        }
        public abstract IReadable ToPen(IPenContext context, int? w = null, int? h = null);
    }
#endif
}
