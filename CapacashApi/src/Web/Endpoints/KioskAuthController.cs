using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Capacash.Application.Common.Interfaces;
using Capacash.Web.Models;
using BCrypt.Net;

namespace Capacash.Web.Controllers
{
    [ApiController]
    [Route("api/kiosk/auth")]
    public class KioskAuthController : ControllerBase
    {
        private readonly IKioskRepository _kioskRepository;
        private readonly IConfiguration _configuration;

        public KioskAuthController(IKioskRepository kioskRepository, IConfiguration configuration)
        {
            _kioskRepository = kioskRepository;
            _configuration = configuration;
        }

         [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] KioskLoginRequest request)
        {
            var kiosk = await _kioskRepository.GetKioskByKioskIdAsync(request.KioskId);
            if (kiosk == null) return Unauthorized(new { Error = "Invalid Kiosk ID or Password." });

            // Verify the password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, kiosk.PasswordHash))
                return Unauthorized(new { Error = "Invalid Kiosk ID or Password." });

            // Generate JWT token for Kiosk
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing in configuration.");
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, kiosk.KioskId),
                    new Claim(ClaimTypes.Role, "Kiosk"),
                    new Claim("CompanyId", kiosk.CompanyId) // Add CompanyId to the token
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }
}
