/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.

using System.Runtime.CompilerServices;

namespace MnM.GWS
{
    #region ICENTER
    public interface ICentre :  IPointF, IRotationParameter, IProperty<ICentre>
    {
        float Cx { get; }
        float Cy { get; }
    }
    #endregion

    #region ICENTER HOLDER
    public interface ICentreHolder
    {
        /// <summary>
        /// Gets or sets centre of this rotation.
        /// </summary>
        ICentre Centre { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void SetCentre(float? x, float? y);
    }
    #endregion

    public struct Centre : ICentre
    {
        /// <summary>
        /// X co-ordinate of centre of rotation.
        /// </summary>
        public readonly float Cx;

        /// <summary>
        /// Y co-ordinate of centre of rotation.
        /// </summary>
        public readonly float Cy;

        static string toStr = "Center ( Cx: {0}, Cy: {1})";

        public Centre(float? cx, float? cy)
        {
            Cx = cx ?? 0;
            Cy = cy ?? 0;
        }
        public Centre(IPoint p, ISize s)
        {
            Cx = (p?.X ?? 0) + (s?.Width ?? 0) / 2f;
            Cy = (p?.Y ?? 0) + (s?.Height ?? 0) / 2f;
        }
        public Centre(IBounds r)
        {
            int x = 0, y = 0, w = 0, h = 0;
            r?.GetBounds(out x, out y, out w, out h);
            Cx = x + w / 2f;
            Cy = y + h / 2f;
        }
        public Centre(IBoundsF r)
        {
            float x = 0, y = 0, w = 0, h = 0;
            r?.GetBounds(out x, out y, out w, out h);
            Cx = x + w / 2f;
            Cy = y + h / 2f;
        }
        public Centre(float x, float y, float w, float h)
        {
            Cx = x + w / 2f;
            Cy = y + h / 2f;
        }
        public Centre(IPointF c, float? cx = null, float? cy = null)
        {
            Cx = cx ?? (c?.X ?? 0);
            Cy = cy ?? (c?.Y ?? 0);
        }

        public Centre(IPoint c, float? cx = null, float? cy = null)
        {
            Cx = cx ?? (c?.X ?? 0);
            Cy = cy ?? (c?.Y ?? 0);
        }
        float ICentre.Cx => Cx;
        float ICentre.Cy => Cy;
        float IPointF.X => Cx;
        float IPointF.Y => Cy;
        ICentre IValue<ICentre>.Value => this;
        object IValue.Value => this;

        public override string ToString()
        {
            return string.Format(toStr, Cx, Cy);
        }

    }
}
