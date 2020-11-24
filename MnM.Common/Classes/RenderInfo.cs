/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if (GWS || Window)
    public class RenderInfo: IRenderInfo
    {
        #region VARIABLES
        public int X, Y;
        public Rotation Rotation;
        public StrokeMode StrokeMode;
        public VectorF Scale;
        public Rectangle Bounds, RecentlyDrawn;
        public Size Clip;

        public IPenContext Foreground;
        public string ShapeID;

        protected DrawCommand drawCommand, calculatedDrawCommand;
        private FillMode fillMode;
        private float stroke;
        #endregion

        #region CONSTRUCTORS
        public RenderInfo() { }
        public RenderInfo(string shapeID)
        {
            ShapeID = shapeID;
            drawCommand = DrawCommand.OddEven;
        }
        #endregion

        #region PROPERTIES
        public DrawCommand Command
        {
            get => calculatedDrawCommand;
            set
            {
                if (drawCommand == value)
                    return;

                drawCommand =  value;
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
        Rotation IRenderInfo.Rotation
        {
            get => Rotation;
            set => Rotation = value;
        }
        string IRenderInfo.ShapeID
        {
            get => ShapeID;
            set => ShapeID = value;
        }
        StrokeMode IRenderInfo.StrokeMode 
        { 
            get => StrokeMode;
            set => StrokeMode = value;
        }
        VectorF IRenderInfo.Scale 
        {
            get => Scale; 
            set => Scale = value;
        }
        IPenContext IRenderInfo.Foreground 
        { 
            get => Foreground; 
            set => Foreground = value; 
        }
        int IRenderInfo.X 
        { 
            get => X; 
            set => X = value; 
        }
        int IRenderInfo.Y
        { 
            get => Y; 
            set => Y = value; 
        }
        Rectangle IRenderInfo.Bounds 
        { 
            get => Bounds; 
            set => Bounds = value;
        }
        Rectangle IRenderInfo.RecentlyDrawn
        {
            get => RecentlyDrawn;
            set => RecentlyDrawn = value;
        }
        Size IPolyInfo.Clip 
        {
            get => Clip;
            set => Clip = value;
        }

        public string PenID => (Foreground as IPen)?.ID ?? null;
        int IOffset.X => X;
        int IOffset.Y => Y;
        Rotation IRotatable.Rotation => Rotation;
        #endregion

        #region SYNC PATTERN AND ANTIALIAS SETTINGS
        protected virtual void SyncCommand()
        {
            calculatedDrawCommand = drawCommand;

            if ((drawCommand & DrawCommand.IgnoreAutoCalculatedFillPatten) == DrawCommand.IgnoreAutoCalculatedFillPatten)
                return;
            if (stroke == 0 || fillMode == FillMode.Original ||
                (drawCommand & DrawCommand.KeepFillRuleForStroking) == DrawCommand.KeepFillRuleForStroking)
            {
                calculatedDrawCommand &= ~DrawCommand.Outlininig;
                return;
            }
            if (stroke != 0 && fillMode == FillMode.FillOutLine)
                calculatedDrawCommand |= DrawCommand.Outlininig;
        }
        #endregion

        #region COPY SETTINGS
        public virtual void CopySettings(ISettable settings, bool flushMode = false)
        {
            if (settings == null || flushMode)
                goto Flush;

            if (settings is IOffset)
            {
                X = (settings as IOffset).X;
                Y = (settings as IOffset).Y;
            }
            if (settings is IRenderInfo)
            {
                var info = settings as IRenderInfo;
                stroke = info.Stroke;
                fillMode = info.FillMode;
                StrokeMode = info.StrokeMode;
                Bounds = info.Bounds;
                Scale = info.Scale;
                Rotation = info.Rotation;
                drawCommand = info.Command;
                CleanCommand();
                RecentlyDrawn = info.RecentlyDrawn;
                Foreground = info.Foreground;
            }
            Flush:
            SyncCommand();
        }
        #endregion

        #region CLEAN DRAW COMMAND
        public void CleanCommand()
        {
            drawCommand &= ~DrawCommand.IgnoreAutoCalculatedFillPatten;
            drawCommand &= ~DrawCommand.KeepFillRuleForStroking;
            drawCommand &= ~DrawCommand.Calculate;
            drawCommand &= ~DrawCommand.KeepFillRuleForStroking;
            drawCommand &= ~DrawCommand.IgnoreAutoCalculatedFillPatten;
            drawCommand &= ~DrawCommand.Outlininig;
            drawCommand &= ~DrawCommand.DrawEndsOnly;
            drawCommand &= ~DrawCommand.DrawLineOnly;
            drawCommand &= ~DrawCommand.NoSorting;
            drawCommand &= ~DrawCommand.CheckForCloseness;
            drawCommand &= ~DrawCommand.FillSinglePointLine;
#if Advanced
            drawCommand &= ~DrawCommand.EraseControl;
            drawCommand &= ~DrawCommand.RemoveControl;
            drawCommand &= ~DrawCommand.RestoreControl;
            drawCommand &= ~DrawCommand.SuspendUpdate;
#endif
            SyncCommand();
        }
        #endregion

        #region ADD - REMOVE COMMAD
        public void AddCommands(params DrawCommand[] commands)
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
        public void RemoveCommands(params DrawCommand[] commands)
        {
            if (commands.Length == 0)
                return;
            foreach (var item in commands)
                drawCommand &= ~item;

            SyncCommand();
        }
        #endregion
    }
#endif
}