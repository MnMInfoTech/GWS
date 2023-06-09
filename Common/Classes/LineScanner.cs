/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    #region ILINE-SCANNER
    /// <summary>
    /// Represents an object which scans polygon lines and gives scan lines.
    /// </summary>
    public interface ILineScanner : IDisposable
    {
        bool StoreLines { set; }

        /// <summary>
        /// Scans a collection of lines using standard line algorithm between two points of a line segment using specified action.
        /// While scanning each line, the processing will not exceed the boundaries defined by min and max values.
        /// For example if horizontal then Start = MinY and End = MaxY. Let say it is 100 and 300 respectively.
        /// Now while processing a line say 50, 99, 90, 301, position of y1 at 99 and y2 at 301 will be ignored as they do not fall in the range of 100 -300.
        /// </summary>
        /// <param name="lines">Collection of lines to render</param>
        bool Scan(IEnumerable<ILineSegment> lines);

        /// <summary>
        /// Sets this object for filling operation in a dynamic mode
        /// i.e without any top and bottom boundary check.
        /// </summary>
        void Begin();

        /// <summary>
        /// Ends current fill operation and resets internal data.
        /// </summary>
        void End();

        /// <summary>
        /// Gets collection of scan-lines accumulated through scanning.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IAxisLine> GetScanLines();

        /// <summary>
        /// Gets collection of straightlines accumulated through scanning.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ILineSegment> GetDrawLines();
    }
    #endregion

    partial class Factory
    {
        class LineScanner : ILineScanner
        {
            #region VARIABLES
            protected Dictionary<int, ICollection<float>> Results;
            protected PrimitiveList<ILineSegment> Lines;

            const float START_EPSILON = GWS.Lines.START_EPSILON;
            const float END_EPSILON = GWS.Lines.END_EPSILON;
            const float EPSILON = GWS.Lines.EPSILON;
            #endregion

            public bool StoreLines 
            {
                set
                {
                    if (value && Lines != null || !value && Lines == null)
                        return;
                    if (value)
                        Lines = new PrimitiveList<ILineSegment>();
                    else
                        Lines = null;
                }
            }

            #region BEGIN - END
            public void Begin()
            {
                Results = new Dictionary<int, ICollection<float>>(8);
                if (Lines != null)
                    Lines.Clear();
            }

            public void End()
            {
                Results = null;
                if (Lines != null)
                    Lines.Clear();
            }
            #endregion

            #region SCAN
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Scan(IEnumerable<ILineSegment> lines)
            {
                if (lines == null)
                    return (false);

                if (Results == null)
                {
                    Results = new Dictionary<int, ICollection<float>>(8);
                }

                #region DEFINE VITAL VARIABLES
                float m, c, initialValue, val;
                int Start, End, step, pos = 0;
                float x1, y1, x2, y2; 
                #endregion

                foreach (var line in lines)
                {
                    if (line == null || !line.Valid)
                        continue;
                    if (Lines != null)
                        Lines.Add(line);

                    #region INTIALIZE X1, Y1
                    x1 = line.X1;
                    y1 = line.Y1;
                    #endregion

                    #region INITIALIZE X2, Y2
                    x2 = line.X2;
                    y2 = line.Y2;
                    #endregion

                    #region CALCULATE LINE VITALS I.E M, C ETC. 
                    var dx = x2 - x1;
                    var dy = y2 - y1;

                    var pdy = dy;
                    var pdx = dx;

                    if (pdy < 0)
                        pdy = -pdy;
                    if (pdx < 0)
                        pdx = -pdx;

                    if (pdy <= EPSILON)
                    {
                        continue;
                    }
                    bool negative = y1 > y2;
                    Start = (int)y1;
                    End = (int)y2;
                    if (y1 - Start >= START_EPSILON)
                        ++Start;
                    if (y2 - End >= END_EPSILON)
                        ++End;
                    step = 1;
                    int lineLength = End - Start;
                    m = dy;

                    bool VerticalLine = pdx == 0;

                    if (dx != 0)
                        m /= dx;
                    c = y1 - m * x1;
                    m = 1 / m;
                    var tc = -c * m;
                    initialValue = val = Start * m + tc;

                    if (negative)
                    {
                        m = -m;
                        step = -step;
                        lineLength = -lineLength;
                        Start += step;
                        initialValue += m;
                    }
                    if (VerticalLine)
                    {
                        m = 0;
                    }
                    #endregion

                    pos = lineLength;
                    val = initialValue;
                    #region LOOP
                    LINE_LOOP:
                    if (pos <= 0)
                        continue;

                    if (!Results.ContainsKey(Start))
                        Results[Start] = new PrimitiveList<float>();

                    Results[Start].Add(val);

                    --pos;
                    initialValue += m;
                    Start += step;
                    val = initialValue;
                    goto LINE_LOOP;
                    #endregion
                }
                return (true);
            }
            #endregion

            #region GET SCANLINES
            public IEnumerable<IAxisLine> GetScanLines()
            {
                IEnumerable<IAxisLine> lines =
                    Results.Select((r) => (IAxisLine)
                    new OddEvenLineF(r.Value, r.Key, LineFill.Horizontal | LineFill.PolyLine));
                return lines;
            }
            #endregion

            #region GET SCANLINES
            public IEnumerable<ILineSegment> GetDrawLines() =>
                Lines;
            #endregion

            #region DISPOSE
            public virtual void Dispose()
            {
                End();
            }
            #endregion
        }
    }
}
