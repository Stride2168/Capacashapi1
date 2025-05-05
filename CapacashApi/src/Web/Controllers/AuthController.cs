using Capacash.Application.Auth.Commands;
using Capacash.Application.Kiosks.Commands;
using Microsoft.AspNetCore.Mvc;
using Capacash.Application.Commons.DTOs;
namespace Capacash.Web.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register-employee")]
        public async Task<IActionResult> RegisterEmployee([FromBody] RegisterEmployeeDto request)
        {
            try
            {
                var command = new RegisterEmployeeCommand(
                    request.FullName,
                    request.Email,
                    request.Password,
                    request.CompanyId,
                    request.PhoneNumber);

                var token = await _mediator.Send(command);
                return Ok(new { Token = token });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.KioskId))
                {
                    // Handle kiosk login
                    var kioskCommand = new KioskLoginCommand(request.KioskId, request.Password);
                    var kioskResult = await _mediator.Send(kioskCommand);
                    return Ok(new { Token = kioskResult.Token });
                }
                else if (!string.IsNullOrEmpty(request.Email))
                {
                    // Handle user login
                    var userCommand = new LoginCommand(request.Email, request.Password);
                    var userToken = await _mediator.Send(userCommand);
                    return Ok(new { Token = userToken });
                }
                else
                {
                    return BadRequest(new { Error = "Either Email or KioskId must be provided" });
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDto dto)
        {
            try
            {
                var command = new RegisterAdminCommand(
                    dto.FullName,
                    dto.Email,
                    dto.Password,
                    dto.CompanyId);

                var token = await _mediator.Send(command);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }



 
}