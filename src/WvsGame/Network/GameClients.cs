using System.Collections.ObjectModel;

namespace Destiny.Network
{
    public sealed class GameClients : KeyedCollection<long, GameClient>
    {
        protected override void InsertItem(int index, GameClient item)
        {
            base.InsertItem(index, item);

            WvsGame.CenterConnection.UpdatePopulation(this.Count);
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);

            WvsGame.CenterConnection.UpdatePopulation(this.Count);
        }

        protected override long GetKeyForItem(GameClient item)
        {
            return item.ID;
        }
    }
}
