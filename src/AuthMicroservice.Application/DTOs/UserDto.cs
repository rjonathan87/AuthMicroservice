namespace AuthMicroservice.Application.DTOs
{
    public class UserDto
    {
        public required string UserId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public bool IsLocked { get; set; }
    }
}