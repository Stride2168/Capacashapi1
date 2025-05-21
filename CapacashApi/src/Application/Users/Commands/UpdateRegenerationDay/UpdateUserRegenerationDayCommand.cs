using MediatR;
using System;

namespace Capacash.Application.Users.Commands.UpdateRegenerationDay
{
    public record UpdateUserRegenerationDayCommand(Guid UserId, int RegenerationDay) : IRequest<Unit>;
}
