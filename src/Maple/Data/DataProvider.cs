using Destiny.Data;
using Destiny.Maple.Commands;
using System.Diagnostics;

namespace Destiny.Maple.Data
{
    public static class DataProvider
    {
        public static bool IsInitialized { get; private set; }

        public static AvailableStyles AvailableStyles { get; private set; }
        public static CachedItems CachedItems { get; private set; }
        public static CachedSkills CachedSkills { get; private set; }
        public static CachedMaps CachedMaps { get; private set; }
        public static CachedQuests CachedQuests { get; private set; }
        public static CreationData CharacterCreationData { get; private set; }

        public static void Initialize()
        {
            using (Database.TemporarySchema("mcdb"))
            {
                Database.Test();

                Stopwatch sw = new Stopwatch();

                sw.Start();

                Log.Inform("Loading data...");

                DataProvider.AvailableStyles = new AvailableStyles();
                DataProvider.CachedItems = new CachedItems();
                DataProvider.CachedSkills = new CachedSkills();
                DataProvider.CachedMaps = new CachedMaps();
                DataProvider.CachedQuests = new CachedQuests();
                DataProvider.CharacterCreationData = new CreationData();

                CommandFactory.Initialize();

                sw.Stop();

                Log.Success("Maple data loaded in {0}ms.", sw.ElapsedMilliseconds);

                DataProvider.IsInitialized = true;
            }
        }
    }
}
