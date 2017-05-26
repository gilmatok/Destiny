using MongoDB.Driver;

namespace Destiny.Utility
{
    public sealed class Database
    {
        private readonly IMongoClient mClient;
        private readonly IMongoDatabase mDatabase;

        public Database(string connectionString, string databaseName)
        {
            mClient = new MongoClient(connectionString);
            mDatabase = mClient.GetDatabase(databaseName);
        }
    }
}
