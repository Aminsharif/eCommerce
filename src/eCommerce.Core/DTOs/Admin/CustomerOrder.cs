namespace eCommerce.Core.DTOs.Admin
{
    public class CustomerOrder
    {
        public string OrderId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}