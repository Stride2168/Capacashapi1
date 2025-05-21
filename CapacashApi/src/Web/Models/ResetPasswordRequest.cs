// Web/Models/Wallet/ResetPasswordRequest.cs
namespace Capacash.Web.Models.Wallet;

public class ResetPasswordRequest
{
    public string NewPassword { get; set; } = default!;
}
