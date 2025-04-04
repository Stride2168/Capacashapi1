using Capacash.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Capacash.Web.Endpoints
{
    [ApiController]
    [Route("api/wallet")]
    [Authorize] // ✅ Require authentication
    public class WalletController : ControllerBase
    {
        private readonly IWalletRepository _walletRepository;
 private readonly ITransactionRepository _transactionRepository;
        public WalletController(IWalletRepository walletRepository, ITransactionRepository transactionRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
                        _transactionRepository = transactionRepository;
        }

    [HttpGet("me")]
public async Task<IActionResult> GetMyWallet()
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    Console.WriteLine($"🔍 Extracted User ID: {userId}");

    if (string.IsNullOrEmpty(userId))
    {
        Console.WriteLine("❌ User ID is missing or invalid.");
        return Unauthorized(new { Error = "Invalid user session." });
    }

    var wallet = await _walletRepository.GetWalletByUserIdAsync(userId);
    if (wallet == null)
    {
        Console.WriteLine($"❌ Wallet not found for User ID: {userId}");
        return NotFound(new { Error = "Wallet not found." });
    }

    Console.WriteLine($"✅ Wallet found: {wallet.Id}");
    return Ok(wallet);
}
 // ✅ Fetch transaction history for the authenticated employee
    [HttpGet("transactions")]
public async Task<IActionResult> GetMyTransactionHistory([FromQuery] string? filter)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    Console.WriteLine($"🔍 Extracted User ID: {userId}");

    if (string.IsNullOrEmpty(userId))
    {
        Console.WriteLine("❌ User ID is missing or invalid.");
        return Unauthorized(new { Error = "Invalid user session." });
    }

    var transactions = await _transactionRepository.GetTransactionsByUserIdAsync(Guid.Parse(userId));

    if (transactions == null || !transactions.Any())
    {
        Console.WriteLine($"❌ No transactions found for User ID: {userId}");
        return NotFound(new { Error = "No transactions found." });
    }

    Console.WriteLine($"✅ Transactions found: {transactions.Count()}");

    // ✅ Apply Filtering Based on Query Parameter
    DateTime now = DateTime.UtcNow;
    switch (filter?.ToLower())
    {
        case "today":
            transactions = transactions.Where(t => t.TransactionDate.Date == now.Date);
            break;
        case "lastweek":
            transactions = transactions.Where(t => t.TransactionDate >= now.AddDays(-7));
            break;
        case "last30days":
            transactions = transactions.Where(t => t.TransactionDate >= now.AddDays(-30));
            break;
    }

    return Ok(transactions.Select(t => new 
    {
        t.TransactionId,
        t.Amount,
        t.TransactionDate,
        t.UserId
    }));
}

    }
}
