namespace Destiny.Maple
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
}
