namespace BTL_Mongodb.Models.ViewModel
{
    public class CartItemViewModel
    {
        public List<CartItemModel> CartItems { get; set; }
        public double GrandTotal => CartItems.Sum(x => x.Quantity * x.Price);
    }
}
