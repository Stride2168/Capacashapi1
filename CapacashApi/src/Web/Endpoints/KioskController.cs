using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capacash.Application.Common.Interfaces;
using Capacash.Application.Services;
using Capacash.Web.Models;
using System.Security.Claims;

namespace Capacash.Web.Controllers
{
    [Authorize(Roles = "Kiosk")]
    [ApiController]
    [Route("api/kiosks")]
    public class KioskController : ControllerBase
    {
        private readonly IKioskRepository _kioskRepository;
        private readonly ITransactionService _transactionService;

        // Inject the necessary services
        public KioskController(IKioskRepository kioskRepository, ITransactionService transactionService)
        {
            _kioskRepository = kioskRepository;
            _transactionService = transactionService;
        }

       [HttpPost("process-transaction")]
public async Task<IActionResult> ProcessTransaction([FromBody] TransactionDto dto)
{
    try
    {
        // Debugging: Check received UserId and Amount
        Console.WriteLine($"Received UserId: {dto.UserId}");
        Console.WriteLine($"Received Amount: {dto.Amount}");

        // Validate UserId
        if (dto.UserId == Guid.Empty)
            return BadRequest(new { Error = "UserId cannot be empty or invalid." });

        // Ensure the request is from an authenticated Kiosk Operator
        var kioskIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(kioskIdClaim))
            return Unauthorized(new { Error = "Unauthorized: No Kiosk ID found." });

        // Debugging: Print KioskId Claim
        Console.WriteLine($"Received Kiosk ID from claim: {kioskIdClaim}");

        // Validate Kiosk
        var kiosk = await _kioskRepository.GetKioskByKioskIdAsync(kioskIdClaim);
        if (kiosk == null)
            return Unauthorized(new { Error = "Unauthorized: Invalid kiosk." });

        // Process transaction via the Transaction Service
        await _transactionService.ProcessTransactionAsync(dto.UserId, dto.Amount);

        return Ok(new { Message = "Transaction successful." });
    }
    catch (Exception ex)
    {
        return BadRequest(new { Error = ex.Message });
    }
}


    }
}
