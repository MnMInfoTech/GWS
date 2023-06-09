/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details.
* Author: Mukesh Adhvaryu
*/
#if (GWS || Window)
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    #region IBRUSH
    /// <summary>
    /// Represents a brush with certain fill style and gradient for drawing a shape on screen.
    /// </summary>
    public interface IBrush : IPen, ISize, IValid, ICloneable, ISettings, ISizeHolder, IDisposable
    {
        /// <summary>
        /// Gets gradient type this brush currently represents.
        /// </summary>
        sbyte Gradient { get; }
    }
    #endregion
}

namespace MnM.GWS
{
#if DevSupport
    public
#else
    internal
#endif
    unsafe abstract class Brush: IBrush, IExResizable
    {
        #region VARIABLES
        protected int width, height, length;
        protected Size OriginalSize;
        protected int OriginalLength;
        protected sbyte Gradient;
        protected int[] Data;

        const uint AMASK = Colours.AMASK;
        const uint RBMASK = Colours.RBMASK;
        const uint GMASK = Colours.GMASK;
        const uint AGMASK = AMASK | GMASK;
        const uint ONEALPHA = Colours.ONEALPHA;
        const int Inversion = Colours.Inversion;

        protected int Rx, Ry;
        protected bool MatchSize;

        protected bool IsRotated;
        protected int Cx, Cy;
        protected int Cosi, Sini;
        protected float Cos, Sin;
        protected int BrushX, BrushY;
        protected int XCx, YCy;
        protected bool Antialiased;

        protected int BigExp = Vectors.BigExp;
        protected int Big = Vectors.Big;

        const byte MAX = 255;
        #endregion

        #region MATH.ATAN2
        protected const int atan2SIZE = 1024;

        // Output will swing from -STRETCH to STRETCH (default: Math.PI)
        // Useful to change to 1 if you would normally do "atan2(y, x) / Math.PI"

        // Inverse of SIZE
        protected const int negativeatan2EZIS = -atan2SIZE;

        protected readonly static float[] ATAN2_TABLE_PPY = new float[atan2SIZE + 1];
        protected readonly static float[] ATAN2_TABLE_PPX = new float[atan2SIZE + 1];
        protected readonly static float[] ATAN2_TABLE_PNY = new float[atan2SIZE + 1];
        protected readonly static float[] ATAN2_TABLE_PNX = new float[atan2SIZE + 1];
        protected readonly static float[] ATAN2_TABLE_NPY = new float[atan2SIZE + 1];
        protected readonly static float[] ATAN2_TABLE_NPX = new float[atan2SIZE + 1];
        protected readonly static float[] ATAN2_TABLE_NNY = new float[atan2SIZE + 1];
        protected readonly static float[] ATAN2_TABLE_NNX = new float[atan2SIZE + 1];
        #endregion

        #region CONSTRUCTORS
        static Brush()
        {
            for (int i = 0; i <= atan2SIZE; i++)
            {
                float f = (float)i / atan2SIZE;
                ATAN2_TABLE_PPY[i] = (float)(Math.Atan(f) * Angles.PI / Angles.PI);
                ATAN2_TABLE_PPX[i] = (Angles.PI * 0.5f - ATAN2_TABLE_PPY[i]);
                ATAN2_TABLE_PNY[i] = (-ATAN2_TABLE_PPY[i]);
                ATAN2_TABLE_PNX[i] = (ATAN2_TABLE_PPY[i] - Angles.PI * 0.5f);
                ATAN2_TABLE_NPY[i] = (Angles.PI - ATAN2_TABLE_PPY[i]);
                ATAN2_TABLE_NPX[i] = (ATAN2_TABLE_PPY[i] + Angles.PI * 0.5f);
                ATAN2_TABLE_NNY[i] = (ATAN2_TABLE_PPY[i] - Angles.PI);
                ATAN2_TABLE_NNX[i] = (-Angles.PI * 0.5f - ATAN2_TABLE_PPY[i]);
            }
        }
        protected Brush()
        {
            MatchSize = true;
        }
        #endregion

