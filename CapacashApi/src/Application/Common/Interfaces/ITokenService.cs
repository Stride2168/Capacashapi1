using Capacash.Domain.Entities;

public interface ITokenService
{
    string GenerateKioskToken(Kiosk kiosk);
    // ... other token methods
}