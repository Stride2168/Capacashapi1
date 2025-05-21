namespace Capacash.Web.Models;

public class RegenerateUserCreditRequest
{
    public decimal Amount { get; set; }
    public string InitiatedBy { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
