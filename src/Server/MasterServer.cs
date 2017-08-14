namespace Destiny.Server
{
    public static class MasterServer
    {
        public static bool IsAlive { get; private set; }

        public static LoginServer Login { get; private set; }
        public static WorldServer[] Worlds { get; private set; }
        public static CashShopServer CashShop { get; private set; }

        static MasterServer()
        {
            MasterServer.Login = new LoginServer(8484);
            MasterServer.Worlds = new WorldServer[1];

            for (byte i = 0; i < MasterServer.Worlds.Length; i++)
            {
                MasterServer.Worlds[i] = new WorldServer();
            }

            MasterServer.CashShop = new CashShopServer(9000);
        }

        public static void Start()
        {
            if (MasterServer.IsAlive)
            {
                return;
            }

            MasterServer.Login.Start();

            foreach (WorldServer world in MasterServer.Worlds)
            {
                world.Start();
            }

            MasterServer.CashShop.Start();

            MasterServer.IsAlive = true;

            Log.Success("MasterServer started.");
        }

        public static void Stop()
        {
            if (!MasterServer.IsAlive)
            {
                return;
            }

            MasterServer.Login.Stop();

            foreach (WorldServer world in MasterServer.Worlds)
            {
                world.Stop();
            }

            MasterServer.CashShop.Stop();

            MasterServer.IsAlive = false;

            Log.Warn("MasterServer stopped.");
        }
    }
}
