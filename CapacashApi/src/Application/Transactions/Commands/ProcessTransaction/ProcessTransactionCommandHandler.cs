using Capacash.Domain.Entities;
using Capacash.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

public class ProcessTransactionCommandHandler : IRequestHandler<ProcessTransactionCommand, string>
{
    private readonly IAppDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProcessTransactionCommandHandler> _logger;

    public ProcessTransactionCommandHandler(
         IAppDbContext context,
        IUserRepository userRepository,
        ILogger<ProcessTransactionCommandHandler> logger)
    {
        _context = context; 
        _userRepository = userRepository;
        _logger = logger;
    }

   public async Task<string> Handle(ProcessTransactionCommand request, CancellationToken cancellationToken)
{
    var user = await _userRepository.GetUserByIdAsync(request.UserId)
        ?? throw new UnauthorizedAccessException("User not found");

    // Get kiosk by its string-based ID
    var kiosk = await _context.Kiosks
        .FirstOrDefaultAsync(k => k.KioskId == request.KioskId, cancellationToken)
        ?? throw new InvalidOperationException("Invalid kiosk");

    // Ensure user belongs to same company
    if (user.CompanyId != kiosk.CompanyId)
        throw new InvalidOperationException("User and kiosk company mismatch");

    var wallet = await _context.Wallets
        .FirstOrDefaultAsync(w => w.UserId == request.UserId, cancellationToken)
        ?? throw new InvalidOperationException("Wallet not found");

    // Deduct balance
    wallet.DeductBalance(request.Amount);

    // Create transaction using kiosk.Guid (not kiosk.KioskId string)
    var transaction = new Transaction(
        userId: request.UserId,
        amount: request.Amount,
        transactionType: request.TransactionType,
        companyId: user.CompanyId,
        kioskId: kiosk.Id  // This is Guid, as required
    );

    _context.Transactions.Add(transaction);
    await _context.SaveChangesAsync(cancellationToken);

    return $"Purchased ₱{request.Amount:0.00} at Kiosk {request.KioskId}";
}


}
