/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice must not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window

namespace MnM.GWS
{
    public partial class Settings : ISettings
    {
        #region VARIABLES
        ulong DrawCommand, CalculatedDrawCommand;
        private FillMode fillMode;
        private float stroke;
        Session MySession = new Session();
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
            MySession.ShapeID = shapeID;
            DrawCommand = MnM.GWS.Command.OddEven;
            Initialize();
        }
        partial void Initialize();
        #endregion

        #region PROPERTIES
        public ulong Command
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
        public Rotation Rotation { get; set; }
        public StrokeMode StrokeMode { get; set; }
        public VectorF Scale { get; set; }
        public IPenContext PenContext { get; set; }
        public IRectangle Bounds { get; set; }
        public Size Clip { get; set; }
        public ISession Session => MySession;
        #endregion

        #region SYNC PATTERN AND ANTIALIAS SETTINGS
        void SyncCommand()
        {
            CalculatedDrawCommand = DrawCommand;
            bool IgnoreAutoCalculatedFillPatten =
                (DrawCommand & MnM.GWS.Command.IgnoreAutoCalculatedFillPatten) == MnM.GWS.Command.IgnoreAutoCalculatedFillPatten;
            bool EraseControl = (DrawCommand & MnM.GWS.Command.EraseObject) == MnM.GWS.Command.EraseObject;
            bool KeepFillRuleForStroking = (DrawCommand & MnM.GWS.Command.KeepFillRuleForStroking) == MnM.GWS.Command.KeepFillRuleForStroking;
            if (IgnoreAutoCalculatedFillPatten)
                return;
            if (stroke == 0 || fillMode == FillMode.Original || KeepFillRuleForStroking || EraseControl)
            {
                CalculatedDrawCommand &= ~MnM.GWS.Command.Outlininig;
                return;
            }
            if (stroke != 0 && fillMode == FillMode.FillOutLine)
                CalculatedDrawCommand |= MnM.GWS.Command.Outlininig;
        }
        #endregion

        #region COPY SETTINGS
        public void Receive(IDrawParams settings, bool flushMode = false)
        {
            if (settings == null || flushMode)
                goto Flush;

            if (settings is IPoint)
            {
                MySession.DstX = ((IPoint)settings).X;
                MySession.DstY = ((IPoint)settings).Y;
            }
            if (settings is IDstPoint)
            {
                MySession.DstX = ((IDstPoint)settings).X;
                MySession.DstY = ((IDstPoint)settings).Y;
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
                MySession.Copy(info.Session);
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
            DrawCommand &= ~ MnM.GWS.Command.IgnoreAutoCalculatedFillPatten;
            DrawCommand &= ~MnM.GWS.Command.KeepFillRuleForStroking;
            DrawCommand &= ~MnM.GWS.Command.CalculateOnly;
            DrawCommand &= ~MnM.GWS.Command.KeepFillRuleForStroking;
            DrawCommand &= ~MnM.GWS.Command.IgnoreAutoCalculatedFillPatten;
            DrawCommand &= ~MnM.GWS.Command.Outlininig;
            DrawCommand &= ~MnM.GWS.Command.DrawEndsOnly;
            DrawCommand &= ~MnM.GWS.Command.DrawLineOnly;
            DrawCommand &= ~MnM.GWS.Command.NoSorting;
            DrawCommand &= ~MnM.GWS.Command.CheckForCloseness;
            DrawCommand &= ~MnM.GWS.Command.FillSinglePointLine;
            DrawCommand &= ~MnM.GWS.Command.EraseObject;
            DrawCommand &= ~MnM.GWS.Command.RestoreObject;
            DrawCommand &= ~MnM.GWS.Command.AddObject;
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
            MySession.DstX = MySession.DstY = 0;
            Bounds = Rectangle.Empty;
            MySession.ShapeID = 0;
            Session.Clear();
        }
        #endregion
    }
}
#endif
