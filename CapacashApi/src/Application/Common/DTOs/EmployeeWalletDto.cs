namespace Capacash.Application.Commons.DTOs{
public record EmployeeWalletDto(
    Guid UserId,
    string FullName,
    string Email,
    decimal Balance,
    DateTime LastUpdated
);}