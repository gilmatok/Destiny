using System;
using System.Collections.Generic;
using System.IO;

namespace Destiny.Data
{
    public sealed class ItemData
    {
        public static ItemKind GetType(int pItemIdentifier) { return (ItemKind)(pItemIdentifier / 10000); }
        public static bool IsRechargeable(ItemKind pType) { return pType == ItemKind.Star || pType == ItemKind.Bullet; }

        public static ushort ReduceCardIdentifier(int pCardIdentifier) { return (ushort)(pCardIdentifier % 10000); }

        [Flags]
        public enum EItemFlags : ushort
        {
            None = 0 << 0,
            Time_Limited = 1 << 0,
            TimeLimited = 1 << 0,
            No_Trade = 1 << 1,
            NoTrade = 1 << 1,
            No_Sale = 1 << 2,
            NoSale = 1 << 2,
            Karma_Scissorable = 1 << 3,
            KarmaScissorable = 1 << 3,
            Expire_On_Logout = 1 << 4,
            ExpireOnLogout = 1 << 4,
            Block_Pickup = 1 << 5,
            BlockPickup = 1 << 5,
            Quest = 1 << 6,
            Cash_Item = 1 << 7,
            Cash = 1 << 7,
            Party_Quest = 1 << 8,
            PartyQuest = 1 << 8
        }

        public sealed class ItemEquipmentData
        {
            [Flags]
            public enum EItemEquipmentFlags : ushort
            {
                None = 0 << 0,
                Wear_Trade_Block = 1 << 0,
                WearTradeBlock = 1 << 0,
                Pet_Big_Loot_Range = 1 << 1,
                PetBigLootRange = 1 << 1,
                Pet_Auto_HP = 1 << 2,
                PetAutoHP = 1 << 2,
                Pet_Auto_MP = 1 << 3,
                PetAutoMP = 1 << 3,
                Pet_Sweep_For_Drops = 1 << 4,
                PetSweepForDrops = 1 << 4,
                Pet_Loot_Money = 1 << 5,
                PetLootMoney = 1 << 5,
                Pet_Loot_Items = 1 << 6,
                PetLootItems = 1 << 6,
                Pet_Loot_Others = 1 << 7,
                PetLootOthers = 1 << 7,
                Pet_Loot_Ignore = 1 << 8,
                PetLootIgnore = 1 << 8
            }

