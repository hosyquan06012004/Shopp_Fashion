using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BTL_Mongodb.Models
{
    public class Role
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
