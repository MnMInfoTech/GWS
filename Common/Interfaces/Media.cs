/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.
#if (Window)

namespace MnM.GWS
{
    using System;
    using System.IO;
    /// <summary>
    /// Represents an object which facilitates audio broadcasting.
    /// </summary>
    public interface IAudio : ISound, IDisposable
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
}
#endif
