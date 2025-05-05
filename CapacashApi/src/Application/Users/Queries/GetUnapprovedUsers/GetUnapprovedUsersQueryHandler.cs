
using Capacash.Application.Users.Queries.GetUnapprovedUsers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Capacash.Application.Common.Interfaces;
using Capacash.Application.Commons.DTOs;
public class GetUnapprovedUsersQueryHandler : IRequestHandler<GetUnapprovedUsersQuery, List<UnapprovedUserDto>>
{
    private readonly IAppDbContext _context;

    public GetUnapprovedUsersQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UnapprovedUserDto>> Handle(GetUnapprovedUsersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Where(u => !u.IsApproved && u.CompanyId == request.CompanyId)
            .Select(u => new UnapprovedUserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email
            })
            .ToListAsync(cancellationToken);
    }
}
