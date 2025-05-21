using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Capacash.Application.Common.Interfaces;
using Capacash.Application.Users.Commands.RegenerateUserCredit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Capacash.Application.Wallets.Commands.MonthlyRegenerateCredit
{
    public class MonthlyRegenerateCreditHandler : IRequestHandler<MonthlyRegenerateCreditCommand, Unit>
    {
        private readonly IAppDbContext _context;
        private readonly IMediator _mediator;

        public MonthlyRegenerateCreditHandler(IAppDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

      public async Task<Unit> Handle(MonthlyRegenerateCreditCommand request, CancellationToken cancellationToken)
{
    var today = DateTime.UtcNow.Day;

    // Get all approved users who have a regeneration day set AND it's today
    var usersToRegenerate = await _context.Users
        .Where(u =>
            u.IsApproved &&
            u.RegenerationDayOfMonth != null &&
            u.RegenerationDayOfMonth == today)
        .ToListAsync(cancellationToken);

    foreach (var user in usersToRegenerate)
    {
        // Find the corresponding WalletRegenerationSetting for the user's company
        var setting = await _context.WalletRegenerationSettings
            .FirstOrDefaultAsync(s => s.CompanyId == user.CompanyId, cancellationToken);

        if (setting == null)
            continue;

        await _mediator.Send(new RegenerateUserCreditCommand(
            user.Id,
            setting.MonthlyAmount,
            "System", // Auto-initiated
            "Monthly Auto-Regeneration"
        ), cancellationToken);
    }

    return Unit.Value;
}

    }
}