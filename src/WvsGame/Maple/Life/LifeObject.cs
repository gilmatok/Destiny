using Destiny.Data;
using Destiny.Maple.Maps;

namespace Destiny.Maple.Life
{
    public abstract class LifeObject : MapObject
    {
        public int MapleID { get; private set; }
        public short Foothold { get; private set; }
        public short MinimumClickX { get; private set; }
        public short MaximumClickX { get; private set; }
        public bool FacesLeft { get; private set; }
        public int RespawnTime { get; private set; }

        public LifeObject(Datum datum)
        {
            this.MapleID = (int)datum["lifeid"];
            this.Position = new Point((short)datum["x_pos"], (short)datum["y_pos"]);
            this.Foothold = (short)datum["foothold"];
            this.MinimumClickX = (short)datum["min_click_pos"];
            this.MaximumClickX = (short)datum["max_click_pos"];
            this.FacesLeft = ((string)datum["flags"]).Contains("faces_left");
            this.RespawnTime = (int)datum["respawn_time"];
        }
    }
}
