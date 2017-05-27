using Destiny.Server;
using Destiny.Utility;
using System;

namespace Destiny
{
    internal static class Destiny
    {
        public static Random Random = new Random();

        private static void Main(string[] args)
        {
            Logger.Entitle("Destiny");

            try
            {
                Config.Load();

                Database.Initialize();

                MasterServer.Instance.Start();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }

            while (MasterServer.Instance.IsAlive)
            {
                // TODO: Implement a CLI of some sort.

                Console.Read();
            }

            Logger.Write(LogLevel.Info, "Press any key to quit.");

            Console.Read();
        }
    }
}
