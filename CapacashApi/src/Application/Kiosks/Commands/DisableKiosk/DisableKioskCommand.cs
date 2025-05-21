using MediatR;

namespace Capacash.Application.Kiosks.Commands;

public record DisableKioskCommand(Guid Id) : IRequest<Unit>;
