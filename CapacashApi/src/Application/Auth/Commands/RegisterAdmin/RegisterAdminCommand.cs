namespace Capacash.Application.Auth.Commands
{
    public record RegisterAdminCommand(
        string FullName,
        string Email,
        string Password,
        string CompanyId,
        string PhoneNumber
    ) : IRequest<string>;
}
