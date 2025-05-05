namespace Capacash.Application.Wallets.Commands;

public record TransferFundsCommand(
    Guid SenderId,
    Guid RecipientId,
    decimal Amount
) : IRequest<string>;
