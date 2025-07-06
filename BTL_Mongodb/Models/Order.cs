using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BTL_Mongodb.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();


        public string? UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string TownCity { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? Note { get; set; }
        public string Status { get; set; } = "Pending";
        public double TotalAmount { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}
