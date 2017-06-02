using Destiny.Network;
using Destiny.Server;
using Destiny.Utility;
using MySql.Data.MySqlClient;
using Destiny.Core.IO;
using Destiny.Game.Maps;
using Destiny.Network.Packet;

namespace Destiny.Game.Characters
{
    public sealed class Character : MapObject, IMoveable
    {
        public MapleClient Client { get; private set; }

        public int ID { get; set; }
        public string Name { get; set; }
        public byte SpawnPoint { get; set; }
        public byte Stance { get; set; }
        public short Foothold { get; set; }
        public byte Portals { get; set; }
        public bool IsInitialized { get; set; }

        public CharacterStats Stats { get; private set; }
        public CharacterItems Items { get; private set; }
        public CharacterSkills Skills { get; private set; }
        public CharacterQuests Quests { get; private set; }
        public ControlledMobs ControlledMobs { get; private set; }
        public ControlledNpcs ControlledNpcs { get; private set; }

        public bool IsGm
        {
            get
            {
                return this.Client.Account.GmLevel >= GmLevel.Intern;
            }
        }

        public bool FacesLeft
        {
            get
            {
                return this.Stance % 2 == 0;
            }
        }

        public override MapObjectType Type
        {
            get
            {
                return MapObjectType.Character;
            }
        }

        public Character(MapleClient client, DatabaseQuery query, bool cashShop = false)
             : base()
        {
            this.Client = client;

            this.ID = query.GetInt("character_id");
            this.Name = query.GetString("name");
            this.Map = MasterServer.Instance.Worlds[this.Client.World].Channels[this.Client.Channel].Maps[query.GetInt("map")];
            this.SpawnPoint = query.GetByte("spawn_point");
            this.Stats = new CharacterStats(this, query);

            using (DatabaseQuery itemQuery = Database.Query("SELECT * FROM `items` WHERE `character_id` = @character_id", new MySqlParameter("character_id", this.ID)))
            {
                byte[] slots = new byte[(byte)InventoryType.Count];

                slots[(byte)InventoryType.Equipment] = query.GetByte("equipment_slots");
                slots[(byte)InventoryType.Usable] = query.GetByte("usable_slots");
                slots[(byte)InventoryType.Setup] = query.GetByte("setup_slots");
                slots[(byte)InventoryType.Etcetera] = query.GetByte("etcetera_slots");
                slots[(byte)InventoryType.Cash] = query.GetByte("cash_slots");

                this.Items = new CharacterItems(this, slots, itemQuery);
            }

            using (DatabaseQuery skillQuery = Database.Query("SELECT * FROM `skills` WHERE `character_id` = @character_id", new MySqlParameter("character_id", this.ID)))
            {
                this.Skills = new CharacterSkills(this, skillQuery);
            }

            using (DatabaseQuery questQuery = null)
            {
                this.Quests = new CharacterQuests(this, questQuery);
            }

            this.ControlledMobs = new ControlledMobs(this);
            this.ControlledNpcs = new ControlledNpcs(this);
        }

        public void Save()
        {
            Database.Execute("UPDATE `characters` SET skin = @skin, face = @face, hair = @hair, level = @level, job = @job, strength = @strength, " +
                             "dexterity = @dexterity, intelligence = @intelligence, luck = @luck, health = @health, max_health = @max_health, mana = @mana, " +
                             "max_mana = @max_mana, ability_points = @ability_points, skill_points = @skill_points, experience = @experience, fame = @fame, " +
                             "map = @map, spawn_point = @spawn_point, mesos = @mesos, equipment_slots = @equipment_slots, usable_slots = @usable_slots, " +
                             "setup_slots = @setup_slots, etcetera_slots = @etcetera_slots, cash_slots = @cash_slots WHERE `character_id` = @character_id",
                             new MySqlParameter("character_id", this.ID),
                             new MySqlParameter("skin", this.Stats.Skin),
                             new MySqlParameter("face", this.Stats.Face),
                             new MySqlParameter("hair", this.Stats.Hair),
                             new MySqlParameter("level", this.Stats.Level),
                             new MySqlParameter("job", (short)this.Stats.Job),
                             new MySqlParameter("strength", this.Stats.Strength),
                             new MySqlParameter("dexterity", this.Stats.Dexterity),
                             new MySqlParameter("intelligence", this.Stats.Intelligence),
                             new MySqlParameter("luck", this.Stats.Luck),
                             new MySqlParameter("health", this.Stats.Health),
                             new MySqlParameter("max_health", this.Stats.MaxHealth),
                             new MySqlParameter("mana", this.Stats.Mana),
                             new MySqlParameter("max_mana", this.Stats.MaxMana),
                             new MySqlParameter("ability_points", this.Stats.AbilityPoints),
                             new MySqlParameter("skill_points", this.Stats.SkillPoints),
                             new MySqlParameter("experience", this.Stats.Experience),
                             new MySqlParameter("fame", this.Stats.Fame),
                             new MySqlParameter("map", this.Map.MapleID),
                             new MySqlParameter("spawn_point", this.SpawnPoint),
                             new MySqlParameter("mesos", this.Stats.Mesos),
                             new MySqlParameter("equipment_slots", 24),
                             new MySqlParameter("usable_slots", 24),
                             new MySqlParameter("setup_slots", 24),
                             new MySqlParameter("etcetera_slots", 24),
                             new MySqlParameter("cash_slots", 48));
        }

        public void Notify(string message, NoticeType type = NoticeType.Pink)
        {
            this.Client.Send(UserPacket.BrodcastMsg(message, type));
        }

        public void ChangeMap(int mapID, byte portalID = 0)
        {
            this.SpawnPoint = portalID;

            this.Map.Characters.Remove(this);

            this.Map = MasterServer.Instance.Worlds[this.Client.World].Channels[this.Client.Channel].Maps[mapID];

            this.Client.Send(MapPacket.SetField(this, false));

            this.Map.Characters.Add(this);
        }

        public void Encode(OutPacket oPacket, long flag = long.MaxValue)
        {
            oPacket
                .WriteLong(flag)
                .WriteByte(); // NOTE: Unknown.

            this.Stats.Encode(oPacket);

            oPacket
                .WriteByte(20) // NOTE: Max buddylist size.
                .WriteBool(false) // NOTE: Blessing of Fairy.
                .WriteInt(); // NOTE: Mesos.

            this.Items.Encode(oPacket);
            this.Skills.Encode(oPacket);
            this.Quests.Encode(oPacket);

            oPacket
                .WriteShort() // NOTE: Mini games record.
                .WriteShort() // NOTE: Rings (1).
                .WriteShort() // NOTE: Rings (2). 
                .WriteShort(); // NOTE: Rings (3).

            // NOTE: Teleport rock locations.
            for (int i = 0; i < 15; i++)
            {
                oPacket.WriteInt(999999999);
            }

            oPacket
                .WriteInt() // NOTE: Monster book cover ID.
                .WriteByte() // NOTE: Unknown.
                .WriteShort() // NOTE: Monster book cards count.
                .WriteShort() // NOTE: New year cards.
                .WriteShort() // NOTE: Area information.
                .WriteShort(); // NOTE: Unknown.
        }
    }
}
