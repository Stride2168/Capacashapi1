using Capacash.Application.Common.Interfaces;
using Capacash.Domain.Entities;
using Capacash.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Capacash.Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all wallets from the database
        public async Task<List<Wallet>> GetAllWalletsAsync()
        {
            return await _context.Wallets.ToListAsync();
        }

        // Create a new wallet for a user
        public async Task<Wallet> CreateWalletAsync(Guid userId)
        {
            var wallet = new Wallet(userId);
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        // Get wallet by userId (overloaded for string and Guid)
        public async Task<Wallet?> GetWalletByUserIdAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var userGuid))
            {
                return null; // Invalid GUID format
            }

            return await GetWalletByUserIdAsync(userGuid); // Use the existing method for Guid type
        }

        // Get wallet by userId
        public async Task<Wallet?> GetWalletByUserIdAsync(Guid userId)
        {
            return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        }

        // Update the wallet's information (balance and updated timestamp)
        public async Task UpdateWalletAsync(Wallet wallet)
        {
            if (wallet == null) return; // Guard clause to ensure wallet isn't null

            var existingWallet = await _context.Wallets.FindAsync(wallet.Id);
            if (existingWallet != null)
            {
                existingWallet.Balance = wallet.Balance;
                existingWallet.UpdatedAt = wallet.UpdatedAt; // Update timestamp or other fields as necessary
                await _context.SaveChangesAsync();
            }
        }
   public async Task<List<Wallet>> GetWalletsByCompanyIdAsync(string companyId)
{
    return await _context.Wallets
        .Join(_context.Users, 
            wallet => wallet.UserId, 
            user => user.Id, 
            (wallet, user) => new { wallet, user })
        .Where(wu => wu.user.CompanyId == companyId)
        .Select(wu => wu.wallet)
        .ToListAsync();
}

    }
}
