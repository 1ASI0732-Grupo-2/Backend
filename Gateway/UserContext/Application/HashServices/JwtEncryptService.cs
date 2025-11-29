using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using workstation_backend.UserContext.Domain.Models.Entities;
using workstation_backend.UserContext.Domain.Services;

namespace workstation_backend.UserContext.Application.HashServices;

    public class JwtEncryptService : IJwtEncryptService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationHours;

        public JwtEncryptService(IConfiguration configuration)
        {
            _configuration = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            _issuer = _configuration["Jwt:Issuer"] ?? "YourWebApi";
            _audience = _configuration["Jwt:Audience"] ?? "YourWebApiUsers";
            _expirationHours = int.Parse(_configuration["Jwt:ExpirationHours"] ?? "1");
        }

        public string Encrypt(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("firstName", user.FirstName),
                    new Claim("lastName", user.LastName),
                    new Claim("dni", user.Dni),
                    new Claim("phoneNumber", user.PhoneNumber),
                    new Claim("userRole", user.Role.ToString()),
                    new Claim("createdAt", user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, 
                        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                        ClaimValueTypes.Integer64)
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(_expirationHours),
                    Issuer = _issuer,
                    Audience = _audience,
                    SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar el token JWT: {ex.Message}", ex);
            }
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _key,
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero 
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                
                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                throw new UnauthorizedAccessException("El token ha expirado");
            }
            catch (SecurityTokenException ex)
            {
                throw new UnauthorizedAccessException($"Token inválido: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al validar el token: {ex.Message}", ex);
            }
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadJwtToken(token);
                
                return jsonToken.ValidTo < DateTime.UtcNow;
            }
            catch (Exception)
            {
                return true; 
            }
        }

        public string RefreshToken(string token)
        {
            try
            {
                var principal = ValidateToken(token);
                
                var userId = GetClaimValue(principal, ClaimTypes.NameIdentifier);
                var email = GetClaimValue(principal, ClaimTypes.Email);
                var firstName = GetClaimValue(principal, "firstName");
                var lastName = GetClaimValue(principal, "lastName");
                var dni = GetClaimValue(principal, "dni");
                var phoneNumber = GetClaimValue(principal, "phoneNumber");
                var roleString = GetClaimValue(principal, "userRole");
                var createdAtString = GetClaimValue(principal, "createdAt");

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
                {
                    throw new UnauthorizedAccessException("Claims del token inválidos");
                }

                if (!Enum.TryParse<UserRole>(roleString, out var role))
                {
                    throw new UnauthorizedAccessException("Rol inválido en el token");
                }

                if (!DateTime.TryParse(createdAtString, out var createdAt))
                {
                    createdAt = DateTime.UtcNow;
                }

                var userForToken = new UserTokenInfo
                {
                    Id = int.Parse(userId),
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Dni = dni,
                    PhoneNumber = phoneNumber,
                    Role = role,
                    CreatedAt = createdAt
                };

                return GenerateTokenFromUserInfo(userForToken);
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al refrescar el token: {ex.Message}", ex);
            }
        }

        public int? GetUserIdFromToken(string token)
        {
            try
            {
                var principal = ValidateToken(token);
                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                return int.TryParse(userIdClaim, out var userId) ? userId : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetEmailFromToken(string token)
        {
            try
            {
                var principal = ValidateToken(token);
                return principal.FindFirst(ClaimTypes.Email)?.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public UserRole? GetRoleFromToken(string token)
        {
            try
            {
                var principal = ValidateToken(token);
                var roleClaim = principal.FindFirst("userRole")?.Value;
                
                return Enum.TryParse<UserRole>(roleClaim, out var role) ? role : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GetClaimValue(ClaimsPrincipal principal, string claimType)
        {
            return principal.FindFirst(claimType)?.Value;
        }

        private string GenerateTokenFromUserInfo(UserTokenInfo userInfo)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim(ClaimTypes.Name, $"{userInfo.FirstName} {userInfo.LastName}"),
                new Claim(ClaimTypes.Role, userInfo.Role.ToString()),
                new Claim("firstName", userInfo.FirstName),
                new Claim("lastName", userInfo.LastName),
                new Claim("dni", userInfo.Dni),
                new Claim("phoneNumber", userInfo.PhoneNumber),
                new Claim("userRole", userInfo.Role.ToString()),
                new Claim("createdAt", userInfo.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, 
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                    ClaimValueTypes.Integer64)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(_expirationHours),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    public User Decrypt(string encrypted)
    {
        throw new NotImplementedException();
    }
}
    internal class UserTokenInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Dni { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
