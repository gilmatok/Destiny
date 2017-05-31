namespace Destiny.Game.Maps
{
    public abstract class MapObject
    {
        public abstract MapObjectType Type { get; }
        public Map Map { get; set; }
        public int ObjectID { get; set; }
        public Point Position { get; set; }
    }
}
