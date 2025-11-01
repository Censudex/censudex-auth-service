using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Src.Models
{
    public class TokenBlacklist
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public DateTime BlacklistedAt { get; set; }
        [Required]
        public DateTime ExpiresAt { get; set; }
    }
}