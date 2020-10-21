using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MnM.GWS.Desktop
{
    public class MsWindow : Form, IRenderTarget
    {
        readonly EventInfo eventInfo = new EventInfo();
        const int dx = 602, dy = 200, dw = 404, dh = 506;
        readonly KeyPressEventArgs KeyPressEventArgs = new KeyPressEventArgs();
        protected ICanvas Canvas;

        int oldWidth, oldHeight;
#if Advanced
        protected int[] Data;
        protected byte [] Alphas;
#endif
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
#if Advanced
            Data = new int[Width * Height];
            Alphas = new byte[Width * Height]
#endif
        }
        #endregion

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
        #region COPY FROM
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void CopyFrom(ICopyable source, int dstX, int dstY, int srcX, int srcY, int srcW, int srcH)
        {
#if Advanced
            var dstRC = this.CompitibleRc(dstX, dstY, srcW, srcH);
            fixed (int* p = Data)
            {
                var textureData = (IntPtr)p;
                source.CopyTo(srcX, srcY, dstRC.Width, dstRC.Height, textureData, Data.Length, rc.Width, 0, 0);
            }
#endif
            //var dstRC = this.CompitibleRc(dstX, dstY, srcW, srcH);
            //IntPtr textureData;
            //int lockedLength;
            //Lock(dstRC, out textureData, out lockedLength);
            //source.CopyTo(srcX, srcY, dstRC.Width, dstRC.Height, textureData, lockedLength, Width, 0, 0);
            //Unlock();
            //CopyToRenderer(Handle, dstRC, dstRC);
        }
        #endregion
        protected override void OnResizeBegin(EventArgs e)
        {
            oldWidth = Width;
            oldHeight = Height;
            base.OnResizeBegin(e);
        }
        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            Canvas.Resize(Width, Height);
#if Advanced
            Data = Blocks.ResizedData(Data, Width, Height, oldWidth, oldHeight);
            Alpha = Blocks.ResizedData(Alpha, Width, Height, oldWidth, oldHeight);
#endif
        }

        class Block : _Block
        {
            public Block()
            {

            }
        }
    }
}
