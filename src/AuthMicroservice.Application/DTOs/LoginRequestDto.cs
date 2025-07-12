namespace AuthMicroservice.Application.DTOs
{
    /// <summary>
    /// DTO para la solicitud de inicio de sesión.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Correo electrónico del usuario que intenta iniciar sesión.
        /// </summary>
        public required string Email { get; set; }
        
        /// <summary>
        /// Contraseña del usuario que intenta iniciar sesión.
        /// </summary>
        public required string Password { get; set; }
    }
}