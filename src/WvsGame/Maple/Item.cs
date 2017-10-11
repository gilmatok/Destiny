using Destiny.Network;
using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using Destiny.Maple.Maps;
using System;
using System.Collections.Generic;
using Destiny.IO;
using Destiny.Data;

namespace Destiny.Maple
{
    public class Item : Drop
    {
        public static ItemType GetType(int mapleID)
        {
            return (ItemType)(mapleID / 1000000);
        }

        public CharacterItems Parent { get; set; }

        public int ID { get; private set; }
        public int AccountID { get; private set; }
        public int MapleID { get; private set; }
        public short Slot { get; set; }
        private short maxPerStack;
        private short quantity;
        public string Creator { get; set; }
        public DateTime Expiration { get; set; }
        public int? PetID { get; set; }

        public bool IsCash { get; private set; }
        public bool OnlyOne { get; private set; }
        public bool PreventsSlipping { get; private set; }
        public bool PreventsColdness { get; private set; }
        public bool IsTradeBlocked { get; private set; }
        public bool IsScissored { get; private set; }
        public bool IsStored { get; set; }
        public int SalePrice { get; private set; }
        public int Meso { get; private set; }

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

        // Consume data properties are prefixed with 'C'.
        public int CItemId { get; private set; }
        public string CFlags { get; private set; }
        public string CCureAilments { get; private set; }
        public short CEffect { get; private set; }
        public short CHealth { get; private set; }
        public short CMana { get; private set; }
        public short CHealthPercentage { get; private set; }
        public short CManaPercentage { get; private set; }
        public int CMoveTo { get; private set; }
        public short CProb { get; private set; }
        public int CBuffTime { get; private set; }
        public short CWeaponAttack { get; private set; }
        public short CMagicAttack { get; private set; }
        public short CWeaponDefense { get; private set; }
        public short CMagicDefense { get; private set; }
        public short CAccuracy { get; private set; }
        public short CAvoid { get; private set; }
        public short CSpeed { get; private set; }
        public short CJump { get; private set; }
        public short CMorph { get; private set; }

        public List<Tuple<int, short>> Summons { get; private set; }


        public ItemType Type
        {
            get
            {
                return Item.GetType(this.MapleID);
            }
        }

        public WeaponType WeaponType
        {
            get
            {
                switch (this.MapleID / 10000 % 100)
                {
                    case 30:
                        return WeaponType.Sword1H;

                    case 31:
                        return WeaponType.Axe1H;

                    case 32:
                        return WeaponType.Blunt1H;

                    case 33:
                        return WeaponType.Dagger;

                    case 37:
                        return WeaponType.Wand;

                    case 38:
                        return WeaponType.Staff;

                    case 40:
                        return WeaponType.Sword2H;

                    case 41:
                        return WeaponType.Axe2H;

                    case 42:
                        return WeaponType.Blunt2H;

                    case 43:
                        return WeaponType.Spear;

                    case 44:
                        return WeaponType.PoleArm;

                    case 45:
                        return WeaponType.Bow;

                    case 46:
                        return WeaponType.Crossbow;

                    case 47:
                        return WeaponType.Claw;

                    case 48:
                        return WeaponType.Knuckle;

                    case 49:
                        return WeaponType.Gun;

                    default:
                        return WeaponType.NotAWeapon;
                }
            }
        }

