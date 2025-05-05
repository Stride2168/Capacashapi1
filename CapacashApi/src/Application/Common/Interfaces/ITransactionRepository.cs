using System.Threading.Tasks;
using Capacash.Domain.Entities;

namespace Capacash.Application.Common.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(Guid userId);
        Task<Transaction?> GetTransactionByIdAsync(int id);
        Task CreateTransactionAsync(Transaction transaction);
         Task<List<Transaction>> GetTransactionsByCompanyIdAsync(string companyId);
            Task<List<Transaction>> GetTransactionsByUserIdAsync(Guid userId, string? filter = null);
             Task AddAsync(Transaction transaction);
             
    }
}
