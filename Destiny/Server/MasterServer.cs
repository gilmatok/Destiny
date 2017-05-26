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

        public Database Database { get; private set; }
        public LoginServer Login { get; private set; }
        public WorldServer[] Worlds { get; private set; }

        // TODO: Get values from config.
        public MasterServer()
        {
            this.Database = new Database("mongodb://127.0.0.1:27017", "Destiny");

            int worlds = 1;
            byte channels = 2;

            this.Login = new LoginServer(8484);
            this.Worlds = new WorldServer[worlds];

            short port = 8485;

            for (int i = 0; i < worlds; i++)
            {
                this.Worlds[i] = new WorldServer((byte)i, port, channels);

                port += channels;
            }
        }

        public void Start()
        {
            // TODO: Load data.

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
