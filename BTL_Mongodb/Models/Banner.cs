using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace BTL_Mongodb.Models
{
    public class Banner
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [Required(ErrorMessage = "tiêu đề lớn H1 không được để trống")]
        [BsonElement("h1")]
        public string H1 { get; set; }

        [Required(ErrorMessage = "tiêu đề lớn nhỏ không được để trống")]
        [BsonElement("h6")]
        public string H6 { get; set; }

        [Required(ErrorMessage = "Mô tả banner không được để trống")]
        [BsonElement("p")]
        public string P {  get; set; }

        [BsonElement("image")]
        public string? Image {  get; set; } 
    }
}
