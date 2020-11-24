using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    public sealed class Pixels: IPixels
    {
        public IntPtr Source { get; set; }
        public int Length { get; set;}
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
