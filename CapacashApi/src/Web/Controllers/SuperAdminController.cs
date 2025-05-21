using Capacash.Application.SuperAdmin.Commands;
using Capacash.Application.SuperAdmin.Queries;
using Capacash.Application.Common.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CapacashApi.Application.Common.Security;

namespace Capacash.Web.Controllers{

[ApiController]
[Route("api/superadmin")]
[Authorize(Roles = "superadmin")]

public class SuperAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public SuperAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/superadmin/pending-admins
    [HttpGet("pending-admins")]
    public async Task<IActionResult> GetPendingAdmins()
    {
        var result = await _mediator.Send(new GetPendingAdminsQuery());
        return Ok(result);
    }

    // POST: api/superadmin/approve-admin
    [HttpPost("approve-admin")]
    public async Task<IActionResult> ApproveAdmin([FromBody] ApproveAdminDto dto)
    {
        try
        {
            await _mediator.Send(new ApproveAdminRequestCommand(dto.UserId, dto.IsApproved));
            return Ok(new { Message = "Admin approval updated." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}}
