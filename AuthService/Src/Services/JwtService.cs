using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthService.Src.Configurations;
using AuthService.Src.Data;
using AuthService.Src.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Src.Services
{
    /// <summary>
    /// Servicio para la generación y validación de tokens JWT.
    /// </summary>
    public class JwtService : IJwtService
    {
        /// <summary>
        /// Configuraciones de JWT.
        /// </summary>
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// Contexto de la base de datos para gestionar tokens en lista negra.
        /// </summary>
        private readonly AuthDbContext _dbContext;

        /// <summary>
        /// Clave secreta decodificada en bytes.
        /// </summary>
        private readonly byte[] _secretKey;

        /// <summary>
        /// Constructor del servicio JwtService.
        /// </summary>
        /// <param name="jwtSettings">Configuraciones de JWT.</param>
        /// <param name="dbContext">Contexto de la base de datos para gestionar tokens en lista negra.</param>
        /// <exception cref="InvalidOperationException">Excepción lanzada si la clave secreta no cumple con los requisitos de longitud.</exception>
        public JwtService(JwtSettings jwtSettings, AuthDbContext dbContext)
        {
            _jwtSettings = jwtSettings;
            _dbContext = dbContext;

            // Decodificar el secreto desde Base64 una sola vez
            try
            {
                _secretKey = Convert.FromBase64String(_jwtSettings.Secret);
                
                // Validar que tenga al menos 256 bits (32 bytes)
                if (_secretKey.Length < 32)
                {
                    throw new InvalidOperationException(
                        $"JWT_SECRET debe tener al menos 32 bytes. Actual: {_secretKey.Length} bytes");
                }
            }
            catch (FormatException)
            {
                // Si no es Base64, intentar como UTF8 (fallback)
                _secretKey = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
                
                if (_secretKey.Length < 32)
                {
                    throw new InvalidOperationException(
                        $"JWT_SECRET debe tener al menos 32 bytes. Actual: {_secretKey.Length} bytes. " +
                        $"Genera uno nuevo con: openssl rand -base64 32");
                }
            }
        }

        /// <summary>
        /// Genera un token JWT para un usuario dado.
        /// </summary>
        /// <param name="userId">Identificador único del usuario.</param>
        /// <param name="username">Nombre del usuario.</param>
        /// <param name="email">Email del usuario.</param>
        /// <param name="role">Rol del usuario.</param>
        /// <returns>Token JWT generado.</returns>
        public string GenerateToken(string userId, string username, string email, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(JwtRegisteredClaimNames.GivenName, username),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
            };

            var key = new SymmetricSecurityKey(_secretKey);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Valida un token JWT y devuelve los claims si es válido.
        /// </summary>
        /// <param name="token">Token JWT a validar.</param>
        /// <returns>ClaimsPrincipal si el token es válido; de lo contrario, null.</returns>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Agrega un token a la lista negra.
        /// </summary>
        /// <param name="token">Token de JWT a agregar a la lista negra.</param>
        /// <param name="userId">Identificador del usuario al que pertenece el token.</param>
        /// <param name="expiresAt">Fecha y hora de expiración del token.</param>
        /// <returns>Resultado de la operación de agregar a la lista negra.</returns>
        public Task BlacklistTokenAsync(string token, string userId, DateTime expiresAt)
        {
            var blacklistedToken = new Models.TokenBlacklist
            {
                Id = Guid.NewGuid(),
                Token = token,
                UserId = userId,
                BlacklistedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt
            };

            _dbContext.TokenBlacklists.Add(blacklistedToken);
            return _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Verifica si un token está en la lista negra.
        /// </summary>
        /// <param name="token">Token JWT a verificar.</param>
        /// <returns>True si el token está en la lista negra; de lo contrario, false.</returns>
        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            return await _dbContext.TokenBlacklists.AnyAsync(x => x.Token == token);
        }
    }
}