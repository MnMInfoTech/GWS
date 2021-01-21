/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
using System;
using System.Runtime.InteropServices;

namespace MnM.GWS
{
    public class Pointer : IIntPtr
    {
        GCHandle handle;
        IntPtr objPtr;
        public Pointer(object target)
        {
            handle = GCHandle.Alloc(target);
            objPtr = GCHandle.ToIntPtr(handle);
        }
        ~Pointer()
        {
            if (handle.IsAllocated)
                handle.Free();
        }

        public IntPtr Handle => objPtr;

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
