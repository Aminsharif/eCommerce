using Microsoft.AspNetCore.Components;
using MudBlazor;
using eCommerce.Core.Enums;
using System.Net.Http.Json;

namespace eCommerce.BlazorWasm.Pages.Account
{
    public partial class Preferences
    {
        // These fields are now defined in the Razor file
        // private bool _isLoading;
        // private bool _isValid;
        // private MudForm _form;
        // private PreferencesDto _preferences;
        // These collections are now defined in the Razor file
        // private readonly string[] _languages;
        // private readonly List<CurrencyDto> _currencies;
        // private readonly string[] _timeZones;

        protected override async Task OnInitializedAsync()
        {
            await LoadPreferencesAsync();
        }

        private async Task LoadPreferencesAsync()
        {
            try
            {
                _preferences = await Http.GetFromJsonAsync<PreferencesDto>("api/account/preferences") 
                    ?? new PreferencesDto();
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading preferences: {ex.Message}", Severity.Error);
                _preferences = new PreferencesDto();
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task SavePreferencesAsync()
        {
            if (_form?.IsValid ?? false)
            {
                _isSaving = true;
                try
                {
                    await Http.PutAsJsonAsync("api/account/preferences", _preferences);
                    Snackbar.Add("Preferences saved successfully", Severity.Success);
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"Error saving preferences: {ex.Message}", Severity.Error);
                }
                finally
                {
                    _isSaving = false;
                }
            }
        }

        private async Task Logout()
        {
            NavigationManager.NavigateTo("/");
        }

        // Using PreferencesDto from the Razor file instead

        // CurrencyDto is defined in the Razor file
    }
}