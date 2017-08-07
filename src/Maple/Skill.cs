using Destiny.Core.IO;
using Destiny.Core.Network;
using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using System;

namespace Destiny.Maple
{
    public sealed class Skill
    {
        public CharacterSkills Parent { get; set; }

        private byte currentLevel;
        private byte maxLevel;
        private DateTime cooldownEnd = DateTime.MinValue;

        public int ID { get; set; }
        public int MapleID { get; set; }

        public sbyte MobCount { get; set; }
        public sbyte HitCount { get; set; }
        public short Range { get; set; }
        public int BuffTime { get; set; }
        public short CostMP { get; set; }
        public short CostHP { get; set; }
        public short Damage { get; set; }
        public int FixedDamage { get; set; }
        public byte CriticalDamage { get; set; }
        public sbyte Mastery { get; set; }
        public int OptionalItemCost { get; set; }
        public int CostItem { get; set; }
        public short ItemCount { get; set; }
        public short CostBullet { get; set; }
        public short CostMeso { get; set; }
        public short ParameterA { get; set; }
        public short ParameterB { get; set; }
        public short Speed { get; set; }
        public short Jump { get; set; }
        public short Strength { get; set; }
        public short WeaponAttack { get; set; }
        public short WeaponDefense { get; set; }
        public short MagicAttack { get; set; }
        public short MagicDefense { get; set; }
        public short Accuracy { get; set; }
        public short Avoid { get; set; }
        public short HP { get; set; }
        public short MP { get; set; }
        public short Probability { get; set; }
        public short Morph { get; set; }
        public Point LT { get; private set; }
        public Point RB { get; private set; }
        public int Cooldown { get; set; }

        public bool HasBuff
        {
            get
            {
                return this.BuffTime > 0;
            }
        }

        public byte CurrentLevel
        {
            get
            {
                return currentLevel;
            }
            set
            {
                currentLevel = value;

                if (this.Parent != null)
                {
                    this.Recalculate();

                    if (this.Character.IsInitialized)
                    {
                        this.Update();
                    }
                }
            }
        }

        public byte MaxLevel
        {
            get
            {
                return maxLevel;
            }
            set
            {
                maxLevel = value;

                if (this.Parent != null && this.Character.IsInitialized)
                {
                    this.Update();
                }
            }
        }

        public Skill CachedReference
        {
            get
            {
                return DataProvider.Skills[this.MapleID][this.CurrentLevel];
            }
        }

        public Character Character
        {
            get
            {
                return this.Parent.Parent;
            }
        }

        public bool IsFromFourthJob
        {
            get
            {
                return this.MapleID > 1000000 && (this.MapleID / 10000).ToString()[2] == '2'; // TODO: Redo that.
            }
        }

        public bool IsFromBeginner
        {
            get
            {
                return this.MapleID % 10000000 > 999 && this.MapleID % 10000000 < 1003;
            }
        }

        public bool IsCoolingDown
        {
            get
            {
                return DateTime.Now < this.CooldownEnd;
            }
        }

        public int RemainingCooldownSeconds
        {
            get
            {
                return Math.Min(0, (int)(this.CooldownEnd - DateTime.Now).TotalSeconds);
            }
        }

        public DateTime CooldownEnd
        {
            get
            {
                return cooldownEnd;
            }
            set
            {
                cooldownEnd = value;

                if (this.IsCoolingDown)
                {
                    //using (Packet outPacket = new Packet(MapleServerOperationCode.Cooldown))
                    //{
                    //    outPacket.WriteInt(this.MapleID);
                    //    outPacket.WriteShort((short)this.RemainingCooldownSeconds);

                    //    this.Character.Client.Send(outPacket);
                    //}

                    //Delay.Execute(this.RemainingCooldownSeconds * 1000, () =>
                    //{
                    //    using (Packet outPacket = new Packet(MapleServerOperationCode.Cooldown))
                    //    {
                    //        outPacket.WriteInt(this.MapleID);
                    //        outPacket.WriteShort(0);

                    //        this.Character.Client.Send(outPacket);
                    //    }
                    //});
                }
            }
        }

        private bool Assigned { get; set; }

        public Skill(int mapleID)
        {
            this.MapleID = mapleID;
            this.CurrentLevel = 0;
            this.MaxLevel = (byte)DataProvider.Skills[this.MapleID].Count;
        }

        public Skill(int mapleID, byte currentLevel, byte maxLevel)
        {
            this.MapleID = mapleID;
            this.CurrentLevel = currentLevel;
            this.MaxLevel = maxLevel;
        }

