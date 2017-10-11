using Destiny.Network;
using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using System;
using Destiny.IO;
using Destiny.Threading;

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
        public DateTime Expiration { get; set; }

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
        public short Avoidability { get; set; }
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
                    using (Packet oPacket = new Packet(ServerOperationCode.Cooldown))
                    {
                        oPacket
                            .WriteInt(this.MapleID)
                            .WriteShort((short)this.RemainingCooldownSeconds);

                        this.Character.Client.Send(oPacket);
                    }

                    Delay.Execute(() =>
                    {
                        using (Packet oPacket = new Packet(ServerOperationCode.Cooldown))
                        {
                            oPacket
                                .WriteInt(this.MapleID)
                                .WriteShort(0);

                            this.Character.Client.Send(oPacket);
                        }
                    }, (this.RemainingCooldownSeconds * 1000));
                }
            }
        }

        private bool Assigned { get; set; }

        public Skill(int mapleID, DateTime? expiration = null)
        {
            this.MapleID = mapleID;
            this.CurrentLevel = 0;
            this.MaxLevel = (byte)DataProvider.Skills[this.MapleID].Count;

            if (!expiration.HasValue)
            {
                expiration = new DateTime(2079, 1, 1, 12, 0, 0); // NOTE: Default expiration time (permanent).
            }

            this.Expiration = (DateTime)expiration;
        }

        public Skill(int mapleID, byte currentLevel, byte maxLevel, DateTime? expiration = null)
        {
            this.MapleID = mapleID;
            this.CurrentLevel = currentLevel;
            this.MaxLevel = maxLevel;

            if (!expiration.HasValue)
            {
                expiration = new DateTime(2079, 1, 1, 12, 0, 0); // NOTE: Default expiration time (permanent).
            }

            this.Expiration = (DateTime)expiration;
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
                this.Expiration = (DateTime)datum["Expiration"];
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
                this.Avoidability = (short)datum["avoid"];
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
            datum["Expiration"] = this.Expiration;
            datum["CooldownEnd"] = this.CooldownEnd;

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
            Database.Delete("skills", "ID = {0}", this.ID);

            this.Assigned = false;
        }

        public void Update()
        {
            using (Packet oPacket = new Packet(ServerOperationCode.ChangeSkillRecordResult))
            {
                oPacket
                    .WriteByte(1)
                    .WriteShort(1)
                    .WriteInt(this.MapleID)
                    .WriteInt(this.CurrentLevel)
                    .WriteInt(this.MaxLevel)
                    .WriteDateTime(this.Expiration)
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
            this.Avoidability = this.CachedReference.Avoidability;
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

            if (this.CostItem > 0)
            {

            }

            if (this.CostBullet > 0)
            {

            }

            if (this.CostMeso > 0)
            {

            }

            if (this.Cooldown > 0)
            {
                this.CooldownEnd = DateTime.Now.AddSeconds(this.Cooldown);
            }
        }

        //public void Cast(Packet iPacket)
        //{
        //    if (!this.Character.IsAlive)
        //    {
        //        return;
        //    }

        //    if (this.IsCoolingDown)
        //    {
        //        return;
        //    }

        //    if (this.MapleID == (int)SkillNames.Priest.MysticDoor)
        //    {
        //        Point origin = new Point(iPacket.ReadShort(), iPacket.ReadShort());

        //        // TODO: Open mystic door.
        //    }

        //    this.Character.Health -= this.CostHP;
        //    this.Character.Mana -= this.CostMP;

        //    if (this.Cooldown > 0)
        //    {
        //        this.CooldownEnd = DateTime.Now.AddSeconds(this.Cooldown);
        //    }

        //    // TODO: Money cost.

        //    byte type = 0;
        //    byte direction = 0;
        //    short addedInfo = 0;

        //    switch (this.MapleID)
        //    {
        //        case (int)SkillNames.Priest.MysticDoor:
        //            // NOTe: Prevents the default case from executing, there's no packet data left for it.
        //            break;

        //        case (int)SkillNames.Brawler.MpRecovery:
        //            {
        //                short healthMod = (short)((this.Character.MaxHealth * this.ParameterA) / 100);
        //                short manaMod = (short)((healthMod * this.ParameterB) / 100);

        //                this.Character.Health -= healthMod;
        //                this.Character.Mana += manaMod;
        //            }
        //            break;

        //        case (int)SkillNames.Shadower.Smokescreen:
        //            {
        //                Point origin = new Point(iPacket.ReadShort(), iPacket.ReadShort());

        //                // TODO: Mists.
        //            }
        //            break;

        //        case (int)SkillNames.Corsair.Battleship:
        //            {
        //                // TODO: Reset Battleship health.
        //            }
        //            break;

        //        case (int)SkillNames.Crusader.ArmorCrash:
        //        case (int)SkillNames.WhiteKnight.MagicCrash:
        //        case (int)SkillNames.DragonKnight.PowerCrash:
        //            {
        //                iPacket.ReadInt(); // NOTE: Unknown, probably CRC.
        //                byte mobs = iPacket.ReadByte();

        //                for (byte i = 0; i < mobs; i++)
        //                {
        //                    int objectID = iPacket.ReadInt();

        //                    Mob mob;

        //                    try
        //                    {
        //                        mob = this.Character.Map.Mobs[objectID];
        //                    }
        //                    catch (KeyNotFoundException)
        //                    {
        //                        return;
        //                    }

        //                    // TODO: Mob crash skill.
        //                }
        //            }
        //            break;

        //        case (int)SkillNames.Hero.MonsterMagnet:
        //        case (int)SkillNames.Paladin.MonsterMagnet:
        //        case (int)SkillNames.DarkKnight.MonsterMagnet:
        //            {
        //                int mobs = iPacket.ReadInt();

        //                for (int i = 0; i < mobs; i++)
        //                {
        //                    int objectID = iPacket.ReadInt();

        //                    Mob mob;

        //                    try
        //                    {
        //                        mob = this.Character.Map.Mobs[objectID];
        //                    }
        //                    catch (KeyNotFoundException)
        //                    {
        //                        return;
        //                    }

        //                    bool success = iPacket.ReadBool();

        //                    // TODO: Packet.
        //                }

        //                direction = iPacket.ReadByte();
        //            }
        //            break;

        //        case (int)SkillNames.FirePoisonWizard.Slow:
        //        case (int)SkillNames.IceLightningWizard.Slow:
        //        case (int)SkillNames.BlazeWizard.Slow:
        //        case (int)SkillNames.Page.Threaten:
        //            {
        //                iPacket.ReadInt(); // NOTE: Unknown, probably CRC.

        //                byte mobs = iPacket.ReadByte();

        //                for (byte i = 0; i < mobs; i++)
        //                {
        //                    int objectID = iPacket.ReadInt();

        //                    Mob mob;

        //                    try
        //                    {
        //                        mob = this.Character.Map.Mobs[objectID];
        //                    }
        //                    catch (KeyNotFoundException)
        //                    {
        //                        return;
        //                    }
        //                }

        //                // TODO: Apply mob status.
        //            }
        //            break;

        //        case (int)SkillNames.FirePoisonMage.Seal:
        //        case (int)SkillNames.IceLightningMage.Seal:
        //        case (int)SkillNames.BlazeWizard.Seal:
        //        case (int)SkillNames.Priest.Doom:
        //        case (int)SkillNames.Hermit.ShadowWeb:
        //        case (int)SkillNames.NightWalker.ShadowWeb:
        //        case (int)SkillNames.Shadower.NinjaAmbush:
        //        case (int)SkillNames.NightLord.NinjaAmbush:
        //            {
        //                byte mobs = iPacket.ReadByte();

        //                for (byte i = 0; i < mobs; i++)
        //                {
        //                    int objectID = iPacket.ReadInt();

        //                    Mob mob;

        //                    try
        //                    {
        //                        mob = this.Character.Map.Mobs[objectID];
        //                    }
        //                    catch (KeyNotFoundException)
        //                    {
        //                        return;
        //                    }
        //                }

        //                // TODO: Apply mob status.
        //            }
        //            break;

        //        case (int)SkillNames.Bishop.HerosWill:
        //        case (int)SkillNames.IceLightningArchMage.HerosWill:
        //        case (int)SkillNames.FirePoisonArchMage.HerosWill:
        //        case (int)SkillNames.DarkKnight.HerosWill:
        //        case (int)SkillNames.Hero.HerosWill:
        //        case (int)SkillNames.Paladin.HerosWill:
        //        case (int)SkillNames.NightLord.HerosWill:
        //        case (int)SkillNames.Shadower.HerosWill:
        //        case (int)SkillNames.Bowmaster.HerosWill:
        //        case (int)SkillNames.Marksman.HerosWill:
        //            {
        //                // TODO: Add Buccaneer & Corsair.

        //                // TODO: Remove Sedcude debuff.
        //            }
        //            break;

        //        case (int)SkillNames.Priest.Dispel:
        //            {

        //            }
        //            break;

        //        case (int)SkillNames.Cleric.Heal:
        //            {
        //                short healthRate = this.HP;

        //                if (healthRate > 100)
        //                {
        //                    healthRate = 100;
        //                }

        //                int partyPlayers = this.Character.Party != null ? this.Character.Party.Count : 1;
        //                short healthMod = (short)(((healthRate * this.Character.MaxHealth) / 100) / partyPlayers);

        //                if (this.Character.Party != null)
        //                {
        //                    int experience = 0;

        //                    List<PartyMember> members = new List<PartyMember>();

        //                    foreach (PartyMember member in this.Character.Party)
        //                    {
        //                        if (member.Character != null && member.Character.Map.MapleID == this.Character.Map.MapleID)
        //                        {
        //                            members.Add(member);
        //                        }
        //                    }

        //                    foreach (PartyMember member in members)
        //                    {
        //                        short memberHealth = member.Character.Health;

        //                        if (memberHealth > 0 && memberHealth < member.Character.MaxHealth)
        //                        {
        //                            member.Character.Health += healthMod;

        //                            if (member.Character != this.Character)
        //                            {
        //                                experience += 20 * (member.Character.Health - memberHealth) / (8 * member.Character.Level + 190);
        //                            }
        //                        }
        //                    }

        //                    if (experience > 0)
        //                    {
        //                        this.Character.Experience += experience;
        //                    }
        //                }
        //                else
        //                {
        //                    this.Character.Health += healthRate;
        //                }
        //            }
        //            break;

        //        case (int)SkillNames.Fighter.Rage:
        //        case (int)SkillNames.DawnWarrior.Rage:
        //        case (int)SkillNames.Spearman.IronWill:
        //        case (int)SkillNames.Spearman.HyperBody:
        //        case (int)SkillNames.FirePoisonWizard.Meditation:
        //        case (int)SkillNames.IceLightningWizard.Meditation:
        //        case (int)SkillNames.BlazeWizard.Meditation:
        //        case (int)SkillNames.Cleric.Bless:
        //        case (int)SkillNames.Priest.HolySymbol:
        //        case (int)SkillNames.Bishop.Resurrection:
        //        case (int)SkillNames.Bishop.HolyShield:
        //        case (int)SkillNames.Bowmaster.SharpEyes:
        //        case (int)SkillNames.Marksman.SharpEyes:
        //        case (int)SkillNames.Assassin.Haste:
        //        case (int)SkillNames.NightWalker.Haste:
        //        case (int)SkillNames.Hermit.MesoUp:
        //        case (int)SkillNames.Bandit.Haste:
        //        case (int)SkillNames.Buccaneer.SpeedInfusion:
        //        case (int)SkillNames.ThunderBreaker.SpeedInfusion:
        //        case (int)SkillNames.Buccaneer.TimeLeap:
        //        case (int)SkillNames.Hero.MapleWarrior:
        //        case (int)SkillNames.Paladin.MapleWarrior:
        //        case (int)SkillNames.DarkKnight.MapleWarrior:
        //        case (int)SkillNames.FirePoisonArchMage.MapleWarrior:
        //        case (int)SkillNames.IceLightningArchMage.MapleWarrior:
        //        case (int)SkillNames.Bishop.MapleWarrior:
        //        case (int)SkillNames.Bowmaster.MapleWarrior:
        //        case (int)SkillNames.Marksman.MapleWarrior:
        //        case (int)SkillNames.NightLord.MapleWarrior:
        //        case (int)SkillNames.Shadower.MapleWarrior:
        //        case (int)SkillNames.Buccaneer.MapleWarrior:
        //        case (int)SkillNames.Corsair.MapleWarrior:
        //            {
        //                if (this.MapleID == (int)SkillNames.Buccaneer.TimeLeap)
        //                {
        //                    // TODO: Remove all cooldowns.
        //                }

        //                if (this.Character.Party != null)
        //                {
        //                    byte targets = iPacket.ReadByte();

        //                    // TODO: Get affected party members.

        //                    List<PartyMember> affected = new List<PartyMember>();

        //                    foreach (PartyMember member in affected)
        //                    {
        //                        using (Packet oPacket = new Packet(ServerOperationCode.Effect))
        //                        {
        //                            oPacket
        //                                .WriteByte((byte)UserEffect.SkillAffected)
        //                                .WriteInt(this.MapleID)
        //                                .WriteByte(1)
        //                                .WriteByte(1);

        //                            member.Character.Client.Send(oPacket);
        //                        }

        //                        using (Packet oPacket = new Packet(ServerOperationCode.RemoteEffect))
        //                        {
        //                            oPacket
        //                                .WriteInt(member.Character.ID)
        //                                .WriteByte((byte)UserEffect.SkillAffected)
        //                                .WriteInt(this.MapleID)
        //                                .WriteByte(1)
        //                                .WriteByte(1);

        //                            member.Character.Map.Broadcast(oPacket, member.Character);
        //                        }

        //                        member.Character.Buffs.Add(this, 0);
        //                    }
        //                }
        //            }
        //            break;

        //        case (int)SkillNames.Beginner.EchoOfHero:
        //        case (int)SkillNames.Noblesse.EchoOfHero:
        //        case (int)SkillNames.SuperGM.Haste:
        //        case (int)SkillNames.SuperGM.HolySymbol:
        //        case (int)SkillNames.SuperGM.Bless:
        //        case (int)SkillNames.SuperGM.HyperBody:
        //        case (int)SkillNames.SuperGM.HealPlusDispel:
        //        case (int)SkillNames.SuperGM.Resurrection:
        //            {
        //                byte targets = iPacket.ReadByte();
        //                Func<Character, bool> condition = null;
        //                Action<Character> action = null;

        //                switch (this.MapleID)
        //                {
        //                    case (int)SkillNames.SuperGM.HealPlusDispel:
        //                        {
        //                            condition = new Func<Character, bool>((target) => target.IsAlive);
        //                            action = new Action<Character>((target) =>
        //                            {
        //                                target.Health = target.MaxHealth;
        //                                target.Mana = target.MaxMana;

        //                                // TODO: Use dispell.
        //                            });
        //                        }
        //                        break;

        //                    case (int)SkillNames.SuperGM.Resurrection:
        //                        {
        //                            condition = new Func<Character, bool>((target) => !target.IsAlive);
        //                            action = new Action<Character>((target) =>
        //                            {
        //                                target.Health = target.MaxHealth;
        //                            });
        //                        }
        //                        break;

        //                    default:
        //                        {
        //                            condition = new Func<Character, bool>((target) => true);
        //                            action = new Action<Character>((target) =>
        //                            {
        //                                target.Buffs.Add(this, 0);
        //                            });
        //                        }
        //                        break;
        //                }

        //                for (byte i = 0; i < targets; i++)
        //                {
        //                    int targetID = iPacket.ReadInt();

        //                    Character target = this.Character.Map.Characters[targetID];

        //                    if (target != this.Character && condition(target))
        //                    {
        //                        using (Packet oPacket = new Packet(ServerOperationCode.Effect))
        //                        {
        //                            oPacket
        //                                .WriteByte((byte)UserEffect.SkillAffected)
        //                                .WriteInt(this.MapleID)
        //                                .WriteByte(1)
        //                                .WriteByte(1);

        //                            target.Client.Send(oPacket);
        //                        }

        //                        using (Packet oPacket = new Packet(ServerOperationCode.RemoteEffect))
        //                        {
        //                            oPacket
        //                                .WriteInt(target.ID)
        //                                .WriteByte((byte)UserEffect.SkillAffected)
        //                                .WriteInt(this.MapleID)
        //                                .WriteByte(1)
        //                                .WriteByte(1);

        //                            target.Map.Broadcast(oPacket, target);
        //                        }

        //                        action(target);
        //                    }
        //                }
        //            }
        //            break;

        //        default:
        //            {
        //                type = iPacket.ReadByte();

        //                switch (type)
        //                {
        //                    case 0x80:
        //                        addedInfo = iPacket.ReadShort();
        //                        break;
        //                }
        //            }
        //            break;
        //    }

        //    using (Packet oPacket = new Packet(ServerOperationCode.Effect))
        //    {
        //        oPacket
        //            .WriteByte((byte)UserEffect.SkillUse)
        //            .WriteInt(this.MapleID)
        //            .WriteByte(1)
        //            .WriteByte(1);

        //        this.Character.Client.Send(oPacket);
        //    }

        //    using (Packet oPacket = new Packet(ServerOperationCode.RemoteEffect))
        //    {
        //        oPacket
        //            .WriteInt(Character.ID)
        //            .WriteByte((byte)UserEffect.SkillUse)
        //            .WriteInt(this.MapleID)
        //            .WriteByte(1)
        //            .WriteByte(1);

        //        this.Character.Map.Broadcast(oPacket, this.Character);
        //    }

        //    if (this.HasBuff)
        //    {
        //        this.Character.Buffs.Add(this, 0);
        //    }
        //}

        public byte[] ToByteArray()
        {
            using (ByteBuffer oPacket = new ByteBuffer())
            {
                oPacket
                    .WriteInt(this.MapleID)
                    .WriteInt(this.CurrentLevel)
                    .WriteDateTime(this.Expiration);

                if (this.IsFromFourthJob)
                {
                    oPacket.WriteInt(this.MaxLevel);
                }

                oPacket.Flip();
                return oPacket.GetContent();
            }
        }
    }
}
