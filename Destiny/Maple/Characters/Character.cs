using Destiny.Server;
using Destiny.Core.IO;
using Destiny.Maple.Maps;
using System;
using Destiny.Core.Network;
using Destiny.Utility;
using MySql.Data.MySqlClient;

namespace Destiny.Maple.Characters
{
    public sealed class Character : MapObject, IMoveable, ISpawnable
    {
        public MapleClient Client { get; private set; }

        public int ID { get; set; }
        public int AccountID { get; set; }
        public string Name { get; set; }
        public bool IsInitialized { get; private set; }

        public byte SpawnPoint { get; set; }
        public byte Stance { get; set; }
        public short Foothold { get; set; }
        public byte Portals { get; set; }

        public CharacterItems Items { get; private set; }
        public CharacterSkills Skills { get; private set; }
        public CharacterQuests Quests { get; private set; }
        public ControlledMobs ControlledMobs { get; private set; }
        public ControlledNpcs ControlledNpcs { get; private set; }

        private Gender m_gender;
        private byte m_skin;
        private int m_face;
        private int m_hair;
        private byte m_level;
        private Job m_job;
        private short m_strength;
        private short m_dexterity;
        private short m_intelligence;
        private short m_luck;
        private short m_health;
        private short m_maxHealth;
        private short m_mana;
        private short m_maxMana;
        private short m_abilityPoints;
        private short m_skillPoints;
        private int m_experience;
        private short m_fame;
        private int m_mesos;

        public Gender Gender
        {
            get
            {
                return m_gender;
            }
            set
            {
                m_gender = value;

                if (this.IsInitialized)
                {
                    // TODO: Is there a gender set packet?
                }
            }
        }

