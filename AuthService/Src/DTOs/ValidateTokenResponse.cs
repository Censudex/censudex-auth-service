using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Src.DTOs
{
    public class ValidateTokenResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
    }
}