            [Flags]
            public enum EItemEquipmentSlots : ulong
            {
                None = (ulong)0 << 0,
                Helmet = (ulong)1 << 0,
                Face_Accessory = (ulong)1 << 1,
                FaceAccessory = (ulong)1 << 1,
                Eye_Accessory = (ulong)1 << 2,
                EyeAccessory = (ulong)1 << 2,
                Earring = (ulong)1 << 3,
                Top = (ulong)1 << 4,
                Bottom = (ulong)1 << 5,
                Shoe = (ulong)1 << 6,
                Glove = (ulong)1 << 7,
                Cape = (ulong)1 << 8,
                Shield = (ulong)1 << 9,
                Weapon = (ulong)1 << 10,
                Ring_1 = (ulong)1 << 11,
                Ring1 = (ulong)1 << 11,
                Ring_2 = (ulong)1 << 12,
                Ring2 = (ulong)1 << 12,
                Pet_Equip_1 = (ulong)1 << 13,
                PetEquip1 = (ulong)1 << 13,
                Ring_3 = (ulong)1 << 14,
                Ring3 = (ulong)1 << 14,
                Ring_4 = (ulong)1 << 15,
                Ring4 = (ulong)1 << 15,
                Pendant = (ulong)1 << 16,
                Taming_Mob = (ulong)1 << 17,
                TamingMob = (ulong)1 << 17,
                Saddle = (ulong)1 << 18,
                Pet_Collar = (ulong)1 << 19,
                PetCollar = (ulong)1 << 19,
                Pet_Label_Ring_1 = (ulong)1 << 20,
                PetLabelRing1 = (ulong)1 << 20,
                Pet_Item_Pouch_1 = (ulong)1 << 21,
                PetItemPouch1 = (ulong)1 << 21,
                Pet_Meso_Magnet_1 = (ulong)1 << 22,
                PetMesoMagnet1 = (ulong)1 << 22,
                Pet_Auto_HP = (ulong)1 << 23,
                PetAutoHP = (ulong)1 << 23,
                Pet_Auto_MP = (ulong)1 << 24,
                PetAutoMP = (ulong)1 << 24,
                Pet_Wing_Boots_1 = (ulong)1 << 25,
                PetWingBoots1 = (ulong)1 << 25,
                Pet_Binoculars_1 = (ulong)1 << 26,
                PetBinoculars1 = (ulong)1 << 26,
                Pet_Magic_Scales_1 = (ulong)1 << 27,
                PetMagicScales1 = (ulong)1 << 27,
                Pet_Quote_Ring_1 = (ulong)1 << 28,
                PetQuoteRing1 = (ulong)1 << 28,
                Pet_Equip_2 = (ulong)1 << 29,
                PetEquip2 = (ulong)1 << 29,
                Pet_Label_Ring_2 = (ulong)1 << 30,
                PetLabelRing2 = (ulong)1 << 30,
                Pet_Quote_Ring_2 = (ulong)1 << 31,
                PetQuoteRing2 = (ulong)1 << 31,
                Pet_Item_Pouch_2 = (ulong)1 << 32,
                PetItemPouch2 = (ulong)1 << 32,
                Pet_Meso_Magnet_2 = (ulong)1 << 33,
                PetMesoMagnet2 = (ulong)1 << 33,
                Pet_Wing_Boots_2 = (ulong)1 << 34,
                PetWingBoots2 = (ulong)1 << 34,
                Pet_Binoculars_2 = (ulong)1 << 35,
                PetBinoculars2 = (ulong)1 << 35,
                Pet_Magic_Scales_2 = (ulong)1 << 36,
                PetMagicScales2 = (ulong)1 << 36,
                Pet_Equip_3 = (ulong)1 << 37,
                PetEquip3 = (ulong)1 << 37,
                Pet_Label_Ring_3 = (ulong)1 << 38,
                PetLabelRing3 = (ulong)1 << 38,
                Pet_Quote_Ring_3 = (ulong)1 << 39,
                PetQuoteRing3 = (ulong)1 << 39,
                Pet_Item_Pouch_3 = (ulong)1 << 40,
                PetItemPouch3 = (ulong)1 << 40,
                Pet_Meso_Magnet_3 = (ulong)1 << 41,
                PetMesoMagnet3 = (ulong)1 << 41,
                Pet_Wing_Boots_3 = (ulong)1 << 42,
                PetWingBoots3 = (ulong)1 << 42,
                Pet_Binoculars_3 = (ulong)1 << 43,
                PetBinoculars3 = (ulong)1 << 43,
                Pet_Magic_Scales_3 = (ulong)1 << 44,
                PetMagicScales3 = (ulong)1 << 44,
                Pet_Item_Ignore_1 = (ulong)1 << 45,
                PetItemIgnore1 = (ulong)1 << 45,
                Pet_Item_Ignore_2 = (ulong)1 << 46,
                PetItemIgnore2 = (ulong)1 << 46,
                Pet_Item_Ignore_3 = (ulong)1 << 47,
                PetItemIgnore3 = (ulong)1 << 47,
                Medal = (ulong)1 << 48,
                Belt = (ulong)1 << 49
            }

            [Flags]
            public enum EItemEquipmentJobFlags : byte
            {
                None = 0 << 0,
                Beginner = 1 << 0,
                Warrior = 1 << 1,
                Magician = 1 << 2,
                Bowman = 1 << 3,
                Thief = 1 << 4,
                Pirate = 1 << 5
            }

            public sealed class ItemEquipmentTimelessLevelData
            {
                public byte Level { get; set; }
                public byte Experience { get; set; }
                public byte MinStrength { get; set; }
                public byte MaxStrength { get; set; }
                public byte MinDexterity { get; set; }
                public byte MaxDexterity { get; set; }
                public byte MinIntellect { get; set; }
                public byte MaxIntellect { get; set; }
                public byte MinLuck { get; set; }
                public byte MaxLuck { get; set; }
                public byte MinSpeed { get; set; }
                public byte MaxSpeed { get; set; }
                public byte MinJump { get; set; }
                public byte MaxJump { get; set; }
                public byte MinWeaponAttack { get; set; }
                public byte MaxWeaponAttack { get; set; }
                public byte MinWeaponDefense { get; set; }
                public byte MaxWeaponDefense { get; set; }
                public byte MinMagicAttack { get; set; }
                public byte MaxMagicAttack { get; set; }
                public byte MinMagicDefense { get; set; }
                public byte MaxMagicDefense { get; set; }
                public byte MinHP { get; set; }
                public byte MaxHP { get; set; }
                public byte MinMP { get; set; }
                public byte MaxMP { get; set; }
                public byte MinAccuracy { get; set; }
                public byte MaxAccuracy { get; set; }
                public byte MinAvoidance { get; set; }
                public byte MaxAvoidance { get; set; }

