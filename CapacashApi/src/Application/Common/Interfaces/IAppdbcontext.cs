
// Application/Common/Interfaces/IAppDbContext.cs
using Capacash.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Capacash.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
DbSet<WalletRegenerationSetting> WalletRegenerationSettings { get; }
        DbSet<User> Users { get; set; }
        DbSet<Employee> Employees { get; set; }
        DbSet<Kiosk> Kiosks { get; set; }
        DbSet<Wallet> Wallets { get; set; }
        DbSet<Capacash.Domain.Entities.Transaction> Transactions { get; set; }
        DbSet<CreditRegeneration> CreditRegenerations { get; set; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}