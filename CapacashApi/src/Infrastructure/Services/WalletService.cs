using Capacash.Application.Common.Interfaces;
using Capacash.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Capacash.Infrastructure.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        // Method to create a wallet for a user
    // Infrastructure/Services/WalletService.cs
public async Task<Wallet> CreateWalletAsync(Guid userId, String companyId)
{
    // Validate inputs
    if (userId == Guid.Empty)
        throw new ArgumentException("User ID cannot be empty");
    if (companyId == String.Empty)
        throw new ArgumentException("Company ID cannot be empty");

    // Check if wallet already exists
    var existingWallet = await _walletRepository.GetByUserIdAsync(userId);
    if (existingWallet != null)
        throw new InvalidOperationException("Wallet already exists for this user");

    // Create new wallet
    return await _walletRepository.CreateWalletAsync(userId, companyId);
}

        // Method to add credit to an employee's wallet
        public async Task AddCreditToWalletAsync(Guid userId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId);

            if (wallet == null)
            {
                throw new Exception("Wallet not found for the user.");
            }

            // Add the amount to the wallet balance
            wallet.Balance += amount;
            wallet.UpdatedAt = DateTime.UtcNow;  // Update the timestamp

            await _walletRepository.UpdateWalletAsync(wallet);  // Use repository to update wallet
        }

        // Method to get all employee wallets
        public async Task<List<Wallet>> GetAllWalletsAsync()
        {
            return await _walletRepository.GetAllWalletsAsync(); // Should return Task<List<Wallet>>
        }

        // Method to get a specific employee's wallet
        public async Task<Wallet> GetWalletByUserIdAsync(Guid userId)
        {
            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId);

            if (wallet == null)
            {
                throw new Exception("Wallet not found for the user.");
            }

            return wallet;  // Should return Task<Wallet>
        }
        public async Task<List<Wallet>> GetWalletsByCompanyIdAsync(string companyId)
    {
        return await _walletRepository.GetWalletsByCompanyIdAsync(companyId);  // Filters wallets by CompanyId
    }
    }
}
