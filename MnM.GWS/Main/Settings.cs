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
        Command DrawCommand, CalculatedDrawCommand;
        private FillMode fillMode;
        private float stroke;
        Session session = new Session();
        #endregion

        #region CONSTRUCTORS
        internal Settings() { }
        internal Settings(uint shapeID, IPenContext context):
            this(shapeID)
        {
            PenContext = context;
        }
        internal Settings(uint shapeID)
        {
            ShapeID = shapeID;
            DrawCommand = Command.OddEven;
            Initialize();
        }
        partial void Initialize();
        #endregion

        #region PROPERTIES
        public Command Command
        {
            get => CalculatedDrawCommand;
            set
            {
                if (DrawCommand == value)
                    return;
                DrawCommand = value;
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
        public int ProcessID { get => session.ProcessID; set => session.ProcessID = value; }
        public uint ShapeID { get => session.ShapeID; set => session.ShapeID = value; }
        public Rotation Rotation { get; set; }
        public StrokeMode StrokeMode { get; set; }
        public VectorF Scale { get; set; }
        public IPenContext PenContext { get; set; }
        public IRectangle Bounds { get; set; }
        public Size Clip { get; set; }
        public ISession Session => session;
        #endregion

        #region SYNC PATTERN AND ANTIALIAS SETTINGS
        void SyncCommand()
        {
            CalculatedDrawCommand = DrawCommand;
            bool IgnoreAutoCalculatedFillPatten =
                (DrawCommand & Command.IgnoreAutoCalculatedFillPatten) == Command.IgnoreAutoCalculatedFillPatten;
            bool EraseControl = (DrawCommand & Command.EraseObject) == Command.EraseObject;
            bool KeepFillRuleForStroking = (DrawCommand & Command.KeepFillRuleForStroking) == Command.KeepFillRuleForStroking;
            if (IgnoreAutoCalculatedFillPatten)
                return;
            if (stroke == 0 || fillMode == FillMode.Original || KeepFillRuleForStroking || EraseControl)
            {
                CalculatedDrawCommand &= ~Command.Outlininig;
                return;
            }
            if (stroke != 0 && fillMode == FillMode.FillOutLine)
                CalculatedDrawCommand |= Command.Outlininig;
        }
        #endregion

        #region COPY SETTINGS
        public void Receive(IDrawParams settings, bool flushMode = false)
        {
            if (settings == null || flushMode)
                goto Flush;

            if (settings is IPoint)
            {
                session.DstX = ((IPoint)settings).X;
                session.DstY = ((IPoint)settings).Y;
            }
            if (settings is IDstPoint)
            {
                session.DstX = ((IDstPoint)settings).X;
                session.DstY = ((IDstPoint)settings).Y;
            }
            if (settings is IRectangle)
            {
                Session.Copy((IRectangle)settings);
            }
            if (settings is ISettings)
            {
                var info = settings as ISettings;
                stroke = info.Stroke;
                fillMode = info.FillMode;
                StrokeMode = info.StrokeMode;
                Scale = info.Scale;
                Rotation = info.Rotation;
                DrawCommand = info.Command;
                session.Copy(info.Session);
                CleanCommand();
                PenContext = info.PenContext;
            }
        Flush:
            SyncCommand();
        }
        #endregion

        #region CLEAN DRAW COMMAND
        public void CleanCommand()
        {
            DrawCommand &= ~Command.IgnoreAutoCalculatedFillPatten;
            DrawCommand &= ~Command.KeepFillRuleForStroking;
            DrawCommand &= ~Command.CalculateOnly;
            DrawCommand &= ~Command.KeepFillRuleForStroking;
            DrawCommand &= ~Command.IgnoreAutoCalculatedFillPatten;
            DrawCommand &= ~Command.Outlininig;
            DrawCommand &= ~Command.DrawEndsOnly;
            DrawCommand &= ~Command.DrawLineOnly;
            DrawCommand &= ~Command.NoSorting;
            DrawCommand &= ~Command.CheckForCloseness;
            DrawCommand &= ~Command.FillSinglePointLine;
            DrawCommand &= ~Command.EraseObject;
            DrawCommand &= ~Command.RestoreObject;
            DrawCommand &= ~Command.AddObject;
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
                DrawCommand |= item;
            }
            SyncCommand();
        }
        public void RemoveCommands(params Command[] commands)
        {
            if (commands.Length == 0)
                return;
            foreach (var item in commands)
                DrawCommand &= ~item;

            SyncCommand();
        }
        #endregion

        #region FLUSH
        public void Flush()
        {
            DrawCommand = CalculatedDrawCommand = 0;
            stroke = 0;
            StrokeMode = StrokeMode.StrokeMiddle;
            fillMode = FillMode.Original;
            Rotation = Rotation.Empty;
            Scale = VectorF.Empty;
            session.DstX = session.DstY = 0;
            Bounds = Rectangle.Empty;
            ShapeID = 0;
            Session.Clear();
        }
        #endregion
    }
}
#endif
