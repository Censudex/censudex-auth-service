using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Src.DTOs
{
    /// <summary>
    /// DTO para la solicitud de cierre de sesión.
    /// </summary>
    public class LogoutRequest
    {
        /// <summary>
        /// Token de autenticación del usuario que desea cerrar sesión.
        /// </summary>
        [Required(ErrorMessage = "The token is required for logout.")]
        public string Token { get; set; } = string.Empty;
    }
}