using static Destiny.Constants.CharacterConstants;
using Destiny.Maple.Characters;
using Destiny.Maple.Maps;
using Destiny.Network;

namespace Destiny.Maple.Life
{
    public sealed class Mist : MapObject, ISpawnable
    {
        private Rectangle mistPosition { get; set; }
        public Character mistOwner { get; set; }
        public Skill mistSkill { get; set; }

        public MapleMapObjectType getType()
        {
            return MapleMapObjectType.Mist;
        }

        public enum MistType : int
        {
            mobMist = 0,
            playerPoisonMist = 1,
            playerSmokescreen = 2,
            unknown = 3, 
            recoveryMist = 4
            // flame gear
            // poison bomb
        }

        public MistType mistType { get; set; }

        public MistType getMistType()
        {
            switch (this.mistSkill.MapleID)
            {
                case (int) SkillNames.FirePoisonMage.PoisonMist:
                    return MistType.playerPoisonMist;

                case (int) SkillNames.Shadower.Smokescreen:
                    return MistType.playerSmokescreen;
            }

            return MistType.mobMist;
        }

        public Mist(Rectangle boundingBox, Character character, Skill skill)
        {
            this.ObjectID = ObjectID;
            this.mistSkill = skill;
            this.mistType = getMistType();
            this.mistOwner = character;          
            this.mistPosition = boundingBox;
        }

        public Mist MistObject(Rectangle boundingBox, Character character, Skill skill)
        {
            this.ObjectID = ObjectID;
            this.mistSkill = skill;
            this.mistType = getMistType();
            this.mistOwner = character;
            this.mistPosition = boundingBox;

            return this;
        }

        public static void SpawnMist(GameClient client, Mist mistToSpawn)
        {
            Packet mistPacket = mistToSpawn.GetCreatePacket();

            client.Send(mistPacket);
        }

        public Packet GetCreatePacket()
        {
            return this.GetSpawnPacket();
        }

        public Packet GetSpawnPacket()
        {
            return this.GetInternalPacket();
        }

        private Packet GetInternalPacket()
        {
            Packet oPacket = new Packet(ServerOperationCode.AffectedAreaCreated);

                    oPacket
                        .WriteInt(this.ObjectID)
                        .WriteInt((int)this.mistType) 
                        .WriteInt(this.mistOwner.ID)
                        .WriteInt(this.mistSkill.MapleID)
                        .WriteByte(this.mistSkill.CurrentLevel)
                        .WriteShort((short)this.mistSkill.Cooldown)
                        .WriteInt(this.mistPosition.RB.X)
                        .WriteInt(this.mistPosition.RB.Y)
                        .WriteInt(this.mistPosition.RB.X + this.mistPosition.LT.Y)
                        .WriteInt(this.mistPosition.RB.Y + this.mistPosition.LT.Y)
                        .WriteInt(0); // ???
                    
            return oPacket;
        }

        public Packet GetDestroyPacket()
        {
            Packet oPacket = new Packet(ServerOperationCode.AffectedAreaRemoved);

            oPacket.WriteInt(this.ObjectID);

            return oPacket;
        }

    }
}