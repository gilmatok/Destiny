using Destiny.Server;
using Destiny.Utility;
using System;

namespace Destiny
{
    internal static class Application
    {
        private static void Main(string[] args)
        {
            Logger.Entitle("Destiny");

            try
            {
                Database.Initialize();

                MasterServer.Start();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }

            while (MasterServer.IsAlive)
            {
                // TODO: Implement a CLI of some sort.

                Console.Read();
            }

            Logger.Write(LogLevel.Info, "Press any key to quit.");

            Console.Read();
        }
    }
}