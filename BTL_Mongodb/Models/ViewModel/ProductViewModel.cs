using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BTL_Mongodb.Models.ViewModel
{
    public class ProductViewModel
    {
        [BsonId] // Chỉ định đây là _id trong MongoDB
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")] // Ánh xạ _id từ MongoDB sang Id trong model
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Description")]
        public string? Description { get; set; }

        [BsonElement("Price")]
        public double? Price { get; set; }

        [BsonElement("CategoryName")]
        public string CategoryName { get; set; }

        [BsonElement("BrandName")]
        public string BrandName { get; set; }

        [BsonElement("Image")]
        public string Image { get; set; }

        [BsonElement("Images")]
        public List<string> Images { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime? CreatedAt { get; set; }
    }
}
