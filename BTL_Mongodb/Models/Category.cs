using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace BTL_Mongodb.Models
{
    public class Category
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("name")]
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên danh mục phải từ 3 đến 100 ký tự")]
        public string Name { get; set; }

        [BsonElement("status")]
        [Required(ErrorMessage = "Trạng thái không được để trống")]
        public CategoryStatus? Status { get; set; }

    }

}
