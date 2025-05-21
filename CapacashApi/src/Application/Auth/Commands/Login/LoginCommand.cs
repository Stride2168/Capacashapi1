namespace Capacash.Application.Auth.Commands
{
    // Create a new DTO to hold both Token and Role
    public record LoginResponse(string Token, string Role);

    public record LoginCommand(
        string Email,
        string Password
    ) : IRequest<LoginResponse>; // Change return type to LoginResponse
}
