using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capacash.Application.Common.Interfaces;
using Capacash.Web.Models;
using System.Security.Claims;
using Capacash.Application.Transaction.Commands;
using Capacash.Application.Transaction.Queries;


namespace Capacash.Web.Controllers
{
    [Authorize(Roles = "Kiosk")]
    [ApiController]
    [Route("api/kiosks")]
    public class KioskController : ControllerBase
    {

        private readonly IMediator _mediator;
        public KioskController(IMediator mediator)
        {

            _mediator = mediator;
        }

        [HttpPost("generate-qr")]
        public async Task<IActionResult> GenerateQr([FromBody] GenerateQrCommand command)
        {
            Console.WriteLine($"[Controller] Received Amount: {command.Amount}");
            var qr = await _mediator.Send(command);
            return Ok(qr);
        }
        [HttpPost("generate-qr-image")]
        public async Task<IActionResult> GenerateQrImage([FromBody] GenerateQrCommand command)
        {
            Console.WriteLine($"[Controller] Received Amount: {command.Amount}");
            var base64Qr = await _mediator.Send(command);

            var imageBytes = Convert.FromBase64String(base64Qr);

            return File(imageBytes, "image/png");
        }


[HttpGet("transactions/today-total")]
        public async Task<IActionResult> GetTodayTransactionTotal(
    [FromQuery] string? companyId,
    [FromQuery] Guid? kioskId,
    [FromQuery] string? transactionType)
        {
            var total = await _mediator.Send(new GetTodaysTotalQuery
            {
                CompanyId = companyId,
                KioskId = kioskId,
                TransactionType = transactionType
            });

            return Ok(new { total });
        }
[HttpGet("transactions/me")]
public async Task<IActionResult> GetTransactionsByKiosk([FromQuery] string? transactionType)
{
    // Retrieve nameid (KioskId) from JWT claims
    var nameId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(nameId))
    {
        return Unauthorized("Kiosk ID (nameid) is missing in the JWT.");
    }

    // Log the extracted nameid for debugging purposes
    Console.WriteLine($"Extracted nameid: {nameId}");

    // Retrieve the transactions related to the given kiosk (nameid)
    var transactions = await _mediator.Send(new GetTransactionsByKioskQuery
    {
        KioskCode = nameId  // Use nameid as KioskId for filtering
    });

    // Optionally, filter by transaction type if provided
    if (!string.IsNullOrEmpty(transactionType))
    {
        transactions = transactions.Where(t => t.TransactionType == transactionType).ToList();
    }

    // If no transactions found, return a meaningful response
    if (!transactions.Any())
    {
        return NotFound("No transactions found for this kiosk.");
    }

    return Ok(new { transactions });
}



    } }

