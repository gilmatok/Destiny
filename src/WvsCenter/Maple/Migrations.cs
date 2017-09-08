using System;
using System.Collections.ObjectModel;

namespace Destiny.Maple
{
    public sealed class Migrations : KeyedCollection<string, Migration>
    {
        public int Validate(string host, int characterID)
        {
            foreach (Migration migration in this)
            {
                if (migration.Host == host && migration.CharacterID == characterID)
                {
                    this.Remove(migration);

                    if ((DateTime.Now - migration.Expiry).TotalSeconds > 30)
                    {
                        return 0;
                    }

                    return migration.AccountID;
                }
            }

            return 0;
        }

        protected override string GetKeyForItem(Migration item)
        {
            return item.Host;
        }
    }
}
