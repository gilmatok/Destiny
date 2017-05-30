namespace Destiny.Game.Maps
{
    public sealed class MapPortals : MapObjects<Portal>
    {
        public MapPortals(Map map) : base(map) { }

        protected override int GetKeyForItem(Portal item)
        {
            return item.ID;
        }
    }
}
