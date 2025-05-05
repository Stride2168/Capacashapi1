// Application/Wallets/Commands/CreditWalletCommandHandler.cs
using Capacash.Application.Common.Interfaces;
using Capacash.Domain.Entities;
using Capacash.Application.Wallets.Commands.CreditWallet.CreditWalletCommand;

namespace Capacash.Application.Wallets.Commands
{
    public class CreditWalletCommandHandler : IRequestHandler<CreditWalletCommand, Unit>
    {
        private readonly IWalletService _walletService;
        private readonly IUserRepository _userRepository;
        private readonly IAppDbContext _context;
        
        public CreditWalletCommandHandler(
            IWalletService walletService,
            IUserRepository userRepository,
            IAppDbContext context)
        {
            _walletService = walletService;
            _userRepository = userRepository;
            _context = context;
        }

   public async Task<Unit> Handle(CreditWalletCommand request, CancellationToken cancellationToken)
{
    var employee = await _userRepository.GetUserByIdAsync(request.UserId);
    if (employee == null)
    {
        throw new ArgumentException("Employee not found.");
    }

    // Ensure the admin can only credit users within the same company
    if (employee.CompanyId != request.AdminCompanyId)
    {
        throw new UnauthorizedAccessException("You are not authorized to credit this employee's wallet.");
    }

    // 1. Add money to the wallet
    await _walletService.AddCreditToWalletAsync(request.UserId, request.Amount);

    // 2. Create a new Transaction properly
    var transaction = new Transaction(
        userId: request.UserId,
        amount: request.Amount,
        transactionType: "Credit" // <-- set to Credit
    )
    {
        User = employee // <-- link the User navigation property
    };

    // 3. Save the transaction
    _context.Transactions.Add(transaction);
    await _context.SaveChangesAsync(cancellationToken);

    return Unit.Value;
}


    }
}