        public byte Skin
        {
            get
            {
                return m_skin;
            }
            set
            {
                m_skin = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Skin);
                    this.UpdateApperance();
                }
            }
        }

        public int Face
        {
            get
            {
                return m_face;
            }
            set
            {
                m_face = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Face);
                    this.UpdateApperance();
                }
            }
        }

        public int Hair
        {
            get
            {
                return m_hair;
            }
            set
            {
                m_hair = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Hair);
                    this.UpdateApperance();
                }
            }
        }

        public byte Level
        {
            get
            {
                return m_level;
            }

            set
            {
                m_level = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Level);

                    using (OutPacket oPacket = new OutPacket(SendOps.ShowForeignBuff))
                    {
                        oPacket
                            .WriteInt(this.ID)
                            .WriteByte();

                        this.Map.Broadcast(oPacket, this);
                    }
                }
            }
        }

        public Job Job
        {
            get
            {
                return m_job;
            }
            set
            {
                m_job = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Job);

                    using (OutPacket oPacket = new OutPacket(SendOps.ShowForeignBuff))
                    {
                        oPacket
                            .WriteInt(this.ID)
                            .WriteByte(8);

                        this.Map.Broadcast(oPacket, this);
                    }
                }
            }
        }

        public short Strength
        {
            get
            {
                return m_strength;
            }

            set
            {
                m_strength = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Strength);
                }
            }
        }

        public short Dexterity
        {
            get
            {
                return m_dexterity;
            }

            set
            {
                m_dexterity = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Dexterity);
                }
            }
        }

        public short Intelligence
        {
            get
            {
                return m_intelligence;
            }

            set
            {
                m_intelligence = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Intelligence);
                }
            }
        }

        public short Luck
        {
            get
            {
                return m_luck;
            }

            set
            {
                m_luck = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Luck);
                }
            }
        }

        public short Health
        {
            get
            {
                return m_health;
            }

            set
            {
                m_health = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Health);
                }
            }
        }

        public short MaxHealth
        {
            get
            {
                return m_maxHealth;
            }

            set
            {
                m_maxHealth = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.MaxHealth);
                }
            }
        }

        public short Mana
        {
            get
            {
                return m_mana;
            }

            set
            {
                m_mana = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Mana);
                }
            }
        }

        public short MaxMana
        {
            get
            {
                return m_maxMana;
            }

            set
            {
                m_maxMana = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.MaxMana);
                }
            }
        }

        public short AbilityPoints
        {
            get
            {
                return m_abilityPoints;
            }

            set
            {
                m_abilityPoints = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.AbilityPoints);
                }
            }
        }

        public short SkillPoints
        {
            get
            {
                return m_skillPoints;
            }

            set
            {
                m_skillPoints = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.SkillPoints);
                }
            }
        }

        public int Experience
        {
            get
            {
                return m_experience;
            }

            set
            {
                m_experience = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Experience);
                }
            }
        }

        public short Fame
        {
            get
            {
                return m_fame;
            }

            set
            {
                m_fame = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Fame);
                }
            }
        }

        public int Mesos
        {
            get
            {
                return m_mesos;
            }
            set
            {
                m_mesos = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Mesos);
                }
            }
        }

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

        public bool IsRanked
        {
            get
            {
                return this.Level >= 30;
            }
        }

        public Character(MapleClient client, DatabaseQuery query)
        {
            this.Client = client;

            this.ID = query.GetInt("character_id");
            this.AccountID = query.GetInt("account_id");
            this.Name = query.GetString("name");
            this.Gender = (Gender)query.GetByte("gender");
            this.Skin = query.GetByte("skin");
            this.Face = query.GetInt("face");
            this.Hair = query.GetInt("hair");
            this.Level = query.GetByte("level");
            this.Job = (Job)query.GetShort("job");
            this.Strength = query.GetShort("strength");
            this.Dexterity = query.GetShort("dexterity");
            this.Intelligence = query.GetShort("intelligence");
            this.Luck = query.GetShort("luck");
            this.Health = query.GetShort("health");
            this.MaxHealth = query.GetShort("max_health");
            this.Mana = query.GetShort("mana");
            this.MaxMana = query.GetShort("max_mana");
            this.AbilityPoints = query.GetShort("ability_points");
            this.SkillPoints = query.GetShort("skill_points");
            this.Experience = query.GetInt("experience");
            this.Fame = query.GetShort("fame");
            this.Mesos = query.GetInt("mesos");
            this.Map = MasterServer.Channels[this.Client.Channel].Maps[query.GetInt("map")];
            this.SpawnPoint = query.GetByte("spawn_point");

            using (DatabaseQuery itemQuery = Database.Query("SELECT * FROM items WHERE character_id = @character_id", new MySqlParameter("character_id", this.ID)))
            {
                byte[] slots = new byte[(byte)InventoryType.Count];

                slots[(byte)InventoryType.Equipment] = query.GetByte("equipment_slots");
                slots[(byte)InventoryType.Usable] = query.GetByte("usable_slots");
                slots[(byte)InventoryType.Setup] = query.GetByte("setup_slots");
                slots[(byte)InventoryType.Etcetera] = query.GetByte("etcetera_slots");
                slots[(byte)InventoryType.Cash] = query.GetByte("cash_slots");

                this.Items = new CharacterItems(this, slots, itemQuery);
            }

            using (DatabaseQuery skillQuery = Database.Query("SELECT * FROM skills WHERE character_id = @character_id", new MySqlParameter("character_id", this.ID)))
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
                             new MySqlParameter("skin", this.Skin),
                             new MySqlParameter("face", this.Face),
                             new MySqlParameter("hair", this.Hair),
                             new MySqlParameter("level", this.Level),
                             new MySqlParameter("job", (short)this.Job),
                             new MySqlParameter("strength", this.Strength),
                             new MySqlParameter("dexterity", this.Dexterity),
                             new MySqlParameter("intelligence", this.Intelligence),
                             new MySqlParameter("luck", this.Luck),
                             new MySqlParameter("health", this.Health),
                             new MySqlParameter("max_health", this.MaxHealth),
                             new MySqlParameter("mana", this.Mana),
                             new MySqlParameter("max_mana", this.MaxMana),
                             new MySqlParameter("ability_points", this.AbilityPoints),
                             new MySqlParameter("skill_points", this.SkillPoints),
                             new MySqlParameter("experience", this.Experience),
                             new MySqlParameter("fame", this.Fame),
                             new MySqlParameter("map", this.Map.MapleID),
                             new MySqlParameter("spawn_point", this.SpawnPoint),
                             new MySqlParameter("mesos", this.Mesos),
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
                    this.EncodeData(oPacket);

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

                    this.EncodeData(oPacket);

                    oPacket.WriteDateTime(DateTime.Now);
                }

                this.Client.Send(oPacket);
            }

            this.IsInitialized = true;

            if (!cashShop)
            {
                this.Map.Characters.Add(this);
            }
        }

        public void Update(params StatisticType[] statistics)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.StatChanged))
            {
                oPacket.WriteBool(); // TODO: bOnExclRequest.

                int flag = 0;

                foreach (StatisticType statistic in statistics)
                {
                    flag |= (int)statistic;
                }

                oPacket.WriteInt(flag);

                Array.Sort(statistics);

                foreach (StatisticType statistic in statistics)
                {
                    switch (statistic)
                    {
                        case StatisticType.Skin:
                            oPacket.WriteByte(this.Skin);
                            break;

                        case StatisticType.Face:
                            oPacket.WriteInt(this.Face);
                            break;

                        case StatisticType.Hair:
                            oPacket.WriteInt(this.Hair);
                            break;

                        case StatisticType.Level:
                            oPacket.WriteByte(this.Level);
                            break;

                        case StatisticType.Job:
                            oPacket.WriteShort((short)this.Job);
                            break;

                        case StatisticType.Strength:
                            oPacket.WriteShort(this.Strength);
                            break;

                        case StatisticType.Dexterity:
                            oPacket.WriteShort(this.Dexterity);
                            break;

                        case StatisticType.Intelligence:
                            oPacket.WriteShort(this.Intelligence);
                            break;

                        case StatisticType.Luck:
                            oPacket.WriteShort(this.Luck);
                            break;

                        case StatisticType.Health:
                            oPacket.WriteShort(this.Health);
                            break;

                        case StatisticType.MaxHealth:
                            oPacket.WriteShort(this.MaxHealth);
                            break;

                        case StatisticType.Mana:
                            oPacket.WriteShort(this.Mana);
                            break;

                        case StatisticType.MaxMana:
                            oPacket.WriteShort(this.MaxMana);
                            break;

                        case StatisticType.AbilityPoints:
                            oPacket.WriteShort(this.AbilityPoints);
                            break;

                        case StatisticType.SkillPoints:
                            oPacket.WriteShort(this.SkillPoints);
                            break;

                        case StatisticType.Experience:
                            oPacket.WriteInt(this.Experience);
                            break;

                        case StatisticType.Fame:
                            oPacket.WriteShort(this.Fame);
                            break;

                        case StatisticType.Mesos:
                            oPacket.WriteInt(this.Mesos);
                            break;
                    }
                }

                this.Client.Send(oPacket);
            }
        }

        private void UpdateApperance()
        {
            using (OutPacket oPacket = new OutPacket(SendOps.AvatarModified))
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteBool(true);

                this.EncodeApperance(oPacket);

                oPacket
                    .WriteByte()
                    .WriteShort();

                this.Map.Broadcast(oPacket, this);
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
                    .WriteShort(this.Health)
                    .WriteBool(false) // NOTE: Follow.
                    .WriteDateTime(DateTime.Now);

                this.Client.Send(oPacket);
            }

            MasterServer.Channels[this.Client.Channel].Maps[mapID].Characters.Add(this);
        }

        public void EncodeStatistics(OutPacket oPacket)
        {
            oPacket
                .WriteInt(this.ID)
                .WritePaddedString(this.Name, 13)
                .WriteByte((byte)this.Gender)
                .WriteByte(this.Skin)
                .WriteInt(this.Face)
                .WriteInt(this.Hair)
                .WriteLong()
                .WriteLong()
                .WriteLong()
                .WriteByte(this.Level)
                .WriteShort((short)this.Job)
                .WriteShort(this.Strength)
                .WriteShort(this.Dexterity)
                .WriteShort(this.Intelligence)
                .WriteShort(this.Luck)
                .WriteShort(this.Health)
                .WriteShort(this.MaxHealth)
                .WriteShort(this.Mana)
                .WriteShort(this.MaxMana)
                .WriteShort(this.AbilityPoints)
                .WriteShort(this.SkillPoints)
                .WriteInt(this.Experience)
                .WriteShort(this.Fame)
                .WriteInt()
                .WriteInt(this.Map.MapleID)
                .WriteByte(this.SpawnPoint)
                .WriteInt();
        }

        public void EncodeApperance(OutPacket oPacket)
        {
            oPacket
                .WriteByte((byte)this.Gender)
                .WriteByte(this.Skin)
                .WriteInt(this.Face)
                .WriteBool(true)
                .WriteInt(this.Hair);

            this.Items.EncodeEquipment(oPacket);

            oPacket
                .WriteInt()
                .WriteInt()
                .WriteInt();
        }

        public void EncodeData(OutPacket oPacket, long flag = long.MaxValue)
        {
            oPacket
                .WriteLong(flag)
                .WriteByte(); // NOTE: Unknown.

            this.EncodeStatistics(oPacket);

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

        public OutPacket GetCreatePacket()
        {
            throw new NotImplementedException();
        }

        public OutPacket GetSpawnPacket()
        {
            throw new NotImplementedException();
        }

        public OutPacket GetDestroyPacket()
        {
            throw new NotImplementedException();
        }
    }
}