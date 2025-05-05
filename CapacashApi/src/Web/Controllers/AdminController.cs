using Capacash.Application.Kiosks.Queries;
using Capacash.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Capacash.Application.Transactions.Queries;
using Capacash.Application.Wallets.Commands.CreditWallet.CreditWalletCommand;
using Capacash.Application.Kiosks.Commands.CreateKiosk;
using Capacash.Application.Users.Commands.ApproveUsers.ApproveUserCommand;
using Capacash.Application.Users.Queries.GetUnapprovedUsers;
using Capacash.Application.Wallets.Queries;
using Capacash.Application.Commons.DTOs;
namespace Capacash.WebAPI.Controllers
{
   [Route("api/admin")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("kiosk/create")]
    public async Task<IActionResult> CreateKiosk([FromBody] CreateKioskRequest request)
    {
        var companyId = User.FindFirst("CompanyId")?.Value;
        if (string.IsNullOrEmpty(companyId))
            return Unauthorized(new { Error = "Company ID is missing in the token." });

        var command = new CreateKioskCommand(
            request.KioskId,
            request.Password,
            request.Name,
            request.Location,
            companyId
        );

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("kiosk/all")]
    public async Task<IActionResult> GetAllKiosks()
    {
        var companyId = User.FindFirst("CompanyId")?.Value;
        if (string.IsNullOrEmpty(companyId))
            return Unauthorized(new { Error = "Company ID is missing in the token." });

        var query = new GetAllKiosksQuery(companyId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("credit")]
public async Task<IActionResult> CreditEmployeeWallet([FromBody] CreditRequest request)
{
    var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var companyId = User.FindFirst("CompanyId")?.Value;

    if (string.IsNullOrEmpty(adminId) || string.IsNullOrEmpty(companyId))
        return Unauthorized(new { Error = "Invalid admin session." });

    if (!Guid.TryParse(adminId, out var adminGuid))
    {
        return Unauthorized(new { Error = "Invalid admin ID." });
    }

    var command = new CreditWalletCommand(
        request.UserId,
        request.Amount,
        adminGuid,
        companyId
    );

    await _mediator.Send(command);
    return Ok("Credit added successfully.");
}


  

[HttpGet("transactions")]
public async Task<IActionResult> GetAllTransactions(
    [FromQuery] Guid? userId,
    [FromQuery] string? transactionType,
    [FromQuery] string? dateRange,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate)
{
    var companyId = User.FindFirst("CompanyId")?.Value;
    if (string.IsNullOrWhiteSpace(companyId))
        return Unauthorized(new { Error = "Company ID is missing in the token." });

    // Process date range filters
    var dateFilter = ProcessDateFilter(dateRange, ref startDate, ref endDate);

    var query = new GetTransactionsQuery(
    companyId,
    userId,
    transactionType,
    startDate ?? dateFilter.StartDate,
    endDate ?? dateFilter.EndDate
);


    var result = await _mediator.Send(query);
    return Ok(result);
}

private (DateTime? StartDate, DateTime? EndDate) ProcessDateFilter(string? dateRange, ref DateTime? startDate, ref DateTime? endDate)
{
    if (!string.IsNullOrEmpty(dateRange))
    {
        var now = DateTime.UtcNow;
        switch (dateRange.ToLower())
        {
            case "today":
                startDate = now.Date;
                break;
            case "yesterday":
                startDate = now.Date.AddDays(-1);
                endDate = now.Date.AddMilliseconds(-1);
                break;
            case "lastweek":
                startDate = now.Date.AddDays(-7);
                break;
            case "last30days":
                startDate = now.Date.AddDays(-30);
                break;
            case "thismonth":
                startDate = new DateTime(now.Year, now.Month, 1);
                break;
            case "lastmonth":
                var firstDayOfLastMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
                startDate = firstDayOfLastMonth;
                endDate = firstDayOfLastMonth.AddMonths(1).AddMilliseconds(-1);
                break;
        }
    }

    return (startDate, endDate);
}

    
[HttpPost("approve-user/{userId}")]
public async Task<IActionResult> ApproveUser(Guid userId)
{
    var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var companyId = User.FindFirst("CompanyId")?.Value;
    
    if (string.IsNullOrEmpty(adminId) || string.IsNullOrEmpty(companyId))
    {
        return Unauthorized(new { Error = "Invalid admin session." });
    }

    try
    {
        await _mediator.Send(new ApproveUserCommand(
            userId,
            Guid.Parse(adminId),
            companyId
        ));
        return Ok(new { Message = "User approved successfully." });
    }
    catch (NotFoundException)
    {
        return NotFound(new { Error = "User not found." });
    }
    catch (UnauthorizedAccessException)
    {
        return Unauthorized(new { Error = "Cannot approve user from another company." });
    }
    catch (InvalidOperationException ex)  // Only keep where the message might vary
    {
        return BadRequest(new { Error = ex.Message });
    }
    catch
    {
        return StatusCode(500, new { Error = "An error occurred while approving the user." });
    }
}
    [HttpPost("bulk-approve-users")]
    public async Task<IActionResult> BulkApproveUsers()
    {
        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var companyId = User.FindFirst("CompanyId")?.Value;
        
        if (string.IsNullOrEmpty(adminId) || string.IsNullOrEmpty(companyId))
        {
            return Unauthorized(new { Error = "Invalid admin session." });
        }

        try
        {
            var count = await _mediator.Send(new BulkApproveUsersCommand(
                Guid.Parse(adminId),
                companyId
            ));
            
            return Ok(new { 
                Message = count > 0 
                    ? $"{count} users approved successfully." 
                    : "No unapproved users found.",
                ApprovedCount = count
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                Error = "An error occurred during bulk approval.",
                Details = ex.Message
            });
        }
    }
[HttpGet("users/unapproved")]
public async Task<IActionResult> GetUnapprovedUsers()
{
    var companyId = User.FindFirst("CompanyId")?.Value;
    if (string.IsNullOrEmpty(companyId))
        return Unauthorized(new { Error = "Company ID is missing in the token." });

    var result = await _mediator.Send(new GetUnapprovedUsersQuery(companyId));
    return Ok(result);
}
[HttpGet("employees/wallets")]
public async Task<IActionResult> GetEmployeeWallets()
{
    var companyId = User.FindFirst("CompanyId")?.Value;
    if (string.IsNullOrEmpty(companyId))
    {
        return Unauthorized(new { Error = "Company ID is missing in the token." });
    }

    try
    {
        var result = await _mediator.Send(new GetEmployeeWalletsQuery(companyId));
        return Ok(result);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { 
            Error = "Error retrieving employee wallets",
            Details = ex.Message
        });
    }
}
[HttpGet("transactions/export/excel")]
public async Task<IActionResult> ExportTransactionsToExcel([FromQuery] Guid? userId)
{
    var companyId = User.FindFirst("CompanyId")?.Value;
    if (string.IsNullOrEmpty(companyId))
        return Unauthorized(new { Error = "Company ID is missing in the token." });

    var query = new GetTransactionsQuery(companyId, userId);
    var transactions = await _mediator.Send(query);

    using var workbook = new ClosedXML.Excel.XLWorkbook();
    var worksheet = workbook.Worksheets.Add("Transactions");

    // Header
    worksheet.Cell(1, 1).Value = "Transaction ID";
    worksheet.Cell(1, 2).Value = "User ID";
    worksheet.Cell(1, 3).Value = "Amount";
    worksheet.Cell(1, 4).Value = "Transaction Type";
    worksheet.Cell(1, 5).Value = "Date";

    var row = 2;
    foreach (var tx in transactions)
    {
        worksheet.Cell(row, 1).Value = tx.TransactionId;
        worksheet.Cell(row, 2).Value = tx.UserId.ToString();
        worksheet.Cell(row, 3).Value = tx.Amount;
        worksheet.Cell(row, 4).Value = tx.TransactionType ?? "Unknown";  // Handle null values in case TransactionType is empty
        worksheet.Cell(row, 5).Value = tx.TransactionDate;
        worksheet.Cell(row, 5).Style.NumberFormat.Format = "yyyy-MM-dd HH:mm:ss";
        row++;
    }

    using var stream = new MemoryStream();
    workbook.SaveAs(stream);
    stream.Position = 0;

    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "transactions.xlsx");
}




}
}
