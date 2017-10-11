using Destiny.Data;
using Destiny.Network;
using Destiny.Maple.Characters;
using System;
using System.Collections.Generic;

namespace Destiny.Maple.Life
{
    public sealed class MobSkill
    {
        public static Dictionary<short, List<int>> Summons { get; set; }

        public byte MapleID { get; private set; }
        public byte Level { get; private set; }
        public short EffectDelay { get; private set; }

        public int Duration { get; private set; }
        public short MpCost { get; private set; }
        public int ParameterA { get; private set; }
        public int ParameterB { get; private set; }
        public short Chance { get; private set; }
        public short TargetCount { get; private set; }
        public int Cooldown { get; private set; }
        public Point LT { get; private set; }
        public Point RB { get; private set; }
        public short PercentageLimitHP { get; private set; }
        public short SummonLimit { get; private set; }
        public short SummonEffect { get; private set; }

        public MobSkill(Datum datum)
        {
            this.MapleID = (byte)(int)datum["skillid"];
            this.Level = (byte)(short)datum["skill_level"];
            this.EffectDelay = (short)(short)datum["effect_delay"];
        }

        public void Load(Datum datum)
        {
            this.Duration = (short)datum["buff_time"];
            this.MpCost = (short)datum["mp_cost"];
            this.ParameterA = (int)datum["x_property"];
            this.ParameterB = (int)datum["y_property"];
            this.Chance = (short)datum["chance"];
            this.TargetCount = (short)datum["target_count"];
            this.Cooldown = (int)datum["cooldown"];
            this.LT = new Point((short)datum["ltx"], (short)datum["lty"]);
            this.RB = new Point((short)datum["rbx"], (short)datum["rby"]);
            this.PercentageLimitHP = (short)datum["hp_limit_percentage"];
            this.SummonLimit = (short)datum["summon_limit"];
            this.SummonEffect = (short)datum["summon_effect"];
        }

