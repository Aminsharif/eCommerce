namespace eCommerce.Core.DTOs.Auth
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePicture { get; set; }
        public string Role { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
} 