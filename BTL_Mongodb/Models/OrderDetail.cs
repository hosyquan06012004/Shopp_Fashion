namespace BTL_Mongodb.Models
{
    public class OrderDetail
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Total => Quantity * Price;
    }
}
