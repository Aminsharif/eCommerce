namespace eCommerce.Core.DTOs.Admin;

public class DashboardStatsDto
{
    public int TotalOrders { get; set; }
    public decimal Revenue { get; set; }
    public int TotalProducts { get; set; }
    public int TotalCustomers { get; set; }
}