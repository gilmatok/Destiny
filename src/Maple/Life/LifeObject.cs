using Destiny.Maple.Maps;
using Destiny.Utility;

namespace Destiny.Maple.Life
{
    public abstract class LifeObject : MapObject
    {
        public int MapleID { get; private set; }
        public short Foothold { get; private set; }
        public short MinimumClickX { get; private set; }
        public short MaximumClickX { get; private set; }
        public bool FacesLeft { get; private set; }

        public LifeObject(DatabaseQuery query)
        {
            this.MapleID = query.GetInt("lifeid");
            this.Position = new Point(query.GetShort("x_pos"), query.GetShort("y_pos"));
            this.Foothold = query.GetShort("foothold");
            this.MinimumClickX = query.GetShort("min_click_pos");
            this.MaximumClickX = query.GetShort("max_click_pos");
            this.FacesLeft = query.GetString("flags").Contains("faces_left");
        }
    }
}
