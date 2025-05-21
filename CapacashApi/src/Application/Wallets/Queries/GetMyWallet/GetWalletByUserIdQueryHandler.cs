using Capacash.Application.Common.Interfaces;
using Capacash.Application.Commons.DTOs;
using Capacash.Application.Wallets.Queries;
using Microsoft.Extensions.Logging;

public class GetWalletByUserIdQueryHandler : IRequestHandler<GetWalletByUserIdQuery, WalletDto>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUserRepository _userRepository;  // Add IUserRepository
    private readonly ILogger<GetWalletByUserIdQueryHandler> _logger;
    
    public GetWalletByUserIdQueryHandler(
        IWalletRepository walletRepository,
        IUserRepository userRepository,  // Inject IUserRepository
        ILogger<GetWalletByUserIdQueryHandler> logger)
    {
        _walletRepository = walletRepository;
        _userRepository = userRepository;  // Assign to class variable
        _logger = logger;
    }

    public async Task<WalletDto> Handle(GetWalletByUserIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔍 Extracted User ID: {UserId}", request.UserId);

        if (string.IsNullOrEmpty(request.UserId))
        {
            _logger.LogWarning("❌ User ID is missing or invalid.");
            throw new UnauthorizedAccessException("Invalid user session.");
        }

        var wallet = await _walletRepository.GetWalletByUserIdAsync(request.UserId);
        if (wallet == null)
        {
            _logger.LogWarning("❌ Wallet not found for User ID: {UserId}", request.UserId);
            throw new Capacash.Application.Common.Exceptions.NotFoundException("Wallet not found.");
        }

        var user = await _userRepository.GetByIdAsync(Guid.Parse(request.UserId)); // Fetch User by UserId
        if (user == null)
        {
            _logger.LogWarning("❌ User not found for User ID: {UserId}", request.UserId);
            throw new UnauthorizedAccessException("User not found.");
        }

        _logger.LogInformation("✅ Wallet found: {WalletId}", wallet.Id);

        return new WalletDto(
            wallet.Id,
            wallet.Balance,
            wallet.UserId,
            wallet.CreatedAt,
            user.FullName  // Pass FullName to WalletDto
        );
    }
}
