/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

using System;
using System.IO;

namespace MnM.GWS
{
#if AllHidden
    partial class NativeFactory
    {
#else
    public
#endif
        sealed class StbImageProcessor : IImageProcessor
        {
            internal StbImageProcessor() { }
            public void Dispose()
            {

            }

            public Lot<byte[], int, int> Read(string path) =>
                STBImage.Processor.Read(path);

            public Lot<byte[], int, int> Read(Stream stream) =>
                STBImage.Processor.Read(stream);

            public Lot<byte[], int, int> Read(byte[] stream) =>
                STBImage.Processor.Read(stream);

            public void Write(IntPtr pixels, int width, int height, int len, int pitch, Stream dest, ImageFormat format, int quality = 50) =>
                STBImage.Processor.Write(pixels, width, height, len, pitch, dest, format, quality);
        }
#if AllHidden
    }
#endif
}
 