/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Manan Adhvaryu.
#if (GWS || Window)
namespace MnM.GWS
{
    #region ISTROKE
    public interface IStroke : IProperty<float>
    {
        /// <summary>
        /// Gets stroke value settings for this object.
        /// </summary>
        float Stroke { get; }
    }
    #endregion

    #region STROKE
    public struct Stroke : IStroke
    {
        #region VARIABLES
        readonly float Value;
        public readonly static Stroke Empty = new Stroke();
        #endregion

        public Stroke(float stroke)
        {
            this.Value = stroke;
        }
        public Stroke(IStroke stroke)
        {
            this.Value = stroke.Stroke;
        }
        float IStroke.Stroke => Value;
        object IValue.Value => Value;
        float IValue<float>.Value => Value;

        public static implicit operator float(Stroke gwsStroke) =>
            gwsStroke.Value;

        public override string ToString() =>
            Value.ToString();
    }
    #endregion

    partial class Parameters
    {

        #region TO STROKE
        public static IStroke ToStroke(this float stroke) =>
            new Stroke(stroke);
        public static IStroke ToStroke(this int stroke) =>
            new Stroke(stroke);
        public static IStroke ToStroke(this IStroke stroke) =>
            new Stroke(stroke);
        #endregion
    }
}
#endif
