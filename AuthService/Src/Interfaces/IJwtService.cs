using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.Src.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string username, string email, string role);
        ClaimsPrincipal? ValidateToken(string token);
    }
}