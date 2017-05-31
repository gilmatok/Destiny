using Destiny.Utility;
using System.Diagnostics;

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
        public CommandFactory Commands { get; private set; }
        public LoginServer Login { get; private set; }
        public WorldServer[] Worlds { get; private set; }

        public MasterServer()
        {
            this.Data = new DataProvider();
            this.Commands = new CommandFactory();
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
            Stopwatch sw = new Stopwatch();

            sw.Start();

            this.Data.Initialize();
            this.Commands.Initialize();

            this.Login.Start();

            foreach (WorldServer world in this.Worlds)
            {
                world.Start();
            }

            this.IsAlive = true;

            sw.Stop();

            Logger.Write(LogLevel.Success, "Destiny started in {0:N2} seconds.", sw.Elapsed.TotalSeconds);
        }

        public void Stop()
        {
            this.Login.Stop();

            foreach (WorldServer world in this.Worlds)
            {
                world.Stop();
            }

            this.IsAlive = false;

            Logger.Write(LogLevel.Info, "Destiny stopped.");
        }
    }
}
