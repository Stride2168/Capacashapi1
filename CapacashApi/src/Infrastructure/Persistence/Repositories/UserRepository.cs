using Capacash.Application.Common.Interfaces;
using Capacash.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Capacash.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Capacash.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<bool> ExistsAdminForCompanyAsync(string companyId)
    {
        return await _context.Users.AnyAsync(u => u.CompanyId == companyId && u.Role == "Admin");
    }
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByCompanyIdAsync(string companyId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.CompanyId == companyId && u.Role == "Admin");
    }

    public async Task<List<User>> GetUnapprovedUsersAsync()
    {
        return await _context.Users.Where(u => !u.IsApproved && u.Role == "Employee").ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user); // ✅ Fixed _dbContext -> _context
        await _context.SaveChangesAsync();
    }
    public async Task<List<User>> GetUnapprovedUsersByCompanyAsync(string companyId)
{
    return await _context.Users
        .Where(u => !u.IsApproved && u.Role == "Employee" && u.CompanyId == companyId)
        .ToListAsync();
}
public async Task UpdateRangeAsync(List<User> users)
{
    _context.Users.UpdateRange(users);
    await _context.SaveChangesAsync();
}public async Task<List<User>> GetUnapprovedEmployeesByCompanyAsync(string companyId)
{
    return await _context.Users
        .Where(u => !u.IsApproved && u.Role == "Employee" && u.CompanyId == companyId)
        .ToListAsync();

        
}
public async Task<User?> GetByIdAsync(Guid id)
{
    return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
}


}
