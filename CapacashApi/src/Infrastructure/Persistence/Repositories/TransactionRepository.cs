using System.Threading.Tasks;
using Capacash.Domain.Entities;
using Capacash.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Capacash.Infrastructure.Persistence;

namespace Capacash.Application.Common.Interfaces
{   
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public async Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(Guid userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }
        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task CreateTransactionAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }
          public async Task<List<Transaction>> GetTransactionsByCompanyIdAsync(string companyId)
        {
            return await _context.Transactions
                .Where(t => _context.Users.Any(u => u.Id == t.UserId && u.CompanyId == companyId))
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }
     public async Task<List<Transaction>> GetTransactions1ByCompanyIdAsync(string companyId, DateTime? startDate, DateTime? endDate)
{
    var query = _context.Transactions
                        .Include(t => t.User)  // Ensure you include the related User entity
                        .AsQueryable();

    // Filter by CompanyId in the User entity, with null check for User
    query = query.Where(t => t.User != null && t.User.CompanyId == companyId);

    // Filter by Date Range if provided
    if (startDate.HasValue)
        query = query.Where(t => t.TransactionDate >= startDate.Value);

    if (endDate.HasValue)
        query = query.Where(t => t.TransactionDate <= endDate.Value);

    return await query.OrderByDescending(t => t.TransactionDate).ToListAsync();
}

    }
}
