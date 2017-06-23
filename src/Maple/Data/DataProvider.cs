using Destiny.Maple.Commands;
using Destiny.Utility;
using System.Diagnostics;

namespace Destiny.Maple.Data
{
    public static class DataProvider
    {
        public static bool IsInitialized { get; private set; }

        public static CachedItems CachedItems { get; private set; }
        public static CachedMaps CachedMaps { get; private set; }

        public static void Initialize()
        {
            using (Database.TemporarySchema("mcdb"))
            {
                DataProvider.IsInitialized = false;

                if (DataProvider.CachedItems != null)
                {
                    DataProvider.CachedItems.Clear();
                }

                if (DataProvider.CachedMaps!= null)
                {
                    DataProvider.CachedMaps.Clear();
                }

                Database.Test();

                Stopwatch sw = new Stopwatch();

                sw.Start();

                Log.Inform("Loading data...");

                DataProvider.CachedItems = new CachedItems();
                DataProvider.CachedMaps = new CachedMaps();

                CommandFactory.Initialize();

                sw.Stop();

                Log.Success("Maple data loaded in {0}ms.", sw.ElapsedMilliseconds);

                DataProvider.IsInitialized = true;
            }
        }
    }
}
