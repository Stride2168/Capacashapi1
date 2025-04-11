using Capacash.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Capacash.Infrastructure.Data
{
    public interface IAppDbContext
    {
        // DbSets for your entities
        DbSet<User> Users { get; set; }
        DbSet<Employee> Employees { get; set; }
        DbSet<Kiosk> Kiosks { get; set; }
        // Any other DbSets for your entities
    }
}
