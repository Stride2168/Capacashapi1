// Application/Users/Commands/RegenerateUserCredit/RegenerateUserCreditCommand.cs
namespace Capacash.Application.Users.Commands.RegenerateUserCredit;

public record RegenerateUserCreditCommand(
    Guid UserId,
    decimal Amount,
    string InitiatedBy,
    string? Notes = null) : IRequest<CreditRegenerationResult>;

public record CreditRegenerationResult(
    string Message,
    decimal NewBalance);