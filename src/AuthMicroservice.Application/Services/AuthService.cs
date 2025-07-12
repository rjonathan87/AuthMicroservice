using AuthMicroservice.Application.DTOs;
using AuthMicroservice.Application.Interfaces;
using AuthMicroservice.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Dynamic;

namespace AuthMicroservice.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasherService _passwordHasherService;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, IPasswordHasherService passwordHasherService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<string> LoginAsync(LoginRequestDto loginRequest)
        {
            var userObj = await _userRepository.GetByEmailAsync(loginRequest.Email);
            
            // Si el usuario no existe, lanzar excepción
            if (userObj == null)
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            // Usamos reflexión para acceder a las propiedades de manera segura
            var userType = userObj.GetType();
            var passwordHashProperty = userType.GetProperty("PasswordHash");
            var usernameProperty = userType.GetProperty("Username");
            var userIdProperty = userType.GetProperty("UserId");

            if (passwordHashProperty == null || usernameProperty == null || userIdProperty == null)
            {
                throw new InvalidOperationException("El objeto de usuario no tiene las propiedades esperadas.");
            }

            var passwordHash = passwordHashProperty.GetValue(userObj) as string;
            var username = usernameProperty.GetValue(userObj) as string;
            var userId = userIdProperty.GetValue(userObj);

            if (passwordHash == null || username == null || userId == null)
            {
                throw new InvalidOperationException("Valores de usuario no válidos.");
            }

            // Verificar la contraseña
            if (!_passwordHasherService.VerifyPassword(loginRequest.Password, passwordHash))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            // Generar el token JWT
            var secretKey = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("SecretKey is not configured.");
            }

            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60")),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}