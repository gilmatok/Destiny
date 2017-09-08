namespace Destiny.Maple.Maps
{
    public abstract class MapObject
    {
        public Map Map { get; set; }
        public int ObjectID { get; set; }
        public Point Position { get; set; }
    }
}
