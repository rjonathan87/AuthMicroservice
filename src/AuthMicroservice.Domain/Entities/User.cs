namespace AuthMicroservice.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public bool IsLocked { get; set; }
        public string PasswordHash { get; set; } // Agregar propiedad PasswordHash
    }
}