                public void Save(BinaryWriter pWriter)
                {
                    pWriter.Write(Level);
                    pWriter.Write(Experience);
                    pWriter.Write(MinStrength);
                    pWriter.Write(MaxStrength);
                    pWriter.Write(MinDexterity);
                    pWriter.Write(MaxDexterity);
                    pWriter.Write(MinIntellect);
                    pWriter.Write(MaxIntellect);
                    pWriter.Write(MinLuck);
                    pWriter.Write(MaxLuck);
                    pWriter.Write(MinSpeed);
                    pWriter.Write(MaxSpeed);
                    pWriter.Write(MinJump);
                    pWriter.Write(MaxJump);
                    pWriter.Write(MinWeaponAttack);
                    pWriter.Write(MaxWeaponAttack);
                    pWriter.Write(MinWeaponDefense);
                    pWriter.Write(MaxWeaponDefense);
                    pWriter.Write(MinMagicAttack);
                    pWriter.Write(MaxMagicAttack);
                    pWriter.Write(MinMagicDefense);
                    pWriter.Write(MaxMagicDefense);
                    pWriter.Write(MinHP);
                    pWriter.Write(MaxHP);
                    pWriter.Write(MinMP);
                    pWriter.Write(MaxMP);
                    pWriter.Write(MinAccuracy);
                    pWriter.Write(MaxAccuracy);
                    pWriter.Write(MinAvoidance);
                    pWriter.Write(MaxAvoidance);
                }

                public void Load(BinaryReader pReader)
                {
                    Level = pReader.ReadByte();
                    Experience = pReader.ReadByte();
                    MinStrength = pReader.ReadByte();
                    MaxStrength = pReader.ReadByte();
                    MinDexterity = pReader.ReadByte();
                    MaxDexterity = pReader.ReadByte();
                    MinIntellect = pReader.ReadByte();
                    MaxIntellect = pReader.ReadByte();
                    MinLuck = pReader.ReadByte();
                    MaxLuck = pReader.ReadByte();
                    MinSpeed = pReader.ReadByte();
                    MaxSpeed = pReader.ReadByte();
                    MinJump = pReader.ReadByte();
                    MaxJump = pReader.ReadByte();
                    MinWeaponAttack = pReader.ReadByte();
                    MaxWeaponAttack = pReader.ReadByte();
                    MinWeaponDefense = pReader.ReadByte();
                    MaxWeaponDefense = pReader.ReadByte();
                    MinMagicAttack = pReader.ReadByte();
                    MaxMagicAttack = pReader.ReadByte();
                    MinMagicDefense = pReader.ReadByte();
                    MaxMagicDefense = pReader.ReadByte();
                    MinHP = pReader.ReadByte();
                    MaxHP = pReader.ReadByte();
                    MinMP = pReader.ReadByte();
                    MaxMP = pReader.ReadByte();
                    MinAccuracy = pReader.ReadByte();
                    MaxAccuracy = pReader.ReadByte();
                    MinAvoidance = pReader.ReadByte();
                    MaxAvoidance = pReader.ReadByte();
                }
            }

            public sealed class ItemEquipmentTimelessSkillData
            {
                public byte Level { get; set; }
                public int SkillIdentifier { get; set; }
                public byte SkillLevel { get; set; }
                public byte Probability { get; set; }

                public void Save(BinaryWriter pWriter)
                {
                    pWriter.Write(Level);
                    pWriter.Write(SkillIdentifier);
                    pWriter.Write(SkillLevel);
                    pWriter.Write(Probability);
                }

                public void Load(BinaryReader pReader)
                {
                    Level = pReader.ReadByte();
                    SkillIdentifier = pReader.ReadInt32();
                    SkillLevel = pReader.ReadByte();
                    Probability = pReader.ReadByte();
                }
            }


