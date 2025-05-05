// Application/Users/Commands/ApproveUserCommand.cs
namespace Capacash.Application.Users.Commands.ApproveUsers.ApproveUserCommand;

public record ApproveUserCommand(
    Guid UserId,
    Guid AdminId,
    string CompanyId
) : IRequest<Unit>;

public record BulkApproveUsersCommand(
    Guid AdminId,
    string CompanyId
) : IRequest<int>;