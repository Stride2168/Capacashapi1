using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Capacash.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Capacash.Application.Kiosks.Commands.KioskLogin
{
    public class KioskLoginCommandHandler : IRequestHandler<KioskLoginCommand, string>
    {
        private readonly IKioskRepository _kioskRepository;
        private readonly IConfiguration _configuration;

        public KioskLoginCommandHandler(IKioskRepository kioskRepository, IConfiguration configuration)
        {
            _kioskRepository = kioskRepository;
            _configuration = configuration;
        }

        public async Task<string> Handle(KioskLoginCommand request, CancellationToken cancellationToken)
        {
            var kiosk = await _kioskRepository.GetKioskByKioskIdAsync(request.KioskId);
            if (kiosk == null || !BCrypt.Net.BCrypt.Verify(request.Password, kiosk.PasswordHash))
                throw new UnauthorizedAccessException("Invalid Kiosk ID or Password.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing.");
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, kiosk.KioskId),
                    new Claim(ClaimTypes.Role, "Kiosk"),
                    new Claim("CompanyId", kiosk.CompanyId)
                }),
                Expires = DateTime.UtcNow.AddHours(8),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
