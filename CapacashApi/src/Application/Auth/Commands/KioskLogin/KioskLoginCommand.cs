namespace Capacash.Application.Kiosks.Commands;

public record KioskLoginCommand(
    string KioskId,
    string Password
) : IRequest<AuthResult>;

public record AuthResult(string Token);