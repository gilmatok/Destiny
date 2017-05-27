using Destiny.Data;
using Destiny.Utility;
using System.Collections.Generic;
using System.IO;

namespace Destiny.Server
{
    public sealed class DataProvider
    {
        public Dictionary<byte, Dictionary<byte, AbilityData>> Abilities { get; private set; }
        public Dictionary<int, Dictionary<byte, SkillData>> Skills { get; private set; }
        public Dictionary<int, NpcData> Npcs { get; private set; }
        public Dictionary<int, ReactorData> Reactors { get; private set; }
        public Dictionary<int, MobData> Mobs { get; private set; }
        public Dictionary<int, QuestData> Quests { get; private set; }
        public Dictionary<int, ItemData> Items { get; private set; }
        public Dictionary<int, MapData> Maps { get; private set; }

        public DataProvider()
        {
            this.Abilities = new Dictionary<byte, Dictionary<byte, AbilityData>>();
            this.Skills = new Dictionary<int, Dictionary<byte, SkillData>>();
            this.Npcs = new Dictionary<int, NpcData>();
            this.Reactors = new Dictionary<int, ReactorData>();
            this.Mobs = new Dictionary<int, MobData>();
            this.Quests = new Dictionary<int, QuestData>();
            this.Items = new Dictionary<int, ItemData>();
            this.Maps = new Dictionary<int, MapData>();
        }

        public void Load()
        {
            Logger.Initializer("Loading Data", () =>
            {
                int count;

                using (FileStream stream = File.Open(Config.Instance.Binary, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        count = reader.ReadInt32();

                        while (count-- > 0)
                        {
                            AbilityData ability = new AbilityData();

                            ability.Load(reader);

                            Dictionary<byte, AbilityData> levels = this.Abilities.GetOrDefault(ability.Identifier, null);

                            if (levels == null)
                            {
                                levels = new Dictionary<byte, AbilityData>();

                                this.Abilities.Add(ability.Identifier, levels);
                            }

                            levels.Add(ability.Level, ability);
                        }

                        count = reader.ReadInt32();

                        while (count-- > 0)
                        {
                            SkillData skill = new SkillData();

                            skill.Load(reader);

                            Dictionary<byte, SkillData> levels = this.Skills.GetOrDefault(skill.Identifier, null);

                            if (levels == null)
                            {
                                levels = new Dictionary<byte, SkillData>();

                                this.Skills.Add(skill.Identifier, levels);
                            }

                            levels.Add(skill.Level, skill);
                        }

                        count = reader.ReadInt32();

                        while (count-- > 0)
                        {
                            NpcData npc = new NpcData();

                            npc.Load(reader);

                            this.Npcs.Add(npc.Identifier, npc);
                        }

                        count = reader.ReadInt32();

                        while (count-- > 0)
                        {
                            ReactorData reactor = new ReactorData();

                            reactor.Load(reader);

                            this.Reactors.Add(reactor.Identifier, reactor);
                        }

                        count = reader.ReadInt32();

                        while (count-- > 0)
                        {
                            MobData mob = new MobData();

                            mob.Load(reader);

                            this.Mobs.Add(mob.Identifier, mob);
                        }

                        count = reader.ReadInt32();

                        while (count-- > 0)
                        {
                            QuestData quest = new QuestData();

                            quest.Load(reader);

                            this.Quests.Add(quest.Identifier, quest);
                        }

                        count = reader.ReadInt32();

                        while (count-- > 0)
                        {
                            ItemData item = new ItemData();

                            item.Load(reader);

                            this.Items.Add(item.Identifier, item);
                        }

                        count = reader.ReadInt32();

                        while (count-- > 0)
                        {
                            MapData map = new MapData();

                            map.Load(reader);

                            this.Maps.Add(map.Identifier, map);
                        }
                    }
                }
            });
        }
    }
}
