using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BTL_Mongodb.Models
{
    public class RolePermission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonRepresentation(BsonType.ObjectId)]
        public string RoleId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string PermissionId { get; set; }
         public DateTime GrantedAt { get; set; }  
    }
}
