using System.Collections.ObjectModel;

namespace Destiny.Maple
{
    public sealed class Worlds : KeyedCollection<byte, World>
    {
        internal Worlds() : base() { }

        internal World Next(ServerType type)
        {
            lock (this)
            {
                foreach (World loopWorld in this)
                {
                    if (type == ServerType.Channel && loopWorld.IsFull)
                    {
                        continue;
                    }
                    else if (type == ServerType.Shop && loopWorld.HasShop)
                    {
                        continue;
                    }

                    return loopWorld;
                }

                return null;
            }
        }

        protected override byte GetKeyForItem(World item)
        {
            return item.ID;
        }
    }
}
