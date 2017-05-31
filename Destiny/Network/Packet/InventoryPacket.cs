using Destiny.Core.IO;
using Destiny.Game;
using Destiny.Network.Handler;

namespace Destiny.Network.Packet
{
    public static class InventoryPacket
    {
        public static byte[] InventoryOperation(bool unk, params InventoryOperation[] operations)
        {
            using (OutPacket oPacket = new OutPacket(SendOps.InventoryOperation))
            {
                oPacket
                    .WriteBool(unk)
                    .WriteByte((byte)operations.Length);

                sbyte addedByte = -1;

                foreach(InventoryOperation operation in operations)
                {
                    oPacket
                        .WriteByte((byte)operation.Type)
                        .WriteByte((byte)(((byte)Item.GetInventory(operation.Item.MapleID)) + 1));

                    switch (operation.Type)
                    {
                        case InventoryOperationType.AddItem:
                            {
                                oPacket.WriteShort(operation.CurrentSlot);
                                operation.Item.Encode(oPacket);
                            }
                            break;

                        case InventoryOperationType.ModifyQuantity:
                            {
                                oPacket
                                    .WriteShort(operation.CurrentSlot)
                                    .WriteShort(operation.Item.Quantity);
                            }
                            break;

                        case InventoryOperationType.ModifySlot:
                            {
                                oPacket
                                    .WriteShort(operation.OldSlot)
                                    .WriteShort(operation.CurrentSlot);

                                if (addedByte == -1)
                                {
                                    if (operation.OldSlot < 0)
                                    {
                                        addedByte = 1;
                                    }
                                    else if (operation.CurrentSlot < 0)
                                    {
                                        addedByte = 2;
                                    }
                                }
                            }
                            break;
                        case InventoryOperationType.RemoveItem:
                            {
                                oPacket.WriteShort(operation.CurrentSlot);
                            }
                            break;
                    }
                }

                if (addedByte != -1)
                {
                    oPacket.WriteSByte(addedByte);
                }

                return oPacket.ToArray();
            }
        }
    }
}
