/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)
using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    /// <summary>
    /// Represnts a base object to offer sound generation capabilities of the current operating system.
    /// </summary>
    public interface ISound : IDisposable
    {
        /// <summary>
        /// Specifies whether the sound should get repeated again and again after broadcasting is completed once.
        /// </summary>
        bool Loop { get; set; }

        /// Loads an audio file to play sound.
        /// <param name="file">File which is to load to play sound.</param>
        void Load(string file);
        /// <summary>
        /// Initiates the playing routine.
        /// </summary>
        /// <returns></returns>
        bool Play();
        /// <summary>
        /// Pause the current playback of sound.
        /// </summary>
        void Pause();
        /// <summary>
        /// compeletly stops the playback of sound.
        /// </summary>
        void Stop();
    }

#if DevSupport
    public
#else
    internal
#endif
    abstract class Sound : ISound
    {
        public bool Loop { get; set; }
        public abstract void Load(string file);
        public abstract bool Play();
        public bool IsDisposed { get; protected set; }

        public abstract void Pause();
        public abstract void Stop();

        public abstract void Dispose();
        public abstract void Quit();
    }
}
#endif
