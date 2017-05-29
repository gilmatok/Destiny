namespace Destiny.Game
{
    public abstract class MapObject : Moveable
    {
        public abstract MapObjectType Type { get; }
        public Map Map { get; set; }
        public int ObjectID { get; set; }
    }
}
