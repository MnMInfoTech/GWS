/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if MS
using MnM.GWS.Desktop;

namespace MnM.GWS
{
    partial class NativeFactory
    {
        partial void newNativeTarget(ref INativeTarget target, int x, int y, int w, int h)
        {
            target = new MSTarget(x, y, w, h);
        }
    }
}
#endif
