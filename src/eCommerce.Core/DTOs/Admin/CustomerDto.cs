namespace eCommerce.Core.DTOs.Admin
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime RegisterDate { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
        public string Status { get; set; } = string.Empty;
        public CustomerAddress? Address { get; set; }
        public List<CustomerOrder> RecentOrders { get; set; } = new();
    }

    public enum CustomerStatus
    {
        Active,
        Inactive,
        Blocked
    }
}