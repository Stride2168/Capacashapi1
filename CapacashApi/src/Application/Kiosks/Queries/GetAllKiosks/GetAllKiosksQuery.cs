using MediatR;
using Capacash.Domain.Entities;
using System.Collections.Generic;

namespace Capacash.Application.Kiosks.Queries
{
    public record GetAllKiosksQuery(string CompanyId) : IRequest<List<Kiosk>>;
}
