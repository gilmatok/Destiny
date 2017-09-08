using System.Collections.ObjectModel;

namespace Destiny.Maple
{
    public sealed class Worlds : KeyedCollection<byte, World>
    {
        public Worlds() : base() { }
    
        protected override byte GetKeyForItem(World item)
        {
            return item.ID;
        }
    }
}
