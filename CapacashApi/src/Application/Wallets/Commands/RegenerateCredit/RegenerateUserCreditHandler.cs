    // Application/Users/Commands/RegenerateUserCredit/RegenerateUserCreditHandler.cs
    using Capacash.Application.Common.Interfaces;
    using Capacash.Domain.Entities;
    using Capacash.Domain.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Capacash.Application.Common.Exceptions;

    namespace Capacash.Application.Users.Commands.RegenerateUserCredit;

    public class RegenerateUserCreditHandler : IRequestHandler<RegenerateUserCreditCommand, CreditRegenerationResult>
    {
        private readonly IAppDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RegenerateUserCreditHandler> _logger;

        public RegenerateUserCreditHandler(
            IAppDbContext context,
            IUserRepository userRepository,
            ILogger<RegenerateUserCreditHandler> logger)
        {
            _context = context;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<CreditRegenerationResult> Handle(
            RegenerateUserCreditCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Validate user exists
            var user = await _userRepository.GetUserByIdAsync(request.UserId)
        ?? throw new Capacash.Application.Common.Exceptions.NotFoundException("User", request.UserId);

    ;

            // 2. Get or create wallet
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == request.UserId, cancellationToken)
                ?? new Wallet(request.UserId, user.CompanyId);

            if (wallet.Id == Guid.Empty)
            {
                _context.Wallets.Add(wallet);
            }

            // 3. Apply credit regeneration
            wallet.AddBalance(request.Amount);

            // 4. Create audit record
            var regeneration = new CreditRegeneration(
                userId: request.UserId,
                amount: request.Amount,
                type: RegenerationType.ManualAdjustment,
                initiatedBy: request.InitiatedBy,
                notes: request.Notes)
            {
                User = user
            };

            // Add to context
            _context.CreditRegenerations.Add(regeneration);
            
            // 5. Save changes
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Regenerated {Amount} credits for user {Email}", 
                request.Amount, user.Email);

            return new CreditRegenerationResult(
                $"Successfully regenerated ₱{request.Amount:0.00} for {user.FullName}",
                wallet.Balance);
        }
    }