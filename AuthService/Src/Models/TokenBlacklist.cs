using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Src.Models
{
    /// <summary>
    /// Respresentación de un token en la lista negra.
    /// </summary>
    public class TokenBlacklist
    {
        /// <summary>
        /// Identificador único del token en la lista negra.
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// El token que ha sido puesto en la lista negra.
        /// </summary>
        [Required]
        public string Token { get; set; } = string.Empty;
        /// <summary>
        /// ID del usuario asociado al token en la lista negra.
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;
        /// <summary>
        /// Fecha y hora en que el token fue puesto en la lista negra.
        /// </summary>
        [Required]
        public DateTime BlacklistedAt { get; set; }
        /// <summary>
        /// Fecha y hora en que el token expirará.
        /// </summary>
        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}