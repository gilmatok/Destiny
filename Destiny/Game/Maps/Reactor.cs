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
                return MapObjectType.Reactor;
            }
        }

        public Reactor(int mapleID)
        {
            this.MapleID = mapleID;
        }
    }
}
