using MongoDB.Bson;

namespace Destiny.Game
{
    public sealed class Account
    {
        public ObjectId _id { get; set; }
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public void Save()
        {

        }
    }
}
