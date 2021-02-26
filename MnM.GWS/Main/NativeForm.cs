/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
#if HideGWSObjects
    partial class NativeFactory
    {
#else
    public
#endif
        partial class NativeForm : _Events, IForm
        {
            #region VARIABLES
            readonly ICanvas Canvas;
            readonly INativeTarget Target;
            const int formX = 602, formY = 200, formW = 404, formH = 506;
            readonly DrawEventArgs drawEventArgs = new DrawEventArgs();
            #endregion

            #region CONSTRCUTORS
            public NativeForm(INativeTarget target)
            {
                Target = target;
                Canvas = Factory.newCanvas(target);
                Canvas.Background = Rgba.ActiveCaption;
                Target.Form = this;
            }
            public NativeForm(int formW, int formH) :
                this(Factory.newNativeTarget(formW, formH))
            { }
            public NativeForm() :
                this(Factory.newNativeTarget(formW, formH))
            { }
            public NativeForm(int formX, int formY, int formW, int formH) :
                this(Factory.newNativeTarget(formX, formY, formW, formH))
            { }
        #endregion

        #region PROPERTIES
        public IAnimations Animations => Canvas.Animations;
            public IPenContext Background
            {
                set => Canvas.Background = value;
            }
            public bool IsDisposed { get; private set; }
            public IntPtr Handle => Target.Handle;
            public int Width => Canvas.Width;
            public int Height => Canvas.Height;
            public string Text
            {
                get => Target.Text;
                set => Target.Text = value;
            }
            int ILength.Length => Canvas.Length;
            public bool Freezed
            {
                get => Canvas.Freezed;
            }
            #endregion

            #region CONSOLIDATE
            public IPerimeter CopyScreen(IntPtr destination, int dstLen,
                int dstW, int dstX, int dstY, IBoundable copyArea, ulong command = 0, IMultiBuffered backBuffer = null, IntPtr? Pen = null)
            {
                return Canvas.CopyScreen(destination, dstLen, dstW, dstX, dstY, copyArea, command, backBuffer, Pen);
            }
            #endregion

            #region RENDER
            public void Render(IRenderable Renderable, ISettings Settings)
            {
                Canvas.Render(Renderable, Settings);
            }
            #endregion

            #region WRITE PIXEL
            public void WritePixel(int val, int axis, bool horizontal, int color, float? Alpha, ulong command, ISession boundary)
            {
                Canvas.WritePixel(val, axis, horizontal, color, Alpha, command, boundary);
            }
            #endregion

            #region WRITE LINE
            public unsafe void WriteLine(int* colors, int srcIndex, int srcW, int length, bool horizontal, int x, int y,
                float? Alpha, byte* imageAlphas, ulong command, ISession boundary)
            {
                Canvas.WriteLine(colors, srcIndex, srcW, length, horizontal, x, y, Alpha, imageAlphas, command, boundary);
            }
            #endregion

            #region COPY TO
            public IPerimeter CopyTo(IntPtr destination, int dstLen, int dstW, int dstX, int dstY, IBoundable copyArea, ulong command = 0)
            {
                return Canvas.CopyTo(destination, dstLen, dstW, dstX, dstY, copyArea, command);
            }
            #endregion

            #region DISPOSE
            public override void Dispose()
            {
                IsDisposed = true;
                Canvas.Dispose();
            }
            #endregion

            #region CLEAR
            public IPerimeter Clear(IBoundable clear, ulong command = 0)
            {
                return Canvas.Clear(clear, command);
            }
            #endregion

            #region WRITE BLOCK
            public IPerimeter WriteBlock(IntPtr source, int srcW, int srcH, int dstX, int dstY, IBoundable copyArea,
                ulong command, IntPtr alphaBytes = default(IntPtr))
            {
                return Canvas.WriteBlock(source, srcW, srcH, dstX, dstY, copyArea, command, alphaBytes);
            }
            #endregion

            #region RESIZE
            public void Resize(int? newWidth = null, int? newHeight = null)
            {
                Canvas.Resize(newWidth, newHeight);
            }
            #endregion

            #region UPDATE
            public void Update(ulong command, IBoundable area) 
            {
                Canvas.Update(command, area);
            }
            #endregion

            #region REFRESH
            public void Refresh(ulong command = 0)
            {
                Canvas.Refresh(command);
            }
            #endregion

            #region RAISE PAINT
            public void InvokePaint(ulong command = 0, int processID = 0)
            {
                drawEventArgs.Graphics = Canvas;
                drawEventArgs.ProcessID = processID;
                OnPaint(drawEventArgs);
                if ((command & Command.InvalidateOnly) != Command.InvalidateOnly)
                    Update(command, new Perimeter(0, 0, Width, Height, processID, 0, 0));
            }
            #endregion

            #region BACKGROUND CHANGED
            public event EventHandler<IEventArgs> BackgroundChanged
            {
                add => Canvas.BackgroundChanged += value;
                remove => Canvas.BackgroundChanged -= value;
            }
            #endregion

            #region SHOW - HIDE
            public void Show() =>
                Target.Show();
            public void Hide() =>
                Target.Hide();
            #endregion

            #region PUSH EVENT
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void PushEvent(IEventInfo e)
            {
#if Advanced
                Canvas.PushEvent(e);
                if (e.Handled)
                    return;
#endif
                base.PushEvent(e);
            }
            #endregion

            #region ROTATE -FLIP
            public Size RotateAndScale(out IntPtr Data, Rotation angle, bool antiAliased = true, float scale = 1)
            {
                return Canvas.RotateAndScale(out Data, angle, antiAliased, scale);
            }

            public Size Flip(out IntPtr Data, FlipMode flipMode)
            {
                return Canvas.Flip(out Data, flipMode);
            }
            #endregion

            #region ICANVAS
            int IReadable.ReadPixel(int x, int y, IReadSession session) =>
                Canvas.ReadPixel(x, y, session);
            void IReadable.ReadLine(int start, int end, int axis, bool horizontal, out int[] pixels, out int srcIndex, out int length, IReadSession session) =>
                Canvas.ReadLine(start, end, axis, horizontal, out pixels, out srcIndex, out length, session);
            #endregion
        }
        partial class NativeForm : IObjCollection
        {
            #region PROPERTIES
            public int ObjectCount => Canvas.ObjectCount;
            public IRenderable this[uint id] => Canvas[id];
            public IRenderable this[string name] => Canvas[name];
            public ISettings this[IRenderable shape] => Canvas[shape];
            #endregion

            #region CONTAINS
            public bool Contains(IRenderable item)
            {
                return Canvas.Contains(item);
            }
            public bool Contains(uint itemID)
            {
                return Canvas.Contains(itemID);
            }
            #endregion

            #region ADD
            public U Add<U>(U shape, ISettings settings, bool? suspendUpdate = null) where U : IRenderable
            {
                return Canvas.Add(shape, settings, suspendUpdate);
            }
            public U Add<U>(U shape) where U : IRenderable
            {
                return Canvas.Add(shape);
            }
            public void AddRange<U>(IEnumerable<U> controls) where U : IRenderable
            {
                Canvas.AddRange(controls);
            }
            #endregion

            #region REMOVE
            public bool Remove(IRenderable item)
            {
                return Canvas.Remove(item);
            }
            public void RemoveAll()
            {
                Canvas.RemoveAll();
            }
            #endregion

            #region ENUMERATOR
            public IEnumerator<IRenderable> GetEnumerator()
            {
                return Canvas.GetEnumerator();
            }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)Canvas).GetEnumerator();
            }
            #endregion

            #region QUERY
            public IEnumerable<IRenderable> Query(Predicate<ISettings> condition = null)
            {
                return Canvas.Query(condition);
            }
            public IRenderable QueryFirst(Predicate<ISettings> condition = null)
            {
                return Canvas.QueryFirst(condition);
            }
            public IEnumerable<IShape> QueryDraw(Predicate<ISettings> condition = null)
            {
                return Canvas.QueryDraw(condition);
            }
            public IShape QueryFirstDraw(Predicate<ISettings> condition = null)
            {
                return Canvas.QueryFirstDraw(condition);
            }
            #endregion
        }
#if Advanced
#endif
#if HideGWSObjects
    }
#endif
}
