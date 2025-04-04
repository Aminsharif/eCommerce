using System.Net.Http.Json;
using eCommerce.BlazorWasm.Services.Interfaces;
using eCommerce.Core.DTOs.Settings;

namespace eCommerce.BlazorWasm.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly HttpClient _httpClient;

        public SettingsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralSettingsDto> GetGeneralSettingsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<GeneralSettingsDto>("api/admin/settings/general") ?? new GeneralSettingsDto();
            }
            catch
            {
                return new GeneralSettingsDto();
            }
        }

        public async Task<bool> UpdateGeneralSettingsAsync(GeneralSettingsDto settings)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/admin/settings/general", settings);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<PaymentSettingsDto> GetPaymentSettingsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<PaymentSettingsDto>("api/admin/settings/payment") ?? new PaymentSettingsDto();
            }
            catch
            {
                return new PaymentSettingsDto();
            }
        }

        public async Task<bool> UpdatePaymentSettingsAsync(PaymentSettingsDto settings)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/admin/settings/payment", settings);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<ShippingMethodDto>> GetShippingMethodsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<ShippingMethodDto>>("api/admin/settings/shipping") ?? new List<ShippingMethodDto>();
            }
            catch
            {
                return new List<ShippingMethodDto>();
            }
        }

        public async Task<bool> UpdateShippingMethodsAsync(List<ShippingMethodDto> methods)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/admin/settings/shipping", methods);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<EmailSettingsDto> GetEmailSettingsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<EmailSettingsDto>("api/admin/settings/email") ?? new EmailSettingsDto();
            }
            catch
            {
                return new EmailSettingsDto();
            }
        }

        public async Task<bool> UpdateEmailSettingsAsync(EmailSettingsDto settings)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/admin/settings/email", settings);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> TestEmailSettingsAsync(EmailSettingsDto settings)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/admin/settings/email/test", settings);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<CurrencySettingsDto> GetCurrencySettingsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CurrencySettingsDto>("api/admin/settings/currency") ?? new CurrencySettingsDto();
            }
            catch
            {
                return new CurrencySettingsDto();
            }
        }

        public async Task<bool> UpdateCurrencySettingsAsync(CurrencySettingsDto settings)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/admin/settings/currency", settings);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<string>> GetAvailableCurrenciesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<string>>("api/admin/settings/currencies") ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<TaxSettingsDto> GetTaxSettingsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<TaxSettingsDto>("api/admin/settings/tax") ?? new TaxSettingsDto();
            }
            catch
            {
                return new TaxSettingsDto();
            }
        }

        public async Task<bool> UpdateTaxSettingsAsync(TaxSettingsDto settings)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/admin/settings/tax", settings);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        Task<List<ShippingMethodDto>> ISettingsService.GetShippingMethodsAsync()
        {
            throw new NotImplementedException();
        }

        Task<bool> ISettingsService.UpdateShippingMethodsAsync(List<ShippingMethodDto> methods)
        {
            throw new NotImplementedException();
        }
    }
}