            public EItemEquipmentFlags Flags { get; set; }
            public EItemEquipmentSlots Slots { get; set; }
            public byte AttackSpeed { get; set; }
            public byte HealHP { get; set; }
            public byte Scrolls { get; set; }
            public ushort RequiredStrength { get; set; }
            public ushort RequiredDexterity { get; set; }
            public ushort RequiredIntellect { get; set; }
            public ushort RequiredLuck { get; set; }
            public byte RequiredFame { get; set; }
            public EItemEquipmentJobFlags RequiredJob { get; set; }
            public ushort HP { get; set; }
            public ushort MP { get; set; }
            public ushort Strength { get; set; }
            public ushort Dexterity { get; set; }
            public ushort Intellect { get; set; }
            public ushort Luck { get; set; }
            public byte Hands { get; set; }
            public byte WeaponAttack { get; set; }
            public byte MagicAttack { get; set; }
            public byte WeaponDefense { get; set; }
            public byte MagicDefense { get; set; }
            public byte Accuracy { get; set; }
            public byte Avoidance { get; set; }
            public byte Speed { get; set; }
            public byte Jump { get; set; }
            public byte Traction { get; set; }
            public byte TamingMob { get; set; }
            public byte IceDamage { get; set; }
            public byte FireDamage { get; set; }
            public byte PoisonDamage { get; set; }
            public byte LightningDamage { get; set; }
            public byte ElementalDefault { get; set; }
            public List<ItemEquipmentTimelessLevelData> TimelessLevels { get; set; }
            public List<ItemEquipmentTimelessSkillData> TimelessSkills { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write((ushort)Flags);
                pWriter.Write((ulong)Slots);
                pWriter.Write(AttackSpeed);
                pWriter.Write(HealHP);
                pWriter.Write(Scrolls);
                pWriter.Write(RequiredStrength);
                pWriter.Write(RequiredDexterity);
                pWriter.Write(RequiredIntellect);
                pWriter.Write(RequiredLuck);
                pWriter.Write(RequiredFame);
                pWriter.Write((byte)RequiredJob);
                pWriter.Write(HP);
                pWriter.Write(MP);
                pWriter.Write(Strength);
                pWriter.Write(Dexterity);
                pWriter.Write(Intellect);
                pWriter.Write(Luck);
                pWriter.Write(Hands);
                pWriter.Write(WeaponAttack);
                pWriter.Write(MagicAttack);
                pWriter.Write(WeaponDefense);
                pWriter.Write(MagicDefense);
                pWriter.Write(Accuracy);
                pWriter.Write(Avoidance);
                pWriter.Write(Speed);
                pWriter.Write(Jump);
                pWriter.Write(Traction);
                pWriter.Write(TamingMob);
                pWriter.Write(IceDamage);
                pWriter.Write(FireDamage);
                pWriter.Write(PoisonDamage);
                pWriter.Write(LightningDamage);
                pWriter.Write(ElementalDefault);

                pWriter.Write(TimelessLevels.Count);
                TimelessLevels.ForEach(l => l.Save(pWriter));

                pWriter.Write(TimelessSkills.Count);
                TimelessSkills.ForEach(s => s.Save(pWriter));
            }

            public void Load(BinaryReader pReader)
            {
                Flags = (EItemEquipmentFlags)pReader.ReadUInt16();
                Slots = (EItemEquipmentSlots)pReader.ReadUInt64();
                AttackSpeed = pReader.ReadByte();
                HealHP = pReader.ReadByte();
                Scrolls = pReader.ReadByte();
                RequiredStrength = pReader.ReadUInt16();
                RequiredDexterity = pReader.ReadUInt16();
                RequiredIntellect = pReader.ReadUInt16();
                RequiredLuck = pReader.ReadUInt16();
                RequiredFame = pReader.ReadByte();
                RequiredJob = (EItemEquipmentJobFlags)pReader.ReadByte();
                HP = pReader.ReadUInt16();
                MP = pReader.ReadUInt16();
                Strength = pReader.ReadUInt16();
                Dexterity = pReader.ReadUInt16();
                Intellect = pReader.ReadUInt16();
                Luck = pReader.ReadUInt16();
                Hands = pReader.ReadByte();
                WeaponAttack = pReader.ReadByte();
                MagicAttack = pReader.ReadByte();
                WeaponDefense = pReader.ReadByte();
                MagicDefense = pReader.ReadByte();
                Accuracy = pReader.ReadByte();
                Avoidance = pReader.ReadByte();
                Speed = pReader.ReadByte();
                Jump = pReader.ReadByte();
                Traction = pReader.ReadByte();
                TamingMob = pReader.ReadByte();
                IceDamage = pReader.ReadByte();
                FireDamage = pReader.ReadByte();
                PoisonDamage = pReader.ReadByte();
                LightningDamage = pReader.ReadByte();
                ElementalDefault = pReader.ReadByte();

                int levelsCount = pReader.ReadInt32();
                TimelessLevels = new List<ItemEquipmentTimelessLevelData>(levelsCount);
                while (levelsCount-- > 0)
                {
                    ItemEquipmentTimelessLevelData level = new ItemEquipmentTimelessLevelData();
                    level.Load(pReader);
                    TimelessLevels.Add(level);
                }

                int skillsCount = pReader.ReadInt32();
                TimelessSkills = new List<ItemEquipmentTimelessSkillData>(skillsCount);
                while (skillsCount-- > 0)
                {
                    ItemEquipmentTimelessSkillData skill = new ItemEquipmentTimelessSkillData();
                    skill.Load(pReader);
                    TimelessSkills.Add(skill);
                }
            }
        }

