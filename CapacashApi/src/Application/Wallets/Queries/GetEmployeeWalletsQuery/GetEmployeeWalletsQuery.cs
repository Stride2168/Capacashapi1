using Capacash.Application.Commons.DTOs;
namespace Capacash.Application.Wallets.Queries{

public record GetEmployeeWalletsQuery(string CompanyId) : IRequest<List<EmployeeWalletDto>>
{}

}

