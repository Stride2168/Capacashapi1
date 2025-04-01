using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capacash.Application.Common.Interfaces;
using Capacash.Domain.Entities;
using Capacash.Web.Models;
using BCrypt.Net;

namespace Capacash.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/kiosks")]
    public class KioskController : ControllerBase
    {
        private readonly IKioskRepository _kioskRepository;

        public KioskController(IKioskRepository kioskRepository)
        {
            _kioskRepository = kioskRepository;
        }

        // ✅ Admin creates a kiosk with a unique ID and password
    [HttpPost("create")]
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


        // ✅ Admin can view all kiosks
           // Get all kiosks for the current Admin's company
        [HttpGet("all")]
        public async Task<IActionResult> GetAllKiosks()
        {
            // Extract the CompanyId from the JWT token
            var companyId = User.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyId))
                return Unauthorized(new { Error = "Company ID is missing in the token." });

            var kiosks = await _kioskRepository.GetAllKiosksAsync(companyId);
            return Ok(kiosks);
        }


        // ✅ Admin can get a kiosk by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetKiosk(Guid id)
        {
            var kiosk = await _kioskRepository.GetKioskByIdAsync(id);
            if (kiosk == null) return NotFound(new { Error = "Kiosk not found." });
            return Ok(kiosk);
        }

        // ✅ Admin can delete a kiosk
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteKiosk(Guid id)
        {
            var success = await _kioskRepository.DeleteKioskAsync(id);
            if (!success) return NotFound(new { Error = "Kiosk not found." });
            return Ok(new { Message = "Kiosk deleted successfully." });
        }
    }
}
