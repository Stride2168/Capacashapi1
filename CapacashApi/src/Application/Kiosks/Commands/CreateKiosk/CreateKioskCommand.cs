using MediatR;

namespace Capacash.Application.Kiosks.Commands.CreateKiosk
{
    public class CreateKioskCommand : IRequest<Guid> // returns Kiosk Id
    {
        public string KioskId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Location { get; set; }
        public string CompanyId { get; set; } = string.Empty;

        public CreateKioskCommand(string kioskId, string password, string? name, string? location, string companyId)
        {
            KioskId = kioskId;
            Password = password;
            Name = name;
            Location = location;
            CompanyId = companyId;
        }
    }
}
