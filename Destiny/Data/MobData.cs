using System;
using System.Collections.Generic;
using System.IO;

namespace Destiny.Data
{
    public sealed class MobData
    {
        [Flags]
        public enum EMobFlags : ushort
        {
            None = 0 << 0,
            Boss = 1 << 0,
            Undead = 1 << 1,
            Flying = 1 << 2,
            Friendly = 1 << 3,
            Public_Reward = 1 << 4,
            PublicReward = 1 << 4,
            Explosive_Reward = 1 << 5,
            ExplosiveReward = 1 << 5,
            Invincible = 1 << 6,
            Auto_Aggro = 1 << 7,
            AutoAggro = 1 << 7,
            Damaged_By_Normal_Attacks_only = 1 << 8,
            DamagedByNormalAttacksOnly = 1 << 8,
            No_Remove_On_Death = 1 << 9,
            NoRemoveOnDeath = 1 << 9,
            Cannot_Damage_Player = 1 << 10,
            CannotDamagePlayer = 1 << 10,
            Player_Cannot_Damage = 1 << 11,
            PlayerCannotDamage = 1 << 11
        }

        public enum EMobMagicModifier : byte
        {
            Immune = 1,
            Normal = 2,
            Strong = 3,
            Weak = 4
        }

        public sealed class MobAbilityData
        {
            public byte AbilityIdentifier { get; set; }
            public byte AbilityLevel { get; set; }
            public ushort EffectDelay { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write(AbilityIdentifier);
                pWriter.Write(AbilityLevel);
                pWriter.Write(EffectDelay);
            }

            public void Load(BinaryReader pReader)
            {
                AbilityIdentifier = pReader.ReadByte();
                AbilityLevel = pReader.ReadByte();
                EffectDelay = pReader.ReadUInt16();
            }
        }

        public sealed class MobAttackData
        {
            [Flags]
            public enum EMobAttackFlags : byte
            {
                None = 0 << 0,
                Deadly = 1 << 0,
                Magic = 2 << 0,
                Knockback = 3 << 0,
                Jumpable = 4 << 0
            }


            public EMobAttackFlags Flags { get; set; }
            public byte MPCost { get; set; }
            public ushort MPBurn { get; set; }
            public byte AbilityIdentifier { get; set; }
            public byte AbilityLevel { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write((byte)Flags);
                pWriter.Write(MPCost);
                pWriter.Write(MPBurn);
                pWriter.Write(AbilityIdentifier);
                pWriter.Write(AbilityLevel);
            }

            public void Load(BinaryReader pReader)
            {
                Flags = (EMobAttackFlags)pReader.ReadByte();
                MPCost = pReader.ReadByte();
                MPBurn = pReader.ReadUInt16();
                AbilityIdentifier = pReader.ReadByte();
                AbilityLevel = pReader.ReadByte();
            }
        }

        public sealed class MobDropData
        {
            [Flags]
            public enum EMobDropFlags
            {
                None = 0 << 0,
                Is_Mesos = 1 << 0,
                IsMesos = 1 << 0
            }

            public EMobDropFlags Flags { get; set; }
            public int ItemIdentifier { get; set; }
            public int Minimum { get; set; }
            public int Maximum { get; set; }
            public ushort QuestIdentifier { get; set; }
            public int Chance { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write((byte)Flags);
                pWriter.Write(ItemIdentifier);
                pWriter.Write(Minimum);
                pWriter.Write(Maximum);
                pWriter.Write(QuestIdentifier);
                pWriter.Write(Chance);
            }

            public void Load(BinaryReader pReader)
            {
                Flags = (EMobDropFlags)pReader.ReadByte();
                ItemIdentifier = pReader.ReadInt32();
                Minimum = pReader.ReadInt32();
                Maximum = pReader.ReadInt32();
                QuestIdentifier = pReader.ReadUInt16();
                Chance = pReader.ReadInt32();
            }
        }


        public int Identifier { get; set; }
        public EMobFlags Flags { get; set; }
        public byte Level { get; set; }
        public int HP { get; set; }
        public int MP { get; set; }
        public int HPRecovery { get; set; }
        public ushort MPRecovery { get; set; }
        public int ExplodeHP { get; set; }
        public int Experience { get; set; }
        public int LinkIdentifier { get; set; }
        public byte SummonType { get; set; }
        public int Knockback { get; set; }
        public ushort FixedDamage { get; set; }
        public int DeathBuffIdentifier { get; set; }
        public int DeathAfter { get; set; }
        public byte Traction { get; set; }
        public int DamagedBySkillIdentifierOnly { get; set; }
        public int DamagedByMobIdentifierOnly { get; set; }
        public byte HPBarColor { get; set; }
        public byte HPBarBackgroundColor { get; set; }
        public byte CarnivalPoints { get; set; }
        public ushort PhysicalAttack { get; set; }
        public uint PhysicalDefense { get; set; }
        public ushort MagicalAttack { get; set; }
        public uint MagicalDefense { get; set; }
        public short Accuracy { get; set; }
        public ushort Avoidance { get; set; }
        public short Speed { get; set; }
        public short ChaseSpeed { get; set; }
        public EMobMagicModifier IceModifier { get; set; }
        public EMobMagicModifier FireModifier { get; set; }
        public EMobMagicModifier PoisonModifier { get; set; }
        public EMobMagicModifier LightningModifier { get; set; }
        public EMobMagicModifier HolyModifier { get; set; }
        public EMobMagicModifier NonElementalModifier { get; set; }
        public List<MobAbilityData> Abilities { get; set; }
        public List<MobAttackData> Attacks { get; set; }
        public List<int> Summons { get; set; }
        public List<MobDropData> Drops { get; set; }

