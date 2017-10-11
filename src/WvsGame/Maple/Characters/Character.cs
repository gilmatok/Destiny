using Destiny.Maple.Maps;
using System;
using System.Collections.Generic;
using Destiny.Maple.Commands;
using Destiny.Maple.Life;
using Destiny.Data;
using Destiny.Maple.Data;
using Destiny.Maple.Interaction;
using Destiny.Network;
using Destiny.IO;
using Destiny.Maple.Scripting;

namespace Destiny.Maple.Characters
{
    public sealed class Character : MapObject, IMoveable, ISpawnable
    {
        public GameClient Client { get; private set; }

        public int ID { get; set; }
        public int AccountID { get; set; }
        public byte WorldID { get; set; }
        public string Name { get; set; }
        public bool IsInitialized { get; private set; }

        public byte SpawnPoint { get; set; }
        public byte Stance { get; set; }
        public short Foothold { get; set; }
        public byte Portals { get; set; }
        public int Chair { get; set; }
        public int? GuildRank { get; set; }

        public CharacterItems Items { get; private set; }
        public CharacterSkills Skills { get; private set; }
        public CharacterQuests Quests { get; private set; }
        public CharacterBuffs Buffs { get; private set; }
        public CharacterKeymap Keymap { get; private set; }
        public CharacterTrocks Trocks { get; private set; }
        public CharacterMemos Memos { get; private set; }
        public CharacterStorage Storage { get; private set; }
        public CharacterVariables Variables { get; private set; }
        public ControlledMobs ControlledMobs { get; private set; }
        public ControlledNpcs ControlledNpcs { get; private set; }
        public Trade Trade { get; set; }
        public PlayerShop PlayerShop { get; set; }

        private DateTime LastHealthHealOverTime = new DateTime();
        private DateTime LastManaHealOverTime = new DateTime();

        private Gender gender;
        private byte skin;
        private int face;
        private int hair;
        private byte level;
        private Job job;
        private short strength;
        private short dexterity;
        private short intelligence;
        private short luck;
        private short health;
        private short maxHealth;
        private short mana;
        private short maxMana;
        private short abilityPoints;
        private short skillPoints;
        private int experience;
        private short fame;
        private int meso;
        private Npc lastNpc;
        private Quest lastQuest;
        private string chalkboard;

        public Gender Gender
        {
            get
            {
                return gender;
            }
            set
            {
                gender = value;

                if (this.IsInitialized)
                {
                    using (Packet oPacket = new Packet(ServerOperationCode.SetGender))
                    {
                        oPacket.WriteByte((byte)this.Gender);

                        this.Client.Send(oPacket);
                    }
                }
            }
        }

