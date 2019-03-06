using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ExternalCheat.Memory;
using ExternalCheat.Memory.Pattern;

namespace ExternalCheat
{
    internal class Program
    {
        private static void Main()
        {
            Console.Title = "External Cheat Example";
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Waiting for game process..");

            var processId = WaitForGame();

            Console.WriteLine($"Game process found. ID: {processId}");

            var handle = GetGameHandle(processId);

            if (handle == IntPtr.Zero)
            {
                CriticalError("Error. Process handle acquirement failed.\n" +
                              "Insufficient rights?");
            }

            Console.WriteLine($"Handle was acquired: 0x{handle.ToInt32():X}");
            
            var memory = new MemoryManager(handle);
            var playerBase = memory.PatternScan(new MemoryPattern("ED 03 00 00 01 00 00 00"));

            if (playerBase == IntPtr.Zero)
            {
                CriticalError("Can't find player in memory!");
            }

            var playerHealth = playerBase + 24;

            Console.WriteLine($"Current health: {memory.ReadInt32(playerHealth)}");

            memory.WriteInt32(playerHealth, int.MaxValue);

            Console.WriteLine($"New health: {memory.ReadInt32(playerHealth)}");

            Console.ReadKey();
        }

        private static int WaitForGame()
        {
            while (true)
            {
                var prcs = Process.GetProcessesByName("SimpleConsoleGame");

                if (prcs.Length != 0)
                {
                    return prcs.First().Id;
                }

                Thread.Sleep(150);
            }
        }

        private static IntPtr GetGameHandle(int id)
        {
            return WinAPI.OpenProcess(WinAPI.ProcessAccessFlags.All, false, id);
        }

        private static void CriticalError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n{message}\n\n");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Press any key to exit..");

            Console.ReadKey(true);
            
            Environment.Exit(0);
        }
    }
}
