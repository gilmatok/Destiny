using Destiny.Maple.Data;
using System.Diagnostics;

namespace Destiny.Server
{
    public static class MasterServer
    {
        public static bool IsAlive { get; private set; }

        public static LoginServer Login { get; private set; }
        public static ChannelServer[] Channels { get; private set; }
        public static CashShopServer CashShop { get; private set; }

        static MasterServer()
        {
            MasterServer.Login = new LoginServer(8484);
            MasterServer.Channels = new ChannelServer[2];

            for (byte i = 0; i < 2; i++)
            {
                MasterServer.Channels[i] = new ChannelServer(i, (short)(8585 + i));
            }

            MasterServer.CashShop = new CashShopServer(9000);
        }

        public static void Start()
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            DataProvider.Initialize();

            MasterServer.Login.Start();

            foreach (ChannelServer channel in MasterServer.Channels)
            {
                channel.Start();
            }

            MasterServer.CashShop.Start();

            sw.Stop();

            MasterServer.IsAlive = true;

            Logger.Write(LogLevel.Success, "MasterServer started in {0}ms.", sw.ElapsedMilliseconds);
        }

        public static void Stop()
        {
            MasterServer.Login.Stop();

            foreach (ChannelServer channel in MasterServer.Channels)
            {
                channel.Stop();
            }

            MasterServer.CashShop.Stop();

            MasterServer.IsAlive = false;

            Logger.Write(LogLevel.Info, "MasterServer stopped.");
        }
    }
}
