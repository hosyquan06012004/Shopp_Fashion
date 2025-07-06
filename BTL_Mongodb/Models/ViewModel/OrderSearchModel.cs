namespace BTL_Mongodb.Models.ViewModel
{
    public class OrderSearchModel
    {
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public double? MinAmount { get; set; }
        public double? MaxAmount { get; set; }
    }
}
