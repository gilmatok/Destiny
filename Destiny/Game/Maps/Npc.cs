using Destiny.Server;

namespace Destiny.Game.Maps
{
    public sealed class Npc : MapObject
    {
        public int MapleID { get; private set; }
        public short MinimumClickX { get; private set; }
        public short MaximumClickX { get; private set; }
        public bool Flip { get; private set; }
        public int StorageCost { get; private set; }

        public override MapObjectType Type
        {
            get
            {
                return MapObjectType.Npc;
            }
        }

        //public Npc(int mapleID)
        //{
        //    NpcData data = MasterServer.Instance.Data.Npcs[mapleID];

        //    this.MapleID = mapleID;
        //    this.StorageCost = data.StorageCost;
        //}

        //public Npc(MapData.MapNpcData data)
        //    : this(data.Identifier)
        //{
        //    this.Foothold = data.Foothold;
        //    this.Position = new Point(data.X, data.Y);
        //    this.MinimumClickX = data.MinClickX;
        //    this.MaximumClickX = data.MaxClickX;
        //    this.Flip = (data.Flags & MapData.MapNpcData.EMapNpcFlags.FacesLeft) != 0;
        //}
    }
}
