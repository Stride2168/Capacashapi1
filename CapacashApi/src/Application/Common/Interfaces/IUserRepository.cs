using Capacash.Domain.Entities;

namespace Capacash.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByCompanyIdAsync(string companyId);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
Task<List<User>> GetUnapprovedUsersByCompanyAsync(string companyId);
Task UpdateRangeAsync(List<User> users);
Task<List<User>> GetUnapprovedEmployeesByCompanyAsync(string companyId);
  Task<User?> GetByIdAsync(Guid id);
    Task<bool> ExistsAdminForCompanyAsync(string companyId);
}
