using Destiny.Network;
using Destiny.Data;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using Destiny.Maple.Maps;
using System;
using System.Collections.Generic;
using Destiny.Threading;

namespace Destiny.Maple.Life
{
    public sealed class Mob : MapObject, IMoveable, ISpawnable, IControllable
    {
        public int MapleID { get; private set; }
        public Character Controller { get; set; }
        public Dictionary<Character, uint> Attackers { get; private set; }
        public SpawnPoint SpawnPoint { get; private set; }
        public byte Stance { get; set; }
        public bool IsProvoked { get; set; }
        public bool CanDrop { get; set; }
        public List<Loot> Loots { get; private set; }
        public short Foothold { get; set; }
        public MobSkills Skills { get; private set; }
        public Dictionary<MobSkill, DateTime> Cooldowns { get; private set; }
        public List<MobStatus> Buffs { get; private set; }
        public List<int> DeathSummons { get; private set; }

        public short Level { get; private set; }
        public uint Health { get; set; }
        public uint Mana { get; set; }
        public uint MaxHealth { get; private set; }
        public uint MaxMana { get; private set; }
        public uint HealthRecovery { get; private set; }
        public uint ManaRecovery { get; private set; }
        public int ExplodeHealth { get; private set; }
        public uint Experience { get; private set; }
        public int Link { get; private set; }
        public short SummonType { get; private set; }
        public int KnockBack { get; private set; }
        public int FixedDamage { get; private set; }
        public int DeathBuff { get; private set; }
        public int DeathAfter { get; private set; }
        public double Traction { get; private set; }
        public int DamagedBySkillOnly { get; private set; }
        public int DamagedByMobOnly { get; private set; }
        public int DropItemPeriod { get; private set; }
        public byte HpBarForeColor { get; private set; }
        public byte HpBarBackColor { get; private set; }
        public byte CarnivalPoints { get; private set; }
        public int WeaponAttack { get; private set; }
        public int WeaponDefense { get; private set; }
        public int MagicAttack { get; private set; }
        public int MagicDefense { get; private set; }
        public short Accuracy { get; private set; }
        public short Avoidability { get; private set; }
        public short Speed { get; private set; }
        public short ChaseSpeed { get; private set; }

        public bool IsFacingLeft
        {
            get
            {
                return this.Stance % 2 == 0;
            }
        }

        public bool CanRespawn
        {
            get
            {
                return true; // TODO.
            }
        }

        public int SpawnEffect { get; set; }

        public Mob CachedReference
        {
            get
            {
                return DataProvider.Mobs[this.MapleID];
            }
        }

        public Mob(Datum datum)
            : base()
        {
            this.MapleID = (int)datum["mobid"];

            this.Level = (short)datum["mob_level"];
            this.Health = this.MaxHealth = (uint)datum["hp"];
            this.Mana = this.MaxMana = (uint)datum["mp"];
            this.HealthRecovery = (uint)datum["hp_recovery"];
            this.ManaRecovery = (uint)datum["mp_recovery"];
            this.ExplodeHealth = (int)datum["explode_hp"];
            this.Experience = (uint)datum["experience"];
            this.Link = (int)datum["link"];
            this.SummonType = (short)datum["summon_type"];
            this.KnockBack = (int)datum["knockback"];
            this.FixedDamage = (int)datum["fixed_damage"];
            this.DeathBuff = (int)datum["death_buff"];
            this.DeathAfter = (int)datum["death_after"];
            this.Traction = (double)datum["traction"];
            this.DamagedBySkillOnly = (int)datum["damaged_by_skill_only"];
            this.DamagedByMobOnly = (int)datum["damaged_by_mob_only"];
            this.DropItemPeriod = (int)datum["drop_item_period"];
            this.HpBarForeColor = (byte)(sbyte)datum["hp_bar_color"];
            this.HpBarBackColor = (byte)(sbyte)datum["hp_bar_bg_color"];
            this.CarnivalPoints = (byte)(sbyte)datum["carnival_points"];
            this.WeaponAttack = (int)datum["physical_attack"];
            this.WeaponDefense = (int)datum["physical_defense"];
            this.MagicAttack = (int)datum["magical_attack"];
            this.MagicDefense = (int)datum["magical_defense"];
            this.Accuracy = (short)datum["accuracy"];
            this.Avoidability = (short)datum["avoidability"];
            this.Speed = (short)datum["speed"];
            this.ChaseSpeed = (short)datum["chase_speed"];

            this.Loots = new List<Loot>();
            this.Skills = new MobSkills(this);
            this.DeathSummons = new List<int>();
        }

