using System;

namespace MnM.GWS
{
    #region IIMAGE PROCESS ARGS
    public interface IImageProcessArgs: IProcessArgs
    {
        /// <summary>
        /// 
        /// </summary>
        IntPtr ImageSource { get; }

        /// <summary>
        /// 
        /// </summary>
        int ImageWidth { get; }

        /// <summary>
        /// 
        /// </summary>
        int ImageHeight { get; }

        /// <summary>
        /// 
        /// </summary>
        IBounds ImageCopyArea { get; }
    }
    #endregion

    internal unsafe class ImageProcessArgs : ProcessArgs, IImageProcessArgs
    {
        internal IBounds ImageCopyArea;
        internal IntPtr ImageSource;
        internal int ImageWidth;
        internal int ImageHeight;
        IBounds IImageProcessArgs.ImageCopyArea => ImageCopyArea;
        IntPtr IImageProcessArgs.ImageSource => ImageSource;
        int IImageProcessArgs.ImageWidth => ImageWidth;
        int IImageProcessArgs.ImageHeight => ImageHeight;
    }
}
