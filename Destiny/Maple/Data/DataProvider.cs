using Destiny.Maple.Commands;
using Destiny.Utility;

namespace Destiny.Maple.Data
{
    public static class DataProvider
    {
        public static bool IsInitialized { get; private set; }

        public static CachedItems CachedItems { get; private set; }

        public static void Initialize()
        {
            using (Database.TemporarySchema("mcdb"))
            {
                DataProvider.IsInitialized = false;

                if (DataProvider.CachedItems != null)
                {
                    DataProvider.CachedItems.Clear();
                }

                DataProvider.CachedItems = new CachedItems();

                CommandFactory.Initialize();

                DataProvider.IsInitialized = true;
            }
        }
    }
}
