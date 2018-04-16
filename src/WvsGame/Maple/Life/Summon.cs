using Destiny.Constants;
using Destiny.Maple.Characters;
using Destiny.Maple.Maps;
using Destiny.Network;

namespace Destiny.Maple.Life
{
    public sealed class Summon : MapObject, ISpawnable
    {
        public Character summonOwner { get; set; }
        public Skill summonSkill { get; set; }
        public Point summonPosition { get; set; }  
        public int health { get; set; }

        public SummonConstants.SummonMovementType movementType;

        public MapleMapObjectType getType()
        {
            return MapleMapObjectType.Summon;
        }

        public bool puppet
        {
            get
            {
                return summonSkill.MapleID == (int)CharacterConstants.SkillNames.Ranger.Puppet
                       || summonSkill.MapleID == (int)CharacterConstants.SkillNames.Sniper.Puppet;
            }
        }

        public bool animated { get; set; }

        public bool stationary
        {
            get
            {
                return puppet || summonSkill.MapleID == (int)CharacterConstants.SkillNames.Outlaw.Octopus;
            }
        }

        public Packet GetCreatePacket()
        {
            return GetSpawnPacket();
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
                .WriteShort(this.summonPosition.X)
                .WriteShort(this.summonPosition.Y)
                .Skip(3) // flags??
                .WriteByte((byte)this.movementType) // summon movement flag
                .WriteByte(this.puppet ? (byte)0 : (byte)1) // summon attack flag?
                .WriteByte(this.animated ? (byte)0 : (byte)1);  // summon animation flag?

            return oPacket;
        }

    }
}