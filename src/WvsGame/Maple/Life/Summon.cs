using Destiny.Constants;
using Destiny.Maple.Characters;
using Destiny.Maple.Maps;
using Destiny.Network;

namespace Destiny.Maple.Life
{
    public sealed class Summon : MapObject, ISpawnable
    {
        public SummonConstants.SummonMovementType movementType;
        public Character summonOwner { get; set; }
        public Skill summonSkill { get; set; }
        public int health { get; set; }
        public MapleMapObjectType getType()
        { return MapleMapObjectType.Summon; }
        public bool spawned { get; set; }
        public bool animated { get; set; }

        public Summon(Character owner, Skill skill, Point summonPos, SummonConstants.SummonMovementType movementType)
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
            return summonSkill.MapleID == (int) CharacterConstants.SkillNames.Ranger.Puppet ||
                   summonSkill.MapleID == (int) CharacterConstants.SkillNames.Sniper.Puppet ||
                   summonSkill.MapleID == (int) CharacterConstants.SkillNames.WindArcher3.Puppet;
        }

        public bool IsStationary(Summon summon)
        {
            return GetSummonMovementType(summon.summonSkill) == SummonConstants.SummonMovementType.Stationary;
        }

        public static SummonConstants.SummonMovementType GetSummonMovementType(Skill summonSkill)
        {
            SummonConstants.SummonMovementType summonMovementType = SummonConstants.SummonMovementType.Follow;

            switch (summonSkill.MapleID)
            {
                #region stationary
                case (int)CharacterConstants.SkillNames.Ranger.Puppet:
                    summonMovementType = SummonConstants.SummonMovementType.Stationary;
                    break;

                case (int)CharacterConstants.SkillNames.Sniper.Puppet:
                    summonMovementType = SummonConstants.SummonMovementType.Stationary;
                    break;

                case (int)CharacterConstants.SkillNames.WindArcher3.Puppet:
                    summonMovementType = SummonConstants.SummonMovementType.Stationary;
                    break;

                case (int)CharacterConstants.SkillNames.Outlaw.Octopus:
                    summonMovementType = SummonConstants.SummonMovementType.Stationary;
                    break;

                case (int)CharacterConstants.SkillNames.Corsair.WrathoftheOctopi:
                    summonMovementType = SummonConstants.SummonMovementType.Stationary;
                    break;
                #endregion

                #region circle
                case (int)CharacterConstants.SkillNames.Bowmaster.Phoenix:
                    summonMovementType = SummonConstants.SummonMovementType.CircleFollow;
                    break;

                case (int)CharacterConstants.SkillNames.Marksman.Frostprey:
                    summonMovementType = SummonConstants.SummonMovementType.CircleFollow;
                    break;
                #endregion

                #region teleport
                case (int)CharacterConstants.SkillNames.FirePoisonArchMage.Elquines:
                    summonMovementType = SummonConstants.SummonMovementType.TeleportFollow;
                    break;

                case (int)CharacterConstants.SkillNames.IceLightningArchMage.Ifrit:
                    summonMovementType = SummonConstants.SummonMovementType.TeleportFollow;
                    break;

                case (int)CharacterConstants.SkillNames.Priest.SummonDragon:
                    summonMovementType = SummonConstants.SummonMovementType.TeleportFollow;
                    break;

                case (int)CharacterConstants.SkillNames.Bishop.Bahamut:
                    summonMovementType = SummonConstants.SummonMovementType.TeleportFollow;
                    break;

                case (int)CharacterConstants.SkillNames.BlazeWizard3.Ifrit:
                    summonMovementType = SummonConstants.SummonMovementType.TeleportFollow;
                    break;
                #endregion

                #region follow
                case (int)CharacterConstants.SkillNames.DarkKnight.Beholder:
                    summonMovementType = SummonConstants.SummonMovementType.Follow;
                    break;

                case (int)CharacterConstants.SkillNames.BlazeWizard.Flame:
                    summonMovementType = SummonConstants.SummonMovementType.Follow;
                    break;

                case (int)CharacterConstants.SkillNames.DawnWarrior.Soul:
                    summonMovementType = SummonConstants.SummonMovementType.Follow;
                    break;

                case (int)CharacterConstants.SkillNames.WindArcher.Storm:
                    summonMovementType = SummonConstants.SummonMovementType.Follow;
                    break;

                case (int)CharacterConstants.SkillNames.NightWalker.Darkness:
                    summonMovementType = SummonConstants.SummonMovementType.Follow;
                    break;

                case (int)CharacterConstants.SkillNames.ThunderBreaker.LightningSprite:
                    summonMovementType = SummonConstants.SummonMovementType.Follow;
                    break;
                #endregion

                default:
                    summonMovementType = SummonConstants.SummonMovementType.Follow;
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
                case (int)CharacterConstants.SkillNames.Ranger.Puppet:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)CharacterConstants.SkillNames.Sniper.Puppet:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)CharacterConstants.SkillNames.Outlaw.Octopus:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)CharacterConstants.SkillNames.Corsair.WrathoftheOctopi:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)CharacterConstants.SkillNames.WindArcher3.Puppet:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;
                #endregion

                #region circleSummons
                case (int)CharacterConstants.SkillNames.Bowmaster.Phoenix:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)CharacterConstants.SkillNames.Marksman.Frostprey:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;
                #endregion

                #region teleportSummons
                case (int)CharacterConstants.SkillNames.FirePoisonArchMage.Elquines:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)CharacterConstants.SkillNames.IceLightningArchMage.Ifrit:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)CharacterConstants.SkillNames.Priest.SummonDragon:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)CharacterConstants.SkillNames.Bishop.Bahamut:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)CharacterConstants.SkillNames.BlazeWizard3.Ifrit:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;
                #endregion

                #region followSummons
                case (int)CharacterConstants.SkillNames.DarkKnight.Beholder:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)CharacterConstants.SkillNames.BlazeWizard.Flame:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)CharacterConstants.SkillNames.DawnWarrior.Soul:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)CharacterConstants.SkillNames.WindArcher.Storm:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)CharacterConstants.SkillNames.NightWalker.Darkness:
                    entityToSummon = new Summon(sumSkill.Character, sumSkill, sumSkill.Character.Position, GetSummonMovementType(sumSkill));
                    break;

                case (int)CharacterConstants.SkillNames.ThunderBreaker.LightningSprite:
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