using Capacash.Domain.Entities;
using Capacash.Application.Common.Interfaces;
using Capacash.Application.Wallets.Commands;

public class TransferFundsCommandHandler : IRequestHandler<TransferFundsCommand, string>
{
    private readonly IWalletRepository _walletRepo;
    private readonly IUserRepository _userRepo;
private readonly ITransactionRepository _transactionRepo;

public TransferFundsCommandHandler(
    IWalletRepository walletRepo,
    IUserRepository userRepo,
    ITransactionRepository transactionRepo)
{
    _walletRepo = walletRepo;
    _userRepo = userRepo;
    _transactionRepo = transactionRepo;
}


    public async Task<string> Handle(TransferFundsCommand request, CancellationToken cancellationToken)
    {
        var sender = await _userRepo.GetByIdAsync(request.SenderId);
        var recipient = await _userRepo.GetByIdAsync(request.RecipientId);

        if (sender == null || recipient == null)
            throw new ArgumentException("Sender or recipient not found.");

        if (sender.CompanyId != recipient.CompanyId)
            throw new UnauthorizedAccessException("Transfers allowed only within the same company.");

        var senderWallet = await _walletRepo.GetByUserIdAsync(request.SenderId);
        var recipientWallet = await _walletRepo.GetByUserIdAsync(request.RecipientId);

        if (senderWallet == null || recipientWallet == null)
            throw new InvalidOperationException("Sender or recipient wallet not found.");

       // Execute wallet transfer logic
senderWallet.TransferTo(recipientWallet, request.Amount);

await _walletRepo.UpdateAsync(senderWallet);
await _walletRepo.UpdateAsync(recipientWallet);

// Log transfer transactions
await _transactionRepo.AddAsync(new Transaction(
    userId: request.SenderId,
    amount: -request.Amount,
    transactionType: "TransferSent"
));

await _transactionRepo.AddAsync(new Transaction(
    userId: request.RecipientId,
    amount: request.Amount,
    transactionType: "TransferReceived"
));


        return $"Transferred ₱{request.Amount} to {recipient.FullName}";
    }
}