        public Skill(Datum datum)
        {
            if (DataProvider.IsInitialized)
            {
                this.ID = (int)datum["ID"];
                this.Assigned = true;

                this.MapleID = (int)datum["MapleID"];
                this.CurrentLevel = (byte)datum["CurrentLevel"];
                this.MaxLevel = (byte)datum["MaxLevel"];
                this.CooldownEnd = (DateTime)datum["CooldownEnd"];
            }
            else
            {
                this.MapleID = (int)datum["skillid"];
                this.CurrentLevel = (byte)(short)datum["skill_level"];
                this.MobCount = (sbyte)datum["mob_count"];
                this.HitCount = (sbyte)datum["hit_count"];
                this.Range = (short)datum["range"];
                this.BuffTime = (int)datum["buff_time"];
                this.CostHP = (short)datum["hp_cost"];
                this.CostMP = (short)datum["mp_cost"];
                this.Damage = (short)datum["damage"];
                this.FixedDamage = (int)datum["fixed_damage"];
                this.CriticalDamage = (byte)datum["critical_damage"];
                this.Mastery = (sbyte)datum["mastery"];
                this.OptionalItemCost = (int)datum["optional_item_cost"];
                this.CostItem = (int)datum["item_cost"];
                this.ItemCount = (short)datum["item_count"];
                this.CostBullet = (short)datum["bullet_cost"];
                this.CostMeso = (short)datum["money_cost"];
                this.ParameterA = (short)datum["x_property"];
                this.ParameterB = (short)datum["y_property"];
                this.Speed = (short)datum["speed"];
                this.Jump = (short)datum["jump"];
                this.Strength = (short)datum["str"];
                this.WeaponAttack = (short)datum["weapon_atk"];
                this.MagicAttack = (short)datum["magic_atk"];
                this.WeaponDefense = (short)datum["weapon_def"];
                this.MagicDefense = (short)datum["magic_def"];
                this.Accuracy = (short)datum["accuracy"];
                this.Avoid = (short)datum["avoid"];
                this.HP = (short)datum["hp"];
                this.MP = (short)datum["mp"];
                this.Probability = (short)datum["prop"];
                this.Morph = (short)datum["morph"];
                this.LT = new Point((short)datum["ltx"], (short)datum["lty"]);
                this.RB = new Point((short)datum["rbx"], (short)datum["rby"]);
                this.Cooldown = (int)datum["cooldown_time"];
            }
        }

        public void Save()
        {
            Datum datum = new Datum("skills");

            datum["CharacterID"] = this.Character.ID;
            datum["MapleID"] = this.MapleID;
            datum["CurrentLevel"] = this.CurrentLevel;
            datum["MaxLevel"] = this.MaxLevel;
            datum["CooldownEnd"] = this.CooldownEnd;

            if (this.Assigned)
            {
                datum.Update("ID = '{0}'", this.ID);
            }
            else
            {
                this.ID = datum.InsertAndReturnID();
                this.Assigned = true;
            }
        }

        public void Delete()
        {
            Database.Delete("skills", "ID = '{0}'", this.ID);

            this.Assigned = false;
        }

        public void Update()
        {
            using (OutPacket oPacket = new OutPacket(ServerOperationCode.ChangeSkillRecordResult))
            {
                oPacket
                    .WriteByte(1)
                    .WriteShort(1)
                    .WriteInt(this.MapleID)
                    .WriteInt(this.CurrentLevel)
                    .WriteInt(this.MaxLevel)
                    .WriteLong(-1) // NOTE: Expiration.
                    .WriteByte(4);

                this.Character.Client.Send(oPacket);
            }
        }

        public void Recalculate()
        {
            this.MobCount = this.CachedReference.MobCount;
            this.HitCount = this.CachedReference.HitCount;
            this.Range = this.CachedReference.Range;
            this.BuffTime = this.CachedReference.BuffTime;
            this.CostMP = this.CachedReference.CostMP;
            this.CostHP = this.CachedReference.CostHP;
            this.Damage = this.CachedReference.Damage;
            this.FixedDamage = this.CachedReference.FixedDamage;
            this.CriticalDamage = this.CachedReference.CriticalDamage;
            this.Mastery = this.CachedReference.Mastery;
            this.OptionalItemCost = this.CachedReference.OptionalItemCost;
            this.CostItem = this.CachedReference.CostItem;
            this.ItemCount = this.CachedReference.ItemCount;
            this.CostBullet = this.CachedReference.CostBullet;
            this.CostMeso = this.CachedReference.CostMeso;
            this.ParameterA = this.CachedReference.ParameterA;
            this.ParameterB = this.CachedReference.ParameterB;
            this.Speed = this.CachedReference.Speed;
            this.Jump = this.CachedReference.Jump;
            this.Strength = this.CachedReference.Strength;
            this.WeaponAttack = this.CachedReference.WeaponAttack;
            this.WeaponDefense = this.CachedReference.WeaponDefense;
            this.MagicAttack = this.CachedReference.MagicAttack;
            this.MagicDefense = this.CachedReference.MagicDefense;
            this.Accuracy = this.CachedReference.Accuracy;
            this.Avoid = this.CachedReference.Avoid;
            this.HP = this.CachedReference.HP;
            this.MP = this.CachedReference.MP;
            this.Probability = this.CachedReference.Probability;
            this.Morph = this.CachedReference.Morph;
            this.LT = this.CachedReference.LT;
            this.RB = this.CachedReference.RB;
            this.Cooldown = this.CachedReference.Cooldown;
        }

        public void Cast()
        {
            if (this.IsCoolingDown)
            {
                return;
            }

            this.Character.Health -= this.CostHP;
            this.Character.Mana -= this.CostMP;

            if (this.Cooldown > 0)
            {
                this.CooldownEnd = DateTime.Now.AddSeconds(this.Cooldown);
            }

            // TODO: Buffs.
            // TODO: Effects.
        }

        public void Encode(OutPacket oPacket)
        {
            oPacket
                .WriteInt(this.MapleID)
                .WriteInt(this.CurrentLevel)
                .WriteLong(-1); // NOTE: Expiration.

            if (this.IsFromFourthJob)
            {
                oPacket.WriteInt(this.MaxLevel);
            }
        }
    }
}
