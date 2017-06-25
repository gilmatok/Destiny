using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using Destiny.Maple.Maps;
using Destiny.Utility;
using System;

namespace Destiny.Maple
{
    public sealed class Item : Drop
    {
        public static ItemType GetType(int mapleID)
        {
            return (ItemType)(mapleID / 1000000);
        }

        public CharacterItems Parent { get; set; }

        public int ID { get; private set; }
        public int MapleID { get; private set; }
        public short Slot { get; set; }
        private short maxPerStack;
        private short quantity;
        public string Creator { get; private set; }

        public bool IsCash { get; private set; }
        public bool OnlyOne { get; private set; }
        public bool PreventsSlipping { get; private set; }
        public bool PreventsColdness { get; private set; }
        public bool IsTradeBlocked { get; private set; }
        public bool IsScisored { get; private set; }
        public int SalePrice { get; private set; }

        public byte UpgradesAvailable { get; private set; }
        public byte UpgradesApplied { get; private set; }
        public short Strength { get; private set; }
        public short Dexterity { get; private set; }
        public short Intelligence { get; private set; }
        public short Luck { get; private set; }
        public short Health { get; private set; }
        public short Mana { get; private set; }
        public short WeaponAttack { get; private set; }
        public short MagicAttack { get; private set; }
        public short WeaponDefense { get; private set; }
        public short MagicDefense { get; private set; }
        public short Accuracy { get; private set; }
        public short Avoidability { get; private set; }
        public short Agility { get; private set; }
        public short Speed { get; private set; }
        public short Jump { get; private set; }

        public byte AttackSpeed { get; private set; }
        public short RecoveryRate { get; private set; }
        public short KnockBackChance { get; private set; }

        public short RequiredLevel { get; private set; }
        public short RequiredStrength { get; private set; }
        public short RequiredDexterity { get; private set; }
        public short RequiredIntelligence { get; private set; }
        public short RequiredLuck { get; private set; }
        public short RequiredFame { get; private set; }
        public Job RequiredJob { get; private set; }

        public ItemType Type
        {
            get
            {
                return Item.GetType(this.MapleID);
            }
        }

        public Item CachedReference
        {
            get
            {
                return DataProvider.CachedItems[this.MapleID];
            }
        }

        public Character Character
        {
            get
            {
                return this.Parent.Parent;
            }
        }

        public short MaxPerStack
        {
            get
            {
                if (this.IsRechargeable && this.Parent != null)
                {
                    return maxPerStack;
                }
                else
                {
                    return maxPerStack;
                }
            }
            set
            {
                maxPerStack = value;
            }
        }

