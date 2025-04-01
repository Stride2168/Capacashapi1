using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Capacash.Application.Common.Interfaces;
using Capacash.Domain.Entities;
using Capacash.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Capacash.Infrastructure.Repositories
{
    public class KioskRepository : IKioskRepository
    {
        private readonly AppDbContext _context;

        public KioskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Kiosk> CreateKioskAsync(string kioskId, string passwordHash, string? name, string? location, string companyId)
        {
            var kiosk = new Kiosk(kioskId, passwordHash, name, location, companyId);
            _context.Kiosks.Add(kiosk);
            await _context.SaveChangesAsync();
            return kiosk;
        }

        public async Task<Kiosk?> GetKioskByIdAsync(Guid id)
        {
            return await _context.Kiosks.FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<Kiosk?> GetKioskByKioskIdAsync(string kioskId)
        {
            return await _context.Kiosks.FirstOrDefaultAsync(k => k.KioskId == kioskId);
        }

        // Add filtering by CompanyId when retrieving kiosks
        public async Task<List<Kiosk>> GetAllKiosksAsync(string companyId)
        {
            return await _context.Kiosks.Where(k => k.CompanyId == companyId).ToListAsync();
        }

        public async Task<bool> DeleteKioskAsync(Guid id)
        {
            var kiosk = await _context.Kiosks.FindAsync(id);
            if (kiosk == null) return false;

            _context.Kiosks.Remove(kiosk);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
