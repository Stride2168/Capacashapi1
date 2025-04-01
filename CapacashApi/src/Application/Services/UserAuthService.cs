using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Capacash.Application.Common.Interfaces;
using Capacash.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Capacash.Application.Services
{
    public class UserAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
private readonly IKioskRepository _kioskRepository;
      public UserAuthService(IUserRepository userRepository, IKioskRepository kioskRepository, IConfiguration configuration)
{
    _userRepository = userRepository;
    _kioskRepository = kioskRepository; // Assign the injected repository
    _configuration = configuration;
}

public async Task<string> RegisterUserAsync(string fullName, string email, string password, string companyId)
{
    var existingUser = await _userRepository.GetByEmailAsync(email);
    if (existingUser != null)
        throw new Exception("User already exists.");

    var admin = await _userRepository.GetByCompanyIdAsync(companyId);
    if (admin == null)
        throw new Exception("Invalid Company ID. Please enter a valid Company ID assigned by your Admin.");

    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
    var newUser = new User
    {
        FullName = fullName,
        Email = email,
        PasswordHash = hashedPassword,
        Role = "Employee",
        CompanyId = companyId,
        IsApproved = false // ✅ New employees are NOT approved yet
    };

    await _userRepository.AddAsync(newUser);
    
    // ❌ Do NOT generate a token yet (since the employee is not approved)
    return "Registration successful! Waiting for Admin approval.";
}

     public async Task<string> LoginAsync(string email, string password)
{
    var user = await _userRepository.GetByEmailAsync(email);
    if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        throw new Exception("Invalid credentials.");

    if (user.Role == "Employee" && !user.IsApproved)
        throw new Exception("Your account has not been approved by an Admin yet.");

    return GenerateJwtToken(user);
}


private string GenerateJwtToken(User user)
{
    var jwtKey = _configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(jwtKey))
        throw new Exception("JWT Key is missing from configuration.");

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),  // user.Id is a Guid here
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

    if (!string.IsNullOrEmpty(user.CompanyId))
    {
        claims.Add(new Claim("CompanyId", user.CompanyId));
    }

    var token = new JwtSecurityToken(
        _configuration["Jwt:Issuer"] ?? "DefaultIssuer",
        _configuration["Jwt:Audience"] ?? "DefaultAudience",
        claims,
        expires: DateTime.UtcNow.AddHours(2),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}


private string GenerateKioskJwtToken(string kioskId)
{
    var jwtKey = _configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(jwtKey))
        throw new Exception("JWT Key is missing from configuration.");

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, kioskId)  // Using kioskId as string
    };

    var token = new JwtSecurityToken(
        _configuration["Jwt:Issuer"] ?? "DefaultIssuer",
        _configuration["Jwt:Audience"] ?? "DefaultAudience",
        claims,
        expires: DateTime.UtcNow.AddHours(2),
        signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
}





public async Task<string> RegisterAdminAsync(string fullName, string email, string password, string companyId)
{
    var existingUser = await _userRepository.GetByEmailAsync(email);
    if (existingUser != null)
        throw new Exception("User already exists.");

    // Ensure the company ID is unique for Admins
    var existingCompany = await _userRepository.GetByCompanyIdAsync(companyId);
    if (existingCompany != null)
        throw new Exception("Company ID is already taken.");

    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
    var newAdmin = new User
    {
        FullName = fullName,
        Email = email,
        PasswordHash = hashedPassword,
        Role = "Admin",  // ✅ Admin role
        CompanyId = companyId  // ✅ Assign company ID
    };

    await _userRepository.AddAsync(newAdmin);
    return GenerateJwtToken(newAdmin);
}


    }
    
}
