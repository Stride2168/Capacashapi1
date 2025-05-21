using System.Threading.Tasks;
using Capacash.Domain.Entities;

namespace Capacash.Application.Common.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Capacash.Domain.Entities.Transaction>> GetTransactionsByUserIdAsync(Guid userId);

        Task<Capacash.Domain.Entities.Transaction?> GetTransactionByIdAsync(int id);
        Task CreateTransactionAsync(Capacash.Domain.Entities.Transaction transaction);
         Task<List<Capacash.Domain.Entities.Transaction>> GetTransactionsByCompanyIdAsync(string companyId);
            Task<List<Capacash.Domain.Entities.Transaction>> GetTransactionsByUserIdAsync(Guid userId, string? filter = null);
             Task AddAsync(Capacash.Domain.Entities.Transaction transaction);
              IQueryable<Capacash.Domain.Entities.Transaction> GetTransactionsByUserIdQueryable(Guid userId);
    }
}
