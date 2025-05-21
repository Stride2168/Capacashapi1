// Application/Users/Commands/ResetPassword/ResetPasswordCommand.cs
using MediatR;

namespace Capacash.Application.Users.Commands.ResetPassword;

public record ResetPasswordCommand(Guid UserId, string NewPassword) : IRequest<string>;
