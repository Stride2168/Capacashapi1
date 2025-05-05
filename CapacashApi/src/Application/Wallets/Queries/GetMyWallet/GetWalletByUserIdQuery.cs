using  Capacash.Application.Commons.DTOs;
namespace Capacash.Application.Wallets.Queries{

public record GetWalletByUserIdQuery(string UserId) : IRequest<WalletDto>;}