        #region PROPERTIES
        public int Width => width;
        public int Height => height;
        public int Length => length;
        public bool IsDisposed { get; private set; }
        public Size Size
        {
            get => new Size(width, height);
            set
            {
                if (!value)
                    return;
                ((IExResizable)this).Resize(value.Width, value.Height, out _);
            }
        }
        protected unsafe int* data
        {
            get
            {
                fixed (int* p = Data)
                    return p;
            }
        }
        sbyte IBrush.Gradient => Gradient;
        protected abstract int SolidBrushColour { get; }
        public abstract string ID { get; }
        object IValue.Value => this;
        bool IValid.Valid => true;
        #endregion

        #region INDEX OF
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract int IndexOf(int x, int y, bool intCalculation, out int x0, out int y0, out float x3, out float y3);
        #endregion

        #region READ PIXEL
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadPixel(int x, int y)
        {
            if (Gradient == BrushType.Solid)
                return Data[0];
            int index;

            index = IndexOf(x, y, true, out _, out _, out _, out _);
            if (index < 0)
                return 0;
            return Data[index];
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

            if (Gradient == BrushType.Solid)
            {
                source = new int[length];
                fixed (int* p = source)
                {
                    int colour = SolidBrushColour;
                    for (int j = 0; j < length; j++)
                        p[j] = colour;
                    srcIndex = 0;
                }
                return;
            }

            #region SET REQUISITE VARIABLES
            bool aa = Antialiased;
            int x0 = 0, y0 = 0;
            var intCalculation = Gradient != BrushType.Texture || !aa;
            float x3 = 0, y3 = 0;
            int x = Horizontal ? Start : Axis;
            int y = Horizontal ? Axis : Start;

            int ix = Horizontal ? 1 : 0;
            int iy = Horizontal ? 0 : 1;
            int i = 0;
            int index;
            int* pxl;
            #endregion

            #region DECIDE IF LOOP IS REQUIRED
            bool inLoop = IsRotated || !Horizontal ||
            (Gradient > BrushType.DiagonalCentral &&
            Gradient <= BrushType.MiddleCircular);
            #endregion

            fixed (int* data = Data)
            {
                if (!inLoop)
                    goto NON_LOOP;

                source = new int[length];
                fixed (int* p = source)
                    pxl = p;

                LOOP:
                index = IndexOf(x, y, intCalculation, out x0, out y0, out x3, out y3);
                if (index < 0)
                    goto NEXT;
                if (!IsRotated || intCalculation || index == this.length - 1)
                {
                    pxl[i] = data[index];
                    goto NEXT;
                }

                pxl[i] = Blend(index, x0, y0, x3, y3);

            NEXT:
                x += ix;
                y += iy;
                ++i;

                if (i >= length)
                    goto EXIT_LOOP;
                goto LOOP;

            EXIT_LOOP:
                return;

            NON_LOOP:
                index = IndexOf(x, y, intCalculation, out _, out _, out _, out _);
                if (index < 0)
                {
                    srcIndex = 0;
                    length = 0;
                    return;
                }

                #region HORIZONTAL LINE COPY
                if (Gradient == BrushType.Vertical ||
                    Gradient == BrushType.VerticalCentral)
                {
                    source = data[index].Repeat(length + 1);
                    srcIndex = 0;
                }
                else
                {
                    source = Data;
                    srcIndex = index;
                }
                if (Gradient != BrushType.Vertical &&
                    Gradient != BrushType.VerticalCentral &&
                    Gradient != BrushType.Rectangular)
                {
                    if (srcIndex + length > this.length)
                    {
                        var difference = srcIndex + length - this.length;
                        length -= ++difference;
                    }
                }

                if (source.Length == 0 || length < 0)
                {
                    srcIndex = 0;
                    length = 0;
                    return;
                }
                #endregion
                return;
            }
        }
        #endregion

