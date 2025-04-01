using Capacash.Application.Services;
using Capacash.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

[HttpPost("approve-employee/{id}")] //  Using 'id'
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


[HttpGet("unapproved-employees")]
public async Task<IActionResult> GetUnapprovedEmployees()
{
    // Get the logged-in Admin's Company ID from JWT
    var adminCompanyId = User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;

    if (string.IsNullOrEmpty(adminCompanyId))
        return Unauthorized(new { Message = "Invalid admin session." });

    var employees = await _adminService.GetUnapprovedEmployeesByCompanyAsync(adminCompanyId);

    if (employees.Count == 0)
        return NotFound(new { Message = "No unapproved employees found for your company." });

    return Ok(employees);
}

    }
}
