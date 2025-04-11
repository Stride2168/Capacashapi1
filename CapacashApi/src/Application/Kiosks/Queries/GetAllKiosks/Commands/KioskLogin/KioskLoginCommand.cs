using MediatR;

namespace Capacash.Application.Kiosks.Commands.KioskLogin
{
    public class KioskLoginCommand : IRequest<string> // Returns JWT token
    {
        public string KioskId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public KioskLoginCommand(string kioskId, string password)
        {
            KioskId = kioskId;
            Password = password;
        }
    }
}
