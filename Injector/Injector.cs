using System;
using System.IO;
using System.Text;

namespace Injector
{
    internal enum InjectionType
    {
        LoadLibrary,
        ManualMap
    }

    internal class Injector
    {
        public static void Inject(string libraryPath, int processID, InjectionType type)
        {
            switch (type)
            {
                case InjectionType.LoadLibrary:
                    InjectLoadLibrary(libraryPath, processID);
                    break;
                case InjectionType.ManualMap:
                    InjectManualMap(libraryPath, processID);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static void InjectLoadLibrary(string libraryPath, int processID)
        {
            var handle = WinAPI.OpenProcess(WinAPI.ProcessAccessFlags.All, false, processID);

            if (handle == IntPtr.Zero)
            {
                Console.WriteLine("Can't open process.");
                return;
            }

            var libraryPathBytes = Encoding.ASCII.GetBytes(libraryPath);

            Console.WriteLine($"Process was opened. Handle: 0x{handle.ToInt32():X}. Allocating memory..");

            var memory = WinAPI.VirtualAllocEx(handle,
                IntPtr.Zero,
                256,
                WinAPI.AllocationType.Commit | WinAPI.AllocationType.Reserve,
                WinAPI.MemoryProtection.ExecuteReadWrite);
                
            Console.WriteLine($"Memory is allocated at 0x{memory.ToInt32():X}. Writing..");

            WinAPI.WriteProcessMemory(handle, memory, libraryPathBytes, libraryPathBytes.Length, out var bytesWritten);

            Console.WriteLine($"Bytes written: {bytesWritten}");

            var funcAddr = WinAPI.GetProcAddress(WinAPI.GetModuleHandle("kernel32"), "LoadLibraryA");
            var thread = WinAPI.CreateRemoteThread(handle, IntPtr.Zero, IntPtr.Zero, funcAddr, memory, 0, IntPtr.Zero);
            
            Console.WriteLine($"Started thread with handle 0x{thread.ToInt32():X} at 0x{funcAddr.ToInt32():X}");
        }

        private static void InjectManualMap(string libraryPath, int processID)
        {
            var handle = WinAPI.OpenProcess(WinAPI.ProcessAccessFlags.All, false, processID);

            if (handle == IntPtr.Zero)
            {
                Console.WriteLine("Can't open process.");
                return;
            }

            var library = File.ReadAllBytes(libraryPath);

            var ntHeaders = library[32]; // IMAGE_DOS_HEADERS + 32
            var optionalHeader = library[ntHeaders] + 24; // IMAGE_NT_HEADERS + 32
            var entryPointPointer = library[optionalHeader] + 16; // _IMAGE_OPTIONAL_HEADER + 16
            var entryPoint = BitConverter.ToInt32(library, entryPointPointer);

            Console.WriteLine($"{entryPoint:X}");

//            Console.WriteLine($"Process was opened. Handle: 0x{handle.ToInt32():X}. Allocating memory..");
//
//            var memory = WinAPI.VirtualAllocEx(handle,
//                IntPtr.Zero,
//                (uint) library.Length,
//                WinAPI.AllocationType.Commit | WinAPI.AllocationType.Reserve,
//                WinAPI.MemoryProtection.ExecuteReadWrite);
//

        }
    }
}
