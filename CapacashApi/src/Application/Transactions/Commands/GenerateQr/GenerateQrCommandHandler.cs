using System.Security.Claims;
using Capacash.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

public class GenerateQrCommandHandler : IRequestHandler<GenerateQrCommand, string>
{
    private readonly IQrCodeService _qrCodeService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GenerateQrCommandHandler(IQrCodeService qrCodeService, IHttpContextAccessor httpContextAccessor)
    {
        _qrCodeService = qrCodeService;
        _httpContextAccessor = httpContextAccessor;
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

    var encryptedPayload = _qrCodeService.EncryptPayload(payload);
    var qrCodeBase64 = _qrCodeService.GenerateQrCodeBase64(encryptedPayload);

    return Task.FromResult(qrCodeBase64);
}


}
