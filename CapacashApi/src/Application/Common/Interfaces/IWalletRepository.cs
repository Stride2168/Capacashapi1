using Capacash.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Capacash.Application.Common.Interfaces
{
    public interface IWalletRepository
    {

        Task<Wallet?> GetWalletByUserIdAsync(Guid userId);

             Task<List<Wallet>> GetAllWalletsAsync();
        Task UpdateWalletAsync(Wallet wallet);
     Task<List<Wallet>> GetWalletsByCompanyIdAsync(string companyId);   
  Task AddAsync(Wallet wallet);
    Task<Wallet?> GetByUserIdAsync(Guid userId);
        Task<Wallet?> GetWalletByUserIdAsync(string userId);

        Task<Wallet> CreateWalletAsync(Guid userId, String companyId);
         Task<List<Wallet>> GetEmployeeWalletsByCompanyIdAsync(string companyId);
         Task UpdateAsync(Wallet wallet);

    }
}
