using System.Collections.ObjectModel;

namespace Destiny.Maple.Commands
{
    public sealed class Commands : KeyedCollection<string, Command>
    {
        protected override string GetKeyForItem(Command item)
        {
            return item.Name;
        }
    }
}
