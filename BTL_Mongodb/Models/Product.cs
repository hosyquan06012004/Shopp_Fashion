using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace BTL_Mongodb.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [BsonElement("name")]
        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự.")]
        public string Name { get; set; }

        [BsonElement("description")]
        [StringLength(500, ErrorMessage = "Mô tả sản phẩm không được vượt quá 500 ký tự.")]
        public string? Description { get; set; }

        [BsonElement("price")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá sản phẩm phải là một giá trị dương.")]
        [Required(ErrorMessage = "Tên sản phẩm không được để trống.")]
        public double? Price { get; set; }

        [BsonElement("categoryId")]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required(ErrorMessage = "Thể loại không được để trống.")]
        public string CategoryId { get; set; }

        [BsonElement("brandId")]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required(ErrorMessage = "Thương hiệu không được để trống.")]
        public string BrandId { get; set; }

        [BsonElement("image")]
        [Url(ErrorMessage = "Ảnh chính phải là một URL hợp lệ.")]
        public string? Image { get; set; }

        [BsonElement("images")]
        public List<string>? Images { get; set; }


        [BsonElement("createdAt")]
        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Ngày tạo không được để trống.")]
        public DateTime? CreatedAt { get; set; }
    }
}
