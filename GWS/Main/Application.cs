/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if (GWS || WIndow)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    partial class Application
    {
        #region VARIABLES
        static Dictionary<string, IEventProcessor> Windows = new Dictionary<string, IEventProcessor>(4);
        internal static IEventPoller EventWindow;
        #endregion

        #region PROPERTIES
#if Window
        public static bool IsWindowsRunning => 
            (status & LoopStatus.IsRunning) == LoopStatus.IsRunning && Windows.Count > 0;
#endif
        public static IRenderWindow MouseWindow
        {
            get
            {
                return GetWindow<IRenderWindow>((w) => (w.Flags & GwsWindowFlags.HostingMouse) == GwsWindowFlags.HostingMouse);
            }
        }
        #endregion

        #region REGISTER - DE-REGISTER WINDOW
        internal static void Register(this IEventProcessor window, IEventPoller oSWindow)
        {
            if (window == null)
                return;
            if (Windows.ContainsKey(window.ID))
                return;
            Windows.Add(window.ID, window);
            if (EventWindow == null)
                EventWindow = oSWindow;
        }
        internal static void Deregister(this IEventProcessor window)
        {
            Windows.Remove(window.ID + "");
            if (Windows.Count == 0)
                EventWindow = null;
        }
        #endregion

        #region GET WINDOW
        internal static bool GetWindow<T>(string name, out T window) where T : IID<string>
        {
            window = default(T);

            if (!Windows.TryGetValue(name + "", out IEventProcessor ep))
                return false;

            if (ep is T)
            {
                window = (T)ep;
                return true;
            }
            return false;
        }
        internal static T GetWindow<T>(Predicate<T> condition) where T : IID<string>
        {
            return Windows.Values.OfType<T>().FirstOrDefault(x => condition(x));
        }
        #endregion

        #region EXISTS
        internal static bool Exists(this IEventProcessor window) =>
            Windows.ContainsKey(window.ID + "");
        #endregion

        #region GET WINDOWS
        internal static IEnumerable<T> GetWindows<T>(Predicate<T> condition) where T : IID<string>
        {
            return Windows.Values.OfType<T>().Where(x => condition(x));
        }
        #endregion

        #region POLL EVENT
#if Window
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool PollEvent(out IExternalEventInfo e, out IEventProcessor window, out bool windowFound)
        {
            e = null;
            window = null;
            windowFound = false;
            if ((status & LoopStatus.IsRunning) != LoopStatus.IsRunning || Windows.Count == 0 || EventWindow == null)
                return false;
            if (EventWindow.PollEvent(out e) &&
                   GetWindow(e.ID, out window))
            {
               windowFound = true;
            }
            return true;
        }
#endif
        #endregion

        #region RUN
        public static unsafe void Run(params IShowable[] controls)
        {
            if ((status & LoopStatus.IsRunning) == LoopStatus.IsRunning)
                return;
            if (controls.Length > 0)
            {
                foreach (var item in controls)
                    item.Show();
            }
            status |= LoopStatus.IsRunning;

#if Window

            while ((status & LoopStatus.IsRunning) == LoopStatus.IsRunning && Windows.Count > 0 && EventWindow != null)
            {
                if (EventWindow.PollEvent(out IExternalEventInfo e) && GetWindow(e.ID, out IEventProcessor window))
                {
                    if (!window.ProcessEvent(e))
                        continue;
                }
            }
            Quit();
#elif MS
            System.Windows.Forms.Application.Run();
            Quit();
#endif
        }
        #endregion
    }
}
#endif
