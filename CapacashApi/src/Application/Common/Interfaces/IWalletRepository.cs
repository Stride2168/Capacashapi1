using Capacash.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Capacash.Application.Common.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet> CreateWalletAsync(Guid userId);
        Task<Wallet?> GetWalletByUserIdAsync(Guid userId);
        Task<Wallet?> GetWalletByUserIdAsync(string userId);
             Task<List<Wallet>> GetAllWalletsAsync();
        Task UpdateWalletAsync(Wallet wallet);
  Task<List<Wallet>> GetWalletsByCompanyIdAsync(string companyId);
    }
}
