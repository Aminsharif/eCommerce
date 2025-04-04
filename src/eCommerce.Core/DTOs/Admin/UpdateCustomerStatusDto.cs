namespace eCommerce.Core.DTOs.Admin
{
    public class UpdateCustomerStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}