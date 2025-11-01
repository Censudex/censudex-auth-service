using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Src.DTOs
{
    public class LogoutRequest
    {
        [Required(ErrorMessage = "The token is required for logout.")]
        public string Token { get; set; } = string.Empty;
    }
}