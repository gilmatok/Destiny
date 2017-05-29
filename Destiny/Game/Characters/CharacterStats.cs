using Destiny.Core.IO;
using Destiny.Packet;
using Destiny.Utility;

namespace Destiny.Game
{
    public sealed class CharacterStats
    {
        public Character Parent { get; private set; }

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
        private int mesos;

        public Gender Gender
        {
            get
            {
                return gender;
            }
            set
            {
                gender = value;

                if (this.Parent.IsInitialized)
                {
                    // TODO: Is there a gender set packet?
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
                skin = value;

                if (this.Parent.IsInitialized)
                {
                    this.Update(StatisticType.Skin);

                    // TODO: Appearance update to map.
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
                face = value;

                if (this.Parent.IsInitialized)
                {
                    this.Update(StatisticType.Face);

                    // TODO: Appearance update to map.
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
                hair = value;

                if (this.Parent.IsInitialized)
                {
                    this.Update(StatisticType.Hair);

                    // TODO: Appearance update to map.
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
                level = value;

                if (this.Parent.IsInitialized)
                {
                    this.Update(StatisticType.Level);
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

                if (this.Parent.IsInitialized)
                {
                    this.Update(StatisticType.Job);
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

                if (this.Parent.IsInitialized)
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

                if (this.Parent.IsInitialized)
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

                if (this.Parent.IsInitialized)
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

                if (this.Parent.IsInitialized)
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

                if (this.Parent.IsInitialized)
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

                if (this.Parent.IsInitialized)
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

                if (this.Parent.IsInitialized)
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

                if (this.Parent.IsInitialized)
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

                if (this.Parent.IsInitialized)
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

                if (this.Parent.IsInitialized)
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
                experience = value;

                if (this.Parent.IsInitialized)
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

                if (this.Parent.IsInitialized)
                {
                    this.Update(StatisticType.Fame);
                }
            }
        }

        public int Mesos
        {
            get
            {
                return mesos;
            }
            set
            {
                mesos = value;

                if (this.Parent.IsInitialized)
                {
                    this.Update(StatisticType.Mesos);
                }
            }
        }

        public CharacterStats(Character parent, DatabaseQuery query)
        {
            this.Parent = parent;

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
        }

        public void Update(params StatisticType[] statistics)
        {
            this.Parent.Client.Send(PlayerPacket.StatChanged(this.Parent, statistics));
        }

        public void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteInt(this.Parent.ID)
                .WriteStringFixed(this.Parent.Name, 13)
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
                .WriteInt(this.Parent.Map.MapleID)
                .WriteByte(this.Parent.SpawnPoint)
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

            this.Parent.Items.EncodeEquipment(oPacket);

            oPacket
                .WriteInt()
                .WriteInt()
                .WriteInt();
        }
    }
}
