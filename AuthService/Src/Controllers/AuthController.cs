using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Src.DTOs;
using AuthService.Src.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtService _jwtService;

        public AuthController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Email == Environment.GetEnvironmentVariable("ADMIN_EMAIL") && request.Password == Environment.GetEnvironmentVariable("ADMIN_PASSWORD"))
            {
                var token = _jwtService.GenerateToken(
                    userId: Guid.NewGuid().ToString(),
                    username: "admin",
                    email: request.Email,
                    role: "Administrator"
                );

                var response = new LoginResponse
                {
                    Token = token,
                    User = new UserInfo
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = request.Email,
                        Username = "admin",
                        Role = "Administrator"
                    }
                };

                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
        }
        
        [HttpGet("validate")]
        public IActionResult ValidateToken()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(new ValidateTokenResponse
                    {
                        IsValid = false,
                        Message = "Token not provided"
                    });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();

                var principal = _jwtService.ValidateToken(token);
                if (principal == null)
                {
                    return Unauthorized(new ValidateTokenResponse
                    {
                        IsValid = false,
                        Message = "Token invalid or expired"
                    });
                }

                var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                var username = principal.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
                var role = principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                return Ok(new ValidateTokenResponse
                {
                    IsValid = true,
                    Message = "Token valid",
                    UserId = userId,
                    Username = username,
                    Role = role
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new ValidateTokenResponse
                {
                    IsValid = false,
                    Message = "Error validating token"
                });
            }
        }
    }
}