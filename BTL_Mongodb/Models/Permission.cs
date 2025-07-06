using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BTL_Mongodb.Models
{
    public class Permission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("ModuleName")]
        public string Module { get; set; }

        [BsonElement("ActionName")]
        public string Action { get; set; }

        public string Description { get; set; }
    }
}
