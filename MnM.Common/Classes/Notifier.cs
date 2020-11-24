/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

using System.Drawing;

namespace MnM.GWS
{
#if (GWS || Window)
    /// <summary>
    /// Represents an object which holds an information about last invalidated area.
    /// </summary>
    public sealed class Notifier
    {
        #region VARIABLES
        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = 0, maxY = 0;

        /// <summary>
        /// Gets or sets the last drawn area.
        /// </summary>
        Rectangle recentlyDrawn;
        #endregion

        #region PROPERTIES
        public Rectangle RecentlyDrawn => recentlyDrawn;
        #endregion

        #region INVALIDATE
        /// <summary>
        /// Notifies the top, left, right and bottom co-ordinates of recently drawn to update last drawn area.
        /// </summary>
        /// <param name="x">X co-ordinate of the recently drawn area.</param>
        /// <param name="y">Y co-ordinate of the recently drawn area.</param>
        /// <param name="w">Width of the recently drawn area.</param>
        /// <param name="h">Height of the recently drawn area.</param>
        public void Invalidate(int x, int y, int w, int h)
        {
            if (x < minX)
                minX = x;
            if (y < minY)
                minY = y;

            if (x + w > maxX)
                maxX = w;
            if (y + h > maxY)
                maxY = h;
        }

        /// <summary>
        /// Resets this object directly with given rectangle parameter.
        /// </summary>
        /// <param name="rc">Rectnagle to update last drawn area with</param>
        public void Invalidate(Rectangle rc)
        {
            if (!rc)
            {
                minX = minY = int.MaxValue;
                maxX = maxY = 0;
                recentlyDrawn = rc;
                return;
            }
            var x = rc.X - Vectors.LOffset;
            var y = rc.Y - Vectors.LOffset;
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            
            recentlyDrawn = Rectangle.FromLTRB(x, y, maxX + Vectors.LOffset, maxY + Vectors.LOffset);
        }
        #endregion

        #region RESET
        /// <summary>
        /// 
        /// </summary>
        /// <param name="onlyFinalizeRecentInvalidation"></param>
        public void Reset(bool onlyFinalizeRecentInvalidation = false)
        {
            var x = minX - Vectors.LOffset;
            var y = minY - Vectors.LOffset;
            if (x < 0)
                x = 0; 
            if (y < 0)
                y = 0;

            if (maxX == 0 || maxY == 0)
                recentlyDrawn = Rectangle.Empty;
            else
                recentlyDrawn = Rectangle.FromLTRB(x, y, maxX + Vectors.LOffset, maxY + Vectors.LOffset);

            if (!onlyFinalizeRecentInvalidation)
            {
                minX = minY = int.MaxValue;
                maxX = maxY = 0;
            }
        }

        #endregion

        public override string ToString()
        {
            return recentlyDrawn.ToString();
        }
    }
#endif
}
