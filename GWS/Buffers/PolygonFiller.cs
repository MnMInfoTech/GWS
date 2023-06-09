/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if GWS || Window
using System;
using System.Collections.Generic;
using System.Linq;

namespace MnM.GWS
{
    #region IPOLYGON-FILLER
    /// <summary>
    /// Represents an object which renders a polygon using specified fill action and draw action.
    /// </summary>
    public interface IPolygonFiller : ILineScanner
    {
        /// <summary>
        /// Gets or sets an option of polygon filling.
        /// </summary>
        FillCommand Mode { get; set; }

        /// <summary>
        /// Fills and draw current set of scan lines and draw lines using available RenderAction.
        /// </summary>
        bool Render(FillCommand polyState = 0);

        /// <summary>
        /// Fills current set of scan lines using specified FillAction.
        /// </summary>
        bool RenderAny(IEnumerable<IScanLine> scanLines, IEnumerable<IScanPoint> drawLines,
            params IInLineParameter[] parameters);

        /// <summary>
        /// Creates vertical spans from y to bottom, scan specified lines and creates scanlines
        /// and finally renders scan lines using renderaction associated with this object.
        /// </summary>
        /// <param name="y">Far top boundary where filling should be considered from</param>
        /// <param name="bottom">Far bottom boundary where filling should be considered upto</param>
        /// <param name="lines">Collection of lines to render</param>
        /// <returns></returns>
        bool DoAll(IEnumerable<ILine> lines);
    }
    #endregion

    partial class Factory
    {
        sealed class PolygonFiller : LineScanner, IPolygonFiller
        {
            #region VARIABLES
            RenderAction FillAction;
            FillCommand State;
            #endregion

            #region CONSTRUCTOR
            public PolygonFiller(RenderAction fillAction)
            {
                FillAction = fillAction;
            }
            #endregion

            #region PROPERTIES
            public FillCommand Mode
            {
                get => State;
                set
                {
                    State |= value;
                }
            }
            #endregion

            #region RENDER
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Render(FillCommand polyState = 0)
            {
                polyState |= State;
                bool drawEdges = (polyState & FillCommand.SkipDraw) != FillCommand.SkipDraw &&
                    (polyState & FillCommand.DrawOutLines) == FillCommand.DrawOutLines;
                bool fill = (polyState & FillCommand.SkipFill) != FillCommand.SkipDraw;

                IEnumerable<IAxisLine> lines = fill?
                    Results.Select((r) => (IAxisLine)
                    new OddEvenLineF(r.Value, r.Key, LineFill.Horizontal)): null;

                IEnumerable<ILineSegment> DrawLines = drawEdges? Lines : null;
                return FillAction(lines, DrawLines, null);
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool RenderAny(IEnumerable<IScanLine> scanLines, IEnumerable<IScanPoint> drawLines,
                params IInLineParameter[] parameters)
            {
                return FillAction(scanLines, drawLines, null, parameters);
            }
            #endregion

            #region DO-ALL
            public bool DoAll(IEnumerable<ILine> lines)
            {
                Begin();
                Scan(lines);
                return Render();
            }
            #endregion

            #region DISPOSE
            public override void Dispose()
            {
                base.Dispose();
                FillAction = null;
            }
            #endregion
        }
    }
}
#endif

