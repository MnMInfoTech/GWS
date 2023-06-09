using System;
using System.Collections.Generic;
using System.Text;

namespace MnM.GWS
{
    public interface IMemoryOccupation: ICloneable
    {
        ushort MB { get; }
        uint KB { get; }
    }
    public interface IMemoryOccupier
    {
        /// <summary>
        /// Gets total memory occupation of this object in mega bytes (MB) as well as kilo bytes(KB).
        /// </summary>
        IMemoryOccupation MemoryOccupation { get; }
    }

    public struct MemoryOccupation: IMemoryOccupation
    {
        const float kbfactor = 1 / 1024f;
        readonly ushort mb;
        readonly uint kb;

        public MemoryOccupation(IMemoryOccupation memoryOccupation)
        {
            mb = memoryOccupation.MB;
            kb = memoryOccupation.KB;
        }
        MemoryOccupation(ushort MB)
        {
            mb = MB;
            kb = (uint)mb * 1024;
        }
        MemoryOccupation(uint KB)
        {
            kb = KB;
            var _mb = kb * kbfactor;
            mb = (ushort)(kb * kbfactor).Round();
        }
        public ushort MB
        {
            get => mb;
            internal set
            {
                this = new MemoryOccupation(value);
            }
        }
        public uint KB
        {
            get => kb;
            internal set
            {
                this = new MemoryOccupation(value);
            }
        }

        public void BeginMonitoring(out long total)
        {
            total = -GC.GetTotalMemory(true);
        }
        public void EndMonitoring(long total)
        {
            var now = GC.GetTotalMemory(true);
            total += now;
            this = new MemoryOccupation((uint)(total * kbfactor));
        }
        public object Clone()
        {
            return new MemoryOccupation(this);
        }

        public static MemoryOccupation operator +(MemoryOccupation p1, IMemoryOccupation p2)
        {
            return new MemoryOccupation((ushort)(p1.mb + p2.MB));
        }

        public override string ToString()
        {
            return string.Format("MB: {0}, KB: {1}", mb, kb);
        }
    }
}
