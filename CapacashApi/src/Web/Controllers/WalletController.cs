using Capacash.Application.Common.Interfaces;
using Capacash.Application.Commons.DTOs;
using Capacash.Application.Wallets.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Capacash.Web.Models.Wallet;
using Capacash.Application.Wallets.Commands;
using Capacash.Application.Transactions.Queries.GetUserTransactions;
using Capacash.Application.Users.Commands.ResetPassword;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<WalletController> _logger;
        public WalletController(ILogger<WalletController> logger, IMediator mediator, IQrCodeService qrCodeService, INotificationService notificationService, IUserRepository userRepo)
        {
            _mediator = mediator;
            _qrCodeService = qrCodeService;
            _notificationService = notificationService;
            _userRepo = userRepo;
            _logger = logger;
        }
[HttpGet("ping")]
[AllowAnonymous]
public IActionResult Ping() => Ok("pong");
[Authorize] 
[HttpGet("me")]
[Authorize(Roles = "Employee,Admin")]
public async Task<IActionResult> GetMyWallet()
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    if (string.IsNullOrWhiteSpace(userId))
    {
        return Unauthorized(new { Error = "Invalid user session. User ID is missing." });
    }

    try
    {
        var result = await _mediator.Send(new GetWalletByUserIdQuery(userId));
        return Ok(result);  // The response now includes FullName due to WalletDto update
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
        _logger.LogError(ex, "An error occurred while fetching the wallet.");
        return StatusCode(500, new { Error = "An unexpected error occurred." });
    }
}



[HttpGet("transactions")]
[Authorize(Roles = "Employee,Admin")]
public async Task<IActionResult> GetMyTransactionHistory(
    [FromQuery] string? searchTerm,
    [FromQuery] string? type,
    [FromQuery] string? dateRange)
{
    var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
    _logger.LogInformation("Fetching transactions for UserID: {UserId}", userIdString);

    if (!Guid.TryParse(userIdString, out var userId))
    {
        _logger.LogWarning("Invalid user ID format: {UserId}", userIdString);
        return Unauthorized("Invalid user ID format.");
    }

    try
    {
        var query = new GetUserTransactionsQuery(
            userId: userId,
            searchTerm: searchTerm,
            transactionType: type,
            dateRange: dateRange
        );

        _logger.LogInformation("Sending GetUserTransactionsQuery with filters: searchTerm={Search}, type={Type}, dateRange={DateRange}", 
            searchTerm, type, dateRange);

        var result = await _mediator.Send(query);

        _logger.LogInformation("Successfully retrieved {Count} transactions for user {UserId}", 
            result.Count, userId);

        return Ok(result);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while retrieving transactions for UserID: {UserId}", userId);
        return StatusCode(500, new { Error = ex.Message });
    }
}   


 [HttpPost("scan-qr")]
[Authorize(Roles = "Employee,Admin")]
public async Task<IActionResult> ScanQrAndProcess([FromBody] ScanQrRequest request)
{
    try
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized("Invalid token. User ID missing.");

        var payload = _qrCodeService.DecryptPayload<KioskPurchasePayload>(request.QrData);
        
        // Validate amount before creating command
        if (payload.Amount <= 0)
            return BadRequest(new { Error = "Amount must be greater than zero" });

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
            Guid.Parse(userId),
            "Purchase Successful", 
            $"You paid ₱{payload.Amount:0.00} at Kiosk {payload.KioskId}"
        );

        return Ok(new { 
            Message = result,
            Amount = payload.Amount,
            KioskId = payload.KioskId
        });
    }
    catch (ArgumentException ex) when (ex.ParamName == "amount")
    {
        return BadRequest(new { Error = ex.Message });
    }
    catch (Exception ex)
    {
        return BadRequest(new { Error = ex.Message });
    }
}




[HttpPost("transfer")]
[Authorize(Roles = "Employee,Admin")]
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
[Authorize(Roles = "Employee,Admin")]
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
[HttpPost("reset-password")]
[Authorize(Roles = "Employee,Admin")]
public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
{
    var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (!Guid.TryParse(userIdStr, out var userId))
        return Unauthorized("Invalid user session.");

    try
    {
        var command = new ResetPasswordCommand(userId, request.NewPassword);
        var result = await _mediator.Send(command);
        return Ok(new { Message = result });
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(new { Error = ex.Message });
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { Error = ex.Message });
    }
}


    }
    
}
