using Capacash.Application.Common.Interfaces;
using Capacash.Application.Services;
using Capacash.Domain.Entities;
using Capacash.Infrastructure.Persistence;
using Capacash.Infrastructure.Persistence.Repositories;
using Capacash.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Capacash.WebAPI.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Only Admins can access
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;
        private readonly IWalletService _walletService; // Added IWalletService
        private readonly IKioskRepository _kioskRepository;  // Added IKioskRepository for Kiosk actions
        private readonly ITransactionService _transactionService;  // Added ITransactionService for handling transactions
private readonly IUserRepository _userRepository;  // Add IUserRepository to manage users (if needed)
    private readonly ITransactionRepository _transactionRepository;  // Added this also
        // Inject IWalletService and other dependencies into the constructor
        public AdminController(AdminService adminService, IWalletService walletService, 
        IKioskRepository kioskRepository, ITransactionService transactionService, IUserRepository userRepository,  
        ITransactionRepository transactionRepository )
        {
            _adminService = adminService;
            _walletService = walletService;
            _kioskRepository = kioskRepository;  
            _transactionService = transactionService; 
            _userRepository = userRepository;
            _transactionRepository = transactionRepository; 
        }

        // Admin creates a kiosk with a unique ID and password
        [HttpPost("kiosk/create")]
        public async Task<IActionResult> CreateKiosk([FromBody] CreateKioskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.KioskId) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { Error = "Kiosk ID and Password are required." });

            // Hash the password for security
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Extract the CompanyId from the JWT token (admin's companyId)
            var companyId = User.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyId))
                return Unauthorized(new { Error = "Company ID is missing in the token." });

            // Pass the companyId when calling the repository method
            var kiosk = await _kioskRepository.CreateKioskAsync(request.KioskId, passwordHash, request.Name, request.Location, companyId);
            return Ok(kiosk);
        }

        // Admin can view all kiosks
        [HttpGet("kiosk/all")]
        public async Task<IActionResult> GetAllKiosks()
        {
            var companyId = User.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyId))
                return Unauthorized(new { Error = "Company ID is missing in the token." });

            var kiosks = await _kioskRepository.GetAllKiosksAsync(companyId);
            return Ok(kiosks);
        }

        // Admin can get a kiosk by ID
        [HttpGet("kiosk/{id}")]
        public async Task<IActionResult> GetKiosk(Guid id)
        {
            var kiosk = await _kioskRepository.GetKioskByIdAsync(id);
            if (kiosk == null) return NotFound(new { Error = "Kiosk not found." });
            return Ok(kiosk);
        }

        // Admin can delete a kiosk
        [HttpDelete("kiosk/delete/{id}")]
        public async Task<IActionResult> DeleteKiosk(Guid id)
        {
            var success = await _kioskRepository.DeleteKioskAsync(id);
            if (!success) return NotFound(new { Error = "Kiosk not found." });
            return Ok(new { Message = "Kiosk deleted successfully." });
        }

        // Admin approves an employee by ID
        [HttpPost("approve-employee/{id}")]
        public async Task<IActionResult> ApproveEmployee(Guid id)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminId))
                return Unauthorized(new { Message = "Unauthorized access." });

            var success = await _adminService.ApproveEmployeeByIdAsync(id, Guid.Parse(adminId));

            if (!success)
                return NotFound(new { Message = "Employee not found or unauthorized approval attempt." });

            return Ok(new { Message = "Employee approved successfully." });
        }

        // Admin can view all unapproved employees
        [HttpGet("unapproved-employees")]
        public async Task<IActionResult> GetUnapprovedEmployees()
        {
            var adminCompanyId = User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;

            if (string.IsNullOrEmpty(adminCompanyId))
                return Unauthorized(new { Message = "Invalid admin session." });

            var employees = await _adminService.GetUnapprovedEmployeesByCompanyAsync(adminCompanyId);

            if (employees.Count == 0)
                return NotFound(new { Message = "No unapproved employees found for your company." });

            return Ok(employees);
        }

        // Admin adds credit to an employee's wallet
       [HttpPost("credit")]
