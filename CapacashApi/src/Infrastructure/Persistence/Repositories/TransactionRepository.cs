using System.Threading.Tasks;
using Capacash.Domain.Entities;
using Capacash.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Capacash.Infrastructure.Persistence;
using Capacash.Application.Transactions.Queries.GetUserTransactions;

namespace Capacash.Application.Common.Interfaces // Removed unnecessary semicolon here
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Capacash.Domain.Entities.Transaction>> GetTransactionsByUserIdAsync(Guid userId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }
          public IQueryable<Capacash.Domain.Entities.Transaction> GetTransactionsByUserIdQueryable(Guid userId)
    {
        return _context.Transactions
            .Where(t => t.UserId == userId)
            .AsQueryable();
    }
public async Task AddAsync(Capacash.Domain.Entities.Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<Capacash.Domain.Entities.Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task CreateTransactionAsync(Capacash.Domain.Entities.Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Capacash.Domain.Entities.Transaction>> GetTransactionsByCompanyIdAsync(string companyId)
        {
            return await _context.Transactions
                .Where(t => _context.Users.Any(u => u.Id == t.UserId && u.CompanyId == companyId))
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<List<Capacash.Domain.Entities.Transaction>> GetTransactionsByUserIdAsync(Guid userId, string? filter = null)
        {
            IQueryable<Capacash.Domain.Entities.Transaction> query = _context.Transactions.Where(t => t.UserId == userId);

          if (!string.IsNullOrWhiteSpace(filter))
{
    query = query.Where(t => t.TransactionId != null && t.TransactionId.Contains(filter));
}

            return await query.OrderByDescending(t => t.TransactionDate).ToListAsync();
        }
    }
}
