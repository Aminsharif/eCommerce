using System.ComponentModel.DataAnnotations;

namespace eCommerce.Core.DTOs.Settings
{
    public class ShippingMethodDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal BaseCost { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(0, int.MaxValue)]
        public int EstimatedDaysMin { get; set; }

        [Range(0, int.MaxValue)]
        public int EstimatedDaysMax { get; set; }

        public bool IsDefault { get; set; }

        public decimal? FreeShippingThreshold { get; set; }

        public string[]? RestrictedCountries { get; set; }
    }
}