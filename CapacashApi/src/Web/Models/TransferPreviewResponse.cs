public class TransferPreviewResponse
{
    public Guid RecipientId { get; set; }
    public string RecipientName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Message { get; set; } = "Please confirm this transfer.";
}
