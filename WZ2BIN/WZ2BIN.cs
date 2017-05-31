using Destiny;
using System;
using System.Diagnostics;

namespace WZ2BIN
{
    internal static class WZ2BIN
    {
        private static void Main(string[] args)
        {
            Logger.Entitle("WZ2BIN");

            try
            {
                string inputPath = args[0];
                string outputPath = args[1];

                Stopwatch sw = new Stopwatch();

                sw.Start();

                ItemExport.Export(inputPath, outputPath);
                EquipExport.Export(inputPath, outputPath);
                MapExport.Export(inputPath, outputPath);

                sw.Stop();

                Logger.Write(LogLevel.Info, "Exported data in {0}ms.", sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }

            Logger.Write(LogLevel.Info, "Press any key to quit.");

            Console.Read();
        }
    }
}