        public Mob(int mapleID)
        {
            this.MapleID = mapleID;

            this.Level = this.CachedReference.Level;
            this.Health = this.CachedReference.Health;
            this.Mana = this.CachedReference.Mana;
            this.MaxHealth = this.CachedReference.MaxHealth;
            this.MaxMana = this.CachedReference.MaxMana;
            this.HealthRecovery = this.CachedReference.HealthRecovery;
            this.ManaRecovery = this.CachedReference.ManaRecovery;
            this.ExplodeHealth = this.CachedReference.ExplodeHealth;
            this.Experience = this.CachedReference.Experience;
            this.Link = this.CachedReference.Link;
            this.SummonType = this.CachedReference.SummonType;
            this.KnockBack = this.CachedReference.KnockBack;
            this.FixedDamage = this.CachedReference.FixedDamage;
            this.DeathBuff = this.CachedReference.DeathBuff;
            this.DeathAfter = this.CachedReference.DeathAfter;
            this.Traction = this.CachedReference.Traction;
            this.DamagedBySkillOnly = this.CachedReference.DamagedBySkillOnly;
            this.DamagedByMobOnly = this.CachedReference.DamagedByMobOnly;
            this.DropItemPeriod = this.CachedReference.DropItemPeriod;
            this.HpBarForeColor = this.CachedReference.HpBarForeColor;
            this.HpBarBackColor = this.CachedReference.HpBarBackColor;
            this.CarnivalPoints = this.CachedReference.CarnivalPoints;
            this.WeaponAttack = this.CachedReference.WeaponAttack;
            this.WeaponDefense = this.CachedReference.WeaponDefense;
            this.MagicAttack = this.CachedReference.MagicAttack;
            this.MagicDefense = this.CachedReference.MagicDefense;
            this.Accuracy = this.CachedReference.Accuracy;
            this.Avoidability = this.CachedReference.Avoidability;
            this.Speed = this.CachedReference.Speed;
            this.ChaseSpeed = this.CachedReference.ChaseSpeed;

            this.Loots = this.CachedReference.Loots;
            this.Skills = this.CachedReference.Skills;
            this.DeathSummons = this.CachedReference.DeathSummons;

            this.Attackers = new Dictionary<Character, uint>();
            this.Cooldowns = new Dictionary<MobSkill, DateTime>();
            this.Buffs = new List<MobStatus>();
            this.Stance = 5;
            this.CanDrop = true;
        }

        public Mob(SpawnPoint spawnPoint)
            : this(spawnPoint.MapleID)
        {
            this.SpawnPoint = spawnPoint;
            this.Foothold = this.SpawnPoint.Foothold;
            this.Position = this.SpawnPoint.Position;
            this.Position.Y -= 1; // TODO: Is this needed?
        }

        public Mob(int mapleID, Point position)
            : this(mapleID)
        {
            this.Foothold = 0; // TODO.
            this.Position = position;
            this.Position.Y -= 5; // TODO: Is this needed?
        }

        public void AssignController()
        {
            if (this.Controller == null)
            {
                int leastControlled = int.MaxValue;
                Character newController = null;

                lock (this.Map.Characters)
                {
                    foreach (Character character in this.Map.Characters)
                    {
                        if (character.ControlledMobs.Count < leastControlled)
                        {
                            leastControlled = character.ControlledMobs.Count;
                            newController = character;
                        }
                    }
                }

                if (newController != null)
                {
                    this.IsProvoked = false;

                    newController.ControlledMobs.Add(this);
                }
            }
        }

        public void SwitchController(Character newController)
        {
            lock (this)
            {
                if (this.Controller != newController)
                {
                    this.Controller.ControlledMobs.Remove(this);

                    newController.ControlledMobs.Add(this);
                }
            }
        }

        public void Move(Packet iPacket)
        {
            short moveAction = iPacket.ReadShort();
            bool cheatResult = (iPacket.ReadByte() & 0xF) != 0;
            byte centerSplit = iPacket.ReadByte();
            int illegalVelocity = iPacket.ReadInt();
            iPacket.Skip(8);
            iPacket.ReadByte();
            iPacket.ReadInt();

            Movements movements = Movements.Decode(iPacket);

            this.Position = movements.Position;
            this.Foothold = movements.Foothold;
            this.Stance = movements.Stance;

            byte skillID = 0;
            byte skillLevel = 0;
            MobSkill skill = null;
            
            if (skill != null)
            {
                if (this.Health * 100 / this.MaxHealth > skill.PercentageLimitHP ||
                    (this.Cooldowns.ContainsKey(skill) && this.Cooldowns[skill].AddSeconds(skill.Cooldown) >= DateTime.Now) ||
                    ((MobSkillName)skill.MapleID) == MobSkillName.Summon && this.Map.Mobs.Count >= 100)
                {
                    skill = null;
                }
            }

            if (skill != null)
            {
                skill.Cast(this);
            }

            using (Packet oPacket = new Packet(ServerOperationCode.MobCtrlAck))
            {
                oPacket
                    .WriteInt(this.ObjectID)
                    .WriteShort(moveAction)
                    .WriteBool(cheatResult)
                    .WriteShort((short)this.Mana)
                    .WriteByte(skillID)
                    .WriteByte(skillLevel);

                this.Controller.Client.Send(oPacket);
            }

            using (Packet oPacket = new Packet(ServerOperationCode.MobMove))
            {
                oPacket
                    .WriteInt(this.ObjectID)
                    .WriteBool(false)
                    .WriteBool(cheatResult)
                    .WriteByte(centerSplit)
                    .WriteInt(illegalVelocity)
                    .WriteBytes(movements.ToByteArray());

                this.Map.Broadcast(oPacket, this.Controller);
            }
        }