        public sealed class ItemConsumeData
        {
            [Flags]
            public enum EItemConsumeFlags : uint
            {
                None = 0 << 0,
                Auto_Consume = 1 << 0,
                AutoConsume = 1 << 0,
                No_Mouse_Cancel = 1 << 1,
                NoMouseCancel = 1 << 1,
                Ignore_Continent = 1 << 2,
                IgnoreContinent = 1 << 2,
                Party_Item = 1 << 3,
                PartyItem = 1 << 3,
                Ghost = 1 << 4,
                Barrier = 1 << 5,
                Prevent_Drowning = 1 << 6,
                PreventDrowning = 1 << 6,
                Prevent_Freezing = 1 << 7,
                PreventFreezing = 1 << 7,
                Override_Traction = 1 << 8,
                OverrideTraction = 1 << 8,
                Cure_Darkness = 1 << 9,
                CureDarkness = 1 << 9,
                Cure_Poison = 1 << 10,
                CurePoison = 1 << 10,
                Cure_Curse = 1 << 11,
                CureCurse = 1 << 11,
                Cure_Seal = 1 << 12,
                CureSeal = 1 << 12,
                Cure_Weakness = 1 << 13,
                CureWeakness = 1 << 13,
                Meso_Up = 1 << 14,
                MesoUp = 1 << 14,
                Drop_Up_For_Party = 1 << 15,
                DropUpForParty = 1 << 15,
                Ignore_Physical_Defense = 1 << 16,
                IgnorePhysicalDefense = 1 << 16,
                Ignore_Magic_Defense = 1 << 17,
                IgnoreMagicDefense = 1 << 17
            }

            public enum EItemConsumeLootIncrease : byte
            {
                None = 0,
                All_Items = 1,
                All = 1,
                Specific_Item = 2,
                Specific = 2,
                Item_Range = 3,
                Range = 3
            }

            public EItemConsumeFlags Flags { get; set; }
            public byte Effect { get; set; }
            public ushort HP { get; set; }
            public ushort MP { get; set; }
            public short HPPercent { get; set; }
            public short MPPercent { get; set; }
            public int MoveTo { get; set; }
            public byte DecreaseHunger { get; set; }
            public byte DecreaseFatigue { get; set; }
            public byte CarnivalPoints { get; set; }
            public int CreateItem { get; set; }
            public byte Probability { get; set; }
            public ushort Time { get; set; }
            public short WeaponAttack { get; set; }
            public short MagicAttack { get; set; }
            public short WeaponDefense { get; set; }
            public short MagicDefense { get; set; }
            public short Accuracy { get; set; }
            public byte Avoidance { get; set; }
            public short Speed { get; set; }
            public byte Jump { get; set; }
            public byte Morph { get; set; }
            public EItemConsumeLootIncrease LootIncrease { get; set; }
            public int LootIncreaseItemIdentifier { get; set; }
            public ushort LootIncreaseItemIdentifierRange { get; set; }
            public byte LootIncreaseMapIdentifierRange { get; set; }
            public byte IceDefense { get; set; }
            public byte FireDefense { get; set; }
            public byte PoisonDefense { get; set; }
            public byte LightningDefense { get; set; }
            public byte DarknessDefense { get; set; }
            public byte CurseDefense { get; set; }
            public byte SealDefense { get; set; }
            public byte WeaknessDefense { get; set; }
            public byte StunDefense { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write((uint)Flags);
                pWriter.Write(Effect);
                pWriter.Write(HP);
                pWriter.Write(MP);
                pWriter.Write(HPPercent);
                pWriter.Write(MPPercent);
                pWriter.Write(MoveTo);
                pWriter.Write(DecreaseHunger);
                pWriter.Write(DecreaseFatigue);
                pWriter.Write(CarnivalPoints);
                pWriter.Write(CreateItem);
                pWriter.Write(Probability);
                pWriter.Write(Time);
                pWriter.Write(WeaponAttack);
                pWriter.Write(MagicAttack);
                pWriter.Write(WeaponDefense);
                pWriter.Write(MagicDefense);
                pWriter.Write(Accuracy);
                pWriter.Write(Avoidance);
                pWriter.Write(Speed);
                pWriter.Write(Jump);
                pWriter.Write(Morph);
                pWriter.Write((byte)LootIncrease);
                pWriter.Write(LootIncreaseItemIdentifier);
                pWriter.Write(LootIncreaseItemIdentifierRange);
                pWriter.Write(LootIncreaseMapIdentifierRange);
                pWriter.Write(IceDefense);
                pWriter.Write(FireDefense);
                pWriter.Write(PoisonDefense);
                pWriter.Write(LightningDefense);
                pWriter.Write(DarknessDefense);
                pWriter.Write(CurseDefense);
                pWriter.Write(SealDefense);
                pWriter.Write(WeaknessDefense);
                pWriter.Write(StunDefense);
            }

