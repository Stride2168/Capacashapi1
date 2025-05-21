// Application/Kiosks/Commands/KioskLoginCommandHandler.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Capacash.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Capacash.Application.Kiosks.Commands;

public class KioskLoginCommandHandler : IRequestHandler<KioskLoginCommand, AuthResult>
{
    private readonly IKioskRepository _kioskRepository;
    private readonly IConfiguration _configuration;

    public KioskLoginCommandHandler(
        IKioskRepository kioskRepository,
        IConfiguration configuration)
    {
        _kioskRepository = kioskRepository;
        _configuration = configuration;
    }

    public async Task<AuthResult> Handle(KioskLoginCommand request, CancellationToken cancellationToken)
    {
        var kiosk = await _kioskRepository.GetKioskByKioskIdAsync(request.KioskId);
        
        if (kiosk == null || !BCrypt.Net.BCrypt.Verify(request.Password, kiosk.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid Kiosk ID or Password.");
        }

        if (!kiosk.IsActive)
        {
            throw new InvalidOperationException("This kiosk is currently disabled.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing in configuration.");
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
           Subject = new ClaimsIdentity(new[]
{
    new Claim(ClaimTypes.NameIdentifier, kiosk.Id.ToString()), // ✅ Use Guid
    new Claim(ClaimTypes.Role, "Kiosk"),
    new Claim("CompanyId", kiosk.CompanyId),
    new Claim("KioskCode", kiosk.KioskId) // ✅ Optional: if you want to keep the public-facing code too
}),

            Expires = DateTime.UtcNow.AddHours(8),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new AuthResult(tokenHandler.WriteToken(token));
    }
}