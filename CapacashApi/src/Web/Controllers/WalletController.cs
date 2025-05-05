using Capacash.Application.Common.Interfaces;
using Capacash.Application.Commons.DTOs;
using Capacash.Application.Wallets.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Capacash.Web.Models.Wallet;
using Capacash.Application.Wallets.Commands;
using Capacash.Application.Transactions.Queries.GetUserTransactions;
namespace Capacash.Web.Controllers
{
    [ApiController]
    [Route("api/wallet")]
    [Authorize] 
    public class WalletController : ControllerBase
    {
        private readonly IMediator _mediator;
private readonly IQrCodeService _qrCodeService;
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepo;
public WalletController(IMediator mediator, IQrCodeService qrCodeService, INotificationService notificationService, IUserRepository userRepo)
{
    _mediator = mediator;
    _qrCodeService = qrCodeService;
    _notificationService = notificationService;
      _userRepo = userRepo;
}

   [HttpGet("me")]
public async Task<IActionResult> GetMyWallet()
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (string.IsNullOrWhiteSpace(userId))
    {
        return Unauthorized(new { Error = "Invalid user session. User ID is missing." });
    }

    try
    {
        var result = await _mediator.Send(new GetWalletByUserIdQuery(userId)); // Pass string directly
        return Ok(result);
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(new { Error = ex.Message });
    }
    catch (NotFoundException ex)
    {
        return NotFound(new { Error = ex.Message });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { Error = ex.Message });
    }
}

[HttpGet("transactions")]
public async Task<IActionResult> GetMyTransactionHistory(
    [FromQuery] string? searchTerm,
    [FromQuery] string? type,
    [FromQuery] string? dateRange)
{
    var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
    
    if (!Guid.TryParse(userIdString, out var userId))
        return Unauthorized("Invalid user ID format.");

    try
    {
        var query = new GetUserTransactionsQuery(
            userId: userId,
            searchTerm: searchTerm,
            transactionType: type,
            dateRange: dateRange
        );

        var result = await _mediator.Send(query);
        return Ok(result);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { Error = ex.Message });
    }
}


 [HttpPost("scan-qr")]
[Authorize(Roles = "Employee")]
public async Task<IActionResult> ScanQrAndProcess([FromBody] ScanQrRequest request)
{
    try
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("Invalid token. User ID missing.");

        // Decrypt to strongly-typed payload
        var payload = _qrCodeService.DecryptPayload<KioskPurchasePayload>(request.QrData);
        
        // Validate QR isn't expired (5 minute window)
        if (payload.Timestamp < DateTime.UtcNow.AddMinutes(-5))
            return BadRequest(new { Error = "QR code expired" });

      var command = new ProcessTransactionCommand(
    Guid.Parse(userId),
    payload.KioskId,
    payload.Timestamp,
    payload.Amount,
    "Purchase"
);
        var result = await _mediator.Send(command);

        await _notificationService.SendNotificationAsync(
            userId,
            "Purchase Successful", 
            $"You paid ₱{payload.Amount:0.00} at Kiosk {payload.KioskId}"
        );

        return Ok(new { 
            Message = result,
            Amount = payload.Amount,
            KioskId = payload.KioskId
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new { Error = ex.Message });
    }
}




[HttpPost("transfer")]
[Authorize(Roles = "Employee")]
public async Task<IActionResult> TransferFunds([FromBody] TransferRequest request)
{
    var senderIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(senderIdStr, out var senderId))
        return Unauthorized("Invalid user session.");

    var command = new TransferFundsCommand(
        SenderId: senderId,
        RecipientId: request.RecipientId,
        Amount: request.Amount
    );

    try
    {
        var result = await _mediator.Send(command);
        return Ok(new { Message = result });
    }
    catch (Exception ex)
    {
        return BadRequest(new { Error = ex.Message });
    }
}
[HttpPost("transfer/preview")]
[Authorize(Roles = "Employee")]
public async Task<IActionResult> PreviewTransfer([FromBody] TransferRequest request)
{
    var senderIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(senderIdStr, out var senderId))
        return Unauthorized("Invalid user session.");

    var sender = await _userRepo.GetByIdAsync(senderId);
    if (sender == null)
        return Unauthorized("Sender not found.");

    var recipient = await _userRepo.GetByIdAsync(request.RecipientId);
    if (recipient == null)
        return NotFound(new { Error = "Recipient not found." });

    if (sender.CompanyId != recipient.CompanyId)
        return BadRequest(new { Error = "Transfers must be within the same company." });

    return Ok(new TransferPreviewResponse
    {
        RecipientId = recipient.Id,
        RecipientName = recipient.FullName,
        Amount = request.Amount
    });
}


    }
}