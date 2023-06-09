/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

#if !Advance && (GWS || Window)

using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MnM.GWS
{
    class Canvas : Renderer, IExCanvas
    {
        #region VARIABLES
        /// <summary>
        /// Width of this object.
        /// </summary>
        protected int width;

        /// <summary>
        /// Height of this object
        /// </summary>
        protected int height;

        protected readonly int OriginalWidth, OriginalHeight;

        /// <summary>
        /// colour pixels of this object.
        /// </summary>
        protected int[] Pixels;

        /// <summary>
        /// Render Target this graphics is associated with.
        /// </summary>
        internal readonly IExRenderTarget Target;
        #endregion

        #region CONSTANTS
        protected const uint AMASK = Colours.AMASK;
        protected const uint RBMASK = Colours.RBMASK;
        protected const uint GMASK = Colours.GMASK;
        protected const uint AGMASK = AMASK | GMASK;
        protected const uint ONEALPHA = Colours.ONEALPHA;
        protected const int Inversion = Colours.Inversion;
        protected const uint AlphaRemoval = Colours.AlphaRemoval;

        protected const byte ZERO = 0;
        protected const byte ONE = 1;
        protected const byte TWO = 2;
        protected const byte MAX = 255;

        protected const int NOCOLOR = 0;
        protected const uint UNOCOLOR = 0;
        protected const float START_EPSILON = Lines.START_EPSILON;
        protected const float END_EPSILON = Lines.END_EPSILON;

        protected const int Big = Vectors.Big;
        protected const int BigExp = Vectors.BigExp;

        protected const int Transparent = Colours.Transparent;
        protected const uint White = Colours.UWhite;
        #endregion

        #region CONSTRUCTORS
        public Canvas(int w, int h)  
        {
            width = OriginalWidth = w;
            height = OriginalHeight = h; 
            Pixels = new int[w * h];
        }
        public unsafe Canvas(int[] data, int w, int h, bool makeCopy = false) :
            this(w, h)
        {
            if (data == null)
                return;
            if (makeCopy)
                Array.Copy(data, Pixels, data.Length);
            else
                Pixels = data;
        }
        public unsafe Canvas(int w, int h, byte[] data, bool switchRBChannel = false) :
            this(w, h)
        {
            if (data == null)
                return;
            fixed (byte* src = data)
            {
                CopyBytes(src, switchRBChannel);
            }
        }
        public unsafe Canvas(IntPtr data, int w, int h, bool switchRBChannel = false) :
            this(w, h)
        {
            CopyBytes((byte*)data, switchRBChannel);
        }
        public Canvas(IRenderTarget target)  
        {
            if (!(target is IExRenderTarget))
            {
                throw new Exception("Specified target is not compatible with this view.");
            }
            Target = (IExRenderTarget)target;
            width = OriginalWidth = target.Width;
            height = OriginalHeight = target.Height;
            Pixels = new int[width * height];
        }
        public unsafe Canvas(IRenderTarget target, IntPtr data, bool switchRBChannel = false) :
            this(target)
        {
            CopyBytes((byte*)data, switchRBChannel);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe void CopyBytes(byte* src, bool switchRBChannel = false)
        {
            if (src == null)
                return;
            fixed (int* d = Pixels)
            {
                byte* dst = (byte*)d;
                var len = width * height * 4;
                if (switchRBChannel)
                {
                    for (int i = 0; i < len; i += 4)
                    {
                        dst[i] = src[i + 2];
                        dst[i + 1] = src[i + 1];
                        dst[i + 2] = src[i];
                        dst[i + 3] = src[i + 3];
                    }
                }
                else
                {
                    for (int i = 0; i < len; i += 1)
                    {
                        dst[i] = src[i];
                    }
                }
            }
        }
        #endregion

        #region PROPERTIES
        public sealed override int Width => width;
        public sealed override int Height => height;
        public sealed override bool Valid => width > 0 && height > 0 && (viewState & ViewState.Disposed) != ViewState.Disposed;
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
        protected unsafe IntPtr Source 
        {
            get
            {
                fixed (int* p = Pixels)
                    return (IntPtr)p;
            }
        }
        bool IOriginCompatible.IsOriginBased => true;
        IntPtr ISource<IntPtr>.Source => Source;
        int IPoint.X => 0;
        int IPoint.Y => 0;
        IRenderTarget IRenderTargetHolder.Target => Target;
        #endregion

        #region CREATE RENDER ACTION
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe sealed override RenderAction CreateRenderAction(IEnumerable<IParameter> Parameters)
        {
            #region PARSE PARAMETERS
            Parameters.Extract(out IExSession session);
            int uniX = UniversalDrawOffset?.X ?? 0;
            int uniY = UniversalDrawOffset?.Y ?? 0;
            #endregion

            #region PARSE COMMAND
            var command = session.Command;

            bool Screen = (command & Command.Screen) == Command.Screen && Target != null;
            bool Calculate2 = (command & Command.Calculate2) == Command.Calculate2;
            bool XORFill = (command & Command.XORFill) == Command.XORFill;
            bool SkipDraw = (command & Command.SkipDraw) == Command.SkipDraw && !Calculate2;
            bool SkipFill = (command & Command.SkipFill) == Command.SkipFill && !Calculate2;
            bool Calculate = (command & Command.Calculate) == Command.Calculate && !Calculate2 ||
                SkipDraw && SkipFill;
            bool AutoSize = (command & Command.SizeToFit) == Command.SizeToFit;
            bool FullView = (command & Command.FullView) == Command.FullView;
            bool NoClip = (command & Command.NoClip) == Command.NoClip;
            #endregion

            #region ACTION DECLARATION
            return (lines, points, image, parameters) =>
            {
                #region ASSIGN INLINE-PARAMETERS
                if (parameters?.Length > 0)
                    session.CopyFrom(parameters, true);

                command = session.Command;
                Screen = (command & Command.Screen) == Command.Screen && Target != null;
                SkipDraw = (command & Command.SkipDraw) == Command.SkipDraw;
                SkipFill = (command & Command.SkipFill) == Command.SkipFill;
                #endregion

                #region BOUNDARY INITIALISATION
                var Boundaries = session.Boundaries.OfType<IExBoundary>();

                bool HasBoundary = session.Boundaries.Count > 0 &&
                    Boundaries.Any(bdr => bdr.Kind != BoundaryKind.AbsBoundary);
                int[] Boundary = new int[8];
                int[] AbsBoundary = new int[4];

                Boundary[0] = Boundary[1] = AbsBoundary[0] = AbsBoundary[1] = int.MaxValue;
                #endregion

                #region GET BACKGROUND PEN
                IPen BackgroundPen = Rgba.Transparent;
                #endregion

                if (!Calculate && !AutoSize)
                    goto NORMAL_RENDERING;

                #region CALCULATE ROUTINE
                if (points != null)
                {
                    Process<IAxisPointProcessArgs, RenderProcessResult> pointProcess =
                    (arg) =>
                    {
                        return 0;
                    };
                    points.Process(pointProcess, width, height, ref AbsBoundary, session, uniX, uniY);
                }
                else if (lines != null)
                {
                    Process<IAxisLineProcessArgs, RenderProcessResult> lineProcess =
                    (arg) =>
                    {
                        return 0;
                    };
                    lines.Process(lineProcess, width, height, ref AbsBoundary, session, uniX, uniY);
                }
                else if (image != null)
                {
                    int dstW = Screen ? Target.Width : Width;
                    int dstH = Screen ? Target.Height : Height;

                    Process<IImageProcessArgs, IBounds> imageProcess =
                    (arg) =>
                    {
                        var dstX = arg.DstX;
                        var dstY = arg.DstY;

                        int x = 0, y = 0, w = image.Width, h = image.Height;
                        session.CopyArea?.GetBounds(out x, out y, out w, out h);

                        if (!Blocks.CorrectRegion(
                        ref x, ref y, ref w, ref h,
                        image.Width, image.Height, ref dstX, ref dstY, dstW, dstW * dstH, out _, out _, false))
                        {
                            return null;
                        }

                        return new UpdateArea(dstX, dstY, w + 1, h + 1, session.Type);
                    };

                    imageProcess.Process(image.Source, image.Width, image.Height, dstW, dstH, ref AbsBoundary, session, uniX, uniY);
                }
                if (AutoSize && AbsBoundary[2] > 0 && AbsBoundary[3] > 0 && this is IExResizable)
                {
                    var iw = AbsBoundary[2] - AbsBoundary[0];
                    var ih = AbsBoundary[3] - AbsBoundary[1];

                    if (iw > Width || ih > Height)
                    {
                        if (iw < Width)
                            iw = Width;
                        if (ih < Height)
                            ih = Height;
                        ((IExResizable)this).Resize(iw, ih, out _, ResizeCommand.SizeOnlyToFit);
                    }
                }
                if (FullView && AbsBoundary[2] > 0 && AbsBoundary[3] > 0)
                {
                    session.UserPoint = new Point(session.UserPoint, -AbsBoundary[0], -AbsBoundary[1]);
                    goto NORMAL_RENDERING;
                }
                #endregion

                goto IMAGE_DRAW;

            NORMAL_RENDERING:
                #region GET FOREGROUND PEN
                IPenContext PenContext = session.PenContext;
                IPen Pen;
                if (PenContext is IPen)
                {
                    Pen = (IPen)PenContext;
                    //if (Pen is IBrushSettings)
                    //{
                    //    ((IBrushSettings)Pen).ReceiveSettings(Parameters);
                    //}
                }
                else if (PenContext is IBrushStyle)
                {
                    IBrush brush = Factory.newBrush((IBrushStyle)PenContext, session.RenderBounds.Width, session.RenderBounds.Height);
                    brush.ReceiveSettings(new IParameter[] { session.Rotation, session.RenderBounds, (session.Command | Command.BrushNoSizeToFit).Replace() });
                    Pen = brush;
                }
                else
                    Pen = Rgba.Black;
                session.PenContext = Pen;
                #endregion

                #region FILL LINES  
                if (!SkipFill && !XORFill && lines != null)
                {
                    FillLines(lines, session, BackgroundPen, ref AbsBoundary, ref Boundary);
                }
                #endregion

                #region DRAW POINTS
                if (!SkipDraw && !XORFill && points != null)
                {
                    DrawPoints(points, session, BackgroundPen, ref AbsBoundary, ref Boundary);
                }
            #endregion

            IMAGE_DRAW:
                #region IMAGE DRAW
                if (image != null)
                {
                    IBounds updateRect = null;
                    int srcW = image.Width;
                    int srcH = image.Height;

                    #region LOCATION & CLIP VARIABLES
                    int dstX = 0, dstY = 0;
                    if (session.UserPoint != null)
                    {
                        dstX = session.UserPoint.X;
                        dstY = session.UserPoint.Y;
                    }
                    if (session.Offset != null)
                    {
                        dstX += session.Offset.X;
                        dstY += session.Offset.Y;
                    }

                    int dstW = Screen ? Target.Width : Width;
                    int dstH = Screen ? Target.Height : Height;

                    int x = 0, y = 0, w = srcW, h = srcH;
                    if (session.CopyArea?.Valid == true)
                        session.CopyArea.GetBounds(out x, out y, out w, out h);

                    if (!Blocks.CorrectRegion(ref x, ref y, ref w, ref h, srcW, srcH,
                        ref dstX, ref dstY, dstW, dstW * dstH, out _, out _, false))
                    {
                        goto UPDATE_BOUNDARY;
                    }
                    #endregion

                    #region SOURCE DETERMINATION
                    bool ToBeRotated =
                        (session.Rotation != null && session.Rotation.Valid)
                        || (session.Scale != null && session.Scale.HasScale);

                    session.UserPoint = new Point(dstX, dstY);
                    IntPtr Source = image.Source;

                    if (ToBeRotated)
                    {
                        var res = Factory.ImageProcessor.RotateAndScale(image.Source, srcW, srcH, x, y, w, h,
                            session.Interpolation, true, session.Rotation, session.Scale, BackgroundPen, false).Result;

                        Source = res.Item2;
                        srcW = res.Item1.Width;
                        srcH = res.Item1.Height;
                    }
                    #endregion

                    #region PERFORM IMAGE DRAW
                    DrawImage
                    (
                        Source, srcW, srcH, session, dstX, dstY, x, y, w, h, BackgroundPen, ref Boundary
                    );
                    Array.Copy(Boundary, AbsBoundary, 4);
                #endregion

                UPDATE_IMAGE_AREA:
                    if (updateRect != null && updateRect.Valid)
                    {
                        if (HasBoundary)
                        {
                            foreach (var boundary in session.Boundaries.OfType<IExBoundary>())
                                boundary.Update(updateRect);
                        }
                    }
                }
            #endregion

            UPDATE_BOUNDARY:
                #region UPDATE BOUNDARY
                if (HasBoundary && Boundary[2] != 0 && Boundary[3] != 0)
                {
                    ++Boundary[2];
                    ++Boundary[3];

                    foreach (var boundary in Boundaries.
                    Where(bdr => bdr.Kind != BoundaryKind.AbsBoundary))
                    {
                        boundary.Update(Boundary);
                    }
                }
                if (AbsBoundary[2] != 0 && AbsBoundary[3] != 0)
                {
                    ++AbsBoundary[2];
                    ++AbsBoundary[3];

                    foreach (var absBoundary in Boundaries.
                    Where(bdr => bdr.Kind == BoundaryKind.AbsBoundary))
                        absBoundary.Update(AbsBoundary);
                }
                #endregion
                return true;
            };
            #endregion
        }
        #endregion

        #region DRAW POINTS
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe void DrawPoints(IEnumerable<IScanPoint> points, IExSession session, IPen BackgroundPen,
            ref int[] AbsBoundary, ref int[] Boundary)
        {
            #region COPY OF VARAIBLES TO BE USED IN PROCESS LAMBADA
            int uniX = UniversalDrawOffset?.X ?? 0;
            int uniY = UniversalDrawOffset?.Y ?? 0;
            #endregion

            #region GET TRANSPARENCY
            var transparentValue = session.Transparency;
            bool hasTransparency = transparentValue != 0;
            byte opacity = (byte)(MAX - transparentValue);
            #endregion

            #region SET BOUNDARY 
            int* boundary = null;
            fixed (int* p = Boundary)
                boundary = p;
            #endregion

            #region COMMAND PARSING
            var command = session.Command;
            bool Screen = (command & Command.Screen) == Command.Screen && Target != null;
            var Persistent = (command & Command.Persistent) == Command.Persistent;
            var Backdrop = (command & Command.Backdrop) == Command.Backdrop;
            var NoClip = (command & Command.NoClip) == Command.NoClip;
            var KeepBorderOnBackdrop = (command & Command.KeepBorderOnBackdrop) == Command.KeepBorderOnBackdrop;

            bool HoverBorder = (command & Command.HoverBorder) == Command.HoverBorder;
            bool InvertBorder = (command & Command.InvertedBorder) == Command.InvertedBorder;
            bool InvertColour = (command & Command.InvertColour) == Command.InvertColour;
            if (InvertBorder && !HoverBorder)
                InvertColour = !InvertColour;
            bool Transparent = (command & Command.Transparent) == Command.Transparent && !InvertBorder;

            var BackScreen = Backdrop && Screen;
            bool Calculate2 = (command & Command.Calculate2) == Command.Calculate2;
            #endregion

            #region BLENDING VARIABLES
            int srcColour, dstColour, bkgColour = 0, penColour = 0, originalColour = 0;
            byte iDelta = ZERO, alpha = ZERO;
            uint C1, C2, RB, AG, invAlpha;
            var Pen = session.PenContext as IPen ?? Rgba.Black;
            bool IsColour = Pen is IColour;
            if (IsColour)
                penColour = ((IColour)Pen).Colour;
            var AShift = Colours.AShift;
            #endregion

            #region SCREEN VARIABLES
            var type = session.Type;
            bool StayTop = type == ObjType.Top || type == ObjType.UnSpecified;

            int* view = (int*)(Screen ? Target.Source : Source);
            int viewW = Screen ? Target.Width : width;
            int viewH = Screen ? Target.Height : height;
            int viewLen = viewW * viewH;
            #endregion

            Process<IAxisPointProcessArgs, RenderProcessResult> PointProcess = (arg) =>
            {
                var x = arg.DstX;
                var y = arg.DstY;
                iDelta = arg.Alpha;
                bool horizontal = arg.Horizontal;
                #region GET VARS AT INDEX
                var i = x + y * viewW;
                srcColour = HoverBorder ? view[i] : Pen.ReadPixel(arg.PenX, arg.PenY);
                if
                (
                    i < 0
                    || srcColour == NOCOLOR
                )
                {
                    return RenderProcessResult.MoveNextPixel;
                }

                if (InvertColour)
                    srcColour ^= Inversion;
                srcColour = (iDelta << AShift) | (srcColour & Inversion);
                var transparency = hasTransparency;
                dstColour = originalColour = view[i];
                if (BackScreen)
                    bkgColour = BackgroundPen.ReadPixel(x, y);
                #endregion

                #region DRAW ON BUFFER
                alpha = iDelta;

                if (Backdrop && dstColour != bkgColour)
                {
                    if (KeepBorderOnBackdrop ||
                        i - 1 > 0 && view[i - 1] == bkgColour ||
                        i + 1 < viewLen && view[i + 1] == bkgColour ||
                        i - viewW > 0 && view[i - viewW] == bkgColour ||
                        i + viewW < viewLen && view[i + viewW] == bkgColour)
                    {
                        goto BLEND_SCREEN;
                    }
                    return RenderProcessResult.MoveNextPixel;
                }
                if (dstColour == bkgColour || iDelta == MAX)
                    goto ASSIGN_SCREEN;

                BLEND_SCREEN:
                #region BLEND
                C1 = (uint)dstColour;
                C2 = (uint)srcColour;
                invAlpha = (byte)(MAX - alpha);
                RB = ((invAlpha * (C1 & RBMASK)) + (alpha * (C2 & RBMASK))) >> 8;
                AG = (invAlpha * ((C1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((C2 & GMASK) >> 8)));
                srcColour = (int)((RB & RBMASK) | (AG & AGMASK));
                //if (!Back)
                iDelta = (byte)((srcColour >> AShift) & 0xFF);
            #endregion

            ASSIGN_SCREEN:
                if (Transparent)
                    goto NOTIFY;

                #region ASSIGN
                if (transparency)
                {
                    transparency = false;
                    alpha = opacity;
                    goto BLEND_SCREEN;
                }
                view[i] = (iDelta << AShift) | (srcColour & Inversion);

            NOTIFY:
                #region NOTIFY
                if (x < boundary[0])
                {
                    boundary[0] = x;
                    boundary[5] = y;
                }
                if (y < boundary[1])
                {
                    boundary[1] = y;
                    boundary[4] = x;
                }
                if (x > boundary[2])
                {
                    boundary[2] = x;
                    boundary[7] = y;
                }
                if (y > boundary[3])
                {
                    boundary[3] = y;
                    boundary[6] = x;
                }
                #endregion
                #endregion
                #endregion

                return 0;
            };

            points.Process(PointProcess, viewW, viewH, ref AbsBoundary, session, uniX, uniY);
        }
        #endregion

        #region FILL LINES
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe void FillLines
        (
            IEnumerable<IScanLine> lines, IExSession session, IPen BackgroundPen,
            ref int[] AbsBoundary, ref int[] Boundary
        )
        {
            #region COPY OF VARAIBLES TO BE USED IN PROCESS LAMBADA
            int uniX = UniversalDrawOffset?.X ?? 0;
            int uniY = UniversalDrawOffset?.Y ?? 0;
            #endregion

            #region GET TRANSPARENCY
            var transparentValue = session.Transparency;
            bool hasTransparency = transparentValue != 0;
            byte opacity = (byte)(MAX - transparentValue);
            #endregion

            #region SET BOUNDARY
            int* boundary = null;
            fixed (int* p = Boundary)
                boundary = p;
            #endregion

            #region COMMAND PARSING
            var command = session.Command;
            bool Screen = (command & Command.Screen) == Command.Screen && Target != null;
            bool Top = (command & Command.Persistent) == Command.Persistent && Target != null;
            var Backdrop = (command & Command.Backdrop) == Command.Backdrop;
            bool Calculate = (command & Command.Calculate) == Command.Calculate;

            bool Calculate2 = (command & Command.Calculate2) == Command.Calculate2;

            bool HoverBorder = (command & Command.HoverBorder) == Command.HoverBorder;
            bool InvertColour = (command & Command.InvertColour) == Command.InvertColour;
            bool Transparent = (command & Command.Transparent) == Command.Transparent;
            #endregion

            #region BLENDING VARIABLES
            int srcColour, dstColour, originalColour = 0, bkgColour = 0, penColour = 0;
            byte alpha = ZERO;
            uint C1, C2, RB, AG, invAlpha;
            var Pen = session.PenContext as IPen ?? Rgba.Black;
            bool IsColour = Pen is IColour;
            if (IsColour)
                penColour = ((IColour)Pen).Colour;
            int[] source;
            int srcIndex;
            var AShift = Colours.AShift;
            #endregion

            #region SCREEN VARIBALES
            var type = session.Type;
            bool StayTop = type == ObjType.Top;
            bool Back;
            bool BackScreen = Backdrop && Screen;
            #endregion

            #region BUFFER POINTER VAIRABLES
            int* view = (int*)(Screen ? Target.Source : Source);
            int viewW = Screen ? Target.Width : width;
            int viewH = Screen ? Target.Height : height;
            int viewLen = viewW * viewH;
            #endregion

            Process<IAxisLineProcessArgs, RenderProcessResult> LineProcess = (arg) =>
            {
                #region CALCULATE START INDEX ON DESTINATION
                var dstX = arg.DstX;
                var dstY = arg.DstY;
                var iDelta = arg.Alpha;
                var horizontal = arg.Horizontal;
                var copyLength = arg.CopyLength;
                #endregion

                #region CALCULATE START INDEX ON DESTINATION
                var x = dstX;
                var y = dstY;
                var viewIndex = dstX + dstY * viewW;
                var viewPlus = horizontal ? 1 : viewW;
                var ix = horizontal ? 1 : 0;
                var iy = !horizontal ? 1 : 0;
                var viewLast = viewIndex + viewPlus * copyLength;
                #endregion

                #region EXTRACT PEN DATA
                if (!IsColour)
                {
                    Pen.ReadLine(arg.PenStart, arg.PenStart + arg.CopyLength, arg.PenAxis,
                        horizontal, out source, out srcIndex, out copyLength);
                }
                else
                {
                    source = penColour.Repeat(copyLength);
                    srcIndex = 0;
                }
                var srcW = copyLength;
                var srcPlus = horizontal || srcW == copyLength ? 1 : srcW;
                var s = srcIndex;
                #endregion

                fixed (int* src = source)
                {
                    #region BUFFER LOOP
                    for (int i = viewIndex; i < viewLast; i += viewPlus, s += srcPlus, x += ix, y += iy)
                    {
                        #region GET CURENT DATA AT POSITION
                        dstColour = originalColour = view[i];
                        alpha = (byte)((dstColour >> AShift) & 0xFF);
                        srcColour = HoverBorder ? view[i] : src[s];
                        if (InvertColour)
                            srcColour ^= Inversion;
                        Back = (Backdrop) && !(Top || StayTop);
                        if (BackScreen)
                        {
                            bkgColour = BackgroundPen.ReadPixel(x, y);
                        }
                        #endregion

                        #region DRAW ON BUFFER
                        if (dstColour == bkgColour)
                        {
                            if (BackScreen)
                                goto SCREEN_FRONT_DRAW;
                            goto ASSIGN;
                        }

                        if (!Back)
                            goto SCREEN_FRONT_DRAW;

                        if (alpha < TWO || alpha == MAX)
                            continue;
                        alpha = (byte)(MAX - alpha);
                        goto BLEND;

                    SCREEN_FRONT_DRAW:
                        alpha = iDelta;
                        if (alpha == MAX || alpha < TWO)
                            goto ASSIGN;
                        #endregion

                        BLEND:
                        #region BLEND
                        C1 = (uint)dstColour;
                        C2 = (uint)srcColour;
                        invAlpha = (byte)(MAX - alpha);
                        RB = ((invAlpha * (C1 & RBMASK)) + (alpha * (C2 & RBMASK))) >> 8;
                        AG = (invAlpha * ((C1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((C2 & GMASK) >> 8)));
                        srcColour = (int)((RB & RBMASK) | (AG & AGMASK));
                    #endregion

                    ASSIGN:
                        #region ASSIGN
                        if (Transparent)
                            goto NOTIFY;

                        srcColour = (iDelta << AShift) | (srcColour & Inversion);
                        view[i] = srcColour;

                    NOTIFY:
                        #region NOTIFY
                        if (x < boundary[0])
                        {
                            boundary[0] = x;
                            boundary[5] = y;
                        }
                        if (y < boundary[1])
                        {
                            boundary[1] = y;
                            boundary[4] = x;
                        }
                        if (x > boundary[2])
                        {
                            boundary[2] = x;
                            boundary[7] = y;
                        }
                        if (y > boundary[3])
                        {
                            boundary[3] = y;
                            boundary[6] = x;
                        }
                        #endregion
                        #endregion
                    }
                    #endregion
                }
                return 0;
            };

            lines.Process(LineProcess, viewW, viewH, ref AbsBoundary, session, uniX, uniY);
        }
        #endregion

        #region DRAW IMAGE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe void DrawImage
        (
            IntPtr source, int srcW, int srcH, IExSession session, int dstX, int dstY, int copyX, int copyY, int copyW, int copyH,
             IPen BackgroundPen, ref int[] Boundary)
        {
            int uniX = UniversalDrawOffset?.X ?? 0;
            int uniY = UniversalDrawOffset?.Y ?? 0;

            #region COMMAND PARSING
            var command = session.Command;
            bool Backdrop = (command & Command.Backdrop) == Command.Backdrop;
            bool Screen = (command & Command.Screen) == Command.Screen && Target != null;
            bool RGBOnly = (command & Command.CopyRGBOnly) == Command.CopyRGBOnly;
            bool SwitchRB = (command & Command.SwapRedBlueChannel) == Command.SwapRedBlueChannel;
            bool BackScreen = Backdrop && Screen;
            bool SizeToFit = (command & Command.SizeToFit) == Command.SizeToFit;

            bool InvertColour = (command & Command.InvertColour) == Command.InvertColour;
            bool Transparent = (command & Command.Transparent) == Command.Transparent;
            #endregion

            #region BUFFER POINTERS     
            var boundary = Boundary;
            var src = (int*)source;
            int* dst = (int*)(Screen ? Target.Source : Source);
            int dstW = Screen ? Target.Width : width;
            int dstH = Screen ? Target.Height : height;
            int dstLen = dstW * dstH;
            #endregion

            #region LOCATION VARIABLES
            dstX += uniX;
            dstY += uniY;
            var type = session.Type;
            int srcLen = srcW * srcH;
            int srcIndex = copyX + copyY * srcW;
            int dstIndex = dstX + dstY * dstW;
            #endregion

            #region BLENDING VARIABLES
            int srcColour = 0, dstColour = 0, bkgColour = 0;
            uint C1, C2, RB, AG, invAlpha;
            byte alpha, iDelta;
            byte r, g, b;
            bool transparent = false;
            bool hasTransparency = session.Transparency != 0;
            byte opacity = (byte)(MAX - session.Transparency);
            var RShift = Colours.RShift;
            var GShift = Colours.GShift;
            var BShift = Colours.BShift;
            var AShift = Colours.AShift;
            bool IsColour = BackgroundPen is IColour;
            if (IsColour)
            {
                bkgColour = ((IColour)BackgroundPen).Colour;
            }
            #endregion

            #region LOOP
            int x = dstX;
            int y = dstY;

            for (int j = 0; j < copyH; j++, srcIndex += srcW, dstIndex += dstW, y++)
            {
                if (srcIndex >= srcLen || dstIndex >= dstLen)
                    break;

                #region RESET HORIZONTAL LINE START
                x = dstX;
                int last = srcIndex + copyW;
                int i = dstIndex;
                #endregion

                for (int s = srcIndex; s < last; i++, x++, s++)
                {
                    #region GET CURENT DATA AT POSITION
                    srcColour = src[s];
                    if (srcColour == NOCOLOR)
                        continue;

                    dstColour = dst[i];
                    var originalColour = dstColour;

                    alpha = (byte)((dstColour >> AShift) & 0xFF);
                    if (RGBOnly)
                        srcColour = (MAX << AShift) | (srcColour & Inversion);

                    iDelta = (byte)((srcColour >> AShift) & 0xFF);
                    if (SwitchRB)
                    {
                        b = (byte)((srcColour >> RShift) & 0xFF);
                        g = (byte)((srcColour >> GShift) & 0xFF);
                        r = (byte)((srcColour >> BShift) & 0xFF);
                        srcColour = (iDelta << AShift)
                                | ((r & 0xFF) << RShift)
                                | ((g & 0xFF) << GShift)
                                | ((b & 0xFF) << BShift);
                    }

                    if (InvertColour)
                        srcColour ^= Inversion;
                    transparent = hasTransparency;
                    #endregion

                    #region HANDLE BACKDROP
                    if (BackScreen && !IsColour)
                    {
                        bkgColour = BackgroundPen.ReadPixel(x, y);
                    }

                    if (dstColour == bkgColour)
                    {
                        if (BackScreen)
                            goto BUFFER_FRONT_DRAW;
                        goto ASSIGN;
                    }
                    #endregion

                    #region HANDLE FRONT DRAW
                    if (!Backdrop)
                        goto BUFFER_FRONT_DRAW;

                    if (alpha < TWO || alpha == MAX)
                        continue;
                    alpha = (byte)(MAX - alpha);
                    goto BLEND;

                BUFFER_FRONT_DRAW:
                    alpha = iDelta;
                    if (alpha == MAX || alpha < TWO)
                        goto ASSIGN;
                    #endregion

                    BLEND:
                    #region BLEND
                    C1 = (uint)dstColour;
                    C2 = (uint)srcColour;
                    invAlpha = (uint)(MAX - alpha);
                    RB = ((invAlpha * (C1 & RBMASK)) + (alpha * (C2 & RBMASK))) >> 8;
                    AG = (invAlpha * ((C1 & AGMASK) >> 8)) + (alpha * (ONEALPHA | ((C2 & GMASK) >> 8)));
                    srcColour = (int)((RB & RBMASK) | (AG & AGMASK));
                    if (!Backdrop)
                        iDelta = (byte)((srcColour >> AShift) & 0xFF);
                    #endregion

                    ASSIGN:
                    #region ASSIGN
                    if (transparent)
                    {
                        transparent = false;
                        alpha = opacity;
                        goto BLEND;
                    }
                    if (Transparent)
                        goto NOTIFY;

                    dst[i] = (iDelta << AShift) | (srcColour & Inversion);

                NOTIFY:
                    #region NOTIFY
                    if (x < boundary[0])
                    {
                        boundary[0] = x;
                        boundary[5] = y;
                    }
                    if (copyY < boundary[1])
                    {
                        boundary[1] = y;
                        boundary[4] = x;
                    }
                    if (copyX > boundary[2])
                    {
                        boundary[2] = x;
                        boundary[7] = y;
                    }
                    if (copyY > boundary[3])
                    {
                        boundary[3] = y;
                        boundary[6] = x;
                    }
                    #endregion
                    #endregion
                }
            }
            #endregion
        }
        #endregion

        #region COPY TO
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe ISize CopyTo(IntPtr destination, int dstLen, int dstW, IEnumerable<IParameter> Parameters = null)
        {
            if (destination == IntPtr.Zero || !Valid)
                return GWS.Size.Empty;

            int dstH = dstLen / dstW;

            #region PARSE PARAMETERS
            Parameters.Extract(out IExSession Info);
            int dstX = Info.UserPoint?.X ?? 0;
            int dstY = Info.UserPoint?.Y ?? 0;

            if (Info.Offset != null)
            {
                dstX += Info.Offset.X;
                dstY += Info.Offset.Y;
            }
            #endregion

            #region COMMAND PARSING
            var command = Info.Command;
            bool Screen = (command & Command.CopyScreen) == Command.CopyScreen && Target != null;
            #endregion

            #region COPY - PASTE AREA CALCULATION
            int srcW = Screen ? Target.Width : Width;
            int srcH = Screen ? Target.Height : Height;

            int x, y, w, h;
            if (
                !Blocks.CorrectRegion(srcW, srcH, null, null, ref dstX, ref dstY, dstW,
                dstH, out x, out y, out w, out h, out _, out _))
            {
                return null;
            }
            #endregion

            #region GET BACKGROUND PEN
            IPen BackgroundPen = null;
            if (Info.PenContext is IPen)
                BackgroundPen = ((IPen)Info.PenContext);
            if (BackgroundPen == null)
            {
                BackgroundPen = Rgba.Transparent;
                if (this is IBackgroundPenHolder)
                    BackgroundPen = ((IBackgroundPenHolder)this).BackgroundPen;
            }
            Info.PenContext = BackgroundPen;
            #endregion

            var src = Extract(Info, out srcW, out srcH);
            Blocks.CopyBlock(src, new Rectangle(0, 0, srcW, srcH), srcW * srcH,
                srcW, srcH, destination, dstX, dstY, dstW, dstLen, command.ToEnum<CopyCommand>());
            return new Size(srcW, srcH);
        }
        #endregion

        #region UPDATE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Update(IBounds area, UpdateCommand command)
        {
            if ((viewState & ViewState.Disposed) == ViewState.Disposed ||
                (viewState & ViewState.Loading) == ViewState.Loading)
                return;
            if (this is IRefreshable && (
                (viewState & ViewState.FullyLoaded) != ViewState.FullyLoaded
                && (viewState & ViewState.Loading) != ViewState.Loading))
            {
                ((IRefreshable)this).Refresh(command &= ~(UpdateCommand.SkipDisplayUpdate));
                return;
            }
            IBounds Current = area ?? new UpdateArea(0, 0, width, height);
            if (!Current.Valid)
                return;

            if ((command & UpdateCommand.UpdateScreenOnly) != UpdateCommand.UpdateScreenOnly && Target != null)
            {
                Current.GetBounds(out int x, out int y, out int w, out int h);
                CopyTo
                (
                    Target.Source, Target.Width * Target.Height, Target.Width,
                    new IParameter[]
                    {
                    (Command.CopyContentOnly).Add(),
                    new Point(x, y),
                    new Area(x,y,w,h)
                    }
                );
            }

            if ((viewState & ViewState.Hidden) == ViewState.Hidden)
                return;

            if ((command & UpdateCommand.SkipDisplayUpdate) != UpdateCommand.SkipDisplayUpdate && Target != null)
                Target.Update(Current, command);
        }
        #endregion

        #region CLEAR
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void Clear(IEnumerable<IParameter> parameters)
        {
            if ((viewState & ViewState.Disposed) == ViewState.Disposed)
                return;

            #region PARSE PARAMETERS
            parameters.Extract(out IExSession info);
            #endregion

            bool SkipForeground = (info.Command & Command.SkipDesktop) == Command.SkipDesktop;
            int x = 0;
            int y = 0;
            int w = width;
            int h = height;
            if (info.CopyArea != null && info.CopyArea.Valid)
                info.CopyArea.GetBounds(out x, out y, out w, out h);
            var rc = Rectangles.CompitibleRc(width, height, x, y, w, h);
            rc.GetBounds(out x, out y, out w, out h);

            #region PEN INITIALIZATION
            IPen pen = null;
            if (info.PenContext != null)
            {
                if (info.PenContext is IPen)
                    pen = (IPen)info.PenContext;
                else
                    pen = info.PenContext.ToPen(w, h);
            }
            if (pen == null)
            {
                pen = Rgba.Transparent;
            }
            #endregion

            info.CopyArea = rc;
            info.UserPoint = new Point(rc);

            var length = width * height;

            if (Target != null)
                pen.CopyTo(Target.Source, Target.Width * Target.Height, Target.Width, info);

            if (!SkipForeground)
            {
                Blocks.CopyBlock(IntPtr.Zero, rc, length, width, height, Source, x, y, width,
                    length, info.Command.ToEnum<CopyCommand>());
            }

            if ((info.Command & Command.SkipDisplayUpdate) != Command.SkipDisplayUpdate)
                Target?.Update(new UpdateArea(rc), 0);
        }
        #endregion

        #region EXTRACT
        unsafe IntPtr Extract(IExSession session, out int viewW, out int viewH)
        {
            viewW = width;
            viewH = height;

            #region COMMAND PARSING
            var command = session.Command;
            bool InvertColour = (command & Command.InvertColour) == Command.InvertColour;
            bool Screen = (command & Command.CopyScreen) == Command.CopyScreen && Target != null;
            bool RGBOnly = (command & Command.CopyRGBOnly) == Command.CopyRGBOnly;
            bool EdgeDetectable = (command & Command.CopyBackground) != Command.CopyBackground &&
                (command & Command.CopyContentOnly) != Command.CopyContentOnly;
            bool SwitchRB = (command & Command.SwapRedBlueChannel) == Command.SwapRedBlueChannel;
            bool RestoreView = (command & Command.RestoreView) == Command.RestoreView;
            bool External = Target == null;
            var invertRotation = (command & Command.BrushInvertRotation) == Command.BrushInvertRotation;
            var SizeToFit = (command & Command.SizeToFit) == Command.SizeToFit;
            bool ToBeRotated =
                (command & Command.SkipRotateScale) != Command.SkipRotateScale &&
                ((session.Rotation != null && session.Rotation.Valid)
                    || (session.Scale != null && session.Scale.HasScale)
                    || (session.ImageSize != null && session.ImageSize.Valid)
                );
            #endregion

            #region INITIALIZE TRANSPARENCY FLAGS
            bool transparent = false;
            bool hasTransparency = session.Transparency != ZERO;
            byte opacity = (byte)(MAX - session.Transparency);
            #endregion

            #region BLENDING VARIABLES
            int dstColour, srcColour, penColour = 0;
            byte iDelta;
            uint C1, C2, invAlpha, RB, AG;
            byte r, g, b;
            int
                RShift = Colours.RShift,
                GShift = Colours.GShift,
                BShift = Colours.BShift,
                AShift = Colours.AShift;
            #endregion

            #region GET BACKGROUND PEN
            IPen BkgPen = null;
            if (session.PenContext is IPen)
                BkgPen = ((IPen)session.PenContext);
            if (BkgPen == null)
            {
                BkgPen = Rgba.Transparent;
                if (this is IBackgroundPenHolder)
                    BkgPen = ((IBackgroundPenHolder)this).BackgroundPen;
            }

            bool IsColour = BkgPen is IColour;
            if (IsColour)
            {
                penColour = ((IColour)BkgPen).Colour;
                if (SwitchRB)
                {
                    b = (byte)((penColour >> RShift) & 0xFF);
                    g = (byte)((penColour >> GShift) & 0xFF);
                    r = (byte)((penColour >> BShift) & 0xFF);
                    penColour = (MAX << AShift)
                            | ((r & 0xFF) << RShift)
                            | ((g & 0xFF) << GShift)
                            | ((b & 0xFF) << BShift);
                }
            }
            #endregion

            #region VARIABLE INITIALIZATION
            viewW = Screen ? Target.Width : Width;
            viewH = Screen ? Target.Height : Height;
            int copyX = 0, copyY = 0, copyW = viewW, copyH = viewH;
            session.CopyArea?.GetBounds(out copyX, out copyY, out copyW, out copyH);
            int dstX = 0, dstY = 0, dstW = copyW, dstH = copyH;
            int* view = (int*)(Screen ? Target.Source : Source);
            if (!Blocks.CorrectRegion(ref copyX, ref copyY, ref copyW, ref copyH, viewW, viewH,
                ref dstX, ref dstY, dstW, dstW * dstH, out _, out _, false))
            {
                return IntPtr.Zero;
            }
            dstW = copyW;
            dstH = copyH;
            int x = copyX;
            int y = copyY;
            #endregion

            fixed (int* dst = new int[dstW * dstH])
            {
                var dstIndex = dstX + dstY * dstW;
                var srcIndex = copyX + copyY * viewW;
                //int dstLen = dstW * dstH;

                #region LOOP
                for (int i = 0; i < dstH; i++, srcIndex += viewW, dstIndex += dstW, y++)
                {
                    int s = srcIndex;
                    int last = dstIndex + copyW;
                    x = copyX;

                    for (int t = dstIndex; t < last; t++, s++, x++)
                    {
                        srcColour = view[s];
                        iDelta = MAX;
                        if (srcColour == NOCOLOR)
                            goto DETERMINE_EDGE_DETECTION;
                        else
                        {

                        }
                        if (RGBOnly)
                            srcColour = (MAX << AShift) | (srcColour & Inversion);
                        iDelta = (byte)((srcColour >> AShift) & 0xFF);

                        if (SwitchRB)
                        {
                            b = (byte)((srcColour >> RShift) & 0xFF);
                            g = (byte)((srcColour >> GShift) & 0xFF);
                            r = (byte)((srcColour >> BShift) & 0xFF);
                            srcColour = (iDelta << AShift)
                                    | ((r & 0xFF) << RShift)
                                    | ((g & 0xFF) << GShift)
                                    | ((b & 0xFF) << BShift);
                        }

                    DETERMINE_EDGE_DETECTION:
                        if (EdgeDetectable)
                        {
                            dst[t] = srcColour;
                            continue;
                        }

                        if (dst[t] == NOCOLOR || !RestoreView)
                        {
                            if (!IsColour)
                            {
                                penColour = BkgPen.ReadPixel(x, y);
                                if (SwitchRB)
                                {
                                    b = (byte)((penColour >> RShift) & 0xFF);
                                    g = (byte)((penColour >> GShift) & 0xFF);
                                    r = (byte)((penColour >> BShift) & 0xFF);
                                    penColour = (MAX << AShift)
                                            | ((r & 0xFF) << RShift)
                                            | ((g & 0xFF) << GShift)
                                            | ((b & 0xFF) << BShift);
                                }
                            }
                            dst[t] = penColour;
                        }
                        dstColour = dst[t];

                        if (srcColour == NOCOLOR)
                            continue;

                        if (iDelta < TWO || iDelta == MAX)
                            goto ASSIGN;

                        BLEND:
                        C1 = (uint)dstColour;
                        C2 = (uint)srcColour;
                        invAlpha = (uint)(MAX - iDelta);
                        RB = ((invAlpha * (C1 & RBMASK)) + (iDelta * (C2 & RBMASK))) >> 8;
                        AG = (invAlpha * ((C1 & AGMASK) >> 8)) + (iDelta * (ONEALPHA | ((C2 & GMASK) >> 8)));
                        srcColour = (int)((RB & RBMASK) | (AG & AGMASK));

                        if (!External)
                            srcColour = (iDelta << AShift) | (srcColour & Inversion);

                        ASSIGN:
                        if (transparent)
                        {
                            transparent = false;
                            iDelta = opacity;
                            goto BLEND;
                        }

                        if (InvertColour)
                            srcColour ^= Inversion;
                        dst[t] = srcColour;
                    }
                }
                #endregion

                viewW = dstW;
                viewH = dstH;
                session.CopyArea = null;

                #region IF ROTATE OR SCALE
                if (ToBeRotated)
                {
                    int newWidth = dstW, newHeight = dstH;
                    if (session.ImageSize != null && session.ImageSize.Valid)
                    {
                        newWidth = session.ImageSize.Width;
                        newHeight = session.ImageSize.Height;
                    }
                    var res = Factory.ImageProcessor.RotateAndScale
                    (
                        (IntPtr)dst, dstW, dstH,
                        0, 0, newWidth, newHeight,
                        session.Interpolation,
                        SizeToFit, session.Rotation, session.Scale,
                        BkgPen, false
                    );

                    viewW = res.Result.Item1.Width;
                    viewH = res.Result.Item1.Height;
                    return res.Result.Item2;
                }
                #endregion

                return (IntPtr)dst;
            }
        }
        IntPtr IExtracter<IntPtr>.Extract(IExSession session, out int srcW, out int srcH) =>
            Extract(session, out srcW, out srcH);
        #endregion

        #region GATHER EDGES
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe VectorF[] GatherEdges(int X, int Y, int W, int H, params IParameter[] parameters)
        {
            int di = 0;
            int i;
            int colour;

            int srcLen = width * height;
            PrimitiveList<VectorF> pts = new PrimitiveList<VectorF>((W + H) * 2);
            float fx, fy;
            int bottom = Y + H;
            int right = X + W;
            parameters.Extract(out IExSession session);
            var Data = (int*)Extract(session, out int srcW, out int srcH);

            for (int y = Y; y < bottom; y++, di += srcW)
            {
                i = di;
                for (int x = X; x < right; x++, i++)
                {
                    colour = Data[i];
                    if (colour == 0)
                        continue;

                    var alpha = Colours.Alphas[(byte)((colour >> Colours.AShift) & 0xFF)];
                    fx = x;
                    fy = y;

                    if (i - 1 >= 0 && Data[i - 1] == 0)
                        fx -= alpha;
                    else if (i - srcW >= 0 && Data[i - srcW] == 0)
                        fy -= alpha;
                    else if (i + 1 < srcLen && Data[i + 1] == 0)
                        fx += alpha;
                    else if (i + srcW < srcLen && Data[i + srcW] == 0)
                        fy += alpha;
                    else
                        continue;

                    pts.Add(new VectorF(fx, fy, PointKind.Edge));
                }
            }
            var array = new VectorF[pts.Count];
            Array.Copy(((IArrayHolder<VectorF>)pts).Data, array, array.Length);
            return array;
        }
        #endregion

        #region ROTATE AND SCALE
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<Tuple<ISize, IntPtr>> RotateAndScale(IEnumerable<IParameter> parameters)
        {
            parameters.Extract(out IExSession session);

            #region PARSE COMMAND
            bool SizeToFit = (session.Command & Command.SizeToFit) == Command.SizeToFit;
            bool Screen = (session.Command & Command.Screen) == Command.Screen && Target != null;
            #endregion

            #region GET BACKGROUND PEN
            IPen BackgroundPen = null;
            if (session.PenContext is IPen)
                BackgroundPen = (IPen)session.PenContext;
            #endregion

            session.Command |= Command.SkipRotateScale;
            var Source = ((IExtracter<IntPtr>)this).Extract(session, out int srcW, out int srcH);

            if (Source == IntPtr.Zero)
                return Tuple.Create((ISize)GWS.Size.Empty, IntPtr.Zero);
            int x = 0;
            int y = 0;
            int w = srcW;
            int h = srcH;

            if (w > srcW)
                w = srcW;

            if (h > srcH)
                h = srcH;

            if (x + w > srcW)
                w = (srcW - x);
            if (y + h > srcH)
                h = (srcH - y);

            if (w < 0 || h < 0)
                return Tuple.Create((ISize)GWS.Size.Empty, IntPtr.Zero);

            var result = await Factory.ImageProcessor.RotateAndScale
                 (
                     Source,
                     srcW, srcH,
                     x, y, w, h,
                     session.Interpolation,
                     SizeToFit,
                     session.Rotation,
                     session.Scale,
                     BackgroundPen, false
                 );
            return result;
        }
        Tuple<ISize, IntPtr> IRotatableSource.RotateAndScale(IEnumerable<IParameter> parameters)
        {
            var r = RotateAndScale(parameters);
            return r.Result;
        }
        #endregion

        #region RESIZE   
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal virtual object Resize(int w, int h, out bool success, ResizeCommand resizeCommand)
        {
            success = false;
            if (
               (viewState & ViewState.Disposed) == ViewState.Disposed ||
               (w == Width && h == Height) ||
               (w == 0 && h == 0))
                return this;

            bool SizeOnlyToFit = (resizeCommand & ResizeCommand.SizeOnlyToFit) == ResizeCommand.SizeOnlyToFit;

            if (SizeOnlyToFit && Width > w && Height > h)
                return this;

            var State = viewState;
            viewState |= ViewState.Disposed;

            bool AutoReSizeContent = !SizeOnlyToFit && (resizeCommand & ResizeCommand.AutoReSizeContent) == ResizeCommand.AutoReSizeContent;
            bool NotLessThanOriginal = !SizeOnlyToFit && (resizeCommand & ResizeCommand.NotLessThanOriginal) == ResizeCommand.NotLessThanOriginal;

            if (SizeOnlyToFit)
            {
                if (w < Width)
                    w = Width;
                if (h < Height)
                    h = Height;
            }
            if (NotLessThanOriginal)
            {
                if (w < OriginalWidth)
                    w = OriginalWidth;
                if (h < OriginalHeight)
                    h = OriginalHeight;
            }

            if (AutoReSizeContent
            )
            {
                Interpolation interpolation = 0;
                if ((resizeCommand & ResizeCommand.AutoReSizeContentBicubic) == ResizeCommand.AutoReSizeContentBicubic)
                {
                    interpolation = Interpolation.Bicubic;
                }
                else if ((resizeCommand & ResizeCommand.AutoReSizeContentBilinear) == ResizeCommand.AutoReSizeContentBilinear)
                {
                    interpolation = Interpolation.Bilinear;
                }
                var t = Factory.ImageProcessor.RotateAndScale
                (
                    ((ISource<IntPtr>)this).Source, Width, Height, 0, 0, Width, Height, interpolation,
                    SizeOnlyToFit, null, new Scale(Width, Height, w, h)
                ).Result;
                w = t.Item1.Width;
                h = t.Item1.Height;
                width = w;
                height = h;
                Pixels = new int[width * height];
                Blocks.Copy(t.Item2, 0, ((ISource<IntPtr>)this).Source, 0, w * h);
            }
            else
            {
                Blocks.ResizedData(ref Pixels, w, h, width, height);
                width = w;
                height = h;
            }
            viewState = State;
            success = true;
            return this;
        }
        object IExResizable.Resize(int w, int h, out bool success, ResizeCommand resizeCommand)=>
            Resize(w, h, out success, resizeCommand);
        #endregion

        #region CLONE
        public virtual object Clone()
        {
            if ((viewState & ViewState.Disposed) == ViewState.Disposed)
                return null;

            Canvas target = new Canvas(width, height);

            Array.Copy(Pixels, target.Pixels, width * height);
            return target;
        }
        #endregion

        #region GET ORIGIN-BASED VERSION
        IOriginCompatible IOriginCompatible.GetOriginBasedVersion() => this;
        #endregion

        #region DISPOSE
        public virtual void Dispose()
        {
            if ((viewState & ViewState.Disposed) != ViewState.Disposed)
            {
                viewState |= ViewState.Disposed;
                Pixels = null;
            }
        }
        #endregion
    }
}
#endif