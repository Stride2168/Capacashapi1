using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Capacash.Application.Common.Interfaces;
using Capacash.Web.Models;
using System.Security.Claims;



namespace Capacash.Web.Controllers
{
    [Authorize(Roles = "Kiosk")]
    [ApiController]
    [Route("api/kiosks")]
    public class KioskController : ControllerBase
    {
    
private readonly IMediator _mediator;
        public KioskController( IMediator mediator)
        {
            
          _mediator = mediator;
        }

[HttpPost("generate-qr")]
public async Task<IActionResult> GenerateQrCode([FromBody] decimal amount)
{
    try
    {
        // The command handler returns a base64 string
        var base64String = await _mediator.Send(new GenerateQrCommand(amount));

        // Decode base64 into bytes
        var imageBytes = Convert.FromBase64String(base64String);

        // Return image as PNG file
        return File(imageBytes, "image/png");
    }
    catch (Exception ex)
    {
        return BadRequest(new { Error = ex.Message });
    }
}



    }
}
