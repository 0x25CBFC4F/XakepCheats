using System;

namespace ExternalCheat.Memory
{
    public class MemoryRegion
    {
        public IntPtr BaseAddress { get; set; }
        public IntPtr RegionSize { get; set; }
        public uint Protect { get; set; }
    }
}
