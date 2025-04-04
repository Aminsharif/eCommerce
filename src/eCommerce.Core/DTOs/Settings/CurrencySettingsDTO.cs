using System.ComponentModel.DataAnnotations;

namespace eCommerce.Core.DTOs.Settings
{
    public class CurrencySettingsDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(3)]
        public string CurrencyCode { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string CurrencySymbol { get; set; } = string.Empty;

        public bool IsDefault { get; set; }

        public decimal ExchangeRate { get; set; } = 1.0m;

        [Required]
        [StringLength(50)]
        public string CurrencyName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}