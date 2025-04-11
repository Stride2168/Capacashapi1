using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capacash.Application.Common.Interfaces;
using Capacash.Web.Models;
using System.Security.Claims;
using Capacash.Application.Transactions.Commands.ProcessTransaction;


namespace Capacash.Web.Controllers
{
    [Authorize(Roles = "Kiosk")]
    [ApiController]
    [Route("api/kiosks")]
    public class KioskController : ControllerBase
    {
    
private readonly IMediator _mediator;
        // Inject the necessary services
        public KioskController( IMediator mediator)
        {
            
          _mediator = mediator;
        }

       [HttpPost("process-transaction")]
public async Task<IActionResult> ProcessTransaction([FromBody] TransactionDto dto)
{
    try
    {
        var kioskId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(kioskId))
            return Unauthorized(new { Error = "No Kiosk ID found." });

        var result = await _mediator.Send(new ProcessTransactionCommand(dto.UserId, dto.Amount, kioskId));
        return Ok(new { Message = result });
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(new { Error = ex.Message });
    }
    catch (Exception ex)
    {
        return BadRequest(new { Error = ex.Message });
    }
}


    }
}
