namespace TechStore.API.DTOs.Auth
{
    public class AuthResponseDto
    {
        public int UserId { get; set; }

        public int UserTypeId { get; set; }

        public string UserTypeName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}
