/* Copyright (c) 2016-2018 owned by M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details.
* Author: Mukesh Adhvaryu
*/

namespace MnM.GWS
{
    public interface IGreyScale: IProperty<GreyScale>, IInLineParameter
    {
        GreyScale GreyScale { get; }
    }
    partial class Parameters
    {
        struct pGreyScale : IGreyScale
        {
            readonly GreyScale GreyScale;

            public pGreyScale(GreyScale greyScale)
            {
                GreyScale = greyScale;
            }

            GreyScale IGreyScale.GreyScale => GreyScale;
            object IValue.Value => GreyScale;
            GreyScale IValue<GreyScale>.Value => GreyScale;

            public static implicit operator GreyScale(pGreyScale greyScaleHolder) =>
                greyScaleHolder.GreyScale;

            public override string ToString() =>
                GreyScale.ToString();
        }

        #region TO GREY SCALE
        public static IGreyScale ToParameter(this GreyScale greyScale) =>
            new pGreyScale(greyScale);
        #endregion
    }
}
