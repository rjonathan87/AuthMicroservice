using AuthMicroservice.Application.DTOs;

namespace AuthMicroservice.Application.Interfaces;

/// <summary>
/// Servicio para autenticación de usuarios
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Autentica a un usuario por sus credenciales y devuelve un token JWT
    /// </summary>
    Task<string> LoginAsync(LoginRequestDto loginRequest);
}