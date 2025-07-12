namespace AuthMicroservice.Application.DTOs
{
    /// <summary>
    /// DTO para información básica del usuario.
    /// </summary>
    public class UserInfoDto
    {
        public required string UserId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}