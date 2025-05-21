using System.Security.Claims;
using Capacash.Application.Common.Interfaces;
using Capacash.Application.Transaction.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Newtonsoft.Json;
public class GenerateQrCommandHandler : IRequestHandler<GenerateQrCommand, string>
{
    private readonly IQrCodeService _qrCodeService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _env;

    public GenerateQrCommandHandler(
        IQrCodeService qrCodeService,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment env)
    {
        _qrCodeService = qrCodeService;
        _httpContextAccessor = httpContextAccessor;
        _env = env;
    }

    public Task<string> Handle(GenerateQrCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null || httpContext.User == null)
            throw new UnauthorizedAccessException("Missing HTTP context or user.");

        var kioskId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(kioskId))
            throw new UnauthorizedAccessException("Kiosk ID is missing from token.");

        var payload = new
        {
            kioskId,
            request.Amount,
            timestamp = DateTime.UtcNow,
            nonce = Guid.NewGuid().ToString("N")
        };
Console.WriteLine($"Amount being passed: {request.Amount}");
        var encryptedPayload = _qrCodeService.EncryptPayload(payload);

        // ✅ Load the logo from wwwroot/images/logo.png
        var logoPath = Path.Combine(_env.WebRootPath, "images", "capacash.png");

        if (!File.Exists(logoPath))
            throw new FileNotFoundException("QR logo not found: " + logoPath);

        var logoBytes = File.ReadAllBytes(logoPath);

        // ✅ Generate QR code with embedded logo
        var qrCodeBase64 = _qrCodeService.GenerateQrCodeBase64WithLogo(encryptedPayload, logoBytes);

        return Task.FromResult(qrCodeBase64);
    }
}
