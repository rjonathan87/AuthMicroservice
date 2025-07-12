namespace AuthMicroservice.Application.DTOs
{
    /// <summary>
    /// DTO para solicitar la renovación de un token.
    /// </summary>
    public class RefreshTokenRequestDto
    {
        /// <summary>
        /// Token de refresco utilizado para obtener un nuevo token de acceso.
        /// </summary>
        public required string RefreshToken { get; set; }
    }
}