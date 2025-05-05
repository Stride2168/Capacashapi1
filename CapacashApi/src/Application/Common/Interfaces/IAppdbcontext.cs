using Capacash.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Capacash.Application.Common.Interfaces
{
    public interface IAppDbContext
    {
        // DbSets for your entities
        DbSet<User> Users { get; set; }
        DbSet<Employee> Employees { get; set; }
        DbSet<Kiosk> Kiosks { get; set; }
         DbSet<Wallet> Wallets { get; set; }
        DbSet<Transaction> Transactions { get; set; }
      Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