        public void Save(BinaryWriter pWriter)
        {
            pWriter.Write(Identifier);
            pWriter.Write((ushort)Flags);
            pWriter.Write(Level);
            pWriter.Write(HP);
            pWriter.Write(MP);
            pWriter.Write(HPRecovery);
            pWriter.Write(MPRecovery);
            pWriter.Write(ExplodeHP);
            pWriter.Write(Experience);
            pWriter.Write(LinkIdentifier);
            pWriter.Write(SummonType);
            pWriter.Write(Knockback);
            pWriter.Write(FixedDamage);
            pWriter.Write(DeathBuffIdentifier);
            pWriter.Write(DeathAfter);
            pWriter.Write(Traction);
            pWriter.Write(DamagedBySkillIdentifierOnly);
            pWriter.Write(DamagedByMobIdentifierOnly);
            pWriter.Write(HPBarColor);
            pWriter.Write(HPBarBackgroundColor);
            pWriter.Write(CarnivalPoints);
            pWriter.Write(PhysicalAttack);
            pWriter.Write(PhysicalDefense);
            pWriter.Write(MagicalAttack);
            pWriter.Write(MagicalDefense);
            pWriter.Write(Accuracy);
            pWriter.Write(Avoidance);
            pWriter.Write(Speed);
            pWriter.Write(ChaseSpeed);
            pWriter.Write((byte)IceModifier);
            pWriter.Write((byte)FireModifier);
            pWriter.Write((byte)PoisonModifier);
            pWriter.Write((byte)LightningModifier);
            pWriter.Write((byte)HolyModifier);
            pWriter.Write((byte)NonElementalModifier);

            pWriter.Write(Abilities.Count);
            Abilities.ForEach(a => a.Save(pWriter));

            pWriter.Write(Attacks.Count);
            Attacks.ForEach(a => a.Save(pWriter));

            pWriter.Write(Summons.Count);
            Summons.ForEach(s => pWriter.Write(s));

            pWriter.Write(Drops.Count);
            Drops.ForEach(d => d.Save(pWriter));
        }

        public void Load(BinaryReader pReader)
        {
            Identifier = pReader.ReadInt32();
            Flags = (EMobFlags)pReader.ReadUInt16();
            Level = pReader.ReadByte();
            HP = pReader.ReadInt32();
            MP = pReader.ReadInt32();
            HPRecovery = pReader.ReadInt32();
            MPRecovery = pReader.ReadUInt16();
            ExplodeHP = pReader.ReadInt32();
            Experience = pReader.ReadInt32();
            LinkIdentifier = pReader.ReadInt32();
            SummonType = pReader.ReadByte();
            Knockback = pReader.ReadInt32();
            FixedDamage = pReader.ReadUInt16();
            DeathBuffIdentifier = pReader.ReadInt32();
            DeathAfter = pReader.ReadInt32();
            Traction = pReader.ReadByte();
            DamagedBySkillIdentifierOnly = pReader.ReadInt32();
            DamagedByMobIdentifierOnly = pReader.ReadInt32();
            HPBarColor = pReader.ReadByte();
            HPBarBackgroundColor = pReader.ReadByte();
            CarnivalPoints = pReader.ReadByte();
            PhysicalAttack = pReader.ReadUInt16();
            PhysicalDefense = pReader.ReadUInt32();
            MagicalAttack = pReader.ReadUInt16();
            MagicalDefense = pReader.ReadUInt32();
            Accuracy = pReader.ReadInt16();
            Avoidance = pReader.ReadUInt16();
            Speed = pReader.ReadInt16();
            ChaseSpeed = pReader.ReadInt16();
            IceModifier = (EMobMagicModifier)pReader.ReadByte();
            FireModifier = (EMobMagicModifier)pReader.ReadByte();
            PoisonModifier = (EMobMagicModifier)pReader.ReadByte();
            LightningModifier = (EMobMagicModifier)pReader.ReadByte();
            HolyModifier = (EMobMagicModifier)pReader.ReadByte();
            NonElementalModifier = (EMobMagicModifier)pReader.ReadByte();

            int abilitiesCount = pReader.ReadInt32();
            Abilities = new List<MobAbilityData>(abilitiesCount);
            while (abilitiesCount-- > 0)
            {
                MobAbilityData ability = new MobAbilityData();
                ability.Load(pReader);
                Abilities.Add(ability);
            }

            int attacksCount = pReader.ReadInt32();
            Attacks = new List<MobAttackData>(attacksCount);
            while (attacksCount-- > 0)
            {
                MobAttackData attack = new MobAttackData();
                attack.Load(pReader);
                Attacks.Add(attack);
            }

            int summonsCount = pReader.ReadInt32();
            Summons = new List<int>(summonsCount);
            while (summonsCount-- > 0) Summons.Add(pReader.ReadInt32());

            int dropsCount = pReader.ReadInt32();
            Drops = new List<MobDropData>(dropsCount);
            while (dropsCount-- > 0)
            {
                MobDropData drop = new MobDropData();
                drop.Load(pReader);
                Drops.Add(drop);
            }
        }
    }
}
