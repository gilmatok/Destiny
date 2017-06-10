using Destiny.Game.Commands;
using Destiny.Game.Data;
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
        
        public LoginServer Login { get; private set; }
        public WorldServer[] Worlds { get; private set; }
        public ShopServer Shop { get; private set; }

        public BeautyDataProvider Beauty { get; private set; }
        public ItemDataProvider Items { get; private set; }
        public EquipDataProvider Equips { get; private set; }
        public NpcDataProvider Npcs { get; private set; }
        public SkillDataProvider Skills { get; private set; }
        public AbilityDataProvider Abilities { get; private set; }
        public MapDataProvider Maps { get; private set; }
        public CommandFactory Commands { get; private set; }

        public MasterServer()
        {
            this.Login = new LoginServer(Config.Instance.Login);
            this.Worlds = new WorldServer[Config.Instance.Worlds.Count];
            this.Shop = new ShopServer(Config.Instance.Shop);

            int i = 0;

            foreach (CWorld config in Config.Instance.Worlds)
            {
                this.Worlds[i++] = new WorldServer(config);
            }

            this.Beauty = new BeautyDataProvider();
            this.Items = new ItemDataProvider();
            this.Equips = new EquipDataProvider();
            this.Npcs = new NpcDataProvider();
            this.Skills = new SkillDataProvider();
            this.Abilities = new AbilityDataProvider();
            this.Maps = new MapDataProvider();
            this.Commands = new CommandFactory();
        }

        public void Start()
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();

            this.LoadData();

            this.Login.Start();

            foreach (WorldServer world in this.Worlds)
            {
                world.Start();
            }

            this.Shop.Start();

            this.IsAlive = true;

            sw.Stop();

            Logger.Write(LogLevel.Success, "Destiny started in {0:N2} seconds.", sw.Elapsed.TotalSeconds);
        }

        private void LoadData()
        {
            this.Beauty.Load();
            this.Items.Load();
            this.Equips.Load();
            this.Npcs.Load();
            this.Skills.Load();
            this.Abilities.Load();
            this.Maps.Load();
            this.Commands.Load();
        }

        public void Stop()
        {
            this.Login.Stop();

            foreach (WorldServer world in this.Worlds)
            {
                world.Stop();
            }

            this.Shop.Stop();

            this.IsAlive = false;

            Logger.Write(LogLevel.Info, "Destiny stopped.");
        }
    }
}
