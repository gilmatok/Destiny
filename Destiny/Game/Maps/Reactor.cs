using System;

namespace Destiny.Game.Maps
{
    public sealed class Reactor : MapObject
    {
        public int MapleID { get; private set; }
        public short MimimumClickX { get; private set; }
        public short MaximumClickX { get; private set; }
        public int RespawnTime { get; private set; }
        public string Label { get; private set; }

        public override MapObjectType Type
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Reactor(int mapleID)
        {
            this.MapleID = mapleID;
        }

        //public Reactor(MapData.MapReactorData data)
        //    : this(data.ReactorIdentifier)
        //{
        //    this.Flags = data.Flags;
        //    this.Foothold = data.Foothold;
        //    this.Position = new Point(data.X, data.Y);
        //    this.Label = data.Name;
        //}
    }
}
