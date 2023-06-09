using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MnM.GWS
{
    public static class ImageProcessing
    {
        const int Inversion = Colours.Inversion;

        #region ANIMATED GIF FRAME
        /// <summary>
        /// Load GIF from file.
        /// </summary>
        /// <param name="path">Path of file containing images.</param>
        /// <param name="x">Width of graphic</param>
        /// <param name="y">Height of graphic</param>
        /// <param name="comp">actual colour composition</param>
        /// <param name="requiredComposition">Required colour composition</param>
        /// <returns>IAnimatedGifFrame containing GIF data.</returns>
        public static IAnimatedGifFrame[] GifFromStream(string path,
            out int x, out int y, out int comp, int requiredComposition = 4)
        {
            IAnimatedGifFrame[] frames = null;
            using (Stream ms = File.Open(path, FileMode.Open))
            {
                frames = GifFromStream(ms, out x, out y, out comp, requiredComposition);
            }
            return (frames);
        }

        /// <summary>
        /// Load GIF from file.
        /// </summary>
        /// <param name="path">Path of file containing images.</param>
        /// <param name="requiredComposition">Required colour composition</param>
        /// <returns></returns>
        public static Tuple<IAnimatedGifFrame[], Vector, int> GifFromStream(string path, int requiredComposition = 4)
        {
            IAnimatedGifFrame[] frames = null;
            int x, y;
            int comp;
            using (Stream ms = File.Open(path, FileMode.Open))
            {
                frames = GifFromStream(ms, out x, out y, out comp, requiredComposition);
            }
            return Tuple.Create(frames, new Vector(x, y), comp);
        }
        /// <summary>
        /// Load GIF from file.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="x">Width of graphic</param>
        /// <param name="y">Height of graphic</param>
        /// <param name="comp">actual colour composition</param>
        /// <param name="requiredComposition">Required colour composition</param>
        /// <returns></returns>

        public static IAnimatedGifFrame[] GifFromStream(Stream stream, out int x, out int y, out int comp, int requiredComposition) =>
           Factory.ImageProcessor.ReadAnimatedGif(stream, out x, out y, out comp, requiredComposition);
        #endregion

        #region IMAGE READ
        /// <summary>
        /// Reads a file located on a given path on disk or network drive and provides a processed data to be used for creating memory buffer. 
        /// </summary>
        /// <param name="path">Path of a file located on disk or network drive</param>
        /// <returns>
        /// Pair.Item1 - data in bytes array
        /// Pair.Item2 - Width information.
        /// Pair.Item3 - Height information.
        /// </returns>
        public static Tuple<byte[], int, int> ReadImage(string path)
        {
            using (var stream = File.Open(path, FileMode.Open))
            {
                return Factory.ImageProcessor.Read(stream).Result;
            }
        }
        #endregion

        #region WRITE IMAGE
        /// <summary>
        /// Writes a given memory block to a file on a given path.
        /// </summary>
        /// <param name="image">Memory block to write to disk file</param>
        /// <param name="path">Path of a file to create and write dat to</param>
        /// <param name="format">Format of the targeted image file</param>
        /// <param name="quality">Resolution quality of the tageted image file</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void Write(this IImageProcessor writer, IImageContext image, string path,
            params IParameter[] parameters)
        {
            try
            {
                using (var stream = File.Open(path, FileMode.OpenOrCreate))
                {
                    if (await image.ExtractImage(parameters, out IImageSource result, out IExSession info))
                        await writer.Write(result.Source, result.Width, result.Height, stream,
                            info.UnUsedParameters);
                }
            }
            catch { }
        }

        /// <summary>
        /// Writes a given memory block to a file on a given path.
        /// </summary>
        /// <param name="image">Memory block to write to disk file</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void Write(this IImageProcessor writer, IImageContext image,
            Stream stream, params IParameter[] parameters)
        {
            if (await image.ExtractImage(parameters, out IImageSource result, out IExSession info))
                await writer.Write(result.Source, result.Width, result.Height, stream,
                    info.UnUsedParameters);
        }

        /// <summary>
        /// Writes a given memory block to a file on a given path.
        /// </summary>
        /// <param name="pixels">Memory block to write to disk file</param>
        /// <param name="width">Width of memory block</param>
        /// <param name="height"></param>
        /// <param name="path">Path of a file to create and write dat to</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void Write(this IImageProcessor writer,
            int[] pixels, int width, int height, string path,
            params IParameter[] parameters)
        {
            try
            {
                using (var stream = File.Open(path, FileMode.OpenOrCreate))
                {
                    var ptr = pixels.ToPtr(out GCHandle handle);
                    await writer.Write(ptr, width, height, stream, parameters);
                    handle.Free();
                }
            }
            catch { }
        }

        /// <summary>
        /// Writes a given memory block to a file on a given path.
        /// </summary>
        /// <param name="pixels">Memory block to write to disk file</param>
        /// <param name="width">Width of memory block</param>
        /// <param name="height"></param>
        /// <param name="path">Path of a file to create and write dat to</param>
        /// <param name="format">Format of the targeted image file</param>
        /// <param name="quality">Resolution quality of the tageted image file</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void Write(this IImageProcessor writer, IntPtr pixels,
            int width, int height, string path, params IParameter[] parameters)
        {
            //var ext = path.TrimEnd().Substring(3).ToUpper();
            //switch (ext)
            //{
            //    case "PNG":
            //    case "BMP":
            //    case "JPG":
            //    case "HDR":
            //    case "TGA":
            //    case "GWS":
            //    default:
            //        path += ".PNG";
            //        break;
            //}
            try
            {
                using (var stream = File.Open(path, FileMode.OpenOrCreate))
                {
                    await writer.Write(pixels, width, height, stream, parameters);
                }
            }
            catch { }
        }

        /// <summary>
        /// Writes a given memory block to a file on a given path.
        /// </summary>
        /// <param name="image">Memory block to write to disk file</param>
        /// <param name="path">Path of a file to create and write dat to</param>
        /// <param name="format">Format of the targeted image file</param>
        /// <param name="quality">Resolution quality of the tageted image file</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void Write(this IImageProcessor writer, IImageContext image,
            IEnumerable<IParameter> parameters, string path)
        {
            try
            {
                using (var stream = File.Open(path, FileMode.OpenOrCreate))
                {
                    if (await image.ExtractImage(parameters, out IImageSource result, out IExSession info))
                        await writer.Write(result.Source, result.Width, result.Height, stream,
                            info.UnUsedParameters);
                }
            }
            catch { }
        }

        /// <summary>
        /// Writes a given memory block to a file on a given path.
        /// </summary>
        /// <param name="image">Memory block to write to disk file</param>
        /// <param name="format">Format of the targeted image file</param>
        /// <param name="quality">Resolution quality of the tageted image file</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void Write(this IImageProcessor writer, IImageContext image,
            IEnumerable<IParameter> parameters, Stream stream)
        {
            if (await image.ExtractImage(parameters, out IImageSource result, out IExSession info))
                await writer.Write(result.Source, result.Width, result.Height, stream,
                    info.UnUsedParameters);
        }

        /// <summary>
        /// Writes a given memory block to a file on a given path.
        /// </summary>
        /// <param name="pixels">Memory block to write to disk file</param>
        /// <param name="width">Width of memory block</param>
        /// <param name="height"></param>
        /// <param name="path">Path of a file to create and write dat to</param>
        /// <param name="format">Format of the targeted image file</param>
        /// <param name="quality">Resolution quality of the tageted image file</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void Write(this IImageProcessor writer, int[] pixels, int width, int height,
            IEnumerable<IParameter> parameters, string path)
        {
            try
            {
                using (var stream = File.Open(path, FileMode.OpenOrCreate))
                {
                    var ptr = pixels.ToPtr(out GCHandle handle);
                    await writer.Write(ptr, width, height, stream, parameters);
                    handle.Free();
                }
            }
            catch { }
        }

        /// <summary>
        /// Writes a given memory block to a file on a given path.
        /// </summary>
        /// <param name="pixels">Memory block to write to disk file</param>
        /// <param name="width">Width of memory block</param>
        /// <param name="height"></param>
        /// <param name="path">Path of a file to create and write dat to</param>
        /// <param name="format">Format of the targeted image file</param>
        /// <param name="quality">Resolution quality of the tageted image file</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void Write(this IImageProcessor writer, IntPtr pixels,
            int width, int height, IEnumerable<IParameter> parameters, string path)
        {
            try
            {
                using (var stream = File.Open(path, FileMode.OpenOrCreate))
                {
                    await writer.Write(pixels, width, height, stream, parameters);
                }
            }
            catch { }
        }
        #endregion

        #region SAVE AS
        /// <summary>
        /// Saves entire image or a portion of it with or without backgound to a disk file in a specified image format.
        /// </summary>
        /// <param name="block">Image which is to be saved.</param>
        /// <param name="file">Path of a file to create and write data to</param>
        /// <param name="parameters">Null represents a whole chunk of memory block.
        /// Other wise a prtion determined by location X, Y and size Width, Height of the portion rectangle.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void SaveAs(this IImageContext image, string file,
            params IParameter[] parameters)
        {
            if (await image.ExtractImage(parameters, out IImageSource result, out IExSession info))
                await SaveImage(result.Source, result.Width, result.Height, file,
                    info.UnUsedParameters);
        }

        /// <summary>
        /// Saves entire image or a portion of it with or without backgound to a disk file in a specified image format.
        /// </summary>
        /// <param name="block">Image which is to be saved.</param>
        /// <param name="file">Path of a file to create and write data to</param>
        /// <param name="parameters">Null represents a whole chunk of memory block.
        /// Other wise a prtion determined by location X, Y and size Width, Height of the portion rectangle.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void SaveAs(this IImageContext image,
            IEnumerable<IParameter> parameters, string file)
        {
            if (await image.ExtractImage(parameters, out IImageSource result, out IExSession info))
                await SaveImage(result.Source, result.Width, result.Height, file,
                    info.UnUsedParameters);
        }

        /// <summary>
        /// Saves entire image or a portion of it with or without backgound to a disk file in a specified image format.
        /// </summary>
        /// <param name="block">Image which is to be saved.</param>
        /// <param name="file">Path of a file to create and write data to</param>
        /// <param name="parameters">Null represents a whole chunk of memory block.
        /// Other wise a prtion determined by location X, Y and size Width, Height of the portion rectangle.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void SaveAs(IntPtr data, int width, int height,
            string file, params IParameter[] parameters)
        {
            if (width != 0 && height != 0)
                await SaveImage(data, width, height, file, parameters);
        }

        /// <summary>
        /// Saves entire image or a portion of it with or without backgound to a disk file in a specified image format.
        /// </summary>
        /// <param name="block">Image which is to be saved.</param>
        /// <param name="file">Path of a file to create and write data to</param>
        /// <param name="parameters">Null represents a whole chunk of memory block.
        /// Other wise a prtion determined by location X, Y and size Width, Height of the portion rectangle.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        static async Task<bool> SaveImage(IntPtr data, int width, int height,
            string file, IEnumerable<IParameter> parameters)
        {
            ImageFormat format = ImageFormat.PNG;

            if (parameters != null)
            {
                format = parameters.OfType<IImageFormat>().
                    LastOrDefault()?.Format ?? ImageFormat.PNG;
            }
            file += "." + (format).ToString();

            using (var stream = File.Open(file, FileMode.OpenOrCreate))
            {
                return await Factory.ImageProcessor.Write(data, width, height, stream, parameters);
            }
        }
        #endregion

        #region EXTRACT IMAGE
        internal static unsafe Task<bool> ExtractImage(this IImageContext imageContext,
           IEnumerable<IParameter> parameters, out IImageSource Image, out IExSession session)
        {
            parameters.Extract(out session);
            var OriginalCommand = session.Command;

            bool ToBeRotated = !(imageContext is IColour) &&
                (session.Rotation != null && session.Rotation.Valid) ||
                (session.Scale != null && session.Scale.HasScale ||
                (session.ImageSize != null && session.ImageSize.Valid));

            session.Command |= Command.SkipRotateScale;
            IntPtr Source = IntPtr.Zero;

            bool SwapRedBlueChannel = (session.Command & Command.SwapRedBlueChannel) == Command.SwapRedBlueChannel;
            bool CopyRGBOnly = (session.Command & Command.CopyRGBOnly) == Command.CopyRGBOnly;

            if (!ToBeRotated && imageContext is IImageSource)
            {
                if (
                    SwapRedBlueChannel
                    || CopyRGBOnly || session.CopyArea != null
                    )
                {
                    goto NORMAL_SOURCE_DETERMINATION;
                }
               
                Image = (IImageSource)imageContext;
                return Task.FromResult(true);
            }

            NORMAL_SOURCE_DETERMINATION:
            int srcW = 0, srcH = 0, x = 0, y = 0, w = 0, h = 0;
            session.CopyArea?.GetBounds(out x, out y, out w, out h);

            if (imageContext is ISize)
            {
                srcW = ((ISize)imageContext).Width;
                srcH = ((ISize)imageContext).Height;
                if (w == 0)
                    w = srcW;
                if (h == 0)
                    h = srcH;

            }
            else if (imageContext is IColour)
            {
                if (w == 0)
                    w = 16;
                if (h == 0)
                    h = 16;
                srcW = w; 
                srcH = h;               
            }
            else
                goto HANDLE_UNKNOWN;

            #region SOURCE DETERMINATION
            if (imageContext is IExtracter<IntPtr>)
            {
                Source = ((IExtracter<IntPtr>)imageContext).Extract(session, out srcW, out srcH);
                goto SOURCE_FOUND;
            }
            if (imageContext is ICopyable || imageContext is IPenContext)
            {
                if (w == 0)
                    w = srcW;
                if (h == 0)
                    h = srcH;
                var block = new int[w * h];
                Source = block.ToPtr(out GCHandle handle);
                if (imageContext is ICopyable)
                    ((ICopyable)imageContext).CopyTo(Source, w * h, w, session);
                else
                    ((IPenContext)imageContext).CopyTo(Source, w * h, w, session);
                srcW = w;
                srcH = h;
                x = y = 0;
                if (handle.IsAllocated)
                    handle.Free();
                goto PROCESS_SOURCE;
            }
            if (imageContext is ISource<IntPtr>)
            {
                var img = ((ISource<IntPtr>)imageContext);
                Source = img.Source;
                srcW = img.Width;
                srcH = img.Height;

                goto SOURCE_FOUND;
            }
            #endregion

            HANDLE_UNKNOWN:
            Factory.factory.DealWithUnknownImageSource(imageContext, ref Source, ref srcW, ref srcH);

            SOURCE_FOUND:
            if (w == 0)
                w = srcW;
            if (h == 0)
                h = srcH;

            PROCESS_SOURCE:
            if (Source == IntPtr.Zero)
            {
                Image = null;
                return Task.FromResult(false);
            }
            if (ToBeRotated)
            {
                IPen BkgPen = null;
                if (session.PenContext is IPen)
                    BkgPen = ((IPen)session.PenContext);
                if (BkgPen == null)
                {
                    BkgPen = Rgba.Transparent;
                    if (imageContext is IBackgroundContextSetter)
                        BkgPen = ((IBackgroundContextSetter)imageContext).BackgroundPen;
                }
                int newWidth = srcW, newHeight = srcH;
                if (session.ImageSize != null && session.ImageSize.Valid)
                {
                    newWidth = session.ImageSize.Width;
                    newHeight = session.ImageSize.Height;
                }

                var res = Factory.ImageProcessor.RotateAndScale
                    (Source, srcW, srcH, x, y, newHeight, newWidth, session.Interpolation, true, session.Rotation, session.Scale, BkgPen, false).Result;
                srcW = res.Item1.Width;
                srcH = res.Item1.Height;
                Source = res.Item2;
                session.CopyArea = new Rectangle(0, 0, srcW, srcH);
            }
            Image = new ImageSource(Source, srcW, srcH);
            session.Command = OriginalCommand;
            return Task.FromResult(true);
        }
        #endregion

        #region DRAW IMAGE
        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangle to the given destination.
        /// </summary>
        /// <param name="graphics">graphics which to render a memory block on</param>
        /// <param name="source">1D array interpreted as a 2D array of Pixels with specified srcW width</param>
        /// <param name="dstX">Top Left x co-ordinate of destination on graphics</param>
        /// <param name="dstY">Top left y co-ordinate of destination on graphics</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUpdateArea DrawImage(this IRenderer graphics, IImageContext source,
            int dstX, int dstY, CopyCommand command = 0, UpdateCommand updateCommand = 0, IImageSize newImageSize = null)
        {
            var parameters = new IParameter[] { command.Add(), new Point(dstX, dstY), updateCommand.Add(), newImageSize };
            return graphics._DrawImage(parameters, source);
        }

        /// <summary>
        /// Draws an image by taking an area from a 1D array representing a rectangle to the given destination.
        /// </summary>
        /// <param name="graphics">graphics which to render a memory block on</param>
        /// <param name="source">1D array interpreted as a 2D array of Pixels with specified srcW width</param>
        /// <param name="dstX">Top Left x co-ordinate of destination on graphics</param>
        /// <param name="dstY">Top left y co-ordinate of destination on graphics</param>
        /// <param name="copyArea"></param>
        /// <param name="command"></param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUpdateArea DrawImage(this IRenderer graphics, IImageContext source,
            int dstX, int dstY, IBounds copyArea, CopyCommand command = 0, UpdateCommand updateCommand = 0, IImageSize newImageSize = null)
        {
            var parameters = new IParameter[]
            {
                command.Add(),
                new Point(dstX, dstY),
                new Area(copyArea),
                updateCommand.Add(),
                newImageSize
            };
            return graphics._DrawImage(parameters, source);
        }

        /// <summary>
        /// Writes portion of data specified by copyArea parameter from a given memory block and 
        /// pastes it onto destination block at given loaction specified by dstPoint parameter.
        /// </summary>
        /// <param name="source">Source memory block to write.</param>
        /// <param name="copyArea">Specifies the area to copy from this object.</param>
        /// <param name="dstPoint">Destination point where data pasting should begin from.</param>
        /// <param name="command">Command to control pixel line writing.</param>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IUpdateArea DrawImage(this IRenderer graphics, IImageContext source,
            IBounds copyArea, IPoint dstPoint, CopyCommand command = 0, UpdateCommand updateCommand = 0, IImageSize newImageSize = null)
        {
            var parameters = new IParameter[]
            {
                command.Add(),
                new Point(dstPoint),
                new Area(copyArea),
                updateCommand.Add(),
                newImageSize
            };
            return graphics._DrawImage(parameters, source);
        }

        /// <summary> 
        /// Writes portion of data specified by copyX, copyY, copyW, copyH parameters from a given memory block and 
        /// pastes it onto this texture at given loaction specified by dstX and dstY parameters.
        /// </summary>
        /// <param name="source">Source memory block to write.</param>
        /// <param name="parameters">Collection of parameters to control image drawing operation.</param>
        /// Can be null.</param>
        public static IUpdateArea DrawImage(this IRenderer graphics,
            IImageContext source, params IParameter[] parameters) =>
           graphics._DrawImage(parameters, source);

        /// <summary> 
        /// Writes portion of data specified by copyX, copyY, copyW, copyH parameters from a given memory block and 
        /// pastes it onto this texture at given loaction specified by dstX and dstY parameters.
        /// </summary>
        /// <param name="source">Source memory block to write.</param>
        public static IUpdateArea DrawImage(this IRenderer graphics, IImageContext source) =>
           graphics._DrawImage(null, source);

        /// <summary> 
        /// Writes portion of data specified by copyX, copyY, copyW, copyH parameters from a given memory block and 
        /// pastes it onto this texture at given loaction specified by dstX and dstY parameters.
        /// </summary>
        /// <param name="parameters">Collection of parameters to control image drawing operation.</param>
        /// <param name="source">Source memory block to write.</param>
        /// Can be null.</param>
        public static IUpdateArea DrawImage(this IRenderer renderer,
           IEnumerable<IParameter> parameters, IImageContext source) =>
           renderer._DrawImage(parameters, source);

        /// <summary> 
        /// Writes portion of data specified by copyX, copyY, copyW, copyH parameters from a given memory block and 
        /// pastes it onto this texture at given loaction specified by dstX and dstY parameters.
        /// </summary>
        /// <param name="parameters">Collection of parameters to control image drawing operation.</param>
        /// <param name="source">Source memory block to write.</param>
        /// Can be null.</param>
        static IUpdateArea _DrawImage(this IRenderer renderer, IEnumerable<IParameter> parameters, IImageContext source)
        {
            if (!source.ExtractImage(parameters, out IImageSource image, out IExSession info).Result
                | image.Source == IntPtr.Zero)
            {
                return UpdateArea.Empty;
            }
            var boundary = Factory.newBoundary();
            info.Boundaries.Add(boundary);
            var OriginalCommand = info.Command;
            if((OriginalCommand & Command.SkipRotateScale)!= Command.SkipRotateScale)
                info.Command |= Command.SkipRotateScale;
            var action = renderer.CreateRenderAction(info);
            action(null, null, image);
            info.Command = OriginalCommand;
            return boundary;
        }
        #endregion

        #region COPY TO
        /// <summary>
        /// Provides a paste routine to paste the specified chunk of data to a given destination pointer on a given location.
        /// </summary>
        /// <param name="dest">Specifies a pointer where the block should get copied</param>
        /// <param name="dstLen">Specifies the current length of the destination pointer</param>
        /// <param name="dstW">Specifies the current width by which the pixel writing should be wrapped to the next line</param>
        /// <param name="parameters">Collection of parameters to control image drawing operation.</param>
        /// Can be null.</param>
        public static unsafe ISize CopyTo(this IPenContext penContext, IntPtr dest, int dstLen, int dstW,
            IEnumerable<IParameter> parameters = null)
        {
            int* dst = (int*)dest;
            int dstH = dstLen / dstW;

            #region EXTRACT PARAMETRS
            parameters.Extract(out IExSession info);
            int dstX = info.UserPoint?.X ?? 0;
            int dstY = info.UserPoint?.Y ?? 0;
            #endregion

            if (info.CopyArea == null)
                info.CopyArea = new Rectangle(dstX, dstY, dstW, dstH);

            info.CopyArea.GetBounds(out int x, out int y, out int w, out int h);
#if Advance
            if (info.Clip != null && info.Clip.Valid)
            {
                info.Clip.GetBounds(out int clipX, out int clipY, out int clipW, out int clipH);
                int clipR = clipX + clipW;
                int clipB = clipY + clipH;

                if (dstX > clipR && dstY > clipB)
                    return Size.Empty;

                if (dstY < clipY || dstY > clipB || dstX > clipR)
                    return Size.Empty;

                var dstEX = dstX + w;
                if (dstX < clipX)
                    dstX = clipX;
                if (dstEX > clipR)
                    dstEX = clipR;
                w = dstEX - dstX;
            }
#endif
            if (w < 0)
                return Size.Empty;

            var right = x + w;
            var bottom = y + h;

            if (y < 0)
            {
                bottom += y;
                y = 0;
            }
            int dstIndex = dstX + dstY * dstW;
            var r = x + w;
            var b = y + h;
            int len;
            if (y < 0)
            {
                b += y;
                y = 0;
            }

            IPen pen;

            if (penContext is IPen)
                pen = (IPen)penContext;
            else
            {
                pen = Factory.factory.ToPen(penContext, dstW, dstH);
            }

            var copyCommand = info.Command.ToEnum<CopyCommand>();
            while (y < b)
            {
                pen.ReadLine(x, r, y, true, out int[] penData, out int srcIndex, out len);
                fixed (int* src = penData)
                    Blocks.Copy(src, srcIndex, dst, dstIndex, len, copyCommand);

                dstIndex += dstW;
                ++y;
            }
            return new Size(w, h);
        }
        #endregion
    }
}
