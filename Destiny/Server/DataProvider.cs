using Destiny.Game.Data;
using Destiny.Utility;
using System.Collections.Generic;
using System.IO;

namespace Destiny.Server
{
    public sealed class DataProvider
    {
        public Dictionary<int, ItemData> Items { get; private set; }
        public Dictionary<int, EquipData> Equips { get; private set; }
        public Dictionary<int, Dictionary<byte, SkillData>> Skills { get; private set; }
        public Dictionary<int, MapData> Maps { get; private set; }

        public void Initialize()
        {
            this.LoadItems();
            this.LoadEquips();
            this.LoadSkills();
            this.LoadMaps();
        }

        private void LoadItems()
        {
            using (FileStream stream = File.Open(Path.Combine(Config.Instance.Binary, "Items.bin"), FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int count = reader.ReadInt32();

                    this.Items = new Dictionary<int, ItemData>(count);

                    while (count-- > 0)
                    {
                        ItemData item = new ItemData();

                        item.Load(reader);

                        this.Items.Add(item.MapleID, item);
                    }
                }
            }
        }

        private void LoadEquips()
        {
            using (FileStream stream = File.Open(Path.Combine(Config.Instance.Binary, "Equips.bin"), FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int count = reader.ReadInt32();

                    this.Equips = new Dictionary<int, EquipData>(count);

                    while (count-- > 0)
                    {
                        EquipData equip = new EquipData();

                        equip.Load(reader);

                        this.Equips.Add(equip.MapleID, equip);
                    }
                }
            }
        }

        private void LoadSkills()
        {
            using (FileStream stream = File.Open(Path.Combine(Config.Instance.Binary, "Skills.bin"), FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int count = reader.ReadInt32();

                    this.Skills = new Dictionary<int, Dictionary<byte, SkillData>>(count);

                    while (count-- > 0)
                    {
                        SkillData skill = new SkillData();

                        skill.Load(reader);

                        if (!this.Skills.ContainsKey(skill.MapleID))
                        {
                            this.Skills.Add(skill.MapleID, new Dictionary<byte, SkillData>());
                        }

                        this.Skills[skill.MapleID].Add(skill.Level, skill);
                    }
                }
            }
        }

        private void LoadMaps()
        {
            using (FileStream stream = File.Open(Path.Combine(Config.Instance.Binary, "Maps.bin"), FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    int count = reader.ReadInt32();

                    this.Maps = new Dictionary<int, MapData>(count);

                    while (count-- > 0)
                    {
                        MapData map = new MapData();

                        map.Load(reader);

                        this.Maps.Add(map.MapleID, map);
                    }
                }
            }
        }
    }
}
