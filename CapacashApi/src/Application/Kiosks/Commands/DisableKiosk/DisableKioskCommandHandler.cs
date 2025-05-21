using Capacash.Application.Common.Interfaces;
using Capacash.Application.Kiosks.Commands;
using MediatR;

namespace Capacash.Application.Kiosks.Commands.DisableKiosk;

public class DisableKioskCommandHandler : IRequestHandler<DisableKioskCommand, Unit>
{
    private readonly IAppDbContext _context;

    public DisableKioskCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DisableKioskCommand request, CancellationToken cancellationToken)
    {
        var kiosk = await _context.Kiosks.FindAsync(new object[] { request.Id }, cancellationToken);

        if (kiosk == null || kiosk.IsDeleted)
            throw new KeyNotFoundException("Kiosk not found.");

        kiosk.Disable();
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
