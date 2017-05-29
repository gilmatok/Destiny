namespace Destiny.Game
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
    }
}
