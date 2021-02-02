/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice must not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window

namespace MnM.GWS
{
    public sealed partial class Settings : ISettings
    {
        #region VARIABLES
        Command drawCommand, calculatedDrawCommand;
        private FillMode fillMode;
        private float stroke;
        #endregion

        #region CONSTRUCTORS
        internal Settings(int shapeID, IPenContext context)
        {
            Boundary = Factory.newBoundary();
            ShapeID = shapeID;
            drawCommand = Command.OddEven;
            PenContext = context;
        }
        #endregion

        #region PROPERTIES
        public Command Command
        {
            get => calculatedDrawCommand;
            set
            {
                if (drawCommand == value)
                    return;

                drawCommand = value;
                SyncCommand();
            }
        }
        public FillMode FillMode
        {
            get => fillMode;
            set
            {
                fillMode = value;
                SyncCommand();
            }
        }
        public float Stroke
        {
            get => stroke;
            set
            {
                stroke = value;
                SyncCommand();
            }
        }
        public Rotation Rotation { get; set; }
        public int ShapeID 
        {
            get => Boundary.ShapeID;
            set => Boundary.ShapeID = value; 
        }
        public StrokeMode StrokeMode { get; set; }
        public VectorF Scale { get; set; }
        public IPenContext PenContext { get; set; }
        public IRectangle Bounds { get; set; }
        public IBoundary Boundary { get; private set; }
        public Size Clip { get; set; }
        #endregion

        #region SYNC PATTERN AND ANTIALIAS SETTINGS
        void SyncCommand()
        {
            calculatedDrawCommand = drawCommand;
            bool IgnoreAutoCalculatedFillPatten =
                (drawCommand & Command.IgnoreAutoCalculatedFillPatten) == Command.IgnoreAutoCalculatedFillPatten;
            bool EraseControl = (drawCommand & Command.Erase) == Command.Erase;
            bool KeepFillRuleForStroking = (drawCommand & Command.KeepFillRuleForStroking) == Command.KeepFillRuleForStroking;
            if (IgnoreAutoCalculatedFillPatten)
                return;
            if (stroke == 0 || fillMode == FillMode.Original || KeepFillRuleForStroking || EraseControl)
            {
                calculatedDrawCommand &= ~Command.Outlininig;
                return;
            }
            if (stroke != 0 && fillMode == FillMode.FillOutLine)
                calculatedDrawCommand |= Command.Outlininig;
        }
        #endregion

        #region COPY SETTINGS
        public void Receive(IDrawParams settings, bool flushMode = false)
        {
            if (settings == null || flushMode)
                goto Flush;

            if (settings is IPoint)
            {
                Boundary.DstX = ((IPoint)settings).X;
                Boundary.DstY = ((IPoint)settings).Y;
            }
            if (settings is IDrawnArea)
            {
                Boundary.DstX = ((IDrawnArea)settings).Boundary.DstX;
                Boundary.DstY = ((IDrawnArea)settings).Boundary.DstY;
            }
            if (settings is ISettings)
            {
                var info = settings as ISettings;
                stroke = info.Stroke;
                fillMode = info.FillMode;
                StrokeMode = info.StrokeMode;
                Scale = info.Scale;
                Rotation = info.Rotation;
                drawCommand = info.Command;
                CleanCommand();
                Boundary.Copy(info.Boundary);
                PenContext = info.PenContext;
            }
        Flush:
            SyncCommand();
        }
        #endregion

        #region CLEAN DRAW COMMAND
        public void CleanCommand()
        {
            drawCommand &= ~Command.IgnoreAutoCalculatedFillPatten;
            drawCommand &= ~Command.KeepFillRuleForStroking;
            drawCommand &= ~Command.CalculateOnly;
            drawCommand &= ~Command.KeepFillRuleForStroking;
            drawCommand &= ~Command.IgnoreAutoCalculatedFillPatten;
            drawCommand &= ~Command.Outlininig;
            drawCommand &= ~Command.DrawEndsOnly;
            drawCommand &= ~Command.DrawLineOnly;
            drawCommand &= ~Command.NoSorting;
            drawCommand &= ~Command.CheckForCloseness;
            drawCommand &= ~Command.FillSinglePointLine;
            drawCommand &= ~Command.Erase;
            drawCommand &= ~Command.Restore;
            drawCommand &= ~Command.AddMode;
            SyncCommand();
        }
        #endregion

        #region ADD - REMOVE COMMAD
        public void AddCommands(params Command[] commands)
        {
            if (commands.Length == 0)
                return;
            foreach (var item in commands)
            {
                if (item == 0)
                    continue;
                drawCommand |= item;
            }
            SyncCommand();
        }
        public void RemoveCommands(params Command[] commands)
        {
            if (commands.Length == 0)
                return;
            foreach (var item in commands)
                drawCommand &= ~item;

            SyncCommand();
        }
        #endregion

        #region FLUSH
        public void Flush()
        {
            drawCommand = calculatedDrawCommand = 0;
            stroke = 0;
            StrokeMode = StrokeMode.StrokeMiddle;
            fillMode = FillMode.Original;
            Rotation = Rotation.Empty;
            Scale = VectorF.Empty;
            Boundary.DstX = Boundary.DstY = 0;
            Bounds = Rectangle.Empty;
            ShapeID = 0;
            Boundary.ClearBounds();
        }
        #endregion
    }
}
#endif