public async Task<IActionResult> CreditEmployeeWallet([FromBody] CreditRequest request)
{
    try
    {
        if (request.Amount <= 0)
            return BadRequest("Amount must be greater than zero.");

        // Get the admin's userId from the current session or token
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(adminId))
            return Unauthorized(new { Error = "Invalid admin session." });

        // Retrieve admin information (assuming _userRepository is available)
        var admin = await _userRepository.GetUserByIdAsync(Guid.Parse(adminId));
        if (admin == null || string.IsNullOrEmpty(admin.CompanyId))
            return Unauthorized(new { Error = "Unauthorized: Admin company not found." });

        // Retrieve the employee's information
        var employee = await _userRepository.GetUserByIdAsync(request.UserId);
        if (employee == null)
            return NotFound(new { Error = "Employee not found." });

        // Check if the employee's companyId matches the admin's companyId
        if (employee.CompanyId != admin.CompanyId)
            return Unauthorized(new { Error = "You are not authorized to credit this employee's wallet." });

        // Call the service to add credit
        await _walletService.AddCreditToWalletAsync(request.UserId, request.Amount);

        return Ok("Credit added successfully.");
    }
    catch (Exception ex)
    {
        return BadRequest(ex.Message);
    }
}


 // Admin can view all employee wallets belonging to the same company
[HttpGet("wallets")]
public async Task<IActionResult> GetAllWallets()
{
    try
    {
        // Get the Admin's ID from the JWT token
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(adminId))
            return Unauthorized(new { Error = "Invalid admin session." });

        // Retrieve the admin's details and get their CompanyId
        var admin = await _userRepository.GetUserByIdAsync(Guid.Parse(adminId));
        if (admin == null || string.IsNullOrEmpty(admin.CompanyId))
            return Unauthorized(new { Error = "Unauthorized: Admin company not found." });

        // Fetch all wallets that belong to the admin's company
        var wallets = await _walletService.GetWalletsByCompanyIdAsync(admin.CompanyId);
        return Ok(wallets);
    }
    catch (Exception ex)
    {
        // Return a bad request if there's an error
        return BadRequest(ex.Message);
    }
}

      [HttpGet("wallet/{userId}")]
public async Task<IActionResult> GetEmployeeWallet(Guid userId)
{
    try
    {
        // Get the admin's userId from the current session or token
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(adminId))
            return Unauthorized(new { Error = "Invalid admin session." });

        // Retrieve admin information (assuming _userRepository is available)
        var admin = await _userRepository.GetUserByIdAsync(Guid.Parse(adminId));
        if (admin == null || string.IsNullOrEmpty(admin.CompanyId))
            return Unauthorized(new { Error = "Unauthorized: Admin company not found." });

        // Retrieve the employee's wallet
        var wallet = await _walletService.GetWalletByUserIdAsync(userId);

        if (wallet == null)
            return NotFound(new { Error = "Wallet not found for the specified user." });

        // Check if the employee's companyId matches the admin's companyId
        var employee = await _userRepository.GetUserByIdAsync(userId);
        if (employee == null || employee.CompanyId != admin.CompanyId)
            return Unauthorized(new { Error = "You are not authorized to view this employee's wallet." });

        return Ok(wallet);
    }
    catch (Exception ex)
    {
        return BadRequest(ex.Message);
    }
}

[HttpGet("transactions")]
public async Task<IActionResult> GetAllTransactions([FromQuery] Guid? userId, [FromQuery] string? filter)
{
    var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(adminId))
        return Unauthorized(new { Error = "Invalid admin session." });

    var admin = await _userRepository.GetUserByIdAsync(Guid.Parse(adminId));
    if (admin == null || string.IsNullOrEmpty(admin.CompanyId))
        return Unauthorized(new { Error = "Unauthorized: Admin company not found." });

    DateTime? startDate = null;
    DateTime? endDate = DateTime.UtcNow;

    switch (filter)
    {
        case "today":
            startDate = DateTime.UtcNow.Date;
            break;
        case "lastWeek":
            startDate = DateTime.UtcNow.AddDays(-7);
            break;
        case "last30Days":
            startDate = DateTime.UtcNow.AddDays(-30);
            break;
    }

    var transactions = await _transactionRepository.GetTransactions1ByCompanyIdAsync(admin.CompanyId, startDate, endDate);
    return Ok(transactions);
}


    }
}