namespace Destiny.Game
{
    public sealed class Map
    {
        public int MapleID { get; private set; }
        public int ReturnMap { get; private set; }
        public int ForcedReturn { get; private set; }

        public void Broadcast(byte[] buffer)
        {

        }
    }
}
