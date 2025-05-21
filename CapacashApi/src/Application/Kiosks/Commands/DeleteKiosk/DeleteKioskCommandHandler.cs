using Capacash.Application.Common.Interfaces;
using Capacash.Application.Kiosks.Commands;
using MediatR;

namespace Application.Kiosks.Handlers;

public class DeleteKioskCommandHandler : IRequestHandler<DeleteKioskCommand, Unit>
{
    private readonly IAppDbContext _context;

    public DeleteKioskCommandHandler(IAppDbContext context) 
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteKioskCommand request, CancellationToken cancellationToken)
    {
        var kiosk = await _context.Kiosks.FindAsync(new object[] { request.Id }, cancellationToken);

        if (kiosk == null || kiosk.IsDeleted)
            throw new KeyNotFoundException("Kiosk not found.");

        kiosk.Delete();
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
