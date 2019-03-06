using System;
using System.Runtime.InteropServices;

namespace SimpleConsoleGame
{
    internal class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct Player
        {
            public int CharacterId;
            public int CharacterModelId;
            public int Health;
            public int Ammo;
        }

        private static void Main()
        {
            Console.Title = "";
            Console.ForegroundColor = ConsoleColor.White;

            var player = new Player
            {
                CharacterId = 1005,
                CharacterModelId = 1,
                Health = 100,
                Ammo = 20
            };

            while (true)
            {
                player.Health -= 15;

                if (player.Health > 0)
                {
                    Console.Write($"Player health is {player.Health} ");
                    Console.ReadLine();
                }
                else
                {
                    Console.Write("Player is dead. ");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
        }
    }
}
