using Blazored.LocalStorage;
using eCommerce.Core.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Diagnostics;

namespace eCommerce.BlazorWasm.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            
            if (string.IsNullOrWhiteSpace(savedToken))
            {
                Debug.WriteLine("No auth token found in local storage");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            Debug.WriteLine("Processing JWT token from local storage");
            var claims = JwtParser.ParseClaimsFromJwt(savedToken);
            var roleClaims = claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role").ToList();
            
            Debug.WriteLine($"Found {roleClaims.Count} initial role claims");
            foreach (var rc in roleClaims)
            {
                Debug.WriteLine($"Initial role claim: {rc.Value}");
            }
            
            // Convert string roles to UserRole enum and ensure consistent casing
            var roles = roleClaims
                .Select(rc => 
                {
                    Debug.WriteLine($"Attempting to parse role: {rc.Value}");
                    if (Enum.TryParse<UserRole>(rc.Value, true, out var role))
                    {
                        var normalizedRole = role.ToString();
                        Debug.WriteLine($"Successfully normalized role: {rc.Value} -> {normalizedRole}");
                        return normalizedRole;
                    }
                    Debug.WriteLine($"Failed to parse role: {rc.Value}");
                    return null;
                })
                .Where(r => r != null)
                .ToList();

            var identity = new ClaimsIdentity(claims, "jwt");
            // Add normalized role claims
            foreach (var role in roles)
            {
                Debug.WriteLine($"Adding normalized role claim: {role}");
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            
            var user = new ClaimsPrincipal(identity);
            Debug.WriteLine($"Final user roles: {string.Join(", ", user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value))}");
            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(string token)
        {
            Debug.WriteLine("Processing authentication notification");
            var claims = JwtParser.ParseClaimsFromJwt(token);
            var roleClaims = claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role").ToList();

            Debug.WriteLine($"Found {roleClaims.Count} role claims in new token");
            var roles = roleClaims
                .Select(rc => 
                {
                    Debug.WriteLine($"Parsing role from token: {rc.Value}");
                    if (Enum.TryParse<UserRole>(rc.Value, true, out var role))
                    {
                        var normalizedRole = role.ToString();
                        Debug.WriteLine($"Normalized role from token: {normalizedRole}");
                        return normalizedRole;
                    }
                    return null;
                })
                .Where(r => r != null)
                .ToList();

            var identity = new ClaimsIdentity(claims, "jwt");
            foreach (var role in roles)
            {
                Debug.WriteLine($"Adding role claim to identity: {role}");
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var authenticatedUser = new ClaimsPrincipal(identity);
            Debug.WriteLine($"Final authenticated user roles: {string.Join(", ", authenticatedUser.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value))}");
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            Debug.WriteLine("Processing user logout");
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }
    }
}