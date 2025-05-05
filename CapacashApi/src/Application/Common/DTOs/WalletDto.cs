namespace Capacash.Application.Commons.DTOs{
public record WalletDto(
    Guid Id,
    decimal Balance,
    Guid UserId,
    DateTime CreatedAt
);}