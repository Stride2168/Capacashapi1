// Application/Wallets/Queries/GetWalletByUserIdQueryHandler.cs
using Capacash.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Capacash.Application.Commons.DTOs;

namespace Capacash.Application.Wallets.Queries;

public class GetWalletByUserIdQueryHandler : IRequestHandler<GetWalletByUserIdQuery, WalletDto>
{
    private readonly IWalletRepository _walletRepository;
    private readonly ILogger<GetWalletByUserIdQueryHandler> _logger;
    
    public GetWalletByUserIdQueryHandler(
        IWalletRepository walletRepository,
        ILogger<GetWalletByUserIdQueryHandler> logger)
    {
        _walletRepository = walletRepository;
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

        _logger.LogInformation("✅ Wallet found: {WalletId}", wallet.Id);
        return new WalletDto(
            wallet.Id,
            wallet.Balance,
            wallet.UserId,
            wallet.CreatedAt
        );
    }

}