        public Item CachedReference
        {
            get
            {
                return DataProvider.Items[this.MapleID];
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
                return DataProvider.Items.WizetItemIDs.Contains(this.MapleID);
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
                if (this.IsScissored) flags |= (byte)ItemFlags.Scissored;
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

        public bool IsConsumable
        {
            get
            {
                return this.MapleID / 10000 >= 200 && this.MapleID / 10000 < 204;
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

        public bool IsOverall
        {
            get
            {
                return this.MapleID / 10000 == 105;
            }
        }

        public bool IsWeapon
        {
            get
            {
                return this.WeaponType != WeaponType.NotAWeapon;
            }
        }

        public bool IsShield
        {
            get
            {
                return this.MapleID / 10000 % 100 == 9;
            }
        }

        public bool IsPet
        {
            get
            {
                return this.MapleID >= 5000000 && this.MapleID <= 5000100;
            }
        }

        public bool IsTownScroll
        {
            get
            {
                return this.MapleID >= 2030000 && this.MapleID < 2030020;
            }
        }

        public bool IsTwoHanded
        {
            get
            {
                switch (this.WeaponType)
                {
                    case WeaponType.Sword2H:
                    case WeaponType.Axe2H:
                    case WeaponType.Blunt2H:
                    case WeaponType.Spear:
                    case WeaponType.PoleArm:
                    case WeaponType.Bow:
                    case WeaponType.Crossbow:
                    case WeaponType.Claw:
                    case WeaponType.Knuckle:
                    case WeaponType.Gun:
                        return true;

                    default:
                        return false;
                }
            }
        }

        public bool IsBlocked
        {
            get
            {
                return this.IsCash || this.IsSealed || (this.IsTradeBlocked && !this.IsScissored);
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

        public Item(int mapleID, short quantity = 1, DateTime? expiration = null, bool equipped = false)
        {
            this.MapleID = mapleID;
            this.MaxPerStack = this.CachedReference.MaxPerStack;
            this.Quantity = (this.Type == ItemType.Equipment) ? (short)1 : quantity;
            if (equipped) this.Slot = (short)this.GetEquippedSlot();

            if (!expiration.HasValue)
            {
                expiration = new DateTime(2079, 1, 1, 12, 0, 0); // NOTE: Default expiration time (permanent).
            }

            this.Expiration = (DateTime)expiration;

            this.IsCash = this.CachedReference.IsCash;
            this.OnlyOne = this.CachedReference.OnlyOne;
            this.IsTradeBlocked = this.CachedReference.IsTradeBlocked;
            this.IsScissored = this.CachedReference.IsScissored;
            this.SalePrice = this.CachedReference.SalePrice;
            this.RequiredLevel = this.CachedReference.RequiredLevel;
            this.Meso = this.CachedReference.Meso;

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
            else if (this.IsConsumable)
            {

                this.CFlags = this.CachedReference.CFlags;
                this.CCureAilments = this.CachedReference.CCureAilments;
                this.CEffect = this.CachedReference.CEffect;
                this.CHealth = this.CachedReference.CHealth;
                this.CMana = this.CachedReference.CMana;
                this.CHealthPercentage = this.CachedReference.CHealthPercentage;
                this.CManaPercentage = this.CachedReference.CManaPercentage;
                this.CMoveTo = this.CachedReference.CMoveTo;
                this.CProb = this.CachedReference.CProb;
                this.CBuffTime = this.CachedReference.CBuffTime;
                this.CWeaponAttack = this.CachedReference.CWeaponAttack;
                this.CMagicAttack = this.CachedReference.CMagicAttack;
                this.CWeaponDefense = this.CachedReference.CWeaponDefense;
                this.CMagicDefense = this.CachedReference.CMagicDefense;
                this.CAccuracy = this.CachedReference.CAccuracy;
                this.CAvoid = this.CachedReference.CAvoid;
                this.CSpeed = this.CachedReference.CSpeed;
                this.CJump = this.CachedReference.CJump;
                this.CMorph = this.CachedReference.CMorph;
            }

            this.Summons = this.CachedReference.Summons;
        }

        public Item(Datum datum)
        {
            if (DataProvider.IsInitialized)
            {
                this.ID = (int)datum["ID"];
                this.Assigned = true;

                this.AccountID = (int)datum["AccountID"];
                this.MapleID = (int)datum["MapleID"];
                this.MaxPerStack = this.CachedReference.MaxPerStack;
                this.Quantity = (short)datum["Quantity"];
                this.Slot = (short)datum["Slot"];
                this.Creator = (string)datum["Creator"];
                this.Expiration = (DateTime)datum["Expiration"];
                this.PetID = (int?)datum["PetID"];

                this.IsCash = this.CachedReference.IsCash;
                this.OnlyOne = this.CachedReference.OnlyOne;
                this.IsTradeBlocked = this.CachedReference.IsTradeBlocked;
                this.IsScissored = false;
                this.IsStored = (bool)datum["IsStored"];
                this.SalePrice = this.CachedReference.SalePrice;
                this.RequiredLevel = this.CachedReference.RequiredLevel;
                this.Meso = this.CachedReference.Meso;

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
                else if (this.IsConsumable)
                {
                    this.CFlags = this.CachedReference.CFlags;
                    this.CCureAilments = this.CachedReference.CCureAilments;
                    this.CEffect = this.CachedReference.CEffect;
                    this.CHealth = this.CachedReference.CHealth;
                    this.CMana = this.CachedReference.CMana;
                    this.CHealthPercentage = this.CachedReference.CHealthPercentage;
                    this.CManaPercentage = this.CachedReference.CManaPercentage;
                    this.CMoveTo = this.CachedReference.CMoveTo;
                    this.CProb = this.CachedReference.CProb;
                    this.CBuffTime = this.CachedReference.CBuffTime;
                    this.CWeaponAttack = this.CachedReference.CWeaponAttack;
                    this.CMagicAttack = this.CachedReference.CMagicAttack;
                    this.CWeaponDefense = this.CachedReference.CWeaponDefense;
                    this.CMagicDefense = this.CachedReference.CMagicDefense;
                    this.CAccuracy = this.CachedReference.CAccuracy;
                    this.CAvoid = this.CachedReference.CAvoid;
                    this.CSpeed = this.CachedReference.CSpeed;
                    this.CJump = this.CachedReference.CJump;
                    this.CMorph = this.CachedReference.CMorph;
                }

                this.Summons = this.CachedReference.Summons;
            }
            else
            {
                this.MapleID = (int)datum["itemid"];
                this.MaxPerStack = (short)datum["max_slot_quantity"];

                this.IsCash = ((string)datum["flags"]).Contains("cash_item");
                this.OnlyOne = (sbyte)datum["max_possession_count"] > 0;
                this.IsTradeBlocked = ((string)datum["flags"]).Contains("no_trade");
                this.IsScissored = false;
                this.SalePrice = (int)datum["price"];
                this.RequiredLevel = (byte)datum["min_level"];
                this.Meso = (int)datum["money"];

                this.Summons = new List<Tuple<int, short>>();
            }
        }

        public void LoadConsumeData(Datum datum)
        {
            //this.CFlags = datum["flags"];
            //this.CCureAilments = datum["cure_ailments"];
            this.CEffect = (byte)datum["effect"];
            this.CHealth = (short)datum["hp"];
            this.CMana = (short)datum["mp"];
            this.CHealthPercentage = (short)datum["hp_percentage"];
            this.CManaPercentage = (short)datum["mp_percentage"];
            this.CMoveTo = (int)datum["move_to"];
            this.CProb = (byte)datum["prob"];
            this.CBuffTime = (int)datum["buff_time"];
            this.CWeaponAttack = (short)datum["weapon_attack"];
            this.CMagicAttack = (short)datum["magic_attack"];
            this.CWeaponDefense = (short)datum["weapon_defense"];
            this.CMagicAttack = (short)datum["magic_attack"];
            this.CAccuracy = (short)datum["accuracy"];
            this.CAvoid = (short)datum["avoid"];
            this.CSpeed = (short)datum["speed"];
            this.CJump = (short)datum["jump"];
            this.CMorph = (short)datum["morph"];
        }

        public void LoadEquipmentData(Datum datum)
        {
            this.RequiredStrength = (short)datum["req_str"];
            this.RequiredDexterity = (short)datum["req_dex"];
            this.RequiredIntelligence = (short)datum["req_int"];
            this.RequiredLuck = (short)datum["req_luk"];
            this.RequiredFame = (short)datum["req_fame"];

            this.UpgradesAvailable = (byte)(ushort)datum["scroll_slots"];
            this.UpgradesApplied = 0;

            this.Health = (short)datum["hp"];
            this.Mana = (short)datum["mp"];
            this.Strength = (short)datum["strength"];
            this.Dexterity = (short)datum["dexterity"];
            this.Intelligence = (short)datum["intelligence"];
            this.Luck = (short)datum["luck"];
            this.WeaponAttack = (short)datum["weapon_attack"];
            this.WeaponDefense = (short)datum["weapon_defense"];
            this.MagicAttack = (short)datum["magic_attack"];
            this.MagicDefense = (short)datum["magic_defense"];
            this.Accuracy = (short)datum["accuracy"];
            this.Avoidability = (short)datum["avoid"];
            this.Speed = (short)datum["speed"];
            this.Jump = (short)datum["jump"];
            this.Agility = (short)datum["hands"];
        }

        public void Save()
        {
            Datum datum = new Datum("items");

            datum["AccountID"] = this.Character.AccountID;
            datum["CharacterID"] = this.Character.ID;
            datum["MapleID"] = this.MapleID;
            datum["Quantity"] = this.Quantity;
            datum["Slot"] = this.Slot;
            datum["Creator"] = this.Creator;
            datum["UpgradesAvailable"] = this.UpgradesAvailable;
            datum["UpgradesApplied"] = this.UpgradesApplied;
            datum["Strength"] = this.Strength;
            datum["Dexterity"] = this.Dexterity;
            datum["Intelligence"] = this.Intelligence;
            datum["Luck"] = this.Luck;
            datum["Health"] = this.Health;
            datum["Mana"] = this.Mana;
            datum["WeaponAttack"] = this.WeaponAttack;
            datum["MagicAttack"] = this.MagicAttack;
            datum["WeaponDefense"] = this.WeaponDefense;
            datum["MagicDefense"] = this.MagicDefense;
            datum["Accuracy"] = this.Accuracy;
            datum["Avoidability"] = this.Avoidability;
            datum["Agility"] = this.Agility;
            datum["Speed"] = this.Speed;
            datum["Jump"] = this.Jump;
            datum["IsScissored"] = this.IsScissored;
            datum["IsStored"] = this.IsStored;
            datum["PreventsSlipping"] = this.PreventsSlipping;
            datum["PreventsColdness"] = this.PreventsColdness;

            if (this.Assigned)
            {
                datum.Update("ID = {0}", this.ID);
            }
            else
            {
                this.ID = datum.InsertAndReturnID();
                this.Assigned = true;
            }
        }

        public void Delete()
        {
            Database.Delete("items", "ID = {0}", this.ID);

            this.Assigned = false;
        }

        public void Update()
        {
            using (Packet oPacket = new Packet(ServerOperationCode.InventoryOperation))
            {
                oPacket
                    .WriteBool(true)
                    .WriteByte(1)
                    .WriteByte((byte)InventoryOperationType.ModifyQuantity)
                    .WriteByte((byte)this.Type)
                    .WriteShort(this.Slot)
                    .WriteShort(this.Quantity);

                this.Character.Client.Send(oPacket);
            }
        }

        public void Equip()
        {
            if (this.Type != ItemType.Equipment)
            {
                throw new InvalidOperationException("Can only equip equipment items.");
            }

            if ((this.Character.Strength < this.RequiredStrength ||
                this.Character.Dexterity < this.RequiredDexterity ||
                this.Character.Intelligence < this.RequiredIntelligence ||
                this.Character.Luck < this.RequiredLuck) &&
                !this.Character.IsMaster)
            {
                return;
            }

            short sourceSlot = this.Slot;
            EquipmentSlot destinationSlot = this.GetEquippedSlot();

            Item top = this.Parent[EquipmentSlot.Top];
            Item bottom = this.Parent[EquipmentSlot.Bottom];
            Item weapon = this.Parent[EquipmentSlot.Weapon];
            Item shield = this.Parent[EquipmentSlot.Shield];

            Item destination = this.Parent[destinationSlot];

            if (destination != null)
            {
                destination.Slot = sourceSlot;
            }

            this.Slot = (short)destinationSlot;

            using (Packet oPacket = new Packet(ServerOperationCode.InventoryOperation))
            {
                oPacket
                    .WriteBool(true)
                    .WriteByte(1)
                    .WriteByte((byte)InventoryOperationType.ModifySlot)
                    .WriteByte((byte)this.Type)
                    .WriteShort(sourceSlot)
                    .WriteShort((short)destinationSlot)
                    .WriteByte(1);

                this.Character.Client.Send(oPacket);
            }

            switch (destinationSlot)
            {
                case EquipmentSlot.Bottom:
                    {
                        if (top != null && top.IsOverall)
                        {
                            top.Unequip();
                        }
                    }
                    break;

                case EquipmentSlot.Top:
                    {
                        if (this.IsOverall && bottom != null)
                        {
                            bottom.Unequip();
                        }
                    }
                    break;

                case EquipmentSlot.Shield:
                    {
                        if (weapon != null && weapon.IsTwoHanded)
                        {
                            weapon.Unequip();
                        }
                    }
                    break;

                case EquipmentSlot.Weapon:
                    {
                        if (this.IsTwoHanded && shield != null)
                        {
                            shield.Unequip();
                        }
                    }
                    break;
            }

            this.Character.UpdateApperance();
        }

        public void Unequip(short destinationSlot = 0)
        {
            if (this.Type != ItemType.Equipment)
            {
                throw new InvalidOperationException("Cna only unequip equipment items.");
            }

            short sourceSlot = this.Slot;

            if (destinationSlot == 0)
            {
                destinationSlot = this.Parent.GetNextFreeSlot(ItemType.Equipment);
            }

            this.Slot = destinationSlot;

            using (Packet oPacket = new Packet(ServerOperationCode.InventoryOperation))
            {
                oPacket
                    .WriteBool(true)
                    .WriteByte(1)
                    .WriteByte((byte)InventoryOperationType.ModifySlot)
                    .WriteByte((byte)this.Type)
                    .WriteShort(sourceSlot)
                    .WriteShort(destinationSlot)
                    .WriteByte(1);

                this.Character.Client.Send(oPacket);
            }

            this.Character.UpdateApperance();
        }

        public void Drop(short quantity)
        {
            if (this.IsRechargeable)
            {
                quantity = this.Quantity;
            }

            if (this.IsBlocked)
            {
                return;
            }

            if (quantity > this.Quantity)
            {
                return;
            }

            if (quantity == this.Quantity)
            {
                using (Packet oPacket = new Packet(ServerOperationCode.InventoryOperation))
                {
                    oPacket
                        .WriteBool(true)
                        .WriteByte(1)
                        .WriteByte((byte)InventoryOperationType.RemoveItem)
                        .WriteByte((byte)this.Type)
                        .WriteShort(this.Slot);

                    if (this.IsEquipped)
                    {
                        oPacket.WriteByte(1);
                    }

                    this.Character.Client.Send(oPacket);
                }

                this.Dropper = this.Character;
                this.Owner = null;

                this.Character.Map.Drops.Add(this);

                this.Parent.Remove(this, false);
            }
            else if (quantity < this.Quantity)
            {
                this.Quantity -= quantity;

                using (Packet oPacket = new Packet(ServerOperationCode.InventoryOperation))
                {
                    oPacket
                        .WriteBool(true)
                        .WriteByte(1)
                        .WriteByte((byte)InventoryOperationType.ModifyQuantity)
                        .WriteByte((byte)this.Type)
                        .WriteShort(this.Slot)
                        .WriteShort(this.Quantity);

                    this.Character.Client.Send(oPacket);
                }

                Item dropped = new Item(this.MapleID, quantity)
                {
                    Dropper = this.Character,
                    Owner = null
                };

                this.Character.Map.Drops.Add(dropped);
            }
        }

        public void Move(short destinationSlot)
        {
            short sourceSlot = this.Slot;

            Item destination = this.Parent[this.Type, destinationSlot];

            if (destination != null &&
                this.Type != ItemType.Equipment &&
                this.MapleID == destination.MapleID &&
                !this.IsRechargeable &&
                destination.Quantity < destination.MaxPerStack)
            {
                if (this.Quantity + destination.Quantity > destination.MaxPerStack)
                {
                    this.Quantity -= (short)(destination.MaxPerStack - destination.Quantity);

                    using (Packet oPacket = new Packet(ServerOperationCode.InventoryOperation))
                    {
                        oPacket
                            .WriteBool(true)
                            .WriteByte(2)
                            .WriteByte((byte)InventoryOperationType.ModifyQuantity)
                            .WriteByte((byte)this.Type)
                            .WriteShort(sourceSlot)
                            .WriteShort(this.Quantity)
                            .WriteByte((byte)InventoryOperationType.ModifyQuantity)
                            .WriteByte((byte)destination.Type)
                            .WriteShort(destinationSlot)
                            .WriteShort(destination.Quantity);

                        this.Character.Client.Send(oPacket);
                    }
                }
                else
                {
                    destination.Quantity += this.Quantity;

                    using (Packet oPacket = new Packet(ServerOperationCode.InventoryOperation))
                    {
                        oPacket
                            .WriteBool(true)
                            .WriteByte(2)
                            .WriteByte((byte)InventoryOperationType.RemoveItem)
                            .WriteByte((byte)this.Type)
                            .WriteShort(sourceSlot)
                            .WriteByte((byte)InventoryOperationType.ModifyQuantity)
                            .WriteByte((byte)destination.Type)
                            .WriteShort(destinationSlot)
                            .WriteShort(destination.Quantity);

                        this.Character.Client.Send(oPacket);
                    }
                }
            }
            else
            {
                if (destination != null)
                {
                    destination.Slot = sourceSlot;
                }

                this.Slot = destinationSlot;

                using (Packet oPacket = new Packet(ServerOperationCode.InventoryOperation))
                {
                    oPacket
                        .WriteBool(true)
                        .WriteByte(1)
                        .WriteByte((byte)InventoryOperationType.ModifySlot)
                        .WriteByte((byte)this.Type)
                        .WriteShort(sourceSlot)
                        .WriteShort(destinationSlot);

                    this.Character.Client.Send(oPacket);
                }
            }
        }

        public byte[] ToByteArray(bool zeroPosition = false, bool leaveOut = false)
        {
            using (ByteBuffer oPacket = new ByteBuffer())
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
                    .WriteByte((byte)(this.PetID != null ? 3 : this.Type == ItemType.Equipment ? 1 : 2))
                    .WriteInt(this.MapleID)
                    .WriteBool(this.IsCash);

                if (this.IsCash)
                {
                    oPacket.WriteLong(1); // TODO: Unique ID for cash items.
                }

                oPacket.WriteDateTime(this.Expiration);

                if (this.PetID != null)
                {

                }
                else if (this.Type == ItemType.Equipment)
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
                        .WriteString(this.Creator)
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
                        .WriteString(this.Creator)
                        .WriteByte(this.Flags)
                        .WriteByte();

                    if (this.IsRechargeable)
                    {
                        oPacket.WriteLong(); // TODO: Unique ID.
                    }
                }

                oPacket.Flip();
                return oPacket.GetContent();
            }
        }

        private EquipmentSlot GetEquippedSlot()
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

            return (EquipmentSlot)slot;
        }

        public override Packet GetShowGainPacket()
        {
            Packet oPacket = new Packet(ServerOperationCode.Message);

            oPacket
                .WriteByte((byte)MessageType.DropPickup)
                .WriteBool(false)
                .WriteInt(this.MapleID)
                .WriteInt(this.Quantity)
                .WriteInt()
                .WriteInt();

            return oPacket;
        }
    }
}
