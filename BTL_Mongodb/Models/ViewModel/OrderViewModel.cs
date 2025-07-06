namespace BTL_Mongodb.Models.ViewModel
{
    public class OrderViewModel
    {
        public string Id { get; set; }
        public string? UserId { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Status { get; set; } = "Pending";
        public double TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; } = new();
    }
}
