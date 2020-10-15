/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

using System;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    public class Pointer : IIntPtr
    {
        GCHandle handle;
        public Pointer(object target)
        {
            handle = GCHandle.Alloc(target);
        }
        ~Pointer()
        {
            if (handle.IsAllocated)
                handle.Free();
        }

        public IntPtr Handle => 
            GCHandle.ToIntPtr(handle);

        public void Dispose()
        {
            if (handle.IsAllocated)
                handle.Free();
            GC.SuppressFinalize(this);
        }

        public T Instance<T>()=>
            ((T)handle.Target);
    }
}
