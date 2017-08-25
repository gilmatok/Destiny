using Destiny.Data;
using Destiny.Maple.Commands;
using System.Diagnostics;

namespace Destiny.Maple.Data
{
    public static class DataProvider
    {
        public static bool IsInitialized { get; private set; }

        public static AvailableStyles Styles { get; private set; }
        public static CachedItems Items { get; private set; }
        public static CachedSkills Skills { get; private set; }
        public static CachedMobs Mobs { get; private set; }
        public static CachedReactors Reactors { get; private set; }
        public static CachedMaps Maps { get; private set; }
        public static CachedQuests Quests { get; private set; }
        public static CreationData CreationData { get; private set; }
        public static CachedStrings Strings { get; private set; }

        public static void Initialize()
        {
            using (Database.TemporarySchema("mcdb"))
            {
                Database.Test();

                Stopwatch sw = new Stopwatch();

                sw.Start();

                Log.Inform("Loading data...");

                DataProvider.Styles = new AvailableStyles();
                DataProvider.Items = new CachedItems();
                DataProvider.Skills = new CachedSkills();
                DataProvider.Mobs = new CachedMobs();
                DataProvider.Reactors = new CachedReactors();
                DataProvider.Maps = new CachedMaps();
                DataProvider.Quests = new CachedQuests();
                DataProvider.CreationData = new CreationData();
                //DataProvider.Strings = new CachedStrings();

                CommandFactory.Initialize();

                sw.Stop();

                Log.Success("Maple data loaded in {0}ms.", sw.ElapsedMilliseconds);

                DataProvider.IsInitialized = true;
            }
        }
    }
}