            public void Load(BinaryReader pReader)
            {
                Flags = (EItemConsumeFlags)pReader.ReadUInt32();
                Effect = pReader.ReadByte();
                HP = pReader.ReadUInt16();
                MP = pReader.ReadUInt16();
                HPPercent = pReader.ReadInt16();
                MPPercent = pReader.ReadInt16();
                MoveTo = pReader.ReadInt32();
                DecreaseHunger = pReader.ReadByte();
                DecreaseFatigue = pReader.ReadByte();
                CarnivalPoints = pReader.ReadByte();
                CreateItem = pReader.ReadInt32();
                Probability = pReader.ReadByte();
                Time = pReader.ReadUInt16();
                WeaponAttack = pReader.ReadInt16();
                MagicAttack = pReader.ReadInt16();
                WeaponDefense = pReader.ReadInt16();
                MagicDefense = pReader.ReadInt16();
                Accuracy = pReader.ReadInt16();
                Avoidance = pReader.ReadByte();
                Speed = pReader.ReadInt16();
                Jump = pReader.ReadByte();
                Morph = pReader.ReadByte();
                LootIncrease = (EItemConsumeLootIncrease)pReader.ReadByte();
                LootIncreaseItemIdentifier = pReader.ReadInt32();
                LootIncreaseItemIdentifierRange = pReader.ReadUInt16();
                LootIncreaseMapIdentifierRange = pReader.ReadByte();
                IceDefense = pReader.ReadByte();
                FireDefense = pReader.ReadByte();
                PoisonDefense = pReader.ReadByte();
                LightningDefense = pReader.ReadByte();
                DarknessDefense = pReader.ReadByte();
                CurseDefense = pReader.ReadByte();
                SealDefense = pReader.ReadByte();
                WeaknessDefense = pReader.ReadByte();
                StunDefense = pReader.ReadByte();
            }
        }

        public sealed class ItemCardData
        {
            public int StartMapIdentifier { get; set; }
            public int EndMapIdentifier { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write(StartMapIdentifier);
                pWriter.Write(EndMapIdentifier);
            }

            public void Load(BinaryReader pReader)
            {
                StartMapIdentifier = pReader.ReadInt32();
                EndMapIdentifier = pReader.ReadInt32();
            }
        }

        public sealed class ItemPetData
        {
            [Flags]
            public enum EItemPetFlags : byte
            {
                None = 0 << 0,
                No_Revive = 1 << 0,
                NoRevive = 1 << 0,
                No_Move_To_Cash_Shop = 1 << 1,
                NoMoveToCashShop = 1 << 1,
                Auto_React = 1 << 2,
                AutoReact = 1 << 2
            }

            public sealed class ItemPetEvolutionData
            {
                public int NextEvolutionItemIdentifier { get; set; }
                public ushort Chance { get; set; }

                public void Save(BinaryWriter pWriter)
                {
                    pWriter.Write(NextEvolutionItemIdentifier);
                    pWriter.Write(Chance);
                }

                public void Load(BinaryReader pReader)
                {
                    NextEvolutionItemIdentifier = pReader.ReadInt32();
                    Chance = pReader.ReadUInt16();
                }
            }

            public sealed class ItemPetInteractionData
            {
                public byte Closeness { get; set; }
                public byte Success { get; set; }

                public void Save(BinaryWriter pWriter)
                {
                    pWriter.Write(Closeness);
                    pWriter.Write(Success);
                }

                public void Load(BinaryReader pReader)
                {
                    Closeness = pReader.ReadByte();
                    Success = pReader.ReadByte();
                }
            }


            public EItemPetFlags Flags { get; set; }
            public byte Hunger { get; set; }
            public byte Life { get; set; }
            public ushort LimitedLife { get; set; }
            public int EvolutionItemIdentifier { get; set; }
            public byte RequiredLevelToEvolve { get; set; }
            public List<ItemPetEvolutionData> Evolutions { get; set; }
            public List<ItemPetInteractionData> Interactions { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write((byte)Flags);
                pWriter.Write(Hunger);
                pWriter.Write(Life);
                pWriter.Write(LimitedLife);
                pWriter.Write(EvolutionItemIdentifier);
                pWriter.Write(RequiredLevelToEvolve);

                pWriter.Write(Evolutions.Count);
                Evolutions.ForEach(e => e.Save(pWriter));

                pWriter.Write(Interactions.Count);
                Interactions.ForEach(i => i.Save(pWriter));
            }

            public void Load(BinaryReader pReader)
            {
                Flags = (EItemPetFlags)pReader.ReadByte();
                Hunger = pReader.ReadByte();
                Life = pReader.ReadByte();
                LimitedLife = pReader.ReadUInt16();
                EvolutionItemIdentifier = pReader.ReadInt32();
                RequiredLevelToEvolve = pReader.ReadByte();

                int evolutionsCount = pReader.ReadInt32();
                Evolutions = new List<ItemPetEvolutionData>(evolutionsCount);
                while (evolutionsCount-- > 0)
                {
                    ItemPetEvolutionData evolution = new ItemPetEvolutionData();
                    evolution.Load(pReader);
                    Evolutions.Add(evolution);
                }

                int interactionsCount = pReader.ReadInt32();
                Interactions = new List<ItemPetInteractionData>(interactionsCount);
                while (interactionsCount-- > 0)
                {
                    ItemPetInteractionData interaction = new ItemPetInteractionData();
                    interaction.Load(pReader);
                    Interactions.Add(interaction);
                }
            }
        }

