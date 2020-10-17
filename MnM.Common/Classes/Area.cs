/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */

namespace MnM.GWS
{
#if (GWS || Window)
    /// <summary>
    /// Represents an object which holds an information about last invalidated area.
    /// </summary>
    public sealed class Area
    {
        #region VARIABLES
        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = 0, maxY = 0;
        public const int LocationOffset = 3;
        public const int SizeOffset = LocationOffset * 2;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Gets or sets the last drawn area.
        /// </summary>
        public Rectangle Value
        {
            get
            {
                var x = minX - LocationOffset;
                var y = minY - LocationOffset;
                if (x < 0)
                    x = 0;
                if (y < 0)
                    y = 0;
                if (maxX == 0 || maxY == 0)
                    return Rectangle.Empty;
                return Rectangle.FromLTRB(x, y, maxX + SizeOffset, maxY + SizeOffset);
            }
            set
            {
                if (!value)
                {
                    minX = minY = int.MaxValue;
                    maxX = maxY = 0;
                }
                else
                {
                    var x = value.X + LocationOffset;
                    var y = value.Y + LocationOffset;
                    var r = value.Right - SizeOffset;
                    var b = value.Bottom - SizeOffset;
                    Notify(x, y, r, b);
                }
            }
        }

        /// <summary>
        /// Gets X co-ordinate of last drawn area.
        /// </summary>
        public int X
        {
            get
            {
                if (maxX == 0 || maxY == 0)
                    return 0;
                var x = minX - LocationOffset;
                if (x < 0)
                    x = 0;
                if (x > maxX + SizeOffset)
                    return maxX + SizeOffset;
                return x;
            }
        }

        ///Gets Y co-ordinate of last drawn area.
        public int Y
        {
            get
            {
                if (maxX == 0 || maxY == 0)
                    return 0;
                var y = minY - LocationOffset;
                if (y < 0)
                    y = 0;
                if (y > maxY + SizeOffset)
                    return maxY + SizeOffset;
                return y;
            }
        }

        /// <summary>
        ///Gets Width of last drawn area.
        /// </summary>
        public int Width
        {
            get
            {
                if (maxX == 0 || maxY == 0)
                    return 0;
                var x = minX - LocationOffset;
                if (x < 0)
                    x = 0;
                var right = maxX + SizeOffset;
                if (x > right)
                    return x;
                return right - x;
            }
        }

        /// <summary>
        /// Gets height of last drawn area.
        /// </summary>
        public int Height
        {
            get
            {
                if (maxX == 0 || maxY == 0)
                    return 0;
                var y = minY - LocationOffset;
                if (y < 0)
                    y = 0;
                var bottom = maxY + SizeOffset;
                if (y > bottom)
                    return y;
                return bottom - y;
            }
        }
        #endregion

        #region NOTIFY
        /// <summary>
        /// Notifies the top, left, right and bottom co-ordinates of recently drawn to update last drawn area.
        /// </summary>
        /// <param name="x">X co-ordinate of the recently drawn area.</param>
        /// <param name="y">Y co-ordinate of the recently drawn area.</param>
        /// <param name="right">Far right co-ordinate of the recently drawn area.</param>
        /// <param name="bottom">Far bottom co-ordinate of the recently drawn area.</param>
        public void Notify(int x, int y, int right, int bottom)
        {
            if (x < minX)
                minX = x;
            if (y < minY)
                minY = y;

            if (x > maxX)
                maxX = x;
            if (y > maxY)
                maxY = y;

            if (right < minX)
                minX = right;
            if (bottom < minY)
                minY = bottom;
            if (right > maxX)
                maxX = right;
            if (bottom > maxY)
                maxY = bottom;
        }
        #endregion

        #region RESET
        /// <summary>
        /// Resets this object directly with given parameters.
        /// </summary>
        /// <param name="x">X co-ordinate of the recently drawn area.</param>
        /// <param name="y">Y co-ordinate of the recently drawn area.</param>
        /// <param name="right">Far right co-ordinate of the recently drawn area.</param>
        /// <param name="bottom">Far bottom co-ordinate of the recently drawn area.</param>
        public void Reset(int x, int y, int right, int bottom)
        {
            minX = x;
            minY = y;
            maxX = right;
            maxY = bottom;
        }

        /// <summary>
        /// Resets this object directly with given rectangle parameter.
        /// </summary>
        /// <param name="rc">Rectnagle to update last drawn area with</param>
        public void Reset(Rectangle rc)
        {
            minX = rc.X;
            minY = rc.Y;
            maxX = rc.Right;
            maxY = rc.Bottom;
        }
        public void Reset()
        {
            minX = minY = int.MaxValue;
            maxX = maxY = 0;
        }
        #endregion

        #region COPY
        /// <summary>
        /// Gets co-ordinates of last drawn area.
        /// </summary>
        /// <param name="x">X co-ordinate of the recently drawn area.</param>
        /// <param name="y">Y co-ordinate of the recently drawn area.</param>
        /// <param name="w">Width of the recently drawn area.</param>
        /// <param name="h">Height of the recently drawn area.</param>
        public void Copy(out int x, out int y, out int w, out int h)
        {
            x = y = w = h = 0;
            if (maxX == 0 || maxY == 0)
                return;
            x = minX - LocationOffset;
            y = minY - LocationOffset;
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            w = maxX + SizeOffset - x;
            h = maxY + SizeOffset - y;
        }
        #endregion

        /// <summary>
        /// Converts this oject to rectangle structure.
        /// </summary>
        /// <param name="area"></param>
        public static implicit operator Rectangle(Area area)
        {
            var x = area.minX - LocationOffset;
            var y = area.minY - LocationOffset;
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;

            if (area.maxX == 0 || area.maxY == 0)
                return Rectangle.Empty;
            return Rectangle.FromLTRB(x, y, area.maxX + SizeOffset, area.maxY + SizeOffset);
        }

        public static implicit operator bool(Area area)
        {
            if (area.maxX == 0 || area.maxY == 0 || area.minX == int.MinValue || area.minY == int.MinValue)
                return false;
            return true;
        }

        public override string ToString()
        {
            return minX + ", " + minY + ", " + maxX + ", " + maxY;
        }
    }
#endif
}
