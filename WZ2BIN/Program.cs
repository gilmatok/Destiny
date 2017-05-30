using System;
using System.Diagnostics;

namespace WZ2BIN
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string inputPath = args[0];
            string outputPath = args[1];

            Stopwatch sw = new Stopwatch();

            sw.Start();

            ItemExport.Export(inputPath, outputPath);
            EquipExport.Export(inputPath, outputPath);
            MapExport.Export(inputPath, outputPath);

            sw.Stop();

            Console.WriteLine("Took {0}ms.", sw.ElapsedMilliseconds);

            Console.Read();
        }
    }
}
