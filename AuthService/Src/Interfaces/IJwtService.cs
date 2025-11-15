using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.Src.Interfaces
{
    /// <summary>
    /// Interface para el servicio de JWT.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Genera un token JWT para el usuario especificado.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="email">Email del usuario</param>
        /// <param name="role">Rol del usuario</param>
        /// <returns>Token JWT generado.</returns>
        string GenerateToken(string userId, string username, string email, string role);

        /// <summary>
        /// Valida el token JWT y devuelve los claims si es válido.
        /// </summary>
        /// <param name="token">Token JWT a validar.</param>
        /// <returns>ClaimsPrincipal si el token es válido; de lo contrario, null.</returns>
        ClaimsPrincipal? ValidateToken(string token);

        /// <summary>
        /// Verifica si un token está en la lista negra.
        /// </summary>
        /// <param name="token">Token JWT a verificar.</param>
        /// <returns>True si el token está en la lista negra; de lo contrario, false.</returns>
        Task<bool> IsTokenBlacklistedAsync(string token);
        
        /// <summary>
        /// Agrega un token a la lista negra.
        /// </summary>
        /// <param name="token">Token JWT a agregar.</param>
        /// <param name="userId">ID del usuario asociado al token.</param>
        /// <param name="expiresAt">Fecha y hora de expiración del token.</param>
        Task BlacklistTokenAsync(string token, string userId, DateTime expiresAt);
    }
}