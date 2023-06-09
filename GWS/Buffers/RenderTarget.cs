/* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

#if (GWS || Window)
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    #region IRENDER TARGET
    /// <summary>
    /// Represents an object which has a capability to receive data from copyable source object.
    /// </summary>
    public partial interface IRenderTarget : IImageData, IUpdatable, IVisible, IPoint, IRenderWindowHolder, IMemoryOccupier, IPen
    { }
    #endregion

    #region IEx RENDER TARGET
    internal partial interface IExRenderTarget : IRenderTarget, IDisposable, IExRenderWindowHolder
    { }
    #endregion

    public interface IRenderTargetHolder
    {
        IRenderTarget Target { get; }
    }

    internal interface IExRenderTargetHolder : IRenderTargetHolder
    {
        new IExRenderTarget Target { get; }
    }

    public abstract partial class RenderTarget : IExRenderTarget, IExResizable
    {
        #region VARIABLES
        /// <summary>
        /// SDL Window object.
        /// </summary>
        readonly internal IExRenderWindow RenderWindow;
        MemoryOccupation memoryOccupation;
        #endregion

        #region CONSTRUCTOR
        protected RenderTarget(IRenderWindow window)
        {
            if (!(window is IExRenderWindow))
            {
                throw new ArgumentException("Given window is not compatible wint this target!");
            }
            RenderWindow = (IExRenderWindow)window;
            memoryOccupation = new MemoryOccupation();
            memoryOccupation.BeginMonitoring(out long total);
            memoryOccupation.EndMonitoring(total);
        }
        #endregion

        #region PROPERTIES
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract string ID { get; }
        public bool Valid => Width > 0 && Height > 0;
        public bool Visible => RenderWindow.Visible;
        public int X => RenderWindow.X;
        public int Y => RenderWindow.Y;
        public IMemoryOccupation MemoryOccupation => memoryOccupation;
        protected abstract IntPtr Source { get; }
        IntPtr ISource<IntPtr>.Source => Source;
        IExRenderWindow IExRenderWindowHolder.RenderWindow => RenderWindow;
        IRenderWindow IRenderWindowHolder.RenderWindow => RenderWindow;
        object IValue.Value => this;
        #endregion

        #region INDEX OF
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe int IndexOf(int x, int y)
        {
            #region CALCULATE INDEX
            int length = Width * Height;
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            var index = x + y * Width;

            if (index > length - 1)
                index %= length;
            #endregion

            return index;
        }
        #endregion

        #region READ PIXEL
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int ReadPixel(int x, int y)
        {
            int index = IndexOf(x, y);

            if (index < 0)
                return 0;
            return ((int*)Source)[index];
        }
        #endregion

        #region READ LINE/S
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void ReadLine(int Start, int End, int Axis, bool Horizontal, out int[] source,
            out int srcIndex, out int length)
        {
            #region INITIALIZE OUT VARIABLES IN CASE WE HAVE TO RETURN WITHOUT FURTHER PROCESSING
            source = new int[1];
            srcIndex = 0;
            #endregion

            #region CHECK POSITIVE LENGTH
            if (Start > End)
            {
                var temp = Start;
                Start = End;
                End = temp;
            }
            length = End - Start;

            if (length < 0)
                return;

            if (length == 0)
                length = 1;
            #endregion

            source = new int[length];
            int* data = (int*)Source;
            int counter = Horizontal ? 1 : Width;
            fixed (int* p = source)
            {
                var x = Horizontal ? Start : Axis;
                var y = Horizontal ? Axis : Start;
                int index = x + y * Width;
                Blocks.Copy(data, index, p, 0, length, 0, 1, counter);
                srcIndex = 0;
            }
        }
        #endregion

        #region UPDATE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public abstract void Update(IBounds bounds, UpdateCommand command = 0);
        #endregion

        #region RESIZE
        object IExResizable.Resize(int w, int h, out bool success, ResizeCommand resizeCommand)
        {
            success = false;
            if (
               (RenderWindow.ViewState & ViewState.Disposed) == ViewState.Disposed ||
               (w == Width && h == Height) ||
               (w == 0 && h == 0))
                return this;

            bool SizeOnlyToFit = (resizeCommand & ResizeCommand.SizeOnlyToFit) == ResizeCommand.SizeOnlyToFit;

            if (SizeOnlyToFit && Width > w && Height > h)
                return this;

            if (SizeOnlyToFit)
            {
                if (w < Width)
                    w = Width;
                if (h < Height)
                    h = Height;
            }

            memoryOccupation = new MemoryOccupation();
            memoryOccupation.BeginMonitoring(out long total);
            //BeforeResize(w, h);
            ResizeInternal(w, h);
            memoryOccupation.EndMonitoring(total);
            var rc = new UpdateArea(0, 0, w, h);
            RenderWindow.View.Update(rc, UpdateCommand.RestoreScreen);
            success = true;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void ResizeInternal(int w, int h);
        #endregion

        #region DISPOSE
        public virtual void Dispose() { }
        #endregion
    }
}
#endif
