using System.ComponentModel.DataAnnotations;

namespace eCommerce.Core.DTOs.Settings
{
    public class TaxSettingsDto
    {
        public bool EnableTax { get; set; } = true;
        public decimal DefaultTaxRate { get; set; } = 0.0m;
        public bool IncludeTaxInPrice { get; set; } = false;
        public bool EnableTaxByRegion { get; set; } = false;
        public string TaxRegistrationNumber { get; set; } = string.Empty;
        public string TaxOffice { get; set; } = string.Empty;
        public bool DisplayTaxSummary { get; set; } = true;
    }
}