using Capacash.Application.Common.Interfaces;
using Capacash.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Capacash.Application.Kiosks.Commands.CreateKiosk
{
    public class CreateKioskCommandHandler : IRequestHandler<CreateKioskCommand, Guid>
    {
        private readonly IKioskRepository _kioskRepository;

        public CreateKioskCommandHandler(IKioskRepository kioskRepository)
        {
            _kioskRepository = kioskRepository;
        }

        public async Task<Guid> Handle(CreateKioskCommand request, CancellationToken cancellationToken)
        {
            // Check if kiosk already exists
            var existing = await _kioskRepository.GetKioskByKioskIdAndCompanyAsync(request.KioskId, request.CompanyId);
            if (existing != null)
                throw new InvalidOperationException("Kiosk with this ID already exists in your company.");

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create and save kiosk
            var kiosk = await _kioskRepository.CreateKioskAsync(
                request.KioskId, passwordHash, request.Name, request.Location, request.CompanyId);

            return kiosk.Id;
        }
    }
}
