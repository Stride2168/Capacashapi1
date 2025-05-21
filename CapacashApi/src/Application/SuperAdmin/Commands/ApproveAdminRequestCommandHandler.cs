using Capacash.Application.Common.Interfaces;
using Capacash.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Capacash.Application.SuperAdmin.Commands;

public class ApproveAdminRequestCommandHandler : IRequestHandler<ApproveAdminRequestCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly INotificationService _notification;
    private readonly IWalletRepository _walletRepository;

    public ApproveAdminRequestCommandHandler(
        IUserRepository userRepo,
        INotificationService notification,
        IWalletRepository walletRepository)
    {
        _userRepo = userRepo;
        _notification = notification;
        _walletRepository = walletRepository;
    }

    public async Task Handle(ApproveAdminRequestCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.GetByIdAsync(request.UserId);
        if (user == null || user.Role != "Admin")
            throw new KeyNotFoundException("Admin user not found.");

        user.IsApproved = request.IsApproved;
        await _userRepo.UpdateAsync(user);

        // ✅ If admin is approved, create wallet (if not exists)
        if (request.IsApproved)
        {
            var existingWallet = await _walletRepository.GetWalletByUserIdAsync(user.Id);
            if (existingWallet == null)
            {
                var wallet = new Wallet(user.Id, user.CompanyId!)
                {
                    Balance = 0, // Default wallet amount for Admin (adjust as needed)
                    UpdatedAt = DateTime.UtcNow
                };
                await _walletRepository.AddAsync(wallet);
            }
        }

        // Notify the admin
        var message = request.IsApproved
            ? "Your admin account has been approved!"
            : "Your admin request was rejected.";

        await _notification.SendNotificationAsync(user.Id, "Admin Approval Status", message);
    }
}
