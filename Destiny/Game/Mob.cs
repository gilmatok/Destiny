using System.Collections.Generic;
using System.IO;

namespace Destiny.Game
{
    public sealed class MobAbility
    {
        public byte Identifier { get; set; }
        public byte Level { get; set; }
        public ushort EffectDelay { get; set; }

        public MobAbility(BinaryReader reader)
        {
            this.Identifier = reader.ReadByte();
            this.Level = reader.ReadByte();
            this.EffectDelay = reader.ReadUInt16();
        }
    }

    public sealed class MobAttack
    {
        public MobAttackFlags Flags { get; set; }
        public byte MPCost { get; set; }
        public ushort MPBurn { get; set; }
        public byte AbilityID { get; set; }
        public byte AbilityLevel { get; set; }

        public MobAttack(BinaryReader reader)
        {
            this.Flags = (MobAttackFlags)reader.ReadByte();
            this.MPCost = reader.ReadByte();
            this.MPBurn = reader.ReadUInt16();
            this.AbilityID = reader.ReadByte();
            this.AbilityLevel = reader.ReadByte();
        }
    }

    public sealed class MobDrop
    {
        public MobDropFlags Flags { get; set; }
        public int ItemID { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public ushort QuestID { get; set; }
        public int Chance { get; set; }

        public MobDrop(BinaryReader reader)
        {
            this.Flags = (MobDropFlags)reader.ReadByte();
            this.ItemID = reader.ReadInt32();
            this.Minimum = reader.ReadInt32();
            this.Maximum = reader.ReadInt32();
            this.QuestID = reader.ReadUInt16();
            this.Chance = reader.ReadInt32();
        }
    }

    public sealed class Mob
    {
        public int Identifier { get; private set; }
        public MobFlags Flags { get; private set; }
        public byte Level { get; private set; }
        public int HP { get; private set; }
        public int MP { get; private set; }
        public int HPRecovery { get; private set; }
        public ushort MPRecovery { get; private set; }
        public int ExplodeHP { get; private set; }
        public int Experience { get; private set; }
        public int LinkIdentifier { get; private set; }
        public byte SummonType { get; private set; }
        public int Knockback { get; private set; }
        public ushort FixedDamage { get; private set; }
        public int DeathBuffIdentifier { get; private set; }
        public int DeathAfter { get; private set; }
        public byte Traction { get; private set; }
        public int DamagedBySkillIdentifierOnly { get; private set; }
        public int DamagedByMobIdentifierOnly { get; private set; }
        public byte HPBarColor { get; private set; }
        public byte HPBarBackgroundColor { get; private set; }
        public byte CarnivalPoints { get; private set; }
        public ushort PhysicalAttack { get; private set; }
        public uint PhysicalDefense { get; private set; }
        public ushort MagicalAttack { get; private set; }
        public uint MagicalDefense { get; private set; }
        public short Accuracy { get; private set; }
        public ushort Avoidance { get; private set; }
        public short Speed { get; private set; }
        public short ChaseSpeed { get; private set; }
        public MobMagicModifier IceModifier { get; private set; }
        public MobMagicModifier FireModifier { get; private set; }
        public MobMagicModifier PoisonModifier { get; private set; }
        public MobMagicModifier LightningModifier { get; private set; }
        public MobMagicModifier HolyModifier { get; private set; }
        public MobMagicModifier NonElementalModifier { get; private set; }
        public List<MobAbility> Abilities { get; private set; }
        public List<MobAttack> Attacks { get; private set; }
        public List<int> Summons { get; private set; }
        public List<MobDrop> Drops { get; private set; }

        public Mob(BinaryReader reader)
        {
            this.Identifier = reader.ReadInt32();
            this.Flags = (MobFlags)reader.ReadUInt16();
            this.Level = reader.ReadByte();
            this.HP = reader.ReadInt32();
            this.MP = reader.ReadInt32();
            this.HPRecovery = reader.ReadInt32();
            this.MPRecovery = reader.ReadUInt16();
            this.ExplodeHP = reader.ReadInt32();
            this.Experience = reader.ReadInt32();
            this.LinkIdentifier = reader.ReadInt32();
            this.SummonType = reader.ReadByte();
            this.Knockback = reader.ReadInt32();
            this.FixedDamage = reader.ReadUInt16();
            this.DeathBuffIdentifier = reader.ReadInt32();
            this.DeathAfter = reader.ReadInt32();
            this.Traction = reader.ReadByte();
            this.DamagedBySkillIdentifierOnly = reader.ReadInt32();
            this.DamagedByMobIdentifierOnly = reader.ReadInt32();
            this.HPBarColor = reader.ReadByte();
            this.HPBarBackgroundColor = reader.ReadByte();
            this.CarnivalPoints = reader.ReadByte();
            this.PhysicalAttack = reader.ReadUInt16();
            this.PhysicalDefense = reader.ReadUInt32();
            this.MagicalAttack = reader.ReadUInt16();
            this.MagicalDefense = reader.ReadUInt32();
            this.Accuracy = reader.ReadInt16();
            this.Avoidance = reader.ReadUInt16();
            this.Speed = reader.ReadInt16();
            this.ChaseSpeed = reader.ReadInt16();
            this.IceModifier = (MobMagicModifier)reader.ReadByte();
            this.FireModifier = (MobMagicModifier)reader.ReadByte();
            this.PoisonModifier = (MobMagicModifier)reader.ReadByte();
            this.LightningModifier = (MobMagicModifier)reader.ReadByte();
            this.HolyModifier = (MobMagicModifier)reader.ReadByte();
            this.NonElementalModifier = (MobMagicModifier)reader.ReadByte();

            int abilitiesCount = reader.ReadInt32();

            this.Abilities = new List<MobAbility>(abilitiesCount);

            while (abilitiesCount-- > 0)
            {
                this.Abilities.Add(new MobAbility(reader));
            }

            int attacksCount = reader.ReadInt32();

            this.Attacks = new List<MobAttack>(attacksCount);

            while (attacksCount-- > 0)
            {
                this.Attacks.Add(new MobAttack(reader));
            }

            int summonsCount = reader.ReadInt32();

            this.Summons = new List<int>(summonsCount);

            while (summonsCount-- > 0)
            {
                this.Summons.Add(reader.ReadInt32());
            }

            int dropsCount = reader.ReadInt32();

            this.Drops = new List<MobDrop>(dropsCount);

            while (dropsCount-- > 0)
            {
                this.Drops.Add(new MobDrop(reader));
            }
        }
    }
}
