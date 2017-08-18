using Destiny.Maple.Characters;
using Destiny.Maple.Social;
using System.Collections.ObjectModel;

namespace Destiny.Network
{
    public sealed class PartyRegistery : KeyedCollection<int, Party>
    {
        private int Counter { get; set; }

        public PartyRegistery() : base() { }

        public Party Create(Character leader)
        {
            Party party = new Party(++this.Counter);

            party.Add(leader);

            return party;
        }

        protected override int GetKeyForItem(Party item)
        {
            return item.ID;
        }
    }
}
