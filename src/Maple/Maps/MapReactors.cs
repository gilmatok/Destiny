using Destiny.Core.IO;
using Destiny.Maple.Characters;
using Destiny.Maple.Data;
using Destiny.Maple.Life.Reactors;

namespace Destiny.Maple.Maps
{
    public sealed class MapReactors : MapObjects<Reactor>
    {
        public MapReactors(Map map) : base(map) { }

        protected override void InsertItem(int index, Reactor item)
        {
            base.InsertItem(index, item);

            if (DataProvider.IsInitialized)
            {
                using (OutPacket oPacket = item.GetCreatePacket())
                {
                    this.Map.Broadcast(oPacket);
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            if (DataProvider.IsInitialized)
            {
                Reactor item = base.Items[index];

                using (OutPacket oPacket = item.GetDestroyPacket())
                {
                    this.Map.Broadcast(oPacket);
                }
            }

            base.RemoveItem(index);
        }

        public void Hit(InPacket iPacket, Character character)
        {
            int objectID = iPacket.ReadInt();
            int characterPos = iPacket.ReadInt();
            short actionDelay = iPacket.ReadShort();
            iPacket.ReadInt(); //NOTE: Unknown
            int skillID = iPacket.ReadInt();

            Reactor reactor = this.Map.Reactors[objectID];

            //TODO: Validate that char was in a valid position to hit
            bool valid = true;
            if (valid)
            {
                reactor.Hit(actionDelay, skillID);
            }
        }

        public void Touch(InPacket iPacket, Character character)
        {
            //NOTE: This packet is used only on map 610030400, "The Test of Wit"
            int objectID = iPacket.ReadInt();
            byte state = iPacket.ReadByte();

            Reactor reactor = this.Map.Reactors[objectID];
            if (reactor != null && reactor.ActivateByTouch)
            {
                //TODO
            }
        }
    }
}
