/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if Window && SDL
using System;
using System.IO;
using System.Runtime.InteropServices;

using MnM.GWS.SDL;

namespace MnM.GWS
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate void AudioCallback(IntPtr userdata, byte* stream, int len);

    abstract class SdlSoundBase : Sound
    {
        protected static readonly string[] devices = new string[0];
        protected static readonly string[] drivers = new string[0];
        protected static bool initialized;
        protected int currentDevice;
        const string libSDL = SdlAPI.libSDL;

        static SdlSoundBase()
        {
            try
            {
                drivers = new string[SdlAudio.GetNumAudioDrivers()];
                for (int i = 0; i < drivers.Length; ++i)
                {
                    var driver = SdlAudio.GetAudioDriver(i);
                    drivers[i] = driver;
                }

                SdlAudio.Init(SystemFlags.AUDIO);
                int count = SdlAudio.GetNumAudioDevices(0);
                devices = new string[count];
                for (int i = 0; i < count; ++i)
                {
                    devices[i] = SdlAudio.GetAudioDeviceName(i, 0);
                }
                initialized = true;
            }
            catch { }
        }

        public override void Pause()
        {
            SdlAudio.PauseAudioDevice(currentDevice, 1);
        }
        public override void Stop()
        {
            SdlAudio.CloseAudioDevice(currentDevice);
        }

        public override void Dispose()
        {
            if (!initialized)
                return;
            IsDisposed = true;
            if (currentDevice != 0)
                SdlAudio.CloseAudioDevice(currentDevice);
        }
        public override void Quit()
        {
            if (!initialized)
                return;
            QuitAudio();
        }

        #region STRUCTS
        [StructLayout(LayoutKind.Sequential)]
        protected struct SdlAudioSpec
        {
            public int freq;
            public int format; // SDL_AudioFormat
            public int channels;
            public int silence;
            public int samples;
            public int size;
            public AudioCallback callback;
            public IntPtr userdata; // void*
        }
        #endregion

        #region SDL BINDINGS
        [DllImport(libSDL, EntryPoint = "SDL_Init", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern int Init(SystemFlags flags);

        //SDL_AudioInit
        static int InitAudio(string driver_name) => audioInit(Operations.UTF8_ToNative(driver_name));
        [DllImport(libSDL, EntryPoint = "SDL_AudioInit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        protected static extern int audioInit(byte[] driver_name);

        //SDL_AudioQuit
        [DllImport(libSDL, EntryPoint = "SDL_AudioQuit", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        protected internal static extern void QuitAudio();
        //SDL_BuildAudioCVT

        //SDL_ClearQueuedAudio
        [DllImport(libSDL, EntryPoint = "SDL_ClearQueuedAudio", CallingConvention = CallingConvention.Cdecl)]
        protected static extern void ClearQueuedAudio(uint dev);

        //SDL_CloseAudioDevice
        [DllImport(libSDL, EntryPoint = "SDL_CloseAudioDevice", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        protected static extern void CloseAudioDevice(int dev);

        //SDL_ConvertAudio

        //SDL_DequeueAudio
        [DllImport(libSDL, EntryPoint = "SDL_DequeueAudio", CallingConvention = CallingConvention.Cdecl)]
        protected static extern uint DequeueAudio(uint dev, IntPtr IntPtr, uint len);

        //SDL_FreeWAV
        [DllImport(libSDL, EntryPoint = "SDL_FreeWAV", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        protected static extern void FreeWAV(IntPtr audio_buf);

        //SDL_GetAudioDeviceName
        protected static string GetAudioDeviceName(int index, int iscapture) =>
            Operations.UTF8_ToManaged(getAudioDeviceName(index, iscapture));

        [DllImport(libSDL, EntryPoint = "SDL_GetAudioDeviceName", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr getAudioDeviceName(int index, int iscapture);

        //SDL_GetAudioDeviceStatus
        [DllImport(libSDL, EntryPoint = "SDL_GetAudioDeviceStatus", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        internal static extern AudioStatus GetAudioDeviceStatus(uint dev);

        //SDL_GetAudioDriver
        internal static string GetAudioDriver(int index) => Operations.UTF8_ToManaged(getAudioDriver(index));
        [DllImport(libSDL, EntryPoint = "SDL_GetAudioDriver", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr getAudioDriver(int index);

        //SDL_GetAudioStatus
        [DllImport(libSDL, EntryPoint = "SDL_GetAudioStatus", CallingConvention = CallingConvention.Cdecl)]
        internal static extern AudioStatus GetAudioStatus();

        //SDL_GetCurrentAudioDriver
        protected static string GetCurrentAudioDriver() => Operations.UTF8_ToManaged(getCurrentAudioDriver());
        [DllImport(libSDL, EntryPoint = "SDL_GetCurrentAudioDriver", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr getCurrentAudioDriver();

        //SDL_GetNumAudioDevices
        [DllImport(libSDL, EntryPoint = "SDL_GetNumAudioDevices", CallingConvention = CallingConvention.Cdecl)]
        protected static extern int GetNumAudioDevices(int iscapture);

        //SDL_GetNumAudioDrivers
        [DllImport(libSDL, EntryPoint = "SDL_GetNumAudioDrivers", CallingConvention = CallingConvention.Cdecl)]
        protected static extern int GetNumAudioDrivers();

        //SDL_GetQueuedAudioSize
        [DllImport(libSDL, EntryPoint = "SDL_GetQueuedAudioSize", CallingConvention = CallingConvention.Cdecl)]
        protected static extern int GetQueuedAudioSize(int dev);

        //SDL_LoadWAV
        protected static SdlAudioSpec LoadWAV(string file, ref SdlAudioSpec spec, out IntPtr audio_buf, out int audio_len)
        {
            SdlAudioSpec result;
            IntPtr rwops = RWFromFile(file, "rb");
            IntPtr result_ptr = loadWAV_RW(
                rwops,
                1,
                ref spec,
                out audio_buf,
                out audio_len
            );
            result = result_ptr.ToObj<SdlAudioSpec>();
            return result;
        }

        //SDL_LoadWAV_RW
        [DllImport(libSDL, EntryPoint = "SDL_LoadWAV_RW", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr loadWAV_RW(IntPtr src, int freesrc, ref SdlAudioSpec spec, out IntPtr audio_buf, out int audio_len);

        //SDL_RWFromFile
        [DllImport(libSDL, EntryPoint = "SDL_RWFromFile", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        static extern IntPtr RWFromFile(string file, string mode);


        //SDL_LockAudioDevice
        [DllImport(libSDL, EntryPoint = "SDL_LockAudioDevice", CallingConvention = CallingConvention.Cdecl)]
        static extern void LockAudioDevice(uint dev);


        //SDL_MixAudioFormat
        [DllImport(libSDL, EntryPoint = "SDL_MixAudioFormat", CallingConvention = CallingConvention.Cdecl)]
        protected static extern void MixAudioFormat(
            [Out()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 3)]
                byte[] dst,
            [In()] [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 3)]
                byte[] src, ushort format, uint len, int volume);

        [DllImport(libSDL, EntryPoint = "SDL_MixAudioFormat", CallingConvention = CallingConvention.Cdecl)]
        internal unsafe static extern void MixAudioFormat(byte* dst, byte* src, int format, int len, int volume);

        //SDL_OpenAudioDevice
        protected static int OpenAudioDevice(string device, int iscapture,
            ref SdlAudioSpec desired, out SdlAudioSpec obtained, int allowed_changes) =>
            openAudioDevice(Operations.UTF8_ToNative(device), iscapture, ref desired, out obtained, allowed_changes);

        [DllImport(libSDL, EntryPoint = "SDL_OpenAudioDevice", CallingConvention = CallingConvention.Cdecl)]
        static extern int openAudioDevice(byte[] device, int iscapture,
            ref SdlAudioSpec desired, out SdlAudioSpec obtained, int allowed_changes);

        //SDL_PauseAudio
        [DllImport(libSDL, EntryPoint = "SDL_PauseAudio", CallingConvention = CallingConvention.Cdecl)]
        protected static extern void PauseAudio(int pause_on);

        //SDL_PauseAudioDevice
        [DllImport(libSDL, EntryPoint = "SDL_PauseAudioDevice", CallingConvention = CallingConvention.Cdecl)]
        protected static extern void PauseAudioDevice(int dev, int pause_on);

        //SDL_QueueAudio
        [DllImport(libSDL, EntryPoint = "SDL_QueueAudio", CallingConvention = CallingConvention.Cdecl)]
        protected static extern int QueueAudio(int dev, IntPtr audio, int len);

        [DllImport(libSDL, EntryPoint = "SDL_QueueAudio", CallingConvention = CallingConvention.Cdecl)]
        protected static extern int QueueAudio(int dev, byte[] audio, int len);

        //SDL_UnlockAudio
        [DllImport(libSDL, EntryPoint = "SDL_UnlockAudio", CallingConvention = CallingConvention.Cdecl)]
        protected static extern void UnlockAudio();

        //SDL_UnlockAudioDevice
        [DllImport(libSDL, EntryPoint = "SDL_UnlockAudioDevice", CallingConvention = CallingConvention.Cdecl)]
        protected static extern void UnlockAudioDevice(uint dev);

        [DllImport(libSDL, EntryPoint = "SDL_AudioStreamAvailable", CallingConvention = CallingConvention.Cdecl)]
        protected static extern int AudioStreamAvailable(IntPtr stream);

        [DllImport(libSDL, EntryPoint = "SDL_AudioStreamClear", CallingConvention = CallingConvention.Cdecl)]
        protected static extern void AudioStreamClear(IntPtr stream);


        [DllImport(libSDL, EntryPoint = "SDL_FreeAudioStream", CallingConvention = CallingConvention.Cdecl)]
        static extern void FreeAudioStream(IntPtr stream);

        [DllImport(libSDL, EntryPoint = "SDL_NewAudioStream", CallingConvention = CallingConvention.Cdecl)]
        protected static extern IntPtr NewAudioStream(ushort src_format, byte src_channels,
            int src_rate, ushort dst_format, byte dst_channels, int dst_rate);
        #endregion
    }

    unsafe sealed class SdlAudio : SdlSoundBase, IAudio
    {
        #region VARIABLES
        SdlAudioSpec Spec;
        static bool opened;
        static int[] formats;
        PrimitiveList<Input> PlayList;
        int currentIndex;

        #endregion

        #region CONSTRUCTORS
        public SdlAudio()
        {
            Volume = 64;
            PlayList = new PrimitiveList<Input>(10);
        }
        #endregion

        #region PROPERTIES
        public int SampleRate => Spec.samples;
        public int ChannelCount => Spec.channels;
        public int DeviceCount => devices?.Length ?? 0;
        public SoundStatus Status { get; private set; }
        public string[] Devices => devices;
        public int Volume { get; set; }
        #endregion

        #region METHODS
        public void Load(Stream fs)
        {
            var input = new Input();
            int headersize = 44;
            input.header = new byte[headersize];
            fs.Read(input.header, 0, headersize);
            int length = Convert.ToInt32(fs.Length - headersize);
            input.data = new byte[length];
            fs.Read(input.data, 0, length);
            input.pointer = 0;
            input.length = length;
            PlayList.Add(input);
        }
        public override void Load(string file)
        {
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                Load(fs);
            }
        }
        public override bool Play()
        {
            if (!initialized)
                return false;
            var input = PlayList[currentIndex];
            //Spec.callback = FillBuffer;
            for (int i = 0; i < formats.Length; i++)
            {
                Spec.format = formats[i];
                Spec.freq = input.Frequency;
                Spec.channels = input.Channels;
                Spec.silence = 255;
                Spec.samples = Convert.ToUInt16(Math.Pow(2, 15) - 1);
                Spec.size = 0;
                try
                {
                    currentDevice = SdlAudio.OpenAudioDevice(null, 0, ref Spec, out SdlAudioSpec obtained, (int)AudioAdjust.FORMAT_CHANGE);
                    if (currentDevice != 0)
                    {
                        Spec = obtained;
                        break;
                    }
                }
                catch { continue; }
            }
            opened = currentDevice != 0;
            if (!opened)
                return false;
            SdlAudio.QueueAudio(currentDevice, input.data, input.length);
            SdlAudio.PauseAudioDevice(currentDevice, 0);
            return Loop;
        }
        unsafe void FillBuffer(IntPtr usrdata, byte* stream, int len)
        {
            var input = PlayList[currentIndex];
            fixed (byte* src = input.data)
            {
                int* source = (int*)((IntPtr)src);
                int* dest = (int*)((IntPtr)stream);
                Blocks.Copy(source, input.pointer, dest, 0, len / 4);
            }
            input.pointer += len;
        }
        public override void Dispose()
        {
            base.Dispose();
            PlayList.Clear();
            //FreeWAV(buffer);
        }
        #endregion

        #region STRUCTS
        private class Input
        {
            public byte[] header;
            public byte[] data;
            public int pointer;
            public int length;

            public int Frequency
            {
                get
                {
                    int res = (header[24] << 0) + (header[25] << 8) + (header[26] << 16) + (header[27] << 24);
                    return res;
                }
            }
            public int Channels
            {
                get
                {
                    return Convert.ToInt32((header[22] << 0) + (header[23] << 8));
                }
            }
            public int Remaining
            {
                get
                {
                    return length - pointer;
                }
            }
        }
        #endregion
    }

    unsafe sealed class SdlSound : SdlSoundBase
    {
        #region VARIABLES
        private IntPtr Buffer;
        private int Length;
        SdlAudioSpec Spec;
        string file;
        static bool opened;
        #endregion

        #region CONSTRUCTORS
        public SdlSound() { }
        #endregion

        #region PROPERTIES
        public int SampleRate => Spec.samples;
        public int ChannelCount => Spec.channels;
        public int DeviceCount => devices?.Length ?? 0;
        public string[] Devices => devices;
        #endregion

        #region METHODS
        public override void Load(string file)
        {
            this.file = file;
            try
            {
                SdlAudio.LoadWAV(file, ref Spec, out Buffer, out Length);
                opened = true;
            }
            catch
            {
                opened = false;
            }
        }
        public override bool Play()
        {
            if (!opened)
                return false;
            try
            {
                currentDevice = SdlAudio.OpenAudioDevice(null, 0, ref Spec, out _, (int)AudioAdjust.FORMAT_CHANGE);
                SdlAudio.QueueAudio(currentDevice, Buffer, Length);
                SdlAudio.PauseAudioDevice(currentDevice, 0);
                return Loop;
            }
            catch { }
            return false;
        }
        public override void Dispose()
        {
            if (!initialized)
                return;
            SdlAudio.FreeWAV(Buffer);
        }
        #endregion
    }

}
#endif
