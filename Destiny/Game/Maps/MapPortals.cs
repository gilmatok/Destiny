namespace Destiny.Game.Maps
{
    public sealed class MapPortals : MapObjects<Portal>
    {
        public MapPortals(Map map) : base(map) { }

        public Portal this[string label]
        {
            get
            {
                foreach (Portal portal in this)
                {
                    if (portal.Data.Label == label)
                    {
                        return portal;
                    }
                }

                return null;
            }
        }

        protected override int GetKeyForItem(Portal item)
        {
            return item.ID;
        }
    }
}
