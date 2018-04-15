using Destiny.Maple.Characters;
using Destiny.Maple.Maps;
using Destiny.Network;

namespace Destiny.Maple.Life
{
    public sealed class Summon : MapObject, ISpawnable
    {
        private Rectangle summonPosition { get; set; }
        public Character summonOwner { get; set; }
        public Skill summonSkill { get; set; }

        public Packet GetCreatePacket()
        {
            throw new System.NotImplementedException();
        }
        public Packet GetDestroyPacket()
        {
            throw new System.NotImplementedException();
        }

        public Packet GetSpawnPacket()
        {
            throw new System.NotImplementedException();
        }

    }
}