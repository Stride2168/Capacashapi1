using MediatR;

namespace Capacash.Application.SuperAdmin.Commands{

public record ApproveAdminRequestCommand(Guid UserId, bool IsApproved) : IRequest;}
