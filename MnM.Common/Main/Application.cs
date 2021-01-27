/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MnM.GWS
{
    public static partial class Application
    {
        #region VARIABLES - CONSTS
        #region CONSTS
        public const StringComparison NoCase = StringComparison.CurrentCultureIgnoreCase;
        public const string ImplementedInAdvanceVersionOnly = "Sorry this is only implemented  the Advanced version! For more information - visit www.mnminfotech.co.uk";
        public const byte True = 1;
        public const byte False = 0;
#if GWS || Window
        static readonly Dictionary<string, IEventProcessor> Windows = new Dictionary<string, IEventProcessor>(4);
#endif
        static volatile bool IsClosing;
        #endregion

        #region EVENT STORAGE
        static volatile bool EventsRunning = false;
        #endregion

        #region EXTERNAL LIBRARY NAMES
#if IPHONE
        public const string libSDL ="__Internal";
        public const string libTTF = "__Internal";
        public const string libFT = "__Internal";

#elif LINUX || ANDROID
        public const string libSDL = "libSDL2.so";
        public const string libTTF = "libSDL2_ttf.so";
        public const string libFT = "libfreetype-6.so";

#elif OSX
        public const string libSDL = "libSDL2.dylib";
        public const string libTTF = "libTTF.dylib";
        public const string libFT = "libfreetype-6.dylib";
#else
        public const string libSDL = "SDL2.dll";
        public const string libTTF = "SDL2_ttf.dll";
        public const string libFT = "libfreetype-6.dll";
        //internal const string libFT = "freetype.dll";
#endif
        #endregion
        #endregion

        #region CONSTRUCTORS
        static Application()
        {
        }
        #endregion

        #region ATTACH CONTEXT
        /// <summary>
        /// Attach context such as Drawing or Window Context to the Instance.
        /// Order of Attachment should be...
        /// First DrawingContext.
        /// Second WindowContext.
        /// Then rest of the contexts;
        /// </summary>
        /// <param name="attachment"></param>
        public static void Attach(IAttachment attachment)
        {
            if (attachment == null)
                return;
            if (attachment is IEvaluator)
            {
                Eval.Attach(attachment as IEvaluator);
                return;
            }
            else
            {
#if (GWS || Window)
                if (attachment is IFactory)
                {
                    Factory.Attach(attachment as IFactory);
                    return;
                }
#endif
            }
        }
        #endregion

        #region PROPERTIES
#if Window
        public static bool IsRunning => EventsRunning && Windows.Count > 0;

#endif
        #endregion

        #region DEBUG WRITELINE
        public static void Write(this string message) =>
            Debug.WriteLine(message);
        #endregion

#if GWS || Window
        #region REGISTER - DE-REGISTER WINDOW
        public static void Register(this IEventProcessor window)
        {
            if (window == null)
                return;
            if (Windows.ContainsKey(window.ID))
                return;
            Windows.Add(window.ID, window);
        }
        public static void Deregister(this IEventProcessor window)
        {
            Windows.Remove(window.ID + "");
        }
        public static bool GetWindow<T>(string name, out T window) where T : IEventProcessor
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
        public static bool Exists(this IEventProcessor window) =>
            Windows.ContainsKey(window.ID + "");
        public static T GetWindow<T>(Predicate<T> condition) where T : IEventProcessor
        {
            return Windows.Values.OfType<T>().FirstOrDefault(x => condition(x));
        }
        public static IEnumerable<T> GetWindows<T>(Predicate<T> condition) where T : IEventProcessor
        {
            return Windows.Values.OfType<T>().Where(x => condition(x));
        }
        #endregion
#endif
        #region RUN - QUIT - HALT - RESUME
        public static void Run(params IShowable[] controls)
        {
            if (EventsRunning)
                return;
            EventsRunning = true;

            if (controls.Length > 0)
            {
                foreach (var item in controls)
                    item.Show();
            }
#if Window

            while (EventsRunning && Windows.Count > 0)
            {
                if (Factory.IsDisposed)
                    break;

                IEvent e;
                while (Factory.PollEvent(out e))
                {
                    if (GetWindow(e.ID, out IEventProcessor window))
                    {
                        if (!window.ProcessEvent(e))
                            continue;
                    }
                }
            }
#elif MS
            System.Windows.Forms.Application.Run();
#endif
            Quit();
        }
        public static void Quit()
        {
            IsClosing = true;
            EventsRunning = false;
            Factory.Dispose();
#if GWS || Window
            foreach (var item in Windows.Values)
                item.Dispose();
            Windows.Clear();
#endif
            IsClosing = false;
        }
        #endregion
    }
}