        #region BLEND
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected unsafe int Blend(int index, int x0, int y0, float x3, float y3)
        {
            uint alpha, invAlpha, C1, C2, RB, AG;
            var data = this.data;
            int Colour;

            float Dx = x3 - x0;
            float Dy = y3 - y0;

            if (Dx == 0 && Dy == 0)
            {
                Colour = data[index];
                goto ASSIGN;
            }

            #region BI-LINEAR INTERPOLATION
            int N = index + width;
            bool Only2 = (N >= length || N + 1 >= length);

            C1 = (uint)data[index++];
            C2 = (uint)data[index];

            alpha = (uint)(Dx * MAX);

            if (alpha == MAX)
                C1 = C2;

            else if (alpha != 0)
            {
                invAlpha = MAX - alpha;

                //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-colour-fast?
                RB = ((invAlpha * (C1 & RBMASK)) + (alpha * (C2 & RBMASK))) >> 8;
                AG = (invAlpha * ((C1 & AGMASK) >> 8)) +
                    (alpha * (ONEALPHA | ((C2 & GMASK) >> 8)));
                C1 = ((RB & RBMASK) | (AG & AGMASK));
            }
            if (Only2)
            {
                Colour = (int)C1;
                goto ASSIGN;
            }

            uint C3 = (uint)data[N++];
            uint C4 = (uint)data[N];

            if (alpha == MAX)
                C3 = C4;
            else if (alpha != 0)
            {
                invAlpha = MAX - alpha;

                //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-colour-fast?
                RB = ((invAlpha * (C3 & RBMASK)) + (alpha * (C4 & RBMASK))) >> 8;
                AG = (invAlpha * ((C3 & AGMASK) >> 8)) +
                    (alpha * (ONEALPHA | ((C4 & GMASK) >> 8)));
                C3 = ((RB & RBMASK) | (AG & AGMASK));
            }

            alpha = (uint)(Dy * MAX);

            if (alpha == MAX)
                Colour = (int)C3;
            else if (alpha != 0)
            {
                invAlpha = MAX - alpha;

                //https://www.generacodice.com/en/articolo/247775/How-to-alpha-blend-RGBA-unsigned-byte-colour-fast?
                RB = ((invAlpha * (C1 & RBMASK)) + (alpha * (C3 & RBMASK))) >> 8;
                AG = (invAlpha * ((C1 & AGMASK) >> 8)) +
                    (alpha * (ONEALPHA | ((C3 & GMASK) >> 8)));
                Colour = (int)((RB & RBMASK) | (AG & AGMASK));
            }
            else
                Colour = (int)C1;
            #endregion

            ASSIGN:
            return Colour;
        }
        #endregion

