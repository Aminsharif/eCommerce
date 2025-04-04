using eCommerce.Core.DTOs.Settings;

namespace eCommerce.BlazorWasm.Services.Interfaces
{
    public interface ISettingsService
    {
        Task<GeneralSettingsDto> GetGeneralSettingsAsync();
        Task<bool> UpdateGeneralSettingsAsync(GeneralSettingsDto settings);
        Task<PaymentSettingsDto> GetPaymentSettingsAsync();
        Task<bool> UpdatePaymentSettingsAsync(PaymentSettingsDto settings);
        Task<List<ShippingMethodDto>> GetShippingMethodsAsync();
        Task<bool> UpdateShippingMethodsAsync(List<ShippingMethodDto> methods);
        Task<EmailSettingsDto> GetEmailSettingsAsync();
        Task<bool> UpdateEmailSettingsAsync(EmailSettingsDto settings);
        Task<bool> TestEmailSettingsAsync(EmailSettingsDto settings);
        Task<CurrencySettingsDto> GetCurrencySettingsAsync();
        Task<bool> UpdateCurrencySettingsAsync(CurrencySettingsDto settings);
        Task<List<string>> GetAvailableCurrenciesAsync();
        Task<TaxSettingsDto> GetTaxSettingsAsync();
        Task<bool> UpdateTaxSettingsAsync(TaxSettingsDto settings);
    }
}