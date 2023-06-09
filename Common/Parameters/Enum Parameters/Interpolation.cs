/* Licensed under the MIT/X11 license.
* Copyright (c) 2016-2018 jointly owned by eBestow Technocracy India Pvt. Ltd. & M&M Info-Tech UK Ltd.
* This notice may not be removed from any source distribution.
* See license.txt for detailed licensing details. */
// Author: Mukesh Adhvaryu.

namespace MnM.GWS
{
    public interface IInterpolation : IInLineParameter, IProperty<Interpolation>
    { }

    partial class Parameters
    {
        struct pInterpolation : IInterpolation
        {
            readonly Interpolation Value;
            public pInterpolation(Interpolation interpolation)
            {
                Value = interpolation;
            }
            object IValue.Value => Value;
            Interpolation IValue<Interpolation>.Value => Value;

            public override string ToString() =>
                Value.ToString();
        }

        public static IInterpolation ToParameter(this Interpolation interpolation) =>
            new pInterpolation(interpolation);
    }
}
