using System;
using System.Collections.Generic;
using System.Linq;
using ExternalCheat.Memory.Pattern;

namespace ExternalCheat.Memory
{
    public class MemoryManager
    {
        private readonly IntPtr _processHandle;

        public MemoryManager(IntPtr handle)
        {
            _processHandle = handle;
        }

        public IntPtr PatternScan(MemoryPattern pattern)
        {
            var regions = QueryMemoryRegions();

            foreach (var memoryRegion in regions)
            {
                var addr = ScanForPatternInRegion(memoryRegion, pattern);

                if (addr == IntPtr.Zero)
                {
                    continue;
                }

                return addr;
            }

            return IntPtr.Zero;
        }

        public byte[] ReadMemory(IntPtr addr, int size)
        {
            var buff = new byte[size];
            return WinAPI.ReadProcessMemory(_processHandle, addr, buff, size, out _) ? buff : null;
        }

        public int ReadInt32(IntPtr addr)
        {
            return BitConverter.ToInt32(ReadMemory(addr, 4), 0);
        }

        public void WriteInt32(IntPtr addr, int value)
        {
            var b = BitConverter.GetBytes(value);
            WinAPI.WriteProcessMemory(_processHandle, addr, b, b.Length, out _);
        }

        public IntPtr ScanForPatternInRegion(MemoryRegion region, MemoryPattern pattern)
        {
            var endAddr = (int) region.RegionSize - pattern.Size;
            var wholeMemory = ReadMemory(region.BaseAddress, (int) region.RegionSize);

            for (var addr = 0; addr < endAddr; addr++)
            {
                var buff = new byte[pattern.Size];
                Array.Copy(wholeMemory, addr, buff, 0, buff.Length);

                var found = true;

                for (var i = 0; i < pattern.Size; i++)
                {
                    if (!pattern.PatternParts[i].Matches(buff[i]))
                    {
                        found = false;
                        break;
                    }
                }

                if (!found)
                {
                    continue;
                }

                return region.BaseAddress + addr;
            }

            return IntPtr.Zero;
        }

        /*public IntPtr ScanForPatternInRegion(MemoryRegion region, MemoryPattern pattern)
        {
            var endAddr = (int) region.RegionSize - pattern.Size;
            var wholeMemory = ReadMemory(region.BaseAddress, (int) region.RegionSize);

            for (var addr = 0; addr < endAddr; addr++)
            {
                var b = wholeMemory.Skip(addr).Take(pattern.Size).ToArray();

                if (!pattern.PatternParts.First().Matches(b.First()))
                {
                    continue;
                }

                if (!pattern.PatternParts.Last().Matches(b.Last()))
                {
                    continue;
                }

                var found = true;

                for (var i = 1; i < pattern.Size - 1; i++)
                {
                    if (!pattern.PatternParts[i].Matches(b[i]))
                    {
                        found = false;
                        break;
                    }
                }

                if (!found)
                {
                    continue;
                }

                return region.BaseAddress + addr;
            }

            return IntPtr.Zero;
        }*/

        public List<MemoryRegion> QueryMemoryRegions() {
            long curr = 0;
            var regions = new List<MemoryRegion>();

            while (true) {
                try {
                    var memDump = WinAPI.VirtualQueryEx(_processHandle, (IntPtr) curr, out var memInfo, 28);
                    
                    if (memDump == 0) break;

                    if ((memInfo.State & 0x1000) != 0 && (memInfo.Protect & 0x100) == 0)
                    {
                        regions.Add(new MemoryRegion
                        {
                            BaseAddress = memInfo.BaseAddress,
                            RegionSize = memInfo.RegionSize,
                            Protect = memInfo.Protect
                        });
                    }

                    curr = (long) memInfo.BaseAddress + (long) memInfo.RegionSize;
                } catch {
                    break;
                }
            }

            return regions;
        }
    }
}
