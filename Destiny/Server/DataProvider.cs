using Destiny.Server.Data;

namespace Destiny.Server
{
    public sealed class DataProvider
    {
        public BeautyDataProvider Beauty { get; private set; }
        public ItemDataProvider Items { get; private set; }
        public EquipDataProvider Equips { get; private set; }
        public NpcDataProvider Npcs { get; private set; }
        public SkillDataProvider Skills { get; private set; }
        public AbilityDataProvider Abilities { get; private set; }
        public MapDataProvider Maps { get; private set; }

        public DataProvider()
        {
            this.Beauty = new BeautyDataProvider();
            this.Items = new ItemDataProvider();
            this.Equips = new EquipDataProvider();
            this.Npcs = new NpcDataProvider();
            this.Skills = new SkillDataProvider();
            this.Abilities = new AbilityDataProvider();
            this.Maps = new MapDataProvider();
        }

        public void Initialize()
        {
            this.Beauty.Load();
            this.Items.Load();
            this.Equips.Load();
            this.Npcs.Load();
            this.Skills.Load();
            this.Abilities.Load();
            this.Maps.Load();
        }
    }
}
