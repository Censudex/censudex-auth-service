using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Src.Configurations
{
    /// <summary>
    /// Configuración de JWT para la autenticación.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Clave secreta para firmar los tokens JWT.
        /// </summary>
        public string Secret { get; set; } = string.Empty;
        /// <summary>
        /// Emisor del token JWT.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;
        /// <summary>
        /// Audiencia del token JWT.
        /// </summary>
        public string Audience { get; set; } = string.Empty;
        /// <summary>
        /// Tiempo de expiración del token en minutos.
        /// </summary>
        public int ExpirationMinutes { get; set; } = 60;
    }
}