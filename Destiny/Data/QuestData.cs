using System;
using System.Collections.Generic;
using System.IO;

namespace Destiny.Data
{
    public sealed class QuestData
    {
        public sealed class QuestRequirementData
        {
            public enum EQuestRequirementState : byte
            {
                Start = 1,
                End = 2
            }

            public enum EQuestRequirementType : byte
            {
                Mob = 1,
                Item = 2,
                Quest = 3
            }


            public EQuestRequirementState State { get; set; }
            public EQuestRequirementType Type { get; set; }
            public int Parameter1 { get; set; }
            public int Parameter2 { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write((byte)State);
                pWriter.Write((byte)Type);
                pWriter.Write(Parameter1);
                pWriter.Write(Parameter2);
            }

            public void Load(BinaryReader pReader)
            {
                State = (EQuestRequirementState)pReader.ReadByte();
                Type = (EQuestRequirementType)pReader.ReadByte();
                Parameter1 = pReader.ReadInt32();
                Parameter2 = pReader.ReadInt32();
            }
        }

        public sealed class QuestRewardData
        {
            [Flags]
            public enum EQuestRewardFlags : byte
            {
                None = 0 << 0,
                Only_Master_Level = 1 << 0,
                OnlyMasterLevel = 1 << 0
            }

            public enum EQuestRewardState : byte
            {
                Start = 1,
                End = 2
            }

            public enum EQuestRewardType : byte
            {
                Item = 1,
                Exp = 2,
                Experience = 2,
                Mesos = 3,
                Fame = 4,
                Skill = 5,
                Buff = 6,
                Pet_Closeness = 7,
                PetCloseness = 7,
                Pet_Speed = 8,
                PetSpeed = 8,
                Pet_Skill = 9,
                PetSkill = 9
            }

            public enum EQuestRewardGender : byte
            {
                Male = 1,
                Female = 2,
                Both = 3
            }

            [Flags]
            public enum EQuestRewardTracks : uint
            {
                None = 0 << 0,
                Beginner = 1 << 0,
                Warrior = 1 << 1,
                Magician = 1 << 2,
                Bowman = 1 << 3,
                Thief = 1 << 4,
                Pirate = 1 << 5,
                Cygnus_Beginner = 1 << 6,
                CygnusBeginner = 1 << 6,
                Cygnus_Warrior = 1 << 7,
                CygnusWarrior = 1 << 7,
                Cygnus_Magician = 1 << 8,
                CygnusMagician = 1 << 8,
                Cygnus_Bowman = 1 << 9,
                CygnusBowman = 1 << 9,
                Cygnus_Thief = 1 << 10,
                CygnusThief = 1 << 10,
                Cygnus_Pirate = 1 << 11,
                CygnusPirate = 1 << 11,
                Episode2_Beginner = 1 << 12,
                Episode2_Warrior = 1 << 13,
                Episode2_Bowman = 1 << 14,
                Episode2_Magician = 1 << 15,
                Episode2_Thief = 1 << 16,
                Episode2_Pirate = 1 << 17,
            }


            public EQuestRewardFlags Flags { get; set; }
            public EQuestRewardState State { get; set; }
            public EQuestRewardType Type { get; set; }
            public int Parameter1 { get; set; }
            public int Parameter2 { get; set; }
            public byte MasterLevel { get; set; }
            public EQuestRewardGender Gender { get; set; }
            public EQuestRewardTracks Tracks { get; set; }
            public short Job { get; set; }
            public int Prop { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write((byte)Flags);
                pWriter.Write((byte)State);
                pWriter.Write((byte)Type);
                pWriter.Write(Parameter1);
                pWriter.Write(Parameter2);
                pWriter.Write(MasterLevel);
                pWriter.Write((byte)Gender);
                pWriter.Write((ushort)Tracks);
                pWriter.Write(Job);
                pWriter.Write(Prop);
            }

            public void Load(BinaryReader pReader)
            {
                Flags = (EQuestRewardFlags)pReader.ReadByte();
                State = (EQuestRewardState)pReader.ReadByte();
                Type = (EQuestRewardType)pReader.ReadByte();
                Parameter1 = pReader.ReadInt32();
                Parameter2 = pReader.ReadInt32();
                MasterLevel = pReader.ReadByte();
                Gender = (EQuestRewardGender)pReader.ReadByte();
                Tracks = (EQuestRewardTracks)pReader.ReadUInt16();
                Job = pReader.ReadInt16();
                Prop = pReader.ReadInt32();
            }
        }


        public ushort Identifier { get; set; }
        public ushort NextIdentifier { get; set; }
        public byte Area { get; set; }
        public byte MinLevel { get; set; }
        public byte MaxLevel { get; set; }
        public ushort PetCloseness { get; set; }
        public byte TamingMobLevel { get; set; }
        public int RepeatWait { get; set; }
        public ushort Fame { get; set; }
        public List<ushort> Jobs { get; set; }
        public List<QuestRequirementData> Requirements { get; set; }
        public List<QuestRewardData> Rewards { get; set; }

        public void Save(BinaryWriter pWriter)
        {
            pWriter.Write(Identifier);
            pWriter.Write(NextIdentifier);
            pWriter.Write(Area);
            pWriter.Write(MinLevel);
            pWriter.Write(MaxLevel);
            pWriter.Write(PetCloseness);
            pWriter.Write(TamingMobLevel);
            pWriter.Write(RepeatWait);
            pWriter.Write(Fame);

            pWriter.Write(Jobs.Count);
            Jobs.ForEach(i => pWriter.Write(i));

            pWriter.Write(Requirements.Count);
            Requirements.ForEach(r => r.Save(pWriter));

            pWriter.Write(Rewards.Count);
            Rewards.ForEach(r => r.Save(pWriter));
        }

        public void Load(BinaryReader pReader)
        {
            Identifier = pReader.ReadUInt16();
            NextIdentifier = pReader.ReadUInt16();
            Area = pReader.ReadByte();
            MinLevel = pReader.ReadByte();
            MaxLevel = pReader.ReadByte();
            PetCloseness = pReader.ReadUInt16();
            TamingMobLevel = pReader.ReadByte();
            RepeatWait = pReader.ReadInt32();
            Fame = pReader.ReadUInt16();

            int jobsCount = pReader.ReadInt32();
            Jobs = new List<ushort>(jobsCount);
            while (jobsCount-- > 0) Jobs.Add(pReader.ReadUInt16());

            int requirementsCount = pReader.ReadInt32();
            Requirements = new List<QuestRequirementData>(requirementsCount);
            while (requirementsCount-- > 0)
            {
                QuestRequirementData requirement = new QuestRequirementData();
                requirement.Load(pReader);
                Requirements.Add(requirement);
            }

            int rewardsCount = pReader.ReadInt32();
            Rewards = new List<QuestRewardData>(rewardsCount);
            while (rewardsCount-- > 0)
            {
                QuestRewardData reward = new QuestRewardData();
                reward.Load(pReader);
                Rewards.Add(reward);
            }
        }
    }
}
