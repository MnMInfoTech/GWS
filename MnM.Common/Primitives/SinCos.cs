using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    public struct SinCos
    {
        public readonly float Sin;
        public readonly float Cos;

        public SinCos(float angle)
        {
            Angles.SinCos(angle, out Sin, out Cos);
        }
        public SinCos(float angle, float deviation)
        {
            Angles.SinCos(angle, out Sin, out Cos);
            Sin += deviation;
            Cos += deviation;
        }
    }
}
