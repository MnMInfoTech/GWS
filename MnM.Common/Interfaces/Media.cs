/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if (Window)
    using System;
    using System.IO;
    /// <summary>
    /// Represents an object which facilitates audio broadcasting.
    /// </summary>
    public interface IAudio: ISound, IDisposable
    {
        /// <summary>
        /// Sample Rate to be used for processing.
        /// </summary>
        int SampleRate { get; }
        /// <summary>
        /// No of channels available for processing the audio.
        /// </summary>
        int ChannelCount { get; }
        /// <summary>
        /// Current status of audio file processing.
        /// </summary>
        SoundStatus Status { get; }
        /// <summary>
        /// Name of devices availble for audio processing.
        /// </summary>
        string[] Devices { get; }
        /// <summary>
        /// Numer of devices available for audio processing. 
        /// </summary>
        int DeviceCount { get; }

        //System.DateTime Duration { get; }
        //float Pitch { get; set; }
        //int Volume { get; set; }
        //Vector3 Position { get; set; }
        //DateTime PlayingOffset { get; set; }


        /// <summary>
        /// Loads a memory stream to play audio.
        /// </summary>
        /// <param name="stream">Stream to be loaded and played.</param>
        void Load(Stream stream);
    }
    /// <summary>
    /// Represnts a base object to offer sound generation capabilities of the current operating system.
    /// </summary>
    public interface ISound: IDisposable
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
#endif
}
