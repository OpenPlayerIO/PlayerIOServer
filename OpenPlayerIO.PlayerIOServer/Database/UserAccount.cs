using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OpenPlayerIO.PlayerIOServer.Database
{
    public enum AccountType { Simple = 0 }

    public class UserAccount
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public AccountType Type { get; set; }
        public string Name { get; set; }
        public string ConnectUserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long Registered { get; set; }
    }
}
