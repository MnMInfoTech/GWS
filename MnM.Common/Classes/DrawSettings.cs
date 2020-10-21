/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if(GWS || Window)
    public class DrawSettings : IDrawSettings
    {
        #region VARIABLES
        protected DrawCommand drawCommand;
        protected FillCommand fillCommand;
        protected LineCommand lineCommand;
        protected BrushCommand brushCommand;

        protected DrawCommand previousDrawCommand;
        protected FillCommand calculatedFillCommand;

        private FillMode fillMode;
        private float stroke;

        protected string bufferID;
        protected string penID, shapeID;
#endregion

        #region CONSTRUCTORS
        public DrawSettings()
        {
            lineCommand = 0;
            fillCommand = FillCommand.OddEven;
        }
        public DrawSettings(IRenderable renderable, string bufferID)
            : this()
        {
            shapeID = renderable.ID;
            this.bufferID = bufferID;
        }
        #endregion

        #region PROPERTIES
        public DrawCommand DrawCommand
        {
            get => drawCommand;
            set
            {
                if (drawCommand == value)
                    return;

                if (value == 0)
                    value = previousDrawCommand;

                if (value.HasFlag(DrawCommand.Permanent))
                {
                    value &= ~DrawCommand.Permanent;
                    previousDrawCommand = value;
                }
                drawCommand =  value;
                SyncDrawCommand();
            }
        }
        public BrushCommand BrushCommand
        {
            get => brushCommand;
            set
            {
                bool sync = value.HasFlag(BrushCommand.IgnoreAutoCalculatedFillPatten) !=
                    brushCommand.HasFlag(BrushCommand.IgnoreAutoCalculatedFillPatten) ||
                    value.HasFlag(BrushCommand.KeepFillRuleForStroking) !=
                    brushCommand.HasFlag(BrushCommand.KeepFillRuleForStroking);

                brushCommand = value;
                if (sync)
                    SyncFillCommand();
            }
        }
        public FillCommand FillCommand
        {
            get
            {
                return brushCommand.HasFlag(BrushCommand.IgnoreAutoCalculatedFillPatten) ? fillCommand : calculatedFillCommand;
            }
            set
            {
                if (value.HasFlag(FillCommand.DrawEndsOnly))
                    value &= ~FillCommand.DrawLineOnly;
                if (value.HasFlag(FillCommand.DrawLineOnly))
                    value &= ~FillCommand.DrawEndsOnly;
                fillCommand = value;
                SyncFillCommand();
            }
        }
        public LineCommand LineCommand
        {
            get => lineCommand;
            set
            {
                lineCommand = value;
                SyncLineCommand();
            }
        }

        public IReadContext Foreground { get; set; }
        public StrokeMode StrokeMode { get; set; }
        public FillMode FillMode
        {
            get => fillMode;
            set
            {
                fillMode = value;
                SyncFillCommand();
            }
        }
        public float Stroke
        {
            get => stroke;
            set
            {
                stroke = value;
                SyncFillCommand();
            }
        }
        public Rotation Rotation { get; set; }
        public VectorF Scale { get; set; }
        public virtual int X { get; set; }
        public virtual int Y { get; set; }
        public Size Clip { get; set; }

        public bool FreezeSettings { get; set; }

        public string PenID
        {
            get => penID;
            set => penID = value;
        }
        public string BufferID
        {
            get => bufferID;
            set => bufferID = value;
        }
        public string ShapeID
        {
            get => shapeID;
            set => shapeID = value;
        }
        public Rectangle Bounds { get; set; }
        public virtual Rectangle RecentlyDrawn { get; protected set; }
        int IID<int>.ID => 0;
        #endregion

        #region SYNC PATTERN AND ANTIALIAS SETTINGS
        protected virtual void SyncDrawCommand() { }
        protected virtual void SyncLineCommand() { }
        protected virtual void SyncFillCommand()
        {
            calculatedFillCommand = fillCommand;

            if (brushCommand.HasFlag(BrushCommand.IgnoreAutoCalculatedFillPatten))
                return;
            if (stroke == 0 || fillMode == FillMode.Original ||
                brushCommand.HasFlag(BrushCommand.KeepFillRuleForStroking))
            {
                calculatedFillCommand &= ~FillCommand.Outlininig;
                return;
            }
            if (stroke != 0 && fillMode == FillMode.FillOutLine)
                calculatedFillCommand |= FillCommand.Outlininig;
        }
        #endregion

        #region GET FILL PARAMS
        public virtual void GetFillParameters(out bool CheckForCloseness, out bool LineOnly, out bool EndsOnly)
        {
            CheckForCloseness = fillCommand.HasFlag(FillCommand.CheckForCloseness);
            LineOnly = fillCommand.HasFlag(FillCommand.DrawLineOnly);
            EndsOnly = fillCommand.HasFlag(FillCommand.DrawEndsOnly);
        }
        #endregion

        #region COPY SETTINGS
        public virtual void CopySettings(ISettable settings, bool flushMode = false)
        {
            if (settings == null || flushMode)
                goto mks;

            if (settings is IOffset)
            {
                X = (settings as IOffset).X;
                Y = (settings as IOffset).Y;
            }
            if (settings is IDrawInfo)
            {
                var info = settings as IDrawInfo;
                stroke = info.Stroke;
                fillMode = info.FillMode;
                StrokeMode = info.StrokeMode;
                penID = info.PenID;
                bufferID = info.BufferID;
                shapeID = info.ShapeID;
                Bounds = info.Bounds;
                Scale = info.Scale;
                Rotation = info.Rotation;
                RecentlyDrawn = info.RecentlyDrawn;
            }
            if(settings is IRenderInfo)
            {
                var info = settings as IRenderInfo;
                fillCommand = calculatedFillCommand = info.FillCommand;
                brushCommand = info.BrushCommand;
                lineCommand = info.LineCommand;
                drawCommand = info.DrawCommand;
            }
            SyncDrawCommand();
            SyncLineCommand();
            SyncFillCommand();

            return;

        mks:
            Flush();
        }
        #endregion

        #region FLUSH
        public virtual void Flush()
        {
            penID = null;
            bufferID = null;
            shapeID = null;
            Clip = Size.Empty;
            
            brushCommand &= ~BrushCommand.IgnoreAutoCalculatedFillPatten;

            if (FreezeSettings)
                return;
#if Advanced
            brushCommand &= ~BrushCommand.NoAutoSizing;
#endif

            brushCommand &= ~BrushCommand.InvertRotation;
            brushCommand &= ~BrushCommand.KeepFillRuleForStroking;
            brushCommand &= ~BrushCommand.InvertColor;

            X = Y = 0;
            Scale = Vector.Empty;
            StrokeMode = StrokeMode.StrokeMiddle;
            fillMode = FillMode.Original;
            stroke = 0;
            lineCommand = LineCommand.None;
            fillCommand = FillCommand.OddEven;

            Rotation = Rotation.Empty;
            SyncLineCommand();
            SyncFillCommand();
        }
        #endregion
    }
#endif
}
