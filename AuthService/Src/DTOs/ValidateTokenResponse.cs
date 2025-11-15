using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Src.DTOs
{
    /// <summary>
    /// DTO para la respuesta de validación de token.
    /// </summary>
    public class ValidateTokenResponse
    {
        /// <summary>
        /// Indica si el token es válido o no.
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// Mensaje relacionado con la validación del token.
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// ID del usuario asociado al token, si es válido.
        /// </summary>
        public string? UserId { get; set; }
        /// <summary>
        /// Nombre de usuario asociado al token, si es válido.
        /// </summary>
        public string? Username { get; set; }
        /// <summary>
        /// Rol asociado al token, si es válido.
        /// </summary>
        public string? Role { get; set; }
    }
}