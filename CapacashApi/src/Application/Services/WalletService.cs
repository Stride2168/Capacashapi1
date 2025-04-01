using Capacash.Application.Common.Interfaces;
using Capacash.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Capacash.Application.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<bool> CreateWalletForUserAsync(Guid userId)
        {
            var existingWallet = await _walletRepository.GetWalletByUserIdAsync(userId);
            if (existingWallet != null)
                return false; // Wallet already exists

            var wallet = await _walletRepository.CreateWalletAsync(userId);
            return wallet != null;
        }
    }
}
