using Destiny.Server;

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

        public Mob(int mapleID)
        {
            this.MapleID = mapleID;
        }
    }
}
