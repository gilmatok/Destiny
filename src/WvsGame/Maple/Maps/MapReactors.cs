using Destiny.Network;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using Destiny.Maple.Life;
using Destiny.Threading;

namespace Destiny.Maple.Maps
{
    public sealed class MapReactors : MapObjects<Reactor>
    {
        public MapReactors(Map map) : base(map) { }

        protected override void InsertItem(int index, Reactor item)
        {
            lock (this)
            {
                base.InsertItem(index, item);

                if (DataProvider.IsInitialized)
                {
                    using (Packet oPacket = item.GetCreatePacket())
                    {
                        this.Map.Broadcast(oPacket);
                    }
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            lock (this)
            {
                Reactor item = base.Items[index];

                if (DataProvider.IsInitialized)
                {
                    using (Packet oPacket = item.GetDestroyPacket())
                    {
                        this.Map.Broadcast(oPacket);
                    }
                }

                base.RemoveItem(index);

                if (item.SpawnPoint != null)
                {
                    Delay.Execute(() => item.SpawnPoint.Spawn(), (item.SpawnPoint.RespawnTime <= 0 ? 30 : item.SpawnPoint.RespawnTime) * 100);
                }
            }
        }

        public void Hit(Packet iPacket, Character character)
        {
            int objectID = iPacket.ReadInt();

            if (!this.Contains(objectID))
            {
                return;
            }

            Point characterPosition = new Point(iPacket.ReadShort(), iPacket.ReadShort());
            short actionDelay = iPacket.ReadShort();
            iPacket.ReadInt(); // NOTE: Unknown
            int skillID = iPacket.ReadInt();

            Reactor reactor = this.Map.Reactors[objectID];

            bool valid = true; // TODO: Validate position between attacker and reactor.

            if (valid)
            {
                reactor.Hit(character, actionDelay, skillID);
            }
        }

        public void Touch(Packet iPacket, Character character)
        {
            
        }
    }
}
