using Destiny.Network;
using Destiny.Server;
using Destiny.Utility;
using MySql.Data.MySqlClient;
using Destiny.Core.IO;
using Destiny.Game.Maps;
using Destiny.Server.Data;
using System;

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
            this.Stats = new CharacterStats(this, query);

            int mapID = query.GetInt("map");
            byte spawnPoint = query.GetByte("spawn_point");

            if (this.IsGm) // NOTE: Gms are spawned in the Gm map by default to avoid being seen by other players.
            {
                mapID = 180000000;
                spawnPoint = 0;
            }
            else if (MasterServer.Instance.Data.Maps.GetMapData(mapID).ForcedReturnMapID != MapData.INVALID_MAP_ID)
            {
                mapID = MasterServer.Instance.Data.Maps.GetMapData(mapID).ForcedReturnMapID;
                spawnPoint = 0; // TODO: Should it be randomized?
            }
            else if (!MasterServer.Instance.Data.Maps.IsValidMap(mapID)) // NOTE: Just in case the user purposely edits a wrong map in the database.
            {
                mapID = 100000000;
                spawnPoint = 0;
            }

            this.Map = MasterServer.Instance.Worlds[this.Client.World].Channels[this.Client.Channel].Maps[mapID];
            this.SpawnPoint = spawnPoint;

            this.Position = this.Map.Portals[this.SpawnPoint].Position;
            this.Foothold = 0;
            this.Stance = 0;

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

        public void Initialize(bool cashShop = false)
        {
            using (OutPacket oPacket = new OutPacket(cashShop ? SendOps.SetCashShop : SendOps.SetField))
            {
                if (cashShop)
                {
                    this.Encode(oPacket);

                    oPacket
                        .WriteByte(1)
                        .WriteMapleString(this.Client.Account.Username)
                        .WriteInt()
                        .WriteShort()
                        .WriteZero(121);

                    for (int i = 1; i <= 8; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            oPacket
                                .WriteInt(i)
                                .WriteInt(j)
                                .WriteInt(50200004)
                                .WriteInt(i)
                                .WriteInt(j)
                                .WriteInt(50200069)
                                .WriteInt(i)
                                .WriteInt(j)
                                .WriteInt(50200117)
                                .WriteInt(i)
                                .WriteInt(j)
                                .WriteInt(50100008)
                                .WriteInt(i)
                                .WriteInt(j)
                                .WriteInt(50000047);
                        }
                    }

                    oPacket
                        .WriteInt()
                        .WriteShort()
                        .WriteByte()
                        .WriteInt(75);
                }
                else
                {
                    oPacket
                        .WriteInt(this.Client.Channel)
                        .WriteByte(++this.Portals)
                        .WriteBool(true)
                        .WriteShort(); // NOTE: Floating messages at top corner.

                    for (int i = 0; i < 3; i++)
                    {
                        oPacket.WriteInt(Constants.Random.Next());
                    }

                    this.Encode(oPacket);
                    oPacket.WriteDateTime(DateTime.Now);
                }

                this.Client.Send(oPacket);
            }

            this.IsInitialized = true;

            if (!cashShop)
            {
                this.Map.Characters.Add(this);

                this.Notify(MasterServer.Instance.Worlds[this.Client.World].TickerMessage, NoticeType.Ticker);
            }
        }

        public void Notify(string message, NoticeType type = NoticeType.Pink)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.BroadcastMsg))
            {
                oPacket.WriteByte((byte)type);

                if (type == NoticeType.Ticker)
                {
                    oPacket.WriteBool(!string.IsNullOrEmpty(message));
                }

                oPacket.WriteMapleString(message);

                this.Client.Send(oPacket);
            }
        }

        public void ChangeMap(int mapID, byte portalID = 0)
        {
            this.Map.Characters.Remove(this);

            this.SpawnPoint = portalID;

            using (OutPacket oPacket = new OutPacket(SendOps.SetField))
            {
                oPacket
                    .WriteInt(this.Client.Channel)
                    .WriteByte(++this.Portals)
                    .WriteBool()
                    .WriteShort()
                    .WriteByte()
                    .WriteInt(mapID)
                    .WriteByte(this.SpawnPoint)
                    .WriteShort(this.Stats.Health)
                    .WriteBool(false) // NOTE: Follow.
                    .WriteDateTime(DateTime.Now);

                this.Client.Send(oPacket);
            }

            MasterServer.Instance.Worlds[this.Client.World].Channels[this.Client.Channel].Maps[mapID].Characters.Add(this);
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
