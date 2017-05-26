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

        public LoginServer Login { get; private set; }
        public WorldServer[] Worlds { get; private set; }

        public MasterServer()
        {
            // TODO: Get values from config.
            int worlds = 1;
            byte channels = 2;

            this.Login = new LoginServer(8484); // TODO: Get port from config.
            this.Worlds = new WorldServer[worlds];

            short port = 8485;

            for (int i = 0; i < worlds; i++)
            {
                this.Worlds[i] = new WorldServer((byte)i, port, channels);

                port += channels;
            }
        }

        public void Run()
        {
            // TODO: Load data.

            this.Login.Run();

            foreach (WorldServer world in this.Worlds)
            {
                world.Run();
            }

            this.IsAlive = true;

            Logger.Write(LogLevel.Success, "MasterServer started.");
        }

        public void Shutdown()
        {
            this.Login.Shutdown();

            foreach (WorldServer world in this.Worlds)
            {
                world.Shutdown();
            }

            this.IsAlive = false;

            Logger.Write(LogLevel.Info, "MasterServer stopped.");
        }
    }
}
