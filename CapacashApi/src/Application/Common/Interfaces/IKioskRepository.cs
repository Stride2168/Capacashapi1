using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Capacash.Domain.Entities;

namespace Capacash.Application.Common.Interfaces
{
    public interface IKioskRepository
    {
        Task<Kiosk> CreateKioskAsync(string kioskId, string passwordHash, string? name, string? location, string companyId);
        Task<Kiosk?> GetKioskByIdAsync(Guid id);
        Task<Kiosk?> GetKioskByKioskIdAsync(string kioskId);
        
        // Add companyId parameter to the method
        Task<List<Kiosk>> GetAllKiosksAsync(string companyId);
        
        Task<bool> DeleteKioskAsync(Guid id);
    }
}


