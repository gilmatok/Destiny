using System;

namespace Destiny.Maple
{
    public sealed class InventoryFullException : Exception
    {
        public InventoryFullException() : base("The inventory is full.") { }
    }
}
