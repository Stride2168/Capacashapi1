using Capacash.Application.Common.Interfaces;
using Capacash.Domain.Entities;
using MediatR;

namespace Capacash.Application.SuperAdmin.Queries;

public class GetPendingAdminsQueryHandler : IRequestHandler<GetPendingAdminsQuery, List<User>>
{
    private readonly IUserRepository _userRepo;

    public GetPendingAdminsQueryHandler(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<List<User>> Handle(GetPendingAdminsQuery request, CancellationToken cancellationToken)
    {
        return await _userRepo.GetUsersByConditionAsync(u => u.Role == "Admin" && !u.IsApproved);
    }
}
