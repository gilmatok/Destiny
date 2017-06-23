using Destiny.Core.IO;
using Destiny.Maple;
using Destiny.Maple.Maps;
using System.Collections.Generic;

namespace Destiny.Handler
{
    public static class InventoryHandler
    {
        public static void HandleItemMovement(MapleClient client, InPacket iPacket)
        {
            iPacket.Skip(4); // NOTE: tRequestTime (ticks).
            InventoryType inventory = (InventoryType)iPacket.ReadByte();
            short slot1 = iPacket.ReadShort();
            short slot2 = iPacket.ReadShort();
            short quantity = iPacket.ReadShort();

            bool dropped = slot2 == 0;
            bool equippedSlot1 = slot1 < 0;
            bool equippedSlot2 = slot2 < 0;

            if (dropped)
            {

            }
            else
            {
                client.Character.Items.Swap(inventory, slot1, slot2);
            }

            if (equippedSlot1 || equippedSlot2)
            {

            }
        }

        public static void HandleMesoDrop(MapleClient client, InPacket iPacket)
        {
            iPacket.Skip(4); // NOTE: tRequestTime (ticks).
            int amount = iPacket.ReadInt();

            if (amount > client.Character.Meso || amount < 10 || amount > 50000)
            {
                return;
            }

            client.Character.Meso -= amount;

            Meso meso = new Meso(amount)
            {
                Dropper = client.Character,
                Owner = null
            };

            client.Character.Map.Drops.Add(meso);
        }

        public static void HandlePickup(MapleClient client, InPacket iPacket)
        {
            iPacket.Skip(1);
            iPacket.Skip(4);
            Point position = iPacket.ReadPoint();

            // TODO: Validate distance between picker and position.

            int objectID = iPacket.ReadInt();

            Drop drop;

            try
            {
                drop = client.Character.Map.Drops[objectID];
            }
            catch (KeyNotFoundException)
            {
                return;
            }

            if (drop.Picker != null)
            {
                return;
            }

            try
            {
                drop.Picker = client.Character;

                if (drop is Meso)
                {
                    client.Character.Meso += ((Meso)drop).Amount;
                }
                else if (drop is Item)
                {

                }

                client.Character.Map.Drops.Remove(objectID);

                using (OutPacket oPacket = drop.GetShowGainPacket())
                {
                    client.Send(oPacket);
                }
            }
            catch (InventoryFullException)
            {

            }
        }
    }
}
