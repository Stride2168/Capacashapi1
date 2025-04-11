using MediatR;
using Capacash.Domain.Entities;
using Capacash.Application.Common.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capacash.Application.Kiosks.Queries
{
    public class GetAllKiosksQueryHandler : IRequestHandler<GetAllKiosksQuery, List<Kiosk>>
    {
        private readonly IKioskRepository _kioskRepository;

        public GetAllKiosksQueryHandler(IKioskRepository kioskRepository)
        {
            _kioskRepository = kioskRepository;
        }

        public async Task<List<Kiosk>> Handle(GetAllKiosksQuery request, CancellationToken cancellationToken)
        {
            return await _kioskRepository.GetAllKiosksAsync(request.CompanyId);
        }
    }
}
