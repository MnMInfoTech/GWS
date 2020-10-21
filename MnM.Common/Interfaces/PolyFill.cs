/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if (GWS || Window) 
    using System;
    using System.Collections.Generic;

    #region IPOLYFILL
    /// <summary>
    /// Fills a polygon structure with specified PolyFill enum option.
    /// </summary>
    public interface IPolyFill: IDisposable
    {
        #region PROPERTIES
        /// <summary>
        /// Far top boundary to which filling must be confined.
        /// </summary>
        int MinY { get; }

        /// <summary>
        /// Far bottom boundary to which filling must be confined.
        /// </summary>
        int MaxY { get; }

        /// <summary>
        /// Far Left boundary to which filling must be confined.
        /// If it is set to 0 then it becomes non effective.
        /// </summary>
        int MinX { get; set; }

        /// <summary>
        /// Far Right boundary to which filling must be confined.
        /// If it is set to 0 then it becomes non effective.
        /// </summary>
        int MaxX { get; set; }

        /// <summary>
        /// Gets or sets the fill pattern for rendering.
        /// </summary>
        FillCommand FillCommand { get; set; }

        /// <summary>
        /// A Scan action delegate to record line pixels to be processed for scan line filling.
        /// </summary>
        PixelAction<float> ScanAction { get; }
        #endregion

        #region BEGIN
        /// <summary>
        /// Sets this object for filling operation.
        /// </summary>
        /// <param name="y">Far top boundary where filling should be considered from</param>
        /// <param name="bottom">Far bottom boundary where filling should be considered upto</param>
        /// <param name="fillPattern">Fill pattern to be used to perform scan line filling</param>
        void Begin(int y, int bottom, FillCommand fillPattern);
        #endregion

        #region FILL
        /// <summary>
        /// Performs horizontal scan line filling using specified action.
        /// </summary>
        /// <param name="fillAction">Action to be used for filling.</param>
        /// <param name="lineCommand">Line command to be used to draw end pixels</param>
        void Fill(FillAction<float> fillAction, LineCommand lineCommand);

        /// <summary>
        /// Performs horizontal scan line filling on given buffer using specified pen.
        /// </summary>
        /// <param name="buffer">Buffer to fill line to.</param>
        /// <param name="pen">Pen to read line from.</param>
        /// <param name="lineCommand">Line command to be used to draw end pixels</param>
        void Fill(IBlock buffer, IReadable pen, LineCommand lineCommand = 0);
        #endregion

        #region END
        /// <summary>
        /// Ends current fill operation and resets internal data.
        /// </summary>
        void End();
        #endregion

        #region SCAN
        /// <summary>
        /// Scans a line using standard line algorithm between two points of a line segment using specified action.
        /// Line will be scanned horizontally i.e. from y1 to y2 taking rounded values of both,
        /// </summary>
        /// <param name="x1">X corordinate of start point</param>
        /// <param name="y1">Y corordinate of start point</param>
        /// <param name="x2">X corordinate of end point</param>
        /// <param name="y2">Y corordinate of end point</param>
        void Scan(float x1, float y1, float x2, float y2);

        /// <summary>
        /// Includes a point specified by x and y parameters in filling operation.
        /// </summary>
        /// <param name="x">X co-ordinate of point.</param>
        /// <param name="y">Y co-ordinate of point.</param>
        void Scan(float x, int y);

        /// <summary>
        /// Includes the given point in filling operation.
        /// </summary>
        /// <param name="p">The point to include.</param>
        void Scan(VectorF p);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Points"></param>
        /// <param name="Contours"></param>
        void Scan(IList<VectorF> Points, IList<int> Contours = null);
        #endregion

        #region FILL LINE
        /// <summary>
        /// Fills an axial fragmented scan line - using odd - even fill rule exclusively.
        /// </summary>
        /// <param name="data">Collection which contains fragments.</param>
        /// <param name="axis">Axis value of the line. If horizontal is true then it is Y otherwise X axis.</param>
        /// <param name="horizontal">If true, line should be scanned from top to bottom otherwise left to right</param>
        /// <param name="action">A FillAction delegate which has routine to do something with the axial information provided.</param>
        /// <param name="alpha">Alha factor to apply to whole line if supplied at all.</param>
        void FillLine(ICollection<float> data, int axis, bool horizontal, FillAction<float> action, float? alpha = null);

        /// <summary>
        /// Fills an axial fragmented scan line - using odd - even fill rule exclusively.
        /// </summary>
        /// <param name="buffer">Buffer to fill line to.</param>
        /// <param name="pen">Pen to read line from.</param>
        /// <param name="data">Collection which contains fragments.</param>
        /// <param name="axis">Axis value of the line. If horizontal is true then it is Y otherwise X axis.</param>
        /// <param name="horizontal">If true, line should be scanned from top to bottom otherwise left to right</param>
        /// <param name="alpha">Alha factor to apply to whole line if supplied at all.</param>
        void FillLine(IBlock buffer, IReadable pen, ICollection<float> data, int axis, bool horizontal, float? alpha = null);
        #endregion
    }
    #endregion
#endif
}
