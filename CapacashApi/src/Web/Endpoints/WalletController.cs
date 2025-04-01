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

        public WalletController(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
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


    }
}
