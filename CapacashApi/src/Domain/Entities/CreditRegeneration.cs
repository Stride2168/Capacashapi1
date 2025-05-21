// Domain/Entities/CreditRegeneration.cs
using Capacash.Domain.Enums;
namespace Capacash.Domain.Entities;

public class CreditRegeneration
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime RegenerationDate { get; private set; } = DateTime.UtcNow;
    public RegenerationType Type { get; private set; }
    public string InitiatedBy { get; private set; }
    public string? Notes { get; private set; }

    // Navigation property - made nullable to resolve constructor issue
    public virtual User? User { get; set; }

    public CreditRegeneration(
        Guid userId, 
        decimal amount, 
        RegenerationType type,
        string initiatedBy,
        string? notes = null)
    {
        UserId = userId;
        Amount = amount > 0 ? amount : throw new ArgumentException("Amount must be positive");
        Type = type;
        InitiatedBy = initiatedBy ?? throw new ArgumentNullException(nameof(initiatedBy));
        Notes = notes;
    }

    // Private constructor for EF Core
    private CreditRegeneration() { InitiatedBy = string.Empty; }
}