        public short Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                if (value > this.MaxPerStack)
                {
                    throw new ArgumentException("Quantity too high.");
                }
                else
                {
                    quantity = value;
                }
            }
        }

        public bool IsSealed
        {
            get
            {
                return DataProvider.CachedItems.WizetItemIDs.Contains(this.MapleID);
            }
        }

        public byte Flags
        {
            get
            {
                byte flags = 0;

                if (this.IsSealed) flags |= (byte)ItemFlags.Sealed;
                if (this.PreventsSlipping) flags |= (byte)ItemFlags.AddPreventSlipping;
                if (this.PreventsColdness) flags |= (byte)ItemFlags.AddPreventColdness;
                if (this.IsScisored) flags |= (byte)ItemFlags.Scisored;
                if (this.IsTradeBlocked) flags |= (byte)ItemFlags.Untradeable;

                return flags;
            }
        }

        public bool IsEquipped
        {
            get
            {
                return this.Slot < 0;
            }
        }

        public bool IsEquippedCash
        {
            get
            {
                return this.Slot < -100;
            }
        }

        public bool IsRechargeable
        {
            get
            {
                return this.IsThrowingStar || this.IsBullet;
            }
        }

        public bool IsThrowingStar
        {
            get
            {
                return this.MapleID / 10000 == 207;
            }
        }

        public bool IsBullet
        {
            get
            {
                return this.MapleID / 10000 == 233;
            }
        }

        public bool IsArrow
        {
            get
            {
                return this.IsArrowForBow || this.IsArrowForCrossbow;
            }
        }

        public bool IsArrowForBow
        {
            get
            {
                return this.MapleID >= 2060000 && this.MapleID < 2061000;
            }
        }

        public bool IsArrowForCrossbow
        {
            get
            {
                return this.MapleID >= 2061000 && this.MapleID < 2062000;
            }
        }

        public byte AbsoluteSlot
        {
            get
            {
                if (this.IsEquipped)
                {
                    return (byte)(this.Slot * -1);
                }
                else
                {
                    throw new InvalidOperationException("Attempting to retrieve absolute slot for non-equipped item.");
                }
            }
        }

        public byte ComputedSlot
        {
            get
            {
                if (this.IsEquippedCash)
                {
                    return ((byte)(this.AbsoluteSlot - 100));
                }
                else if (this.IsEquipped)
                {
                    return this.AbsoluteSlot;
                }
                else
                {
                    return (byte)this.Slot;
                }
            }
        }

        public bool Assigned { get; set; }

        public Item(int mapleID, short quantity = 1, bool equipped = false)
        {
            this.MapleID = mapleID;
            this.MaxPerStack = this.CachedReference.MaxPerStack;
            this.Quantity = (this.Type == ItemType.Equipment) ? (short)1 : quantity;
            if (equipped) this.Slot = this.GetEquippedSlot();
            this.Creator = string.Empty;

            this.IsCash = this.CachedReference.IsCash;
            this.OnlyOne = this.CachedReference.OnlyOne;
            this.IsTradeBlocked = this.CachedReference.IsTradeBlocked;
            this.IsScisored = this.CachedReference.IsScisored;
            this.SalePrice = this.CachedReference.SalePrice;
            this.RequiredLevel = this.CachedReference.RequiredLevel;

            if (this.Type == ItemType.Equipment)
            {
                this.PreventsSlipping = this.CachedReference.PreventsSlipping;
                this.PreventsColdness = this.CachedReference.PreventsColdness;

                this.AttackSpeed = this.CachedReference.AttackSpeed;
                this.RecoveryRate = this.CachedReference.RecoveryRate;
                this.KnockBackChance = this.CachedReference.KnockBackChance;

                this.RequiredStrength = this.CachedReference.RequiredStrength;
                this.RequiredDexterity = this.CachedReference.RequiredDexterity;
                this.RequiredIntelligence = this.CachedReference.RequiredIntelligence;
                this.RequiredLuck = this.CachedReference.RequiredLuck;
                this.RequiredFame = this.CachedReference.RequiredFame;
                this.RequiredJob = this.CachedReference.RequiredJob;

                this.UpgradesAvailable = this.CachedReference.UpgradesAvailable;
                this.UpgradesApplied = this.CachedReference.UpgradesApplied;
                this.Strength = this.CachedReference.Strength;
                this.Dexterity = this.CachedReference.Dexterity;
                this.Intelligence = this.CachedReference.Intelligence;
                this.Luck = this.CachedReference.Luck;
                this.Health = this.CachedReference.Health;
                this.Mana = this.CachedReference.Mana;
                this.WeaponAttack = this.CachedReference.WeaponAttack;
                this.MagicAttack = this.CachedReference.MagicAttack;
                this.WeaponDefense = this.CachedReference.WeaponDefense;
                this.MagicDefense = this.CachedReference.MagicDefense;
                this.Accuracy = this.CachedReference.Accuracy;
                this.Avoidability = this.CachedReference.Avoidability;
                this.Agility = this.CachedReference.Agility;
                this.Speed = this.CachedReference.Speed;
                this.Jump = this.CachedReference.Jump;
            }
        }

        public Item(Datum datum)
        {
            if (DataProvider.IsInitialized)
            {
                this.ID = (int)datum["ID"];
                this.Assigned = true;

                this.MapleID = (int)datum["MapleID"];
                this.MaxPerStack = this.CachedReference.MaxPerStack;
                this.Quantity = (short)datum["Quantity"];
                this.Slot = (short)datum["Slot"];
                this.Creator = (string)datum["Creator"];

                this.IsCash = this.CachedReference.IsCash;
                this.OnlyOne = this.CachedReference.OnlyOne;
                this.IsTradeBlocked = this.CachedReference.IsTradeBlocked;
                this.IsScisored = false;
                this.SalePrice = this.CachedReference.SalePrice;
                this.RequiredLevel = this.CachedReference.RequiredLevel;

                if (this.Type == ItemType.Equipment)
                {
                    this.AttackSpeed = this.CachedReference.AttackSpeed;
                    this.RecoveryRate = this.CachedReference.RecoveryRate;
                    this.KnockBackChance = this.CachedReference.KnockBackChance;

                    this.RequiredStrength = this.CachedReference.RequiredStrength;
                    this.RequiredDexterity = this.CachedReference.RequiredDexterity;
                    this.RequiredIntelligence = this.CachedReference.RequiredIntelligence;
                    this.RequiredLuck = this.CachedReference.RequiredLuck;
                    this.RequiredFame = this.CachedReference.RequiredFame;
                    this.RequiredJob = this.CachedReference.RequiredJob;

                    this.UpgradesAvailable = (byte)datum["UpgradesAvailable"];
                    this.UpgradesApplied = (byte)datum["UpgradesApplied"];
                    this.Strength = (short)datum["Strength"];
                    this.Dexterity = (short)datum["Dexterity"];
                    this.Intelligence = (short)datum["Intelligence"];
                    this.Luck = (short)datum["Luck"];
                    this.Health = (short)datum["Health"];
                    this.Mana = (short)datum["Mana"];
                    this.WeaponAttack = (short)datum["WeaponAttack"];
                    this.MagicAttack = (short)datum["MagicAttack"];
                    this.WeaponDefense = (short)datum["WeaponDefense"];
                    this.MagicDefense = (short)datum["MagicDefense"];
                    this.Accuracy = (short)datum["Accuracy"];
                    this.Avoidability = (short)datum["Avoidability"];
                    this.Agility = (short)datum["Agility"];
                    this.Speed = (short)datum["Speed"];
                    this.Jump = (short)datum["Jump"];
                }
            }
            else
            {
                this.MapleID = (int)datum["itemid"];
                this.MaxPerStack = (short)datum["max_slot_quantity"];

                this.IsCash = ((string)datum["flags"]).Contains("cash_item");
                this.OnlyOne = (sbyte)datum["max_possession_count"] > 0;
                this.IsTradeBlocked = ((string)datum["flags"]).Contains("no_trade");
                this.IsScisored = false;
                this.SalePrice = (int)datum["price"];
                this.RequiredLevel = (byte)datum["min_level"];
            }
        }

        public void Save()
        {

        }

        public void Delete()
        {

        }

        public void Equip()
        {

        }

        public void Unequip(short destinationSlot = 0)
        {

        }

        public void Move(short destinationSlot)
        {

        }

        public void Drop(short quantity)
        {

        }

        public void Encode(OutPacket oPacket, bool zeroPosition = false, bool leaveOut = false)
        {
            if (!zeroPosition && !leaveOut)
            {
                byte slot = this.ComputedSlot;

                if (slot < 0)
                {
                    slot = (byte)(slot * -1);
                }
                else if (slot > 100)
                {
                    slot -= 100;
                }

                if (this.Type == ItemType.Equipment)
                {
                    oPacket.WriteShort(slot);
                }
                else
                {
                    oPacket.WriteByte(slot);
                }
            }

            oPacket
                .WriteByte((byte)(this.Type == ItemType.Equipment ? 1 : 2))
                .WriteInt(this.MapleID)
                .WriteBool(false)
                .WriteLong(); // TODO: Expiration.

            if (this.Type == ItemType.Equipment)
            {
                oPacket
                    .WriteByte(this.UpgradesAvailable)
                    .WriteByte(this.UpgradesApplied)
                    .WriteShort(this.Strength)
                    .WriteShort(this.Dexterity)
                    .WriteShort(this.Intelligence)
                    .WriteShort(this.Luck)
                    .WriteShort(this.Health)
                    .WriteShort(this.Mana)
                    .WriteShort(this.WeaponAttack)
                    .WriteShort(this.MagicAttack)
                    .WriteShort(this.WeaponDefense)
                    .WriteShort(this.MagicDefense)
                    .WriteShort(this.Accuracy)
                    .WriteShort(this.Avoidability)
                    .WriteShort(this.Agility)
                    .WriteShort(this.Speed)
                    .WriteShort(this.Jump)
                    .WriteMapleString(this.Creator)
                    .WriteByte(this.Flags)
                    .WriteByte();

                if (!this.IsEquippedCash)
                {
                    oPacket
                        .WriteByte()
                        .WriteByte()
                        .WriteShort()
                        .WriteShort()
                        .WriteInt()
                        .WriteLong()
                        .WriteLong()
                        .WriteInt(-1);
                }
            }
            else
            {
                oPacket
                    .WriteShort(this.Quantity)
                    .WriteMapleString(this.Creator)
                    .WriteByte(this.Flags)
                    .WriteByte();

                if (this.IsRechargeable)
                {
                    oPacket.WriteLong(); // TODO: Unique ID.
                }
            }
        }

        private short GetEquippedSlot()
        {
            short slot = 0;

            if (this.MapleID >= 1000000 && this.MapleID < 1010000)
            {
                slot -= 1;
            }
            else if (this.MapleID >= 1010000 && this.MapleID < 1020000)
            {
                slot -= 2;
            }
            else if (this.MapleID >= 1020000 && this.MapleID < 1030000)
            {
                slot -= 3;
            }
            else if (this.MapleID >= 1030000 && this.MapleID < 1040000)
            {
                slot -= 4;
            }
            else if (this.MapleID >= 1040000 && this.MapleID < 1060000)
            {
                slot -= 5;
            }
            else if (this.MapleID >= 1060000 && this.MapleID < 1070000)
            {
                slot -= 6;
            }
            else if (this.MapleID >= 1070000 && this.MapleID < 1080000)
            {
                slot -= 7;
            }
            else if (this.MapleID >= 1080000 && this.MapleID < 1090000)
            {
                slot -= 8;
            }
            else if (this.MapleID >= 1102000 && this.MapleID < 1103000)
            {
                slot -= 9;
            }
            else if (this.MapleID >= 1092000 && this.MapleID < 1100000)
            {
                slot -= 10;
            }
            else if (this.MapleID >= 1300000 && this.MapleID < 1800000)
            {
                slot -= 11;
            }
            else if (this.MapleID >= 1112000 && this.MapleID < 1120000)
            {
                slot -= 12;
            }
            else if (this.MapleID >= 1122000 && this.MapleID < 1123000)
            {
                slot -= 17;
            }
            else if (this.MapleID >= 1900000 && this.MapleID < 2000000)
            {
                slot -= 18;
            }

            if (this.IsCash)
            {
                slot -= 100;
            }

            return slot;
        }

        public override OutPacket GetShowGainPacket()
        {
            OutPacket oPacket = new OutPacket(ServerOperationCode.Message);

            oPacket
                .WriteByte()
                .WriteByte()
                .WriteInt(this.MapleID)
                .WriteInt(this.Quantity)
                .WriteInt()
                .WriteInt();

            return oPacket;
        }
    }
}
