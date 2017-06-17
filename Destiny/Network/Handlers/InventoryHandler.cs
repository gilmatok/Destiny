using Destiny.Core.IO;

namespace Destiny.Handler
{
    public static class InventoryHandler
    {
        public static void HandleItemMovement(MapleClient client, InPacket iPacket)
        {
            //iPacket.Skip(4); // NOTE: tRequestTime (ticks).
            //InventoryType inventory = (InventoryType)iPacket.ReadByte();
            //short slot1 = iPacket.ReadShort();
            //short slot2 = iPacket.ReadShort();
            //short quantity = iPacket.ReadShort();

            //bool dropped = slot2 == 0;
            //bool equippedSlot1 = slot1 < 0;
            //bool equippedSlot2 = slot2 < 0;

            //if (dropped)
            //{

            //}
            //else
            //{
            //    client.Character.Items.Swap(inventory, slot1, slot2);
            //}

            //if (equippedSlot1 || equippedSlot2)
            //{

            //}
        }
    }
}
