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
            try
            {
                var token = _jwtService.GenerateToken(request.Id, request.Username, request.Email, request.Role);

                return Ok(new
                {
                    Token = token,
                    User = new UserInfo
                    {
                        Id = request.Id,
                        Username = request.Username,
                        Email = request.Email,
                        Role = request.Role
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error during login" });
            }
        }

        [HttpPost("validate-token")]
        public async Task<ActionResult<ValidateTokenResponse>> ValidateToken([FromBody] ValidateTokenRequest request)
        {
            try
            {
                var token = request.Token;

                // Verificar si está en la blacklist
                if (await _jwtService.IsTokenBlacklistedAsync(token))
                {
                    return Unauthorized(new ValidateTokenResponse
                    {
                        IsValid = false,
                        Message = "Token has been revoked"
                    });
                }

                // Validar token
                var principal = _jwtService.ValidateToken(token);
                if (principal == null)
                {
                    return Unauthorized(new ValidateTokenResponse
                    {
                        IsValid = false,
                        Message = "Token is invalid or expired"
                    });
                }

                var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                var username = principal.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
                var role = principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                return Ok(new ValidateTokenResponse
                {
                    IsValid = true,
                    Message = "Token is valid",
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

        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody] LogoutRequest request)
        {
            try
            {
                // Validar token antes de agregarlo a la blacklist
                var principal = _jwtService.ValidateToken(request.Token);
                if (principal == null)
                {
                    return BadRequest(new { message = "Token is invalid" });
                }

                // Obtener información del token
                var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "unknown";
                var expClaim = principal.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
                var expiresAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim!)).UtcDateTime;

                // Agregar a blacklist
                await _jwtService.BlacklistTokenAsync(request.Token, userId, expiresAt);


                return Ok(new
                {
                    success = true,
                    message = "Session closed successfully"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Error closing session" });
            }
        }
    }
}