using System;
using System.Threading.Tasks;
using Capacash.Domain.Entities;
using Capacash.Application.Common.Interfaces;

namespace Capacash.Infrastructure.Services
{
    public class TransactionService : ITransactionService  // Implement the ITransactionService interface
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IUserRepository _userRepository;  // Add IUserRepository to get user details

        public TransactionService(ITransactionRepository transactionRepository, IWalletRepository walletRepository, IUserRepository userRepository)
        {
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
            _userRepository = userRepository;  // Inject IUserRepository to access user data
        }

        public async Task<bool> ProcessTransactionAsync(Guid userId, decimal amount)
        {
            var wallet = await _walletRepository.GetWalletByUserIdAsync(userId);
            if (wallet == null)
                throw new InvalidOperationException("Wallet not found.");

            if (wallet.Balance < amount)
                throw new InvalidOperationException("Insufficient balance.");

            // Get user details to fetch the CompanyId
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            // Check if companyId is null
            if (string.IsNullOrEmpty(user.CompanyId))
                throw new InvalidOperationException("User does not have a valid company ID.");

            // ✅ Generate a new transaction with a unique Transaction ID and the CompanyId
            var transaction = new Transaction(userId, amount);
            await _transactionRepository.CreateTransactionAsync(transaction);

            // Deduct balance
            wallet.DeductBalance(amount);
            await _walletRepository.UpdateWalletAsync(wallet);

            return true;
        }
    }
}
