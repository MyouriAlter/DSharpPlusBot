using System;
using System.Text;
using DSharpPlusBot.Client;

namespace DSharpPlusBot
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            new OliviaClient().RunClientAsync().GetAwaiter().GetResult();
        }
    }
}