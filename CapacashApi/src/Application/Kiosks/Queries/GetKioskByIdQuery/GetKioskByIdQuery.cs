using Capacash.Domain.Entities;

namespace Capacash.Application.Kiosks.Queries
{
    public record GetKioskByIdQuery(Guid Id) : IRequest<Kiosk>;
}