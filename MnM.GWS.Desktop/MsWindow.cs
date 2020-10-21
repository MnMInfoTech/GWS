using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace MnM.GWS.Desktop
{
    public class MsWindow : Form, IForm
    {
        const int dx = 602, dy = 200, dw = 404, dh = 506;
        readonly ICanvas Canvas;

        #region CONSTRUCTORS
        public MsWindow() :
            this(dx, dy, dw, dh)
        { }
        public MsWindow(int width, int height) :
            this(dx, dy, width, height)
        { }
        public MsWindow(int x, int y, int width, int height) : base()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(x, y);
            Size = new System.Drawing.Size(width, height);
            Canvas = Factory.newCanvas(this);
        }
        #endregion

        #region PROPERTIES
        public string ID => Canvas.ID;
        public int Length => Canvas.Length;
        public bool Antialiased
        {
            get => Canvas.Antialiased;
            set { }
        }
        public IReadContext Background
        {
            get => Canvas.Background;
            set => Canvas.Background = value;
        }
        public IDrawSettings Settings => Canvas.Settings;
        public IObjCollection Objects => Canvas.Objects;
#if Advanced
        public IObjectDraw ObjectDraw => Canvas.ObjectDraw;
#endif
        #endregion

        #region PAINT
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var size = Canvas.Portion(out IntPtr Data, e.ClipRectangle.X,
                e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height);
            if (Data == IntPtr.Zero)
                return;
            var bitmap = new Bitmap(size.Width, size.Height, size.Width * 4, PixelFormat.Format32bppArgb, Data);
            e.Graphics.DrawImage(bitmap, e.ClipRectangle.X, e.ClipRectangle.Y);
        }
        #endregion

        #region COPY FROM
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void CopyFrom(ICopyable source, int dstX, int dstY, int srcX, int srcY, int srcW, int srcH, bool updateImmediate = true)
        {
            source.CopyTo(Canvas, dstX, dstY, srcX, srcY, srcW, srcH, updateImmediate);
        }
        #endregion

        #region COPY TO
        public Rectangle CopyTo(IWritable block, int dstX, int dstY, int copyX, int copyY, int copyW, int copyH, bool updateImmediate = true) =>
            Canvas.CopyTo(block, dstX, dstY, copyX, copyY, copyW, copyH, updateImmediate);

        public Rectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr destination, int dstLen, int dstW, int dstX, int dstY) =>
            Canvas.CopyTo(copyX, copyY, copyW, copyH, destination, dstLen, dstW, dstX, dstY);
        #endregion

        #region WRITE PIXEL
        public void WritePixel(int val, int axis, bool horizontal, int color, float? Alpha) =>
            Canvas.WritePixel(val, axis, horizontal, color, Alpha);
        #endregion

        #region WRITE LINE
        public unsafe void WriteLine(int* source, int srcIndex, int srcW, int length, bool horizontal, int x, int y, float? Alpha) =>
            Canvas.WriteLine(source, srcIndex, srcW, length, horizontal, x, y, Alpha);
        #endregion

        #region INVALIDATE
        public void Invalidate(int x, int y, int width, int height, bool updateImmediate = false) 
        {
            Invalidate(new Rectangle(x, y, width, height));
            if (updateImmediate)
                Update();
        }
        #endregion

        #region BEGIN - END
        void IRenderSession.Begin(IRenderable renderable, out IPen pen) =>
            Canvas.Begin(renderable, out pen);

        void IRenderSession.End(IPen pen) =>
            Canvas.End(pen);
        #endregion

        #region CLEAR
        public Rectangle Clear(bool updateImmediate = false) =>
            Canvas.Clear(updateImmediate);

        public Rectangle Clear(int x, int y, int width, int height, bool updateImmediate = false) =>
            Canvas.Clear(x, y, width, height, updateImmediate);
        #endregion

        #region RESIZE
        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            Canvas.Resize(Width, Height);
        }
        #endregion

        #region ROTATE -FLIP
        public Size RotateAndScale(out int[] Data, Rotation angle, bool antiAliased = true, float scale = 1) =>
            Canvas.RotateAndScale(out Data, angle, antiAliased, scale);

        public Size Flip(out int[] Data, Flip flipMode) =>
            Canvas.Flip(out Data, flipMode);
        #endregion

        #region CLONE
        object ICloneable.Clone() => Canvas.Clone();
        #endregion
    }
}
