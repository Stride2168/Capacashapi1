namespace Capacash.Application.Auth.Commands
{
    public record LoginCommand(
        string Email,
        string Password
    ) : IRequest<string>;
}