using Capacash.Application.Common.Interfaces;
using Capacash.Domain.Entities;
using Capacash.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Capacash.Infrastructure.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet> CreateWalletAsync(Guid userId)
        {
            var wallet = new Wallet(userId);
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<Wallet?> GetWalletByUserIdAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid))
        {
            return null; // Invalid GUID format
        }

        return await GetWalletByUserIdAsync(userGuid); // ✅ Reuse the existing method
    }
 // ✅ Add back the missing method
    public async Task<Wallet?> GetWalletByUserIdAsync(Guid userId)
    {
        return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
    }
    }
}
