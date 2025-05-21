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
    _logger.LogInformation("Processing transaction request...");
    
    // 1. Input Validation
    if (request.Amount <= 0)
        throw new ArgumentException("Amount must be greater than zero", nameof(request.Amount));
    
    _logger.LogInformation($"Amount: {request.Amount}");
    
    // 2. Get User
    var user = await _userRepository.GetUserByIdAsync(request.UserId)
        ?? throw new UnauthorizedAccessException("User not found");
    
    _logger.LogInformation($"User found: {user.FullName}");
    
    // 3. Get Kiosk (using string KioskId)
    var kiosk = await _context.Kiosks
        .FirstOrDefaultAsync(k => k.KioskId == request.KioskId, cancellationToken)
        ?? throw new InvalidOperationException("Invalid kiosk");
    
    _logger.LogInformation($"Kiosk found: {kiosk.KioskId}");
    
    // 4. Company Validation
    if (user.CompanyId != kiosk.CompanyId)
        throw new InvalidOperationException("User and kiosk company mismatch");
    
    // 5. Get Wallet
    var wallet = await _context.Wallets
        .FirstOrDefaultAsync(w => w.UserId == request.UserId, cancellationToken)
        ?? throw new InvalidOperationException("Wallet not found");
    
    _logger.LogInformation($"Wallet found for user {wallet.UserId}");
    
    // 6. Process Transaction
    wallet.DeductBalance(request.Amount);

    var transaction = new Transaction(
        userId: request.UserId,
        amount: request.Amount,
        transactionType: request.TransactionType,
        companyId: user.CompanyId,
        kioskId: kiosk.Id,
        kioskName: kiosk.Name // Add kiosk name here
    );

    _context.Transactions.Add(transaction);
    await _context.SaveChangesAsync(cancellationToken);

    // 7. Log and return
    _logger.LogInformation($"Processed transaction for user {request.UserId} at kiosk {kiosk.KioskId}");
    return $"Purchased ₱{request.Amount:0.00} at {kiosk.Name ?? "Kiosk"} {kiosk.KioskId}";
}

}