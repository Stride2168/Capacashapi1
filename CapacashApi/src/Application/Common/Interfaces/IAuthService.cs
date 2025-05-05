// Application/Common/Interfaces/IAuthService.cs
namespace Capacash.Application.Common.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(string email, string password);
    Task<string> RegisterUserAsync(string fullName, string email, string password, string companyId, string PhoneNumber);
    Task<string> RegisterAdminAsync(string fullName, string email, string password, string companyId);
}