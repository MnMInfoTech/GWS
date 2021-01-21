/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    public abstract class _PolyFill : IPolyFill
    {
    #region VARIABLES
        VectorF ActivePoint;
        protected Command drawCommand;
        protected Size clip;
        protected bool Sorting, FillSinglePoint, EndsOnly;
    #endregion

    #region PROPERTIES
        public int MinY { get; protected set; }
        public int MaxY { get; protected set; }
        public int MinX { get; set; }
        public int MaxX { get; set; }
        public Command Command
        {
            get => drawCommand;
            set
            {
                drawCommand = value;
                Sorting = !value.HasFlag(Command.NoSorting);
                FillSinglePoint = value.HasFlag(Command.FillSinglePointLine);
                EndsOnly = value.HasFlag(Command.DrawEndsOnly);
            }
        }
        public Size Clip 
        {
            get => clip;
            set => clip = value;
        }
        public PixelAction ScanAction =>
            NotifyScanResult;
    #endregion

    #region BEGIN
        public virtual void Begin(int y, int bottom)
        {
            Numbers.Order(ref y, ref bottom);
            MinY = y;
            MaxY = bottom;
            MinX = MaxX = 0;
        }
    #endregion

    #region FILL
        public abstract void Fill(FillAction fillAction);
    #endregion

    #region END
        public virtual void End()
        {
            drawCommand = 0;
            MinY = 0;
            MaxY = 0;
            MinX = MaxX = 0;
        }
    #endregion

    #region FILL LINE
        public abstract void FillLine(ICollection<float> data, int axis, bool horizontal, FillAction action, float? alpha = null);
    #endregion

    #region SCAN
        public virtual void Scan(float x1, float y1, float x2, float y2)
        {
            Renderer.ScanLine(x1, y1, x2, y2, true, ScanAction);
        }
        public void Scan(params ILine[] lines) =>
            Scan(lines as IEnumerable<ILine>);
        public void Scan(IEnumerable<ILine> lines)
        {
            foreach (var line in lines)
                Renderer.ScanLine(line.X1, line.Y1, line.X2, line.Y2, true, ScanAction);
        }
        public abstract void Scan(float x, int y);
        public abstract void Scan(VectorF p);
        public abstract void Scan(IList<VectorF> Points, IList<int> Contours = null);
    #endregion

    #region NOTIFY SCAN
        protected abstract void NotifyScanResult(float value, int axis, bool horizontal, Command command);
    #endregion

    #region DISPOSE
        public void Dispose() => End();
    #endregion
    }
}
#endif
