using System;

namespace Destiny.Game
{
    public sealed class InventoryFullException : Exception
    {
        public InventoryFullException() : base("The inventory is full.") { }
    }
}
