using MediatR;

namespace Capacash.Application.Kiosks.Commands;

public record DeleteKioskCommand(Guid Id) : IRequest<Unit>;
