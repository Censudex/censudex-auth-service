using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Src.DTOs
{
    /// <summary>
    /// Respuesta de inicio de sesión.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Token JWT emitido tras una autenticación exitosa.
        /// </summary>
        public string Token { get; set; } = string.Empty;
        /// <summary>
        /// Información del usuario autenticado.
        /// </summary>
        public UserInfo? User { get; set; }

    }

    public class UserInfo
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// Nombre de usuario.
        /// </summary>
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// Rol del usuario.
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}