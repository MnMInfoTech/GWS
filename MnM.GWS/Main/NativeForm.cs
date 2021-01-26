/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
using System;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
#if AllHidden
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
            #endregion

            #region CONSTRCUTORS
            public NativeForm(INativeTarget target)
            {
                Target = target;
                Canvas = Factory.newCanvas(target);
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
            public IObjCollection Objects => Canvas.Objects;
            public IPenContext Background
            {
                get => Canvas.Background;
                set => Canvas.Background = value;
            }
            public override string ID => Target.ID;
            public bool IsDisposed => Target.IsDisposed;
            public IntPtr Handle => Target.Handle;
            public int Width => Canvas.Width;
            public int Height => Canvas.Height;
            public string Name => Target.Name;
            public string Text
            {
                get => Target.Text;
                set => Target.Text = value;
            }
            int ILength.Length => Canvas.Length;
            #endregion

            #region CONSOLIDATE
            public IRectangle Consolidate(IntPtr destination, int dstLen,
                int dstW, int dstX, int dstY, IRectangle copyArea, IImageData backBuffer, Command Command = 0, IntPtr? Pen = null)
            {
                return Canvas.Consolidate(destination, dstLen, dstW, dstX, dstY, copyArea, backBuffer, Command, Pen);
            }
            #endregion

            #region RENDER
            public bool Render(IRenderable Renderable, ISettings Settings = null, bool? suspendUpdate = null)
            {
                return Canvas.Render(Renderable, Settings, suspendUpdate);
            }
            #endregion

            #region WRITE PIXEL
            public void WritePixel(int val, int axis, bool horizontal, int color, float? Alpha, Command Command, string ShapeID, INotifier boundary)
            {
                Canvas.WritePixel(val, axis, horizontal, color, Alpha, Command, ShapeID, boundary);
            }
            #endregion

            #region WRITE LINE
            public unsafe void WriteLine(int* colors, int srcIndex, int srcW, int length, bool horizontal, int x, int y, float? Alpha, byte* imageAlphas, Command Command, string ShapeID, INotifier boundary)
            {
                Canvas.WriteLine(colors, srcIndex, srcW, length, horizontal, x, y, Alpha, imageAlphas, Command, ShapeID, boundary);
            }
            #endregion

            #region COPY TO
            public IRectangle CopyTo(IntPtr destination, int dstLen, int dstW, int dstX, int dstY, IRectangle copyArea, Command command = 0)
            {
                return Canvas.CopyTo(destination, dstLen, dstW, dstX, dstY, copyArea, command);
            }
            #endregion

            #region DISPOSE
            public override void Dispose()
            {
                Canvas.Dispose();
            }
            #endregion

            #region CLEAR
            public IRectangle Clear(int clearX, int clearY, int clearW, int clearH, Command command = Command.None)
            {
                return Canvas.Clear(clearX, clearY, clearW, clearH, command);
            }
            #endregion

            #region COPY FROM
            public IRectangle CopyFrom(IntPtr source, int srcW, int srcH, int dstX, int dstY, IRectangle copyArea, Command Command, IntPtr alphaBytes = default)
            {
                return Canvas.CopyFrom(source, srcW, srcH, dstX, dstY, copyArea, Command, alphaBytes);
            }
            #endregion

            #region RESIZE
            public void Resize(int? newWidth = null, int? newHeight = null)
            {
                Canvas.Resize(newWidth, newHeight);
            }
            #endregion

            #region UPDATE
            public void Update(Command command = Command.None, IRectangle boundary = null)
            {
                Canvas.Update(command, boundary);
            }
            #endregion

            #region REFRESH
            public void Refresh()
            {
                Canvas.Refresh();
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
        }

#if Advanced
        partial class NativeForm : IContainer, IImageData
        {
            #region PROPERTIES
            public IRectangle ClipRectangle
            {
                get => Canvas.ClipRectangle;
                set => Canvas.ClipRectangle = value;
            }
            public bool Clipped => Canvas.Clipped;
            public bool SupportBackgroundBuffer
            {
                get => Canvas.SupportBackgroundBuffer;
                set => Canvas.SupportBackgroundBuffer = value;
            }
            public IEventPusher ActiveObject =>
                Canvas.ActiveObject;
            #endregion

            #region FIND, CHECK ELEMENT
            public IRenderable FindElement(int x, int y) =>
                Canvas.FindElement(x, y);
            public bool CheckElement(IRenderable renderable, int x, int y) =>
                Canvas.CheckElement(renderable, x, y);
            #endregion

            #region FOCUS - UNFOCUS
            public bool Focus(IRenderable shape) =>
                Canvas.Focus(shape);

            public bool Unfocus(IRenderable shape) =>
                Canvas.Unfocus(shape);
            #endregion

            #region GET DATA
            void IImageData.GetData(out int[] Pixels, out byte[] Alphas, bool BackgroundBuffer) =>
                Canvas.GetData(out Pixels, out Alphas, BackgroundBuffer);
            #endregion
        }
#endif
#if AllHidden
    }
#endif
}
