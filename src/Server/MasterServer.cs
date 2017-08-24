using Destiny.Data;
using Destiny.IO;

namespace Destiny.Server
{
    public static class MasterServer
    {
        public static bool IsAlive { get; private set; }

        public static LoginServer Login { get; private set; }
        public static WorldServer[] Worlds { get; private set; }

        static MasterServer()
        {
            int worldCount = Settings.GetInt("Server/Worlds");
            MasterServer.Login = new LoginServer(8484);
            MasterServer.Worlds = new WorldServer[worldCount];

            for (byte i = 0; i < MasterServer.Worlds.Length; i++)
            {
                MasterServer.Worlds[i] = new WorldServer(i);
            }
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

            Database.Execute("UPDATE characters SET PartyID = {0}", new object[] { null }); // TODO: Move else-where.

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

            MasterServer.IsAlive = false;

            Log.Warn("MasterServer stopped.");
        }
    }
}
