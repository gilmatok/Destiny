using Destiny.Core.IO;
using Destiny.Network;
using Destiny.Utility;
using System;

namespace Destiny.Game.Characters
{
    public sealed class CharacterStats
    {
        public Character Parent { get; private set; }

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
                return m_skin;
            }
            set
            {
                m_skin = value;

                if (this.Parent.IsInitialized)
                {
                    this.Update(StatisticType.Skin);

                    //  TODO: Appearance update to map.
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
                return m_hair;
            }
            set
            {
                m_hair = value;

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
                return m_level;
            }

            set
            {
                m_level = value;

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
                return m_job;
            }
            set
            {
                m_job = value;

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
                return m_strength;
            }

            set
            {
                m_strength = value;

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
                return m_dexterity;
            }

            set
            {
                m_dexterity = value;

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
                return m_intelligence;
            }

            set
            {
                m_intelligence = value;

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
                return m_luck;
            }

            set
            {
                m_luck = value;

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
                return m_health;
            }

            set
            {
                m_health = value;

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
                return m_maxHealth;
            }

            set
            {
                m_maxHealth = value;

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
                return m_mana;
            }

            set
            {
                m_mana = value;

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
                return m_maxMana;
            }

            set
            {
                m_maxMana = value;

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
                return m_abilityPoints;
            }

            set
            {
                m_abilityPoints = value;

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
                return m_skillPoints;
            }

            set
            {
                m_skillPoints = value;

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
                return m_experience;
            }

            set
            {
                m_experience = value;

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
                return m_fame;
            }

            set
            {
                m_fame = value;

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
                return m_mesos;
            }
            set
            {
                m_mesos = value;

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

                this.Parent.Client.Send(oPacket);
            }
        }

        public void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteInt(this.Parent.ID)
                .WritePaddedString(this.Parent.Name, 13)
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