        public void Buff(MobStatus buff, short value, MobSkill skill)
        {
            using (Packet oPacket = new Packet(ServerOperationCode.MobStatSet))
            {
                oPacket
                    .WriteInt(this.ObjectID)
                    .WriteLong()
                    .WriteInt()
                    .WriteInt((int)buff)
                    .WriteShort(value)
                    .WriteShort(skill.MapleID)
                    .WriteShort(skill.Level)
                    .WriteShort(-1)
                    .WriteShort(0) // Delay
                    .WriteInt();

                this.Map.Broadcast(oPacket);
            }

            Delay.Execute(() =>
            {
                using (Packet Packet = new Packet(ServerOperationCode.MobStatReset))
                {
                    Packet
                        .WriteInt(this.ObjectID)
                        .WriteLong()
                        .WriteInt()
                        .WriteInt((int)buff)
                        .WriteInt();

                    this.Map.Broadcast(Packet);
                }

                this.Buffs.Remove(buff);
            }, skill.Duration * 1000);
        }

        public void Heal(uint hp, int range)
        {
            this.Health = Math.Min(this.MaxHealth, (uint)(this.Health + hp + Application.Random.Next(-range / 2, range / 2)));

            using (Packet Packet = new Packet(ServerOperationCode.MobDamaged))
            {
                Packet
                    .WriteInt(this.ObjectID)
                    .WriteByte()
                    .WriteInt((int)-hp)
                    .WriteByte()
                    .WriteByte()
                    .WriteByte();

                this.Map.Broadcast(Packet);
            }
        }

        public void Die()
        {
            if (!this.Map.Mobs.Remove(this))
            {

            }
        }

        public bool Damage(Character attacker, uint amount)
        {
            lock (this)
            {
                uint originalAmount = amount;

                amount = Math.Min(amount, this.Health);

                if (this.Attackers.ContainsKey(attacker))
                {
                    this.Attackers[attacker] += amount;
                }
                else
                {
                    this.Attackers.Add(attacker, amount);
                }

                this.Health -= amount;

                using (Packet oPacket = new Packet(ServerOperationCode.MobHPIndicator))
                {
                    oPacket
                        .WriteInt(this.ObjectID)
                        .WriteByte((byte)((this.Health * 100) / this.MaxHealth));

                    attacker.Client.Send(oPacket);
                }

                if (this.Health <= 0)
                {
                    return true;
                }

                return false;
            }
        }

        public Packet GetCreatePacket()
        {
            return this.GetInternalPacket(false, true);
        }

        public Packet GetSpawnPacket()
        {
            return this.GetInternalPacket(false, false);
        }

        public Packet GetControlRequestPacket()
        {
            return this.GetInternalPacket(true, false);
        }

        private Packet GetInternalPacket(bool requestControl, bool newSpawn)
        {
            Packet oPacket = new Packet(requestControl ? ServerOperationCode.MobChangeController : ServerOperationCode.MobEnterField);

            if (requestControl)
            {
                oPacket.WriteByte((byte)(this.IsProvoked ? 2 : 1));
            }

            oPacket
                .WriteInt(this.ObjectID)
                .WriteByte((byte)(this.Controller == null ? 5 : 1))
                .WriteInt(this.MapleID)
                .Skip(15) // NOTE: Unknown.
                .WriteByte(0x88) // NOTE: Unknown.
                .Skip(6) // NOTE: Unknown.
                .WriteShort(this.Position.X)
                .WriteShort(this.Position.Y)
                .WriteByte((byte)(0x02 | (this.IsFacingLeft ? 0x01 : 0x00)))
                .WriteShort(this.Foothold)
                .WriteShort(this.Foothold);

            if (this.SpawnEffect > 0)
            {
                oPacket
                    .WriteByte((byte)this.SpawnEffect)
                    .WriteByte()
                    .WriteShort();

                if (this.SpawnEffect == 15)
                {
                    oPacket.WriteByte();
                }
            }

            oPacket
                .WriteByte((byte)(newSpawn ? -2 : -1))
                .WriteByte()
                .WriteByte(byte.MaxValue) // NOTE: Carnival team.
                .WriteInt(); // NOTE: Unknown.

            return oPacket;
        }

        public Packet GetControlCancelPacket()
        {
            Packet oPacket = new Packet(ServerOperationCode.MobChangeController);

            oPacket
                .WriteBool(false)
                .WriteInt(this.ObjectID);

            return oPacket;
        }

        public Packet GetDestroyPacket()
        {
            Packet oPacket = new Packet(ServerOperationCode.MobLeaveField);

            oPacket
                .WriteInt(this.ObjectID)
                .WriteByte(1)
                .WriteByte(1); // TODO: Death effects.

            return oPacket;
        }
    }
}
