using Destiny.Game;
using MongoDB.Bson;
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

        public Account GetAccount(string username)
        {
            var collection = mDatabase.GetCollection<Account>("accounts");
            var filter = new BsonDocument("Username", username);

            return collection.Find(filter).FirstOrDefault();
        }

        public void AddAccount(Account account)
        {
            var collection = mDatabase.GetCollection<Account>("accounts");

            account.AccountId = this.GetAccountID();

            collection.InsertOne(account);
        }

        private int GetAccountID()
        {
            var collection = mDatabase.GetCollection<Account>("accounts");
            var filter = new BsonDocument();

            int id = -1;

            foreach (var account in collection.Find(filter).ToList())
            {
                if (account.AccountId > id)
                {
                    id = account.AccountId;
                }
            }

            return id + 1;
        }
    }
}
