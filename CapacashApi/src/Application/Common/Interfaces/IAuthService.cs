// Application/Common/Interfaces/IAuthService.cs
using Capacash.Domain.Entities;
namespace Capacash.Application.Common.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(string email, string password);
    Task<string> RegisterUserAsync(string fullName, string email, string password, string companyId, string PhoneNumber);
    Task<string> RegisterAdminAsync(string fullName, string email, string password, string companyId);
    string GenerateJwtToken(User user); 
      Task<string> GetUserRoleAsync(string email);
}