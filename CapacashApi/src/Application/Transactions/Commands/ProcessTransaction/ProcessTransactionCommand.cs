public class ProcessTransactionCommand : IRequest<string>
{
    public Guid UserId { get; }
    public string KioskId { get; }
    public DateTime Timestamp { get; }
    public decimal Amount { get; }
    public string TransactionType { get; }

    public ProcessTransactionCommand(
        Guid userId,       
        string kioskId,      
        DateTime timestamp,  
        decimal amount,      
        string transactionType) 
    {
        UserId = userId;
        KioskId = kioskId;  
        Timestamp = timestamp;
        Amount = amount;
        TransactionType = transactionType;
    }
}