        #region COPY SETTINGS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReceiveSettings(IEnumerable<IParameter> parameters)
        {
            if (parameters == null)
                return;

            int x, y, w, h;

           parameters.Extract(out  IExSession session);

            session.RenderBounds.GetBounds(out x, out y, out w, out h);
            if (w < 0)
                w = -w;
            
            if (h < 0)
                h = -h;
            
            if (w > Vectors.UHD8kWidth)
                w = Vectors.UHD8kWidth;
            if (h > Vectors.UHD8kHeight)
                h = Vectors.UHD8kHeight;

            var Command = session.Command;
            var Rotation = session.Rotation;

            MatchSize = (Command & Command.BrushNoSizeToFit) != Command.BrushNoSizeToFit;
            Antialiased = (Command & Command.Bresenham) != Command.Bresenham;

            var invertRotation = (Command & Command.BrushInvertRotation) == Command.BrushInvertRotation;
            var noautoPositioning =  (Command & Command.BrushNoAutoPosition) == Command.BrushNoAutoPosition;
            var followCanvas = (Command & Command.BrushFollowCanvas) == Command.BrushFollowCanvas;

            if (Gradient != BrushType.Texture)
            {
                if (MatchSize)
                {
                    bool sucess = false;
                    ResizeInternally(w, h, 0, ref sucess);
                }
                else
                {
                    w = width;
                    h = height;
                }
            }
            IsRotated = false;
            bool HasSkewAngle = false;

            if (Rotation != null)
            {
                HasSkewAngle = Rotation.Skew != null &&
                    Rotation.Skew.HasScale &&
                    Rotation.Skew.Type == SkewType.Diagonal;
                IsRotated = Rotation.HasAngle || HasSkewAngle;
            }
            if (IsRotated)
            {
                var degree = Rotation.Angle;
                if (HasSkewAngle)
                    degree += Rotation.Skew.Degree;

                Angles.SinCos(degree, out Sin, out Cos);
                Angles.SinCos(degree, out Sini, out Cosi);

                if (Rotation.Centre != null)
                {
                    Cx = Rotation.Centre.Cx.Round();
                    Cy = Rotation.Centre.Cy.Round();
                }
                else
                {
                    Cx = x + w / 2;
                    Cy = y + h / 2;
                }

                if (invertRotation)
                {
                    Sini = -Sini;
                    Sin = -Sin;
                }
            }
            else
            {
                Cx = Cy = Cosi = Sini = 0;
                Cos = Sin = 0;
            }

            if (!noautoPositioning)
            {
                BrushX = -x;
                BrushY = -y;
            }
            else if (followCanvas)
            {
                BrushX = x;
                BrushY = y;
            }
            else
            {
                BrushX = 0;
                BrushY = 0;
            }
            XCx = BrushX + Cx;
            YCy = BrushY + Cy;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FlushSettings()
        {
            BrushX = BrushY = XCx = YCy = 0;
            Cx = Cy = Cosi = Sini = 0;
            Cos = Sin = 0;
            Antialiased = false;
            if (MatchSize)
                Restore();
            MatchSize = true;
        }
        #endregion

        #region RESIZE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        object IExResizable.Resize(int w, int h, out bool success, ResizeCommand resizeCommand)
        {
            success = false;
            if ((w == Width && h == Height) || (w == 0 && h == 0) ||
                   (w == OriginalSize.Width && h == OriginalSize.Height))
                return this;

            if (w > Vectors.UHD8kWidth)
                w = Vectors.UHD8kWidth;
            if (h > Vectors.UHD8kHeight)
                h = Vectors.UHD8kHeight;

            bool SizeOnlyToFit = (resizeCommand & ResizeCommand.SizeOnlyToFit) == ResizeCommand.SizeOnlyToFit;
            bool NotLessThanOriginal = !SizeOnlyToFit && (resizeCommand & ResizeCommand.NotLessThanOriginal) == ResizeCommand.NotLessThanOriginal;

            if (SizeOnlyToFit && Width > w && Height > h)
                return this;
            if (SizeOnlyToFit)
            {
                if (w < Width)
                    w = Width;
                if (h < Height)
                    h = Height;
            }
            if (NotLessThanOriginal)
            {
                if (w < OriginalSize.Width)
                    w = OriginalSize.Width;
                if (h < OriginalSize.Height)
                    h = OriginalSize.Height;
            }            

            ResizeInternally(w, h, resizeCommand, ref success);
            if (success)
            {
                if (Gradient != BrushType.Texture)
                    Store();
            }
            return this;
        }
        protected abstract void ResizeInternally(int w, int h, ResizeCommand resizeCommand, ref bool sucess);
        #endregion

        #region STORE - RESTORE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void Store();

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected abstract void Restore();
        #endregion

        #region CLONE
        public abstract object Clone();
        #endregion

        #region DISPOSE
        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                width = height = length = 0;
                Data = null;
                Dispose2();
            }
        }
        protected virtual void Dispose2() { }
        #endregion
    }
}
#endif