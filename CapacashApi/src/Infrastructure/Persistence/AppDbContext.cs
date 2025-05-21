using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Capacash.Domain.Entities;
using System;
using System.IO;
using Capacash.Infrastructure.Persistence.Configuration;
using Capacash.Infrastructure.Persistence.Configurations;

using Capacash.Application.Common.Interfaces;
using CapacashApi.Infrastructure.Identity;
namespace Capacash.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<WalletRegenerationSetting> WalletRegenerationSettings { get; set; } = null!;

        public DbSet<User> Users { get; set; }
        public DbSet<Kiosk> Kiosks { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
            public DbSet<Wallet> Wallets { get; set; } 
public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public AppDbContext() { }
  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Web")) 
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .Build();

        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string is missing.");
        }

        optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly("CapacashApi.Infrastructure")); 
    }
}
   protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply entity configurations
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new WalletConfiguration());
            modelBuilder.ApplyConfiguration(new KioskConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());

        }
        public DbSet<CreditRegeneration> CreditRegenerations { get; set; }

    }
}
