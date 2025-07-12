namespace AuthMicroservice.Application.DTOs
{
    /// <summary>
    /// DTO para representar un token JWT con su fecha de expiración.
    /// </summary>
    public class TokenDto
    {
        /// <summary>
        /// Token JWT generado.
        /// </summary>
        public required string Token { get; set; }
        
        /// <summary>
        /// Fecha y hora de expiración del token JWT.
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }
}