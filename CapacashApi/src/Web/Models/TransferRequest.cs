namespace Capacash.Web.Models.Wallet;

public class TransferRequest
{
    public Guid RecipientId { get; set; }
    public decimal Amount { get; set; }
}
