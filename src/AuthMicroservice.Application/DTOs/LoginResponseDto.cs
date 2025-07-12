namespace AuthMicroservice.Application.DTOs
{
    /// <summary>
    /// DTO para la respuesta de inicio de sesión exitoso.
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// Identificador único del usuario autenticado.
        /// </summary>
        public required string UserId { get; set; }
        
        /// <summary>
        /// Nombre de usuario del usuario autenticado.
        /// </summary>
        public required string Username { get; set; }
        
        /// <summary>
        /// Correo electrónico del usuario autenticado.
        /// </summary>
        public required string Email { get; set; }
        
        /// <summary>
        /// Token JWT generado para la sesión.
        /// </summary>
        public required string Token { get; set; }
        
        /// <summary>
        /// Token de refresco para renovar el token JWT.
        /// </summary>
        public required string RefreshToken { get; set; }
        
        /// <summary>
        /// Fecha y hora de expiración del token JWT.
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }
}