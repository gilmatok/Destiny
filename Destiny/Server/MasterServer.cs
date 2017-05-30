using Destiny.Utility;

namespace Destiny.Server
{
    public sealed class MasterServer
    {
        private static MasterServer mInstance;

        public static MasterServer Instance
        {
            get
            {
                return mInstance ?? (mInstance = new MasterServer());
            }
        }

        public bool IsAlive { get; private set; }

        public DataProvider Data { get; private set; }
        public LoginServer Login { get; private set; }
        public WorldServer[] Worlds { get; private set; }

        public MasterServer()
        {
            this.Data = new DataProvider();
            this.Login = new LoginServer(Config.Instance.Login);
            this.Worlds = new WorldServer[Config.Instance.Worlds.Count];

            int i = 0;

            foreach (CWorld config in Config.Instance.Worlds)
            {
                this.Worlds[i++] = new WorldServer(config);
            }
        }

        public void Start()
        {
            this.Data.Load();

            this.Login.Start();

            foreach (WorldServer world in this.Worlds)
            {
                world.Start();
            }

            this.IsAlive = true;

            Logger.Write(LogLevel.Success, "MasterServer started.");
        }

        public void Stop()
        {
            this.Login.Stop();

            foreach (WorldServer world in this.Worlds)
            {
                world.Stop();
            }

            this.IsAlive = false;

            Logger.Write(LogLevel.Info, "MasterServer stopped.");
        }
    }
}
