// Application/Wallets/Queries/GetEmployeeWalletsQueryHandler.cs
using Capacash.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Capacash.Application.Commons.DTOs;
namespace Capacash.Application.Wallets.Queries;

public class GetEmployeeWalletsQueryHandler : IRequestHandler<GetEmployeeWalletsQuery, List<EmployeeWalletDto>>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetEmployeeWalletsQueryHandler> _logger;

    public GetEmployeeWalletsQueryHandler(
        IWalletRepository walletRepository,
        IUserRepository userRepository,
        ILogger<GetEmployeeWalletsQueryHandler> logger)
    {
        _walletRepository = walletRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

   public async Task<List<EmployeeWalletDto>> Handle(
    GetEmployeeWalletsQuery request,
    CancellationToken cancellationToken)
{
    // Get all employee wallets for the company
    var wallets = await _walletRepository.GetEmployeeWalletsByCompanyIdAsync(request.CompanyId);
    
    if (!wallets.Any())
    {
        _logger.LogInformation("No employee wallets found for company {CompanyId}", request.CompanyId);
        return new List<EmployeeWalletDto>();
    }

    // Get user details for these wallets
    var userIds = wallets.Select(w => w.UserId).Distinct().ToList();
    var employees = await _userRepository.GetUsersByIdsAsync(userIds);

    var result = new List<EmployeeWalletDto>();
    
    foreach (var wallet in wallets)
    {
        var employee = employees.FirstOrDefault(e => e.Id == wallet.UserId);
        if (employee != null)
        {
            result.Add(new EmployeeWalletDto(
                employee.Id,
                employee.FullName,
                employee.Email,
                wallet.Balance,
                wallet.UpdatedAt ?? wallet.CreatedAt));
        }
    }

    _logger.LogInformation("Retrieved {Count} employee wallets for company {CompanyId}", 
        result.Count, request.CompanyId);

    return result;
}
    }


