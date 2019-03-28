using Destiny.Maple.Characters;
using Destiny.Maple.Maps;
using Destiny.Network;
using static Destiny.Constants.CharacterConstants;
using static Destiny.Constants.SummonConstants;

namespace Destiny.Maple.Life
{
    public sealed class Summon : MapObject, ISpawnable
    {
        public SummonMovementType movementType;
        public Character summonOwner { get; set; }
        public Skill summonSkill { get; set; }
        public int health { get; set; }
        public MapleMapObjectType getType()
        { return MapleMapObjectType.Summon; }
        public bool spawned { get; set; }
        public bool animated { get; set; }

        public Summon(Character owner, Skill skill, Point summonPos, SummonMovementType movementType)
        {
            this.movementType = movementType;
            this.summonOwner = owner;
            this.Map = owner.Map;
            this.summonSkill = skill;
            this.summonSkill.CurrentLevel = skill.MaxLevel;
            this.Position = summonPos;
            spawned = false;
        }

        public bool IsPuppet(Skill summonSkill)
        {
            return summonSkill.MapleID == (int) SkillNames.Ranger.Puppet ||
                   summonSkill.MapleID == (int) SkillNames.Sniper.Puppet ||
                   summonSkill.MapleID == (int) SkillNames.WindArcher3.Puppet;
        }

        public bool IsStationary(Summon summon)
        {
            return GetSummonMovementType(summon.summonSkill) == SummonMovementType.Stationary;
        }

        public static SummonMovementType GetSummonMovementType(Skill summonSkill)
        {
            SummonMovementType summonMovementType = SummonMovementType.Follow;

            switch (summonSkill.MapleID)
            {
                #region stationary
                case (int)SkillNames.Ranger.Puppet:
                    summonMovementType = SummonMovementType.Stationary;
                    break;

                case (int)SkillNames.Sniper.Puppet:
                    summonMovementType = SummonMovementType.Stationary;
                    break;

                case (int)SkillNames.WindArcher3.Puppet:
                    summonMovementType = SummonMovementType.Stationary;
                    break;

                case (int)SkillNames.Outlaw.Octopus:
                    summonMovementType = SummonMovementType.Stationary;
                    break;

                case (int)SkillNames.Corsair.WrathoftheOctopi:
                    summonMovementType = SummonMovementType.Stationary;
                    break;
                #endregion

                #region circle
                case (int)SkillNames.Bowmaster.Phoenix:
                    summonMovementType = SummonMovementType.CircleFollow;
                    break;

                case (int)SkillNames.Marksman.Frostprey:
                    summonMovementType = SummonMovementType.CircleFollow;
                    break;
                #endregion

                #region teleport
                case (int)SkillNames.FirePoisonArchMage.Elquines:
                    summonMovementType = SummonMovementType.TeleportFollow;
                    break;

                case (int)SkillNames.IceLightningArchMage.Ifrit:
                    summonMovementType = SummonMovementType.TeleportFollow;
                    break;

                case (int)SkillNames.Priest.SummonDragon:
                    summonMovementType = SummonMovementType.TeleportFollow;
                    break;

                case (int)SkillNames.Bishop.Bahamut:
                    summonMovementType = SummonMovementType.TeleportFollow;
                    break;

                case (int)SkillNames.BlazeWizard3.Ifrit:
                    summonMovementType = SummonMovementType.TeleportFollow;
                    break;
                #endregion

                #region follow
                case (int)SkillNames.DarkKnight.Beholder:
                    summonMovementType = SummonMovementType.Follow;
                    break;

                case (int)SkillNames.BlazeWizard.Flame:
                    summonMovementType = SummonMovementType.Follow;
                    break;

                case (int)SkillNames.DawnWarrior.Soul:
                    summonMovementType = SummonMovementType.Follow;
                    break;

                case (int)SkillNames.WindArcher.Storm:
                    summonMovementType = SummonMovementType.Follow;
                    break;

                case (int)SkillNames.NightWalker.Darkness:
                    summonMovementType = SummonMovementType.Follow;
                    break;

                case (int)SkillNames.ThunderBreaker.LightningSprite:
                    summonMovementType = SummonMovementType.Follow;
                    break;
                #endregion

                default:
                    summonMovementType = SummonMovementType.Follow;
                    break;
            }

            return summonMovementType;
        }

        public static Summon GetNewSummonFromSkill(Skill sumSkill)
        {
            Summon entityToSummon = null;
          
            switch (sumSkill.MapleID)
            {
                #region stationarySummons
                case (int)SkillNames.Ranger.Puppet:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)SkillNames.Sniper.Puppet:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)SkillNames.Outlaw.Octopus:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)SkillNames.Corsair.WrathoftheOctopi:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)SkillNames.WindArcher3.Puppet:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;
                #endregion

                #region circleSummons
                case (int)SkillNames.Bowmaster.Phoenix:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)SkillNames.Marksman.Frostprey:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;
                #endregion

                #region teleportSummons
                case (int)SkillNames.FirePoisonArchMage.Elquines:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)SkillNames.IceLightningArchMage.Ifrit:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)SkillNames.Priest.SummonDragon:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)SkillNames.Bishop.Bahamut:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)SkillNames.BlazeWizard3.Ifrit:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;
                #endregion

                #region followSummons
                case (int)SkillNames.DarkKnight.Beholder:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)SkillNames.BlazeWizard.Flame:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)SkillNames.DawnWarrior.Soul:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)SkillNames.WindArcher.Storm:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)SkillNames.NightWalker.Darkness:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)SkillNames.ThunderBreaker.LightningSprite:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;
                #endregion

                default:
                    entityToSummon = null;
                    break;
            }

            return entityToSummon;
        }

        public Packet GetCreatePacket()
        {
            return GetSpawnPacket();
        }

        public Packet GetSpawnPacket()
        {
            Packet oPacket = new Packet(ServerOperationCode.SummonedCreated);

            oPacket
                .WriteInt(this.summonOwner.ID)
                .WriteInt(this.ObjectID)
                .WriteInt(this.summonSkill.MapleID)
                .WriteByte(0x0A) //v83 ?? magic number :D
                .WriteByte(this.summonSkill.CurrentLevel)
                .WriteInt(this.summonSkill.MapleID)
                .WriteShort(this.Position.X)
                .WriteShort(this.Position.Y)
                .Skip(3) // flags??
                .WriteByte((byte)this.movementType) // summon movement flag
                .WriteByte(IsPuppet(this.summonSkill) ? (byte)0 : (byte)1) // summon attack flag?
                .WriteByte(this.animated ? (byte)0 : (byte)1);  // summon animation flag?

            return oPacket;
        }

        public Packet GetDestroyPacket()
        {
            Packet oPacket = new Packet(ServerOperationCode.SummonedRemoved);

            oPacket
                .WriteInt(this.summonOwner.ID)
                .WriteInt(this.ObjectID)
                .WriteByte((byte)1); //if animatedDeath then byte 4

            return oPacket;
        }
    }
}