using MediatR;

namespace Capacash.Application.Kiosks.Commands.EnableKiosk;

public record EnableKioskCommand(Guid Id) : IRequest<Unit>;
