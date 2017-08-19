using Destiny.Network;
using Destiny.Core.IO;
using Destiny.Maple.Maps;
using System;
using Destiny.Core.Network;
using System.Collections.Generic;
using Destiny.Maple.Commands;
using Destiny.Maple.Life;
using Destiny.Data;
using Destiny.Maple.Data;
using Destiny.Maple.Interaction;

namespace Destiny.Maple.Characters
{
    public sealed class Character : MapObject, IMoveable, ISpawnable
    {
        public MapleClient Client { get; private set; }

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

        public CharacterItems Items { get; private set; }
        public CharacterSkills Skills { get; private set; }
        public CharacterQuests Quests { get; private set; }
        public CharacterKeymap Keymap { get; private set; }
        public CharacterPets Pets { get; private set; }
        public CharacterMemos Memos { get; private set; }
        public CharacterStorage Storage { get; private set; }
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
                    using (OutPacket oPacket = new OutPacket(ServerOperationCode.SetGender))
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

                            using (OutPacket oPacket = new OutPacket(ServerOperationCode.ShowForeignBuff))
                            {
                                oPacket
                                    .WriteInt(this.ID)
                                    .WriteByte((byte)ForeignEffect.Level);

                                this.Map.Broadcast(oPacket, this);
                            }
                        }
                    }
                }
            }
        }

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

                    using (OutPacket oPacket = new OutPacket(ServerOperationCode.ShowForeignBuff))
                    {
                        oPacket
                            .WriteInt(this.ID)
                            .WriteByte((byte)ForeignEffect.Job);

                        this.Map.Broadcast(oPacket, this);
                    }
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
                health = value;

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
                mana = value;

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

                    using (OutPacket oPacket = new OutPacket(ServerOperationCode.Message))
                    {
                        oPacket
                            .WriteByte((byte)MessageType.IncreaseEXP)
                            .WriteBool(true)
                            .WriteInt(delta)
                            .WriteBool() // NOTE: In chat.
                            .WriteInt() // NOTE: Monster Book bonus.
                            .WriteShort() // NOTE: Unknown.
                            .WriteInt() // NOTE: Wedding bonus.
                            .WriteByte() // NOTE: Party bonus.
                            .WriteInt() // NOTE: Party bonus.
                            .WriteInt() // NOTE: Equip bonus.
                            .WriteInt() // NOTE: Internet Cafe bonus.
                            .WriteInt(); // NOTE: Rainbow Week bonus.

                        this.Client.Send(oPacket);
                    }
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

        public bool IsGm
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
                lastNpc = value;
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

                using (OutPacket oPacket = new OutPacket(ServerOperationCode.Chalkboard))
                {
                    oPacket
                        .WriteInt(this.ID)
                        .WriteBool(!string.IsNullOrEmpty(this.Chalkboard))
                        .WriteMapleString(this.Chalkboard);

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

        public Character(int id = 0, MapleClient client = null)
        {
            this.ID = id;
            this.Client = client;

            this.Items = new CharacterItems(this, 24, 24, 24, 24, 48);
            this.Skills = new CharacterSkills(this);
            this.Quests = new CharacterQuests(this);
            this.Keymap = new CharacterKeymap(this);
            this.Pets = new CharacterPets(this);
            this.Memos = new CharacterMemos(this);
            this.Storage = new CharacterStorage(this);

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
            this.Health = (short)datum["Health"];
            this.MaxHealth = (short)datum["MaxHealth"];
            this.Mana = (short)datum["Mana"];
            this.MaxMana = (short)datum["MaxMana"];
            this.AbilityPoints = (short)datum["AbilityPoints"];
            this.SkillPoints = (short)datum["SkillPoints"];
            this.Experience = (int)datum["Experience"];
            this.Fame = (short)datum["Fame"];
            this.Map = this.Client.Channel.Maps[(int)datum["Map"]];
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
            this.Keymap.Load();
            this.Memos.Load();
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
            datum["Map"] = this.Map.MapleID;
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
            this.Keymap.Save();
            this.Pets.Save();

            Log.Inform("Saved character '{0}' to database.", this.Name);
        }

        public void Delete()
        {

        }

        public void Initialize(bool cashShop = false)
        {
            using (OutPacket oPacket = new OutPacket(cashShop ? ServerOperationCode.SetCashShop : ServerOperationCode.SetField))
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
                        .WriteInt(this.Client.ChannelID)
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

                this.Keymap.Send();

                this.Memos.Send();
            }
        }

        public void Update(params StatisticType[] statistics)
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.StatChanged))
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
                            oPacket.WriteInt(this.Meso);
                            break;
                    }
                }

                this.Client.Send(oPacket);
            }
        }

        public void UpdateApperance()
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.AvatarModified))
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

        public void Release()
        {
            this.Update();
        }

        public void Notify(string message, NoticeType type = NoticeType.Pink)
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.BroadcastMsg))
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

        public void ChangeMap(InPacket iPacket)
        {
            byte portals = iPacket.ReadByte();

            if (portals != this.Portals)
            {
                return;
            }

            int destinationID = iPacket.ReadInt();

            switch (destinationID)
            {
                case -1:
                    {
                        string label = iPacket.ReadMapleString();
                        Portal portal;

                        try
                        {
                            portal = this.Map.Portals[label];
                        }
                        catch (KeyNotFoundException)
                        {
                            return;
                        }

                        this.ChangeMap(portal.DestinationMapID, portal.Link.ID);
                    }
                    break;
            }
        }

        public void ChangeMap(int mapID, byte portalID = 0)
        {
            // NOTE: If the map doesn't exist, this line will throw an exception. Calling method needs to catch and handle that situation.
            Map newMap = this.Client.Channel.Maps[mapID];

            // NOTE: If a portal isn't specified, a random spawn point will be chosen.
            if (portalID == 0)
            {
                List<Portal> spawnPoints = new List<Portal>();

                foreach (Portal loopPortal in newMap.Portals)
                {
                    if (loopPortal.IsSpawnPoint)
                    {
                        spawnPoints.Add(loopPortal);
                    }
                }

                this.SpawnPoint = spawnPoints.Count > 0 ? spawnPoints[Constants.Random.Next(0, spawnPoints.Count - 1)].ID : (byte)0;
            }
            else
            {
                this.SpawnPoint = portalID;
            }

            Map oldMap = this.Map;
            oldMap.Characters.Remove(this);

            try
            {
                newMap.Characters.Add(this);
            }
            catch (Exception e)
            {
                //Failed to change map... Attempt to add the character back to the map they were on
                oldMap.Characters.Add(this);
                throw e;
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.SetField))
            {
                oPacket
                    .WriteInt(this.Client.ChannelID)
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

        public void Move(InPacket iPacket)
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

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.UserMove))
            {
                oPacket.WriteInt(this.ID);

                movements.Encode(oPacket);

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

        public void Sit(InPacket iPacket)
        {
            short seatID = iPacket.ReadShort();

            if (seatID == -1)
            {
                this.Chair = 0;

                using (OutPacket oPacket = new OutPacket(ServerOperationCode.ShowChair))
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

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.Sit))
            {
                oPacket.WriteBool(seatID != -1);

                if (seatID != -1)
                {
                    oPacket.WriteShort(seatID);
                }

                this.Client.Send(oPacket);
            }
        }

        public void SitChair(InPacket iPacket)
        {
            int mapleID = iPacket.ReadInt();

            if (!this.Items.Contains(mapleID))
            {
                return;
            }

            this.Chair = mapleID;

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.ShowChair))
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteInt(mapleID);

                this.Map.Broadcast(oPacket, this);
            }
        }

        public void Attack(InPacket iPacket, AttackType type)
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
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.CloseRangeAttack))
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

        public void Damage(InPacket iPacket)
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

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.UserHit))
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

        public void Talk(InPacket iPacket)
        {
            string text = iPacket.ReadMapleString();
            bool shout = iPacket.ReadBool(); // NOTE: Used for skill macros.

            if (text.StartsWith(Constants.CommandIndiciator.ToString()))
            {
                CommandFactory.Execute(this, text);
            }
            else
            {
                using (OutPacket oPacket = new OutPacket(ServerOperationCode.UserChat))
                {
                    oPacket
                        .WriteInt(this.ID)
                        .WriteBool(this.IsGm)
                        .WriteMapleString(text)
                        .WriteBool(shout);

                    this.Map.Broadcast(oPacket);
                }
            }
        }

        public void Express(InPacket iPacket)
        {
            int expressionID = iPacket.ReadInt();

            if (expressionID > 7) // NOTE: Cash facial expression.
            {
                int mapleID = 5159992 + expressionID;

                // TODO: Validate if item exists.
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.UserEmotion))
            {
                oPacket
                    .WriteInt(this.ID)
                    .WriteInt(expressionID);

                this.Map.Broadcast(oPacket, this);
            }
        }

        public void Converse(InPacket iPacket)
        {
            int objectID = iPacket.ReadInt();

            this.Converse(this.Map.Npcs[objectID]);
        }

        public void Converse(Npc npc)
        {
            this.LastNpc = npc;
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

        public void DistributeAP(InPacket iPacket)
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

        public void AutoDistributeAP(InPacket iPacket)
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

        public void HealOverTime(InPacket iPacket)
        {
            iPacket.ReadInt(); // NOTE: Ticks.
            iPacket.ReadInt(); // NOTE: Unknown.
            short healthAmount = iPacket.ReadShort();
            short manaAmount = iPacket.ReadShort();

            if (healthAmount != 0)
            {
                if ((DateTime.Now - this.LastHealthHealOverTime).Seconds < 2)
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
                if ((DateTime.Now - this.LastManaHealOverTime).Seconds < 2)
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

        public void DistributeSP(InPacket iPacket)
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

        public void DropMeso(InPacket iPacket)
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

        public void InformOnCharacter(InPacket iPacket)
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

            if (target.IsGm && !this.IsGm)
            {
                return;
            }

            using (OutPacket oPacket = new OutPacket(ServerOperationCode.CharacterInformation))
            {
                oPacket
                    .WriteInt(target.ID)
                    .WriteByte(target.Level)
                    .WriteShort((short)target.Job)
                    .WriteShort(target.Fame)
                    .WriteBool() // NOTE: Marriage.
                    .WriteMapleString("-") // NOTE: Guild name.
                    .WriteMapleString("-") // NOTE: Alliance name.
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

        public void UseCommand(InPacket iPacket)
        {
            CommandType type = (CommandType)iPacket.ReadByte();
            string targetName = iPacket.ReadMapleString();

            Character target = null;

            switch (type)
            {
                case CommandType.Find:
                    {
                        if (target == null)
                        {
                            using (OutPacket oPacket = new OutPacket(ServerOperationCode.Command))
                            {
                                oPacket
                                    .WriteByte(0x0A)
                                    .WriteMapleString(targetName)
                                    .WriteBool(false);

                                this.Client.Send(oPacket);
                            }
                        }
                        else
                        {
                            using (OutPacket oPacket = new OutPacket(ServerOperationCode.Command))
                            {
                                oPacket
                                    .WriteByte(0x09)
                                    .WriteMapleString(targetName)
                                    .WriteByte(1) // NOTE: 0 - MTS, 1 - Map, 2 - Cash Shop.
                                    .WriteInt(target.Map.MapleID); // NOTE: -1 if MTS/Cash Shop.

                                if (true) // NOTE: Does not apply if MTS/Cash Shop.
                                {
                                    oPacket
                                        .WriteInt() // NOTE: Unknown.
                                        .WriteInt(); // NOTE: Unknown.
                                }

                                this.Client.Send(oPacket);
                            }
                        }
                    }
                    break;

                case CommandType.Whisper:
                    {
                        string text = iPacket.ReadMapleString();

                        using (OutPacket oPacket = new OutPacket(ServerOperationCode.Command))
                        {
                            oPacket
                                .WriteByte(10)
                                .WriteMapleString(targetName)
                                .WriteBool(target != null);

                            this.Client.Send(oPacket);
                        }

                        if (target != null)
                        {
                            using (OutPacket oPacket = new OutPacket(ServerOperationCode.Command))
                            {
                                oPacket
                                    .WriteByte(18)
                                    .WriteMapleString(this.Name)
                                    .WriteByte(this.Client.ChannelID)
                                    .WriteByte() // NOTE: Unknown.
                                    .WriteMapleString(text);

                                target.Client.Send(oPacket);
                            }
                        }
                    }
                    break;
            }
        }

        public void Interact(InPacket iPacket)
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
                                    string description = iPacket.ReadMapleString();

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

        public void ChangeMapSpecial(InPacket iPacket)
        {
            byte portals = iPacket.ReadByte();

            if (portals != this.Portals)
            {
                return;
            }

            string label = iPacket.ReadMapleString();
            Portal portal;

            try
            {
                portal = this.Map.Portals[label];
            }
            catch (KeyNotFoundException)
            {
                return;
            }
        }

        public void Encode(OutPacket oPacket)
        {
            this.EncodeStatistics(oPacket);
            this.EncodeApperance(oPacket);

            if (!this.Client.IsInViewAllChar)
            {
                oPacket.WriteByte(); //NOTE: Family
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

        public void EncodeApperance(OutPacket oPacket, bool mega = false)
        {
            oPacket
                .WriteByte((byte)this.Gender)
                .WriteByte(this.Skin)
                .WriteInt(this.Face)
                .WriteBool(mega)
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
                .WriteInt(this.Meso);

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
            return this.GetSpawnPacket();
        }

        public OutPacket GetSpawnPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.UserEnterField);

            oPacket
                .WriteInt(this.ID)
                .WriteByte(this.Level)
                .WriteMapleString(this.Name)
                .WriteMapleString(string.Empty) // NOTE: Guild name.
                .WriteZero(6) // NOTE: Guild emblems.
                .WriteInt()
                .WriteShort()
                .WriteByte(0xFC)
                .WriteByte(1)
                .WriteInt();

            int buffmask = 0;

            oPacket
                .WriteInt((int)((buffmask >> 32) & 0xFFFFFFFFL))
                .WriteInt((int)(buffmask & 0xFFFFFFFFL));

            int magic = Constants.Random.Next();

            oPacket
                .WriteZero(6)
                .WriteInt(magic)
                .WriteZero(11)
                .WriteInt(magic)
                .WriteZero(11)
                .WriteInt(magic)
                .WriteShort()
                .WriteByte()
                .WriteLong()
                .WriteInt(magic)
                .WriteZero(9)
                .WriteInt(magic)
                .WriteShort()
                .WriteInt()
                .WriteZero(10)
                .WriteInt(magic)
                .WriteZero(13)
                .WriteInt(magic)
                .WriteShort()
                .WriteByte()
                .WriteShort((short)this.Job);

            this.EncodeApperance(oPacket);

            oPacket
                .WriteInt(this.Items.Available(5110000))
                .WriteInt() // NOTE: Item effect.
                .WriteInt((int)(Item.GetType(this.Chair) == ItemType.Setup ? this.Chair : 0))
                .WritePoint(this.Position)
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
                    .WriteMapleString(this.PlayerShop.Description)
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
                oPacket.WriteMapleString(this.Chalkboard);
            }

            oPacket
                .WriteByte() // NOTE: Couple ring.
                .WriteByte() // NOTE: Friendship ring.
                .WriteByte() // NOTE: Marriage ring.
                .WriteZero(3) // NOTE: Unknown.
                .WriteByte(byte.MaxValue); // NOTE: Team.

            return oPacket;
        }

        public OutPacket GetDestroyPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.UserLeaveField);

            oPacket.WriteInt(this.ID);

            return oPacket;
        }
    }
}