        public byte Skin
        {
            get
            {
                return skin;
            }
            set
            {
                if (!DataProvider.Styles.Skins.Contains(value))
                {
                    throw new StyleUnavailableException();
                }

                skin = value;

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
                return face;
            }
            set
            {
                if ((this.Gender == Gender.Male && !DataProvider.Styles.MaleFaces.Contains(value)) ||
                    this.Gender == Gender.Female && !DataProvider.Styles.FemaleFaces.Contains(value))
                {
                    throw new StyleUnavailableException();
                }

                face = value;

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
                return hair;
            }
            set
            {
                if ((this.Gender == Gender.Male && !DataProvider.Styles.MaleHairs.Contains(value)) ||
                    this.Gender == Gender.Female && !DataProvider.Styles.FemaleHairs.Contains(value))
                {
                    throw new StyleUnavailableException();
                }

                hair = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Hair);
                    this.UpdateApperance();
                }
            }
        }

        public int HairStyleOffset
        {
            get
            {
                return (this.Hair / 10) * 10;
            }
        }

        public int FaceStyleOffset
        {
            get
            {
                return (this.Face - (10 * (this.Face / 10))) + (this.Gender == Gender.Male ? 20000 : 21000);
            }
        }

        public int HairColorOffset
        {
            get
            {
                return this.Hair - (10 * (this.Hair / 10));
            }
        }

        public int FaceColorOffset
        {
            get
            {
                return ((this.Face / 100) - (10 * (this.Face / 1000))) * 100;
            }
        }

        // TODO: Update party's properties.
        public byte Level
        {
            get
            {
                return level;
            }
            set
            {
                if (value > 200)
                {
                    throw new ArgumentException("Level cannot exceed 200.");
                }

                int delta = value - this.Level;

                if (!this.IsInitialized)
                {
                    level = value;
                }
                else
                {
                    if (delta < 0)
                    {
                        level = value;

                        this.Update(StatisticType.Level);
                    }
                    else
                    {
                        for (int i = 0; i < delta; i++)
                        {
                            // TODO: Health/Mana improvement.

                            level++;

                            if (this.IsCygnus)
                            {
                                this.AbilityPoints += 6;
                            }
                            else
                            {
                                this.AbilityPoints += 5;
                            }

                            if (this.Job != Job.Beginner && this.Job != Job.Noblesse && this.Job != Job.Legend)
                            {
                                this.SkillPoints += 3;
                            }

                            this.Update(StatisticType.Level);

                            this.ShowRemoteUserEffect(UserEffect.LevelUp);
                        }

                        this.Health = this.MaxHealth;
                        this.Mana = this.MaxMana;
                    }
                }
            }
        }

        // TODO: Update party's properties.
        public Job Job
        {
            get
            {
                return job;
            }
            set
            {
                job = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Job);

                    this.ShowRemoteUserEffect(UserEffect.JobChanged);
                }
            }
        }

        public short Strength
        {
            get
            {
                return strength;
            }
            set
            {
                strength = value;

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
                return dexterity;
            }
            set
            {
                dexterity = value;

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
                return intelligence;
            }
            set
            {
                intelligence = value;

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
                return luck;
            }
            set
            {
                luck = value;

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
                return health;
            }
            set
            {
                if (value < 0)
                {
                    health = 0;
                }
                else if (value > this.MaxHealth)
                {
                    health = this.MaxHealth;
                }
                else
                {
                    health = value;
                }

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
                return maxHealth;
            }
            set
            {
                maxHealth = value;

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
                return mana;
            }
            set
            {
                if (value < 0)
                {
                    mana = 0;
                }
                else if (value > this.MaxMana)
                {
                    mana = this.MaxMana;
                }
                else
                {
                    mana = value;
                }

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
                return maxMana;
            }
            set
            {
                maxMana = value;

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
                return abilityPoints;
            }
            set
            {
                abilityPoints = value;

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
                return skillPoints;
            }
            set
            {
                skillPoints = value;

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
                return experience;
            }
            set
            {
                int delta = value - experience;

                experience = value;

                if (true) // NOTE: A server setting for multi-leveling.
                {
                    while (experience >= ExperienceTables.CharacterLevel[this.Level])
                    {
                        experience -= ExperienceTables.CharacterLevel[this.Level];

                        this.Level++;
                    }
                }
                else
                {
                    if (experience >= ExperienceTables.CharacterLevel[this.Level])
                    {
                        experience -= ExperienceTables.CharacterLevel[this.Level];

                        this.Level++;
                    }

                    if (experience >= ExperienceTables.CharacterLevel[this.Level])
                    {
                        experience = ExperienceTables.CharacterLevel[this.Level] - 1;
                    }
                }

                if (this.IsInitialized && delta != 0)
                {
                    this.Update(StatisticType.Experience);
                }
            }
        }

        public short Fame
        {
            get
            {
                return fame;
            }
            set
            {
                fame = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Fame);
                }
            }
        }

        public int Meso
        {
            get
            {
                return meso;
            }
            set
            {
                meso = value;

                if (this.IsInitialized)
                {
                    this.Update(StatisticType.Mesos);
                }
            }
        }

        public bool IsAlive
        {
            get
            {
                return this.Health > 0;
            }
        }

        public bool IsMaster
        {
            get
            {
                //TODO: Add GM levels and/or character-specific GM rank
                return Client.Account != null ? Client.Account.IsMaster : false;
            }
        }

        // TODO: Improve this check.
        public bool IsCygnus
        {
            get
            {
                return (short)this.Job >= 1000 && (short)this.Job <= 2000;
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

        public Npc LastNpc
        {
            get
            {
                return lastNpc;
            }
            set
            {
                if (value == null)
                {
                    if (value.Scripts.ContainsKey(this))
                    {
                        value.Scripts.Remove(this);
                    }
                }

                lastNpc = value;
            }
        }

        public Quest LastQuest
        {
            get
            {
                return lastQuest;
            }
            set
            {
                lastQuest = value;

                // TODO: Add checks.
            }
        }

        public string Chalkboard
        {
            get
            {
                return chalkboard;
            }
            set
            {
                chalkboard = value;

                using (Packet oPacket = new Packet(ServerOperationCode.Chalkboard))
                {
                    oPacket
                        .WriteInt(this.ID)
                        .WriteBool(!string.IsNullOrEmpty(this.Chalkboard))
                        .WriteString(this.Chalkboard);

                    this.Map.Broadcast(oPacket);
                }
            }
        }

        public Portal ClosestPortal
        {
            get
            {
                Portal closestPortal = null;
                double shortestDistance = double.PositiveInfinity;

                foreach (Portal loopPortal in this.Map.Portals)
                {
                    double distance = loopPortal.Position.DistanceFrom(this.Position);

                    if (distance < shortestDistance)
                    {
                        closestPortal = loopPortal;
                        shortestDistance = distance;
                    }
                }

                return closestPortal;
            }
        }

        public Portal ClosestSpawnPoint
        {
            get
            {
                Portal closestPortal = null;
                double shortestDistance = double.PositiveInfinity;

                foreach (Portal loopPortal in this.Map.Portals)
                {
                    if (loopPortal.IsSpawnPoint)
                    {
                        double distance = loopPortal.Position.DistanceFrom(this.Position);

                        if (distance < shortestDistance)
                        {
                            closestPortal = loopPortal;
                            shortestDistance = distance;
                        }
                    }
                }

                return closestPortal;
            }
        }

        private bool Assigned { get; set; }

        public Character(int id = 0, GameClient client = null)
        {
            this.ID = id;
            this.Client = client;

            this.Items = new CharacterItems(this, 24, 24, 24, 24, 48);
            this.Skills = new CharacterSkills(this);
            this.Quests = new CharacterQuests(this);
            this.Buffs = new CharacterBuffs(this);
            this.Keymap = new CharacterKeymap(this);
            this.Trocks = new CharacterTrocks(this);
            this.Memos = new CharacterMemos(this);
            this.Storage = new CharacterStorage(this);
            this.Variables = new CharacterVariables(this);

            this.Position = new Point(0, 0);
            this.ControlledMobs = new ControlledMobs(this);
            this.ControlledNpcs = new ControlledNpcs(this);
        }

        public void Load()
        {
            Datum datum = new Datum("characters");

            datum.Populate("ID = {0}", this.ID);

            this.ID = (int)datum["ID"];
            this.Assigned = true;

            this.AccountID = (int)datum["AccountID"];
            this.WorldID = (byte)datum["WorldID"];
            this.Name = (string)datum["Name"];
            this.Gender = (Gender)datum["Gender"];
            this.Skin = (byte)datum["Skin"];
            this.Face = (int)datum["Face"];
            this.Hair = (int)datum["Hair"];
            this.Level = (byte)datum["Level"];
            this.Job = (Job)datum["Job"];
            this.Strength = (short)datum["Strength"];
            this.Dexterity = (short)datum["Dexterity"];
            this.Intelligence = (short)datum["Intelligence"];
            this.Luck = (short)datum["Luck"];
            this.MaxHealth = (short)datum["MaxHealth"];
            this.MaxMana = (short)datum["MaxMana"];
            this.Health = (short)datum["Health"];
            this.Mana = (short)datum["Mana"];
            this.AbilityPoints = (short)datum["AbilityPoints"];
            this.SkillPoints = (short)datum["SkillPoints"];
            this.Experience = (int)datum["Experience"];
            this.Fame = (short)datum["Fame"];
            this.Map = DataProvider.Maps[(int)datum["MapID"]];
            this.SpawnPoint = (byte)datum["SpawnPoint"];
            this.Meso = (int)datum["Meso"];

            this.Items.MaxSlots[ItemType.Equipment] = (byte)datum["EquipmentSlots"];
            this.Items.MaxSlots[ItemType.Usable] = (byte)datum["UsableSlots"];
            this.Items.MaxSlots[ItemType.Setup] = (byte)datum["SetupSlots"];
            this.Items.MaxSlots[ItemType.Etcetera] = (byte)datum["EtceteraSlots"];
            this.Items.MaxSlots[ItemType.Cash] = (byte)datum["CashSlots"];

            this.Items.Load();
            this.Skills.Load();
            this.Quests.Load();
            this.Buffs.Load();
            this.Keymap.Load();
            this.Trocks.Load();
            this.Memos.Load();
            this.Variables.Load();
        }

        public void Save()
        {
            if (this.IsInitialized)
            {
                this.SpawnPoint = this.ClosestSpawnPoint.ID;
            }

            Datum datum = new Datum("characters");

            datum["AccountID"] = this.AccountID;
            datum["WorldID"] = this.WorldID;
            datum["Name"] = this.Name;
            datum["Gender"] = (byte)this.Gender;
            datum["Skin"] = this.Skin;
            datum["Face"] = this.Face;
            datum["Hair"] = this.Hair;
            datum["Level"] = this.Level;
            datum["Job"] = (short)this.Job;
            datum["Strength"] = this.Strength;
            datum["Dexterity"] = this.Dexterity;
            datum["Intelligence"] = this.Intelligence;
            datum["Luck"] = this.Luck;
            datum["Health"] = this.Health;
            datum["MaxHealth"] = this.MaxHealth;
            datum["Mana"] = this.Mana;
            datum["MaxMana"] = this.MaxMana;
            datum["AbilityPoints"] = this.AbilityPoints;
            datum["SkillPoints"] = this.SkillPoints;
            datum["Experience"] = this.Experience;
            datum["Fame"] = this.Fame;
            datum["MapID"] = this.Map.MapleID;
            datum["SpawnPoint"] = this.SpawnPoint;
            datum["Meso"] = this.Meso;

            datum["EquipmentSlots"] = this.Items.MaxSlots[ItemType.Equipment];
            datum["UsableSlots"] = this.Items.MaxSlots[ItemType.Usable];
            datum["SetupSlots"] = this.Items.MaxSlots[ItemType.Setup];
            datum["EtceteraSlots"] = this.Items.MaxSlots[ItemType.Etcetera];
            datum["CashSlots"] = this.Items.MaxSlots[ItemType.Cash];

            if (this.Assigned)
            {
                datum.Update("ID = {0}", this.ID);
            }
            else
            {
                this.ID = datum.InsertAndReturnID();
                this.Assigned = true;
            }

            this.Items.Save();
            this.Skills.Save();
            this.Quests.Save();
            this.Buffs.Save();
            this.Keymap.Save();
            this.Trocks.Save();
            this.Variables.Save();

            Log.Inform("Saved character '{0}' to database.", this.Name);
        }

        public void Initialize()
        {
            using (Packet oPacket = new Packet(ServerOperationCode.SetField))
            {
                oPacket
                    .WriteInt(WvsGame.ChannelID)
                    .WriteByte(++this.Portals)
                    .WriteBool(true)
                    .WriteShort(); // NOTE: Floating messages at top corner.

                for (int i = 0; i < 3; i++)
                {
                    oPacket.WriteInt(Application.Random.Next());
                }

                oPacket
                    .WriteBytes(this.DataToByteArray())
                    .WriteDateTime(DateTime.UtcNow);

                this.Client.Send(oPacket);
            }

            using (Packet oPacket = new Packet(ServerOperationCode.ClaimSvrStatusChanged))
            {
                oPacket.WriteBool(true);

                this.Client.Send(oPacket);
            }

            this.IsInitialized = true;

            this.Map.Characters.Add(this);

            this.Keymap.Send();

            this.Memos.Send();
        }

        public void Update(params StatisticType[] statistics)
        {
            using (Packet oPacket = new Packet(ServerOperationCode.StatChanged))
            {
                oPacket.WriteBool(true); // TODO: bOnExclRequest.

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
                            oPacket.WriteInt(this.Meso);
                            break;
                    }
                }

                this.Client.Send(oPacket);
            }
        }

        public void UpdateApperance()
        {
            using (Packet oPacket = new Packet(ServerOperationCode.AvatarModified))
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteBool(true)
                    .WriteBytes(this.AppearanceToByteArray())
                    .WriteByte()
                    .WriteShort();

                this.Map.Broadcast(oPacket, this);
            }
        }

        public void Release()
        {
            this.Update();
        }

        public void Notify(string message, NoticeType type = NoticeType.Pink)
        {
            using (Packet oPacket = new Packet(ServerOperationCode.BroadcastMsg))
            {
                oPacket.WriteByte((byte)type);

                if (type == NoticeType.Ticker)
                {
                    oPacket.WriteBool(!string.IsNullOrEmpty(message));
                }

                oPacket.WriteString(message);

                this.Client.Send(oPacket);
            }
        }

        public void ChangeMap(Packet iPacket)
        {
            byte portals = iPacket.ReadByte();

            if (portals != this.Portals)
            {
                return;
            }

            int mapID = iPacket.ReadInt();
            string portalLabel = iPacket.ReadString();
            iPacket.ReadByte(); // NOTE: Unknown.
            bool wheel = iPacket.ReadBool();

            switch (mapID)
            {
                case 0: // NOTE: Death.
                    {
                        if (this.IsAlive)
                        {
                            return;
                        }

                        this.Health = 50;
                        this.ChangeMap(this.Map.ReturnMapID);
                    }
                    break;

                case -1: // NOTE: Portal.
                    {
                        Portal portal;

                        try
                        {
                            portal = this.Map.Portals[portalLabel];
                        }
                        catch (KeyNotFoundException)
                        {
                            return;
                        }

                        // TODO: Validate player and portal position.

                        /*if (this.Level < this.Client.Channel.Maps[portal.DestinationMapID].RequiredLevel)
                        {
                            // TODO: Send a force of ground portal message.

                            return;
                        }*/

                        this.ChangeMap(portal.DestinationMapID, portal.Link.ID);
                    }
                    break;

                default: // NOTE: Admin '/m' command.
                    {
                        if (!this.IsMaster)
                        {
                            return;
                        }

                        // TODO: Validate map ID.

                        this.ChangeMap(mapID);
                    }
                    break;
            }
        }

        public void ChangeMap(int mapID, string portalLabel)
        {
            this.ChangeMap(mapID, DataProvider.Maps[mapID].Portals[portalLabel].ID);
        }

        public void ChangeMap(int mapID, byte portalID = 0, bool fromPosition = false, Point position = null)
        {
            this.Map.Characters.Remove(this);

            using (Packet oPacket = new Packet(ServerOperationCode.SetField))
            {
                oPacket
                    .WriteInt(WvsGame.ChannelID)
                    .WriteByte(++this.Portals)
                    .WriteBool(false)
                    .WriteShort()
                    .WriteByte()
                    .WriteInt(mapID)
                    .WriteByte(this.SpawnPoint)
                    .WriteShort(this.Health)
                    .WriteBool(fromPosition);

                if (fromPosition)
                {
                    oPacket
                        .WriteShort(position.X)
                        .WriteShort(position.Y);
                }

                oPacket.WriteDateTime(DateTime.Now);

                this.Client.Send(oPacket);
            }

            DataProvider.Maps[mapID].Characters.Add(this);
        }

        public void AddAbility(StatisticType statistic, short mod, bool isReset)
        {
            short maxStat = short.MaxValue; // TODO: Should this be a setting?
            bool isSubtract = mod < 0;

            lock (this)
            {
                switch (statistic)
                {
                    case StatisticType.Strength:
                        if (this.Strength >= maxStat)
                        {
                            return;
                        }

                        this.Strength += mod;
                        break;

                    case StatisticType.Dexterity:
                        if (this.Dexterity >= maxStat)
                        {
                            return;
                        }

                        this.Dexterity += mod;
                        break;

                    case StatisticType.Intelligence:
                        if (this.Intelligence >= maxStat)
                        {
                            return;
                        }

                        this.Intelligence += mod;
                        break;

                    case StatisticType.Luck:
                        if (this.Luck >= maxStat)
                        {
                            return;
                        }

                        this.Luck += mod;
                        break;

                    case StatisticType.MaxHealth:
                    case StatisticType.MaxMana:
                        {
                            // TODO: This is way too complicated for now.
                        }
                        break;
                }

                if (!isReset)
                {
                    this.AbilityPoints -= mod;
                }

                // TODO: Update bonuses.
            }
        }

        public void Move(Packet iPacket)
        {
            byte portals = iPacket.ReadByte();

            if (portals != this.Portals)
            {
                return;
            }

            iPacket.ReadInt(); // NOE: Unknown.

            Movements movements = Movements.Decode(iPacket);

            this.Position = movements.Position;
            this.Foothold = movements.Foothold;
            this.Stance = movements.Stance;

            using (Packet oPacket = new Packet(ServerOperationCode.Move))
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteBytes(movements.ToByteArray());

                this.Map.Broadcast(oPacket, this);
            }

            if (this.Foothold == 0)
            {
                // NOTE: Player is floating in the air.
                // GMs might be legitmately in this state due to GM fly.
                // We shouldn't mess with them because they have the tools toget out of falling off the map anyway.

                // TODO: Attempt to find foothold.
                // If none found, check the player fall counter.
                // If it's over 3, reset the player's map.
            }
        }

        public void Sit(Packet iPacket)
        {
            short seatID = iPacket.ReadShort();

            if (seatID == -1)
            {
                this.Chair = 0;

                using (Packet oPacket = new Packet(ServerOperationCode.SetActiveRemoteChair))
                {
                    oPacket
                        .WriteInt(this.ID)
                        .WriteInt();

                    this.Map.Broadcast(oPacket, this);
                }
            }
            else
            {
                this.Chair = seatID;
            }

            using (Packet oPacket = new Packet(ServerOperationCode.Sit))
            {
                oPacket.WriteBool(seatID != -1);

                if (seatID != -1)
                {
                    oPacket.WriteShort(seatID);
                }

                this.Client.Send(oPacket);
            }
        }

        public void SitChair(Packet iPacket)
        {
            int mapleID = iPacket.ReadInt();

            if (!this.Items.Contains(mapleID))
            {
                return;
            }

            this.Chair = mapleID;

            using (Packet oPacket = new Packet(ServerOperationCode.SetActiveRemoteChair))
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteInt(mapleID);

                this.Map.Broadcast(oPacket, this);
            }
        }

        public void Attack(Packet iPacket, AttackType type)
        {
            Attack attack = new Attack(iPacket, type);

            if (attack.Portals != this.Portals)
            {
                return;
            }

            Skill skill = null;

            if (attack.SkillID > 0)
            {
                skill = this.Skills[attack.SkillID];

                skill.Cast();
            }

            // TODO: Modify packet based on attack type.
            using (Packet oPacket = new Packet(ServerOperationCode.CloseRangeAttack))
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteByte((byte)((attack.Targets * 0x10) + attack.Hits))
                    .WriteByte() // NOTE: Unknown.
                    .WriteByte((byte)(attack.SkillID != 0 ? skill.CurrentLevel : 0)); // NOTE: Skill level.

                if (attack.SkillID != 0)
                {
                    oPacket.WriteInt(attack.SkillID);
                }

                oPacket
                    .WriteByte() // NOTE: Unknown.
                    .WriteByte(attack.Display)
                    .WriteByte(attack.Animation)
                    .WriteByte(attack.WeaponSpeed)
                    .WriteByte() // NOTE: Skill mastery.
                    .WriteInt(); // NOTE: Unknown.

                foreach (var target in attack.Damages)
                {
                    oPacket
                        .WriteInt(target.Key)
                        .WriteByte(6);

                    foreach (uint hit in target.Value)
                    {
                        oPacket.WriteUInt(hit);
                    }
                }

                this.Map.Broadcast(oPacket, this);
            }

            foreach (KeyValuePair<int, List<uint>> target in attack.Damages)
            {
                Mob mob;

                try
                {
                    mob = this.Map.Mobs[target.Key];
                }
                catch (KeyNotFoundException)
                {
                    continue;
                }

                mob.IsProvoked = true;
                mob.SwitchController(this);

                foreach (uint hit in target.Value)
                {
                    if (mob.Damage(this, hit))
                    {
                        mob.Die();
                    }
                }
            }
        }

        private const sbyte BumpDamage = -1;
        private const sbyte MapDamage = -2;

        public void Damage(Packet iPacket)
        {
            iPacket.Skip(4); // NOTE: Ticks.
            sbyte type = (sbyte)iPacket.ReadByte();
            iPacket.ReadByte(); // NOTE: Elemental type.
            int damage = iPacket.ReadInt();
            bool damageApplied = false;
            bool deadlyAttack = false;
            byte hit = 0;
            byte stance = 0;
            int disease = 0;
            byte level = 0;
            short mpBurn = 0;
            int mobObjectID = 0;
            int mobID = 0;
            int noDamageSkillID = 0;

            if (type != MapDamage)
            {
                mobID = iPacket.ReadInt();
                mobObjectID = iPacket.ReadInt();

                Mob mob;

                try
                {
                    mob = this.Map.Mobs[mobObjectID];
                }
                catch (KeyNotFoundException)
                {
                    return;
                }

                if (mobID != mob.MapleID)
                {
                    return;
                }

                if (type != BumpDamage)
                {
                    // TODO: Get mob attack and apply to disease/level/mpBurn/deadlyAttack.
                }
            }

            hit = iPacket.ReadByte();
            byte reduction = iPacket.ReadByte();
            iPacket.ReadByte(); // NOTE: Unknown.

            if (reduction != 0)
            {
                // TODO: Return damage (Power Guard).
            }

            if (type == MapDamage)
            {
                level = iPacket.ReadByte();
                disease = iPacket.ReadInt();
            }
            else
            {
                stance = iPacket.ReadByte();

                if (stance > 0)
                {
                    // TODO: Power Stance.
                }
            }

            if (damage == -1)
            {
                // TODO: Validate no damage skills.
            }

            if (disease > 0 && damage != 0)
            {
                // NOTE: Fake/Guardian don't prevent disease.
                // TODO: Add disease buff.
            }

            if (damage > 0)
            {
                // TODO: Check for Meso Guard.
                // TODO: Check for Magic Guard.
                // TODO: Check for Achilles.

                if (!damageApplied)
                {
                    if (deadlyAttack)
                    {
                        // TODO: Deadly attack function.
                    }
                    else
                    {
                        this.Health -= (short)damage;
                    }

                    if (mpBurn > 0)
                    {
                        this.Mana -= (short)mpBurn;
                    }
                }

                // TODO: Apply damage to buffs.
            }

            using (Packet oPacket = new Packet(ServerOperationCode.Hit))
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteSByte(type);

                switch (type)
                {
                    case MapDamage:
                        {
                            oPacket
                                .WriteInt(damage)
                                .WriteInt(damage);
                        }
                        break;

                    default:
                        {
                            oPacket
                                .WriteInt(damage) // TODO: ... or PGMR damage.
                                .WriteInt(mobID)
                                .WriteByte(hit)
                                .WriteByte(reduction);

                            if (reduction > 0)
                            {
                                // TODO: PGMR stuff.
                            }

                            oPacket
                                .WriteByte(stance)
                                .WriteInt(damage);

                            if (noDamageSkillID > 0)
                            {
                                oPacket.WriteInt(noDamageSkillID);
                            }
                        }
                        break;
                }

                this.Map.Broadcast(oPacket, this);
            }
        }

        public void Talk(Packet iPacket)
        {
            string text = iPacket.ReadString();
            bool shout = iPacket.ReadBool(); // NOTE: Used for skill macros.

            if (text.StartsWith(Application.CommandIndiciator.ToString()))
            {
                CommandFactory.Execute(this, text);
            }
            else
            {
                using (Packet oPacket = new Packet(ServerOperationCode.UserChat))
                {
                    oPacket
                        .WriteInt(this.ID)
                        .WriteBool(this.IsMaster)
                        .WriteString(text)
                        .WriteBool(shout);

                    this.Map.Broadcast(oPacket);
                }
            }
        }

        public void Express(Packet iPacket)
        {
            int expressionID = iPacket.ReadInt();

            if (expressionID > 7) // NOTE: Cash facial expression.
            {
                int mapleID = 5159992 + expressionID;

                // TODO: Validate if item exists.
            }

            using (Packet oPacket = new Packet(ServerOperationCode.Emotion))
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteInt(expressionID);

                this.Map.Broadcast(oPacket, this);
            }
        }

        public void ShowLocalUserEffect(UserEffect effect)
        {
            using (Packet oPacket = new Packet(ServerOperationCode.Effect))
            {
                oPacket.WriteByte((byte)effect);

                this.Client.Send(oPacket);
            }
        }

        public void ShowRemoteUserEffect(UserEffect effect, bool skipSelf = false)
        {
            using (Packet oPacket = new Packet(ServerOperationCode.RemoteEffect))
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteByte((byte)effect);

                this.Map.Broadcast(oPacket, skipSelf ? this : null);
            }
        }

        public void Converse(int mapleID)
        {
            // TODO.
        }

        public void Converse(Packet iPacket)
        {
            int objectID = iPacket.ReadInt();

            this.Converse(this.Map.Npcs[objectID]);
        }

        public void Converse(Npc npc, Quest quest = null)
        {
            this.LastNpc = npc;
            this.LastQuest = quest;

            this.LastNpc.Converse(this);
        }

        public void DistributeAP(StatisticType type, short amount = 1)
        {
            switch (type)
            {
                case StatisticType.Strength:
                    this.Strength += amount;
                    break;

                case StatisticType.Dexterity:
                    this.Dexterity += amount;
                    break;

                case StatisticType.Intelligence:
                    this.Intelligence += amount;
                    break;

                case StatisticType.Luck:
                    this.Luck += amount;
                    break;

                case StatisticType.MaxHealth:
                    // TODO: Get addition based on other factors.
                    break;

                case StatisticType.MaxMana:
                    // TODO: Get addition based on other factors.
                    break;
            }
        }

        public void DistributeAP(Packet iPacket)
        {
            if (this.AbilityPoints == 0)
            {
                return;
            }

            iPacket.ReadInt(); // NOTE: Ticks.
            StatisticType type = (StatisticType)iPacket.ReadInt();

            this.DistributeAP(type);
            this.AbilityPoints--;
        }

        public void AutoDistributeAP(Packet iPacket)
        {
            iPacket.ReadInt(); // NOTE: Ticks.
            int count = iPacket.ReadInt(); // NOTE: There are always 2 primary stats for each job, but still.

            int total = 0;

            for (int i = 0; i < count; i++)
            {
                StatisticType type = (StatisticType)iPacket.ReadInt();
                int amount = iPacket.ReadInt();

                if (amount > this.AbilityPoints || amount < 0)
                {
                    return;
                }

                this.DistributeAP(type, (short)amount);

                total += amount;
            }

            this.AbilityPoints -= (short)total;
        }

        public void HealOverTime(Packet iPacket)
        {
            iPacket.ReadInt(); // NOTE: Ticks.
            iPacket.ReadInt(); // NOTE: Unknown.
            short healthAmount = iPacket.ReadShort(); // TODO: Validate
            short manaAmount = iPacket.ReadShort(); // TODO: Validate

            if (healthAmount != 0)
            {
                if ((DateTime.Now - this.LastHealthHealOverTime).TotalSeconds < 2)
                {
                    return;
                }
                else
                {
                    this.Health += healthAmount;
                    this.LastHealthHealOverTime = DateTime.Now;
                }
            }

            if (manaAmount != 0)
            {
                if ((DateTime.Now - this.LastManaHealOverTime).TotalSeconds < 2)
                {
                    return;
                }
                else
                {
                    this.Mana += manaAmount;
                    this.LastManaHealOverTime = DateTime.Now;
                }
            }
        }

        public void DistributeSP(Packet iPacket)
        {
            if (this.SkillPoints == 0)
            {
                return;
            }

            iPacket.ReadInt(); // NOTE: Ticks.
            int mapleID = iPacket.ReadInt();

            if (!this.Skills.Contains(mapleID))
            {
                this.Skills.Add(new Skill(mapleID));
            }

            Skill skill = this.Skills[mapleID];

            // TODO: Check for skill requirements.

            if (skill.IsFromBeginner)
            {
                // TODO: Handle beginner skills.
            }

            if (skill.CurrentLevel + 1 <= skill.MaxLevel)
            {
                if (!skill.IsFromBeginner)
                {
                    this.SkillPoints--;
                }

                this.Release();

                skill.CurrentLevel++;
            }
        }

        public void DropMeso(Packet iPacket)
        {
            iPacket.Skip(4); // NOTE: tRequestTime (ticks).
            int amount = iPacket.ReadInt();

            if (amount > this.Meso || amount < 10 || amount > 50000)
            {
                return;
            }

            this.Meso -= amount;

            Meso meso = new Meso(amount)
            {
                Dropper = this,
                Owner = null
            };

            this.Map.Drops.Add(meso);
        }

        public void InformOnCharacter(Packet iPacket)
        {
            iPacket.Skip(4);
            int characterID = iPacket.ReadInt();

            Character target;

            try
            {
                target = this.Map.Characters[characterID];
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            if (target.IsMaster && !this.IsMaster)
            {
                return;
            }

            using (Packet oPacket = new Packet(ServerOperationCode.CharacterInformation))
            {
                oPacket
                    .WriteInt(target.ID)
                    .WriteByte(target.Level)
                    .WriteShort((short)target.Job)
                    .WriteShort(target.Fame)
                    .WriteBool(false) // NOTE: Marriage.
                    .WriteString("-") // NOTE: Guild name.
                    .WriteString("-") // NOTE: Alliance name.
                    .WriteByte() // NOTE: Unknown.
                    .WriteByte() // NOTE: Pets.
                    .WriteByte() // NOTE: Mount.
                    .WriteByte() // NOTE: Wishlist.
                    .WriteInt() // NOTE: Monster Book level.
                    .WriteInt() // NOTE: Monster Book normal cards. 
                    .WriteInt() // NOTE: Monster Book special cards.
                    .WriteInt() // NOTE: Monster Book total cards.
                    .WriteInt() // NOTE: Monster Book cover.
                    .WriteInt() // NOTE: Medal ID.
                    .WriteShort(); // NOTE: Medal quests.

                this.Client.Send(oPacket);
            }
        }

        // TODO: Should we refactor it in a way that sends it to the buddy/party/guild objects
        // instead of pooling the world for characters?
        public void MultiTalk(Packet iPacket)
        {
            MultiChatType type = (MultiChatType)iPacket.ReadByte();
            byte count = iPacket.ReadByte();

            List<int> recipients = new List<int>();

            while (count-- > 0)
            {
                int recipientID = iPacket.ReadInt();

                recipients.Add(recipientID);
            }

            string text = iPacket.ReadString();

            switch (type)
            {
                case MultiChatType.Buddy:
                    {

                    }
                    break;

                case MultiChatType.Party:
                    {

                    }
                    break;

                case MultiChatType.Guild:
                    {

                    }
                    break;

                case MultiChatType.Alliance:
                    {

                    }
                    break;
            }

            // NOTE: This is here for convinience. If you accidently use another text window (like party) and not the main text window,
            // your commands won't be shown but instead executed from there as well.
            if (text.StartsWith(Application.CommandIndiciator.ToString()))
            {
                CommandFactory.Execute(this, text);
            }
            else
            {
                using (Packet oPacket = new Packet(ServerOperationCode.GroupMessage))
                {
                    oPacket
                        .WriteByte((byte)type)
                        .WriteString(this.Name)
                        .WriteString(text);

                    foreach (int recipient in recipients)
                    {
                        //this.Client.World.GetCharacter(recipient).Client.Send(oPacket);
                    }
                }
            }
        }

        // TODO: Cash Shop/MTS scenarios.
        public void UseCommand(Packet iPacket)
        {
            /*CommandType type = (CommandType)iPacket.ReadByte();
            string targetName = iPacket.ReadString();

            Character target = null;// this.Client.World.GetCharacter(targetName);

            switch (type)
            {
                case CommandType.Find:
                    {
                        if (target == null)
                        {
                            using (Packet oPacket = new Packet(ServerOperationCode.Whisper))
                            {
                                oPacket
                                    .WriteByte(0x0A)
                                    .WriteString(targetName)
                                    .WriteBool(false);

                                this.Client.Send(oPacket);
                            }
                        }
                        else
                        {
                            bool isInSameChannel = this.Client.ChannelID == target.Client.ChannelID;

                            using (Packet oPacket = new Packet(ServerOperationCode.Whisper))
                            {
                                oPacket
                                    .WriteByte(0x09)
                                    .WriteString(targetName)
                                    .WriteByte((byte)(isInSameChannel ? 1 : 3))
                                    .WriteInt(isInSameChannel ? target.Map.MapleID : target.Client.ChannelID)
                                    .WriteInt() // NOTE: Unknown.
                                    .WriteInt(); // NOTE: Unknown.

                                this.Client.Send(oPacket);
                            }
                        }
                    }
                    break;

                case CommandType.Whisper:
                    {
                        string text = iPacket.ReadString();

                        using (Packet oPacket = new Packet(ServerOperationCode.Whisper))
                        {
                            oPacket
                                .WriteByte(10)
                                .WriteString(targetName)
                                .WriteBool(target != null);

                            this.Client.Send(oPacket);
                        }

                        if (target != null)
                        {
                            using (Packet oPacket = new Packet(ServerOperationCode.Whisper))
                            {
                                oPacket
                                    .WriteByte(18)
                                    .WriteString(this.Name)
                                    .WriteByte(this.Client.ChannelID)
                                    .WriteByte() // NOTE: Unknown.
                                    .WriteString(text);

                                target.Client.Send(oPacket);
                            }
                        }
                    }
                    break;
            }*/
        }

        public void Interact(Packet iPacket)
        {
            InteractionCode code = (InteractionCode)iPacket.ReadByte();

            switch (code)
            {
                case InteractionCode.Create:
                    {
                        InteractionType type = (InteractionType)iPacket.ReadByte();

                        switch (type)
                        {
                            case InteractionType.Omok:
                                {

                                }
                                break;

                            case InteractionType.Trade:
                                {
                                    if (this.Trade == null)
                                    {
                                        this.Trade = new Trade(this);
                                    }
                                }
                                break;

                            case InteractionType.PlayerShop:
                                {
                                    string description = iPacket.ReadString();

                                    if (this.PlayerShop == null)
                                    {
                                        this.PlayerShop = new PlayerShop(this, description);
                                    }
                                }
                                break;

                            case InteractionType.HiredMerchant:
                                {

                                }
                                break;
                        }
                    }
                    break;

                case InteractionCode.Visit:
                    {
                        if (this.PlayerShop == null)
                        {
                            int objectID = iPacket.ReadInt();

                            if (this.Map.PlayerShops.Contains(objectID))
                            {
                                this.Map.PlayerShops[objectID].AddVisitor(this);
                            }
                        }
                    }
                    break;

                default:
                    {
                        if (this.Trade != null)
                        {
                            this.Trade.Handle(this, code, iPacket);
                        }
                        else if (this.PlayerShop != null)
                        {
                            this.PlayerShop.Handle(this, code, iPacket);
                        }
                    }
                    break;
            }
        }

        public void UseAdminCommand(Packet iPacket)
        {
            if (!this.IsMaster)
            {
                return;
            }

            AdminCommandType type = (AdminCommandType)iPacket.ReadByte();

            switch (type)
            {
                case AdminCommandType.Hide:
                    {
                        bool hide = iPacket.ReadBool();

                        if (hide)
                        {
                            // TODO: Add SuperGM's hide buff.
                        }
                        else
                        {
                            // TOOD: Remove SuperGM's hide buff.
                        }
                    }
                    break;

                case AdminCommandType.Send:
                    {
                        string name = iPacket.ReadString();
                        int destinationID = iPacket.ReadInt();

                        Character target = null;// this.Client.World.GetCharacter(name);

                        if (target != null)
                        {
                            target.ChangeMap(destinationID);
                        }
                        else
                        {
                            using (Packet oPacket = new Packet(ServerOperationCode.AdminResult))
                            {
                                oPacket
                                    .WriteByte(6)
                                    .WriteByte(1);

                                this.Client.Send(oPacket);
                            }
                        }
                    }
                    break;

                case AdminCommandType.Summon:
                    {
                        int mobID = iPacket.ReadInt();
                        int count = iPacket.ReadInt();

                        if (DataProvider.Mobs.Contains(mobID))
                        {
                            for (int i = 0; i < count; i++)
                            {
                                this.Map.Mobs.Add(new Mob(mobID, this.Position));
                            }
                        }
                        else
                        {
                            this.Notify("invalid mob: " + mobID); // TODO: Actual message.
                        }
                    }
                    break;

                case AdminCommandType.CreateItem:
                    {
                        int itemID = iPacket.ReadInt();

                        this.Items.Add(new Item(itemID));
                    }
                    break;

                case AdminCommandType.DestroyFirstITem:
                    {
                        // TODO: What does this do?
                    }
                    break;

                case AdminCommandType.GiveExperience:
                    {
                        int amount = iPacket.ReadInt();

                        this.Experience += amount;
                    }
                    break;

                case AdminCommandType.Ban:
                    {
                        string name = iPacket.ReadString();

                        Character target = null;//this.Client.World.GetCharacter(name);

                        if (target != null)
                        {
                            target.Client.Stop();
                        }
                        else
                        {
                            using (Packet oPacket = new Packet(ServerOperationCode.AdminResult))
                            {
                                oPacket
                                    .WriteByte(6)
                                    .WriteByte(1);

                                this.Client.Send(oPacket);
                            }
                        }
                    }
                    break;

                case AdminCommandType.Block:
                    {
                        // TODO: Ban.
                    }
                    break;

                case AdminCommandType.ShowMessageMap:
                    {
                        // TODO: What does this do?
                    }
                    break;

                case AdminCommandType.Snow:
                    {
                        // TODO: We have yet to implement map weather.
                    }
                    break;

                case AdminCommandType.VarSetGet:
                    {
                        // TODO: This seems useless. Should we implement this?
                    }
                    break;

                case AdminCommandType.Warn:
                    {
                        string name = iPacket.ReadString();
                        string text = iPacket.ReadString();

                        Character target = null;// this.Client.World.GetCharacter(name);

                        if (target != null)
                        {
                            target.Notify(text, NoticeType.Popup);
                        }

                        using (Packet oPacket = new Packet(ServerOperationCode.AdminResult))
                        {
                            oPacket
                                .WriteByte(29)
                                .WriteBool(target != null);

                            this.Client.Send(oPacket);
                        }
                    }
                    break;
            }
        }

        public void EnterPortal(Packet iPacket)
        {
            byte portals = iPacket.ReadByte();

            if (portals != this.Portals)
            {
                return;
            }

            string label = iPacket.ReadString();

            Portal portal;

            try
            {
                portal = this.Map.Portals[label];
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            if (false) // TODO: Check if portal is onlyOnce and player already used it.
            {
                // TODO: Send a "closed for now" portal message.

                return;
            }

            try
            {
                new PortalScript(portal, this).Execute();
            }
            catch (Exception ex)
            {
                Log.Error("Script error: {0}", ex.ToString());
            }
        }

        public void Report(Packet iPacket)
        {
            ReportType type = (ReportType)iPacket.ReadByte();
            string victimName = iPacket.ReadString();
            iPacket.ReadByte(); // NOTE: Unknown.
            string description = iPacket.ReadString();

            ReportResult result;

            switch (type)
            {
                case ReportType.IllegalProgramUsage:
                    {
                    }
                    break;

                case ReportType.ConversationClaim:
                    {
                        string chatLog = iPacket.ReadString();
                    }
                    break;
            }

            if (true) // TODO: Check for available report claims.
            {
                /*if (this.Client.World.IsCharacterOnline(victimName)) // TODO: Should we check for map existance instead? The hacker can teleport away before the reported is executed.
                {
                    if (this.Meso >= 300)
                    {
                        this.Meso -= 300;

                        // TODO: Update GMs of reported player.
                        // TODO: Update available report claims.

                        result = ReportResult.Success;
                    }
                    else
                    {
                        result = ReportResult.UnknownError;
                    }
                }
                else
                {
                    result = ReportResult.UnableToLocate;
                }*/
                result = ReportResult.Success;
            }
            else
            {
                result = ReportResult.Max10TimesADay;
            }

            using (Packet oPacket = new Packet(ServerOperationCode.SueCharacterResult))
            {
                oPacket.WriteByte((byte)result);

                this.Client.Send(oPacket);
            }
        }

        public byte[] ToByteArray(bool viewAllCharacters = false)
        {
            using (ByteBuffer oPacket = new ByteBuffer())
            {
                oPacket
                    .WriteBytes(this.StatisticsToByteArray())
                    .WriteBytes(this.AppearanceToByteArray());

                if (!viewAllCharacters)
                {
                    oPacket.WriteByte(); // NOTE: Family
                }

                oPacket.WriteBool(this.IsRanked);

                if (this.IsRanked)
                {
                    oPacket
                        .WriteInt()
                        .WriteInt()
                        .WriteInt()
                        .WriteInt();
                }

                oPacket.Flip();
                return oPacket.GetContent();
            }
        }

        public byte[] StatisticsToByteArray()
        {
            using (ByteBuffer oPacket = new ByteBuffer())
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteStringFixed(this.Name, 13)
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

                oPacket.Flip();
                return oPacket.GetContent();
            }
        }

        public byte[] AppearanceToByteArray()
        {
            using (ByteBuffer oPacket = new ByteBuffer())
            {
                oPacket
                    .WriteByte((byte)this.Gender)
                    .WriteByte(this.Skin)
                    .WriteInt(this.Face)
                    .WriteBool(true)
                    .WriteInt(this.Hair);

                Dictionary<byte, int> visibleLayer = new Dictionary<byte, int>();
                Dictionary<byte, int> hiddenLayer = new Dictionary<byte, int>();

                foreach (Item item in this.Items.GetEquipped())
                {
                    byte slot = item.AbsoluteSlot;

                    if (slot < 100 && !visibleLayer.ContainsKey(slot))
                    {
                        visibleLayer[slot] = item.MapleID;
                    }
                    else if (slot > 100 && slot != 111)
                    {
                        slot -= 100;

                        if (visibleLayer.ContainsKey(slot))
                        {
                            hiddenLayer[slot] = visibleLayer[slot];
                        }

                        visibleLayer[slot] = item.MapleID;
                    }
                    else if (visibleLayer.ContainsKey(slot))
                    {
                        hiddenLayer[slot] = item.MapleID;
                    }
                }

                foreach (KeyValuePair<byte, int> entry in visibleLayer)
                {
                    oPacket
                        .WriteByte(entry.Key)
                        .WriteInt(entry.Value);
                }

                oPacket.WriteByte(byte.MaxValue);

                foreach (KeyValuePair<byte, int> entry in hiddenLayer)
                {
                    oPacket
                        .WriteByte(entry.Key)
                        .WriteInt(entry.Value);
                }

                oPacket.WriteByte(byte.MaxValue);

                Item cashWeapon = this.Items[EquipmentSlot.CashWeapon];

                oPacket.WriteInt(cashWeapon != null ? cashWeapon.MapleID : 0);

                oPacket
                    .WriteInt()
                    .WriteInt()
                    .WriteInt();

                oPacket.Flip();
                return oPacket.GetContent();
            }
        }

        public byte[] DataToByteArray(long flag = long.MaxValue)
        {
            using (ByteBuffer oPacket = new ByteBuffer())
            {
                oPacket
                    .WriteLong(flag)
                    .WriteByte() // NOTE: Unknown.
                    .WriteBytes(this.StatisticsToByteArray())
                    .WriteByte(20) // NOTE: Max buddylist size.
                    .WriteBool(false) // NOTE: Blessing of Fairy.
                    .WriteInt(this.Meso)
                    .WriteBytes(this.Items.ToByteArray())
                    .WriteBytes(this.Skills.ToByteArray())
                    .WriteBytes(this.Quests.ToByteArray())
                    .WriteShort() // NOTE: Mini games record.
                    .WriteShort() // NOTE: Rings (1).
                    .WriteShort() // NOTE: Rings (2). 
                    .WriteShort() // NOTE: Rings (3).
                    .WriteBytes(this.Trocks.RegularToByteArray())
                    .WriteBytes(this.Trocks.VIPToByteArray())
                    .WriteInt() // NOTE: Monster Book cover ID.
                    .WriteByte() // NOTE: Monster Book cards.
                    .WriteShort() // NOTE: New Year Cards.
                    .WriteShort() // NOTE: QuestRecordEX.
                    .WriteShort() // NOTE: AdminShop.
                    .WriteShort(); // NOTE: Unknown.

                oPacket.Flip();
                return oPacket.GetContent();
            }
        }

        public Packet GetCreatePacket()
        {
            return this.GetSpawnPacket();
        }

        public Packet GetSpawnPacket()
        {
            Packet oPacket = new Packet(ServerOperationCode.UserEnterField);

            oPacket
                .WriteInt(this.ID)
                .WriteByte(this.Level)
                .WriteString(this.Name);

            if (false)
            {
                oPacket
                    .WriteString("")
                    .WriteShort()
                    .WriteByte()
                    .WriteShort()
                    .WriteByte();
            }
            else
            {
                oPacket.Skip(8);
            }

            oPacket
                .WriteBytes(this.Buffs.ToByteArray())
                .WriteShort((short)this.Job)
                .WriteBytes(this.AppearanceToByteArray())
                .WriteInt(this.Items.Available(5110000))
                .WriteInt() // NOTE: Item effect.
                .WriteInt((int)(Item.GetType(this.Chair) == ItemType.Setup ? this.Chair : 0))
                .WriteShort(this.Position.X)
                .WriteShort(this.Position.Y)
                .WriteByte(this.Stance)
                .WriteShort(this.Foothold)
                .WriteByte()
                .WriteByte()
                .WriteInt(1)
                .WriteLong();

            if (this.PlayerShop != null && this.PlayerShop.Owner == this)
            {
                oPacket
                    .WriteByte(4)
                    .WriteInt(this.PlayerShop.ObjectID)
                    .WriteString(this.PlayerShop.Description)
                    .WriteByte()
                    .WriteByte()
                    .WriteByte(1)
                    .WriteByte((byte)(this.PlayerShop.IsFull ? 1 : 2)) // NOTE: Visitor availability.
                    .WriteByte();
            }
            else
            {
                oPacket.WriteByte();
            }

            bool hasChalkboard = !string.IsNullOrEmpty(this.Chalkboard);

            oPacket.WriteBool(hasChalkboard);

            if (hasChalkboard)
            {
                oPacket.WriteString(this.Chalkboard);
            }

            oPacket
                .WriteByte() // NOTE: Couple ring.
                .WriteByte() // NOTE: Friendship ring.
                .WriteByte() // NOTE: Marriage ring.
                .Skip(3) // NOTE: Unknown.
                .WriteByte(byte.MaxValue); // NOTE: Team.

            return oPacket;
        }

        public Packet GetDestroyPacket()
        {
            Packet oPacket = new Packet(ServerOperationCode.UserLeaveField);

            oPacket.WriteInt(this.ID);

            return oPacket;
        }
    }
}