using Destiny.Core.IO;
using Destiny.Game;

namespace Destiny.Network.Handler
{
    public enum InventoryOperationType : byte
    {
        AddItem,
        ModifyQuantity,
        ModifySlot,
        RemoveItem
    }

    public sealed class InventoryOperation
    {
        public InventoryOperationType Type { get; private set; }
        public Item Item { get; private set; }
        public short OldSlot { get; private set; }
        public short CurrentSlot { get; private set; }

        public InventoryOperation(InventoryOperationType type, Item item, short oldSlot, short currentSlot)
        {
            this.Type = type;
            this.Item = item;
            this.OldSlot = oldSlot;
            this.CurrentSlot = currentSlot;
        }
    }

    public static class InventoryHandler
    {
        public static void HandleItemMove(MapleClient client, InPacket iPacket)
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
    }
}
