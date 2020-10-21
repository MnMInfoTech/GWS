/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System;

namespace MnM.GWS.Desktop
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

#if Window
            MnM.GWS.Application.Attach(SdlFactory.Instance);
#else
            MnM.GWS.Application.Attach(NativeFactory.Instance);
#endif
            System.Windows.Forms.Application.Run(Demo.Instance);
        }
    }
}