        public sealed class ItemRechargeData
        {
            public float Price { get; set; }
            public byte WeaponAttack { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write(Price);
                pWriter.Write(WeaponAttack);
            }

            public void Load(BinaryReader pReader)
            {
                Price = pReader.ReadSingle();
                WeaponAttack = pReader.ReadByte();
            }
        }

        public sealed class ItemScrollData
        {
            [Flags]
            public enum EItemScrollFlags : byte
            {
                None = 0 << 0,
                Rand_Stat = 1 << 0,
                RandStat = 1 << 0,
                Recover_Slot = 1 << 1,
                RecoverSlot = 1 << 1,
                Warm_Support = 1 << 2,
                WarmSupport = 1 << 2,
                Prevent_Slip = 1 << 3,
                PreventSlip = 1 << 3
            }


            public EItemScrollFlags Flags { get; set; }
            public byte Success { get; set; }
            public byte BreakItem { get; set; }
            public byte Strength { get; set; }
            public byte Dexterity { get; set; }
            public byte Intellect { get; set; }
            public byte Luck { get; set; }
            public byte HP { get; set; }
            public byte MP { get; set; }
            public byte WeaponAttack { get; set; }
            public byte MagicAttack { get; set; }
            public byte WeaponDefense { get; set; }
            public byte MagicDefense { get; set; }
            public byte Accuracy { get; set; }
            public byte Avoidance { get; set; }
            public byte Speed { get; set; }
            public byte Jump { get; set; }
            public List<int> Targets { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write((byte)Flags);
                pWriter.Write(Success);
                pWriter.Write(BreakItem);
                pWriter.Write(Strength);
                pWriter.Write(Dexterity);
                pWriter.Write(Intellect);
                pWriter.Write(Luck);
                pWriter.Write(HP);
                pWriter.Write(MP);
                pWriter.Write(WeaponAttack);
                pWriter.Write(MagicAttack);
                pWriter.Write(WeaponDefense);
                pWriter.Write(MagicDefense);
                pWriter.Write(Accuracy);
                pWriter.Write(Avoidance);
                pWriter.Write(Speed);
                pWriter.Write(Jump);

                pWriter.Write(Targets.Count);
                foreach (int itemIdentifier in Targets) pWriter.Write(itemIdentifier);
            }

            public void Load(BinaryReader pReader)
            {
                Flags = (EItemScrollFlags)pReader.ReadByte();
                Success = pReader.ReadByte();
                BreakItem = pReader.ReadByte();
                Strength = pReader.ReadByte();
                Dexterity = pReader.ReadByte();
                Intellect = pReader.ReadByte();
                Luck = pReader.ReadByte();
                HP = pReader.ReadByte();
                MP = pReader.ReadByte();
                WeaponAttack = pReader.ReadByte();
                MagicAttack = pReader.ReadByte();
                WeaponDefense = pReader.ReadByte();
                MagicDefense = pReader.ReadByte();
                Accuracy = pReader.ReadByte();
                Avoidance = pReader.ReadByte();
                Speed = pReader.ReadByte();
                Jump = pReader.ReadByte();

                int targetCount = pReader.ReadInt32();
                Targets = new List<int>(targetCount);
                while (targetCount-- > 0) Targets.Add(pReader.ReadInt32());
            }
        }

        public sealed class ItemMorphData
        {
            public byte Morph { get; set; }
            public byte Success { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write(Morph);
                pWriter.Write(Success);
            }

            public void Load(BinaryReader pReader)
            {
                Morph = pReader.ReadByte();
                Success = pReader.ReadByte();
            }
        }

        public sealed class ItemSkillData
        {
            public int SkillIdentifier { get; set; }
            public byte SkillLevel { get; set; }
            public byte MasterSkillLevel { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write(SkillIdentifier);
                pWriter.Write(SkillLevel);
                pWriter.Write(MasterSkillLevel);
            }

            public void Load(BinaryReader pReader)
            {
                SkillIdentifier = pReader.ReadInt32();
                SkillLevel = pReader.ReadByte();
                MasterSkillLevel = pReader.ReadByte();
            }
        }

        public sealed class ItemSummonData
        {
            public int MobIdentifier { get; set; }
            public byte Chance { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write(MobIdentifier);
                pWriter.Write(Chance);
            }

