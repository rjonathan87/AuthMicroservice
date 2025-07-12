namespace AuthMicroservice.Application.DTOs
{
    /// <summary>
    /// DTO para la solicitud de registro de usuario.
    /// </summary>
    public class RegisterRequestDto
    {
        /// <summary>
        /// Nombre de usuario del nuevo usuario.
        /// </summary>
        public required string Username { get; set; }
        
        /// <summary>
        /// Correo electrónico del nuevo usuario.
        /// </summary>
        public required string Email { get; set; }
        
        /// <summary>
        /// Contraseña del nuevo usuario.
        /// </summary>
        public required string Password { get; set; }
    }
}