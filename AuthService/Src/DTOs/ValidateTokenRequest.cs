using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Src.DTOs
{
    /// <summary>
    /// DTO para la solicitud de validación de token.
    /// </summary>
    public class ValidateTokenRequest
    {
        /// <summary>
        /// Token de autenticación a validar.
        /// </summary>
        [Required(ErrorMessage = "The token is required for token validation.")]
        public string Token { get; set; } = string.Empty;
    }
}