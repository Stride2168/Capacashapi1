using System;
using System.Threading.Tasks;
using Capacash.Domain.Entities;

namespace Capacash.Application.Common.Interfaces
{
    public interface IWalletService
    {
     Task AddCreditToWalletAsync(Guid userId, decimal amount);
        Task<Wallet> GetWalletByUserIdAsync(Guid userId);
        Task<List<Wallet>> GetAllWalletsAsync();
        Task<bool> CreateWalletForUserAsync(Guid userId);
           Task<List<Wallet>> GetWalletsByCompanyIdAsync(string companyId); 
    }
}
