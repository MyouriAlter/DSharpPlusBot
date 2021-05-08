using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace DSharpPlusBot.Utilities
{
    public class BotUtilities
    {
        private static readonly Random Random = new Random();

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicallyInstalledSystemMemory(out long totalMemoryInKilobytes);

        public static float InstalledMemory()
        {
            GetPhysicallyInstalledSystemMemory(out var memoryInKilobytes);
            return (float) memoryInKilobytes / 1024;
        }

        public static float GetCpuPerf()
        {
            var cpuPerformance = new PerformanceCounter
            {
                CategoryName = "Processor",
                CounterName = "% Processor Time",
                InstanceName = "_Total"
            };
            var initialValue = cpuPerformance.NextValue();
            Thread.Sleep(1000);
            var cpuValue = cpuPerformance.NextValue();
            return cpuValue;
        }

        public static float RamUsage()
        {
            var memCounter = new PerformanceCounter
            {
                CategoryName = "Memory",
                CounterName = "Available MBytes"
            };
            var initialValue = memCounter.NextValue();
            Thread.Sleep(1000);
            var ramValue = memCounter.NextValue();
            return ramValue;
        }
        
        public static string GetRandomAlphaNumeric(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static int RandomColor()
        {
            return Random.Next(1, 255);
        }
        
    }
}