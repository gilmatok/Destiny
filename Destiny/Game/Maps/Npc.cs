using Destiny.Data;

namespace Destiny.Game
{
    public sealed class Npc : MapObject
    {
        public int MapleID { get; private set; }
        public bool Flip { get; private set; }
        public short MinClickPos { get; private set; }
        public short MaxClickPos { get; private set; }

        public override MapObjectType Type
        {
            get
            {
                return MapObjectType.Npc;
            }
        }

        public Npc(int identifier)
        {
            this.MapleID = identifier;
        }

        public Npc(MapData.MapNpcData spawn)
            : this(spawn.Identifier)
        {
            this.Flip = (spawn.Flags & MapData.MapNpcData.EMapNpcFlags.FacesLeft) != 0;
            this.Foothold = spawn.Foothold;
            this.Position = new Point(spawn.X, spawn.Y);
            this.MinClickPos = spawn.MinClickX;
            this.MaxClickPos = spawn.MaxClickX;
        }
    }
}