        public void Cast(Mob caster)
        {
            MobStatus status = MobStatus.None;
            CharacterDisease disease = CharacterDisease.None;
            bool heal = false;
            bool banish = false;
            bool dispel = false;

            switch ((MobSkillName)this.MapleID)
            {
                case MobSkillName.WeaponAttackUp:
                case MobSkillName.WeaponAttackUpAreaOfEffect:
                case MobSkillName.WeaponAttackUpMonsterCarnival:
                    status = MobStatus.WeaponAttackUp;
                    break;

                case MobSkillName.MagicAttackUp:
                case MobSkillName.MagicAttackUpAreaOfEffect:
                case MobSkillName.MagicAttackUpMonsterCarnival:
                    status = MobStatus.MagicAttackUp;
                    break;

                case MobSkillName.WeaponDefenseUp:
                case MobSkillName.WeaponDefenseUpAreaOfEffect:
                case MobSkillName.WeaponDefenseUpMonsterCarnival:
                    status = MobStatus.WeaponDefenseUp;
                    break;

                case MobSkillName.MagicDefenseUp:
                case MobSkillName.MagicDefenseUpAreaOfEffect:
                case MobSkillName.MagicDefenseUpMonsterCarnival:
                    status = MobStatus.MagicDefenseUp;
                    break;

                case MobSkillName.HealAreaOfEffect:
                    heal = true;
                    break;

                case MobSkillName.Seal:
                    disease = CharacterDisease.Sealed;
                    break;

                case MobSkillName.Darkness:
                    disease = CharacterDisease.Darkness;
                    break;

                case MobSkillName.Weakness:
                    disease = CharacterDisease.Weaken;
                    break;

                case MobSkillName.Stun:
                    disease = CharacterDisease.Stun;
                    break;

                case MobSkillName.Curse:
                    // TODO: Curse.
                    break;

                case MobSkillName.Poison:
                    disease = CharacterDisease.Poison;
                    break;

                case MobSkillName.Slow:
                    disease = CharacterDisease.Slow;
                    break;

                case MobSkillName.Dispel:
                    dispel = true;
                    break;

                case MobSkillName.Seduce:
                    disease = CharacterDisease.Seduce;
                    break;

                case MobSkillName.SendToTown:
                    // TODO: Send to town.
                    break;

                case MobSkillName.PoisonMist:
                    // TODO: Spawn poison mist.
                    break;

                case MobSkillName.Confuse:
                    disease = CharacterDisease.Confuse;
                    break;

                case MobSkillName.Zombify:
                    // TODO: Zombify.
                    break;

                case MobSkillName.WeaponImmunity:
                    status = MobStatus.WeaponImmunity;
                    break;

                case MobSkillName.MagicImmunity:
                    status = MobStatus.MagicImmunity;
                    break;

                case MobSkillName.WeaponDamageReflect:
                case MobSkillName.MagicDamageReflect:
                case MobSkillName.AnyDamageReflect:
                    // TODO: Reflect.
                    break;

                case MobSkillName.AccuracyUpMonsterCarnival:
                case MobSkillName.AvoidabilityUpMonsterCarnival:
                case MobSkillName.SpeedUpMonsterCarnival:
                    // TODO: Monster carnival buffs.
                    break;

                case MobSkillName.Summon:

                    foreach (int mobId in MobSkill.Summons[this.Level])
                    {
                        Mob summon = new Mob(mobId)
                        {
                            Position = caster.Position,
                            SpawnEffect = this.SummonEffect
                        };

                        caster.Map.Mobs.Add(summon);
                    }
                    break;
            }

            foreach (Mob affectedMob in this.GetAffectedMobs(caster))
            {
                if (heal)
                {
                    affectedMob.Heal((uint)this.ParameterA, this.ParameterB);
                }

                if (status != MobStatus.None && !affectedMob.Buffs.Contains(status))
                {
                    affectedMob.Buff(status, (short)this.ParameterA, this);
                }
            }

            foreach (Character affectedCharacter in this.GetAffectedCharacters(caster))
            {
                if (dispel)
                {
                    //affectedCharacter.Dispel();
                }

                if (banish)
                {
                    affectedCharacter.ChangeMap(affectedCharacter.Map.ReturnMapID);
                }

                if (disease != CharacterDisease.None)
                {
                    using (Packet oPacket = new Packet(ServerOperationCode.TemporaryStatSet))
                    {
                        oPacket
                            .WriteLong()
                            .WriteLong((long)disease);

                        oPacket
                            .WriteShort((short)this.ParameterA)
                            .WriteShort(this.MapleID)
                            .WriteShort(this.Level)
                            .WriteInt(this.Duration);

                        oPacket
                            .WriteShort()
                            .WriteShort(900)
                            .WriteByte(1);

                        affectedCharacter.Client.Send(oPacket);
                    }

                    //map packet.
                }
            }

            caster.Mana -= (uint)this.MpCost;

            if (caster.Cooldowns.ContainsKey(this))
            {
                caster.Cooldowns[this] = DateTime.Now;
            }
            else
            {
                caster.Cooldowns.Add(this, DateTime.Now);
            }
        }

        private IEnumerable<Character> GetAffectedCharacters(Mob caster)
        {
            Rectangle boundingBox = new Rectangle(this.LT + caster.Position, this.RB + caster.Position);

            foreach (Character character in caster.Map.Characters)
            {
                if (character.Position.IsInRectangle(boundingBox))
                {
                    yield return character;
                }
            }
        }

        private IEnumerable<Mob> GetAffectedMobs(Mob caster)
        {
            Rectangle boundingBox = new Rectangle(this.LT + caster.Position, this.RB + caster.Position);

            foreach (Mob mob in caster.Map.Mobs)
            {
                if (mob.Position.IsInRectangle(boundingBox))
                {
                    yield return mob;
                }
            }
        }
    }
}
