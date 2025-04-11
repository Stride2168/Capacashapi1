using Capacash.Application.Common.Interfaces;
using Capacash.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Capacash.Infrastructure.Services
{
    public class AdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IWalletService _walletService;

        public AdminService(IUserRepository userRepository, IWalletService walletService) // ✅ Inject both dependencies
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
        }

        public async Task<bool> ApproveEmployeeByIdAsync(Guid id, Guid adminId)
        {
            // 🔍 Get Admin making the request
            var admin = await _userRepository.GetByIdAsync(adminId);
            if (admin == null || admin.Role != "Admin")
                return false; // ❌ Invalid Admin

            // 🔍 Get Employee (based on 'Id' column)
            var employee = await _userRepository.GetByIdAsync(id);
            if (employee == null || employee.IsApproved) 
                return false; // ❌ Employee not found or already approved

            // 🚨 Ensure Employee belongs to the same Company as Admin
            if (employee.CompanyId != admin.CompanyId)
                return false; // ❌ Unauthorized approval attempt

            // ✅ Approve Employee
            employee.IsApproved = true;
            await _userRepository.UpdateAsync(employee);

            // 🔥 Auto-create wallet upon approval
            await _walletService.CreateWalletForUserAsync(employee.Id);

            return true; // 🎉 Success!
        }

        public async Task<List<User>> GetUnapprovedEmployeesByCompanyAsync(string companyId)
        {
            return await _userRepository.GetUnapprovedEmployeesByCompanyAsync(companyId);
        }
    }
}
