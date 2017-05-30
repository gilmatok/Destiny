using System.IO;

namespace Destiny.Game.Maps
{
    public sealed class Mob : MapObject
    {
        public override MapObjectType Type
        {
            get
            {
                return MapObjectType.Mob;
            }
        }

        public int MapleID { get; private set; }
        public int RespawnTime { get; private set; }

        public Mob(int mapleID)
        {
            this.MapleID = mapleID;

            this.Stance = 5;
        }

        public Mob(BinaryReader reader)
            : this(reader.ReadInt32())
        {
            this.Position = new Point(reader.ReadInt16(), reader.ReadInt16());
            this.Foothold = reader.ReadInt16();
            reader.ReadBoolean();
            reader.ReadInt16();
            reader.ReadInt16();
            reader.ReadBoolean();
            this.RespawnTime = reader.ReadInt32();
        }
    }
}
