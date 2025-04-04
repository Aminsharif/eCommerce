using System.Security.Claims;
using System.Text.Json;
using System.Diagnostics;

namespace eCommerce.BlazorWasm.Services
{
    public static class JwtParser
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            
            Debug.WriteLine($"JWT Payload: {JsonSerializer.Serialize(keyValuePairs, new JsonSerializerOptions { WriteIndented = true })}");
            
            var claims = new List<Claim>();
            foreach (var kvp in keyValuePairs)
            {
                if (kvp.Value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                {
                    var values = element.EnumerateArray().Select(x => x.ToString());
                    foreach (var value in values)
                    {
                        Debug.WriteLine($"Adding array claim - Type: {kvp.Key}, Value: {value}");
                        claims.Add(new Claim(kvp.Key, value));
                    }
                }
                else
                {
                    Debug.WriteLine($"Adding single claim - Type: {kvp.Key}, Value: {kvp.Value}");
                    claims.Add(new Claim(kvp.Key, kvp.Value.ToString()));
                }
            }
            
            // Add default Name claim if missing
            if (!claims.Any(c => c.Type == ClaimTypes.Name))
            {
                Debug.WriteLine("Adding default anonymous name claim");
                claims.Add(new Claim(ClaimTypes.Name, "anonymous"));
            }

            // Log role claims specifically
            var roleClaims = claims.Where(c => c.Type == "role" || c.Type == ClaimTypes.Role).ToList();
            Debug.WriteLine($"Found {roleClaims.Count} role claims:");
            foreach (var roleClaim in roleClaims)
            {
                Debug.WriteLine($"Role claim - Type: {roleClaim.Type}, Value: {roleClaim.Value}");
            }

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}