namespace Destiny.Maple.Maps
{
    public sealed class Reactor : MapObject
    {
        public int MapleID { get; private set; }
        public short MimimumClickX { get; private set; }
        public short MaximumClickX { get; private set; }
        public int RespawnTime { get; private set; }
        public string Label { get; private set; }
       
        public Reactor(int mapleID)
        {
            this.MapleID = mapleID;
        }
    }
}
