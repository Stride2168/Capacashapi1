using System;
using System.Threading.Tasks;
using Capacash.Domain.Entities;
using Capacash.Application.Common.Interfaces;

namespace Capacash.Application.Services
{
    public class TransactionService : ITransactionService  // Implement the ITransactionService interface
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;

        public TransactionService(ITransactionRepository transactionRepository, IWalletRepository walletRepository)
        {
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
        }

public async Task<bool> ProcessTransactionAsync(Guid userId, decimal amount)
{
    var wallet = await _walletRepository.GetWalletByUserIdAsync(userId);
    if (wallet == null)
        throw new InvalidOperationException("Wallet not found.");

    if (wallet.Balance < amount)
        throw new InvalidOperationException("Insufficient balance.");

    // Deduct balance
    wallet.DeductBalance(amount);
    await _walletRepository.UpdateWalletAsync(wallet);

    // ✅ Generate a new transaction with a unique Transaction ID
    var transaction = new Transaction(userId, amount);
    await _transactionRepository.CreateTransactionAsync(transaction);

    return true;
}




    }
}
