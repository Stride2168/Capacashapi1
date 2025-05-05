// Application/Users/Commands/BulkApproveUsersCommandHandler.cs
using Capacash.Application.Common.Interfaces;
using Capacash.Application.Users.Commands.ApproveUsers.ApproveUserCommand;
using Microsoft.Extensions.Logging;

namespace Capacash.Application.Users.Commands;

public class BulkApproveUsersCommandHandler : IRequestHandler<BulkApproveUsersCommand, int>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<BulkApproveUsersCommandHandler> _logger;

    public BulkApproveUsersCommandHandler(
        IUserRepository userRepository,
        ILogger<BulkApproveUsersCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<int> Handle(BulkApproveUsersCommand request, CancellationToken cancellationToken)
    {
        var unapprovedUsers = await _userRepository.GetUnapprovedEmployeesByCompanyAsync(request.CompanyId);
        
        if (!unapprovedUsers.Any())
        {
            _logger.LogInformation("No unapproved employees found for company {CompanyId}", request.CompanyId);
            return 0;
        }

        foreach (var user in unapprovedUsers)
        {
            user.IsApproved = true;
        }

        var count = await _userRepository.BulkUpdateAsync(unapprovedUsers);
        _logger.LogInformation("Bulk approved {Count} users for company {CompanyId}", count, request.CompanyId);

        return count;
    }
}