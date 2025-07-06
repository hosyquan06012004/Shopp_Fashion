using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using BTL_Mongodb.Models.Validations;
namespace BTL_Mongodb.Models
{
    public class Brand
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        [Required(ErrorMessage = "Tên thương hiệu không được để trống")]
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("logo")]
        public string? Logo { get; set; }
        [BsonElement("createdAt")]
        [DateNotInFuture(ErrorMessage = "Ngày tạo không được lớn hơn ngày hiện tại")]
        [Required(ErrorMessage = "Ngày thành lập không được để trống")]
        public DateTime? CreatedAt { get; set; }
    }
}
