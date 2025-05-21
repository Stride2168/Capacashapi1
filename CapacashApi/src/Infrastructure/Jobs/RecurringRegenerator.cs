using Capacash.Application.Wallets.Commands.MonthlyRegenerateCredit;
using MediatR;

public class RecurringRegenerator : IRecurringRegenerator
{
    private readonly IMediator _mediator;

    public RecurringRegenerator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Execute()
    {
        await _mediator.Send(new MonthlyRegenerateCreditCommand());
    }
}
