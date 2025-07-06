namespace BTL_Mongodb.Models
{
    public class CartItemModel
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string? Image { get; set; }

        public double Total => Quantity * Price;

        public CartItemModel() { }

        public CartItemModel(Product product)
        {
            ProductId = product.Id;
            ProductName = product.Name;
            Price = product.Price ?? 0;
            Quantity = 1;
            Image = product.Image;
        }
    }
}
