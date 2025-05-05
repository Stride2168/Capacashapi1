// Application/Wallets/Commands/CreditWalletCommand.cs

namespace Capacash.Application.Wallets.Commands.CreditWallet.CreditWalletCommand
{
    public record CreditWalletCommand(
        Guid UserId,
        decimal Amount,
        Guid AdminId,
        string AdminCompanyId
    ) : IRequest<Unit>;
}


