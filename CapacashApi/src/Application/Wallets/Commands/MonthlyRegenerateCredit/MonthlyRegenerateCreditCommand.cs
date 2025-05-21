using MediatR;

namespace Capacash.Application.Wallets.Commands.MonthlyRegenerateCredit
{
    public record MonthlyRegenerateCreditCommand : IRequest<Unit>;
}
