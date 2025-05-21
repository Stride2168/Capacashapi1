using MediatR;
using System.Threading.Tasks;
using Capacash.Application.Wallets.Commands.MonthlyRegenerateCredit;

public class RecurringMediatorExecutor
{
    private readonly IMediator _mediator;

    public RecurringMediatorExecutor(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task ExecuteMonthlyRegeneration()
    {
        await _mediator.Send(new MonthlyRegenerateCreditCommand());
    }
}
