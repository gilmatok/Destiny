using Destiny.Utility;
using System.Collections.Generic;
using System.IO;

namespace Destiny.Server.Data
{
    public sealed class SkillDataProvider
    {
        private Dictionary<int, Dictionary<byte, SkillData>> mSkills;

        public SkillDataProvider()
        {
            mSkills = new Dictionary<int, Dictionary<byte, SkillData>>();
        }

        public void Load()
        {
            mSkills.Clear();

            int count;

            using (BinaryReader reader = new BinaryReader(File.OpenRead(Path.Combine(Config.Instance.Binary, "Skills.bin"))))
            {
                count = reader.ReadInt32();
                while (count-- > 0)
                {
                    SkillData skill = new SkillData();
                    skill.Load(reader);

                    if (!mSkills.ContainsKey(skill.MapleID))
                    {
                        mSkills.Add(skill.MapleID, new Dictionary<byte, SkillData>());
                    }

                    mSkills[skill.MapleID].Add(skill.Level, skill);
                }
            }
        }
    }

    public sealed class SkillData
    {
        public static bool IsFourthJobRelated(int pSkillIdentifier) { return ((pSkillIdentifier / 10000) % 10) == 2; }

        public int MapleID { get; set; }
        public byte Level { get; set; }
        public byte MobCount { get; set; }
        public byte HitCount { get; set; }
        public ushort Range { get; set; }
        public int Duration { get; set; }
        public ushort MPCost { get; set; }
        public byte HPCost { get; set; }
        public ushort Damage { get; set; }
        public byte FixedDamage { get; set; }
        public byte CriticalDamage { get; set; }
        public byte Mastery { get; set; }
        public int OptionalItemCost { get; set; }
        public int ItemCost { get; set; }
        public byte ItemCount { get; set; }
        public byte BulletCost { get; set; }
        public ushort MoneyCost { get; set; }
        public int Parameter1 { get; set; }
        public int Parameter2 { get; set; }
        public short Speed { get; set; }
        public byte Jump { get; set; }
        public byte Strength { get; set; }
        public short WeaponAttack { get; set; }
        public short WeaponDefense { get; set; }
        public short MagicAttack { get; set; }
        public short MagicDefense { get; set; }
        public byte Accuracy { get; set; }
        public byte Avoidance { get; set; }
        public ushort HP { get; set; }
        public byte MP { get; set; }
        public byte Prop { get; set; }
        public ushort Morph { get; set; }
        public short LeftTopX { get; set; }
        public short LeftTopY { get; set; }
        public short RightBottomX { get; set; }
        public short RightBottomY { get; set; }
        public ushort Cooldown { get; set; }

        public void Save(BinaryWriter pWriter)
        {
            pWriter.Write(MapleID);
            pWriter.Write(Level);
            pWriter.Write(MobCount);
            pWriter.Write(HitCount);
            pWriter.Write(Range);
            pWriter.Write(Duration);
            pWriter.Write(MPCost);
            pWriter.Write(HPCost);
            pWriter.Write(Damage);
            pWriter.Write(FixedDamage);
            pWriter.Write(CriticalDamage);
            pWriter.Write(Mastery);
            pWriter.Write(OptionalItemCost);
            pWriter.Write(ItemCost);
            pWriter.Write(ItemCount);
            pWriter.Write(BulletCost);
            pWriter.Write(MoneyCost);
            pWriter.Write(Parameter1);
            pWriter.Write(Parameter2);
            pWriter.Write(Speed);
            pWriter.Write(Jump);
            pWriter.Write(Strength);
            pWriter.Write(WeaponAttack);
            pWriter.Write(WeaponDefense);
            pWriter.Write(MagicAttack);
            pWriter.Write(MagicDefense);
            pWriter.Write(Accuracy);
            pWriter.Write(Avoidance);
            pWriter.Write(HP);
            pWriter.Write(MP);
            pWriter.Write(Prop);
            pWriter.Write(Morph);
            pWriter.Write(LeftTopX);
            pWriter.Write(LeftTopY);
            pWriter.Write(RightBottomX);
            pWriter.Write(RightBottomY);
            pWriter.Write(Cooldown);
        }

        public void Load(BinaryReader pReader)
        {
            MapleID = pReader.ReadInt32();
            Level = pReader.ReadByte();
            MobCount = pReader.ReadByte();
            HitCount = pReader.ReadByte();
            Range = pReader.ReadUInt16();
            Duration = pReader.ReadInt32();
            MPCost = pReader.ReadUInt16();
            HPCost = pReader.ReadByte();
            Damage = pReader.ReadUInt16();
            FixedDamage = pReader.ReadByte();
            CriticalDamage = pReader.ReadByte();
            Mastery = pReader.ReadByte();
            OptionalItemCost = pReader.ReadInt32();
            ItemCost = pReader.ReadInt32();
            ItemCount = pReader.ReadByte();
            BulletCost = pReader.ReadByte();
            MoneyCost = pReader.ReadUInt16();
            Parameter1 = pReader.ReadInt32();
            Parameter2 = pReader.ReadInt32();
            Speed = pReader.ReadInt16();
            Jump = pReader.ReadByte();
            Strength = pReader.ReadByte();
            WeaponAttack = pReader.ReadInt16();
            WeaponDefense = pReader.ReadInt16();
            MagicAttack = pReader.ReadInt16();
            MagicDefense = pReader.ReadInt16();
            Accuracy = pReader.ReadByte();
            Avoidance = pReader.ReadByte();
            HP = pReader.ReadUInt16();
            MP = pReader.ReadByte();
            Prop = pReader.ReadByte();
            Morph = pReader.ReadUInt16();
            LeftTopX = pReader.ReadInt16();
            LeftTopY = pReader.ReadInt16();
            RightBottomX = pReader.ReadInt16();
            RightBottomY = pReader.ReadInt16();
            Cooldown = pReader.ReadUInt16();
        }
    }
}
