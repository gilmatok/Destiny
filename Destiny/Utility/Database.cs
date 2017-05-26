using Destiny.Game;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

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

        public Character GetCharacter(int id)
        {
            var collection = mDatabase.GetCollection<Character>("characters");
            var filter = new BsonDocument("CharacterId", id);

            return collection.Find(filter).FirstOrDefault();
        }

        public List<Character> GetCharacters(int accountId, byte worldId)
        {
            var collection = mDatabase.GetCollection<Character>("characters");
            var elements = new List<BsonElement>(new BsonElement[] { new BsonElement("AccountId", accountId), new BsonElement("WorldId", worldId) });
            var filter = new BsonDocument(elements);

            return collection.Find(filter).ToList();
        }

        public bool CharacterNameTaken(string name)
        {
            var collection = mDatabase.GetCollection<Character>("characters");
            var filter = new BsonDocument("Name", name);

            return collection.Find(filter).Any();
        }

        public void AddCharacter(Character character)
        {
            var collection = mDatabase.GetCollection<Character>("characters");

            character.CharacterId = this.GetCharacterID();

            collection.InsertOne(character);
        }

        private int GetAccountID()
        {
            var collection = mDatabase.GetCollection<Account>("accounts");
            var filter = new BsonDocument();

            int id = 1;

            foreach (var account in collection.Find(filter).ToList())
            {
                if (account.AccountId > id)
                {
                    id = account.AccountId;
                }
            }

            return id + 1;
        }

        private int GetCharacterID()
        {
            var collection = mDatabase.GetCollection<Character>("characters");
            var filter = new BsonDocument();

            int id = 1;

            foreach (var character in collection.Find(filter).ToList())
            {
                if (character.CharacterId > id)
                {
                    id = character.CharacterId;
                }
            }

            return id + 1;
        }
    }
}
