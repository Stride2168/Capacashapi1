// Application/Users/Commands/ApproveUserCommandHandler.cs
using Capacash.Application.Common.Interfaces;
using Capacash.Application.Common.Exceptions;
using Capacash.Application.Users.Commands.ApproveUsers.ApproveUserCommand;
using Microsoft.Extensions.Logging;
using Capacash.Domain.Entities;

namespace Capacash.Application.Users.Commands;

public class ApproveUserCommandHandler : IRequestHandler<ApproveUserCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly ILogger<ApproveUserCommandHandler> _logger;

    public ApproveUserCommandHandler(
        IUserRepository userRepository,
        IWalletRepository walletRepository,
        ILogger<ApproveUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _walletRepository = walletRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(ApproveUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            throw new UnauthorizedAccessException("User not found.");
        }

        _logger.LogInformation("Employee CompanyId: {UserCompanyId}, Admin CompanyId: {AdminCompanyId}", user.CompanyId, request.CompanyId);

        // Check if the employee's CompanyId matches the admin's CompanyId (as string)
        if (user.CompanyId != request.CompanyId)
        {
            _logger.LogWarning("Admin {AdminId} attempted to approve user from a different company", request.AdminId);
            throw new UnauthorizedAccessException("Cannot approve user from another company.");
        }

        if (user.Role != "Employee")
        {
            _logger.LogWarning("Attempted to approve a non-employee user: {UserId}", request.UserId);
            throw new InvalidOperationException("Only employees can be approved.");
        }

        if (user.IsApproved)
        {
            _logger.LogInformation("User {UserId} is already approved", request.UserId);
            return Unit.Value;  // Return success with no changes if already approved
        }

        user.IsApproved = true;
        await _userRepository.UpdateAsync(user);

        var wallet = new Wallet(user.Id, request.CompanyId)
        {
            Balance = 5000,  
            UpdatedAt = DateTime.UtcNow
        };

        await _walletRepository.AddAsync(wallet);

        _logger.LogInformation("User {UserId} approved and wallet created by admin {AdminId}", request.UserId, request.AdminId);

        return Unit.Value;
    }
}
