namespace Destiny.Maple.Maps
{
    public abstract class MapObject
    {
        public Map Map { get; set; }
        public int ObjectID { get; set; }
        public Point Position { get; set; }

        /*public Point Position = new Point(0,0);  

        public Point getPosition()
        {
        return Position;
        }

        public void setPosition(Point position)
        {
        this.Position.X = position.X;
        this.Position.Y = position.Y;
        }*/

        public MapleMapObjectType Type { get; set; }

        public enum MapleMapObjectType
        {
            Npc,
            Monster,
            Item,
            Player,
            Door,
            Summon,
            Shop,
            MiniGame,
            Mist,
            Reactor,
            HiredMerchant,
            PlayerNpc
        }
    }
}
