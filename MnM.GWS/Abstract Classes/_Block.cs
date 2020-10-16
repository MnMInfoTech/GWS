using System;
/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public abstract class _Block: IBlock
    {
        #region VARIABLES
        protected int width, height, length;
        protected bool isDisposed;
        protected string ShapeName;

        protected bool AntiAliased;
        protected bool CheckForCloseness;
        protected bool LineOnly;
        protected bool EndsOnly;
        
        protected IPen FrgPen;
        #endregion

        #region PROPERTIES
        public string ID { get; protected set; }
        public int Width => width;
        public int Height => height;
        public int Length => length;
        public IReadContext Foreground
        {
            get => FrgPen;
            set
            {
                if (value == null)
                {
                    (FrgPen as IDisposable)?.Dispose();
                    FrgPen = null;
                    return;
                }
                FrgPen = value.ToPen(Width, Height);
            }
        }
        public bool IsDisposed => isDisposed;

        public abstract
#if Advanced
            IDrawSettings2
#else
            IDrawSettings
#endif
            Settings
        { get; }

        protected abstract unsafe int* source { get; }
#if Advanced
        public unsafe abstract byte* SourceAlphas { set; }
        protected abstract unsafe byte* alphas { get; }
#endif
        #endregion

        #region RENDER
        public virtual void Render(IRenderable renderable, IReadContext readContext = null, int? drawX = null, int? drawY = null)
        {
            IPen Pen;
            IReadContext Context = readContext;

            Begin(renderable, out Pen, drawX, drawY);

            if (Pen != null)
                Context = Pen;

            if (renderable is IRenderable2)
            {
                ((IRenderable2)renderable).Draw(readContext, out Pen);
                End(Pen);
                return;
            }
            else if (renderable is IDrawable)
            {
                var drawable = renderable as IDrawable;
                if (drawable.Draw(this, Context, out Pen))
                {
                    End(Pen);
                    return;
                }
                var shape = drawable.ToShape();
                IShape Shape;
                if (shape != null)
                {
                    if (shape is IShape)
                        Shape = shape as IShape;
                    else
                        Shape = new Shape(shape, (drawable as IRecognizable)?.Name ?? "Shape");

                    this.RenderShape(Shape, Context, out Pen);
                    End(Pen);
                    return;
                }
            }
            else if (renderable is IShape)
            {
                this.RenderShape(renderable as IShape, Context, out Pen);
                End(Pen);
                return;
            }

            RenderCustom(renderable, Context, out Pen);
            End(Pen);
            return;
        }

        /// <summary>
        /// Renders specified shape on this buffer with specified reading context.
        /// </summary>
        /// <param name="shape">Shape to render on the buffer.</param>
        /// <param name="readContext">A pen context which to create a buffer pen from.</param>
        /// <param name="Pen">Resultant pen created from conversion of read context.</param>
        protected abstract void RenderShape(IShape shape, IReadContext readContext, out IPen Pen);
        #endregion

        #region RENDER CUSTOM
        /// <summary>
        /// Renders any custom element which  inherits neither IShape nor IDawable on a given buffer with specified reading context.
        /// </summary>
        /// <param name="shape">Shape object which is to be rendered</param>
        /// <param name="context">A pen context which to create a buffer pen from</param>
        /// <returns></returns>
        protected virtual void RenderCustom(IRenderable shape, IReadContext readContext, out IPen Pen) { Pen = null; }
        #endregion

        #region BEGIN
        /// <summary>
        /// Tells this renderer to get hold of buffer specified and prepare for rendering operation.
        /// This method must be called before any individual shape rendering method is called Except main render method.
        /// </summary>
        /// <param name="Shape"></param>
        protected abstract void Begin(IRenderable Shape, out IPen Pen, int? drawX = null, int? drawY = null);
        #endregion

        #region END
        /// <summary>
        /// Performs clean up task on this renderer, current buffer as well as brush.
        /// </summary>
        protected abstract void End(IPen Pen);
        #endregion

        #region BLEND
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int Blend(int dstColor, int srcColor, byte alpha)
        {
            if (alpha == 255)
                return srcColor;

            //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-color-fast?
            uint C1 = (uint)dstColor;
            uint C2 = (uint)srcColor;
            uint invAlpha = 255 - (uint)alpha;
            uint RB = ((invAlpha * (C1 & Colors.RBMASK)) + (alpha * (C2 & Colors.RBMASK))) >> 8;
            uint AG = (invAlpha * ((C1 & Colors.AGMASK) >> 8)) + (alpha * (Colors.ONEALPHA | ((C2 & Colors.GMASK) >> 8)));
            int color = (int)((RB & Colors.RBMASK) | (AG & Colors.AGMASK));
            return color;
        }
        #endregion

        #region WRITE PIXEL
        public abstract void WritePixel(int val, int axis, bool horizontal, int color, float? Alpha);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WritePixel(float val, int axis, bool horizontal, int color)
        {
            int intVal = (int)val;

            float alpha = val - intVal;

            if (alpha == 0 || !AntiAliased)
            {
                WritePixel(intVal, axis, horizontal, color, null);
                return;
            }

            int x = horizontal ? intVal : axis;
            int y = horizontal ? axis : intVal;

            if (horizontal)
            {
                WritePixel(x, y, true, color, 1 - alpha);
                WritePixel(x + 1, y, true, color, alpha);
            }
            else
            {
                WritePixel(y, x, false, color, 1 - alpha);
                WritePixel(y + 1, x, false, color, alpha);
            }
        }
        #endregion

        #region WRITE LINE
        public abstract unsafe void WriteLine(int* source, int srcIndex, int srcW, int length, bool horizontal, int x, int y, float? Alpha);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void WriteLine(float start, float end, int axis, bool horizontal, IReadable pen, float? Alpha)
        {
            if (float.IsNaN(start) && float.IsNaN(end))
                return;

            Numbers.Order(ref start, ref end);

            bool IsPoint = start == end;
            int Start, End;
            bool jump = false;
            bool NotSoClose = true;

            Start = start.Ceiling();
            End = end.Ceiling();
            int w = width;
            int h = height;

            if (CheckForCloseness)
                NotSoClose = (End - (int)start) > 1;

            int copyLength;

            if (NotSoClose && !IsPoint && !EndsOnly)
            {
                int destVal = Start;
                int destAxis = axis;
                int dstX, dstY;

                if (Start == int.MinValue && End == int.MinValue)
                    return;

                dstX = horizontal ? destVal : destAxis;
                dstY = horizontal ? destAxis : destVal;

                dstX += Settings.X;
                dstY += Settings.Y;

                if (horizontal && (dstY < 0 || dstY >= h))
                    return;
                if (!horizontal && (dstX < 0 || dstX >= w))
                    return;

                copyLength = End - Start;

                if (Start < 0)
                    Start = 0;

                if (horizontal)
                {
                    if (dstX < 0)
                    {
                        copyLength += dstX;
                        dstX = 0;
                    }
                }
                else
                {
                    if (dstY < 0)
                    {
                        copyLength += dstY;
                        dstY = 0;
                    }
                }

                if (horizontal)
                {
                    if (dstX + copyLength >= w)
                        copyLength = -dstX + w;
                }
                else
                {
                    if (dstY + copyLength >= h)
                        copyLength = -dstY + h;
                }

                pen.ReadLine(Start, Start + copyLength, axis, horizontal, out int[] source, out int srcIndex, out copyLength);
                if (copyLength == 0)
                    return;

                fixed (int* src = source)
                    WriteLine(src, srcIndex, copyLength, copyLength, horizontal, dstX, dstY, Alpha);
                if (jump)
                    return;
            }

            if (LineOnly)
                return;

            int x, y, intVal;

            intVal = (int)start;
            x = horizontal ? intVal : axis;
            y = horizontal ? axis : intVal;
            int color = pen.ReadPixel(x, y);

            WritePixel(start, axis, horizontal, color);

            intVal = (int)end;
            x = horizontal ? intVal : axis;
            y = horizontal ? axis : intVal;
            color = pen.ReadPixel(x, y);
            WritePixel(end, axis, horizontal, color);
        }
        #endregion

        #region COPY TO
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Rectangle CopyTo(IWritable block, int destX, int destY, int copyX, int copyY, int copyW, int copyH)
        {
            var copy = Rects.CompitibleRc(width, height, copyX, copyY, copyW, copyH);

#if Advanced
            block.SourceAlphas = alphas;
#endif
            Rectangle dstRc;
            var x = copy.X;
            var y = copy.Y;
            copyW = copy.Width;

            var b = y + copy.Height;
            if (y < 0)
            {
                b += y;
                y = 0;
            }
            int srcLen = length;
            int srcIndex = x + y * width;
            int srcW = width;
            var dy = destY;
            int* src = source;

            for (int j = y; j <= b; j++)
            {
                block.WriteLine(src, srcIndex, srcW, copyW, true, destX, dy++, null);
                srcIndex += srcW;
                if (srcIndex >= srcLen)
                    break;
            }
            dstRc = new Rectangle(destX, destY, copyW, dy - destY);

#if Advanced
            block.SourceAlphas = null;
#endif

            if (dstRc)
                block.Invalidate(dstRc.X, dstRc.Y, dstRc.Width, dstRc.Height, true);

            return dstRc;
        }
        public abstract Rectangle CopyTo(int copyX, int copyY, int copyW, int copyH, IntPtr destination, int dstLen, int dstW, int dstX, int dstY);
        #endregion

        #region INVALIDATE
        public abstract void Invalidate(int x, int y, int width, int height, bool updateImmediate = false);
        #endregion

        #region CLONE
        public abstract object Clone();
        #endregion
    }
}
