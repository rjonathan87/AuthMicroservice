using AuthMicroservice.Application.DTOs;
using AuthMicroservice.Application.Interfaces;
using AuthMicroservice.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthMicroservice.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _dbContext;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService, 
            IPasswordHasherService passwordHasherService, 
            AuthDbContext dbContext,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _passwordHasherService = passwordHasherService;
            _dbContext = dbContext;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    await CreateLoginHistory(null, false, request.Email);
                    return Unauthorized("Credenciales inválidas.");
                }
                
                var isPasswordValid = _passwordHasherService.VerifyPassword(request.Password, user.PasswordHash);
                if (!isPasswordValid)
                {
                    user.FailedLoginAttempts++;
                    user.LastLoginAttempt = DateTime.UtcNow;
                    
                    if (user.FailedLoginAttempts >= 3)
                    {
                        user.IsLocked = true;
                        await _dbContext.SaveChangesAsync();
                        await CreateLoginHistory(user.UserId, false, user.Email);
                        return Unauthorized("Cuenta bloqueada por múltiples intentos fallidos.");
                    }
                    
                    await _dbContext.SaveChangesAsync();
                    await CreateLoginHistory(user.UserId, false, user.Email);
                    return Unauthorized("Credenciales inválidas.");
                }

                user.FailedLoginAttempts = 0;
                user.LastLoginAttempt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                string accessToken = await _authService.LoginAsync(request);
                var refreshToken = await GenerateRefreshToken(user);

                await CreateLoginHistory(user.UserId, true, user.Email);

                var loginResponse = new LoginResponseDto
                {
                    UserId = user.UserId.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1)
                };

                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el inicio de sesión");
                return StatusCode(500, "Error interno del servidor al procesar el inicio de sesión.");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email || u.Username == request.Username);
                if (existingUser != null)
                {
                    return BadRequest(existingUser.Email == request.Email 
                        ? "Ya existe un usuario con este correo electrónico." 
                        : "Ya existe un usuario con este nombre de usuario.");
                }

                var hashedPassword = _passwordHasherService.HashPassword(request.Password);

                var newUser = new AuthMicroservice.Infrastructure.Data.User
                {
                    UserId = Guid.NewGuid(),
                    Email = request.Email,
                    PasswordHash = hashedPassword, 
                    Username = request.Username,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsLocked = false,
                    FailedLoginAttempts = 0
                };

                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

                var loginRequest = new LoginRequestDto
                {
                    Email = request.Email,
                    Password = request.Password
                };

                string accessToken = await _authService.LoginAsync(loginRequest);
                var refreshToken = await GenerateRefreshToken(newUser);

                var authResponse = new LoginResponseDto
                {
                    UserId = newUser.UserId.ToString(),
                    Username = newUser.Username,
                    Email = newUser.Email,
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1)
                };

                return Ok(authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el registro de usuario");
                return StatusCode(500, "Error interno del servidor al procesar el registro.");
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                _logger.LogInformation("Attempting to refresh token: {RefreshToken}", request.RefreshToken);

                var refreshTokenEntity = await _dbContext.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && rt.ExpirationDate > DateTime.UtcNow);

                if (refreshTokenEntity == null)
                {
                    _logger.LogWarning("Refresh token not found or expired: {RefreshToken}", request.RefreshToken);
                    return Unauthorized("Token de refresco inválido o expirado.");
                }

                var user = refreshTokenEntity.User;

                if (user.IsLocked)
                {
                    _logger.LogWarning("User account is locked: {UserId}", user.UserId);
                    return Unauthorized("Esta cuenta está bloqueada.");
                }

                var accessToken = await GenerateAccessToken(user);

                var tokenDto = new TokenDto
                {
                    Token = accessToken,
                    ExpiresAt = DateTime.UtcNow.AddHours(1)
                };

                _dbContext.RefreshTokens.Remove(refreshTokenEntity);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Refresh token successfully processed for user: {UserId}", user.UserId);
                return Ok(tokenDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la renovación del token");
                return StatusCode(500, "Error interno del servidor al refrescar el token.");
            }
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequestDto request)
        {
            try
            {
                var jwtToken = await _dbContext.JwtTokens
                    .FirstOrDefaultAsync(jt => jt.TokenIdentifier == request.Token);

                if (jwtToken == null)
                {
                    return NotFound("Token no encontrado.");
                }

                jwtToken.RevokedAt = DateTime.UtcNow;
                jwtToken.Reason = "Revocado por el usuario";
                await _dbContext.SaveChangesAsync();

                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la revocación del token");
                return StatusCode(500, "Error interno del servidor al revocar el token.");
            }
        }

        #region Private Helper Methods

        private async Task<string> GenerateAccessToken(AuthMicroservice.Infrastructure.Data.User user)
        {
            var tokenId = Guid.NewGuid().ToString();
            
            var jwtToken = new AuthMicroservice.Infrastructure.Data.JwtToken
            {
                TokenIdentifier = tokenId,
                ExpirationDate = DateTime.UtcNow.AddHours(1),
                UserId = user.UserId
            };
            
            _dbContext.JwtTokens.Add(jwtToken);
            await _dbContext.SaveChangesAsync();
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim("jti", tokenId)
            };
            
            var roles = await GetUserRoles(user.UserId);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("La clave secreta de JWT no está configurada.");
            }
            
            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<string> GenerateRefreshToken(AuthMicroservice.Infrastructure.Data.User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim("jti", Guid.NewGuid().ToString())
            };

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("La clave secreta de JWT no está configurada.");
            }

            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = tokenHandler.WriteToken(token);

            var refreshTokenEntity = new AuthMicroservice.Infrastructure.Data.RefreshToken
            {
                Token = refreshToken,
                ExpirationDate = tokenDescriptor.Expires.Value,
                UserId = user.UserId
            };

            _dbContext.RefreshTokens.Add(refreshTokenEntity);
            await _dbContext.SaveChangesAsync();

            return refreshToken;
        }

        private async Task<List<string>> GetUserRoles(Guid userId)
        {
            var userRoles = await _dbContext.UserRoles
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.RoleName)
                .ToListAsync();
                
            return userRoles;
        }

        private async Task CreateLoginHistory(Guid? userId, bool isSuccess, string usernameAttempt = "")
        {
            var loginHistory = new AuthMicroservice.Infrastructure.Data.LoginHistory
            {
                UserId = userId,
                UsernameAttempt = isSuccess ? (await _dbContext.Users.FindAsync(userId))?.Username ?? string.Empty : usernameAttempt,
                LoginTimestamp = DateTime.UtcNow,
                IsSuccess = isSuccess,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            };
            
            _dbContext.LoginHistories.Add(loginHistory);
            await _dbContext.SaveChangesAsync();
        }

        #endregion
    }
}
