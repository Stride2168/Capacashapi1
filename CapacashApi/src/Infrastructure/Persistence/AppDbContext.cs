using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Capacash.Domain.Entities;
using System;
using System.IO;
using Capacash.Infrastructure.Persistence.Configuration;
using Capacash.Infrastructure.Persistence.Configurations;

namespace Capacash.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        
        public DbSet<User> Users { get; set; }
        public DbSet<Kiosk> Kiosks { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
            public DbSet<Wallet> Wallets { get; set; } 
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public AppDbContext() { }

protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (!optionsBuilder.IsConfigured)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Web")) // 👈 Point to Web project's config
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .Build();

        string? connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string is missing.");
        }

        optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly("CapacashApi.Infrastructure")); // 👈 Set correct migrations assembly
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
    }
}
