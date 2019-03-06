using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Injector
{
    internal class Program
    {
        private static void Main()
        {
            Console.Title = "";
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("> Enter DLL name: ");
            var dllName = Console.ReadLine();

            if (string.IsNullOrEmpty(dllName) || !File.Exists(dllName))
            {
                Console.WriteLine("DLL name is invalid!");
                Console.ReadLine();
                return;
            }

            var fullPath = Path.GetFullPath(dllName);

            Console.Write("> Enter process name: ");
            var processName = Console.ReadLine();

            if (string.IsNullOrEmpty(dllName))
            {
                Console.WriteLine("Process name is invalid!");
                Console.ReadLine();
                return;
            }

            var prcs = Process.GetProcessesByName(processName);

            if (prcs.Length == 0)
            {
                Console.WriteLine("Process wasn't found.");
                Console.ReadLine();
                return;
            }

            var prcId = prcs.First().Id;
            
            Injector.Inject(fullPath, prcId, InjectionType.LoadLibrary);
            Console.ReadLine();
        }
    }
}
