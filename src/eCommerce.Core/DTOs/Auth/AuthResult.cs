namespace eCommerce.Core.DTOs.Auth
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;

        public static AuthResult Success(string message = "")
        {
            return new AuthResult { Succeeded = true, Message = message };
        }

        public static AuthResult Failure(string message)
        {
            return new AuthResult { Succeeded = false, Message = message };
        }
    }
}