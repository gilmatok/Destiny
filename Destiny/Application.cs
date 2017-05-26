using Destiny.Server;
using System;

namespace Destiny
{
    internal class Application
    {
        private static void Main(string[] args)
        {
            Logger.Entitle("Destiny");

            try
            {
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
