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

            // Acceder directamente a las propiedades del objeto
            var passwordHash = userObj.PasswordHash;
            var username = userObj.Username;
            var userId = userObj.Id;

            if (string.IsNullOrEmpty(passwordHash) || string.IsNullOrEmpty(username) || userId == Guid.Empty)
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