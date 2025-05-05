namespace Capacash.Application.Auth.Commands
{
    public record RegisterEmployeeCommand(
        string FullName,
        string Email,
        string Password,
        string CompanyId,
        string PhoneNumber
    ) : IRequest<string>; // Returns JWT token
}
