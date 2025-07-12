namespace AuthMicroservice.Application.DTOs
{
    /// <summary>
    /// DTO para la solicitud de revocación de un token JWT.
    /// </summary>
    public class RevokeTokenRequestDto
    {
        /// <summary>
        /// Token JWT a revocar.
        /// </summary>
        public required string Token { get; set; }
    }
}