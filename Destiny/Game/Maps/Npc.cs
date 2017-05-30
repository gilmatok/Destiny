using System.IO;

namespace Destiny.Game.Maps
{
    public sealed class Npc : MapObject
    {
        public int MapleID { get; private set; }
        public bool Flip { get; private set; }
        public short MinimumClickX { get; private set; }
        public short MaximumClickX { get; private set; }
        public bool Hide { get; private set; }

        public override MapObjectType Type
        {
            get
            {
                return MapObjectType.Npc;
            }
        }

        public Npc(BinaryReader reader)
        {
            this.MapleID = reader.ReadInt32();
            this.Position = new Point(reader.ReadInt16(), reader.ReadInt16());
            this.Foothold = reader.ReadInt16();
            this.Flip = reader.ReadBoolean();
            this.MinimumClickX = reader.ReadInt16();
            this.MaximumClickX = reader.ReadInt16();
            this.Hide = reader.ReadBoolean();
            reader.ReadInt32();
        }
    }
}
