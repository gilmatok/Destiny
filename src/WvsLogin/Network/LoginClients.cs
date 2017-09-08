using System.Collections.ObjectModel;

namespace Destiny.Network
{
    public sealed class LoginClients : KeyedCollection<long, LoginClient>
    {
        protected override long GetKeyForItem(LoginClient item)
        {
            return item.ID;
        }
    }
}