            public void Load(BinaryReader pReader)
            {
                MobIdentifier = pReader.ReadInt32();
                Chance = pReader.ReadByte();
            }
        }


        public int Identifier { get; set; }
        public EItemFlags Flags { get; set; }
        public int Price { get; set; }
        public ushort MaxSlotQuantity { get; set; }
        public byte MaxPossessionCount { get; set; }
        public byte MinLevel { get; set; }
        public byte MaxLevel { get; set; }
        public int Experience { get; set; }
        public byte MakerLevel { get; set; }
        public int Money { get; set; }
        public int StateChangeItem { get; set; }
        public int NPC { get; set; }
        public ItemEquipmentData Equipment { get; set; }
        public ItemConsumeData Consume { get; set; }
        public ItemCardData Card { get; set; }
        public ItemPetData Pet { get; set; }
        public ItemRechargeData Recharge { get; set; }
        public ItemScrollData Scroll { get; set; }
        public List<ItemMorphData> Morphs { get; set; }
        public List<ItemSkillData> Skills { get; set; }
        public List<ItemSummonData> Summons { get; set; }

        public void Save(BinaryWriter pWriter)
        {
            pWriter.Write(Identifier);
            pWriter.Write((ushort)Flags);
            pWriter.Write(Price);
            pWriter.Write(MaxSlotQuantity);
            pWriter.Write(MaxPossessionCount);
            pWriter.Write(MinLevel);
            pWriter.Write(MaxLevel);
            pWriter.Write(Experience);
            pWriter.Write(MakerLevel);
            pWriter.Write(Money);
            pWriter.Write(StateChangeItem);
            pWriter.Write(NPC);
            pWriter.Write(Equipment != null);
            if (Equipment != null) Equipment.Save(pWriter);
            pWriter.Write(Consume != null);
            if (Consume != null) Consume.Save(pWriter);
            pWriter.Write(Card != null);
            if (Card != null) Card.Save(pWriter);
            pWriter.Write(Pet != null);
            if (Pet != null) Pet.Save(pWriter);
            pWriter.Write(Recharge != null);
            if (Recharge != null) Recharge.Save(pWriter);
            pWriter.Write(Scroll != null);
            if (Scroll != null) Scroll.Save(pWriter);
            pWriter.Write(Morphs.Count);
            Morphs.ForEach(m => m.Save(pWriter));
            pWriter.Write(Skills.Count);
            Skills.ForEach(s => s.Save(pWriter));
            pWriter.Write(Summons.Count);
            Summons.ForEach(s => s.Save(pWriter));
        }

        public void Load(BinaryReader pReader)
        {
            Identifier = pReader.ReadInt32();
            Flags = (EItemFlags)pReader.ReadUInt16();
            Price = pReader.ReadInt32();
            MaxSlotQuantity = pReader.ReadUInt16();
            MaxPossessionCount = pReader.ReadByte();
            MinLevel = pReader.ReadByte();
            MaxLevel = pReader.ReadByte();
            Experience = pReader.ReadInt32();
            MakerLevel = pReader.ReadByte();
            Money = pReader.ReadInt32();
            StateChangeItem = pReader.ReadInt32();
            NPC = pReader.ReadInt32();
            if (pReader.ReadBoolean())
            {
                Equipment = new ItemEquipmentData();
                Equipment.Load(pReader);
            }
            if (pReader.ReadBoolean())
            {
                Consume = new ItemConsumeData();
                Consume.Load(pReader);
            }
            if (pReader.ReadBoolean())
            {
                Card = new ItemCardData();
                Card.Load(pReader);
            }
            if (pReader.ReadBoolean())
            {
                Pet = new ItemPetData();
                Pet.Load(pReader);
            }
            if (pReader.ReadBoolean())
            {
                Recharge = new ItemRechargeData();
                Recharge.Load(pReader);
            }
            if (pReader.ReadBoolean())
            {
                Scroll = new ItemScrollData();
                Scroll.Load(pReader);
            }

            int morphsCount = pReader.ReadInt32();
            Morphs = new List<ItemMorphData>(morphsCount);
            while (morphsCount-- > 0)
            {
                ItemMorphData morph = new ItemMorphData();
                morph.Load(pReader);
                Morphs.Add(morph);
            }

            int skillsCount = pReader.ReadInt32();
            Skills = new List<ItemSkillData>(skillsCount);
            while (skillsCount-- > 0)
            {
                ItemSkillData skill = new ItemSkillData();
                skill.Load(pReader);
                Skills.Add(skill);
            }

            int summonsCount = pReader.ReadInt32();
            Summons = new List<ItemSummonData>(summonsCount);
            while (summonsCount-- > 0)
            {
                ItemSummonData summon = new ItemSummonData();
                summon.Load(pReader);
                Summons.Add(summon);
            }
        }
    }
}
